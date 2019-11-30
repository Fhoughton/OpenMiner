// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapBlockX
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System;

namespace StudioForge.BlockWorld
{
  public struct MapBlockX : IComparable<MapBlockX>, IEquatable<MapBlockX>
  {
    public MapBlock Data;
    public GlobalPoint3D Point;

    public MapBlockX(MapBlock data, GlobalPoint3D p)
    {
      this.Data = data;
      this.Point = p;
    }

    public bool Equals(MapBlockX other)
    {
      if (this.Data == other.Data)
        return this.Point == other.Point;
      return false;
    }

    public int CompareTo(MapBlockX other)
    {
      int num = this.Data.CompareTo(other.Data);
      if (num == 0)
        num = this.Point.CompareTo(other.Point);
      return num;
    }
  }
}
