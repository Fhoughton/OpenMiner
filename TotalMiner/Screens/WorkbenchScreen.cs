// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.WorkbenchScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.Screens
{
  internal class WorkbenchScreen : CraftingScreen
  {
    public WorkbenchScreen(GameInstance instance, Player player)
      : base(instance, player)
    {
      this.gridsize = 3;
    }

    protected override void OnScreenClosed()
    {
      base.OnScreenClosed();
      this.player.Raise_WorkBenchClosed();
    }
  }
}
