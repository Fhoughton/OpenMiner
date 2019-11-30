// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.ProximityDetectorBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Storage;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class ProximityDetectorBlock : PlayerBlock
  {
    public byte Range;
    public BlockTargetTypes TargetTypes;
    public string OnEntryScriptName;
    public string OnExitScriptName;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.ProximityDetector;
      }
    }

    public ProximityDetectorBlock()
    {
    }

    public ProximityDetectorBlock(GlobalPoint3D p)
      : base(p)
    {
      this.Point = p;
      this.Range = (byte) 3;
      this.TargetTypes = BlockTargetTypes.None;
    }

    public ProximityDetectorBlock(GlobalPoint3D p, Player player)
      : base(p, player)
    {
      this.Point = p;
      this.Range = (byte) 3;
      this.TargetTypes = BlockTargetTypes.None;
    }

    public override void SetScript(string name, DataBlockScriptType type)
    {
      switch (type)
      {
        case DataBlockScriptType.None:
        case DataBlockScriptType.Entry:
          this.OnEntryScriptName = name;
          break;
        case DataBlockScriptType.Exit:
          this.OnExitScriptName = name;
          break;
      }
    }

    public override void RenameScript(string oldName, string newName)
    {
      if (this.OnEntryScriptName == oldName)
        this.OnEntryScriptName = newName;
      if (!(this.OnExitScriptName == oldName))
        return;
      this.OnExitScriptName = newName;
    }

    public bool IsActive
    {
      get
      {
        return this.TargetTypes != BlockTargetTypes.None;
      }
    }

    public bool IsTargeting(BlockTargetTypes target)
    {
      return (this.TargetTypes & target) == target;
    }

    public void ToggleTargetType(BlockTargetTypes target)
    {
      if (this.IsTargeting(target))
        this.TargetTypes &= ~target;
      else
        this.TargetTypes |= target;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      ProximityDetectorBlock proximityDetectorBlock = from as ProximityDetectorBlock;
      this.Range = proximityDetectorBlock.Range;
      this.TargetTypes = proximityDetectorBlock.TargetTypes;
      this.OnEntryScriptName = proximityDetectorBlock.OnEntryScriptName;
      this.OnExitScriptName = proximityDetectorBlock.OnExitScriptName;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (version > 173)
      {
        this.Range = reader.ReadByte();
      }
      else
      {
        int num1 = (int) reader.ReadByte();
        int num2 = (int) reader.ReadByte();
        this.Range = reader.ReadByte();
        reader.ReadBoolean();
      }
      this.TargetTypes = (BlockTargetTypes) reader.ReadByte();
      if (version <= 229)
        return;
      this.OnEntryScriptName = reader.ReadString();
      this.OnExitScriptName = reader.ReadString();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Range);
      writer.Write((byte) this.TargetTypes);
      writer.Write(this.OnEntryScriptName != null ? this.OnEntryScriptName : "");
      writer.Write(this.OnExitScriptName != null ? this.OnExitScriptName : "");
    }

    public void LoadFromSaveData(SaveMineBlockState state)
    {
      this.LoadFromSaveData((SavePlayerBlockState) state);
      this.Point = state.Point;
      this.Range = state.TriggerRadius;
      this.TargetTypes = state.TargetTypes;
    }
  }
}
