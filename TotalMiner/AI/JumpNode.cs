// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.JumpNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Jump", BehaviourTreeNodeType.Action)]
  internal class JumpNode : BehaviourTreeNode
  {
    public float Height;

    public override string ToStringParms
    {
      get
      {
        return this.Height.ToString();
      }
    }

    public JumpNode()
    {
    }

    public JumpNode(INPCBehaviour npc)
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
        this.npc.Jump(this.Height);
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Height = reader.ReadSingle();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Height);
    }
  }
}
