// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.FloodFillerNew
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Generators
{
  internal class FloodFillerNew
  {
    public const int MaxFloodSize = 400;
    private const int maxFloodPoints = 50000;
    public static int CurrentFloodPoints;
    private List<FloodFillerNew.FloodPoint> points;
    private MapBlock newBlockData;
    private Map map;
    private MapStrategy strategy;
    private GlobalPoint3D currentPoint;
    private List<GlobalPoint3D> floodPoints;
    private UpdateBlockMethod method;
    private bool lightEmitting;
    private GamerID playerID;

    public GlobalPoint3D[] FloodPoints
    {
      get
      {
        lock (this.floodPoints)
          return this.floodPoints.ToArray();
      }
    }

    public void FloodPhysics(Map mapx, GlobalPoint3D p, Block blockID, GamerID playerID)
    {
      this.map = mapx;
      this.strategy = this.map.MapStrategy;
      this.currentPoint = p;
      this.newBlockData.BlockID = (byte) blockID;
      this.playerID = playerID;
      this.floodPoints = new List<GlobalPoint3D>();
    }

    public void Restart(Map map, Block blockID, GamerID playerID, GlobalPoint3D[] points)
    {
      this.map = map;
      this.strategy = map.MapStrategy;
      this.newBlockData.BlockID = (byte) blockID;
      this.playerID = playerID;
      this.floodPoints = new List<GlobalPoint3D>((IEnumerable<GlobalPoint3D>) points);
      this.currentPoint = this.floodPoints[0];
    }

    public bool UpdatePhysics(int iterations)
    {
      for (int index = 0; index < iterations; ++index)
      {
        if (this.UpdatePhysicsCore())
          return true;
      }
      return false;
    }

    private bool UpdatePhysicsCore()
    {
      MapBlock blockData = this.map.GetBlockData(this.currentPoint);
      if (this.CanFlood(this.currentPoint, blockData.BlockID))
      {
        MapChunk mapChunk = this.map.SetBlockDataInternal(this.currentPoint, blockData, this.newBlockData, UpdateBlockMethod.Flood);
        if (mapChunk != null && this.lightEmitting)
          mapChunk.SetChunkFlag(ChunkFlags.LightDirty);
        this.map.AddChunkToCommitList(blockData.Chunk, UpdateBlockMethod.Flood);
        int num = this.CountAirBlocksAroundAndBelow(this.currentPoint);
        if (num > 0)
        {
          if (num > 1 && FloodFillerNew.CurrentFloodPoints < 50000)
          {
            this.floodPoints.Add(this.currentPoint);
            ++FloodFillerNew.CurrentFloodPoints;
          }
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
        --FloodFillerNew.CurrentFloodPoints;
      }
      return true;
    }

    private int CountAirBlocksAroundAndBelow(GlobalPoint3D p)
    {
      int num = 0;
      --p.X;
      if (p.X >= this.map.MapBound.Min.X && this.CanFlood(p, this.map.GetBlockID(p)))
        ++num;
      p.X += 2;
      if (p.X < this.map.MapBound.Max.X && this.CanFlood(p, this.map.GetBlockID(p)))
        ++num;
      --p.X;
      --p.Z;
      if (p.Z >= this.map.MapBound.Min.Z && this.CanFlood(p, this.map.GetBlockID(p)))
        ++num;
      p.Z += 2;
      if (p.Z < this.map.MapBound.Max.Z && this.CanFlood(p, this.map.GetBlockID(p)))
        ++num;
      --p.Z;
      --p.Y;
      if (p.Y > this.map.MapBound.Min.Y && p.Y < this.map.MapBound.Max.Y && this.CanFlood(p, this.map.GetBlockID(p)))
        ++num;
      return num;
    }

    private GlobalPoint3D? GetNextAirBlock(GlobalPoint3D p)
    {
      --p.Y;
      if (p.Y > this.map.MapBound.Min.Y && this.CanFlood(p, this.map.GetBlockID(p)))
        return new GlobalPoint3D?(p);
      ++p.Y;
      --p.X;
      if (p.X >= this.map.MapBound.Min.X && this.CanFlood(p, this.map.GetBlockID(p)))
        return new GlobalPoint3D?(p);
      ++p.X;
      --p.Z;
      if (p.Z >= this.map.MapBound.Min.Z && this.CanFlood(p, this.map.GetBlockID(p)))
        return new GlobalPoint3D?(p);
      ++p.Z;
      ++p.X;
      if (p.X < this.map.MapBound.Max.X && this.CanFlood(p, this.map.GetBlockID(p)))
        return new GlobalPoint3D?(p);
      --p.X;
      ++p.Z;
      if (p.Z < this.map.MapBound.Max.Z && this.CanFlood(p, this.map.GetBlockID(p)))
        return new GlobalPoint3D?(p);
      return new GlobalPoint3D?();
    }

    public bool FloodFill(
      Map mapx,
      GlobalPoint3D p,
      Block blockID,
      int maxBeforeRollback,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      this.map = mapx;
      if (this.map.IsValidPoint(p))
      {
        this.strategy = mapx.MapStrategy;
        this.playerID = playerID;
        this.method = method;
        this.lightEmitting = this.map.IsBlockLightSource((byte) blockID);
        this.points = new List<FloodFillerNew.FloodPoint>(maxBeforeRollback);
        this.newBlockData.BlockID = (byte) blockID;
        if (this.FloodFill(p))
          return true;
        this.Rollback();
      }
      return false;
    }

    private bool CanFlood(GlobalPoint3D p, byte blockID)
    {
      if (blockID != (byte) 0)
        return false;
      if (this.strategy == null)
        return true;
      ClearBlockResult clearBlockResult = this.strategy.GetClearBlockResult(p, this.newBlockData.BlockID, UpdateBlockMethod.Flood, this.playerID, false);
      if (clearBlockResult != ClearBlockResult.Success)
        return clearBlockResult == ClearBlockResult.AlreadyClear;
      return true;
    }

    private bool FloodFill(GlobalPoint3D p)
    {
      MapBlock blockData = this.map.GetBlockData(p);
      if (this.CanFlood(p, blockData.BlockID))
      {
        this.newBlockData.Chunk = blockData.Chunk;
        MapChunk mapChunk = this.map.SetBlockDataInternal(p, blockData, this.newBlockData, this.method);
        if (mapChunk != null && this.lightEmitting)
          mapChunk.SetChunkFlag(ChunkFlags.LightDirty);
        if (this.method != UpdateBlockMethod.Generation)
          this.map.AddChunkToCommitList(blockData.Chunk, this.method);
        this.points.Add(new FloodFillerNew.FloodPoint()
        {
          Point = p,
          BlockData = blockData
        });
        if (this.points.Count == this.points.Capacity)
          return false;
        if (p.X > 0)
        {
          --p.X;
          if (!this.FloodFill(p))
            return false;
          ++p.X;
        }
        if (p.Z > 0)
        {
          --p.Z;
          if (!this.FloodFill(p))
            return false;
          ++p.Z;
        }
        if (p.X < this.map.MapSize.X - 1)
        {
          ++p.X;
          if (!this.FloodFill(p))
            return false;
          --p.X;
        }
        if (p.Z < this.map.MapSize.Z - 1)
        {
          ++p.Z;
          if (!this.FloodFill(p))
            return false;
          --p.Z;
        }
        if (p.Y > -this.map.MapSize.Y + 2)
        {
          --p.Y;
          if (!this.FloodFill(p))
            return false;
        }
      }
      return true;
    }

    private void Rollback()
    {
      foreach (FloodFillerNew.FloodPoint point in this.points)
        this.map.SetBlockDataInternal(point.Point, MapBlock.Empty, point.BlockData, this.method);
    }

    private void LightFill(Block blockID)
    {
      if (!this.map.IsBlockLightSource((byte) blockID))
        return;
      int next = MapLightingByPointThreadedWrapper.Pool.GetNext();
      MapLightingByPointThreadedWrapper pointThreadedWrapper = MapLightingByPointThreadedWrapper.Pool.List[next];
      MapBlock newBlockData = new MapBlock()
      {
        BlockID = (byte) blockID
      };
      foreach (FloodFillerNew.FloodPoint point in this.points)
      {
        pointThreadedWrapper.Initialize(this.map, next, point.Point, point.BlockData, newBlockData);
        pointThreadedWrapper.Update();
      }
    }

    private struct FloodPoint
    {
      public GlobalPoint3D Point;
      public MapBlock BlockData;
    }
  }
}
