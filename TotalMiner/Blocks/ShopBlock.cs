// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.ShopBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Storage;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class ShopBlock : ChestBlock
  {
    public PriceList PriceList;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Shop;
      }
    }

    public ShopBlock()
    {
    }

    public ShopBlock(GlobalPoint3D p, Inventory inventory)
      : base(p, inventory)
    {
    }

    protected override Inventory CreateInventory(int size)
    {
      return (Inventory) null;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      ShopBlock shopBlock = from as ShopBlock;
      this.PriceList = shopBlock.PriceList != null ? new PriceList(shopBlock.PriceList.Type, shopBlock.PriceList) : (PriceList) null;
    }

    public void LoadFromSaveData(SaveShopBlockState state)
    {
      this.LoadFromSaveData((SavePlayerBlockState) state);
      if (this.Inventory == null)
        this.Inventory = new Inventory(0, 0, 0, true);
      this.Inventory.LoadFromSaveData(state.Inventory);
      this.PriceList = state.PriceList != null ? new PriceList(PriceList.PriceListType.PlayerShop, state.PriceList) : (PriceList) null;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.PriceList = this.ReadPriceList(reader, PriceList.PriceListType.PlayerShop, version);
    }

    private PriceList ReadPriceList(
      BinaryReader reader,
      PriceList.PriceListType type,
      int version)
    {
      PriceList priceList = (PriceList) null;
      if (version > 156)
      {
        int num = reader.ReadInt32();
        if (num > 0)
        {
          priceList = new PriceList(type);
          for (int index = 0; index < num; ++index)
          {
            PriceList.Price price = ShopBlock.ReadPrice(reader, version);
            if (index < priceList.Prices.Length)
              priceList.Prices[index] = price;
          }
        }
      }
      return priceList;
    }

    private static PriceList.Price ReadPrice(BinaryReader reader, int version)
    {
      return new PriceList.Price()
      {
        Buy = reader.ReadInt32(),
        Sell = reader.ReadInt32(),
        Perc = reader.ReadInt32(),
        UsePerc = reader.ReadBoolean(),
        ForSale = reader.ReadBoolean()
      };
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      this.WritePriceList(writer);
    }

    private void WritePriceList(BinaryWriter writer)
    {
      if (this.PriceList == null)
      {
        writer.Write(0);
      }
      else
      {
        writer.Write(this.PriceList.Prices.Length);
        for (int index = 0; index < this.PriceList.Prices.Length; ++index)
          this.WritePrice(writer, this.PriceList.Prices[index]);
      }
    }

    private void WritePrice(BinaryWriter writer, PriceList.Price price)
    {
      writer.Write(price.Buy);
      writer.Write(price.Sell);
      writer.Write(price.Perc);
      writer.Write(price.UsePerc);
      writer.Write(price.ForSale);
    }
  }
}
