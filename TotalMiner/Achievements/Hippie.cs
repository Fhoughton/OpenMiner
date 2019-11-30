// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Hippie
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Hippie : Unlockable
  {
    public Hippie(Player player)
      : base(player, ActorType.Hippie, "Throw 30 stacks of flowers at an\nenemy.", new GameMode[2]
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
      this.player.ItemThrown += new Player.CharacterAndItemEventHandler(this.OnItemThrown);
    }

    protected override void UnhookEvents()
    {
      this.player.ItemThrown -= new Player.CharacterAndItemEventHandler(this.OnItemThrown);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.HippieFlowersThrownAtEnemy >= 30;
      }
    }

    private void OnItemThrown(object sender, ActorItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.Actor == null || (e.Actor.IsPlayer || e.Actor.AITarget != this.player) || e.ItemID != Item.RedFlowers && e.ItemID != Item.YellowFlowers && (e.ItemID != Item.PurpleFlowers && e.ItemID != Item.WhiteFlowers))
        return;
      ++this.player.UnlockData.HippieFlowersThrownAtEnemy;
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
        list.Add(string.Format("Flowers thrown at enemy: {0} of 30", (object) this.player.UnlockData.HippieFlowersThrownAtEnemy));
        return list;
      }
    }
  }
}
