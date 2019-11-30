// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Diablo
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Diablo : Unlockable
  {
    public Diablo(Player player)
      : base(player, ActorType.Diablo, "Kill me, with a Ruby sword.", new GameMode[2]
      {
        GameMode.DigDeep,
        GameMode.Survival
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
        return this.player.UnlockData.DiabloUnlocked;
      }
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.Weapon != Item.RubySword || (e.Actor.IsPlayer || e.Actor.ActorType != ActorType.Diablo) || e.Actor.IsCustomMob)
        return;
      this.player.UnlockData.DiabloUnlocked = true;
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
        list.Add(string.Format("Diablo killed with Ruby Sword: {0}", (object) this.player.UnlockData.DiabloUnlocked));
        return list;
      }
    }
  }
}
