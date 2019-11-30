// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsRandomNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsRandom", BehaviourTreeNodeType.Conditional)]
  internal class IsRandomNode : BehaviourTreeNode
  {
    public int Chances;
    public int OutOf;

    public override string ToStringParms
    {
      get
      {
        return string.Format("{0}/{1}", (object) this.Chances, (object) this.OutOf);
      }
    }

    public IsRandomNode()
    {
      this.Chances = 1;
      this.OutOf = 2;
    }

    public IsRandomNode(INPCBehaviour npc)
      : base(npc)
    {
      this.Chances = 1;
      this.OutOf = 2;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
        this.Status = BehaviourTreeNodeStatus.Failure;
      else
        this.Status = this.Chances <= 0 || this.OutOf <= 0 || this.npc.Random.Next(this.OutOf) >= this.Chances ? BehaviourTreeNodeStatus.Failure : BehaviourTreeNodeStatus.Success;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Chances = reader.ReadInt32();
      this.OutOf = reader.ReadInt32();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Chances);
      writer.Write(this.OutOf);
    }
  }
}
