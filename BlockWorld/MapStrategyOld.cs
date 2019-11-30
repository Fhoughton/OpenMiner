// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapStrategyOld
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public abstract class MapStrategyOld
  {
    protected MapOld map;
    public readonly List<MapStrategyOld.MarkerBlock> Markers;
    public readonly bool IsRemote;

    protected MapStrategyOld(bool isRemote)
    {
      this.IsRemote = isRemote;
      this.Markers = new List<MapStrategyOld.MarkerBlock>();
    }

    public void UnloadContent()
    {
      this.UnloadContentCore();
      this.Markers.Clear();
      this.map = (MapOld) null;
    }

    public virtual void UnloadContentCore()
    {
    }

    public virtual void Begin(MapOld map)
    {
      this.map = map;
      this.Markers.Clear();
      this.BeginCore();
    }

    public virtual ClearBlockResult GetClearBlockResult(
      Point3D p,
      byte blockID,
      UpdateBlockMethod method,
      short playerID)
    {
      return ClearBlockResult.Success;
    }

    protected virtual void BeginCore()
    {
    }

    public abstract void MapCleared(ushort blockData);

    public abstract void BlockChanged(
      Point3D p,
      ushort oldBlockData,
      ushort newBlockData,
      UpdateBlockMethod method,
      short playerID,
      bool transmit);

    public abstract void LightChanged(
      Point3D p,
      ushort blockData,
      byte light,
      UpdateBlockMethod method);

    public abstract void BeginFlood();

    public abstract void EndFlood(bool success);

    public abstract void Update();

    public struct MarkerBlock
    {
      public Point3D Point;
      public byte GamerID;
    }
  }
}
