// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.FloodFiller
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Generators
{
  internal class FloodFiller
  {
    public const int MaxFloodSize = 400;
    private const int maxFloodPoints = 50000;
    public static int CurrentFloodPoints;
    private List<FloodFiller.FloodPoint> points;
    private bool isLightSource;
    private ushort newBlockData;
    private byte block;
    private MapOld map;
    private UpdateBlockMethod method;
    private Point3D currentPoint;
    private List<Point3D> floodPoints;
    private short playerID;

    public short PlayerID
    {
      get
      {
        return this.playerID;
      }
    }

    public Block BlockID
    {
      get
      {
        return (Block) this.block;
      }
    }

    public UpdateBlockMethod Method
    {
      get
      {
        return this.method;
      }
    }

    public void FloodPhysics(
      MapOld mapx,
      Point3D p,
      Block blockID,
      UpdateBlockMethod methodx,
      short playerID)
    {
      this.map = mapx;
      this.currentPoint = p;
      this.block = (byte) blockID;
      this.method = methodx;
      this.playerID = playerID;
      this.floodPoints = new List<Point3D>();
    }

    public void Restart(
      MapOld map,
      Block blockID,
      UpdateBlockMethod method,
      short playerID,
      Point3D[] points)
    {
      this.map = map;
      this.block = (byte) blockID;
      this.method = method;
      this.playerID = playerID;
      this.floodPoints = new List<Point3D>((IEnumerable<Point3D>) points);
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
      if (this.CanFlood(this.map.GetBlockID(this.currentPoint)))
      {
        this.map.SetBlockData(this.currentPoint, this.block, (byte) 0, this.method, this.playerID, false);
        int num = this.CountAirBlocksAroundAndBelow(this.currentPoint);
        if (num > 0)
        {
          if (num > 1 && FloodFiller.CurrentFloodPoints < 50000)
          {
            this.floodPoints.Add(this.currentPoint);
            ++FloodFiller.CurrentFloodPoints;
          }
          Point3D? nextAirBlock = this.GetNextAirBlock(this.currentPoint);
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
          Point3D? nextAirBlock = this.GetNextAirBlock(this.currentPoint);
          if (nextAirBlock.HasValue)
          {
            this.currentPoint = nextAirBlock.Value;
            return false;
          }
        }
        this.floodPoints.RemoveAt(this.floodPoints.Count - 1);
        --FloodFiller.CurrentFloodPoints;
      }
      return true;
    }

    private int CountAirBlocksAroundAndBelow(Point3D p)
    {
      int num = 0;
      --p.X;
      if (p.X >= 0 && this.CanFlood(this.map.GetBlockID(p)))
        ++num;
      p.X += 2;
      if (p.X < this.map.MapSize.X && this.CanFlood(this.map.GetBlockID(p)))
        ++num;
      --p.X;
      --p.Z;
      if (p.Z >= 0 && this.CanFlood(this.map.GetBlockID(p)))
        ++num;
      p.Z += 2;
      if (p.Z < this.map.MapSize.Z && this.CanFlood(this.map.GetBlockID(p)))
        ++num;
      --p.Z;
      --p.Y;
      if (p.Y > -this.map.MapSize.Y && this.CanFlood(this.map.GetBlockID(p)))
        ++num;
      return num;
    }

    private Point3D? GetNextAirBlock(Point3D p)
    {
      --p.Y;
      if (p.Y > -this.map.MapSize.Y && this.CanFlood(this.map.GetBlockID(p)))
        return new Point3D?(p);
      ++p.Y;
      --p.X;
      if (p.X >= 0 && this.CanFlood(this.map.GetBlockID(p)))
        return new Point3D?(p);
      ++p.X;
      --p.Z;
      if (p.Z >= 0 && this.CanFlood(this.map.GetBlockID(p)))
        return new Point3D?(p);
      ++p.Z;
      ++p.X;
      if (p.X < this.map.MapSize.X && this.CanFlood(this.map.GetBlockID(p)))
        return new Point3D?(p);
      --p.X;
      ++p.Z;
      if (p.Z < this.map.MapSize.Z && this.CanFlood(this.map.GetBlockID(p)))
        return new Point3D?(p);
      return new Point3D?();
    }

    public bool FloodFill(
      MapOld mapx,
      Point3D p,
      Block blockID,
      int maxBeforeRollback,
      UpdateBlockMethod methodx,
      short playerID)
    {
      this.map = mapx;
      this.method = methodx;
      this.points = new List<FloodFiller.FloodPoint>(maxBeforeRollback);
      this.block = (byte) blockID;
      this.isLightSource = this.map.IsLightSource(this.block);
      this.newBlockData = this.map.BuildBlockData(this.block, (byte) 0, (byte) 0);
      MapStrategyOld mapStrategy = this.map.MapStrategy;
      try
      {
        if (this.map.IsValidPoint(p))
        {
          mapStrategy?.BeginFlood();
          if (this.isLightSource || this.method != UpdateBlockMethod.Generation)
          {
            this.FloodFill(p);
            foreach (FloodFiller.FloodPoint point in this.points)
            {
              this.map.SetBlockDataInternal(point.Point, point.BlockData);
              this.map.SetBlockData(point.Point, this.block, (byte) 0, UpdateBlockMethod.Flood, playerID, false);
            }
          }
          else
            this.FloodFill(p);
          mapStrategy?.EndFlood(true);
          return true;
        }
      }
      catch (FloodToBigException ex)
      {
        this.Rollback();
      }
      finally
      {
        mapStrategy?.EndFlood(false);
      }
      return false;
    }

    private bool CanFlood(byte blockID)
    {
      if (blockID != (byte) 0 && blockID != (byte) 51 && (blockID != (byte) 115 && blockID != (byte) 52) && (blockID != (byte) 116 && blockID != (byte) 140 && blockID != (byte) 204))
        return this.map.IsIcon(blockID);
      return true;
    }

    private void FloodFill(Point3D p)
    {
      ushort blockData = this.map.GetBlockData(p);
      if (!this.CanFlood((byte) blockData))
        return;
      this.map.SetBlockDataInternal(p, this.newBlockData);
      this.points.Add(new FloodFiller.FloodPoint()
      {
        Point = p,
        BlockData = blockData
      });
      if (this.points.Count == this.points.Capacity)
        throw new FloodToBigException();
      if (p.X > 0)
      {
        --p.X;
        this.FloodFill(p);
        ++p.X;
      }
      if (p.Z > 0)
      {
        --p.Z;
        this.FloodFill(p);
        ++p.Z;
      }
      if (p.X < this.map.MapSize.X - 1)
      {
        ++p.X;
        this.FloodFill(p);
        --p.X;
      }
      if (p.Z < this.map.MapSize.Z - 1)
      {
        ++p.Z;
        this.FloodFill(p);
        --p.Z;
      }
      if (p.Y <= -this.map.MapSize.Y + 2)
        return;
      --p.Y;
      this.FloodFill(p);
    }

    private void Rollback()
    {
      foreach (FloodFiller.FloodPoint point in this.points)
        this.map.SetBlockDataInternal(point.Point, point.BlockData);
    }

    private struct FloodPoint
    {
      public Point3D Point;
      public ushort BlockData;
    }
  }
}
