// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsAgeNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsAge", BehaviourTreeNodeType.Conditional)]
  internal class IsAgeNode : ComparisonNodeSingle
  {
    public IsAgeNode()
    {
    }

    public IsAgeNode(INPCBehaviour npc)
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
          this.Status = this.Compare(target.Age, this.Value) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
    }
  }
}
