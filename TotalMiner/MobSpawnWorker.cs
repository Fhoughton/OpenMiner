// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MobSpawnWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class MobSpawnWorker : IThreadWorkItem
  {
    private PriorityLevel priority;
    private NpcManager mobManager;
    private GameInstance instance;

    public string Name
    {
      get
      {
        return nameof (MobSpawnWorker);
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
        return true;
      }
    }

    public MobSpawnWorker(GameInstance instance, PriorityLevel priority)
    {
      this.instance = instance;
      this.mobManager = instance.NpcManager;
      this.priority = priority;
    }

    public void Update()
    {
      try
      {
        if (!this.instance.IsMapActive || !this.mobManager.IsEnabledField)
          return;
        this.mobManager.UpdateNpcSpawns();
      }
      finally
      {
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
      }
    }
  }
}
