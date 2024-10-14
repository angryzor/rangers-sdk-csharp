using CppSharp;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Generators.TS;
using CppSharp.Types;
using System.Linq;

namespace RangersSDKBindingsGenerator.TypeMaps
{
    public abstract class ContainerTypeMap : ShimmedReferenceTypeMap
    {
        protected static string[] UnmanagedReplacedClasses = new string[]{ "Vector2", "Vector3", "Vector4", "Quaternion", "Matrix34", "Matrix44", "Position", "HandleBase", "Handle" };

        protected static bool IsValueClass(Type type)
        {
            return type.TryGetClass(out Class @class) && @class.Type == ClassType.ValueType;
        }
        protected static bool IsUnmanagedReplacedClass(Type type)
        {
            return type.TryGetClass(out Class @class) && UnmanagedReplacedClasses.Contains(@class.Name);
        }

        protected static bool IsUnmanaged(Type type)
        {
            return type.IsPrimitiveType() || type.IsPointerToPrimitiveType() || type.IsEnumType() || IsValueClass(type) || IsUnmanagedReplacedClass(type);
        }

        protected string GetTypeName(QualifiedType type)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            return type.Visit(typePrinter).Type;
        }

        protected string GetUnmanaged(QualifiedType type)
        {
            return IsUnmanaged(type.Type) ? $"{GetTypeName(type)}" : "nint";
        }

