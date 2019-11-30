// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.WaitNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GUI;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Wait", BehaviourTreeNodeType.Action)]
  internal class WaitNode : TimerNode
  {
    public WaitNode()
    {
    }

    public WaitNode(INPCBehaviour npc)
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
        base.UpdateCore(engine);
        if (this.Status != BehaviourTreeNodeStatus.Failure)
          return;
        this.Status = BehaviourTreeNodeStatus.Running;
      }
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "MilliSeconds":
          win.SetToolTip("Execution of the behaviour tree will halt at this node until this amount of time in milli seconds has passed. The timer is then reset");
          break;
      }
    }
  }
}
