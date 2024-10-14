using System;
using System.Reflection;
using System.Runtime.InteropServices;
using RangersSDK.Hedgehog.Foundation;
using RangersSDK.Hedgehog.Game;

namespace RangersSDK.Hedgehog.Game
{
    public unsafe partial class GameObject
    {
        public T CreateComponent<T>() where T : GOComponent
        {
            var @class = typeof(T).GetProperty("Class").GetValue(null) as GOComponentClass;
            return (T)(object)typeof(T).GetMethod("__GetOrCreateInstance", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { CreateComponent(@class).__Instance, false, true });
        }

        public T GetComponent<T>() where T : GOComponent
        {
            var @class = typeof(T).GetProperty("Class").GetValue(null) as GOComponentClass;
            return (T)(object)typeof(T).GetMethod("__GetOrCreateInstance", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { GetComponent(@class).__Instance, false, true });
        }

        public unsafe T GetWorldData<T>() where T : unmanaged
        {
            var rflClass = RflClass.__GetOrCreateInstance((nint)1, false, true); // This is really dirty but this function doesn't actually use the parameter so why do an expensive lookup.
            var worldDataPtr = GetWorldDataByClass(rflClass);
            //var getMethod = typeof(T).GetMethod("__GetOrCreateInstance", BindingFlags.Static | BindingFlags.NonPublic);
            
            //if (getMethod != null)
            //    return (T)(object)getMethod.Invoke(null, new object[] { worldDataPtr, false, true });

            return *(T*)worldDataPtr;
        }
    }
}
