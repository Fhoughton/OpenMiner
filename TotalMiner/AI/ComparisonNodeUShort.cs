// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ComparisonNodeUShort
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public abstract class ComparisonNodeUShort : ComparisonNode<ushort>
  {
    protected ComparisonNodeUShort()
    {
    }

    protected ComparisonNodeUShort(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override bool Compare(ushort actual, ushort value)
    {
      switch (this.CompareType)
      {
        case Parser.CompareState.Binary:
          return (int) actual != (int) value;
        case Parser.CompareState.Equal:
          return (int) actual == (int) value;
        case Parser.CompareState.NotEqual:
          return (int) actual != (int) value;
        case Parser.CompareState.LessThan:
          return (int) actual < (int) value;
        case Parser.CompareState.LessThanOrEqual:
          return (int) actual <= (int) value;
        case Parser.CompareState.GreaterThan:
          return (int) actual > (int) value;
        case Parser.CompareState.GreaterThanOrEqual:
          return (int) actual >= (int) value;
        case Parser.CompareState.Modulus:
          return (int) actual % (int) value == 0;
        default:
          return actual != (ushort) 0;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Value = reader.ReadUInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Value);
    }
  }
}
