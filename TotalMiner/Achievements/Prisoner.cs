// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Prisoner
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Prisoner : Unlockable
  {
    public Prisoner(Player player)
      : base(player, ActorType.Prisoner, "Escape to the surface.", new GameMode[1]
      {
        GameMode.DigDeep
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

    protected override void HookEvents()
    {
      this.player.EscapedToSurface += new EventHandler(this.OnEscapedToSurface);
    }

    protected override void UnhookEvents()
    {
      this.player.EscapedToSurface -= new EventHandler(this.OnEscapedToSurface);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.PrisonerUnlocked;
      }
    }

    private void OnEscapedToSurface(object sender, EventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      this.player.UnlockData.PrisonerUnlocked = true;
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
        list.Add(string.Format("Escaped to surface: {0}", (object) this.player.UnlockData.PrisonerUnlocked));
        return list;
      }
    }
  }
}
