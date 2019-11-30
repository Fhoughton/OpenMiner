// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MapLightingByChunkThreadedWrapper
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal class MapLightingByChunkThreadedWrapper : IThreadWorkItem
  {
    public static StudioForge.Engine.Core.Pool<MapLightingByChunkThreadedWrapper> Pool = new StudioForge.Engine.Core.Pool<MapLightingByChunkThreadedWrapper>();
    public static StudioForge.Engine.Core.Pool<MapLightingByChunkTM> LightingPool = new StudioForge.Engine.Core.Pool<MapLightingByChunkTM>();
    private int poolHandle;
    private Map map;
    private MapChunk chunk;
    private GameInstance instance;
    private bool disableBlockLuminence;

    public string Name
    {
      get
      {
        return "ChunkLighting";
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
        return false;
      }
    }

    public void Initialize(
      GameInstance instance,
      Map map,
      int poolHandle,
      MapChunk chunk,
      bool disableBlockLuminence)
    {
      this.instance = instance;
      this.poolHandle = poolHandle;
      this.map = map;
      this.chunk = chunk;
      this.disableBlockLuminence = disableBlockLuminence;
    }

    public void Update()
    {
      int i = -1;
      try
      {
        i = MapLightingByChunkThreadedWrapper.LightingPool.GetNext();
        MapLightingByChunkTM lightingByChunkTm = MapLightingByChunkThreadedWrapper.LightingPool.List[i];
        lightingByChunkTm.Initialize(this.map, this.chunk, this.disableBlockLuminence);
        lightingByChunkTm.Update();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(63, ex);
      }
      finally
      {
        if (i >= 0)
          MapLightingByChunkThreadedWrapper.LightingPool.Release(i);
        MapLightingByChunkThreadedWrapper.Pool.Release(this.poolHandle);
      }
    }
  }
}
