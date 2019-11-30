// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandTrees
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Generators;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandTrees : CreativeCommandWorkItem
  {
    private GlobalPoint3D min;
    private GlobalPoint3D max;

    public CreativeCommandTrees(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      if (this.Op.ClearMarkers)
        this.instance.CreativeModeHelper.RemoveMarkers(this.Op.GamerID, false);
      this.RandomReseed(this.Op);
      this.min = this.Op.Min;
      --this.min.Y;
      this.max = this.Op.Max;
      GlobalPoint3D xmin = this.Op.XMin;
      GlobalPoint3D xmax = this.Op.XMax;
      CreativeGenerateTreeData data = (CreativeGenerateTreeData) this.Op.Data;
      GlobalPoint3D p = new GlobalPoint3D();
      float num = 1f / (float) data.TreeCount;
      int max = 0;
      if (data.CompsSelected[data.CompsSelected.Length - 1])
      {
        foreach (bool flag in data.CompsSelected)
        {
          if (flag)
            ++max;
        }
      }
      int mapHeight = this.map.MapHeight;
      for (int index = 0; index < data.TreeCount; ++index)
      {
        if (this.Op.Abort)
          return;
        p.X = this.min.X + this.random.Next(this.max.X - this.min.X);
        p.Z = this.min.Z + this.random.Next(this.max.Z - this.min.Z);
        p.Y = (int) this.map.GetHeight(p);
        if (p.Y >= this.min.Y && p.Y <= this.max.Y && (p.X < xmin.X || p.X > xmax.X || (p.Z < xmin.Z || p.Z > xmax.Z) || (p.Y < xmin.Y - 1 || p.Y > xmax.Y)) && BlockData.IsGrassOrDirt((Block) this.map.GetBlockID(p)))
        {
          if (max > 0 && this.random.Next(max) == 0)
          {
            VegetationGenerator.GrassDecoration((Map) this.map, p, 1f, 1, 5, 5, 0.25f, 10, this.Op.Max.Y, this.random, UpdateBlockMethod.Player, this.Op.GamerID, false);
            VegetationGenerator.FlowerDecoration((Map) this.map, p, 1f, 1, 5, 6, 0.5f, this.Op.Max.Y, this.random, UpdateBlockMethod.Player, this.Op.GamerID, false);
          }
          else
            VegetationGenerator.AddTree(this.instance, (Map) this.map, data.TreeModels, p, this.random, UpdateBlockMethod.CreativeHelper, false);
        }
        this.Op.Progress += num;
      }
      this.map.Commit();
    }
  }
}
