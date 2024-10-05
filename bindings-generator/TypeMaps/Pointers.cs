using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using System;

namespace RangersSDKBindingsGenerator.TypeMaps
{
    public abstract class PointerTypeMap : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField ? new CustomType(typePrinter.IntPtrType) : new CustomType((ctx.Type as TemplateSpecializationType).Arguments[0].Type.Visit(typePrinter).Type);
        }

        public override bool IsValueType => true;

        public override void MarshalToManaged(MarshalContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);

            if (!Type.TryGetClass(out var @class)) {
                ctx.Return.Write(ctx.ReturnVarName);
                return;
            }

            var pointee = (Type as TemplateSpecializationType).Arguments[0].Type.Visit(typePrinter).Type;

            ctx.Return.Write($"{pointee}.__GetOrCreateInstance({ctx.ReturnVarName})");
        }

        public override void MarshalToNative(MarshalContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            ctx.Return.Write($"{ctx.Parameter.Name}.__Instance");
        }
    }

    [TypeMap("hh::needle::intrusive_ptr", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class NeedleIntrusivePtrTypeMap : PointerTypeMap
    {
    }

    [TypeMap("hh::fnd::Reference", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class ReferenceTypeMap : PointerTypeMap
    {
    }

    [TypeMap("hh::fnd::HandleBase", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class HandleBaseTypeMap : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::RangersSDK.Hh.Fnd.HandleBase";
    }

    [TypeMap("hh::fnd::Handle", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class HandleTypeMap : ShimmedValueTypeMap
    {
        protected override string TypeName
        {
            get
            {
                var typePrinter = new CSharpTypePrinter(Context);
                var pointee = Type.GetFinalPointee() ?? Type;
                TemplateSpecializationType specType = pointee as TemplateSpecializationType;
                TagType tagType = pointee as TagType;

                if (specType != null)
                    return $"global::RangersSDK.Hh.Fnd.Handle<{specType.Arguments[0].Type.Visit(typePrinter).Type}>";
                else if (tagType != null)
                    return $"global::RangersSDK.Hh.Fnd.Handle<{(tagType.Declaration as ClassTemplateSpecialization).Arguments[0].Type.Visit(typePrinter).Type}>";

                return "itsallfuckedup";
            }
        }
    }

    [TypeMap("hh::fnd::ResReflectionT", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class ResReflectionTTypeMap : TypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            return ctx.Type.IsReference() ? new CustomType(typePrinter.IntPtrType) : new CustomType($"global::RangersSDK.Hh.Fnd.ResReflection");
        }
    }
}
