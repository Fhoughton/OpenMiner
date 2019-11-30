// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapLightingByPoint3
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System.Collections.Generic;

namespace StudioForge.BlockWorld
{
  public class MapLightingByPoint3
  {
    private List<GlobalPoint3D> addLights = new List<GlobalPoint3D>();
    protected Map map;
    protected GlobalPoint3D origin;
    protected MapBlock newBlockData;
    protected MapBlock oldBlockData;
    protected byte maxLight;
    protected GlobalPoint3D p;
    protected GlobalPoint3D mapBoundMin;
    protected GlobalPoint3D mapBoundMax;
    protected Point3D chunksize;
    protected bool isUpdatingSunlight;
    protected bool firstWrite;
    protected int recursionCount;

    public void Initialize(Map map, GlobalPoint3D p, MapBlock oldBlockData, MapBlock newBlockData)
    {
      this.map = map;
      this.origin = p;
      this.oldBlockData = oldBlockData;
      this.newBlockData = newBlockData;
      this.chunksize = map.ChunkSize;
      this.maxLight = (byte) map.MaxLight;
      this.mapBoundMin = map.MapBound.Min;
      this.mapBoundMax = map.MapBound.Max;
      this.InitializeCore();
    }

    protected virtual void InitializeCore()
    {
    }

    public void Update()
    {
      try
      {
        do
          ;
        while (this.map.IsLightingInProgress);
        this.map.IsLightingInProgress = true;
        this.isUpdatingSunlight = true;
        this.UpdateCore();
        this.isUpdatingSunlight = false;
        this.UpdateCore();
      }
      finally
      {
        this.map.IsLightingInProgress = false;
      }
    }

    private void UpdateCore()
    {
      this.p = this.origin;
      byte blockId = this.newBlockData.BlockID;
      byte opacity = this.map.GetOpacity(blockId);
      byte num1 = this.GetLight();
      byte light;
      if (this.isUpdatingSunlight)
      {
        if (this.p.Y >= (int) this.map.GetHeightForLighting(this.p))
        {
          light = this.maxLight;
          if (this.map.IsBlockAffectSunlightForHeightCalculation(blockId))
            light = (int) opacity >= (int) light ? (byte) 0 : (byte) ((int) light - (int) opacity);
        }
        else
        {
          byte neighbourSunLight = this.map.GetMaxNeighbourSunLight(this.p, this.p);
          light = (int) opacity >= (int) neighbourSunLight ? (byte) 0 : (byte) ((int) neighbourSunLight - (int) opacity);
        }
      }
      else
      {
        byte neighbourBlockLight = this.map.GetMaxNeighbourBlockLight(this.p, this.p);
        light = (int) opacity >= (int) neighbourBlockLight ? (byte) 0 : (byte) ((int) neighbourBlockLight - (int) opacity);
        byte luminance = this.map.GetLuminance(ref this.p, this.newBlockData.BlockID);
        byte num2 = this.MustRefreshLight(this.newBlockData.BlockID) ? (byte) 0 : this.map.GetLuminance(ref this.p, this.oldBlockData.BlockID);
        if ((int) luminance > (int) light || (int) light < (int) num2)
          light = luminance;
        if ((int) num2 > (int) num1)
          num1 = num2;
      }
      this.firstWrite = true;
      if ((int) light > (int) num1)
        this.AddLight(ref this.p, light);
      else if ((int) light < (int) num1)
      {
        this.RemoveLight(ref this.p, light);
        foreach (GlobalPoint3D addLight in this.addLights)
        {
          this.p = addLight;
          this.AddLight(ref this.p, this.GetLight());
        }
        this.addLights.Clear();
      }
      else
      {
        MapChunk chunk = this.map.GetChunk(this.p);
        if (chunk == null)
          return;
        int num2 = this.p.X % this.chunksize.X;
        int num3 = this.p.Y % this.chunksize.Y;
        int num4 = this.p.Z % this.chunksize.Z;
        if (num2 == 0)
          chunk.LeftNeighbour()?.SetChunkFlag(ChunkFlags.MeshDirty);
        if (num3 == 0)
          chunk.DownNeighbour()?.SetChunkFlag(ChunkFlags.MeshDirty);
        if (num4 == 0)
          chunk.ForwardNeighbour()?.SetChunkFlag(ChunkFlags.MeshDirty);
        if (num2 == this.chunksize.X - 1)
          chunk.RightNeighbour()?.SetChunkFlag(ChunkFlags.MeshDirty);
        if (num3 == this.chunksize.Y - 1)
          chunk.UpNeighbour()?.SetChunkFlag(ChunkFlags.MeshDirty);
        if (num4 == this.chunksize.Z - 1)
          chunk.BackwardNeighbour()?.SetChunkFlag(ChunkFlags.MeshDirty);
        chunk.SetChunkFlag(ChunkFlags.MeshDirty);
      }
    }

