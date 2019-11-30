// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandLine
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandLine : CreativeCommandWorkItem
  {
    private List<GlobalPoint3D> wayPoints = new List<GlobalPoint3D>();

    public CreativeCommandLine(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      this.Op.BlockID1 = (byte) MathHelper.Clamp((float) this.Op.BlockID1, 1f, 16f);
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
        this.GenerateLine(this.wayPoints[index], this.wayPoints[index + 1], false);
        this.Op.Progress += num;
        this.map.Commit();
      }
      this.wayPoints.Clear();
    }

    private void GenerateLine(GlobalPoint3D p1, GlobalPoint3D p2, bool side)
    {
      GlobalPoint3D globalPoint3D1 = GlobalPoint3D.Negate(this.map.MapBound.Min);
      BoxInt mapBound = this.map.MapBound;
      mapBound.Min += globalPoint3D1;
      mapBound.Max += globalPoint3D1;
      Vector3 vector3_1 = new Vector3();
      Vector3 vector3_2 = new Vector3();
      Vector3 vector3_3 = new Vector3();
      Vector3 blockCenter1 = this.map.GetBlockCenter(p1);
      Vector3 blockCenter2 = this.map.GetBlockCenter(p2);
      blockCenter1.X += (float) globalPoint3D1.X;
      blockCenter1.Y += (float) globalPoint3D1.Y;
      blockCenter1.Z += (float) globalPoint3D1.Z;
      blockCenter2.X += (float) globalPoint3D1.X;
      blockCenter2.Y += (float) globalPoint3D1.Y;
      blockCenter2.Z += (float) globalPoint3D1.Z;
      Vector3 vector3_4 = blockCenter2 - blockCenter1;
      GlobalPoint3D point = p1;
      int num1;
      int num2;
      if ((double) vector3_4.X > 0.0)
      {
        num1 = 1;
        num2 = mapBound.Max.X;
        vector3_1.X = (float) (mapBound.Min.X + (point.X + 1));
      }
      else
      {
        num1 = -1;
        num2 = mapBound.Min.X - 1;
        vector3_1.X = (float) (mapBound.Min.X + point.X);
      }
      int num3;
      int num4;
      if ((double) vector3_4.Y > 0.0)
      {
        num3 = 1;
        num4 = mapBound.Max.Y;
        vector3_1.Y = (float) (mapBound.Min.Y + (point.Y + 1));
      }
      else
      {
        num3 = -1;
        num4 = mapBound.Min.Y - 1;
        vector3_1.Y = (float) (mapBound.Min.Y + point.Y);
      }
      int num5;
      int num6;
      if ((double) vector3_4.Z > 0.0)
      {
        num5 = 1;
        num6 = mapBound.Max.Z;
        vector3_1.Z = (float) (mapBound.Min.Z + (point.Z + 1));
      }
      else
      {
        num5 = -1;
        num6 = mapBound.Min.Z - 1;
        vector3_1.Z = (float) (mapBound.Min.Z + point.Z);
      }
      float num7 = this.map.TileSize * 0.5f;
      if ((double) vector3_4.X != 0.0)
      {
        float num8 = 1f / vector3_4.X;
        vector3_2.X = (vector3_1.X + num7 - blockCenter1.X) * num8;
        vector3_3.X = (float) num1 * num8;
      }
      else
        vector3_2.X = float.MaxValue;
      if ((double) vector3_4.Y != 0.0)
      {
        float num8 = 1f / vector3_4.Y;
        vector3_2.Y = (vector3_1.Y - num7 - blockCenter1.Y) * num8;
        vector3_3.Y = (float) num3 * num8;
      }
      else
        vector3_2.Y = float.MaxValue;
      if ((double) vector3_4.Z != 0.0)
      {
        float num8 = 1f / vector3_4.Z;
        vector3_2.Z = (vector3_1.Z + num7 - blockCenter1.Z) * num8;
        vector3_3.Z = (float) num5 * num8;
      }
      else
        vector3_2.Z = float.MaxValue;
      Matrix rotationY = Matrix.CreateRotationY(-1.570796f);
      Vector3 vector3_5 = Vector3.Transform(new Vector3(vector3_4.X, 0.0f, vector3_4.Z), rotationY);
      vector3_5.Normalize();
      float num9 = Vector3.DistanceSquared(blockCenter1, blockCenter2);
      int num10 = 0;
      int blockId1 = (int) this.Op.BlockID1;
      float num11 = (float) (blockId1 + 1) * 0.5f;
      if ((double) num11 < 2.0)
        num11 = 2f;
      while (true)
      {
        Vector3 blockCenter3 = this.map.GetBlockCenter(point);
        if ((double) Vector3.DistanceSquared(blockCenter1, blockCenter3) < (double) num9)
        {
          GlobalPoint3D globalPoint3D2 = point - globalPoint3D1;
          int y = globalPoint3D2.Y;
          for (int index = 0; index < (int) this.Op.BlockID2; ++index)
          {
            this.map.SetBlockData(globalPoint3D2, this.Op.BlockID, (byte) 0, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
            ++globalPoint3D2.Y;
          }
          globalPoint3D2.Y = y;
          if (!side && blockId1 > 1)
          {
            this.GenerateLine(globalPoint3D2, this.map.GetPoint(this.map.GetBlockCenter(globalPoint3D2) + vector3_5 * num11), true);
            if (blockId1 > 2)
              this.GenerateLine(globalPoint3D2, this.map.GetPoint(this.map.GetBlockCenter(globalPoint3D2) - vector3_5 * num11), true);
          }
          if ((double) vector3_2.X < (double) vector3_2.Y)
          {
            if ((double) vector3_2.X < (double) vector3_2.Z)
            {
              point.X += num1;
              if (point.X != num2)
                vector3_2.X += vector3_3.X;
              else
                goto label_21;
            }
            else
            {
              point.Z += num5;
              if (point.Z != num6)
                vector3_2.Z += vector3_3.Z;
              else
                goto label_31;
            }
          }
          else if ((double) vector3_2.Y < (double) vector3_2.Z)
          {
            point.Y += num3;
            if (point.Y != num4)
              vector3_2.Y += vector3_3.Y;
            else
              goto label_34;
          }
          else
          {
            point.Z += num5;
            if (point.Z != num6)
              vector3_2.Z += vector3_3.Z;
            else
              goto label_38;
          }
          ++num10;
        }
        else
          break;
      }
      return;
label_21:
      return;
label_31:
      return;
label_34:
      return;
label_38:;
    }
  }
}
