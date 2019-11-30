// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ComparisonNodeSingle
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public abstract class ComparisonNodeSingle : ComparisonNode<float>
  {
    protected ComparisonNodeSingle()
    {
    }

    protected ComparisonNodeSingle(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override bool Compare(float actual, float value)
    {
      switch (this.CompareType)
      {
        case Parser.CompareState.Binary:
          return (double) actual != (double) value;
        case Parser.CompareState.Equal:
          return (double) actual == (double) value;
        case Parser.CompareState.NotEqual:
          return (double) actual != (double) value;
        case Parser.CompareState.LessThan:
          return (double) actual < (double) value;
        case Parser.CompareState.LessThanOrEqual:
          return (double) actual <= (double) value;
        case Parser.CompareState.GreaterThan:
          return (double) actual > (double) value;
        case Parser.CompareState.GreaterThanOrEqual:
          return (double) actual >= (double) value;
        case Parser.CompareState.Modulus:
          return (double) actual % (double) value == 0.0;
        default:
          return (double) actual != 0.0;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Value = reader.ReadSingle();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Value);
    }
  }
}
