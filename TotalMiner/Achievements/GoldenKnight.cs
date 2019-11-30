// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.GoldenKnight
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class GoldenKnight : Unlockable
  {
    public GoldenKnight(Player player)
      : base(player, ActorType.GoldenKnight, "Have 300 remote visitors\nrate your world.", new GameMode[4]
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
      }, new NetworkSessionType[1]
      {
        NetworkSessionType.PlayerMatch
      })
    {
    }

    protected override void HookEvents()
    {
      this.player.WorldRated += new Player.PlayerEventHandler(this.OnWorldRated);
    }

    protected override void UnhookEvents()
    {
      this.player.WorldRated -= new Player.PlayerEventHandler(this.OnWorldRated);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.GoldenKnightUnlocked;
      }
    }

    private void OnWorldRated(object sender, PlayerEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || ++this.player.UnlockData.GoldenKnightRatesReceived < 300)
        return;
      this.player.UnlockData.GoldenKnightUnlocked = true;
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
        list.Add(string.Format("Remote Visitors: {0} of 300", (object) this.player.UnlockData.GoldenKnightRatesReceived));
        return list;
      }
    }
  }
}
