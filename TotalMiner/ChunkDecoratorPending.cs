// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkDecoratorPending
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal class ChunkDecoratorPending : IThreadWorkItem
  {
    public static StudioForge.Engine.Core.Pool<ChunkDecoratorPending> Pool = new StudioForge.Engine.Core.Pool<ChunkDecoratorPending>();
    private MapChunkTM chunk;
    private int poolHandle;

    public string Name
    {
      get
      {
        return nameof (ChunkDecoratorPending);
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

    public void Initialize(int poolHandle, MapChunkTM chunk)
    {
      this.poolHandle = poolHandle;
      this.chunk = chunk;
    }

    public void Update()
    {
      try
      {
        this.chunk.DecoratePendingChunkData();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(104, ex);
      }
      finally
      {
        this.chunk.DecoratePendingEnd();
        ChunkDecoratorPending.Pool.Release(this.poolHandle);
      }
    }
  }
}
