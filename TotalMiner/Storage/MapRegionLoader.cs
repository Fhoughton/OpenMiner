// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.MapRegionLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.IO;

namespace StudioForge.TotalMiner.Storage
{
  internal class MapRegionLoader
  {
    public void LoadRegion(MapRegionTM region, IProgressBar progress)
    {
      if (region == null)
        return;
      lock (Globals1.SaveSemaphore)
      {
        try
        {
          region.LastGoodSaveNum = -1;
          int saveVersion = Globals2.GameProperties.SaveGame.Header.SaveVersion;
          this.LoadRegionCore(region, saveVersion, progress);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(97, ex);
          throw ex;
        }
      }
    }

    private int GetRegionHashCode(MapRegion region, int version)
    {
      if (version < 120)
        return region.GetHashCodeOld();
      return region.GetHashCode();
    }

    private void LoadRegionCore(MapRegionTM region, int version, IProgressBar progress)
    {
      if (Globals2.GameProperties.SaveGame.DirNumber <= 0)
        return;
      int regionHashCode = this.GetRegionHashCode((MapRegion) region, version);
      string str = Globals2.GameProperties.SaveGame.MapFilePath + regionHashCode.ToString() + ".reg";
      try
      {
        string path = str;
        if (!FileSystem.IsFileExist(path))
          return;
        byte[] buffer;
        using (Stream stream = FileSystem.OpenRead(path))
        {
          buffer = new byte[stream.Length];
          stream.Read(buffer, 0, buffer.Length);
        }
        int num = 0;
        using (MemoryStream memoryStream = new MemoryStream(buffer, false))
        {
          using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
          {
            this.ReadRegion(reader, region, version, progress);
            region.LastGoodSaveNum = num;
          }
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(97, ex);
      }
    }

    private void ReadRegion(
      BinaryReader reader,
      MapRegionTM region,
      int version,
      IProgressBar progress)
    {
      if (reader.ReadInt32() != this.GetRegionHashCode((MapRegion) region, version) || reader.ReadInt32() != region.Offset.X || (reader.ReadInt32() != region.Offset.Y || reader.ReadInt32() != region.Offset.Z))
        return;
      int num = reader.ReadInt32();
      if (num > 0)
      {
        float increment = 1f / (float) num;
        for (int index = 0; index < num; ++index)
        {
          this.ReadChunk(reader, region, version);
          progress?.AddProgress(increment);
        }
      }
      else
        progress?.AddProgress(1f);
    }

    private MapChunk ReadChunk(BinaryReader reader, MapRegionTM region, int version)
    {
      int hash = reader.ReadInt32();
      MapChunk chunk;
      if (version < 85)
        chunk = region.GetChunk(new Point3D()
        {
          X = reader.ReadInt32(),
          Y = reader.ReadInt32(),
          Z = reader.ReadInt32()
        });
      else
        chunk = region.GetChunk(version >= 120 ? Point3D.FromHash(hash) : MapChunk.GetChunkOffsetOld(region.Map, hash));
      if (version < 66)
      {
        ChunkFlags chunkFlags1 = ChunkFlags.Generated;
        if (version > 63)
        {
          if (reader.ReadBoolean())
            chunkFlags1 |= ChunkFlags.Decorated;
        }
        else
          chunkFlags1 |= ChunkFlags.Decorated;
        if (version > 57 && reader.ReadBoolean())
          chunkFlags1 = chunkFlags1;
        ChunkFlags chunkFlags2;
        if (version > 63)
        {
          if (reader.ReadBoolean())
            chunkFlags2 = chunkFlags1 | ChunkFlags.UserEdited;
        }
        else
          chunkFlags2 = chunkFlags1 | ChunkFlags.UserEdited;
      }
      chunk.ReadData(reader, version, false);
      return chunk;
    }
  }
}
