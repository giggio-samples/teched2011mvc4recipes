md ..\..\..\Package
md ..\..\..\Package\Compile
md ..\..\..\Package\Compile\bin
md ..\..\..\Package\Compile\bin\Debug

copy Lambda3.ConvertAspxToRazor.nuspec ..\..\..\Package\Compile\
copy .\bin\Debug\ConvertAspxToRazorRecipe.dll ..\..\..\Package\Compile\bin\Debug\ConvertAspxToRazorRecipe.original.dll
copy ..\packages\AspNetMvc4.RecipeSdk.0.1\lib\net40\Microsoft.VisualStudio.Web.Mvc.Extensibility.1.0.dll ..\..\..\Package\Compile\bin\Debug\
copy ..\..\..\libs\Telerik.RazorConverter.dll ..\..\..\Package\Compile\bin\Debug\
copy Lambda3.ConvertAspxToRazor.nuspec ..\..\..\Package\Compile\

..\..\..\tools\ilmerge.exe /v4 /out:..\..\..\Package\Compile\bin\Debug\ConvertAspxToRazorRecipe.dll ..\..\..\Package\Compile\bin\Debug\ConvertAspxToRazorRecipe.original.dll ..\..\..\Package\Compile\bin\Debug\Telerik.RazorConverter.dll
..\..\..\tools\nuget.exe pack ..\..\..\Package\Compile\Lambda3.ConvertAspxToRazor.nuspec -OutputDirectory ..\..\..\Package -NoPackageAnalysis