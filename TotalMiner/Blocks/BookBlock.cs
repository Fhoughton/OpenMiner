// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.BookBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class BookBlock : DataBlock
  {
    public ushort ID;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Book;
      }
    }

    public BookBlock()
    {
    }

    public BookBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      this.ID = (from as BookBlock).ID;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.ID = version > 169 ? reader.ReadUInt16() : (ushort) reader.ReadByte();
      if (version >= 173)
        return;
      reader.ReadString();
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
        reader.ReadString();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.ID);
    }
  }
}
