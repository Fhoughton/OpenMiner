// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Handyman
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Handyman : Unlockable
  {
    public Handyman(Player player)
      : base(player, ActorType.Handyman, "Craft every craftable item\nat least once.", new GameMode[1]
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
        return this.AllItemsCrafted;
      }
    }

    private void OnItemCrafted(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      this.player.UnlockData.HandymanActions[(int) e.ItemID] = true;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private bool AllItemsCrafted
    {
      get
      {
        foreach (Blueprint blueprint in Blueprints.BlueprintList)
        {
          if (blueprint.CraftType == BlueprintCraftType.Crafting && blueprint.IsOrigValid && (!this.HasCrafted(blueprint.Result.ItemID) && blueprint.Result.ItemID != Item.MobSpawn))
            return false;
        }
        return true;
      }
    }

    private bool HasCrafted(Item itemID)
    {
      return this.player.UnlockData.HandymanActions[(int) itemID];
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
          if (blueprint.CraftType == BlueprintCraftType.Crafting && blueprint.IsOrigValid && !this.HasCrafted(blueprint.Result.ItemID))
            stringList.Add(string.Format("{0} crafted: false", (object) Globals1.ItemData[(int) ItemData.ConvertItemIDToBlockID(blueprint.Result.ItemID)].Name));
        }
        stringList.Sort();
        list.AddRange((IEnumerable<string>) stringList);
        return list;
      }
    }
  }
}
