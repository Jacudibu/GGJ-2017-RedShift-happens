using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBuffer<T>
{
    public int IndexOfLastItemAdded
    {
        get
        {
            if (end > 0)
                return end - 1;
            else
                return size - 1;
        }
    }

    private T[] content;
    private int end, start, size;

    public T[] Content
    {
        get
        {
            return content;
        }
        set
        {
            content = value;
        }
    }

    public CircularBuffer(int size)
    {
        this.size = size;
        content = new T[size];
        start = 0;
        end = 0;
    }

    public void Flush()
    {
        content = new T[size];
        start = 0;
        end = 0;
    }

    public void Push(T value)
    {
        if ((end + 1) % size == start)
            start = (start + 1) % size;

        content[end] = value;
        end = (end + 1) % size;
    }

    public T PopEnd()
    {
        if (end != start)
        {
            end = (end - 1 + size) % size;
            return content[end];
        }
        else
        {
            return content[start];
        }
    }

    public T PopStart()
    {
        if (end != start)
        {
            T result = content[start];
            start = (start + 1 + size) % size;
            return result;
        }
        else
        {
            return content[end];
        }
    }
}