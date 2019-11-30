// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.LoadNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Load", BehaviourTreeNodeType.Logic)]
  internal class LoadNode : BehaviourTreeNode
  {
    public string Behaviour;
    public string Dialog;

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      INPCBehaviour target = this.GetTarget(engine.Tree.TreeType, BehaviourTreeNodeCompareTarget.Self);
      if (this.Behaviour.IsNotEmpty())
        target.LoadBehaviour(BehaviourTreeType.AI, this.Behaviour);
      if (this.Dialog.IsNotEmpty())
        target.LoadBehaviour(BehaviourTreeType.Dialog, this.Dialog);
      this.Status = BehaviourTreeNodeStatus.Success;
    }

    public override bool IsPropertyEnabled(string propertyName)
    {
      switch (propertyName)
      {
        case "Continue":
          return false;
        default:
          return base.IsPropertyEnabled(propertyName);
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Behaviour = reader.ReadString();
      this.Dialog = reader.ReadString();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Behaviour != null ? this.Behaviour : "");
      writer.Write(this.Dialog != null ? this.Dialog : "");
    }
  }
}
