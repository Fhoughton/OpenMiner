// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.King
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;

namespace StudioForge.TotalMiner.Achievements
{
  internal class King : Unlockable
  {
    public King(Player player)
      : base(player, ActorType.King, "Host an online game with at least\n10 concurrent players.", new GameMode[4]
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

    protected override void InitializeCore()
    {
      base.InitializeCore();
    }

    protected override void HookEvents()
    {
      this.player.WorldVisited += new Player.PlayerEventHandler(this.OnWorldVisited);
    }

    protected override void UnhookEvents()
    {
      this.player.WorldVisited -= new Player.PlayerEventHandler(this.OnWorldVisited);
    }

    public override bool IsUnlocked
    {
      get
      {
        if (this.player.UnlockData.KingUnlocked)
          return true;
        if (this.player.Gamer != null && this.player.Gamer.IsHost && NetworkManager.Instance != null)
          return NetworkManager.Instance.RemoteGamerCount >= 9;
        return false;
      }
    }

    private void OnWorldVisited(object sender, PlayerEventArgs e)
    {
      if (!this.IsUnlockableDifficulty || !this.IsUnlocked)
        return;
      this.player.UnlockData.KingUnlocked = true;
      this.Unlock();
    }
  }
}
