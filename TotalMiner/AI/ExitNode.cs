// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ExitNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Exit", BehaviourTreeNodeType.Logic)]
  internal class ExitNode : BehaviourTreeNode
  {
    public override bool CanExecute
    {
      get
      {
        return false;
      }
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
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
