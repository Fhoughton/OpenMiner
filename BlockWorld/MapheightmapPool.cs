// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapheightmapPool
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using StudioForge.Engine.Core;

namespace StudioForge.BlockWorld
{
  public class MapheightmapPool : Pool<MapHeightmap>
  {
    private Map map;

    public MapheightmapPool(Map map, int capacity)
      : base(capacity)
    {
      this.map = map;
      this.PreallocateHeightMaps();
    }

    private void PreallocateHeightMaps()
    {
      for (int index = 0; index < this.List.Length; ++index)
        this.List[index].Pregenerate(this.map);
    }

    protected override MapHeightmap CreateInstance()
    {
      MapHeightmap instance = base.CreateInstance();
      if (this.map != null)
        instance.Pregenerate(this.map);
      return instance;
    }
  }
}
