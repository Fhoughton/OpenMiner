// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Cowboy
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Cowboy : Unlockable
  {
    public Cowboy(Player player)
      : base(player, ActorType.Cowboy, "Kill 50 enemies while hanging\nfrom a rope.", new GameMode[3]
      {
        GameMode.DigDeep,
        GameMode.Survival,
        GameMode.Peaceful
      }, new GameDifficulty[2]
      {
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
        return this.player.UnlockData.CowboyEnemiesKilled >= 50;
      }
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || !this.player.IsOnRope)
        return;
      ++this.player.UnlockData.CowboyEnemiesKilled;
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
        list.Add(string.Format("Enemies killed while hanging from rope: {0} of 50", (object) this.player.UnlockData.CowboyEnemiesKilled));
        return list;
      }
    }
  }
}
