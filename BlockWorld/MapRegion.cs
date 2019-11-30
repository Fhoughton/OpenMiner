// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapRegion
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class MapRegion : IHasContent
  {
    public static List<MapChunk> DontSave = new List<MapChunk>();
    private List<MapChunk> neighbours = new List<MapChunk>(27);
    public Map Map;
    public GlobalPoint3D Offset;
    public BoundingBox Box;
    public MapChunk[] Chunks;
    public OctreeBase<MapChunk> Octree;
    public QuadtreeBase<MapChunk> Quadtree;
    public MapHeightmap HeightMap;
    public int ChunksDecoratedCount;
    protected Point3D chunkSize;
    protected int chunksPerRow;
    protected int chunksPerPlane;

    public bool IsValidPoint(GlobalPoint3D p)
    {
      if (p.X >= this.Offset.X && p.X < this.Offset.X + this.Map.RegionSize.X && (p.Y >= this.Offset.Y && p.Y < this.Offset.Y + this.Map.RegionSize.Y) && p.Z >= this.Offset.Z)
        return p.Z < this.Offset.Z + this.Map.RegionSize.Z;
      return false;
    }

    public bool IsValidPoint(Point3D p)
    {
      if (p.X >= 0 && p.X < this.Map.RegionSize.X && (p.Y >= this.Offset.Y && p.Y < this.Map.RegionSize.Y) && p.Z >= this.Offset.Z)
        return p.Z < this.Map.RegionSize.Z;
      return false;
    }

    public Point3D GetLocalPoint(GlobalPoint3D p)
    {
      return new Point3D()
      {
        X = p.X - this.Offset.X,
        Y = p.Y - this.Offset.Y,
        Z = p.Z - this.Offset.Z
      };
    }

    public GlobalPoint3D GetGlobalPoint(Point3D p)
    {
      return new GlobalPoint3D()
      {
        X = p.X + this.Offset.X,
        Y = p.Y + this.Offset.Y,
        Z = p.Z + this.Offset.Z
      };
    }

    public static int GetHashCode(Map map, GlobalPoint3D p)
    {
      return map.GetRegionHashCode(p);
    }

    public override int GetHashCode()
    {
      return MapRegion.GetHashCode(this.Map, this.Offset);
    }

    public int GetHashCodeOld()
    {
      int num1 = (this.Offset.X - this.Map.MapBound.Min.X) / this.Map.RegionSize.X;
      int num2 = (this.Offset.Y - this.Map.MapBound.Min.Y) / this.Map.RegionSize.Y;
      int num3 = (this.Offset.Z - this.Map.MapBound.Min.Z) / this.Map.RegionSize.Z;
      return ((num2 & 15) << 28) + ((num1 & 16383) << 14) + (num3 & 16383);
    }

    public void GetChunksToSave(List<MapChunk> chunksToSave)
    {
      foreach (MapChunk chunk in this.Chunks)
      {
        if (chunk != null && chunk.IsChunkFlagSet(ChunkFlags.UserEdited | ChunkFlags.HasSpecialBlocks))
          chunksToSave.Add(chunk);
      }
    }

    public virtual int MemorySize
    {
      get
      {
        int num = 60;
        if (this.Octree != null)
          num += this.Octree.MemorySize;
        if (this.Quadtree != null)
          num += this.Quadtree.MemorySize;
        if (this.Chunks != null)
        {
          num = num + this.Chunks.Length * 4 + this.neighbours.Count * 4;
          foreach (MapChunk chunk in this.Chunks)
          {
            if (chunk != null)
              num += chunk.MemorySize;
          }
        }
        return num;
      }
    }

    public bool IsAllChunksDecorated
    {
      get
      {
        if (this.Chunks == null)
          return false;
        return this.ChunksDecoratedCount >= this.Chunks.Length;
      }
    }

    public virtual void Initialize(Map map, GlobalPoint3D offset)
    {
      this.Map = map;
      this.Offset = offset;
      Vector3 min = offset.ToVector3() * this.Map.TileSize;
      Vector3 max = min + this.Map.RegionSize.ToVector3() * this.Map.TileSize;
      this.Box = new BoundingBox(min, max);
    }

    public void LoadContent(InitState state)
    {
      this.LoadContentCore();
    }

    protected virtual void LoadContentCore()
    {
    }

    public void UnloadContent()
    {
      this.UnloadContentCore();
      if (this.Chunks != null)
      {
        for (int index = 0; index < this.Chunks.Length; ++index)
        {
          if (this.Chunks[index] != null)
            this.Chunks[index].UnloadContent();
        }
      }
      if (this.Octree != null)
        this.Octree.Clear();
      if (this.Quadtree != null)
        this.Quadtree.Clear();
      if (this.neighbours == null)
        return;
      this.neighbours.Clear();
    }

    public void UnloadForRecycle()
    {
      if (this.Chunks == null)
        return;
      for (int index = 0; index < this.Chunks.Length; ++index)
      {
        if (this.Chunks[index] != null)
          this.Chunks[index].UnloadForRecycle();
      }
    }

    protected virtual void UnloadContentCore()
    {
    }

    public virtual void Pregenerate(
      bool isGenerated,
      bool ignoreInsideMapTest,
      IProgressBar progress)
    {
      this.chunkSize = this.Map.ChunkSize;
      this.chunksPerRow = this.Map.RegionSize.X / this.chunkSize.X;
      this.chunksPerPlane = this.chunksPerRow * (this.Map.RegionSize.Z / this.chunkSize.Z);
      this.InitializeSpatialTree(progress);
      this.PregenerateChunks(isGenerated, ignoreInsideMapTest, progress);
    }

    protected virtual void PregenerateChunks(
      bool isGenerated,
      bool ignoreInsideMapTest,
      IProgressBar progress)
    {
      this.Chunks = new MapChunk[this.chunksPerPlane * (this.Map.RegionSize.Y / this.chunkSize.Y)];
      Point3D zero = Point3D.Zero;
      Point3D regionSize = this.Map.RegionSize;
      float increment = 1f / (float) this.Chunks.Length;
      ChunkFlags flag = ChunkFlags.MeshDirty;
      if (isGenerated)
        flag |= ChunkFlags.Generated | ChunkFlags.Decorated | ChunkFlags.ReceivedFromHost;
      for (zero.Y = 0; zero.Y < regionSize.Y; zero.Y += this.chunkSize.Y)
      {
        for (zero.Z = 0; zero.Z < regionSize.Z; zero.Z += this.chunkSize.Z)
        {
          for (zero.X = 0; zero.X < regionSize.X; zero.X += this.chunkSize.X)
          {
            if (ignoreInsideMapTest || this.Map.IsIntersectingMap(this.Offset + zero, this.chunkSize))
            {
              MapChunk chunk = this.CreateChunk(zero);
              chunk.SetChunkFlag(flag);
              this.Chunks[this.GetChunkIndex(zero)] = chunk;
              this.Octree.AddObject(chunk, chunk.Box);
              this.Quadtree.AddObject(chunk, chunk.Box);
              ++this.Map.TotalChunks;
            }
            progress?.AddProgress(increment);
          }
        }
      }
    }

    protected virtual MapChunk CreateChunk(Point3D offset)
    {
      return new MapChunk(this, offset);
    }

    private void InitializeSpatialTree(IProgressBar progress)
    {
      int splitCount = this.GetSplitCount(this.chunkSize.X, this.Map.RegionSize.X);
      this.GetSplitCount(this.chunkSize.Y, this.Map.RegionSize.Y);
      this.GetSplitCount(this.chunkSize.Z, this.Map.RegionSize.Z);
      BoundingBox box = new BoundingBox(Vector3.Zero, this.Map.RegionSize.ToVector3() * this.Map.TileSize);
      if (splitCount == 1)
      {
        this.Octree = (OctreeBase<MapChunk>) new OctreeLeaf<MapChunk>((StudioForge.BlockWorld.Octree<MapChunk>) null, box);
        this.Quadtree = (QuadtreeBase<MapChunk>) new QuadtreeLeaf<MapChunk>((StudioForge.BlockWorld.Quadtree<MapChunk>) null, box);
      }
      else
      {
        this.Octree = (OctreeBase<MapChunk>) new StudioForge.BlockWorld.Octree<MapChunk>((StudioForge.BlockWorld.Octree<MapChunk>) null, box);
        ((StudioForge.BlockWorld.Octree<MapChunk>) this.Octree).Split(this.Map, splitCount - 1);
        this.Quadtree = (QuadtreeBase<MapChunk>) new StudioForge.BlockWorld.Quadtree<MapChunk>((StudioForge.BlockWorld.Quadtree<MapChunk>) null, box);
        ((StudioForge.BlockWorld.Quadtree<MapChunk>) this.Quadtree).Split(this.Map, splitCount);
      }
    }

    private int GetSplitCount(int chunksize, int regionsize)
    {
      if (chunksize == regionsize)
        return 1;
      chunksize *= 2;
      int num = 1;
      for (; chunksize < regionsize; chunksize *= 2)
        ++num;
      return num;
    }

    public void RegenerateChunks(GlobalPoint3D offset)
    {
      this.Offset = offset;
      for (int index = 0; index < this.Chunks.Length; ++index)
        this.Chunks[index].Reset(ChunkFlags.MeshDirty);
    }

    public byte GetBlockID(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return this.Map.OutOfBoundsBlockID;
      return chunk.GetBlockID(chunk.GetMapIndex(localPoint));
    }

    public byte GetBlockID_Test(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return this.Map.OutOfBoundsBlockID;
      return chunk.GetBlockID_Test(chunk.GetMapIndex(localPoint));
    }

    public byte GetBlockIDNoCache(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return this.Map.OutOfBoundsBlockID;
      return chunk.GetBlockIDNoCache(localPoint);
    }

    public MapLight GetLight(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return this.Map.SunLight;
      return chunk.GetLight(localPoint);
    }

    public MapLight GetLightNoCache(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return this.Map.SunLight;
      return chunk.GetLightNoCache(localPoint);
    }

    public byte GetSunLight(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return this.Map.SunLight.SunLight;
      return chunk.GetSunLight(localPoint);
    }

    public byte GetBlockLight(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return this.Map.SunLight.BlockLight;
      return chunk.GetBlockLight(localPoint);
    }

    public byte GetAUXData(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return 0;
      return chunk.GetAUXData(localPoint);
    }

    public byte GetAUXDataNoCache(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return 0;
      return chunk.GetAUXDataNoCache(localPoint);
    }

    public byte GetAUXHighData(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return 0;
      return chunk.GetAUXHighData(localPoint);
    }

    public byte GetAUXHighDataNoCache(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return 0;
      return chunk.GetAUXHighDataNoCache(localPoint);
    }

    public byte GetAUXFullData(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return 0;
      return chunk.GetAUXFullData(localPoint);
    }

    public byte GetAUXFullDataNoCache(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk == null)
        return 0;
      return chunk.GetAUXFullDataNoCache(localPoint);
    }

    public MapBlock GetBlockData(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk != null)
        return chunk.GetBlockData(chunk.GetMapIndex(localPoint));
      return new MapBlock()
      {
        BlockID = this.Map.OutOfBoundsBlockID,
        Light = this.Map.SunLight,
        AuxData = 0
      };
    }

    public MapBlock GetBlockIDAndAux(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk != null)
        return chunk.GetBlockIDAndAux(chunk.GetMapIndex(localPoint));
      return new MapBlock()
      {
        BlockID = this.Map.OutOfBoundsBlockID,
        AuxData = 0
      };
    }

    public MapBlock GetBlockIDAndAuxNoCache(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk != null)
        return chunk.GetBlockIDAndAuxNoCache(chunk.GetMapIndex(localPoint));
      return new MapBlock()
      {
        BlockID = this.Map.OutOfBoundsBlockID,
        AuxData = 0
      };
    }

    public MapBlock GetBlockAndLight(GlobalPoint3D p)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk != null)
        return chunk.GetBlockAndLight(chunk.GetMapIndex(localPoint));
      return new MapBlock()
      {
        BlockID = this.Map.OutOfBoundsBlockID,
        AuxData = 0
      };
    }

    public ushort GetHeight(GlobalPoint3D p)
    {
      return this.HeightMap.GetHeight(p.X, p.Z);
    }

    public ushort GetHeight(Point3D p)
    {
      return this.HeightMap.GetHeightLocal(p.X, p.Z);
    }

    public MapChunk SetBlockData(
      GlobalPoint3D p,
      MapBlock oldBlockdata,
      MapBlock newBlockdata,
      UpdateBlockMethod method)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk != null)
      {
        oldBlockdata.Chunk = newBlockdata.Chunk = chunk;
        chunk.SetBlockData(localPoint, newBlockdata, method);
        this.Map.AdjustHeightMapInternal(this, p, newBlockdata, method, method != UpdateBlockMethod.Generation);
      }
      return chunk;
    }

    public MapChunk SetBlockIDAndAux(
      GlobalPoint3D p,
      MapBlock oldBlockdata,
      MapBlock newBlockdata,
      UpdateBlockMethod method)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      if (chunk != null)
      {
        oldBlockdata.Chunk = newBlockdata.Chunk = chunk;
        chunk.SetBlockIDAndAux(localPoint, newBlockdata, method);
        this.Map.AdjustHeightMapInternal(this, p, newBlockdata, method, method != UpdateBlockMethod.Generation);
      }
      return chunk;
    }

    public MapChunk SetAuxData(GlobalPoint3D p, byte auxData, UpdateBlockMethod method)
    {
      Point3D localPoint = this.GetLocalPoint(p);
      MapChunk chunk = this.GetChunk(localPoint);
      chunk?.SetAuxData(localPoint, auxData, method);
      return chunk;
    }

    public void SetHeight(int x, int z, ushort h, ushort h1)
    {
      this.HeightMap.SetHeight(x, z, h, h1);
    }

    public void SetHeight(ushort value)
    {
      if ((int) this.HeightMap.HeightMap[0] == (int) value)
        return;
      this.HeightMap.SetHeight(value);
    }

    public MapChunk GetChunk(GlobalPoint3D p)
    {
      int chunkIndex = this.GetChunkIndex(this.GetLocalPoint(p));
      if (this.Chunks == null)
        return (MapChunk) null;
      return this.Chunks[chunkIndex];
    }

    public MapChunk GetChunk(Point3D p)
    {
      int chunkIndex = this.GetChunkIndex(p);
      if (this.Chunks == null)
        return (MapChunk) null;
      return this.Chunks[chunkIndex];
    }

    private int GetChunkIndex(Point3D p)
    {
      int num1 = p.X / this.chunkSize.X;
      int num2 = p.Y / this.chunkSize.Y;
      int num3 = p.Z / this.chunkSize.Z;
      return num1 + num3 * this.chunksPerRow + num2 * this.chunksPerPlane;
    }

    public MapRegion LeftNeighbour()
    {
      return this.Map.GetRegion(this.Offset - new GlobalPoint3D(this.Map.RegionSize.X, 0, 0));
    }

    public MapRegion ForwardNeighbour()
    {
      return this.Map.GetRegion(this.Offset - new GlobalPoint3D(0, 0, this.Map.RegionSize.Z));
    }

    public MapRegion RightNeighbour()
    {
      return this.Map.GetRegion(this.Offset + new GlobalPoint3D(this.Map.RegionSize.X, 0, 0));
    }

    public MapRegion BackwardNeighbour()
    {
      return this.Map.GetRegion(this.Offset + new GlobalPoint3D(0, 0, this.Map.RegionSize.Z));
    }

    public MapRegion UpNeighbour()
    {
      return this.Map.GetRegion(this.Offset + new GlobalPoint3D(0, this.Map.RegionSize.Y, 0));
    }

    public MapRegion DownNeighbour()
    {
      return this.Map.GetRegion(this.Offset - new GlobalPoint3D(0, this.Map.RegionSize.Y, 0));
    }
  }
}
