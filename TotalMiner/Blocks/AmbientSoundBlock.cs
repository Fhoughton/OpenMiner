// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.AmbientSoundBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Storage;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class AmbientSoundBlock : DataBlock
  {
    public const int MaxRange = 100;
    public int SoundID;
    public float Volume;
    public ushort Distance;
    public byte LoopDelayIndex;
    public DayOrNight DayOrNight;
    public bool RequiresPower;
    public bool DisplayNotPlayingMessage;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.AmbientSound;
      }
    }

    public AmbientSoundBlock()
    {
    }

    public AmbientSoundBlock(GlobalPoint3D p)
      : base(p)
    {
      this.Volume = 1f;
      this.Distance = (ushort) 20;
      this.LoopDelayIndex = (byte) 0;
      this.DayOrNight = DayOrNight.None;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      AmbientSoundBlock ambientSoundBlock = from as AmbientSoundBlock;
      this.Volume = ambientSoundBlock.Volume;
      this.Distance = ambientSoundBlock.Distance;
      this.LoopDelayIndex = ambientSoundBlock.LoopDelayIndex;
      this.DayOrNight = ambientSoundBlock.DayOrNight;
      this.RequiresPower = ambientSoundBlock.RequiresPower;
      this.DisplayNotPlayingMessage = ambientSoundBlock.DisplayNotPlayingMessage;
      this.SoundID = ambientSoundBlock.SoundID;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.SoundID = reader.ReadInt32();
      this.Volume = reader.ReadSingle();
      this.Distance = reader.ReadUInt16();
      this.LoopDelayIndex = reader.ReadByte();
      this.DayOrNight = (DayOrNight) reader.ReadByte();
      this.RequiresPower = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.SoundID);
      writer.Write(this.Volume);
      writer.Write(this.Distance);
      writer.Write(this.LoopDelayIndex);
      writer.Write((byte) this.DayOrNight);
      writer.Write(this.RequiresPower);
    }

    public void LoadFromSaveData(SaveAmbientSoundState state)
    {
      this.SoundID = (int) state.SoundID;
      this.Volume = state.Volume;
      this.Distance = state.Distance;
      this.LoopDelayIndex = state.LoopDelay;
      this.DayOrNight = state.DayOrNight;
      this.RequiresPower = state.RequiresPower;
      this.Point = state.Point;
    }
  }
}
