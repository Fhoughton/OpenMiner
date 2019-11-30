// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.FloodFill
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class FloodFill : IThreadWorkItem
  {
    private List<GlobalPoint3D> floodPoints = new List<GlobalPoint3D>(500);
    public GamerID PlayerID;
    private PriorityLevel priority;
    private MapTM map;
    private GameInstance instance;
    private MapStrategyTM strategy;
    private byte blockID;
    private int count;
    private int maxDistance;
    private BoxInt mapBound;
    private UpdateBlockMethod method;
    private GlobalPoint3D currentPoint;
    private GlobalPoint3D originalPoint;

    public string Name
    {
      get
      {
        return nameof (FloodFill);
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

    public void Initialize(
      PriorityLevel priority,
      GameInstance instance,
      MapTM map,
      GlobalPoint3D p,
      Block blockID,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      this.map = map;
      this.instance = instance;
      this.strategy = map.MapStrategy as MapStrategyTM;
      this.priority = priority;
      this.count = 0;
      this.currentPoint = p;
      this.originalPoint = p;
      this.blockID = (byte) blockID;
      this.method = method;
      this.PlayerID = playerID;
      this.maxDistance = 75;
      this.mapBound = map.MapBound;
      this.floodPoints.Clear();
    }

    public void Update()
    {
      if (this.instance.IsMapActiveIgnoreGuide)
      {
        int num = 100;
        for (int index = 0; index < num && this.count < int.MaxValue; ++this.count)
        {
          if (this.UpdatePhysicsCore())
          {
            this.map.Commit();
            return;
          }
          ++index;
        }
        this.map.Commit();
      }
      if (this.count >= int.MaxValue)
        return;
      Player player = this.instance.GetPlayer(this.PlayerID);
      if (player != null && player.HasAbortedFloods)
        return;
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, true, this.priority);
    }

    private bool UpdatePhysicsCore()
    {
      if (this.CanFlood(this.currentPoint))
      {
        this.map.SetBlockData(this.currentPoint, this.blockID, (byte) 0, this.method, this.PlayerID, false);
        int num = this.CountAirBlocksAroundAndBelow(this.currentPoint);
        if (num > 0)
        {
          if (num > 1)
            this.floodPoints.Add(this.currentPoint);
          GlobalPoint3D? nextAirBlock = this.GetNextAirBlock(this.currentPoint);
          if (nextAirBlock.HasValue)
          {
            this.currentPoint = nextAirBlock.Value;
            return false;
          }
        }
      }
      while (this.floodPoints.Count > 0)
      {
        this.currentPoint = this.floodPoints[this.floodPoints.Count - 1];
        if (this.map.GetBlockID(this.currentPoint) == (byte) 0)
          return false;
        if (this.CountAirBlocksAroundAndBelow(this.currentPoint) > 0)
        {
          GlobalPoint3D? nextAirBlock = this.GetNextAirBlock(this.currentPoint);
          if (nextAirBlock.HasValue)
          {
            this.currentPoint = nextAirBlock.Value;
            return false;
          }
        }
        this.floodPoints.RemoveAt(this.floodPoints.Count - 1);
      }
      return true;
    }

    private int CountAirBlocksAroundAndBelow(GlobalPoint3D p)
    {
      int num = 0;
      --p.X;
      if (this.CanFlood(p))
        ++num;
      p.X += 2;
      if (this.CanFlood(p))
        ++num;
      --p.X;
      --p.Z;
      if (this.CanFlood(p))
        ++num;
      p.Z += 2;
      if (this.CanFlood(p))
        ++num;
      --p.Z;
      --p.Y;
      if (this.CanFlood(p))
        ++num;
      return num;
    }

    private GlobalPoint3D? GetNextAirBlock(GlobalPoint3D p)
    {
      --p.Y;
      if (this.CanFlood(p))
        return new GlobalPoint3D?(p);
      ++p.Y;
      --p.X;
      if (this.CanFlood(p))
        return new GlobalPoint3D?(p);
      ++p.X;
      --p.Z;
      if (this.CanFlood(p))
        return new GlobalPoint3D?(p);
      ++p.Z;
      ++p.X;
      if (this.CanFlood(p))
        return new GlobalPoint3D?(p);
      --p.X;
      ++p.Z;
      if (this.CanFlood(p))
        return new GlobalPoint3D?(p);
      return new GlobalPoint3D?();
    }

    private bool CanFlood(GlobalPoint3D p)
    {
      if (p.X >= this.mapBound.Min.X && p.X < this.mapBound.Max.X && (p.Y > this.mapBound.Min.Y && p.Y < this.mapBound.Max.Y) && (p.Z >= this.mapBound.Min.Z && p.Z < this.mapBound.Max.Z && (double) GlobalPoint3D.Distance(p, this.originalPoint) <= (double) this.maxDistance))
      {
        Block blockId = (Block) this.map.GetBlockID(p);
        if (blockId == Block.None || this.map.BlockData[(int) blockId].IsIcon || blockId == Block.Fire)
          return !this.strategy.IsInZoneType(p, ZoneType.NoEdit, this.PlayerID);
      }
      return false;
    }
  }
}
