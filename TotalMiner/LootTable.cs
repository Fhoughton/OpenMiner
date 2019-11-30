// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.LootTable
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner
{
  public class LootTable
  {
    public List<LootDrop> Table = new List<LootDrop>();
    public GlobalPoint3D? Point;

    public int Count
    {
      get
      {
        return this.Table.Count;
      }
    }

    public LootTable Clone()
    {
      return new LootTable()
      {
        Table = new List<LootDrop>((IEnumerable<LootDrop>) this.Table),
        Point = this.Point
      };
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.Point = new GlobalPoint3D?();
      this.Table.Clear();
      if (version > 183 && reader.ReadBoolean())
        this.Point = new GlobalPoint3D?(new GlobalPoint3D(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
      int num = reader.ReadInt32();
      LootDrop lootDrop = new LootDrop();
      for (int index = 0; index < num; ++index)
      {
        lootDrop.ItemID = (Item) reader.ReadUInt16();
        lootDrop.Count = (int) reader.ReadUInt16();
        lootDrop.Percent = reader.ReadSingle();
        this.Table.Add(lootDrop);
      }
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.Point.HasValue);
      if (this.Point.HasValue)
      {
        writer.Write(this.Point.Value.X);
        writer.Write(this.Point.Value.Y);
        writer.Write(this.Point.Value.Z);
      }
      writer.Write(this.Table.Count);
      for (int index = 0; index < this.Table.Count; ++index)
      {
        writer.Write((ushort) this.Table[index].ItemID);
        writer.Write((ushort) this.Table[index].Count);
        writer.Write(this.Table[index].Percent);
      }
    }
  }
}
