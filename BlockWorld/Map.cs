// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.Map
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public abstract class Map
  {
    public static List<Map> LiveMaps = new List<Map>(50);
    public List<GlobalPoint3D> PointsOfPenetration = new List<GlobalPoint3D>(9);
    public float WaterFlowSpeedX = 0.05f;
    public float WaterFlowSpeedY = 0.01f;
    public float LavaFlowSpeedX = 0.01f;
    public float LavaFlowSpeedY = 1f / 500f;
    private Dictionary<long, MapChunk> commitList = new Dictionary<long, MapChunk>(100);
    private object shiftLock = new object();
    private List<MapRegion> releasedRegions = new List<MapRegion>();
    private List<MapHeightmap> releasedHeightmaps = new List<MapHeightmap>();
    private List<MapRegion> discardedRegionsOnShift = new List<MapRegion>();
    private List<MapHeightmap> discardedHeightMapsOnShift = new List<MapHeightmap>();
    private Dictionary<long, MapChunk> tempHashList = new Dictionary<long, MapChunk>();
    public const byte MaxWaterLevel = 8;
    private const float ShiftBoxWidth = 352f;
    public static int UncompressedCount;
    public static int CompressedCount;
    public static RLEStreamBufferManager RLEStreamBufferManager;
    public Dictionary<int, MapRegion> Regions;
    public Dictionary<int, MapHeightmap> HeightMaps;
    public string Name;
    public Point3D ChunkSize;
    public Point3D RegionSize;
    public BoxInt MapBound;
    public Point3D MapCenter;
    public Point3D MapIndexCenter;
    public Point3D MapMaxSize;
    public PcgRandom Random;
    public Vector3 Position;
    public Matrix World;
    public readonly float MaxLight;
    public float TileSize;
    public float HalfTileSize;
    public int Seed;
    public ushort SeaLevel;
    public bool SaveDataEnabled;
    public byte OutOfBoundsBlockID;
    public byte WaterBlockID;
    public byte LavaBlockID;
    public byte RopeBlockID;
    public byte BedrockID;
    public byte InvisibleBarrierID;
    public bool AllowEdgeClear;
    public float LightCycle;
    public MapStrategy MapStrategy;
    public readonly bool IsHost;
    public readonly MapLight SunLight;
    public int TimeStampRleCache;
    public bool IsCommitAllowed;
    public int TotalChunks;
    public ChunkCacheManager ChunkCacheManager;
    public bool IsLightingInProgress;
    public bool IsInfinite;
    private int regionsPerX;
    private int regionsPerPlane;
    private bool commit;
    private Dictionary<int, MapRegion> swapRegions;
    private Dictionary<int, MapHeightmap> swapHeightMaps;

    public GlobalPoint3D MapSize
    {
      get
      {
        return this.MapBound.Max - this.MapBound.Min;
      }
    }

    public BoundingBox Box
    {
      get
      {
        return new BoundingBox(this.MapBound.Min * this.TileSize + this.Position, this.MapBound.Max * this.TileSize + this.Position);
      }
    }

    public int MapHeight
    {
      get
      {
        return this.MapBound.Max.Y - this.MapBound.Min.Y;
      }
    }

    public int ChunkLength
    {
      get
      {
        return this.ChunkSize.X * this.ChunkSize.Y * this.ChunkSize.Z;
      }
    }

    public virtual int MemorySize
    {
      get
      {
        int num = 0;
        if (this.ChunkCacheManager != null)
          num += this.ChunkCacheManager.MemorySize;
        if (this.Regions != null)
        {
          foreach (KeyValuePair<int, MapRegion> region in this.Regions)
            num += region.Value.MemorySize;
          foreach (KeyValuePair<int, MapHeightmap> heightMap in this.HeightMaps)
            num += heightMap.Value.MemorySize;
          lock (this.shiftLock)
          {
            foreach (MapRegion releasedRegion in this.releasedRegions)
              num += releasedRegion.MemorySize;
            foreach (MapHeightmap releasedHeightmap in this.releasedHeightmaps)
              num += releasedHeightmap.MemorySize;
          }
        }
        return num;
      }
    }

    public abstract byte GetOpacity(byte blockID);

    public abstract byte GetLuminance(ref GlobalPoint3D p);

    public abstract byte GetLuminance(ref GlobalPoint3D p, byte blockID);

    public abstract ushort GetBlastResistance(byte blockID);

    public abstract byte GetBlockBufferType(byte blockID);

    public abstract bool IsBlockRotated(byte blockID);

    public abstract bool IsBlockSolid(byte blockID);

    public abstract bool IsBlockPassable(byte blockID);

    public abstract bool IsBlockIcon(byte blockID);

    public abstract bool IsBlockAttachable(byte blockID);

    public abstract bool IsBlockLiquid(byte blockID);

    public abstract bool IsBlockOre(byte blockID);

    public virtual bool IsBlockLightSource(byte blockID)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      return this.GetLuminance(ref p, blockID) > (byte) 0;
    }

    public BoundingBox LeftBoundaryBox
    {
      get
      {
        GlobalPoint3D min = this.MapBound.Min;
        GlobalPoint3D max = this.MapBound.Max;
        return new BoundingBox(new Vector3((float) min.X * this.TileSize, 0.0f, (float) min.Z * this.TileSize), new Vector3(((float) min.X + 352f) * this.TileSize, (float) max.Y * this.TileSize, (float) max.Z * this.TileSize));
      }
    }

    public BoundingBox ForwardBoundaryBox
    {
      get
      {
        GlobalPoint3D min = this.MapBound.Min;
        GlobalPoint3D max = this.MapBound.Max;
        return new BoundingBox(new Vector3((float) min.X * this.TileSize, 0.0f, (float) min.Z * this.TileSize), new Vector3((float) max.X * this.TileSize, (float) max.Y * this.TileSize, ((float) min.Z + 352f) * this.TileSize));
      }
    }

    public BoundingBox RightBoundaryBox
    {
      get
      {
        GlobalPoint3D min = this.MapBound.Min;
        GlobalPoint3D max = this.MapBound.Max;
        return new BoundingBox(new Vector3(((float) max.X - 352f) * this.TileSize, 0.0f, (float) min.Z * this.TileSize), new Vector3((float) max.X * this.TileSize, (float) max.Y * this.TileSize, (float) max.Z * this.TileSize));
      }
    }

    public BoundingBox BackwardBoundaryBox
    {
      get
      {
        GlobalPoint3D min = this.MapBound.Min;
        GlobalPoint3D max = this.MapBound.Max;
        return new BoundingBox(new Vector3((float) min.X * this.TileSize, 0.0f, ((float) max.Z - 352f) * this.TileSize), new Vector3((float) max.X * this.TileSize, (float) max.Y * this.TileSize, (float) max.Z * this.TileSize));
      }
    }

    public Map(
      string name,
      float tileSize,
      bool isInfinite,
      BoxInt totalMapBound,
      BoxInt mapBound,
      Point3D regionSize,
      Point3D chunkSize,
      int maxLight,
      int seed,
      int initialCacheCount,
      int cacheExpandSize,
      MapStrategy strategy,
      bool isHost)
    {
      Map.LiveMaps.Add(this);
      Map.UncompressedCount = 0;
      Map.CompressedCount = 0;
      this.Name = name;
      this.Seed = seed;
      this.MaxLight = (float) maxLight;
      this.IsHost = isHost;
      this.Random = new PcgRandom(seed);
      this.SunLight = new MapLight()
      {
        BlockLight = (byte) 0,
        SunLight = (byte) this.MaxLight
      };
      GlobalPoint3D globalPoint3D = totalMapBound.Max - totalMapBound.Min;
      this.MapBound = mapBound;
      this.RegionSize = regionSize;
      this.ChunkSize = chunkSize;
      this.TileSize = tileSize;
      this.HalfTileSize = tileSize * 0.5f;
      this.MapMaxSize = !(this.IsInfinite = isInfinite) ? (Point3D) globalPoint3D : new Point3D(globalPoint3D.X / 2, this.MapHeight, globalPoint3D.Z / 2);
      this.InitRegionData();
      this.ChunkCacheManager = new ChunkCacheManager(this, initialCacheCount, cacheExpandSize);
      this.ChunkCacheManager.Initialize((InitState) null);
      if ((this.MapStrategy = strategy) == null)
        return;
      strategy.Initialize(this);
    }

    public void InitRegionData()
    {
      this.regionsPerX = (this.MapSize.X + this.RegionSize.X - 1) / this.RegionSize.X;
      int num = (this.MapSize.Z + this.RegionSize.Z - 1) / this.RegionSize.Z;
      this.regionsPerPlane = this.regionsPerX * num;
      this.Regions = new Dictionary<int, MapRegion>(this.regionsPerPlane * ((this.MapSize.Y + this.RegionSize.Y - 1) / this.RegionSize.Y));
      this.HeightMaps = new Dictionary<int, MapHeightmap>();
      this.MapCenter = (Point3D) (this.MapBound.Min + (this.IsInfinite ? new Point3D(this.regionsPerX * this.RegionSize.X / 2, 0, num * this.RegionSize.Z / 2) : Point3D.Zero));
      this.TotalChunks = 0;
    }

    public void ClearMap(MapBlock blockData)
    {
      this.Fill(this.MapBound.Min, this.MapBound.Max - GlobalPoint3D.One, blockData, false);
    }

    public void UnloadContent()
    {
      Map.LiveMaps.Remove(this);
      this.UnloadContentCore();
      if (this.ChunkCacheManager != null)
        this.ChunkCacheManager.ReleaseAll();
      if (this.MapStrategy != null)
        this.MapStrategy.UnloadContent();
      foreach (KeyValuePair<int, MapRegion> region in this.Regions)
        region.Value?.UnloadContent();
      foreach (MapRegion releasedRegion in this.releasedRegions)
        releasedRegion?.UnloadContent();
      foreach (KeyValuePair<int, MapHeightmap> heightMap in this.HeightMaps)
        heightMap.Value?.UnloadContent();
      foreach (MapHeightmap releasedHeightmap in this.releasedHeightmaps)
        releasedHeightmap?.UnloadContent();
      this.HeightMaps.Clear();
      this.Regions.Clear();
      this.releasedHeightmaps.Clear();
      this.releasedRegions.Clear();
      if (this.swapRegions != null)
        this.swapRegions.Clear();
      if (this.swapHeightMaps == null)
        return;
      this.swapHeightMaps.Clear();
    }

    protected virtual void UnloadContentCore()
    {
    }

    public void Commit()
    {
      lock (this.commitList)
        this.commit = true;
    }

    private void UpdateCommit()
    {
      lock (this.commitList)
      {
        this.CommitCore();
        this.commit = false;
      }
    }

    protected virtual void CommitCore()
    {
      if (this.commitList.Count <= 0)
        return;
      foreach (KeyValuePair<long, MapChunk> commit in this.commitList)
        this.CommitChunk(commit.Value);
      this.commitList.Clear();
    }

    protected virtual void CommitChunk(MapChunk chunk)
    {
      chunk?.Commit();
    }

    public void AddChunkToCommitList(MapChunk chunk, UpdateBlockMethod method)
    {
      if (!this.IsCommitAllowed || chunk == null || !this.ShouldCommit(method))
        return;
      lock (this.commitList)
      {
        long globalHashCode = chunk.GetGlobalHashCode();
        if (this.commitList.ContainsKey(globalHashCode))
          return;
        this.commitList.Add(globalHashCode, chunk);
        chunk.SetChunkFlag(ChunkFlags.Committed);
      }
    }

    private bool ShouldCommit(UpdateBlockMethod method)
    {
      return method != UpdateBlockMethod.Generation;
    }

    public void Update(UpdateState state)
    {
      this.UpdateCore(state);
      this.MapStrategy.Update(state);
      this.World = Matrix.CreateTranslation(this.Position);
      if (!this.commit)
        return;
      this.UpdateCommit();
    }

    protected virtual void UpdateCore(UpdateState state)
    {
    }

    public void LoadingUpdate()
    {
      this.UpdateCommit();
    }

    public GlobalPoint3D GetPoint(Vector3 position)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      if ((double) this.TileSize == 1.0)
      {
        zero.X = (int) position.X;
        zero.Y = (int) position.Y;
        zero.Z = (int) position.Z;
      }
      else
      {
        zero.X = (int) ((double) position.X / (double) this.TileSize);
        zero.Y = (int) ((double) position.Y / (double) this.TileSize);
        zero.Z = (int) ((double) position.Z / (double) this.TileSize);
      }
      if ((double) position.X < 0.0)
        --zero.X;
      if ((double) position.Y < 0.0)
        --zero.Y;
      if ((double) position.Z < 0.0)
        --zero.Z;
      return zero;
    }

    public GlobalPoint3D GetPoint(float x, float y, float z)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      if ((double) this.TileSize == 1.0)
      {
        zero.X = (int) x;
        zero.Y = (int) y;
        zero.Z = (int) z;
      }
      else
      {
        zero.X = (int) ((double) x / (double) this.TileSize);
        zero.Y = (int) ((double) y / (double) this.TileSize);
        zero.Z = (int) ((double) z / (double) this.TileSize);
      }
      if ((double) x < 0.0)
        --zero.X;
      if ((double) y < 0.0)
        --zero.Y;
      if ((double) z < 0.0)
        --zero.Z;
      return zero;
    }

    public GlobalPoint3D GetPointFromGlobalHash(long hash)
    {
      GlobalPoint3D regionOffset = this.GetRegionOffset((int) (hash >> 32));
      GlobalPoint3D pointFromLocalHash = this.GetPointFromLocalHash((int) hash);
      regionOffset.X += pointFromLocalHash.X;
      regionOffset.Y += pointFromLocalHash.Y;
      regionOffset.Z += pointFromLocalHash.Z;
      return regionOffset;
    }

    public GlobalPoint3D GetPointFromLocalHash(int hash)
    {
      return new GlobalPoint3D()
      {
        X = hash >> 20,
        Z = hash >> 10 & 1023,
        Y = hash & 1023
      };
    }

    public GlobalPoint3D GetGroundPoint(GlobalPoint3D p)
    {
      if (!this.IsPassable(p))
      {
        ++p.Y;
        while (p.Y < this.MapBound.Max.Y - 1 && !this.IsPassable(p))
          ++p.Y;
        --p.Y;
      }
      else
      {
        while (p.Y > this.MapBound.Min.Y && this.IsPassable(p))
          --p.Y;
      }
      return p;
    }

    public GlobalPoint3D Clamp(GlobalPoint3D p)
    {
      return GlobalPoint3D.Clamp(this.MapBound.Min, this.MapBound.Max, p);
    }

    public GlobalPoint3D Clamp(GlobalPoint3D p, int edge)
    {
      GlobalPoint3D globalPoint3D = new GlobalPoint3D(edge, edge, edge);
      return GlobalPoint3D.Clamp(this.MapBound.Min + globalPoint3D, this.MapBound.Max - globalPoint3D, p);
    }

    public BoundingBox GetBox(GlobalPoint3D p)
    {
      BoundingBox boundingBox = new BoundingBox()
      {
        Min = {
          X = (float) p.X * this.TileSize,
          Y = (float) p.Y * this.TileSize,
          Z = (float) p.Z * this.TileSize
        }
      };
      boundingBox.Max.X = boundingBox.Min.X + this.TileSize;
      boundingBox.Max.Y = boundingBox.Min.Y + this.TileSize;
      boundingBox.Max.Z = boundingBox.Min.Z + this.TileSize;
      return boundingBox;
    }

    public BoundingBox GetBox(GlobalPoint3D p1, GlobalPoint3D p2)
    {
      return new BoundingBox()
      {
        Min = {
          X = (float) p1.X * this.TileSize,
          Y = (float) p1.Y * this.TileSize,
          Z = (float) p1.Z * this.TileSize
        },
        Max = {
          X = (float) p2.X + this.TileSize,
          Y = (float) p2.Y + this.TileSize,
          Z = (float) p2.Z + this.TileSize
        }
      };
    }

    public byte GetBlockID(Vector3 position)
    {
      return this.GetBlockID(this.GetPoint(position - this.Position));
    }

    public byte GetBlockID(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return this.OutOfBoundsBlockID;
      return region.GetBlockID(p);
    }

    public byte GetBlockID_Test(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return this.OutOfBoundsBlockID;
      return region.GetBlockID_Test(p);
    }

    public byte GetBlockIDNoCache(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return this.OutOfBoundsBlockID;
      return region.GetBlockIDNoCache(p);
    }

    public MapBlock GetBlockData(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region != null)
        return region.GetBlockData(p);
      return this.BuildBlockData((MapChunk) null, (byte) 0, (byte) 0, (byte) 0, (byte) 0);
    }

    public MapBlock GetBlockIDAndAux(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region != null)
        return region.GetBlockIDAndAux(p);
      return this.BuildBlockData((MapChunk) null, (byte) 0, (byte) 0, (byte) 0, (byte) 0);
    }

    public MapBlock GetBlockIDAndAuxNoCache(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region != null)
        return region.GetBlockIDAndAuxNoCache(p);
      return this.BuildBlockData((MapChunk) null, (byte) 0, (byte) 0, (byte) 0, (byte) 0);
    }

    public MapBlock GetBlockAndLight(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region != null)
        return region.GetBlockAndLight(p);
      return this.BuildBlockData((MapChunk) null, (byte) 0, (byte) 0, (byte) 0, (byte) 0);
    }

    public bool IsBlockDataEqual(Vector3 pos, byte blockID, byte aux)
    {
      MapBlock blockIdAndAux = this.GetBlockIDAndAux(this.GetPoint(pos));
      if ((int) blockIdAndAux.BlockID == (int) blockID)
        return ((int) blockIdAndAux.AuxData & 7) == (int) aux;
      return false;
    }

    public MapChunk SetBlockData(
      GlobalPoint3D p,
      byte blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
      if ((int) blockIdAndAux.BlockID != (int) blockID || (int) blockIdAndAux.AuxData != (int) auxData)
      {
        MapBlock mapBlock = new MapBlock()
        {
          BlockID = blockID,
          AuxData = auxData
        };
        MapChunk mapChunk = this.SetBlockIDAndAuxInternal(p, blockIdAndAux, mapBlock, method);
        if (mapChunk != null)
        {
          blockIdAndAux.Chunk = mapBlock.Chunk = mapChunk;
          this.MapStrategy.BlockChanged(p, blockIdAndAux, mapBlock, method, playerID, transmit);
          return mapChunk;
        }
      }
      return (MapChunk) null;
    }

    public MapChunk SetBlockData(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      MapChunk mapChunk = this.SetBlockDataInternal(p, oldBlockData, newBlockData, method);
      if (mapChunk == null)
        return (MapChunk) null;
      oldBlockData.Chunk = newBlockData.Chunk = mapChunk;
      this.MapStrategy.BlockChanged(p, oldBlockData, newBlockData, method, playerID, transmit);
      return mapChunk;
    }

    public MapChunk SetBlockDataInternal(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method)
    {
      return this.GetRegion(p)?.SetBlockData(p, oldBlockData, newBlockData, method);
    }

    private MapChunk SetBlockIDAndAuxInternal(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method)
    {
      return this.GetRegion(p)?.SetBlockIDAndAux(p, oldBlockData, newBlockData, method);
    }

    public MapBlock BuildBlockData(
      MapChunk chunk,
      byte blockID,
      byte sunlight,
      byte blocklight,
      byte auxdata)
    {
      return new MapBlock()
      {
        Chunk = chunk,
        BlockID = blockID,
        Light = new MapLight()
        {
          SunLight = sunlight,
          BlockLight = blocklight
        },
        AuxData = auxdata
      };
    }

    public MapBlock BuildBlockData(
      MapChunk chunk,
      byte blockID,
      MapLight light,
      byte auxdata)
    {
      return new MapBlock()
      {
        Chunk = chunk,
        BlockID = blockID,
        Light = light,
        AuxData = auxdata
      };
    }

    public static MapBlock BuildBlockDataStatic(
      MapChunk chunk,
      byte blockID,
      byte sunlight,
      byte blocklight,
      byte auxdata)
    {
      return new MapBlock()
      {
        Chunk = chunk,
        BlockID = blockID,
        Light = new MapLight()
        {
          SunLight = sunlight,
          BlockLight = blocklight
        },
        AuxData = auxdata
      };
    }

    private bool ShouldDirtyLightBeFlagged(MapChunk chunk, GlobalPoint3D p, MapBlock data)
    {
      if (this.GetLuminance(ref p, data.BlockID) <= (byte) 0 && (data.BlockID == (byte) 0 || data.Light.SunLight <= (byte) 1 && data.Light.BlockLight <= (byte) 1))
        return this.CountSurroundingBlocksWithLight(p, 1) > 0;
      return true;
    }

    private int CountSurroundingBlocksWithLight(GlobalPoint3D p, int range)
    {
      int num = 0;
      GlobalPoint3D p1 = new GlobalPoint3D();
      for (p1.Y = p.Y - range; p1.Y <= p.Y + range; ++p1.Y)
      {
        for (p1.Z = p.Z - range; p1.Z <= p.Z + range; ++p1.Z)
        {
          for (p1.X = p.X - range; p1.X <= p.X + range; ++p1.X)
          {
            if (p1.X != p.X || p1.Y != p.Y || p1.Z != p.Z)
            {
              MapLight light = this.GetLight(p1);
              if (light.SunLight > (byte) 0 || light.BlockLight > (byte) 0)
                ++num;
            }
          }
        }
      }
      return num;
    }

    public ClearBlockResult ClearBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit)
    {
      return this.ClearBlock(p, method, gamerID, transmit, false, false);
    }

    public ClearBlockResult ClearBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit,
      bool isRelatedClear,
      bool bordersLiquidCheck)
    {
      ClearBlockResult clearBlockResult = ClearBlockResult.AlreadyClear;
      byte blockId = this.GetBlockID(p);
      if (blockId > (byte) 0)
      {
        if (bordersLiquidCheck && this.IsRetainingLiquid(p))
          return ClearBlockResult.PermissionDenied;
        clearBlockResult = this.GetClearBlockResult(p, blockId, method, gamerID, isRelatedClear);
        if (clearBlockResult == ClearBlockResult.Success)
        {
          if (!isRelatedClear && !this.IsBlockAttachable(blockId))
            this.CheckForRelatedClears(p, method == UpdateBlockMethod.Player ? UpdateBlockMethod.PlayerRelated : method, method == UpdateBlockMethod.Player ? gamerID : GamerID.Sys1);
          this.SetBlockData(p, (byte) 0, (byte) 0, method, gamerID, transmit);
        }
      }
      return clearBlockResult;
    }

    public ClearBlockResult GetClearBlockResult(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      if (this.IsValidPoint(p))
        return this.GetClearBlockResult(p, this.GetBlockID(p), method, playerID, false);
      return ClearBlockResult.OutOfBounds;
    }

    public ClearBlockResult GetClearBlockResult(
      GlobalPoint3D p,
      byte blockID,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool isRelatedClear)
    {
      if ((int) blockID == (int) this.BedrockID || p.Y == this.MapBound.Min.Y)
        return ClearBlockResult.BedRock;
      if (!this.AllowEdgeClear && this.IsOnEdge(p) && (p.Y <= (int) this.SeaLevel && !this.IsNothingButAirAndWaterAbove(p)))
        return ClearBlockResult.OutOfBounds;
      if (p.Y < this.MapBound.Max.Y && (int) blockID == (int) this.RopeBlockID && (int) this.GetBlockID(p + GlobalPoint3D.Up) == (int) this.RopeBlockID)
        return ClearBlockResult.CantCutRope;
      if (!this.GetChunk(p).IsGenerated)
        return ClearBlockResult.ChunkNotGenerated;
      return this.MapStrategy.GetClearBlockResult(p, blockID, method, gamerID, isRelatedClear);
    }

    public static bool IsCreativeHelperMethod(UpdateBlockMethod method)
    {
      if (method != UpdateBlockMethod.CreativeHelper)
        return method == UpdateBlockMethod.CreativeHelperPriority;
      return true;
    }

    private bool IsNothingButAirAndWaterAbove(GlobalPoint3D p)
    {
      while (p.Y++ < (int) this.SeaLevel)
      {
        byte blockIdNoCache = this.GetBlockIDNoCache(p);
        if (blockIdNoCache > (byte) 0 && (int) blockIdNoCache != (int) this.WaterBlockID)
          return false;
      }
      return true;
    }

    public void CheckForRelatedClears(GlobalPoint3D p, UpdateBlockMethod method, GamerID playerID)
    {
      ++p.Y;
      bool flag1 = true;
      bool flag2 = method == UpdateBlockMethod.Generation;
      if (p.Y < this.MapBound.Max.Y)
      {
        byte blockIdNoCache = this.GetBlockIDNoCache(p);
        if (this.IsBlockLiquid(blockIdNoCache))
        {
          this.MapStrategy.AddLiquidFlow(p, blockIdNoCache, method);
          if (flag2)
            return;
          flag1 = false;
        }
        else if (!flag2 && (this.IsAttached(p, BlockFace.Up) || this.IsBlockIcon(blockIdNoCache)))
        {
          int num = (int) this.ClearBlock(p, method, playerID, false, true, false);
        }
      }
      --p.Y;
      --p.X;
      if (p.X >= this.MapBound.Min.X)
      {
        byte blockID = flag1 ? this.GetBlockIDNoCache(p) : (byte) 0;
        if (flag1 && this.IsBlockLiquid(blockID))
          this.MapStrategy.AddLiquidFlow(p, blockID, method);
        else if (!flag2 && this.IsAttached(p, BlockFace.Left))
        {
          int num = (int) this.ClearBlock(p, method, playerID, false, true, false);
        }
      }
      p.X += 2;
      if (p.X < this.MapBound.Max.X)
      {
        byte blockID = flag1 ? this.GetBlockIDNoCache(p) : (byte) 0;
        if (flag1 && this.IsBlockLiquid(blockID))
          this.MapStrategy.AddLiquidFlow(p, blockID, method);
        else if (!flag2 && this.IsAttached(p, BlockFace.Right))
        {
          int num = (int) this.ClearBlock(p, method, playerID, false, true, false);
        }
      }
      --p.X;
      --p.Z;
      if (p.Z >= this.MapBound.Min.Z)
      {
        byte blockID = flag1 ? this.GetBlockIDNoCache(p) : (byte) 0;
        if (flag1 && this.IsBlockLiquid(blockID))
          this.MapStrategy.AddLiquidFlow(p, blockID, method);
        else if (!flag2 && this.IsAttached(p, BlockFace.Forward))
        {
          int num = (int) this.ClearBlock(p, method, playerID, false, true, false);
        }
      }
      p.Z += 2;
      if (p.Z < this.MapBound.Max.Z)
      {
        byte blockID = flag1 ? this.GetBlockIDNoCache(p) : (byte) 0;
        if (flag1 && this.IsBlockLiquid(blockID))
          this.MapStrategy.AddLiquidFlow(p, blockID, method);
        else if (!flag2 && this.IsAttached(p, BlockFace.Backward))
        {
          int num = (int) this.ClearBlock(p, method, playerID, false, true, false);
        }
      }
      if (flag2)
        return;
      --p.Z;
      --p.Y;
      if (p.Y < this.MapBound.Min.Y || !this.IsAttached(p, BlockFace.Down))
        return;
      int num1 = (int) this.ClearBlock(p, method, playerID, false, true, false);
    }

    public bool HasAttachment(GlobalPoint3D p)
    {
      ++p.Y;
      if (p.Y < this.MapBound.Max.Y && this.IsAttached(p, BlockFace.Up))
        return true;
      --p.Y;
      --p.X;
      if (p.X >= this.MapBound.Min.X && this.IsAttached(p, BlockFace.Left))
        return true;
      p.X += 2;
      if (p.X < this.MapBound.Max.X && this.IsAttached(p, BlockFace.Right))
        return true;
      --p.X;
      --p.Z;
      if (p.Z >= this.MapBound.Min.Z && this.IsAttached(p, BlockFace.Forward))
        return true;
      p.Z += 2;
      if (p.Z < this.MapBound.Max.Z && this.IsAttached(p, BlockFace.Backward))
        return true;
      --p.Z;
      --p.Y;
      return p.Y > this.MapBound.Min.Y && this.IsAttached(p, BlockFace.Down);
    }

    public byte GetAuxData(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      return region != null ? region.GetAUXData(p) : (byte) 0;
    }

    public byte GetAuxDataNoCache(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      return region != null ? region.GetAUXDataNoCache(p) : (byte) 0;
    }

    public byte GetAuxHighData(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      return region != null ? region.GetAUXHighData(p) : (byte) 0;
    }

    public byte GetAuxHighDataNoCache(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      return region != null ? region.GetAUXHighDataNoCache(p) : (byte) 0;
    }

    public byte GetAuxFullData(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      return region != null ? region.GetAUXFullData(p) : (byte) 0;
    }

    public byte GetAuxFullDataNoCache(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      return region != null ? region.GetAUXFullDataNoCache(p) : (byte) 0;
    }

    public bool HasChanged(byte auxData)
    {
      return ((int) auxData & 8) > 0;
    }

    public bool HasChanged(MapBlock blockData)
    {
      return ((int) blockData.AuxData & 8) > 0;
    }

    public bool HasChanged(GlobalPoint3D p)
    {
      return ((int) this.GetAuxFullData(p) & 8) > 0;
    }

    public virtual void RotateBlock(ref MapBlock blockData, int facing, bool from)
    {
    }

    public MapChunk SetAuxData(
      GlobalPoint3D p,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      return this.SetAuxData(p, this.GetAuxData(p), auxData, method, playerID, transmit);
    }

    public MapChunk SetAuxData(
      GlobalPoint3D p,
      byte oldAuxData,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return (MapChunk) null;
      MapChunk mapChunk = region.SetAuxData(p, auxData, method);
      if (mapChunk != null)
        this.MapStrategy.AuxChanged(p, oldAuxData, auxData, method, playerID, transmit);
      return mapChunk;
    }

    public MapLight GetLight(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return this.SunLight;
      return region.GetLight(p);
    }

    public MapLight GetLightNoCache(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return this.SunLight;
      return region.GetLightNoCache(p);
    }

    public byte GetSunLight(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return this.SunLight.SunLight;
      return region.GetSunLight(p);
    }

    public byte GetBlockLight(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return this.SunLight.BlockLight;
      return region.GetBlockLight(p);
    }

    public Vector2 GetSunAndBlockLightNormalized(GlobalPoint3D p)
    {
      MapLight light = this.GetLight(p);
      return new Vector2((float) light.SunLight / this.MaxLight, (float) light.BlockLight / this.MaxLight);
    }

    public MapLight GetMaxNeighbourLight(GlobalPoint3D p)
    {
      return this.GetMaxNeighbourLight(p, p);
    }

    public MapLight GetMaxNeighbourLight(GlobalPoint3D p, GlobalPoint3D op)
    {
      MapLight mapLight = new MapLight();
      if (p.X > this.MapBound.Min.X && p.X - 1 != op.X)
      {
        --p.X;
        MapLight light = this.GetLight(p);
        ++p.X;
        if ((int) light.SunLight > (int) mapLight.SunLight)
          mapLight.SunLight = light.SunLight;
        if ((int) light.BlockLight > (int) mapLight.BlockLight)
          mapLight.BlockLight = light.BlockLight;
      }
      if (p.X < this.MapBound.Max.X - 1 && p.X + 1 != op.X)
      {
        ++p.X;
        MapLight light = this.GetLight(p);
        --p.X;
        if ((int) light.SunLight > (int) mapLight.SunLight)
          mapLight.SunLight = light.SunLight;
        if ((int) light.BlockLight > (int) mapLight.BlockLight)
          mapLight.BlockLight = light.BlockLight;
      }
      if (p.Y > this.MapBound.Min.Y && p.Y - 1 != op.Y)
      {
        --p.Y;
        MapLight light = this.GetLight(p);
        ++p.Y;
        if ((int) light.SunLight > (int) mapLight.SunLight)
          mapLight.SunLight = light.SunLight;
        if ((int) light.BlockLight > (int) mapLight.BlockLight)
          mapLight.BlockLight = light.BlockLight;
      }
      if (p.Y < this.MapBound.Max.Y - 1 && p.Y + 1 != op.Y)
      {
        ++p.Y;
        MapLight light = this.GetLight(p);
        --p.Y;
        if ((int) light.SunLight > (int) mapLight.SunLight)
          mapLight.SunLight = light.SunLight;
        if ((int) light.BlockLight > (int) mapLight.BlockLight)
          mapLight.BlockLight = light.BlockLight;
      }
      if (p.Z > this.MapBound.Min.Z && p.Z - 1 != op.Z)
      {
        --p.Z;
        MapLight light = this.GetLight(p);
        ++p.Z;
        if ((int) light.SunLight > (int) mapLight.SunLight)
          mapLight.SunLight = light.SunLight;
        if ((int) light.BlockLight > (int) mapLight.BlockLight)
          mapLight.BlockLight = light.BlockLight;
      }
      if (p.Z < this.MapBound.Max.Z - 1 && p.Z + 1 != op.Z)
      {
        ++p.Z;
        MapLight light = this.GetLight(p);
        --p.Z;
        if ((int) light.SunLight > (int) mapLight.SunLight)
          mapLight.SunLight = light.SunLight;
        if ((int) light.BlockLight > (int) mapLight.BlockLight)
          mapLight.BlockLight = light.BlockLight;
      }
      return mapLight;
    }

    public byte GetMaxNeighbourSunLight(GlobalPoint3D p, GlobalPoint3D op)
    {
      byte num = 0;
      if (this.CanBlockSeeTheSky(p))
      {
        num = this.SunLight.SunLight;
      }
      else
      {
        if (p.X > this.MapBound.Min.X && p.X - 1 != op.X)
        {
          --p.X;
          byte sunLight = this.GetSunLight(p);
          ++p.X;
          if ((int) sunLight > (int) num)
            num = sunLight;
        }
        if (p.X < this.MapBound.Max.X - 1 && p.X + 1 != op.X)
        {
          ++p.X;
          byte sunLight = this.GetSunLight(p);
          --p.X;
          if ((int) sunLight > (int) num)
            num = sunLight;
        }
        if (p.Y < this.MapBound.Max.Y - 1 && p.Y + 1 != op.Y)
        {
          ++p.Y;
          byte sunLight = this.GetSunLight(p);
          --p.Y;
          if ((int) sunLight > (int) num)
            num = sunLight;
        }
        if (p.Z > this.MapBound.Min.Z && p.Z - 1 != op.Z)
        {
          --p.Z;
          byte sunLight = this.GetSunLight(p);
          ++p.Z;
          if ((int) sunLight > (int) num)
            num = sunLight;
        }
        if (p.Z < this.MapBound.Max.Z - 1 && p.Z + 1 != op.Z)
        {
          ++p.Z;
          byte sunLight = this.GetSunLight(p);
          --p.Z;
          if ((int) sunLight > (int) num)
            num = sunLight;
        }
      }
      return num;
    }

    public byte GetMaxNeighbourBlockLight(GlobalPoint3D p, GlobalPoint3D op)
    {
      byte num1 = 0;
      if (p.X > this.MapBound.Min.X && p.X - 1 != op.X)
      {
        --p.X;
        byte luminance = this.GetLuminance(ref p);
        byte num2 = this.GetBlockLight(p);
        if ((int) luminance > (int) num2)
          num2 = luminance;
        if ((int) num2 > (int) num1)
          num1 = num2;
        ++p.X;
      }
      if (p.X < this.MapBound.Max.X - 1 && p.X + 1 != op.X)
      {
        ++p.X;
        byte luminance = this.GetLuminance(ref p);
        byte num2 = this.GetBlockLight(p);
        if ((int) luminance > (int) num2)
          num2 = luminance;
        if ((int) num2 > (int) num1)
          num1 = num2;
        --p.X;
      }
      if (p.Y > this.MapBound.Min.Y && p.Y - 1 != op.Y)
      {
        --p.Y;
        byte luminance = this.GetLuminance(ref p);
        byte num2 = this.GetBlockLight(p);
        if ((int) luminance > (int) num2)
          num2 = luminance;
        if ((int) num2 > (int) num1)
          num1 = num2;
        ++p.Y;
      }
      if (p.Y < this.MapBound.Max.Y - 1 && p.Y + 1 != op.Y)
      {
        ++p.Y;
        byte luminance = this.GetLuminance(ref p);
        byte num2 = this.GetBlockLight(p);
        if ((int) luminance > (int) num2)
          num2 = luminance;
        if ((int) num2 > (int) num1)
          num1 = num2;
        --p.Y;
      }
      if (p.Z > this.MapBound.Min.Z && p.Z - 1 != op.Z)
      {
        --p.Z;
        byte luminance = this.GetLuminance(ref p);
        byte num2 = this.GetBlockLight(p);
        if ((int) luminance > (int) num2)
          num2 = luminance;
        if ((int) num2 > (int) num1)
          num1 = num2;
        ++p.Z;
      }
      if (p.Z < this.MapBound.Max.Z - 1 && p.Z + 1 != op.Z)
      {
        ++p.Z;
        byte luminance = this.GetLuminance(ref p);
        byte num2 = this.GetBlockLight(p);
        if ((int) luminance > (int) num2)
          num2 = luminance;
        if ((int) num2 > (int) num1)
          num1 = num2;
        --p.Z;
      }
      return num1;
    }

    public bool CanBlockSeeTheSky(GlobalPoint3D p)
    {
      return p.Y >= (int) this.GetHeight(p);
    }

    public float GetLightNormalized(byte light)
    {
      return (float) light / this.MaxLight;
    }

    public float GetLightNormalized(GlobalPoint3D p)
    {
      return this.GetLightNormalized(this.GetLight(p));
    }

    public float GetLightNormalized(MapBlock data)
    {
      return this.GetLightNormalized(Math.Max((byte) ((double) data.Light.SunLight * (double) this.LightCycle), data.Light.BlockLight));
    }

    public float GetLightNormalized(MapLight data)
    {
      return this.GetLightNormalized(Math.Max((byte) ((double) data.SunLight * (double) this.LightCycle), data.BlockLight));
    }

    public float GetSunLightNormalized(GlobalPoint3D p)
    {
      return this.GetLightNormalized(this.GetLight(p).SunLight);
    }

    public float GetBlockLightNormalized(GlobalPoint3D p)
    {
      return this.GetLightNormalized(this.GetLight(p).BlockLight);
    }

    public void Fill(GlobalPoint3D from, GlobalPoint3D to, MapBlock data, bool isUserEdited)
    {
      BoundingBox box = new BoundingBox(new Vector3((float) from.X * this.TileSize, (float) from.Y * this.TileSize, (float) from.Z * this.TileSize), new Vector3((float) to.X * this.TileSize, (float) to.Y * this.TileSize, (float) to.Z * this.TileSize));
      foreach (KeyValuePair<int, MapRegion> region in this.Regions)
      {
        MapRegion mapRegion = region.Value;
        if (mapRegion.Box.Contains(box) != ContainmentType.Disjoint)
        {
          for (int index = 0; index < mapRegion.Chunks.Length; ++index)
          {
            MapChunk chunk = mapRegion.Chunks[index];
            if (chunk != null)
            {
              BoundingBox boundingBox;
              boundingBox.Min = chunk.Box.Min + mapRegion.Box.Min;
              boundingBox.Max = chunk.Box.Max + mapRegion.Box.Min;
              if (boundingBox.Contains(box) != ContainmentType.Disjoint)
                chunk.Fill(from, to, data, isUserEdited);
            }
          }
        }
      }
      this.SetHeight(from, to);
    }

    public void Fill2(GlobalPoint3D from, GlobalPoint3D to, byte blockID, byte aux)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      for (p.Y = from.Y; p.Y <= to.Y; ++p.Y)
      {
        for (p.Z = from.Z; p.Z <= to.Z; ++p.Z)
        {
          for (p.X = from.X; p.X <= to.X; ++p.X)
            this.SetBlockData(p, blockID, aux, UpdateBlockMethod.Strategy, GamerID.Sys1, false);
        }
      }
    }

    public void CopyFrom(
      Map srcMap,
      GlobalPoint3D srcOffset,
      GlobalPoint3D destOffset,
      GlobalPoint3D size,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      int facing,
      UpdateBlockMethod method,
      Map.CopyAccess copyAccess,
      GamerID playerID,
      IProgressBar progress)
    {
      this.PreCopy(srcMap, facing, method, playerID);
      GlobalPoint3D zero1 = GlobalPoint3D.Zero;
      GlobalPoint3D zero2 = GlobalPoint3D.Zero;
      GlobalPoint3D zero3 = GlobalPoint3D.Zero;
      float increment = 1f / (float) (size.X * size.Y * size.Z);
      for (zero1.Y = 0; zero1.Y < size.Y; ++zero1.Y)
      {
        for (zero1.Z = 0; zero1.Z < size.Z; ++zero1.Z)
        {
          for (zero1.X = 0; zero1.X < size.X; ++zero1.X)
          {
            zero3.X = zero1.X + srcOffset.X;
            zero3.Y = zero1.Y + srcOffset.Y;
            zero3.Z = zero1.Z + srcOffset.Z;
            if (zero3.X < xmin.X || zero3.X > xmax.X || (zero3.Y < xmin.Y || zero3.Y > xmax.Y) || (zero3.Z < xmin.Z || zero3.Z > xmax.Z))
            {
              MapBlock blockData = srcMap.GetBlockData(zero3);
              if (copyAccess == Map.CopyAccess.Full || this.CanCopy(ref blockData))
              {
                this.RotateBlock(ref blockData, facing, true);
                zero2.X = zero1.X + destOffset.X;
                zero2.Y = zero1.Y + destOffset.Y;
                zero2.Z = zero1.Z + destOffset.Z;
                this.AdjustBlockDataForMove(ref blockData);
                if ((this.SetBlockData(zero2, blockData.BlockID, blockData.AuxData, method, GamerID.Sys1, false) ?? this.SetCopySameBlockData(zero2, blockData.BlockID, blockData.AuxData, method, GamerID.Sys1, false)) != null)
                  this.CopyToSetBlock(srcMap, zero3, zero2, blockData.BlockID, blockData.AuxData, facing, method, playerID);
              }
            }
            progress?.AddProgress(increment);
          }
        }
      }
      this.PostCopy(srcMap, facing, method, playerID);
    }

    public BoxInt? CopyTo(
      Map destMap,
      GlobalPoint3D srcOffset,
      GlobalPoint3D destOffset,
      GlobalPoint3D size,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      int facing,
      UpdateBlockMethod method,
      Map.CopyType copyType,
      Map.CopyAccess copyAccess,
      GamerID playerID,
      bool transmit,
      IProgressBar progress)
    {
      destMap.PreCopy(this, facing, method, playerID);
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D2 = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D3 = new GlobalPoint3D();
      GlobalPoint3D p = new GlobalPoint3D();
      GlobalPoint3D maxValue = GlobalPoint3D.MaxValue;
      GlobalPoint3D minValue = GlobalPoint3D.MinValue;
      float increment = 1f / (float) (size.X * size.Y * size.Z);
      bool flag1 = copyType == Map.CopyType.Overwrite;
      bool flag2 = copyType == Map.CopyType.NoOverwrite;
      for (globalPoint3D1.Y = 0; globalPoint3D1.Y < size.Y; ++globalPoint3D1.Y)
      {
        globalPoint3D3.Y = globalPoint3D1.Y;
        p.Y = globalPoint3D3.Y + destOffset.Y;
        if (p.Y > destMap.MapBound.Min.Y && p.Y < destMap.MapBound.Max.Y)
        {
          for (globalPoint3D1.Z = 0; globalPoint3D1.Z < size.Z; ++globalPoint3D1.Z)
          {
            switch (facing)
            {
              case 0:
                globalPoint3D3.Z = globalPoint3D1.Z;
                break;
              case 1:
                globalPoint3D3.X = -globalPoint3D1.Z;
                break;
              case 2:
                globalPoint3D3.Z = -globalPoint3D1.Z;
                break;
              case 3:
                globalPoint3D3.X = globalPoint3D1.Z;
                break;
            }
            for (globalPoint3D1.X = 0; globalPoint3D1.X < size.X; ++globalPoint3D1.X)
            {
              switch (facing)
              {
                case 0:
                  globalPoint3D3.X = globalPoint3D1.X;
                  break;
                case 1:
                  globalPoint3D3.Z = globalPoint3D1.X;
                  break;
                case 2:
                  globalPoint3D3.X = -globalPoint3D1.X;
                  break;
                case 3:
                  globalPoint3D3.Z = -globalPoint3D1.X;
                  break;
              }
              p.X = globalPoint3D3.X + destOffset.X;
              p.Z = globalPoint3D3.Z + destOffset.Z;
              if (p.X >= destMap.MapBound.Min.X && p.X < destMap.MapBound.Max.X && (p.Z >= destMap.MapBound.Min.Z && p.Z < destMap.MapBound.Max.Z))
              {
                if (p.X < xmin.X || p.X > xmax.X || (p.Y < xmin.Y || p.Y > xmax.Y) || (p.Z < xmin.Z || p.Z > xmax.Z))
                {
                  globalPoint3D2.X = globalPoint3D1.X + srcOffset.X;
                  globalPoint3D2.Y = globalPoint3D1.Y + srcOffset.Y;
                  globalPoint3D2.Z = globalPoint3D1.Z + srcOffset.Z;
                  MapBlock blockData = this.GetBlockData(globalPoint3D2);
                  if ((blockData.BlockID > (byte) 0 || flag1) && (copyAccess == Map.CopyAccess.Full || this.CanCopy(ref blockData)))
                  {
                    byte blockId = destMap.GetBlockID(p);
                    if (!flag2 || blockId == (byte) 0)
                    {
                      if (this.IsOkForCopyToOverwrite(destMap.MapStrategy.GetClearBlockResult(p, blockId, method, playerID, false)))
                      {
                        this.RotateBlock(ref blockData, facing, false);
                        destMap.AdjustBlockDataForMove(ref blockData);
                        if ((destMap.SetBlockData(p, blockData.BlockID, blockData.AuxData, method, playerID, transmit) ?? destMap.SetCopySameBlockData(p, blockData.BlockID, blockData.AuxData, method, GamerID.Sys1, false)) != null)
                          destMap.CopyToSetBlock(this, globalPoint3D2, p, blockData.BlockID, blockData.AuxData, facing, method, playerID);
                      }
                      if (p.X < maxValue.X)
                        maxValue.X = p.X;
                      if (p.Y < maxValue.Y)
                        maxValue.Y = p.Y;
                      if (p.Z < maxValue.Z)
                        maxValue.Z = p.Z;
                      if (p.X > minValue.X)
                        minValue.X = p.X;
                      if (p.Y > minValue.Y)
                        minValue.Y = p.Y;
                      if (p.Z > minValue.Z)
                        minValue.Z = p.Z;
                    }
                  }
                }
                progress?.AddProgress(increment);
              }
            }
          }
        }
      }
      if (!(maxValue != GlobalPoint3D.MaxValue) || !(minValue != GlobalPoint3D.MinValue))
        return new BoxInt?();
      destMap.PostCopy(this, facing, method, playerID);
      return new BoxInt?(new BoxInt()
      {
        Min = maxValue,
        Max = minValue
      });
    }

    protected virtual void AdjustBlockDataForMove(ref MapBlock blockData)
    {
    }

    protected virtual MapChunk SetCopySameBlockData(
      GlobalPoint3D p,
      byte blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
      return (MapChunk) null;
    }

    protected virtual void CopyToSetBlock(
      Map srcMap,
      GlobalPoint3D srcPoint,
      GlobalPoint3D p,
      byte blockID,
      byte auxData,
      int facing,
      UpdateBlockMethod method,
      GamerID playerID)
    {
    }

    protected virtual void PreCopy(
      Map srcMap,
      int facing,
      UpdateBlockMethod method,
      GamerID playerID)
    {
    }

    protected virtual void PostCopy(
      Map srcMap,
      int facing,
      UpdateBlockMethod method,
      GamerID playerID)
    {
    }

    protected virtual bool CanCopy(ref MapBlock blockData)
    {
      return true;
    }

    protected virtual bool IsOkForCopyToOverwrite(ClearBlockResult result)
    {
      switch (result)
      {
        case ClearBlockResult.Failure:
        case ClearBlockResult.OutOfBounds:
        case ClearBlockResult.ChunkNotGenerated:
        case ClearBlockResult.AnotherPlayerHasThisBlockOpen:
        case ClearBlockResult.BedRock:
        case ClearBlockResult.PermissionDenied:
        case ClearBlockResult.NoEditZone:
          return false;
        default:
          return true;
      }
    }

    public void FindPassableSpace(
      GlobalPoint3D origin,
      int radius,
      int distanceFromOrigin,
      List<GlobalPoint3D> results)
    {
      GlobalPoint3D p;
      for (p.Y = origin.Y - distanceFromOrigin; p.Y <= origin.Y + distanceFromOrigin; ++p.Y)
      {
        for (p.Z = origin.Z - distanceFromOrigin; p.Z <= origin.Z + distanceFromOrigin; ++p.Z)
        {
          for (p.X = origin.X - distanceFromOrigin; p.X <= origin.X + distanceFromOrigin; ++p.X)
          {
            if (this.CheckFreeSpaceAround(p, radius))
              results.Add(p);
          }
        }
      }
    }

    private bool CheckFreeSpaceAround(GlobalPoint3D p, int radius)
    {
      GlobalPoint3D p1;
      for (p1.Y = p.Y - radius; p1.Y <= p.Y + radius; ++p1.Y)
      {
        if (p1.Y <= this.MapBound.Min.Y || p1.Y >= this.MapBound.Max.Y - 1)
          return false;
        for (p1.Z = p.Z - radius; p1.Z <= p.Z + radius; ++p1.Z)
        {
          if (p1.Z <= this.MapBound.Min.Z || p1.Z >= this.MapBound.Max.Z - 1)
            return false;
          for (p1.X = p.X - radius; p1.X <= p.X + radius; ++p1.X)
          {
            if (p1.X <= this.MapBound.Min.X || p1.X >= this.MapBound.Max.X - 1 || !this.IsPassable(p1))
              return false;
          }
        }
      }
      return true;
    }

    public void FindEmptyBlocks(
      GlobalPoint3D origin,
      int radius,
      int distanceFromOrigin,
      List<GlobalPoint3D> results)
    {
      GlobalPoint3D p;
      for (p.Y = origin.Y - distanceFromOrigin; p.Y <= origin.Y + distanceFromOrigin; ++p.Y)
      {
        for (p.Z = origin.Z - distanceFromOrigin; p.Z <= origin.Z + distanceFromOrigin; ++p.Z)
        {
          for (p.X = origin.X - distanceFromOrigin; p.X <= origin.X + distanceFromOrigin; ++p.X)
            this.FindEmptyBlocksAround(p, radius, results);
        }
      }
    }

    private void FindEmptyBlocksAround(GlobalPoint3D p, int radius, List<GlobalPoint3D> results)
    {
      GlobalPoint3D p1;
      for (p1.Y = p.Y - radius; p1.Y <= p.Y + radius; ++p1.Y)
      {
        if (p1.Y > this.MapBound.Min.Y && p1.Y < this.MapBound.Max.Y - 1)
        {
          for (p1.Z = p.Z - radius; p1.Z <= p.Z + radius; ++p1.Z)
          {
            if (p1.Z > this.MapBound.Min.Z && p1.Z < this.MapBound.Max.Z - 1)
            {
              for (p1.X = p.X - radius; p1.X <= p.X + radius; ++p1.X)
              {
                if (p1.X > this.MapBound.Min.X && p1.X < this.MapBound.Max.X - 1 && (!results.Contains(p1) && this.GetBlockIDNoCache(p1) == (byte) 0))
                  results.Add(p1);
              }
            }
          }
        }
      }
    }

    public void Replace(byte blockID, byte withID)
    {
    }

    public void RemoveAll(byte blockID, short playerID)
    {
    }

    public Vector3 GetBlockCenter(GlobalPoint3D point)
    {
      Vector3 zero = Vector3.Zero;
      if ((double) this.TileSize == 1.0)
      {
        zero.X = (float) point.X + 0.5f;
        zero.Y = (float) point.Y + 0.5f;
        zero.Z = (float) point.Z + 0.5f;
      }
      else
      {
        zero.X = (float) point.X * this.TileSize + this.HalfTileSize;
        zero.Y = (float) point.Y * this.TileSize + this.HalfTileSize;
        zero.Z = (float) point.Z * this.TileSize + this.HalfTileSize;
      }
      return zero;
    }

    public Vector3 GetBlockCenter(Point3D point)
    {
      Vector3 zero = Vector3.Zero;
      if ((double) this.TileSize == 1.0)
      {
        zero.X = (float) point.X + 0.5f;
        zero.Y = (float) point.Y + 0.5f;
        zero.Z = (float) point.Z + 0.5f;
      }
      else
      {
        zero.X = (float) point.X * this.TileSize + this.HalfTileSize;
        zero.Y = (float) point.Y * this.TileSize + this.HalfTileSize;
        zero.Z = (float) point.Z * this.TileSize + this.HalfTileSize;
      }
      return zero;
    }

    public Vector3 GetPosition(GlobalPoint3D point)
    {
      Vector3 zero = Vector3.Zero;
      if ((double) this.TileSize == 1.0)
      {
        zero.X = (float) point.X;
        zero.Y = (float) (point.Y + 1);
        zero.Z = (float) point.Z;
      }
      else
      {
        zero.X = (float) point.X * this.TileSize;
        zero.Y = (float) point.Y * this.TileSize + this.TileSize;
        zero.Z = (float) point.Z * this.TileSize;
      }
      return zero;
    }

    public bool IsSolid(GlobalPoint3D p)
    {
      return this.IsBlockSolid(this.GetBlockIDNoCache(p));
    }

    public bool IsPassable(Vector3 position)
    {
      return this.IsPassable(this.GetPoint(position));
    }

    public bool IsPassable(GlobalPoint3D p)
    {
      return this.IsBlockPassable(this.GetBlockIDNoCache(p));
    }

    public bool IsPassable(BoundingBox box)
    {
      lock (this.PointsOfPenetration)
      {
        this.GetPointsOfPenetration(box.Min, box.Max);
        foreach (GlobalPoint3D p in this.PointsOfPenetration)
        {
          if (!this.IsPassable(p))
            return false;
        }
      }
      return true;
    }

    public bool IsIcon(Vector3 position)
    {
      return this.IsIcon(this.GetPoint(position));
    }

    public bool IsIcon(GlobalPoint3D p)
    {
      return this.IsBlockIcon(this.GetBlockIDNoCache(p));
    }

    public bool IsAttached(GlobalPoint3D p, BlockFace face)
    {
      MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
      if (this.IsBlockAttachable(blockIdAndAux.BlockID))
        return this.GetFaceFromAux(blockIdAndAux.BlockID, blockIdAndAux.AuxData) == face;
      return false;
    }

    protected virtual BlockFace GetFaceFromAux(byte blockID, byte aux)
    {
      return (BlockFace) ((uint) aux & 7U);
    }

    public bool IsClearAndAbove(GlobalPoint3D p, int andAboveCount)
    {
      if (this.GetBlockIDNoCache(p) != (byte) 0)
        return false;
      for (; andAboveCount > 0; --andAboveCount)
      {
        ++p.Y;
        if (this.GetBlockIDNoCache(p) > (byte) 0)
          return false;
      }
      return true;
    }

    public bool IsOnly(GlobalPoint3D min, GlobalPoint3D max, Map.IsOnlyQualifier qualifier)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      for (zero.Z = min.Z; zero.Z <= max.Z; ++zero.Z)
      {
        for (zero.Y = min.Y; zero.Y <= max.Y; ++zero.Y)
        {
          for (zero.X = min.X; zero.X <= max.X; ++zero.X)
          {
            if (!qualifier(this.GetBlockIDNoCache(zero)))
              return false;
          }
        }
      }
      return true;
    }

    public bool IsFacesSurroundedBy(GlobalPoint3D p, byte blockID)
    {
      --p.Y;
      if (p.Y >= this.MapBound.Min.Y && (int) this.GetBlockIDNoCache(p) != (int) blockID)
        return false;
      p.Y += 2;
      if (p.Y < this.MapBound.Max.Y && (int) this.GetBlockIDNoCache(p) != (int) blockID)
        return false;
      --p.Y;
      --p.X;
      if (p.X >= this.MapBound.Min.X && (int) this.GetBlockIDNoCache(p) != (int) blockID)
        return false;
      p.X += 2;
      if (p.X < this.MapBound.Max.X && (int) this.GetBlockIDNoCache(p) != (int) blockID)
        return false;
      --p.X;
      --p.Z;
      if (p.Z >= this.MapBound.Min.Z && (int) this.GetBlockIDNoCache(p) != (int) blockID)
        return false;
      p.Z += 2;
      return p.Z >= this.MapBound.Max.Z || (int) this.GetBlockIDNoCache(p) == (int) blockID;
    }

    public virtual Vector3 GetLiquidFlowDirection(GlobalPoint3D p)
    {
      Vector3 zero = Vector3.Zero;
      MapBlock blockIdAndAuxNoCache1 = this.GetBlockIDAndAuxNoCache(p);
      if ((int) blockIdAndAuxNoCache1.BlockID == (int) this.WaterBlockID || (int) blockIdAndAuxNoCache1.BlockID == (int) this.LavaBlockID)
      {
        byte num = (int) blockIdAndAuxNoCache1.BlockID == (int) this.WaterBlockID ? this.WaterBlockID : this.LavaBlockID;
        blockIdAndAuxNoCache1.AuxData &= (byte) 7;
        if (blockIdAndAuxNoCache1.AuxData > (byte) 0)
        {
          --p.X;
          MapBlock blockIdAndAuxNoCache2 = this.GetBlockIDAndAuxNoCache(p);
          bool flag1 = (int) blockIdAndAuxNoCache2.BlockID == (int) num;
          p.X += 2;
          MapBlock blockIdAndAuxNoCache3 = this.GetBlockIDAndAuxNoCache(p);
          bool flag2 = (int) blockIdAndAuxNoCache3.BlockID == (int) num;
          --p.X;
          --p.Z;
          MapBlock blockIdAndAuxNoCache4 = this.GetBlockIDAndAuxNoCache(p);
          bool flag3 = (int) blockIdAndAuxNoCache4.BlockID == (int) num;
          p.Z += 2;
          MapBlock blockIdAndAuxNoCache5 = this.GetBlockIDAndAuxNoCache(p);
          bool flag4 = (int) blockIdAndAuxNoCache5.BlockID == (int) num;
          --p.Z;
          blockIdAndAuxNoCache2.AuxData &= (byte) 7;
          blockIdAndAuxNoCache3.AuxData &= (byte) 7;
          blockIdAndAuxNoCache4.AuxData &= (byte) 7;
          blockIdAndAuxNoCache5.AuxData &= (byte) 7;
          if ((int) num == (int) this.WaterBlockID)
          {
            if (flag1 && (int) blockIdAndAuxNoCache2.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.X += this.WaterFlowSpeedY;
            if (flag2 && (int) blockIdAndAuxNoCache3.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.X -= this.WaterFlowSpeedY;
            if (flag3 && (int) blockIdAndAuxNoCache4.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.Z += this.WaterFlowSpeedY;
            if (flag4 && (int) blockIdAndAuxNoCache5.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.Z -= this.WaterFlowSpeedY;
          }
          else
          {
            if (flag1 && (int) blockIdAndAuxNoCache2.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.X += this.LavaFlowSpeedY;
            if (flag2 && (int) blockIdAndAuxNoCache3.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.X -= this.LavaFlowSpeedY;
            if (flag3 && (int) blockIdAndAuxNoCache4.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.Z += this.LavaFlowSpeedY;
            if (flag4 && (int) blockIdAndAuxNoCache5.AuxData < (int) blockIdAndAuxNoCache1.AuxData)
              zero.Z -= this.LavaFlowSpeedY;
          }
        }
      }
      return zero;
    }

    public bool IsValidPoint(int x, int y, int z)
    {
      if (x >= this.MapBound.Min.X && x < this.MapBound.Max.X && (y >= this.MapBound.Min.Y && y < this.MapBound.Max.Y) && z >= this.MapBound.Min.Z)
        return z < this.MapBound.Max.Z;
      return false;
    }

    public bool IsValidPoint(GlobalPoint3D p)
    {
      if (p.X >= this.MapBound.Min.X && p.X < this.MapBound.Max.X && (p.Y >= this.MapBound.Min.Y && p.Y < this.MapBound.Max.Y) && p.Z >= this.MapBound.Min.Z)
        return p.Z < this.MapBound.Max.Z;
      return false;
    }

    public bool IsInsideMap(GlobalPoint3D p, Point3D b)
    {
      if (p.X >= this.MapBound.Min.X + b.X && p.X < this.MapBound.Max.X - b.X && (p.Y >= this.MapBound.Min.Y + b.Y && p.Y < this.MapBound.Max.Y - b.Y) && p.Z >= this.MapBound.Min.Z + b.Z)
        return p.Z < this.MapBound.Max.Z - b.Z;
      return false;
    }

    public bool IsIntersectingMap(GlobalPoint3D offset, Point3D chunksize)
    {
      if (offset.X + chunksize.X > this.MapBound.Min.X && offset.X < this.MapBound.Max.X && (offset.Y + chunksize.Y > this.MapBound.Min.Y && offset.Y < this.MapBound.Max.Y) && offset.Z + chunksize.Z > this.MapBound.Min.Z)
        return offset.Z < this.MapBound.Max.Z;
      return false;
    }

    public bool IsIntersectingMap(BoundingBox box)
    {
      if ((double) box.Max.X > (double) this.MapBound.Min.X * (double) this.TileSize && (double) box.Min.X < (double) this.MapBound.Max.X * (double) this.TileSize && ((double) box.Max.Y > (double) this.MapBound.Min.Y * (double) this.TileSize && (double) box.Min.Y < (double) this.MapBound.Max.Y * (double) this.TileSize) && (double) box.Max.Z > (double) this.MapBound.Min.Z * (double) this.TileSize)
        return (double) box.Min.Z < (double) this.MapBound.Max.Z * (double) this.TileSize;
      return false;
    }

    public bool IsNextTo(GlobalPoint3D p, byte blockID)
    {
      return this.IsNextTo(p, blockID, -1, false, false);
    }

    public bool IsNextTo(GlobalPoint3D p, byte blockID, int auxMatch)
    {
      return this.IsNextTo(p, blockID, auxMatch, false, false);
    }

    public bool IsNextTo(
      GlobalPoint3D p,
      byte blockID,
      int auxMatch,
      bool ignoreSelf,
      bool ignoreBelow)
    {
      if (!ignoreSelf && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAuxData(p) == auxMatch))
        return true;
      --p.X;
      if (p.X >= this.MapBound.Min.X && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAuxData(p) == auxMatch))
        return true;
      ++p.X;
      ++p.Y;
      if (p.Y < this.MapBound.Max.Y && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAuxData(p) == auxMatch))
        return true;
      --p.Y;
      --p.Z;
      if (p.Z >= this.MapBound.Min.Z && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAuxData(p) == auxMatch))
        return true;
      ++p.Z;
      ++p.X;
      if (p.X < this.MapBound.Max.X && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAuxData(p) == auxMatch))
        return true;
      --p.X;
      ++p.Z;
      if (p.Z < this.MapBound.Max.Z && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAuxData(p) == auxMatch))
        return true;
      --p.Z;
      if (!ignoreBelow)
      {
        --p.Y;
        if (p.Y >= this.MapBound.Min.Y && (int) this.GetBlockID(p) == (int) blockID && (auxMatch == -1 || (int) this.GetAuxData(p) == auxMatch))
          return true;
        ++p.Y;
      }
      return false;
    }

    public bool IsNextTo(GlobalPoint3D min, GlobalPoint3D max, byte blockID)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      for (p.Z = min.Z; p.Z <= max.Z; ++p.Z)
      {
        for (p.Y = min.Y; p.Y <= max.Y; ++p.Y)
        {
          for (p.X = min.X; p.X <= max.X; ++p.X)
          {
            if ((int) this.GetBlockIDNoCache(p) == (int) blockID)
              return true;
          }
        }
      }
      return false;
    }

    private bool IsOnEdge(GlobalPoint3D p)
    {
      if (p.X != this.MapBound.Min.X && p.X != this.MapBound.Max.X - 1 && (p.Y != this.MapBound.Min.Y && p.Y != this.MapBound.Max.Y - 1) && p.Z != this.MapBound.Min.Z)
        return p.Z == this.MapBound.Max.Z - 1;
      return true;
    }

    private bool IsRetainingLiquid(GlobalPoint3D p)
    {
      --p.X;
      if (p.X >= this.MapBound.Min.X)
      {
        MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
        if (this.IsBlockLiquid(blockIdAndAux.BlockID) && ((int) blockIdAndAux.AuxData & 7) < 7)
          return true;
      }
      ++p.X;
      ++p.Y;
      if (p.Y < this.MapBound.Max.Y)
      {
        MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
        if (this.IsBlockLiquid(blockIdAndAux.BlockID) && ((int) blockIdAndAux.AuxData & 7) < 7)
          return true;
      }
      --p.Y;
      --p.Z;
      if (p.Z >= this.MapBound.Min.Z)
      {
        MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
        if (this.IsBlockLiquid(blockIdAndAux.BlockID) && ((int) blockIdAndAux.AuxData & 7) < 7)
          return true;
      }
      ++p.Z;
      ++p.X;
      if (p.X < this.MapBound.Max.X)
      {
        MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
        if (this.IsBlockLiquid(blockIdAndAux.BlockID) && ((int) blockIdAndAux.AuxData & 7) < 7)
          return true;
      }
      --p.X;
      ++p.Z;
      if (p.Z < this.MapBound.Max.Z)
      {
        MapBlock blockIdAndAux = this.GetBlockIDAndAux(p);
        if (this.IsBlockLiquid(blockIdAndAux.BlockID) && ((int) blockIdAndAux.AuxData & 7) < 7)
          return true;
      }
      return false;
    }

    public BlastResult CreateBlast(
      GlobalPoint3D p,
      float blastStrength,
      int blastRadius,
      PcgRandom rand,
      UpdateBlockMethod method,
      bool buildPointsOnly,
      GamerID playerID,
      ushort seed)
    {
      if (rand == null)
        rand = new PcgRandom((int) seed);
      else
        rand.Seed((int) seed);
      BlastResult result = new BlastResult()
      {
        BuildPointsOnly = buildPointsOnly
      };
      if (buildPointsOnly)
        result.PointsCleared = new List<GlobalPoint3D>(400);
      result.LowestY = int.MaxValue;
      for (int index = 0; index < blastRadius; ++index)
      {
        this.CreateBlastPlaneFast(p, p + GlobalPoint3D.Down * index, blastStrength, blastRadius, rand, method, playerID, ref result);
        if (index > 0)
          this.CreateBlastPlaneFast(p, p + GlobalPoint3D.Up * index, blastStrength, blastRadius, rand, method, playerID, ref result);
      }
      return result;
    }

    private void CreateBlastPlaneFast(
      GlobalPoint3D blastOrigin,
      GlobalPoint3D p,
      float blastStrength,
      int blastRadius,
      PcgRandom rand,
      UpdateBlockMethod method,
      GamerID playerID,
      ref BlastResult result)
    {
      if (p.Y >= this.MapBound.Max.Y || p.Y <= this.MapBound.Min.Y)
        return;
      float num1 = (float) (blastRadius * blastRadius);
      bool flag1 = method == UpdateBlockMethod.Generation;
      bool bordersLiquidCheck = flag1 && this.Random.Next(20) != 0;
      Vector3 vector3_1 = new Vector3((float) blastOrigin.X, (float) blastOrigin.Y, (float) blastOrigin.Z);
      Vector3 vector3_2 = vector3_1;
      vector3_2.Y = (float) p.Y;
      bool flag2 = flag1;
      if (!flag2)
      {
        float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
        if ((double) num2 == 0.0)
          num2 = 1f;
        flag2 = (double) this.GetBlastResistance(this.GetBlockID(p)) < (double) blastStrength / (double) num2;
      }
      if (flag2)
      {
        if (result.BuildPointsOnly)
        {
          result.PointsCleared.Add(p);
        }
        else
        {
          int num2 = (int) this.ClearBlock(p, method, playerID, false, false, bordersLiquidCheck);
        }
        if (p.Y < result.LowestY)
          result.LowestY = p.Y;
      }
      for (int index = 1; index <= blastRadius; ++index)
      {
        GlobalPoint3D p1 = p;
        p1.X = p.X - index;
        vector3_2.X = (float) p1.X;
        if (p1.X >= this.MapBound.Min.X)
        {
          for (p1.Z = p.Z - index; p1.Z <= p.Z + index && p1.Z < this.MapBound.Max.Z - 1; ++p1.Z)
          {
            if (p1.Z > 0 && (index < blastRadius - 1 || rand.Next(4) > 0))
            {
              vector3_2.Z = (float) p1.Z;
              float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
              if ((double) num2 <= (double) num1 && (flag1 || (double) this.GetBlastResistance(this.GetBlockID(p1)) < (double) blastStrength / (double) num2))
              {
                if (result.BuildPointsOnly)
                {
                  result.PointsCleared.Add(p1);
                }
                else
                {
                  int num3 = (int) this.ClearBlock(p1, method, playerID, false, false, bordersLiquidCheck);
                  if ((int) num2 == (int) num1)
                    this.BlastEdgeCleared(p1, method, playerID);
                }
                if (p1.Y < result.LowestY)
                  result.LowestY = p1.Y;
              }
            }
          }
        }
        --p1.Z;
        vector3_2.Z = (float) p1.Z;
        if (p1.Z >= this.MapBound.Min.Z)
        {
          for (; p1.X <= p.X + index && p1.X < this.MapBound.Max.X - 1; ++p1.X)
          {
            if (p1.X > 0 && (index < blastRadius - 1 || rand.Next(4) > 0))
            {
              ushort blastResistance = this.GetBlastResistance(this.GetBlockID(p1));
              if (flag1 || (double) blastResistance < (double) blastStrength)
              {
                vector3_2.X = (float) p1.X;
                float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
                if ((double) num2 <= (double) num1 && (flag1 || (double) blastResistance < (double) blastStrength / (double) num2))
                {
                  if (result.BuildPointsOnly)
                  {
                    result.PointsCleared.Add(p1);
                  }
                  else
                  {
                    int num3 = (int) this.ClearBlock(p1, method, playerID, false, false, bordersLiquidCheck);
                    if ((int) num2 == (int) num1)
                      this.BlastEdgeCleared(p1, method, playerID);
                  }
                  if (p1.Y < result.LowestY)
                    result.LowestY = p1.Y;
                }
              }
            }
          }
        }
        --p1.X;
        vector3_2.X = (float) p1.X;
        if (p1.X >= this.MapBound.Min.X)
        {
          for (; p1.Z > p.Z - index && p1.Z > 0; --p1.Z)
          {
            if (p1.Z < this.MapBound.Max.Z - 1 && (index < blastRadius - 1 || rand.Next(4) > 0))
            {
              ushort blastResistance = this.GetBlastResistance(this.GetBlockID(p1));
              if (flag1 || (double) blastResistance < (double) blastStrength)
              {
                vector3_2.Z = (float) p1.Z;
                float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
                if ((double) num2 <= (double) num1 && (flag1 || (double) blastResistance < (double) blastStrength / (double) num2))
                {
                  if (result.BuildPointsOnly)
                  {
                    result.PointsCleared.Add(p1);
                  }
                  else
                  {
                    int num3 = (int) this.ClearBlock(p1, method, playerID, false, false, bordersLiquidCheck);
                    if ((int) num2 == (int) num1)
                      this.BlastEdgeCleared(p1, method, playerID);
                  }
                  if (p1.Y < result.LowestY)
                    result.LowestY = p1.Y;
                }
              }
            }
          }
        }
        for (; p1.X > p.X - index && p1.X > 0; --p1.X)
        {
          if (p1.X < this.MapBound.Max.X - 1 && (index < blastRadius - 1 || rand.Next(4) > 0))
          {
            ushort blastResistance = this.GetBlastResistance(this.GetBlockID(p1));
            if (flag1 || (double) blastResistance < (double) blastStrength)
            {
              vector3_2.X = (float) p1.X;
              float num2 = Vector3.DistanceSquared(vector3_1, vector3_2);
              if ((double) num2 <= (double) num1 && (flag1 || (double) blastResistance < (double) blastStrength / (double) num2))
              {
                if (result.BuildPointsOnly)
                {
                  result.PointsCleared.Add(p1);
                }
                else
                {
                  int num3 = (int) this.ClearBlock(p1, method, playerID, false, false, bordersLiquidCheck);
                  if ((int) num2 == (int) num1)
                    this.BlastEdgeCleared(p1, method, playerID);
                }
                if (p1.Y < result.LowestY)
                  result.LowestY = p1.Y;
              }
            }
          }
        }
      }
    }

    protected virtual void BlastEdgeCleared(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID playerID)
    {
    }

    public void GetPointsOfPenetration(Vector3 min, Vector3 max)
    {
      Vector3 zero = Vector3.Zero;
      float num1 = (float) this.MapBound.Min.X * this.TileSize;
      float num2 = (float) this.MapBound.Min.Y * this.TileSize;
      float num3 = (float) this.MapBound.Min.Z * this.TileSize;
      if ((double) min.X < (double) num1)
      {
        min.X = num1;
        if ((double) max.X < (double) num1)
          max.X = num1;
      }
      if ((double) min.Y < (double) num2)
        min.Y = num2;
      if ((double) max.Y < (double) num2)
        max.Y = num2;
      if ((double) min.Z < (double) num3)
      {
        min.Z = num3;
        if ((double) max.Z < (double) num3)
          max.Z = num3;
      }
      float num4 = (float) (this.MapBound.Max.X - 1) * this.TileSize;
      float num5 = (float) (this.MapBound.Max.Z - 1) * this.TileSize;
      if ((double) max.X > (double) num4)
      {
        max.X = num4;
        if ((double) min.X > (double) num4)
          min.X = num4;
      }
      if ((double) max.Z > (double) num5)
      {
        max.Z = num5;
        if ((double) min.Z > (double) num5)
          min.Z = num5;
      }
      this.PointsOfPenetration.Clear();
      for (float y = min.Y; (double) y < (double) max.Y + (double) this.TileSize; y += this.TileSize)
      {
        zero.Y = y;
        if ((double) zero.Y > (double) max.Y)
          zero.Y = max.Y;
        for (float z = min.Z; (double) z < (double) max.Z + (double) this.TileSize; z += this.TileSize)
        {
          zero.Z = z;
          if ((double) zero.Z > (double) max.Z)
            zero.Z = max.Z;
          for (float x = min.X; (double) x < (double) max.X + (double) this.TileSize; x += this.TileSize)
          {
            zero.X = x;
            if ((double) zero.X > (double) max.X)
              zero.X = max.X;
            GlobalPoint3D point = this.GetPoint(zero);
            if (!this.PointsOfPenetration.Contains(point))
              this.PointsOfPenetration.Add(point);
          }
        }
      }
    }

    public void PregenerateRegions(bool isGenerated, bool useHeightMap, IProgressBar progress)
    {
      int capacity = 0;
      if (progress != null)
      {
        capacity = (this.MapSize.X + this.RegionSize.X - 1) / this.RegionSize.X * ((this.MapSize.Y + this.RegionSize.Y - 1) / this.RegionSize.Y) * ((this.MapSize.Z + this.RegionSize.Z - 1) / this.RegionSize.Z);
        progress.Factor /= (float) capacity;
      }
      GlobalPoint3D p = new GlobalPoint3D();
      if (this.IsInfinite)
      {
        this.swapRegions = new Dictionary<int, MapRegion>(capacity);
        this.swapHeightMaps = new Dictionary<int, MapHeightmap>();
        int num = this.regionsPerX / 2;
        for (p.Y = this.MapBound.Min.Y; p.Y < this.MapBound.Max.Y; p.Y += this.RegionSize.Y)
        {
          for (p.Z = this.MapCenter.Z - this.RegionSize.Z * num; p.Z < this.MapCenter.Z + this.RegionSize.Z * num; p.Z += this.RegionSize.Z)
          {
            for (p.X = this.MapCenter.X - this.RegionSize.X * num; p.X < this.MapCenter.X + this.RegionSize.X * num; p.X += this.RegionSize.X)
              this.AddRegion(p, isGenerated, false, useHeightMap, progress);
          }
        }
      }
      else
      {
        for (p.Y = this.MapBound.Min.Y; p.Y < this.MapBound.Max.Y; p.Y += this.RegionSize.Y)
        {
          for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
          {
            for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
              this.AddRegion(p, isGenerated, false, useHeightMap, progress);
          }
        }
      }
      if (progress == null)
        return;
      progress.Factor *= (float) capacity;
    }

    public void AddRegion(
      GlobalPoint3D p,
      bool isGenerated,
      bool ignoreInsideMapTest,
      bool useHeightMap,
      IProgressBar progress)
    {
      if (this.GetRegion(p) != null)
        return;
      MapRegion mapRegion = this.AddRegionCore(p, isGenerated, ignoreInsideMapTest, progress);
      this.Regions.Add(this.GetRegionHashCode(p), mapRegion);
      if (!useHeightMap)
        return;
      mapRegion.HeightMap = this.AddHeightMap(p);
      p.Y = 0;
      int regionHashCode = this.GetRegionHashCode(p);
      if (this.HeightMaps.ContainsKey(regionHashCode))
        return;
      this.HeightMaps.Add(regionHashCode, mapRegion.HeightMap);
    }

    private MapRegion AddRegionCore(
      GlobalPoint3D p,
      bool isGenerated,
      bool ignoreInsideMapTest,
      IProgressBar progress)
    {
      bool recycled;
      MapRegion newRegion = this.GetNewRegion(out recycled);
      newRegion.Initialize(this, p);
      if (!recycled)
        newRegion.Pregenerate(isGenerated, ignoreInsideMapTest, progress);
      else
        newRegion.RegenerateChunks(newRegion.Offset);
      return newRegion;
    }

    protected virtual MapRegion CreateRegion()
    {
      return new MapRegion();
    }

    private MapHeightmap GetNewHeightMap()
    {
      if (this.releasedHeightmaps.Count <= 0)
        return new MapHeightmap();
      MapHeightmap releasedHeightmap = this.releasedHeightmaps[this.releasedHeightmaps.Count - 1];
      this.releasedHeightmaps.RemoveAt(this.releasedHeightmaps.Count - 1);
      return releasedHeightmap;
    }

    private MapRegion GetNewRegion(out bool recycled)
    {
      if (this.releasedRegions.Count > 0)
      {
        MapRegion releasedRegion = this.releasedRegions[this.releasedRegions.Count - 1];
        this.releasedRegions.RemoveAt(this.releasedRegions.Count - 1);
        recycled = true;
        return releasedRegion;
      }
      recycled = false;
      return this.CreateRegion();
    }

    public MapRegion GetRegion(GlobalPoint3D p)
    {
      if (p.Y < 0)
        return (MapRegion) null;
      int regionHashCode = this.GetRegionHashCode(p);
      MapRegion mapRegion = (MapRegion) null;
      if (this.Regions != null)
        this.Regions.TryGetValue(regionHashCode, out mapRegion);
      return mapRegion;
    }

    public int GetRegionHashCode(GlobalPoint3D p)
    {
      int num1 = 0;
      p.Y /= this.RegionSize.Y;
      int num2;
      if (p.X < 0)
      {
        ++p.X;
        num2 = -p.X / this.RegionSize.X + 1;
        num1 |= 1073741824;
      }
      else
        num2 = p.X / this.RegionSize.X;
      int num3;
      if (p.Z < 0)
      {
        ++p.Z;
        num3 = -p.Z / this.RegionSize.Z + 1;
        num1 |= 536870912;
      }
      else
        num3 = p.Z / this.RegionSize.Z;
      return num1 + ((num2 & 2047) << 18) + ((num3 & 2047) << 7) + (p.Y & (int) sbyte.MaxValue);
    }

    public GlobalPoint3D GetRegionOffset(int hash)
    {
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      globalPoint3D.X = hash >> 18 & 2047;
      globalPoint3D.Z = hash >> 7 & 2047;
      globalPoint3D.Y = hash & (int) sbyte.MaxValue;
      globalPoint3D.X *= this.RegionSize.X;
      globalPoint3D.Y *= this.RegionSize.Y;
      globalPoint3D.Z *= this.RegionSize.Z;
      if ((hash & 1073741824) > 0)
        globalPoint3D.X = -globalPoint3D.X;
      if ((hash & 536870912) > 0)
        globalPoint3D.Z = -globalPoint3D.Z;
      return globalPoint3D;
    }

    private bool HaveRegionInSameHeightMapSpace(MapRegion region)
    {
      foreach (KeyValuePair<int, MapRegion> region1 in this.Regions)
      {
        MapRegion mapRegion = region1.Value;
        if (mapRegion.Offset.X == region.Offset.X && mapRegion.Offset.Z == region.Offset.Z)
          return true;
      }
      return false;
    }

    public long GetGlobalHashCode(GlobalPoint3D p)
    {
      MapRegion region = this.GetRegion(p);
      if (region == null)
        return 0;
      GlobalPoint3D globalPoint3D = p - region.Offset;
      return ((long) this.GetRegionHashCode(region.Offset) << 32) + (long) globalPoint3D.GetHashCode();
    }

    public long GetChunkGlobalHashCode(MapChunk chunk)
    {
      if (chunk == null)
        return 0;
      return chunk.GetGlobalHashCode();
    }

    public MapChunk GetChunk(GlobalPoint3D p)
    {
      return this.GetRegion(p)?.GetChunk(p);
    }

    public MapChunk GetChunk(Vector3 pos)
    {
      GlobalPoint3D point = this.GetPoint(pos);
      return this.GetRegion(point)?.GetChunk(point);
    }

    public MapChunk GetChunk(long hash)
    {
      return this.GetChunk(this.GetRegionOffset((int) (hash >> 32)) + GlobalPoint3D.FromHashCode((int) hash));
    }

    public void GetChunks(BoundingBox box, Dictionary<long, MapChunk> result)
    {
      GlobalPoint3D min;
      GlobalPoint3D max;
      this.GetBoxMinMax(box, out min, out max);
      this.GetChunks(min, max, result);
    }

    public void GetChunks(BoxInt bound, Dictionary<long, MapChunk> result)
    {
      this.GetChunks(bound.Min, bound.Max, result);
    }

    public void GetChunks(GlobalPoint3D min, GlobalPoint3D max, List<long> result)
    {
      lock (this.tempHashList)
      {
        this.GetChunks(min, max, this.tempHashList);
        foreach (KeyValuePair<long, MapChunk> tempHash in this.tempHashList)
          result.Add(tempHash.Key);
        this.tempHashList.Clear();
      }
    }

    public void GetChunks(GlobalPoint3D min, GlobalPoint3D max, Dictionary<long, MapChunk> result)
    {
      if (result == null || !this.ValidateMinMax(ref min, ref max))
        return;
      GlobalPoint3D p = new GlobalPoint3D();
      for (p.Y = min.Y; p.Y < max.Y; p.Y += this.ChunkSize.Y)
        this.GetChunksZ(p, min, max, result);
      p.Y = max.Y;
      this.GetChunksZ(p, min, max, result);
    }

    private void GetChunksZ(
      GlobalPoint3D p,
      GlobalPoint3D min,
      GlobalPoint3D max,
      Dictionary<long, MapChunk> result)
    {
      for (p.Z = min.Z; p.Z < max.Z; p.Z += this.ChunkSize.Z)
        this.GetChunksX(p, min, max, result);
      p.Z = max.Z;
      this.GetChunksX(p, min, max, result);
    }

    private void GetChunksX(
      GlobalPoint3D p,
      GlobalPoint3D min,
      GlobalPoint3D max,
      Dictionary<long, MapChunk> result)
    {
      for (p.X = min.X; p.X < max.X; p.X += this.ChunkSize.X)
      {
        MapChunk chunk = this.GetChunk(p);
        if (chunk != null)
        {
          long chunkGlobalHashCode = this.GetChunkGlobalHashCode(chunk);
          if (!result.ContainsKey(chunkGlobalHashCode))
            result.Add(chunkGlobalHashCode, chunk);
        }
      }
      p.X = max.X;
      MapChunk chunk1 = this.GetChunk(p);
      if (chunk1 == null)
        return;
      long chunkGlobalHashCode1 = this.GetChunkGlobalHashCode(chunk1);
      if (result.ContainsKey(chunkGlobalHashCode1))
        return;
      result.Add(chunkGlobalHashCode1, chunk1);
    }

    public MapChunk GetChunkWithinDepth(Vector2 depth)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = this.Random.Next(this.MapSize.X - 2) + 1 + this.MapBound.Min.X;
      p.Z = this.Random.Next(this.MapSize.Z - 2) + 1 + this.MapBound.Min.Z;
      int mapHeight = this.MapHeight;
      int max = (double) depth.X == 0.0 ? mapHeight : mapHeight - (int) ((double) mapHeight * (double) depth.X);
      int min = mapHeight - (int) ((double) mapHeight * (double) depth.Y);
      p.Y = this.Random.Next(min, max);
      return this.GetChunk(p);
    }

    private void GetBoxMinMax(BoundingBox box, out GlobalPoint3D min, out GlobalPoint3D max)
    {
      min = new GlobalPoint3D();
      if ((double) this.TileSize == 1.0)
      {
        min.X = (int) box.Min.X;
        min.Y = (int) box.Min.Y;
        min.Z = (int) box.Min.Z;
      }
      else
      {
        min.X = (int) ((double) box.Min.X / (double) this.TileSize);
        min.Y = (int) ((double) box.Min.Y / (double) this.TileSize);
        min.Z = (int) ((double) box.Min.Z / (double) this.TileSize);
      }
      Vector3 vector3 = new Vector3();
      vector3.X = box.Max.X / this.TileSize;
      vector3.Y = box.Max.Y / this.TileSize;
      vector3.Z = box.Max.Z / this.TileSize;
      if ((double) (int) vector3.X == (double) vector3.X && (double) vector3.X > (double) min.X)
        --vector3.X;
      if ((double) (int) vector3.Y == (double) vector3.Y && (double) vector3.Y > (double) min.Y)
        --vector3.Y;
      if ((double) (int) vector3.Z == (double) vector3.Z && (double) vector3.Z > (double) min.Z)
        --vector3.Z;
      max = new GlobalPoint3D();
      max.X = (int) vector3.X;
      max.Y = (int) vector3.Y;
      max.Z = (int) vector3.Z;
    }

    public bool ValidateMinMax(ref GlobalPoint3D min, ref GlobalPoint3D max)
    {
      if (min.X > max.X)
      {
        int x = max.X;
        max.X = min.X;
        min.X = x;
      }
      if (min.Y > max.Y)
      {
        int y = max.Y;
        max.Y = min.Y;
        min.Y = y;
      }
      if (min.Z > max.Z)
      {
        int z = max.Z;
        max.Z = min.Z;
        min.Z = z;
      }
      if (min.X >= this.MapBound.Max.X || max.X < this.MapBound.Min.X || (min.Y >= this.MapBound.Max.Y || max.Y < this.MapBound.Min.Y) || (min.Z >= this.MapBound.Max.Z || max.Z < this.MapBound.Min.Z))
        return false;
      min = GlobalPoint3D.Clamp(this.MapBound.Min, this.MapBound.Max, min);
      max = GlobalPoint3D.Clamp(this.MapBound.Min, this.MapBound.Max, max);
      long globalHashCode = this.GetGlobalHashCode(min);
      GlobalPoint3D regionOffset = this.GetRegionOffset((int) (globalHashCode >> 32));
      min = regionOffset + Point3D.FromHash((int) globalHashCode);
      return true;
    }

    public MapHeightmap GetHeightMap(GlobalPoint3D p)
    {
      p.Y = 0;
      MapHeightmap mapHeightmap;
      this.HeightMaps.TryGetValue(this.GetRegionHashCode(p), out mapHeightmap);
      return mapHeightmap;
    }

    public ushort GetHeight(GlobalPoint3D p)
    {
      MapHeightmap heightMap = this.GetHeightMap(p);
      if (heightMap != null)
        return heightMap.GetHeight(p.X, p.Z);
      return (ushort) this.MapBound.Max.Y;
    }

    public ushort GetHeightForLighting(GlobalPoint3D p)
    {
      MapHeightmap heightMap = this.GetHeightMap(p);
      if (heightMap != null)
        return heightMap.GetHeightForLighting(p.X, p.Z);
      return (ushort) this.MapBound.Max.Y;
    }

    protected MapHeightmap AddHeightMap(GlobalPoint3D p)
    {
      MapHeightmap mapHeightmap = this.GetHeightMap(p);
      if (mapHeightmap == null)
      {
        mapHeightmap = this.GetNewHeightMap();
        mapHeightmap.Initialize(this, p, (ushort) Math.Min(this.MapBound.Max.Y - 2, (int) this.SeaLevel));
      }
      return mapHeightmap;
    }

    public void SetHeight(GlobalPoint3D from, GlobalPoint3D to)
    {
      GlobalPoint3D p = from;
      p.Y = to.Y;
      for (; p.Z <= to.Z; ++p.Z)
      {
        for (p.X = from.X; p.X <= to.X; ++p.X)
          this.SetHeight(p, (ushort) p.Y);
      }
    }

    public void SetHeight(GlobalPoint3D p, ushort h1)
    {
      this.GetHeightMap(p)?.SetHeight(p.X, p.Z, (ushort) p.Y, h1);
    }

    public virtual bool IsBlockAffectSunlightForHeightCalculation(byte blockID)
    {
      return blockID != (byte) 0;
    }

    public void AdjustHeightMapInternal(
      MapRegion region,
      GlobalPoint3D p,
      MapBlock newBlockData,
      UpdateBlockMethod method,
      bool commitChunk)
    {
      if (region.HeightMap == null)
        return;
      byte blockId1 = newBlockData.BlockID;
      if (blockId1 > (byte) 0 && (int) blockId1 != (int) this.InvisibleBarrierID)
      {
        ushort height = region.HeightMap.GetHeight(p.X, p.Z);
        ushort heightForLighting = region.HeightMap.GetHeightForLighting(p.X, p.Z);
        ushort h = height;
        ushort num1 = heightForLighting;
        if ((int) height < p.Y)
          h = (ushort) p.Y;
        if ((int) heightForLighting < p.Y && this.IsBlockAffectSunlightForHeightCalculation(blockId1))
          num1 = (ushort) p.Y;
        if ((int) height == (int) h && (int) heightForLighting == (int) num1)
          return;
        newBlockData.Chunk.Region.SetHeight(p.X, p.Z, h, num1);
        if (!commitChunk || (int) heightForLighting == (int) num1)
          return;
        ushort num2 = Math.Max(heightForLighting, num1);
        ushort num3 = Math.Min(heightForLighting, num1);
        for (p.Y = (int) num2; p.Y >= (int) num3; p.Y -= this.ChunkSize.Y)
        {
          MapChunk chunk = this.GetChunk(p);
          if (chunk != newBlockData.Chunk)
          {
            newBlockData.Chunk = chunk;
            this.AddChunkToCommitList(chunk, method);
          }
        }
      }
      else
      {
        ushort height = region.HeightMap.GetHeight(p.X, p.Z);
        ushort heightForLighting = region.HeightMap.GetHeightForLighting(p.X, p.Z);
        ushort h = height;
        ushort h1 = heightForLighting;
        bool flag1 = (int) height == p.Y;
        bool flag2 = (int) heightForLighting == p.Y;
        if (!flag1 && !flag2)
          return;
        while (--p.Y > this.MapBound.Min.Y && (flag1 || flag2))
        {
          byte blockId2 = this.GetBlockID(p);
          bool flag3 = blockId2 == (byte) 0 || (int) blockId2 == (int) this.InvisibleBarrierID;
          bool flag4 = flag3 || !this.IsBlockAffectSunlightForHeightCalculation(blockId2);
          if (flag4 && commitChunk)
          {
            MapChunk chunk = this.GetChunk(p);
            if (chunk != newBlockData.Chunk)
            {
              newBlockData.Chunk = chunk;
              this.AddChunkToCommitList(chunk, method);
            }
          }
          if (!flag3 && flag1)
          {
            h = (ushort) p.Y;
            flag1 = false;
          }
          if (!flag4 && flag2)
          {
            h1 = (ushort) p.Y;
            flag2 = false;
          }
        }
        if (commitChunk)
        {
          MapChunk chunk = this.GetChunk(p);
          if (chunk != newBlockData.Chunk)
          {
            newBlockData.Chunk = chunk;
            this.AddChunkToCommitList(chunk, method);
          }
        }
        if ((int) height == (int) h && (int) heightForLighting == (int) h1)
          return;
        newBlockData.Chunk.Region.SetHeight(p.X, p.Z, h, h1);
      }
    }

    public int MostCommonHeight(Point range)
    {
      int num1 = 0;
      int num2 = 0;
      int[] numArray = new int[range.Y + 1];
      foreach (KeyValuePair<int, MapRegion> region in this.Regions)
      {
        MapHeightmap heightMap = region.Value.HeightMap;
        if (heightMap != null)
        {
          for (int index = 0; index < heightMap.HeightMap.Length; ++index)
          {
            int height = (int) heightMap.HeightMap[index];
            if (height >= range.X && height <= range.Y)
            {
              ++numArray[height];
              if (numArray[height] > num2)
              {
                num2 = numArray[height];
                num1 = height;
              }
            }
          }
        }
      }
      return num1;
    }

    public void UpdateHeightData(MapChunk chunk, bool isAir)
    {
      MapHeightmap heightMap = chunk.Region.HeightMap;
      if (heightMap == null)
        return;
      Point3D offset = chunk.Offset;
      GlobalPoint3D globalOffset = chunk.GlobalOffset;
      GlobalPoint3D point = new GlobalPoint3D();
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
      for (int index1 = 0; index1 < this.ChunkSize.Z; ++index1)
      {
        point.Z = offset.Z + index1;
        globalPoint3D1.Z = globalOffset.Z + index1;
        for (int index2 = 0; index2 < this.ChunkSize.X; ++index2)
        {
          point.X = offset.X + index2;
          globalPoint3D1.X = globalOffset.X + index2;
          ushort heightLocal = heightMap.GetHeightLocal(point.X, point.Z);
          ushort localForLighting = heightMap.GetHeightLocalForLighting(point.X, point.Z);
          if ((int) heightLocal >= globalOffset.Y || (int) localForLighting >= globalOffset.Y || !isAir)
          {
            int num1 = globalOffset.Y + this.ChunkSize.Y;
            if ((int) heightLocal < num1 || (int) localForLighting < num1)
            {
              ushort h = heightLocal;
              ushort h1 = localForLighting;
              bool flag1 = false;
              bool flag2 = false;
              if ((int) heightLocal >= globalOffset.Y && (int) heightLocal < num1)
                h = globalOffset.Y == 0 ? (ushort) 0 : (ushort) (globalOffset.Y - 1);
              if ((int) localForLighting >= globalOffset.Y && (int) localForLighting < num1)
                h1 = globalOffset.Y == 0 ? (ushort) 0 : (ushort) (globalOffset.Y - 1);
              for (int index3 = this.ChunkSize.Y - 1; index3 >= 0; --index3)
              {
                point.Y = offset.Y + index3;
                globalPoint3D1.Y = globalOffset.Y + index3;
                int mapIndex = chunk.GetMapIndex(point);
                byte blockId = chunk.GetBlockID(mapIndex);
                if (blockId > (byte) 0 && (int) blockId != (int) this.InvisibleBarrierID)
                {
                  if (globalPoint3D1.Y > (int) h)
                  {
                    h = (ushort) globalPoint3D1.Y;
                    flag1 = true;
                  }
                  if (globalPoint3D1.Y > (int) h1 && this.IsBlockAffectSunlightForHeightCalculation(blockId))
                  {
                    h1 = (ushort) globalPoint3D1.Y;
                    flag2 = true;
                  }
                  if (flag1 && flag2)
                    break;
                }
              }
              if ((int) h != (int) heightLocal || (int) h1 != (int) localForLighting)
              {
                if ((int) h < globalOffset.Y || (int) h1 < globalOffset.Y)
                {
                  if ((int) h < globalOffset.Y)
                    h = (ushort) 0;
                  if ((int) h1 < globalOffset.Y)
                    h1 = (ushort) 0;
                  GlobalPoint3D globalPoint3D2 = globalPoint3D1;
                  --globalPoint3D2.Y;
                  bool flag3 = true;
                  MapChunk chunk1 = this.GetChunk(globalPoint3D2);
                  while (flag3 && chunk1 != null && chunk1.IsGenerated)
                  {
                    if ((int) h == globalPoint3D2.Y)
                      h = globalPoint3D2.Y > this.ChunkSize.Y ? (ushort) (globalPoint3D2.Y - this.ChunkSize.Y) : (ushort) 0;
                    if ((int) h1 == globalPoint3D2.Y)
                      h1 = globalPoint3D2.Y > this.ChunkSize.Y ? (ushort) (globalPoint3D2.Y - this.ChunkSize.Y) : (ushort) 0;
                    int num2 = 0;
                    while (num2 < this.ChunkSize.Y)
                    {
                      int mapIndex = chunk1.GetMapIndex(globalPoint3D2);
                      byte blockId = chunk1.GetBlockID(mapIndex);
                      if (blockId > (byte) 0 && (int) blockId != (int) this.InvisibleBarrierID)
                      {
                        if (globalPoint3D2.Y > (int) h)
                          h = (ushort) globalPoint3D2.Y;
                        if (globalPoint3D2.Y > (int) h1 && this.IsBlockAffectSunlightForHeightCalculation(blockId))
                          h1 = (ushort) globalPoint3D2.Y;
                        if ((int) h >= globalPoint3D2.Y && (int) h1 >= globalPoint3D2.Y)
                        {
                          flag3 = false;
                          break;
                        }
                      }
                      ++num2;
                      --globalPoint3D2.Y;
                    }
                    if (flag3)
                      chunk1 = this.GetChunk(globalPoint3D2);
                  }
                }
                heightMap.SetHeight(globalPoint3D1.X, globalPoint3D1.Z, h, h1);
              }
            }
          }
        }
      }
    }

    private void SaveRegion(MapRegion region)
    {
      this.SaveRegionCore(region);
      region.UnloadForRecycle();
      this.releasedRegions.Add(region);
    }

    protected virtual void SaveRegionCore(MapRegion region)
    {
    }

    protected virtual void LoadRegionCore(MapRegion region)
    {
    }

    private void SaveHeightMap(MapHeightmap hgtmap)
    {
      this.releasedHeightmaps.Add(hgtmap);
    }

    protected virtual void OnMapShiftBegin(BlockFace direction)
    {
    }

    protected virtual void OnMapShiftEnd(BlockFace direction)
    {
    }

    public void ShiftLeft()
    {
      if (this.MapBound.Min.X - this.RegionSize.X < -this.MapMaxSize.X)
        return;
      this.OnMapShiftBegin(BlockFace.Left);
      GlobalPoint3D p = new GlobalPoint3D();
      lock (this.shiftLock)
      {
        this.swapHeightMaps.Clear();
        for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
        {
          for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
          {
            MapHeightmap heightMap = this.GetHeightMap(p);
            if (p.X == this.MapBound.Max.X - this.RegionSize.X)
              this.discardedHeightMapsOnShift.Add(heightMap);
            else
              this.swapHeightMaps.Add(this.GetRegionHashCode(heightMap.Offset), heightMap);
          }
          p.X = this.MapBound.Min.X - this.RegionSize.X;
          MapHeightmap mapHeightmap = this.AddHeightMap(p);
          this.swapHeightMaps.Add(this.GetRegionHashCode(mapHeightmap.Offset), mapHeightmap);
        }
        this.swapRegions.Clear();
        for (p.Y = this.MapBound.Min.Y; p.Y < this.MapBound.Max.Y; p.Y += this.RegionSize.Y)
        {
          for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
          {
            for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
            {
              MapRegion region = this.GetRegion(p);
              if (p.X == this.MapBound.Max.X - this.RegionSize.X)
                this.discardedRegionsOnShift.Add(region);
              else
                this.swapRegions.Add(this.GetRegionHashCode(region.Offset), region);
            }
            p.X = this.MapBound.Min.X - this.RegionSize.X;
            MapRegion region1 = this.AddRegionCore(p, false, true, (IProgressBar) null);
            region1.HeightMap = this.GetHeightMapFromSwap(p);
            this.LoadRegionCore(region1);
            this.swapRegions.Add(this.GetRegionHashCode(region1.Offset), region1);
          }
        }
        Dictionary<int, MapRegion> regions = this.Regions;
        Dictionary<int, MapHeightmap> heightMaps = this.HeightMaps;
        this.Regions = this.swapRegions;
        this.HeightMaps = this.swapHeightMaps;
        this.MapBound.Min.X -= this.RegionSize.X;
        this.MapBound.Max.X -= this.RegionSize.X;
        this.MapCenter.X -= this.RegionSize.X;
        this.swapRegions = regions;
        this.swapHeightMaps = heightMaps;
        foreach (MapRegion region in this.discardedRegionsOnShift)
          this.SaveRegion(region);
        foreach (MapHeightmap hgtmap in this.discardedHeightMapsOnShift)
          this.SaveHeightMap(hgtmap);
        this.discardedRegionsOnShift.Clear();
        this.discardedHeightMapsOnShift.Clear();
        this.OnMapShiftEnd(BlockFace.Left);
      }
    }

    public void ShiftForward()
    {
      if (this.MapBound.Min.Z - this.RegionSize.Z < -this.MapMaxSize.Z)
        return;
      this.OnMapShiftBegin(BlockFace.Forward);
      GlobalPoint3D p = new GlobalPoint3D();
      lock (this.shiftLock)
      {
        this.swapHeightMaps.Clear();
        for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
        {
          for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
          {
            MapHeightmap heightMap = this.GetHeightMap(p);
            if (p.Z == this.MapBound.Max.Z - this.RegionSize.Z)
              this.discardedHeightMapsOnShift.Add(heightMap);
            else
              this.swapHeightMaps.Add(this.GetRegionHashCode(heightMap.Offset), heightMap);
          }
          p.Z = this.MapBound.Min.Z - this.RegionSize.Z;
          MapHeightmap mapHeightmap = this.AddHeightMap(p);
          this.swapHeightMaps.Add(this.GetRegionHashCode(mapHeightmap.Offset), mapHeightmap);
        }
        this.swapRegions.Clear();
        for (p.Y = this.MapBound.Min.Y; p.Y < this.MapBound.Max.Y; p.Y += this.RegionSize.Y)
        {
          for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
          {
            for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
            {
              MapRegion region = this.GetRegion(p);
              if (p.Z == this.MapBound.Max.Z - this.RegionSize.Z)
                this.discardedRegionsOnShift.Add(region);
              else
                this.swapRegions.Add(this.GetRegionHashCode(region.Offset), region);
            }
            p.Z = this.MapBound.Min.Z - this.RegionSize.Z;
            MapRegion region1 = this.AddRegionCore(p, false, true, (IProgressBar) null);
            region1.HeightMap = this.GetHeightMapFromSwap(p);
            this.LoadRegionCore(region1);
            this.swapRegions.Add(this.GetRegionHashCode(region1.Offset), region1);
          }
        }
        Dictionary<int, MapRegion> regions = this.Regions;
        Dictionary<int, MapHeightmap> heightMaps = this.HeightMaps;
        this.Regions = this.swapRegions;
        this.HeightMaps = this.swapHeightMaps;
        this.MapBound.Min.Z -= this.RegionSize.Z;
        this.MapBound.Max.Z -= this.RegionSize.Z;
        this.MapCenter.Z -= this.RegionSize.Z;
        this.swapRegions = regions;
        this.swapHeightMaps = heightMaps;
        foreach (MapRegion region in this.discardedRegionsOnShift)
          this.SaveRegion(region);
        foreach (MapHeightmap hgtmap in this.discardedHeightMapsOnShift)
          this.SaveHeightMap(hgtmap);
        this.discardedRegionsOnShift.Clear();
        this.discardedHeightMapsOnShift.Clear();
        this.OnMapShiftEnd(BlockFace.Forward);
      }
    }

    public void ShiftRight()
    {
      if (this.MapBound.Max.X >= this.MapMaxSize.X)
        return;
      this.OnMapShiftBegin(BlockFace.Right);
      GlobalPoint3D p = new GlobalPoint3D();
      lock (this.shiftLock)
      {
        this.swapHeightMaps.Clear();
        for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
        {
          for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
          {
            MapHeightmap heightMap = this.GetHeightMap(p);
            if (p.X == this.MapBound.Min.X)
              this.discardedHeightMapsOnShift.Add(heightMap);
            else
              this.swapHeightMaps.Add(this.GetRegionHashCode(heightMap.Offset), heightMap);
          }
          p.X = this.MapBound.Max.X;
          MapHeightmap mapHeightmap = this.AddHeightMap(p);
          this.swapHeightMaps.Add(this.GetRegionHashCode(mapHeightmap.Offset), mapHeightmap);
        }
        this.swapRegions.Clear();
        for (p.Y = this.MapBound.Min.Y; p.Y < this.MapBound.Max.Y; p.Y += this.RegionSize.Y)
        {
          for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
          {
            for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
            {
              MapRegion region = this.GetRegion(p);
              if (p.X == this.MapBound.Min.X)
                this.discardedRegionsOnShift.Add(region);
              else
                this.swapRegions.Add(this.GetRegionHashCode(region.Offset), region);
            }
            p.X = this.MapBound.Max.X;
            MapRegion region1 = this.AddRegionCore(p, false, true, (IProgressBar) null);
            region1.HeightMap = this.GetHeightMapFromSwap(p);
            this.LoadRegionCore(region1);
            this.swapRegions.Add(this.GetRegionHashCode(region1.Offset), region1);
          }
        }
        Dictionary<int, MapRegion> regions = this.Regions;
        Dictionary<int, MapHeightmap> heightMaps = this.HeightMaps;
        this.Regions = this.swapRegions;
        this.HeightMaps = this.swapHeightMaps;
        this.MapBound.Min.X += this.RegionSize.X;
        this.MapBound.Max.X += this.RegionSize.X;
        this.MapCenter.X += this.RegionSize.X;
        this.swapRegions = regions;
        this.swapHeightMaps = heightMaps;
        foreach (MapRegion region in this.discardedRegionsOnShift)
          this.SaveRegion(region);
        foreach (MapHeightmap hgtmap in this.discardedHeightMapsOnShift)
          this.SaveHeightMap(hgtmap);
        this.discardedRegionsOnShift.Clear();
        this.discardedHeightMapsOnShift.Clear();
        this.OnMapShiftEnd(BlockFace.Right);
      }
    }

    public void ShiftBackward()
    {
      if (this.MapBound.Max.Z >= this.MapMaxSize.Z)
        return;
      this.OnMapShiftBegin(BlockFace.Backward);
      GlobalPoint3D p = new GlobalPoint3D();
      lock (this.shiftLock)
      {
        this.swapHeightMaps.Clear();
        for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
        {
          for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
          {
            MapHeightmap heightMap = this.GetHeightMap(p);
            if (p.Z == this.MapBound.Min.Z)
              this.discardedHeightMapsOnShift.Add(heightMap);
            else
              this.swapHeightMaps.Add(this.GetRegionHashCode(heightMap.Offset), heightMap);
          }
          p.Z = this.MapBound.Max.Z;
          MapHeightmap mapHeightmap = this.AddHeightMap(p);
          this.swapHeightMaps.Add(this.GetRegionHashCode(mapHeightmap.Offset), mapHeightmap);
        }
        this.swapRegions.Clear();
        for (p.Y = this.MapBound.Min.Y; p.Y < this.MapBound.Max.Y; p.Y += this.RegionSize.Y)
        {
          for (p.X = this.MapBound.Min.X; p.X < this.MapBound.Max.X; p.X += this.RegionSize.X)
          {
            for (p.Z = this.MapBound.Min.Z; p.Z < this.MapBound.Max.Z; p.Z += this.RegionSize.Z)
            {
              MapRegion region = this.GetRegion(p);
              if (p.Z == this.MapBound.Min.Z)
                this.discardedRegionsOnShift.Add(region);
              else
                this.swapRegions.Add(this.GetRegionHashCode(region.Offset), region);
            }
            p.Z = this.MapBound.Max.Z;
            MapRegion region1 = this.AddRegionCore(p, false, true, (IProgressBar) null);
            region1.HeightMap = this.GetHeightMapFromSwap(p);
            this.LoadRegionCore(region1);
            this.swapRegions.Add(this.GetRegionHashCode(region1.Offset), region1);
          }
        }
        Dictionary<int, MapRegion> regions = this.Regions;
        Dictionary<int, MapHeightmap> heightMaps = this.HeightMaps;
        this.Regions = this.swapRegions;
        this.HeightMaps = this.swapHeightMaps;
        this.MapBound.Min.Z += this.RegionSize.Z;
        this.MapBound.Max.Z += this.RegionSize.Z;
        this.MapCenter.Z += this.RegionSize.Z;
        this.swapRegions = regions;
        this.swapHeightMaps = heightMaps;
        foreach (MapRegion region in this.discardedRegionsOnShift)
          this.SaveRegion(region);
        foreach (MapHeightmap hgtmap in this.discardedHeightMapsOnShift)
          this.SaveHeightMap(hgtmap);
        this.discardedRegionsOnShift.Clear();
        this.discardedHeightMapsOnShift.Clear();
        this.OnMapShiftEnd(BlockFace.Backward);
      }
    }

    private MapHeightmap GetHeightMapFromSwap(GlobalPoint3D p)
    {
      p.Y = 0;
      MapHeightmap mapHeightmap;
      this.swapHeightMaps.TryGetValue(this.GetRegionHashCode(p), out mapHeightmap);
      return mapHeightmap;
    }

    public delegate bool AddToBlastedListHandler(GlobalPoint3D orig, GlobalPoint3D p);

    public delegate bool IsOnlyQualifier(byte blockID);

    public enum CopyType
    {
      Overwrite,
      NoOverwrite,
      Merge,
    }

    public enum CopyAccess
    {
      Full,
      Restricted,
    }
  }
}
