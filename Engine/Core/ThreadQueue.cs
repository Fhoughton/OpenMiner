// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.ThreadQueue
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace StudioForge.Engine.Core
{
  public class ThreadQueue
  {
    private Queue<IThreadWorkItem> workItems = new Queue<IThreadWorkItem>(50);
    private Queue<IThreadWorkItem> tempWorkItems = new Queue<IThreadWorkItem>(50);
    private object queueLock = new object();
    private int cantWaitItems;

    public string Name { get; set; }

    public int WorkItemsCount
    {
      get
      {
        lock (this.queueLock)
          return this.workItems.Count;
      }
    }

    public int WorkItemsCountNoLock
    {
      get
      {
        return this.workItems.Count;
      }
    }

    public IThreadWorkItem[] WorkItems
    {
      get
      {
        lock (this.queueLock)
          return this.workItems.ToArray();
      }
    }

    public bool CanWait
    {
      get
      {
        return this.cantWaitItems == 0;
      }
    }

    public void ClearQueue()
    {
      lock (this.queueLock)
        this.workItems.Clear();
      this.cantWaitItems = 0;
    }

    public bool ContainsItem(IThreadWorkItem item)
    {
      return this.workItems.Contains(item);
    }

    public bool QueueWorkItem(IThreadWorkItem item, bool testContains)
    {
      lock (this.queueLock)
      {
        if (testContains && this.workItems.Contains(item))
          return false;
        this.workItems.Enqueue(item);
        if (!item.CanWait)
          ++this.cantWaitItems;
        return true;
      }
    }

    public bool InsertWorkItem(IThreadWorkItem item, bool testContains)
    {
      lock (this.queueLock)
      {
        if (testContains && this.workItems.Contains(item))
          return false;
        this.tempWorkItems.Clear();
        this.tempWorkItems.Enqueue(item);
        if (!item.CanWait)
          ++this.cantWaitItems;
        while (this.workItems.Count > 0)
          this.tempWorkItems.Enqueue(this.workItems.Dequeue());
        Queue<IThreadWorkItem> workItems = this.workItems;
        this.workItems = this.tempWorkItems;
        this.tempWorkItems = workItems;
        return true;
      }
    }

    public bool InsertWorkItem(IThreadWorkItem item, int index, bool testContains)
    {
      lock (this.queueLock)
      {
        if (index >= this.workItems.Count)
          return this.QueueWorkItem(item, testContains);
        if (index == 0)
          return this.InsertWorkItem(item, testContains);
        if (testContains && this.workItems.Contains(item))
          return false;
        this.tempWorkItems.Clear();
        while (this.workItems.Count > 0 && this.tempWorkItems.Count < index)
          this.tempWorkItems.Enqueue(this.workItems.Dequeue());
        this.tempWorkItems.Enqueue(item);
        if (!item.CanWait)
          ++this.cantWaitItems;
        while (this.workItems.Count > 0)
          this.tempWorkItems.Enqueue(this.workItems.Dequeue());
        Queue<IThreadWorkItem> workItems = this.workItems;
        this.workItems = this.tempWorkItems;
        this.tempWorkItems = workItems;
        return true;
      }
    }

    public bool InsertWorkItem(List<IThreadWorkItem> items, bool testContains)
    {
      lock (this.queueLock)
      {
        if (testContains)
        {
          foreach (IThreadWorkItem threadWorkItem in items)
          {
            if (this.workItems.Contains(threadWorkItem))
              return false;
          }
        }
        this.tempWorkItems.Clear();
        foreach (IThreadWorkItem threadWorkItem in items)
        {
          this.tempWorkItems.Enqueue(threadWorkItem);
          if (!threadWorkItem.CanWait)
            ++this.cantWaitItems;
        }
        while (this.workItems.Count > 0)
          this.tempWorkItems.Enqueue(this.workItems.Dequeue());
        Queue<IThreadWorkItem> workItems = this.workItems;
        this.workItems = this.tempWorkItems;
        this.tempWorkItems = workItems;
        return true;
      }
    }

    public IThreadWorkItem GetNextWorkItem()
    {
      if (Monitor.TryEnter(this.queueLock))
      {
        try
        {
          int count = this.workItems.Count;
          if (count > 0)
          {
            if (!this.workItems.Peek().IsSleeping)
            {
              IThreadWorkItem threadWorkItem = this.workItems.Dequeue();
              if (!threadWorkItem.CanWait)
                --this.cantWaitItems;
              return threadWorkItem;
            }
            if (count > 1)
            {
              IThreadWorkItem threadWorkItem1 = this.workItems.Dequeue();
              this.workItems.Enqueue(threadWorkItem1);
              IThreadWorkItem threadWorkItem2;
              for (threadWorkItem2 = this.workItems.Peek(); threadWorkItem2.IsSleeping && threadWorkItem2 != threadWorkItem1; threadWorkItem2 = this.workItems.Peek())
                this.workItems.Enqueue(this.workItems.Dequeue());
              if (threadWorkItem2 != threadWorkItem1)
              {
                IThreadWorkItem threadWorkItem3 = this.workItems.Dequeue();
                if (!threadWorkItem3.CanWait)
                  --this.cantWaitItems;
                return threadWorkItem3;
              }
            }
          }
        }
        catch (Exception ex)
        {
          return (IThreadWorkItem) null;
        }
        finally
        {
          Monitor.Exit(this.queueLock);
        }
      }
      return (IThreadWorkItem) null;
    }

    public void CancelWorkItem(IThreadWorkItem item)
    {
      lock (this.queueLock)
      {
        if (!this.workItems.Contains(item))
          return;
        this.cantWaitItems = 0;
        Queue<IThreadWorkItem> threadWorkItemQueue = new Queue<IThreadWorkItem>();
        while (this.workItems.Count > 0)
        {
          IThreadWorkItem threadWorkItem = this.workItems.Dequeue();
          if (threadWorkItem != item)
          {
            threadWorkItemQueue.Enqueue(threadWorkItem);
            if (!threadWorkItem.CanWait)
              ++this.cantWaitItems;
          }
        }
        this.workItems = threadWorkItemQueue;
      }
    }
  }
}
