// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.RLEStreamByte
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;
using System.IO;

namespace StudioForge.BlockWorld
{
  public class RLEStreamByte
  {
    public static Pool<CustomArray<byte>> WorkStreamPool = new Pool<CustomArray<byte>>();
    public short StreamID = -1;
    public short CacheID = -1;
    public int TimeStamp;
    public int StreamIndex;
    public int StreamSize;
    public int BufferSize;
    public int CacheIndex;

    public int MemorySize
    {
      get
      {
        return 28;
      }
    }

    protected RLEStreamByte()
    {
    }

    public RLEStreamByte(MapChunk chunk, byte data)
    {
      this.Fill(chunk, data);
    }

    public void UnloadContent(MapChunk chunk)
    {
      lock (chunk.RleLock)
      {
        this.ReleaseCacheNoLock(chunk);
        Map.RLEStreamBufferManager.Release(this);
      }
    }

    public byte GetData(MapChunk chunk, int mapIndex)
    {
      this.TimeStamp = chunk.Region.Map.TimeStampRleCache;
      lock (chunk.RleLock)
      {
        if (this.CacheID == (short) -1)
          this.Cache(chunk, false);
        return chunk.Region.Map.ChunkCacheManager.Cache[(int) this.CacheID][this.CacheIndex + mapIndex];
      }
    }

    public byte GetData_Test(MapChunk chunk, int mapIndex)
    {
      this.TimeStamp = chunk.Region.Map.TimeStampRleCache;
      lock (chunk.RleLock)
      {
        if (this.CacheID == (short) -1)
          this.Cache(chunk, false);
        return chunk.Region.Map.ChunkCacheManager.Cache[(int) this.CacheID][this.CacheIndex + mapIndex];
      }
    }

    public byte GetDataNoCache(MapChunk chunk, int mapIndex)
    {
      lock (chunk.RleLock)
      {
        if (this.CacheID != (short) -1)
          return chunk.Region.Map.ChunkCacheManager.Cache[(int) this.CacheID][this.CacheIndex + mapIndex];
        if (this.StreamSize == 2)
          return Map.RLEStreamBufferManager.Stream[(int) this.StreamID][this.StreamIndex + 1];
        return this.GetStreamDataNoLock(this.GetStreamIndex(mapIndex));
      }
    }

    public byte GetStreamDataNoLock(int index)
    {
      if (index < this.StreamSize)
        return Map.RLEStreamBufferManager.Stream[(int) this.StreamID][this.StreamIndex + index];
      return 0;
    }

    public bool Contains(MapChunk chunk, byte value)
    {
      lock (chunk.RleLock)
      {
        if (this.CacheID == (short) -1)
          return this.ContainsInStream(value);
        return this.ContainsInCache(chunk, value);
      }
    }

    protected int GetStreamIndex(int mapIndex)
    {
      if (this.StreamSize < 1)
        return 0;
      byte[][] stream = Map.RLEStreamBufferManager.Stream;
      int num1 = (int) stream[(int) this.StreamID][this.StreamIndex] + 1;
      int num2;
      int num3;
      for (num2 = 1; num1 <= mapIndex && num2 < this.StreamSize - 2; num2 = num3 + 1)
      {
        num3 = num2 + 1;
        num1 += (int) stream[(int) this.StreamID][this.StreamIndex + num3] + 1;
      }
      return num2;
    }

    private int GetStreamIndexNew(int mapIndex)
    {
      if (this.StreamSize < 1)
        return 0;
      int maxValue = (int) sbyte.MaxValue;
      byte[] numArray = Map.RLEStreamBufferManager.Stream[(int) this.StreamID];
      int streamIndex = this.StreamIndex;
      int num1 = 0;
      int num2 = this.StreamSize - 2;
      while (streamIndex - this.StreamIndex < num2)
      {
        int num3 = (int) numArray[streamIndex];
        if (num3 <= maxValue)
        {
          int num4 = num1 + num3 + 1;
          if (num4 >= mapIndex)
            return streamIndex + 1;
          num1 = num4;
          streamIndex += 2;
        }
        else
        {
          int num4 = num3 - maxValue;
          int num5 = num1 + num4;
          if (num5 >= mapIndex)
            return streamIndex + (mapIndex - num1);
          num1 = num5;
          streamIndex += num4 + 1;
        }
      }
      return this.StreamSize - 1;
    }

