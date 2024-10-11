using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.Interop
{
    public interface InteropIsomorphism<M, U> where U : unmanaged
    {
        public U GetUnmanaged(M obj);
        public M GetManaged(U obj);
        public void ReleaseUnmanaged(U obj);
    }

    public class IdentityInteropIsomorphism<M> : InteropIsomorphism<M, M> where M : unmanaged
    {
        public M GetUnmanaged(M obj) { return obj; }
        public M GetManaged(M obj) { return obj; }
        public void ReleaseUnmanaged(M obj) { }
    }

    public class AnsiStringInteropIsomorphism : InteropIsomorphism<string, nint>
    {
        static HashSet<nint> allocatedStrings = new HashSet<nint>();
        public nint GetUnmanaged(string obj)
        {
            nint ptr = Marshal.StringToHGlobalAnsi(obj);
            allocatedStrings.Add(ptr);
            return ptr;
        }
        public string GetManaged(nint obj) { return Marshal.PtrToStringAnsi(obj); }
        public void ReleaseUnmanaged(nint obj)
        {
            if (allocatedStrings.Contains(obj))
            {
                allocatedStrings.Remove(obj);
                Marshal.FreeHGlobal(obj);
            }
        }
    }

    //public class UTF8StringInteropIsomorphism : InteropIsomorphism<string, nint>
    //{
    //    static HashSet<nint> allocatedStrings = new HashSet<nint>();
    //    public nint GetUnmanaged(string obj)
    //    {
    //        nint ptr = Marshal.StringToHGlobalAnsi(obj);
    //        allocatedStrings.Add(ptr);
    //        return ptr;
    //    }
    //    public string GetManaged(nint obj) { return Marshal.PtrToStringUTF8(obj); }
    //    public void ReleaseUnmanaged(nint obj)
    //    {
    //        if (allocatedStrings.Contains(obj))
    //        {
    //            allocatedStrings.Remove(obj);
    //            Marshal.FreeHGlobal(obj);
    //        }
    //    }
    //}

    public class UniStringInteropIsomorphism : InteropIsomorphism<string, nint>
    {
        static HashSet<nint> allocatedStrings = new HashSet<nint>();
        public nint GetUnmanaged(string obj)
        {
            nint ptr = Marshal.StringToHGlobalUni(obj);
            allocatedStrings.Add(ptr);
            return ptr;
        }
        public string GetManaged(nint obj) { return Marshal.PtrToStringUni(obj); }
        public void ReleaseUnmanaged(nint obj)
        {
            if (allocatedStrings.Contains(obj))
            {
                allocatedStrings.Remove(obj);
                Marshal.FreeHGlobal(obj);
            }
        }
    }
}
