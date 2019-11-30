// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.ThreadedQueueWorker
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.Threading;

namespace StudioForge.Engine.Core
{
  public class ThreadedQueueWorker : ThreadedWorkerBase
  {
    private ThreadQueue queue1;
    private ThreadQueue queue2;
    private ThreadedWorkerQueueQuery canRunItemOnQueue2;

    public ThreadedQueueWorker(int id)
      : base(id)
    {
    }

    public void Start(
      ThreadQueue queue1,
      ThreadQueue queue2,
      ThreadedWorkerQueueQuery canRunItemOnQueue2)
    {
      this.queue1 = queue1;
      this.queue2 = queue2;
      this.canRunItemOnQueue2 = canRunItemOnQueue2;
      this.Start();
    }

    protected override void ThreadedUpdateCore()
    {
      bool canWait = this.queue1.CanWait;
      bool flag = this.queue2 == null || this.queue2.CanWait;
      if (canWait && flag)
        Thread.Sleep(1);
      IThreadWorkItem nextWorkItem1 = this.queue1.GetNextWorkItem();
      if (nextWorkItem1 != null)
      {
        this.UpdateItem(this.queue1, this.id, nextWorkItem1);
      }
      else
      {
        if (this.queue2 == null)
          return;
        IThreadWorkItem nextWorkItem2 = this.queue2.GetNextWorkItem();
        if (nextWorkItem2 == null)
          return;
        if (this.canRunItemOnQueue2 == null || this.canRunItemOnQueue2(nextWorkItem2))
        {
          this.UpdateItem(this.queue2, this.id, nextWorkItem2);
        }
        else
        {
          if (this.canRunItemOnQueue2 == null)
            return;
          this.queue2.InsertWorkItem(nextWorkItem2, false);
        }
      }
    }

    protected void UpdateItem(ThreadQueue queue, int id, IThreadWorkItem item)
    {
      this.isExecutingItem = true;
      try
      {
        this.OnItemUpdateExecuted(queue, id, item);
        item.Update();
      }
      finally
      {
        this.isExecutingItem = false;
      }
    }

    protected virtual void OnItemUpdateExecuted(
      ThreadQueue queue,
      int workerID,
      IThreadWorkItem item)
    {
    }
  }
}
