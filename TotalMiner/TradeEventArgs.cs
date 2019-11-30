// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TradeEventArgs
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  internal class TradeEventArgs
  {
    public Item ItemID;
    public int Quantity;
    public int Value;
    public bool Sell;

    public TradeEventArgs(Item item, int qty, int value, bool sell)
    {
      this.ItemID = item;
      this.Quantity = qty;
      this.Value = value;
      this.Sell = sell;
    }
  }
}
