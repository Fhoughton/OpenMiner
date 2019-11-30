// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.SwingHandNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("SwingHand", BehaviourTreeNodeType.Action)]
  internal class SwingHandNode : BehaviourTreeNode
  {
    public InventoryHand Hand;

    public override string ToStringParms
    {
      get
      {
        return this.Hand.ToString();
      }
    }

    public SwingHandNode()
    {
    }

    public SwingHandNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        this.npc.SwingHand(this.Hand);
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Hand = (InventoryHand) reader.ReadByte();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.Hand);
    }
  }
}
