// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemDataXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public class ItemDataXML
  {
    public Item ItemID;
    public string IDString;
    public string Name;
    public string Desc;
    public bool IsValid;
    public bool IsEnabled;
    public bool LockedDD;
    public bool LockedCR;
    public bool LockedSU;
    public bool HasItemProxy;
    public int MinCSPrice;
    public int StackSize;
    public ushort Durability;
    public float StrikeDamage;
    public float StrikeReach;
    public short HealPower;
    public ushort BurnTime;
    public float SmeltTime;
    public byte ParticleLight;
    public ItemSelectModeFlag SelectFlag;
    public bool CanDropIfLocked;
    public ushort DropChance;
    public PluralType Plural;
  }
}
