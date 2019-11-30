// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsDistanceNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsDistance", BehaviourTreeNodeType.Conditional)]
  internal class IsDistanceNode : ComparisonNodeSingle
  {
    public Vector3 Position;
    public CoordType PositionType;

    public override string ToStringParms
    {
      get
      {
        return this.ShortPosType(this.PositionType) + " " + base.ToStringParms;
      }
    }

    public IsDistanceNode()
    {
      this.CompareType = Parser.CompareState.LessThanOrEqual;
    }

    public IsDistanceNode(INPCBehaviour npc)
      : base(npc)
    {
      this.CompareType = Parser.CompareState.LessThanOrEqual;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null || engine.Tree == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        INPCBehaviour target = this.GetTarget(engine.Tree.TreeType, this.CompareTarget, NpcQueryPreference.Source);
        if (target == null)
          this.Status = BehaviourTreeNodeStatus.Failure;
        else
          this.Status = this.Compare(Vector3.DistanceSquared(this.npc.GetFinalPosition(this.PositionType, this.Position), target.Position), this.Value * this.Value) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.PositionType = (CoordType) reader.ReadByte();
      this.Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.PositionType);
      writer.Write(this.Position.X);
      writer.Write(this.Position.Y);
      writer.Write(this.Position.Z);
    }
  }
}
