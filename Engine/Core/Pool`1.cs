// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Pool`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public class Pool<T> : PoolBase<T> where T : new()
  {
    public Pool()
      : base(2)
    {
    }

    public Pool(int capacity)
      : base(capacity, capacity, false)
    {
    }

    public Pool(int capacity, int expandSize, bool preallocate)
      : base(capacity, expandSize, preallocate)
    {
    }

    protected override T CreateInstance()
    {
      return new T();
    }
  }
}
