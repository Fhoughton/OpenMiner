// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandCopyToClipboard
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandCopyToClipboard : CreativeCommandWorkItem
  {
    public CreativeCommandCopyToClipboard(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      GlobalPoint3D min = this.Op.Min;
      GlobalPoint3D max = this.Op.Max;
      Player player = this.instance.GetPlayer(this.Op.GamerID);
      GlobalPoint3D pmin;
      GlobalPoint3D pmax;
      this.CropEmptySpace(min, max, out pmin, out pmax);
      Vector3 vector3 = pmin.ToVector3() + (pmax - pmin) * 0.5f;
      int facing = 0;
      GlobalPoint3D copySize = pmax - pmin + GlobalPoint3D.One;
      MapModel clipboardModel = CreativeModeHelper.CreateClipboardModel(this.instance, this.map, pmin, GlobalPoint3D.One, copySize, this.Op.XMin, this.Op.XMax, MapModel.EdgeBufferHalf, facing, true, this.Op.GamerID, (IProgressBar) this.Op);
      player.AddClipboard(clipboardModel, (VoxelModelManager) null);
    }

    private void CropEmptySpace(
      GlobalPoint3D min,
      GlobalPoint3D max,
      out GlobalPoint3D pmin,
      out GlobalPoint3D pmax)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      pmin = new GlobalPoint3D(int.MaxValue, int.MaxValue, int.MaxValue);
      pmax = new GlobalPoint3D(int.MinValue, int.MinValue, int.MinValue);
      for (p.Y = min.Y; p.Y <= max.Y; ++p.Y)
      {
        for (p.Z = min.Z; p.Z <= max.Z; ++p.Z)
        {
          for (p.X = min.X; p.X <= max.X; ++p.X)
          {
            if (this.map.GetBlockID(p) != (byte) 0)
            {
              if (p.X < pmin.X)
                pmin.X = p.X;
              if (p.X > pmax.X)
                pmax.X = p.X;
              if (p.Y < pmin.Y)
                pmin.Y = p.Y;
              if (p.Y > pmax.Y)
                pmax.Y = p.Y;
              if (p.Z < pmin.Z)
                pmin.Z = p.Z;
              if (p.Z > pmax.Z)
                pmax.Z = p.Z;
            }
          }
        }
      }
    }
  }
}
