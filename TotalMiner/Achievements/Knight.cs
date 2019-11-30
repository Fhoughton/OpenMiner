// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Knight
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Knight : Unlockable
  {
    public Knight(Player player)
      : base(player, ActorType.Knight, "Reach Bedrock and kill at least\n50 enemies along the way.", new GameMode[1]
      {
        GameMode.DigDeep
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
      this.player.ItemProspect += new ItemEventHandler(this.OnItemProspect);
      this.player.KillCharacter += new Player.CharacterEventHandler(this.OnKillCharacter);
    }

    protected override void UnhookEvents()
    {
      this.player.ItemProspect -= new ItemEventHandler(this.OnItemProspect);
      this.player.KillCharacter -= new Player.CharacterEventHandler(this.OnKillCharacter);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.KnightUnlocked)
          return true;
        if (this.player.UnlockData.KnightBedrockReached)
          return this.player.UnlockData.KnightEnemiesKilled >= 50;
        return false;
      }
    }

    private void OnItemProspect(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || e.ItemID != Item.Bedrock)
        return;
      this.player.UnlockData.KnightBedrockReached = true;
      if (!this.IsUnlocked)
        return;
      this.player.UnlockData.KnightUnlocked = true;
      this.Unlock();
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || this.player.UnlockData.KnightBedrockReached)
        return;
      ++this.player.UnlockData.KnightEnemiesKilled;
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
        list.Add(string.Format("Enemies killed before reaching bedrock: {0} of 50", (object) this.player.UnlockData.KnightEnemiesKilled));
        list.Add(string.Format("Bedrock reached and prospected: {0}", (object) this.player.UnlockData.KnightBedrockReached));
        return list;
      }
    }
  }
}
