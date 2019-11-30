// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Sage
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Sage : Unlockable
  {
    public Sage(Player player)
      : base(player, ActorType.Sage, "Find every Wisdom Scroll.", new GameMode[1]
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
      this.player.FindWisdomScroll += new IntEventHandler(this.OnFindWisdomScroll);
    }

    protected override void UnhookEvents()
    {
      this.player.FindWisdomScroll -= new IntEventHandler(this.OnFindWisdomScroll);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (!this.player.UnlockData.SageUnlocked)
          return this.player.UnlockData.SageWisdomsFound >= Wisdom.WisdomList.Length;
        return true;
      }
    }

    private void OnFindWisdomScroll(object sender, IntEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.SageWisdomsFound;
      if (!this.IsUnlocked)
        return;
      this.player.UnlockData.SageUnlocked = true;
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
        list.Add(string.Format("Wisdom scrolls found: {0} of {1}", (object) this.player.UnlockData.SageWisdomsFound, (object) Wisdom.WisdomList.Length));
        return list;
      }
    }
  }
}
