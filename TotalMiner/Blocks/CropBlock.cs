// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.CropBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class CropBlock : DataBlock
  {
    public ushort Timer;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Crop;
      }
    }

    public CropBlock()
    {
    }

    public CropBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      this.Timer = (from as CropBlock).Timer;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Timer = reader.ReadUInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Timer);
    }
  }
}
