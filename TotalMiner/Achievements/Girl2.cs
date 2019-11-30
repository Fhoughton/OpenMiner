// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Girl2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Girl2 : Unlockable
  {
    public Girl2(Player player)
      : base(player, ActorType.Girl2, "Go shopping.", new GameMode[3]
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
        return this.player.UnlockData.Girl2ShopPurchase;
      }
    }

    private void OnItemTraded(object sender, TradeEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.Sell || e.Value <= 0)
        return;
      this.player.UnlockData.Girl2ShopPurchase = true;
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
        list.Add(string.Format("Item purchased in shop: {0}", (object) this.player.UnlockData.Girl2ShopPurchase));
        return list;
      }
    }
  }
}
