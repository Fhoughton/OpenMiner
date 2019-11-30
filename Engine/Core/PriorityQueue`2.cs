// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.PriorityQueue`2
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.Collections.Generic;
using System.Linq;

namespace StudioForge.Engine.Core
{
  public class PriorityQueue<P, T>
  {
    private SortedDictionary<P, Queue<T>> list = new SortedDictionary<P, Queue<T>>();

    public void Enqueue(P priority, T value)
    {
      Queue<T> objQueue;
      if (!this.list.TryGetValue(priority, out objQueue))
      {
        objQueue = new Queue<T>();
        this.list.Add(priority, objQueue);
      }
      objQueue.Enqueue(value);
    }

    public T Dequeue()
    {
      KeyValuePair<P, Queue<T>> keyValuePair = this.list.First<KeyValuePair<P, Queue<T>>>();
      T obj = keyValuePair.Value.Dequeue();
      if (keyValuePair.Value.Count == 0)
        this.list.Remove(keyValuePair.Key);
      return obj;
    }

    public bool IsEmpty
    {
      get
      {
        return !this.list.Any<KeyValuePair<P, Queue<T>>>();
      }
    }
  }
}
