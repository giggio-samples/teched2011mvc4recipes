using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Text;
using ConvertAspxToRazorRecipe.Properties;
using Microsoft.VisualStudio.Web.Mvc.Extensibility;
using Microsoft.VisualStudio.Web.Mvc.Extensibility.Recipes;
using Telerik.RazorConverter.Razor.Converters;
using Telerik.RazorConverter.Razor.DOM;
using Telerik.RazorConverter.Razor.Rendering;
using Telerik.RazorConverter.WebForms.DOM;
using Telerik.RazorConverter.WebForms.Filters;
using Telerik.RazorConverter.WebForms.Parsing;

namespace ConvertAspxToRazorRecipe {
    [Export(typeof(IRecipe))]
    public class ConvertAspxToRazor : IFolderRecipe {
        public bool Execute(ProjectFolder folder)
        {

            var parser = new WebFormsParser(new WebFormsNodeFactory(), new WebFormsNodeFilterProvider(new WebFormsCodeGroupFactory()));
            var renderer = new RazorViewRenderer(new RazorNodeRendererProvider());
            var converter = new WebFormsToRazorConverter(new RazorNodeConverterProvider(new RazorDirectiveNodeFactory(), new RazorSectionNodeFactory(), new RazorCodeNodeFactory(), new RazorTextNodeFactory(), new RazorCommentNodeFactory(), new RazorExpressionNodeFactory(), new ContentTagConverterConfiguration()));

            foreach (var file in folder.Files)
            {
                var folderPath = folder.FullName;
                var filePath = Path.Combine(folderPath, file.Name);

                var webFormsPageSource = File.ReadAllText(filePath, Encoding.UTF8);
                var webFormsDocument = parser.Parse(webFormsPageSource);
                var razorDom = converter.Convert(webFormsDocument);
                var razorPage = renderer.Render(razorDom);
                var outputFileName = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(file.Name) + ".cshtml");
                File.WriteAllText(outputFileName, razorPage, Encoding.UTF8);
                folder.DteProjectItems.AddFromFile(outputFileName);
            }
            return true;
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

