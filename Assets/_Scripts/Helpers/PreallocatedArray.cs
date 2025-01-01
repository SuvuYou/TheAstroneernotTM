using System;
using UnityEngine;

public class PreallocatedArray<T>
{
    private T[] _array; 
    private int _count;

    public int Count => _count;
    public int Capacity => _array.Length;
    public T[] FullArray => _array;

    public PreallocatedArray(int initialSize)
    {
        if (initialSize <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(initialSize));
        
        _array = new T[initialSize];
        _count = 0;
    }

    public void Add(T element)
    {
        if (_count >= _array.Length)
            throw new InvalidOperationException("Array capacity exceeded. Consider resizing.");

        _array[_count++] = element;
    }

    public void AddWithResize(T element)
    {
        if (_count >= _array.Length)
        {
            Resize(_array.Length * 2); 
        }

        _array[_count++] = element;
    }

    public void ResetCount()
    {
        _count = 0;
    }

    public void SetAt(int index, T element)
    {
        if (index < 0 || index >= _array.Length)
            throw new IndexOutOfRangeException($"Index {index} is out of bounds for array of size {_array.Length}.");

        if (index >= _count)
            _count = index + 1;
        
        _array[index] = element;
    }

    public void SetCount(int newCount)
    {
        if (newCount < 0 || newCount > _array.Length)
            throw new ArgumentOutOfRangeException(nameof(newCount), "Count must be between 0 and the array's capacity.");
        
        _count = newCount;
    }

    public ArraySegment<T> GetActiveArraySegment()
    {
        return new ArraySegment<T>(_array, 0, _count);
    }

    public void ProcessActiveArray(Action<T> action)
    {  
        for (int i = 0; i < _count; i++)
        {
            action(_array[i]);
        }
    }

    public void Resize(int newSize)
    {
        if (newSize <= _array.Length)
            throw new ArgumentException("New size must be greater than the current size.");

        T[] newArray = new T[newSize];
        Array.Copy(_array, newArray, _count);
        _array = newArray;
    }
}