// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.PoolBase`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public abstract class PoolBase<T>
  {
    public T[] List;
    private Stack<int> unused;
    private int capacity;
    private int expandSize;

    public int UsedCount
    {
      get
      {
        return this.List.Length - this.unused.Count;
      }
    }

    public PoolBase()
      : this(2)
    {
    }

    public PoolBase(int capacity)
      : this(capacity, capacity, false)
    {
    }

    public PoolBase(int capacity, int expandSize, bool preallocate)
    {
      if (capacity < 1)
        capacity = 2;
      if (expandSize < 1)
        expandSize = capacity;
      this.capacity = capacity;
      this.expandSize = expandSize;
      this.unused = new Stack<int>();
      lock (this.unused)
      {
        this.List = new T[capacity];
        for (int index = 0; index < capacity; ++index)
        {
          this.unused.Push(index);
          if (preallocate)
            this.List[index] = this.CreateInstance();
        }
      }
    }

    public int GetNext()
    {
      lock (this.unused)
      {
        if (this.unused.Count == 0)
          this.CreateMore();
        int index = this.unused.Pop();
        if ((object) this.List[index] == null)
          this.List[index] = this.CreateInstance();
        return index;
      }
    }

    public T GetNextItem()
    {
      return this.List[this.GetNext()];
    }

    public void Release(int i)
    {
      lock (this.unused)
      {
        if (i < 0 || i >= this.List.Length || this.unused.Contains(i))
          return;
        this.ReleaseItemCore(this.List[i]);
        this.unused.Push(i);
      }
    }

    public void Release(T t)
    {
      lock (this.unused)
      {
        for (int index = this.List.Length - 1; index >= 0; --index)
        {
          if ((object) this.List[index] != null && this.List[index].Equals((object) t))
          {
            this.ReleaseItemCore(t);
            this.unused.Push(index);
            break;
          }
        }
      }
    }

    protected virtual void ReleaseItemCore(T t)
    {
    }

    public void ReleaseAll()
    {
      lock (this.unused)
      {
        this.unused.Clear();
        for (int index = 0; index < this.capacity; ++index)
        {
          this.unused.Push(index);
          this.List[index] = default (T);
        }
      }
    }

    private void CreateMore()
    {
      this.capacity += this.expandSize;
      T[] objArray = new T[this.capacity];
      Array.Copy((Array) this.List, (Array) objArray, this.List.Length);
      for (int length = this.List.Length; length < objArray.Length; ++length)
        this.unused.Push(length);
      this.List = objArray;
    }

    protected abstract T CreateInstance();
  }
}
