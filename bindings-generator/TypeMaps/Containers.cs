using CppSharp;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Generators.TS;
using CppSharp.Types;

namespace RangersSDKBindingsGenerator.TypeMaps
{
    public abstract class ContainerTypeMap : ShimmedReferenceTypeMap
    {
        protected bool IsUnmanaged(Type type)
        {
            return type.IsPrimitiveType() || type.IsPointerToPrimitiveType() || type.IsEnumType() || (type.TryGetClass(out Class @class) && (@class.Type == ClassType.ValueType || @class.Name == "Handle"));
        }

        protected string GetPrefix(Type type)
        {
            return IsUnmanaged(type) ? "Unmanaged" : "Managed";
        }
        protected string GetShortPrefix(Type type)
        {
            return GetPrefix(type).Substring(0, 1);
        }
    }

    [TypeMap("csl::ut::MoveArray", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class MoveArray : ContainerTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var typePrinter = new CSharpTypePrinter(Context);
                var innerType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var prefix = GetPrefix(innerType.Type);

                return $"global::RangersSDK.Csl.Ut.{prefix}MoveArray<{innerType.Visit(typePrinter).Type}>";
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
                var typePrinter = new CSharpTypePrinter(Context);
                var innerType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var prefix = GetPrefix(innerType.Type);

                return $"global::RangersSDK.Csl.Ut.{prefix}InplaceMoveArray<{innerType.Visit(typePrinter).Type}>";
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
                var typePrinter = new CSharpTypePrinter(Context);
                var innerType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var prefix = GetPrefix(innerType.Type);

                return $"global::RangersSDK.Csl.Ut.{prefix}MoveArray32<{innerType.Visit(typePrinter).Type}>";
            }
        }
    }

    [TypeMap("csl::ut::LinkListNode", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class LinkListNode : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField
                ? new CustomType($"global::RangersSDK.Csl.Ut.LinkListNode")
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
                var typePrinter = new CSharpTypePrinter(Context);
                var innerType = (Type as TemplateSpecializationType).Arguments[0].Type;

                return $"global::RangersSDK.Csl.Ut.LinkList<{innerType.Visit(typePrinter).Type}>";
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
                var typePrinter = new CSharpTypePrinter(Context);
                var keyType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var valueType = (Type as TemplateSpecializationType).Arguments[1].Type;
                var typeName = IsUnmanaged(keyType.Type)
                    ? $"NakedPointerMap<{valueType.Visit(typePrinter).Type}>"
                    : $"PointerMap<{keyType.Visit(typePrinter).Type}, {valueType.Visit(typePrinter).Type}>";
                var prefix = GetPrefix(valueType.Type);

                return $"global::RangersSDK.Csl.Ut.{prefix}{typeName}";
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
                var typePrinter = new CSharpTypePrinter(Context);
                var valueType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var prefix = GetPrefix(valueType.Type);

                return $"global::RangersSDK.Csl.Ut.{prefix}StringMap<{valueType.Visit(typePrinter).Type}>";
            }
        }
    }

    [TypeMap("csl::fnd::Delegate", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class CSLDelegate : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            return new CustomType("global::RangersSDK.Csl.Fnd.CSLDelegate");
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
                var typePrinter = new CSharpTypePrinter(Context);
                var firstType = (Type as TemplateSpecializationType).Arguments[0].Type;
                var secondType = (Type as TemplateSpecializationType).Arguments[1].Type;
                var firstPrefix = GetShortPrefix(firstType.Type);
                var secondPrefix = GetShortPrefix(secondType.Type);

                return $"global::RangersSDK.Csl.Ut.{firstPrefix}{secondPrefix}Pair<{firstType.Visit(typePrinter).Type}, {secondType.Visit(typePrinter).Type}>";
            }
        }
    }
}