// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.DeathmatchMiniGame
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class DeathmatchMiniGame : IMiniGame
  {
    public DeathmatchWinType WinType;
    public readonly bool Eating;
    private float elapsed;
    private Player leader;
    private int leaderKills;
    private GameInstance instance;
    private List<DeathmatchMiniGame.PlayerData> data;
    private float sentElapsed;

    public float Elapsed
    {
      get
      {
        return this.elapsed;
      }
    }

    public Player Leader
    {
      get
      {
        return this.leader;
      }
    }

    public MiniGameType GameType
    {
      get
      {
        return MiniGameType.Deathmatch;
      }
    }

    public bool IsEatingAllowed
    {
      get
      {
        return this.Eating;
      }
    }

    public float EndTime
    {
      get
      {
        switch (this.WinType)
        {
          case DeathmatchWinType.MostKillsIn5Minutes:
            return 300f;
          case DeathmatchWinType.MostKillsIn10Minutes:
            return 600f;
          case DeathmatchWinType.MostKillsIn20Minutes:
            return 1200f;
          default:
            return 0.0f;
        }
      }
    }

    public DeathmatchMiniGame(DeathmatchWinType winType, bool eating)
    {
      this.WinType = winType;
      this.Eating = eating;
    }

    public static string GetStartError(GameInstance instance)
    {
      if (NetworkManager.Instance.AllGamerCount < 2)
        return "At least 2 players are needed to play a Deathmatch";
      return (string) null;
    }

    public void Start(GameInstance instance, Player startedBy)
    {
      this.instance = instance;
      string message = startedBy.DisplayGamertag + " has started a Deathmatch";
      string str = "To Win: " + Utils.InsertSpacesBeforeCapitals(this.WinType.ToString());
      CoreGlobals.Message.ShowMessage(message, 2f, 2.5f, Color.Red);
      this.elapsed = 0.0f;
      this.sentElapsed = 0.0f;
      this.leader = (Player) null;
      this.leaderKills = 0;
      instance.IsCombatEnabled = true;
      this.data = new List<DeathmatchMiniGame.PlayerData>();
      this.HookEvents();
    }

    private void HookEvents()
    {
      if (!NetworkManager.Instance.IsSessionOpen)
        return;
      foreach (Gamer allEnabledGamer in NetworkManager.Instance.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null)
          tag.PlayerDied += new Player.CharacterEventHandler(this.OnPlayerDied);
      }
    }

    private void UnhookEvents()
    {
      if (!NetworkManager.Instance.IsSessionOpen)
        return;
      foreach (Gamer allGamer in NetworkManager.Instance.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null)
          tag.PlayerDied -= new Player.CharacterEventHandler(this.OnPlayerDied);
      }
    }

    private void OnPlayerDied(object sender, ActorEventArgs e)
    {
      this.RegisterKill(e.Actor as Player, sender as Player);
    }

    public void Abort()
    {
      this.UnhookEvents();
    }

    public void End()
    {
      this.UnhookEvents();
      this.instance.AddScreen((GameScreen) new DeathmatchResultsScreen(this), (Player) null);
      this.instance.MiniGame = (IMiniGame) null;
    }

    public void Update()
    {
      this.elapsed += Services.ElapsedTime;
      float endTime = this.EndTime;
      if ((double) endTime > 0.0 && (double) this.elapsed >= (double) endTime)
        this.End();
      if (!this.instance.IsHost || (double) this.elapsed < (double) this.sentElapsed + 2.0)
        return;
      NetworkManager.Instance.SendMiniGameTimer(this.elapsed);
    }

    public void UpdateTimerFromHost(float elapsed)
    {
      this.elapsed = elapsed;
    }

    public void RegisterKill(Player killer, Player killed)
    {
      if (killer == null || killed == null)
        return;
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < this.data.Count && (!flag1 || !flag2); ++index)
      {
        DeathmatchMiniGame.PlayerData playerData = this.data[index];
        if (playerData.Player == killer)
        {
          ++playerData.Kills;
          flag1 = true;
          if (playerData.Kills >= this.leaderKills)
          {
            this.leaderKills = playerData.Kills;
            this.leader = playerData.Player;
          }
          this.data[index] = playerData;
        }
        if (playerData.Player == killed)
        {
          ++playerData.Killed;
          flag2 = true;
          this.data[index] = playerData;
        }
      }
      if (!flag1)
      {
        DeathmatchMiniGame.PlayerData playerData = new DeathmatchMiniGame.PlayerData()
        {
          Player = killer,
          Kills = 1,
          Killed = killer == killed ? 1 : 0
        };
        this.data.Add(playerData);
        if (playerData.Kills >= this.leaderKills)
        {
          this.leaderKills = playerData.Kills;
          this.leader = playerData.Player;
        }
      }
      if (!flag2 && killer != killed)
        this.data.Add(new DeathmatchMiniGame.PlayerData()
        {
          Player = killed,
          Killed = 1
        });
      switch (this.WinType)
      {
        case DeathmatchWinType.FirstTo5Kills:
          if (this.leaderKills < 5)
            break;
          this.End();
          break;
        case DeathmatchWinType.FirstTo10Kills:
          if (this.leaderKills < 10)
            break;
          this.End();
          break;
        case DeathmatchWinType.FirstTo20Kills:
          if (this.leaderKills < 20)
            break;
          this.End();
          break;
      }
    }

    public DeathmatchMiniGame.PlayerData[] GetWinners()
    {
      int num = 0;
      foreach (DeathmatchMiniGame.PlayerData playerData in this.data)
      {
        if (playerData.Kills >= num)
          num = playerData.Kills;
      }
      List<DeathmatchMiniGame.PlayerData> playerDataList = new List<DeathmatchMiniGame.PlayerData>();
      foreach (DeathmatchMiniGame.PlayerData playerData in this.data)
      {
        if (playerData.Kills == num)
          playerDataList.Add(playerData);
      }
      return playerDataList.ToArray();
    }

    public DeathmatchMiniGame.PlayerData GetPlayerData(Player player)
    {
      foreach (DeathmatchMiniGame.PlayerData playerData in this.data)
      {
        if (playerData.Player == player)
          return playerData;
      }
      return new DeathmatchMiniGame.PlayerData()
      {
        Player = player,
        Kills = 0,
        Killed = 0
      };
    }

    public void ReadPacket(PacketReader reader, byte dataType)
    {
    }

    public void EquipOnDeath(Player player)
    {
      if (this.instance.IsFiniteResources)
        return;
      player.AddToInventory(Item.WoodBow, 1);
      player.AddToInventory(Item.FlintArrow, 100);
      player.AddToInventory(Item.FlintArrow, 100);
      player.AddToInventory(Item.GrenadeLauncher, 1);
      player.AddToInventory(Item.Grenade, 100);
      player.AddToInventory(Item.Grenade, 100);
      player.AddToInventory(Item.DiamondSword, 1);
      player.AddToInventory(Item.DiamondSpear, 1);
      player.AddToInventory(Item.DiamondShield, 1);
    }

    public void RespawnOnDeath(Player player)
    {
      player.Position = this.instance.Map.GetBlockCenter(this.SpawnPoint + GlobalPoint3D.Up);
    }

    public GlobalPoint3D SpawnPoint
    {
      get
      {
        GlobalPoint3D shopPoint = Globals2.GameProperties.ShopPoint;
        shopPoint.X += this.instance.Random.Next(80) - 40;
        shopPoint.Z += this.instance.Random.Next(80) - 40;
        shopPoint.Y = (int) this.instance.Map.GetRegion(shopPoint).GetHeight(shopPoint);
        return shopPoint;
      }
    }

    public struct PlayerData
    {
      public Player Player;
      public int Kills;
      public int Killed;
    }
  }
}
