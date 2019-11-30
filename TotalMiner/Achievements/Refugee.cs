// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Refugee
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Refugee : Unlockable
  {
    public Refugee(Player player)
      : base(player, ActorType.Refugee, "Kill your first remote player over\nXbox Live.", new GameMode[2]
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
    }

    protected override void UnhookEvents()
    {
      this.player.KillCharacter -= new Player.CharacterEventHandler(this.OnKillCharacter);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.RefugeePlayerKills >= 1;
      }
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      Player actor = e.Actor as Player;
      if (actor == null || actor.Gamer.IsLocal)
        return;
      ++this.player.UnlockData.RefugeePlayerKills;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
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
        list.Add(string.Format("Remote players killed: {0} of 1", (object) this.player.UnlockData.RefugeePlayerKills));
        return list;
      }
    }
  }
}
