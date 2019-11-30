// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.InvaderMan
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Screens;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class InvaderMan : Unlockable
  {
    public InvaderMan(Player player)
      : base(player, ActorType.InvaderMan, "20k points on Total Invaders\n1.75m points on Total Rush.", new GameMode[4]
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
      this.player.TotalInvadersScore += new IntEventHandler(this.OnTotalInvadersScore);
      this.player.TotalRushScore += new IntEventHandler(this.OnTotalRushScore);
    }

    protected override void UnhookEvents()
    {
      this.player.TotalInvadersScore -= new IntEventHandler(this.OnTotalInvadersScore);
      this.player.TotalRushScore -= new IntEventHandler(this.OnTotalRushScore);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.TotalInvadersScore < 20000)
          return this.player.UnlockData.TotalRushScore > 1750000;
        return true;
      }
    }

    private void OnTotalInvadersScore(object sender, IntEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.Value <= this.player.UnlockData.TotalInvadersScore)
        return;
      this.player.UnlockData.TotalInvadersScore = e.Value;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnTotalRushScore(object sender, IntEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.Value <= this.player.UnlockData.TotalRushScore)
        return;
      this.player.UnlockData.TotalRushScore = e.Value;
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
        list.Add(string.Format("Total Invaders Highscore (20,000 needed): {0}", (object) this.player.UnlockData.TotalInvadersScore));
        list.Add(string.Format("Or Total Rush Highscore (1,750,000 needed): {0}", (object) this.player.UnlockData.TotalRushScore));
        return list;
      }
    }

    protected override void Unlock()
    {
      if (this.player.GameInstance != null && !this.player.IsGod)
      {
        PauseMenuScreen pauseMenuScreen = new PauseMenuScreen(GameInstance.Instance, this.player);
        GameInstance.Instance.AddScreen((GameScreen) pauseMenuScreen, this.player);
      }
      base.Unlock();
    }
  }
}
