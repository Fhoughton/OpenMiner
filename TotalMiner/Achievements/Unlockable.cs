// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Unlockable
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Achievements
{
  internal abstract class Unlockable : IDisposable
  {
    public ActorType ActorType;
    public string UnlockInstruction;
    public GameMode[] GameModes;
    public GameDifficulty[] Difficulties;
    public NetworkSessionType[] SessionTypes;
    protected Player player;

    public event EventHandler Unlocked;

    private void RaiseUnlocked()
    {
      if (this.Unlocked == null)
        return;
      this.Unlocked((object) this, EventArgs.Empty);
    }

    public virtual bool IsUnlocked
    {
      get
      {
        return false;
      }
    }

    public virtual bool IsNPC
    {
      get
      {
        return false;
      }
    }

    public virtual bool IsDisplayed
    {
      get
      {
        if (this.IsUnlocked || this.IsNPC)
          return true;
        if (this.player != null)
          return this.player.IsGod;
        return false;
      }
    }

    protected bool IsUnlockableGameMode
    {
      get
      {
        if (NetworkManager.Instance == null || !NetworkManager.Instance.IsSessionOpen || !Globals2.IsValidGameHeader)
          return false;
        if (this.GameModes == null || this.GameModes.Length == 0)
          return true;
        return Array.IndexOf<GameMode>(this.GameModes, Globals2.GameProperties.SaveGame.Header.GameMode) >= 0;
      }
    }

    protected bool IsUnlockableDifficulty
    {
      get
      {
        if (NetworkManager.Instance == null || !NetworkManager.Instance.IsSessionOpen)
          return false;
        if (this.Difficulties == null || this.Difficulties.Length == 0)
          return true;
        return Array.IndexOf<GameDifficulty>(this.Difficulties, Globals2.GameProperties.SaveGame.Header.GameDifficulty) >= 0;
      }
    }

    protected bool IsUnlockableSessionType
    {
      get
      {
        if (NetworkManager.Instance == null || !NetworkManager.Instance.IsSessionOpen)
          return false;
        if (this.SessionTypes == null || this.SessionTypes.Length == 0)
          return true;
        return Array.IndexOf<NetworkSessionType>(this.SessionTypes, NetworkManager.Instance.Session.SessionType) >= 0;
      }
    }

    protected bool ShouldHookEvents
    {
      get
      {
        return false;
      }
    }

    public virtual bool HasProgress
    {
      get
      {
        return false;
      }
    }

    public virtual List<string> ProgressList
    {
      get
      {
        return (List<string>) null;
      }
    }

    public Unlockable(
      Player player,
      ActorType mobType,
      string unlockInstruction,
      GameMode[] gameMode,
      GameDifficulty[] difficulty,
      NetworkSessionType[] sessionType)
    {
      this.player = player;
      this.ActorType = mobType;
      this.UnlockInstruction = unlockInstruction;
      this.GameModes = gameMode;
      this.Difficulties = difficulty;
      this.SessionTypes = sessionType;
      this.InitializeCore();
      if (!this.ShouldHookEvents)
        return;
      this.HookEvents();
    }

    protected virtual void InitializeCore()
    {
    }

    protected virtual void HookEvents()
    {
    }

    public void Dispose()
    {
      this.UnhookEvents();
    }

    protected virtual void UnhookEvents()
    {
    }

    public void RehookEvents()
    {
      this.UnhookEvents();
      this.HookEvents();
    }

    protected virtual void Unlock()
    {
      if (this.player.GameInstance == null || this.player.IsGod)
        return;
      UnlockablesScreen unlockablesScreen = new UnlockablesScreen(GameInstance.Instance, this.player, this, true, false, new Action<Player, ActorType>(this.player.SetAvatar), this.ActorType, (GameScreen) null);
      GameInstance.Instance.AddScreen((GameScreen) unlockablesScreen, this.player);
      GameInstance.Instance.AddNotification(this.player, " has unlocked the " + (object) this.ActorType, NotifyRecipient.Global);
      this.UnhookEvents();
      Globals2.SaveGamertagData(true, false, true);
      this.RaiseUnlocked();
    }

    protected void AddReqsMetProgress(List<string> list)
    {
      if (NetworkManager.Instance == null || NetworkManager.Instance.GameInstance == null)
        return;
      int count = list.Count;
      if (this.GameModes != null)
        list.Add("Game Mode: " + (this.ArrayContains<GameMode>(this.GameModes, Globals2.GameProperties.SaveGame.Header.GameMode) ? "True" : "False"));
      if (this.Difficulties != null)
        list.Add("Difficulty: " + (this.ArrayContains<GameDifficulty>(this.Difficulties, Globals2.GameProperties.SaveGame.Header.GameDifficulty) ? "True" : "False"));
      if (this.SessionTypes != null)
        list.Add("Session Type: " + (this.ArrayContains<NetworkSessionType>(this.SessionTypes, NetworkManager.Instance.Session.SessionType) ? "True" : "False"));
      if (list.Count == count)
        return;
      list.Add("--------------------------------------------");
    }

    private bool ArrayContains<T>(T[] list, T s)
    {
      foreach (T obj in list)
      {
        if (obj.Equals((object) s))
          return true;
      }
      return false;
    }
  }
}
