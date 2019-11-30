// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.HasInventoryNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("HasInventory", BehaviourTreeNodeType.Conditional)]
  internal class HasInventoryNode : ComparisonNodeInt
  {
    public Item Item;

    public override string ToStringParms
    {
      get
      {
        return this.CutString(this.Item.ToString(), 8) + " " + base.ToStringParms;
      }
    }

    public HasInventoryNode()
    {
    }

    public HasInventoryNode(INPCBehaviour npc)
    {
      this.npc = npc;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null || engine.Tree == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        INPCBehaviour target = this.GetTarget(engine.Tree.TreeType, this.CompareTarget);
        if (target == null)
          this.Status = BehaviourTreeNodeStatus.Failure;
        else
          this.Status = this.Compare(target.Inventory.ItemCount(this.Item), this.Value) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Item = (Item) reader.ReadUInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((ushort) this.Item);
    }
  }
}
