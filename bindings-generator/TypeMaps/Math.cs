using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using System.Runtime.Intrinsics;

namespace RangersSDKBindingsGenerator.TypeMaps
{
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
    public class Vector3 : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Vector3";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector3));
        }
    }

    [TypeMap("csl::math::Vector4", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Vector4 : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Vector4";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector4));
        }
    }

    [TypeMap("csl::math::Quaternion", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Quaternion : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Quaternion";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Quaternion));
        }
    }

    [TypeMap("csl::math::Matrix34", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Matrix34 : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Matrix4x4";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Matrix4x4));
        }
    }

    [TypeMap("csl::math::Matrix44", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Matrix44 : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::System.Numerics.Matrix4x4";

        public override CppSharp.AST.Type SignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Matrix4x4));
        }
    }
}