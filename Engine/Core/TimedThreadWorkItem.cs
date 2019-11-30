// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.TimedThreadWorkItem
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.Diagnostics;

namespace StudioForge.Engine.Core
{
  public abstract class TimedThreadWorkItem : IThreadWorkItem
  {
    protected Stopwatch timer;
    protected bool canWait;
    private int sleepTime;
    private PriorityLevel priority;

    public abstract string Name { get; }

    public bool IsSleeping
    {
      get
      {
        return this.timer.ElapsedMilliseconds < (long) this.sleepTime;
      }
    }

    public bool CanWait
    {
      get
      {
        return this.canWait;
      }
    }

    public TimedThreadWorkItem(PriorityLevel priority, int sleepTimeInMillisecs)
    {
      this.priority = priority;
      this.sleepTime = sleepTimeInMillisecs;
      this.canWait = true;
      this.timer = new Stopwatch();
      this.timer.Start();
    }

    public void Update()
    {
      try
      {
        this.UpdateCore();
      }
      finally
      {
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
        this.timer.Reset();
        this.timer.Start();
      }
    }

    protected abstract void UpdateCore();
  }
}
