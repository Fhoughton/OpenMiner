// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SaveInventoryState
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.Collections.Generic;

namespace StudioForge.TotalMiner.Storage
{
  internal class SaveInventoryState
  {
    public List<SaveInventoryItem> Items = new List<SaveInventoryItem>();
    public short PackSize;
    public short EquipSize;
    public short TempSize;
    public ushort HotBarLeftID;
    public ushort HotBarRightID;
    public bool AllowZeroCountItems;
  }
}
