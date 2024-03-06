using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecData<K, T> where T : class
{
    IReadOnlyList<T> All { get; }

    T Get(K id);

    T this[K id] { get; }
}
