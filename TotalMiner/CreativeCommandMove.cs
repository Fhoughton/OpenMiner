// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandMove
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Blocks;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandMove : CreativeCommandWorkItem
  {
    public CreativeCommandMove(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      GlobalPoint3D min = this.Op.Min;
      GlobalPoint3D max1 = this.Op.Max;
      GlobalPoint3D point = this.Op.Point;
      GlobalPoint3D max2 = point + (max1 - min);
      if (this.instance.IsInZoneType(point, max2, ZoneType.Spawn, this.Op.GamerID))
        return;
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D2 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D3 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D4 = new GlobalPoint3D();
      if (min.X > point.X)
      {
        globalPoint3D2.X = min.X;
        globalPoint3D3.X = max1.X;
        globalPoint3D4.X = point.X;
        globalPoint3D1.X = 1;
      }
      else
      {
        globalPoint3D2.X = max1.X;
        globalPoint3D3.X = min.X;
        globalPoint3D4.X = point.X + (max1.X - min.X);
        globalPoint3D1.X = -1;
      }
      if (min.Y > point.Y)
      {
        globalPoint3D2.Y = min.Y;
        globalPoint3D3.Y = max1.Y;
        globalPoint3D4.Y = point.Y;
        globalPoint3D1.Y = 1;
      }
      else
      {
        globalPoint3D2.Y = max1.Y;
        globalPoint3D3.Y = min.Y;
        globalPoint3D4.Y = point.Y + (max1.Y - min.Y);
        globalPoint3D1.Y = -1;
      }
      if (min.Z > point.Z)
      {
        globalPoint3D2.Z = min.Z;
        globalPoint3D3.Z = max1.Z;
        globalPoint3D4.Z = point.Z;
        globalPoint3D1.Z = 1;
      }
      else
      {
        globalPoint3D2.Z = max1.Z;
        globalPoint3D3.Z = min.Z;
        globalPoint3D4.Z = point.Z + (max1.Z - min.Z);
        globalPoint3D1.Z = -1;
      }
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      bool flag = !this.instance.IsFiniteResources;
      p1.Y = globalPoint3D2.Y;
      for (p2.Y = globalPoint3D4.Y; (globalPoint3D1.Y >= 0 ? (p1.Y <= globalPoint3D3.Y ? 1 : 0) : (p1.Y >= globalPoint3D3.Y ? 1 : 0)) != 0; p2.Y += globalPoint3D1.Y)
      {
        p1.Z = globalPoint3D2.Z;
        for (p2.Z = globalPoint3D4.Z; (globalPoint3D1.Z >= 0 ? (p1.Z <= globalPoint3D3.Z ? 1 : 0) : (p1.Z >= globalPoint3D3.Z ? 1 : 0)) != 0; p2.Z += globalPoint3D1.Z)
        {
          p1.X = globalPoint3D2.X;
          for (p2.X = globalPoint3D4.X; (globalPoint3D1.X >= 0 ? (p1.X <= globalPoint3D3.X ? 1 : 0) : (p1.X >= globalPoint3D3.X ? 1 : 0)) != 0; p2.X += globalPoint3D1.X)
          {
            if (this.Op.Abort)
              return;
            MapBlock blockIdAndAux = this.map.GetBlockIDAndAux(p1);
            if (Globals1.ItemData[(int) blockIdAndAux.BlockID].IsEnabled)
            {
              DataBlock dataBlock = this.map.MapStrategyTM.GetDataBlock(p1);
              if (flag || dataBlock == null || !dataBlock.HasInventory)
              {
                BlockData.AdjustBlockDataForMove(ref blockIdAndAux);
                this.map.SetBlockData(p2, blockIdAndAux.BlockID, blockIdAndAux.AuxData, UpdateBlockMethod.CreativeHelper, this.Op.GamerID, false);
                this.map.SetBlockData(p1, (byte) 0, (byte) 0, UpdateBlockMethod.CreativeHelper, this.Op.GamerID, false);
                if (dataBlock != null)
                  this.MoveDataBlock(p2, dataBlock, UpdateBlockMethod.CreativeHelper);
              }
            }
            p1.X += globalPoint3D1.X;
          }
          p1.Z += globalPoint3D1.Z;
        }
        p1.Y += globalPoint3D1.Y;
      }
      this.map.Commit();
    }

    private void MoveDataBlock(GlobalPoint3D p, DataBlock dataBlock, UpdateBlockMethod method)
    {
      dataBlock.Point = p;
      this.map.MapStrategyTM.AddDataBlock(dataBlock, method, false);
      if (dataBlock.ClassType != DataBlockType.Sign)
        return;
      this.instance.MapRenderer.SignsChanged(false);
    }
  }
}
