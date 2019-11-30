// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.MetaExecuteScript
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.Net
{
  internal class MetaExecuteScript : MetaExecuteBase
  {
    public string ScriptName;
    public int LineNo;
    public GlobalPoint3D? ScriptOffset;
    public GlobalPoint3D? BlockOffset;

    public override MetaExecuteType Type
    {
      get
      {
        return MetaExecuteType.Script;
      }
    }

    public override void ReadState(BinaryReader reader)
    {
      this.ScriptName = reader.ReadString();
      this.LineNo = reader.ReadInt32();
    }

    public override void WriteState(BinaryWriter writer)
    {
      writer.Write(this.ScriptName);
      writer.Write(this.LineNo);
    }
  }
}
