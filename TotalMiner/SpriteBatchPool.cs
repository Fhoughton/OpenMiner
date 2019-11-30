// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SpriteBatchPool
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class SpriteBatchPool : PoolBase<SpriteBatchSafe>
  {
    public SpriteBatchPool(int capacity)
      : base(capacity, 1, true)
    {
    }

    protected override SpriteBatchSafe CreateInstance()
    {
      return new SpriteBatchSafe(CoreGlobals.GraphicsDevice);
    }
  }
}
