using System;
using System.Reflection;
using System.Runtime.InteropServices;
using RangersSDK.Hedgehog.Foundation;

namespace RangersSDK.Hedgehog.Foundation
{
    public unsafe partial class ResourceManager
    {
        public T GetResource<T>(string name) where T : ManagedResource
        {
            var typeInfo = typeof(T).GetProperty("typeInfo").GetValue(null) as ResourceTypeInfo;
            return (T)(object)typeof(T).GetMethod("__GetOrCreateInstance", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { GetResource(name, RangersSDK.Hedgehog.Graphics.ResModel.typeInfo).__Instance, false, true });
        }
    }
}
