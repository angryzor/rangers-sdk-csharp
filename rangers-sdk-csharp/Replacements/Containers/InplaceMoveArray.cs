using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe partial class InplaceMoveArray<T, U, Iso, Specialization> : Array<T, U, Iso, InplaceMoveArray<T, U, Iso, Specialization>.__Internal>
        where U : unmanaged
        where Iso : InteropIsomorphism<T, U>, new()
        where Specialization : unmanaged
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal : ArrayInternalProvider<U>
        {
            private Specialization specialization;

            public U* Buffer
            {
                get { fixed (Specialization* moveArray = &specialization) return ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Buffer; }
                set { fixed (Specialization* moveArray = &specialization) ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Buffer = value; }
            }

            public nint Allocator
            {
                get { fixed (Specialization* moveArray = &specialization) return ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Allocator; }
                set { fixed (Specialization* moveArray = &specialization) ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Allocator = value; }
            }

            public uint Length
            {
                get { fixed (Specialization* moveArray = &specialization) return ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Length; }
                set { fixed (Specialization* moveArray = &specialization) ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Length = value; }
            }

            public uint Capacity
            {
                get { fixed (Specialization* moveArray = &specialization) return ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Capacity; }
                set { fixed (Specialization* moveArray = &specialization) ((MoveArray<T, U, Iso>.__Internal*)moveArray)->Capacity = value; }
            }
        }

        public InplaceMoveArray(IAllocator allocator) : base(allocator) { }
        public InplaceMoveArray(nint native) : base(native) { }
    }
}
