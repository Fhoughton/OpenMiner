// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MapChunkTM
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.IO;

namespace StudioForge.TotalMiner
{
  internal class MapChunkTM : MapChunk
  {
    public static int MaxQueuedChunkRequests = 75;
    public MapChunkContent Content;
    public static bool UpdateItemOverrideNotThreaded;

    public int TotalMeshSize
    {
      get
      {
        if (this.Content == null)
          return 0;
        return this.Content.TotalMeshSize((MapChunk) this);
      }
    }

    public bool NeedsPriorityLoading
    {
      get
      {
        return this.LastBlockEditedIndex >= 0;
      }
    }

    public MapChunkTM(MapRegion region, Point3D offset)
      : base(region, offset)
    {
    }

    public override void LoadContent(InitState state)
    {
      base.LoadContent(state);
      this.Content = new MapChunkContent((MapChunk) this);
    }

    public override void UnloadContent()
    {
      this.Content.UnloadChunk(this);
      base.UnloadContent();
    }

    public override void UnloadForRecycle()
    {
      this.UnloadMesh();
      base.UnloadForRecycle();
    }

    public void UnloadMesh()
    {
      this.Content.UnloadChunk(this);
    }

    private void UpdateItem(GameInstance instance, IThreadWorkItem worker, bool priority)
    {
      if (MapChunkTM.UpdateItemOverrideNotThreaded)
        worker.Update();
      else
        ThreadQueueManager.Instance.QueueWorkItem(worker, true, priority ? PriorityLevel.Urgent : PriorityLevel.Normal);
    }

    public override bool ShouldGenerate(bool skipRequestsNotReceivedCheck)
    {
      if (skipRequestsNotReceivedCheck || NetworkManager.Instance.ChunksRequestedNotReceivedCount < MapChunkTM.MaxQueuedChunkRequests)
        return base.ShouldGenerate(skipRequestsNotReceivedCheck);
      return false;
    }

    protected override void GenerateCore()
    {
      if (!Globals2.GameProperties.SaveGame.Header.Pre18)
      {
        int next = ChunkGenerator.Pool.GetNext();
        ChunkGenerator chunkGenerator = ChunkGenerator.Pool.List[next];
        GameInstance instance = (this.Region.Map as MapTM).Instance;
        chunkGenerator.Initialize(next, instance, instance.CurrentBiome, (MapChunk) this);
        this.UpdateItem(instance, (IThreadWorkItem) chunkGenerator, false);
      }
      else
        this.GenerateEnd();
      if (NetworkManager.Instance.IsHost)
        return;
      NetworkManager.Instance.EnqueueChunkRequest(this);
    }

    protected override void DecorateCore()
    {
      if (!Globals2.GameProperties.SaveGame.Header.Pre18)
      {
        int next = ChunkDecorator.Pool.GetNext();
        ChunkDecorator chunkDecorator = ChunkDecorator.Pool.List[next];
        GameInstance instance = (this.Region.Map as MapTM).Instance;
        chunkDecorator.Initialize(next, instance, instance.CurrentBiome, (MapChunk) this);
        this.UpdateItem(instance, (IThreadWorkItem) chunkDecorator, false);
      }
      else
        this.DecorateEnd();
    }

    public override void DecorateEnd()
    {
      base.DecorateEnd();
      MapTM map = this.Region.Map as MapTM;
      if (!map.IsHost || map.IsChunkPending((MapChunk) this))
        return;
      ((MapTM) this.Region.Map).Instance.NetworkManager.ChunkIsDecorated((MapChunk) this, ChunkFlags.ReceivedFromHost);
    }

    public override bool ShouldDecoratePending
    {
      get
      {
        if (this.IsChunkFlagsSet(ChunkFlags.Generated | ChunkFlags.Decorated) && !this.IsChunkFlagSet(ChunkFlags.Decorating))
        {
          MapTM map = this.Region.Map as MapTM;
          if (map != null && map.IsChunkPending((MapChunk) this))
            return this.AllNeighboursMeetCondition(map.CanProcessPendingChunkTest);
        }
        return false;
      }
    }

