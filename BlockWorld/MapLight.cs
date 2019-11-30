// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapLight
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using System;

namespace StudioForge.BlockWorld
{
  public struct MapLight : IComparable<MapLight>, IEquatable<MapLight>
  {
    public static MapLight Empty = new MapLight();
    public byte SunLight;
    public byte BlockLight;

    public byte GetMaxLight(Map map)
    {
      byte num = (byte) ((double) this.SunLight * (double) map.LightCycle);
      if ((int) num <= (int) this.BlockLight)
        return this.BlockLight;
      return num;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public static bool operator ==(MapLight a, MapLight b)
    {
      if ((int) a.SunLight == (int) b.SunLight)
        return (int) a.BlockLight == (int) b.BlockLight;
      return false;
    }

    public static bool operator !=(MapLight a, MapLight b)
    {
      if ((int) a.SunLight == (int) b.SunLight)
        return (int) a.BlockLight != (int) b.BlockLight;
      return true;
    }

    public static bool operator <(MapLight a, MapLight b)
    {
      if ((int) a.SunLight < (int) b.SunLight)
        return (int) a.BlockLight < (int) b.BlockLight;
      return false;
    }

    public static bool operator >(MapLight a, MapLight b)
    {
      if ((int) a.SunLight > (int) b.SunLight)
        return (int) a.BlockLight > (int) b.BlockLight;
      return false;
    }

    public static bool operator <(MapLight a, int b)
    {
      if ((int) a.SunLight < b)
        return (int) a.BlockLight < b;
      return false;
    }

    public static bool operator >(MapLight a, int b)
    {
      if ((int) a.SunLight <= b)
        return (int) a.BlockLight > b;
      return true;
    }

    public static bool operator <=(MapLight a, MapLight b)
    {
      if ((int) a.SunLight <= (int) b.SunLight)
        return (int) a.BlockLight <= (int) b.BlockLight;
      return false;
    }

    public static bool operator >=(MapLight a, MapLight b)
    {
      if ((int) a.SunLight >= (int) b.SunLight)
        return (int) a.BlockLight >= (int) b.BlockLight;
      return false;
    }

    public bool Equals(MapLight other)
    {
      if ((int) this.SunLight == (int) other.SunLight)
        return (int) this.BlockLight == (int) other.BlockLight;
      return false;
    }

    public int CompareTo(MapLight other)
    {
      int num = (int) this.SunLight - (int) other.SunLight;
      if (num == 0)
        num = (int) this.BlockLight - (int) other.BlockLight;
      return num;
    }

    public static MapLight FromUShort(ushort data)
    {
      return MapLight.FromByte((byte) ((uint) data >> 8));
    }

    public static MapLight FromByte(byte data)
    {
      return new MapLight()
      {
        SunLight = (byte) ((uint) data >> 4),
        BlockLight = (byte) ((uint) data & 15U)
      };
    }

    public byte ToByte()
    {
      return (byte) ((uint) this.BlockLight + ((uint) this.SunLight << 4));
    }

    public static byte ToByte(byte sunLight, byte blockLight)
    {
      return (byte) ((uint) blockLight + ((uint) sunLight << 4));
    }

    public Vector2 ToVector2(Map map)
    {
      return new Vector2((float) this.SunLight / map.MaxLight, (float) this.BlockLight / map.MaxLight);
    }

    public static byte GetBlockLight(byte light)
    {
      return (byte) ((uint) light & 15U);
    }

    public static byte GetSunLight(byte light)
    {
      return (byte) ((uint) light >> 4);
    }
  }
}
