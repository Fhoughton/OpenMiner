// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.WifiReceiverBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class WifiReceiverBlock : DataBlock
  {
    public int Index;
    public ushort Frequency1;
    public ushort Frequency2;
    public bool CurrentOutput;
    public BinaryOperatorType Gate;
    public List<long> Transmitters;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.WifiReceiver;
      }
    }

    public WifiReceiverBlock()
    {
    }

    public WifiReceiverBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      WifiReceiverBlock wifiReceiverBlock = from as WifiReceiverBlock;
      this.Index = wifiReceiverBlock.Index;
      this.Frequency1 = wifiReceiverBlock.Frequency1;
      this.Frequency2 = wifiReceiverBlock.Frequency2;
      this.CurrentOutput = wifiReceiverBlock.CurrentOutput;
      this.Gate = wifiReceiverBlock.Gate;
      this.Transmitters = wifiReceiverBlock.Transmitters == null || wifiReceiverBlock.Transmitters.Count <= 0 ? (List<long>) null : new List<long>((IEnumerable<long>) wifiReceiverBlock.Transmitters);
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Index = reader.ReadInt32();
      this.Frequency1 = reader.ReadUInt16();
      this.Frequency2 = reader.ReadUInt16();
      if (version < 174)
      {
        reader.ReadBoolean();
        reader.ReadBoolean();
      }
      this.Gate = (BinaryOperatorType) reader.ReadByte();
      this.CurrentOutput = false;
      this.Transmitters = (List<long>) null;
      if (version <= 173)
        return;
      if (version > 174 && version < 206)
        this.CurrentOutput = reader.ReadBoolean();
      if (version >= 206)
        return;
      int capacity = (int) reader.ReadUInt16();
      if (capacity <= 0)
        return;
      this.Transmitters = new List<long>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.Transmitters.Add(reader.ReadInt64());
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Index);
      writer.Write(this.Frequency1);
      writer.Write(this.Frequency2);
      writer.Write((byte) this.Gate);
    }
  }
}
