// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkDecorator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using System;

namespace StudioForge.TotalMiner
{
  internal class ChunkDecorator : ChunkGeneratorBase
  {
    public static StudioForge.Engine.Core.Pool<ChunkDecorator> Pool = new StudioForge.Engine.Core.Pool<ChunkDecorator>();

    public override string Name
    {
      get
      {
        return nameof (ChunkDecorator);
      }
    }

    protected override void UpdateCore()
    {
      try
      {
        this.biome.DecorateChunk(this.chunk);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(95, ex);
      }
      finally
      {
        ChunkDecorator.Pool.Release(this.poolHandle);
        this.chunk.DecorateEnd();
      }
    }
  }
}
