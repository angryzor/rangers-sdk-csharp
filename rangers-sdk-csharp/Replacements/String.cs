using RangersSDK.Interop;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class String : MoveArray32<byte, byte, IdentityInteropIsomorphism<byte>>
    {
        public String(nint native) : base(native) { }
    }
}
