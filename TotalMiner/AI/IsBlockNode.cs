// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsBlockNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsBlock", BehaviourTreeNodeType.Conditional)]
  internal class IsBlockNode : BehaviourTreeNode
  {
    public Block BlockID;
    public Vector3 Position;
    public CoordType PositionType;

    public override string ToStringParms
    {
      get
      {
        return this.BlockID.ToString();
      }
    }

    public IsBlockNode()
    {
    }

    public IsBlockNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
        this.Status = BehaviourTreeNodeStatus.Failure;
      else if (this.PositionType == CoordType.None && this.npc.SwingFace == BlockFace.ProxyDefault)
        this.Status = this.BlockID == Block.None ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      else
        this.Status = this.npc.Map.GetBlockID(this.npc.Map.GetPoint(this.npc.GetFinalPosition(this.PositionType, this.Position))) == this.BlockID ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.BlockID = (Block) reader.ReadByte();
      this.Position.X = reader.ReadSingle();
      this.Position.Y = reader.ReadSingle();
      this.Position.Z = reader.ReadSingle();
      this.PositionType = (CoordType) reader.ReadByte();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.BlockID);
      writer.Write(this.Position.X);
      writer.Write(this.Position.Y);
      writer.Write(this.Position.Z);
      writer.Write((byte) this.PositionType);
    }
  }
}
