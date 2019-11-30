// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandWorkItem
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal abstract class CreativeCommandWorkItem
  {
    protected PcgRandom random = new PcgRandom(new Random().Next());
    public CreativeOperationData Op;
    protected MapTM map;
    protected GameInstance instance;
    protected CreativeModeHelper helper;

    public CreativeCommandWorkItem(GameInstance instance)
    {
      this.instance = instance;
      this.helper = instance.CreativeModeHelper;
    }

    public void Initialize(CreativeOperationData op)
    {
      this.Op = op;
      this.map = op.Map;
    }

    protected void RandomReseed(CreativeOperationData op)
    {
      if (op.Percent >= (byte) 100)
        return;
      this.random.Seed(op.Seed == 0 ? this.random.Next() : op.Seed);
    }

    public void Update()
    {
      this.UpdateCore();
    }

    protected abstract void UpdateCore();
  }
}
