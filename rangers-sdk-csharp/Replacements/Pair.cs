using RangersSDK.Interop;
using System;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class Pair<F, S, FU, SU, FIso, SIso>
        where FU : unmanaged
        where SU : unmanaged
        where FIso : InteropIsomorphism<F, FU>, new()
        where SIso : InteropIsomorphism<S, SU>, new()
    {
        public class __InteropIsomorphism : RefTypeReplacement<Pair<F, S, FU, SU, FIso, SIso>.__Internal>, InteropIsomorphism<Pair<F, S, FU, SU, FIso, SIso>, nint>
        {
            public nint GetUnmanaged(Pair<F, S, FU, SU, FIso, SIso> obj) { return (nint)obj.instance; }
            public Pair<F, S, FU, SU, FIso, SIso> GetManaged(nint obj) { return new Pair<F, S, FU, SU, FIso, SIso>(obj); }
            public void ReleaseUnmanaged(nint obj) { }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct __Internal
        {
            internal FU First { get; set; }
            internal SU Second { get; set; }
        }

        public __Internal* instance;
        private FIso fIso = new FIso();
        private SIso sIso = new SIso();

        public Pair(nint native)
        {
            instance = (__Internal*)native;
        }

        public F First
        {
            get { return fIso.GetManaged(instance->First); }
            set {
                fIso.ReleaseUnmanaged(instance->First);
                instance->First = fIso.GetUnmanaged(value);
            }
        }

        public S Second
        {
            get { return sIso.GetManaged(instance->Second); }
            set
            {
                sIso.ReleaseUnmanaged(instance->Second);
                instance->Second = sIso.GetUnmanaged(value);
            }
        }
    }
}
