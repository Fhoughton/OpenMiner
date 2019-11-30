// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Angel
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Angel : Unlockable
  {
    public Angel(Player player)
      : base(player, ActorType.Angel, "Save 15 players from death by killing\nthe enemy preying on them.", new GameMode[3]
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
        return this.player.UnlockData.AngelPlayersSaved >= 15;
      }
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || !(e.Actor is NpcBase))
        return;
      Player target = e.Target as Player;
      if (target == null || target == this.player)
        return;
      ++this.player.UnlockData.AngelPlayersSaved;
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
        list.Add(string.Format("Players saved: {0} of 15", (object) this.player.UnlockData.AngelPlayersSaved));
        return list;
      }
    }
  }
}
