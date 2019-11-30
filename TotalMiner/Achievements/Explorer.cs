// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Explorer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Explorer : Unlockable
  {
    public Explorer(Player player)
      : base(player, ActorType.Explorer, "Find your first Blueprint\nand your first Wisdom Scroll.", new GameMode[1]
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
      this.player.FindBlueprint += new IntEventHandler(this.OnFindBlueprint);
      this.player.FindWisdomScroll += new IntEventHandler(this.OnFindWisdomScroll);
    }

    protected override void UnhookEvents()
    {
      this.player.FindBlueprint -= new IntEventHandler(this.OnFindBlueprint);
      this.player.FindWisdomScroll -= new IntEventHandler(this.OnFindWisdomScroll);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.ExplorerUnlocked)
          return true;
        if (this.player.UnlockData.ExplorerBlueprintsFound >= 1)
          return this.player.UnlockData.ExplorerWisdomsFound >= 1;
        return false;
      }
    }

    private void OnFindBlueprint(object sender, IntEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.ExplorerBlueprintsFound;
      if (!this.IsUnlocked)
        return;
      this.player.UnlockData.ExplorerUnlocked = true;
      this.Unlock();
    }

    private void OnFindWisdomScroll(object sender, IntEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.ExplorerWisdomsFound;
      if (!this.IsUnlocked)
        return;
      this.player.UnlockData.ExplorerUnlocked = true;
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
        list.Add(string.Format("Blueprint found: {0}", (object) (this.player.UnlockData.ExplorerBlueprintsFound >= 1)));
        list.Add(string.Format("Wisdom scroll found: {0}", (object) (this.player.UnlockData.ExplorerWisdomsFound >= 1)));
        return list;
      }
    }
  }
}
