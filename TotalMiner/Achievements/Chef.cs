// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Chef
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Chef : Unlockable
  {
    public Chef(Player player)
      : base(player, ActorType.Chef, "Craft every ingredient and\nCook every food type at least once.", new GameMode[3]
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
        return this.AllItemsCooked;
      }
    }

    private void OnItemCrafted(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      this.player.UnlockData.ChefActions[(int) e.ItemID] = true;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private bool AllItemsCooked
    {
      get
      {
        if (Blueprints.BlueprintList == null)
          return false;
        foreach (Blueprint blueprint in Blueprints.BlueprintList)
        {
          if (blueprint.IsOrigValid && this.IsValidItem(blueprint.Result.ItemID) && !this.HasCooked(blueprint.Result.ItemID))
            return false;
        }
        return true;
      }
    }

    private bool IsValidItem(Item itemID)
    {
      return Globals1.ItemTypeData[(int) itemID].Type == ItemType.Food;
    }

    private bool HasCooked(Item itemID)
    {
      return this.player.UnlockData.ChefActions[(int) itemID];
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
        List<string> stringList = new List<string>();
        foreach (Blueprint blueprint in Blueprints.BlueprintList)
        {
          if (blueprint.IsOrigValid && this.IsValidItem(blueprint.Result.ItemID) && !this.HasCooked(blueprint.Result.ItemID))
            stringList.Add(string.Format("{0} prepared/cooked: false", (object) blueprint.Result.ItemID));
        }
        stringList.Sort();
        list.AddRange((IEnumerable<string>) stringList);
        return list;
      }
    }
  }
}
