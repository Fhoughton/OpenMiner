// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Zone
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;

namespace StudioForge.TotalMiner
{
  public class Zone
  {
    public float SpeedMultiplier = 1f;
    public float GravityMultiplier = 1f;
    public const float MaxEdgeMinus = 0.01f;
    public string Name;
    public GamerID GamerID;
    public ZoneType ZoneType;
    public GlobalPoint3D Min;
    public GlobalPoint3D Max;
    public ZoneBuilderType BuilderType;
    public string Builder;
    public string OnEntryScriptName;
    public string OnExitScriptName;
    public short CombatLevelDifference;

    public bool HasOtherOptions
    {
      get
      {
        if ((this.Builder == null || this.Builder.Length <= 0) && (this.OnEntryScriptName == null || this.OnEntryScriptName.Length <= 0) && ((this.OnExitScriptName == null || this.OnExitScriptName.Length <= 0) && (this.CombatLevelDifference == (short) 0 && (double) this.SpeedMultiplier == 1.0)))
          return (double) this.GravityMultiplier != 1.0;
        return true;
      }
    }

    public Zone(string name, ZoneType type, GlobalPoint3D min, GlobalPoint3D max)
    {
      this.Name = name;
      this.ZoneType = type;
      this.Min = min;
      this.Max = max;
      this.GamerID = GamerID.Sys1;
    }

    public bool IsInZone(Map map, BoundingBox box)
    {
      float tileSize = map.TileSize;
      BoundingBox box1 = new BoundingBox();
      float num = 0.075f;
      box1.Min.X = (float) this.Min.X * tileSize + num;
      box1.Min.Y = (float) this.Min.Y * tileSize + num;
      box1.Min.Z = (float) this.Min.Z * tileSize + num;
      box1.Max.X = (float) (this.Max.X + 1) * tileSize - num;
      box1.Max.Y = (float) (this.Max.Y + 1) * tileSize - num;
      box1.Max.Z = (float) (this.Max.Z + 1) * tileSize - num;
      return box.Intersects(box1);
    }

    public bool IsInZone(MapOld map, BoundingBox box)
    {
      float tileSize = map.TileSize;
      return box.Intersects(new BoundingBox()
      {
        Min = {
          X = (float) this.Min.X * tileSize,
          Y = (float) this.Min.Y * tileSize,
          Z = (float) this.Min.Z * tileSize
        },
        Max = {
          X = (float) ((double) (this.Max.X + 1) * (double) tileSize - 0.00999999977648258),
          Y = (float) ((double) (this.Max.Y + 1) * (double) tileSize - 0.00999999977648258),
          Z = (float) ((double) (this.Max.Z + 1) * (double) tileSize - 0.00999999977648258)
        }
      });
    }

    public bool Contains(Zone zone)
    {
      if (zone.Min.X >= this.Min.X && zone.Max.X <= this.Max.X && (zone.Min.Y >= this.Min.Y && zone.Max.Y <= this.Max.Y) && zone.Min.Z >= this.Min.Z)
        return zone.Max.Z <= this.Max.Z;
      return false;
    }

    public bool HasZoneType(ZoneType type)
    {
      return (this.ZoneType & type) == type;
    }

    public void ToggleType(ZoneType type)
    {
      if (this.HasZoneType(type))
        this.ZoneType &= ~type;
      else
        this.ZoneType |= type;
    }

    public void SetType(ZoneType type, bool on)
    {
      if (!on)
        this.ZoneType &= ~type;
      else
        this.ZoneType |= type;
    }
  }
}
