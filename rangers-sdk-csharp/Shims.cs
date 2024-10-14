using RangersSDK.Hedgehog.Foundation;
using RangersSDK.Hedgehog.Game;
using System;
using System.Reflection;
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

namespace RangersSDK.Hedgehog.Needle
{
    public unsafe partial class Model
    {
        [StructLayout(LayoutKind.Sequential, Size = 48)]
        public partial struct __Internal
        {
            internal nint vfptr_NeedleRefcountObject;
            internal uint pad;
            internal uint refCount;
            internal ulong tnrcrUnk1;
            internal global::RangersSDK.Hedgehog.Needle.EntryLink.__Internal entryLink;
            internal uint hash;
            internal uint tnruoUnk4;
        }
    }
    public unsafe partial class EntryLink
    {
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        public partial struct __Internal
        {
            internal nint prev;
            internal nint next;
        }
    }
}
