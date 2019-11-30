// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SavePlayerState
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.Storage
{
  internal class SavePlayerState : SaveCharacterState
  {
    public PlayerSettings Settings = new PlayerSettings();
    public PlayerStats Statistics = new PlayerStats();
    public CharacterSkillsData SkillsData = new CharacterSkillsData();
    public string Gamertag;
    public bool IsNewPlayer;
    public bool JetPackActive;
    public bool CraftInstructionMessageShown;
    public bool NewComPackMessageShown;
    public bool Message4;
    public byte RatingStars;
    public int GoldEarned;
    public ushort[] ItemsCrafted;
    public ItemAction[] ItemActions;
    public int ScrollsFound;
    public bool BedRockProspected;
    public int EnemiesKilledBeforeBedrock;
    public int Reach;
    public PriceList DefaultPriceList;
    public int LastTransmitterFrequency;
    public string ClanName;
    public int ClanBannerID;
    public GlobalPoint3D? Waypoint;
    public History History;
    public ActionLog ActionLog;
  }
}
