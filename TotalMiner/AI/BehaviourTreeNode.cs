// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.BehaviourTreeNode
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StudioForge.TotalMiner.AI
{
  public abstract class BehaviourTreeNode : StudioForge.Engine.Core.Node, IPropertyEditorControl
  {
    public bool IsEnabled = true;
    public bool Continue;
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.None)]
    public BehaviourTreeNodeStatus Status;
    [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.None)]
    protected INPCBehaviour npc;

    public event EventHandler Validated;

    public void Raise_Validated()
    {
      if (this.Validated == null)
        return;
      this.Validated((object) this, EventArgs.Empty);
    }

    public INPCBehaviour NPC
    {
      get
      {
        return this.npc;
      }
    }

    public virtual bool CanExecute
    {
      get
      {
        if (!this.IsEnabled)
          return false;
        if (this.Status != BehaviourTreeNodeStatus.Ready)
          return this.Status == BehaviourTreeNodeStatus.Running;
        return true;
      }
    }

    public bool IsComplete
    {
      get
      {
        if (this.IsEnabled && this.Status != BehaviourTreeNodeStatus.Success)
          return this.Status == BehaviourTreeNodeStatus.Failure;
        return true;
      }
    }

    public virtual object ForPropertyEditor
    {
      get
      {
        return (object) this;
      }
    }

    public virtual string ToStringParms
    {
      get
      {
        return (string) null;
      }
    }

    public BehaviourTreeNode()
    {
    }

    public BehaviourTreeNode(INPCBehaviour npc)
    {
      this.npc = npc;
    }

    public virtual void SetNPC(INPCBehaviour npc)
    {
      this.npc = npc;
      for (BehaviourTreeNode behaviourTreeNode = this.firstChild as BehaviourTreeNode; behaviourTreeNode != null; behaviourTreeNode = behaviourTreeNode.nextSibling as BehaviourTreeNode)
        behaviourTreeNode.SetNPC(npc);
    }

    public void InsertProxies(BehaviourTree tree, BehaviourTreeType treeType)
    {
      ProxyNode proxyNode = this as ProxyNode;
      if (proxyNode != null)
      {
        BehaviourTree behaviour = Globals1.GetBehaviour(treeType, proxyNode.Tree);
        if (behaviour != null)
        {
          BehaviourTreeNode root = behaviour.Clone(this.npc).Root as BehaviourTreeNode;
          if (root != null)
          {
            proxyNode.firstChild = (StudioForge.Engine.Core.Node) null;
            proxyNode.AddChild((StudioForge.Engine.Core.Node) root);
          }
        }
      }
      (this.firstChild as BehaviourTreeNode)?.InsertProxies(tree, treeType);
      (this.nextSibling as BehaviourTreeNode)?.InsertProxies(tree, treeType);
    }

    public void Update(ITMBehaviourExecutionEngine engine)
    {
      this.UpdateCore(engine);
    }

    protected abstract void UpdateCore(ITMBehaviourExecutionEngine engine);

    protected INPCBehaviour GetTarget(
      BehaviourTreeType treeType,
      BehaviourTreeNodeCompareTarget targetType)
    {
      return this.GetTarget(treeType, targetType, NpcQueryPreference.None);
    }

    protected INPCBehaviour GetTarget(
      BehaviourTreeType treeType,
      BehaviourTreeNodeCompareTarget targetType,
      NpcQueryPreference query)
    {
      if (targetType == BehaviourTreeNodeCompareTarget.Self)
        return this.npc;
      if (treeType == BehaviourTreeType.Dialog)
        return this.npc.CurrentDialogTarget;
      TargetData? lastTargetedBy = TargetingSystem.GetLastTargetedBy(this.npc);
      if (!lastTargetedBy.HasValue)
        return (INPCBehaviour) null;
      if (((lastTargetedBy.Value.Query | query) & NpcQueryPreference.Source) == NpcQueryPreference.None)
        return (INPCBehaviour) null;
      return lastTargetedBy.Value.Targeter;
    }

    string IPropertyEditorControl.ToString(string propertyName, object data)
    {
      return this.ToStringCore(propertyName, data);
    }

    protected virtual string ToStringCore(string propertyName, object data)
    {
      return (string) null;
    }

    public virtual bool IsPropertyEnabled(string propertyName)
    {
      return true;
    }

    public virtual void SetPropertyDefaults()
    {
    }

    public virtual void SetPropertyEditorDefaults(string name, Window win)
    {
      switch (name)
      {
        case "Continue":
          win.SetToolTip("If TRUE, execution will continue as if the node succeeded, even if it fails");
          break;
        case "IsEnabled":
          win.SetToolTip("If FALSE, the node and it's children will not execute");
          break;
      }
    }

    object IPropertyEditorControl.Validate(
      string propertyName,
      string input,
      out string adjustedInput)
    {
      return this.ValidateCore(propertyName, input, out adjustedInput);
    }

    protected virtual object ValidateCore(
      string propertyName,
      string input,
      out string adjustedInput)
    {
      adjustedInput = input;
      return (object) null;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (version > 251)
        this.IsEnabled = version <= 272 ? !reader.ReadBoolean() : reader.ReadBoolean();
      this.Continue = reader.ReadBoolean();
    }

    protected void ReadTypeList(BinaryReader reader, List<ActorType> list)
    {
      list.Clear();
      ushort num = reader.ReadUInt16();
      if (num <= (ushort) 0)
        return;
      for (int index = 0; index < (int) num; ++index)
        list.Add((ActorType) reader.ReadByte());
    }

    protected void ReadBlockList(BinaryReader reader, List<Block> list)
    {
      list.Clear();
      ushort num = reader.ReadUInt16();
      if (num <= (ushort) 0)
        return;
      for (int index = 0; index < (int) num; ++index)
        list.Add((Block) reader.ReadByte());
    }

    protected void ReadItemList(BinaryReader reader, List<Item> list)
    {
      list.Clear();
      ushort num = reader.ReadUInt16();
      if (num <= (ushort) 0)
        return;
      for (int index = 0; index < (int) num; ++index)
        list.Add((Item) reader.ReadUInt16());
    }

    protected override bool ShouldWriteChildren
    {
      get
      {
        return this.GetType() != typeof (ProxyNode);
      }
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.IsEnabled);
      writer.Write(this.Continue);
    }

    protected void WriteTypeList(BinaryWriter writer, List<ActorType> list)
    {
      if (list != null && list.Count > 0)
      {
        writer.Write((ushort) list.Count);
        foreach (ActorType actorType in list)
          writer.Write((byte) actorType);
      }
      else
        writer.Write((ushort) 0);
    }

    protected void WriteBlockList(BinaryWriter writer, List<Block> list)
    {
      if (list != null && list.Count > 0)
      {
        writer.Write((ushort) list.Count);
        foreach (Block block in list)
          writer.Write((byte) block);
      }
      else
        writer.Write((ushort) 0);
    }

    protected void WriteItemList(BinaryWriter writer, List<Item> list)
    {
      if (list != null && list.Count > 0)
      {
        writer.Write((ushort) list.Count);
        foreach (Item obj in list)
          writer.Write((ushort) obj);
      }
      else
        writer.Write((ushort) 0);
    }

    public static BehaviourTreeNodeType GetNodeTypeEnum(Type type)
    {
      foreach (Attribute customAttribute in Attribute.GetCustomAttributes((MemberInfo) type))
      {
        BehaviourTreeNodeAttribute treeNodeAttribute = customAttribute as BehaviourTreeNodeAttribute;
        if (treeNodeAttribute != null)
          return treeNodeAttribute.Type;
      }
      return BehaviourTreeNodeType.None;
    }

    public static bool GetNodeIsImplemented(Type type)
    {
      foreach (Attribute customAttribute in Attribute.GetCustomAttributes((MemberInfo) type))
      {
        BehaviourTreeNodeAttribute treeNodeAttribute = customAttribute as BehaviourTreeNodeAttribute;
        if (treeNodeAttribute != null)
          return treeNodeAttribute.IsImplemented;
      }
      return false;
    }

    public static string GetNodeTypeName(Type type)
    {
      foreach (Attribute customAttribute in Attribute.GetCustomAttributes((MemberInfo) type))
      {
        BehaviourTreeNodeAttribute treeNodeAttribute = customAttribute as BehaviourTreeNodeAttribute;
        if (treeNodeAttribute != null)
          return treeNodeAttribute.Name;
      }
      return type.Name;
    }

    protected string CutString(string s, int len)
    {
      if (s == null)
        return (string) null;
      string str = (string) null;
      int length = s.Length;
      if (length > len)
      {
        length = len;
        str = "..";
      }
      return s.Substring(0, length) + str;
    }

    protected string ShortPosType(CoordType t)
    {
      switch (t)
      {
        case CoordType.Absolute:
          return "Abs";
        case CoordType.PositionRelative:
          return "Pos";
        case CoordType.VelocityRelative:
          return "Vel";
        case CoordType.ViewRelative:
          return "View";
        case CoordType.SpawnRelative:
          return "Spawn";
        case CoordType.TargetRelative:
          return "Target";
        case CoordType.TargetsTargetRelative:
          return "TargetsTarget";
        default:
          return (string) null;
      }
    }
  }
}
