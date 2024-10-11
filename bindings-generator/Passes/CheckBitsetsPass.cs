using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Passes;
using System.Linq;

namespace RangersSDKBindingsGenerator.Passes
{
    public class CheckBitsetsPass : TranslationUnitPass
    {
        public override bool VisitClassTemplateDecl(ClassTemplate template)
        {
            if (!base.VisitClassTemplateDecl(template))
                return false;

            if (template.Name != "Bitset" || template.Namespace.Name != "Utility")
                return false;

            //template.ExplicitlyIgnore();

            foreach (var specialization in template.Specializations)
            {
                //specialization.ExplicitlyIgnore();

                var type = specialization.Arguments[0].Type.Type;

                if (!type.IsEnumType())
                    continue;

                Enumeration @enum;

                if (!type.TryGetDeclaration<Enumeration>(out @enum))
                    continue;

                @enum.GenerationKind = GenerationKind.Generate;
                @enum.Modifiers |= Enumeration.EnumModifiers.Flags;
                @enum.BuiltinType = specialization.Arguments[1].Type.Type as BuiltinType;

                foreach (var item in @enum.Items)
                    item.Value = 1ul << ((int)item.Value);
            }

            return true;
        }
    }
}