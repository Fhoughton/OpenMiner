// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.PropertiesNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Properties", BehaviourTreeNodeType.Action)]
  internal class PropertiesNode : BehaviourTreeNode
  {
    public NpcProperties Properties = new NpcProperties();

    public override object ForPropertyEditor
    {
      get
      {
        return (object) this.Properties;
      }
    }

    public PropertiesNode()
    {
      this.Continue = true;
    }

    public PropertiesNode(INPCBehaviour npc)
      : base(npc)
    {
      this.Continue = true;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        this.npc.SetProperties(this.Properties);
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Properties.ReadState(reader, version);
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      this.Properties.WriteState(writer);
    }
  }
}
