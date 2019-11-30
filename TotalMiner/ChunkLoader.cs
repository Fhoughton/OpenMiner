// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ChunkLoader : IThreadWorkItem
  {
    private BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);
    private BoundingFrustum frustum2 = new BoundingFrustum(Matrix.Identity);
    private List<MapChunk> chunks = new List<MapChunk>(2000);
    private List<ChunkLoader.PlayerChunk> playerChunks = new List<ChunkLoader.PlayerChunk>(1000);
    private List<MapRegion> regionsInViewRange = new List<MapRegion>(9);
    private List<long> chunksForRemotes = new List<long>(100);
    private List<MapRegion> regionsSortedByDistanceToPlayer = new List<MapRegion>();
    private SurroundingChunkEnumerator surroundingChunkEnumerator = new SurroundingChunkEnumerator();
    private SurroundingChunkEnumerator surroundingChunkEnumerator2 = new SurroundingChunkEnumerator();
    private SurroundingChunkEnumerator surroundingChunkEnumeratorCalc = new SurroundingChunkEnumerator();
    private SurroundingChunkEnumerator surroundingChunkEnumeratorCalc2 = new SurroundingChunkEnumerator();
    private int biomeHandle = -1;
    private List<MapChunkTM> chunksToUnsplit = new List<MapChunkTM>(50);
    public int MaxItems;
    private MapTM map;
    private bool isThreaded;
    private GameInstance instance;
    private Comparison<ChunkLoader.PlayerChunk> chunksForLoading;
    private Comparison<MapRegion> sortRegionByDistance;
    private Vector3 positionsToSortRegionsAgainst;
    private TerrainGeneratorBase biome;
    private int queuedCount;
    private int skipGenerateSurroundingSurfaceChunksImmediate;
    private float[] furthest;
    private MapChunkTM[] furthestChunk;

    public string Name
    {
      get
      {
        return nameof (ChunkLoader);
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

    public void Initialize(GameInstance instance, MapTM map, bool isThreaded)
    {
      this.map = map;
      this.instance = instance;
      this.isThreaded = isThreaded;
      this.MaxItems = ThreadQueueManager.Instance.GetProcessorScale(60, 100);
      this.chunksForLoading = new Comparison<ChunkLoader.PlayerChunk>(this.SortChunksForLoading);
    }

    public void Update()
    {
      if (this.isThreaded)
      {
        try
        {
          this.UpdateCore();
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(58, ex);
        }
        finally
        {
          this.RestartChunkLoader();
        }
      }
      else
        this.UpdateCore();
    }

    private void RestartChunkLoader()
    {
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, true, PriorityLevel.Normal);
    }

    private void UpdateCore()
    {
      this.queuedCount = 0;
      if (this.instance == null || this.map == null || NetworkManager.Instance == null)
        return;
      if (this.instance.MustReduceMeshSize)
        this.UnloadChunksToReduceMeshSize(4000000);
      int maxItems = this.MaxItems;
      this.queuedCount += this.GenerateChunksForRemotes();
      this.MaxItems = maxItems;
      bool canGenerate1 = this.map.IsHost || NetworkManager.Instance.ChunksRequestedNotReceivedCount < MapChunkTM.MaxQueuedChunkRequests;
      foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null)
        {
          this.queuedCount += this.GenerateImmediateChunk(tag.VirtualPlayer, canGenerate1);
          this.MaxItems += maxItems;
        }
      }
      this.MaxItems = maxItems;
      NetworkManager.Instance.RequeueOldChunkRequests();
      bool canGenerate2 = this.map.IsHost || this.queuedCount == 0;
      if (this.queuedCount < this.MaxItems)
      {
        if (!this.map.IsHost)
        {
          if (NetworkManager.Instance.ChunksRequestedNotReceivedCount >= 10)
            goto label_25;
        }
        try
        {
          MapTM.GetBiome(this.instance.CurrentBiome, out this.biome, out this.biomeHandle);
          if (this.biomeHandle >= 0)
            this.biome.Initialize(this.instance, this.map, Globals2.GameProperties.SaveGame.Header.BiomeParams);
          this.MaxItems = maxItems;
          if (this.queuedCount < this.MaxItems)
          {
            if (ThreadQueueManager.Instance.MainWorkItemCount < this.MaxItems - 4)
            {
              int cachesUsed = this.map.ChunkCacheManager.CachesUsed;
              if (cachesUsed < 5000)
              {
                if (this.GenerateOrLoadChunksInViewFrustum(canGenerate2) == 0)
                {
                  if (cachesUsed < 1000)
                  {
                    if (this.map.IsHost)
                      this.GenerateOrLoadChunksOutsideViewFrustum();
                  }
                }
              }
            }
          }
        }
        finally
        {
          if (this.biomeHandle >= 0)
            MapTM.ReleaseBiome(this.instance.CurrentBiome, this.biome, this.biomeHandle);
        }
      }