    protected override void DecoratePendingCore()
    {
      int next = ChunkDecoratorPending.Pool.GetNext();
      ChunkDecoratorPending decoratorPending = ChunkDecoratorPending.Pool.List[next];
      GameInstance instance = (this.Region.Map as MapTM).Instance;
      decoratorPending.Initialize(next, this);
      this.UpdateItem(instance, (IThreadWorkItem) decoratorPending, false);
    }

    public void DecoratePendingChunkData()
    {
      MapTM map = this.Region.Map as MapTM;
      MapChunkPendingData data;
      if (!map.GetAndRemoveChunkPendingData((MapChunk) this, out data))
        return;
      this.DecoratePendingCore2(map, data);
    }

    private void DecoratePendingCore2(MapTM map, MapChunkPendingData data)
    {
      MapStrategyTM mapStrategy = map.MapStrategy as MapStrategyTM;
      lock (this.RleLock)
      {
        try
        {
          this.BlockData.SetStream((MapChunk) this, data.BlockData.StreamID, data.BlockData.StreamIndex, data.BlockData.StreamSize);
          this.LightData.SetStream((MapChunk) this, data.LightData.StreamID, data.LightData.StreamIndex, data.LightData.StreamSize);
          this.AuxData.SetStream((MapChunk) this, data.AuxData.StreamID, data.AuxData.StreamIndex, data.AuxData.StreamSize);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(54, ex);
        }
        if (this.BlockData.StreamSize == 2)
        {
          if (this.BlockData.GetStreamDataNoLock(0) == (byte) 0)
            data.Flags |= ChunkFlags.ChunkIsAllAir;
        }
      }
      try
      {
        map.LoadDataBlocksFromChunkDataAndDoAnyChunkDataConversion((MapChunk) this, data.Sender);
        map.UpdateHeightData((MapChunk) this, (data.Flags & ChunkFlags.ChunkIsAllAir) == ChunkFlags.ChunkIsAllAir);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(43, ex);
      }
      ChunkFlags chunkFlags = data.Flags | ChunkFlags.ReceivedFromHost | ChunkFlags.MeshDirty;
      if (data.LightData.StreamSize == 0)
        chunkFlags |= ChunkFlags.LightDirty;
      if (map.IsHost)
        map.Instance.NetworkManager.ChunkIsDecorated((MapChunk) this, chunkFlags);
      else
        this.SetChunkFlag(chunkFlags);
      mapStrategy.ApplyDataBlocksOnChunkDecorated((MapChunk) this);
    }

    protected override void LightCore(bool priority)
    {
      if (!this.IsChunkFlagSet(ChunkFlags.ChunkIsAllAir | ChunkFlags.ChunkIsAllSolid))
      {
        int next = MapLightingByChunkThreadedWrapper.Pool.GetNext();
        MapLightingByChunkThreadedWrapper chunkThreadedWrapper = MapLightingByChunkThreadedWrapper.Pool.List[next];
        GameInstance instance = (this.Region.Map as MapTM).Instance;
        chunkThreadedWrapper.Initialize(instance, this.Region.Map, next, (MapChunk) this, false);
        this.UpdateItem(instance, (IThreadWorkItem) chunkThreadedWrapper, priority | this.NeedsPriorityLoading);
      }
      else
        this.LightEnd();
    }

    public override void LightEnd()
    {
      base.LightEnd();
      GameInstance instance = ((MapTM) this.Region.Map).Instance;
      if (instance == null || instance.NpcManager == null)
        return;
      instance.NpcManager.LightHasChanged((MapChunk) this);
    }

    protected override void LoadMeshCore(
      bool priority,
      bool useNewBuilder,
      Action<bool, object> action,
      object state)
    {
      if (this.IsChunkFlagsSet(ChunkFlags.ChunkIsAllSolid) ? this.CheckForNonSolidNeighbours() : !this.IsChunkFlagsSet(ChunkFlags.ChunkIsAllAir))
      {
        int next = ChunkMeshCreator.Pool.GetNext();
        ChunkMeshCreator chunkMeshCreator = ChunkMeshCreator.Pool.List[next];
        GameInstance instance = (this.Region.Map as MapTM).Instance;
        chunkMeshCreator.Initialize(next, instance, this, useNewBuilder, action, state);
        this.UpdateItem(instance, (IThreadWorkItem) chunkMeshCreator, priority | this.NeedsPriorityLoading);
      }
      else
      {
        this.UpdateFlags = ChunkUpdateFlags.None;
        this.LoadMeshEnd(true);
      }
    }

