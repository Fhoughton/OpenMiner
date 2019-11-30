// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandWall
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandWall : CreativeCommandWorkItem
  {
    private List<GlobalPoint3D> wayPoints = new List<GlobalPoint3D>();

    public CreativeCommandWall(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      this.Op.BlockID1 = (byte) MathHelper.Clamp((float) this.Op.BlockID1, 1f, 16f);
      this.Op.BlockID2 = (byte) MathHelper.Clamp((float) this.Op.BlockID2, 1f, 16f);
      this.wayPoints.Clear();
      lock (this.map.MapStrategyTM.MarkerBlocks)
      {
        foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.map.MapStrategyTM.MarkerBlocks)
        {
          if (markerBlock.GamerID == this.Op.GamerID)
            this.wayPoints.Add(markerBlock.Point);
        }
      }
      if (this.Op.ClearMarkers)
        this.instance.CreativeModeHelper.RemoveMarkers(this.Op.GamerID, false);
      float num = 1f / (float) (this.wayPoints.Count - 1);
      for (int index = 0; index < this.wayPoints.Count - 1; ++index)
      {
        if (this.Op.Abort)
          return;
        this.GeneratePath(this.wayPoints[index], this.wayPoints[index + 1], -1);
        this.Op.Progress += num;
        this.map.Commit();
      }
      this.wayPoints.Clear();
    }

    private void GeneratePath(GlobalPoint3D p1, GlobalPoint3D p2, int height)
    {
      GlobalPoint3D globalPoint3D1 = GlobalPoint3D.Negate(this.map.MapBound.Min);
      BoxInt mapBound = this.map.MapBound;
      mapBound.Min += globalPoint3D1;
      mapBound.Max += globalPoint3D1;
      Vector2 vector2_1 = new Vector2();
      Vector2 vector2_2 = new Vector2();
      Vector2 vector2_3 = new Vector2();
      Vector3 blockCenter1 = this.map.GetBlockCenter(p1);
      Vector2 vector2_4 = new Vector2(blockCenter1.X, blockCenter1.Z);
      Vector3 blockCenter2 = this.map.GetBlockCenter(p2);
      Vector2 vector2_5 = new Vector2(blockCenter2.X, blockCenter2.Z);
      vector2_4.X += (float) globalPoint3D1.X;
      vector2_4.Y += (float) globalPoint3D1.Z;
      vector2_5.X += (float) globalPoint3D1.X;
      vector2_5.Y += (float) globalPoint3D1.Z;
      Vector2 vector2_6 = vector2_5 - vector2_4;
      GlobalPoint3D point = p1;
      int num1;
      int num2;
      if ((double) vector2_6.X > 0.0)
      {
        num1 = 1;
        num2 = mapBound.Max.X;
        vector2_1.X = (float) (mapBound.Min.X + (point.X + 1));
      }
      else
      {
        num1 = -1;
        num2 = mapBound.Min.X - 1;
        vector2_1.X = (float) (mapBound.Min.X + point.X);
      }
      int num3;
      int num4;
      if ((double) vector2_6.Y > 0.0)
      {
        num3 = 1;
        num4 = mapBound.Max.Z;
        vector2_1.Y = (float) (mapBound.Min.Z + (point.Z + 1));
      }
      else
      {
        num3 = -1;
        num4 = mapBound.Min.Z - 1;
        vector2_1.Y = (float) (mapBound.Min.Z + point.Z);
      }
      if ((double) vector2_6.X != 0.0)
      {
        float num5 = 1f / vector2_6.X;
        vector2_2.X = (vector2_1.X - vector2_4.X) * num5;
        vector2_3.X = (float) num1 * num5;
      }
      else
        vector2_2.X = float.MaxValue;
      if ((double) vector2_6.Y != 0.0)
      {
        float num5 = 1f / vector2_6.Y;
        vector2_2.Y = (vector2_1.Y - vector2_4.Y) * num5;
        vector2_3.Y = (float) num3 * num5;
      }
      else
        vector2_2.Y = float.MaxValue;
      Matrix rotationY = Matrix.CreateRotationY(-1.570796f);
      Vector3 vector3 = Vector3.Transform(new Vector3(vector2_6.X, 0.0f, vector2_6.Y), rotationY);
      vector3.Normalize();
      float num6 = Vector2.DistanceSquared(vector2_4, vector2_5);
      int num7 = 0;
      int blockId1 = (int) this.Op.BlockID1;
      float num8 = (float) (blockId1 + 1) * 0.5f;
      if ((double) num8 < 2.0)
        num8 = 2f;
      while (true)
      {
        Vector3 blockCenter3 = this.map.GetBlockCenter(point);
        if ((double) Vector2.DistanceSquared(vector2_4, new Vector2(blockCenter3.X, blockCenter3.Z)) < (double) num6)
        {
          GlobalPoint3D globalPoint3D2 = point - globalPoint3D1;
          globalPoint3D2.Y = height < 0 ? this.GetHeight(globalPoint3D2) : height;
          if (this.IsValidHeight(globalPoint3D2.Y))
          {
            this.SetPathBlock(globalPoint3D2);
            if (height < 0 && blockId1 > 1)
            {
              this.GeneratePath(globalPoint3D2, this.map.GetPoint(this.map.GetBlockCenter(globalPoint3D2) + vector3 * num8), globalPoint3D2.Y);
              if (blockId1 > 2)
                this.GeneratePath(globalPoint3D2, this.map.GetPoint(this.map.GetBlockCenter(globalPoint3D2) - vector3 * num8), globalPoint3D2.Y);
            }
          }
          if ((double) vector2_2.X < (double) vector2_2.Y)
          {
            point.X += num1;
            if (point.X != num2)
              vector2_2.X += vector2_3.X;
            else
              goto label_15;
          }
          else
          {
            point.Z += num3;
            if (point.Z != num4)
              vector2_2.Y += vector2_3.Y;
            else
              goto label_22;
          }
          ++num7;
        }
        else
          break;
      }
      return;
label_15:
      return;
label_22:;
    }

    private void SetPathBlock(GlobalPoint3D p)
    {
      int blockId2 = (int) this.Op.BlockID2;
      int height = this.GetHeight(p);
      if (this.IsValidHeight(height) && height < p.Y)
      {
        blockId2 += p.Y - height;
        p.Y = height;
      }
      for (int index = 0; index < blockId2; ++index)
      {
        this.map.SetBlockData(p, this.Op.BlockID, (byte) 0, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
        ++p.Y;
      }
      if (p.Y >= this.map.MapBound.Max.Y - 1)
        return;
      byte blockIdNoCache = this.map.GetBlockIDNoCache(p);
      if ((int) blockIdNoCache == (int) this.Op.BlockID || !this.CanOverwrite((Block) blockIdNoCache))
        return;
      int num = (int) this.map.ClearBlock(p, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
    }

    private int GetHeight(GlobalPoint3D p)
    {
      p.Y = (int) this.map.GetHeight(p);
      Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p);
      if (blockIdNoCache == (Block) this.Op.BlockID)
        return this.map.MapBound.Min.Y;
      if (blockIdNoCache == Block.Marker)
      {
        --p.Y;
        blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p);
      }
      for (; !this.CanOverwrite(blockIdNoCache); blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p))
        --p.Y;
      return p.Y;
    }

    private bool IsValidHeight(int y)
    {
      return y > this.map.MapBound.Min.Y;
    }

    private bool CanOverwrite(Block blockID)
    {
      switch (blockID)
      {
        case Block.None:
        case Block.Wood:
        case Block.BirchWood:
          return false;
        default:
          if (!ItemData.IsSubTypeAny(blockID, ItemSubType.Leaves))
            return !this.map.IsBlockIcon((byte) blockID);
          return false;
      }
    }
  }
}
