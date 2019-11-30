// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.EnvironmentWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class EnvironmentWorker : TimedThreadWorkItem
  {
    private GameInstance instance;
    private List<EnvironmentWorker.SortKey> sorter;
    private Stack<ushort> hashes;

    public override string Name
    {
      get
      {
        return nameof (EnvironmentWorker);
      }
    }

    public EnvironmentWorker(GameInstance instance, PriorityLevel priority)
      : base(priority, 100)
    {
      this.instance = instance;
      GlobalPoint3D mapSize = instance.Map.MapSize;
      Point3D chunkSize = instance.Map.ChunkSize;
      this.sorter = new List<EnvironmentWorker.SortKey>(mapSize.X / chunkSize.X * mapSize.Z / chunkSize.Z);
      this.hashes = new Stack<ushort>(this.sorter.Count);
      this.ResetUpdateStack();
    }

    private void ResetUpdateStack()
    {
      for (ushort index = 0; (int) index < this.sorter.Capacity; ++index)
        this.sorter.Add(new EnvironmentWorker.SortKey()
        {
          Index = index,
          Key = this.GetNewSortKey((int) index)
        });
      this.sorter.Sort(new Comparison<EnvironmentWorker.SortKey>(this.SortKeys));
      foreach (EnvironmentWorker.SortKey sortKey in this.sorter)
        this.hashes.Push(sortKey.Index);
      this.sorter.Clear();
    }

    private int GetNewSortKey(int i)
    {
      return this.instance.Map.Random.Next();
    }

    private int SortKeys(EnvironmentWorker.SortKey k1, EnvironmentWorker.SortKey k2)
    {
      return k1.Key.CompareTo(k2.Key);
    }

    protected override void UpdateCore()
    {
      if (!this.instance.IsMapActive)
        return;
      if (!this.instance.IsFiniteResources)
        return;
      try
      {
        for (int index = 0; index < 1; ++index)
          this.UpdateEnvironment();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(25, ex);
      }
    }

    private void UpdateEnvironment()
    {
      MapTM map = this.instance.Map;
      ushort num1 = this.hashes.Pop();
      if (this.hashes.Count < 1)
        this.ResetUpdateStack();
      GlobalPoint3D mapSize = this.instance.Map.MapSize;
      Point3D chunkSize = map.ChunkSize;
      int num2 = (int) num1 % (mapSize.X / chunkSize.X);
      int num3 = (int) num1 / (mapSize.Z / chunkSize.Z);
      GlobalPoint3D p = new GlobalPoint3D(num2 * chunkSize.X, 0, num3 * chunkSize.Z);
      p.Y = (int) map.GetHeight(p);
      MapChunk chunk = map.GetChunk(p);
      if (!chunk.IsDecorated)
        return;
      GlobalPoint3D globalOffset = chunk.GlobalOffset;
      bool flag1 = false;
      bool flag2 = this.instance.SunMoon.Season != SeasonType.Winter;
      for (int index1 = 0; index1 < chunkSize.X; ++index1)
      {
        p.X = globalOffset.X + index1;
        for (int index2 = 0; index2 < chunkSize.Z; ++index2)
        {
          if (flag2 || map.Random.Next(2) == 0)
          {
            p.Z = globalOffset.Z + index2;
            p.Y = (int) map.GetHeight(p);
            Block blockIdNoCache = (Block) map.GetBlockIDNoCache(p);
            if (flag2)
            {
              if (blockIdNoCache == Block.SnowLayer)
              {
                map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
                flag1 = true;
              }
            }
            else if (blockIdNoCache != Block.SnowLayer && !map.IsBlockIcon((byte) blockIdNoCache))
            {
              ++p.Y;
              if (p.Y < map.MapBound.Max.Y)
              {
                map.SetBlockData(p, (byte) 145, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true);
                flag1 = true;
              }
            }
          }
        }
      }
      if (!flag1)
        return;
      map.Commit();
    }

    private struct SortKey
    {
      public ushort Index;
      public int Key;
    }
  }
}
