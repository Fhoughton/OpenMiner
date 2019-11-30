// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SleepUpdate
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Threading;

namespace StudioForge.TotalMiner
{
  internal class SleepUpdate : IThreadWorkItem
  {
    private GameInstance instance;

    public string Name
    {
      get
      {
        return nameof (SleepUpdate);
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return false;
      }
    }

    public SleepUpdate(GameInstance instance)
    {
      this.instance = instance;
    }

    public void Update()
    {
      while (this.instance.IsSleeping)
      {
        if (this.instance.IsMapActiveIgnoreGuide)
          this.instance.UpdateGame((UpdateState) null);
        Thread.Sleep(TimeSpan.FromMilliseconds(0.200000002980232));
      }
      if (!this.instance.IsHost)
        return;
      this.instance.FlagAllPlayersSleepHasFinished();
      this.instance.NetworkManager.SendSleepState(0.0f);
      this.instance.NetworkManager.SendGameState(true);
    }
  }
}
