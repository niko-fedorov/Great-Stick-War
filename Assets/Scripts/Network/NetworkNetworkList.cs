using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class NetworkListList<T> : NetworkList<T>, IList<T> where T : unmanaged, IEquatable<T>
{
    public bool IsReadOnly => false;

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
            array.SetValue(this[i], arrayIndex++);
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
