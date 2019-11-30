// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Pirate
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Pirate : Unlockable
  {
    private List<GlobalPoint3D> chestsFound = new List<GlobalPoint3D>();

    public Pirate(Player player)
      : base(player, ActorType.Pirate, "Open 30 treasure chests.", new GameMode[2]
      {
        GameMode.DigDeep,
        GameMode.Survival
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
      this.player.TreasureChestOpened += new BlockEventHandler(this.OnTreasureChestOpened);
    }

    protected override void UnhookEvents()
    {
      this.player.TreasureChestOpened -= new BlockEventHandler(this.OnTreasureChestOpened);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.PirateChestsOpened >= 30;
      }
    }

    private void OnTreasureChestOpened(object sender, BlockEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.BlockID != Block.Chest)
        return;
      foreach (GlobalPoint3D globalPoint3D in this.chestsFound)
      {
        if (globalPoint3D == e.Point)
          return;
      }
      this.chestsFound.Add(e.Point);
      MapStrategyTM mapStrategy = NetworkManager.Instance.GameInstance.Map.MapStrategy as MapStrategyTM;
      if (mapStrategy == null || (mapStrategy.GetDataBlock(e.Point) as ChestBlock).Inventory.TotalPackItemsCount <= 0)
        return;
      ++this.player.UnlockData.PirateChestsOpened;
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
        list.Add(string.Format("Treasure chests opened: {0} of 30", (object) this.player.UnlockData.PirateChestsOpened));
        return list;
      }
    }
  }
}
