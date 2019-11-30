// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.ThreadedGameObject
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace StudioForge.Engine.Core
{
  public abstract class ThreadedGameObject : GameObjectBase
  {
    public ManualResetEvent TriggerEvent;
    public ManualResetEvent CompleteEvent;
    public object Tag;
    protected bool run;
    protected Thread thread;
    private double lastFrameTime;
    private double longestFrameTime;
    private double timeSmoothTimer;

    public bool IsStarted
    {
      get
      {
        if (this.thread != null)
          return this.thread.IsAlive;
        return false;
      }
    }

    public double LastFrameTime
    {
      get
      {
        return this.lastFrameTime;
      }
    }

    public double LongestFrameTime
    {
      get
      {
        return this.longestFrameTime;
      }
    }

    protected override void UnloadContentCore()
    {
      this.End();
      base.UnloadContentCore();
    }

    public void Start(bool waitSynch)
    {
      if (waitSynch)
      {
        this.TriggerEvent = new ManualResetEvent(false);
        this.CompleteEvent = new ManualResetEvent(false);
      }
      this.run = true;
      this.thread = !waitSynch ? new Thread(new ThreadStart(this.ThreadedUpdateNoTiming)) : new Thread(new ThreadStart(this.ThreadedUpdate));
      this.thread.Start();
    }

    public void End()
    {
      if (!this.run)
        return;
      this.run = false;
      if (this.TriggerEvent != null)
        this.TriggerEvent.Set();
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      while (this.thread.IsAlive)
      {
        if (stopwatch.ElapsedMilliseconds > 2000L)
        {
          this.thread.Abort();
          break;
        }
      }
    }

    private void ThreadedUpdateNoTiming()
    {
      while (this.run)
      {
        try
        {
          this.ThreadedUpdateCore(0.0);
        }
        catch (Exception ex)
        {
          CoreGlobals.ThreadException = ex;
        }
      }
    }

    private void ThreadedUpdate()
    {
      double num = 0.0;
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      while (this.run)
      {
        if (this.TriggerEvent != null)
          this.TriggerEvent.WaitOne();
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double elapsed = totalSeconds - num;
        num = totalSeconds;
        try
        {
          this.ThreadedUpdateCore(elapsed);
        }
        catch (Exception ex)
        {
          CoreGlobals.ThreadException = ex;
        }
        this.lastFrameTime = stopwatch.Elapsed.TotalSeconds - num;
        this.timeSmoothTimer += this.lastFrameTime;
        if (this.lastFrameTime > this.longestFrameTime || this.timeSmoothTimer > 2.0)
        {
          this.longestFrameTime = this.lastFrameTime;
          this.timeSmoothTimer = 0.0;
        }
        if (this.TriggerEvent != null)
          this.TriggerEvent.Reset();
      }
    }

    protected abstract void ThreadedUpdateCore(double elapsed);
  }
}
