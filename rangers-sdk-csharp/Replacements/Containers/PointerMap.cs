using RangersSDK.Interop;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class PointerMapBase<K, V, U, KIso, VIso> : HashMap<K, V, nint, U, KIso, VIso>
        where U : unmanaged
        where KIso : InteropIsomorphism<K, nint>, new()
        where VIso : InteropIsomorphism<V, U>, new()
    {
        public class __InteropIsomorphism : InteropIsomorphism<PointerMapBase<K, V, U, KIso, VIso>, nint>
        {
            public nint GetUnmanaged(PointerMapBase<K, V, U, KIso, VIso> obj) { return (nint)obj.instance; }
            public PointerMapBase<K, V, U, KIso, VIso> GetManaged(nint obj) { return new PointerMapBase<K, V, U, KIso, VIso>(obj); }
            public void ReleaseUnmanaged(nint obj) { }
        }

        public PointerMapBase(nint native) : base(native) { }

        protected override ulong CalculateHash(nint key)
        {
            ulong k = (ulong)key;

            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdul;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53ul;
            k ^= k >> 33;

            return k;
        }

        protected override bool CompareKeys(nint key1, nint key2)
        {
            return key1 == key2;
        }
    }

    public unsafe partial class PointerMap<K, V, U, KIso, VIso> : PointerMapBase<K, V, U, KIso, VIso>
        where U : unmanaged
        where KIso : InteropIsomorphism<K, nint>, new()
        where VIso : InteropIsomorphism<V, U>, new()
    {
        public PointerMap(nint native) : base(native) { }
    }

    public unsafe partial class NakedPointerMap<V, U, Iso> : PointerMapBase<nint, V, U, IdentityInteropIsomorphism<nint>, Iso>
        where U : unmanaged
        where Iso : InteropIsomorphism<V, U>, new()
    {
        public NakedPointerMap(nint native) : base(native) { }
    }
}
