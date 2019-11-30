// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandSphere
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandSphere : CreativeCommandWorkItem
  {
    public CreativeCommandSphere(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      this.Op.BlockID1 = (byte) MathHelper.Clamp((float) this.Op.BlockID1, 1f, 150f);
      this.RandomReseed(this.Op);
      byte blockId = this.Op.BlockID;
      int percent = (int) this.Op.Percent;
      int blockId1 = (int) this.Op.BlockID1;
      int num = blockId1 * blockId1;
      GlobalPoint3D point = this.Op.Point;
      GlobalPoint3D globalPoint3D1 = point - blockId1;
      if (globalPoint3D1.Y <= this.map.MapBound.Min.Y)
        globalPoint3D1.Y = this.map.MapBound.Min.Y + 1;
      GlobalPoint3D globalPoint3D2 = point + blockId1;
      GlobalPoint3D xmin = this.Op.XMin;
      GlobalPoint3D xmax = this.Op.XMax;
      Player player = this.instance.GetPlayer(this.Op.GamerID);
      player?.ChangeLog.LogSetSphere(this.instance, player, point, blockId1, (Item) this.Op.BlockID, percent, this.Op.Seed);
      IProgressBar op = (IProgressBar) this.Op;
      op.Reset();
      float increment = 1f / (float) ((globalPoint3D2.Z - globalPoint3D1.Z + 1) * (globalPoint3D2.Y - globalPoint3D1.Y + 1));
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      for (zero.Y = globalPoint3D2.Y; zero.Y >= globalPoint3D1.Y; --zero.Y)
      {
        for (zero.Z = globalPoint3D1.Z; zero.Z <= globalPoint3D2.Z; ++zero.Z)
        {
          op.AddProgress(increment);
          if (this.Op.Abort)
            return;
          for (zero.X = globalPoint3D1.X; zero.X <= globalPoint3D2.X; ++zero.X)
          {
            if ((zero.X < xmin.X || zero.X > xmax.X || (zero.Y < xmin.Y || zero.Y > xmax.Y) || (zero.Z < xmin.Z || zero.Z > xmax.Z)) && ((double) GlobalPoint3D.DistanceSquared(zero, point) <= (double) num && (percent == 100 || this.random.Next(100) < percent)))
              this.map.SetBlockData(zero, blockId, (byte) 0, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
          }
        }
      }
      this.map.Commit();
    }
  }
}
