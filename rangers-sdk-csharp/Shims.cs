using System;
using System.Runtime.InteropServices;

public unsafe partial class RTL_CRITICAL_SECTION
{
    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    public struct __Internal
    {
        internal IntPtr DebugInfo;
        internal uint LockCount;
        internal uint RecursionCount;
        internal IntPtr OwningThread;
        internal IntPtr LockSemaphore;
        internal ulong SpinCount;
    }
}
