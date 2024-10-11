using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace RangersSDK.CSLib.Utility
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct VariableString
    {
        [SuppressUnmanagedCodeSecurity, DllImport("rangers-sdk", EntryPoint = "?assign@VariableString@ut@csl@@IEAAXPEAVIAllocator@fnd@3@PEBDH@Z", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void assign(VariableString* self, nint allocator, string str, int size);

        [SuppressUnmanagedCodeSecurity, DllImport("rangers-sdk", EntryPoint = "??1VariableString@ut@csl@@QEAA@XZ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void dtor(VariableString* self);

        internal nint Buffer { get; set; }
        internal nint AllocatorAndOwnedFlag { get; set; } // The code takes advantage of the alignment of allocators to stuff an owned flag in the LSB.

        public VariableString()
        {
            Buffer = IntPtr.Zero;
            AllocatorAndOwnedFlag = Hedgehog.Foundation.MemoryRouter.ModuleAllocator.__Instance;
        }

        internal bool IsBufferOwned
        {
            get
            {
                return ((ulong)AllocatorAndOwnedFlag & 1ul) != 0;
            }
        }

        internal IAllocator.__Internal* Allocator
        {
            get
            {
                return (IAllocator.__Internal*)(nint)((ulong)AllocatorAndOwnedFlag & 0xFFFFFFFFFFFFFFFEul);
            }
        }

        internal string Value
        {
            get { return Marshal.PtrToStringAnsi(Buffer); }
            set { fixed (VariableString* self = &this) assign(self, Hedgehog.Foundation.MemoryRouter.ModuleAllocator.__Instance, value, -1); }
        }

        internal void Dispose()
        {
            fixed (VariableString* self = &this) dtor(self);
        }
    }
}
