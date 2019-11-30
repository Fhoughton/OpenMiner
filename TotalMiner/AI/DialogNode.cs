// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.DialogNode
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.API;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Dialog", BehaviourTreeNodeType.Dialog)]
  public class DialogNode : BehaviourTreeNode
  {
    public bool StopMoving = true;
    public string Text;
    public bool DisableOnceRead;
    public bool DisableBackButton;
    public float TargetTime;
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.None)]
    public bool IsRead;

    public override string ToStringParms
    {
      get
      {
        return this.CutString(this.Text, 8);
      }
    }

    public bool MustGoBack
    {
      get
      {
        for (Node parent = this.parent; parent != null; parent = parent.Parent)
        {
          DialogNode dialogNode = parent as DialogNode;
          if (dialogNode != null && dialogNode.DisableBackButton && dialogNode.DisableOnceRead)
            return true;
        }
        return false;
      }
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        if (this.StopMoving)
          this.npc.StandStill();
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    public void UpdateFromHandler(ITMPlayer player)
    {
      this.Update((ITMBehaviourExecutionEngine) null);
      if (this.Status != BehaviourTreeNodeStatus.Success || (double) this.TargetTime <= 0.0 || player == null)
        return;
      TargetingSystem.Target(this.npc, player as INPCBehaviour, (int) ((double) this.TargetTime * 60.0), NpcQueryPreference.Source);
    }

    public override bool IsPropertyEnabled(string propertyName)
    {
      switch (propertyName)
      {
        case "DisableOnceRead":
        case "DisableBackButton":
          return DialogNode.GetBranchDepth((Node) this) % 2 == 1;
        case "StopMoving":
          return DialogNode.GetBranchDepth((Node) this) % 2 == 1;
        case "Continue":
          return false;
        default:
          return base.IsPropertyEnabled(propertyName);
      }
    }

    public static int GetBranchDepth(Node node)
    {
      int num = 0;
      while (node != null)
      {
        node = node.Parent;
        if (node != null && node is DialogNode)
          ++num;
      }
      return num;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Text = reader.ReadString();
      this.DisableOnceRead = reader.ReadBoolean();
      this.DisableBackButton = reader.ReadBoolean();
      this.StopMoving = reader.ReadBoolean();
      if (version <= 263)
        return;
      this.TargetTime = reader.ReadSingle();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Text != null ? this.Text : "");
      writer.Write(this.DisableOnceRead);
      writer.Write(this.DisableBackButton);
      writer.Write(this.StopMoving);
      writer.Write(this.TargetTime);
    }
  }
}
