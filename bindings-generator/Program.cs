using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using System.IO;
using RangersSDKBindingsGenerator.Passes;
using System.Linq;

namespace RangersSDKBindingsGenerator
{
    internal class Program : ILibrary
    {
        string outputDir;
        string include;
        string entry;
        string lib;

        static void Main(string[] args)
        {
            var program = new Program();
            program.outputDir = args[0];
            program.include = args[1];
            program.entry = args[2];
            program.lib = args[3];

            System.Console.WriteLine(string.Format("Output dir: {0}", program.outputDir));
            System.Console.WriteLine(string.Format("Include dirs: {0}", program.include));
            System.Console.WriteLine(string.Format("Entrypoint: {0}", program.entry));
            System.Console.WriteLine(string.Format("Lib: {0}", program.lib));

            ConsoleDriver.Run(program);
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
            ctx.IgnoreClassWithName("SingletonInitNode");
            ctx.IgnoreClassWithName("UIMusicSelect");
            ctx.IgnoreClassWithName("Sonic");
            ctx.IgnoreClassWithName("Amy");
            ctx.IgnoreClassWithName("Knuckles");
            ctx.IgnoreClassWithName("Tails");
            ctx.IgnoreClassWithName("ResAnimation");
            ctx.IgnoreClassWithName("MousePickingViewer");
            ctx.IgnoreClassWithName("PhysicsMousePickingViewer");
            ctx.IgnoreClassWithName("GameJobQueue");
            ctx.IgnoreClassWithName("JobInitializer");
            ctx.IgnoreFunctionWithName("EntryUniqueElementControl");
            ctx.IgnoreFunctionWithName("LeaveUniqueElementControl");
            ctx.FindClass("PhysicsWorld").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("PhysicsWorldBullet").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("TagReplaceable").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("TagReplacer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("RenderManagerBase").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("RenderManager").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("GOCAnimation").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("GOCAnimationSingle").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("GOCAnimator").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("Application").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("GameApplication").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("MyApplication").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("FxCamera").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("LocalFxCamera").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("ResourceContainer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("StaticResourceContainer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("DynamicResourceContainer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ctx.FindClass("_RTL_CRITICAL_SECTION").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.IgnoreSystemDeclarationsPass));
            ctx.FindTypedef("RTL_CRITICAL_SECTION").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.IgnoreSystemDeclarationsPass));
            ctx.FindClass("_RTL_CRITICAL_SECTION").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.StripUnusedSystemTypesPass));
            ctx.FindTypedef("RTL_CRITICAL_SECTION").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.StripUnusedSystemTypesPass));
            ctx.FindClass("_RTL_CRITICAL_SECTION").First().GenerationKind = GenerationKind.Internal;
            ctx.FindTypedef("RTL_CRITICAL_SECTION").First().GenerationKind = GenerationKind.Internal;

            foreach (TranslationUnit unit in ctx.TranslationUnits)
            {
                unit.FindNamespace("hh")?.FindNamespace("needle")?.FindNamespace("ImplDX11")?.ExplicitlyIgnore();
                unit.FindNamespace("hh")?.FindNamespace("rsdx")?.ExplicitlyIgnore();
            }
        }

        public void Setup(Driver driver)
        {
            driver.ParserOptions.LanguageVersion = CppSharp.Parser.LanguageVersion.CPP17;

            driver.Options.GeneratorKind = GeneratorKind.CSharp;
            driver.Options.CompileCode = false;
            driver.Options.OutputDir = outputDir;

            var module = driver.Options.AddModule("RangersSDK");
            module.SharedLibraryName = "rangers-sdk";
            module.IncludeDirs.Add(@"C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\MSVC\14.37.32822\include");
            foreach (var includeDir in include.Split(";")) {
                if (includeDir != "")
                    module.IncludeDirs.Add(includeDir);
            }
            module.Headers.Add(entry);
            module.LibraryDirs.Add(Path.GetDirectoryName(lib));
            module.Libraries.Add(Path.GetFileName(lib));
        }

        public void SetupPasses(Driver driver)
        {
            driver.AddTranslationUnitPass(new CheckBitsetsPass());
        }
    }
}