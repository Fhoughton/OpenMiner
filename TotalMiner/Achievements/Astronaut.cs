// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Astronaut
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal class Astronaut : Unlockable
  {
    public Astronaut(Player player)
      : base(player, ActorType.Astronaut, "Visit 10 remote worlds.", new GameMode[4]
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

    protected override void HookEvents()
    {
      this.player.EnterMap += new MapEventHandler(this.OnEnterMap);
    }

    protected override void UnhookEvents()
    {
      this.player.EnterMap -= new MapEventHandler(this.OnEnterMap);
    }

    public override bool IsUnlocked
    {
      get
      {
        return this.player.UnlockData.AstronautWorldsVisited.Count >= 10;
      }
    }

    private void OnEnterMap(object sender, MapEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || this.player.Gamer.IsHost)
        return;
      int num = Globals2.BuildMapID(Globals2.GameProperties.SaveGame.Header);
      if (this.player.UnlockData.AstronautWorldsVisited.Contains(num))
        return;
      this.player.UnlockData.AstronautWorldsVisited.Add(num);
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
        list.Add(string.Format("Online worlds visited: {0} of 10", (object) this.player.UnlockData.AstronautWorldsVisited.Count));
        return list;
      }
    }
  }
}
