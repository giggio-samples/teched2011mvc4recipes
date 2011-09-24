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
            
            var candidateFilesToConvert = new List<string>(from f in folder.Files select Path.Combine(folder.FullName, f.Name));
            //var candidateFilesToConvert = Directory.EnumerateFiles(@"c:\temp\");
            var filesToConvert = AskUserToSelectFiles(candidateFilesToConvert);
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
            if (!result.Value)
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

