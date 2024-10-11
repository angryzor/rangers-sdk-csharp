using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using System.IO;
using RangersSDKBindingsGenerator.Passes;
using System.Linq;
using System.Xml.Linq;
using System;
using RangersSDKBindingsGenerator.TypeMaps;
using CppSharp.Generators.C;
using CppSharp.Types;

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
            ctx.FindClass("ResAnimation").First().IsStatic = false;

            //foreach (var @class in ctx.FindClass("InplaceMoveArray"))
            //{
            //    @class.GenerationKind = GenerationKind.Generate;
            //    foreach (var spec in @class.Specializations)
            //        spec.GenerationKind = GenerationKind.Generate;
            //}
            //foreach (var @class in ctx.FindClass("intrusive_ptr"))
            //    foreach (var spec in @class.Specializations)
            //        spec.GenerationKind = GenerationKind.Generate;
            //foreach (var @class in ctx.FindClass("Reference"))
            //    foreach (var spec in @class.Specializations)
            //        spec.GenerationKind = GenerationKind.Generate;
            //foreach (var @class in ctx.FindClass("Handle"))
            //    foreach (var spec in @class.Specializations)
            //        spec.GenerationKind = GenerationKind.Generate;
            foreach (var @class in ctx.FindClass("HandleManager"))
                foreach (var spec in @class.Specializations)
                    spec.GenerationKind = GenerationKind.Generate;
            //foreach (var @class in ctx.FindClass("ResReflectionT"))
            //    foreach (var spec in @class.Specializations)
            //        spec.GenerationKind = GenerationKind.Generate;
        }

        public void Preprocess(Driver driver, ASTContext ctx)
        {
            ctx.IgnoreClassWithName("SingletonInitNode");
            ctx.IgnoreClassWithName("UIMusicSelect");
            ctx.IgnoreClassWithName("Sonic");
            ctx.IgnoreClassWithName("Amy");
            ctx.IgnoreClassWithName("Knuckles");
            ctx.IgnoreClassWithName("Tails");
            ctx.IgnoreClassWithName("MousePickingViewer");
            ctx.IgnoreClassWithName("PhysicsMousePickingViewer");
            ctx.IgnoreClassWithName("PhysicsViewerContext");
            ctx.IgnoreClassWithName("GameViewerBase");
            ctx.IgnoreClassWithName("PhysicsViewerBase");
            ctx.IgnoreClassWithName("PhysicsPickedObjectViewer");
            ctx.IgnoreClassWithName("ObjectViewerContext");
            ctx.IgnoreClassWithName("GameViewerContext");
            ctx.IgnoreClassWithName("ServiceViewerContextBase");
            ctx.IgnoreClassWithName("ViewerContext");
            ctx.IgnoreClassWithName("GameJobQueue");
            ctx.IgnoreClassWithName("JobInitializer");
            //ctx.IgnoreClassWithName("ActionMapping");
            //ctx.IgnoreClassWithName("AxisMapping");
            //ctx.IgnoreClassWithName("InputComponent");
            ctx.IgnoreClassWithName("EventStack");
            ctx.IgnoreClassWithName("UIListViewElement");
            ctx.IgnoreClassWithName("GameModeResourceManager");
            ctx.IgnoreClassWithName("PlayerStateBase");
            ctx.IgnoreClassWithName("LightComponent");
            ctx.IgnoreClassWithName("CaptureComponent");
            ctx.IgnoreClassWithName("ScreenShotComponent");
            ctx.IgnoreClassWithName("FreeCamera");
            ctx.IgnoreClassWithName("ArcBallCameraController");
            ctx.IgnoreClassWithName("DefaultFreeCameraController");
            ctx.IgnoreClassWithName("DebugCameraManager");
            ctx.IgnoreClassWithName("GOCPlayerHsm");
            ctx.IgnoreClassWithName("Handle");
            ctx.IgnoreClassWithName("DrawSystem");
            ctx.IgnoreClassWithName("AsmData");
            ctx.IgnoreClassWithName("CastListener");
            ctx.IgnoreClassWithName("Reference");
            ctx.IgnoreClassWithName("intrusive_ptr");
            ctx.IgnoreClassWithName("MessageAsyncHandlerInplace");
            ctx.IgnoreClassWithName("ResReflectionT");
            ctx.IgnoreClassWithName("RenderingEngineNeedle");
            ctx.IgnoreClassWithName("RenderingEngineRangers");
            ctx.IgnoreClassWithName("RenderManager");
            ctx.IgnoreClassWithName("GOCVisualUserModel");
            ctx.IgnoreClassWithName("GOCVisualModelImpl");
            ctx.IgnoreClassWithName("ObjSumoPoleSlingshotLineConfig");

            ctx.SetClassAsValueType("Vector2");
            ctx.SetClassAsValueType("Vector3");
            ctx.SetClassAsValueType("Vector4");
            ctx.SetClassAsValueType("Quaternion");
            ctx.SetClassAsValueType("Matrix34");
            ctx.SetClassAsValueType("Matrix44");
            ctx.SetClassAsValueType("Position");
            ctx.SetClassAsValueType("HandleBase");

            ctx.SetClassAsValueType("RflClassEnum");
            ctx.SetClassAsValueType("RflClassEnumMember");
            ctx.SetClassAsValueType("RflClassMember");
            ctx.SetClassAsValueType("RflCustomAttribute");
            ctx.SetClassAsValueType("RflEntity");
            ctx.SetClassAsValueType("HandleBase");
            ctx.SetClassAsValueType("VariableString");
            foreach (string className in new string[] { "Handle", "RflArray" })
            {
                foreach (ClassTemplate item in ctx.FindDecl<ClassTemplate>(className))
                {
                    item.TemplatedClass.Type = ClassType.ValueType;

                    foreach (var specialization in item.Specializations)
                    {
                        specialization.Type = ClassType.ValueType;
                    }
                }
            }
            //foreach (ClassTemplate item in ctx.FindDecl<ClassTemplate>("intrusive_ptr"))
            //{
            //    item.TemplatedClass.Type = ClassType.ValueType;

            //    foreach (var specialization in item.Specializations)
            //    {
            //        specialization.Type = ClassType.ValueType;
            //    }
            //}
            //foreach (ClassTemplate item in ctx.FindDecl<ClassTemplate>("Reference"))
            //{
            //    item.TemplatedClass.Type = ClassType.ValueType;

            //    foreach (var specialization in item.Specializations)
            //    {
            //        specialization.Type = ClassType.ValueType;
            //    }
            //}


            foreach (var (@namespace, @class) in new[] { ("hh::fnd", "Handle") })
            {
                driver.Context.TypeMaps.FindTypeMap($"{@namespace}::{@class}", GeneratorKind.CSharp, out var handleTypeMap);
                foreach (ClassTemplate item in ctx.FindDecl<ClassTemplate>(@class))
                {
                    foreach (var specialization in item.Specializations)
                    {
                        var typePrinter = new CppTypePrinter(driver.Generator.Context)
                        {
                            ResolveTypeMaps = false,
                            PrintTypeQualifiers = false,
                            PrintTypeModifiers = false,
                            PrintLogicalNames = true
                        };

                        typePrinter.PushContext(TypePrinterContextKind.Native);

                        foreach (var resolveTypeDefs in new[] { false, true })
                        {
                            foreach (var typePrintScopeKind in
                                new[] { TypePrintScopeKind.Local, TypePrintScopeKind.Qualified })
                            {
                                typePrinter.ResolveTypedefs = resolveTypeDefs;
                                typePrinter.PushScope(typePrintScopeKind);
                                var typeName = specialization.Visit(typePrinter);
                                typePrinter.PopScope();
                                driver.Context.TypeMaps.TypeMaps[typeName] = handleTypeMap;
                            }
                        }
                    }
                }
            }

            ctx.IgnoreHeadersWithName("hhRflArray.h");
            ctx.IgnoreHeadersWithName("hhHandle.h");
            ctx.IgnoreHeadersWithName("Delegate.h");

            ctx.IgnoreFunctionWithName("EntryUniqueElementControl");
            ctx.IgnoreFunctionWithName("LeaveUniqueElementControl");

            ctx.FindClass("Singleton").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.TrimSpecializationsPass));
            ctx.FindClass("Singleton").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.SpecializationMethodsWithDependentPointersPass));
            //ctx.FindClass("PhysicsWorldBullet").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("TagReplaceable").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("TagReplacer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("RenderManagerBase").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("RenderManager").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("GOCAnimation").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("GOCAnimationSingle").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("GOCAnimator").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ////ctx.FindClass("Application").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("GameApplication").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("MyApplication").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("FxCamera").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ////ctx.FindClass("LocalFxCamera").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("ResourceContainer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("StaticResourceContainer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("DynamicResourceContainer").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("ResAnimation").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("ResAnimationPxd").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("AnimationControl").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            ////ctx.FindClass("AnimationControlInternal").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("AnimationControlPxd").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("CyanAllocator").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("TransitionEffect").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));
            //ctx.FindClass("BlendingTransitionEffect").First().ExcludeFromPasses.Add(typeof(CppSharp.Passes.GetterSetterToPropertyPass));

            ctx.FindClass("MemoryRouter").First().FindMethod("GetTempAllocator").ExplicitlyIgnore();
            ctx.FindClass("BinaryData").First().FindMethod("ApplyMemoryImageToProject").ExplicitlyIgnore();

            foreach (TranslationUnit unit in ctx.TranslationUnits)
            {
                //unit.FindNamespace("hh")?.FindNamespace("needle")?.FindNamespace("ImplDX11")?.ExplicitlyIgnore();
                //unit.FindNamespace("hh")?.FindNamespace("gfx")?.ExplicitlyIgnore();
                //unit.FindNamespace("hh")?.FindNamespace("gfnd")?.ExplicitlyIgnore();
                unit.FindNamespace("hh")?.FindNamespace("needle")?.ExplicitlyIgnore();
                unit.FindNamespace("hh")?.FindNamespace("rsdx")?.ExplicitlyIgnore();
                unit.FindNamespace("hh")?.FindNamespace("ui")?.ExplicitlyIgnore();
                unit.FindNamespace("hh")?.FindNamespace("dbg")?.ExplicitlyIgnore();
                unit.FindNamespace("hh")?.FindNamespace("game")?.FindNamespace("dmenu")?.ExplicitlyIgnore();
                unit.FindNamespace("app")?.ExplicitlyIgnore();
                //unit.FindNamespace("heur")?.ExplicitlyIgnore();
                unit.FindNamespace("csl")?.FindNamespace("ut")?.ExplicitlyIgnore();
                unit.FindNamespace("csl")?.FindNamespace("math")?.ExplicitlyIgnore();
                //unit.FindNamespace("Cyan")?.ExplicitlyIgnore();
            }

            ctx.RenameNamespace("csl::fnd", "Foundation");
            ctx.RenameNamespace("csl::ut", "Utility");
            ctx.RenameNamespace("csl::geom", "Geometry");

            ctx.RenameNamespace("hh::ut", "Utility");
            ctx.RenameNamespace("hh::fnd", "Foundation");
            ctx.RenameNamespace("hh::anim", "Animation");
            ctx.RenameNamespace("hh::cri", "CRI");
            ctx.RenameNamespace("hh::dbg", "Debug");
            ctx.RenameNamespace("hh::dv", "DvScene");
            ctx.RenameNamespace("hh::eff", "Effects");
            ctx.RenameNamespace("hh::fw", "Framework");
            ctx.RenameNamespace("hh::gfnd", "GraphicsFoundation");
            ctx.RenameNamespace("hh::gfx", "Graphics");
            ctx.RenameNamespace("hh::hid", "HID");
            ctx.RenameNamespace("hh::snd", "Sound");
            ctx.RenameNamespace("hh::ui", "UI");

            ctx.RenameNamespace("app_cmn::fsm", "FSM");
            ctx.RenameNamespace("app_cmn::rfl", "Reflection");

            ctx.RenameNamespace("app::ut", "Utility");
            ctx.RenameNamespace("app::fnd", "Foundation");
            ctx.RenameNamespace("app::gfx", "Graphics");
            ctx.RenameNamespace("app::hid", "HID");
            ctx.RenameNamespace("app::snd", "Sound");
            ctx.RenameNamespace("app::evt", "Events");
            ctx.RenameNamespace("app::trr", "Terrain");
            ctx.RenameNamespace("app::ui", "UI");

            ctx.RenameNamespace("csl", "CSLib");
            ctx.RenameNamespace("hh", "Hedgehog");
            ctx.RenameNamespace("app", "Application");
            ctx.RenameNamespace("app_cmn", "ApplicationCommon");
        }

        public void Setup(Driver driver)
        {
            driver.ParserOptions.LanguageVersion = CppSharp.Parser.LanguageVersion.CPP17;

            driver.Options.GeneratorKind = GeneratorKind.CSharp;
            driver.Options.CompileCode = false;
            driver.Options.OutputDir = outputDir;
            driver.Options.CustomClassCodeCallback += GenerateInteropIsomorphism;
            driver.Options.GenerateClassTemplates = true;

            var module = driver.Options.AddModule("RangersSDK");
            module.SharedLibraryName = "rangers-sdk";
            module.IncludeDirs.Add(@"C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\MSVC\14.37.32822\include");
            foreach (var includeDir in include.Split(";"))
            {
                if (includeDir != "")
                    module.IncludeDirs.Add(includeDir);
            }
            module.Headers.Add(entry);
            module.LibraryDirs.Add(Path.GetDirectoryName(lib));
            module.Libraries.Add(Path.GetFileName(lib));
        }

        public static void GenerateInteropIsomorphism(CodeGenerator gen, Class @class)
        {
            if (!@class.IsAbstractImpl && @class.IsRefType && !@class.IsStatic && (gen.Options.GetClassGenerationOptions?.Invoke(@class)?.GenerateNativeToManaged ?? true))
            {
                var typeName = @class.Name;

                if (@class.IsDependent && @class.TemplateParameters.Any())
                    typeName += $"<{string.Join(", ", @class.TemplateParameters.Select(p => p.Name))}>";

                gen.PushBlock(BlockKind.Class);
                gen.WriteLines($@"public new class __InteropIsomorphism : global::RangersSDK.Interop.InteropIsomorphism<{typeName}, nint>
{{
    public nint GetUnmanaged({typeName} obj) {{ return obj.__Instance; }}
    public {typeName} GetManaged(nint obj) {{ return {typeName}.__GetOrCreateInstance(obj, false, true); }}
    public void ReleaseUnmanaged(nint obj) {{ }}
}}");
                gen.PopBlock(NewLineKind.BeforeNextBlock);
            }
        }

        public void SetupPasses(Driver driver)
        {
            driver.AddTranslationUnitPass(new CheckBitsetsPass());
            driver.AddTranslationUnitPass(new ReadonlyContainerFieldsPass());
        }
    }
}