using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using RangersSDK.CSLib.Foundation;
using RangersSDK.Hedgehog.Foundation;

namespace RangersSDK.Reflection
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class RflClassAttribute : Attribute
    {
        public RflClassAttribute() { }
    }

    public static class RflClassReflector
    {
        private static List<RflTypeInfo.TypeConstructor> constructDelegates = new List<RflTypeInfo.TypeConstructor>();

        static RflClassMember.Type GetTypeFor(Type type)
        {
            if (type == typeof(bool)) return RflClassMember.Type.TYPE_BOOL;
            if (type == typeof(byte)) return RflClassMember.Type.TYPE_UINT8;
            if (type == typeof(sbyte)) return RflClassMember.Type.TYPE_SINT8;
            if (type == typeof(ushort)) return RflClassMember.Type.TYPE_UINT16;
            if (type == typeof(short)) return RflClassMember.Type.TYPE_SINT16;
            if (type == typeof(uint)) return RflClassMember.Type.TYPE_UINT32;
            if (type == typeof(int)) return RflClassMember.Type.TYPE_SINT32;
            if (type == typeof(ulong)) return RflClassMember.Type.TYPE_UINT64;
            if (type == typeof(long)) return RflClassMember.Type.TYPE_SINT64;
            if (type == typeof(float)) return RflClassMember.Type.TYPE_FLOAT;
            if (type == typeof(System.Numerics.Vector2)) return RflClassMember.Type.TYPE_VECTOR2;
            if (type == typeof(System.Numerics.Vector3)) return RflClassMember.Type.TYPE_VECTOR3;
            if (type == typeof(System.Numerics.Vector4)) return RflClassMember.Type.TYPE_VECTOR4;
            if (type == typeof(System.Numerics.Quaternion)) return RflClassMember.Type.TYPE_QUATERNION;
            if (type == typeof(System.Numerics.Matrix4x4)) return RflClassMember.Type.TYPE_MATRIX34;

            throw new ArgumentException();
        }
        static uint GetSize(Type type)
        {
            if (type == typeof(bool)) return 1;
            if (type == typeof(byte)) return 1;
            if (type == typeof(sbyte)) return 1;
            if (type == typeof(ushort)) return 2;
            if (type == typeof(short)) return 2;
            if (type == typeof(uint)) return 4;
            if (type == typeof(int)) return 4;
            if (type == typeof(ulong)) return 8;
            if (type == typeof(long)) return 8;
            if (type == typeof(float)) return 4;
            if (type == typeof(System.Numerics.Vector2)) return 8;
            if (type == typeof(System.Numerics.Vector3)) return 16;
            if (type == typeof(System.Numerics.Vector4)) return 16;
            if (type == typeof(System.Numerics.Quaternion)) return 16;
            if (type == typeof(System.Numerics.Matrix4x4)) return 64;

            throw new ArgumentException();
        }
        static uint GetAlignment(Type type)
        {
            if (type == typeof(bool)) return 1;
            if (type == typeof(byte)) return 1;
            if (type == typeof(sbyte)) return 1;
            if (type == typeof(ushort)) return 2;
            if (type == typeof(short)) return 2;
            if (type == typeof(uint)) return 4;
            if (type == typeof(int)) return 4;
            if (type == typeof(ulong)) return 8;
            if (type == typeof(long)) return 8;
            if (type == typeof(float)) return 4;
            if (type == typeof(System.Numerics.Vector2)) return 4;
            if (type == typeof(System.Numerics.Vector3)) return 16;
            if (type == typeof(System.Numerics.Vector4)) return 16;
            if (type == typeof(System.Numerics.Quaternion)) return 16;
            if (type == typeof(System.Numerics.Matrix4x4)) return 16;

            throw new ArgumentException();
        }

        static RflClassMember GenerateRflClassMember(FieldInfo fieldInfo, uint offset)
        {
            return new RflClassMember
            {
                MPName = fieldInfo.Name,
                MType = GetTypeFor(fieldInfo.FieldType),
                MSubType = RflClassMember.Type.TYPE_VOID,
                MOffset = offset,
            };
        }

        static uint Align(uint offset, uint alignment)
        {
            return (offset + alignment - 1) & ~(alignment - 1);
        }

        public static RflClass GenerateRflClass(Type type)
        {
            uint offset = 0;
            uint maxAlignment = 1;

            RflClass baseType = null;

            if (type.BaseType != null && type.BaseType.FullName != "System.ValueType")
            {
                baseType = GenerateRflClass(type.BaseType);
                offset += baseType.MClassSize;
                maxAlignment = (uint)baseType.Alignment;
            }

            RflClassMember[] members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(field =>
            {
                var alignment = GetAlignment(field.FieldType);
                offset = Align(offset, alignment);
                var member = GenerateRflClassMember(field, offset);
                offset += GetSize(field.FieldType);
                maxAlignment = System.Math.Max(maxAlignment, alignment);
                return member;
            }).ToArray();

            offset = Align(offset, maxAlignment);

            return new RflClass
            {
                MPName = type.Name,
                MPParent = baseType,
                MClassSize = offset,
                MPMembers = members,
            };
        }

        public interface RflType<T>
        {
            public void Initialize(IAllocator allocator);
        }

        public static RflTypeInfo GenerateRflTypeInfo(Type type, RflClass rflClass)
        {
            ulong classSize = rflClass.SizeInBytes;

            RflTypeInfo.TypeConstructor construct = (instance, allocator) =>
            {
                unsafe
                {
                    for (ulong i = 0; i < classSize; i++)
                        Marshal.WriteByte(instance + (nint)i, 0);
                }
                return instance;
            };

            constructDelegates.Add(construct);

            return new RflTypeInfo
            {
                MPName = type.Name,
                MPScopedName = type.Name,
                MFpConstruct = construct,
                MSize = classSize,
            };
        }

        public static void DiscoverRflClasses()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()))
            {
                if (Attribute.GetCustomAttribute(type, typeof(RflClassAttribute)) != null)
                {
                    var rflc = GenerateRflClass(type);
                    var rflti = GenerateRflTypeInfo(type, rflc);

                    Singleton<RflClassNameRegistry>.instance.Register(rflc);
                    Singleton<RflTypeInfoRegistry>.instance.Register(rflti);
                }
            }
        }
    }
}
