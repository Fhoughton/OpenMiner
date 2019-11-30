// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.MapSaver
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace StudioForge.TotalMiner.Storage
{
  internal class MapSaver
  {
    private static List<MapChunk> chunksToSave = new List<MapChunk>(100);

    public static void SaveMapToFile(GameInstance instance, IProgressBar progress, bool autoSave)
    {
      if (!Monitor.TryEnter(Globals1.SaveSemaphore))
        throw new OtherDiskActivityInProgressException();
      try
      {
        SaveGameFileInfo saveGame = Globals2.GameProperties.SaveGame;
        saveGame.Header.IsAutoSave = autoSave;
        MapSaver.PrepareIfNewSave(instance, saveGame);
        MapSaver.SaveMapCore(instance, saveGame, progress);
      }
      catch (OutOfMemoryException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(32, (Exception) ex);
        MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Not enough memory to save.\n\nTry reducing your view distance and go to a corner of the map\nthen wait a couple of minutes and retry the save.", "Ok");
        instance.AddScreen((GameScreen) messageBoxScreen, instance.NetworkManager.LocalGamers[0].Tag as StudioForge.TotalMiner.Player);
        throw ex;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(32, ex);
        throw ex;
      }
      finally
      {
        Monitor.Exit(Globals1.SaveSemaphore);
      }
    }

    private static void SaveMapCore(
      GameInstance instance,
      SaveGameFileInfo gameSave,
      IProgressBar progress)
    {
      SaveMapHead header = gameSave.Header;
      int saveVersion = header.SaveVersion;
      header.Format = SaveFormat.SlowLoad;
      string mapFilePath = gameSave.MapFilePath;
      progress?.AddProgress(0.05f);
      SaveDataResult data = new SaveDataResult()
      {
        SaveData = MapSaver.BuildSaveData(instance),
        SerializedData = new MapSerializer()
      };
      progress?.AddProgress(0.05f);
      if (progress != null)
        progress.Factor = 0.1f;
      MapSaver.SaveMapData(mapFilePath, data, progress, true, false);
      gameSave.Header.SaveVersion = data.SaveData.Header.SaveVersion;
      gameSave.Header.DateSaved = data.SaveData.Header.DateSaved;
      Globals2.SaveParticleTemplates();
      Globals2.SaveGlobalScripts(instance.Scripts);
      Globals2.SaveGamertagDataNoLockNoFlush();
      if (progress != null)
        progress.Factor = 0.8f;
      MapSaver.WriteRegions(instance, progress, data.SaveData.Header.TerrainData.Biome == BiomeType.Flat && saveVersion < 158);
      foreach (Mod activePlugin in ModManager.ActivePlugins)
        activePlugin.Plugin.WorldSaved(gameSave.Header.SaveVersion);
    }

    private static void PrepareIfNewSave(GameInstance instance, SaveGameFileInfo saveGame)
    {
      if (saveGame.DirNumber != 0)
        return;
      saveGame.DirNumber = Globals2.GetNewMapDirNumber(saveGame.MapType);
      if (saveGame.Header.MapName != null && !(saveGame.Header.MapName == "") && !(saveGame.Header.MapName == "New"))
        return;
      saveGame.Header.MapName = "World" + (object) saveGame.DirNumber;
    }

    public static void SaveMapData(
      string path,
      SaveDataResult data,
      IProgressBar progress,
      bool allowCleanupOfOldFiles,
      bool saveHeaderDatOnly)
    {
      SaveMapHead header = data.SaveData.Header;
      int saveVersion = header.SaveVersion;
      if (!FileSystem.IsDirExist(path))
        FileSystem.CreateDir(path);
      else if (header.IsAutoSave && !saveHeaderDatOnly)
        Globals2.EmptyDirectory(path);
      if (!FileSystem.IsFileExist(path + "header.par") && allowCleanupOfOldFiles)
        Globals2.EmptyDirectory(path);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          MapSaver.WriteMapData(writer, data.SaveData, progress);
          Globals2.WriteFileWithHash(path + "header.dat", (Stream) memoryStream, writer, saveVersion);
        }
      }
      if (!saveHeaderDatOnly)
      {
        if (data.SerializedData != null)
          data.SerializedData.Serialize(path + "header.bin", data.SaveData.Map as MapTM);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
          {
            MapSaver.WritePlayerData(writer, data.SaveData, progress);
            Globals2.WriteFileWithHash(path + "player.dat", (Stream) memoryStream, writer, saveVersion);
          }
        }
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
          {
            MapSaver.WritePhotoThumbnails(writer, data.SaveData.Map as MapTM);
            Globals2.WriteFileNoHash(path + "photos.dat", writer);
          }
        }
      }
      if (!allowCleanupOfOldFiles)
        return;
      using (Stream output = FileSystem.OpenWrite(path + "header.par"))
      {
        using (BinaryWriter binaryWriter = new BinaryWriter(output))
        {
          binaryWriter.Write(294);
          binaryWriter.Write(0);
          binaryWriter.Write(0);
        }
      }
    }

    public static void WriteMapData(BinaryWriter writer, SaveData data, IProgressBar progress)
    {
      MapSaver.WriteMapHead(writer, data.Header);
      progress?.AddProgress(0.2f);
      MapSaver.WriteMapState(writer, data, false);
      progress?.AddProgress(0.5f);
    }

    public static void WritePlayerData(BinaryWriter writer, SaveData data, IProgressBar progress)
    {
      MapSaver.WritePlayerData(writer, data);
      progress?.AddProgress(0.3f);
    }

    private static void WriteRegions(
      GameInstance instance,
      IProgressBar progress,
      bool clearExistingRegFiles)
    {
      if (clearExistingRegFiles)
        FileSystem.EmptyDir(Globals2.GameProperties.SaveGame.MapFilePath, "*.reg");
      if (progress != null)
        progress.Factor /= (float) instance.Map.Regions.Count;
      foreach (MapRegion mapRegion in instance.Map.Regions.Values)
      {
        try
        {
          MapSaver.SaveRegion(instance, mapRegion as MapRegionTM, progress);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(85, ex);
        }
      }
    }

    public static void SaveRegion(GameInstance instance, MapRegionTM region, IProgressBar progress)
    {
      if (region == null)
        return;
      region.GetChunksToSave(MapSaver.chunksToSave);
      progress?.AddProgress(0.1f);
      if (MapSaver.chunksToSave.Count > 0)
      {
        try
        {
          lock (Globals1.SaveSemaphore)
            MapSaver.SaveRegionCore(instance, region, MapSaver.chunksToSave, progress);
        }
        finally
        {
          MapSaver.chunksToSave.Clear();
        }
      }
      else
        progress?.AddProgress(1f);
    }

    private static void SaveRegionCore(
      GameInstance instance,
      MapRegionTM region,
      List<MapChunk> chunksToSave,
      IProgressBar progress)
    {
      bool flag = false;
      while (true)
      {
        try
        {
          MapSaver.WriteRegion((MapRegion) region, chunksToSave, progress);
          break;
        }
        catch (OutOfMemoryException ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(32, (Exception) ex);
          if (flag)
            throw ex;
          instance.FreeupMemoryForSave();
          flag = true;
        }
      }
    }

    public static void CopyMapFiles(string fromPath, string toPath, bool ignoreHeader)
    {
      foreach (string file in FileSystem.GetFiles(fromPath, "*.*"))
      {
        if (!ignoreHeader || !Globals2.ExtractFileFromPath(file).StartsWith("header.dat"))
          MapSaver.CopyFile(file, toPath);
      }
    }

    private static void CopyFile(string filename, string toPath)
    {
      string path = toPath + Globals2.ExtractFileFromPath(filename);
      using (Stream src = FileSystem.OpenRead(filename))
      {
        using (Stream dest = FileSystem.OpenWrite(path))
        {
          Utils.CopyStream(src, dest, 0, 0);
          dest.Flush();
        }
      }
    }

    public static SaveData BuildSaveData(GameInstance instance)
    {
      SaveData saveData = new SaveData()
      {
        Map = (Map) instance.Map,
        Header = Globals2.GameProperties.SaveGame.Header.Clone()
      };
      saveData.Header.ExeVersion = 27302;
      saveData.Header.SaveVersion = 294;
      saveData.Header.TerrainData.SeaLevel = instance.Map.SeaLevel;
      saveData.Header.CurrentMapBound = instance.Map.MapBound;
      saveData.GameState = MapSaver.BuildGameStateData(instance);
      saveData.PlayerState = instance.BuildPlayerSaveStates();
      saveData.ArcadeState = MapSaver.BuildArcadeStateData(instance);
      saveData.GameSettings = Globals2.GameSettings.Clone();
      saveData.Header.UnusedInt1 = 0;
      return saveData;
    }

    private static SaveGameState BuildGameStateData(GameInstance instance)
    {
      SaveGameState saveGameState = new SaveGameState();
      saveGameState.ItemsEnabled = MapSaver.BuildValidItemsTable();
      saveGameState.GlobaScriptsUsed = new SaveGameState.ScriptResultList(instance.GetAllUsedGlobalScripts);
      saveGameState.TotalGameTime = instance.TotalGameTime;
      saveGameState.SunRotation = instance.SunMoon != null ? instance.SunMoon.Rotation : 0.0f;
      saveGameState.PlayerCount = instance.PlayerCountToSave;
      saveGameState.MaxConcurrentPlayerCount = Globals2.MaxConcurrentPlayers;
      saveGameState.Scrolls = MapSaver.BuildScrollData(instance);
      saveGameState.Blueprints = MapSaver.BuildBlueprintData(instance);
      saveGameState.Zones = MapSaver.BuildZoneData(instance.Map.MapStrategy);
      saveGameState.Books = MapSaver.BuildBookData(instance.Map.MapStrategy);
      saveGameState.Particles = MapSaver.BuildParticleData(instance);
      saveGameState.LockedTable = MapSaver.BuildLockedTable(instance);
      saveGameState.BlockTextures = MapSaver.BuildBlockTextures(instance);
      saveGameState.MapMarkers = instance.MapMarkers.ToArray();
      saveGameState.Scripts = instance.Scripts;
      saveGameState.AdventureScripts = instance.GetAdventureScriptNameList();
      saveGameState.EventScripts = instance.GetEventScriptNameList();
      saveGameState.Teleports = MapSaver.BuildTeleportData(instance.Map.MapStrategy);
      saveGameState.SpawnInventory = new SaveInventoryState();
      saveGameState.LastTransmitterFrequency = (int) instance.MapStrategyTM.LastTransmitterFrequency;
      MapSaver.BuildInventoryData(instance.SpawnInventory, saveGameState.SpawnInventory);
      saveGameState.History = new History(instance.History);
      saveGameState.ClanHistory = instance.ClanHistory;
      if (saveGameState.ClanHistory == null)
        saveGameState.ClanHistory = new Dictionary<string, History>();
      return saveGameState;
    }

    public static SavePlayerState BuildPlayerData(
      GameInstance instance,
      StudioForge.TotalMiner.Player player)
    {
      SavePlayerState savePlayerState = (SavePlayerState) null;
      if (player.SaveState != null)
        savePlayerState = MapSaver.BuildPlayerData(instance, player, player.SaveState);
      return savePlayerState;
    }

    public static SavePlayerState BuildPlayerData(
      GameInstance instance,
      StudioForge.TotalMiner.Player player,
      SavePlayerState result)
    {
      MapSaver.BuildCharacterDataCore(instance, (Actor2) player, (SaveCharacterState) result);
      if (player.Gamer != null)
      {
        result.Gamertag = player.Gamer.Gamertag;
        result.JetPackActive = player.IsFlying;
        result.Reach = player.Reach;
        if (player.UnlockData != null)
        {
          result.ScrollsFound = player.UnlockData.SageWisdomsFound;
          result.GoldEarned = player.UnlockData.EntrepreneurGoldEarned;
          result.BedRockProspected = player.UnlockData.KnightBedrockReached;
          result.EnemiesKilledBeforeBedrock = player.UnlockData.KnightEnemiesKilled;
        }
        result.Settings = player.Settings.Clone();
        result.Statistics = player.GetStatisticsClone();
        result.DefaultPriceList = player.DefaultPriceList;
        result.LastTransmitterFrequency = (int) player.LastTransmitterFrequency;
        result.ClanName = player.ClanName;
        result.ClanBannerID = player.ClanBannerID;
        result.History = new History(player.History);
        result.ActionLog = new ActionLog(player.ActionLog);
        result.SkillsData = player.LocalSkillsData;
      }
      return result;
    }

    private static SaveArcadeState BuildArcadeStateData(GameInstance instance)
    {
      return new SaveArcadeState()
      {
        TotalInvadersHighScore = StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore,
        TotalInvadersHighScoreGamer = StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreGamer,
        TotalInvadersHighScoreVersion = StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreVersion,
        TotalRushHighScore = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore,
        TotalRushHighScoreGamer = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreGamer,
        TotalRushHighScoreVersion = StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreVersion
      };
    }

    private static SaveCharacterState BuildCharacterData(
      GameInstance instance,
      Actor2 character)
    {
      SaveCharacterState result = new SaveCharacterState();
      MapSaver.BuildCharacterDataCore(instance, character, result);
      return result;
    }

    private static void BuildCharacterBaseDataCore(
      GameInstance instance,
      StudioForge.TotalMiner.Actor character,
      SaveCharacterBaseState result)
    {
      result.Position = character.Position;
      result.Health = character.Health;
      result.Seed = character.Seed;
      result.Inventory = MapSaver.BuildCharacterInventoryData(character);
    }

    private static void BuildCharacterDataCore(
      GameInstance instance,
      Actor2 character,
      SaveCharacterState result)
    {
      MapSaver.BuildCharacterBaseDataCore(instance, (StudioForge.TotalMiner.Actor) character, (SaveCharacterBaseState) result);
      result.ViewAngle = character.ViewAngle;
      result.Oxygen = character.Oxygen;
      result.Permission = character.Permission;
    }

    private static SaveInventoryState BuildCharacterInventoryData(StudioForge.TotalMiner.Actor character)
    {
      SaveInventoryState state = new SaveInventoryState();
      MapSaver.BuildInventoryData((Inventory) character.Inventory, state);
      return state;
    }

    private static void BuildInventoryData(Inventory inventory, SaveInventoryState state)
    {
      if (inventory == null)
        return;
      state.PackSize = inventory.PackSize;
      state.EquipSize = inventory.EquipSize;
      state.TempSize = inventory.TempSize;
      state.AllowZeroCountItems = inventory.AllowZeroCountItems;
      EquipmentInventory equipmentInventory = inventory as EquipmentInventory;
      if (equipmentInventory != null)
      {
        state.HotBarLeftID = (ushort) equipmentInventory.HotBarLeftSlotID;
        state.HotBarRightID = (ushort) equipmentInventory.HotBarRightSlotID;
      }
      for (int index = 0; index < inventory.Count; ++index)
      {
        InventoryItem inventoryItem = inventory[index];
        if ((inventory.AllowZeroCountItems ? inventoryItem.ItemID_Raw != Item.None && inventoryItem.ItemID_Raw != Item.Clipboard : inventoryItem.ItemID != Item.None && inventoryItem.ItemID != Item.Clipboard && inventoryItem.Count > 0) && ItemData.IsEnabled(inventoryItem.ItemID))
          state.Items.Add(new SaveInventoryItem()
          {
            SlotID = (ushort) index,
            ItemID = (ushort) inventoryItem.ItemID_Raw,
            Count = inventoryItem.Count,
            Durability = inventoryItem.Durability
          });
      }
    }

    private static void BuildPlayerData(
      Map map,
      List<SavePlayerBlockState> state,
      Dictionary<long, DoorBlock> lockedDoors)
    {
      foreach (DoorBlock doorBlock in lockedDoors.Values)
        state.Add(new SavePlayerBlockState()
        {
          Point = doorBlock.Point,
          Gamertag = doorBlock.Gamertag
        });
    }

    private static WisdomScrollState[] BuildScrollData(GameInstance instance)
    {
      WisdomScrollState[] wisdomScrollStateArray = new WisdomScrollState[Wisdom.WisdomList.Length];
      for (int index = 0; index < Wisdom.WisdomList.Length; ++index)
      {
        WisdomItem wisdom = Wisdom.WisdomList[index];
        wisdomScrollStateArray[index] = new WisdomScrollState()
        {
          Point = wisdom.Point,
          IsEnabled = wisdom.IsEnabled,
          IsGenerated = wisdom.IsGenerated
        };
      }
      return wisdomScrollStateArray;
    }

    private static BlueprintState[] BuildBlueprintData(GameInstance instance)
    {
      BlueprintState[] blueprintStateArray = new BlueprintState[Blueprints.BlueprintList.Length];
      for (int index = 0; index < Blueprints.BlueprintList.Length; ++index)
      {
        Blueprint blueprint = Blueprints.BlueprintList[index];
        blueprintStateArray[index] = new BlueprintState()
        {
          Point = blueprint.Point,
          IsEnabled = blueprint.IsEnabled,
          IsUnearthed = blueprint.IsUnearthed,
          IsGenerated = blueprint.IsGenerated
        };
      }
      return blueprintStateArray;
    }

    private static List<SaveZoneState> BuildZoneData(MapStrategy strategy)
    {
      List<SaveZoneState> saveZoneStateList = new List<SaveZoneState>();
      foreach (Zone zone in (strategy as MapStrategyTM).Zones)
      {
        if (!zone.GamerID.IsGamer)
        {
          SaveZoneState saveZoneState = new SaveZoneState()
          {
            Name = zone.Name,
            Type = zone.ZoneType,
            Min = zone.Min,
            Max = zone.Max,
            Builder = zone.Builder,
            BuilderType = zone.BuilderType,
            OnEntryScript = zone.OnEntryScriptName,
            OnExitScript = zone.OnExitScriptName,
            CombatLevelDifference = zone.CombatLevelDifference,
            SpeedMultiplier = zone.SpeedMultiplier,
            GravityMultiplier = zone.GravityMultiplier
          };
          saveZoneStateList.Add(saveZoneState);
        }
      }
      return saveZoneStateList;
    }

    private static List<SaveBookState> BuildBookData(MapStrategy strategy)
    {
      List<SaveBookState> saveBookStateList = new List<SaveBookState>();
      foreach (BookData bookData in (strategy as MapStrategyTM).BookDataList.Values)
      {
        SaveBookState saveBookState = new SaveBookState()
        {
          ID = bookData.ID,
          Title = bookData.Title,
          Text = bookData.Text
        };
        saveBookStateList.Add(saveBookState);
      }
      return saveBookStateList;
    }

    private static List<SaveTeleportState> BuildTeleportData(
      MapStrategy strategy)
    {
      List<SaveTeleportState> saveTeleportStateList = new List<SaveTeleportState>();
      foreach (TeleportBlock teleport in (strategy as MapStrategyTM).Teleports)
      {
        SaveTeleportState saveTeleportState = new SaveTeleportState()
        {
          Point = teleport.Point,
          Channel = teleport.Channel
        };
        saveTeleportStateList.Add(saveTeleportState);
      }
      return saveTeleportStateList;
    }

    private static List<SaveItemParticle> BuildParticleData(
      GameInstance instance)
    {
      return new List<SaveItemParticle>();
    }

    private static void BuildParticleData(
      ItemParticleSystem particleSystem,
      List<SaveItemParticle> result)
    {
      LinkedList<int> indicesUsed = particleSystem.IndicesUsed;
      ItemParticle[] particles = particleSystem.Particles;
      LinkedListNode<int> linkedListNode = indicesUsed.First;
      SaveItemParticle saveItemParticle = new SaveItemParticle();
      float num = 100000f;
      for (; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        int index = linkedListNode.Value;
        ItemParticle itemParticle = particles[index];
        if ((double) itemParticle.Age > (double) num)
        {
          saveItemParticle.Item = itemParticle.Item;
          saveItemParticle.Position = itemParticle.Position;
          result.Add(saveItemParticle);
        }
      }
    }

    private static bool[] BuildLockedTable(GameInstance instance)
    {
      return instance.LockedTable;
    }

    private static bool[] BuildValidItemsTable()
    {
      bool[] flagArray = new bool[Globals1.ItemData.Length];
      for (int index = 0; index < Globals1.ItemData.Length; ++index)
        flagArray[index] = Globals1.ItemData[index].IsEnabled;
      return flagArray;
    }

    private static Block[,] BuildBlockTextures(GameInstance instance)
    {
      return instance.Map.BlockTextures;
    }

    private static void WriteMapState(BinaryWriter writer, SaveData data, bool isForClient)
    {
      MapSaver.WriteGameStateData(writer, data.GameState, isForClient);
      MapSaver.WriteArcadeStateData(writer, data.ArcadeState);
      data.GameSettings.WriteState(writer);
    }

    private static void WriteMapStateForClient(
      GameInstance instance,
      BinaryWriter writer,
      SaveData data)
    {
      MapSaver.WriteMapState(writer, data, true);
      MapSaver.WritePlayerDataForClient(instance, writer, data);
    }

    private static void WriteMapHead(BinaryWriter writer, SaveMapHead head)
    {
      head.ExeVersion = 27302;
      head.SaveVersion = 294;
      writer.Write(head.SaveVersion);
      writer.Write(head.ExeVersion);
      writer.Write(head.CreatedVersion);
      writer.Write(head.MapName);
      writer.Write(head.OwnerGamerTag);
      writer.Write(head.DateCreated);
      writer.Write(head.DateSaved = Utils.DateToBinary(DateTime.Now));
      writer.Write(head.IsAutoSave);
      writer.Write(head.TotalMapBound.Min.X);
      writer.Write(head.TotalMapBound.Min.Y);
      writer.Write(head.TotalMapBound.Min.Z);
      writer.Write(head.TotalMapBound.Max.X);
      writer.Write(head.TotalMapBound.Max.Y);
      writer.Write(head.TotalMapBound.Max.Z);
      writer.Write(head.CurrentMapBound.Min.X);
      writer.Write(head.CurrentMapBound.Min.Y);
      writer.Write(head.CurrentMapBound.Min.Z);
      writer.Write(head.CurrentMapBound.Max.X);
      writer.Write(head.CurrentMapBound.Max.Y);
      writer.Write(head.CurrentMapBound.Max.Z);
      writer.Write((ushort) head.RegionSize.X);
      writer.Write((ushort) head.RegionSize.Y);
      writer.Write((ushort) head.RegionSize.Z);
      writer.Write((ushort) head.ChunkSize.X);
      writer.Write((ushort) head.ChunkSize.Y);
      writer.Write((ushort) head.ChunkSize.Z);
      writer.Write(head.MapSeed);
      writer.Write((int) head.Format);
      writer.Write(head.GameType);
      writer.Write((int) head.GameMode);
      writer.Write(head.HoursSlept);
      writer.Write(head.RatingCount * 144);
      writer.Write(head.UnusedInt1);
      writer.Write(head.DepthReached);
      writer.Write((int) head.Attribute);
      writer.Write(head.RatingStars * 4498.25f);
      writer.Write((int) head.GameDifficulty);
      writer.Write(head.PvPCombat);
      writer.Write(head.CombatEnabled);
      writer.Write(head.FiniteMode);
      writer.Write(head.PassiveMobs);
      writer.Write(head.EnemyMobs);
      writer.Write(head.KeepItemsOnDeath);
      writer.Write(head.SkillsEnabled);
      writer.Write(head.SkillsLocal);
      writer.Write(head.XPMultiplier);
      writer.Write(head.DayNightActive);
      writer.Write(head.WeatherActive);
      writer.Write(head.WindFactor);
      writer.Write(head.UnusedByte1);
      writer.Write(head.DaysIntoGame);
      writer.Write((int) head.DefaultPermission);
      writer.Write(head.MaxPlayers);
      writer.Write(head.PrivateSlots);
      writer.Write(head.CombatLevelDifference);
      writer.Write(head.ClanProtection);
      writer.Write(head.TexturePack);
      writer.Write((int) head.TerrainData.Biome);
      writer.Write((ushort) head.TerrainData.GroundBlock);
      writer.Write(head.TerrainData.Iterations);
      writer.Write(head.TerrainData.MaxParticles);
      writer.Write(head.TerrainData.SeaLevel);
      head.BiomeParams.WriteState(writer);
      Globals1.WriteStringList(writer, ModManager.GetModNames(ModManager.ActiveMods));
    }

    private static void WriteGameStateData(
      BinaryWriter writer,
      SaveGameState data,
      bool isForClient)
    {
      writer.Write((byte) data.MaxConcurrentPlayerCount);
      Globals1.WriteRandBuffer(writer, 8);
      writer.Write(data.TotalGameTime);
      writer.Write(data.SunRotation);
      writer.Write(data.PlayerCount);
      writer.Write(data.LastTransmitterFrequency);
      writer.Write(data.Blueprints.Length);
      foreach (BlueprintState blueprint in data.Blueprints)
      {
        writer.Write(blueprint.IsUnearthed);
        writer.Write(blueprint.IsUnearthed);
        writer.Write(blueprint.IsGenerated);
        writer.Write(blueprint.Point.X);
        writer.Write(blueprint.Point.Y);
        writer.Write(blueprint.Point.Z);
      }
      writer.Write(data.ItemsEnabled.Length);
      foreach (bool flag in data.ItemsEnabled)
        writer.Write(flag);
      writer.Write(data.Scrolls.Length);
      foreach (WisdomScrollState scroll in data.Scrolls)
      {
        writer.Write(scroll.IsEnabled);
        writer.Write(scroll.IsGenerated);
        writer.Write(scroll.Point.X);
        writer.Write(scroll.Point.Y);
        writer.Write(scroll.Point.Z);
      }
      writer.Write(data.Zones.Count);
      foreach (SaveZoneState zone in data.Zones)
        MapSaver.WriteZoneData(writer, zone);
      writer.Write(data.Books.Count);
      foreach (SaveBookState book in data.Books)
        MapSaver.WriteBookData(writer, book);
      writer.Write(data.LockedTable.Length);
      foreach (bool flag in data.LockedTable)
        writer.Write(flag);
      writer.Write(data.Particles.Count);
      foreach (SaveItemParticle particle in data.Particles)
      {
        writer.Write((ushort) particle.Item.ItemID);
        writer.Write(particle.Item.Count);
        writer.Write(particle.Item.Durability);
        writer.Write(particle.Position.X);
        writer.Write(particle.Position.Y);
        writer.Write(particle.Position.Z);
      }
      MapSaver.WriteMapFloodUpdates(data, writer);
      MapSaver.WriteBlockTextures(writer, data.BlockTextures);
      List<MapMarker> mapMarkerList = new List<MapMarker>(data.MapMarkers.Length);
      foreach (MapMarker mapMarker in data.MapMarkers)
      {
        if (mapMarker.Type != MapMarkerType.Graveyard)
          mapMarkerList.Add(mapMarker);
      }
      writer.Write(mapMarkerList.Count);
      foreach (MapMarker mapMarker in mapMarkerList)
      {
        writer.Write(mapMarker.Point.X);
        writer.Write(mapMarker.Point.Y);
        writer.Write(mapMarker.Point.Z);
        writer.Write((byte) mapMarker.Type);
        writer.Write(mapMarker.Label != null ? mapMarker.Label : "");
      }
      MapSaver.WriteInventory(writer, data.SpawnInventory);
      MapSaver.WriteScripts(writer, data.Scripts, false, false, isForClient, data);
      MapSaver.WriteBehaviours(writer);
      MapSaver.WriteStringList(writer, data.AdventureScripts);
      writer.Write(data.EventScripts.Count);
      foreach (KeyValuePair<ScriptEvent, string> eventScript in data.EventScripts)
      {
        writer.Write((byte) eventScript.Key);
        writer.Write(eventScript.Value);
      }
      MapSaver.WriteTeleportData(writer, data);
      MapSaver.WriteHistory(writer, data.History);
      if (data.ClanHistory == null)
        data.ClanHistory = new Dictionary<string, History>();
      lock (data.ClanHistory)
      {
        int num = 0;
        foreach (KeyValuePair<string, History> keyValuePair in data.ClanHistory)
        {
          if (keyValuePair.Value.TableCount > 0)
            ++num;
        }
        writer.Write(num);
        foreach (KeyValuePair<string, History> keyValuePair in data.ClanHistory)
        {
          if (keyValuePair.Value.TableCount > 0)
          {
            writer.Write(keyValuePair.Key);
            MapSaver.WriteHistory(writer, keyValuePair.Value);
          }
        }
      }
    }

    private static void WriteTeleportData(BinaryWriter writer, SaveGameState data)
    {
      writer.Write(data.Teleports.Count);
      foreach (SaveTeleportState teleport in data.Teleports)
      {
        writer.Write((short) teleport.Point.X);
        writer.Write((short) teleport.Point.Y);
        writer.Write((short) teleport.Point.Z);
        writer.Write(teleport.Channel);
      }
    }

    public static List<Script> WriteSystemScripts(
      BinaryWriter writer,
      List<Script> scripts)
    {
      return MapSaver.WriteScripts(writer, scripts, false, true, false, (SaveGameState) null);
    }

    public static List<Script> WriteGlobalScripts(
      BinaryWriter writer,
      List<Script> scripts)
    {
      return MapSaver.WriteScripts(writer, scripts, true, false, false, (SaveGameState) null);
    }

    private static List<Script> WriteScripts(
      BinaryWriter writer,
      List<Script> scripts,
      bool global,
      bool system,
      bool isForClient,
      SaveGameState data)
    {
      long position1 = writer.BaseStream.Position;
      writer.Write(0);
      List<Script> scriptList1 = new List<Script>();
      List<Script> scriptList2 = global || data == null || data.GlobaScriptsUsed == null ? (List<Script>) null : data.GlobaScriptsUsed();
      for (int index = 0; index < scripts.Count; ++index)
      {
        Script script = scripts[index];
        bool flag = isForClient;
        if (!isForClient)
        {
          if (script.Name.StartsWith("global\\", StringComparison.OrdinalIgnoreCase))
            flag = global || data == null || scriptList2 == null || scriptList2.Contains(script);
          else if (script.Name.StartsWith("system\\", StringComparison.OrdinalIgnoreCase))
            flag = system;
          else if (!global && !system)
            flag = true;
        }
        if (flag)
        {
          MapSaver.WriteScript(writer, script);
          scriptList1.Add(script);
        }
      }
      long position2 = writer.BaseStream.Position;
      writer.BaseStream.Position = position1;
      writer.Write(scriptList1.Count);
      writer.BaseStream.Position = position2;
      return scriptList1;
    }

    private static void WriteScript(BinaryWriter writer, Script script)
    {
      writer.Write(script.Name);
      writer.Write(script.Alias);
      MapSaver.WriteStringList(writer, script.Commands);
    }

    public static string SaveScript(string path, Script script)
    {
      string path1 = path + script.Name.Replace('\\', '_') + ".scr";
      using (Stream stream = FileSystem.OpenWrite(path1))
      {
        using (StreamWriter streamWriter = new StreamWriter(stream))
        {
          for (int line = 0; line < script.Commands.Count; ++line)
          {
            string str = script.Commands[line];
            if (script.IsInConditionalBlock(line))
              str = "   " + str;
            streamWriter.WriteLine(str);
          }
        }
      }
      return path1;
    }

    private static void WriteBehaviours(BinaryWriter writer)
    {
      long position1 = writer.BaseStream.Position;
      int num = 0;
      writer.Write(num);
      if (Globals1.BehaviourTrees != null)
      {
        foreach (BehaviourTree behaviourTree in Globals1.BehaviourTrees)
        {
          if (!behaviourTree.Immutable && !behaviourTree.Name.StartsWith("Global\\", StringComparison.OrdinalIgnoreCase))
          {
            behaviourTree.WriteState(writer);
            ++num;
          }
        }
      }
      long position2 = writer.BaseStream.Position;
      writer.BaseStream.Position = position1;
      writer.Write(num);
      writer.BaseStream.Position = position2;
    }

    private static void WriteStringList(BinaryWriter writer, List<string> list)
    {
      writer.Write(list.Count);
      for (int index = 0; index < list.Count; ++index)
        writer.Write(list[index]);
    }

    public static void WriteBlockTextures(BinaryWriter writer, Block[,] blockTextures)
    {
      writer.Write(blockTextures.GetLength(0));
      writer.Write(blockTextures.GetLength(1));
      for (int index1 = 0; index1 < blockTextures.GetLength(0); ++index1)
      {
        for (int index2 = 0; index2 < blockTextures.GetLength(1); ++index2)
          writer.Write((byte) blockTextures[index1, index2]);
      }
    }

    private static void WriteRandBuffer(BinaryWriter writer, int max)
    {
      PcgRandom pcgRandom = new PcgRandom(new Random().Next());
      byte num = (byte) pcgRandom.Next(max);
      writer.Write(num);
      for (int index = 0; index < (int) num; ++index)
        writer.Write((byte) pcgRandom.Next((int) byte.MaxValue));
    }

    private static void WriteSignData(BinaryWriter writer, SaveSignsState data)
    {
      writer.Write(data.SignCount);
      if (data.SignCount <= 0)
        return;
      writer.Write(data.SignText.Count);
      for (int index = 0; index < data.SignText.Count; ++index)
        writer.Write(data.SignText[index]);
      foreach (SaveSignState sign in data.Signs)
        MapSaver.WriteSignData(writer, sign);
    }

    private static void WriteArcadeStateData(BinaryWriter writer, SaveArcadeState data)
    {
      writer.Write(data.TotalInvadersHighScore);
      writer.Write(data.TotalInvadersHighScoreGamer);
      writer.Write(data.TotalInvadersHighScoreVersion);
      writer.Write(data.TotalRushHighScore);
      writer.Write(data.TotalRushHighScoreGamer);
      writer.Write(data.TotalRushHighScoreVersion);
    }

    private static void WritePhotoThumbnails(BinaryWriter writer, MapTM map)
    {
      if (map != null)
      {
        int num = 0;
        for (int textureIndex = 1; textureIndex < 16; ++textureIndex)
        {
          if (map.GetBlockTextureID(Block.Painting, textureIndex) != Block.None)
            ++num;
        }
        writer.Write(num);
        for (int textureIndex = 1; textureIndex < 16; ++textureIndex)
        {
          if (map.GetBlockTextureID(Block.Painting, textureIndex) != Block.None)
          {
            writer.Write(textureIndex);
            MapSaver.WriteColorArray(writer, GraphicStatics.PhotoData.PhotoThumbnail64ColorData[textureIndex]);
            MapSaver.WriteColorArray(writer, GraphicStatics.PhotoData.PhotoThumbnail16ColorData[textureIndex]);
          }
        }
      }
      else
        writer.Write(0);
    }

    public static void WriteColorArray(BinaryWriter writer, Color[] colorData)
    {
      int num = colorData != null ? colorData.Length : 0;
      writer.Write(num);
      if (num <= 0)
        return;
      for (int index = 0; index < num; ++index)
      {
        Color color = colorData[index];
        writer.Write(color.R);
        writer.Write(color.G);
        writer.Write(color.B);
      }
    }

    private static void WritePlayerData(BinaryWriter writer, SaveData data)
    {
      writer.Write(Globals2.BuildMapID(data.Header));
      int num = 0;
      for (int index = 0; index < data.PlayerState.Count; ++index)
      {
        if (data.PlayerState[index].Gamertag != null)
          ++num;
      }
      writer.Write(num);
      for (int index = 0; index < data.PlayerState.Count; ++index)
      {
        if (data.PlayerState[index].Gamertag != null)
          MapSaver.WritePlayerData(writer, data.PlayerState[index]);
      }
    }

    private static void WritePlayerDataForClient(
      GameInstance instance,
      BinaryWriter writer,
      SaveData data)
    {
      NetworkGamer[] array = instance.NetworkManager.AllGamers.ToArray();
      int num = 0;
      foreach (Gamer gamer in array)
      {
        if (gamer.Tag is StudioForge.TotalMiner.Player)
          ++num;
      }
      writer.Write(num);
      foreach (NetworkGamer networkGamer in array)
      {
        StudioForge.TotalMiner.Player tag = networkGamer.Tag as StudioForge.TotalMiner.Player;
        if (tag != null)
        {
          SavePlayerState data1 = MapSaver.BuildPlayerData(instance, tag);
          writer.Write(MapSaver.GetGamerSaveIndex(instance, networkGamer.Gamertag));
          MapSaver.WritePlayerData(writer, data1);
        }
      }
    }

    private static int GetGamerSaveIndex(GameInstance instance, string gamertag)
    {
      for (int index = 0; index < instance.PlayerSaves.Count; ++index)
      {
        if (instance.PlayerSaves[index].Gamertag == gamertag)
          return index;
      }
      return -1;
    }

    private static void WritePlayerData(BinaryWriter writer, SavePlayerState data)
    {
      MapSaver.WriteCharacterData(writer, (SaveCharacterState) data);
      Globals2.WriteGamertag(writer, data.Gamertag);
      writer.Write(data.IsNewPlayer);
      writer.Write(data.ClanName == null ? "" : data.ClanName);
      writer.Write(data.ClanBannerID);
      writer.Write(data.JetPackActive);
      writer.Write((byte) data.Reach);
      writer.Write(data.CraftInstructionMessageShown);
      writer.Write(data.NewComPackMessageShown);
      writer.Write(data.Message4);
      writer.Write(data.LastTransmitterFrequency);
      writer.Write(data.RatingStars);
      writer.Write(data.GoldEarned);
      writer.Write(data.ScrollsFound);
      writer.Write(data.BedRockProspected);
      writer.Write(data.EnemiesKilledBeforeBedrock);
      writer.Write(data.ItemsCrafted.Length);
      foreach (ushort num in data.ItemsCrafted)
        writer.Write(num);
      writer.Write(data.Settings.GamePadSensitivity);
      writer.Write(data.Settings.MouseSensitivity);
      writer.Write(data.Settings.FOVNormalized);
      writer.Write(data.Settings.HudVisibleForSettingsSave);
      writer.Write(data.Settings.MapVisible);
      writer.Write(data.Settings.RumbleOn);
      writer.Write((byte) data.Settings.Nameplates);
      writer.Write(data.Settings.InvertY);
      writer.Write(data.Settings.BlueprintFinderVisible);
      writer.Write(data.Settings.AutoplaceTime);
      writer.Write(data.Settings.HotBarToTransparentTime);
      writer.Write(data.Settings.DisplayXPGains);
      writer.Write(data.Settings.Bobbing);
      writer.Write(0.0f);
      writer.Write(data.Settings.MobNameplates);
      writer.Write((byte) data.Settings.CameraType);
      writer.Write((byte) data.Settings.UserControlSetting);
      writer.Write((byte) data.Settings.WieldType);
      writer.Write(data.Statistics.SecondsPlayed);
      writer.Write(data.Statistics.DistanceWalked);
      writer.Write(data.Statistics.DistanceFlown);
      writer.Write(data.Statistics.BlocksCleared);
      writer.Write(data.Statistics.BlocksPlaced);
      writer.Write(data.Statistics.BlocksPickedUp);
      writer.Write(data.Statistics.ItemsPickedUp);
      writer.Write(data.Statistics.DamageDealt);
      writer.Write(data.Statistics.DamageTaken);
      writer.Write(data.Statistics.TotalKills);
      writer.Write(data.Statistics.TotalDeaths);
      writer.Write(data.Statistics.PlayerKills);
      writer.Write(data.Statistics.NPCKills);
      writer.Write(data.Statistics.LootValue);
      writer.Write(data.Statistics.GrenadesLaunched);
      writer.Write(data.Statistics.FillerInt1);
      writer.Write(data.Statistics.FillerInt2);
      writer.Write(data.Statistics.FillerInt3);
      writer.Write(data.Statistics.FillerInt4);
      writer.Write(data.Statistics.FillerInt5);
      writer.Write(data.Statistics.FillerInt6);
      writer.Write(data.Statistics.FillerInt7);
      writer.Write(data.Statistics.FillerInt8);
      writer.Write(data.Statistics.FillerInt9);
      writer.Write(data.Statistics.FillerInt10);
      writer.Write(data.Statistics.FillerFloat1);
      writer.Write(data.Statistics.FillerFloat2);
      writer.Write(data.Statistics.FillerFloat3);
      writer.Write(data.Statistics.FillerFloat4);
      writer.Write(data.Statistics.FillerFloat5);
      if (data.Waypoint.HasValue)
      {
        writer.Write(data.Waypoint.Value.X);
        writer.Write(data.Waypoint.Value.Z);
      }
      else
        writer.Write(-1);
      MapSaver.WritePriceList(writer, data.DefaultPriceList);
      MapSaver.WriteHistory(writer, data.History);
      data.ActionLog.WriteState(writer);
      if (data.SkillsData != null)
        data.SkillsData.WriteState(writer);
      else
        writer.Write(0);
    }

    private static void WriteHistory(BinaryWriter writer, History history)
    {
      if (history != null)
        history.WriteState(writer);
      else
        writer.Write(0);
    }

    private static void WritePriceList(BinaryWriter writer, PriceList priceList)
    {
      if (priceList == null)
      {
        writer.Write(0);
      }
      else
      {
        writer.Write(priceList.Prices.Length);
        for (int index = 0; index < priceList.Prices.Length; ++index)
          MapSaver.WritePrice(writer, priceList.Prices[index]);
      }
    }

    private static void WritePrice(BinaryWriter writer, PriceList.Price price)
    {
      writer.Write(price.Buy);
      writer.Write(price.Sell);
      writer.Write(price.Perc);
      writer.Write(price.UsePerc);
      writer.Write(price.ForSale);
    }

    private static void WriteCharacterData(BinaryWriter writer, SaveCharacterState data)
    {
      MapSaver.WriteCharacterBaseData(writer, (SaveCharacterBaseState) data);
      writer.Write(data.ViewAngle.X);
      writer.Write(data.ViewAngle.Y);
      writer.Write(data.Oxygen);
      writer.Write((ushort) data.Permission);
    }

    private static void WriteCharacterBaseData(BinaryWriter writer, SaveCharacterBaseState data)
    {
      writer.Write((byte) data.MobType);
      writer.Write(data.Position.X);
      writer.Write(data.Position.Y);
      writer.Write(data.Position.Z);
      writer.Write(data.Health);
      writer.Write(data.Seed);
      MapSaver.WriteInventory(writer, data.Inventory);
    }

    private static void WriteInventory(BinaryWriter writer, SaveInventoryState state)
    {
      writer.Write(state.PackSize);
      writer.Write(state.EquipSize);
      writer.Write(state.TempSize);
      writer.Write(state.HotBarLeftID);
      writer.Write(state.HotBarRightID);
      writer.Write(state.Items.Count);
      writer.Write(state.AllowZeroCountItems);
      writer.Write((short) 0);
      writer.Write((short) 0);
      foreach (SaveInventoryItem saveInventoryItem in state.Items)
      {
        writer.Write(saveInventoryItem.SlotID);
        writer.Write(saveInventoryItem.ItemID);
        writer.Write(saveInventoryItem.Count);
        writer.Write(saveInventoryItem.Durability);
      }
    }

    private static void WritePlayerBlockData(BinaryWriter writer, SavePlayerBlockState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      writer.Write(state.Gamertag == null ? "" : state.Gamertag);
    }

    private static void WriteChestData(BinaryWriter writer, SaveChestState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      MapSaver.WriteInventory(writer, (SaveInventoryState) state);
      writer.Write(state.Gamertag == null ? "" : state.Gamertag);
    }

    private static void WriteFurnaceData(BinaryWriter writer, SaveFurnaceState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      writer.Write(state.BurnTime);
      writer.Write(state.SmeltTime);
      writer.Write(state.MaterialPlacer == null ? "" : state.MaterialPlacer);
      MapSaver.WriteInventory(writer, (SaveInventoryState) state);
    }

    private static void WriteSentryTurretData(BinaryWriter writer, SaveSentryTurretState state)
    {
      MapSaver.WriteChestData(writer, (SaveChestState) state);
      writer.Write(state.Cooldown);
      writer.Write((byte) state.TargetTypes);
      writer.Write(state.RequiresPower);
    }

    private static void WriteMineBlockData(BinaryWriter writer, SaveMineBlockState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      writer.Write((byte) state.TargetTypes);
      writer.Write(state.TriggerDelay);
      writer.Write(state.TriggerRadius);
      writer.Write(state.BlastRadius);
      writer.Write(state.DetonateInRange);
      writer.Write(state.Gamertag == null ? "" : state.Gamertag);
    }

    private static void WriteShopBlockData(BinaryWriter writer, SaveShopBlockState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      writer.Write(state.Gamertag == null ? "" : state.Gamertag);
      MapSaver.WriteInventory(writer, state.Inventory);
      MapSaver.WritePriceList(writer, state.PriceList);
    }

    private static void WriteFireData(BinaryWriter writer, SaveFireState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      writer.Write(state.Target.X);
      writer.Write(state.Target.Y);
      writer.Write(state.Target.Z);
      writer.Write(state.Strength);
      writer.Write(state.SpreadTimer);
    }

    private static void WriteSignData(BinaryWriter writer, SaveSignState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      writer.Write(state.Text1);
      writer.Write(state.Text2);
      writer.Write(state.Text3);
      writer.Write(state.Text4);
    }

    private static void WriteNPCData(BinaryWriter writer, SaveNPCState state)
    {
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
      writer.Write((short) state.Type);
      writer.Write(state.ViewDirection.X);
      writer.Write(state.ViewDirection.Y);
      writer.Write(state.Text == null ? "" : state.Text);
    }

    private static void WriteZoneData(BinaryWriter writer, SaveZoneState state)
    {
      writer.Write(state.Name);
      writer.Write((byte) state.Type);
      writer.Write(state.Min.X);
      writer.Write(state.Min.Y);
      writer.Write(state.Min.Z);
      writer.Write(state.Max.X);
      writer.Write(state.Max.Y);
      writer.Write(state.Max.Z);
      writer.Write((byte) state.BuilderType);
      Globals2.WriteGamertag(writer, state.Builder == null ? "" : state.Builder);
      writer.Write(state.OnEntryScript == null ? "" : state.OnEntryScript);
      writer.Write(state.OnExitScript == null ? "" : state.OnExitScript);
      writer.Write(state.CombatLevelDifference);
      writer.Write(state.SpeedMultiplier);
      writer.Write(state.GravityMultiplier);
    }

    private static void WriteBookData(BinaryWriter writer, SaveBookState state)
    {
      writer.Write(state.ID);
      writer.Write(state.Title == null ? "" : state.Title);
      int num = state.Text == null ? 0 : state.Text.Length;
      writer.Write(num);
      for (int index = 0; index < num; ++index)
        writer.Write(state.Text[index] == null ? "" : state.Text[index]);
    }

    private static void WriteAmbientSoundData(BinaryWriter writer, SaveAmbientSoundState state)
    {
      writer.Write(state.SoundID);
      writer.Write(state.Volume);
      writer.Write(state.Distance);
      writer.Write(state.LoopDelay);
      writer.Write((byte) state.DayOrNight);
      writer.Write(state.RequiresPower);
      writer.Write(state.Point.X);
      writer.Write(state.Point.Y);
      writer.Write(state.Point.Z);
    }

    private static void WriteScriptBlockData(BinaryWriter writer, SaveScriptBlockState state)
    {
      writer.Write(state.Hash);
      writer.Write(state.PowerOnScript);
      writer.Write(state.PowerOffScript);
    }

    private static void WriteMapFloodUpdates(SaveGameState data, BinaryWriter writer)
    {
      writer.Write(0);
    }

    private static void WriteBoolArray(BinaryWriter writer, bool[] array)
    {
      writer.Write(array.Length);
      foreach (bool flag in array)
        writer.Write(flag);
    }

    private static void WriteGlobalPoint3DList(BinaryWriter writer, List<GlobalPoint3D> list)
    {
      writer.Write(list.Count);
      foreach (GlobalPoint3D globalPoint3D in list)
      {
        writer.Write((short) globalPoint3D.X);
        writer.Write((short) globalPoint3D.Y);
        writer.Write((short) globalPoint3D.Z);
      }
    }

    private static void WriteRegion(
      MapRegion region,
      List<MapChunk> chunksToSave,
      IProgressBar progress)
    {
      int hashCode = region.GetHashCode();
      using (Stream output = FileSystem.OpenWrite(Globals2.GameProperties.SaveGame.MapFilePath + hashCode.ToString() + ".reg"))
      {
        using (BinaryWriter writer = new BinaryWriter(output))
        {
          writer.Write(hashCode);
          writer.Write(region.Offset.X);
          writer.Write(region.Offset.Y);
          writer.Write(region.Offset.Z);
          writer.Write(chunksToSave.Count);
          float increment = 1f / (float) chunksToSave.Count;
          foreach (MapChunk chunk in chunksToSave)
          {
            progress?.AddProgress(increment);
            MapSaver.WriteChunk(writer, chunk);
            writer.Flush();
          }
        }
      }
    }

    private static void WriteChunk(BinaryWriter writer, MapChunk chunk)
    {
      writer.Write(chunk.GetHashCode());
      chunk.WriteData(writer);
    }

    public byte[] GetGameData(GameInstance instance)
    {
      SaveData data = MapSaver.BuildSaveData(instance);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          MapSaver.WriteMapHead(writer, data.Header);
          MapSaver.WriteMapStateForClient(instance, writer, data);
          new MapSerializer().SerializeBinary(writer, instance.Map, true);
          byte[] buffer = new byte[memoryStream.Length];
          memoryStream.Position = 0L;
          memoryStream.Read(buffer, 0, (int) memoryStream.Length);
          return buffer;
        }
      }
    }
  }
}