        protected string GetIso(QualifiedType type)
        {
            return IsUnmanaged(type.Type) ? $"global::RangersSDK.Interop.IdentityInteropIsomorphism<{GetTypeName(type)}>" : $"{GetTypeName(type)}.__InteropIsomorphism";
        }
    }

    [TypeMap("csl::ut::MoveArray", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class MoveArray : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var pointee = Type.GetFinalPointee() ?? Type;
                TemplateSpecializationType specType = pointee as TemplateSpecializationType;
                TagType tagType = pointee as TagType;

                if (specType != null)
                {
                    var innerType = specType.Arguments[0].Type;
                    return $"global::RangersSDK.CSLib.Utility.MoveArray<{GetTypeName(innerType)}, {GetUnmanaged(innerType)}, {GetIso(innerType)}>";
                }
                else if (tagType != null)
                {
                    var innerType = (tagType.Declaration as ClassTemplateSpecialization).Arguments[0].Type;
                    return $"global::RangersSDK.CSLib.Utility.MoveArray<{GetTypeName(innerType)}, {GetUnmanaged(innerType)}, {GetIso(innerType)}>";
                }

                return "itsallfuckedup";
            }
        }
    }

    [TypeMap("csl::ut::InplaceMoveArray", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class InplaceMoveArray : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var innerType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var spec = (Type as TemplateSpecializationType).GetClassTemplateSpecialization();

                return $"global::RangersSDK.CSLib.Utility.InplaceMoveArray<{GetTypeName(innerType)}, {GetUnmanaged(innerType)}, {GetIso(innerType)}, global::RangersSDK.Utility.InplaceMoveArray.{Helpers.InternalStruct}{Helpers.GetSuffixForInternal(spec)}>";
            }
        }
    }

    [TypeMap("csl::ut::InplaceBitArray", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class InplaceBitArray : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var spec = (Type as TemplateSpecializationType).GetClassTemplateSpecialization();

                return $"global::RangersSDK.CSLib.Utility.InplaceBitArray<global::RangersSDK.Utility.InplaceBitArray.{Helpers.InternalStruct}{Helpers.GetSuffixForInternal(spec)}>";
            }
        }
    }

    [TypeMap("csl::ut::MoveArray32", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class MoveArray32 : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var innerType = (Type as TemplateSpecializationType).Arguments[0].Type;

                return $"global::RangersSDK.CSLib.Utility.MoveArray32<{GetTypeName(innerType)}, {GetUnmanaged(innerType)}, {GetIso(innerType)}>";
            }
        }
    }

    [TypeMap("csl::ut::LinkListNode", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class LinkListNode : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField
                ? new CustomType($"global::RangersSDK.CSLib.Utility.LinkListNode")
                : new CustomType($"whatthefuck");
        }
    }

    [TypeMap("csl::ut::LinkList", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class LinkList : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var innerType = (Type as TemplateSpecializationType).Arguments[0].Type;

                return $"global::RangersSDK.CSLib.Utility.LinkList<{GetTypeName(innerType)}, {GetIso(innerType)}>";
            }
        }
    }

    [TypeMap("csl::ut::PointerMap", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class PointerMap : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var keyType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var valueType = (Type as TemplateSpecializationType).Arguments[1].Type;
                var typeName = IsUnmanaged(keyType.Type)
                    ? $"NakedPointerMap<{GetTypeName(valueType)}, {GetUnmanaged(valueType)}, {GetIso(valueType)}>"
                    : $"PointerMap<{GetTypeName(keyType)}, {GetTypeName(valueType)}, {GetUnmanaged(valueType)}, {GetIso(keyType)}, {GetIso(valueType)}>";

                return $"global::RangersSDK.CSLib.Utility.{typeName}";
            }
        }
    }

    [TypeMap("csl::ut::StringMap", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class StringMap : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var valueType = (Type as TemplateSpecializationType).Arguments[0].Type;

                return $"global::RangersSDK.CSLib.Utility.StringMap<{GetTypeName(valueType)}, {GetUnmanaged(valueType)}, {GetIso(valueType)}>";
            }
        }
    }

    [TypeMap("hh::fnd::RflArray", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class RflArrayMap : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField ? new CustomType($"global::RangersSDK.Hedgehog.Foundation.RflArrayInternal<{TypeName}>") : new CustomType($"{TypeName}[]");
        }

        public override void MarshalToManaged(MarshalContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            ctx.Before.WriteLine($"var __returnArr = new {TypeName}[{ctx.ReturnVarName}.size];");
            ctx.Before.WriteLine($"for (int i = 0; i < {ctx.ReturnVarName}.size; i++)");
            ctx.Before.WriteOpenBraceAndIndent();
            ctx.Before.WriteLine($"__returnArr[i] = {TypeName}.__CreateInstance({ctx.ReturnVarName}.buffer[i]);");
            ctx.Before.UnindentAndWriteCloseBrace();
            ctx.Return.Write($"__returnArr");
        }

        public override void MarshalToNative(MarshalContext ctx)
        {
            if (ctx.MarshalKind == MarshalKind.NativeField)
            {
                string assignVar;

                if (ctx.ReturnVarName.LastIndexOf('.') > ctx.ReturnVarName.LastIndexOf("->"))
                {
                    assignVar = Generator.GeneratedIdentifier(ctx.ArgName);
                    ctx.Before.WriteLine($"fixed (global::RangersSDK.Hedgehog.Foundation.RflArrayInternal<{TypeName}.__Internal>* {assignVar} = &{ctx.ReturnVarName})");
                    ctx.Before.WriteOpenBraceAndIndent();
                    (ctx as CSharpMarshalContext).HasCodeBlock = true;
                }
                else
                {
                    assignVar = $"&{ctx.ReturnVarName}";
                }

                // TODO: Clean up native mem.
                ctx.Before.WriteLine($"var __outputBuffer = ({TypeName}.__Internal*)Marshal.AllocHGlobal(sizeof({TypeName}.__Internal) * {ctx.Parameter.Name}.Length);");
                ctx.Before.WriteLine($"for (int i = 0; i < {ctx.Parameter.Name}.Length; i++)");
                ctx.Before.WriteOpenBraceAndIndent();
                ctx.Before.WriteLine($"__outputBuffer[i] = {ctx.Parameter.Name}[i].__Instance;");
                ctx.Before.UnindentAndWriteCloseBrace();
                ctx.Return.WriteLine($"({assignVar})->buffer = __outputBuffer;");
                ctx.Return.Write($"({assignVar})->size = (uint){ctx.Parameter.Name}.Length");
                ctx.ReturnVarName = string.Empty;
            }
            else
            {
                throw new System.ArgumentException();
            }
        }

        private string TypeName => $"{(Type as TemplateSpecializationType).Arguments[0].Type}";
    }

    [TypeMap("csl::fnd::Delegate", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class CSLDelegate : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CustomType("global::RangersSDK.CSLib.Foundation.CSLDelegate");
        }
    }

    [TypeMap("csl::ut::Bitset", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Bitset : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            var templateType = Type.Desugar() as TemplateSpecializationType;
            return templateType.Arguments[0].Type.Type;
        }

        public override bool IsValueType => true;
    }

    [TypeMap("csl::ut::Pair", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class PairMap : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var firstType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var secondType = (Type as TemplateSpecializationType).Arguments[1].Type;

                return $"global::RangersSDK.CSLib.Utility.Pair<{GetTypeName(firstType)}, {GetTypeName(secondType)}, {GetUnmanaged(firstType)}, {GetUnmanaged(secondType)}, {GetIso(firstType)}, {GetIso(secondType)}>";
            }
        }
    }

    [TypeMap("csl::ut::VariableString", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class VariableStringMap : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField ? new CustomType($"global::RangersSDK.CSLib.Utility.VariableString") : new BuiltinType(PrimitiveType.String);
        }

        public override void MarshalToManaged(MarshalContext ctx)
        {
            ctx.Return.Write($"{ctx.ReturnVarName}.Value");
        }

        public override void MarshalToNative(MarshalContext ctx)
        {

            if (ctx.MarshalKind == MarshalKind.NativeField)
            {
                string assignVar;

                if (ctx.ReturnVarName.LastIndexOf('.') > ctx.ReturnVarName.LastIndexOf("->"))
                {
                    assignVar = Generator.GeneratedIdentifier(ctx.ArgName);
                    ctx.Before.WriteLine($"fixed (global::RangersSDK.CSLib.Utility.VariableString* {assignVar} = &{ctx.ReturnVarName})");
                    ctx.Before.WriteOpenBraceAndIndent();
                    (ctx as CSharpMarshalContext).HasCodeBlock = true;
                }
                else
                {
                    assignVar = $"&{ctx.ReturnVarName}";
                }

                ctx.Return.Write($"({assignVar})->Value = {ctx.Parameter.Name}");
                ctx.ReturnVarName = string.Empty;
            }
            else
            {
                var typePrinter = new CSharpTypePrinter(Context);
                string varName = $"__variableString{ctx.ParameterIndex}";
                ctx.Before.WriteLine($"var {varName} = new global::RangersSDK.CSLib.Utility.VariableString();");
                ctx.Before.WriteLine($"{varName}.Value = {ctx.Parameter.Name};");
                ctx.Return.Write($"new {typePrinter.IntPtrType}(&{varName})");
                ctx.Cleanup.WriteLine($"{varName}.Dispose();");
            }
        }
    }
}