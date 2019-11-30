// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.ChunkCacheManager
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace StudioForge.BlockWorld
{
  public class ChunkCacheManager : IHasInitialization
  {
    private Stopwatch compactTimer = new Stopwatch();
    private int cleanupCount = 200;
    private object removeCacheLock = new object();
    private const int cacheRemovalAge = 180;
    public byte[][] Cache;
    public int ExpandCount;
    private Map map;
    private int chunkLength;
    private int cacheSize;
    private List<MapChunk> chunksForImmediateCacheClear;
    private int compactDelay;
    private Stack<int> unused;
    private Stack<int> unusedTemp;
    private List<int> usedKeys;
    private List<int> usedKeysTemp;
    private Dictionary<int, ChunkCacheManager.RLEStreamCache> used;
    private List<ChunkCacheManager.CompactData> compactData;
    private int currentStag;

    public event EventHandler CacheCompacted;

    private void RaiseCacheCompacted()
    {
      if (this.CacheCompacted == null)
        return;
      this.CacheCompacted((object) this, EventArgs.Empty);
    }

    public event EventHandler CacheExpanded;

    private void RaiseCacheExpanded()
    {
      if (this.CacheExpanded == null)
        return;
      this.CacheExpanded((object) this, EventArgs.Empty);
    }

    public bool IsEnabled { get; set; }

    public int CacheCount
    {
      get
      {
        if (this.used == null || this.unused == null)
          return 0;
        return this.used.Count + this.unused.Count;
      }
    }

    public int CachesUsed
    {
      get
      {
        if (this.used == null)
          return 0;
        return this.used.Count;
      }
    }

    public int CachesUnused
    {
      get
      {
        if (this.unused == null)
          return 0;
        return this.unused.Count;
      }
    }

    public int CacheSizeBytesUsed
    {
      get
      {
        return this.CachesUsed * this.chunkLength;
      }
    }

    public float TimeUntilNextCompact
    {
      get
      {
        return (float) ((long) this.compactDelay - this.compactTimer.ElapsedMilliseconds) / 1000f;
      }
    }

    public int CacheSizeBytesTotal
    {
      get
      {
        int num = 0;
        if (this.Cache != null)
        {
          try
          {
            for (int index = 0; index < this.Cache.Length; ++index)
            {
              if (this.Cache[index] != null)
                num += this.Cache[index].Length + 8;
            }
          }
          catch (Exception ex)
          {
          }
        }
        return num;
      }
    }

    public int CacheRefCount
    {
      get
      {
        int num = 0;
        if (this.used != null)
        {
          lock (BuffLock.CacheLock)
          {
            foreach (KeyValuePair<int, ChunkCacheManager.RLEStreamCache> keyValuePair in this.used)
              num += keyValuePair.Value.RefCount;
          }
        }
        return num;
      }
    }

    public virtual int MemorySize
    {
      get
      {
        int num = 36;
        if (this.used != null)
          num = num + this.CacheSizeBytesTotal + this.unused.Count * 4 + this.chunksForImmediateCacheClear.Count * 4;
        return num;
      }
    }

    public ChunkCacheManager(Map map, int initialCacheCount, int cacheSize)
    {
      this.map = map;
      this.cacheSize = cacheSize;
      this.Cache = new byte[initialCacheCount][];
      this.chunkLength = map.ChunkLength;
      this.ExpandCount = 1;
    }

    public void Initialize(InitState state)
    {
      this.compactDelay = 100000;
      this.compactTimer.Reset();
      this.compactTimer.Start();
      this.compactData = new List<ChunkCacheManager.CompactData>(this.cleanupCount);
      this.chunksForImmediateCacheClear = new List<MapChunk>(Math.Max(100, this.cacheSize));
      int capacity = this.Cache.Length * this.cacheSize;
      this.unused = new Stack<int>(capacity);
      this.unusedTemp = new Stack<int>(capacity);
      this.used = (Dictionary<int, ChunkCacheManager.RLEStreamCache>) new DictionaryWithKeyArray<int, ChunkCacheManager.RLEStreamCache>(capacity);
      this.usedKeys = new List<int>(capacity);
      for (short cacheID = 0; (int) cacheID < this.Cache.Length; ++cacheID)
      {
        this.Cache[(int) cacheID] = new byte[this.cacheSize * this.chunkLength];
        for (int index = 0; index < this.cacheSize; ++index)
          this.unused.Push(this.GetKey(cacheID, index * this.chunkLength));
      }
    }

    private int GetKey(short cacheID, int cacheIndex)
    {
      return ((int) cacheID << 16) + cacheIndex / this.chunkLength;
    }

    public byte[] AcquireCache(
      MapChunk chunk,
      RLEStreamByte stream,
      bool addRefCount,
      out short cacheID,
      out int cacheIndex)
    {
      lock (BuffLock.CacheLock)
      {
        this.AcquireCacheNoLock(chunk, stream, addRefCount, out cacheID, out cacheIndex);
        return this.Cache[(int) cacheID];
      }
    }

    private void AcquireCacheNoLock(
      MapChunk chunk,
      RLEStreamByte stream,
      bool addRefCount,
      out short cacheID,
      out int cacheIndex)
    {
      if (this.unused.Count == 0)
        this.Expand();
      int key = this.unused.Pop();
      cacheID = (short) (key >> 16);
      cacheIndex = (int) (ushort) key * this.chunkLength;
      ChunkCacheManager.RLEStreamCache rleStreamCache = new ChunkCacheManager.RLEStreamCache();
      rleStreamCache.RefCount = addRefCount ? 1 : 0;
      rleStreamCache.Chunk = chunk;
      rleStreamCache.Stream = stream;
      if (stream != null)
        stream.TimeStamp = this.map.TimeStampRleCache;
      this.used.Add(key, rleStreamCache);
      this.usedKeys.Add(key);
    }

    private void RecycleCache(int key)
    {
      this.used.Remove(key);
      this.usedKeys.Remove(key);
      this.unused.Push(key);
      if (!this.ShouldTryToReduceCacheSize)
        return;
      this.CheckIfWeCanFreeACache(key);
    }

    public bool ShouldTryToReduceCacheSize
    {
      get
      {
        return (double) this.unused.Count >= (double) this.cacheSize * 1.25 && (double) (this.used.Count % this.cacheSize) <= (double) this.cacheSize * 0.75;
      }
    }

    private void CheckIfWeCanFreeACache(int key)
    {
      short num1 = (short) (key >> 16);
      for (int index = 0; index < this.cacheSize; ++index)
      {
        if (this.used.ContainsKey(((int) num1 << 16) + index))
          return;
      }
      while (this.unused.Count > 0)
      {
        int num2 = this.unused.Pop();
        if (num2 >> 16 != (int) num1)
          this.unusedTemp.Push(num2);
      }
      this.Cache[(int) num1] = (byte[]) null;
      Stack<int> unused = this.unused;
      this.unused = this.unusedTemp;
      this.unusedTemp = unused;
      this.unusedTemp.Clear();
      this.RaiseCacheCompacted();
    }

    private void Expand()
    {
      this.compactTimer.Reset();
      this.compactTimer.Start();
      short cacheID = -1;
      for (int index = 0; index < this.Cache.Length; ++index)
      {
        if (this.Cache[index] == null)
        {
          cacheID = (short) index;
          this.Cache[(int) cacheID] = new byte[this.cacheSize * this.chunkLength];
          break;
        }
      }
      if (cacheID == (short) -1)
      {
        cacheID = (short) this.Cache.Length;
        byte[][] numArray = new byte[(int) cacheID + 1][];
        for (int index = 0; index < this.Cache.Length; ++index)
          numArray[index] = this.Cache[index];
        numArray[(int) cacheID] = new byte[this.cacheSize * this.chunkLength];
        this.Cache = numArray;
      }
      for (int index = 0; index < this.cacheSize; ++index)
        this.unused.Push(this.GetKey(cacheID, index * this.chunkLength));
      this.RaiseCacheExpanded();
    }

    public int GetRefCount(short cacheID, int cacheIndex)
    {
      if (cacheID >= (short) 0)
      {
        int key = this.GetKey(cacheID, cacheIndex);
        lock (BuffLock.CacheLock)
        {
          ChunkCacheManager.RLEStreamCache rleStreamCache;
          if (this.used.TryGetValue(key, out rleStreamCache))
            return rleStreamCache.RefCount;
        }
      }
      return 0;
    }

    public int AddRefCount(short cacheID, int cacheIndex)
    {
      if (cacheID >= (short) 0)
      {
        int key = this.GetKey(cacheID, cacheIndex);
        lock (BuffLock.CacheLock)
        {
          ChunkCacheManager.RLEStreamCache rleStreamCache;
          if (this.used.TryGetValue(key, out rleStreamCache))
          {
            ++rleStreamCache.RefCount;
            this.used[key] = rleStreamCache;
            return rleStreamCache.RefCount;
          }
        }
      }
      return 0;
    }

    public int DecRefCount(short cacheID, int cacheIndex)
    {
      if (cacheID >= (short) 0)
      {
        int key = this.GetKey(cacheID, cacheIndex);
        lock (BuffLock.CacheLock)
        {
          ChunkCacheManager.RLEStreamCache rleStreamCache;
          if (this.used.TryGetValue(key, out rleStreamCache))
          {
            if (rleStreamCache.RefCount > 1)
            {
              --rleStreamCache.RefCount;
              this.used[key] = rleStreamCache;
              return rleStreamCache.RefCount;
            }
            if (rleStreamCache.Stream == null)
            {
              this.RecycleCache(key);
            }
            else
            {
              rleStreamCache.RefCount = 0;
              this.used[key] = rleStreamCache;
            }
          }
        }
      }
      return 0;
    }

    public void RecycleCacheIfNoRefs(short cacheID, int cacheIndex)
    {
      if (cacheID < (short) 0)
        return;
      int key = this.GetKey(cacheID, cacheIndex);
      lock (BuffLock.CacheLock)
      {
        ChunkCacheManager.RLEStreamCache rleStreamCache;
        if (!this.used.TryGetValue(key, out rleStreamCache) || rleStreamCache.RefCount >= 1)
          return;
        this.RecycleCache(key);
      }
    }

    public void ReleaseCacheStream(short cacheID, int cacheIndex)
    {
      if (cacheID < (short) 0)
        return;
      int key = this.GetKey(cacheID, cacheIndex);
      lock (BuffLock.CacheLock)
        this.ReleaseCacheStreamCore(key);
    }

    private void ReleaseCacheStreamCore(int key)
    {
      ChunkCacheManager.RLEStreamCache rleStreamCache;
      if (!this.used.TryGetValue(key, out rleStreamCache))
        return;
      rleStreamCache.Chunk = (MapChunk) null;
      rleStreamCache.Stream = (RLEStreamByte) null;
      this.used[key] = rleStreamCache;
    }

    public bool SetCacheStream(
      MapChunk chunk,
      RLEStreamByte rle,
      short newCacheID,
      int newCacheIndex)
    {
      if (newCacheID >= (short) 0 && ((int) rle.CacheID != (int) newCacheID || rle.CacheIndex != newCacheIndex))
      {
        int key = this.GetKey(newCacheID, newCacheIndex);
        lock (BuffLock.CacheLock)
        {
          ChunkCacheManager.RLEStreamCache rleStreamCache;
          if (this.used.TryGetValue(key, out rleStreamCache))
          {
            if (rle.CacheID >= (short) 0)
              this.ReleaseCacheStreamCore(this.GetKey(rle.CacheID, rle.CacheIndex));
            rleStreamCache.Chunk = chunk;
            rleStreamCache.Stream = rle;
            this.used[key] = rleStreamCache;
            return true;
          }
        }
      }
      return false;
    }

    public void SetChunkCacheForImmedaiteClear(MapChunk chunk)
    {
      lock (this.chunksForImmediateCacheClear)
        this.chunksForImmediateCacheClear.Add(chunk);
    }

    public void DecacheAll()
    {
      for (short cacheID = 0; (int) cacheID < this.Cache.Length; ++cacheID)
        this.DecacheAll(cacheID);
    }

    private void DecacheAll(short cacheID)
    {
      for (int index = 0; index < this.cacheSize; ++index)
      {
        do
          ;
        while (this.DecRefCount(cacheID, index * this.chunkLength) > 0);
        this.RecycleCacheIfNoRefs(cacheID, index * this.chunkLength);
      }
    }

    public void ReleaseAll()
    {
      lock (BuffLock.CacheLock)
      {
        this.DecacheAll();
        for (int index = 0; index < this.Cache.Length; ++index)
          this.Cache[index] = (byte[]) null;
      }
    }

    public void RemoveCachesStaggered(float countFactor)
    {
      if (!Monitor.TryEnter(this.removeCacheLock))
        return;
      try
      {
        this.RemoveImmediateCachesStaggered(countFactor);
        this.RemoveDatedCachesStaggered(countFactor);
      }
      finally
      {
        Monitor.Exit(this.removeCacheLock);
      }
    }

    private void RemoveImmediateCachesStaggered(float countFactor)
    {
      lock (this.chunksForImmediateCacheClear)
      {
        int num = this.chunksForImmediateCacheClear.Count;
        if ((double) countFactor > 0.0)
          num = (int) ((double) num * (double) countFactor);
        for (int index = this.chunksForImmediateCacheClear.Count - 1; index >= 0 && num > 0; --num)
        {
          MapChunk chunk = this.chunksForImmediateCacheClear[index];
          chunk.BlockData.ReleaseCacheToStream(chunk);
          chunk.LightData.ReleaseCacheToStream(chunk);
          chunk.AuxData.ReleaseCacheToStream(chunk);
          this.chunksForImmediateCacheClear.RemoveAt(index);
          --index;
        }
      }
    }

    private void RemoveDatedCachesStaggered(float countFactor)
    {
      ChunkCacheManager.CompactData compactData1 = new ChunkCacheManager.CompactData();
      int num = this.map.TimeStampRleCache - 180;
      if (this.usedKeysTemp == null)
        this.usedKeysTemp = new List<int>(10);
      this.usedKeysTemp.Clear();
      this.compactData.Clear();
      lock (BuffLock.CacheLock)
      {
        for (int index = (double) countFactor == 0.0 ? this.usedKeys.Count : (int) ((double) (this.usedKeys.Count / 180) * (double) countFactor + 1.0); this.currentStag < this.usedKeys.Count && index > 0; --index)
        {
          this.usedKeysTemp.Add(this.usedKeys[this.currentStag]);
          ++this.currentStag;
        }
        if (this.currentStag >= this.usedKeys.Count)
          this.currentStag = 0;
      }
      ChunkCacheManager.RLEStreamCache rleStreamCache;
      for (int index = 0; index < this.usedKeysTemp.Count; ++index)
      {
        int key = this.usedKeysTemp[index];
        lock (BuffLock.CacheLock)
        {
          if (this.used.TryGetValue(key, out rleStreamCache))
          {
            if (rleStreamCache.Stream == null || rleStreamCache.Chunk == null)
            {
              if (rleStreamCache.RefCount < 1)
                this.RecycleCache(key);
            }
            else if (rleStreamCache.Stream.TimeStamp <= num)
            {
              compactData1.Key = key;
              compactData1.Chunk = rleStreamCache.Chunk;
              this.compactData.Add(compactData1);
            }
          }
        }
      }
      for (int index = 0; index < this.compactData.Count; ++index)
      {
        ChunkCacheManager.CompactData compactData2 = this.compactData[index];
        MapChunk chunk = compactData2.Chunk;
        lock (chunk.RleLock)
        {
          lock (BuffLock.CacheLock)
          {
            if (this.used.TryGetValue(compactData2.Key, out rleStreamCache))
            {
              if (rleStreamCache.Stream != null)
              {
                if (rleStreamCache.Stream.TimeStamp <= num)
                {
                  rleStreamCache.Stream.ReleaseCacheToStream(chunk);
                  if (rleStreamCache.RefCount > 0)
                  {
                    rleStreamCache.Chunk = (MapChunk) null;
                    rleStreamCache.Stream = (RLEStreamByte) null;
                    this.used[compactData2.Key] = rleStreamCache;
                  }
                  else
                    this.RecycleCache(compactData2.Key);
                }
              }
            }
          }
        }
      }
    }

    public void RemoveDatedCaches()
    {
    }

    private bool ShouldCleanup(ChunkCacheManager.RLEStreamCache cache, int removalTimeStamp)
    {
      if (cache.Stream == null)
        return cache.RefCount < 1;
      return cache.Stream.TimeStamp <= removalTimeStamp;
    }

    private struct RLEStreamCache
    {
      public int RefCount;
      public RLEStreamByte Stream;
      public MapChunk Chunk;
    }

    private struct CompactData
    {
      public int Key;
      public int NewKey;
      public MapChunk Chunk;
    }
  }
}
