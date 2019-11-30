// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.PriceList
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System;

namespace StudioForge.TotalMiner.Storage
{
  internal class PriceList
  {
    public PriceList.PriceListType Type;
    public PriceList.Price[] Prices;

    public PriceList(PriceList.PriceListType type)
    {
      this.Type = type;
      this.Prices = Globals2.GetNewDefaultPriceList();
    }

    public PriceList(PriceList.PriceListType type, PriceList copy)
    {
      this.Type = type;
      this.Prices = Globals2.GetNewDefaultPriceList();
      if (copy == null)
        return;
      for (int index = 0; index < this.Prices.Length && index < copy.Prices.Length; ++index)
        this.Prices[index] = copy.Prices[index];
    }

    public enum PriceListType
    {
      PlayerDefault,
      PlayerShop,
      SystemShop,
    }

    public struct Price : IEquatable<PriceList.Price>
    {
      public int Buy;
      public int Sell;
      public int Perc;
      public bool UsePerc;
      public bool ForSale;

      public int FinalBuy
      {
        get
        {
          if (!this.UsePerc)
            return this.Buy;
          return (int) ((double) this.Sell * ((double) this.Perc / 100.0));
        }
      }

      public override bool Equals(object obj)
      {
        return false;
      }

      public bool Equals(PriceList.Price p)
      {
        if (this.Buy == p.Buy && this.Sell == p.Sell && (this.Perc == p.Perc && this.UsePerc == p.UsePerc))
          return this.ForSale == p.ForSale;
        return false;
      }

      public override int GetHashCode()
      {
        return base.GetHashCode();
      }
    }
  }
}
