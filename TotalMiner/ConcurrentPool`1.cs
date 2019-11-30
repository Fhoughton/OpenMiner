// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ConcurrentPool`1
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ConcurrentPool<T> where T : class
  {
    private List<ConcurrentPool<T>.PoolItem<T>> list = new List<ConcurrentPool<T>.PoolItem<T>>();
    private Action<T> releaseAction;

    public ConcurrentPool()
    {
    }

    public ConcurrentPool(Action<T> releaseAction)
    {
      this.releaseAction = releaseAction;
    }

    public void AddItem(T item)
    {
      if ((object) item == null)
        return;
      lock (this.list)
      {
        this.list.Add(new ConcurrentPool<T>.PoolItem<T>()
        {
          Item = item
        });
        if (this.list.Count <= 1)
          return;
        this.RemoveOld();
      }
    }

    public T GetLatest()
    {
      lock (this.list)
      {
        int count = this.list.Count;
        if (this.list.Count <= 0)
          return default (T);
        ConcurrentPool<T>.PoolItem<T> poolItem = this.list[count - 1];
        ++poolItem.RefCount;
        this.list[count - 1] = poolItem;
        return poolItem.Item;
      }
    }

    public void Release(T item)
    {
      if ((object) item == null)
        return;
      lock (this.list)
      {
        for (int index = this.list.Count - 1; index >= 0; --index)
        {
          if ((object) this.list[index].Item == (object) item)
          {
            ConcurrentPool<T>.PoolItem<T> poolItem = this.list[index];
            if (--poolItem.RefCount == 0)
            {
              if (index < this.list.Count - 1)
              {
                this.list.RemoveAt(index);
                if (this.releaseAction != null)
                  this.releaseAction(item);
              }
              else
                this.list[index] = poolItem;
            }
            else
              this.list[index] = poolItem;
          }
        }
      }
    }

    private void RemoveOld()
    {
      for (int index = this.list.Count - 2; index >= 0; --index)
      {
        if (this.list[index].RefCount == 0)
        {
          T obj = this.list[index].Item;
          if (this.releaseAction != null)
            this.releaseAction(obj);
          this.list.RemoveAt(index);
        }
      }
    }

    private struct PoolItem<T1>
    {
      public T1 Item;
      public int RefCount;
    }
  }
}
