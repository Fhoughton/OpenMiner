// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapLightingByChunk
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class MapLightingByChunk
  {
    protected Map map;
    protected MapChunk chunk;
    protected Point3D chunkSize;
    protected short blockCacheID;
    protected short auxCacheID;
    protected short lightCacheID;
    protected short oldLightCacheID;
    protected int blockCacheIndex;
    protected int auxCacheIndex;
    protected int lightCacheIndex;
    protected int oldLightCacheIndex;
    protected byte[] blockCache;
    protected byte[] auxCache;
    protected byte[] lightCache;
    protected byte[] oldLightCache;
    protected int planeSize;
    protected int chunkLength;
    protected List<int>[][] sources;
    protected int maxLight;
    protected bool enableBlockLuminence;
    private MapLightingByChunk.AddSourcesDelegate addSourcesFromLeft;
    private MapLightingByChunk.AddSourcesDelegate addSourcesFromForward;
    private MapLightingByChunk.AddSourcesDelegate addSourcesFromRight;
    private MapLightingByChunk.AddSourcesDelegate addSourcesFromBackward;
    private MapLightingByChunk.AddSourcesDelegate addSourcesFromUp;
    private MapLightingByChunk.AddSourcesDelegate addSourcesFromDown;
    private MapLightingByChunk.FlagNeighbourDelegate flagNeighbourLeft;
    private MapLightingByChunk.FlagNeighbourDelegate flagNeighbourForward;
    private MapLightingByChunk.FlagNeighbourDelegate flagNeighbourRight;
    private MapLightingByChunk.FlagNeighbourDelegate flagNeighbourBackward;
    private MapLightingByChunk.FlagNeighbourDelegate flagNeighbourUp;
    private MapLightingByChunk.FlagNeighbourDelegate flagNeighbourDown;

    public virtual long MemorySize
    {
      get
      {
        long num = 36;
        if (this.sources != null)
        {
          num += (long) (this.sources.Length * 4);
          for (int index1 = 0; index1 < this.sources.Length; ++index1)
          {
            for (int index2 = 0; index2 < this.sources[index1].Length; ++index2)
            {
              if (this.sources[index1] != null && this.sources[index1][index2] != null)
                num += (long) (this.sources[index1][index2].Count * 4);
            }
          }
        }
        return num;
      }
    }

    public MapLightingByChunk()
    {
      this.addSourcesFromLeft = new MapLightingByChunk.AddSourcesDelegate(this.AddSourcesFromLeft);
      this.addSourcesFromForward = new MapLightingByChunk.AddSourcesDelegate(this.AddSourcesFromForward);
      this.addSourcesFromRight = new MapLightingByChunk.AddSourcesDelegate(this.AddSourcesFromRight);
      this.addSourcesFromBackward = new MapLightingByChunk.AddSourcesDelegate(this.AddSourcesFromBackward);
      this.addSourcesFromUp = new MapLightingByChunk.AddSourcesDelegate(this.AddSourcesFromUp);
      this.addSourcesFromDown = new MapLightingByChunk.AddSourcesDelegate(this.AddSourcesFromDown);
      this.flagNeighbourLeft = new MapLightingByChunk.FlagNeighbourDelegate(this.FlagLeftNeighbour);
      this.flagNeighbourForward = new MapLightingByChunk.FlagNeighbourDelegate(this.FlagForwardNeighbour);
      this.flagNeighbourRight = new MapLightingByChunk.FlagNeighbourDelegate(this.FlagRightNeighbour);
      this.flagNeighbourBackward = new MapLightingByChunk.FlagNeighbourDelegate(this.FlagBackNeighbour);
      this.flagNeighbourUp = new MapLightingByChunk.FlagNeighbourDelegate(this.FlagUpNeighbour);
      this.flagNeighbourDown = new MapLightingByChunk.FlagNeighbourDelegate(this.FlagDownNeighbour);
    }

    public void Initialize(Map map, MapChunk chunk, bool disableBlockLuminence)
    {
      bool flag = this.sources == null || map.ChunkLength != this.map.ChunkLength;
      this.map = map;
      this.chunk = chunk;
      this.enableBlockLuminence = !disableBlockLuminence;
      this.chunkSize = map.ChunkSize;
      this.planeSize = this.chunkSize.X * this.chunkSize.Z;
      this.chunkLength = this.planeSize * this.chunkSize.Y;
      this.maxLight = (int) map.MaxLight;
      if (!flag)
        return;
      this.InitLightSourceArray();
    }

    private void InitLightSourceArray()
    {
      int num = this.chunkSize.X * this.chunkSize.Y * this.chunkSize.Z + this.chunkSize.X * this.chunkSize.Y * 2 + this.chunkSize.X * this.chunkSize.Z * 2 + this.chunkSize.Z * this.chunkSize.Y * 2;
      this.sources = new List<int>[2][];
      for (int index1 = 0; index1 < 2; ++index1)
      {
        this.sources[index1] = new List<int>[this.maxLight];
        for (int index2 = 0; index2 < this.maxLight; ++index2)
          this.sources[index1][index2] = new List<int>(index1 != 1 || index2 != this.maxLight - 1 ? 10 : num);
      }
    }

    public virtual void Update()
    {
      try
      {
        this.RebuildLightingCore();
      }
      finally
      {
        this.chunk.LightEnd();
      }
    }

    protected void RebuildLightingCore()
    {
      this.blockCacheID = this.oldLightCacheID = this.auxCacheID = this.lightCacheID = (short) -1;
      try
      {
        this.chunk.BlockData.GetCacheAndAddRefCount(this.chunk, out this.blockCacheID, out this.blockCacheIndex);
        this.blockCache = this.map.ChunkCacheManager.Cache[(int) this.blockCacheID];
        this.chunk.LightData.GetCacheAndAddRefCount(this.chunk, out this.oldLightCacheID, out this.oldLightCacheIndex);
        this.oldLightCache = this.map.ChunkCacheManager.Cache[(int) this.oldLightCacheID];
        this.chunk.AuxData.GetCacheAndAddRefCount(this.chunk, out this.auxCacheID, out this.auxCacheIndex);
        this.auxCache = this.map.ChunkCacheManager.Cache[(int) this.auxCacheID];
        this.map.ChunkCacheManager.AcquireCache((MapChunk) null, (RLEStreamByte) null, true, out this.lightCacheID, out this.lightCacheIndex);
        this.lightCache = this.map.ChunkCacheManager.Cache[(int) this.lightCacheID];
        this.RebuildLightingCore2();
        this.chunk.LightData.SetCache(this.chunk, this.lightCacheID, this.lightCacheIndex);
      }
      finally
      {
        this.map.ChunkCacheManager.DecRefCount(this.lightCacheID, this.lightCacheIndex);
        this.map.ChunkCacheManager.DecRefCount(this.auxCacheID, this.auxCacheIndex);
        this.map.ChunkCacheManager.DecRefCount(this.oldLightCacheID, this.oldLightCacheIndex);
        this.map.ChunkCacheManager.DecRefCount(this.blockCacheID, this.blockCacheIndex);
      }
    }

    private void RebuildLightingCore2()
    {
      this.InitSourceLists();
      this.AddNeighbourSources();
      this.ClearLightDataAndBuildSources();
      this.WriteInitialSources();
      this.RebuildLightFromSources();
      this.FlagNeighbours();
    }

    private void InitSourceLists()
    {
      for (int index1 = 0; index1 < 2; ++index1)
      {
        for (int index2 = 0; index2 < this.maxLight; ++index2)
          this.sources[index1][index2].Clear();
      }
    }

    private void AddNeighbourSources()
    {
      this.AddNeigbourSourcesCore(this.chunk.LeftNeighbour(), BlockFace.Left, this.addSourcesFromLeft);
      this.AddNeigbourSourcesCore(this.chunk.RightNeighbour(), BlockFace.Right, this.addSourcesFromRight);
      this.AddNeigbourSourcesCore(this.chunk.DownNeighbour(), BlockFace.Down, this.addSourcesFromDown);
      this.AddNeigbourSourcesCore(this.chunk.UpNeighbour(), BlockFace.Up, this.addSourcesFromUp);
      this.AddNeigbourSourcesCore(this.chunk.ForwardNeighbour(), BlockFace.Forward, this.addSourcesFromForward);
      this.AddNeigbourSourcesCore(this.chunk.BackwardNeighbour(), BlockFace.Backward, this.addSourcesFromBackward);
    }

    private void AddNeigbourSourcesCore(
      MapChunk nchunk,
      BlockFace neighbourID,
      MapLightingByChunk.AddSourcesDelegate addSources)
    {
      if (nchunk == null || !nchunk.IsGenerated)
        return;
      MapLightingByChunk.NeighbourSourceParams data = new MapLightingByChunk.NeighbourSourceParams();
      data.NeighbourID = neighbourID;
      nchunk.BlockData.GetCacheAndAddRefCount(nchunk, out data.NBlockCacheID, out data.NBlockCacheIndex);
      data.NBlockCache = this.map.ChunkCacheManager.Cache[(int) data.NBlockCacheID];
      nchunk.LightData.GetCacheAndAddRefCount(nchunk, out data.NLightCacheID, out data.NLightCacheIndex);
      data.NLightCache = this.map.ChunkCacheManager.Cache[(int) data.NLightCacheID];
      addSources(nchunk, data);
      this.map.ChunkCacheManager.DecRefCount(data.NLightCacheID, data.NLightCacheIndex);
      this.map.ChunkCacheManager.DecRefCount(data.NBlockCacheID, data.NBlockCacheIndex);
    }

    private void AddSourcesFromLeft(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y; ++point.Y)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z; ++point.Z)
        {
          point.X = 0;
          data.Index = this.chunk.GetMapIndex(point);
          point.X = this.chunkSize.X - 1;
          data.NIndex = nchunk.GetMapIndex(point);
          this.AddNeighbourSource(ref data);
        }
      }
    }

    private void AddSourcesFromForward(
      MapChunk nchunk,
      MapLightingByChunk.NeighbourSourceParams data)
    {
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y; ++point.Y)
      {
        for (point.X = 0; point.X < this.chunkSize.X; ++point.X)
        {
          point.Z = 0;
          data.Index = this.chunk.GetMapIndex(point);
          point.Z = this.chunkSize.Z - 1;
          data.NIndex = nchunk.GetMapIndex(point);
          this.AddNeighbourSource(ref data);
        }
      }
    }

    private void AddSourcesFromRight(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y; ++point.Y)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z; ++point.Z)
        {
          point.X = this.chunkSize.X - 1;
          data.Index = this.chunk.GetMapIndex(point);
          point.X = 0;
          data.NIndex = nchunk.GetMapIndex(point);
          this.AddNeighbourSource(ref data);
        }
      }
    }

    private void AddSourcesFromBackward(
      MapChunk nchunk,
      MapLightingByChunk.NeighbourSourceParams data)
    {
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y; ++point.Y)
      {
        for (point.X = 0; point.X < this.chunkSize.X; ++point.X)
        {
          point.Z = this.chunkSize.Z - 1;
          data.Index = this.chunk.GetMapIndex(point);
          point.Z = 0;
          data.NIndex = nchunk.GetMapIndex(point);
          this.AddNeighbourSource(ref data);
        }
      }
    }

    private void AddSourcesFromUp(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      Point3D point = new Point3D();
      for (point.X = 0; point.X < this.chunkSize.X; ++point.X)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z; ++point.Z)
        {
          point.Y = this.chunkSize.Y - 1;
          data.Index = this.chunk.GetMapIndex(point);
          point.Y = 0;
          data.NIndex = nchunk.GetMapIndex(point);
          this.AddNeighbourSource(ref data);
        }
      }
    }

    private void AddSourcesFromDown(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      Point3D point = new Point3D();
      for (point.X = 0; point.X < this.chunkSize.X; ++point.X)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z; ++point.Z)
        {
          point.Y = 0;
          data.Index = this.chunk.GetMapIndex(point);
          point.Y = this.chunkSize.Y - 1;
          data.NIndex = nchunk.GetMapIndex(point);
          this.AddNeighbourSource(ref data);
        }
      }
    }

    private void AddNeighbourSource(ref MapLightingByChunk.NeighbourSourceParams data)
    {
      int index = data.Index;
      byte opacity = this.map.GetOpacity(this.blockCache[this.blockCacheIndex + index]);
      if ((int) opacity >= this.maxLight)
        return;
      int nindex = data.NIndex;
      MapLight mapLight = MapLight.FromByte(data.NLightCache[data.NLightCacheIndex + nindex]);
      byte blockLight = mapLight.BlockLight;
      if ((int) blockLight > (int) opacity)
      {
        byte num = (byte) ((uint) blockLight - (uint) opacity);
        if ((int) num >= (int) MapLight.FromByte(this.oldLightCache[this.oldLightCacheIndex + index]).BlockLight)
          this.sources[0][(int) num - 1].Add(index);
      }
      byte sunLight = mapLight.SunLight;
      if ((int) sunLight <= (int) opacity)
        return;
      byte num1 = (byte) ((uint) sunLight - (uint) opacity);
      if ((int) num1 < (int) MapLight.FromByte(this.oldLightCache[this.oldLightCacheIndex + index]).SunLight)
        return;
      this.sources[1][(int) num1 - 1].Add(index);
    }

    private bool HasGreaterBlockSource(
      MapChunk nchunk,
      ref GlobalPoint3D np,
      int nindex,
      MapBlock ndata)
    {
      return this.map.GetLuminance(ref np, ndata.BlockID) > (byte) 0 || nindex % this.chunkSize.X > 0 && (int) MapLight.GetBlockLight(nchunk.LightData.GetData(nchunk, nindex - 1)) > (int) ndata.Light.BlockLight || (nindex % this.chunkSize.X < this.chunkSize.X - 1 && (int) MapLight.GetBlockLight(nchunk.LightData.GetData(nchunk, nindex + 1)) > (int) ndata.Light.BlockLight || nindex >= this.planeSize && (int) MapLight.GetBlockLight(nchunk.LightData.GetData(nchunk, nindex - this.planeSize)) > (int) ndata.Light.BlockLight) || (nindex < this.chunkLength - this.planeSize && (int) MapLight.GetBlockLight(nchunk.LightData.GetData(nchunk, nindex + this.planeSize)) > (int) ndata.Light.BlockLight || nindex % this.planeSize >= this.chunkSize.X && (int) MapLight.GetBlockLight(nchunk.LightData.GetData(nchunk, nindex - this.chunkSize.X)) > (int) ndata.Light.BlockLight || nindex % this.planeSize < this.planeSize - this.chunkSize.X && (int) MapLight.GetBlockLight(nchunk.LightData.GetData(nchunk, nindex + this.chunkSize.X)) > (int) ndata.Light.BlockLight);
    }

    private bool HasGreaterSunSource(MapChunk nchunk, int nindex, MapBlock ndata)
    {
      return nindex % this.chunkSize.X > 0 && (int) MapLight.GetSunLight(nchunk.LightData.GetData(nchunk, nindex - 1)) > (int) ndata.Light.SunLight || nindex % this.chunkSize.X < this.chunkSize.X - 1 && (int) MapLight.GetSunLight(nchunk.LightData.GetData(nchunk, nindex + 1)) > (int) ndata.Light.SunLight || (nindex >= this.planeSize && (int) MapLight.GetSunLight(nchunk.LightData.GetData(nchunk, nindex - this.planeSize)) > (int) ndata.Light.SunLight || nindex < this.chunkLength - this.planeSize && (int) MapLight.GetSunLight(nchunk.LightData.GetData(nchunk, nindex + this.planeSize)) > (int) ndata.Light.SunLight) || (nindex % this.planeSize >= this.chunkSize.X && (int) MapLight.GetSunLight(nchunk.LightData.GetData(nchunk, nindex - this.chunkSize.X)) > (int) ndata.Light.SunLight || nindex % this.planeSize < this.planeSize - this.chunkSize.X && (int) MapLight.GetSunLight(nchunk.LightData.GetData(nchunk, nindex + this.chunkSize.X)) > (int) ndata.Light.SunLight);
    }

    protected void ClearLightDataAndBuildSources()
    {
      Point3D zero1 = Point3D.Zero;
      Point3D offset1 = this.chunk.Offset;
      GlobalPoint3D min = this.map.MapBound.Min;
      GlobalPoint3D max = this.map.MapBound.Max;
      MapRegion region = this.chunk.Region;
      Point3D offset2 = this.chunk.Offset;
      GlobalPoint3D offset3 = region.Offset;
      MapHeightmap heightMap = region.HeightMap;
      GlobalPoint3D zero2 = GlobalPoint3D.Zero;
      int num = 0;
      int lightCacheIndex = this.lightCacheIndex;
      for (; num < this.chunkLength; ++num)
      {
        this.lightCache[lightCacheIndex++] = (byte) 0;
        if (offset1.X + offset3.X < max.X && offset1.Z + offset3.Z < max.Z)
        {
          if (this.enableBlockLuminence)
          {
            byte blockID = this.blockCache[this.blockCacheIndex + num];
            zero2.X = offset3.X + offset1.X;
            zero2.Y = offset3.Y + offset1.Y;
            zero2.Z = offset3.Z + offset1.Z;
            byte luminance = this.map.GetLuminance(ref zero2, blockID);
            if (luminance > (byte) 0)
              this.sources[0][((int) luminance > this.maxLight ? this.maxLight : (int) luminance) - 1].Add(num);
          }
          int localForLighting = (int) heightMap.GetHeightLocalForLighting(offset1.X, offset1.Z);
          if (offset1.Y + offset3.Y > localForLighting)
            this.sources[1][(int) this.map.SunLight.SunLight - 1].Add(num);
        }
        ++offset1.X;
        if (++zero1.X == this.chunkSize.X)
        {
          zero1.X = 0;
          offset1.X = offset2.X;
          ++offset1.Z;
          if (++zero1.Z == this.chunkSize.Z)
          {
            zero1.Z = 0;
            offset1.Z = offset2.Z;
            ++offset1.Y;
          }
        }
      }
    }

    private void WriteInitialSources()
    {
      for (int index1 = 0; index1 < 2; ++index1)
      {
        for (int index2 = 0; index2 < this.sources[index1].Length; ++index2)
        {
          foreach (int num1 in this.sources[index1][index2])
          {
            byte num2 = this.lightCache[this.lightCacheIndex + num1];
            byte num3 = (byte) ((uint) num2 >> 4);
            byte num4 = (byte) ((uint) num2 & 15U);
            if (index1 == 0)
              num4 = (byte) (index2 + 1);
            else
              num3 = (byte) (index2 + 1);
            this.lightCache[this.lightCacheIndex + num1] = (byte) ((uint) num4 + ((uint) num3 << 4));
          }
        }
      }
    }

    private void RebuildLightFromSources()
    {
      for (int index = 0; index < 2; ++index)
      {
        for (int source = this.maxLight - 1; source >= 0; --source)
        {
          if (this.sources[index][source].Count > 0)
            this.RebuildLight(this.sources[index], source, index == 0);
        }
      }
    }

    private void RebuildLight(List<int>[] sources, int source, bool applyingBlockData)
    {
      for (int index = 0; index < sources[source].Count; ++index)
        this.RebuildLight(sources, sources[source][index], (byte) (source + 1), applyingBlockData);
    }

    private void RebuildLight(List<int>[] sources, int index, byte light, bool applyingBlockData)
    {
      Point3D point = this.chunk.GetPoint(index);
      if (point.X > 0)
        this.WriteLight(this.chunk, sources, index - 1, light, applyingBlockData);
      if (point.X < this.chunkSize.X - 1)
        this.WriteLight(this.chunk, sources, index + 1, light, applyingBlockData);
      if (point.Z > 0)
        this.WriteLight(this.chunk, sources, index - this.chunkSize.X, light, applyingBlockData);
      if (point.Z < this.chunkSize.Z - 1)
        this.WriteLight(this.chunk, sources, index + this.chunkSize.X, light, applyingBlockData);
      if (point.Y > 0)
        this.WriteLight(this.chunk, sources, index - this.planeSize, light, applyingBlockData);
      if (point.Y >= this.chunkSize.Y - 1)
        return;
      this.WriteLight(this.chunk, sources, index + this.planeSize, light, applyingBlockData);
    }

    private void WriteLight(
      MapChunk chunk,
      List<int>[] sources,
      int index,
      byte light,
      bool applyingBlockLight)
    {
      byte opacity = this.map.GetOpacity(this.blockCache[this.blockCacheIndex + index]);
      if ((int) opacity >= (int) light)
        return;
      byte num1 = this.lightCache[this.lightCacheIndex + index];
      byte num2 = applyingBlockLight ? (byte) ((uint) num1 & 15U) : (byte) ((uint) num1 >> 4);
      light -= opacity;
      if ((int) num2 >= (int) light)
        return;
      this.lightCache[this.lightCacheIndex + index] = !applyingBlockLight ? (byte) ((int) num1 & 15 | (int) light << 4) : (byte) ((uint) light | (uint) num1 & 240U);
      sources[(int) light - 1].Add(index);
    }

    private void FlagNeighbours()
    {
      this.FlagNeighbourCore(this.chunk.LeftNeighbour(), BlockFace.Left, this.flagNeighbourLeft);
      this.FlagNeighbourCore(this.chunk.ForwardNeighbour(), BlockFace.Forward, this.flagNeighbourForward);
      this.FlagNeighbourCore(this.chunk.RightNeighbour(), BlockFace.Right, this.flagNeighbourRight);
      this.FlagNeighbourCore(this.chunk.BackwardNeighbour(), BlockFace.Backward, this.flagNeighbourBackward);
      this.FlagNeighbourCore(this.chunk.UpNeighbour(), BlockFace.Up, this.flagNeighbourUp);
      this.FlagNeighbourCore(this.chunk.DownNeighbour(), BlockFace.Down, this.flagNeighbourDown);
    }

    private void FlagNeighbourCore(
      MapChunk nchunk,
      BlockFace neighbourID,
      MapLightingByChunk.FlagNeighbourDelegate flagNeighbour)
    {
      if (nchunk == null || nchunk.IsChunkFlagsSet(ChunkFlags.LightDirty) || !nchunk.IsGenerated)
        return;
      MapLightingByChunk.NeighbourSourceParams data = new MapLightingByChunk.NeighbourSourceParams();
      data.NeighbourID = neighbourID;
      nchunk.BlockData.GetCacheAndAddRefCount(nchunk, out data.NBlockCacheID, out data.NBlockCacheIndex);
      data.NBlockCache = this.map.ChunkCacheManager.Cache[(int) data.NBlockCacheID];
      nchunk.LightData.GetCacheAndAddRefCount(nchunk, out data.NLightCacheID, out data.NLightCacheIndex);
      data.NLightCache = this.map.ChunkCacheManager.Cache[(int) data.NLightCacheID];
      if (flagNeighbour(nchunk, data))
        nchunk.SetChunkFlag(ChunkFlags.LightDirty);
      this.map.ChunkCacheManager.DecRefCount(data.NLightCacheID, data.NLightCacheIndex);
      this.map.ChunkCacheManager.DecRefCount(data.NBlockCacheID, data.NBlockCacheIndex);
    }

    private bool FlagLeftNeighbour(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      bool flag = false;
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y && !flag; ++point.Y)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z && !flag; ++point.Z)
        {
          point.X = 0;
          data.Index = this.chunk.GetMapIndex(point);
          point.X = this.chunkSize.X - 1;
          data.NIndex = nchunk.GetMapIndex(point);
          flag = this.FlagNeighbourIfNeedsUpdate(ref data);
        }
      }
      return flag;
    }

    private bool FlagForwardNeighbour(
      MapChunk nchunk,
      MapLightingByChunk.NeighbourSourceParams data)
    {
      bool flag = false;
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y && !flag; ++point.Y)
      {
        for (point.X = 0; point.X < this.chunkSize.X && !flag; ++point.X)
        {
          point.Z = 0;
          data.Index = this.chunk.GetMapIndex(point);
          point.Z = this.chunkSize.Z - 1;
          data.NIndex = nchunk.GetMapIndex(point);
          flag = this.FlagNeighbourIfNeedsUpdate(ref data);
        }
      }
      return flag;
    }

    private bool FlagRightNeighbour(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      bool flag = false;
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y && !flag; ++point.Y)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z && !flag; ++point.Z)
        {
          point.X = this.chunkSize.X - 1;
          data.Index = this.chunk.GetMapIndex(point);
          point.X = 0;
          data.NIndex = nchunk.GetMapIndex(point);
          flag = this.FlagNeighbourIfNeedsUpdate(ref data);
        }
      }
      return flag;
    }

    private bool FlagBackNeighbour(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      bool flag = false;
      Point3D point = new Point3D();
      for (point.Y = 0; point.Y < this.chunkSize.Y && !flag; ++point.Y)
      {
        for (point.X = 0; point.X < this.chunkSize.X && !flag; ++point.X)
        {
          point.Z = this.chunkSize.Z - 1;
          data.Index = this.chunk.GetMapIndex(point);
          point.Z = 0;
          data.NIndex = nchunk.GetMapIndex(point);
          flag = this.FlagNeighbourIfNeedsUpdate(ref data);
        }
      }
      return flag;
    }

    private bool FlagUpNeighbour(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      bool flag = false;
      Point3D point = new Point3D();
      for (point.X = 0; point.X < this.chunkSize.X && !flag; ++point.X)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z && !flag; ++point.Z)
        {
          point.Y = this.chunkSize.Y - 1;
          data.Index = this.chunk.GetMapIndex(point);
          point.Y = 0;
          data.NIndex = nchunk.GetMapIndex(point);
          flag = this.FlagNeighbourIfNeedsUpdate(ref data);
        }
      }
      return flag;
    }

    private bool FlagDownNeighbour(MapChunk nchunk, MapLightingByChunk.NeighbourSourceParams data)
    {
      bool flag = false;
      Point3D point = new Point3D();
      for (point.X = 0; point.X < this.chunkSize.X && !flag; ++point.X)
      {
        for (point.Z = 0; point.Z < this.chunkSize.Z && !flag; ++point.Z)
        {
          point.Y = 0;
          data.Index = this.chunk.GetMapIndex(point);
          point.Y = this.chunkSize.Y - 1;
          data.NIndex = nchunk.GetMapIndex(point);
          flag = this.FlagNeighbourIfNeedsUpdate(ref data);
        }
      }
      return flag;
    }

    private bool FlagNeighbourIfNeedsUpdate(ref MapLightingByChunk.NeighbourSourceParams data)
    {
      int index = data.Index;
      int nindex = data.NIndex;
      byte opacity = this.map.GetOpacity(data.NBlockCache[data.NBlockCacheIndex + nindex]);
      if ((int) opacity < this.maxLight)
      {
        MapLight mapLight1 = MapLight.FromByte(this.lightCache[this.lightCacheIndex + index]);
        MapLight mapLight2 = MapLight.FromByte(data.NLightCache[data.NLightCacheIndex + nindex]);
        if ((int) mapLight1.SunLight > (int) opacity && (int) mapLight2.SunLight < (int) mapLight1.SunLight - (int) opacity || (int) mapLight1.BlockLight > (int) opacity && (int) mapLight2.BlockLight < (int) mapLight1.BlockLight - (int) opacity)
          return true;
      }
      return false;
    }

    private struct NeighbourSourceParams
    {
      public int Index;
      public byte[] NBlockCache;
      public short NBlockCacheID;
      public int NBlockCacheIndex;
      public byte[] NLightCache;
      public short NLightCacheID;
      public int NLightCacheIndex;
      public int NIndex;
      public BlockFace NeighbourID;
    }

    private delegate void AddSourcesDelegate(
      MapChunk nchunk,
      MapLightingByChunk.NeighbourSourceParams data);

    private delegate bool FlagNeighbourDelegate(
      MapChunk nchunk,
      MapLightingByChunk.NeighbourSourceParams data);
  }
}
