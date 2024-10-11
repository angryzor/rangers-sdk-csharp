using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RangersSDK.CSLib.Utility;

namespace RangersSDK.Hedgehog.Foundation
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct RflArrayInternal<U> where U : unmanaged
    {
        internal U* buffer { get; set; }
        internal uint size { get; set; }
    }

    //public unsafe class RflArray<T, U, Iso> : RefTypeReplacement<RflArray<T, U, Iso>.__Internal>, IReadOnlyList<T>
    //    where U : unmanaged
    //    where Iso : InteropIsomorphism<T, U>, new()
    //{
    //    [StructLayout(LayoutKind.Sequential)]
    //    public unsafe struct __Internal
    //    {
    //        internal U* buffer { get; set; }
    //        internal uint size { get; set; }
    //    }
    //    public class Enumerator : IEnumerator<T>
    //    {
    //        private RflArray<T, U, Iso> arr;
    //        private int index;

    //        public Enumerator(RflArray<T, U, Iso> arr)
    //        {
    //            this.arr = arr;
    //            index = -1;
    //        }

    //        public T Current
    //        {
    //            get { return arr[index]; }
    //        }

    //        object IEnumerator.Current
    //        {
    //            get { return Current; }
    //        }

    //        public bool MoveNext()
    //        {
    //            return ++index < arr.Count;
    //        }

    //        public void Reset()
    //        {
    //            index = -1;
    //        }

    //        public void Dispose()
    //        {
    //        }
    //    }

    //    Iso iso = new Iso();

    //    public RflArray()
    //    {
    //    }

    //    public RflArray(ICollection<T> values)
    //    {
    //        U* newBuffer = (U*)Memory.SDKAllocator.Alloc((ulong)(sizeof(U) * values.Count), 16);

    //        int i = 0;
    //        foreach (T value in values)
    //            newBuffer[i++] = iso.GetUnmanaged(value);

    //        instance->buffer = newBuffer;
    //        instance->size = (uint)values.Count;
    //    }

    //    public RflArray(nint native) : base(native)
    //    {
    //        instance = (__Internal*)native;
    //    }

    //    protected override void DisposeNativeResources()
    //    {
    //        Clear();
    //    }

    //    public void Clear()
    //    {
    //        for (int i = 0; i < Count; i++)
    //            iso.ReleaseUnmanaged(instance->buffer[i]);

    //        if (instance->buffer != null)
    //            Memory.SDKAllocator.Free((nint)instance->buffer);

    //        instance->size = 0;
    //    }

    //    public int Count { get { return (int)instance->size; } }

    //    public T this[int index] {
    //        get { return iso.GetManaged(instance->buffer[index]); }
    //        set {
    //            iso.ReleaseUnmanaged(instance->buffer[index]);
    //            instance->buffer[index] = iso.GetUnmanaged(value);
    //        }
    //    }

    //    public IEnumerator<T> GetEnumerator() { return new Enumerator(this); }

    //    IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
    //}

}
