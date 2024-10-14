using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using System.Runtime.Intrinsics;

namespace RangersSDKBindingsGenerator.TypeMaps
{
    //public abstract class AlignedVectorTypeMap : ShimmedValueTypeMap
    //{
    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField || ctx.Kind == TypePrinterContextKind.Normal || ctx.Kind == TypePrinterContextKind.Native ? new CustomType(InternalTypeName) : new CustomType(TypeName);
    //    }

    //    public override void MarshalToManaged(MarshalContext ctx)
    //    {
    //        base.MarshalToManaged(ctx);

    //        ctx.Return.Write(".Value");
    //    }

    //    public override void MarshalToNative(MarshalContext ctx)
    //    {
    //        if (ctx.Parameter != null)
    //        {
    //            ctx.Before.WriteLine($"var __aligned_{ctx.Parameter.Name} = new {InternalTypeName} {{ Value = {ctx.Parameter.Name} }};");
    //            ctx.Parameter.Name = $"__aligned_{ctx.Parameter.Name}";
    //        }
    //        else
    //        {
    //            ctx.Before.WriteLine($"var __aligned_{ctx.ReturnVarName} = new {InternalTypeName} {{ Value = {ctx.ReturnVarName} }};");
    //            ctx.ReturnVarName = $"__aligned_{ctx.ReturnVarName}";
    //        }

    //        base.MarshalToNative(ctx);
    //    }
    //}

    //[TypeMap("csl::math::Vector2", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Vector2 : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector2";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector2));
    //    }
    //}

    //[TypeMap("csl::math::Vector3", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Vector3 : AlignedVectorTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector3";
    //    protected override string InternalTypeName => "RangersSDK.CSLib.Math.Vector3Internal";
    //}

    //[TypeMap("csl::math::Position", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Position : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector3";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector3));
    //    }
    //}

    //[TypeMap("csl::math::Rotation", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Rotation : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector3";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector3));
    //    }
    //}

    //[TypeMap("csl::math::Vector4", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Vector4 : AlignedVectorTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector4";
    //    protected override string InternalTypeName => "RangersSDK.CSLib.Math.Vector4Internal";
    //}

    //[TypeMap("csl::math::Quaternion", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Quaternion : AlignedVectorTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Quaternion";
    //    protected override string InternalTypeName => "RangersSDK.CSLib.Math.QuaternionInternal";
    //}

    //[TypeMap("csl::math::Matrix34", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Matrix34 : AlignedVectorTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Matrix4x4";
    //    protected override string InternalTypeName => "RangersSDK.CSLib.Math.Matrix34Internal";
    //}

    //[TypeMap("csl::math::Matrix44", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Matrix44 : AlignedVectorTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Matrix4x4";
    //    protected override string InternalTypeName => "RangersSDK.CSLib.Math.Matrix44Internal";
    //}



    public abstract class AlignedVectorTypeMap : ShimmedValueTypeMap
    {
        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return ctx.Type.IsReference() || ctx.MarshalKind == MarshalKind.NativeField || ctx.Kind == TypePrinterContextKind.Normal || ctx.Kind == TypePrinterContextKind.Native ? new CustomType(InternalTypeName) : new CustomType(TypeName);
        }

        public override void MarshalToNative(MarshalContext ctx)
        {
            if (ctx.Parameter != null) {
                ctx.Before.WriteLine($"{TypeName}* __aligned_{ctx.Parameter.Name} = ({TypeName}*)global::RangersSDK.Interop.Memory.AllocAligned(sizeof({TypeName}));");
                ctx.Before.WriteLine($"*__aligned_{ctx.Parameter.Name} = {ctx.Parameter.Name};");
                ctx.Cleanup.WriteLine($"global::RangersSDK.Interop.Memory.FreeAligned((__IntPtr)__aligned_{ctx.Parameter.Name});");

                ctx.Parameter.Name = $"(*__aligned_{ctx.Parameter.Name})";
            }

            base.MarshalToNative(ctx);
        }
    }

    [TypeMap("csl::math::Vector2", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Vector2 : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Vector2";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector2));
        }
    }

    [TypeMap("csl::math::Vector3", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Vector3 : AlignedVectorTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Vector3";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector3));
        }
    }

    [TypeMap("csl::math::Position", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Position : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Vector3";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector3));
        }
    }

    [TypeMap("csl::math::Rotation", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Rotation : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Vector3";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector3));
        }
    }

    [TypeMap("csl::math::Vector4", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Vector4 : AlignedVectorTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Vector4";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector4));
        }
    }

    [TypeMap("csl::math::Quaternion", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Quaternion : AlignedVectorTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Quaternion";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Quaternion));
        }
    }

    [TypeMap("csl::math::Matrix34", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Matrix34 : AlignedVectorTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Matrix4x4";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Matrix4x4));
        }
    }

    [TypeMap("csl::math::Matrix44", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Matrix44 : AlignedVectorTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Matrix4x4";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Matrix4x4));
        }
    }



    //[TypeMap("csl::math::Vector2", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Vector2 : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector2";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector2));
    //    }
    //}

    //[TypeMap("csl::math::Vector3", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Vector3 : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector3";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector3));
    //    }
    //}

    //[TypeMap("csl::math::Position", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Position : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector3";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector3));
    //    }
    //}

    //[TypeMap("csl::math::Rotation", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Rotation : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector3";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector3));
    //    }
    //}

    //[TypeMap("csl::math::Vector4", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Vector4 : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Vector4";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Vector4));
    //    }
    //}

    //[TypeMap("csl::math::Quaternion", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Quaternion : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Quaternion";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Quaternion));
    //    }
    //}

    //[TypeMap("csl::math::Matrix34", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Matrix34 : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Matrix4x4";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Matrix4x4));
    //    }
    //}

    //[TypeMap("csl::math::Matrix44", GeneratorKindID = GeneratorKind.CSharp_ID)]
    //public class Matrix44 : ShimmedValueTypeMap
    //{
    //    protected override string TypeName => $"global::System.Numerics.Matrix4x4";

    //    public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
    //    {
    //        return new CILType(typeof(System.Numerics.Matrix4x4));
    //    }
    //}
}