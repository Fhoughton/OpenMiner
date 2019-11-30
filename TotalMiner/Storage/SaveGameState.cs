// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SaveGameState
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.Collections.Generic;

namespace StudioForge.TotalMiner.Storage
{
  internal class SaveGameState
  {
    public double TotalGameTime;
    public float SunRotation;
    public int PlayerCount;
    public int MaxConcurrentPlayerCount;
    public List<SaveSentryTurretState> SentryTurrets;
    public List<SaveMineBlockState> MineBlocks;
    public List<SaveShopBlockState> ShopBlocks;
    public List<SavePlayerBlockState> LockedDoors;
    public List<SaveChestState> Chests;
    public List<SaveFurnaceState> Furnaces;
    public List<SaveChestState> Bookcases;
    public List<SaveFireState> Fire;
    public SaveSignsState Signs;
    public List<SaveNPCState> NPCs;
    public List<SaveZoneState> Zones;
    public List<SaveBookState> Books;
    public List<SaveAmbientSoundState> AmbientSoundBlocks;
    public WisdomScrollState[] Scrolls;
    public BlueprintState[] Blueprints;
    public List<SaveItemParticle> Particles;
    public List<SaveTeleportState> Teleports;
    public List<SaveScriptBlockState> ScriptBlocks;
    public SaveInventoryState SpawnInventory;
    public bool[] LockedTable;
    public FloodData[] FloodUpdates;
    public Block[,] BlockTextures;
    public bool[] ItemsEnabled;
    public MapMarker[] MapMarkers;
    public List<Script> Scripts;
    public List<string> AdventureScripts;
    public Dictionary<ScriptEvent, string> EventScripts;
    public int LastTransmitterFrequency;
    public History History;
    public Dictionary<string, History> ClanHistory;
    public SaveGameState.ScriptResultList GlobaScriptsUsed;

    public delegate bool ScriptConditional(Script script);

    public delegate List<Script> ScriptResultList();
  }
}
