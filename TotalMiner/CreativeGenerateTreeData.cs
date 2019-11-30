// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeGenerateTreeData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.Generators;
using System;

namespace StudioForge.TotalMiner
{
  internal class CreativeGenerateTreeData
  {
    public int TreeCount;
    public bool[] CompsSelected;
    public VegetationGenerator.FloraModelSetup[] TreeModels;

    public CreativeGenerateTreeData()
    {
    }

    public CreativeGenerateTreeData(CreativeGenerateTreeData copy)
    {
      this.TreeCount = copy.TreeCount;
      this.CompsSelected = new bool[copy.CompsSelected.Length];
      copy.CompsSelected.CopyTo((Array) this.CompsSelected, 0);
    }
  }
}
