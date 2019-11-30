// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.TreeDesignWindow
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal abstract class TreeDesignWindow : Window
  {
    protected NodeTree nodeTree;
    protected DesignerNode designTree;
    protected Vector2 dragStartPos;
    protected TreeDesignWindow.DragMode dragMode;
    protected bool nameMode;
    protected bool isDraggingNode;
    protected Point nodeWindowsOffset;
    protected PlayerIndex playerIndex;

    public event EventHandler WindowsRebuilt;

    private void RaiseWindowsRebuilt()
    {
      if (this.WindowsRebuilt == null)
        return;
      this.WindowsRebuilt((object) this, EventArgs.Empty);
    }

    public DesignerNode DesignTree
    {
      get
      {
        return this.designTree;
      }
    }

    public bool GetNameMode()
    {
      return this.nameMode;
    }

    public TreeDesignWindow.DragMode GetDragMode()
    {
      return this.dragMode;
    }

    public void ToggleDragMode()
    {
      switch (this.dragMode)
      {
        case TreeDesignWindow.DragMode.Move:
          this.dragMode = TreeDesignWindow.DragMode.Copy;
          break;
        case TreeDesignWindow.DragMode.Copy:
          this.dragMode = TreeDesignWindow.DragMode.Move;
          break;
      }
    }

    public void ToggleNameMode()
    {
      this.nameMode = !this.nameMode;
      this.RebuildWindows();
    }

    public virtual bool CanAddNode(DesignerNode parent)
    {
      return true;
    }

    public TreeDesignWindow(
      PlayerIndex playerIndex,
      int x,
      int y,
      int w,
      int h,
      NodeTree tree,
      Point nodeWindowsOffset)
      : base((string) null, x, y, w, h)
    {
      this.playerIndex = playerIndex;
      this.nodeTree = tree;
      this.nodeWindowsOffset = nodeWindowsOffset;
      this.nameMode = false;
      this.designTree = new DesignerNode()
      {
        Tag = (object) tree
      };
    }

    public void RebuildWindows()
    {
      this.firstChild = (Node) null;
      Point nodeWindowsOffset = this.nodeWindowsOffset;
      this.RebuildWindowsCore(this.designTree, ref nodeWindowsOffset);
      this.RaiseWindowsRebuilt();
      this.SetDesignNodeColorDeep(this.designTree.FirstChild as DesignerNode, true);
    }

    protected virtual string GetNodeNameToStringExtra(object node)
    {
      return (string) null;
    }

    private void RebuildWindowsCore(DesignerNode node, ref Point p)
    {
      string str = this.GetNodeTypeName(node.Tag.GetType());
      if (this.nameMode)
      {
        string nameToStringExtra = this.GetNodeNameToStringExtra(node.Tag);
        if (nameToStringExtra != null)
          str = str + ": " + nameToStringExtra;
      }
      float num1 = 0.5f;
      Point winSize = this.GetWinSize(str, num1, node);
      TextBox textBox1 = new TextBox(str, p.X, p.Y, winSize.X, winSize.Y, num1);
      textBox1.Name = str;
      textBox1.Tag = (object) node;
      textBox1.BorderThickness = 1;
      textBox1.TextAlignX = WinTextAlignX.Left;
      textBox1.TextAlignY = WinTextAlignY.Center;
      TextBox textBox2 = textBox1;
      textBox2.Colors = (Window.ColorProfile) StudioForge.TotalMiner.Colors.NodeDisabled;
      textBox2.AddFlags(Window.WinFlags.IsDragable | Window.WinFlags.BorderRounded | Window.WinFlags.UseHoverColorIfDraggedOver);
      textBox2.DragStartHandler += new Window.WindowDragHandler(this.OnNodeDragStart);
      textBox2.DragEndHandler += new Window.WindowDragHandler(this.OnNodeDragEnd);
      this.AddChild((Node) textBox2);
      node.Win = textBox2;
      this.TreeNodeWindowAdded(node, (Window) textBox2);
      p.X += winSize.X + 20;
      DesignerNode node1 = node.FirstChild as DesignerNode;
      while (node1 != null)
      {
        this.AddLine(p.X - (node1.Parent == null || node1 == node1.Parent.FirstChild ? 19 : 11), p.Y + 15, p.X - 6, p.Y + 15, 1);
        int x = p.X;
        int y = p.Y;
        this.RebuildWindowsCore(node1, ref p);
        p.X = x;
        node1 = node1.NextSibling as DesignerNode;
        if (node1 != null)
        {
          int num2 = winSize.Y + 16;
          p.Y += num2;
          this.AddLine(p.X - 11, y + 16, p.X - 11, p.Y + 16, 0);
        }
      }
    }

    private void AddLine(int x1, int y1, int x2, int y2, int addEndScale)
    {
      Line line = new Line((string) null, x1, y1, x2, y2);
      line.Colors = (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeLine;
      this.AddChild((Node) line);
      if (addEndScale <= 0)
        return;
      Window window = new Window((string) null, x2, y1 - 1, 3 * addEndScale, 3 * addEndScale)
      {
        BorderThickness = addEndScale
      };
      window.Colors = (Window.ColorProfile)StudioForge.TotalMiner.Colors.NodeLineEnd;
      this.AddChild((Node) window);
    }

    protected virtual Point GetWinSize(string txt, float scale, DesignerNode node)
    {
      Point point = new Point();
      Vector2 vector2 = CoreGlobals.GameFont.MeasureString(txt) * scale;
      point.Y = (int) ((double) vector2.Y + 10.0);
      point.X = (int) ((double) vector2.X + 30.0) + point.Y / 2 + 6;
      return point;
    }

    protected DesignerNode GetDesignerNode(object tag)
    {
      return this.FindTag(this.designTree, tag);
    }

    private DesignerNode FindTag(DesignerNode node, object tag)
    {
      if (node.Tag == tag)
        return node;
      if (node.FirstChild != null)
      {
        DesignerNode tag1 = this.FindTag(node.FirstChild as DesignerNode, tag);
        if (tag1 != null)
          return tag1;
      }
      for (node = node.NextSibling as DesignerNode; node != null; node = node.NextSibling as DesignerNode)
      {
        DesignerNode tag1 = this.FindTag(node, tag);
        if (tag1 != null)
          return tag1;
      }
      return (DesignerNode) null;
    }

    protected DesignerNode GetTreeNode(Type type)
    {
      return this.FindTag(this.designTree, type);
    }

    private DesignerNode FindTag(DesignerNode node, Type type)
    {
      if (node.Tag.GetType() == type)
        return node;
      if (node.FirstChild != null)
      {
        DesignerNode tag = this.FindTag(node.FirstChild as DesignerNode, type);
        if (tag != null)
          return tag;
      }
      for (node = node.NextSibling as DesignerNode; node != null; node = node.NextSibling as DesignerNode)
      {
        DesignerNode tag = this.FindTag(node, type);
        if (tag != null)
          return tag;
      }
      return (DesignerNode) null;
    }

    protected virtual string GetNodeTypeName(Type type)
    {
      return type.Name;
    }

    protected abstract Window.ColorProfile GetNodeBackColor(DesignerNode node);

    protected virtual void SetDesignNodeColorDeep(DesignerNode dnode, bool isEnabled)
    {
    }

    protected abstract void BuildTree(NodeTree tree);

    public DesignerNode GetNewTreeNode(Type type)
    {
      return new DesignerNode()
      {
        Tag = this.GetNewNodeTag(type)
      };
    }

    protected virtual object GetNewNodeTag(Type type)
    {
      if (!(type != (Type) null))
        return (object) null;
      return Activator.CreateInstance(type);
    }

    public void AddNode(DesignerNode parent, DesignerNode child)
    {
      child.RemoveSelf();
      parent.AddChild((Node) child);
    }

    public void InsertNode(DesignerNode parent, DesignerNode after, DesignerNode child)
    {
      child.RemoveSelf();
      parent.InsertNode((Node) after, (Node) child);
    }

    public bool MoveNode(Window win, int dir)
    {
      if (win != null)
      {
        DesignerNode tag = win.Tag as DesignerNode;
        if (tag != null && tag.Parent != null && (dir <= 0 || tag.NextSibling != null) && (dir >= 0 || tag != tag.Parent.FirstChild))
        {
          Node node2 = dir < 0 ? tag.PrevSibling : tag.NextSibling;
          return tag.Parent.SwapNode((Node) tag, node2);
        }
      }
      return false;
    }

    public void DeleteNode(Window win)
    {
      if (win == null)
        return;
      this.RemoveChild((Node) win);
      this.DeleteNode(win.Tag as DesignerNode);
    }

    protected void DeleteNode(DesignerNode node)
    {
      if (node == null || node.Parent == null)
        return;
      node.Parent.RemoveChild((Node) node);
    }

    protected virtual void TreeNodeWindowAdded(DesignerNode node, Window win)
    {
    }

    protected void CopyTreeNode(DesignerNode treeNode, bool deep)
    {
      DesignerNode designerNode = new DesignerNode();
    }

    public virtual void OnParentClick(object sender, WindowEventArgs e)
    {
    }

    private void OnNodeDragStart(object Sender, WindowDragEventArgs args)
    {
      this.isDraggingNode = true;
      this.dragStartPos = args.Window.Position;
      this.OnNodeDragStartCore(args.Window.Tag as DesignerNode);
    }

    protected virtual void OnNodeDragStartCore(DesignerNode node)
    {
    }

    private void OnNodeDragEnd(object Sender, WindowDragEventArgs args)
    {
      this.isDraggingNode = false;
      Window window = args.Window;
      window.Position = this.dragStartPos;
      DesignerNode designerNode = window.Tag as DesignerNode;
      Window hovered = args.Hovered;
      if (designerNode == null || hovered == null)
        return;
      DesignerNode tag1 = hovered.Tag as DesignerNode;
      if (tag1 != null)
      {
        if (this.dragMode == TreeDesignWindow.DragMode.Move)
        {
          if (designerNode.Parent == null || tag1.IsChildOf((Node) designerNode))
            designerNode = (DesignerNode) null;
        }
        else if (this.dragMode == TreeDesignWindow.DragMode.Copy)
          designerNode = this.CopyTreeNode(designerNode);
        if (designerNode == null || !this.CanAddNode(tag1))
          return;
        this.AddNode(tag1, designerNode);
        this.RebuildWindows();
      }
      else
      {
        if (!(hovered.Parent is Window))
          return;
        DesignerNode tag2 = ((Window) hovered.Parent).Tag as DesignerNode;
        if (tag2 == null)
          return;
        DesignerNode parent = tag2.Parent as DesignerNode;
        if (parent == null)
          return;
        if (this.dragMode == TreeDesignWindow.DragMode.Move)
        {
          if (designerNode.Parent == null || parent.IsChildOf((Node) designerNode))
            designerNode = (DesignerNode) null;
        }
        else if (this.dragMode == TreeDesignWindow.DragMode.Copy)
          designerNode = this.CopyTreeNode(designerNode);
        if (designerNode == null || !this.CanAddNode(parent))
          return;
        DesignerNode after = (double) hovered.Position.Y > 0.0 ? tag2 : (tag2 == parent.FirstChild ? (DesignerNode) null : tag2.PrevSibling as DesignerNode);
        this.InsertNode(parent, after, designerNode);
        this.RebuildWindows();
      }
    }

    protected virtual DesignerNode CopyTreeNode(DesignerNode node)
    {
      return (DesignerNode) null;
    }

    public enum DragMode
    {
      Move,
      Copy,
    }
  }
}
