// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MapLightingByPointThreadedWrapper
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal class MapLightingByPointThreadedWrapper : IThreadWorkItem
  {
    public static StudioForge.Engine.Core.Pool<MapLightingByPointThreadedWrapper> Pool = new StudioForge.Engine.Core.Pool<MapLightingByPointThreadedWrapper>();
    public static StudioForge.Engine.Core.Pool<MapLightingByPointTM> LightingPool = new StudioForge.Engine.Core.Pool<MapLightingByPointTM>();
    private int poolHandle;
    private Map map;
    private GlobalPoint3D point;
    private MapBlock oldBlockData;
    private MapBlock newBlockData;

    public string Name
    {
      get
      {
        return "PointLighting";
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
      Map map,
      int poolHandle,
      GlobalPoint3D point,
      MapBlock oldBlockData,
      MapBlock newBlockData)
    {
      this.poolHandle = poolHandle;
      this.map = map;
      this.point = point;
      this.oldBlockData = oldBlockData;
      this.newBlockData = newBlockData;
    }

    public void Update()
    {
      int i = -1;
      try
      {
        i = MapLightingByPointThreadedWrapper.LightingPool.GetNext();
        MapLightingByPointTM lightingByPointTm = MapLightingByPointThreadedWrapper.LightingPool.List[i];
        lightingByPointTm.Initialize(this.map, this.point, this.oldBlockData, this.newBlockData);
        lightingByPointTm.Update();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(101, ex);
      }
      finally
      {
        if (i >= 0)
          MapLightingByPointThreadedWrapper.LightingPool.Release(i);
        MapLightingByPointThreadedWrapper.Pool.Release(this.poolHandle);
      }
    }
  }
}
