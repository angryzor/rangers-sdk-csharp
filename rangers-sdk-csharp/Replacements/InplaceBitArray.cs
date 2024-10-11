using RangersSDK.Interop;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class InplaceBitArray<Specialization> : InplaceMoveArray<ulong, ulong, IdentityInteropIsomorphism<ulong>, Specialization>
        where Specialization : unmanaged
    {
        public InplaceBitArray(nint native) : base(native) { }
    }
}