    protected virtual bool MustRefreshLight(byte blockID)
    {
      return false;
    }

    private void AddLight(ref GlobalPoint3D op, byte light)
    {
      this.WriteLight(light);
      if (++this.recursionCount < 340)
      {
        GlobalPoint3D p = this.p;
        if (this.p.X > this.mapBoundMin.X && this.p.X - 1 != op.X)
        {
          --this.p.X;
          if (!this.AddLightCore(ref p, light))
            this.FlagLeftNeighbour();
          ++this.p.X;
        }
        if (this.p.X < this.mapBoundMax.X - 1 && this.p.X + 1 != op.X)
        {
          ++this.p.X;
          if (!this.AddLightCore(ref p, light))
            this.FlagRightNeighbour();
          --this.p.X;
        }
        if (this.p.Z > this.mapBoundMin.Z && this.p.Z - 1 != op.Z)
        {
          --this.p.Z;
          if (!this.AddLightCore(ref p, light))
            this.FlagForwardNeighbour();
          ++this.p.Z;
        }
        if (this.p.Z < this.mapBoundMax.Z - 1 && this.p.Z + 1 != op.Z)
        {
          ++this.p.Z;
          if (!this.AddLightCore(ref p, light))
            this.FlagBackNeighbour();
          --this.p.Z;
        }
        if (this.p.Y > this.mapBoundMin.Y && this.p.Y - 1 != op.Y)
        {
          --this.p.Y;
          if (!this.AddLightCore(ref p, light))
            this.FlagDownNeighbour();
          ++this.p.Y;
        }
        if (this.p.Y < this.mapBoundMax.Y - 1 && this.p.Y + 1 != op.Y)
        {
          ++this.p.Y;
          if (!this.AddLightCore(ref p, light))
            this.FlagUpNeighbour();
          --this.p.Y;
        }
      }
      else
        this.map.GetChunk(op).SetChunkFlag(ChunkFlags.LightDirty);
      --this.recursionCount;
    }

    private bool AddLightCore(ref GlobalPoint3D op, byte light)
    {
      byte light1 = this.GetLight();
      if ((int) light1 >= (int) light)
        return false;
      byte blockId = this.map.GetBlockID(this.p);
      byte opacity = this.map.GetOpacity(blockId);
      byte light2 = (int) opacity >= (int) light ? (byte) 0 : (byte) ((int) light - (int) opacity);
      if (this.isUpdatingSunlight && this.p.Y >= (int) this.map.GetHeightForLighting(this.p))
        light2 = this.p.Y == op.Y || this.map.IsBlockAffectSunlightForHeightCalculation(blockId) ? (blockId == (byte) 0 || (int) blockId == (int) this.map.InvisibleBarrierID ? this.maxLight : ((int) opacity >= (int) this.maxLight ? (byte) 0 : (byte) ((uint) this.maxLight - (uint) opacity))) : this.maxLight;
      if ((int) light2 <= (int) light1)
        return false;
      this.AddLight(ref op, light2);
      return true;
    }

    private void RemoveLight(ref GlobalPoint3D op, byte light)
    {
      this.WriteLight(light);
      if (++this.recursionCount < 500)
      {
        GlobalPoint3D p = this.p;
        if (this.p.X > this.mapBoundMin.X && this.p.X - 1 != op.X)
        {
          --this.p.X;
          if (!this.RemoveLightCore(ref p, light))
            this.FlagLeftNeighbour();
          ++this.p.X;
        }
        if (this.p.X < this.mapBoundMax.X - 1 && this.p.X + 1 != op.X)
        {
          ++this.p.X;
          if (!this.RemoveLightCore(ref p, light))
            this.FlagRightNeighbour();
          --this.p.X;
        }
        if (this.p.Z > this.mapBoundMin.Z && this.p.Z - 1 != op.Z)
        {
          --this.p.Z;
          if (!this.RemoveLightCore(ref p, light))
            this.FlagForwardNeighbour();
          ++this.p.Z;
        }
        if (this.p.Z < this.mapBoundMax.Z - 1 && this.p.Z + 1 != op.Z)
        {
          ++this.p.Z;
          if (!this.RemoveLightCore(ref p, light))
            this.FlagBackNeighbour();
          --this.p.Z;
        }
        if (this.p.Y > this.mapBoundMin.Y && this.p.Y - 1 != op.Y)
        {
          --this.p.Y;
          if (!this.RemoveLightCore(ref p, light))
            this.FlagDownNeighbour();
          ++this.p.Y;
        }
        if (this.p.Y < this.mapBoundMax.Y - 1 && this.p.Y + 1 != op.Y)
        {
          ++this.p.Y;
          if (!this.RemoveLightCore(ref p, light))
            this.FlagUpNeighbour();
          --this.p.Y;
        }
      }
      else
        this.map.GetChunk(op).SetChunkFlag(ChunkFlags.LightDirty);
      --this.recursionCount;
    }

