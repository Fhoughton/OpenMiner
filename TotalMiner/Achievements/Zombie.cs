// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Zombie
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Zombie : Unlockable
  {
    public Zombie(Player player)
      : base(player, ActorType.Zombie, "Survive the first 5 nights\nwithout sleeping.", new GameMode[1]
      {
        GameMode.Survival
      }, new GameDifficulty[1]{ GameDifficulty.Legendary }, new NetworkSessionType[3]
      {
        NetworkSessionType.Local,
        NetworkSessionType.SystemLink,
        NetworkSessionType.PlayerMatch
      })
    {
    }

    protected override void HookEvents()
    {
      this.player.GameInstance.SunMoon.SunriseEnded += new EventHandler(this.OnSunriseEnded);
    }

    protected override void UnhookEvents()
    {
      this.player.GameInstance.SunMoon.SunriseEnded -= new EventHandler(this.OnSunriseEnded);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.ZombieDaysSurvived >= 5;
      }
    }

    private void OnSunriseEnded(object sender, EventArgs e)
    {
      if (!this.IsUnlockableDifficulty || this.player.GameInstance == null || (!this.player.GameInstance.IsEnabled || Globals2.GameProperties.SaveGame.Header.HoursSlept != 0) || this.player.Statistics.TotalDeaths != 0)
        return;
      ++this.player.UnlockData.ZombieDaysSurvived;
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
        list.Add(string.Format("Days survived without sleeping: {0} of 5", (object) this.player.UnlockData.ZombieDaysSurvived));
        return list;
      }
    }
  }
}
