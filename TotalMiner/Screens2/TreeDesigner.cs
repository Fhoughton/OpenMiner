// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.TreeDesigner
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens2
{
  internal abstract class TreeDesigner
  {
    public static float ToolTipDelay = 0.25f;
    public bool TooltipsEnabled = true;
    public Window BaseWindow;
    protected Window parentWindow;
    protected TreeDesignWindow designWin;
    protected Window mainMenuContainer;
    protected Window nodeListContainer;
    protected Vector2 dragStartPos;
    protected Rectangle screenRect;
    protected PlayerIndex playerIndex;
    protected Dictionary<string, string> tooltips;
    private TextBox dragModeWin;
    private TextBox nameModeWin;
    private TextBox zoomWin;
    private TextBox tooltipsWin;

    protected virtual bool OneToolBarRowPerType
    {
      get
      {
        return true;
      }
    }

    private string NamesMenuText
    {
      get
      {
        return "Names: " + (this.designWin == null || !this.designWin.GetNameMode() ? "Short" : "Data");
      }
    }

    public TreeDesigner(PlayerIndex playerIndex, Window parent)
    {
      this.playerIndex = playerIndex;
      this.parentWindow = parent;
      this.screenRect.X = this.screenRect.Y = 0;
      this.screenRect.Width = parent.Size.X;
      this.screenRect.Height = parent.Size.Y;
      this.InitTooltips();
    }

    protected virtual void InitTooltips()
    {
    }

    public virtual void LoadContent()
    {
      this.BaseWindow = new Window("designerBase", 0, 0, this.parentWindow.Size.X, this.parentWindow.Size.Y);
      this.BaseWindow.Colors = Window.TransparentColorProfile;
      this.parentWindow.AddChild((StudioForge.Engine.Core.Node) this.BaseWindow);
      this.designWin = this.GetNewTreeDesignWindow();
      this.BaseWindow.AddChild((StudioForge.Engine.Core.Node) this.designWin);
      Canvas parent = this.BaseWindow.Parent as Canvas;
      if (parent != null)
        parent.ZoomHandler += new Window.WindowHandler(this.OnCanvasZoom);
      this.BaseWindow.ClickHandler += new Window.WindowHandler(this.designWin.OnParentClick);
      parent.ClickHandler += new Window.WindowHandler(this.designWin.OnParentClick);
      this.designWin.WindowsRebuilt += new EventHandler(this.WindowsRebuilt);
      this.InitMainMenuContainer();
      this.InitNodeListContainer();
      this.BaseWindow.EnableToolTips(this.TooltipsEnabled);
      this.ResizeWindows();
    }

    private void WindowsRebuilt(object sender, EventArgs e)
    {
      this.ResizeWindows();
    }

    private void ResizeWindows()
    {
      this.parentWindow.AdjustSizeToContainAllChildrenDeep(new Func<Window, Point?>(this.ShouldAdjustSizeToContainAllChildren));
      this.BaseWindow.Size.X = Math.Max(this.screenRect.Width, this.BaseWindow.Size.X);
      this.BaseWindow.Size.Y = Math.Max(this.screenRect.Height, this.BaseWindow.Size.Y);
      this.parentWindow.Size.X = Math.Max(this.screenRect.Width, this.parentWindow.Size.X);
      this.parentWindow.Size.Y = Math.Max(this.screenRect.Height, this.parentWindow.Size.Y);
      this.designWin.Size.X = Math.Max(this.screenRect.Width, this.designWin.Size.X);
      this.designWin.Size.Y = Math.Max(this.screenRect.Height, this.designWin.Size.Y);
    }

    private Point? ShouldAdjustSizeToContainAllChildren(Window win)
    {
      if (win.Tag is DesignerNode)
        return new Point?();
      if (win is Canvas)
        return new Point?(this.BaseWindow.Size);
      return new Point?(Point.Zero);
    }

    protected abstract TreeDesignWindow GetNewTreeDesignWindow();

    protected virtual void InitMainMenuContainer()
    {
      int x1 = 0;
      int y1 = 250;
      int width = 150;
      int height1 = 30;
      int num1 = 1;
      int num2 = 7;
      int height2 = height1 * num2 + num1 * (num2 - 1);
      this.mainMenuContainer = new Window((string) null, x1, y1, width, height2)
      {
        Name = "mainMenuContainer"
      };
      this.mainMenuContainer.Colors = Colors.NodeContainer;
      this.mainMenuContainer.BorderThickness = 1;
      this.BaseWindow.AddChild((StudioForge.Engine.Core.Node) this.mainMenuContainer);
      int y2;
      int x2 = y2 = 0;
      Window window1 = (Window) (this.dragModeWin = new TextBox("Drag: " + (this.designWin != null ? this.designWin.GetDragMode().ToString() : ""), x2, y2, width, height1, 0.5f));
      window1.Colors = (Window.ColorProfile) Colors.NodeDesignerButton;
      window1.ClickHandler += new Window.WindowHandler(this.ClickMainMenuDragMode);
      window1.SetToolTip("Change what happens when you drag a node", TreeDesigner.ToolTipDelay);
      this.mainMenuContainer.AddChild((StudioForge.Engine.Core.Node) window1);
      int y3 = y2 + (height1 + num1);
      Window window2 = (Window) (this.nameModeWin = new TextBox(this.NamesMenuText, x2, y3, width, height1, 0.5f));
      window2.Colors = (Window.ColorProfile) Colors.NodeDesignerButton;
      window2.ClickHandler += new Window.WindowHandler(this.ClickMainMenuNameMode);
      this.mainMenuContainer.AddChild((StudioForge.Engine.Core.Node) window2);
      int y4 = y3 + (height1 + num1);
      Window window3 = (Window) (this.zoomWin = new TextBox("Zoom: 100%", x2, y4, width, height1, 0.5f));
      window3.Colors = (Window.ColorProfile) Colors.NodeDesignerButton;
      window3.ClickHandler += new Window.WindowHandler(this.ClickMainMenuZoom);
      window3.SetToolTip("Click to reset Zoom to 100%", TreeDesigner.ToolTipDelay);
      this.mainMenuContainer.AddChild((StudioForge.Engine.Core.Node) window3);
      int y5 = y4 + (height1 + num1);
      Window window4 = (Window) (this.tooltipsWin = new TextBox("Tooltips: " + (this.TooltipsEnabled ? "On" : "Off"), x2, y5, width, height1, 0.5f));
      window4.Colors = (Window.ColorProfile) Colors.NodeDesignerButton;
      window4.ClickHandler += new Window.WindowHandler(this.ClickMainMenuTooltips);
      this.mainMenuContainer.AddChild((StudioForge.Engine.Core.Node) window4);
      int y6 = y5 + (height1 + num1);
      Window window5 = (Window) new TextBox("Save", x2, y6, width, height1, 0.5f);
      window5.Colors = (Window.ColorProfile) Colors.NodeDesignerButton;
      window5.ClickHandler += new Window.WindowHandler(this.ClickMainMenuSave);
      this.mainMenuContainer.AddChild((StudioForge.Engine.Core.Node) window5);
      int y7 = y6 + (height1 + num1);
      Window window6 = (Window) new TextBox("Save & Exit", x2, y7, width, height1, 0.5f);
      window6.Colors = (Window.ColorProfile) Colors.NodeDesignerButton;
      window6.ClickHandler += new Window.WindowHandler(this.ClickMainMenuSaveAndExit);
      this.mainMenuContainer.AddChild((StudioForge.Engine.Core.Node) window6);
      int y8 = y7 + (height1 + num1);
      Window window7 = (Window) new TextBox("Exit", x2, y8, width, height1, 0.5f);
      window7.Colors = (Window.ColorProfile) Colors.NodeDesignerButton;
      window7.ClickHandler += new Window.WindowHandler(this.ClickMainMenuExit);
      this.mainMenuContainer.AddChild((StudioForge.Engine.Core.Node) window7);
    }

    protected virtual void InitNodeListContainer()
    {
      int x1 = 0;
      int y1 = (int) this.mainMenuContainer.Position.Y + this.mainMenuContainer.Size.Y + 4;
      int height = 28;
      int num = 1;
      float textScale = 0.5f;
      DesignerNodeTagType[] nodeTypes = this.GetNodeTypes();
      this.nodeListContainer = new Window((string) null, x1, y1, 1, 1)
      {
        Name = "NodeContainer"
      };
      this.nodeListContainer.Colors = Colors.NodeContainer;
      this.nodeListContainer.BorderThickness = 1;
      this.BaseWindow.AddChild((StudioForge.Engine.Core.Node) this.nodeListContainer);
      int y2;
      int x2 = y2 = 0;
      float dragEnablePressTime = Window.DefaultDragEnablePressTime;
      Window.DefaultDragEnablePressTime = 0.0f;
      foreach (DesignerNodeTagType designerNodeTagType in nodeTypes)
      {
        bool flag = designerNodeTagType.Type == (Type) null;
        if (flag)
          y2 += 4;
        TextBox textBox = new TextBox(designerNodeTagType.Name, x2, y2, 150, height, textScale);
        textBox.IsEnabled = designerNodeTagType.IsImplemented;
        textBox.AddFlags(Window.WinFlags.IsDragable);
        textBox.Colors = flag ? (Window.ColorProfile) Colors.NodeHeader : (Window.ColorProfile) Colors.NodeType;
        textBox.Tag = (object) designerNodeTagType.Type;
        if (!flag)
        {
          textBox.DragStartHandler += new Window.WindowDragHandler(this.OnNodeTypeDragStart);
          textBox.DragEndHandler += new Window.WindowDragHandler(this.OnNodeTypeDragEnd);
          string tip;
          if (this.tooltips != null && this.tooltips.TryGetValue(designerNodeTagType.Name, out tip))
            textBox.SetToolTip(tip, TreeDesigner.ToolTipDelay);
        }
        this.nodeListContainer.AddChild((StudioForge.Engine.Core.Node) textBox);
        y2 += height + num;
        this.nodeListContainer.Size = new Point((int) Math.Max((float) this.nodeListContainer.Size.X, textBox.Position.X + (float) textBox.Size.X), (int) ((double) textBox.Position.Y + (double) textBox.Size.Y));
      }
      Window.DefaultDragEnablePressTime = dragEnablePressTime;
    }

    protected abstract DesignerNodeTagType[] GetNodeTypes();

    public void OnNodeTypeDragStart(object Sender, WindowDragEventArgs args)
    {
      this.dragStartPos = args.Window.Position;
    }

    public void OnNodeTypeDragEnd(object Sender, WindowDragEventArgs args)
    {
      Window window = args.Window;
      window.Position = this.dragStartPos;
      Window hovered = args.Hovered;
      if (hovered == null)
        return;
      DesignerNode tag1 = hovered.Tag as DesignerNode;
      if (tag1 != null)
      {
        if (!this.designWin.CanAddNode(tag1))
          return;
        DesignerNode newTreeNode = this.designWin.GetNewTreeNode(window.Tag as Type);
        this.designWin.AddNode(tag1, newTreeNode);
        this.designWin.RebuildWindows();
      }
      else
      {
        if (!(hovered.Parent is Window))
          return;
        DesignerNode tag2 = ((Window) hovered.Parent).Tag as DesignerNode;
        if (tag2 == null)
          return;
        if (tag2.Tag is NodeTree)
        {
          if (!this.designWin.CanAddNode(tag2))
            return;
          DesignerNode newTreeNode = this.designWin.GetNewTreeNode(window.Tag as Type);
          this.designWin.AddNode(tag2, newTreeNode);
          this.designWin.RebuildWindows();
        }
        else
        {
          DesignerNode parent = tag2.Parent as DesignerNode;
          if (parent == null || !this.designWin.CanAddNode(parent))
            return;
          DesignerNode after = (double) hovered.Position.Y > 0.0 ? tag2 : (tag2 == parent.FirstChild ? (DesignerNode) null : tag2.PrevSibling as DesignerNode);
          DesignerNode newTreeNode = this.designWin.GetNewTreeNode(window.Tag as Type);
          this.designWin.InsertNode(parent, after, newTreeNode);
          this.designWin.RebuildWindows();
        }
      }
    }

    protected virtual void ClickMainMenuNew(object Sender, WindowEventArgs args)
    {
    }

    protected virtual void ClickMainMenuSave(object Sender, WindowEventArgs args)
    {
    }

    protected virtual void ClickMainMenuSaveAndExit(object Sender, WindowEventArgs args)
    {
    }

    protected virtual void ClickMainMenuDelete(object Sender, WindowEventArgs args)
    {
    }

    protected virtual void ClickMainMenuExit(object Sender, WindowEventArgs args)
    {
    }

    private void ClickMainMenuDragMode(object Sender, WindowEventArgs args)
    {
      this.designWin.ToggleDragMode();
      this.dragModeWin.Text = "Drag: " + this.designWin.GetDragMode().ToString();
    }

    private void ClickMainMenuNameMode(object Sender, WindowEventArgs args)
    {
      this.designWin.ToggleNameMode();
      this.nameModeWin.Text = this.NamesMenuText;
    }

    private void ClickMainMenuZoom(object Sender, WindowEventArgs args)
    {
      Canvas parent = this.BaseWindow.Parent as Canvas;
      if (parent == null)
        return;
      parent.Scale = 1f;
    }

    private void ClickMainMenuTooltips(object Sender, WindowEventArgs args)
    {
      this.TooltipsEnabled = !this.TooltipsEnabled;
      this.BaseWindow.EnableToolTips(this.TooltipsEnabled);
      this.tooltipsWin.Text = "Tooltips: " + (this.TooltipsEnabled ? "On" : "Off");
    }

    private void OnCanvasZoom(object Sender, WindowEventArgs args)
    {
      Canvas window = args.Window as Canvas;
      if (window == null)
        return;
      this.zoomWin.Text = "Zoom: " + (object) (int) ((double) window.Scale * 100.0) + "%";
    }
  }
}
