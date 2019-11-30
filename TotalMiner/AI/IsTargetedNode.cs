// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsTargetedNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.Collections.Generic;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsTargeted", BehaviourTreeNodeType.Conditional)]
  internal class IsTargetedNode : IsNpcTypeQueryNode
  {
    public IsTargetedNode()
    {
    }

    public IsTargetedNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      this.Status = BehaviourTreeNodeStatus.Failure;
      if (this.npc == null)
        return;
      INPCBehaviour target = this.CompareTarget == BehaviourTreeNodeCompareTarget.Self ? this.npc : this.npc.AITarget;
      if (target == null)
        return;
      List<TargetData> targetedBy = TargetingSystem.GetTargetedBy(target, this.Preference, this.SearchTypes, this.ExcludeTypes);
      if (targetedBy == null || targetedBy.Count <= 0)
        return;
      this.Status = BehaviourTreeNodeStatus.Success;
    }
  }
}