label_25:
      this.UnloadSplitChunks();
    }

    private int GenerateImmediateChunk(Player virtualPlayer, bool canGenerate)
    {
      int num = 0;
      if (ThreadQueueManager.Instance.MainWorkItemCount < this.MaxItems)
      {
        int count = 9;
        int y = this.map.ChunkSize.Y;
        GlobalPoint3D point = this.map.GetPoint(virtualPlayer.Position - new Vector3(0.0f, this.map.TileSize * 0.5f, 0.0f));
        this.surroundingChunkEnumerator.Reset((Map) this.map, point, count);
        while (this.surroundingChunkEnumerator.MoveNext())
          num += this.LoadChunkImmediate(this.surroundingChunkEnumerator.Current, this.MaxItems, canGenerate);
        if (ThreadQueueManager.Instance.MainWorkItemCount < this.MaxItems)
        {
          if (point.Y % y < 3)
          {
            point.Y -= y;
            if (point.Y >= this.map.MapBound.Min.Y)
            {
              this.surroundingChunkEnumerator.Reset((Map) this.map, point, count);
              while (this.surroundingChunkEnumerator.MoveNext())
                num += this.LoadChunkImmediate(this.surroundingChunkEnumerator.Current, this.MaxItems, canGenerate);
            }
            point.Y += y;
          }
          if (ThreadQueueManager.Instance.MainWorkItemCount < this.MaxItems && point.Y % y > y - 5)
          {
            point.Y += y;
            if (point.Y < this.map.MapBound.Max.Y)
            {
              this.surroundingChunkEnumerator.Reset((Map) this.map, point, count);
              while (this.surroundingChunkEnumerator.MoveNext())
                num += this.LoadChunkImmediate(this.surroundingChunkEnumerator.Current, this.MaxItems, canGenerate);
            }
          }
        }
      }
      return num;
    }

    private int LoadChunkImmediate(GlobalPoint3D p, int maxWorkItemCount, bool canGenerate)
    {
      int num = 0;
      GlobalPoint3D p1 = p;
      int y = this.map.ChunkSize.Y;
      if (canGenerate)
      {
        for (int index = -y * 4; index <= y * 4 && ThreadQueueManager.Instance.MainWorkItemCount < maxWorkItemCount; index += y)
        {
          p1.Y = p.Y + index;
          this.surroundingChunkEnumerator2.Reset((Map) this.map, p1, 81);
          while (this.surroundingChunkEnumerator2.MoveNext())
          {
            if (this.GenerateChunk(this.map.GetChunk(this.surroundingChunkEnumerator2.Current)))
              ++num;
          }
        }
      }
      for (int index = -y * 3; index <= y * 3 && ThreadQueueManager.Instance.MainWorkItemCount < maxWorkItemCount; index += y)
      {
        p1.Y = p.Y + index;
        this.surroundingChunkEnumerator2.Reset((Map) this.map, p1, 49);
        while (this.surroundingChunkEnumerator2.MoveNext())
        {
          if (this.DecorateChunk(this.map.GetChunk(this.surroundingChunkEnumerator2.Current)))
            ++num;
        }
      }
      for (int index = -y * 2; index <= y * 2 && ThreadQueueManager.Instance.MainWorkItemCount < maxWorkItemCount; index += y)
      {
        p1.Y = p.Y + index;
        this.surroundingChunkEnumerator2.Reset((Map) this.map, p1, 25);
        while (this.surroundingChunkEnumerator2.MoveNext())
        {
          if (this.DecoratePendingChunk(this.map.GetChunk(this.surroundingChunkEnumerator2.Current)))
            ++num;
        }
      }
      for (int index = -y; index <= y && ThreadQueueManager.Instance.MainWorkItemCount < maxWorkItemCount; index += y)
      {
        p1.Y = p.Y + index;
        this.surroundingChunkEnumerator2.Reset((Map) this.map, p1, 9);
        while (this.surroundingChunkEnumerator2.MoveNext())
        {
          if (this.LightChunk(this.map.GetChunk(this.surroundingChunkEnumerator2.Current) as MapChunkTM))
            ++num;
        }
      }
      p1.Y = p.Y;
      if (this.LoadChunk(this.map.GetChunk(p1) as MapChunkTM))
        ++num;
      return num;
    }

    public void CalculateImmediateChunksToGenerate(
      Player player,
      out int generateCount,
      out int decorateCount,
      out int pendingCount,
      out int lightingCount,
      out int loadMeshCount)
    {
      generateCount = decorateCount = pendingCount = lightingCount = loadMeshCount = 0;
      int count = 9;
      int y = this.map.ChunkSize.Y;
      GlobalPoint3D point = this.map.GetPoint(player.Position - new Vector3(0.0f, this.map.TileSize * 0.5f, 0.0f));
      this.surroundingChunkEnumeratorCalc.Reset((Map) this.map, point, count);
      while (this.surroundingChunkEnumeratorCalc.MoveNext())
        this.CalculateChunkImmediate(this.surroundingChunkEnumeratorCalc.Current, ref generateCount, ref decorateCount, ref pendingCount, ref lightingCount, ref loadMeshCount);
      if (point.Y % y < 3)
      {
        point.Y -= y;
        if (point.Y >= this.map.MapBound.Min.Y)
        {
          this.surroundingChunkEnumeratorCalc.Reset((Map) this.map, point, count);
          while (this.surroundingChunkEnumeratorCalc.MoveNext())
            this.CalculateChunkImmediate(this.surroundingChunkEnumeratorCalc.Current, ref generateCount, ref decorateCount, ref pendingCount, ref lightingCount, ref loadMeshCount);
        }
        point.Y += y;
      }
      if (point.Y % y <= y - 5)
        return;
      point.Y += y;
      if (point.Y >= this.map.MapBound.Max.Y)
        return;
      this.surroundingChunkEnumeratorCalc.Reset((Map) this.map, point, count);
      while (this.surroundingChunkEnumeratorCalc.MoveNext())
        this.CalculateChunkImmediate(this.surroundingChunkEnumeratorCalc.Current, ref generateCount, ref decorateCount, ref pendingCount, ref lightingCount, ref loadMeshCount);
    }

    private void CalculateChunkImmediate(
      GlobalPoint3D p,
      ref int generateCount,
      ref int decorateCount,
      ref int pendingCount,
      ref int lightingCount,
      ref int loadMeshCount)
    {
      GlobalPoint3D p1 = p;
      int y = this.map.ChunkSize.Y;
      for (int index = -y * 4; index <= y * 4; index += y)
      {
        p1.Y = p.Y + index;
        this.surroundingChunkEnumeratorCalc2.Reset((Map) this.map, p1, 81);
        while (this.surroundingChunkEnumeratorCalc2.MoveNext())
        {
          MapChunk chunk = this.map.GetChunk(this.surroundingChunkEnumeratorCalc2.Current);
          if (chunk != null && !chunk.IsGenerated)
            ++generateCount;
        }
      }
      for (int index = -y * 3; index <= y * 3; index += y)
      {
        p1.Y = p.Y + index;
        this.surroundingChunkEnumeratorCalc2.Reset((Map) this.map, p1, 49);
        while (this.surroundingChunkEnumeratorCalc2.MoveNext())
        {
          MapChunk chunk = this.map.GetChunk(this.surroundingChunkEnumeratorCalc2.Current);
          if (chunk != null && !chunk.IsDecoratedWithoutReceivedCheck)
            ++decorateCount;
        }
      }
      for (int index = -y * 2; index <= y * 2; index += y)
      {
        p1.Y = p.Y + index;
        this.surroundingChunkEnumeratorCalc2.Reset((Map) this.map, p1, 25);
        while (this.surroundingChunkEnumeratorCalc2.MoveNext())
        {
          MapChunk chunk = this.map.GetChunk(this.surroundingChunkEnumeratorCalc2.Current);
          if (chunk != null && !chunk.IsDecorated)
            ++pendingCount;
        }
      }
      for (int index = -y; index <= y; index += y)
      {
        p1.Y = p.Y + index;
        this.surroundingChunkEnumeratorCalc2.Reset((Map) this.map, p1, 9);
        while (this.surroundingChunkEnumeratorCalc2.MoveNext())
        {
          MapChunk chunk = this.map.GetChunk(this.surroundingChunkEnumeratorCalc2.Current);
          if (chunk != null && chunk.IsLightDirty)
            ++lightingCount;
        }
      }
      p1.Y = p.Y;
      MapChunk chunk1 = this.map.GetChunk(p1);
      if (chunk1 == null || chunk1.IsMeshLoaded)
        return;
      ++loadMeshCount;
    }

    public int GenerateSurroundingSurfaceChunksImmediate(Player virtualPlayer, bool canGenerate)
    {
      int num1 = 0;
      if (this.skipGenerateSurroundingSurfaceChunksImmediate < 1)
      {
        int count = 25;
        int y = this.map.ChunkSize.Y;
        int num2 = y * 2;
        this.surroundingChunkEnumerator.Reset((Map) this.map, this.map.GetPoint(virtualPlayer.Position - new Vector3(0.0f, this.map.TileSize * 0.5f, 0.0f)), count);
        while (this.surroundingChunkEnumerator.MoveNext() && ThreadQueueManager.Instance.MainWorkItemCount < this.MaxItems)
        {
          GlobalPoint3D current = this.surroundingChunkEnumerator.Current;
          int low;
          int high;
          this.GetLowHigh(current, out low, out high);
          low -= num2;
          high += num2;
          for (current.Y = low; current.Y <= high; current.Y += y)
            num1 += this.LoadChunkImmediate(current, this.MaxItems, canGenerate);
        }
        if (num1 == 0)
          this.skipGenerateSurroundingSurfaceChunksImmediate = 4;
      }
      else
        --this.skipGenerateSurroundingSurfaceChunksImmediate;
      return num1;
    }

    private bool IsPlayerAboveSurfaceHeight(Player player)
    {
      GlobalPoint3D point = this.map.GetPoint(player.EyePosition);
      int low;
      int high;
      this.GetLowHigh(point, out low, out high);
      return point.Y > low - 4;
    }

    private void GetLowHigh(GlobalPoint3D p, out int low, out int high)
    {
      if (this.biomeHandle >= 0)
      {
        this.biome.InitializeForGeneralUse(p);
        low = high = this.biome.GetGroundHeightGlobal((Map) this.map, p.X, p.Z);
      }
      else
        low = high = (int) this.map.GetHeight(p);
    }

    private void GetLowHigh(MapChunk chunk, out int low, out int high)
    {
      if (this.biomeHandle >= 0)
        this.biome.InitializeForGeneralUse(chunk);
      Point3D chunkSize = this.map.ChunkSize;
      GlobalPoint3D globalOffset = chunk.GlobalOffset;
      int num1 = this.biomeHandle >= 0 ? this.biome.GetGroundHeightGlobal((Map) this.map, globalOffset.X, globalOffset.Z) : (int) this.map.GetHeight(globalOffset);
      int num2 = num1;
      int num3 = num1;
      globalOffset.X = chunk.GlobalOffset.X + chunkSize.X - 1;
      int num4 = this.biomeHandle >= 0 ? this.biome.GetGroundHeightGlobal((Map) this.map, globalOffset.X, globalOffset.Z) : (int) this.map.GetHeight(globalOffset);
      if (num4 < num2)
        num2 = num4;
      if (num4 > num3)
        num3 = num4;
      globalOffset.Z = chunk.GlobalOffset.Z + chunkSize.Z - 1;
      int num5 = this.biomeHandle >= 0 ? this.biome.GetGroundHeightGlobal((Map) this.map, globalOffset.X, globalOffset.Z) : (int) this.map.GetHeight(globalOffset);
      if (num5 < num2)
        num2 = num5;
      if (num5 > num3)
        num3 = num5;
      globalOffset.X = chunk.GlobalOffset.X;
      int num6 = this.biomeHandle >= 0 ? this.biome.GetGroundHeightGlobal((Map) this.map, globalOffset.X, globalOffset.Z) : (int) this.map.GetHeight(globalOffset);
      if (num6 < num2)
        num2 = num6;
      if (num6 > num3)
        num3 = num6;
      globalOffset.X = chunk.GlobalOffset.X + chunkSize.X / 2;
      globalOffset.Z = chunk.GlobalOffset.Z + chunkSize.Z / 2;
      int num7 = this.biomeHandle >= 0 ? this.biome.GetGroundHeightGlobal((Map) this.map, globalOffset.X, globalOffset.Z) : (int) this.map.GetHeight(globalOffset);
      if (num7 < num2)
        num2 = num7;
      if (num7 > num3)
        num3 = num7;
      low = num2;
      high = num3;
    }

    private bool LoadChunk(MapChunkTM chunk)
    {
      if (chunk == null || !chunk.ShouldLoadMesh || this.instance.MustReduceMeshSize)
        return false;
      chunk.LoadMesh(false, false);
      return true;
    }

    private bool LightChunk(MapChunkTM chunk)
    {
      if (chunk == null || !chunk.ShouldLight)
        return false;
      chunk.Light(false);
      return true;
    }

    private bool DecoratePendingChunk(MapChunk chunk)
    {
      if (chunk == null || !chunk.ShouldDecoratePending)
        return false;
      chunk.DecoratePending();
      return true;
    }

    private bool DecorateChunk(MapChunk chunk)
    {
      if (chunk == null || !chunk.ShouldDecorate)
        return false;
      chunk.Decorate();
      return true;
    }

    private bool GenerateChunk(MapChunk chunk)
    {
      if (chunk == null || !chunk.ShouldGenerate(false))
        return false;
      chunk.Generate();
      return true;
    }

    private void FindChunksInView(Player player, Player virtualPlayer)
    {
      this.regionsInViewRange.Clear();
      this.frustum.Matrix = virtualPlayer.ViewMatrixLocal * player.ProjectionMatrix;
      Vector3 position = virtualPlayer.Position;
      foreach (KeyValuePair<int, MapRegion> region in this.map.Regions)
      {
        MapRegion mapRegion = region.Value;
        BoundingBox box = mapRegion.Box;
        box.Min -= position;
        box.Max -= position;
        if (this.frustum.FastIntersect(ref box))
          this.regionsInViewRange.Add(mapRegion);
      }
      if (this.regionsInViewRange.Count <= 0)
        return;
      this.FindChunksInViewForRegions(virtualPlayer);
    }

    private void FindChunksInViewForRegions(Player virtualPlayer)
    {
      foreach (MapRegion mapRegion in this.regionsInViewRange)
        mapRegion.Octree.GetObjectsWithBoxInsideFrustum(this.frustum, this.chunks, mapRegion.Box.Min - virtualPlayer.Position);
    }

    private int GenerateOrLoadChunksInViewFrustum(bool canGenerate)
    {
      this.playerChunks.Clear();
      Point3D chunkSize = this.map.ChunkSize;
      Vector3 vector3_1 = new Vector3((float) chunkSize.X, (float) chunkSize.Y, (float) chunkSize.Z) * (this.map.TileSize * 0.5f);
      int num1 = chunkSize.Y * 2;
      foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null)
        {
          Player virtualPlayer = tag.VirtualPlayer;
          if (virtualPlayer != null)
          {
            this.chunks.Clear();
            this.FindChunksInView(tag, virtualPlayer);
            Vector3 eyeOffset = virtualPlayer.EyeOffset;
            Vector3 position = virtualPlayer.Position;
            bool flag1 = this.IsPlayerAboveSurfaceHeight(virtualPlayer);
            ChunkLoader.PlayerChunk playerChunk = new ChunkLoader.PlayerChunk();
            GlobalPoint3D globalPoint3D = new GlobalPoint3D();
            foreach (MapChunk chunk in this.chunks)
            {
              if (chunk != null && (chunk.IsMeshDirty || !chunk.IsMeshLoaded))
              {
                GlobalPoint3D globalOffset = chunk.GlobalOffset;
                Vector3 vector3_2 = globalOffset.ToVector3() + vector3_1;
                float num2 = Vector3.DistanceSquared(eyeOffset, vector3_2 - position);
                bool flag2 = false;
                if (flag1)
                {
                  if (chunk.IsChunkDecorated(chunk) && (chunk.IsLightDirty || chunk.IsMeshDirty || !chunk.IsMeshLoaded))
                  {
                    flag2 = true;
                  }
                  else
                  {
                    int low;
                    int high;
                    this.GetLowHigh(chunk, out low, out high);
                    low -= num1;
                    high += num1;
                    int y = globalOffset.Y;
                    int num3 = globalOffset.Y + chunkSize.Y;
                    flag2 = y <= high && num3 >= low;
                  }
                }
                playerChunk.Chunk = chunk;
                playerChunk.Distance = num2;
                playerChunk.Priority = flag2;
                this.playerChunks.Add(playerChunk);
              }
            }
          }
        }
      }
      this.playerChunks.Sort(this.chunksForLoading);
      return this.GenerateOrLoadSortedChunks(canGenerate);
    }

    private int SortChunksForLoading(ChunkLoader.PlayerChunk chunk1, ChunkLoader.PlayerChunk chunk2)
    {
      if (chunk1.Priority && !chunk2.Priority)
        return -1;
      if (chunk2.Priority && !chunk1.Priority)
        return 1;
      return chunk1.Distance.CompareTo(chunk2.Distance);
    }

    private int GenerateOrLoadSortedChunks(bool canGenerate)
    {
      int num = 0;
      int processorScale = ThreadQueueManager.Instance.GetProcessorScale(this.MaxItems, this.MaxItems * 4);
      for (int index = 0; index < this.playerChunks.Count && ThreadQueueManager.Instance.MainWorkItemCount < processorScale; ++index)
      {
        ChunkLoader.PlayerChunk playerChunk = this.playerChunks[index];
        num += this.LoadChunkImmediate(playerChunk.Chunk.GlobalOffset, processorScale, canGenerate);
      }
      return num;
    }

    private void GenerateOrLoadChunksOutsideViewFrustum()
    {
      ChunkFlags flag1 = ChunkFlags.MeshLoaded | ChunkFlags.Committed;
      int num = this.instance.IsMultiplayer ? 30 : this.MaxItems * 2;
      if (ThreadQueueManager.Instance.MainWorkItemCount >= num)
        return;
      bool flag2 = false;
      foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null)
        {
          this.SortRegionsByDistanceToPosition(tag.EyePosition);
          foreach (MapRegion mapRegion in this.regionsSortedByDistanceToPlayer)
          {
            Vector3 min = mapRegion.Box.Min;
            foreach (MapChunk chunk in mapRegion.Chunks)
            {
              if (chunk != null && !chunk.IsChunkFlagSet(flag1))
              {
                if (chunk.ShouldGenerate(false))
                  chunk.Generate();
                else if (chunk.ShouldDecorate)
                  chunk.Decorate();
                else if (chunk.ShouldDecoratePending)
                {
                  chunk.DecoratePending();
                  --num;
                }
                else if (chunk.ShouldLight)
                {
                  chunk.Light(false);
                  --num;
                }
              }
              if (ThreadQueueManager.Instance.MainWorkItemCount >= num)
              {
                flag2 = true;
                break;
              }
            }
            if (ThreadQueueManager.Instance.MainWorkItemCount >= num)
            {
              flag2 = true;
              break;
            }
          }
        }
      }
      if (flag2 || this.instance.BlueprintsToPlace.Count <= 0)
        return;
      Dictionary<int, MapRegion> regions = this.map.Regions;
      bool flag3 = true;
      foreach (MapRegion mapRegion in regions.Values)
      {
        if (!mapRegion.IsAllChunksDecorated)
        {
          flag3 = false;
          break;
        }
      }
      if (!flag3 || this.instance.BlueprintsToPlace.Count <= 0)
        return;
      this.GenerateRemainingBlueprints();
    }

    private void SortRegionsByDistanceToPosition(Vector3 pos)
    {
      this.regionsSortedByDistanceToPlayer.Clear();
      foreach (MapRegion mapRegion in this.map.Regions.Values)
        this.regionsSortedByDistanceToPlayer.Add(mapRegion);
      this.positionsToSortRegionsAgainst = pos;
      if (this.sortRegionByDistance == null)
        this.sortRegionByDistance = new Comparison<MapRegion>(this.SortRegionByDistance);
      this.regionsSortedByDistanceToPlayer.Sort(this.sortRegionByDistance);
    }

    private int SortRegionByDistance(MapRegion reg1, MapRegion reg2)
    {
      return Vector3.DistanceSquared(this.positionsToSortRegionsAgainst, MyExtensions.CenterOfBox(reg1.Box)).CompareTo(Vector3.DistanceSquared(this.positionsToSortRegionsAgainst, MyExtensions.CenterOfBox(reg2.Box)));
    }

    private MapRegion GetClosestRegion(Vector3 pos)
    {
      MapRegion mapRegion1 = (MapRegion) null;
      float num1 = float.MaxValue;
      foreach (MapRegion mapRegion2 in this.map.Regions.Values)
      {
        Vector3 vector3 = MyExtensions.CenterOfBox(mapRegion2.Box) * this.map.TileSize;
        float num2 = Vector3.Distance(pos, vector3);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          mapRegion1 = mapRegion2;
        }
      }
      return mapRegion1;
    }

    private void GenerateRemainingBlueprints()
    {
      for (int index = this.instance.BlueprintsToPlace.Count - 1; index >= 0; --index)
      {
        if (this.GenerateRemainingBlueprint(this.instance.BlueprintsToPlace[index]))
          this.instance.BlueprintsToPlace.RemoveAt(index);
      }
    }

    private bool GenerateRemainingBlueprint(Blueprint bp)
    {
      GlobalPoint3D p = new GlobalPoint3D()
      {
        X = this.instance.Random.Next(this.map.MapBound.Min.X, this.map.MapBound.Max.X),
        Z = this.instance.Random.Next(this.map.MapBound.Min.Z, this.map.MapBound.Max.Z)
      };
      p.Y = (int) this.map.GetHeight(p) + 1;
      MapChunk chunk = this.map.GetChunk(p);
      Point3D localPoint = chunk.GetLocalPoint(p);
      MapBlock data = new MapBlock()
      {
        BlockID = 57,
        Light = chunk.GetLight(localPoint)
      };
      chunk.SetBlockData(localPoint, data, UpdateBlockMethod.Generation);
      this.instance.MapStrategyTM.AddDataBlock((DataBlock) new BlueprintBlock(p)
      {
        ID = bp.ID
      }, UpdateBlockMethod.Generation);
      chunk.SetChunkFlag(ChunkFlags.LightDirty | ChunkFlags.HasSpecialBlocks);
      bp.IsGenerated = true;
      bp.Point = p;
      return true;
    }

    public void QueueChunksForRemoteGenerating(List<long> chunks)
    {
      lock (this.chunksForRemotes)
        this.chunksForRemotes.AddRange((IEnumerable<long>) chunks);
    }

    private int GenerateChunksForRemotes()
    {
      int num = 0;
      lock (this.chunksForRemotes)
      {
        for (int index = 0; index < this.chunksForRemotes.Count; ++index)
        {
          MapChunk chunk = this.map.GetChunk(this.chunksForRemotes[index]);
          if (chunk != null)
          {
            if (!chunk.IsGenerated)
            {
              if (!chunk.IsChunkFlagSet(ChunkFlags.Generating) && this.GenerateChunk(chunk))
                ++num;
            }
            else if (!chunk.IsDecoratedWithoutReceivedCheck)
            {
              if (!chunk.IsChunkFlagSet(ChunkFlags.Decorating))
                num += this.DecorateChunkAndGenerateNeighbours(chunk);
            }
            else if (!chunk.IsDecorated)
            {
              if (!chunk.IsChunkFlagSet(ChunkFlags.Decorating))
                num += this.DecoratePendingChunkAndDecorateNeighbours(chunk);
            }
            else
              this.chunksForRemotes.RemoveAt(index);
          }
          else
            this.chunksForRemotes.RemoveAt(index);
        }
      }
      return num;
    }

    private int GenerateNeighbours(MapChunk chunk)
    {
      int num = 0;
      if (this.GenerateChunk(chunk.LeftNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftForwardNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.ForwardNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightForwardNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightBackwardNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.BackwardNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftBackwardNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.UpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftForwardUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.ForwardUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightForwardUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightBackwardUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.BackwardUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftBackwardUpNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.DownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftDownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftForwardDownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.ForwardDownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightForwardDownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightDownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.RightBackwardDownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.BackwardDownNeighbour()))
        ++num;
      if (this.GenerateChunk(chunk.LeftBackwardDownNeighbour()))
        ++num;
      return num;
    }

    private int DecorateChunkAndGenerateNeighbours(MapChunk chunk)
    {
      int num = 0;
      if (chunk != null)
      {
        num += this.GenerateNeighbours(chunk);
        if (this.DecorateChunk(chunk))
          ++num;
      }
      return num;
    }

    private int DecorateNeighbours(MapChunk chunk)
    {
      return 0 + this.DecorateChunkAndGenerateNeighbours(chunk.LeftNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftForwardNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.ForwardNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightForwardNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightBackwardNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.BackwardNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftBackwardNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.UpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftForwardUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.ForwardUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightForwardUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightBackwardUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.BackwardUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftBackwardUpNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.DownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftDownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftForwardDownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.ForwardDownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightForwardDownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightDownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.RightBackwardDownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.BackwardDownNeighbour()) + this.DecorateChunkAndGenerateNeighbours(chunk.LeftBackwardDownNeighbour());
    }

    private int DecoratePendingChunkAndDecorateNeighbours(MapChunk chunk)
    {
      int num = 0;
      if (chunk != null)
      {
        num += this.DecorateNeighbours(chunk);
        if (this.DecoratePendingChunk(chunk))
          ++num;
      }
      return num;
    }

    private void UnloadChunksToReduceMeshSize(int amountToUnload)
    {
      this.UnloadChunksToReduceMeshSizeCore(amountToUnload);
    }

    private int UnloadChunksToReduceMeshSizeCore(int amountToUnload)
    {
      bool flag1 = true;
      int length = 4;
      if (this.furthest == null || this.furthest.Length < length)
      {
        this.furthest = new float[length];
        this.furthestChunk = new MapChunkTM[length];
      }
      Point3D point3D = this.map.ChunkSize / 2;
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D2 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      int num1 = 9216;
      BoundingBox boundingBox = new BoundingBox();
      int num2 = 1000;
      int num3 = 0;
      while (num3 < amountToUnload && --num2 > 0)
      {
        for (int index = 0; index < this.furthest.Length; ++index)
        {
          this.furthest[index] = (float) num1;
          this.furthestChunk[index] = (MapChunkTM) null;
        }
        Dictionary<int, MapRegion> regions = this.map.Regions;
        foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
        {
          Player tag = localGamer.Tag as Player;
          if (tag != null && tag.IsEnabledField)
          {
            GlobalPoint3D point = this.map.GetPoint(tag.EyePosition);
            this.frustum.Matrix = tag.ViewMatrix * tag.ProjectionMatrix;
            Player virtualPlayer = tag.VirtualPlayer;
            bool flag2 = virtualPlayer != tag;
            if (flag2)
            {
              p2 = this.map.GetPoint(virtualPlayer.EyePosition);
              this.frustum2.Matrix = virtualPlayer.ViewMatrix * tag.ProjectionMatrix;
            }
            foreach (MapRegion mapRegion in regions.Values)
            {
              foreach (MapChunk chunk in mapRegion.Chunks)
              {
                if (chunk != null && !chunk.IsMeshDirty)
                {
                  MapChunkTM mapChunkTm = chunk as MapChunkTM;
                  if (mapChunkTm != null && mapChunkTm.TotalMeshSize > 1000)
                  {
                    if (flag1)
                    {
                      BoundingBox box = mapChunkTm.Box;
                      box.Min += mapRegion.Box.Min;
                      box.Max += mapRegion.Box.Min;
                      if (this.frustum.FastIntersect(ref box) || flag2 && this.frustum2.FastIntersect(ref box))
                        continue;
                    }
                    GlobalPoint3D p1 = chunk.GlobalOffset + point3D;
                    float val1 = GlobalPoint3D.DistanceSquared(p1, point);
                    float num4 = flag2 ? Math.Min(val1, GlobalPoint3D.DistanceSquared(p1, p2)) : val1;
                    for (int index = 0; index < this.furthest.Length; ++index)
                    {
                      if ((double) num4 > (double) this.furthest[index])
                      {
                        this.furthest[index] = num4;
                        this.furthestChunk[index] = mapChunkTm;
                        break;
                      }
                    }
                  }
                }
              }
            }
          }
        }
        for (int index = 0; index < this.furthest.Length; ++index)
        {
          MapChunkTM mapChunkTm = this.furthestChunk[index];
          if (mapChunkTm != null)
          {
            int totalMeshSize = mapChunkTm.TotalMeshSize;
            num3 += totalMeshSize;
            this.instance.MeshSizeReduced((long) totalMeshSize);
            mapChunkTm.UnloadMesh();
          }
        }
      }
      return num3;
    }

    private void UnloadSplitChunks()
    {
      MapTM map = this.map;
      this.chunksToUnsplit.Clear();
      if (map.MapChunkContentBreakdown.Count <= 0)
        return;
      lock (map.MapChunkContentBreakdown)
      {
        foreach (KeyValuePair<long, MapChunkContentData[]> keyValuePair in map.MapChunkContentBreakdown)
        {
          MapChunkTM chunk = this.map.GetChunk(keyValuePair.Key) as MapChunkTM;
          if (chunk != null && chunk.LastBlockEditedIndex < 0)
            this.chunksToUnsplit.Add(chunk);
        }
      }
      foreach (MapChunkTM chunk in this.chunksToUnsplit)
      {
        if (!this.instance.IsInAnyLocalPlayerRange((MapChunk) chunk, 1, false))
        {
          chunk.SetChunkFlag(ChunkFlags.MeshDirty);
          chunk.Content.Alpha = byte.MaxValue;
          this.LoadChunk(chunk);
        }
      }
    }

    private struct PlayerChunk
    {
      public MapChunk Chunk;
      public float Distance;
      public bool Priority;
    }
  }
}
