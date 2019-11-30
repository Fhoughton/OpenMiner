// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.BehaviourTreeDesigner
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.AI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StudioForge.TotalMiner.Screens2
{
  internal abstract class BehaviourTreeDesigner : TreeDesigner
  {
    protected BehaviourTree tree;
    protected Texture2D iconTextures;
    private Action exitScreen;

    public BehaviourTreeDesigner(
      PlayerIndex playerIndex,
      Window parent,
      BehaviourTree tree,
      Action exitScreen)
      : base(playerIndex, parent)
    {
      this.tree = tree;
      this.exitScreen = exitScreen;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.iconTextures = CoreGlobals.Content.Load<Texture2D>("Textures\\AIIcons");
    }

    protected override DesignerNodeTagType[] GetNodeTypes()
    {
      List<Type> result = new List<Type>();
      Type baseType = typeof (BehaviourTreeNode);
      this.LoadNodeTypes(baseType.Assembly, baseType, result);
      this.LoadNodeTypes(Assembly.GetEntryAssembly(), baseType, result);
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        foreach (Assembly assembly in activeMod.Assemblies)
          this.LoadNodeTypes(assembly, baseType, result);
      }
      result.Sort(new Comparison<Type>(this.SortNodeTypes));
      List<DesignerNodeTagType> designerNodeTagTypeList = new List<DesignerNodeTagType>(result.Count);
      BehaviourTreeNodeType behaviourTreeNodeType = BehaviourTreeNodeType.Logic;
      foreach (Type type in result)
      {
        BehaviourTreeNodeType nodeTypeEnum = BehaviourTreeNode.GetNodeTypeEnum(type);
        if (nodeTypeEnum != behaviourTreeNodeType || designerNodeTagTypeList.Count == 0)
        {
          designerNodeTagTypeList.Add(new DesignerNodeTagType()
          {
            Name = nodeTypeEnum.ToString()
          });
          behaviourTreeNodeType = nodeTypeEnum;
        }
        DesignerNodeTagType designerNodeTagType = new DesignerNodeTagType()
        {
          Type = type,
          Name = BehaviourTreeNode.GetNodeTypeName(type),
          IsImplemented = BehaviourTreeNode.GetNodeIsImplemented(type)
        };
        designerNodeTagTypeList.Add(designerNodeTagType);
      }
      return designerNodeTagTypeList.ToArray();
    }

    private void LoadNodeTypes(Assembly assembly, Type baseType, List<Type> result)
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (!type.IsAbstract && type.IsSubclassOf(baseType) && (BehaviourTreeNode.GetNodeIsImplemented(type) && this.IsValidNodeType(type)))
          result.Add(type);
      }
    }

    protected abstract bool IsValidNodeType(Type t);

    private int SortNodeTypes(Type t1, Type t2)
    {
      BehaviourTreeNodeType nodeTypeEnum1 = BehaviourTreeNode.GetNodeTypeEnum(t1);
      BehaviourTreeNodeType nodeTypeEnum2 = BehaviourTreeNode.GetNodeTypeEnum(t2);
      if (nodeTypeEnum1 != nodeTypeEnum2)
      {
        if (nodeTypeEnum1 == BehaviourTreeNodeType.Logic)
          return -1;
        if (nodeTypeEnum2 == BehaviourTreeNodeType.Logic || nodeTypeEnum1 == BehaviourTreeNodeType.Action)
          return 1;
        if (nodeTypeEnum2 == BehaviourTreeNodeType.Action)
          return -1;
      }
      return t1.Name.CompareTo(t2.Name);
    }

    protected override void ClickMainMenuSave(object sender, WindowEventArgs args)
    {
      this.SaveBehaviours();
    }

    protected override void ClickMainMenuSaveAndExit(object sender, WindowEventArgs args)
    {
      this.SaveBehaviours();
      if (this.exitScreen == null)
        return;
      this.exitScreen();
    }

    protected override void ClickMainMenuExit(object Sender, WindowEventArgs args)
    {
      if (this.exitScreen == null)
        return;
      this.exitScreen();
    }

    protected virtual void SaveBehaviours()
    {
      Globals1.DeleteBehaviourTree(this.tree.TreeType, this.tree.Name);
      if (this.designWin.DesignTree.FirstChild != null)
      {
        this.tree.SetRoot((StudioForge.Engine.Core.Node) this.RebuildBehaviourTree(this.designWin.DesignTree.FirstChild as DesignerNode));
        Globals1.BehaviourTrees.Add(this.tree);
      }
      Globals1.SaveBehaviourTrees();
    }

    private BehaviourTreeNode RebuildBehaviourTree(DesignerNode tnode)
    {
      BehaviourTreeNode behaviourTreeNode = tnode != null ? tnode.Tag as BehaviourTreeNode : (BehaviourTreeNode) null;
      if (behaviourTreeNode != null)
      {
        behaviourTreeNode.ClearPointers();
        this.RebuildBehaviourTreeCore(tnode);
        for (DesignerNode nextSibling = tnode.NextSibling as DesignerNode; nextSibling != null; nextSibling = nextSibling.NextSibling as DesignerNode)
          behaviourTreeNode.AddSibling((StudioForge.Engine.Core.Node) this.RebuildBehaviourTreeCore(nextSibling));
      }
      return behaviourTreeNode;
    }

    private BehaviourTreeNode RebuildBehaviourTreeCore(DesignerNode tnode)
    {
      BehaviourTreeNode tag = tnode.Tag as BehaviourTreeNode;
      tag.RemoveAllChildren();
      for (StudioForge.Engine.Core.Node node = tnode.FirstChild; node != null; node = node.NextSibling)
        tag.AddChild((StudioForge.Engine.Core.Node) this.RebuildBehaviourTreeCore(node as DesignerNode));
      return tag;
    }
  }
}
