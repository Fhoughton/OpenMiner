// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkCacheManagerCacheRemoval
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class ChunkCacheManagerCacheRemoval : IThreadWorkItem
  {
    private MapTM map;

    public string Name
    {
      get
      {
        return "CacheRemoval";
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public void Initialize(MapTM map)
    {
      this.map = map;
    }

    public void Update()
    {
      this.map.ChunkCacheManager.RemoveCachesStaggered(0.0f);
    }
  }
}
