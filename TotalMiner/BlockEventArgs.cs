// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BlockEventArgs
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner
{
  public class BlockEventArgs
  {
    public Block BlockID;
    public Item ItemID;
    public MapBlock BlockData;
    public GlobalPoint3D Point;

    public BlockEventArgs(GlobalPoint3D p, Block blockID)
    {
      this.Point = p;
      this.BlockID = blockID;
    }

    public BlockEventArgs(GlobalPoint3D p, MapBlock blockData)
      : this(p, blockData, (Item) blockData.BlockID)
    {
    }

    public BlockEventArgs(GlobalPoint3D p, MapBlock blockData, Item itemID)
    {
      this.Point = p;
      this.BlockData = blockData;
      this.BlockID = (Block) blockData.BlockID;
      this.ItemID = itemID;
    }
  }
}
