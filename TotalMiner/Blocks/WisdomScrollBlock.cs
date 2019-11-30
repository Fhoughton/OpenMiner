// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.WisdomScrollBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class WisdomScrollBlock : DataBlock
  {
    public ushort Index;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.WisdomScroll;
      }
    }

    public WisdomScrollBlock()
    {
    }

    public WisdomScrollBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      this.Index = (from as WisdomScrollBlock).Index;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Index = reader.ReadUInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Index);
    }
  }
}
