// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Carpenter
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Carpenter : Unlockable
  {
    public Carpenter(Player player)
      : base(player, ActorType.Carpenter, "Craft your first Workbench.", new GameMode[3]
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
      this.player.ItemCrafted += new ItemEventHandler(this.OnItemCrafted);
    }

    protected override void UnhookEvents()
    {
      this.player.ItemCrafted -= new ItemEventHandler(this.OnItemCrafted);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.CarpenterWorkbenchCrafted;
      }
    }

    private void OnItemCrafted(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.ItemID != Item.Workbench)
        return;
      this.player.UnlockData.CarpenterWorkbenchCrafted = true;
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
        list.Add(string.Format("Workbench crafted: {0}", (object) this.player.UnlockData.CarpenterWorkbenchCrafted));
        return list;
      }
    }
  }
}
