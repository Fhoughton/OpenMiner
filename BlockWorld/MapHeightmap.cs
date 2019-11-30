// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapHeightmap
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System;

namespace StudioForge.BlockWorld
{
  public class MapHeightmap
  {
    public Map Map;
    public GlobalPoint3D Offset;
    public ushort[] HeightMap;
    public ushort[] HeightMap1;
    private Point3D hgtmapSize;

    public override int GetHashCode()
    {
      return MapRegion.GetHashCode(this.Map, this.Offset);
    }

    public Point3D RowSize
    {
      get
      {
        return this.hgtmapSize;
      }
    }

    public virtual int MemorySize
    {
      get
      {
        return 32 + this.HeightMap.Length * 2;
      }
    }

    public int GetIndex(GlobalPoint3D p)
    {
      return p.X - this.Offset.X + (p.Z - this.Offset.Z) * this.hgtmapSize.X;
    }

    public void Initialize(Map map, GlobalPoint3D offset, Point3D mapSize, ushort[] hgt)
    {
      this.Map = map;
      offset.Y = 0;
      this.Offset = offset;
      this.hgtmapSize = mapSize;
      this.HeightMap = hgt;
      this.HeightMap1 = new ushort[hgt.Length];
      Array.Copy((Array) hgt, (Array) this.HeightMap1, hgt.Length);
    }

    public void Initialize(Map map, GlobalPoint3D offset, ushort defaultValue)
    {
      this.Map = map;
      offset.Y = 0;
      this.Offset = offset;
      this.hgtmapSize = new Point3D(Math.Min(map.MapSize.X, map.RegionSize.X), 0, Math.Min(map.MapSize.Z, map.RegionSize.Z));
      if (this.HeightMap == null || this.HeightMap.Length != this.hgtmapSize.X * this.hgtmapSize.Z)
      {
        this.HeightMap = new ushort[this.hgtmapSize.X * this.hgtmapSize.Z];
        this.HeightMap1 = new ushort[this.hgtmapSize.X * this.hgtmapSize.Z];
        if (defaultValue == (ushort) 0)
          return;
        this.SetHeight(defaultValue);
      }
      else
        this.SetHeight(defaultValue);
    }

    public virtual void Pregenerate(Map map)
    {
      this.Map = map;
      this.hgtmapSize = new Point3D(Math.Min(map.MapSize.X, map.RegionSize.X), 0, Math.Min(map.MapSize.Z, map.RegionSize.Z));
      this.HeightMap = new ushort[this.hgtmapSize.X * this.hgtmapSize.Z];
      this.HeightMap1 = new ushort[this.hgtmapSize.X * this.hgtmapSize.Z];
    }

    public void UnloadContent()
    {
    }

    public ushort GetHeight(int x, int z)
    {
      return (ushort) ((uint) this.HeightMap[x - this.Offset.X + (z - this.Offset.Z) * this.hgtmapSize.X] & (uint) short.MaxValue);
    }

    public ushort GetHeightForLighting(int x, int z)
    {
      return this.HeightMap1[x - this.Offset.X + (z - this.Offset.Z) * this.hgtmapSize.X];
    }

    public ushort GetHeightLocal(int x, int z)
    {
      return (ushort) ((uint) this.HeightMap[x + z * this.hgtmapSize.X] & (uint) short.MaxValue);
    }

    public ushort GetHeightLocalForLighting(int x, int z)
    {
      return this.HeightMap1[x + z * this.hgtmapSize.X];
    }

    public void SetHeight(int x, int z, ushort h, ushort h1)
    {
      int index = x - this.Offset.X + (z - this.Offset.Z) * this.hgtmapSize.X;
      if ((int) h1 != (int) h)
        h |= (ushort) 32768;
      this.HeightMap[index] = h;
      this.HeightMap1[index] = h1;
    }

    public void SetHeight(ushort h)
    {
      for (int index = 0; index < this.HeightMap.Length; ++index)
      {
        this.HeightMap[index] = h;
        this.HeightMap1[index] = h;
      }
    }

    public void SetHeightLocal(int x, int z, ushort h, ushort h1)
    {
      int index = x + z * this.hgtmapSize.X;
      if ((int) h1 != (int) h)
        h |= (ushort) 32768;
      this.HeightMap[index] = h;
      this.HeightMap1[index] = h1;
    }
  }
}
