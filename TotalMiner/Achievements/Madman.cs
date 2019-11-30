// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Madman
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Madman : Unlockable
  {
    public Madman(Player player)
      : base(player, ActorType.Madman, "Detonate 500 explosives.", new GameMode[2]
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
      this.player.DetonateExplosive += new ItemEventHandler(this.OnDetonateExplosive);
    }

    protected override void UnhookEvents()
    {
      this.player.DetonateExplosive -= new ItemEventHandler(this.OnDetonateExplosive);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.MadmanDetonations >= 500;
      }
    }

    private void OnDetonateExplosive(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.MadmanDetonations;
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
        list.Add(string.Format("Explosives detonated: {0} of 500", (object) this.player.UnlockData.MadmanDetonations));
        return list;
      }
    }
  }
}
