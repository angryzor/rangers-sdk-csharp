using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using System.Runtime.Intrinsics;

namespace RangersSDKBindingsGenerator.TypeMaps {
    [TypeMap("csl::ut::Color8", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class Color8Map : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::RangersSDK.CSLib.Utility.Color8";
    }

    [TypeMap("csl::ut::Colorf", GeneratorKindID = GeneratorKind.CSharp_ID)]
    public class ColorfMap : ShimmedValueTypeMap
    {
        protected override string TypeName => $"global::RangersSDK.CSLib.Utility.Colorf";
    }
}
