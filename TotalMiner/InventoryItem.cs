// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.InventoryItem
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public struct InventoryItem
  {
    public static InventoryItem Empty = new InventoryItem(Item.None, 0);
    private Item itemID;
    private int count;
    public ushort Durability;
    public byte CustomID;

    public int Count
    {
      get
      {
        if (this.count < 0)
          return 0;
        return this.count;
      }
      set
      {
        this.count = value >= 0 ? value : 0;
      }
    }

    public bool ShowDurabilityBar
    {
      get
      {
        switch (this.ItemID)
        {
          case Item.Wisdom:
          case Item.Blueprint:
          case Item.Book:
          case Item.Clipboard:
            return false;
          default:
            return this.MaxDurability > (ushort) 0;
        }
      }
    }

    public ushort MaxDurability
    {
      get
      {
        return ItemData.GetItemDurability(this.itemID);
      }
    }

    public int PurchaseValue
    {
      get
      {
        return ItemData.GetMinCustSellPrice(this.itemID) * this.Count;
      }
    }

    public Item ItemID
    {
      get
      {
        if (this.Count <= 0)
          return Item.None;
        return this.itemID;
      }
      set
      {
        this.itemID = value;
      }
    }

    public Item ItemID_Raw
    {
      get
      {
        return this.itemID;
      }
      set
      {
        this.itemID = value;
      }
    }

    public InventoryItem(Item itemID)
    {
      this = new InventoryItem(itemID, 1);
    }

    public InventoryItem(InventoryItemXML data)
    {
      this = new InventoryItem(data.ItemID, data.Count, data.Durability);
    }

    public InventoryItem(Item itemID, int count)
    {
      this = new InventoryItem(itemID, count, ItemData.GetItemDurability(itemID), (byte) 0);
    }

    public InventoryItem(Item itemID, int count, ushort durability)
    {
      this = new InventoryItem(itemID, count, durability, (byte) 0);
    }

    public InventoryItem(Item itemID, int count, ushort durability, byte customID)
    {
      this.itemID = itemID;
      this.Durability = durability;
      this.count = this.Durability > (ushort) 0 ? 1 : count;
      this.CustomID = customID;
    }
  }
}
