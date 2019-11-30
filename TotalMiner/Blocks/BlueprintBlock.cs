// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.BlueprintBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class BlueprintBlock : DataBlock
  {
    public short ID;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Blueprint;
      }
    }

    public BlueprintBlock()
    {
    }

    public BlueprintBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      this.ID = (from as BlueprintBlock).ID;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (version > 243)
      {
        this.ID = reader.ReadInt16();
      }
      else
      {
        this.ID = (short) reader.ReadUInt16();
        int num = (int) reader.ReadUInt16();
      }
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.ID);
    }
  }
}
