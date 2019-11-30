// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.IsAudibleNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GUI;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("IsAudible", BehaviourTreeNodeType.Conditional)]
  internal class IsAudibleNode : IsNpcTypeQueryNode
  {
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.IsCSV)]
    public List<SoundType> SoundTypes = new List<SoundType>();
    public float Distance;

    public IsAudibleNode()
    {
    }

    public IsAudibleNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      this.Status = BehaviourTreeNodeStatus.Failure;
      if (this.npc == null)
        return;
      NpcQueryPreference preference = this.Preference | NpcQueryPreference.Visible;
      if (this.CompareTarget != BehaviourTreeNodeCompareTarget.Self)
        return;
      this.Status = this.npc.FindSound(preference, this.Distance, this.SoundTypes, this.SearchTypes, this.ExcludeTypes).HasValue ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.Preference &= NpcQueryPreference.Closest;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Distance = reader.ReadSingle();
      this.SoundTypes.Clear();
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
        this.SoundTypes.Add((SoundType) reader.ReadUInt16());
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Distance);
      writer.Write(this.SoundTypes.Count);
      foreach (SoundType soundType in this.SoundTypes)
        writer.Write((ushort) soundType);
    }
  }
}