    private bool RemoveLightCore(ref GlobalPoint3D op, byte light)
    {
      if (this.isUpdatingSunlight && this.p.Y >= (int) this.map.GetHeightForLighting(this.p))
        return false;
      byte light1 = this.GetLight();
      byte blockId = this.map.GetBlockID(this.p);
      byte opacity = this.map.GetOpacity(blockId);
      byte light2 = (int) opacity >= (int) light ? (byte) 0 : (byte) ((int) light - (int) opacity);
      if ((int) light2 >= (int) light1)
        return false;
      byte num1 = this.isUpdatingSunlight ? this.map.GetMaxNeighbourSunLight(this.p, op) : this.map.GetMaxNeighbourBlockLight(this.p, op);
      byte num2 = (int) opacity < (int) num1 ? (byte) ((uint) num1 - (uint) opacity) : (byte) 0;
      byte luminance = this.map.GetLuminance(ref this.p, blockId);
      if ((int) luminance > (int) light2 || (int) num2 == (int) light1)
        light2 = luminance;
      if ((int) num2 > (int) light2 && (int) num2 >= (int) light1)
        this.addLights.Add(this.p);
      else if ((int) light2 != (int) light1)
      {
        if (this.isUpdatingSunlight && (int) num2 > (int) light2 && (int) num2 + (int) opacity == (int) this.maxLight)
          light2 = num2;
        this.RemoveLight(ref op, light2);
      }
      return true;
    }

    private byte GetLight(MapLight light)
    {
      if (!this.isUpdatingSunlight)
        return light.BlockLight;
      return light.SunLight;
    }

    private byte GetLight()
    {
      if (!this.isUpdatingSunlight)
        return this.map.GetBlockLight(this.p);
      return this.map.GetSunLight(this.p);
    }

    private void WriteLight(byte light)
    {
      MapChunk chunk = this.map.GetChunk(this.p);
      int mapIndex = chunk.GetMapIndex(this.p);
      byte data = chunk.LightData.GetData(chunk, mapIndex);
      byte num = !this.isUpdatingSunlight ? (byte) (((int) data & 240) + ((int) light & 15)) : (byte) (((int) data & 15) + ((int) light << 4));
      if ((int) num != (int) data)
      {
        chunk.LightData.SetData(chunk, mapIndex, num);
        if (chunk.LastBlockEditedIndex == -1)
          chunk.LastBlockEditedIndex = mapIndex;
        chunk.SetChunkFlag(ChunkFlags.MeshDirty);
        this.firstWrite = false;
      }
      else
      {
        if (!this.firstWrite)
          return;
        chunk.SetChunkFlag(ChunkFlags.MeshDirty);
        this.firstWrite = false;
      }
    }

    private void FlagLeftNeighbour()
    {
      if ((this.p.X + 1) % this.chunksize.X != 0)
        return;
      this.FlagNeighbour();
    }

    private void FlagRightNeighbour()
    {
      if (this.p.X % this.chunksize.X != 0)
        return;
      this.FlagNeighbour();
    }

    private void FlagForwardNeighbour()
    {
      if ((this.p.Z + 1) % this.chunksize.Z != 0)
        return;
      this.FlagNeighbour();
    }

    private void FlagBackNeighbour()
    {
      if (this.p.Z % this.chunksize.Z != 0)
        return;
      this.FlagNeighbour();
    }

    private void FlagDownNeighbour()
    {
      if ((this.p.Y + 1) % this.chunksize.Y != 0)
        return;
      this.FlagNeighbour();
    }

    private void FlagUpNeighbour()
    {
      if (this.p.Y % this.chunksize.Y != 0)
        return;
      this.FlagNeighbour();
    }

    private void FlagNeighbour()
    {
      MapChunk chunk = this.map.GetChunk(this.p);
      if (chunk == null)
        return;
      chunk.SetChunkFlag(ChunkFlags.MeshDirty);
      if (chunk.LastBlockEditedIndex != -1)
        return;
      chunk.LastBlockEditedIndex = chunk.GetMapIndex(this.p);
    }
  }
}
