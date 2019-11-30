// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.HealthNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Health", BehaviourTreeNodeType.Action)]
  internal class HealthNode : BehaviourTreeNode
  {
    public float Points;
    public BehaviourTreeNodeCompareTarget Target;

    public override string ToStringParms
    {
      get
      {
        return this.Target.ToString() + " " + this.Points.ToString();
      }
    }

    public HealthNode()
    {
    }

    public HealthNode(INPCBehaviour npc)
    {
      this.npc = npc;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        INPCBehaviour target = this.GetTarget(engine.Tree.TreeType, this.Target);
        if (target == null)
        {
          this.Status = BehaviourTreeNodeStatus.Failure;
        }
        else
        {
          target.Health += this.Points;
          this.Status = BehaviourTreeNodeStatus.Success;
        }
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Points = reader.ReadSingle();
      this.Target = (BehaviourTreeNodeCompareTarget) reader.ReadByte();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Points);
      writer.Write((byte) this.Target);
    }
  }
}
