using System;
using System.Runtime.InteropServices;

namespace RangersSDK {
    namespace Csl {
        namespace Fnd {
            public unsafe partial class CSLDelegate {
                [StructLayout(LayoutKind.Sequential, Size = 0x38)]
                public struct DelegateItem {
                    public fixed char data[0x30];
                    public IntPtr functor;
                };

                [StructLayout(LayoutKind.Sequential, Size = 520)]
                public new partial struct __Internal
                {
                    internal IntPtr ptr;
                    internal ulong size;
                    internal ulong capacity;
                    internal IntPtr allocator;
                }
            }
        }
    }
}
