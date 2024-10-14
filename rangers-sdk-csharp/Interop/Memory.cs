using RangersSDK.CSLib.Foundation;
using RangersSDK.Hedgehog.Foundation;
using System.Runtime.InteropServices;
using System;

namespace RangersSDK.Interop
{
    internal static class Memory {
        public static IAllocator SDKAllocator{ get { return MemoryRouter.ModuleAllocator; } }

        public static uint Align(uint offset, uint alignment)
        {
            return (offset + alignment - 1) & ~(alignment - 1);
        }

        public static nint Align(nint offset, uint alignment)
        {
            return (offset + (nint)alignment - 1) & ~((nint)alignment - 1);
        }

        public unsafe static IntPtr AllocAligned(int size)
        {
            var unaligned = Marshal.AllocHGlobal(size + 0x18);
            var aligned = Align(unaligned, 16);
            *((IntPtr*)aligned) = unaligned;
            return aligned + 0x10;
        }

        public unsafe static void FreeAligned(IntPtr ptr)
        {
            Marshal.FreeHGlobal(*(IntPtr*)(ptr - 0x10));
        }
    }
}
