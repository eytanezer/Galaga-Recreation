using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;


public class SimplePool<TPool, T> : MonoSingleton<SimplePool<TPool,T>> 
    where TPool : SimplePool<TPool,T>
    where T: MonoBehaviour, IPoolable
{
    [SerializeField] T prefab;
    [SerializeField] int initialSize = 10;
    [SerializeField] int increaseSize = 10;
    
    private readonly Stack<T> _available = new();

    protected override void Awake()
    {
        base.Awake();
        IncreasePool(initialSize);
    }

    public T Get()
    {
        if (_available.Count < 1)
        {
            IncreasePool(increaseSize);
        }
        var pooledObject = _available.Pop();
        
        

        // C# Knows this object implements IPoolable!
        pooledObject.Reset();
        // C# Knows this object is a MonoBehaviour!
        pooledObject.gameObject.SetActive(true);
        
        return pooledObject;
    }
  
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _available.Push(obj);
    }
    
    private void IncreasePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var instance = Instantiate(prefab, parent: this.transform);
            Return(instance);
        }
    }
}