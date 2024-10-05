using CppSharp.Types.Std.CLI;
using RangersSDK.Csl.Ut;
using RangersSDK.Hh.Game;
using RangersSDK.Hh.Game.Dmenu;
using RangersSDK.Interop;
using RangersSDK.SurfRide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RangersSDK.Csl.Ut
{
    public static class Constants
    {
        internal const uint SignBit = 0x80000000;
    }
    public unsafe interface ArrayInternalProvider<U> where U : unmanaged
    {
        internal U* Buffer { get; set; }
        internal uint Length { get; set; }
        internal uint Capacity { get; set; }
        internal nint Allocator { get; set; }
    }

    public unsafe abstract partial class Array<T, U, Iso, I> : IList<T>, IDisposable
        where U : unmanaged
        where Iso : InteropIsomorphism<T, U>
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

        public I* instance;
        private bool createdByManagedCode;
        private Csl.Fnd.IAllocator allocator;
        private bool disposed = false;

        public Array(RangersSDK.Csl.Fnd.IAllocator allocator)
        {
            this.allocator = allocator;
            this.instance = (I*)allocator.Alloc((ulong)sizeof(I), 8);
            this.createdByManagedCode = true;
        }

        public Array(nint native)
        {
            this.instance = (I*)native;
            this.createdByManagedCode = false;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (this.createdByManagedCode)
                {
                    if (instance->Buffer != null)
                    {
                        // TODO: Handle allocator having been changed.
                        Csl.Fnd.IAllocator.__GetOrCreateInstance(instance->Allocator).Free((nint)instance->Buffer);
                        instance->Buffer = null;
                    }

                    allocator.Free((nint)this.instance);
                }

                disposed = true;
            }
        }

        ~Array()
        {
            Dispose(disposing: false);
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

            var allocator = Csl.Fnd.IAllocator.__GetOrCreateInstance(instance->Allocator);
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

                return Iso.GetManaged(instance->Buffer[index]);
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                Iso.ReleaseUnmanaged(instance->Buffer[index]);
                instance->Buffer[index] = Iso.GetUnmanaged(value);
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

            instance->Buffer[Count] = Iso.GetUnmanaged(item);
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
            instance->Buffer[index] = Iso.GetUnmanaged(item);
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
                array[arrayIndex + i] = Iso.GetManaged(instance->Buffer[i]);
        }
        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (Iso.GetManaged(instance->Buffer[i]).Equals(item))
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
                Iso.ReleaseUnmanaged(instance->Buffer[index]);
                Marshal.Copy((nint)(&instance->Buffer[index + 1]), bytes, 0, size);
                Marshal.Copy(bytes, 0, (nint)(&instance->Buffer[index]), size);
            }

            instance->Length--;
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                Iso.ReleaseUnmanaged(instance->Buffer[i]);

            instance->Length = 0;
        }
    }




    public unsafe partial class MoveArray<T, U, Iso> : Array<T, U, Iso, MoveArray<T, U, Iso>.__Internal>
        where U : unmanaged
        where Iso : InteropIsomorphism<T, U>
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal : ArrayInternalProvider<U>
        {
            public U* Buffer { get; set; }
            private ulong length;
            private ulong capacity;
            public nint Allocator { get; set; }

            public uint Length
            {
                get { return (uint)length; }
                set { length = value; }
            }

            public uint Capacity
            {
                get { return (uint)capacity; }
                set { capacity = value; }
            }
        }

        public MoveArray(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public MoveArray(nint native) : base(native) { }
    }

    public unsafe partial class MoveArray32<T, U, Iso> : Array<T, U, Iso, MoveArray32<T, U, Iso>.__Internal>
        where U : unmanaged
        where Iso : InteropIsomorphism<T, U>
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal : ArrayInternalProvider<U>
        {
            public U* Buffer { get; set; }
            public uint Length { get; set; }
            public uint Capacity { get; set; }
            public nint Allocator { get; set; }
        }

        public MoveArray32(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public MoveArray32(nint native) : base(native) { }
    }


    // LinkLists
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct LinkListNode
    {
        internal LinkListNode* PreviousNode { get; set; }
        internal LinkListNode* NextNode { get; set; }
    }

    public unsafe class LinkList<T> : ICollection<T> where T : CppSharpObject<T>
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal
        {
            internal ulong Count { get; set; }
            internal int NodeOffset { get; set; }
            internal LinkListNode SentinelNode { get; set; }
        }

        public __Internal* instance;

        public class Enumerator : IEnumerator<T>
        {
            private LinkList<T> collection;
            private LinkListNode* currentNode;

            public Enumerator(LinkList<T> collection)
            {
                this.collection = collection;
                currentNode = collection.SentinelNode;
            }

            public T Current
            {
                get { return collection.GetItem(currentNode); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                currentNode = currentNode->NextNode;

                return currentNode != collection.SentinelNode;
            }

            public void Reset()
            {
                currentNode = collection.SentinelNode;
            }

            public void Dispose()
            {
            }
        }

        public LinkList(nint native)
        {
            instance = (__Internal*)native;
        }

        protected LinkListNode* SentinelNode { get { return (LinkListNode*)((nint)instance + 0x10); } }

        void Insert(LinkListNode* node, LinkListNode* newNode)
        {
            instance->Count++;

            LinkListNode* oldNext = node->NextNode;

            newNode->PreviousNode = node;
            newNode->NextNode = oldNext;

            node->NextNode = newNode;
            oldNext->PreviousNode = newNode;
        }

        void Remove(LinkListNode* node)
        {
            instance->Count--;

            CppSharpInteropIsomorphism<T>.ReleaseUnmanaged(GetUnmanagedItem(node));

            node->PreviousNode->NextNode = node->NextNode;
            node->NextNode->PreviousNode = node->PreviousNode;

            node->NextNode = null;
            node->PreviousNode = null;
        }

        LinkListNode* FindNode(T item)
        {
            for (LinkListNode* node = SentinelNode->NextNode; node != SentinelNode; node = node->NextNode)
                if (GetItem(node).Equals(item))
                    return node;

            return null;
        }

        LinkListNode* GetUnmanagedNode(nint item)
        {
            return (LinkListNode*)(item + instance->NodeOffset);
        }

        nint GetUnmanagedItem(LinkListNode* node)
        {
            return (nint)node - instance->NodeOffset;
        }

        LinkListNode* GetNode(T item)
        {
            return GetUnmanagedNode(CppSharpInteropIsomorphism<T>.GetUnmanaged(item));
        }

        T GetItem(LinkListNode* node)
        {
            return CppSharpInteropIsomorphism<T>.GetManaged(GetUnmanagedItem(node));
        }

        public int Count { get { return (int)instance->Count; } }
        public bool IsReadOnly { get { return false; } }

        public void Add(T item)
        {
            Insert(SentinelNode->PreviousNode, GetNode(item));
        }

        public void Clear()
        {
            while (SentinelNode->NextNode != SentinelNode)
                Remove(SentinelNode->NextNode);
        }

        public bool Contains(T item)
        {
            return FindNode(item) != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (LinkListNode* node = SentinelNode->NextNode; node != SentinelNode; node = node->NextNode)
                array[arrayIndex++] = GetItem(node);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Remove(T item)
        {
            LinkListNode* node = FindNode(item);

            if (node != null)
            {
                Remove(node);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // Maps
    public unsafe abstract class HashMap<K, V, KU, VU, KIso, VIso> : IDictionary<K, V>
        where KU : unmanaged
        where VU : unmanaged
        where KIso : InteropIsomorphism<K, KU>
        where VIso : InteropIsomorphism<V, VU>
    {
        const ulong INVALID_KEY = 0xFFFFFFFFFFFFFFFFul;

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct Elem
        {
            internal ulong hash { get; set; }
            internal KU key { get; set; }
            internal VU value { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal
        {
            internal Elem* elements { get; set; }
            internal ulong length { get; set; }
            internal ulong capacity { get; set; }
            internal nint allocator { get; set; }
        }

        public __Internal* instance;

        protected abstract ulong CalculateHash(KU key);
        protected abstract bool CompareKeys(KU key1, KU key2);

        public HashMap(nint native)
        {
            instance = (__Internal*)native;
        }

        ulong CalcResize(ulong capacity)
        {
            ulong result;
            for (result = 16; result < capacity; result *= 2) ;

            if (result < 16)
                result = 16;

            return result;
        }
        Elem* AllocateMemory(ulong count)
        {
            var allocator = Csl.Fnd.IAllocator.__GetOrCreateInstance(instance->allocator);
            return (Elem*)allocator.Alloc((ulong)Marshal.SizeOf<Elem>() * count, 16);
        }

        void ReleaseMemory()
        {
            if (!IsUninitialized && instance->allocator != IntPtr.Zero && instance->elements != null)
            {
                var allocator = Csl.Fnd.IAllocator.__GetOrCreateInstance(instance->allocator);
                allocator.Free((nint)instance->elements);
            }
        }

        bool IsUninitialized { get { return (instance->capacity & Constants.SignBit) != 0; } }

		ulong Capacity { get { return instance->capacity & ~Constants.SignBit; } }

        ulong HashMask { get { return Capacity - 1; } }

		void ResizeTbl(ulong capacity)
		{
			ulong oldCap = Capacity;
			Elem* oldElements = instance->elements;

			instance->capacity = capacity;
			instance->length = 0;

			instance->elements = AllocateMemory(capacity);
            for (ulong i = 0; i < capacity; i++)
                Marshal.WriteByte((nint)instance->elements + (nint)i, 0xFF);

			if (oldElements != null)
			{
				for (ulong i = 0; i < oldCap; i++)
				{
					Elem* elem = &oldElements[i];

					if (elem->hash != INVALID_KEY)
					{
						InsertUnmanaged(elem->key, elem->value);
						//elem->value.~TValue();
					}
				}

				if (instance->allocator != IntPtr.Zero)
                {
                    var allocator = Csl.Fnd.IAllocator.__GetOrCreateInstance(instance->allocator);
                    allocator.Free((nint)oldElements);
                }
			}
		}

		void Reserve(ulong capacity)
		{
			ulong cap = CalcResize(capacity);
			if (Capacity < cap)
				ResizeTbl(cap);
		}

        Elem* Find(K key)
        {
            KU uKey = KIso.GetUnmanaged(key);

            try
            {
                if (instance->elements == null)
                    return null;

                ulong hash = CalculateHash(uKey) & 0x7FFFFFFFFFFFFFFF;
                ulong idx = hash & HashMask;
                Elem* elem = &instance->elements[idx];

                if (elem->hash == INVALID_KEY)
                    return null;

                while (elem->hash != hash || !CompareKeys(elem->key, uKey))
                {
                    idx = (idx + 1) & HashMask;
                    elem = &instance->elements[idx];

                    if (elem->hash == INVALID_KEY)
                        return null;
                }

                return elem;
            }
            finally
            {
                KIso.ReleaseUnmanaged(uKey);
            }
        }

        void InsertUnmanaged(KU key, VU value)
        {
            ulong hash = CalculateHash(key) & 0x7FFFFFFFFFFFFFFF;

            if (instance->length > 0 || Capacity > 0)
            {
                if (2 * instance->length >= Capacity)
                {
                    ResizeTbl(2 * Capacity);
                }
            }
            else
            {
                ResizeTbl(CalcResize(Capacity));
            }

            ulong idx = hash & HashMask;
            Elem* elem = &instance->elements[idx];

            if (elem->hash == INVALID_KEY)
            {
                KIso.ReleaseUnmanaged(elem->key);
                elem->hash = hash;
                elem->key = key;
                instance->length++;
            }
            else
            {
                while (elem->hash != hash || !CompareKeys(elem->key, key))
                {
                    idx = (idx + 1) & HashMask;
                    elem = &instance->elements[idx];

                    if (elem->hash == INVALID_KEY)
                    {
                        KIso.ReleaseUnmanaged(elem->key);
                        elem->hash = hash;
                        elem->key = key;
                        instance->length++;
                        break;
                    }
                }
            }

            VIso.ReleaseUnmanaged(elem->value);
            elem->value = value;
        }

        void Insert(K key, V value)
        {
            InsertUnmanaged(KIso.GetUnmanaged(key), VIso.GetUnmanaged(value));
        }

        // Implementation of IDictionary
        public class Enumerator : IEnumerator<KeyValuePair<K, V>>
        {
            private HashMap<K, V, KU, VU, KIso, VIso> collection;
            private int index;

            public Enumerator(HashMap<K, V, KU, VU, KIso, VIso> collection)
            {
                this.collection = collection;
                this.index = -1;
            }

            public KeyValuePair<K, V> Current
            {
                get {
                    return new KeyValuePair<K, V>(
                        KIso.GetManaged(collection.instance->elements[index].key),
                        VIso.GetManaged(collection.instance->elements[index].value)
                    );
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                for (index++; index < (int)collection.Capacity; index++)
                {
                    if (collection.instance->elements[index].hash != INVALID_KEY)
                        break;
                }

                return index < (int)collection.Capacity;
            }

            public void Reset()
            {
                index = -1;
            }

            public void Dispose()
            {
            }
        }

        public int Count { get { return (int)instance->length; } }
        public bool IsReadOnly { get { return false; } }

        public V this[K key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                Elem* elem = Find(key);

                if (elem == null)
                    throw new KeyNotFoundException();
                else
                    return VIso.GetManaged(elem->value);
            }

            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                Insert(key, value);
            }
        }

        public ICollection<K> Keys { get { return null; } }
        public ICollection<V> Values { get { return null; } }

        public void Add(KeyValuePair<K, V> item)
        {
            Insert(item.Key, item.Value);
        }

        public void Add(K key, V value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (Find(key) == null)
                throw new ArgumentException();

            Insert(key, value);
        }

        public void Clear()
        {
            instance->length = 0;

            for (ulong i = 0; i < Capacity; ++i)
            {
                Elem* element = &instance->elements[i];

                //if (element->hash != INVALID_KEY)
                //    element->value.~TValue();

                element->hash = INVALID_KEY;
            }
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            Elem* elem = Find(item.Key);

            return elem != null && VIso.GetManaged(elem->value).Equals(item.Value);
        }

        public bool ContainsKey(K key)
        {
            return Find(key) != null;
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            var enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                array[arrayIndex++] = enumerator.Current;
            }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            Elem* elem = Find(item.Key);

            if (elem == null || !VIso.GetManaged(elem->value).Equals(item.Value))
                return false;

            elem->hash = INVALID_KEY;
            instance->length--;

            KIso.ReleaseUnmanaged(elem->key);
            VIso.ReleaseUnmanaged(elem->value);

            return true;
        }

        public bool Remove(K key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            Elem* elem = Find(key);

            if (elem == null)
                return false;

            elem->hash = INVALID_KEY;
            instance->length--;

            KIso.ReleaseUnmanaged(elem->key);
            VIso.ReleaseUnmanaged(elem->value);

            return true;
        }

        public bool TryGetValue(K key, out V value)
        {
            Elem* elem = Find(key);

            if (elem == null)
            {
                value = default;
                return false;
            }
            else
            {
                value = VIso.GetManaged(elem->value);
                return true;
            }
        }
    }

    public unsafe partial class StringMap<V, U, Iso> : HashMap<string, V, nint, U, AnsiStringInteropIsomorphism, Iso>
        where U : unmanaged
        where Iso : InteropIsomorphism<V, U>
    {
        public StringMap(nint native) : base(native) { }

        protected override ulong CalculateHash(nint key)
        {
            if (key == IntPtr.Zero)
                return 0;

            string str = Marshal.PtrToStringAnsi(key);

            if (str.Length == 0)
                return 0;

            ulong hash = 0;

            for (int i = 1; i < str.Length; i++)
                hash = 31 * hash + str[i];

            return hash;
        }

        protected override bool CompareKeys(nint key1, nint key2)
        {
            string str1 = Marshal.PtrToStringAnsi(key1);
            string str2 = Marshal.PtrToStringAnsi(key2);

            return str1 == str2;
        }
    }

    public unsafe partial class PointerMapBase<K, V, U, KIso, VIso> : HashMap<K, V, nint, U, KIso, VIso>
        where U : unmanaged
        where KIso : InteropIsomorphism<K, nint>
        where VIso : InteropIsomorphism<V, U>
    {
        public PointerMapBase(nint native) : base(native) { }

        protected override ulong CalculateHash(nint key)
        {
            ulong k = (ulong)key;

            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdul;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53ul;
            k ^= k >> 33;

            return k;
        }

        protected override bool CompareKeys(nint key1, nint key2)
        {
            return key1 == key2;
        }
    }

    public unsafe partial class PointerMap<K, V, U, Iso> : PointerMapBase<K, V, U, CppSharpInteropIsomorphism<K>, Iso>
        where K : CppSharpObject<K>
        where U : unmanaged
        where Iso : InteropIsomorphism<V, U>
    {
        public PointerMap(nint native) : base(native) { }
    }

    public unsafe partial class NakedPointerMap<V, U, Iso> : PointerMapBase<nint, V, U, IdentityInteropIsomorphism<nint>, Iso>
        where U : unmanaged
        where Iso : InteropIsomorphism<V, U>
    {
        public NakedPointerMap(nint native) : base(native) { }
    }


    public unsafe partial class VariableString
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct __Internal
        {
            internal IntPtr Buffer { get; set; }
            internal IntPtr Allocator { get; set; }
        }
    }


    public unsafe partial class UnmanagedMoveArray<T> : MoveArray<T, T, IdentityInteropIsomorphism<T>>, CppSharpObject<UnmanagedMoveArray<T>> where T : unmanaged
    {
        public UnmanagedMoveArray(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public UnmanagedMoveArray(nint native) : base(native) { }
        public static UnmanagedMoveArray<T> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UnmanagedMoveArray<T>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class ManagedMoveArray<T> : MoveArray<T, nint, CppSharpInteropIsomorphism<T>>, CppSharpObject<ManagedMoveArray<T>> where T : CppSharpObject<T>
    {
        public ManagedMoveArray(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public ManagedMoveArray(nint native) : base(native) { }
        public static ManagedMoveArray<T> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new ManagedMoveArray<T>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class UnmanagedInplaceMoveArray<T> : MoveArray<T, T, IdentityInteropIsomorphism<T>>, CppSharpObject<UnmanagedInplaceMoveArray<T>> where T : unmanaged
    {
        public UnmanagedInplaceMoveArray(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public UnmanagedInplaceMoveArray(nint native) : base(native) { }
        public static UnmanagedInplaceMoveArray<T> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UnmanagedInplaceMoveArray<T>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class ManagedInplaceMoveArray<T> : MoveArray<T, nint, CppSharpInteropIsomorphism<T>>, CppSharpObject<ManagedInplaceMoveArray<T>> where T : CppSharpObject<T>
    {
        public ManagedInplaceMoveArray(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public ManagedInplaceMoveArray(nint native) : base(native) { }
        public static ManagedInplaceMoveArray<T> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new ManagedInplaceMoveArray<T>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class UnmanagedMoveArray32<T> : MoveArray32<T, T, IdentityInteropIsomorphism<T>>, CppSharpObject<UnmanagedMoveArray32<T>> where T : unmanaged
    {
        public UnmanagedMoveArray32(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public UnmanagedMoveArray32(nint native) : base(native) { }
        public static UnmanagedMoveArray32<T> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UnmanagedMoveArray32<T>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class ManagedMoveArray32<T> : MoveArray32<T, nint, CppSharpInteropIsomorphism<T>>, CppSharpObject<ManagedMoveArray32<T>> where T : CppSharpObject<T>
    {
        public ManagedMoveArray32(Csl.Fnd.IAllocator allocator) : base(allocator) { }
        public ManagedMoveArray32(nint native) : base(native) { }
        public static ManagedMoveArray32<T> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new ManagedMoveArray32<T>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class ManagedStringMap<V> : StringMap<V, nint, CppSharpInteropIsomorphism<V>>, CppSharpObject<ManagedStringMap<V>> where V : CppSharpObject<V>
    {
        public ManagedStringMap(nint native) : base(native) { }
        public static ManagedStringMap<V> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new ManagedStringMap<V>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class UnmanagedStringMap<V> : StringMap<V, V, IdentityInteropIsomorphism<V>>, CppSharpObject<UnmanagedStringMap<V>> where V : unmanaged
    {
        public UnmanagedStringMap(nint native) : base(native) { }
        public static UnmanagedStringMap<V> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UnmanagedStringMap<V>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }

    public unsafe partial class ManagedPointerMap<K, V> : PointerMap<K, V, nint, CppSharpInteropIsomorphism<V>>, CppSharpObject<ManagedPointerMap<K, V>>
        where K : CppSharpObject<K>
        where V : CppSharpObject<V>

    {
        public ManagedPointerMap(nint native) : base(native) { }
        public static ManagedPointerMap<K, V> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new ManagedPointerMap<K, V>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }

    public unsafe partial class UnmanagedPointerMap<K, V> : PointerMap<K, V, V, IdentityInteropIsomorphism<V>>, CppSharpObject<UnmanagedPointerMap<K, V>>
        where K : CppSharpObject<K>
        where V : unmanaged

    {
        public UnmanagedPointerMap(nint native) : base(native) { }
        public static UnmanagedPointerMap<K, V> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UnmanagedPointerMap<K, V>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class ManagedNakedPointerMap<V> : NakedPointerMap<V, nint, CppSharpInteropIsomorphism<V>>, CppSharpObject<ManagedNakedPointerMap<V>> where V : CppSharpObject<V>
    {
        public ManagedNakedPointerMap(nint native) : base(native) { }
        public static ManagedNakedPointerMap<V> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new ManagedNakedPointerMap<V>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class UnmanagedNakedPointerMap<V> : NakedPointerMap<V, V, IdentityInteropIsomorphism<V>>, CppSharpObject<UnmanagedNakedPointerMap<V>> where V : unmanaged
    {
        public UnmanagedNakedPointerMap(nint native) : base(native) { }
        public static UnmanagedNakedPointerMap<V> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UnmanagedNakedPointerMap<V>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }

    public unsafe partial class Pair<F, S, FU, SU, FIso, SIso>
        where FU : unmanaged
        where SU : unmanaged
        where FIso : InteropIsomorphism<F, FU>
        where SIso : InteropIsomorphism<S, SU>
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct __Internal
        {
            internal FU First { get; set; }
            internal SU Second { get; set; }
        }

        public __Internal* instance;

        public Pair(nint native)
        {
            instance = (__Internal*)native;
        }

        public F First
        {
            get { return FIso.GetManaged(instance->First); }
            set {
                FIso.ReleaseUnmanaged(instance->First);
                instance->First = FIso.GetUnmanaged(value);
            }
        }

        public S Second
        {
            get { return SIso.GetManaged(instance->Second); }
            set
            {
                SIso.ReleaseUnmanaged(instance->Second);
                instance->Second = SIso.GetUnmanaged(value);
            }
        }
    }

    public unsafe partial class UUPair<F, S> : Pair<F, S, F, S, IdentityInteropIsomorphism<F>, IdentityInteropIsomorphism<S>>, CppSharpObject<UUPair<F, S>> where F : unmanaged where S : unmanaged
    {
        public UUPair(nint native) : base(native) {}
        public static UUPair<F, S> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UUPair<F, S>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class UMPair<F, S> : Pair<F, S, F, nint, IdentityInteropIsomorphism<F>, CppSharpInteropIsomorphism<S>>, CppSharpObject<UMPair<F, S>> where F : unmanaged where S : CppSharpObject<S>
    {
        public UMPair(nint native) : base(native) { }
        public static UMPair<F, S> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new UMPair<F, S>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class MUPair<F, S> : Pair<F, S, nint, S, CppSharpInteropIsomorphism<F>, IdentityInteropIsomorphism<S>>, CppSharpObject<MUPair<F, S>> where F : CppSharpObject<F> where S : unmanaged
    {
        public MUPair(nint native) : base(native) { }
        public static MUPair<F, S> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new MUPair<F, S>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }
    public unsafe partial class MMPair<F, S> : Pair<F, S, nint, nint, CppSharpInteropIsomorphism<F>, CppSharpInteropIsomorphism<S>>, CppSharpObject<MMPair<F, S>> where F : CppSharpObject<F> where S : CppSharpObject<S>
    {
        public MMPair(nint native) : base(native) { }
        public static MMPair<F, S> __GetOrCreateInstance(nint native, bool saveInstance = false, bool skipVTables = false) { return new MMPair<F, S>(native); }
        public nint __Instance { get { return (nint)instance; } }
    }

    public unsafe partial class String : UnmanagedMoveArray32<byte>
    {
        public String(nint native) : base(native) { }
    }
    public unsafe partial class InplaceBitArray : UnmanagedInplaceMoveArray<ulong>
    {
        public InplaceBitArray(nint native) : base(native) { }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Color8
    {
        uint rgba;

        // Doing it like this to force alignment since there is no alignas equivalent afaik.
        public byte a { get { return (byte)((rgba >>  0) & 0xFFu); } set { rgba = (rgba & 0xFFFFFF00u) | ((uint)value <<  0); } }
        public byte b { get { return (byte)((rgba >>  8) & 0xFFu); } set { rgba = (rgba & 0xFFFF00FFu) | ((uint)value <<  8); } }
        public byte g { get { return (byte)((rgba >> 16) & 0xFFu); } set { rgba = (rgba & 0xFF00FFFFu) | ((uint)value << 16); } }
        public byte r { get { return (byte)((rgba >> 24) & 0xFFu); } set { rgba = (rgba & 0x00FFFFFFu) | ((uint)value << 24); } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Colorf
    {
        public float a;
        public float b;
        public float g;
        public float r;
    }
}
