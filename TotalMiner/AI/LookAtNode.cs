// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.LookAtNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("LookAt", BehaviourTreeNodeType.Action)]
  internal class LookAtNode : BehaviourTreeNode
  {
    public Vector3 Position;
    public CoordType PositionType;
    public bool Instant;

    public override string ToStringParms
    {
      get
      {
        return this.ShortPosType(this.PositionType);
      }
    }

    public LookAtNode()
    {
    }

    public LookAtNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
        this.Status = BehaviourTreeNodeStatus.Failure;
      else
        this.Status = this.npc.LookAt(this.PositionType, this.Position, this.Instant) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Position.X = reader.ReadSingle();
      this.Position.Y = reader.ReadSingle();
      this.Position.Z = reader.ReadSingle();
      this.PositionType = (CoordType) reader.ReadByte();
      this.Instant = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Position.X);
      writer.Write(this.Position.Y);
      writer.Write(this.Position.Z);
      writer.Write((byte) this.PositionType);
      writer.Write(this.Instant);
    }
  }
}
