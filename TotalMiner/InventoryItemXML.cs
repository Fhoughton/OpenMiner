// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.InventoryItemXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public struct InventoryItemXML
  {
    public Item ItemID;
    public ushort Durability;
    public int Count;

    public InventoryItemXML(InventoryItem item)
    {
      this.ItemID = item.ItemID_Raw;
      this.Durability = item.Durability;
      this.Count = item.Count;
    }
  }
}
