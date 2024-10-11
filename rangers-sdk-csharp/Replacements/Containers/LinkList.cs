using RangersSDK.Interop;
using RangersSDK.CSLib.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RangersSDK.CSLib.Utility
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct LinkListNode
    {
        internal LinkListNode* PreviousNode { get; set; }
        internal LinkListNode* NextNode { get; set; }
    }

    public unsafe class LinkList<T, Iso> : RefTypeReplacement<LinkList<T, Iso>.__Internal>, ICollection<T> where Iso : InteropIsomorphism<T, nint>, new()
    {
        public class __InteropIsomorphism : InteropIsomorphism<LinkList<T, Iso>, nint>
        {
            public nint GetUnmanaged(LinkList<T, Iso> obj) { return (nint)obj.instance; }
            public LinkList<T, Iso> GetManaged(nint obj) { return new LinkList<T, Iso>(obj); }
            public void ReleaseUnmanaged(nint obj) { }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct __Internal
        {
            internal ulong Count { get; set; }
            internal int NodeOffset { get; set; }
            internal LinkListNode SentinelNode { get; set; }
        }

        Iso iso = new Iso();

        public class Enumerator : IEnumerator<T>
        {
            private LinkList<T, Iso> collection;
            private LinkListNode* currentNode;

            public Enumerator(LinkList<T, Iso> collection)
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

        public LinkList(nint native) : base(native)
        {
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

            iso.ReleaseUnmanaged(GetUnmanagedItem(node));

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
            return GetUnmanagedNode(iso.GetUnmanaged(item));
        }

        T GetItem(LinkListNode* node)
        {
            return iso.GetManaged(GetUnmanagedItem(node));
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
}
