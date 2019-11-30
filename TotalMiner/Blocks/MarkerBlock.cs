// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.MarkerBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class MarkerBlock : GamerIDBlock
  {
    public bool Exclude;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Marker;
      }
    }

    public MarkerBlock()
    {
    }

    public MarkerBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public MarkerBlock(GlobalPoint3D p, GamerID gamerID)
      : base(p, gamerID)
    {
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      this.Exclude = (from as MarkerBlock).Exclude;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Exclude = false;
      if (version <= 206)
        return;
      this.Exclude = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Exclude);
    }
  }
}
