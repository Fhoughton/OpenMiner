// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapChunk
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.BlockWorld
{
  public class MapChunk : IEnumerable, IHasContent, IEquatable<MapChunk>
  {
    public static ChunkUpdateFlags borderFlags = ChunkUpdateFlags.LeftChunkBorder | ChunkUpdateFlags.ForwardChunkBorder | ChunkUpdateFlags.RightChunkBorder | ChunkUpdateFlags.BackChunkBorder | ChunkUpdateFlags.UpChunkBorder | ChunkUpdateFlags.DownChunkBorder;
    public static ChunkUpdateFlags segmentFlags = ChunkUpdateFlags.LeftSegmentBorder | ChunkUpdateFlags.ForwardSegmentBorder | ChunkUpdateFlags.RightSegmentBorder | ChunkUpdateFlags.BackSegmentBorder | ChunkUpdateFlags.UpSegmentBorder | ChunkUpdateFlags.DownSegmentBorder;
    protected static List<MapChunk> neighbours = new List<MapChunk>(28);
    public object RleLock = new object();
    private ChunkFlags chunkFlagsToKeepOnRead = ChunkFlags.LightDirty | ChunkFlags.UserEdited | ChunkFlags.HasSpecialBlocks;
    private const ChunkFlags airOrSolid = ChunkFlags.ChunkIsAllAir | ChunkFlags.ChunkIsAllSolid;
    public MapRegion Region;
    public Point3D Offset;
    public BoundingBox Box;
    public RLEStreamByte BlockData;
    public RLEStreamByte LightData;
    public RLEStreamByte AuxData;
    public ChunkUpdateFlags UpdateFlags;
    public int LastBlockEditedIndex;
    private ChunkFlags flags;
    private static ChunkTest isChunkLitTest;
    private static ChunkTest isChunkLightingTest;
    private static ChunkTest isChunkGeneratedTest;
    private static ChunkTest isChunkDecoratedTest;
    private static ChunkTest isChunkDecoratedWithoutReceiveCheckTest;
    private static ChunkTest isChunkShouldLightFirst;
    private static ChunkTest isChunkDecoratedAndLitTest;

    public ChunkFlags GetChunkFlagMask(ChunkFlags flag)
    {
      return this.flags & flag;
    }

    public bool IsChunkFlagSet(ChunkFlags flag)
    {
      return (this.flags & flag) > ChunkFlags.None;
    }

    public bool IsChunkFlagsSet(ChunkFlags flag)
    {
      return (this.flags & flag) == flag;
    }

    public void SetChunkFlag(ChunkFlags flag)
    {
      this.flags |= flag;
    }

    public void ClearChunkFlag(ChunkFlags flag)
    {
      this.flags &= ~flag;
    }

    public bool IsUpdateFlagSet(ChunkUpdateFlags flag)
    {
      return (this.UpdateFlags & flag) > ChunkUpdateFlags.None;
    }

    public void SetUpdateFlag(ChunkUpdateFlags flag)
    {
      this.UpdateFlags |= flag;
    }

    public void ClearUpdateFlag(ChunkUpdateFlags flag)
    {
      this.UpdateFlags &= ~flag;
    }

    public void SetChunkFlagForSelfAndOthers(ChunkFlags flags, List<MapChunk> others)
    {
      this.SetChunkFlag(flags);
      foreach (MapChunk other in others)
        other.SetChunkFlag(flags);
    }

    public void CloneFlags(MapChunk chunk)
    {
      this.flags = chunk.flags;
      this.UpdateFlags = chunk.UpdateFlags;
    }

    private Map Map
    {
      get
      {
        return this.Region.Map;
      }
    }

    public GlobalPoint3D GlobalOffset
    {
      get
      {
        return this.GetGlobalPoint(Point3D.Zero);
      }
    }

    public bool IsValidPoint(GlobalPoint3D p)
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      globalPoint3D.X = this.Offset.X + this.Region.Offset.X;
      globalPoint3D.Y = this.Offset.Y + this.Region.Offset.Y;
      globalPoint3D.Z = this.Offset.Z + this.Region.Offset.Z;
      if (p.X >= globalPoint3D.X && p.X < globalPoint3D.X + chunkSize.X && (p.Y >= globalPoint3D.Y && p.Y < globalPoint3D.Y + chunkSize.Y) && p.Z >= globalPoint3D.Z)
        return p.Z < globalPoint3D.Z + chunkSize.Z;
      return false;
    }

    public override int GetHashCode()
    {
      return this.Offset.GetHashCode();
    }

    public long GetGlobalHashCode()
    {
      return ((long) this.Region.GetHashCode() << 32) + (long) this.Offset.GetHashCode();
    }

    public static Point3D GetChunkOffsetOld(Map map, long hash)
    {
      return MapChunk.GetChunkOffsetOld(map, (int) hash);
    }

    public static Point3D GetChunkOffsetOld(Map map, int hash)
    {
      Point3D point3D = new Point3D();
      point3D.X = hash >> 21;
      point3D.Y = hash & 1023;
      point3D.Z = hash >> 10 & 2047;
      Point3D chunkSize = map.ChunkSize;
      point3D.X *= chunkSize.X;
      point3D.Y *= chunkSize.Y;
      point3D.Z *= chunkSize.Z;
      return point3D;
    }

    public bool IsFillOf(byte blockID)
    {
      lock (this.RleLock)
        return this.BlockData.StreamSize == 2 && (int) this.BlockData.GetStreamDataNoLock(0) == (int) blockID;
    }

    public virtual int MemorySize
    {
      get
      {
        return 74 + this.BlockData.MemorySize + this.LightData.MemorySize + this.AuxData.MemorySize;
      }
    }

    public int Seed
    {
      get
      {
        return this.Region.GetHashCode() + this.GetHashCode() + this.Region.Map.Seed;
      }
    }

    public bool Equals(MapChunk c)
    {
      return this.GetGlobalHashCode() == c.GetGlobalHashCode();
    }

    public void ClearTimeStamps()
    {
      this.SetTimeStamps(0);
    }

    public void SetTimeStamps(int timestamp)
    {
      if (this.BlockData != null)
        this.BlockData.TimeStamp = timestamp;
      if (this.LightData != null)
        this.LightData.TimeStamp = timestamp;
      if (this.AuxData == null)
        return;
      this.AuxData.TimeStamp = timestamp;
    }

    public MapChunk(MapRegion region, Point3D offset)
    {
      this.Region = region;
      this.LastBlockEditedIndex = -1;
      this.ResetOffset(offset);
      this.BlockData = new RLEStreamByte(this, (byte) 0);
      this.LightData = new RLEStreamByte(this, region.Map.SunLight.ToByte());
      this.AuxData = new RLEStreamByte(this, (byte) 0);
    }

    private void ResetOffset(Point3D offset)
    {
      this.Offset = offset;
      Vector3 min = new Vector3((float) offset.X * this.Map.TileSize, (float) offset.Y * this.Map.TileSize, (float) offset.Z * this.Map.TileSize);
      Vector3 max = min + new Vector3((float) this.Map.ChunkSize.X * this.Map.TileSize, (float) this.Map.ChunkSize.Y * this.Map.TileSize, (float) this.Map.ChunkSize.Z * this.Map.TileSize);
      this.Box = new BoundingBox(min, max);
    }

    public void Reset(ChunkFlags resetFlags)
    {
      this.BlockData.Fill(this, (byte) 0);
      this.LightData.Fill(this, this.Region.Map.SunLight.ToByte());
      this.AuxData.Fill(this, (byte) 0);
      this.LastBlockEditedIndex = -1;
      this.flags = resetFlags;
      this.UpdateFlags = ChunkUpdateFlags.None;
    }

    public virtual void LoadContent(InitState state)
    {
    }

    public virtual void UnloadContent()
    {
      this.BlockData.UnloadContent(this);
      this.LightData.UnloadContent(this);
      this.AuxData.UnloadContent(this);
    }

    public virtual void UnloadForRecycle()
    {
      this.BlockData.UnloadContent(this);
      this.LightData.UnloadContent(this);
      this.AuxData.UnloadContent(this);
    }

    public static void UnloadStaticContent()
    {
      MapChunk.isChunkLitTest = (ChunkTest) null;
      MapChunk.isChunkLightingTest = (ChunkTest) null;
      MapChunk.isChunkGeneratedTest = (ChunkTest) null;
      MapChunk.isChunkDecoratedTest = (ChunkTest) null;
      MapChunk.isChunkDecoratedWithoutReceiveCheckTest = (ChunkTest) null;
      MapChunk.isChunkShouldLightFirst = (ChunkTest) null;
      MapChunk.isChunkDecoratedAndLitTest = (ChunkTest) null;
    }

    public int GetMapIndex(Point3D point)
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      int num1 = point.X % chunkSize.X;
      int num2 = point.Y % chunkSize.Y;
      int num3 = point.Z % chunkSize.Z;
      return num1 + num3 * chunkSize.X + num2 * (chunkSize.X * chunkSize.Z);
    }

    public int GetMapIndex(GlobalPoint3D point)
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      GlobalPoint3D min = this.Region.Map.MapBound.Min;
      int num1 = (point.X - min.X) % chunkSize.X;
      int num2 = (point.Y - min.Y) % chunkSize.Y;
      int num3 = (point.Z - min.Z) % chunkSize.Z;
      return num1 + num3 * chunkSize.X + num2 * (chunkSize.X * chunkSize.Z);
    }

    public Point3D GetPoint(int index)
    {
      Point3D point3D = new Point3D();
      Point3D chunkSize = this.Region.Map.ChunkSize;
      int num1 = chunkSize.X * chunkSize.Z;
      int num2 = index % num1;
      point3D.X = num2 % chunkSize.X;
      point3D.Y = (index - num2) / num1;
      point3D.Z = (num2 - point3D.X) / chunkSize.X;
      return point3D;
    }

    public GlobalPoint3D GetGlobalPoint(Point3D p)
    {
      return new GlobalPoint3D()
      {
        X = p.X + this.Offset.X + this.Region.Offset.X,
        Y = p.Y + this.Offset.Y + this.Region.Offset.Y,
        Z = p.Z + this.Offset.Z + this.Region.Offset.Z
      };
    }

    public Point3D GetLocalPoint(GlobalPoint3D p)
    {
      return new Point3D()
      {
        X = p.X - this.Offset.X - this.Region.Offset.X,
        Y = p.Y - this.Offset.Y - this.Region.Offset.Y,
        Z = p.Z - this.Offset.Z - this.Region.Offset.Z
      };
    }

    public byte GetBlockID(Point3D p)
    {
      return this.BlockData.GetData(this, this.GetMapIndex(p));
    }

    public byte GetBlockIDNoCache(Point3D p)
    {
      return this.BlockData.GetDataNoCache(this, this.GetMapIndex(p));
    }

    public byte GetBlockID(int index)
    {
      return this.BlockData.GetData(this, index);
    }

    public byte GetBlockID_Test(int index)
    {
      return this.BlockData.GetData_Test(this, index);
    }

    public MapLight GetLight(Point3D p)
    {
      return MapLight.FromByte(this.LightData.GetData(this, this.GetMapIndex(p)));
    }

    public MapLight GetLightNoCache(Point3D p)
    {
      return MapLight.FromByte(this.LightData.GetDataNoCache(this, this.GetMapIndex(p)));
    }

    public byte GetSunLight(Point3D p)
    {
      return (byte) ((uint) this.LightData.GetData(this, this.GetMapIndex(p)) >> 4);
    }

    public byte GetBlockLight(Point3D p)
    {
      return (byte) ((uint) this.LightData.GetData(this, this.GetMapIndex(p)) & 15U);
    }

    public byte GetAUXData(Point3D p)
    {
      return (byte) ((uint) this.AuxData.GetData(this, this.GetMapIndex(p)) & 7U);
    }

    public byte GetAUXDataNoCache(Point3D p)
    {
      return (byte) ((uint) this.AuxData.GetDataNoCache(this, this.GetMapIndex(p)) & 7U);
    }

    public byte GetAUXHighData(int index)
    {
      return (byte) ((uint) this.AuxData.GetData(this, index) >> 4);
    }

    public byte GetAUXHighData(Point3D p)
    {
      return (byte) ((uint) this.AuxData.GetData(this, this.GetMapIndex(p)) >> 4);
    }

    public byte GetAUXHighDataNoCache(int index)
    {
      return (byte) ((uint) this.AuxData.GetDataNoCache(this, index) >> 4);
    }

    public byte GetAUXHighDataNoCache(Point3D p)
    {
      return (byte) ((uint) this.AuxData.GetDataNoCache(this, this.GetMapIndex(p)) >> 4);
    }

    public byte GetAUXFullData(int index)
    {
      return this.AuxData.GetData(this, index);
    }

    public byte GetAUXFullData(Point3D p)
    {
      return this.AuxData.GetData(this, this.GetMapIndex(p));
    }

    public byte GetAUXFullDataNoCache(int index)
    {
      return this.AuxData.GetDataNoCache(this, index);
    }

    public byte GetAUXFullDataNoCache(Point3D p)
    {
      return this.AuxData.GetDataNoCache(this, this.GetMapIndex(p));
    }

    public MapBlock GetBlockData(Point3D p)
    {
      return this.GetBlockData(this.GetMapIndex(p));
    }

    public MapBlock GetBlockData(int index)
    {
      return new MapBlock()
      {
        Chunk = this,
        BlockID = this.BlockData.GetData(this, index),
        Light = MapLight.FromByte(this.LightData.GetData(this, index)),
        AuxData = this.AuxData.GetData(this, index)
      };
    }

    public MapBlock GetBlockAndLight(Point3D p)
    {
      return this.GetBlockAndLight(this.GetMapIndex(p));
    }

    public MapBlock GetBlockAndLight(int index)
    {
      return new MapBlock()
      {
        Chunk = this,
        BlockID = this.BlockData.GetData(this, index),
        Light = MapLight.FromByte(this.LightData.GetData(this, index))
      };
    }

    public MapBlock GetBlockIDAndAux(Point3D p)
    {
      int mapIndex = this.GetMapIndex(p);
      return new MapBlock()
      {
        Chunk = this,
        BlockID = this.BlockData.GetData(this, mapIndex),
        AuxData = this.AuxData.GetData(this, mapIndex)
      };
    }

    public MapBlock GetBlockIDAndAux(int index)
    {
      return new MapBlock()
      {
        Chunk = this,
        BlockID = this.BlockData.GetData(this, index),
        AuxData = this.AuxData.GetData(this, index)
      };
    }

    public MapBlock GetBlockIDAndAuxNoCache(int index)
    {
      return new MapBlock()
      {
        Chunk = this,
        BlockID = this.BlockData.GetDataNoCache(this, index),
        AuxData = this.AuxData.GetDataNoCache(this, index)
      };
    }

    public void SetBlockData(Point3D p, MapBlock data, UpdateBlockMethod method)
    {
      this.OnBlockDataUpdated(ref data, method);
      int mapIndex = this.GetMapIndex(p);
      lock (this.RleLock)
      {
        this.BlockData.SetDataNoLock(this, mapIndex, data.BlockID);
        this.LightData.SetDataNoLock(this, mapIndex, data.Light.ToByte());
        this.AuxData.SetDataNoLock(this, mapIndex, data.AuxData);
      }
    }

    public void SetBlockIDAndAux(Point3D p, MapBlock data, UpdateBlockMethod method)
    {
      this.OnBlockDataUpdated(ref data, method);
      int mapIndex = this.GetMapIndex(p);
      lock (this.RleLock)
      {
        this.BlockData.SetDataNoLock(this, mapIndex, data.BlockID);
        this.AuxData.SetDataNoLock(this, mapIndex, data.AuxData);
      }
    }

    private void OnBlockDataUpdated(ref MapBlock data, UpdateBlockMethod method)
    {
      if (method != UpdateBlockMethod.Generation)
      {
        this.flags |= ChunkFlags.UserEdited;
        data.AuxData |= (byte) 8;
      }
      if ((this.flags & (ChunkFlags.ChunkIsAllAir | ChunkFlags.ChunkIsAllSolid)) <= ChunkFlags.None)
        return;
      if ((this.flags & ChunkFlags.ChunkIsAllSolid) > ChunkFlags.None)
      {
        if (this.Region.Map.GetBlockBufferType(data.BlockID) <= (byte) 1)
          return;
        this.ClearChunkFlag(ChunkFlags.ChunkIsAllSolid);
      }
      else
      {
        if (data.BlockID <= (byte) 0)
          return;
        this.ClearChunkFlag(ChunkFlags.ChunkIsAllAir);
      }
    }

    public void SetAuxData(Point3D p, byte auxData, UpdateBlockMethod method)
    {
      if (method != UpdateBlockMethod.Generation)
      {
        this.flags |= ChunkFlags.UserEdited;
        auxData |= (byte) 8;
      }
      this.AuxData.SetData(this, this.GetMapIndex(p), auxData);
    }

    public bool IsGenerated
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.Generated);
      }
    }

    public bool IsGeneratedOrIsGenerating
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.Generated | ChunkFlags.Generating);
      }
    }

    public virtual bool ShouldGenerate(bool skipRequestsNotReceivedCheck)
    {
      return !this.IsChunkFlagSet(ChunkFlags.Generated | ChunkFlags.Generating);
    }

    public void Generate()
    {
      this.SetChunkFlag(ChunkFlags.Generating);
      this.GenerateCore();
    }

    protected virtual void GenerateCore()
    {
    }

    public virtual void GenerateEnd()
    {
      this.SetChunkFlag(ChunkFlags.Generated);
      this.ClearChunkFlag(ChunkFlags.Generating);
    }

    public bool IsDecorated
    {
      get
      {
        return this.IsChunkFlagsSet(ChunkFlags.Generated | ChunkFlags.Decorated | ChunkFlags.ReceivedFromHost);
      }
    }

    public bool IsDecoratedWithoutReceivedCheck
    {
      get
      {
        return this.IsChunkFlagsSet(ChunkFlags.Generated | ChunkFlags.Decorated);
      }
    }

    public bool IsDecoratedOrIsDecorating
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.Decorated | ChunkFlags.Decorating);
      }
    }

    public virtual bool ShouldDecorate
    {
      get
      {
        if (this.IsGenerated && !this.IsDecoratedOrIsDecorating)
          return this.AllNeighboursGenerated;
        return false;
      }
    }

    public void Decorate()
    {
      this.SetChunkFlag(ChunkFlags.Decorating);
      this.DecorateCore();
    }

    protected virtual void DecorateCore()
    {
    }

    public virtual void DecorateEnd()
    {
      this.SetChunkFlag(ChunkFlags.Generated | ChunkFlags.Decorated);
      this.ClearChunkFlag(ChunkFlags.Decorating);
      ++this.Region.ChunksDecoratedCount;
    }

    public virtual bool ShouldDecoratePending
    {
      get
      {
        return false;
      }
    }

    public void DecoratePending()
    {
      this.SetChunkFlag(ChunkFlags.Decorating);
      this.DecoratePendingCore();
    }

    protected virtual void DecoratePendingCore()
    {
    }

    public virtual void DecoratePendingEnd()
    {
      this.ClearChunkFlag(ChunkFlags.Decorating);
    }

    public bool IsLightDirty
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.LightDirty);
      }
    }

    public bool IsLighting
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.Lighting);
      }
    }

    public virtual bool ShouldLight
    {
      get
      {
        return this.IsChunkFlagsSet(ChunkFlags.Generated | ChunkFlags.Decorated | ChunkFlags.LightDirty | ChunkFlags.ReceivedFromHost) && !this.IsChunkFlagSet(ChunkFlags.Lighting | ChunkFlags.MeshLoading | ChunkFlags.Committed) && (this.AllNeighboursDecorated && !this.NeighbourLightingInProgress);
      }
    }

    public void Light(bool priority)
    {
      this.SetChunkFlag(ChunkFlags.Lighting);
      this.ClearChunkFlag(ChunkFlags.LightDirty);
      this.LightCore(priority);
    }

    protected virtual void LightCore(bool priority)
    {
    }

    public virtual void LightEnd()
    {
      this.SetChunkFlag(ChunkFlags.MeshDirty);
      this.ClearChunkFlag(ChunkFlags.Lighting);
    }

    public bool IsMeshLoaded
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.MeshLoaded);
      }
    }

    public bool IsMeshDirty
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.MeshDirty);
      }
    }

    private bool IsMeshLoading
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.MeshLoading);
      }
    }

    public virtual bool ShouldLoadMesh
    {
      get
      {
        return this.IsChunkFlagsSet(ChunkFlags.Generated | ChunkFlags.Decorated | ChunkFlags.MeshDirty | ChunkFlags.ReceivedFromHost) && !this.IsChunkFlagSet(ChunkFlags.LightDirty | ChunkFlags.Lighting | ChunkFlags.Committed) && this.AllNeighboursDecoratedAndLit;
      }
    }

    public void LoadMesh(bool priority, bool useNewBuilder)
    {
      this.LoadMesh(priority, useNewBuilder, (Action<bool, object>) null, (object) null);
    }

    public void LoadMesh(
      bool priority,
      bool useNewBuilder,
      Action<bool, object> action,
      object state)
    {
      this.SetChunkFlag(ChunkFlags.MeshLoading);
      this.ClearChunkFlag(ChunkFlags.MeshDirty);
      this.ClearUpdateFlag(MapChunk.borderFlags);
      this.LoadMeshCore(priority, useNewBuilder, action, state);
    }

    protected virtual void LoadMeshCore(
      bool priority,
      bool useNewBuilder,
      Action<bool, object> action,
      object state)
    {
    }

    public virtual void LoadMeshEnd(bool success)
    {
      this.SetChunkFlag(success ? ChunkFlags.MeshLoaded : ChunkFlags.MeshDirty);
      this.ClearChunkFlag(ChunkFlags.MeshLoading);
    }

    public bool NeighboursNeedLoadingFirst
    {
      get
      {
        if ((this.UpdateFlags & ChunkUpdateFlags.LeftChunkBorder) == ChunkUpdateFlags.LeftChunkBorder)
        {
          MapChunk mapChunk = this.LeftNeighbour();
          if (mapChunk != null && mapChunk.IsChunkFlagSet(ChunkFlags.MeshDirty | ChunkFlags.MeshLoading) && (mapChunk.UpdateFlags & ChunkUpdateFlags.RightChunkBorder) == ChunkUpdateFlags.None)
            return true;
        }
        if ((this.UpdateFlags & ChunkUpdateFlags.ForwardChunkBorder) == ChunkUpdateFlags.ForwardChunkBorder)
        {
          MapChunk mapChunk = this.ForwardNeighbour();
          if (mapChunk != null && this.IsChunkFlagSet(ChunkFlags.MeshDirty | ChunkFlags.MeshLoading) && (mapChunk.UpdateFlags & ChunkUpdateFlags.BackChunkBorder) == ChunkUpdateFlags.None)
            return true;
        }
        if ((this.UpdateFlags & ChunkUpdateFlags.RightChunkBorder) == ChunkUpdateFlags.RightChunkBorder)
        {
          MapChunk mapChunk = this.RightNeighbour();
          if (mapChunk != null && this.IsChunkFlagSet(ChunkFlags.MeshDirty | ChunkFlags.MeshLoading) && (mapChunk.UpdateFlags & ChunkUpdateFlags.LeftChunkBorder) == ChunkUpdateFlags.None)
            return true;
        }
        if ((this.UpdateFlags & ChunkUpdateFlags.BackChunkBorder) == ChunkUpdateFlags.BackChunkBorder)
        {
          MapChunk mapChunk = this.BackwardNeighbour();
          if (mapChunk != null && this.IsChunkFlagSet(ChunkFlags.MeshDirty | ChunkFlags.MeshLoading) && (mapChunk.UpdateFlags & ChunkUpdateFlags.ForwardChunkBorder) == ChunkUpdateFlags.None)
            return true;
        }
        if ((this.UpdateFlags & ChunkUpdateFlags.UpChunkBorder) == ChunkUpdateFlags.UpChunkBorder)
        {
          MapChunk mapChunk = this.UpNeighbour();
          if (mapChunk != null && this.IsChunkFlagSet(ChunkFlags.MeshDirty | ChunkFlags.MeshLoading) && (mapChunk.UpdateFlags & ChunkUpdateFlags.DownChunkBorder) == ChunkUpdateFlags.None)
            return true;
        }
        if ((this.UpdateFlags & ChunkUpdateFlags.DownChunkBorder) == ChunkUpdateFlags.DownChunkBorder)
        {
          MapChunk mapChunk = this.DownNeighbour();
          if (mapChunk != null && this.IsChunkFlagSet(ChunkFlags.MeshDirty | ChunkFlags.MeshLoading) && (mapChunk.UpdateFlags & ChunkUpdateFlags.UpChunkBorder) == ChunkUpdateFlags.None)
            return true;
        }
        return false;
      }
    }

    public bool IsCommitted
    {
      get
      {
        return this.IsChunkFlagSet(ChunkFlags.Committed);
      }
    }

    public virtual void Commit()
    {
      this.ClearChunkFlag(ChunkFlags.Committed);
    }

    public void Fill(MapBlock data, bool isUserEdited)
    {
      this.BlockData.Fill(this, data.BlockID);
      this.LightData.Fill(this, data.Light.ToByte());
      this.AuxData.Fill(this, data.AuxData);
      if (!isUserEdited)
        return;
      this.SetChunkFlag(ChunkFlags.UserEdited);
    }

    public void Fill(GlobalPoint3D from, GlobalPoint3D to, MapBlock data, bool isUserEdited)
    {
      GlobalPoint3D globalPoint = this.GetGlobalPoint(Point3D.Zero);
      if (from.X > globalPoint.X + this.Map.ChunkSize.X || to.X < globalPoint.X || (from.Y > globalPoint.Y + this.Map.ChunkSize.Y || to.Y < globalPoint.Y) || (from.Z > globalPoint.Z + this.Map.ChunkSize.Z || to.Z < globalPoint.Z))
        return;
      int mapIndex1 = this.GetMapIndex(this.ClampPoint(this.GetLocalPoint(from)));
      int mapIndex2 = this.GetMapIndex(this.ClampPoint(this.GetLocalPoint(to)));
      this.BlockData.Fill(this, mapIndex1, mapIndex2, data.BlockID);
      this.LightData.Fill(this, mapIndex1, mapIndex2, data.Light.ToByte());
      this.AuxData.Fill(this, mapIndex1, mapIndex2, data.AuxData);
      if (!isUserEdited)
        return;
      this.SetChunkFlag(ChunkFlags.UserEdited);
    }

    private Point3D ClampPoint(Point3D p)
    {
      return Point3D.Clamp(Point3D.Zero, this.Region.Map.ChunkSize, p);
    }

    private byte[] ConvertData(bool blockData, bool lightData)
    {
      int num1 = this.Map.ChunkSize.X * this.Map.ChunkSize.Y * this.Map.ChunkSize.Z;
      int x1 = this.Map.ChunkSize.X;
      int x2 = this.Map.ChunkSize.X;
      int z = this.Map.ChunkSize.Z;
      GlobalPoint3D zero1 = GlobalPoint3D.Zero;
      GlobalPoint3D zero2 = GlobalPoint3D.Zero;
      byte num2 = 0;
      byte num3 = 0;
      byte maxValue = byte.MaxValue;
      int num4 = 0;
      int num5 = 0;
      byte[] numArray1 = new byte[num1 * 2];
      for (; num4 < num1; ++num4)
      {
        zero2.X = zero1.X + this.Offset.X;
        zero2.Y = -(zero1.Y + this.Offset.Y);
        zero2.Z = zero1.Z + this.Offset.Z;
        num3 = !blockData ? (!lightData ? this.Map.GetAuxData(zero2) : this.Map.GetLight(zero2).SunLight) : this.Map.GetBlockID(zero2);
        if (maxValue == byte.MaxValue || (int) num3 != (int) num2)
        {
          if (num4 > 0)
          {
            byte[] numArray2 = numArray1;
            int index1 = num5;
            int num6 = index1 + 1;
            int num7 = (int) num2;
            numArray2[index1] = (byte) num7;
            byte[] numArray3 = numArray1;
            int index2 = num6;
            num5 = index2 + 1;
            int num8 = (int) maxValue;
            numArray3[index2] = (byte) num8;
            num2 = num3;
            maxValue = byte.MaxValue;
          }
          else
            num2 = num3;
        }
        if (++zero1.X == this.Map.ChunkSize.X)
        {
          zero1.X = 0;
          if (++zero1.Z == this.Map.ChunkSize.Z)
          {
            zero1.Z = 0;
            ++zero1.Y;
          }
        }
        ++maxValue;
      }
      byte[] numArray4 = numArray1;
      int index3 = num5;
      int num9 = index3 + 1;
      int num10 = (int) num3;
      numArray4[index3] = (byte) num10;
      byte[] numArray5 = numArray1;
      int index4 = num9;
      int length = index4 + 1;
      int num11 = (int) maxValue;
      numArray5[index4] = (byte) num11;
      byte[] numArray6 = new byte[length];
      Array.Copy((Array) numArray1, (Array) numArray6, length);
      return numArray6;
    }

    protected bool IsSurroundedBy(byte blockID)
    {
      MapChunk mapChunk1 = this.LeftNeighbour();
      if (mapChunk1 != null && !mapChunk1.IsFillOf(blockID))
        return false;
      MapChunk mapChunk2 = this.ForwardNeighbour();
      if (mapChunk2 != null && !mapChunk2.IsFillOf(blockID))
        return false;
      MapChunk mapChunk3 = this.RightNeighbour();
      if (mapChunk3 != null && !mapChunk3.IsFillOf(blockID))
        return false;
      MapChunk mapChunk4 = this.BackwardNeighbour();
      if (mapChunk4 != null && !mapChunk4.IsFillOf(blockID))
        return false;
      MapChunk mapChunk5 = this.UpNeighbour();
      if (mapChunk5 != null && !mapChunk5.IsFillOf(blockID))
        return false;
      MapChunk mapChunk6 = this.DownNeighbour();
      return mapChunk6 == null || mapChunk6.IsFillOf(blockID);
    }

    public void ActionNeighbours(ChunkTest test, Action<MapChunk> action)
    {
      lock (MapChunk.neighbours)
      {
        MapChunk.neighbours.Clear();
        this.GetNeighbours(MapChunk.neighbours, test);
        foreach (MapChunk neighbour in MapChunk.neighbours)
          action(neighbour);
      }
    }

    public bool AllNeighboursGenerated
    {
      get
      {
        if (MapChunk.isChunkGeneratedTest == null)
          MapChunk.isChunkGeneratedTest = new ChunkTest(this.IsChunkGenerated);
        return this.AllNeighboursMeetCondition(MapChunk.isChunkGeneratedTest);
      }
    }

    public bool AllNeighboursDecorated
    {
      get
      {
        if (MapChunk.isChunkDecoratedTest == null)
          MapChunk.isChunkDecoratedTest = new ChunkTest(this.IsChunkDecorated);
        return this.AllNeighboursMeetCondition(MapChunk.isChunkDecoratedTest);
      }
    }

    public bool AllNeighboursDecoratedWithoutReceiveCheck
    {
      get
      {
        if (MapChunk.isChunkDecoratedWithoutReceiveCheckTest == null)
          MapChunk.isChunkDecoratedWithoutReceiveCheckTest = new ChunkTest(this.IsChunkDecoratedWithoutReceiveCheck);
        return this.AllNeighboursMeetCondition(MapChunk.isChunkDecoratedWithoutReceiveCheckTest);
      }
    }

    public bool NeighbourLightingInProgress
    {
      get
      {
        if (MapChunk.isChunkLightingTest == null)
          MapChunk.isChunkLightingTest = new ChunkTest(this.IsChunkLighting);
        return this.AnyFaceNeighbourMeetsCondition(MapChunk.isChunkLightingTest);
      }
    }

    public bool AllNeighboursDecoratedAndLit
    {
      get
      {
        if (MapChunk.isChunkDecoratedAndLitTest == null)
          MapChunk.isChunkDecoratedAndLitTest = new ChunkTest(this.IsChunkDecoratedAndLit);
        return this.AllNeighboursMeetCondition(MapChunk.isChunkDecoratedAndLitTest);
      }
    }

    public bool HasNeighboursToLightFirst
    {
      get
      {
        if (MapChunk.isChunkShouldLightFirst == null)
          MapChunk.isChunkShouldLightFirst = new ChunkTest(this.IsChunkShouldLightFirst);
        return this.AnyFaceNeighbourMeetsCondition(MapChunk.isChunkShouldLightFirst);
      }
    }

    public bool AllNeighboursLit
    {
      get
      {
        if (MapChunk.isChunkLitTest == null)
          MapChunk.isChunkLitTest = new ChunkTest(this.IsChunkLit);
        return this.AllNeighboursMeetCondition(MapChunk.isChunkLitTest);
      }
    }

    public bool AllFaceNeighboursLit
    {
      get
      {
        if (MapChunk.isChunkLitTest == null)
          MapChunk.isChunkLitTest = new ChunkTest(this.IsChunkLit);
        return this.AllFaceNeighboursMeetCondition(MapChunk.isChunkLitTest);
      }
    }

    public bool AllChunkTest(MapChunk chunk)
    {
      return true;
    }

    public bool IsChunkLit(MapChunk chunk)
    {
      if (chunk != null)
        return !chunk.IsChunkFlagSet(ChunkFlags.LightDirty | ChunkFlags.Lighting);
      return true;
    }

    public bool IsChunkGenerated(MapChunk chunk)
    {
      if (chunk != null)
        return chunk.IsChunkFlagSet(ChunkFlags.Generated);
      return true;
    }

    public bool IsChunkDecorated(MapChunk chunk)
    {
      if (chunk != null)
        return chunk.IsChunkFlagsSet(ChunkFlags.Generated | ChunkFlags.Decorated | ChunkFlags.ReceivedFromHost);
      return true;
    }

    public bool IsChunkDecoratedWithoutReceiveCheck(MapChunk chunk)
    {
      if (chunk != null)
        return chunk.IsChunkFlagsSet(ChunkFlags.Generated | ChunkFlags.Decorated);
      return true;
    }

    public bool IsChunkDecoratedAndLit(MapChunk chunk)
    {
      if (chunk == null)
        return true;
      if (this.IsChunkDecorated(chunk))
        return this.IsChunkLit(chunk);
      return false;
    }

    public bool IsChunkShouldLightFirst(MapChunk chunk)
    {
      if (chunk != null && !chunk.IsLighting)
        return chunk.IsLightDirty;
      return false;
    }

    public bool IsChunkLighting(MapChunk chunk)
    {
      if (chunk != null)
        return chunk.IsChunkFlagSet(ChunkFlags.Lighting);
      return false;
    }

    public bool AllFaceNeighboursMeetCondition(ChunkTest condition)
    {
      MapChunk chunk1 = this.LeftNeighbour();
      if (!condition(chunk1))
        return false;
      MapChunk chunk2 = this.ForwardNeighbour();
      if (!condition(chunk2))
        return false;
      MapChunk chunk3 = this.RightNeighbour();
      if (!condition(chunk3))
        return false;
      MapChunk chunk4 = this.BackwardNeighbour();
      if (!condition(chunk4))
        return false;
      MapChunk chunk5 = this.UpNeighbour();
      if (!condition(chunk5))
        return false;
      MapChunk chunk6 = this.DownNeighbour();
      return condition(chunk6);
    }

    public bool AnyFaceNeighbourMeetsCondition(ChunkTest condition)
    {
      MapChunk chunk1 = this.LeftNeighbour();
      if (condition(chunk1))
        return true;
      MapChunk chunk2 = this.ForwardNeighbour();
      if (condition(chunk2))
        return true;
      MapChunk chunk3 = this.RightNeighbour();
      if (condition(chunk3))
        return true;
      MapChunk chunk4 = this.BackwardNeighbour();
      if (condition(chunk4))
        return true;
      MapChunk chunk5 = this.UpNeighbour();
      if (condition(chunk5))
        return true;
      MapChunk chunk6 = this.DownNeighbour();
      return condition(chunk6);
    }

    public bool AllNeighboursMeetCondition(ChunkTest condition)
    {
      MapChunk chunk1 = this.LeftNeighbour();
      if (!condition(chunk1))
        return false;
      MapChunk chunk2 = this.LeftForwardNeighbour();
      if (!condition(chunk2))
        return false;
      MapChunk chunk3 = this.ForwardNeighbour();
      if (!condition(chunk3))
        return false;
      MapChunk chunk4 = this.RightForwardNeighbour();
      if (!condition(chunk4))
        return false;
      MapChunk chunk5 = this.RightNeighbour();
      if (!condition(chunk5))
        return false;
      MapChunk chunk6 = this.RightBackwardNeighbour();
      if (!condition(chunk6))
        return false;
      MapChunk chunk7 = this.BackwardNeighbour();
      if (!condition(chunk7))
        return false;
      MapChunk chunk8 = this.LeftBackwardNeighbour();
      if (!condition(chunk8))
        return false;
      MapChunk chunk9 = this.UpNeighbour();
      if (!condition(chunk9))
        return false;
      MapChunk chunk10 = this.LeftUpNeighbour();
      if (!condition(chunk10))
        return false;
      MapChunk chunk11 = this.LeftForwardUpNeighbour();
      if (!condition(chunk11))
        return false;
      MapChunk chunk12 = this.ForwardUpNeighbour();
      if (!condition(chunk12))
        return false;
      MapChunk chunk13 = this.RightForwardUpNeighbour();
      if (!condition(chunk13))
        return false;
      MapChunk chunk14 = this.RightUpNeighbour();
      if (!condition(chunk14))
        return false;
      MapChunk chunk15 = this.RightBackwardUpNeighbour();
      if (!condition(chunk15))
        return false;
      MapChunk chunk16 = this.BackwardUpNeighbour();
      if (!condition(chunk16))
        return false;
      MapChunk chunk17 = this.LeftBackwardUpNeighbour();
      if (!condition(chunk17))
        return false;
      MapChunk chunk18 = this.DownNeighbour();
      if (!condition(chunk18))
        return false;
      MapChunk chunk19 = this.LeftDownNeighbour();
      if (!condition(chunk19))
        return false;
      MapChunk chunk20 = this.LeftForwardDownNeighbour();
      if (!condition(chunk20))
        return false;
      MapChunk chunk21 = this.ForwardDownNeighbour();
      if (!condition(chunk21))
        return false;
      MapChunk chunk22 = this.RightForwardDownNeighbour();
      if (!condition(chunk22))
        return false;
      MapChunk chunk23 = this.RightDownNeighbour();
      if (!condition(chunk23))
        return false;
      MapChunk chunk24 = this.RightBackwardDownNeighbour();
      if (!condition(chunk24))
        return false;
      MapChunk chunk25 = this.BackwardDownNeighbour();
      if (!condition(chunk25))
        return false;
      MapChunk chunk26 = this.LeftBackwardDownNeighbour();
      return condition(chunk26);
    }

    public void GetNeighbours(List<MapChunk> list, ChunkTest test)
    {
      MapChunk chunk1 = this.LeftNeighbour();
      if (chunk1 != null && (test == null || test(chunk1)))
        list.Add(chunk1);
      MapChunk chunk2 = this.LeftForwardNeighbour();
      if (chunk2 != null && (test == null || test(chunk2)))
        list.Add(chunk2);
      MapChunk chunk3 = this.ForwardNeighbour();
      if (chunk3 != null && (test == null || test(chunk3)))
        list.Add(chunk3);
      MapChunk chunk4 = this.RightForwardNeighbour();
      if (chunk4 != null && (test == null || test(chunk4)))
        list.Add(chunk4);
      MapChunk chunk5 = this.RightNeighbour();
      if (chunk5 != null && (test == null || test(chunk5)))
        list.Add(chunk5);
      MapChunk chunk6 = this.RightBackwardNeighbour();
      if (chunk6 != null && (test == null || test(chunk6)))
        list.Add(chunk6);
      MapChunk chunk7 = this.BackwardNeighbour();
      if (chunk7 != null && (test == null || test(chunk7)))
        list.Add(chunk7);
      MapChunk chunk8 = this.LeftBackwardNeighbour();
      if (chunk8 != null && (test == null || test(chunk8)))
        list.Add(chunk8);
      MapChunk chunk9 = this.UpNeighbour();
      if (chunk9 != null && (test == null || test(chunk9)))
        list.Add(chunk9);
      MapChunk chunk10 = this.LeftUpNeighbour();
      if (chunk10 != null && (test == null || test(chunk10)))
        list.Add(chunk10);
      MapChunk chunk11 = this.LeftForwardUpNeighbour();
      if (chunk11 != null && (test == null || test(chunk11)))
        list.Add(chunk11);
      MapChunk chunk12 = this.ForwardUpNeighbour();
      if (chunk12 != null && (test == null || test(chunk12)))
        list.Add(chunk12);
      MapChunk chunk13 = this.RightForwardUpNeighbour();
      if (chunk13 != null && (test == null || test(chunk13)))
        list.Add(chunk13);
      MapChunk chunk14 = this.RightUpNeighbour();
      if (chunk14 != null && (test == null || test(chunk14)))
        list.Add(chunk14);
      MapChunk chunk15 = this.RightBackwardUpNeighbour();
      if (chunk15 != null && (test == null || test(chunk15)))
        list.Add(chunk15);
      MapChunk chunk16 = this.BackwardUpNeighbour();
      if (chunk16 != null && (test == null || test(chunk16)))
        list.Add(chunk16);
      MapChunk chunk17 = this.LeftBackwardUpNeighbour();
      if (chunk17 != null && (test == null || test(chunk17)))
        list.Add(chunk17);
      MapChunk chunk18 = this.DownNeighbour();
      if (chunk18 != null && (test == null || test(chunk18)))
        list.Add(chunk18);
      MapChunk chunk19 = this.LeftDownNeighbour();
      if (chunk19 != null && (test == null || test(chunk19)))
        list.Add(chunk19);
      MapChunk chunk20 = this.LeftForwardDownNeighbour();
      if (chunk20 != null && (test == null || test(chunk20)))
        list.Add(chunk20);
      MapChunk chunk21 = this.ForwardDownNeighbour();
      if (chunk21 != null && (test == null || test(chunk21)))
        list.Add(chunk21);
      MapChunk chunk22 = this.RightForwardDownNeighbour();
      if (chunk22 != null && (test == null || test(chunk22)))
        list.Add(chunk22);
      MapChunk chunk23 = this.RightDownNeighbour();
      if (chunk23 != null && (test == null || test(chunk23)))
        list.Add(chunk23);
      MapChunk chunk24 = this.RightBackwardDownNeighbour();
      if (chunk24 != null && (test == null || test(chunk24)))
        list.Add(chunk24);
      MapChunk chunk25 = this.BackwardDownNeighbour();
      if (chunk25 != null && (test == null || test(chunk25)))
        list.Add(chunk25);
      MapChunk chunk26 = this.LeftBackwardDownNeighbour();
      if (chunk26 == null || test != null && !test(chunk26))
        return;
      list.Add(chunk26);
    }

    public MapChunk LeftNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X >= chunkSize.X)
      {
        Point3D offset = this.Offset;
        offset.X -= chunkSize.X;
        return this.Region.GetChunk(offset);
      }
      MapRegion mapRegion = this.Region.LeftNeighbour();
      if (mapRegion == null)
        return (MapChunk) null;
      Point3D offset1 = this.Offset;
      offset1.X = mapRegion.Map.RegionSize.X - chunkSize.X;
      return mapRegion.GetChunk(offset1);
    }

    public MapChunk LeftForwardNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X < chunkSize.X || this.Offset.Z < chunkSize.Z)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, 0, -chunkSize.Z));
      Point3D offset = this.Offset;
      offset.X -= chunkSize.X;
      offset.Z -= chunkSize.Z;
      return this.Region.GetChunk(offset);
    }

    public MapChunk LeftForwardUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, chunkSize.Y, -chunkSize.Z));
    }

    public MapChunk LeftForwardDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, -chunkSize.Y, -chunkSize.Z));
    }

    public MapChunk LeftBackwardNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X < chunkSize.X || this.Offset.Z >= this.Region.Map.RegionSize.Z - chunkSize.Z)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, 0, chunkSize.Z));
      Point3D offset = this.Offset;
      offset.X -= chunkSize.X;
      offset.Z += chunkSize.Z;
      return this.Region.GetChunk(offset);
    }

    public MapChunk LeftBackwardUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, chunkSize.Y, chunkSize.Z));
    }

    public MapChunk LeftBackwardDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, -chunkSize.Y, chunkSize.Z));
    }

    public MapChunk LeftUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X < chunkSize.X || this.Offset.Y >= this.Region.Map.RegionSize.Y - chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, chunkSize.Y, 0));
      Point3D offset = this.Offset;
      offset.X -= chunkSize.X;
      offset.Y += chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk LeftDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X < chunkSize.X || this.Offset.Y < chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(-chunkSize.X, -chunkSize.Y, 0));
      Point3D offset = this.Offset;
      offset.X -= chunkSize.X;
      offset.Y -= chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk ForwardNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Z >= chunkSize.Z)
      {
        Point3D offset = this.Offset;
        offset.Z -= chunkSize.Z;
        return this.Region.GetChunk(offset);
      }
      MapRegion mapRegion = this.Region.ForwardNeighbour();
      if (mapRegion == null)
        return (MapChunk) null;
      Point3D offset1 = this.Offset;
      offset1.Z = mapRegion.Map.RegionSize.Z - chunkSize.Z;
      return mapRegion.GetChunk(offset1);
    }

    public MapChunk ForwardUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Z < chunkSize.Z || this.Offset.Y >= this.Region.Map.RegionSize.Y - chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(0, chunkSize.Y, -chunkSize.Z));
      Point3D offset = this.Offset;
      offset.Z -= chunkSize.Z;
      offset.Y += chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk ForwardDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Z < chunkSize.X || this.Offset.Y < chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(0, -chunkSize.Y, -chunkSize.Z));
      Point3D offset = this.Offset;
      offset.Z -= chunkSize.Z;
      offset.Y -= chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk RightNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X < this.Region.Map.RegionSize.X - chunkSize.X)
      {
        Point3D offset = this.Offset;
        offset.X += chunkSize.X;
        return this.Region.GetChunk(offset);
      }
      MapRegion mapRegion = this.Region.RightNeighbour();
      if (mapRegion == null)
        return (MapChunk) null;
      Point3D offset1 = this.Offset;
      offset1.X = 0;
      return mapRegion.GetChunk(offset1);
    }

    public MapChunk RightForwardNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X >= this.Region.Map.RegionSize.X - chunkSize.X || this.Offset.Z < chunkSize.Z)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, 0, -chunkSize.Z));
      Point3D offset = this.Offset;
      offset.X += chunkSize.X;
      offset.Z -= chunkSize.Z;
      return this.Region.GetChunk(offset);
    }

    public MapChunk RightForwardUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, chunkSize.Y, -chunkSize.Z));
    }

    public MapChunk RightForwardDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, -chunkSize.Y, -chunkSize.Z));
    }

    public MapChunk RightBackwardNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X >= this.Region.Map.RegionSize.X - chunkSize.X || this.Offset.Z >= this.Region.Map.RegionSize.Z - chunkSize.Z)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, 0, chunkSize.Z));
      Point3D offset = this.Offset;
      offset.X += chunkSize.X;
      offset.Z += chunkSize.Z;
      return this.Region.GetChunk(offset);
    }

    public MapChunk RightBackwardUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, chunkSize.Y, chunkSize.Z));
    }

    public MapChunk RightBackwardDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, -chunkSize.Y, chunkSize.Z));
    }

    public MapChunk RightUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X >= this.Region.Map.RegionSize.X - chunkSize.X || this.Offset.Y >= this.Region.Map.RegionSize.Y - chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, chunkSize.Y, 0));
      Point3D offset = this.Offset;
      offset.X += chunkSize.X;
      offset.Y += chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk RightDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.X >= this.Region.Map.RegionSize.X - chunkSize.X || this.Offset.Y < chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(chunkSize.X, -chunkSize.Y, 0));
      Point3D offset = this.Offset;
      offset.X += chunkSize.X;
      offset.Y -= chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk BackwardNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Z < this.Region.Map.RegionSize.Z - chunkSize.Z)
      {
        Point3D offset = this.Offset;
        offset.Z += chunkSize.Z;
        return this.Region.GetChunk(offset);
      }
      MapRegion mapRegion = this.Region.BackwardNeighbour();
      if (mapRegion == null)
        return (MapChunk) null;
      Point3D offset1 = this.Offset;
      offset1.Z = 0;
      return mapRegion.GetChunk(offset1);
    }

    public MapChunk BackwardUpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Z >= this.Region.Map.RegionSize.Z - chunkSize.Z || this.Offset.Y >= this.Region.Map.RegionSize.Y - chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(0, chunkSize.Y, chunkSize.Z));
      Point3D offset = this.Offset;
      offset.Z += chunkSize.Z;
      offset.Y += chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk BackwardDownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Z >= this.Region.Map.RegionSize.Z - chunkSize.Z || this.Offset.Y < chunkSize.Y)
        return this.Region.Map.GetChunk(this.GlobalOffset + new GlobalPoint3D(0, -chunkSize.Y, chunkSize.Z));
      Point3D offset = this.Offset;
      offset.Z += chunkSize.Z;
      offset.Y -= chunkSize.Y;
      return this.Region.GetChunk(offset);
    }

    public MapChunk UpNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Y < this.Region.Map.RegionSize.Y - chunkSize.Y)
      {
        Point3D offset = this.Offset;
        offset.Y += chunkSize.Y;
        return this.Region.GetChunk(offset);
      }
      MapRegion mapRegion = this.Region.UpNeighbour();
      if (mapRegion == null)
        return (MapChunk) null;
      Point3D offset1 = this.Offset;
      offset1.Y = 0;
      return mapRegion.GetChunk(offset1);
    }

    public MapChunk DownNeighbour()
    {
      Point3D chunkSize = this.Region.Map.ChunkSize;
      if (this.Offset.Y >= chunkSize.Y)
      {
        Point3D offset = this.Offset;
        offset.Y -= chunkSize.Y;
        return this.Region.GetChunk(offset);
      }
      MapRegion mapRegion = this.Region.DownNeighbour();
      if (mapRegion == null)
        return (MapChunk) null;
      Point3D offset1 = this.Offset;
      offset1.Y = this.Region.Map.RegionSize.Y - chunkSize.Y;
      return mapRegion.GetChunk(offset1);
    }

    public bool IsNextTo(Map map, Point3D p, byte blockID)
    {
      return this.IsNextTo(map, p, blockID, -1, false, false);
    }

    public bool IsNextTo(Map map, Point3D p, byte blockID, int auxMatch)
    {
      return this.IsNextTo(map, p, blockID, auxMatch, false, false);
    }

    public bool IsNextTo(
      Map map,
      Point3D p,
      byte blockID,
      int auxMatch,
      bool ignoreSelf,
      bool ignoreBelow)
    {
      if (!ignoreSelf && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAUXData(p) == auxMatch))
        return true;
      --p.X;
      if (p.X >= 0 && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAUXData(p) == auxMatch))
        return true;
      ++p.X;
      ++p.Y;
      if (p.Y < map.ChunkSize.Y && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAUXData(p) == auxMatch))
        return true;
      --p.Y;
      --p.Z;
      if (p.Z >= 0 && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAUXData(p) == auxMatch))
        return true;
      ++p.Z;
      ++p.X;
      if (p.X < map.ChunkSize.X && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAUXData(p) == auxMatch))
        return true;
      --p.X;
      ++p.Z;
      if (p.Z < map.ChunkSize.Z && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAUXData(p) == auxMatch))
        return true;
      --p.Z;
      if (!ignoreBelow)
      {
        --p.Y;
        if (p.Y >= 0 && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAUXData(p) == auxMatch))
          return true;
        ++p.Y;
      }
      return false;
    }

    public IEnumerator GetEnumerator()
    {
      RLEEnumerator result = new RLEEnumerator(this);
      yield return (object) result.DataX;
      while (result.MoveNext())
        yield return (object) result.DataX;
    }

    public void ReadData(BinaryReader reader, int version, bool readDirectToRLE)
    {
      if (version > 65)
        this.flags = (ChunkFlags) reader.ReadInt32() & this.chunkFlagsToKeepOnRead;
      if (version > 79)
      {
        long num = (long) reader.ReadUInt64();
      }
      if (readDirectToRLE)
      {
        this.ReadDirectToRLE(reader, version);
        this.SetChunkFlag(ChunkFlags.Generated | ChunkFlags.Decorated | ChunkFlags.ReceivedFromHost);
      }
      else
        this.ReadDataCore(reader, version);
    }

    protected virtual void ReadDataCore(BinaryReader reader, int version)
    {
      this.ReadDirectToRLE(reader, version);
    }

    protected void ReadDirectToRLE(BinaryReader reader, int version)
    {
      this.BlockData.ReadData(this, reader, version);
      this.LightData.ReadData(this, reader, version);
      this.AuxData.ReadData(this, reader, version);
    }

    public void WriteData(BinaryWriter writer)
    {
      writer.Write((int) this.flags);
      writer.Write((ulong) this.UpdateFlags);
      this.WriteChunkData(writer);
    }

    protected virtual void WriteChunkData(BinaryWriter writer)
    {
      this.BlockData.WriteData(this, writer);
      this.LightData.WriteData(this, writer);
      this.AuxData.WriteData(this, writer);
    }
  }
}
