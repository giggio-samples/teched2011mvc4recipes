using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Media.Imaging;
using ConvertAspxToRazorRecipe.Properties;
using Microsoft.VisualStudio.Web.Mvc.Extensibility;
using Microsoft.VisualStudio.Web.Mvc.Extensibility.Recipes;

namespace ConvertAspxToRazorRecipe {
    [Export(typeof(IRecipe))]
    public class ConvertAspxToRazor : IFolderRecipe {
        public bool Execute(ProjectFolder folder)
        {
            var folderLoweredName = folder.FullName.ToLower();
            var razorFiles = (from f in folder.Files.Select(y => y.Name.ToLower())
                              where f.EndsWith("cshtml")
                              select Path.Combine(folderLoweredName, f)).ToList();
            var candidateFilesToConvert = (from f in folder.Files
                                           where f.Name.ToLower().EndsWith("aspx")
                                           select Path.Combine(folder.FullName, f.Name)).ToList();
            //var razorFiles = Directory.EnumerateFiles(@"c:\temp\").Where(f => f.ToLower().EndsWith("cshtml")).Select(f => f.ToLower());
            //var candidateFilesToConvert = Directory.EnumerateFiles(@"c:\temp\").Where(f => f.ToLower().EndsWith("aspx"));
            candidateFilesToConvert = (from f in candidateFilesToConvert
                                       where !razorFiles.Contains(f.ToLower().Substring(0, f.Length - 4) + "cshtml")
                                       select f).ToList();

            if (candidateFilesToConvert.Count == 0)
            {
                MessageBox.Show(
                    "No files to convert. Check if you have already converted files and remove the existing cshtml files for the aspx's you want to convert.",
                    "No files found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }

            var filesToConvert = AskUserToSelectFiles(candidateFilesToConvert);
            //return true;
            var convertedFiles = AspxToRazor.ConvertFiles(filesToConvert);
            foreach (var convertedFile in convertedFiles)
                folder.DteProjectItems.AddFromFile(convertedFile);
            return true;
        }

        private static IEnumerable<string> AskUserToSelectFiles(IEnumerable<string> fileNamesToConvert)
        {
            var rm = new ResourceManager("ConvertAspxToRazorRecipe.g", typeof (Resources).Assembly);
            var filesToConvert = (from f in fileNamesToConvert select new FileToConvert {FullFileName = f, FileName = Path.GetFileName(f)}).ToList();
            var picker = new FilesPicker{DataContext = filesToConvert};
            var window = new Window
                             {
                                 Content = picker,
                                 SizeToContent = SizeToContent.Height,
                                 Icon = BitmapFrame.Create(rm.GetStream("lambda3.ico", Resources.Culture)),
                                 Width = 400,
                                 Title = "Convert ASPX to Razor",
                                 MinHeight = picker.MinHeight + 50,
                                 MinWidth = picker.MinWidth + 50 
                             };
            var result = window.ShowDialog();
            if (result != null && !result.Value)
                return new List<string>();
            var selectedFiles = (from f in filesToConvert where f.Checked select f.FullFileName).ToList();
            return selectedFiles;
        }

        private class FileToConvert
        {
            public string FullFileName { get; set; }
            public string FileName { get; set; }
            public bool Checked { get; set; }
        }


        public bool IsValidTarget(ProjectFolder folder) {
            return folder.IsMvcViewsFolderOrDescendent();
        }

        public string Description {
            get { return "Converts Aspx files to Razor."; }
        }

        public Icon Icon {
            get { return Resources.Lambda3; }
        }

        public string Name {
            get { return "Convert Aspx to Razor"; }
        }
    }
}

