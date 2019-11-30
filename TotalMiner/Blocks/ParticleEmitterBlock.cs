// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.ParticleEmitterBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Graphics;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class ParticleEmitterBlock : DataBlock
  {
    public int EmitCounter;
    public bool RequiresPower;
    public ParticleData Data;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.ParticleEmitter;
      }
    }

    public ParticleEmitterBlock()
    {
      this.RequiresPower = false;
      this.SetValuesFromTemplate(0);
    }

    public ParticleEmitterBlock(GlobalPoint3D p)
      : base(p)
    {
      this.RequiresPower = false;
      this.SetValuesFromTemplate(0);
    }

    public ParticleEmitterBlock(int templateID)
    {
      this.RequiresPower = false;
      this.SetValuesFromTemplate(templateID);
    }

    public void SetValuesFromTemplate(int templateID)
    {
      Globals2.SetParticleDataFromTemplate(templateID, ref this.Data);
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      ParticleEmitterBlock particleEmitterBlock = from as ParticleEmitterBlock;
      this.RequiresPower = particleEmitterBlock.RequiresPower;
      this.Data.CopyFrom(ref particleEmitterBlock.Data);
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.RequiresPower = reader.ReadBoolean();
      this.Data.ReadState(reader, version);
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.RequiresPower);
      this.Data.WriteState(writer);
    }
  }
}
