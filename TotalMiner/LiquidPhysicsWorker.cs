// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.LiquidPhysicsWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class LiquidPhysicsWorker : TimedThreadWorkItem
  {
    private GameInstance instance;

    public override string Name
    {
      get
      {
        return nameof (LiquidPhysicsWorker);
      }
    }

    public LiquidPhysicsWorker(GameInstance instance, PriorityLevel priority)
      : base(priority, 200)
    {
      this.instance = instance;
    }

    protected override void UpdateCore()
    {
      if (!this.instance.IsMapActiveIgnoreGuide)
        return;
      MapStrategyTM mapStrategyTm = this.instance.MapStrategyTM;
      if (mapStrategyTm == null)
        return;
      mapStrategyTm.UpdateLiquidRemovals();
      mapStrategyTm.UpdateLiquidAdditions();
    }
  }
}
