// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandFill
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandFill : CreativeCommandWorkItem
  {
    private GlobalPoint3D min;
    private GlobalPoint3D max;

    public CreativeCommandFill(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      if (this.Op.ClearMarkers)
        this.instance.CreativeModeHelper.RemoveMarkers(this.Op.GamerID, false);
      this.min = this.Op.Min;
      this.max = this.Op.Max;
      if (this.instance.IsInZoneType(this.min, this.max, ZoneType.Spawn, this.Op.GamerID))
        return;
      this.RandomReseed(this.Op);
      byte blockId = this.Op.BlockID;
      int percent = (int) this.Op.Percent;
      GlobalPoint3D xmin = this.Op.XMin;
      GlobalPoint3D xmax = this.Op.XMax;
      Player player = this.instance.GetPlayer(this.Op.GamerID);
      player?.ChangeLog.LogSetRegion(this.instance, player, this.Op.Min, this.Op.Max, (Item) this.Op.BlockID, percent, this.Op.Seed);
      IProgressBar op = (IProgressBar) this.Op;
      op.Reset();
      float increment = 1f / (float) ((this.max.Z - this.min.Z + 1) * (this.max.Y - this.min.Y + 1));
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      for (zero.Y = this.max.Y; zero.Y >= this.min.Y; --zero.Y)
      {
        for (zero.Z = this.min.Z; zero.Z <= this.max.Z; ++zero.Z)
        {
          op.AddProgress(increment);
          if (this.Op.Abort)
            return;
          for (zero.X = this.min.X; zero.X <= this.max.X; ++zero.X)
          {
            if ((zero.X < xmin.X || zero.X > xmax.X || (zero.Y < xmin.Y || zero.Y > xmax.Y) || (zero.Z < xmin.Z || zero.Z > xmax.Z)) && ((percent == 100 || this.random.Next(100) < percent) && (this.Op.ClearMarkers || this.map.GetBlockID(zero) != (byte) 253)))
              this.map.SetBlockData(zero, blockId, (byte) 0, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
          }
        }
      }
      this.map.Commit();
    }
  }
}
