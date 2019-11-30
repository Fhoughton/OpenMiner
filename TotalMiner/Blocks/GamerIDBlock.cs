// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.GamerIDBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal abstract class GamerIDBlock : DataBlock
  {
    public GamerID GamerID;

    public bool IsOwner(Actor c)
    {
      if (c != null)
        return c.GamerID == this.GamerID;
      return false;
    }

    public GamerIDBlock()
    {
    }

    public GamerIDBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public GamerIDBlock(GlobalPoint3D p, GamerID gamerID)
      : this(p)
    {
      this.GamerID = gamerID;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      this.GamerID = (from as GamerIDBlock).GamerID;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.GamerID = new GamerID(reader.ReadInt16());
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.GamerID.ID);
    }
  }
}
