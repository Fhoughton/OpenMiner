// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ComparisonNode`1
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.GUI;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public abstract class ComparisonNode<T> : BehaviourTreeNode
  {
    public Parser.CompareState CompareType;
    public BehaviourTreeNodeCompareTarget CompareTarget;
    public T Value;
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.None)]
    public bool IsPercent;

    public override string ToStringParms
    {
      get
      {
        return string.Format("{0}{1}{2}", (object) Parser.GetShortCompareString(this.CompareType), (object) this.Value, this.IsPercent ? (object) "%" : (object) "");
      }
    }

    protected ComparisonNode()
    {
    }

    protected ComparisonNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected abstract bool Compare(T actual, T value);

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
    }

    protected override string ToStringCore(string propertyName, object data)
    {
      if (propertyName == "Value" && this.IsPercent)
        return string.Format("{0}%", data);
      return base.ToStringCore(propertyName, data);
    }

    protected override object ValidateCore(
      string propertyName,
      string input,
      out string adjustedInput)
    {
      if (propertyName == "Value")
      {
        this.IsPercent = input.Trim().EndsWith("%");
        if (this.IsPercent)
        {
          adjustedInput = input.Substring(0, input.Length - 1);
          return (object) null;
        }
        this.IsPercent = input.Trim().StartsWith("%");
        if (this.IsPercent)
        {
          adjustedInput = input.Substring(1);
          return (object) null;
        }
      }
      return base.ValidateCore(propertyName, input, out adjustedInput);
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.CompareType = (Parser.CompareState) reader.ReadByte();
      this.CompareTarget = (BehaviourTreeNodeCompareTarget) reader.ReadByte();
      this.IsPercent = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.CompareType);
      writer.Write((byte) this.CompareTarget);
      writer.Write(this.IsPercent);
    }
  }
}
