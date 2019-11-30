// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SaveGameStateOld
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Storage
{
  internal class SaveGameStateOld
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
    public List<SaveFireState> Fire;
    public SaveSignsState Signs;
    public List<SaveNPCState> NPCs;
    public List<SaveZoneState> Zones;
    public List<SaveBookState> Notes;
    public List<bool> Scrolls;
    public List<bool> Blueprints;
    public List<GlobalPoint3D> Teleports;
    public List<SaveItemParticle> Particles;
    public SaveInventoryState SpawnInventory;
    public bool[] LockedTable;
    public FloodData[] FloodUpdates;
    public Block[,] BlockTextures;
  }
}
