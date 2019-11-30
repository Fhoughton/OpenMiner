// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.FireUpdateWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Blocks;
using System.Collections.Generic;
using System.Diagnostics;

namespace StudioForge.TotalMiner
{
  internal class FireUpdateWorker : IThreadWorkItem
  {
    private FireBlock[] blocksToUpdate = new FireBlock[100];
    private List<GlobalPoint3D> empty = new List<GlobalPoint3D>(10);
    private const int maxFireToProcess = 100;
    private PriorityLevel priority;
    private GameInstance instance;
    private MapTM map;
    private bool commitMap;
    private float burnFac;
    private int currentIndex;
    private Stopwatch watch;

    public string Name
    {
      get
      {
        return "FireUpdater";
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

    public FireUpdateWorker(GameInstance instance, PriorityLevel priority)
    {
      this.instance = instance;
      this.priority = priority;
      this.map = instance.Map;
    }

    public void Update()
    {
      try
      {
        if (!this.instance.IsMapActive || !this.instance.IsFiniteResources)
          return;
        this.UpdateCore();
      }
      finally
      {
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
      }
    }

    private void UpdateCore()
    {
      this.commitMap = false;
      if (this.watch == null)
      {
        this.watch = new Stopwatch();
        this.watch.Start();
      }
      this.SpreadFire(this.instance.MapStrategyTM.BurningFireBlocks);
      if (!this.commitMap)
        return;
      this.map.Commit();
    }

    private void SpreadFire(List<FireBlock> fire)
    {
      int num = 0;
      float totalToProcessRatio = 1f;
      lock (fire)
      {
        if (fire.Count > 0)
        {
          if (this.currentIndex >= fire.Count)
            this.currentIndex = 0;
          for (int currentIndex = this.currentIndex; currentIndex < fire.Count && num < 100; ++currentIndex)
            this.blocksToUpdate[num++] = fire[currentIndex];
          totalToProcessRatio = fire.Count > 100 ? 100f / (float) fire.Count : 1f;
        }
      }
      long elapsedMilliseconds = this.watch.ElapsedMilliseconds;
      for (int index = 0; index < num; ++index)
      {
        this.UpdateFire(this.blocksToUpdate[index], totalToProcessRatio);
        ++this.currentIndex;
        if (this.watch.ElapsedMilliseconds - elapsedMilliseconds > 10L)
          break;
      }
    }

    private void UpdateFire(FireBlock fireBlock, float totalToProcessRatio)
    {
      int num1 = 0;
      this.burnFac = 1f;
      if (fireBlock.LastElapsed > 0.0)
      {
        float num2 = (float) (this.watch.Elapsed.TotalSeconds - fireBlock.LastElapsed);
        fireBlock.SecondsAlive += num2;
        int num3 = this.BurnFace(fireBlock, GlobalPoint3D.Left);
        if (num3 > num1)
          num1 = num3;
        int num4 = this.BurnFace(fireBlock, GlobalPoint3D.Forward);
        if (num4 > num1)
          num1 = num4;
        int num5 = this.BurnFace(fireBlock, GlobalPoint3D.Right);
        if (num5 > num1)
          num1 = num5;
        int num6 = this.BurnFace(fireBlock, GlobalPoint3D.Backward);
        if (num6 > num1)
          num1 = num6;
        int num7 = this.BurnFace(fireBlock, GlobalPoint3D.Up);
        if (num7 > num1)
          num1 = num7;
        int num8 = this.BurnFace(fireBlock, GlobalPoint3D.Down);
        if (num8 > num1)
          num1 = num8;
        if ((double) fireBlock.SecondsAlive > (double) num1 * (double) this.burnFac)
        {
          this.map.SetBlockData(fireBlock.Point, (byte) 0, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
          this.commitMap = true;
        }
      }
      fireBlock.LastElapsed = this.watch.Elapsed.TotalSeconds;
    }

    private int BurnFace(FireBlock fireBlock, GlobalPoint3D offset)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = fireBlock.Point.X + offset.X;
      p.Y = fireBlock.Point.Y + offset.Y;
      p.Z = fireBlock.Point.Z + offset.Z;
      ushort num1 = 0;
      Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p);
      if (blockIdNoCache != Block.None)
      {
        num1 = ItemData2.GetBurnTime(this.map, p, (Item) blockIdNoCache);
        if (num1 > (ushort) 0)
        {
          if ((double) fireBlock.SecondsAlive >= (double) num1 * (double) this.burnFac * 0.5 && fireBlock.SpreadCount < (byte) 4 && this.SpreadFromFace(p))
            ++fireBlock.SpreadCount;
          if ((double) fireBlock.SecondsAlive > (double) num1 * (double) this.burnFac)
          {
            int num2 = (int) this.map.ClearBlock(p, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
            this.commitMap = true;
          }
        }
      }
      return (int) num1;
    }

    private bool SpreadFromFace(GlobalPoint3D p)
    {
      return this.SpreadToFace(p, GlobalPoint3D.Left) || this.SpreadToFace(p, GlobalPoint3D.Right) || (this.SpreadToFace(p, GlobalPoint3D.Backward) || this.SpreadToFace(p, GlobalPoint3D.Forward)) || (this.SpreadToFace(p, GlobalPoint3D.Up) || this.SpreadToFace(p, GlobalPoint3D.Down));
    }

    private bool SpreadToFace(GlobalPoint3D p, GlobalPoint3D offset)
    {
      p.X += offset.X;
      p.Y += offset.Y;
      p.Z += offset.Z;
      Block blockIdNoCache = (Block) this.map.GetBlockIDNoCache(p);
      if (blockIdNoCache != Block.None && ItemData2.GetBurnTime(this.map, p, (Item) blockIdNoCache) > (ushort) 0)
      {
        this.empty.Clear();
        this.map.FindEmptyBlocks(p, 1, 0, this.empty);
        if (this.empty.Count > 0)
        {
          GlobalPoint3D p1 = this.empty[this.map.Random.Next(this.empty.Count)];
          switch (this.instance.Map.GetClearBlockResult(p1, UpdateBlockMethod.Strategy, GamerID.Sys1))
          {
            case ClearBlockResult.Success:
            case ClearBlockResult.AlreadyClear:
              this.instance.Map.SetBlockData(p1, (byte) 118, (byte) 1, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
              this.commitMap = true;
              return true;
          }
        }
      }
      return false;
    }
  }
}
