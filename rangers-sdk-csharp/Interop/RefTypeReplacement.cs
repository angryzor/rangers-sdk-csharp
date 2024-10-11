using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;

namespace RangersSDK.Interop {
    public unsafe class RefTypeReplacement<I> : IDisposable
        where I : unmanaged
    {
        public I* instance;
        private bool disposed = false;
        private bool ownedByManagedCode = false;

        public RefTypeReplacement()
        {
            this.instance = (I*)Memory.SDKAllocator.Alloc((ulong)sizeof(I), 16);
            this.ownedByManagedCode = true;
        }

        public RefTypeReplacement(nint native)
        {
            this.instance = (I*)native;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    DisposeManagedResources();

                if (ownedByManagedCode)
                    DisposeNativeResources();

                disposed = true;
            }
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeNativeResources()
        {
            Memory.SDKAllocator.Free((nint)this.instance);
        }

        ~RefTypeReplacement()
        {
            Dispose(disposing: false);
        }
    }
}
