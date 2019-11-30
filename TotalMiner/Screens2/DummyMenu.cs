// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.DummyMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.API;

namespace StudioForge.TotalMiner.Screens2
{
  internal class DummyMenu : NewGuiMenu
  {
    public override string Name
    {
      get
      {
        return "Dummy";
      }
    }

    public DummyMenu(GameInstance instance, Player player)
      : base((ITMGame) instance, (ITMPlayer) player)
    {
    }
  }
}
