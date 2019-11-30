// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Entrepreneur
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Entrepreneur : Unlockable
  {
    public Entrepreneur(Player player)
      : base(player, ActorType.Entrepreneur, "Earn 1 Million in Gold.", new GameMode[3]
      {
        GameMode.DigDeep,
        GameMode.Survival,
        GameMode.Peaceful
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
      this.player.ItemTraded += new Player.TradeEventHandler(this.OnItemTraded);
    }

    protected override void UnhookEvents()
    {
      this.player.ItemTraded -= new Player.TradeEventHandler(this.OnItemTraded);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (!this.player.UnlockData.EntrepreneurUnlocked)
          return this.player.UnlockData.EntrepreneurGoldEarned >= 1000000;
        return true;
      }
    }

    private void OnItemTraded(object sender, TradeEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || !e.Sell)
        return;
      this.player.UnlockData.EntrepreneurGoldEarned += e.Value;
      if (!this.IsUnlocked)
        return;
      this.player.UnlockData.EntrepreneurUnlocked = true;
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
        list.Add(string.Format("Gold Earned: {0}", (object) this.player.UnlockData.EntrepreneurGoldEarned));
        return list;
      }
    }
  }
}
