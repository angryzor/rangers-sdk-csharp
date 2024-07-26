using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Types;

namespace RangersSDKBindingsGenerator.TypeMaps
{
    [TypeMap("csl::math::Vector2", GeneratorKind.CSharp)]
    public class Vector2 : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector2));
        }

        public override bool IsValueType => true;
    }

    [TypeMap("csl::math::Vector3", GeneratorKind.CSharp)]
    public class Vector3 : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector3));
        }

        public override bool IsValueType => true;
    }

    [TypeMap("csl::math::Vector4", GeneratorKind.CSharp)]
    public class Vector4 : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Vector4));
        }

        public override bool IsValueType => true;
    }

    [TypeMap("csl::math::Quaternion", GeneratorKind.CSharp)]
    public class Quaternion : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Quaternion));
        }

        public override bool IsValueType => true;
    }

    [TypeMap("csl::math::Matrix34", GeneratorKind.CSharp)]
    public class Matrix34 : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Matrix4x4));
        }

        public override bool IsValueType => true;
    }

    [TypeMap("csl::math::Matrix44", GeneratorKind.CSharp)]
    public class Matrix44 : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CILType(typeof(System.Numerics.Matrix4x4));
        }

        public override bool IsValueType => true;
    }
}