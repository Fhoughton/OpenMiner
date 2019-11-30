// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ProxyNode
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Core;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Proxy", BehaviourTreeNodeType.Logic)]
  public class ProxyNode : BehaviourTreeNode
  {
    public string Tree;

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      bool flag = false;
      if (this.Tree.IsNotEmpty())
      {
        BehaviourTree behaviour = Globals1.GetBehaviour(engine.Tree.TreeType, this.Tree);
        if (behaviour != null)
        {
          this.ReplaceWith(behaviour.Clone(this.npc).Root);
          flag = true;
        }
      }
      if (!flag && this.Parent != null)
        this.Parent.RemoveChild((Node) this);
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
      this.Tree = reader.ReadString();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Tree != null ? this.Tree : "");
    }
  }
}
