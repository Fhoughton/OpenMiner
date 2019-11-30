// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SaveMapHead
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  public class SaveMapHead
  {
    public GameDifficulty GameDifficulty = GameDifficulty.Normal;
    public bool GoodHash = true;
    public bool PvPCombat = true;
    public float XPMultiplier = 1f;
    public string TexturePack = "Original HD";
    public TerrainData TerrainData = new TerrainData();
    public BiomeParams BiomeParams = new BiomeParams();
    public int ExeVersion;
    public int SaveVersion;
    public int CreatedVersion;
    public string MapName;
    public string OwnerGamerTag;
    public long DateCreated;
    public long DateSaved;
    public bool IsAutoSave;
    public BoxInt TotalMapBound;
    public BoxInt CurrentMapBound;
    public Point3D RegionSize;
    public Point3D ChunkSize;
    public int MapSeed;
    public SaveFormat Format;
    public int GameType;
    public GameMode GameMode;
    public MapAttribute Attribute;
    public bool CombatEnabled;
    public bool FiniteMode;
    public bool PassiveMobs;
    public bool EnemyMobs;
    public bool KeepItemsOnDeath;
    public bool DayNightActive;
    public bool WeatherActive;
    public bool ClanProtection;
    public float WindFactor;
    public int DaysIntoGame;
    public int HoursSlept;
    public int UnusedInt1;
    public byte UnusedByte1;
    public int DepthReached;
    public float RatingStars;
    public int RatingCount;
    public bool SkillsEnabled;
    public bool SkillsLocal;
    public Permissions DefaultPermission;
    public int MaxPlayers;
    public int PrivateSlots;
    public short CombatLevelDifference;
    public List<string> ModNames;

    public int MapWidth
    {
      get
      {
        return this.TotalMapBound.Max.X - this.TotalMapBound.Min.X;
      }
    }

    public int MapHeight
    {
      get
      {
        return this.TotalMapBound.Max.Y - this.TotalMapBound.Min.Y;
      }
    }

    public bool Pre18
    {
      get
      {
        return this.CreatedVersion < 58;
      }
    }

    public SaveMapHead Clone()
    {
      SaveMapHead saveMapHead = new SaveMapHead()
      {
        ExeVersion = this.ExeVersion,
        SaveVersion = this.SaveVersion,
        CreatedVersion = this.CreatedVersion,
        MapName = this.MapName,
        OwnerGamerTag = this.OwnerGamerTag,
        DateCreated = this.DateCreated,
        DateSaved = this.DateSaved,
        IsAutoSave = this.IsAutoSave,
        TotalMapBound = this.TotalMapBound,
        CurrentMapBound = this.CurrentMapBound,
        RegionSize = this.RegionSize,
        ChunkSize = this.ChunkSize,
        MapSeed = this.MapSeed,
        Format = this.Format,
        GameType = this.GameType,
        GameMode = this.GameMode,
        Attribute = this.Attribute,
        GameDifficulty = this.GameDifficulty,
        PvPCombat = this.PvPCombat,
        CombatEnabled = this.CombatEnabled,
        FiniteMode = this.FiniteMode,
        PassiveMobs = this.PassiveMobs,
        EnemyMobs = this.EnemyMobs,
        KeepItemsOnDeath = this.KeepItemsOnDeath,
        DayNightActive = this.DayNightActive,
        WeatherActive = this.WeatherActive,
        WindFactor = this.WindFactor,
        DaysIntoGame = this.DaysIntoGame,
        HoursSlept = this.HoursSlept,
        UnusedInt1 = this.UnusedInt1,
        DepthReached = this.DepthReached,
        RatingStars = this.RatingStars,
        RatingCount = this.RatingCount,
        SkillsEnabled = this.SkillsEnabled,
        SkillsLocal = this.SkillsLocal,
        XPMultiplier = this.XPMultiplier,
        DefaultPermission = this.DefaultPermission
      };
      saveMapHead.UnusedInt1 = this.UnusedInt1;
      saveMapHead.UnusedByte1 = this.UnusedByte1;
      saveMapHead.TexturePack = this.TexturePack;
      saveMapHead.TerrainData = this.TerrainData.Clone();
      saveMapHead.BiomeParams = this.BiomeParams.Clone();
      saveMapHead.MaxPlayers = this.MaxPlayers;
      saveMapHead.PrivateSlots = this.PrivateSlots;
      saveMapHead.CombatLevelDifference = this.CombatLevelDifference;
      saveMapHead.ClanProtection = this.ClanProtection;
      if (this.ModNames != null && this.ModNames.Count > 0)
        saveMapHead.ModNames = new List<string>((IEnumerable<string>) this.ModNames);
      return saveMapHead;
    }

    public void ResetMapBounds()
    {
      if (this.GameMode == GameMode.DigDeep)
      {
        this.ChunkSize = new Point3D(32, 32, 32);
        this.RegionSize = new Point3D(512, 512, 512);
        this.TotalMapBound = new BoxInt()
        {
          Min = new GlobalPoint3D(0, 0, 0),
          Max = new GlobalPoint3D(this.RegionSize.X, this.RegionSize.Y * 6, this.RegionSize.Z)
        };
        this.CurrentMapBound = new BoxInt()
        {
          Min = new GlobalPoint3D(0, 0, 0),
          Max = new GlobalPoint3D(this.RegionSize.X, this.RegionSize.Y * 6, this.RegionSize.Z)
        };
      }
      else if (this.Attribute == MapAttribute.AvatarDesigner)
      {
        this.ChunkSize = new Point3D(32, 32, 32);
        this.RegionSize = new Point3D(256, 256, 256);
        this.TotalMapBound = new BoxInt()
        {
          Min = new GlobalPoint3D(0, 0, 0),
          Max = new GlobalPoint3D(32, 64, 32)
        };
        this.CurrentMapBound = new BoxInt()
        {
          Min = new GlobalPoint3D(0, 0, 0),
          Max = new GlobalPoint3D(32, 64, 32)
        };
      }
      else
      {
        this.ChunkSize = new Point3D(32, 32, 32);
        switch (this.TerrainData.Biome)
        {
          case BiomeType.Flat:
          case BiomeType.Infinite:
            this.RegionSize = new Point3D(256, 256, 256);
            this.TotalMapBound = new BoxInt()
            {
              Min = new GlobalPoint3D(-this.RegionSize.X * 2, 0, -this.RegionSize.Z * 2),
              Max = new GlobalPoint3D(this.RegionSize.X * 2, this.RegionSize.Y * 2, this.RegionSize.Z * 2)
            };
            this.CurrentMapBound = new BoxInt()
            {
              Min = new GlobalPoint3D(-this.RegionSize.X * 2, 0, -this.RegionSize.Z * 2),
              Max = new GlobalPoint3D(this.RegionSize.X * 2, this.RegionSize.Y * 2, this.RegionSize.Z * 2)
            };
            break;
          default:
            this.RegionSize = new Point3D(512, 512, 512);
            this.TotalMapBound = new BoxInt()
            {
              Min = GlobalPoint3D.Zero,
              Max = new GlobalPoint3D(this.RegionSize.X * 2, this.RegionSize.Y, this.RegionSize.Z * 2)
            };
            this.CurrentMapBound = new BoxInt()
            {
              Min = GlobalPoint3D.Zero,
              Max = new GlobalPoint3D(this.RegionSize.X * 2, this.RegionSize.Y, this.RegionSize.Z * 2)
            };
            break;
        }
      }
    }
  }
}
