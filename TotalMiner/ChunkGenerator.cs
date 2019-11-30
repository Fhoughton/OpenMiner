// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkGenerator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using System;

namespace StudioForge.TotalMiner
{
  internal class ChunkGenerator : ChunkGeneratorBase
  {
    public static StudioForge.Engine.Core.Pool<ChunkGenerator> Pool = new StudioForge.Engine.Core.Pool<ChunkGenerator>();

    public override string Name
    {
      get
      {
        return nameof (ChunkGenerator);
      }
    }

    protected override void UpdateCore()
    {
      try
      {
        this.biome.GenerateChunk(this.chunk);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(57, ex);
      }
      finally
      {
        ChunkGenerator.Pool.Release(this.poolHandle);
        this.chunk.GenerateEnd();
      }
    }
  }
}
