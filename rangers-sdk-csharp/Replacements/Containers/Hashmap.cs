using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    public unsafe abstract class HashMap<K, V, KU, VU, KIso, VIso> : RefTypeReplacement<HashMap<K, V, KU, VU, KIso, VIso>.__Internal>, IDictionary<K, V>
        where KU : unmanaged
        where VU : unmanaged
        where KIso : InteropIsomorphism<K, KU>, new()
        where VIso : InteropIsomorphism<V, VU>, new()
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

        private KIso kIso = new KIso();
        private VIso vIso = new VIso();

        protected abstract ulong CalculateHash(KU key);
        protected abstract bool CompareKeys(KU key1, KU key2);

        public HashMap(nint native) : base(native)
        {
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
            var allocator = IAllocator.__GetOrCreateInstance(instance->allocator);
            return (Elem*)allocator.Alloc((ulong)Marshal.SizeOf<Elem>() * count, 16);
        }

        void ReleaseMemory()
        {
            if (!IsUninitialized && instance->allocator != IntPtr.Zero && instance->elements != null)
            {
                var allocator = IAllocator.__GetOrCreateInstance(instance->allocator);
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
                    var allocator = IAllocator.__GetOrCreateInstance(instance->allocator);
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
            KU uKey = kIso.GetUnmanaged(key);

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
                kIso.ReleaseUnmanaged(uKey);
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
                kIso.ReleaseUnmanaged(elem->key);
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
                        kIso.ReleaseUnmanaged(elem->key);
                        elem->hash = hash;
                        elem->key = key;
                        instance->length++;
                        break;
                    }
                }
            }

            vIso.ReleaseUnmanaged(elem->value);
            elem->value = value;
        }

        void Insert(K key, V value)
        {
            InsertUnmanaged(kIso.GetUnmanaged(key), vIso.GetUnmanaged(value));
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
                        collection.kIso.GetManaged(collection.instance->elements[index].key),
                        collection.vIso.GetManaged(collection.instance->elements[index].value)
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
                    return vIso.GetManaged(elem->value);
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

            return elem != null && vIso.GetManaged(elem->value).Equals(item.Value);
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

            if (elem == null || !vIso.GetManaged(elem->value).Equals(item.Value))
                return false;

            elem->hash = INVALID_KEY;
            instance->length--;

            kIso.ReleaseUnmanaged(elem->key);
            vIso.ReleaseUnmanaged(elem->value);

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

            kIso.ReleaseUnmanaged(elem->key);
            vIso.ReleaseUnmanaged(elem->value);

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
                value = vIso.GetManaged(elem->value);
                return true;
            }
        }
    }
}
