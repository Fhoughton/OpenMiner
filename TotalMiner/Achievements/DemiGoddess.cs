// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.DemiGoddess
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner.Achievements
{
  internal class DemiGoddess : Unlockable
  {
    private DemiGod demiGod;

    public DemiGoddess(Player player, DemiGod demiGod)
      : base(player, ActorType.DemiGoddess, "Unlock DemiGod.", new GameMode[4]
      {
        GameMode.DigDeep,
        GameMode.Survival,
        GameMode.Peaceful,
        GameMode.Creative
      }, new GameDifficulty[4]
      {
        GameDifficulty.Peaceful,
        GameDifficulty.Easy,
        GameDifficulty.Normal,
        GameDifficulty.Legendary
      }, new NetworkSessionType[2]
      {
        NetworkSessionType.PlayerMatch,
        NetworkSessionType.Local
      })
    {
      this.demiGod = demiGod;
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.demiGod != null)
          return this.demiGod.IsUnlocked;
        return false;
      }
    }
  }
}
