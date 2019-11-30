// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.DictionaryWithKeyArray`2
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public class DictionaryWithKeyArray<TKey, TValue> : Dictionary<TKey, TValue>
  {
    public CustomArray<TKey> KeyArray;

    public DictionaryWithKeyArray()
    {
      this.KeyArray = new CustomArray<TKey>(10, 2f);
    }

    public DictionaryWithKeyArray(int capacity)
      : base(capacity)
    {
      this.KeyArray = new CustomArray<TKey>(capacity, 2f);
    }

    public new void Add(TKey key, TValue value)
    {
      base.Add(key, value);
      this.KeyArray.Add(key);
    }

    public void Remove(TKey key)
    {
      base.Remove(key);
      this.KeyArray.Remove(key);
    }
  }
}
