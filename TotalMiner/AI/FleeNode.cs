// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.FleeNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Flee", BehaviourTreeNodeType.Action)]
  internal class FleeNode : MoveNode
  {
    public FleeNode()
    {
      this.Distance = 10f;
    }

    public FleeNode(INPCBehaviour npc)
      : base(npc)
    {
      this.Distance = 10f;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
        this.Status = BehaviourTreeNodeStatus.Failure;
      else if (this.npc.AITarget == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        this.Position = this.npc.Position + Vector3.Normalize(this.npc.Position - this.npc.AITarget.Position) * this.Distance;
        this.moveType = this.npc.Properties.MoveType.HasValue ? this.npc.Properties.MoveType.Value : MoveType.Default;
        float distance = this.Distance;
        this.Distance = 0.1f;
        base.UpdateCore(engine);
        this.Distance = distance;
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }
  }
}
