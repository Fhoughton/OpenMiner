// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandReplace
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandReplace : CreativeCommandWorkItem
  {
    public CreativeCommandReplace(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      GlobalPoint3D min = this.Op.Min;
      GlobalPoint3D max = this.Op.Max;
      GlobalPoint3D xmin = this.Op.XMin;
      GlobalPoint3D xmax = this.Op.XMax;
      byte blockId = this.Op.BlockID;
      byte blockId1 = this.Op.BlockID1;
      if (this.instance.IsInZoneType(min, max, ZoneType.NoEdit, this.Op.GamerID))
        return;
      this.RandomReseed(this.Op);
      int percent = (int) this.Op.Percent;
      Player player = this.instance.GetPlayer(this.Op.GamerID);
      player?.ChangeLog.LogReplaceRegion(this.instance, player, this.Op.Min, this.Op.Max, (Item) this.Op.BlockID, (Item) this.Op.BlockID1, (int) this.Op.Percent, this.Op.Seed);
      IProgressBar op = (IProgressBar) this.Op;
      op.Reset();
      float num = (float) ((max.Z - min.Z + 1) * (max.Y - min.Y + 1));
      if (this.Op.BlockID == (byte) 253)
      {
        List<StudioForge.TotalMiner.Blocks.MarkerBlock> markerBlocks = this.map.MapStrategyTM.MarkerBlocks;
        float increment = 1f / (float) markerBlocks.Count;
        for (int index = markerBlocks.Count - 1; index >= 0; --index)
        {
          op.AddProgress(increment);
          StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock = markerBlocks[0];
          if (markerBlock.GamerID == this.Op.GamerID)
            this.map.SetBlockData(markerBlock.Point, blockId1, (byte) 0, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
        }
        if (this.Op.ClearMarkers)
          this.instance.CreativeModeHelper.RemoveMarkers(this.Op.GamerID, false);
      }
      else
      {
        if (this.Op.ClearMarkers)
          this.instance.CreativeModeHelper.RemoveMarkers(this.Op.GamerID, false);
        float increment = 1f / num;
        GlobalPoint3D zero = GlobalPoint3D.Zero;
        for (zero.Y = min.Y; zero.Y <= max.Y; ++zero.Y)
        {
          for (zero.Z = min.Z; zero.Z <= max.Z; ++zero.Z)
          {
            op.AddProgress(increment);
            for (zero.X = min.X; zero.X <= max.X; ++zero.X)
            {
              if (this.Op.Abort)
                return;
              if ((zero.X < xmin.X || zero.X > xmax.X || (zero.Y < xmin.Y || zero.Y > xmax.Y) || (zero.Z < xmin.Z || zero.Z > xmax.Z)) && ((int) this.map.GetBlockID(zero) == (int) blockId && (percent == 100 || this.random.Next(100) < percent)))
                this.map.SetBlockData(zero, blockId1, (byte) 0, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
            }
          }
        }
      }
      this.map.Commit();
    }
  }
}