    public override void LoadMeshEnd(bool success)
    {
      base.LoadMeshEnd(success);
    }

    private bool CheckForNonSolidNeighbours()
    {
      bool flag = false;
      MapChunk mapChunk1 = this.LeftNeighbour();
      if (mapChunk1 != null && !mapChunk1.IsChunkFlagsSet(ChunkFlags.ChunkIsAllSolid))
        flag = true;
      if (!flag)
      {
        MapChunk mapChunk2 = this.ForwardNeighbour();
        if (mapChunk2 != null && !mapChunk2.IsChunkFlagsSet(ChunkFlags.ChunkIsAllSolid))
          flag = true;
      }
      if (!flag)
      {
        MapChunk mapChunk2 = this.RightNeighbour();
        if (mapChunk2 != null && !mapChunk2.IsChunkFlagsSet(ChunkFlags.ChunkIsAllSolid))
          flag = true;
      }
      if (!flag)
      {
        MapChunk mapChunk2 = this.BackwardNeighbour();
        if (mapChunk2 != null && !mapChunk2.IsChunkFlagsSet(ChunkFlags.ChunkIsAllSolid))
          flag = true;
      }
      if (!flag)
      {
        MapChunk mapChunk2 = this.UpNeighbour();
        if (mapChunk2 != null && !mapChunk2.IsChunkFlagsSet(ChunkFlags.ChunkIsAllSolid))
          flag = true;
      }
      if (!flag)
      {
        MapChunk mapChunk2 = this.DownNeighbour();
        if (mapChunk2 != null && !mapChunk2.IsChunkFlagsSet(ChunkFlags.ChunkIsAllSolid))
          flag = true;
      }
      return flag;
    }

    protected override void ReadDataCore(BinaryReader reader, int version)
    {
      ((MapTM) this.Region.Map).AddChunkPendingData((MapChunk) this, new MapChunkPendingData()
      {
        BlockData = this.ReadStreamData(reader, version),
        LightData = this.ReadStreamData(reader, version),
        AuxData = this.ReadStreamData(reader, version)
      });
    }

    private MapChunkPendingStream ReadStreamData(
      BinaryReader reader,
      int version)
    {
      MapChunkPendingStream chunkPendingStream = new MapChunkPendingStream();
      chunkPendingStream.StreamSize = reader.ReadInt32();
      if (version < 118)
        reader.ReadInt32();
      if (chunkPendingStream.StreamSize > 0)
      {
        lock (this.RleLock)
        {
          Map.RLEStreamBufferManager.Allocate(chunkPendingStream.StreamSize, out chunkPendingStream.StreamID, out chunkPendingStream.StreamIndex);
          reader.Read(Map.RLEStreamBufferManager.Stream[(int) chunkPendingStream.StreamID], chunkPendingStream.StreamIndex, chunkPendingStream.StreamSize);
        }
      }
      return chunkPendingStream;
    }

    protected override void WriteChunkData(BinaryWriter writer)
    {
      MapTM map = (MapTM) this.Region.Map;
      lock (this.RleLock)
      {
        MapChunkPendingData data;
        if (map.GetChunkPendingData((MapChunk) this, out data))
          this.WriteChunkDataFromPending(writer, data);
        else
          base.WriteChunkData(writer);
      }
    }

    private void WriteChunkDataFromPending(BinaryWriter writer, MapChunkPendingData data)
    {
      this.WriteRLEStream(writer, data.BlockData);
      this.WriteRLEStream(writer, data.LightData);
      this.WriteRLEStream(writer, data.AuxData);
    }

    private void WriteRLEStream(BinaryWriter writer, MapChunkPendingStream data)
    {
      writer.Write(data.StreamSize);
      if (data.StreamSize <= 0)
        return;
      writer.Write(Map.RLEStreamBufferManager.Stream[(int) data.StreamID], data.StreamIndex, data.StreamSize);
    }
  }
}
