// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.FireBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class FireBlock : DataBlock
  {
    public double LastElapsed;
    public float SecondsAlive;
    public byte SpreadCount;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Fire;
      }
    }

    public FireBlock()
    {
    }

    public FireBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      FireBlock fireBlock = from as FireBlock;
      this.SecondsAlive = fireBlock.SecondsAlive;
      this.LastElapsed = fireBlock.LastElapsed;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (version <= 131)
        return;
      this.SecondsAlive = reader.ReadSingle();
      this.SpreadCount = reader.ReadByte();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.SecondsAlive);
      writer.Write(this.SpreadCount);
    }
  }
}
