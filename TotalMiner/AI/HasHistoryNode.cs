// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.HasHistoryNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("HasHistory", BehaviourTreeNodeType.Conditional)]
  internal class HasHistoryNode : ComparisonNodeLong
  {
    public string HistoryName;
    public ScriptTarget HistoryType;

    public override string ToStringParms
    {
      get
      {
        return this.CutString(this.HistoryName, 8) + " " + base.ToStringParms;
      }
    }

    public HasHistoryNode()
    {
      this.CompareTarget = BehaviourTreeNodeCompareTarget.Target;
    }

    public HasHistoryNode(INPCBehaviour npc)
    {
      this.npc = npc;
      this.CompareTarget = BehaviourTreeNodeCompareTarget.Target;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
        this.Status = BehaviourTreeNodeStatus.Failure;
      else if (this.HistoryName.IsEmpty())
        this.Status = BehaviourTreeNodeStatus.Failure;
      else if (this.CompareTarget != BehaviourTreeNodeCompareTarget.Target)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        long history1;
        if (this.HistoryType == ScriptTarget.System)
        {
          history1 = GameInstance.Instance.History.GetHistory(this.HistoryName);
        }
        else
        {
          if (engine.Tree == null)
          {
            this.Status = BehaviourTreeNodeStatus.Failure;
            return;
          }
          Player target = this.GetTarget(engine.Tree.TreeType, this.CompareTarget) as Player;
          if (target == null)
          {
            this.Status = BehaviourTreeNodeStatus.Failure;
            return;
          }
          History history2 = this.HistoryType == ScriptTarget.Actor ? target.History : GameInstance.Instance.GetClanHistory(target.ClanName);
          if (history2 == null)
          {
            this.Status = BehaviourTreeNodeStatus.Failure;
            return;
          }
          history1 = history2.GetHistory(this.HistoryName.ToLower());
        }
        this.Status = this.Compare(history1, this.Value) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.HistoryType = (ScriptTarget) reader.ReadByte();
      this.HistoryName = reader.ReadString();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.HistoryType);
      writer.Write(this.HistoryName.ToLower());
    }
  }
}
