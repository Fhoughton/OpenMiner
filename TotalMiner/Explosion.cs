// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Explosion
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal class Explosion : IThreadWorkItem
  {
    public static StudioForge.Engine.Core.Pool<Explosion> Pool = new StudioForge.Engine.Core.Pool<Explosion>();
    private int poolIndex;
    private GameInstance instance;
    private QueuedBlast blast;

    public string Name
    {
      get
      {
        return nameof (Explosion);
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

    public void Initialize(int poolIndex, GameInstance instance, QueuedBlast blast)
    {
      this.poolIndex = poolIndex;
      this.instance = instance;
      this.blast = blast;
    }

    public void Update()
    {
      try
      {
        this.instance.Map.CreateBlast(this.blast.Point, this.blast.Strength, this.blast.Radius, this.blast.Random, UpdateBlockMethod.Blast, false, this.blast.PlayerID, this.blast.Seed);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(61, ex);
      }
      finally
      {
        Explosion.Pool.Release(this.poolIndex);
        this.instance.BlastExploded(this.blast);
        this.instance.Map.Commit();
      }
    }
  }
}
