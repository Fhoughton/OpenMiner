// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Medic
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Medic : Unlockable
  {
    public Medic(Player player)
      : base(player, ActorType.Medic, "Heal yourself 50 times, and heal\nanother player 50 times.", new GameMode[2]
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
      this.player.HealPlayer += new Player.PlayerEventHandler(this.OnHealPlayer);
    }

    protected override void UnhookEvents()
    {
      this.player.HealPlayer -= new Player.PlayerEventHandler(this.OnHealPlayer);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.MedicHealedOther >= 50)
          return this.player.UnlockData.MedicHealedSelf >= 50;
        return false;
      }
    }

    private void OnHealPlayer(object sender, PlayerEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      if (e.Player == this.player)
        ++this.player.UnlockData.MedicHealedSelf;
      else
        ++this.player.UnlockData.MedicHealedOther;
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
        list.Add(string.Format("Healed self: {0} of 50", (object) this.player.UnlockData.MedicHealedSelf));
        list.Add(string.Format("Healed others: {0} of 50", (object) this.player.UnlockData.MedicHealedOther));
        return list;
      }
    }
  }
}
