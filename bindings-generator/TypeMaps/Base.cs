using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Generators.TS;
using CppSharp.Types;
using System.Runtime.Intrinsics;

namespace RangersSDKBindingsGenerator.TypeMaps {
    public abstract class ShimmedReferenceTypeMap : TypeMap
    {
        protected abstract string TypeName { get; }

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField ? new CustomType($"{TypeName}.__Internal") : new CustomType($"{TypeName}");
        }

        public override void MarshalToManaged(MarshalContext ctx)
        {
            ctx.Return.Write($"new {TypeName}({ctx.ReturnVarName})");
        }

        // TODO handle `is null ? __IntPtr.Zero`
        public override void MarshalToNative(MarshalContext ctx)
        {
            var typePrinter = new CSharpTypePrinter(Context);
            if (ctx.Parameter != null && !ctx.Parameter.IsOut && !ctx.Parameter.IsInOut)
                ctx.Return.Write($"new {typePrinter.IntPtrType}({ctx.Parameter.Name}.instance)");
            else
                ctx.Return.Write($"{ctx.ReturnVarName}.instance");
        }

        public override string CSharpConstruct()
        {
            return $"new __IntPtr(new {TypeName}(global::RangersSDK.Hedgehog.Foundation.MemoryRouter.ModuleAllocator).instance)";
        }
    }

    public abstract class ShimmedValueTypeMap : TypeMap
    {
        protected abstract string TypeName { get; }

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CustomType(TypeName);
        }

        public override void MarshalToManaged(MarshalContext ctx)
        {
            if (ctx.Parameter != null && !ctx.Parameter.IsOut && !ctx.Parameter.IsInOut)
                ctx.Return.Write($"*({TypeName}*){ctx.Parameter.Name}");
            else
            {
                if (
                    (!(ctx.ReturnType.Type.IsReference() || ctx.ReturnType.Type.IsPointer()) && ctx.MarshalKind == MarshalKind.NativeField)
                    || (ctx.Function != null && ctx.Function.HasIndirectReturnTypeParameter))
                    ctx.Return.Write(ctx.ReturnVarName);
                else
                    ctx.Return.Write($"*({TypeName}*){ctx.ReturnVarName}");
            }
        }

        public override void MarshalToNative(MarshalContext ctx)
        {
             var typePrinter = new CSharpTypePrinter(Context);
            if (ctx.Parameter != null) {
                if (ctx.Parameter.Type.IsReference() || ctx.Parameter.Type.IsPointer())
                    ctx.Return.Write($"new {typePrinter.IntPtrType}(&{ctx.Parameter.Name})");
                else
                    ctx.Return.Write(ctx.Parameter.Name);
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName);
            }
        }

        public override string CSharpConstruct()
        {
            return "";
        }

        public override bool IsValueType => true;
    }
}
