// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.BehaviourTreeDesignWindow
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Reflection;

namespace StudioForge.TotalMiner.Screens2
{
  internal abstract class BehaviourTreeDesignWindow : TreeDesignWindow
  {
    private Texture2D iconTextures;
    private PropertyEditor clickedEditor;
    private PropertyEditor hoverEditor;

    protected virtual bool CanShowPropertiesEditor
    {
      get
      {
        return !this.isDraggingNode;
      }
    }

    public BehaviourTreeDesignWindow(
      PlayerIndex playerIndex,
      int x,
      int y,
      int w,
      int h,
      BehaviourTree tree,
      Texture2D iconTextures,
      Point nodeWindowsOffset)
      : base(playerIndex, x, y, w, h, (NodeTree) tree, nodeWindowsOffset)
    {
      if (iconTextures == null)
        iconTextures = CoreGlobals.Content.Load<Texture2D>("Textures\\AIIcons");
      this.iconTextures = iconTextures;
      this.BuildTree((NodeTree) tree);
      this.RebuildWindows();
    }

    protected override void SetDesignNodeColorDeep(DesignerNode dnode, bool isEnabled)
    {
      if (dnode == null)
        return;
      BehaviourTreeNode tag = dnode.Tag as BehaviourTreeNode;
      if (tag == null)
        return;
      isEnabled &= tag.IsEnabled;
      dnode.Win.Colors = isEnabled ? this.GetNodeBackColor(dnode) : (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeDisabled;
      for (DesignerNode dnode1 = dnode.FirstChild as DesignerNode; dnode1 != null; dnode1 = dnode1.NextSibling as DesignerNode)
        this.SetDesignNodeColorDeep(dnode1, isEnabled);
    }

    protected override void BuildTree(NodeTree tree)
    {
      BehaviourTree tree1 = tree as BehaviourTree;
      if (tree1 == null)
        return;
      for (BehaviourTreeNode node = tree1.Root as BehaviourTreeNode; node != null; node = node.NextSibling as BehaviourTreeNode)
      {
        node.InsertProxies(tree1, tree1.TreeType);
        DesignerNode parent = new DesignerNode()
        {
          Tag = (object) node
        };
        this.designTree.AddChild((Node) parent);
        this.BuildTreeChildren(parent, node);
      }
    }

    private void BuildTreeChildren(DesignerNode parent, BehaviourTreeNode node)
    {
      for (BehaviourTreeNode node1 = node.FirstChild as BehaviourTreeNode; node1 != null; node1 = node1.NextSibling as BehaviourTreeNode)
      {
        DesignerNode parent1 = new DesignerNode()
        {
          Tag = (object) node1
        };
        parent.AddChild((Node) parent1);
        this.BuildTreeChildren(parent1, node1);
      }
    }

    protected override DesignerNode CopyTreeNode(DesignerNode treeNode)
    {
      this.RebuildBehaviourTree(treeNode);
      DesignerNode parent = (DesignerNode) null;
      BehaviourTreeNode node = Node.Clone((Node) (treeNode.Tag as BehaviourTreeNode)) as BehaviourTreeNode;
      if (node != null)
      {
        parent = new DesignerNode() { Tag = (object) node };
        this.BuildTreeChildren(parent, node);
      }
      return parent;
    }

    private BehaviourTreeNode RebuildBehaviourTree(DesignerNode tnode)
    {
      BehaviourTreeNode tag = tnode.Tag as BehaviourTreeNode;
      tag.RemoveAllChildren();
      for (Node node = tnode.FirstChild; node != null; node = node.NextSibling)
        tag.AddChild((Node) this.RebuildBehaviourTree(node as DesignerNode));
      return tag;
    }

    protected override object GetNewNodeTag(Type type)
    {
      object newNodeTag = base.GetNewNodeTag(type);
      this.SetPropertyDefaults(newNodeTag);
      return newNodeTag;
    }

    protected override string GetNodeTypeName(Type type)
    {
      if (!type.IsSubclassOf(typeof (BehaviourTreeNode)))
        return base.GetNodeTypeName(type);
      return BehaviourTreeNode.GetNodeTypeName(type);
    }

    protected override void TreeNodeWindowAdded(DesignerNode node, Window win)
    {
      BehaviourTree tag1 = node.Tag as BehaviourTree;
      if (tag1 != null)
      {
        TextBox textBox = win as TextBox;
        textBox.Text = (string) null;
        win.Colors = (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeTree;
        win.ClearFlags(Window.WinFlags.IsDragable);
        DataField dataField1 = new DataField(Globals2.RemovePath(tag1.Name), 30, 1, win.Size.X - 31, win.Size.Y - 2, textBox.TextScale);
        dataField1.AddFlags(Window.WinFlags.ClipChildren | Window.WinFlags.UseHoverColorIfDraggedOver);
        dataField1.Colors = (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeTree;
        ((ITextInputWindow) dataField1).OnBeginInput = new Action<ITextInputWindow>(this.BeginInputName);
        DataField dataField2 = dataField1;
        ((ITextInputWindow) dataField2).OnValidateInput = ((ITextInputWindow) dataField2).OnValidateInput + new Action<ITextInputWindow>(this.ValidateName);
        win.AddChild((Node) dataField1);
        this.AddIcon(win, -16);
      }
      else
      {
        BehaviourTreeNode tag2 = node.Tag as BehaviourTreeNode;
        if (tag2 == null)
          return;
        win.ClickHandler += new Window.WindowHandler(this.ClickNodeHandler);
        win.HoverStartHandler += new Window.WindowHandler(this.HoverNodeHandler);
        win.HoverEndHandler += new Window.WindowHandler(this.HoverEndNodeHandler);
        win.RightClickHandler += new Window.WindowHandler(this.DeleteNodeHandler);
        win.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
        this.AddIcon(win);
        if (tag2.Continue)
          this.AddContinueIcon(win);
        int num = win.Size.Y / 2;
        Window window1 = new Window((string) null, 0, 0, num, num)
        {
          BorderThickness = 1
        };
        window1.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
        window1.Colors = (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeDropButton;
        window1.LoadTexture(this.iconTextures, false, false, 1f);
        window1.Texture.SrRect = new Rectangle?(new Rectangle(168, 0, 8, 9));
        window1.Texture.DestRect = new Rectangle?(new Rectangle(4, 3, 8, 9));
        window1.ClickHandler += new Window.WindowHandler(this.ClickNodeMoveUp);
        win.AddChild((Node) window1);
        Window window2 = new Window((string) null, 0, num + 1, num, num)
        {
          BorderThickness = 1
        };
        window2.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver);
        window2.Colors = (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeDropButton;
        window2.LoadTexture(this.iconTextures, false, false, 1f);
        window2.Texture.SrRect = new Rectangle?(new Rectangle(176, 0, 8, 9));
        window2.Texture.DestRect = new Rectangle?(new Rectangle(4, 3, 8, 9));
        window2.ClickHandler += new Window.WindowHandler(this.ClickNodeMoveDown);
        win.AddChild((Node) window2);
      }
    }

    protected override string GetNodeNameToStringExtra(object node)
    {
      return (node as BehaviourTreeNode)?.ToStringParms;
    }

    protected override Window.ColorProfile GetNodeBackColor(DesignerNode node)
    {
      Type type = node == null || node.Tag == null ? (Type) null : node.Tag.GetType();
      switch (type != (Type) null ? BehaviourTreeNode.GetNodeTypeEnum(type) : BehaviourTreeNodeType.None)
      {
        case BehaviourTreeNodeType.Conditional:
          return (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeConditional;
        case BehaviourTreeNodeType.Action:
          return (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeAction;
        default:
          return (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeLogic;
      }
    }

    protected override Point GetWinSize(string txt, float scale, DesignerNode node)
    {
      Point winSize = base.GetWinSize(txt, scale, node);
      winSize.X += winSize.Y - 14 - 2;
      return winSize;
    }

    private void AddIcon(Window win)
    {
      this.AddIcon(win, 0);
    }

    private void AddIcon(Window win, int xoffset)
    {
      TextBox textBox = win as TextBox;
      if (textBox == null)
        return;
      int num = textBox.Size.Y - 14;
      int x1 = win.Size.Y / 2 + 6 + xoffset;
      Window window = new Window((string) null, x1, 6, num, num);
      window.AddFlags(Window.WinFlags.UseHoverColorIfDraggedOver | Window.WinFlags.IsNotHoverable);
      window.LoadTexture(this.iconTextures, true, true, 1f);
      window.IsEnabled = false;
      int x2 = 8;
      DesignerNode tag = win.Tag as DesignerNode;
      if (tag != null)
        x2 = tag.Tag is BehaviourTree ? 24 : (tag.Tag is UpdateNode ? 40 : (tag.Tag is ScriptNode || tag.Tag is MessageNode ? 56 : (tag.Tag is AttackNode ? 72 : (tag.Tag is IsVisibleNode ? 88 : (tag.Tag is IsDistanceNode ? 104 : (tag.Tag is MoveNode ? 120 : (tag.Tag is IsNpcTypeQueryNode || tag.Tag is ComparisonNodeBoolean || (tag.Tag is ComparisonNodeByte || tag.Tag is ComparisonNodeDouble) || (tag.Tag is ComparisonNodeInt || tag.Tag is ComparisonNodeLong || (tag.Tag is ComparisonNodeShort || tag.Tag is ComparisonNodeSingle)) || (tag.Tag is ComparisonNodeUInt || tag.Tag is ComparisonNodeULong || tag.Tag is ComparisonNodeUShort) ? 136 : 8)))))));
      window.Texture.SrRect = new Rectangle?(new Rectangle(x2, 0, 16, 16));
      window.Colors = StudioForge.TotalMiner.Colors.IconColors;
      win.AddChild((Node) window);
      textBox.TextOffset.X += (float) (x1 + 22);
    }

    private void AddContinueIcon(Window win)
    {
      TextBox textBox = win as TextBox;
      if (textBox == null)
        return;
      Window window = new Window("ContinueIcon", textBox.Size.X - 10, textBox.Size.Y - 10, 8, 8);
      window.LoadTexture(this.iconTextures, true, true, 1f);
      window.Texture.SrRect = new Rectangle?(new Rectangle(0, 0, 8, 8));
      window.Colors = StudioForge.TotalMiner.Colors.IconColors;
      win.AddChild((Node) window);
    }

    private void ClickNodeMoveUp(object sender, WindowEventArgs e)
    {
      Window win = e.Window.Tag is DesignerNode ? e.Window : (Window) e.Window.Parent;
      if (win == null || !this.MoveNode(win, -1))
        return;
      this.RebuildWindows();
    }

    private void ClickNodeMoveDown(object sender, WindowEventArgs e)
    {
      Window win = e.Window.Tag is DesignerNode ? e.Window : (Window) e.Window.Parent;
      if (win == null || !this.MoveNode(win, 1))
        return;
      this.RebuildWindows();
    }

    private void DeleteNodeHandler(object sender, WindowEventArgs e)
    {
      Window win = e.Window.Tag is DesignerNode ? e.Window : (Window) e.Window.Parent;
      if (win == null)
        return;
      this.DeleteNode(win);
      this.RebuildWindows();
    }

    private void HoverNodeHandler(object sender, WindowEventArgs e)
    {
      if (this.hoverEditor != null)
        this.hoverEditor.RemoveSelf();
      this.hoverEditor = this.OpenEditor(e.Window);
      if (this.clickedEditor == null)
        return;
      this.clickedEditor.IsVisible = false;
    }

    private void HoverEndNodeHandler(object sender, WindowEventArgs e)
    {
      if (this.hoverEditor != null)
      {
        this.hoverEditor.RemoveSelf();
        this.hoverEditor = (PropertyEditor) null;
      }
      if (this.clickedEditor == null)
        return;
      this.clickedEditor.IsVisible = true;
    }

    private void ClickNodeHandler(object sender, WindowEventArgs e)
    {
      if (this.clickedEditor != null)
        this.clickedEditor.RemoveSelf();
      this.clickedEditor = this.OpenEditor(e.Window);
    }

    protected override void OnClickCore(WindowEventArgs e)
    {
      base.OnClickCore(e);
      this.CloseEditor(false, true);
    }

    public override void OnParentClick(object sender, WindowEventArgs e)
    {
      this.CloseEditor(false, true);
    }

    protected override void OnNodeDragStartCore(DesignerNode node)
    {
      this.CloseEditor(true, true);
    }

    protected override bool OnKeyReleaseCore(WindowEventArgs e, Keys[] keys)
    {
      if (keys[0] != Keys.Escape || this.clickedEditor == null)
        return base.OnKeyReleaseCore(e, keys);
      this.CloseEditor(true, true);
      return true;
    }

    private void BeginInputName(ITextInputWindow win)
    {
      BehaviourTree tag = this.designTree.Tag as BehaviourTree;
      if (tag == null)
        return;
      win.Text = tag.Name;
    }

    private void ValidateName(ITextInputWindow win)
    {
      if (!win.Text.IsNotEmpty())
        return;
      string path = Globals2.StripFolderName(win.Text);
      string s = Globals2.RemovePath(path);
      if (!s.IsNotEmpty())
        return;
      win.Text = s;
      BehaviourTree tag = this.designTree.Tag as BehaviourTree;
      if (tag == null)
        return;
      tag.Name = path;
      tag.Immutable = path.StartsWith("System\\", StringComparison.OrdinalIgnoreCase);
    }

    private void CloseEditor(bool hover, bool clicked)
    {
      if (hover && this.hoverEditor != null)
      {
        this.hoverEditor.RemoveSelf();
        this.hoverEditor = (PropertyEditor) null;
      }
      if (!clicked || this.clickedEditor == null)
        return;
      this.clickedEditor.RemoveSelf();
      this.clickedEditor = (PropertyEditor) null;
    }

    private PropertyEditor OpenEditor(Window nodeWin)
    {
      if (this.CanShowPropertiesEditor)
      {
        DesignerNode tag1 = nodeWin.Tag as DesignerNode;
        if (tag1 != null)
        {
          BehaviourTreeNode tag2 = tag1.Tag as BehaviourTreeNode;
          if (tag2 != null)
          {
            int width1 = Math.Min(this.GetPropertyWinSize(tag2.GetType()), GraphicStatics.HUDPos().Width - 220);
            int width2 = GraphicStatics.HUDPos().Width;
            PropertyEditor propertyEditor1 = new PropertyEditor((string) null, (int) nodeWin.Position.X, (int) ((double) nodeWin.Position.Y + (double) nodeWin.Size.Y + 8.0), width1, 0, tag2.ForPropertyEditor, new Action<ITextInputWindow, object>(this.OnPropertyValidated));
            propertyEditor1.BorderThickness = 1;
            propertyEditor1.Colors = (Window.ColorProfile) PropertyEditor.PropertyLabelColors;
            PropertyEditor propertyEditor2 = propertyEditor1;
            if (propertyEditor2.Size.Y > 4)
            {
              this.parent.AddChild((Node) propertyEditor2);
              propertyEditor2.IsEnabled = !this.IsFromProxy(tag2);
              return propertyEditor2;
            }
          }
        }
      }
      return (PropertyEditor) null;
    }

    protected virtual int GetPropertyWinSize(Type type)
    {
      if (type == typeof (DialogNode) || type == typeof (ScriptNode))
        return 1220;
      return !(type == typeof (ProxyNode)) ? 660 : 800;
    }

    private void OnPropertyValidated(ITextInputWindow win, object tag)
    {
      BehaviourTreeNode node = tag as BehaviourTreeNode;
      if (node == null)
        return;
      PropertyEditor.TypeData tag1 = win.Tag as PropertyEditor.TypeData;
      if (tag1 != null)
      {
        switch ((object) (tag1.FieldInfo as FieldInfo) != null ? ((MemberInfo) tag1.FieldInfo).Name : ((object) (tag1.FieldInfo as PropertyInfo) != null ? ((MemberInfo) tag1.FieldInfo).Name : ""))
        {
          case "IsEnabled":
            this.SetDesignNodeColorDeep(this.GetDesignerNode((object) node), true);
            break;
          case "Continue":
            DesignerNode designerNode1 = this.GetDesignerNode(tag);
            if (designerNode1 != null)
            {
              if (node.Continue)
              {
                this.AddContinueIcon((Window) designerNode1.Win);
                break;
              }
              Window child = designerNode1.Win.FindChild("ContinueIcon");
              if (child != null)
              {
                child.RemoveSelf();
                break;
              }
              break;
            }
            break;
        }
      }
      ProxyNode proxyNode = node as ProxyNode;
      if (proxyNode == null)
        return;
      BehaviourTree nodeTree = this.nodeTree as BehaviourTree;
      if (nodeTree == null)
        return;
      proxyNode.InsertProxies(nodeTree, nodeTree.TreeType);
      DesignerNode designerNode2 = this.GetDesignerNode((object) node);
      if (designerNode2 == null)
        return;
      this.BuildTreeChildren(designerNode2, node);
      this.RebuildWindows();
    }

    private bool IsFromProxy(BehaviourTreeNode node)
    {
      for (Node parent = node.Parent; parent != null; parent = parent.Parent)
      {
        if (parent is ProxyNode)
          return true;
      }
      return false;
    }

    protected virtual void SetPropertyDefaults(object node)
    {
      IPropertyEditorControl propertyEditorControl = node as IPropertyEditorControl;
      if (propertyEditorControl == null)
        return;
      propertyEditorControl.SetPropertyDefaults();
      WaypointNode waypointNode = node as WaypointNode;
      if (waypointNode == null)
        return;
      waypointNode.VelocityModifier = 0.5f;
      Player localPlayer = GameInstance.Instance.GetLocalPlayer(this.playerIndex);
      if (localPlayer == null)
        return;
      MapTM map = GameInstance.Instance.Map;
      lock (map.MapStrategyTM.MarkerBlocks)
      {
        foreach (MarkerBlock markerBlock in map.MapStrategyTM.MarkerBlocks)
        {
          if (markerBlock.GamerID == localPlayer.GamerID)
          {
            Vector3 blockCenter = map.GetBlockCenter(markerBlock.Point);
            blockCenter.Y -= map.HalfTileSize;
            waypointNode.AddWaypoint(blockCenter);
          }
        }
      }
    }
  }
}
