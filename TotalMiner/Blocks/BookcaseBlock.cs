// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.BookcaseBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.Blocks
{
  internal class BookcaseBlock : ChestBlock
  {
    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Bookcase;
      }
    }

    public BookcaseBlock()
    {
    }

    public BookcaseBlock(GlobalPoint3D p)
      : base(p, 20)
    {
    }

    protected override Inventory CreateInventory(int size)
    {
      return (Inventory) new BookcaseInventory(size);
    }
  }
}
