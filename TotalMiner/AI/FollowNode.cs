// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.FollowNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Follow", BehaviourTreeNodeType.Action)]
  internal class FollowNode : MoveNode
  {
    public FollowNode()
    {
    }

    public FollowNode(INPCBehaviour npc)
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
        this.npc.LookAt(this.PositionType, this.Position, false);
        base.UpdateCore(engine);
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.Distance = 2f;
    }
  }
}
