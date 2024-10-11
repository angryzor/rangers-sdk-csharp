using RangersSDK.CSLib.Foundation;
using RangersSDK.Hedgehog.Foundation;

namespace RangersSDK.Interop
{
    internal static class Memory {
        public static IAllocator SDKAllocator{ get { return MemoryRouter.ModuleAllocator; } }
    }
}
