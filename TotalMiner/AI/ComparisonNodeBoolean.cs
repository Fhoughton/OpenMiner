// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ComparisonNodeBoolean
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public abstract class ComparisonNodeBoolean : ComparisonNode<bool>
  {
    protected ComparisonNodeBoolean()
    {
    }

    protected ComparisonNodeBoolean(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override bool Compare(bool actual, bool value)
    {
      switch (this.CompareType)
      {
        case Parser.CompareState.Binary:
          return actual != value;
        case Parser.CompareState.Equal:
          return actual == value;
        case Parser.CompareState.NotEqual:
          return actual != value;
        default:
          return actual;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Value = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Value);
    }
  }
}
