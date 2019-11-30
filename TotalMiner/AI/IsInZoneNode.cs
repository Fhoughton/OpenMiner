// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsInZoneNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsInZone", BehaviourTreeNodeType.Conditional)]
  internal class IsInZoneNode : BehaviourTreeNode
  {
    public string Zone;
    public ZoneType ZoneType;

    public override string ToStringParms
    {
      get
      {
        return this.CutString(this.ZoneType.ToString(), 8);
      }
    }

    public IsInZoneNode()
    {
    }

    public IsInZoneNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      this.Status = BehaviourTreeNodeStatus.Failure;
      if (this.npc == null)
        return;
      if (this.ZoneType != ZoneType.None && this.npc.IsInZone(this.ZoneType))
        this.Status = BehaviourTreeNodeStatus.Success;
      if (!this.Zone.IsNotEmpty())
        return;
      this.Status = this.npc.IsInZone(this.Zone) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.ZoneType = (ZoneType) reader.ReadUInt16();
      if (version <= 266)
        return;
      this.Zone = reader.ReadString();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((ushort) this.ZoneType);
      writer.Write(this.Zone != null ? this.Zone : "");
    }
  }
}
