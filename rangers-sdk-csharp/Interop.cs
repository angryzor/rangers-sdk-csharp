using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.Interop
{
    public interface CppSharpObject<M>
    {
        public nint __Instance { get; }
        internal abstract static M __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false);
    }

    //public interface CppSharpObjectWithVTable<M> : CppSharpObject<M>
    //{
    //    internal abstract static M __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false);

    //    public static M GetUnmanagedObject(nint native)
    //    {
    //        return __GetOrCreateInstance(native);
    //    }
    //}

    //public interface CppSharpObjectWithoutVTable<M> : CppSharpObject<M>
    //{
    //    internal abstract static M __CreateInstance(nint native);

    //    public static M GetUnmanagedObject(nint native)
    //    {
    //        return __CreateInstance(native);
    //    }
    //}

    public interface InteropIsomorphism<M, U> where U : unmanaged
    {
        public abstract static U GetUnmanaged(M obj);
        public abstract static M GetManaged(U obj);
        public abstract static void ReleaseUnmanaged(U obj);
    }

    public class IdentityInteropIsomorphism<M> : InteropIsomorphism<M, M> where M : unmanaged
    {
        public static M GetUnmanaged(M obj) { return obj; }
        public static M GetManaged(M obj) { return obj; }
        public static void ReleaseUnmanaged(M obj) { }
    }

    public class AnsiStringInteropIsomorphism : InteropIsomorphism<string, nint>
    {
        static HashSet<nint> allocatedStrings = new HashSet<nint>();
        public static nint GetUnmanaged(string obj)
        {
            nint ptr = Marshal.StringToHGlobalAnsi(obj);
            allocatedStrings.Add(ptr);
            return ptr;
        }
        public static string GetManaged(nint obj) { return Marshal.PtrToStringAnsi(obj); }
        public static void ReleaseUnmanaged(nint obj)
        {
            if (allocatedStrings.Contains(obj))
            {
                allocatedStrings.Remove(obj);
                Marshal.FreeHGlobal(obj);
            }
        }
    }

    public class UTF8StringInteropIsomorphism : InteropIsomorphism<string, nint>
    {
        static HashSet<nint> allocatedStrings = new HashSet<nint>();
        public static nint GetUnmanaged(string obj)
        {
            nint ptr = Marshal.StringToHGlobalAnsi(obj);
            allocatedStrings.Add(ptr);
            return ptr;
        }
        public static string GetManaged(nint obj) { return Marshal.PtrToStringUTF8(obj); }
        public static void ReleaseUnmanaged(nint obj)
        {
            if (allocatedStrings.Contains(obj))
            {
                allocatedStrings.Remove(obj);
                Marshal.FreeHGlobal(obj);
            }
        }
    }

    public class UniStringInteropIsomorphism : InteropIsomorphism<string, nint>
    {
        static HashSet<nint> allocatedStrings = new HashSet<nint>();
        public static nint GetUnmanaged(string obj)
        {
            nint ptr = Marshal.StringToHGlobalUni(obj);
            allocatedStrings.Add(ptr);
            return ptr;
        }
        public static string GetManaged(nint obj) { return Marshal.PtrToStringUni(obj); }
        public static void ReleaseUnmanaged(nint obj)
        {
            if (allocatedStrings.Contains(obj))
            {
                allocatedStrings.Remove(obj);
                Marshal.FreeHGlobal(obj);
            }
        }
    }

    public class CppSharpInteropIsomorphism<M> : InteropIsomorphism<M, nint>
        where M : CppSharpObject<M>
    {
        public static nint GetUnmanaged(M obj) { return obj.__Instance; }
        public static M GetManaged(nint obj) { return M.__GetOrCreateInstance(obj); }
        public static void ReleaseUnmanaged(nint obj) { }
    }
}