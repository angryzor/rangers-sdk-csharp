using System;

namespace RangersSDK.Hedgehog.Foundation
{
    public struct HandleBase
    {
        public uint Value;
    }

    public struct Handle<T> where T : RefByHandleObject
    {
        public HandleBase Base;

        //public T Target
        //{
        //    get
        //    {
        //        return T.HandleManager.Get(this);
        //    }

        //    set
        //    {
        //        T.HandleManager.Set(this, value);
        //    }
        //}
    }
}
