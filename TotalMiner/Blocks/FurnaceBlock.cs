// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.FurnaceBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Storage;
using System;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class FurnaceBlock : PlayerBlock
  {
    public const int FuelSlot = 0;
    public const int Ore1Slot = 1;
    public const int Ore2Slot = 2;
    public const int Ore3Slot = 3;
    public const int ProductSlot = 4;
    public Map Map;
    public Inventory Inventory;
    private float burnTime;
    private float smeltTime;
    private Blueprint product;

    public event EventHandler FurnaceBurnStarted;

    public event EventHandler FurnaceBurnEnded;

    public event EventHandler ItemSmelted;

    public void Raise_FurnaceBurnStarted()
    {
      if (this.FurnaceBurnStarted == null)
        return;
      this.FurnaceBurnStarted((object) this, EventArgs.Empty);
    }

    private void Raise_FurnaceBurnEnded()
    {
      if (this.FurnaceBurnEnded == null)
        return;
      this.FurnaceBurnEnded((object) this, EventArgs.Empty);
    }

    private void Raise_ItemSmelted()
    {
      if (this.ItemSmelted == null)
        return;
      this.ItemSmelted((object) this, EventArgs.Empty);
    }

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Furnace;
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

    public float BurnTime
    {
      get
      {
        return this.burnTime;
      }
    }

    public float SmeltTime
    {
      get
      {
        return this.smeltTime;
      }
    }

    public bool IsSmelting
    {
      get
      {
        return (double) this.smeltTime > 0.0;
      }
    }

    public float CurrentBurnTime
    {
      get
      {
        return this.burnTime;
      }
    }

    public float TotalBurnTime(Player player)
    {
      return this.GetBurnTime(player);
    }

    public float BurnCompleteNormalized(GameInstance instance)
    {
      return this.burnTime / this.GetBurnTime(this.GetPlayer(instance));
    }

    public float BurnCompleteNormalized(Player player)
    {
      return this.burnTime / this.GetBurnTime(player);
    }

    public float CurrentSmeltTime
    {
      get
      {
        return this.smeltTime;
      }
    }

    public float TotalSmeltTime(Player player)
    {
      return this.GetSmeltTime(player);
    }

    public float SmeltCompleteNormalized(GameInstance instance)
    {
      return this.SmeltCompleteNormalized(this.GetPlayer(instance));
    }

    public float SmeltCompleteNormalized(Player player)
    {
      return this.smeltTime / this.GetSmeltTime(player);
    }

    public InventoryItem FuelItem
    {
      get
      {
        return this.Inventory[0];
      }
      set
      {
        this.Inventory[0] = value;
      }
    }

    public int FuelItemCount
    {
      get
      {
        return this.FuelItem.Count;
      }
      set
      {
        this.Inventory.SetItemCount(0, value);
      }
    }

    public InventoryItem Ore1Item
    {
      get
      {
        return this.Inventory[1];
      }
      set
      {
        this.Inventory[1] = value;
      }
    }

    public InventoryItem Ore2Item
    {
      get
      {
        return this.Inventory[2];
      }
      set
      {
        this.Inventory[2] = value;
      }
    }

    public InventoryItem Ore3Item
    {
      get
      {
        return this.Inventory[3];
      }
      set
      {
        this.Inventory[3] = value;
      }
    }

    public int Ore1ItemCount
    {
      get
      {
        return this.Ore1Item.Count;
      }
      set
      {
        this.Inventory.SetItemCount(1, value);
      }
    }

    public int Ore2ItemCount
    {
      get
      {
        return this.Ore2Item.Count;
      }
      set
      {
        this.Inventory.SetItemCount(2, value);
      }
    }

    public int Ore3ItemCount
    {
      get
      {
        return this.Ore3Item.Count;
      }
      set
      {
        this.Inventory.SetItemCount(3, value);
      }
    }

    public InventoryItem ProductItem
    {
      get
      {
        return this.Inventory[4];
      }
      set
      {
        this.Inventory[4] = value;
      }
    }

    public Blueprint Product
    {
      get
      {
        return this.product;
      }
      set
      {
        this.product = value;
        InventoryItem inventoryItem = this.Inventory[4];
        if (this.product == null)
        {
          this.smeltTime = 0.0f;
          if (inventoryItem.Count != 0)
            return;
          inventoryItem.ItemID_Raw = Item.None;
          this.Inventory[4] = inventoryItem;
        }
        else
        {
          if (inventoryItem.ItemID_Raw == this.product.Result.ItemID_Raw || inventoryItem.Count != 0)
            return;
          inventoryItem.ItemID = this.product.Result.ItemID_Raw;
          inventoryItem.Count = 0;
          inventoryItem.Durability = this.product.Result.Durability;
          this.Inventory[4] = inventoryItem;
        }
      }
    }

    public int ProductItemCount
    {
      get
      {
        return this.ProductItem.Count;
      }
      set
      {
        this.Inventory.SetItemCount(4, value);
      }
    }

    public void ResetSmeltTime(Player player, float smeltTime)
    {
      this.smeltTime = smeltTime;
    }

    public FurnaceBlock()
    {
    }

    public FurnaceBlock(Map map, GlobalPoint3D p)
      : base(p)
    {
      this.Map = map;
      this.Inventory = new Inventory(5);
    }

    public void Update(GameInstance instance)
    {
      if (this.Map == null)
        return;
      if (this.HasFuel)
      {
        this.BurnFurnace(instance);
        if (this.Map.GetBlockID(this.Point) != (byte) 49 || this.Map.SetBlockData(this.Point, (byte) 133, (byte) 0, UpdateBlockMethod.Player, GamerID.Sys1, false) == null)
          return;
        instance.SetSwitch(this.Point, true, UpdateBlockMethod.Strategy, (Player) null, false);
        this.Map.Commit();
      }
      else
      {
        this.burnTime = 0.0f;
        this.smeltTime = 0.0f;
        if (this.Map.GetBlockID(this.Point) == (byte) 133 && this.Map.SetBlockData(this.Point, (byte) 49, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, false) != null)
        {
          instance.SetSwitch(this.Point, false, UpdateBlockMethod.Strategy, (Player) null, false);
          this.Map.Commit();
        }
        this.Raise_FurnaceBurnEnded();
      }
    }

    private void BurnFurnace(GameInstance instance)
    {
      Player player = this.GetPlayer(instance);
      this.burnTime += Services.ElapsedTime;
      if ((double) this.burnTime >= (double) this.GetBurnTime(player))
      {
        --this.FuelItemCount;
        this.burnTime = 0.0f;
      }
      if (this.Product == null)
        return;
      Item itemId = this.Product.Result.ItemID;
      if (!this.CanSmelt(player, itemId))
        return;
      this.smeltTime += Services.ElapsedTime;
      if ((double) this.smeltTime < (double) this.GetSmeltTime(player))
        return;
      if (this.ProductItem.ItemID == itemId)
        this.ProductItemCount += this.Product.Result.Count;
      else
        this.ProductItem = this.Product.Result;
      this.product.ReduceSmeltItems(this.Inventory);
      this.GetProduct(player);
      this.smeltTime = 0.0f;
      this.Raise_ItemSmelted();
      if (player == null)
        return;
      player.SkillsData.ItemCrafted(player, itemId);
      player.ActionLog.AddAction(itemId, ItemAction.Crafted);
      player.Raise_ItemCrafted(this.ProductItem.ItemID);
    }

    private bool CanSmelt(Player player, Item itemID)
    {
      if (itemID == Item.None || this.ProductItem.ItemID_Raw != itemID || this.ProductItem.Count >= ItemData.GetStackSize(this.ProductItem.ItemID_Raw))
        return false;
      if (player == null)
        return true;
      return player.CanCraftItem(itemID);
    }

    public bool HasFuel
    {
      get
      {
        return (double) this.GetBurnTime((Player) null) > 0.0;
      }
    }

    private float GetBurnTime(Player player)
    {
      float num = 0.0f;
      if (this.FuelItemCount > 0)
      {
        num = (float) ItemData.GetBurnTime(this.FuelItem.ItemID);
        if (player != null && player.GameInstance.IsSkillsEnabled)
          num *= (float) (1.0 + (double) player.SkillsData.Smelting.LevelWithBonuses((Actor) player) / 100.0);
      }
      return num;
    }

    private float GetSmeltTime(Player player)
    {
      float num = 0.0f;
      if (this.Product != null)
      {
        num = Globals1.ItemData[(int) this.Product.Result.ItemID].SmeltTime;
        if (player != null && player.GameInstance.IsSkillsEnabled)
          num *= (float) (1.0 + (double) player.SkillsData.Smelting.LevelWithBonuses((Actor) player) / 200.0);
      }
      return num;
    }

    public Blueprint GetProduct()
    {
      return this.GetProduct((Player) null);
    }

    public Blueprint GetProduct(Player player)
    {
      return this.product = Blueprints.GetSmeltResult(player, this.Ore1Item, this.Ore2Item, this.Ore3Item);
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      FurnaceBlock furnaceBlock = from as FurnaceBlock;
      this.Map = furnaceBlock.Map;
      this.Inventory = new Inventory(furnaceBlock.Inventory);
      this.burnTime = furnaceBlock.burnTime;
      this.smeltTime = furnaceBlock.smeltTime;
      this.GetProduct();
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (this.Inventory == null)
        this.Inventory = new Inventory(5);
      this.Inventory.ReadState(reader, version);
      this.burnTime = reader.ReadSingle();
      this.smeltTime = reader.ReadSingle();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      if (this.Inventory == null)
        this.Inventory = new Inventory(5);
      this.Inventory.WriteState(writer);
      writer.Write(this.burnTime);
      writer.Write(this.smeltTime);
    }

    public void LoadFromSaveData(SaveFurnaceState state)
    {
      this.smeltTime = state.SmeltTime;
      this.burnTime = state.BurnTime;
      if (state.MaterialPlacer != null && state.MaterialPlacer.Length > 0)
      {
        NetworkGamer gamer = NetworkManager.Instance.GetGamer(state.MaterialPlacer);
        if (gamer != null)
          this.Gamertag = gamer.Gamertag;
      }
      this.Inventory.LoadFromSaveData((SaveInventoryState) state);
      this.GetProduct();
    }
  }
}
