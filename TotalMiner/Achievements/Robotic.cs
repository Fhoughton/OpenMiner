// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Robotic
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Robotic : Unlockable
  {
    public Robotic(Player player)
      : base(player, ActorType.Robotic, "No longer unlockable.", (GameMode[]) null, (GameDifficulty[]) null, (NetworkSessionType[]) null)
    {
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player == null)
          return false;
        return Player.IsRoboticGamerTag(this.player.PlayerIndex, this.player.UnlockData.BadBoy != BadBoyType.None);
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
