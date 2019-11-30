// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Lumberjack
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Lumberjack : Unlockable
  {
    public Lumberjack(Player player)
      : base(player, ActorType.Lumberjack, "Plant 20 saplings, chop 20 replanted\ntrees and craft 80 Wood Planks.", new GameMode[3]
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
      this.player.BlockCleared += new BlockEventHandler(this.OnBlockCleared);
      this.player.BlockPlaced += new BlockEventHandler(this.OnBlockPlaced);
      this.player.ItemCrafted += new ItemEventHandler(this.OnItemCrafted);
    }

    protected override void UnhookEvents()
    {
      this.player.BlockCleared -= new BlockEventHandler(this.OnBlockCleared);
      this.player.BlockPlaced -= new BlockEventHandler(this.OnBlockPlaced);
      this.player.ItemCrafted -= new ItemEventHandler(this.OnItemCrafted);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.LumberJackTreesChopped >= 20 && this.player.UnlockData.LumberJackSaplingsPlanted >= 20)
          return this.player.UnlockData.LumberJackWoodPlanksCrafted >= 80;
        return false;
      }
    }

    private void OnBlockCleared(object sender, BlockEventArgs e)
    {
      GameInstance gameInstance = NetworkManager.Instance.GameInstance;
      if (gameInstance == null || !this.IsUnlockableDifficulty || (e.BlockID != Block.Wood || !gameInstance.Map.HasChanged(e.BlockData)))
        return;
      ++this.player.UnlockData.LumberJackTreesChopped;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnBlockPlaced(object sender, BlockEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.BlockID != Block.Sapling)
        return;
      ++this.player.UnlockData.LumberJackSaplingsPlanted;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnItemCrafted(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.ItemID != Item.WoodPlank)
        return;
      this.player.UnlockData.LumberJackWoodPlanksCrafted += Blueprints.GetCraftCount(Item.WoodPlank);
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
        list.Add(string.Format("Trees chopped: {0} of 20", (object) this.player.UnlockData.LumberJackTreesChopped));
        list.Add(string.Format("Saplings planted: {0} of 20", (object) this.player.UnlockData.LumberJackSaplingsPlanted));
        list.Add(string.Format("Wood Planks crafted: {0} of 80", (object) this.player.UnlockData.LumberJackWoodPlanksCrafted));
        return list;
      }
    }
  }
}
