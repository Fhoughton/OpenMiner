// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Indian
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Indian : Unlockable
  {
    public Indian(Player player)
      : base(player, ActorType.Indian, "Craft a Wood Bow. Craft 200 Arrows.\nKill 50 enemies using your Bow.", new GameMode[2]
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
      this.player.ItemCrafted += new ItemEventHandler(this.OnItemCrafted);
      this.player.KillCharacter += new Player.CharacterEventHandler(this.OnKillCharacter);
    }

    protected override void UnhookEvents()
    {
      this.player.ItemCrafted -= new ItemEventHandler(this.OnItemCrafted);
      this.player.KillCharacter -= new Player.CharacterEventHandler(this.OnKillCharacter);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.IndianBowCrafted && this.player.UnlockData.IndianArrowsCrafted >= 200)
          return this.player.UnlockData.IndianEnemiesKilled >= 50;
        return false;
      }
    }

    private void OnItemCrafted(object sender, ItemEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      if (ItemData.IsSubType(e.ItemID, ItemSubType.Arrow))
        this.player.UnlockData.IndianArrowsCrafted += Blueprints.GetCraftCount(e.ItemID);
      if (e.ItemID == Item.WoodBow)
        this.player.UnlockData.IndianBowCrafted = true;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnKillCharacter(object sender, ActorEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || !ItemData.IsSubTypeAny(e.Weapon, ItemSubType.Bow | ItemSubType.Arrow))
        return;
      ++this.player.UnlockData.IndianEnemiesKilled;
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
        list.Add(string.Format("Wood Bow crafted: {0}", (object) this.player.UnlockData.IndianBowCrafted));
        list.Add(string.Format("Arrows crafted: {0} of 200", (object) this.player.UnlockData.IndianArrowsCrafted));
        list.Add(string.Format("Enemies killed with arrow: {0} of 50", (object) this.player.UnlockData.IndianEnemiesKilled));
        return list;
      }
    }
  }
}
