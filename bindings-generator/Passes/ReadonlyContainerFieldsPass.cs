using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Passes;
using System.Linq;

namespace RangersSDKBindingsGenerator.Passes
{
    public class ReadonlyContainerFieldsPass : TranslationUnitPass
    {
        public override bool VisitFieldDecl(Field field)
        {
            string[] classes = { "MoveArray", "MoveArray32", "InplaceMoveArray", "InplaceBitArray", "LinkList", "PointerMap", "StringMap" };
            foreach (string className in classes)
            {
                if (field.Type.TryGetClass(out var @class) && @class.Name == className)
                {
                    var q = field.QualifiedType;
                    q.Qualifiers.IsConst = true;
                    field.QualifiedType = q;
                }
            }
            return true;
        }
    }
}