    private bool ContainsInStream(byte value)
    {
      byte[][] stream = Map.RLEStreamBufferManager.Stream;
      for (int index = 0; index < this.StreamSize; index += 2)
      {
        if ((int) stream[(int) this.StreamID][this.StreamIndex + index + 1] == (int) value)
          return true;
      }
      return false;
    }

    private bool ContainsInCache(MapChunk chunk, byte value)
    {
      bool flag = false;
      int num = this.CacheIndex + chunk.Region.Map.ChunkLength;
      byte[] numArray = chunk.Region.Map.ChunkCacheManager.Cache[(int) this.CacheID];
      if (numArray != null)
      {
        for (int cacheIndex = this.CacheIndex; cacheIndex < num; ++cacheIndex)
        {
          if ((int) numArray[cacheIndex] == (int) value)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public void SetData(MapChunk chunk, int mapIndex, byte value)
    {
      lock (chunk.RleLock)
        this.SetDataNoLock(chunk, mapIndex, value);
    }

    public void SetDataNoLock(MapChunk chunk, int mapIndex, byte value)
    {
      if (this.CacheID == (short) -1)
      {
        if (this.StreamSize == 2 && (int) Map.RLEStreamBufferManager.Stream[(int) this.StreamID][this.StreamIndex + 1] == (int) value)
          return;
        this.Cache(chunk, false);
      }
      this.TimeStamp = chunk.Region.Map.TimeStampRleCache;
      chunk.Region.Map.ChunkCacheManager.Cache[(int) this.CacheID][this.CacheIndex + mapIndex] = value;
    }

    public void SetStream(MapChunk chunk, byte[] stream)
    {
      lock (chunk.RleLock)
      {
        this.ReleaseCacheNoLock(chunk);
        Map.RLEStreamBufferManager.Allocate(this, stream.Length);
        Array.Copy((Array) stream, 0, (Array) Map.RLEStreamBufferManager.Stream[(int) this.StreamID], this.StreamIndex, this.StreamSize);
      }
    }

    public void SetStream(MapChunk chunk, short newCacheID, int newCacheIndex)
    {
      Map map = chunk.Region.Map;
      lock (chunk.RleLock)
      {
        if ((int) newCacheID != (int) this.CacheID || newCacheIndex != this.CacheIndex)
          this.ReleaseCacheNoLock(chunk);
        short streamID;
        int streamIndex;
        int streamSize;
        RLEStreamByte.CompressToNewStreamNoLock(map.ChunkCacheManager.Cache[(int) newCacheID], newCacheIndex, map.ChunkLength, out streamID, out streamIndex, out streamSize);
        this.SetStream(chunk, streamID, streamIndex, streamSize);
      }
    }

    public void SetStream(MapChunk chunk, short streamID, int streamIndex, int streamSize)
    {
      if (streamID < (short) 0 || streamSize <= 0)
        return;
      lock (chunk.RleLock)
      {
        this.ReleaseCacheNoLock(chunk);
        if ((int) streamID != (int) this.StreamID || streamIndex != this.StreamIndex)
        {
          Map.RLEStreamBufferManager.Release(this);
          this.StreamID = streamID;
          this.StreamIndex = streamIndex;
          this.StreamSize = streamSize;
          this.BufferSize = streamSize;
        }
        else
          this.StreamSize = streamSize;
      }
    }

    public void UpdateStream(MapChunk chunk)
    {
      Map map = chunk.Region.Map;
      lock (chunk.RleLock)
      {
        if (this.CacheID < (short) 0)
          return;
        this.CompressToLocalStreamNoLock(map.ChunkCacheManager.Cache[(int) this.CacheID], this.CacheIndex, map.ChunkLength);
      }
    }

    public void GetCacheAndAddRefCount(MapChunk chunk, out short outCacheID, out int outCacheIndex)
    {
      this.TimeStamp = chunk.Region.Map.TimeStampRleCache;
      lock (chunk.RleLock)
      {
        if (this.CacheID == (short) -1)
          this.Cache(chunk, true);
        else
          chunk.Region.Map.ChunkCacheManager.AddRefCount(this.CacheID, this.CacheIndex);
        outCacheID = this.CacheID;
        outCacheIndex = this.CacheIndex;
      }
    }

    public void SetCache(MapChunk chunk, short newCacheID, int newCacheIndex)
    {
      lock (chunk.RleLock)
      {
        if (!chunk.Region.Map.ChunkCacheManager.SetCacheStream(chunk, this, newCacheID, newCacheIndex))
          return;
        this.CacheID = newCacheID;
        this.CacheIndex = newCacheIndex;
        this.TimeStamp = chunk.Region.Map.TimeStampRleCache;
      }
    }

    public void ReleaseCacheToStream(MapChunk chunk)
    {
      lock (chunk.RleLock)
      {
        if (this.CacheID < (short) 0)
          return;
        this.UpdateStream(chunk);
        this.ReleaseCacheNoLock(chunk);
      }
    }

    private void Cache(MapChunk chunk, bool addRefCount)
    {
      Map map = chunk.Region.Map;
      short cacheID;
      int cacheIndex;
      map.ChunkCacheManager.AcquireCache(chunk, this, addRefCount, out cacheID, out cacheIndex);
      RLEStreamByte.Uncompress(map, map.ChunkCacheManager.Cache[(int) cacheID], cacheIndex, this.StreamID, this.StreamIndex, this.StreamSize);
      this.CacheID = cacheID;
      this.CacheIndex = cacheIndex;
    }

    private void ReleaseCacheNoLock(MapChunk chunk)
    {
      if (this.CacheID < (short) 0)
        return;
      chunk.Region.Map.ChunkCacheManager.ReleaseCacheStream(this.CacheID, this.CacheIndex);
      this.CacheID = (short) -1;
    }

    public static void Uncompress(
      Map map,
      byte[] cache,
      int cacheIndex,
      short streamID,
      int streamIndex,
      int streamSize)
    {
      RLEStreamByte.UncompressOld(map, cache, cacheIndex, streamID, streamIndex, streamSize);
    }

    public static void UncompressNew(
      Map map,
      byte[] cache,
      int cacheIndex,
      short streamID,
      int streamIndex,
      int streamSize)
    {
      byte num1 = 127;
      int num2 = 0;
      int num3 = cacheIndex;
      byte[] numArray = Map.RLEStreamBufferManager.Stream[(int) streamID];
      while (num2 < streamSize)
      {
        int index1 = streamIndex + num2;
        int num4 = (int) numArray[index1];
        int index2 = index1 + 1;
        if (num4 > (int) num1)
        {
          int num5 = num4 - (int) num1;
          for (int index3 = 0; index3 < num5; ++index3)
            cache[num3++] = numArray[index2 + index3];
          num2 += 1 + num5;
        }
        else
        {
          int num5 = num2 != streamSize - 2 ? num4 + 1 : map.ChunkLength - (num3 - cacheIndex);
          byte num6 = numArray[index2];
          for (; num5 > 0; --num5)
            cache[num3++] = num6;
          num2 += 2;
        }
      }
      if (num3 - cacheIndex != map.ChunkLength)
        throw new Exception("bad uncompress");
    }

    public static void UncompressOld(
      Map map,
      byte[] cache,
      int cacheIndex,
      short streamID,
      int streamIndex,
      int streamSize)
    {
      int num1 = 0;
      int num2 = cacheIndex;
      byte[] numArray = Map.RLEStreamBufferManager.Stream[(int) streamID];
      for (; num1 < streamSize - 2; num1 += 2)
      {
        int index1 = streamIndex + num1;
        int num3 = (int) numArray[index1] + 1;
        int index2 = index1 + 1;
        byte num4 = numArray[index2];
        while (num3-- > 0)
          cache[num2++] = num4;
      }
      byte num5 = numArray[streamIndex + num1 + 1];
      int num6 = cacheIndex + map.ChunkLength;
      while (num2 < num6)
        cache[num2++] = num5;
    }

    public static void CompressToNewStreamNoLock(
      byte[] cache,
      int cacheStartIndex,
      int chunkLength,
      out short streamID,
      out int streamIndex,
      out int streamSize)
    {
      int next = RLEStreamByte.WorkStreamPool.GetNext();
      CustomArray<byte> workStream = RLEStreamByte.WorkStreamPool.List[next];
      workStream.Clear();
      streamID = (short) -1;
      streamIndex = 0;
      streamSize = 0;
      try
      {
        streamSize = RLEStreamByte.CompressNoLock(cache, cacheStartIndex, chunkLength, workStream);
        Map.RLEStreamBufferManager.Allocate(streamSize, out streamID, out streamIndex);
        Array.Copy((Array) workStream.Array, 0, (Array) Map.RLEStreamBufferManager.Stream[(int) streamID], streamIndex, streamSize);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(74, ex);
      }
      finally
      {
        RLEStreamByte.WorkStreamPool.Release(next);
      }
    }

    private void CompressToLocalStreamNoLock(byte[] cache, int cacheStartIndex, int chunkLength)
    {
      int next = RLEStreamByte.WorkStreamPool.GetNext();
      CustomArray<byte> workStream = RLEStreamByte.WorkStreamPool.List[next];
      workStream.Clear();
      try
      {
        Map.RLEStreamBufferManager.Allocate(this, RLEStreamByte.CompressNoLock(cache, cacheStartIndex, chunkLength, workStream));
        Array.Copy((Array) workStream.Array, 0, (Array) Map.RLEStreamBufferManager.Stream[(int) this.StreamID], this.StreamIndex, this.StreamSize);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(76, ex);
      }
      finally
      {
        RLEStreamByte.WorkStreamPool.Release(next);
      }
    }

    private static int CompressNoLock(
      byte[] cache,
      int cacheStartIndex,
      int chunkLength,
      CustomArray<byte> workStream)
    {
      return RLEStreamByte.CompressNoLockOld(cache, cacheStartIndex, chunkLength, workStream);
    }

    private static int CompressNoLockNew(
      byte[] cache,
      int cacheStartIndex,
      int chunkLength,
      CustomArray<byte> workStream)
    {
      int index1 = cacheStartIndex + 1;
      int num1 = cacheStartIndex + chunkLength;
      int maxValue = (int) sbyte.MaxValue;
      int num2 = 128;
      bool flag = false;
      while (index1 < num1)
      {
        int num3 = 0;
        int index2;
        for (index2 = index1; index2 < num1 && num3 < num2 && (int) cache[index2 - 1] != (int) cache[index2]; ++num3)
          ++index2;
        if (num3 > 0)
        {
          int num4 = num3;
          if (index2 == num1 && num3 < num2)
            ++num3;
          workStream.Add((byte) (num3 + maxValue));
          for (int index3 = 0; index3 < num3; ++index3)
            workStream.Add(cache[index1 + index3 - 1]);
          if (index2 == num1 && num4 == num2)
          {
            workStream.Add((byte) 0);
            workStream.Add(cache[index2 - 1]);
          }
          index1 = index2;
          flag = false;
        }
        else
        {
          byte t;
          for (t = cache[index1 - 1]; index1 < num1 && num3 < maxValue && (int) t == (int) cache[index1]; ++num3)
            ++index1;
          workStream.Add((byte) num3);
          workStream.Add(t);
          ++index1;
          flag = true;
          if (index1 == num1)
          {
            workStream.Add((byte) (maxValue + 1));
            workStream.Add(cache[index1 - 1]);
            flag = false;
          }
        }
      }
      int len = workStream.Count;
      if (flag)
        len = RLEStreamByte.TrimRight(workStream.Array, len);
      return len;
    }

    private static int CompressNoLockOld(
      byte[] cache,
      int cacheStartIndex,
      int chunkLength,
      CustomArray<byte> workStream)
    {
      byte t1 = 0;
      byte maxValue = byte.MaxValue;
      byte t2 = 0;
      byte t3 = cache[cacheStartIndex];
      int num = cacheStartIndex + chunkLength;
      for (int index = cacheStartIndex + 1; index < num; ++index)
      {
        t1 = cache[index];
        if ((int) t2 == (int) maxValue || (int) t1 != (int) t3)
        {
          workStream.Add(t2);
          workStream.Add(t3);
          t3 = t1;
          t2 = (byte) 0;
        }
        else
          ++t2;
      }
      workStream.Add(t2);
      workStream.Add(t1);
      return RLEStreamByte.TrimRight(workStream.Array, workStream.Count);
    }

    private static int TrimRight(byte[] stream, int len)
    {
      while (len > 3 && (int) stream[len - 3] == (int) stream[len - 1])
      {
        len -= 2;
        stream[len - 2] = (byte) 0;
      }
      return len;
    }

    public void Commit()
    {
    }

    public void CopyBlock(
      MapChunk chunk,
      byte[] src,
      Point3D srcSize,
      GlobalPoint3D srcStartPointInMap,
      GlobalPoint3D srcEndPointInMap)
    {
      GlobalPoint3D globalOffset = chunk.GlobalOffset;
      Point3D chunkSize = chunk.Region.Map.ChunkSize;
      GlobalPoint3D globalPoint3D1 = GlobalPoint3D.Max(globalOffset, srcStartPointInMap);
      GlobalPoint3D globalPoint3D2 = GlobalPoint3D.Min(globalOffset + chunkSize, srcEndPointInMap);
      GlobalPoint3D point = new GlobalPoint3D();
      Map map = chunk.Region.Map;
      this.TimeStamp = chunk.Region.Map.TimeStampRleCache;
      lock (chunk.RleLock)
      {
        if (this.CacheID == (short) -1)
          this.Cache(chunk, false);
        byte[] numArray = map.ChunkCacheManager.Cache[(int) this.CacheID];
        for (point.Y = globalPoint3D1.Y; point.Y < globalPoint3D2.Y; ++point.Y)
        {
          for (point.Z = globalPoint3D1.Z; point.Z < globalPoint3D2.Z; ++point.Z)
          {
            for (point.X = globalPoint3D1.X; point.X < globalPoint3D2.X; ++point.X)
            {
              int mapIndex = chunk.GetMapIndex(point);
              int num1 = (point.X - srcStartPointInMap.X) % srcSize.X;
              int num2 = (point.Y - srcStartPointInMap.Y) % srcSize.Y;
              int num3 = (point.Z - srcStartPointInMap.Z) % srcSize.Z;
              int index = num1 + num3 * srcSize.X + num2 * (srcSize.X * srcSize.Z);
              numArray[this.CacheIndex + mapIndex] = src[index];
            }
          }
        }
      }
    }

    public void Fill(MapChunk chunk, int fromMapIndex, int toMapIndex, byte value)
    {
      if (fromMapIndex == 0 && toMapIndex == chunk.Region.Map.ChunkLength - 1)
      {
        this.Fill(chunk, value);
      }
      else
      {
        this.TimeStamp = chunk.Region.Map.TimeStampRleCache;
        lock (chunk.RleLock)
        {
          if (this.CacheID == (short) -1)
            this.Cache(chunk, false);
          byte[] numArray = chunk.Region.Map.ChunkCacheManager.Cache[(int) this.CacheID];
          for (int index = fromMapIndex; index <= toMapIndex; ++index)
            numArray[this.CacheIndex + index] = value;
        }
      }
    }

    public void Fill(MapChunk chunk, byte value)
    {
      lock (chunk.RleLock)
      {
        this.ReleaseCacheNoLock(chunk);
        Map.RLEStreamBufferManager.Allocate(this, 2);
        Map.RLEStreamBufferManager.Stream[(int) this.StreamID][this.StreamIndex] = (byte) 0;
        Map.RLEStreamBufferManager.Stream[(int) this.StreamID][this.StreamIndex + 1] = value;
      }
    }

    public void ReadData(MapChunk chunk, BinaryReader reader, int version)
    {
      int num = reader.ReadInt32();
      if (version < 118)
        reader.ReadInt32();
      if (num <= 0)
        return;
      lock (chunk.RleLock)
      {
        this.ReleaseCacheNoLock(chunk);
        Map.RLEStreamBufferManager.Allocate(this, num);
        reader.Read(Map.RLEStreamBufferManager.Stream[(int) this.StreamID], this.StreamIndex, num);
      }
    }

    public void WriteData(MapChunk chunk, BinaryWriter writer)
    {
      this.UpdateStream(chunk);
      writer.Write(this.StreamSize);
      if (this.StreamSize <= 0)
        return;
      writer.Write(Map.RLEStreamBufferManager.Stream[(int) this.StreamID], this.StreamIndex, this.StreamSize);
    }
  }
}
