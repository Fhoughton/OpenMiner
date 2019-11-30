// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.MapLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Storage
{
  internal class MapLoader
  {
    private IProgressBar progressBar;

    public SaveDataResult Load(
      GameInstance instance,
      string headerPath,
      bool isNew,
      IProgressBar progressBar)
    {
      this.progressBar = progressBar;
      if (isNew)
        return this.NewMap(instance, progressBar);
      try
      {
        SaveDataResult data = this.LoadHeaderAndGameData(headerPath + "header.dat", headerPath);
        if (data.SaveData.Header.SaveVersion < 55)
          throw new MapLoader.BadVersionOnLoad();
        this.LoadGameNewFormat(instance, data, progressBar);
        return data;
      }
      catch (MapLoader.BadVersionOnLoad ex)
      {
        throw ex;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(98, ex);
      }
      return (SaveDataResult) null;
    }

    private SaveDataResult LoadHeaderAndGameData(string filename, string path)
    {
      SaveDataResult saveDataResult = new SaveDataResult();
      byte[] numArray1 = MapLoader.ReadFileIntoBuffer(filename);
      using (MemoryStream memoryStream = new MemoryStream(numArray1, false))
      {
        using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
        {
          saveDataResult.SaveData = MapLoader.ReadMapHeaderAndGameData(reader);
          if (!Globals2.CheckHash(numArray1, saveDataResult.SaveData.Header.SaveVersion))
            saveDataResult.SaveData.Header.GoodHash = false;
        }
      }
      saveDataResult.SerializedData = MapSerializer.Deserialize(path + "header.bin");
      if (saveDataResult.SaveData.Header.SaveVersion > 126)
      {
        byte[] numArray2 = MapLoader.ReadFileIntoBuffer(path + "player.dat");
        using (MemoryStream memoryStream = new MemoryStream(numArray2, false))
        {
          using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
          {
            saveDataResult.SaveData.PlayerState = MapLoader.ReadPlayerDataCore(reader, saveDataResult.SaveData, saveDataResult.SaveData.Header.SaveVersion);
            if (!Globals2.CheckHash(numArray2, saveDataResult.SaveData.Header.SaveVersion))
              saveDataResult.SaveData.Header.GoodHash = false;
          }
        }
      }
      if (saveDataResult.SaveData.Header.SaveVersion > 137)
      {
        if (FileSystem.IsFileExist(path + "photos.dat"))
        {
          try
          {
            using (MemoryStream memoryStream = new MemoryStream(MapLoader.ReadFileIntoBuffer(path + "photos.dat"), false))
            {
              using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
                MapLoader.ReadPhotoThumbnails(reader, saveDataResult.SaveData.Header.SaveVersion);
            }
          }
          catch (Exception ex)
          {
            Services.ExceptionReporter.ReportExceptionCaught(121, ex);
          }
        }
      }
      MapLoader.FixData(saveDataResult.SaveData, saveDataResult.SaveData.Header.SaveVersion);
      return saveDataResult;
    }

    private static void FixData(SaveData data, int version)
    {
      if (version >= 152 || data.GameState.MapMarkers == null || data.PlayerState == null)
        return;
      List<MapMarker> mapMarkerList = new List<MapMarker>();
      for (int index = 0; index < data.GameState.MapMarkers.Length; ++index)
      {
        bool flag = false;
        MapMarker mapMarker = data.GameState.MapMarkers[index];
        foreach (SavePlayerState savePlayerState in data.PlayerState)
        {
          if (mapMarker.Label == savePlayerState.Gamertag)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          mapMarkerList.Add(mapMarker);
      }
      data.GameState.MapMarkers = mapMarkerList.ToArray();
    }

    private static byte[] ReadFileIntoBuffer(string filename)
    {
      if (!FileSystem.IsFileExist(filename))
        return (byte[]) null;
      using (Stream stream = FileSystem.OpenRead(filename))
      {
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public SaveDataResult NewMap(GameInstance instance, IProgressBar progressBar)
    {
      this.progressBar = progressBar;
      SaveData saveData = new SaveData()
      {
        Header = Globals2.GameProperties.SaveGame.Header,
        GameState = new SaveGameState(),
        PlayerState = new List<SavePlayerState>(),
        ArcadeState = new SaveArcadeState()
      };
      saveData.Header.ExeVersion = 27302;
      saveData.Header.SaveVersion = 294;
      saveData.Header.CreatedVersion = 294;
      saveData.Header.MapName = "New";
      saveData.Header.DateCreated = Utils.DateToBinary(DateTime.Now);
      saveData.Header.KeepItemsOnDeath = false;
      saveData.Header.DayNightActive = true;
      saveData.Header.WeatherActive = true;
      saveData.Header.WindFactor = saveData.Header.TerrainData.GroundBlock != Item.SpaceWorld ? 1f : 0.0f;
      saveData.Header.CombatLevelDifference = (short) 7;
      saveData.Header.ClanProtection = true;
      saveData.GameState.TotalGameTime = 0.0;
      saveData.GameState.SunRotation = -1.274447f;
      saveData.GameState.LockedTable = this.InitializeNewLockedTable(saveData.Header.GameMode);
      saveData.GameState.Particles = new List<SaveItemParticle>();
      saveData.GameState.Teleports = new List<SaveTeleportState>();
      saveData.GameState.Signs = new SaveSignsState();
      saveData.GameState.NPCs = new List<SaveNPCState>();
      saveData.GameState.LockedDoors = new List<SavePlayerBlockState>();
      saveData.GameState.SentryTurrets = new List<SaveSentryTurretState>();
      saveData.GameState.MineBlocks = new List<SaveMineBlockState>();
      saveData.GameState.Chests = new List<SaveChestState>();
      saveData.GameState.Furnaces = new List<SaveFurnaceState>();
      saveData.GameState.Bookcases = new List<SaveChestState>();
      saveData.GameState.ShopBlocks = new List<SaveShopBlockState>();
      saveData.GameState.Fire = new List<SaveFireState>();
      saveData.GameState.Blueprints = new BlueprintState[Blueprints.BlueprintList.Length];
      saveData.GameState.Scrolls = new WisdomScrollState[Wisdom.WisdomList.Length];
      saveData.GameState.Zones = new List<SaveZoneState>();
      saveData.GameState.Books = new List<SaveBookState>();
      saveData.GameState.SpawnInventory = new SaveInventoryState()
      {
        PackSize = (short) 20
      };
      saveData.GameState.AmbientSoundBlocks = new List<SaveAmbientSoundState>();
      saveData.GameState.ScriptBlocks = new List<SaveScriptBlockState>();
      saveData.GameState.Scripts = new List<Script>();
      saveData.GameState.AdventureScripts = new List<string>();
      saveData.GameState.EventScripts = new Dictionary<ScriptEvent, string>();
      Globals2.GameProperties.UseOldGenerator = false;
      SaveDataResult data = new SaveDataResult()
      {
        SaveData = saveData,
        SerializedData = new MapSerializer()
      };
      this.LoadGameNewFormat(instance, data, progressBar);
      return data;
    }

    private bool[] InitializeNewLockedTable(GameMode gamemode)
    {
      bool[] flagArray = new bool[Globals1.ItemData.Length];
      for (int i = 0; i < flagArray.Length; ++i)
        flagArray[i] = Globals1.IsLocked(i, gamemode);
      return flagArray;
    }

    public byte[] LoadGameData(Stream stream)
    {
      using (BinaryReader reader = new BinaryReader(stream))
      {
        MapLoader.ReadMapHeaderAndGameData(reader);
        byte[] buffer = new byte[stream.Position];
        stream.Position = 0L;
        stream.Read(buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public static SaveData LoadMapDataExternal(string path)
    {
      using (Stream input = FileSystem.OpenRead(path + "header.dat"))
      {
        using (BinaryReader reader = new BinaryReader(input))
          return MapLoader.ReadMapHeaderAndGameData(reader);
      }
    }

    public static SaveMapHead LoadMapHeader(string filename)
    {
      byte[] data = MapLoader.ReadFileIntoBuffer(filename);
      if (data == null)
        throw new CorruptWorldFileException();
      SaveMapHead saveMapHead = MapLoader.LoadMapHeader(data);
      saveMapHead.GoodHash = Globals2.CheckHash(data, saveMapHead.SaveVersion);
      return saveMapHead;
    }

    public static SaveMapHead LoadMapHeader(Stream stream)
    {
      using (BinaryReader reader = new BinaryReader(stream))
        return MapLoader.ReadMapHeader(reader);
    }

    public static SaveMapHead LoadMapHeader(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream(data))
        return MapLoader.LoadMapHeader((Stream) memoryStream);
    }

    public SaveDataResult LoadMapForClient(
      GameInstance instance,
      byte[] gameData,
      IProgressBar progressBar)
    {
      if (instance != null && gameData != null)
      {
        using (MemoryStream memoryStream = new MemoryStream(gameData))
        {
          using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
          {
            this.progressBar = progressBar;
            SaveData data1 = MapLoader.ReadMapHeaderAndGameData(reader);
            MapLoader.ReadPlayerDataForClient(reader, data1, data1.Header.SaveVersion);
            if (data1 != null)
            {
              SaveDataResult data2 = new SaveDataResult()
              {
                SaveData = data1,
                SerializedData = new MapSerializer()
              };
              MapSerializer.DeserializeBinary(reader, data2.SerializedData);
              this.LoadGameNewFormat(instance, data2, progressBar);
              return data2;
            }
          }
        }
      }
      return (SaveDataResult) null;
    }

    private void LoadGameNewFormat(
      GameInstance instance,
      SaveDataResult data,
      IProgressBar progress)
    {
      Map.UncompressedCount = 0;
      Map.CompressedCount = 0;
      SaveMapHead header = data.SaveData.Header;
      header.Attribute = Globals2.GameProperties.SaveGame.Header.Attribute;
      header.GameDifficulty = Globals2.GameProperties.SaveGame.Header.GameDifficulty;
      header.CombatEnabled = Globals2.GameProperties.SaveGame.Header.CombatEnabled;
      header.PvPCombat = Globals2.GameProperties.SaveGame.Header.PvPCombat;
      header.SkillsEnabled = Globals2.GameProperties.SaveGame.Header.SkillsEnabled;
      header.SkillsLocal = Globals2.GameProperties.SaveGame.Header.SkillsLocal;
      header.TexturePack = Globals2.GameProperties.SaveGame.Header.TexturePack;
      header.TerrainData = Globals2.GameProperties.SaveGame.Header.TerrainData.Clone();
      header.MaxPlayers = Globals2.GameProperties.SaveGame.Header.MaxPlayers;
      header.PrivateSlots = Globals2.GameProperties.SaveGame.Header.PrivateSlots;
      header.TotalMapBound = Globals2.GameProperties.SaveGame.Header.TotalMapBound;
      header.CurrentMapBound = Globals2.GameProperties.SaveGame.Header.CurrentMapBound;
      Globals2.GameProperties.SaveGame.Header = header;
      bool isInfinite = header.TerrainData.Biome == BiomeType.Infinite || header.MapWidth > 1024;
      MapTM map = new MapTM(instance, "GameWorld", 1f, isInfinite, header.TotalMapBound, header.CurrentMapBound, header.RegionSize, header.ChunkSize, Globals1.BlockData, 15, header.MapSeed, (ushort) 4, 128, (MapStrategy) new MapStrategyTM(instance), instance.IsHost, true);
      map.SeaLevel = data.SaveData.Header.TerrainData.SeaLevel;
      MapLoader.SetMapProperties(header, (Map) map);
      if (progress != null && !Globals2.GameProperties.IsNewMap)
        progress.Factor *= 0.5f;
      map.PregenerateRegions(false, true, progress);
      Globals2.MapSeed = map.Seed;
      data.SaveData.Map = (Map) map;
      if (data.SaveData.Header.SaveVersion > 126 && map.IsHost)
      {
        int playerCount = data.SaveData.GameState.PlayerCount;
        int ratingCount = data.SaveData.Header.RatingCount;
        float ratingStars = data.SaveData.Header.RatingStars;
        data.SaveData.Header.RatingCount = 0;
        data.SaveData.Header.RatingStars = 0.0f;
        data.SaveData.GameState.PlayerCount = data.SaveData.PlayerState.Count;
        int num1 = 0;
        foreach (SavePlayerState savePlayerState in data.SaveData.PlayerState)
        {
          if (savePlayerState.RatingStars > (byte) 0)
          {
            num1 += (int) savePlayerState.RatingStars;
            ++data.SaveData.Header.RatingCount;
          }
        }
        if (data.SaveData.Header.RatingCount > 0)
          data.SaveData.Header.RatingStars = (float) num1 / (float) data.SaveData.Header.RatingCount;
        if (data.SaveData.Header.SaveVersion < 243)
        {
          int num2 = playerCount - data.SaveData.GameState.PlayerCount;
          int num3 = ratingCount - data.SaveData.Header.RatingCount;
          if (num2 > 20 || num3 > 10)
          {
            MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM(string.Format("Player Purge results:\nPlayers purged: {0}\nRatings before purge: {1}\nRatings after purge: {2}\nAverage stars before purge: {3:N2}\nAverage stars after purge: {4:N2}\n\nThis purge has removed players that were\nin this map for less than 2 minutes.", (object) num2, (object) ratingCount, (object) data.SaveData.Header.RatingCount, (object) ratingStars, (object) data.SaveData.Header.RatingStars), "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), (Player) null);
            TotalMinerGame.Instance.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, instance.ControllingPlayer);
          }
        }
      }
      if (!Globals2.GameProperties.IsNewMap && map.IsHost)
      {
        MapRegionLoader mapRegionLoader = new MapRegionLoader();
        if (progress != null)
          progress.Factor /= (float) map.Regions.Count;
        foreach (MapRegion mapRegion in map.Regions.Values)
          mapRegionLoader.LoadRegion(mapRegion as MapRegionTM, progress);
        if (progress != null)
          progress.Factor *= (float) map.Regions.Count;
      }
      map.SaveDataEnabled = true;
      MapLoader.ApplyChanges((Map) map, data);
      map.MapStrategy.Begin((Map) map);
      if (!header.Pre18)
      {
        switch (header.TerrainData.Biome)
        {
          case BiomeType.Flat:
            break;
          case BiomeType.Infinite:
            break;
          default:
            if (!header.BiomeParams.GenerateCaves)
              break;
            MapLoader.GenerateBlastPoints(instance, map, data.SaveData);
            break;
        }
      }
      else
      {
        if (map.IsHost)
          return;
        MapLoader.ClearHeightData(map, data.SaveData);
      }
    }

    private static void ClearHeightData(MapTM map, SaveData data)
    {
      foreach (KeyValuePair<int, MapHeightmap> heightMap in map.HeightMaps)
        heightMap.Value.SetHeight((ushort) 0);
    }

    private static void GenerateBlastPoints(GameInstance instance, MapTM map, SaveData data)
    {
      bool flag = Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.DigDeep;
      int minDepth = flag ? map.MapSize.Y / 10 : (int) map.SeaLevel / 3;
      int maxDepth = flag ? map.MapSize.Y / 4 : (int) map.SeaLevel;
      int treasureChestMinDepth = flag ? (int) map.SeaLevel - 200 : (int) map.SeaLevel - 50;
      int treasureChestChance = flag ? 80 : 40;
      Globals2.GameProperties.BlastPoints = new Dictionary<MapChunk, List<BlastPoint>>(1000);
      DungeonGenerator.CreateSurfaceDungeons(instance, map, data.Header.TerrainData.Biome, 10, minDepth, maxDepth, treasureChestMinDepth, treasureChestChance, Globals2.GameProperties.BlastPoints, (IProgressBar) null);
      if (Globals2.GameProperties.SaveGame.Header.GameMode != GameMode.DigDeep)
        return;
      DungeonGenerator.CreateLowerDungeons((Map) map, 35, Globals2.GameProperties.BlastPoints, (IProgressBar) null);
    }

    private static void LoadScrollData(SaveData data)
    {
      for (int index = 0; index < data.GameState.Scrolls.Length; ++index)
      {
        WisdomScrollState scroll = data.GameState.Scrolls[index];
        if (scroll != null && index < Wisdom.WisdomList.Length)
        {
          Wisdom.WisdomList[index].IsEnabled = scroll.IsEnabled;
          Wisdom.WisdomList[index].IsGenerated = scroll.IsGenerated;
          Wisdom.WisdomList[index].Point = scroll.Point;
        }
      }
    }

    private static void LoadBlueprintData(SaveData data)
    {
      for (int index = 0; index < data.GameState.Blueprints.Length; ++index)
      {
        BlueprintState blueprint = data.GameState.Blueprints[index];
        if (blueprint != null && index < Blueprints.BlueprintList.Length)
        {
          Blueprints.BlueprintList[index].IsEnabled = blueprint.IsEnabled;
          Blueprints.BlueprintList[index].IsUnearthed = blueprint.IsUnearthed;
          Blueprints.BlueprintList[index].IsGenerated = blueprint.IsGenerated;
          Blueprints.BlueprintList[index].Point = blueprint.Point;
        }
      }
    }

    public static void SetMapProperties(SaveMapHead header, Map map)
    {
      map.OutOfBoundsBlockID = header.Attribute == MapAttribute.AvatarDesigner ? (byte) 0 : byte.MaxValue;
      map.WaterBlockID = (byte) 11;
      map.LavaBlockID = (byte) 13;
      map.RopeBlockID = (byte) 72;
      map.BedrockID = (byte) 29;
      map.InvisibleBarrierID = (byte) 125;
    }

    public static SaveData ReadMapHeaderAndGameData(BinaryReader reader)
    {
      SaveData data = new SaveData();
      data.Header = MapLoader.ReadMapHeader(reader);
      int saveVersion = data.Header.SaveVersion;
      if (data.Header.GameMode != GameMode.Creative && Globals2.GameProperties != null && (Globals2.GameProperties.SaveGame != null && Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.Creative))
      {
        data.Header.GameMode = Globals2.GameProperties.SaveGame.Header.GameMode;
        data.Header.FiniteMode = false;
      }
      data.GameState = MapLoader.ReadGameState(reader, data, saveVersion);
      data.PlayerState = MapLoader.ReadPlayerData(reader, data, saveVersion);
      if (saveVersion < (int) byte.MaxValue)
        reader.ReadInt32();
      data.ArcadeState = MapLoader.ReadArcadeData(reader, saveVersion);
      data.GameSettings = new GameSettings();
      if (saveVersion > 277)
        data.GameSettings.ReadState(reader, saveVersion);
      return data;
    }

    private static SaveMapHead ReadMapHeader(BinaryReader reader)
    {
      SaveMapHead saveMapHead = new SaveMapHead();
      int version = saveMapHead.SaveVersion = reader.ReadInt32();
      saveMapHead.ExeVersion = reader.ReadInt32();
      if (version > 35)
        saveMapHead.CreatedVersion = reader.ReadInt32();
      saveMapHead.MapName = Globals2.StripBadChars(reader.ReadString());
      saveMapHead.OwnerGamerTag = Globals2.StripBadChars(reader.ReadString());
      saveMapHead.DateCreated = reader.ReadInt64();
      if (version > 134 && version < 211)
      {
        int num = (int) reader.ReadByte();
      }
      if (version > 208)
      {
        saveMapHead.DateSaved = Globals2.ValidateDateTime(reader.ReadInt64());
      }
      else
      {
        saveMapHead.DateSaved = saveMapHead.DateCreated;
        if (version > 39)
          reader.ReadInt32();
      }
      if (version > 31)
        saveMapHead.IsAutoSave = reader.ReadBoolean();
      if (version > 54)
      {
        saveMapHead.TotalMapBound.Min.X = reader.ReadInt32();
        saveMapHead.TotalMapBound.Min.Y = reader.ReadInt32();
        saveMapHead.TotalMapBound.Min.Z = reader.ReadInt32();
        saveMapHead.TotalMapBound.Max.X = reader.ReadInt32();
        saveMapHead.TotalMapBound.Max.Y = reader.ReadInt32();
        saveMapHead.TotalMapBound.Max.Z = reader.ReadInt32();
        if (version > 216)
        {
          saveMapHead.CurrentMapBound.Min.X = reader.ReadInt32();
          saveMapHead.CurrentMapBound.Min.Y = reader.ReadInt32();
          saveMapHead.CurrentMapBound.Min.Z = reader.ReadInt32();
          saveMapHead.CurrentMapBound.Max.X = reader.ReadInt32();
          saveMapHead.CurrentMapBound.Max.Y = reader.ReadInt32();
          saveMapHead.CurrentMapBound.Max.Z = reader.ReadInt32();
        }
        else
          saveMapHead.CurrentMapBound = saveMapHead.TotalMapBound;
        saveMapHead.RegionSize.X = (int) reader.ReadUInt16();
        saveMapHead.RegionSize.Y = (int) reader.ReadUInt16();
        saveMapHead.RegionSize.Z = (int) reader.ReadUInt16();
        saveMapHead.ChunkSize.X = (int) reader.ReadUInt16();
        saveMapHead.ChunkSize.Y = (int) reader.ReadUInt16();
        saveMapHead.ChunkSize.Z = (int) reader.ReadUInt16();
      }
      else
      {
        saveMapHead.ChunkSize = new Point3D(32, 32, 32);
        saveMapHead.RegionSize = saveMapHead.ChunkSize * 16;
        saveMapHead.TotalMapBound.Min.X = 0;
        saveMapHead.TotalMapBound.Min.Y = 0;
        saveMapHead.TotalMapBound.Min.Z = 0;
        saveMapHead.TotalMapBound.Max.X = reader.ReadInt32();
        saveMapHead.TotalMapBound.Max.Y = reader.ReadInt32();
        saveMapHead.TotalMapBound.Max.Z = reader.ReadInt32();
        saveMapHead.CurrentMapBound = saveMapHead.TotalMapBound;
      }
      saveMapHead.MapSeed = reader.ReadInt32();
      saveMapHead.Format = (SaveFormat) reader.ReadInt32();
      saveMapHead.GameType = reader.ReadInt32();
      int gameMode = reader.ReadInt32();
      saveMapHead.GameMode = (GameMode) gameMode;
      if (version < 55 && (saveMapHead.GameMode == GameMode.DigDeep || saveMapHead.TotalMapBound.Max.X == 0 || (saveMapHead.TotalMapBound.Max.Y == 0 || saveMapHead.TotalMapBound.Max.Z == 0)))
        saveMapHead.ResetMapBounds();
      saveMapHead.HoursSlept = reader.ReadInt32();
      if (version > 21)
        saveMapHead.RatingCount = reader.ReadInt32() / 144;
      saveMapHead.UnusedInt1 = reader.ReadInt32();
      saveMapHead.DepthReached = reader.ReadInt32();
      saveMapHead.Attribute = MapAttribute.Adventure;
      if (version > 21)
        saveMapHead.Attribute = (MapAttribute) reader.ReadInt32();
      if (version > 21)
        saveMapHead.RatingStars = reader.ReadSingle() / 4498.25f;
      int difficulty = 0;
      if (version > 20)
      {
        difficulty = reader.ReadInt32();
        saveMapHead.GameDifficulty = (GameDifficulty) difficulty;
      }
      saveMapHead.PvPCombat = version <= 23 || reader.ReadBoolean();
      saveMapHead.CombatEnabled = version <= 24 ? saveMapHead.GameMode != GameMode.Creative : reader.ReadBoolean();
      saveMapHead.FiniteMode = version <= 24 ? saveMapHead.GameMode != GameMode.Creative : reader.ReadBoolean();
      saveMapHead.PassiveMobs = version <= 118 || reader.ReadBoolean();
      saveMapHead.EnemyMobs = version <= 148 ? saveMapHead.GameMode == GameMode.DigDeep || saveMapHead.GameMode == GameMode.Survival : reader.ReadBoolean();
      saveMapHead.KeepItemsOnDeath = version > 128 && reader.ReadBoolean();
      saveMapHead.SkillsEnabled = version > 107 && reader.ReadBoolean();
      saveMapHead.SkillsLocal = version > 192 && reader.ReadBoolean();
      saveMapHead.XPMultiplier = version <= 192 ? 1f : reader.ReadSingle();
      saveMapHead.DayNightActive = version <= 24 || reader.ReadBoolean();
      if (version < 54)
        saveMapHead.DayNightActive = true;
      saveMapHead.WeatherActive = version <= 213 || reader.ReadBoolean();
      saveMapHead.WindFactor = version <= 232 ? 1f : reader.ReadSingle();
      saveMapHead.UnusedByte1 = version <= 75 ? (byte) 0 : reader.ReadByte();
      saveMapHead.DaysIntoGame = version <= 76 ? 0 : reader.ReadInt32();
      saveMapHead.DefaultPermission = version <= 34 ? Permissions.Fly : (Permissions) reader.ReadInt32();
      if (version < 231)
        saveMapHead.DefaultPermission |= Permissions.SystemShops | Permissions.TextChat;
      saveMapHead.MaxPlayers = version <= 121 ? 16 : reader.ReadInt32();
      saveMapHead.PrivateSlots = version <= 121 ? 0 : reader.ReadInt32();
      saveMapHead.CombatLevelDifference = version <= 184 ? (short) 7 : reader.ReadInt16();
      saveMapHead.ClanProtection = version <= 244 || reader.ReadBoolean();
      if (version > 22)
        saveMapHead.TexturePack = Globals2.StripBadChars(reader.ReadString(), true);
      if (!saveMapHead.FiniteMode && saveMapHead.GameMode != GameMode.Creative)
        saveMapHead.FiniteMode = true;
      saveMapHead.TerrainData = MapLoader.ReadMapTerrain(reader, saveMapHead, version);
      saveMapHead.BiomeParams.Initialize(saveMapHead.TerrainData.Biome, version);
      if (version > 182)
        saveMapHead.BiomeParams.ReadState(reader, version);
      if (version > 275)
        saveMapHead.ModNames = Globals1.ReadStringList(reader);
      MapLoader.FixHeaderResult(saveMapHead, gameMode, difficulty);
      return saveMapHead;
    }

    private static void FixHeaderResult(SaveMapHead result, int gameMode, int difficulty)
    {
      result.DefaultPermission &= ~(Permissions.Save | Permissions.Admin | Permissions.Grief);
      if (result.GameMode == GameMode.Survival && result.GameDifficulty == GameDifficulty.Peaceful)
        result.GameDifficulty = GameDifficulty.Easy;
      if (result.GameMode == GameMode.Peaceful)
        result.GameDifficulty = GameDifficulty.Peaceful;
      if (gameMode < 1 || gameMode > 4)
        result.GameMode = result.MapWidth == 1024 ? GameMode.Creative : GameMode.DigDeep;
      switch (difficulty)
      {
        case 0:
        case 1:
        case 2:
        case 3:
          int num = result.TerrainData.Biome == BiomeType.Infinite || result.TerrainData.Biome == BiomeType.Flat ? 256 : 512;
          if (result.RegionSize.X != num || result.RegionSize.Y != num || result.RegionSize.Z != num)
          {
            if (result.RegionSize.X > num)
              num = 512;
            result.RegionSize = new Point3D(num, num, num);
          }
          if (result.ChunkSize.X == 32 && result.ChunkSize.Y == 32 && result.ChunkSize.Z == 32)
            break;
          result.ChunkSize = new Point3D(32, 32, 32);
          break;
        default:
          result.GameDifficulty = GameDifficulty.Normal;
          goto case 0;
      }
    }

    private static TerrainData ReadMapTerrain(
      BinaryReader reader,
      SaveMapHead head,
      int version)
    {
      TerrainData result = new TerrainData();
      int num1 = version > 8 ? 0 : 1;
      int num2 = reader.ReadInt32() + num1;
      switch (num2)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
          result.Biome = (BiomeType) num2;
          result.GroundBlock = version > 133 ? (Item) reader.ReadUInt16() : (Item) reader.ReadByte();
          result.Iterations = reader.ReadInt32();
          result.MaxParticles = reader.ReadInt32();
          if (version > 19)
          {
            if (version < 57)
              reader.ReadInt32();
            if (version < 67)
            {
              result.SeaLevel = (ushort) reader.ReadInt32();
              if (version >= 55)
                reader.ReadInt32();
            }
            else
            {
              result.SeaLevel = reader.ReadUInt16();
              if (version < 68)
              {
                int num3 = (int) reader.ReadUInt16();
                result.SeaLevel = (ushort) 200;
              }
            }
            if (result.SeaLevel < (ushort) 0 || (int) result.SeaLevel > head.MapHeight)
              MapLoader.SetSeaLevel(head, result);
          }
          else
            MapLoader.SetSeaLevel(head, result);
          return result;
        default:
          num2 = 6;
          goto case 0;
      }
    }

    private static void SetSeaLevel(SaveMapHead head, TerrainData result)
    {
      result.SeaLevel = (ushort) 80;
    }

    private void ReadMapLightingUpdates(BinaryReader reader, SaveData data, int version)
    {
      if (data.Header.UnusedInt1 <= 0)
        return;
      byte[] buffer = new byte[data.Header.UnusedInt1 * 10];
      reader.Read(buffer, 0, buffer.Length);
    }

    private static SaveGameState ReadGameState(
      BinaryReader reader,
      SaveData data,
      int version)
    {
      SaveGameState data1 = new SaveGameState();
      if (version < 278)
      {
        double num1 = (double) reader.ReadSingle();
        double num2 = (double) reader.ReadSingle();
        double num3 = (double) reader.ReadSingle();
        if (version > 143)
        {
          int num4 = (int) reader.ReadByte();
        }
        if (version > 143)
          reader.ReadBoolean();
        if (version > 27)
        {
          double num5 = (double) reader.ReadSingle();
        }
        if (version > 31)
        {
          int num6 = (int) reader.ReadByte();
        }
        if (version > 177)
        {
          int num7 = (int) reader.ReadByte();
        }
        else
        {
          if (version > 32)
            reader.ReadBoolean();
          if (version > 176)
            reader.ReadBoolean();
        }
        data.GameSettings = new GameSettings();
      }
      data1.MaxConcurrentPlayerCount = version <= 42 ? 0 : (int) reader.ReadByte();
      if (data1.MaxConcurrentPlayerCount > 24)
        data1.MaxConcurrentPlayerCount = 1;
      if (version > 19)
        Globals1.ReadRandBuffer(reader);
      if (version > 19)
        data1.TotalGameTime = reader.ReadDouble();
      data1.SunRotation = version <= 53 ? -1.274447f : reader.ReadSingle();
      data1.PlayerCount = reader.ReadInt32();
      if (version < 80 && data1.PlayerCount == 0)
        data1.PlayerCount = 1;
      data1.LastTransmitterFrequency = reader.ReadInt32();
      if (version < 25)
        reader.ReadBoolean();
      data1.Blueprints = MapLoader.ReadBlueprintData(reader, version);
      if (version > 234)
      {
        data1.ItemsEnabled = MapLoader.ReadEnabledItemsTable(reader, data.Header, version);
        for (int index = 0; index < data1.ItemsEnabled.Length && index < Globals1.ItemData.Length; ++index)
          Globals1.ItemData[index].IsEnabled = data1.ItemsEnabled[index];
        for (int index = data1.ItemsEnabled.Length - 1; index < Globals1.ItemData.Length; ++index)
          Globals1.ItemData[index].IsEnabled = true;
        Globals1.ItemData[(int) byte.MaxValue].IsEnabled = false;
        Globals1.ItemData[594].IsEnabled = false;
        if (version < 251)
        {
          Globals1.ItemData[182].IsEnabled = true;
          Globals1.ItemData[571].IsEnabled = true;
          Globals1.ItemData[183].IsEnabled = true;
          Globals1.ItemData[572].IsEnabled = true;
          Globals1.ItemData[184].IsEnabled = true;
          Globals1.ItemData[573].IsEnabled = true;
          Globals1.ItemData[185].IsEnabled = true;
          Globals1.ItemData[223].IsEnabled = true;
          Globals1.ItemData[591].IsEnabled = true;
          Globals1.ItemData[592].IsEnabled = true;
          Globals1.ItemData[574].IsEnabled = true;
          Globals1.ItemData[575].IsEnabled = true;
          Globals1.ItemData[217].IsEnabled = true;
          Globals1.ItemData[218].IsEnabled = true;
          Globals1.ItemData[219].IsEnabled = true;
          Globals1.ItemData[220].IsEnabled = true;
          Globals1.ItemData[221].IsEnabled = true;
          Globals1.ItemData[222].IsEnabled = true;
          Globals1.ItemData[224].IsEnabled = true;
          Globals1.ItemData[225].IsEnabled = true;
          Globals1.ItemData[226].IsEnabled = true;
          Globals1.ItemData[227].IsEnabled = true;
          Globals1.ItemData[228].IsEnabled = true;
        }
      }
      if (version < 107)
      {
        if (version < 105)
          data1.Chests = MapLoader.ReadChestData(reader, version);
        if (version < 105)
          data1.Furnaces = MapLoader.ReadFurnaceData(reader, version);
        data1.Bookcases = version <= 64 ? new List<SaveChestState>() : MapLoader.ReadChestData(reader, version);
        if (version < 106)
          data1.LockedDoors = MapLoader.ReadLockedDoorData(reader, version);
        if (version < 106)
          data1.SentryTurrets = MapLoader.ReadSentryTurretData(reader, version);
        if (version < 106)
          data1.MineBlocks = MapLoader.ReadMineBlockData(reader, version);
        if (version < 106)
          data1.ShopBlocks = MapLoader.ReadShopBlockData(reader, version);
        if (version < 106)
          data1.Fire = MapLoader.ReadFireData(reader, version);
        if (version < 106)
          data1.Signs = MapLoader.ReadSignData(reader, version);
        if (version < 106)
          data1.NPCs = MapLoader.ReadNPCData(reader, version);
      }
      else
      {
        data1.Chests = new List<SaveChestState>();
        data1.Furnaces = new List<SaveFurnaceState>();
        data1.Bookcases = new List<SaveChestState>();
        data1.LockedDoors = new List<SavePlayerBlockState>();
        data1.SentryTurrets = new List<SaveSentryTurretState>();
        data1.MineBlocks = new List<SaveMineBlockState>();
        data1.ShopBlocks = new List<SaveShopBlockState>();
        data1.Fire = new List<SaveFireState>();
        data1.Signs = new SaveSignsState();
        data1.NPCs = new List<SaveNPCState>();
        data1.AmbientSoundBlocks = new List<SaveAmbientSoundState>();
        data1.ScriptBlocks = new List<SaveScriptBlockState>();
        data1.Teleports = new List<SaveTeleportState>();
      }
      data1.Scrolls = MapLoader.ReadWisdomScrollData(reader, version);
      if (version > 45 && version < 74)
        Globals1.ReadGlobalPoint3DList(reader);
      data1.Zones = MapLoader.ReadZoneData(reader, version);
      data1.Books = MapLoader.ReadBookData(reader, version);
      if (version < 107)
      {
        data1.AmbientSoundBlocks = MapLoader.ReadAmbientSoundBlockData(reader, version);
        data1.ScriptBlocks = MapLoader.ReadScriptBlockData(reader, version);
        if (version > 102)
        {
          MapLoader.ReadWifiTransmitterData(reader, version);
          MapLoader.ReadWifiReceiverData(reader, version);
        }
      }
      data1.LockedTable = MapLoader.ReadLockedTable(reader, data.Header, version);
      data1.Particles = MapLoader.ReadParticleData(reader, version);
      if (version < 107)
        data1.Teleports = MapLoader.ReadTeleportData(reader, version);
      if (version > 38)
        MapLoader.ReadFloodUpdates(reader, data1, version);
      else
        data1.FloodUpdates = new FloodData[0];
      if (version > 58)
        MapLoader.ReadBlockTextures(reader, data1, version);
      if (version > 68)
        MapLoader.ReadMapMarkers(reader, data1, version);
      else
        data1.MapMarkers = new MapMarker[0];
      SaveGameState saveGameState = data1;
      SaveInventoryState saveInventoryState;
      if (version <= 95)
        saveInventoryState = new SaveInventoryState()
        {
          PackSize = (short) 20
        };
      else
        saveInventoryState = MapLoader.ReadSpawnInventory(reader, version);
      saveGameState.SpawnInventory = saveInventoryState;
      data1.Scripts = version <= 101 ? new List<Script>() : MapLoader.ReadScripts(reader, version);
      if (version > 265)
        MapLoader.ReadBehaviours(reader, version);
      data1.AdventureScripts = version <= 215 ? new List<string>() : Globals1.ReadStringList(reader);
      data1.EventScripts = new Dictionary<ScriptEvent, string>();
      if (version > 217)
      {
        if (version > 218)
        {
          int num = reader.ReadInt32();
          for (int index = 0; index < num; ++index)
            data1.EventScripts.Add((ScriptEvent) reader.ReadByte(), reader.ReadString());
        }
        else
        {
          data1.EventScripts.Add(ScriptEvent.PlayerJoin, reader.ReadString());
          data1.EventScripts.Add(ScriptEvent.PlayerLeave, reader.ReadString());
        }
      }
      if (version > 138)
        data1.Teleports = MapLoader.ReadTeleportData(reader, version);
      if (version > 160)
        data1.History = MapLoader.ReadHistoryData(reader, version);
      if (version > 219)
      {
        int capacity = reader.ReadInt32();
        if (capacity > 0)
        {
          data1.ClanHistory = new Dictionary<string, History>(capacity);
          for (int index = 0; index < capacity; ++index)
            data1.ClanHistory.Add(reader.ReadString(), MapLoader.ReadHistoryData(reader, version));
        }
      }
      return data1;
    }

    private static bool[] ReadLockedTable(BinaryReader reader, SaveMapHead header, int version)
    {
      int num = reader.ReadInt32();
      bool[] flagArray = new bool[Globals1.ItemData.Length];
      for (int i = 0; i < num; ++i)
      {
        bool flag = reader.ReadBoolean();
        if (version < 39)
        {
          switch ((Item) i)
          {
            case Item.ItemShop:
            case Item.BlockShop:
              flag = header.GameMode != GameMode.Creative;
              break;
          }
        }
        if (!Globals1.IsLocked(i, header.GameMode))
          flag = false;
        if (i < flagArray.Length)
          flagArray[i] = flag;
      }
      if (header.GameMode != GameMode.DigDeep)
      {
        for (int i = 0; i < flagArray.Length; ++i)
        {
          if (!Globals1.IsLocked(i, header.GameMode))
            flagArray[i] = false;
        }
      }
      else
      {
        for (int index = num; index < Globals1.ItemData.Length; ++index)
        {
          bool flag = Globals1.ItemData[index].LockedDD;
          Blueprint blueprint = Blueprints.GetBlueprint((Item) index);
          if (blueprint != null && blueprint.IsDefault)
            flag = false;
          flagArray[index] = flag;
        }
        for (int index = 0; index < flagArray.Length; ++index)
        {
          if (!Globals1.ItemData[index].LockedDD)
            flagArray[index] = false;
        }
        if (header.Pre18)
        {
          flagArray[72] = false;
          flagArray[325] = false;
          flagArray[139] = false;
          flagArray[324] = false;
          flagArray[113] = false;
          flagArray[326] = false;
          flagArray[150] = false;
          flagArray[328] = false;
          flagArray[154] = false;
          flagArray[329] = false;
          flagArray[149] = false;
          flagArray[183] = false;
          flagArray[327] = false;
          flagArray[572] = false;
          flagArray[158] = false;
          flagArray[363] = false;
          flagArray[165] = false;
        }
        else
        {
          flagArray[72] = false;
          flagArray[139] = false;
          flagArray[113] = false;
          flagArray[150] = false;
          flagArray[149] = false;
          flagArray[183] = false;
          flagArray[154] = false;
          flagArray[158] = false;
          flagArray[165] = false;
        }
      }
      return flagArray;
    }

    private static bool[] ReadEnabledItemsTable(
      BinaryReader reader,
      SaveMapHead header,
      int version)
    {
      int val1 = reader.ReadInt32();
      bool[] flagArray = new bool[Math.Max(val1, Globals1.ItemData.Length)];
      for (int index = 0; index < val1; ++index)
        flagArray[index] = reader.ReadBoolean();
      for (int index = val1; index < flagArray.Length; ++index)
        flagArray[index] = true;
      if (version < 251)
      {
        flagArray[182] = true;
        flagArray[571] = true;
        flagArray[184] = true;
        flagArray[573] = true;
        flagArray[183] = true;
        flagArray[572] = true;
        flagArray[185] = true;
        flagArray[186] = true;
        flagArray[187] = true;
        flagArray[188] = true;
        flagArray[189] = true;
        flagArray[190] = true;
        flagArray[191] = true;
        if (version < 240)
          flagArray[78] = true;
      }
      return flagArray;
    }

    private static List<SaveItemParticle> ReadParticleData(
      BinaryReader reader,
      int version)
    {
      int capacity = reader.ReadInt32();
      List<SaveItemParticle> saveItemParticleList = new List<SaveItemParticle>(capacity);
      SaveItemParticle saveItemParticle = new SaveItemParticle();
      for (int index = 0; index < capacity; ++index)
      {
        if (version > 23)
        {
          saveItemParticle.Item.ItemID = (Item) reader.ReadUInt16();
          saveItemParticle.Item.Count = reader.ReadInt32();
          saveItemParticle.Item.Durability = version < 41 ? (ushort) reader.ReadInt32() : reader.ReadUInt16();
          saveItemParticle.Position.X = reader.ReadSingle();
          saveItemParticle.Position.Y = reader.ReadSingle();
          saveItemParticle.Position.Z = reader.ReadSingle();
        }
        else
        {
          saveItemParticle.Item.ItemID = (Item) reader.ReadUInt16();
          saveItemParticle.Item.Count = 1;
          saveItemParticle.Position.X = reader.ReadSingle();
          saveItemParticle.Position.Y = reader.ReadSingle();
          saveItemParticle.Position.Z = reader.ReadSingle();
          reader.ReadInt32();
        }
        saveItemParticleList.Add(saveItemParticle);
      }
      return saveItemParticleList;
    }

    private static List<SaveTeleportState> ReadTeleportData(
      BinaryReader reader,
      int version)
    {
      if (version <= 87)
        return new List<SaveTeleportState>();
      List<SaveTeleportState> saveTeleportStateList = new List<SaveTeleportState>();
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
      {
        GlobalPoint3D globalPoint3D;
        globalPoint3D.X = (int) reader.ReadInt16();
        globalPoint3D.Y = (int) reader.ReadInt16();
        globalPoint3D.Z = (int) reader.ReadInt16();
        byte num2 = reader.ReadByte();
        saveTeleportStateList.Add(new SaveTeleportState()
        {
          Point = globalPoint3D,
          Channel = num2
        });
      }
      return saveTeleportStateList;
    }

    private static History ReadHistoryData(BinaryReader reader, int version)
    {
      History history = new History();
      history.ReadState(reader, version);
      return history;
    }

    private static void ReadFloodUpdates(BinaryReader reader, SaveGameState data, int version)
    {
      int length = reader.ReadInt32();
      data.FloodUpdates = new FloodData[length];
      for (int index1 = 0; index1 < length; ++index1)
      {
        FloodData floodData = new FloodData();
        floodData.BlockID = (Block) reader.ReadByte();
        floodData.Method = (UpdateBlockMethod) reader.ReadByte();
        floodData.Gamertag = reader.ReadString();
        floodData.FloodPoints = new GlobalPoint3D[reader.ReadInt32()];
        int num1 = 6;
        byte[] buffer = new byte[floodData.FloodPoints.Length * num1];
        reader.Read(buffer, 0, buffer.Length);
        GlobalPoint3D globalPoint3D = new GlobalPoint3D();
        int num2 = 0;
        for (int index2 = 0; index2 < floodData.FloodPoints.Length; ++index2)
        {
          ref GlobalPoint3D local1 = ref globalPoint3D;
          byte[] numArray1 = buffer;
          int index3 = num2;
          int num3 = index3 + 1;
          int num4 = (int) numArray1[index3];
          byte[] numArray2 = buffer;
          int index4 = num3;
          int num5 = index4 + 1;
          int num6 = (int) numArray2[index4] * 256;
          int num7 = (int) (ushort) (num4 + num6);
          local1.X = num7;
          ref GlobalPoint3D local2 = ref globalPoint3D;
          byte[] numArray3 = buffer;
          int index5 = num5;
          int num8 = index5 + 1;
          int num9 = (int) numArray3[index5];
          byte[] numArray4 = buffer;
          int index6 = num8;
          int num10 = index6 + 1;
          int num11 = (int) numArray4[index6] * 256;
          int num12 = (int) (ushort) (num9 + num11);
          local2.Y = num12;
          ref GlobalPoint3D local3 = ref globalPoint3D;
          byte[] numArray5 = buffer;
          int index7 = num10;
          int num13 = index7 + 1;
          int num14 = (int) numArray5[index7];
          byte[] numArray6 = buffer;
          int index8 = num13;
          num2 = index8 + 1;
          int num15 = (int) numArray6[index8] * 256;
          int num16 = (int) (ushort) (num14 + num15);
          local3.Z = num16;
          floodData.FloodPoints[index2] = globalPoint3D;
        }
        data.FloodUpdates[index1] = floodData;
      }
    }

    private static void ReadBlockTextures(BinaryReader reader, SaveGameState data, int version)
    {
      data.BlockTextures = MapLoader.ReadBlockTextures(reader, version);
      if (data.BlockTextures.GetLength(0) > 8)
        data.BlockTextures[8, 0] = Block.None;
      if (data.BlockTextures.GetLength(0) > 9)
        data.BlockTextures[9, 0] = Block.None;
      if (data.BlockTextures.GetLength(0) <= 11)
        return;
      data.BlockTextures[11, 0] = Block.None;
    }

    public static Block[,] ReadBlockTextures(BinaryReader reader, int version)
    {
      int length1 = reader.ReadInt32();
      int length2 = reader.ReadInt32();
      Block[,] blockArray = new Block[length1, length2];
      for (int j = 0; j < length1; ++j)
      {
        for (int k = 0; k < length2; ++k)
        {
          Block block = MapLoader.FixBlockTexture(j, k, (Block) reader.ReadByte(), version);
          blockArray[j, k] = block;
        }
      }
      if (version < 252)
      {
        int index = (int) Globals1.BlockData[126].TextureID - 1;
        if (blockArray.GetLength(0) > index)
          blockArray[index, 5] = Block.Wood;
      }
      return blockArray;
    }

    private static Block FixBlockTexture(int j, int k, Block b, int version)
    {
      if (version < 236)
      {
        switch (b)
        {
          case Block.None:
            if (j == 33 && k < 5)
              return (Block) k;
            break;
          case Block.HealthBlock:
            return Block.Stack;
        }
      }
      return b;
    }

    private static List<SaveWifiState> ReadWifiTransmitterData(
      BinaryReader reader,
      int version)
    {
      if (version > 100)
        return MapLoader.ReadWifiData(reader, version);
      return new List<SaveWifiState>();
    }

    private static List<SaveWifiState> ReadWifiReceiverData(
      BinaryReader reader,
      int version)
    {
      if (version > 98)
        return MapLoader.ReadWifiData(reader, version);
      return new List<SaveWifiState>();
    }

    private static List<SaveWifiState> ReadWifiData(
      BinaryReader reader,
      int version)
    {
      int num = reader.ReadInt32();
      List<SaveWifiState> saveWifiStateList = new List<SaveWifiState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveWifiStateList.Add(MapLoader.ReadWifiState(reader, version));
      return saveWifiStateList;
    }

    private static SaveWifiState ReadWifiState(BinaryReader reader, int version)
    {
      SaveWifiState saveWifiState = new SaveWifiState();
      saveWifiState.Hash = reader.ReadInt64();
      saveWifiState.Index = reader.ReadInt32();
      if (version > 100)
        saveWifiState.Frequency = reader.ReadUInt16();
      return saveWifiState;
    }

    private static void ReadMapMarkers(BinaryReader reader, SaveGameState data, int version)
    {
      int length = reader.ReadInt32();
      data.MapMarkers = new MapMarker[length];
      MapMarker mapMarker = new MapMarker();
      for (int index = 0; index < length; ++index)
      {
        mapMarker.Point.X = reader.ReadInt32();
        mapMarker.Point.Y = reader.ReadInt32();
        mapMarker.Point.Z = reader.ReadInt32();
        if (version > 150)
          mapMarker.Type = (MapMarkerType) reader.ReadByte();
        mapMarker.Label = reader.ReadString();
        data.MapMarkers[index] = mapMarker;
      }
    }

    public static List<Script> ReadScripts(BinaryReader reader, int version)
    {
      int capacity = reader.ReadInt32();
      List<Script> scriptList = new List<Script>(capacity);
      for (int index = 0; index < capacity; ++index)
      {
        Script script = new Script(reader.ReadString());
        if (version > 213)
          script.Alias = reader.ReadString();
        script.Commands = Globals1.ReadStringList(reader);
        MapLoader.ConvertScript(script, version);
        scriptList.Add(script);
      }
      return scriptList;
    }

    private static void ConvertScript(Script script, int version)
    {
      if (version < 248)
      {
        Parser parser = new Parser();
        for (int index = 0; index < script.Commands.Count; ++index)
        {
          if (script.Commands[index].StartsWith("skilladdxp", StringComparison.OrdinalIgnoreCase))
          {
            string command = "SkillXP" + script.Commands[index].Substring(10);
            Parser.Token token = new Parser.Token();
            token = parser.GetNextToken(command, 0);
            token = parser.GetNextToken(command, token.EndIndex + 1);
            if (token.Lexeme[0] != '-')
              command = command.Substring(0, token.StartIndex) + "+" + command.Substring(token.StartIndex);
            script.Commands[index] = command;
          }
        }
      }
      if (version >= (int) byte.MaxValue)
        return;
      for (int index = 0; index < script.Commands.Count; ++index)
      {
        string lower = script.Commands[index].Trim().ToLower();
        if (lower.StartsWith("context") || lower.StartsWith("inventory") || (lower.StartsWith("hasinventory") || lower.StartsWith("teleport")) || (lower.StartsWith("Hasplayer") || lower.StartsWith("Hasactor") || lower.StartsWith("ismobcount")))
        {
          int length = lower.IndexOf("[player]", StringComparison.OrdinalIgnoreCase);
          if (length >= 0)
            script.Commands[index] = lower.Substring(0, length) + "[actor]" + lower.Substring(length + 8);
        }
      }
    }

    public static void LoadScripts(string path, List<Script> scripts)
    {
      foreach (string file in FileSystem.GetFiles(path, "*.scr"))
      {
        using (Stream stream = FileSystem.OpenRead(file))
        {
          string name = file.Substring(0, file.Length - 4).Substring(path.Length).Replace('_', '\\');
          using (StreamReader streamReader = new StreamReader(stream))
          {
            Script script1 = new Script(name);
            while (!streamReader.EndOfStream)
            {
              string str = streamReader.ReadLine().Trim().Replace('\t', char.MinValue);
              script1.Commands.Add(str);
            }
            bool flag = true;
            for (int index = 0; index < scripts.Count; ++index)
            {
              Script script2 = scripts[index];
              if (script2.Name == name)
              {
                flag = false;
                script1.ExecutionCount = script2.ExecutionCount;
                script1.LastExecutionTicks = script2.LastExecutionTicks;
                script1.TotalExecutionTicks = script2.TotalExecutionTicks;
                scripts[index] = script1;
                break;
              }
            }
            if (flag)
              scripts.Add(script1);
          }
        }
      }
    }

    public static void LoadScript(string path, Script script)
    {
      using (Stream stream = FileSystem.OpenRead(path + script.Name.Replace('\\', '_') + ".scr"))
      {
        using (StreamReader streamReader = new StreamReader(stream))
        {
          while (!streamReader.EndOfStream)
          {
            string str = streamReader.ReadLine().Trim().Replace('\t', char.MinValue);
            script.Commands.Add(str);
          }
        }
      }
    }

    private static void ReadBehaviours(BinaryReader reader, int version)
    {
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        BehaviourTree behaviourTree = new BehaviourTree(BehaviourTreeType.None, false);
        behaviourTree.ReadState(reader, version);
        if (behaviourTree.Name.IsNotEmpty())
          Globals1.BehaviourTrees.Add(behaviourTree);
      }
    }

    private static List<SavePlayerState> ReadPlayerData(
      BinaryReader reader,
      SaveData data,
      int version)
    {
      if (version < (int) sbyte.MaxValue)
        return MapLoader.ReadPlayerDataCore(reader, data, version);
      List<SavePlayerState> savePlayerStateList = new List<SavePlayerState>(data.GameState.PlayerCount);
      for (int index = 0; index < data.GameState.PlayerCount; ++index)
        savePlayerStateList.Add(new SavePlayerState());
      return savePlayerStateList;
    }

    public static List<SavePlayerState> ReadPlayerDataCore(
      BinaryReader reader,
      SaveData data,
      int version)
    {
      List<SavePlayerState> savePlayerStateList = new List<SavePlayerState>();
      if (reader.BaseStream.Length > 0L)
      {
        int num1 = version < 226 ? 0 : reader.ReadInt32();
        if (num1 == 0 || num1 == Globals2.BuildMapID(data.Header))
        {
          int num2 = version < (int) sbyte.MaxValue ? data.GameState.PlayerCount : reader.ReadInt32();
          try
          {
            for (int index = 0; index < num2; ++index)
            {
              SavePlayerState savePlayerState = MapLoader.ReadPlayerData(data.Header, reader, version);
              if (savePlayerState.Statistics.SecondsPlayed > 10.0)
              {
                savePlayerStateList.Add(savePlayerState);
                if (savePlayerState.Statistics.SecondsPlayed < 60.0)
                  savePlayerState.RatingStars = (byte) 0;
              }
            }
          }
          catch (Exception ex)
          {
            Services.ExceptionReporter.ReportExceptionCaught(41, ex);
          }
        }
      }
      return savePlayerStateList;
    }

    private static void ReadPlayerDataForClient(BinaryReader reader, SaveData data, int version)
    {
      int num = reader.ReadInt32();
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = reader.ReadInt32();
        SavePlayerState savePlayerState = MapLoader.ReadPlayerData(data.Header, reader, version);
        while (index2 >= data.PlayerState.Count)
          data.PlayerState.Add(new SavePlayerState());
        data.PlayerState[index2] = savePlayerState;
      }
    }

    private static SavePlayerState ReadPlayerData(
      SaveMapHead head,
      BinaryReader reader,
      int version)
    {
      SavePlayerState savePlayerState = new SavePlayerState();
      MapLoader.ReadCharacterDataCore(reader, (SaveCharacterState) savePlayerState, version);
      savePlayerState.Inventory.PackSize = (short) 30;
      savePlayerState.Inventory.EquipSize = (short) 7;
      savePlayerState.Inventory.TempSize = (short) 9;
      savePlayerState.Gamertag = version <= 21 ? reader.ReadString() : Globals2.ReadGamertag(reader);
      if (version < 190 && NetworkManager.Instance != null && (NetworkManager.Instance.IsSessionOpen && savePlayerState.Gamertag != NetworkManager.Instance.Session.Host.Gamertag))
        savePlayerState.Permission &= ~(Permissions.Save | Permissions.Admin | Permissions.Grief);
      if (version > 151)
        savePlayerState.IsNewPlayer = reader.ReadBoolean();
      if (version > 211)
        savePlayerState.ClanName = reader.ReadString();
      if (version > 226)
        savePlayerState.ClanBannerID = reader.ReadInt32();
      savePlayerState.JetPackActive = reader.ReadBoolean();
      savePlayerState.Reach = version <= 214 ? 0 : (int) reader.ReadByte();
      savePlayerState.CraftInstructionMessageShown = reader.ReadBoolean();
      savePlayerState.NewComPackMessageShown = reader.ReadBoolean();
      savePlayerState.Message4 = reader.ReadBoolean();
      if (version > (int) sbyte.MaxValue)
        savePlayerState.LastTransmitterFrequency = reader.ReadInt32();
      if (version > 21)
        savePlayerState.RatingStars = reader.ReadByte();
      if (version > 35)
      {
        savePlayerState.GoldEarned = reader.ReadInt32();
        savePlayerState.ScrollsFound = reader.ReadInt32();
        if (version > 168)
        {
          savePlayerState.BedRockProspected = reader.ReadBoolean();
          savePlayerState.EnemiesKilledBeforeBedrock = reader.ReadInt32();
        }
        savePlayerState.ItemsCrafted = new ushort[reader.ReadInt32()];
        for (int index = 0; index < savePlayerState.ItemsCrafted.Length; ++index)
          savePlayerState.ItemsCrafted[index] = reader.ReadUInt16();
      }
      else
        savePlayerState.ItemsCrafted = new ushort[0];
      savePlayerState.Settings.GamePadSensitivity = reader.ReadSingle();
      if (version > 254)
        savePlayerState.Settings.MouseSensitivity = reader.ReadSingle();
      if (version > 60)
        savePlayerState.Settings.FOVNormalized = reader.ReadSingle();
      savePlayerState.Settings.HudVisible = reader.ReadBoolean();
      savePlayerState.Settings.MapVisible = reader.ReadBoolean();
      savePlayerState.Settings.RumbleOn = reader.ReadBoolean();
      if (version > 21)
        savePlayerState.Settings.Nameplates = version <= 50 ? (reader.ReadBoolean() ? NamePlateSetting.Short : NamePlateSetting.None) : (NamePlateSetting) reader.ReadByte();
      if (version < 147)
        reader.ReadBoolean();
      savePlayerState.Settings.InvertY = reader.ReadBoolean();
      savePlayerState.Settings.BlueprintFinderVisible = reader.ReadBoolean();
      if (version > 48)
        savePlayerState.Settings.AutoplaceTime = reader.ReadSingle();
      if (version < 194)
        savePlayerState.Settings.AutoplaceTime = 0.0f;
      if (version > 149)
        savePlayerState.Settings.HotBarToTransparentTime = reader.ReadByte();
      if (version > 107)
        savePlayerState.Settings.DisplayXPGains = reader.ReadBoolean();
      savePlayerState.Settings.Bobbing = version <= 34 || reader.ReadBoolean();
      if (version > 108 && version < 146)
      {
        int num1 = (int) reader.ReadByte();
      }
      if (version > 122)
      {
        double num2 = (double) reader.ReadSingle();
      }
      savePlayerState.Settings.MobNameplates = version <= 124 || reader.ReadBoolean();
      savePlayerState.Settings.CameraType = version <= 139 ? CameraType.Original : (CameraType) reader.ReadByte();
      if (version > 144)
        savePlayerState.Settings.UserControlSetting = (UserControlSetting) reader.ReadByte();
      savePlayerState.Settings.WieldType = version <= 191 ? WieldType.BothHands : (WieldType) reader.ReadByte();
      savePlayerState.Statistics.SecondsPlayed = reader.ReadDouble();
      savePlayerState.Statistics.DistanceWalked = reader.ReadSingle();
      savePlayerState.Statistics.DistanceFlown = reader.ReadSingle();
      savePlayerState.Statistics.BlocksCleared = reader.ReadInt32();
      savePlayerState.Statistics.BlocksPlaced = reader.ReadInt32();
      savePlayerState.Statistics.BlocksPickedUp = reader.ReadInt32();
      savePlayerState.Statistics.ItemsPickedUp = reader.ReadInt32();
      savePlayerState.Statistics.DamageDealt = reader.ReadSingle();
      savePlayerState.Statistics.DamageTaken = reader.ReadSingle();
      savePlayerState.Statistics.TotalKills = reader.ReadInt32();
      savePlayerState.Statistics.TotalDeaths = reader.ReadInt32();
      savePlayerState.Statistics.PlayerKills = reader.ReadInt32();
      savePlayerState.Statistics.NPCKills = reader.ReadInt32();
      savePlayerState.Statistics.LootValue = reader.ReadInt32();
      savePlayerState.Statistics.GrenadesLaunched = reader.ReadInt32();
      savePlayerState.Statistics.TotalKills = savePlayerState.Statistics.PlayerKills + savePlayerState.Statistics.NPCKills;
      savePlayerState.Statistics.FillerInt1 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt2 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt3 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt4 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt5 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt6 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt7 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt8 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt9 = reader.ReadInt32();
      savePlayerState.Statistics.FillerInt10 = reader.ReadInt32();
      savePlayerState.Statistics.FillerFloat1 = reader.ReadSingle();
      savePlayerState.Statistics.FillerFloat2 = reader.ReadSingle();
      savePlayerState.Statistics.FillerFloat3 = reader.ReadSingle();
      savePlayerState.Statistics.FillerFloat4 = reader.ReadSingle();
      savePlayerState.Statistics.FillerFloat5 = reader.ReadSingle();
      if (version > 159)
      {
        int num3 = reader.ReadInt32();
        if (num3 >= 0)
          savePlayerState.Waypoint = new GlobalPoint3D?(new GlobalPoint3D()
          {
            X = num3,
            Z = reader.ReadInt32()
          });
      }
      savePlayerState.DefaultPriceList = version <= 61 ? (PriceList) null : MapLoader.ReadPriceList(reader, PriceList.PriceListType.PlayerDefault, version);
      savePlayerState.History = version <= 160 ? new History() : MapLoader.ReadHistoryData(reader, version);
      savePlayerState.ActionLog = new ActionLog();
      if (version > 163)
        savePlayerState.ActionLog.ReadState(reader, version);
      if (version > 192)
        savePlayerState.SkillsData.ReadState(reader, version);
      return savePlayerState;
    }

    private static PriceList ReadPriceList(
      BinaryReader reader,
      PriceList.PriceListType type,
      int version)
    {
      int num = reader.ReadInt32();
      if (num == 0)
        return (PriceList) null;
      PriceList priceList = new PriceList(type);
      for (int index = 0; index < num; ++index)
      {
        PriceList.Price price = MapLoader.ReadPrice(reader, version);
        if (index < priceList.Prices.Length)
          priceList.Prices[index] = price;
      }
      return priceList;
    }

    private static PriceList.Price ReadPrice(BinaryReader reader, int version)
    {
      return new PriceList.Price()
      {
        Buy = reader.ReadInt32(),
        Sell = reader.ReadInt32(),
        Perc = reader.ReadInt32(),
        UsePerc = reader.ReadBoolean(),
        ForSale = reader.ReadBoolean()
      };
    }

    private static void ReadCharacterDataCore(
      BinaryReader reader,
      SaveCharacterState result,
      int version)
    {
      MapLoader.ReadCharacterBaseDataCore(reader, (SaveCharacterBaseState) result, version);
      result.ViewAngle.X = reader.ReadSingle();
      result.ViewAngle.Y = reader.ReadSingle();
      result.Oxygen = reader.ReadSingle();
      if (version > 21)
      {
        result.Permission = Globals2.DefaultPermission;
        ushort num = version > 155 ? reader.ReadUInt16() : (ushort) reader.ReadByte();
        if (version > 94)
          result.Permission = (Permissions) num;
      }
      if (version <= 110 || version >= 146)
        return;
      int num1 = (int) reader.ReadByte();
      int num2 = (int) reader.ReadByte();
    }

    private static void ReadCharacterBaseDataCore(
      BinaryReader reader,
      SaveCharacterBaseState result,
      int version)
    {
      if (version > 113)
        result.MobType = (ActorType) reader.ReadByte();
      result.Position.X = reader.ReadSingle();
      result.Position.Y = reader.ReadSingle();
      result.Position.Z = reader.ReadSingle();
      result.Health = reader.ReadSingle();
      if (version > 29)
        result.Seed = reader.ReadInt32();
      result.Inventory = MapLoader.ReadInventoryData(reader, version, (short) 30);
      if (version >= 146)
        return;
      MapLoader.ReadInventoryData(reader, version, (short) 10);
    }

    private static SaveArcadeState ReadArcadeData(BinaryReader reader, int version)
    {
      SaveArcadeState saveArcadeState = new SaveArcadeState();
      if (version > 17)
      {
        saveArcadeState.TotalInvadersHighScore = reader.ReadInt32();
        if (version > 236)
          saveArcadeState.TotalInvadersHighScoreGamer = reader.ReadString();
        if (version > 240)
          saveArcadeState.TotalInvadersHighScoreVersion = reader.ReadString();
        if (version > 222)
          saveArcadeState.TotalRushHighScore = reader.ReadInt32();
        if (version < 232)
          saveArcadeState.TotalRushHighScore = 0;
        if (version > 236)
          saveArcadeState.TotalRushHighScoreGamer = reader.ReadString();
        if (version > 240)
          saveArcadeState.TotalRushHighScoreVersion = reader.ReadString();
      }
      return saveArcadeState;
    }

    private static void ReadPhotoThumbnails(BinaryReader reader, int version)
    {
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
        MapLoader.ReadPhotoThumbnail(reader, version);
    }

    private static void ReadPhotoThumbnail(BinaryReader reader, int version)
    {
      int index = reader.ReadInt32();
      MapLoader.ReadPhotoThumbnail(reader, index, version);
      MapLoader.ReadPhotoThumbnail(reader, index, version);
    }

    private static void ReadPhotoThumbnail(BinaryReader reader, int index, int version)
    {
      int count = reader.ReadInt32();
      if (count <= 0)
        return;
      Color[] colorData = new Color[count];
      MapLoader.ReadColorArray(reader, colorData, count, version);
      GraphicStatics.PhotoData.SetPhotoThumbnailColorData((byte) index, colorData);
    }

    public static void ReadColorArray(
      BinaryReader reader,
      Color[] colorData,
      int count,
      int version)
    {
      byte[] numArray = reader.ReadBytes(count * 3);
      int index1 = 0;
      int index2 = 0;
      while (index1 < count)
      {
        colorData[index1] = new Color((int) numArray[index2], (int) numArray[index2 + 1], (int) numArray[index2 + 2], (int) byte.MaxValue);
        ++index1;
        index2 += 3;
      }
    }

    private static SaveInventoryState ReadInventoryData(
      BinaryReader reader,
      int version,
      short sizeOverride)
    {
      SaveInventoryState state = new SaveInventoryState();
      MapLoader.ReadInventoryDataCore(reader, state, version, sizeOverride);
      return state;
    }

    private static void ReadInventoryDataCore(
      BinaryReader reader,
      SaveInventoryState state,
      int version,
      short sizeOverride)
    {
      state.PackSize = sizeOverride;
      if (version > 46)
        state.PackSize = reader.ReadInt16();
      if (state.PackSize == (short) 0)
        state.PackSize = sizeOverride;
      if (version > 145)
        state.EquipSize = reader.ReadInt16();
      if (version > 145)
        state.TempSize = reader.ReadInt16();
      if (version > 145)
        state.HotBarLeftID = reader.ReadUInt16();
      if (version > 145)
        state.HotBarRightID = reader.ReadUInt16();
      int num1 = reader.ReadInt32();
      state.AllowZeroCountItems = version > 71 && reader.ReadBoolean();
      int num2 = (int) reader.ReadInt16();
      int num3 = (int) reader.ReadInt16();
      for (int index = 0; index < num1; ++index)
      {
        SaveInventoryItem saveInventoryItem = new SaveInventoryItem();
        saveInventoryItem.SlotID = reader.ReadUInt16();
        saveInventoryItem.ItemID = reader.ReadUInt16();
        saveInventoryItem.Count = reader.ReadInt32();
        saveInventoryItem.Durability = version < 41 ? (ushort) reader.ReadInt32() : reader.ReadUInt16();
        switch ((Item) saveInventoryItem.ItemID)
        {
          case Item.None:
          case Item.Bedrock:
            continue;
          default:
            if (ItemData.IsEnabled((Item) saveInventoryItem.ItemID))
            {
              MapLoader.FixItemData(ref saveInventoryItem);
              state.Items.Add(saveInventoryItem);
              continue;
            }
            continue;
        }
      }
    }

    private static void FixItemData(ref SaveInventoryItem item)
    {
      Item itemId1 = (Item) item.ItemID;
      item.ItemID = (ushort) MapLoader.FixItemIDs(itemId1);
      Item itemId2 = (Item) item.ItemID;
      switch (itemId2)
      {
        case Item.Wisdom:
          break;
        case Item.Blueprint:
          break;
        case Item.Book:
          break;
        default:
          ushort itemDurability = ItemData.GetItemDurability(itemId2);
          if (itemDurability <= (ushort) 0 || (int) item.Durability <= (int) itemDurability)
            break;
          item.Durability = (ushort) 1;
          break;
      }
    }

    private static Item FixItemIDs(Item itemID)
    {
      Item obj = itemID;
      if ((uint) obj <= 113U)
      {
        switch (obj)
        {
          case Item.Bedrock:
            return Item.None;
          case Item.Rope:
            return Item.RopeIcon;
          case Item.Stairs:
            return Item.StairsIcon;
        }
      }
      else if ((uint) obj <= 150U)
      {
        switch (obj)
        {
          case Item.Fence:
            return Item.FenceIcon;
          case Item.HalfBlock:
            return Item.HalfBlockIcon;
          case Item.Ramp:
            return Item.RampIcon;
        }
      }
      else
      {
        switch (obj)
        {
          case Item.Cylinder:
            return Item.CylinderIcon;
          case Item.Stairs2:
            return Item.Stairs2Icon;
          case Item.HalfBlock2:
            return Item.HalfBlock2Icon;
          case Item.Ramp2:
            return Item.Ramp2Icon;
        }
      }
      return itemID;
    }

    private static List<SavePlayerBlockState> ReadPlayerBlockData(
      BinaryReader reader,
      int version)
    {
      int num = reader.ReadInt32();
      List<SavePlayerBlockState> playerBlockStateList = new List<SavePlayerBlockState>(num + 1);
      for (int index = 0; index < num; ++index)
        playerBlockStateList.Add(MapLoader.ReadPlayerBlockState(reader, version));
      return playerBlockStateList;
    }

    private static SavePlayerBlockState ReadPlayerBlockState(
      BinaryReader reader,
      int version)
    {
      SavePlayerBlockState playerBlockState = new SavePlayerBlockState();
      playerBlockState.Point.X = reader.ReadInt32();
      playerBlockState.Point.Y = reader.ReadInt32();
      playerBlockState.Point.Z = reader.ReadInt32();
      playerBlockState.Gamertag = reader.ReadString();
      if (playerBlockState.Gamertag == "")
        playerBlockState.Gamertag = (string) null;
      return playerBlockState;
    }

    private static List<SavePlayerBlockState> ReadLockedDoorData(
      BinaryReader reader,
      int version)
    {
      if (version > 44)
        return MapLoader.ReadPlayerBlockData(reader, version);
      return new List<SavePlayerBlockState>();
    }

    private static List<SaveSentryTurretState> ReadSentryTurretData(
      BinaryReader reader,
      int version)
    {
      if (version <= 43)
        return new List<SaveSentryTurretState>();
      int num = reader.ReadInt32();
      List<SaveSentryTurretState> sentryTurretStateList = new List<SaveSentryTurretState>(num + 1);
      for (int index = 0; index < num; ++index)
        sentryTurretStateList.Add(MapLoader.ReadSentryTurretState(reader, version));
      return sentryTurretStateList;
    }

    private static SaveSentryTurretState ReadSentryTurretState(
      BinaryReader reader,
      int version)
    {
      SaveSentryTurretState sentryTurretState = new SaveSentryTurretState();
      MapLoader.ReadChestStateCore(reader, version, (SaveChestState) sentryTurretState);
      sentryTurretState.Cooldown = reader.ReadSingle();
      sentryTurretState.TargetTypes = BlockTargetTypes.Players | BlockTargetTypes.Mobs;
      if (version > 47)
        sentryTurretState.TargetTypes = (BlockTargetTypes) reader.ReadByte();
      if (version > 97)
        sentryTurretState.RequiresPower = reader.ReadBoolean();
      return sentryTurretState;
    }

    private static List<SaveMineBlockState> ReadMineBlockData(
      BinaryReader reader,
      int version)
    {
      if (version <= 52)
        return new List<SaveMineBlockState>();
      int num = reader.ReadInt32();
      List<SaveMineBlockState> saveMineBlockStateList = new List<SaveMineBlockState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveMineBlockStateList.Add(MapLoader.ReadMineBlockState(reader, version));
      return saveMineBlockStateList;
    }

    private static SaveMineBlockState ReadMineBlockState(
      BinaryReader reader,
      int version)
    {
      SaveMineBlockState saveMineBlockState = new SaveMineBlockState();
      saveMineBlockState.Point.X = reader.ReadInt32();
      saveMineBlockState.Point.Y = reader.ReadInt32();
      saveMineBlockState.Point.Z = reader.ReadInt32();
      saveMineBlockState.TargetTypes = (BlockTargetTypes) reader.ReadByte();
      saveMineBlockState.TriggerDelay = reader.ReadByte();
      saveMineBlockState.TriggerRadius = reader.ReadByte();
      saveMineBlockState.BlastRadius = reader.ReadByte();
      saveMineBlockState.DetonateInRange = reader.ReadBoolean();
      saveMineBlockState.Gamertag = reader.ReadString();
      if (saveMineBlockState.Gamertag == "")
        saveMineBlockState.Gamertag = (string) null;
      return saveMineBlockState;
    }

    private static List<SaveShopBlockState> ReadShopBlockData(
      BinaryReader reader,
      int version)
    {
      if (version <= 61)
        return new List<SaveShopBlockState>();
      int num = reader.ReadInt32();
      List<SaveShopBlockState> saveShopBlockStateList = new List<SaveShopBlockState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveShopBlockStateList.Add(MapLoader.ReadShopBlockState(reader, version));
      return saveShopBlockStateList;
    }

    private static SaveShopBlockState ReadShopBlockState(
      BinaryReader reader,
      int version)
    {
      SaveShopBlockState saveShopBlockState = new SaveShopBlockState();
      saveShopBlockState.Point.X = reader.ReadInt32();
      saveShopBlockState.Point.Y = reader.ReadInt32();
      saveShopBlockState.Point.Z = reader.ReadInt32();
      saveShopBlockState.Gamertag = reader.ReadString();
      if (saveShopBlockState.Gamertag == "")
        saveShopBlockState.Gamertag = (string) null;
      saveShopBlockState.Inventory = MapLoader.ReadInventoryData(reader, version, (short) 150);
      saveShopBlockState.PriceList = MapLoader.ReadPriceList(reader, PriceList.PriceListType.PlayerShop, version);
      return saveShopBlockState;
    }

    private static List<SaveZoneState> ReadZoneData(
      BinaryReader reader,
      int version)
    {
      if (version <= 49)
        return new List<SaveZoneState>();
      int num = reader.ReadInt32();
      List<SaveZoneState> saveZoneStateList = new List<SaveZoneState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveZoneStateList.Add(MapLoader.ReadZoneState(reader, version));
      return saveZoneStateList;
    }

    private static SaveZoneState ReadZoneState(BinaryReader reader, int version)
    {
      SaveZoneState saveZoneState = new SaveZoneState();
      saveZoneState.Name = reader.ReadString();
      saveZoneState.Type = (ZoneType) reader.ReadByte();
      saveZoneState.Min.X = reader.ReadInt32();
      saveZoneState.Min.Y = reader.ReadInt32();
      saveZoneState.Min.Z = reader.ReadInt32();
      saveZoneState.Max.X = reader.ReadInt32();
      saveZoneState.Max.Y = reader.ReadInt32();
      saveZoneState.Max.Z = reader.ReadInt32();
      if (version > 51)
      {
        if (version > 211)
          saveZoneState.BuilderType = (ZoneBuilderType) reader.ReadByte();
        saveZoneState.Builder = Globals2.ReadGamertag(reader);
        if (version < 212 && saveZoneState.Builder != null && saveZoneState.Builder.Length > 0)
          saveZoneState.BuilderType = ZoneBuilderType.Player;
        if (saveZoneState.BuilderType == ZoneBuilderType.None)
          saveZoneState.Builder = (string) null;
      }
      if (version > 154)
        saveZoneState.OnEntryScript = reader.ReadString();
      if (version > 161)
        saveZoneState.OnExitScript = reader.ReadString();
      if (version > 185)
        saveZoneState.CombatLevelDifference = reader.ReadInt16();
      saveZoneState.SpeedMultiplier = version <= 188 ? 1f : reader.ReadSingle();
      saveZoneState.GravityMultiplier = version <= 188 ? 1f : reader.ReadSingle();
      return saveZoneState;
    }

    private static List<SaveBookState> ReadBookData(
      BinaryReader reader,
      int version)
    {
      if (version <= 62)
        return new List<SaveBookState>();
      int num = reader.ReadInt32();
      List<SaveBookState> saveBookStateList = new List<SaveBookState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveBookStateList.Add(MapLoader.ReadBookState(reader, version));
      return saveBookStateList;
    }

    private static SaveBookState ReadBookState(BinaryReader reader, int version)
    {
      SaveBookState saveBookState = new SaveBookState();
      if (version < 83)
      {
        saveBookState.ID = reader.ReadUInt16();
        reader.ReadInt32();
        reader.ReadInt32();
        reader.ReadInt32();
      }
      else
      {
        saveBookState.ID = version > 169 ? reader.ReadUInt16() : (ushort) reader.ReadByte();
        saveBookState.Title = reader.ReadString();
      }
      int length = reader.ReadInt32();
      saveBookState.Text = new string[length];
      for (int index = 0; index < length; ++index)
        saveBookState.Text[index] = reader.ReadString();
      return saveBookState;
    }

    private static List<SaveChestState> ReadChestData(
      BinaryReader reader,
      int version)
    {
      int num = reader.ReadInt32();
      List<SaveChestState> saveChestStateList = new List<SaveChestState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveChestStateList.Add(MapLoader.ReadChestState(reader, version));
      return saveChestStateList;
    }

    private static SaveChestState ReadChestState(BinaryReader reader, int version)
    {
      SaveChestState state = new SaveChestState();
      MapLoader.ReadChestStateCore(reader, version, state);
      return state;
    }

    private static void ReadChestStateCore(BinaryReader reader, int version, SaveChestState state)
    {
      state.Point.X = reader.ReadInt32();
      state.Point.Y = reader.ReadInt32();
      state.Point.Z = reader.ReadInt32();
      MapLoader.ReadInventoryDataCore(reader, (SaveInventoryState) state, version, (short) 50);
      if (version <= 18)
        return;
      state.Gamertag = reader.ReadString();
      if (!(state.Gamertag == ""))
        return;
      state.Gamertag = (string) null;
    }

    private static List<SaveFurnaceState> ReadFurnaceData(
      BinaryReader reader,
      int version)
    {
      int num = reader.ReadInt32();
      List<SaveFurnaceState> saveFurnaceStateList = new List<SaveFurnaceState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveFurnaceStateList.Add(MapLoader.ReadFurnaceState(reader, version));
      return saveFurnaceStateList;
    }

    private static SaveFurnaceState ReadFurnaceState(
      BinaryReader reader,
      int version)
    {
      SaveFurnaceState saveFurnaceState = new SaveFurnaceState();
      saveFurnaceState.Point.X = reader.ReadInt32();
      saveFurnaceState.Point.Y = reader.ReadInt32();
      saveFurnaceState.Point.Z = reader.ReadInt32();
      saveFurnaceState.BurnTime = reader.ReadSingle();
      saveFurnaceState.SmeltTime = reader.ReadSingle();
      if (version > 26)
        saveFurnaceState.MaterialPlacer = reader.ReadString();
      MapLoader.ReadInventoryDataCore(reader, (SaveInventoryState) saveFurnaceState, version, (short) 4);
      return saveFurnaceState;
    }

    private static SaveInventoryState ReadSpawnInventory(
      BinaryReader reader,
      int version)
    {
      return MapLoader.ReadInventoryData(reader, version, (short) 20);
    }

    private static List<SaveFireState> ReadFireData(
      BinaryReader reader,
      int version)
    {
      if (version < 42)
        return new List<SaveFireState>();
      int num = reader.ReadInt32();
      List<SaveFireState> saveFireStateList = new List<SaveFireState>(num + 1);
      for (int index = 0; index < num; ++index)
        saveFireStateList.Add(MapLoader.ReadFireState(reader, version));
      return saveFireStateList;
    }

    private static SaveFireState ReadFireState(BinaryReader reader, int version)
    {
      return new SaveFireState()
      {
        Point = {
          X = reader.ReadInt32(),
          Y = reader.ReadInt32(),
          Z = reader.ReadInt32()
        },
        Target = {
          X = reader.ReadInt32(),
          Y = reader.ReadInt32(),
          Z = reader.ReadInt32()
        },
        Strength = reader.ReadSingle(),
        SpreadTimer = reader.ReadSingle()
      };
    }

    private static SaveSignsState ReadSignData(BinaryReader reader, int version)
    {
      SaveSignsState saveSignsState = new SaveSignsState();
      saveSignsState.SignCount = reader.ReadInt32();
      if (saveSignsState.SignCount > 0)
      {
        int num = reader.ReadInt32();
        saveSignsState.SignText = new List<string>(num + 1);
        for (int index = 0; index < num; ++index)
          saveSignsState.SignText.Add(reader.ReadString());
        List<SaveSignState> saveSignStateList = new List<SaveSignState>(saveSignsState.SignCount + 1);
        for (int index = 0; index < saveSignsState.SignCount; ++index)
          saveSignStateList.Add(MapLoader.ReadSignState(reader, version));
        saveSignsState.Signs = saveSignStateList;
      }
      return saveSignsState;
    }

    private static SaveSignState ReadSignState(BinaryReader reader, int version)
    {
      return new SaveSignState()
      {
        Point = {
          X = reader.ReadInt32(),
          Y = reader.ReadInt32(),
          Z = reader.ReadInt32()
        },
        Text1 = reader.ReadInt16(),
        Text2 = reader.ReadInt16(),
        Text3 = reader.ReadInt16(),
        Text4 = reader.ReadInt16()
      };
    }

    private static List<SaveNPCState> ReadNPCData(BinaryReader reader, int version)
    {
      List<SaveNPCState> saveNpcStateList = new List<SaveNPCState>();
      if (version > 28)
      {
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
          saveNpcStateList.Add(MapLoader.ReadNPCState(reader, version));
      }
      return saveNpcStateList;
    }

    private static SaveNPCState ReadNPCState(BinaryReader reader, int version)
    {
      return new SaveNPCState()
      {
        Point = {
          X = reader.ReadInt32(),
          Y = reader.ReadInt32(),
          Z = reader.ReadInt32()
        },
        Type = (ActorType) reader.ReadInt16(),
        ViewDirection = {
          X = reader.ReadSingle(),
          Y = reader.ReadSingle()
        },
        Text = reader.ReadString()
      };
    }

    private static List<SaveAmbientSoundState> ReadAmbientSoundBlockData(
      BinaryReader reader,
      int version)
    {
      if (version <= 78)
        return new List<SaveAmbientSoundState>();
      int num = reader.ReadInt32();
      List<SaveAmbientSoundState> ambientSoundStateList = new List<SaveAmbientSoundState>(num + 1);
      for (int index = 0; index < num; ++index)
        ambientSoundStateList.Add(MapLoader.ReadAmbientSoundBlockState(reader, version));
      return ambientSoundStateList;
    }

    private static SaveAmbientSoundState ReadAmbientSoundBlockState(
      BinaryReader reader,
      int version)
    {
      SaveAmbientSoundState ambientSoundState = new SaveAmbientSoundState();
      ambientSoundState.SoundID = reader.ReadInt16();
      ambientSoundState.Volume = reader.ReadSingle();
      ambientSoundState.Distance = reader.ReadUInt16();
      ambientSoundState.LoopDelay = version <= 80 ? (byte) 0 : reader.ReadByte();
      ambientSoundState.DayOrNight = version <= 81 ? DayOrNight.None : (DayOrNight) reader.ReadByte();
      if (version > 99)
        ambientSoundState.RequiresPower = reader.ReadBoolean();
      ambientSoundState.Point.X = reader.ReadInt32();
      ambientSoundState.Point.Y = reader.ReadInt32();
      ambientSoundState.Point.Z = reader.ReadInt32();
      return ambientSoundState;
    }

    private static List<SaveScriptBlockState> ReadScriptBlockData(
      BinaryReader reader,
      int version)
    {
      if (version <= 102)
        return new List<SaveScriptBlockState>();
      int num = reader.ReadInt32();
      List<SaveScriptBlockState> scriptBlockStateList = new List<SaveScriptBlockState>(num + 1);
      for (int index = 0; index < num; ++index)
        scriptBlockStateList.Add(MapLoader.ReadScriptBlockState(reader, version));
      return scriptBlockStateList;
    }

    private static SaveScriptBlockState ReadScriptBlockState(
      BinaryReader reader,
      int version)
    {
      return new SaveScriptBlockState()
      {
        Hash = reader.ReadInt64(),
        PowerOnScript = reader.ReadString(),
        PowerOffScript = reader.ReadString()
      };
    }

    private static WisdomScrollState[] ReadWisdomScrollData(
      BinaryReader reader,
      int version)
    {
      if (version < 64)
      {
        Globals1.ReadBoolList(reader);
        return new WisdomScrollState[0];
      }
      WisdomScrollState[] wisdomScrollStateArray = new WisdomScrollState[reader.ReadInt32()];
      for (int index = 0; index < wisdomScrollStateArray.Length; ++index)
      {
        WisdomScrollState wisdomScrollState = new WisdomScrollState();
        if (version < 78)
        {
          int num = (int) reader.ReadUInt16();
        }
        wisdomScrollState.IsEnabled = reader.ReadBoolean();
        wisdomScrollState.IsGenerated = wisdomScrollState.IsEnabled;
        if (version < 86 || version > 293)
        {
          if (version > 293)
            wisdomScrollState.IsGenerated = reader.ReadBoolean();
          wisdomScrollState.Point.X = reader.ReadInt32();
          wisdomScrollState.Point.Y = reader.ReadInt32();
          wisdomScrollState.Point.Z = reader.ReadInt32();
        }
        wisdomScrollStateArray[index] = wisdomScrollState;
      }
      return wisdomScrollStateArray;
    }

    private static BlueprintState[] ReadBlueprintData(
      BinaryReader reader,
      int version)
    {
      if (version < 64)
      {
        Globals1.ReadBoolList(reader);
        return new BlueprintState[0];
      }
      BlueprintState[] blueprintStateArray = new BlueprintState[reader.ReadInt32()];
      for (int index = 0; index < blueprintStateArray.Length; ++index)
      {
        BlueprintState blueprintState = new BlueprintState();
        if (version < 78)
        {
          int num = (int) reader.ReadUInt16();
        }
        blueprintState.IsEnabled = reader.ReadBoolean();
        blueprintState.IsUnearthed = version <= 83 ? blueprintState.IsEnabled : reader.ReadBoolean();
        blueprintState.IsGenerated = version <= 85 ? blueprintState.IsEnabled : reader.ReadBoolean();
        blueprintState.Point.X = reader.ReadInt32();
        blueprintState.Point.Y = reader.ReadInt32();
        blueprintState.Point.Z = reader.ReadInt32();
        blueprintStateArray[index] = blueprintState;
      }
      return blueprintStateArray;
    }

    public static void ApplyChanges(Map map, SaveDataResult data)
    {
      MapLoader.LoadScrollData(data.SaveData);
      MapLoader.LoadBlueprintData(data.SaveData);
      MapLoader.LoadTeleports(map, data.SaveData);
    }

    private static void LoadTeleports(Map map, SaveData data)
    {
      if (map.IsHost)
        return;
      MapStrategyTM mapStrategy = map.MapStrategy as MapStrategyTM;
      if (mapStrategy == null)
        return;
      foreach (SaveTeleportState teleport in data.GameState.Teleports)
      {
        TeleportBlock teleportBlock = new TeleportBlock(teleport.Point)
        {
          Channel = teleport.Channel
        };
        mapStrategy.AddDataBlock((DataBlock) teleportBlock, UpdateBlockMethod.Generation, true);
      }
    }

    private static void ConvertToNewFormat105(Map map, SaveDataResult data)
    {
      foreach (SaveChestState chest in data.SaveData.GameState.Chests)
      {
        if (chest.Items.Count > 0 || chest.Gamertag != null && chest.Gamertag.Length > 0)
          new ChestBlock(chest.Point, (int) chest.PackSize).LoadFromSaveData(chest);
      }
      foreach (SaveFurnaceState furnace in data.SaveData.GameState.Furnaces)
      {
        if (furnace.Items.Count > 0)
          new FurnaceBlock(map, furnace.Point).LoadFromSaveData(furnace);
      }
    }

    private List<GlobalPoint3D> ConvertBlastLightsToPoints(
      List<List<BlastLight>> blastLights)
    {
      List<GlobalPoint3D> globalPoint3DList = new List<GlobalPoint3D>();
      foreach (List<BlastLight> blastLight1 in blastLights)
      {
        foreach (BlastLight blastLight2 in blastLight1)
          globalPoint3DList.Add(new GlobalPoint3D(blastLight2.Point.X, blastLight2.Point.Y, blastLight2.Point.Z));
      }
      return globalPoint3DList;
    }

    private class BadVersionOnLoad : Exception
    {
    }
  }
}
