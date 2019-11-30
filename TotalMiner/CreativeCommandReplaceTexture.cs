// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandReplaceTexture
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandReplaceTexture : CreativeCommandWorkItem
  {
    public CreativeCommandReplaceTexture(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      if (this.Op.ClearMarkers)
        this.instance.CreativeModeHelper.RemoveMarkers(this.Op.GamerID, false);
      GlobalPoint3D min = this.Op.Min;
      GlobalPoint3D max = this.Op.Max;
      GlobalPoint3D xmin = this.Op.XMin;
      GlobalPoint3D xmax = this.Op.XMax;
      byte blockId = this.Op.BlockID;
      byte blockId1 = this.Op.BlockID1;
      byte blockId2 = this.Op.BlockID2;
      if (this.instance.IsInZoneType(min, max, ZoneType.NoEdit, this.Op.GamerID))
        return;
      this.RandomReseed(this.Op);
      int percent = (int) this.Op.Percent;
      this.instance.GetPlayer(this.Op.GamerID);
      IProgressBar op = (IProgressBar) this.Op;
      op.Reset();
      float increment = 1f / (float) ((max.Z - min.Z + 1) * (max.Y - min.Y + 1));
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
            if ((zero.X < xmin.X || zero.X > xmax.X || (zero.Y < xmin.Y || zero.Y > xmax.Y) || (zero.Z < xmin.Z || zero.Z > xmax.Z)) && (int) this.map.GetBlockID(zero) == (int) blockId)
            {
              byte auxFullData = this.map.GetAuxFullData(zero);
              if ((int) auxFullData >> 4 == (int) blockId1 && (percent == 100 || this.random.Next(100) < percent))
              {
                byte auxData = (byte) (((int) auxFullData & 15) + ((int) blockId2 << 4));
                MapChunk chunk = this.map.SetAuxData(zero, auxData, UpdateBlockMethod.CreativeHelper, GamerID.Sys1, false);
                if (chunk != null && !chunk.IsMeshDirty)
                {
                  chunk.SetChunkFlag(ChunkFlags.MeshDirty);
                  this.map.AddChunkToCommitList(chunk, UpdateBlockMethod.CreativeHelper);
                }
              }
            }
          }
        }
      }
      this.map.Commit();
    }
  }
}
