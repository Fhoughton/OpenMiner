// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapBlock
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System;

namespace StudioForge.BlockWorld
{
  public struct MapBlock : IComparable<MapBlock>, IEquatable<MapBlock>
  {
    public static MapBlock Empty = new MapBlock();
    public byte BlockID;
    public byte AuxData;
    public MapLight Light;
    public MapChunk Chunk;

    public static bool operator ==(MapBlock a, MapBlock b)
    {
      if ((int) a.BlockID == (int) b.BlockID && (int) a.AuxData == (int) b.AuxData)
        return a.Light == b.Light;
      return false;
    }

    public static bool operator !=(MapBlock a, MapBlock b)
    {
      if ((int) a.BlockID == (int) b.BlockID && (int) a.AuxData == (int) b.AuxData)
        return a.Light != b.Light;
      return true;
    }

    public bool Equals(MapBlock other)
    {
      if ((int) this.BlockID == (int) other.BlockID && (int) this.AuxData == (int) other.AuxData)
        return this.Light == other.Light;
      return false;
    }

    public override bool Equals(object obj)
    {
      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public int CompareTo(MapBlock other)
    {
      return (int) this.BlockID - (int) other.BlockID;
    }
  }
}
