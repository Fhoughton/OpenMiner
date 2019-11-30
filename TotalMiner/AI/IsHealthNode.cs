// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsHealthNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GUI;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsHealth", BehaviourTreeNodeType.Conditional)]
  internal class IsHealthNode : ComparisonNodeSingle
  {
    public CompareMaxOrCurrent MaxOrCurrent;

    public IsHealthNode()
    {
    }

    public IsHealthNode(INPCBehaviour npc)
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
          this.Status = this.Compare(this.MaxOrCurrent == CompareMaxOrCurrent.Max ? target.MaxHealth : target.Health, this.IsPercent ? (float) ((double) target.MaxHealth * (double) this.Value / 100.0) : this.Value) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.MaxOrCurrent = CompareMaxOrCurrent.Current;
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "MaxOrCurrent":
          win.SetToolTip("This is typically set to Current. If you are testing for a % of health, this must be set to Current");
          break;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.MaxOrCurrent = (CompareMaxOrCurrent) reader.ReadByte();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.MaxOrCurrent);
    }
  }
}
