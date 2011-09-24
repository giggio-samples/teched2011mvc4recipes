using System.Collections.Generic;
using System.IO;
using System.Text;
using Telerik.RazorConverter.Razor.Converters;
using Telerik.RazorConverter.Razor.DOM;
using Telerik.RazorConverter.Razor.Rendering;
using Telerik.RazorConverter.WebForms.DOM;
using Telerik.RazorConverter.WebForms.Filters;
using Telerik.RazorConverter.WebForms.Parsing;

namespace ConvertAspxToRazorRecipe
{
    public class AspxToRazor
    {
        public static IEnumerable<string> ConvertFiles(IEnumerable<string> filesToConvert)
        {
            var parser = new WebFormsParser(new WebFormsNodeFactory(),
                                            new WebFormsNodeFilterProvider(new WebFormsCodeGroupFactory()));
            var renderer = new RazorViewRenderer(new RazorNodeRendererProvider());
            var converter =
                new WebFormsToRazorConverter(new RazorNodeConverterProvider(new RazorDirectiveNodeFactory(),
                                                                            new RazorSectionNodeFactory(),
                                                                            new RazorCodeNodeFactory(),
                                                                            new RazorTextNodeFactory(),
                                                                            new RazorCommentNodeFactory(),
                                                                            new RazorExpressionNodeFactory(),
                                                                            new ContentTagConverterConfiguration()));
            var convertedFiles = new List<string>();

            foreach (var file in filesToConvert)
            {
                var webFormsPageSource = File.ReadAllText(file, Encoding.UTF8);
                var webFormsDocument = parser.Parse(webFormsPageSource);
                var razorDom = converter.Convert(webFormsDocument);
                var razorPage = renderer.Render(razorDom);
                var outputFileName = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".cshtml");
                File.WriteAllText(outputFileName, razorPage, Encoding.UTF8);
                convertedFiles.Add(outputFileName);
            }
            return convertedFiles;
        }
        
    }
}