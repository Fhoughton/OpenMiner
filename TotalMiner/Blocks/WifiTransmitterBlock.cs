// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.WifiTransmitterBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class WifiTransmitterBlock : DataBlock
  {
    public int Index;
    public ushort Frequency;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.WifiTransmitter;
      }
    }

    public WifiTransmitterBlock()
    {
    }

    public WifiTransmitterBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      WifiTransmitterBlock transmitterBlock = from as WifiTransmitterBlock;
      this.Index = transmitterBlock.Index;
      this.Frequency = transmitterBlock.Frequency;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Index = reader.ReadInt32();
      this.Frequency = reader.ReadUInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Index);
      writer.Write(this.Frequency);
    }
  }
}
