using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class MoveArray32<T, U, Iso> : Array<T, U, Iso, MoveArray32<T, U, Iso>.__Internal>
        where U : unmanaged
        where Iso : InteropIsomorphism<T, U>, new()
    {
        public class __InteropIsomorphism : InteropIsomorphism<MoveArray32<T, U, Iso>, nint>
        {
            public nint GetUnmanaged(MoveArray32<T, U, Iso> obj) { return (nint)obj.instance; }
            public MoveArray32<T, U, Iso> GetManaged(nint obj) { return new MoveArray32<T, U, Iso>(obj); }
            public void ReleaseUnmanaged(nint obj) { }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal : ArrayInternalProvider<U>
        {
            public U* Buffer { get; set; }
            public uint Length { get; set; }
            public uint Capacity { get; set; }
            public nint Allocator { get; set; }
        }

        public MoveArray32(IAllocator allocator) : base(allocator) { }
        public MoveArray32(nint native) : base(native) { }
    }
}
