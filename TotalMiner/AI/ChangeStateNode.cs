// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ChangeStateNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GUI;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("StateChange", BehaviourTreeNodeType.Action)]
  internal class ChangeStateNode : BehaviourTreeNode
  {
    [PropertyEditorField("State")]
    public ActorState NewState;

    public override string ToStringParms
    {
      get
      {
        return this.NewState.ToString();
      }
    }

    public ChangeStateNode()
    {
    }

    public ChangeStateNode(INPCBehaviour npc)
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
        this.npc.ChangeState(this.NewState);
        this.Status = this.npc.ActorState == this.NewState ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.NewState = (ActorState) reader.ReadByte();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.NewState);
    }
  }
}
