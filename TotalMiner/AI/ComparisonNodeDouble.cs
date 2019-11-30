// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ComparisonNodeDouble
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public abstract class ComparisonNodeDouble : ComparisonNode<double>
  {
    protected ComparisonNodeDouble()
    {
    }

    protected ComparisonNodeDouble(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override bool Compare(double actual, double value)
    {
      switch (this.CompareType)
      {
        case Parser.CompareState.Binary:
          return actual != value;
        case Parser.CompareState.Equal:
          return actual == value;
        case Parser.CompareState.NotEqual:
          return actual != value;
        case Parser.CompareState.LessThan:
          return actual < value;
        case Parser.CompareState.LessThanOrEqual:
          return actual <= value;
        case Parser.CompareState.GreaterThan:
          return actual > value;
        case Parser.CompareState.GreaterThanOrEqual:
          return actual >= value;
        case Parser.CompareState.Modulus:
          return actual % value == 0.0;
        default:
          return actual != 0.0;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Value = reader.ReadDouble();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Value);
    }
  }
}
