using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DiffEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyNUnit;

namespace BuilderBuilder.Tests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // To avoid issue getting semantic model
        IEnumerable<PortableExecutableReference> references = new[]
{
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);

        var generator = new BuilderGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        SetupDiffTool();

        return Verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }

    private static void SetupDiffTool()
    {
        static string TargetLeftArguments(string temp, string target)
        {
            var tempTitle = Path.GetFileName(temp);
            var targetTitle = Path.GetFileName(target);
            return $"/u /wl /e \"{target}\" \"{temp}\" /dl \"{targetTitle}\" /dr \"{tempTitle}\"";
        }

        static string TargetRightArguments(string temp, string target)
        {
            var tempTitle = Path.GetFileName(temp);
            var targetTitle = Path.GetFileName(target);
            return $"/u /wl /e \"{temp}\" \"{target}\" /dl \"{tempTitle}\" /dr \"{targetTitle}\"";
        }
        var _ = DiffTools.AddTool(
            name: "MyTool",
            autoRefresh: true,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            binaryExtensions: new[]
            {
                "bmp",
                "cut",
                "dds",
                "exr",
                "g3",
                "gif",
                "hdr",
                "ico",
                "iff",
                "lbm",
                "j2k",
                "j2c",
                "jng",
                "jp2",
                "jpg",
                "jif",
                "jpeg",
                "jpe",
                "jxr",
                "wdp",
                "hdp",
                "koa",
                "mng",
                "pcd",
                "pcx",
                "pfm",
                "pct",
                "pict",
                "pic",
                "png",
                "pbm",
                "pgm",
                "ppm",
                "psd",
                "ras",
                "sgi",
                "rgb",
                "rgba",
                "bw",
                "tga",
                "targa",
                "tif",
                "tiff",
                "wap",
                "wbmp",
                "wbm",
                "webp",
                "xbm",
                "xpm"
            },
            windows: new(
                TargetLeftArguments,
                TargetRightArguments,
                @"D:\Apps\WinMerge\WinMergeU.exe")
        );
    }        // See
}
