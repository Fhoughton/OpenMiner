// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.BookcaseInventory
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.Blocks
{
  internal class BookcaseInventory : Inventory
  {
    public BookcaseInventory(int size)
      : this(size, false)
    {
    }

    public BookcaseInventory(int size, bool allowZeroCountItems)
      : base(size, 0, 0, allowZeroCountItems)
    {
    }

    public BookcaseInventory(Inventory inventory)
      : base(inventory)
    {
    }

    public override bool ItemAllowed(Item item)
    {
      if (item != Item.None && item != Item.Book && item != Item.Blueprint)
        return item == Item.Wisdom;
      return true;
    }
  }
}
