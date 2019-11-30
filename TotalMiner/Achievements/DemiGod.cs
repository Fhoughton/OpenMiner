// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.DemiGod
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class DemiGod : Unlockable
  {
    private int minutesPlayed;
    private Unlockable[] unlockables;

    public DemiGod(Player player)
      : base(player, ActorType.DemiGod, "Various requirements.", new GameMode[4]
      {
        GameMode.DigDeep,
        GameMode.Survival,
        GameMode.Peaceful,
        GameMode.Creative
      }, new GameDifficulty[4]
      {
        GameDifficulty.Peaceful,
        GameDifficulty.Easy,
        GameDifficulty.Normal,
        GameDifficulty.Legendary
      }, new NetworkSessionType[1]
      {
        NetworkSessionType.PlayerMatch
      })
    {
    }

    public void HookEvents2(Unlockable[] unlockables)
    {
      this.unlockables = unlockables;
      if (this.IsUnlocked || this.player.UnlockData == null)
        return;
      if (!this.player.IsHost)
      {
        this.player.EnterMap += new MapEventHandler(this.OnEnterMap);
        this.player.WorldRated += new Player.PlayerEventHandler(this.OnRatedWorld);
      }
      else
      {
        this.player.WorldRated += new Player.PlayerEventHandler(this.OnWorldRated);
        this.player.WorldFavorited += new Player.PlayerEventHandler(this.OnWorldFavorited);
      }
      if (this.player.UnlockData.DemiBlocksMined <= 50001)
        this.player.BlockCleared += new BlockEventHandler(this.OnBlockCleared);
      if (this.player.UnlockData.DemiBlocksPlaced <= 100001)
        this.player.BlockPlaced += new BlockEventHandler(this.OnBlockPlaced);
      this.HookUnlocked(unlockables, 35);
      this.HookUnlocked(unlockables, 39);
      this.HookUnlocked(unlockables, 40);
      this.HookUnlocked(unlockables, 41);
      this.HookUnlocked(unlockables, 42);
      this.HookUnlocked(unlockables, 43);
      this.HookUnlocked(unlockables, 44);
      this.HookUnlocked(unlockables, 49);
      this.HookUnlocked(unlockables, 51);
      this.HookUnlocked(unlockables, 52);
      this.HookUnlocked(unlockables, 53);
      this.HookUnlocked(unlockables, 57);
      this.HookUnlocked(unlockables, 60);
      this.HookUnlocked(unlockables, 61);
      this.HookUnlocked(unlockables, 64);
      this.HookUnlocked(unlockables, 68);
      this.minutesPlayed = (int) (this.player.Statistics.SecondsPlayed / 60.0);
    }

    private bool HookUnlocked(Unlockable[] unlockables, int i)
    {
      if (unlockables[i].IsUnlocked)
        return false;
      unlockables[i].Unlocked += new EventHandler(this.OnAnotherUnlocked);
      return true;
    }

    protected override void UnhookEvents()
    {
      this.player.BlockCleared -= new BlockEventHandler(this.OnBlockCleared);
      this.player.BlockPlaced -= new BlockEventHandler(this.OnBlockPlaced);
      this.player.EnterMap -= new MapEventHandler(this.OnEnterMap);
      this.player.RatedWorld -= new Player.PlayerEventHandler(this.OnRatedWorld);
      this.player.WorldRated -= new Player.PlayerEventHandler(this.OnWorldRated);
      this.player.WorldFavorited -= new Player.PlayerEventHandler(this.OnWorldFavorited);
      this.player.MinutePlayed -= new EventHandler(this.OnMinute);
      this.player.Unlockables.UnlockableList[35].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[39].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[40].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[41].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[42].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[43].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[44].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[49].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[51].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[52].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[53].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[57].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[60].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[61].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[64].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
      this.player.Unlockables.UnlockableList[68].Unlocked -= new EventHandler(this.OnAnotherUnlocked);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.unlockables != null && this.player.UnlockData != null && (this.player.UnlockData.DemiBlocksMined >= 50000 && this.player.UnlockData.DemiBlocksPlaced >= 100000) && (this.player.UnlockData.DemiRatesReceived >= 500 && this.player.UnlockData.DemiRatesGiven >= 100 && (this.player.UnlockData.DemiFavoriters.Count >= 50 && this.player.UnlockData.DemiWorldsVisited.Count >= 100)) && (this.player.UnlockData.DemiWorldsVisited10Mins.Count >= 50 && this.unlockables[35].IsUnlocked && (this.unlockables[39].IsUnlocked && this.unlockables[40].IsUnlocked) && (this.unlockables[41].IsUnlocked && this.unlockables[42].IsUnlocked && (this.unlockables[43].IsUnlocked && this.unlockables[44].IsUnlocked))) && (this.unlockables[49].IsUnlocked && this.unlockables[51].IsUnlocked && (this.unlockables[52].IsUnlocked && this.unlockables[53].IsUnlocked) && (this.unlockables[57].IsUnlocked && this.unlockables[60].IsUnlocked && (this.unlockables[61].IsUnlocked && this.unlockables[64].IsUnlocked))))
          return this.unlockables[68].IsUnlocked;
        return false;
      }
    }

    private void OnBlockCleared(object sender, BlockEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.DemiBlocksMined;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnBlockPlaced(object sender, BlockEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.DemiBlocksPlaced;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnRatedWorld(object sender, PlayerEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.DemiRatesGiven;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnWorldRated(object sender, PlayerEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      ++this.player.UnlockData.DemiRatesReceived;
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnWorldFavorited(object sender, PlayerEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || this.player.UnlockData.DemiFavoriters.Contains(e.Player.Gamertag))
        return;
      this.player.UnlockData.DemiFavoriters.Add(e.Player.Gamertag);
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnMinute(object sender, EventArgs e)
    {
      if (++this.minutesPlayed < 10 || !this.IsUnlockableDifficulty)
        return;
      int num = Globals2.BuildMapID(Globals2.GameProperties.SaveGame.Header);
      if (this.player.UnlockData.DemiWorldsVisited10Mins.Contains(num) || this.player.Statistics.SecondsPlayed < 600.0)
        return;
      this.player.UnlockData.DemiWorldsVisited10Mins.Add(num);
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnEnterMap(object sender, MapEventArgs e)
    {
      if (!this.IsUnlockableDifficulty)
        return;
      int num = Globals2.BuildMapID(Globals2.GameProperties.SaveGame.Header);
      if (this.player.UnlockData.DemiWorldsVisited.Contains(num))
        return;
      this.player.UnlockData.DemiWorldsVisited.Add(num);
      if (!this.IsUnlocked)
        return;
      this.Unlock();
    }

    private void OnAnotherUnlocked(object sender, EventArgs e)
    {
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
        list.Add(string.Format("Blocks mined: {0} of 50,000", (object) this.player.UnlockData.DemiBlocksMined));
        list.Add(string.Format("Blocks placed: {0} of 100,000", (object) this.player.UnlockData.DemiBlocksPlaced));
        list.Add(string.Format("Total Rates given: {0} of 100", (object) this.player.UnlockData.DemiRatesGiven));
        list.Add(string.Format("Total Rates received: {0} of 500", (object) this.player.UnlockData.DemiRatesReceived));
        list.Add(string.Format("Total Favorites received: {0} of 50", (object) this.player.UnlockData.DemiFavoriters.Count));
        list.Add(string.Format("Worlds visited: {0} of 100", (object) this.player.UnlockData.DemiWorldsVisited.Count));
        list.Add(string.Format("Worlds visited for 10 mins or more: {0} of 50", (object) this.player.UnlockData.DemiWorldsVisited10Mins.Count));
        list.Add(string.Format("Girl2 unlocked: {0}", (object) this.unlockables[35].IsUnlocked));
        list.Add(string.Format("Explorer unlocked: {0}", (object) this.unlockables[39].IsUnlocked));
        list.Add(string.Format("Tree Hugger unlocked: {0}", (object) this.unlockables[40].IsUnlocked));
        list.Add(string.Format("Carpenter unlocked: {0}", (object) this.unlockables[41].IsUnlocked));
        list.Add(string.Format("Prisoner unlocked: {0}", (object) this.unlockables[42].IsUnlocked));
        list.Add(string.Format("Pupil unlocked: {0}", (object) this.unlockables[43].IsUnlocked));
        list.Add(string.Format("Jamaican unlocked: {0}", (object) this.unlockables[44].IsUnlocked));
        list.Add(string.Format("Astronaut unlocked: {0}", (object) this.unlockables[49].IsUnlocked));
        list.Add(string.Format("Caveman unlocked: {0}", (object) this.unlockables[51].IsUnlocked));
        list.Add(string.Format("Chef unlocked: {0}", (object) this.unlockables[52].IsUnlocked));
        list.Add(string.Format("Invaderman unlocked: {0}", (object) this.unlockables[53].IsUnlocked));
        list.Add(string.Format("Diablo unlocked: {0}", (object) this.unlockables[57].IsUnlocked));
        list.Add(string.Format("Entrepreneur unlocked: {0}", (object) this.unlockables[60].IsUnlocked));
        list.Add(string.Format("Golden Knight unlocked: {0}", (object) this.unlockables[61].IsUnlocked));
        list.Add(string.Format("Sage unlocked: {0}", (object) this.unlockables[64].IsUnlocked));
        list.Add(string.Format("Zombie unlocked: {0}", (object) this.unlockables[68].IsUnlocked));
        return list;
      }
    }
  }
}
