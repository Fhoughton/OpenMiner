// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ModItemDataXML
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  public struct ModItemDataXML
  {
    public string ItemID;
    public string Name;
    public string Desc;
    public bool? IsValid;
    public bool? IsEnabled;
    public bool? LockedDD;
    public bool? LockedCR;
    public bool? LockedSU;
    public int? MinCSPrice;
    public int? StackSize;
    public ushort? Durability;
    public float? StrikeDamage;
    public float? StrikeReach;
    public short? HealPower;
    public ushort? BurnTime;
    public float? SmeltTime;
    public byte? ParticleLight;
    public ItemSelectModeFlag? SelectFlag;
    public bool? CanDropIfLocked;
    public ushort? DropChance;
    public PluralType? Plural;
  }
}
