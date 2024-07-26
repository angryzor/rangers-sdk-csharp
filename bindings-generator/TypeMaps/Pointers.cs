using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using System;

namespace RangersSDKBindingsGenerator.TypeMaps
{
    [TypeMap("hh::needle::intrusive_ptr", GeneratorKind.CSharp)]
    public class NeedleIntrusivePtr : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            return new CustomType(new CSharpTypePrinter(Context).IntPtrType);
        }

        public override bool IsValueType => true;
    }
}
