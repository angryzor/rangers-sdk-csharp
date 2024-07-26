using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace RangersSDKBindingsGenerator.TypeMaps
{
    [TypeMap("csl::ut::MoveArray", GeneratorKind.CSharp)]
    public class MoveArray : TypeMap
    {
        public override Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CustomType($"System.Collections.Generic.List<{ctx.GetTemplateParameterList()}>");
        }

        //public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
        //{
        //}
    }

    [TypeMap("csl::ut::Bitset", GeneratorKind.CSharp)]
    public class Bitset : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            var templateType = Type.Desugar() as TemplateSpecializationType;
            return templateType.Arguments[0].Type.Type;
        }
    }
}