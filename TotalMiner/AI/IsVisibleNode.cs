// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsVisibleNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsVisible", BehaviourTreeNodeType.Conditional)]
  internal class IsVisibleNode : IsNpcTypeQueryNode
  {
    public float RefindTime = 5f;
    public float Distance;

    public IsVisibleNode()
    {
    }

    public IsVisibleNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      this.Status = BehaviourTreeNodeStatus.Failure;
      if (this.npc == null)
        return;
      NpcQueryPreference preference = this.Preference | NpcQueryPreference.Visible;
      float distance = (double) this.Distance != 0.0 ? this.Distance : (float) this.npc.AIData.RegardRange;
      if (this.CompareTarget == BehaviourTreeNodeCompareTarget.Self)
      {
        INPCBehaviour actor = this.npc.FindActor(preference, distance, this.SearchTypes, this.ExcludeTypes);
        if (actor != null)
          TargetingSystem.Target(actor, this.npc, (int) ((double) this.RefindTime * 60.0), NpcQueryPreference.Source | NpcQueryPreference.Visible);
        this.Status = actor != null ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        if (this.CompareTarget != BehaviourTreeNodeCompareTarget.Target || this.npc.AITarget == null)
          return;
        INPCBehaviour actor = this.npc.AITarget.FindActor(preference, distance, this.SearchTypes, this.ExcludeTypes);
        this.Status = actor == null || actor.AITarget != this.npc ? BehaviourTreeNodeStatus.Failure : BehaviourTreeNodeStatus.Success;
      }
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.Preference &= NpcQueryPreference.Visible | NpcQueryPreference.Closest;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.RefindTime = version <= 262 ? 5f : reader.ReadSingle();
      if (version > 282)
        this.Distance = reader.ReadSingle();
      else
        this.Distance = 0.0f;
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.RefindTime);
      writer.Write(this.Distance);
    }
  }
}
