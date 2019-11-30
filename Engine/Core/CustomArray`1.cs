// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.CustomArray`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public class CustomArray<T>
  {
    public T[] Array;
    public int Count;
    public int MaxCount;
    protected float resizeFactor;

    public CustomArray()
      : this(10, 2f)
    {
    }

    public CustomArray(int initialCapacity, float resizeFactor)
    {
      this.resizeFactor = resizeFactor;
      this.Array = new T[initialCapacity];
    }

    public CustomArray(T[] array, int count, bool copy, float resizeFactor)
    {
      this.resizeFactor = resizeFactor;
      if (copy)
      {
        this.Array = new T[count];
        System.Array.Copy((System.Array) array, (System.Array) this.Array, count);
      }
      else
        this.Array = array;
      this.Count = count;
    }

    public CustomArray(CustomArray<T> array, bool copy)
      : this(array.Array, array.Count, copy, array.resizeFactor)
    {
    }

    public void Add(T t)
    {
      if (this.MaxCount != 0 && this.Count >= this.MaxCount)
        return;
      if (this.Count == this.Array.Length)
        this.Expand();
      this.Array[this.Count++] = t;
    }

    public void Remove(T t)
    {
      for (int index1 = 0; index1 < this.Count; ++index1)
      {
        if (this.Array[index1].Equals((object) t))
        {
          for (int index2 = index1; index2 < this.Count - 1; ++index2)
            this.Array[index2] = this.Array[index2 + 1];
          --this.Count;
          break;
        }
      }
    }

    public void RemoveAt(int i)
    {
      if (i < 0 || i >= this.Count)
        return;
      for (int index = i; index < this.Count - 1; ++index)
        this.Array[index] = this.Array[index + 1];
      --this.Count;
    }

    public void Clear()
    {
      this.Count = 0;
    }

    protected virtual void Expand()
    {
      System.Array.Resize<T>(ref this.Array, (int) ((double) this.Count * (double) this.resizeFactor));
    }
  }
}
