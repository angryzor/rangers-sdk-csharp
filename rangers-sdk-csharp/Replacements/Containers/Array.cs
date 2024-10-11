using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe interface ArrayInternalProvider<U> where U : unmanaged
    {
        public U* Buffer { get; set; }
        public uint Length { get; set; }
        public uint Capacity { get; set; }
        public nint Allocator { get; set; }
    }

    public unsafe abstract partial class Array<T, U, Iso, I> : RefTypeReplacement<I>, IList<T>
        where U : unmanaged
        where Iso : InteropIsomorphism<T, U>, new()
        where I : unmanaged, ArrayInternalProvider<U>
    {
        public class Enumerator : IEnumerator<T>
        {
            private Array<T, U, Iso, I> arr;
            private int index;

            public Enumerator(Array<T, U, Iso, I> arr)
            {
                this.arr = arr;
                index = -1;
            }

            public T Current
            {
                get { return arr[index]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return ++index < arr.Count;
            }

            public void Reset()
            {
                index = -1;
            }

            public void Dispose()
            {
            }
        }

        private Iso iso = new Iso();

        public Array(IAllocator allocator)
        {
        }

        public Array(nint native) : base(native)
        {
        }

        protected override void DisposeNativeResources()
        {
            if (instance->Buffer != null)
            {
                // TODO: Clear
                // TODO: Handle allocator having been changed.
                IAllocator.__GetOrCreateInstance(instance->Allocator).Free((nint)instance->Buffer);
                instance->Buffer = null;
            }

            base.DisposeNativeResources();
        }

        public int Capacity
        {
            get { return (int)(instance->Capacity & ~Constants.SignBit); }
        }

        public bool IsUninitialized
        {
            get { return (instance->Capacity & Constants.SignBit) != 0; }
        }

        public void Reserve(int len)
        {
            if (Count < Capacity)
            {
                return;
            }

            var allocator = IAllocator.__GetOrCreateInstance(instance->Allocator);
            U* buf = (U*)allocator.Alloc((ulong)(Marshal.SizeOf<T>() * len), 16);

            if (buf == null)
            {
                throw new OutOfMemoryException();
            }

            if (instance->Buffer != null)
            {
                var size = Marshal.SizeOf<T>() * Count;
                var bytes = new byte[size];

                Marshal.Copy((nint)instance->Buffer, bytes, 0, size);
                Marshal.Copy(bytes, 0, (nint)buf, size);
            }

            if (!IsUninitialized)
            {
                allocator.Free((nint)instance->Buffer);
            }

            instance->Capacity = (uint)len;
            instance->Buffer = buf;
        }

        // Implementation of IList
        public int Count
        {
            get { return (int)instance->Length; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return iso.GetManaged(instance->Buffer[index]);
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                iso.ReleaseUnmanaged(instance->Buffer[index]);
                instance->Buffer[index] = iso.GetUnmanaged(value);
            }
        }

        public bool Empty
        {
            get { return Count == 0; }
        }

        public void Add(T item)
        {
            if (Count + 1 >= Capacity)
            {
                Reserve((Count + 1) * 2);
            }

            instance->Buffer[Count] = iso.GetUnmanaged(item);
            instance->Length++;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (Count + 1 >= Capacity)
            {
                Reserve((Count + 1) * 2);
            }

            if (index < Count)
            {
                var size = Marshal.SizeOf<T>() * (Count - index);
                var bytes = new byte[size];
                Marshal.Copy((nint)(&instance->Buffer[index]), bytes, 0, size);
                Marshal.Copy(bytes, 0, (nint)(&instance->Buffer[index + 1]), size);
            }

            instance->Length++;
            instance->Buffer[index] = iso.GetUnmanaged(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public bool Remove(T item)
        {
            int idx = IndexOf(item);

            if (idx >= 0)
            {
                RemoveAt(idx);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = iso.GetManaged(instance->Buffer[i]);
        }
        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (iso.GetManaged(instance->Buffer[i]).Equals(item))
                    return i;

            return -1;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (index < Count - 1)
            {
                var size = Marshal.SizeOf<T>() * (Count - index - 1);
                var bytes = new byte[size];
                iso.ReleaseUnmanaged(instance->Buffer[index]);
                Marshal.Copy((nint)(&instance->Buffer[index + 1]), bytes, 0, size);
                Marshal.Copy(bytes, 0, (nint)(&instance->Buffer[index]), size);
            }

            instance->Length--;
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                iso.ReleaseUnmanaged(instance->Buffer[i]);

            instance->Length = 0;
        }
    }
}
