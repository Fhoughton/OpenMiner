// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Generators.SpecialBlockGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Generators
{
  internal class SpecialBlockGenerator
  {
    private PcgRandom random;
    protected List<Point3D> LightPoints;

    protected virtual Point3D AddBlock(
      MapOld map,
      int i,
      Block blockID,
      Vector2 range,
      int maxDepth,
      ValidatePoint validation,
      int minLight,
      PcgRandom random)
    {
      this.random = random;
      Point3D p = new Point3D();
      Point lightPointRange = this.GetLightPointRange(map, range);
      int num1 = lightPointRange.X <= -1 || lightPointRange.Y <= -1 ? 0 : 100;
      int index = 0;
      int num2 = 0;
      do
      {
        do
        {
          do
          {
            do
            {
              if (num1 > 0)
              {
                index = random.Next(lightPointRange.X, lightPointRange.Y + 1);
                p = this.LightPoints[index];
                p.X += random.Next(10) - 5;
                p.Z += random.Next(10) - 5;
                if (--num1 == 0)
                {
                  num1 = 100;
                  lightPointRange.Y += 10;
                  if (lightPointRange.Y >= this.LightPoints.Count)
                    num1 = 0;
                }
              }
              else
              {
                p.X = random.Next(2, map.MapSize.X - 4);
                p.Z = random.Next(2, map.MapSize.Z - 4);
                p.Y = -random.Next((int) ((double) range.X * (double) map.MapSize.Y), (int) ((double) range.Y * (double) map.MapSize.Y));
                if (++num2 == 200)
                {
                  range.X = MathHelper.Max(range.X - 0.05f, 0.0f);
                  range.Y = MathHelper.Min(range.Y + 0.05f, 1f);
                  num2 = 0;
                }
              }
            }
            while (p.Y <= maxDepth);
            p = map.Clamp(p, 2);
          }
          while (map.GetBlockID(p) != (byte) 0);
          do
            ;
          while (--p.Y > -map.MapSize.Y + 1 && map.GetBlockID(p) == (byte) 0);
        }
        while (!this.IsBlockSuitableForBase(map, p));
        ++p.Y;
      }
      while (validation != null && !validation(p, i));
      map.SetBlockData(p, (byte) blockID, (byte) 0, UpdateBlockMethod.Generation, (short) -1, false);
      if (num1 > 0)
        this.LightPoints.RemoveAt(index);
      return p;
    }

    private Point GetLightPointRange(MapOld map, Vector2 range)
    {
      Point point = new Point(-1, -1);
      if (this.LightPoints != null && this.LightPoints.Count > 0)
      {
        for (int index = 0; index < this.LightPoints.Count; ++index)
        {
          Point3D lightPoint = this.LightPoints[index];
          if (-lightPoint.Y >= (int) ((double) range.X * (double) map.MapSize.Y))
          {
            if (-lightPoint.Y <= (int) ((double) range.Y * (double) map.MapSize.Y))
            {
              if (point.X == -1)
                point.X = index;
              if (point.Y < index)
                point.Y = index;
            }
            else
              break;
          }
        }
      }
      return point;
    }

    protected virtual bool IsBlockSuitableForBase(MapOld map, Point3D p)
    {
      if (this.IsBlockSuitable(map, p))
        return this.HasTwoOtherSameHeightNeighbourSurfaces(map, p);
      return false;
    }

    protected virtual bool IsBlockSuitable(MapOld map, Point3D p)
    {
      Block blockId = (Block) map.GetBlockID(p);
      if (map.GetBlockID(p + Point3D.Up) == (byte) 0 && map.IsSolid((byte) blockId) && (!map.IsPassable((byte) blockId) && !ItemData.IsSubTypeAny(blockId, ItemSubType.Leaves)))
        return blockId != Block.WovenLeaves;
      return false;
    }

    private bool HasTwoOtherSameHeightNeighbourSurfaces(MapOld map, Point3D p)
    {
      int num = 0;
      if (this.IsBlockSuitable(map, p + Point3D.Left))
        ++num;
      if (this.IsBlockSuitable(map, p + Point3D.Right))
        ++num;
      if (num == 2)
        return true;
      if (this.IsBlockSuitable(map, p + Point3D.Forward))
        ++num;
      if (num == 2)
        return true;
      if (this.IsBlockSuitable(map, p + Point3D.Backward))
        ++num;
      return num == 2;
    }
  }
}
