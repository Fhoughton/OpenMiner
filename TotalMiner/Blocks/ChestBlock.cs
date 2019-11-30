// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.ChestBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Storage;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class ChestBlock : PlayerBlock
  {
    public Inventory Inventory;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Chest;
      }
    }

    public override bool HasInventory
    {
      get
      {
        if (this.Inventory != null)
          return this.Inventory.HasItems();
        return false;
      }
    }

    public ChestBlock()
    {
    }

    public ChestBlock(GlobalPoint3D p, int inventorySize)
      : this(p, inventorySize, (Player) null)
    {
      this.Inventory = this.CreateInventory(inventorySize);
    }

    public ChestBlock(GlobalPoint3D p, int inventorySize, Player player)
      : base(p, player)
    {
      this.Inventory = this.CreateInventory(inventorySize);
    }

    public ChestBlock(GlobalPoint3D p, Inventory inventory)
      : base(p)
    {
      this.Inventory = inventory;
    }

    public ChestBlock(GlobalPoint3D p, Block blockID, Inventory inventory)
      : base(p)
    {
      if (inventory == null)
        inventory = this.CreateInventory(blockID == Block.Crate ? 20 : 50);
      this.Inventory = inventory;
    }

    protected virtual Inventory CreateInventory(int size)
    {
      return new Inventory(size);
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      ChestBlock chestBlock = from as ChestBlock;
      this.Inventory = this.CreateInventory((int) chestBlock.Inventory.PackSize);
      if (this.Inventory == null)
        this.Inventory = new Inventory((int) chestBlock.Inventory.PackSize, 0, 0, chestBlock.Inventory.AllowZeroCountItems);
      this.Inventory.AllowZeroCountItems = chestBlock.Inventory.AllowZeroCountItems;
      this.Inventory.CopyFromWithPreClear(chestBlock.Inventory);
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (this.Inventory == null)
        this.Inventory = new Inventory(0);
      this.Inventory.ReadState(reader, version);
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      if (this.Inventory == null)
        this.Inventory = new Inventory(0);
      this.Inventory.WriteState(writer);
    }

    public void LoadFromSaveData(SaveChestState state)
    {
      this.Point = state.Point;
      this.Gamertag = state.Gamertag;
      if (this.Gamertag == "")
        this.Gamertag = (string) null;
      this.Inventory.LoadFromSaveData((SaveInventoryState) state);
    }
  }
}
