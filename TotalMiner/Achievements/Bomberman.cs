// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Bomberman
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Bomberman : Unlockable
  {
    public Bomberman(Player player)
      : base(player, ActorType.Bomberman, "Undefined.", new GameMode[3]
      {
        GameMode.DigDeep,
        GameMode.Survival,
        GameMode.Peaceful
      }, new GameDifficulty[4]
      {
        GameDifficulty.Peaceful,
        GameDifficulty.Easy,
        GameDifficulty.Normal,
        GameDifficulty.Legendary
      }, new NetworkSessionType[3]
      {
        NetworkSessionType.Local,
        NetworkSessionType.SystemLink,
        NetworkSessionType.PlayerMatch
      })
    {
    }

    public override bool IsUnlocked
    {
      get
      {
        return true;
      }
    }

    public override bool IsNPC
    {
      get
      {
        return true;
      }
    }
  }
}
