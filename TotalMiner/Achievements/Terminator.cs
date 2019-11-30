// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Terminator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Terminator : Unlockable
  {
    public Terminator(Player player)
      : base(player, ActorType.Terminator, "Kill 200 enemies with the Grenade\nLauncher.", new GameMode[2]
      {
        GameMode.DigDeep,
        GameMode.Survival
      }, new GameDifficulty[3]
      {
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
        return this.player.UnlockData.TerminatorEnemiesKilled >= 200;
      }
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.Actor == null || (e.Actor.IsPlayer || e.Actor.AITarget != this.player) || e.Weapon != Item.Grenade && e.Weapon != Item.GrenadeLauncher)
        return;
      ++this.player.UnlockData.TerminatorEnemiesKilled;
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
        list.Add(string.Format("Enemies killed with grenade launcher: {0} of 200", (object) this.player.UnlockData.TerminatorEnemiesKilled));
        return list;
      }
    }
  }
}
