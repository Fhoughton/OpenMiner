// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Caveman
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Caveman : Unlockable
  {
    public Caveman(Player player)
      : base(player, ActorType.Caveman, "Mine 500 blocks the old fashioned way,\nwith a wood pickaxe.", new GameMode[3]
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
    }

    protected override void UnhookEvents()
    {
      this.player.BlockCleared -= new BlockEventHandler(this.OnBlockCleared);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.CavemanBlocksCleared >= 500;
      }
    }

    private void OnBlockCleared(object sender, BlockEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.ItemID != Item.WoodPickaxe)
        return;
      ++this.player.UnlockData.CavemanBlocksCleared;
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
        list.Add(string.Format("Blocks cleared with Wood Pick: {0} of 500", (object) this.player.UnlockData.CavemanBlocksCleared));
        return list;
      }
    }
  }
}
