// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.ThreadQueueManager
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;

namespace StudioForge.Engine.Core
{
  public class ThreadQueueManager
  {
    private static ThreadQueueManager instance;
    private ThreadQueue PriorityQueue;
    private ThreadQueue MainQueue;
    private List<ThreadedQueueWorker> WorkerThreads;
    private float processorSpeedScale;
    private float processorCountScale;

    public static ThreadQueueManager Instance
    {
      get
      {
        return ThreadQueueManager.instance ?? (ThreadQueueManager.instance = new ThreadQueueManager());
      }
    }

    public bool PriorityQueueValid
    {
      get
      {
        return this.PriorityQueue != null;
      }
    }

    public bool MainQueueValid
    {
      get
      {
        return this.MainQueue != null;
      }
    }

    public IThreadWorkItem[] PriorityWorkItems
    {
      get
      {
        return this.PriorityQueue.WorkItems;
      }
    }

    public int PriorityWorkItemCount
    {
      get
      {
        return this.PriorityQueue.WorkItemsCount;
      }
    }

    public IThreadWorkItem[] MainWorkItems
    {
      get
      {
        return this.MainQueue.WorkItems;
      }
    }

    public int MainWorkItemCount
    {
      get
      {
        return this.MainQueue.WorkItemsCount;
      }
    }

    public void InitWorkerThreads()
    {
      this.InitWorkerThreads(Math.Max(2, Environment.ProcessorCount - 2));
    }

    public void InitWorkerThreads(int processors)
    {
      if (processors < 1)
        processors = 1;
      this.WorkerThreads = new List<ThreadedQueueWorker>();
      for (int id = 0; id < processors; ++id)
        this.WorkerThreads.Add(new ThreadedQueueWorker(id));
      this.PriorityQueue = new ThreadQueue()
      {
        Name = "PriorityQueue"
      };
      this.MainQueue = new ThreadQueue()
      {
        Name = "MainQueue"
      };
      this.processorCountScale = MathHelper.Lerp(0.0f, 1f, Math.Min(1f, (float) processors / 4f));
      this.processorSpeedScale = MathHelper.Lerp(0.0f, 1f, (float) (Math.Max(1800U, Math.Min(this.GetCPUSpeed(), 3400U)) - 1800U) / 1600f);
    }

    public void StartWorkerThreads(ThreadedWorkerQueueQuery canRunItemOnQueue2)
    {
      for (int index = 0; index < this.WorkerThreads.Count; ++index)
      {
        if (index == 0)
        {
          this.WorkerThreads[index].Start(this.PriorityQueue, this.MainQueue, canRunItemOnQueue2);
          this.WorkerThreads[index].IsBackground = true;
          this.WorkerThreads[index].Priority = ThreadPriority.Highest;
        }
        else
        {
          this.WorkerThreads[index].Start(this.MainQueue, this.PriorityQueue, (ThreadedWorkerQueueQuery) null);
          this.WorkerThreads[index].IsBackground = true;
          this.WorkerThreads[index].Priority = ThreadPriority.Lowest;
        }
      }
    }

    public void ClearAllQueuesAndThreads()
    {
      if (this.MainQueue != null)
        this.MainQueue.ClearQueue();
      if (this.PriorityQueue != null)
        this.PriorityQueue.ClearQueue();
      foreach (ThreadedQueueWorker workerThread in this.WorkerThreads)
      {
        try
        {
          workerThread.End(50);
        }
        catch (ThreadAbortException ex)
        {
        }
      }
      this.WorkerThreads.Clear();
    }

    public bool QueueWorkItem(IThreadWorkItem item, bool testContains, PriorityLevel priority)
    {
      if (priority != PriorityLevel.Urgent && priority != PriorityLevel.Priority)
        return this.MainQueue.QueueWorkItem(item, testContains);
      if (priority == PriorityLevel.Urgent && this.MainQueue == this.PriorityQueue)
        return this.PriorityQueue.InsertWorkItem(item, testContains);
      return this.PriorityQueue.QueueWorkItem(item, testContains);
    }

    public bool QueueContainsItem(IThreadWorkItem item, PriorityLevel priority)
    {
      if (priority == PriorityLevel.Priority || priority == PriorityLevel.Urgent)
        return this.PriorityQueue.ContainsItem(item);
      return this.MainQueue.ContainsItem(item);
    }

    public void CancelQueueItem(IThreadWorkItem item, PriorityLevel priority)
    {
      if (priority == PriorityLevel.Priority || priority == PriorityLevel.Urgent)
        this.PriorityQueue.CancelWorkItem(item);
      else
        this.MainQueue.CancelWorkItem(item);
    }

    public void CancelQueueItem(IThreadWorkItem item)
    {
      this.PriorityQueue.CancelWorkItem(item);
      if (this.PriorityQueue == this.MainQueue)
        return;
      this.MainQueue.CancelWorkItem(item);
    }

    public void SuspendWorkerThreads()
    {
      for (int index = 0; index < this.WorkerThreads.Count; ++index)
        this.WorkerThreads[index].IsSuspended = true;
    }

    private bool AreWorkerThreadsSuspended
    {
      get
      {
        for (int index = 0; index < this.WorkerThreads.Count; ++index)
        {
          if (!this.WorkerThreads[index].IsSuspended || this.WorkerThreads[index].IsExecutingItem)
            return false;
        }
        return true;
      }
    }

    public void SuspendWorkerThreadsAndWait(int milliseconds)
    {
      ThreadedWorkerBase.SuspendMilliSeconds = milliseconds;
      this.WaitForWorkersToSuspend();
    }

    public void ResumeWorkerThreads()
    {
      ThreadedWorkerBase.SuspendMilliSeconds = 0;
      for (int index = 0; index < this.WorkerThreads.Count; ++index)
        this.WorkerThreads[index].IsSuspended = false;
    }

    private void WaitForWorkersToSuspend()
    {
      do
        ;
      while (!this.AreWorkerThreadsSuspended);
    }

    public int GetProcessorSpeedScale(int min, int max)
    {
      return (int) MathHelper.Lerp((float) min, (float) max, this.processorSpeedScale);
    }

    public int GetProcessorCountScale(int min, int max)
    {
      return (int) MathHelper.Lerp((float) min, (float) max, this.processorCountScale);
    }

    public int GetProcessorScale(int min, int max)
    {
      return (int) MathHelper.Lerp((float) min, (float) max, (float) (((double) this.processorCountScale + (double) this.processorSpeedScale) * 0.5));
    }

    private uint GetCPUSpeed()
    {
      using (ManagementObject managementObject = new ManagementObject("Win32_Processor.DeviceID='CPU0'"))
        return (uint) managementObject["MaxClockSpeed"];
    }
  }
}
