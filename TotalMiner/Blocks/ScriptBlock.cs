// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.ScriptBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class ScriptBlock : DataBlock
  {
    public string PowerOnScript;
    public string PowerOffScript;
    public float PlayerLookRange;
    public bool ActAsPressurePlate;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Script;
      }
    }

    public ScriptBlock()
    {
    }

    public ScriptBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public override void SetScript(string name, DataBlockScriptType type)
    {
      switch (type)
      {
        case DataBlockScriptType.None:
        case DataBlockScriptType.PowerOn:
          this.PowerOnScript = name;
          break;
        case DataBlockScriptType.PowerOff:
          this.PowerOffScript = name;
          break;
      }
    }

    public override void RenameScript(string oldName, string newName)
    {
      if (this.PowerOnScript == oldName)
        this.PowerOnScript = newName;
      if (!(this.PowerOffScript == oldName))
        return;
      this.PowerOffScript = newName;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      ScriptBlock scriptBlock = from as ScriptBlock;
      this.PowerOnScript = scriptBlock.PowerOnScript;
      this.PowerOffScript = scriptBlock.PowerOffScript;
      this.PlayerLookRange = scriptBlock.PlayerLookRange;
      this.ActAsPressurePlate = scriptBlock.ActAsPressurePlate;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.PowerOnScript = reader.ReadString();
      this.PowerOffScript = reader.ReadString();
      if (version <= 141)
        return;
      this.PlayerLookRange = reader.ReadSingle();
      this.ActAsPressurePlate = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.PowerOnScript != null ? this.PowerOnScript : "");
      writer.Write(this.PowerOffScript != null ? this.PowerOffScript : "");
      writer.Write(this.PlayerLookRange);
      writer.Write(this.ActAsPressurePlate);
    }
  }
}
