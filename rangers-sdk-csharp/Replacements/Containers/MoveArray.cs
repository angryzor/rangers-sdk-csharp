using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class MoveArray<T, U, Iso> : Array<T, U, Iso, MoveArray<T, U, Iso>.__Internal>
        where U : unmanaged
        where Iso : InteropIsomorphism<T, U>, new()
    {
        public class __InteropIsomorphism : InteropIsomorphism<MoveArray<T, U, Iso>, nint>
        {
            public nint GetUnmanaged(MoveArray<T, U, Iso> obj) { return (nint)obj.instance; }
            public MoveArray<T, U, Iso> GetManaged(nint obj) { return new MoveArray<T, U, Iso>(obj); }
            public void ReleaseUnmanaged(nint obj) { }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal : ArrayInternalProvider<U>
        {
            public U* Buffer { get; set; }
            private ulong length;
            private ulong capacity;
            public nint Allocator { get; set; }

            public uint Length
            {
                get { return (uint)length; }
                set { length = value; }
            }

            public uint Capacity
            {
                get { return (uint)capacity; }
                set { capacity = value; }
            }
        }

        public MoveArray(IAllocator allocator) : base(allocator) { }
        public MoveArray(nint native) : base(native) { }
    }
}
