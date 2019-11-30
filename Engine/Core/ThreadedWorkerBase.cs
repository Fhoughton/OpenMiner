// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.ThreadedWorkerBase
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace StudioForge.Engine.Core
{
  public abstract class ThreadedWorkerBase
  {
    public static int SuspendMilliSeconds;
    protected int id;
    protected bool run;
    protected bool isSuspended;
    protected bool isSuspendedForTime;
    protected Thread thread;
    protected bool isExecutingItem;

    public bool IsStarted
    {
      get
      {
        if (this.thread != null)
          return this.thread.IsAlive;
        return false;
      }
    }

    public bool IsSuspended
    {
      get
      {
        if (!this.isSuspended)
          return this.isSuspendedForTime;
        return true;
      }
      set
      {
        this.isSuspended = value;
      }
    }

    public bool IsExecutingItem
    {
      get
      {
        return this.isExecutingItem;
      }
    }

    public bool IsBackground
    {
      get
      {
        if (this.thread == null)
          return false;
        return this.thread.IsBackground;
      }
      set
      {
        if (this.thread == null)
          return;
        this.thread.IsBackground = value;
      }
    }

    public ThreadPriority Priority
    {
      get
      {
        if (this.thread == null)
          return ThreadPriority.Normal;
        return this.thread.Priority;
      }
      set
      {
        if (this.thread == null)
          return;
        this.thread.Priority = value;
      }
    }

    public ThreadedWorkerBase(int id)
    {
      this.id = id;
    }

    public void Start()
    {
      this.run = true;
      this.thread = new Thread(new ThreadStart(this.Update));
      this.thread.Start();
    }

    private void Update()
    {
      while (this.run)
      {
        if (ThreadedWorkerBase.SuspendMilliSeconds > 0)
        {
          this.isSuspendedForTime = true;
          Thread.Sleep(ThreadedWorkerBase.SuspendMilliSeconds);
        }
        else
        {
          try
          {
            this.isSuspendedForTime = false;
            if (!this.isSuspended)
              this.ThreadedUpdateCore();
          }
          catch (Exception ex)
          {
            CoreGlobals.ThreadException = ex;
          }
        }
      }
    }

    protected abstract void ThreadedUpdateCore();

    public virtual void End(int millisecsWait)
    {
      if (!this.run)
        return;
      this.run = false;
      if (millisecsWait == 0)
      {
        if (this.IsStarted)
          this.thread.Abort();
      }
      else
      {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        while (this.thread.IsAlive)
        {
          if (stopwatch.ElapsedMilliseconds > (long) millisecsWait)
          {
            this.thread.Abort();
            break;
          }
        }
      }
      this.thread = (Thread) null;
    }
  }
}
