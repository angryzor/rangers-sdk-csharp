using RangersSDK.Interop;
using System;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class StringMap<V, U, Iso> : HashMap<string, V, nint, U, AnsiStringInteropIsomorphism, Iso>
        where U : unmanaged
        where Iso : InteropIsomorphism<V, U>, new()
    {
        public class __InteropIsomorphism : InteropIsomorphism<StringMap<V, U, Iso>, nint>
        {
            public nint GetUnmanaged(StringMap<V, U, Iso> obj) { return (nint)obj.instance; }
            public StringMap<V, U, Iso> GetManaged(nint obj) { return new StringMap<V, U, Iso>(obj); }
            public void ReleaseUnmanaged(nint obj) { }
        }

        public StringMap(nint native) : base(native) { }

        protected override ulong CalculateHash(nint key)
        {
            if (key == IntPtr.Zero)
                return 0;

            string str = Marshal.PtrToStringAnsi(key);

            if (str.Length == 0)
                return 0;

            ulong hash = 0;

            for (int i = 1; i < str.Length; i++)
                hash = 31 * hash + str[i];

            return hash;
        }

        protected override bool CompareKeys(nint key1, nint key2)
        {
            string str1 = Marshal.PtrToStringAnsi(key1);
            string str2 = Marshal.PtrToStringAnsi(key2);

            return str1 == str2;
        }
    }
}
