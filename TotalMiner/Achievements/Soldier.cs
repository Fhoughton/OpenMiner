// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Soldier
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Soldier : Unlockable
  {
    public Soldier(Player player)
      : base(player, ActorType.Soldier, "Craft a Grenade Launcher. Craft 50\nGrenades. Launch 50 Grenades.", new GameMode[2]
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
      this.player.ItemCrafted += new ItemEventHandler(this.OnItemCrafted);
      this.player.GrenadeLaunched += new EventHandler(this.OnGrenadeLaunched);
    }

    protected override void UnhookEvents()
    {
      this.player.ItemCrafted -= new ItemEventHandler(this.OnItemCrafted);
      this.player.GrenadeLaunched -= new EventHandler(this.OnGrenadeLaunched);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.SoldierGrenadeLauncherCrafted && this.player.UnlockData.SoldierGrenadesCrafted >= 50)
          return this.player.UnlockData.SoldierGrenadesLaunched >= 50;
        return false;
      }
    }

    private void OnItemCrafted(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      if (e.ItemID == Item.Grenade)
        this.player.UnlockData.SoldierGrenadesCrafted += Blueprints.GetCraftCount(Item.Grenade);
      else if (e.ItemID == Item.GrenadeLauncher)
        this.player.UnlockData.SoldierGrenadeLauncherCrafted = true;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnGrenadeLaunched(object sender, EventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.SoldierGrenadesLaunched;
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
        list.Add(string.Format("Grenade Launcher crafted: {0}", (object) this.player.UnlockData.SoldierGrenadeLauncherCrafted));
        list.Add(string.Format("Grenades crafted: {0} of 50", (object) this.player.UnlockData.SoldierGrenadesCrafted));
        list.Add(string.Format("Grenades launched: {0} of 50", (object) this.player.UnlockData.SoldierGrenadesLaunched));
        return list;
      }
    }
  }
}
