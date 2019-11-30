// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Ninja
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Ninja : Unlockable
  {
    public Ninja(Player player)
      : base(player, ActorType.Ninja, "Kill 20 unique remote players without\ndying.", new GameMode[2]
      {
        GameMode.DigDeep,
        GameMode.Survival
      }, new GameDifficulty[3]
      {
        GameDifficulty.Easy,
        GameDifficulty.Normal,
        GameDifficulty.Legendary
      }, new NetworkSessionType[1]
      {
        NetworkSessionType.PlayerMatch
      })
    {
    }

    protected override void HookEvents()
    {
      this.player.KillCharacter += new Player.CharacterEventHandler(this.OnKillCharacter);
      this.player.PlayerDied += new Player.CharacterEventHandler(this.OnPlayerDied);
    }

    protected override void UnhookEvents()
    {
      this.player.KillCharacter -= new Player.CharacterEventHandler(this.OnKillCharacter);
      this.player.PlayerDied -= new Player.CharacterEventHandler(this.OnPlayerDied);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.NinjaKillStreakGamerID.Count >= 20;
      }
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      Player actor = e.Actor as Player;
      if (actor == null || actor.Gamer.IsLocal)
        return;
      int hashCode = actor.Gamertag.GetHashCode();
      if (!this.player.UnlockData.NinjaKillStreakGamerID.Contains(hashCode))
        this.player.UnlockData.NinjaKillStreakGamerID.Add(hashCode);
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnPlayerDied(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || this.player.UnlockData.NinjaKillStreakGamerID.Count >= 20)
        return;
      this.player.UnlockData.NinjaKillStreakGamerID.Clear();
    }

    public override bool HasProgress
    {
      get
      {
        return true;
      }
    }

    public override List<string> ProgressList
    {
      get
      {
        List<string> list = new List<string>();
        this.AddReqsMetProgress(list);
        list.Add(string.Format("Max kill streak of remote players: {0} of 20", (object) this.player.UnlockData.NinjaKillStreakGamerID.Count));
        return list;
      }
    }
  }
}
