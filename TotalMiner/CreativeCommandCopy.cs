// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandCopy
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Blocks;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandCopy : CreativeCommandWorkItem
  {
    public CreativeCommandCopy(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      if (this.Op.ClearMarkers)
        this.instance.CreativeModeHelper.RemoveMarkers(this.Op.GamerID, false);
      GlobalPoint3D min = this.Op.Min;
      GlobalPoint3D max = this.Op.Max;
      GlobalPoint3D point = this.Op.Point;
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D2 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D3 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D4 = new GlobalPoint3D();
      if (min.X > point.X)
      {
        globalPoint3D2.X = min.X;
        globalPoint3D3.X = max.X;
        globalPoint3D4.X = point.X;
        globalPoint3D1.X = 1;
      }
      else
      {
        globalPoint3D2.X = max.X;
        globalPoint3D3.X = min.X;
        globalPoint3D4.X = point.X + (max.X - min.X);
        globalPoint3D1.X = -1;
      }
      if (min.Y > point.Y)
      {
        globalPoint3D2.Y = min.Y;
        globalPoint3D3.Y = max.Y;
        globalPoint3D4.Y = point.Y;
        globalPoint3D1.Y = 1;
      }
      else
      {
        globalPoint3D2.Y = max.Y;
        globalPoint3D3.Y = min.Y;
        globalPoint3D4.Y = point.Y + (max.Y - min.Y);
        globalPoint3D1.Y = -1;
      }
      if (min.Z > point.Z)
      {
        globalPoint3D2.Z = min.Z;
        globalPoint3D3.Z = max.Z;
        globalPoint3D4.Z = point.Z;
        globalPoint3D1.Z = 1;
      }
      else
      {
        globalPoint3D2.Z = max.Z;
        globalPoint3D3.Z = min.Z;
        globalPoint3D4.Z = point.Z + (max.Z - min.Z);
        globalPoint3D1.Z = -1;
      }
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
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
              this.map.SetBlockData(p2, blockIdAndAux.BlockID, blockIdAndAux.AuxData, UpdateBlockMethod.CreativeHelper, this.Op.GamerID, false);
              if (dataBlock != null)
                this.CopyDataBlock(p2, (Block) blockIdAndAux.BlockID, dataBlock, UpdateBlockMethod.CreativeHelper);
            }
            p1.X += globalPoint3D1.X;
          }
          p1.Z += globalPoint3D1.Z;
        }
        p1.Y += globalPoint3D1.Y;
      }
      this.map.Commit();
    }

    private void CopyDataBlock(
      GlobalPoint3D p,
      Block blockID,
      DataBlock srcBlock,
      UpdateBlockMethod method)
    {
      DataBlock orAddDataBlock = this.map.MapStrategyTM.GetOrAddDataBlock(p, blockID, method, GamerID.Sys1, true);
      if (orAddDataBlock == null)
        return;
      orAddDataBlock.CopyFrom(srcBlock);
      orAddDataBlock.Point = p;
      if (blockID != Block.Sign || !(srcBlock as SignBlock).HasText)
        return;
      this.instance.MapRenderer.SignsChanged(false);
    }
  }
}
