// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.UpdateNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Update", BehaviourTreeNodeType.Logic)]
  internal class UpdateNode : BehaviourTreeNode
  {
    public override bool CanExecute
    {
      get
      {
        return true;
      }
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        if (this.npc.IsAlive && !this.IsLeafNode)
          engine.AddNode((BehaviourTreeNode) this.FirstChild);
        this.Status = BehaviourTreeNodeStatus.Running;
      }
    }

    public override bool IsPropertyEnabled(string propertyName)
    {
      switch (propertyName)
      {
        case "Continue":
          return false;
        default:
          return base.IsPropertyEnabled(propertyName);
      }
    }
  }
}
