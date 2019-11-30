// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsMiningNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsMining", BehaviourTreeNodeType.Conditional)]
  internal class IsMiningNode : BehaviourTreeNode
  {
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.IsCSV)]
    public List<Block> BlockIDs = new List<Block>();
    public BehaviourTreeNodeCompareTarget CompareTarget;

    public override string ToStringParms
    {
      get
      {
        return this.BlockIDs.ToString();
      }
    }

    public IsMiningNode()
    {
    }

    public IsMiningNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      this.Status = BehaviourTreeNodeStatus.Failure;
      if (this.npc == null || engine.Tree == null)
        return;
      INPCBehaviour target = this.GetTarget(engine.Tree.TreeType, this.CompareTarget);
      if (target == null || this.BlockIDs.Count < 1 || !target.SwingTargetIsValid || !target.LeftHand.IsSwinging && !target.RightHand.IsSwinging)
        return;
      Block blockId = this.npc.Map.GetBlockID(target.SwingTarget);
      if (blockId == Block.None || !this.BlockIDs.Contains(blockId))
        return;
      this.Status = BehaviourTreeNodeStatus.Success;
    }

    protected override object ValidateCore(
      string propertyName,
      string input,
      out string adjustedInput)
    {
      if (propertyName == "BlockIDs")
        return (object) Utils.ValidateTypeList<Block>(input, out adjustedInput);
      return base.ValidateCore(propertyName, input, out adjustedInput);
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.CompareTarget = (BehaviourTreeNodeCompareTarget) reader.ReadByte();
      this.ReadBlockList(reader, this.BlockIDs);
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.CompareTarget);
      this.WriteBlockList(writer, this.BlockIDs);
    }
  }
}
