// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.HermesWraith
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner.Achievements
{
  internal class HermesWraith : Unlockable
  {
    public HermesWraith(Player player)
      : base(player, ActorType.HermesWraith, "Unique Content Contributors.", (GameMode[]) null, (GameDifficulty[]) null, (NetworkSessionType[]) null)
    {
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player == null)
          return false;
        return this.player.IsHermes;
      }
    }

    public override bool IsDisplayed
    {
      get
      {
        return true;
      }
    }
  }
}
