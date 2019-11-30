// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.MainBehavioursMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.AI;
using System;
using System.Windows.Forms;

namespace StudioForge.TotalMiner.Screens2
{
  internal class MainBehavioursMenu : NewGuiMenu2
  {
    private AIBehaviourTreeDesigner aiDesigner;
    private DialogTreeDesigner dialogDesigner;
    private Window mainMenuContainer;
    private StudioForge.Engine.GUI.ListBox behaviourListbox;
    private StudioForge.Engine.GUI.ListBox dialogListbox;
    private StudioForge.Engine.GUI.TextBox behaviourModeWin;
    private StudioForge.Engine.GUI.TextBox dialogModeWin;
    private StudioForge.Engine.GUI.TextBox behaviourPathWin;
    private StudioForge.Engine.GUI.TextBox dialogPathWin;
    private string behaviourPath;
    private string dialogPath;

    public override string Name
    {
      get
      {
        return "Behaviours";
      }
    }

    public MainBehavioursMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    private bool IsDesignerWinOpen
    {
      get
      {
        if (this.aiDesigner != null && this.aiDesigner.BaseWindow.Parent == this.canvas && this.aiDesigner.BaseWindow.IsVisible)
          return true;
        if (this.dialogDesigner != null && this.dialogDesigner.BaseWindow.Parent == this.canvas)
          return this.dialogDesigner.BaseWindow.IsVisible;
        return false;
      }
    }

    protected override void ResetCanvasTabData()
    {
      if (this.IsDesignerWinOpen)
        this.canvas.SlidingScroll = true;
      else
        base.ResetCanvasTabData();
    }

    private void InitMainContainer()
    {
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -150;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int x = 120;
      int y1 = 110;
      int width1 = 220;
      int height = 34;
      int num1 = 4;
      int num2 = 4;
      int num3 = height * num2 + num1 * (num2 - 1);
      float textScale = 0.6f;
      Window window1 = this.mainMenuContainer = new Window((string) null, x, y1, this.canvas.Size.X - x * 2, num3 + this.canvas.Size.Y - num1 - num3)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window1);
      int width2 = window1.Size.X / 2 - 20;
      int y2;
      int num4 = y2 = 0;
      int num5 = 0;
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window2;
      this.initialNavigable = window2 = (Window) new StudioForge.Engine.GUI.TextBox("New Behaviour", num5 + num4, y2, width1, height, textScale);
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(this.ClickNewBehaviour);
      window1.AddChild((Node) window2);
      int y3 = y2 + (height + num1);
      Window window3 = (Window) (this.behaviourModeWin = new StudioForge.Engine.GUI.TextBox("Mode: Edit", num5 + num4, y3, width1, height, textScale));
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window3.ClickHandler += new Window.WindowHandler(this.ClickBehaviourMode);
      window1.AddChild((Node) window3);
      int y4 = y3 + (height + num1);
      Window window4 = (Window) new StudioForge.Engine.GUI.TextBox("Export", num5 + num4, y4, width1, height, textScale);
      window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window4.ClickHandler += new Window.WindowHandler(this.ClickBehaviourExport);
      window4.SetToolTip("Export all behaviours in the current path to a file on your computer.\nNote this button is not available in Fullscreen.");
      window1.AddChild((Node) window4);
      int y5 = y4 + (height + num1) + (height + num1);
      Window window5 = (Window) new StudioForge.Engine.GUI.TextBox("[..]", num5 + num4, y5, 50, height, textScale);
      window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window5.ClickHandler += new Window.WindowHandler(this.ClickBehaviourPathBack);
      window1.AddChild((Node) window5);
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int num6 = num4 + (window5.Size.X + num1);
      Window window6 = (Window) (this.behaviourPathWin = new StudioForge.Engine.GUI.TextBox("Path: ", num5 + num6, y5, width2 - num6, height, textScale));
      window6.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((Node) window6);
      int num7 = 0;
      int y6 = y5 + (height + num1);
      Window window7 = (Window) (this.behaviourListbox = new StudioForge.Engine.GUI.ListBox((string) null, num5 + num7, y6, width2, window1.Size.Y - y6 - num1));
      window7.Colors = (Window.ColorProfile) Colors.ListBoxColors;
      this.behaviourListbox.ClearFlags(Window.WinFlags.FilteringEnabled);
      this.behaviourListbox.TextScale = textScale;
      this.behaviourListbox.ItemSelectedHandler += new Window.WindowHandler(this.ClickBehaviourName);
      this.behaviourListbox.Size.Y = this.behaviourListbox.Spacing * 18;
      window1.AddChild((Node) window7);
      this.LoadListBox(BehaviourTreeType.AI, "");
      int y7;
      int num8 = y7 = 0;
      int num9 = num5 + (width2 + 40);
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window8 = (Window) new StudioForge.Engine.GUI.TextBox("New Dialog", num9 + num8, y7, width1, height, textScale);
      window8.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window8.ClickHandler += new Window.WindowHandler(this.ClickNewDialog);
      window1.AddChild((Node) window8);
      int y8 = y7 + (height + num1);
      Window window9 = (Window) (this.dialogModeWin = new StudioForge.Engine.GUI.TextBox("Mode: Edit", num9 + num8, y8, width1, height, textScale));
      window9.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window9.ClickHandler += new Window.WindowHandler(this.ClickDialogMode);
      window1.AddChild((Node) window9);
      int y9 = y8 + (height + num1);
      Window window10 = (Window) new StudioForge.Engine.GUI.TextBox("Export", num9 + num8, y9, width1, height, textScale);
      window10.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window10.ClickHandler += new Window.WindowHandler(this.ClickDialogExport);
      window10.SetToolTip("Export all dialog in the current path to a file on your computer.\nNote this button is not available in Fullscreen.");
      window1.AddChild((Node) window10);
      int y10 = y9 + (height + num1) + (height + num1);
      Window window11 = (Window) new StudioForge.Engine.GUI.TextBox("[..]", num9 + num8, y10, 50, height, textScale);
      window11.ClickHandler += new Window.WindowHandler(this.ClickDialogPathBack);
      window11.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window1.AddChild((Node) window11);
      StudioForge.Engine.GUI.TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int num10 = num8 + (window11.Size.X + num1);
      Window window12 = (Window) (this.dialogPathWin = new StudioForge.Engine.GUI.TextBox("Path: ", num9 + num10, y10, width2 - num10, height, textScale));
      window12.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
      window1.AddChild((Node) window12);
      int num11 = 0;
      int y11 = y10 + (height + num1);
      Window window13 = (Window) (this.dialogListbox = new StudioForge.Engine.GUI.ListBox((string) null, num9 + num11, y11, width2, window1.Size.Y - y11 - num1));
      window13.Colors = (Window.ColorProfile) Colors.ListBoxColors;
      this.dialogListbox.ClearFlags(Window.WinFlags.FilteringEnabled);
      this.dialogListbox.TextScale = textScale;
      this.dialogListbox.ItemSelectedHandler += new Window.WindowHandler(this.ClickDialogName);
      this.dialogListbox.Size.Y = this.dialogListbox.Spacing * 18;
      window1.AddChild((Node) window13);
      this.LoadListBox(BehaviourTreeType.Dialog, "");
    }

    private bool IsExportEnabled()
    {
      GraphicsDeviceManager service = Services.GetService<GraphicsDeviceManager>();
      if (service != null)
        return !service.IsFullScreen;
      return false;
    }

    private void ClickNewBehaviour(object sender, WindowEventArgs e)
    {
      this.canvas.RemoveChild((Node) this.mainMenuContainer);
      this.aiDesigner = new AIBehaviourTreeDesigner(this.playerIndex, (Window) this.canvas, new BehaviourTree(BehaviourTreeType.AI, false)
      {
        Name = "Behaviour" + (object) Globals1.BehaviourTrees.Count + (object) 1
      }, new Action(this.ExitAIDesignerScreen));
      this.aiDesigner.LoadContent();
      this.canvas.AdjustSizeToContainAllChildrenDeep(new Func<Window, Point?>(this.ShouldAdjustSizeToContainAllChildren));
      this.canvas.SlidingScroll = true;
      this.canvas.SetMouse(Point.Zero);
    }

    private void ClickBehaviourMode(object sender, WindowEventArgs e)
    {
      if (this.behaviourModeWin.Text == "Mode: Edit")
        this.behaviourModeWin.Text = "Mode: Copy";
      else if (this.behaviourModeWin.Text == "Mode: Copy")
      {
        this.behaviourModeWin.Text = "Mode: Delete";
        this.behaviourModeWin.Colors = (Window.ColorProfile) Colors.ButtonWarnColors;
      }
      else
      {
        this.behaviourModeWin.Text = "Mode: Edit";
        this.behaviourModeWin.Colors = (Window.ColorProfile) Colors.ButtonColors;
      }
    }

    private void ClickBehaviourExport(object sender, WindowEventArgs e)
    {
      if (!this.IsExportEnabled())
        return;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
      saveFileDialog1.FileName = "Behaviour.db";
      saveFileDialog1.Title = "Select Behaviour Export file name";
      saveFileDialog1.ValidateNames = true;
      saveFileDialog1.Filter = "Behaviour Database (*.db)|*.db";
      saveFileDialog1.FilterIndex = 0;
      saveFileDialog1.OverwritePrompt = true;
      SaveFileDialog saveFileDialog2 = saveFileDialog1;
      if (saveFileDialog2.ShowDialog() != DialogResult.OK)
        return;
      this.ExportBehaviours(saveFileDialog2.FileName);
    }

    private void ClickNewDialog(object sender, WindowEventArgs e)
    {
      this.canvas.RemoveChild((Node) this.mainMenuContainer);
      this.dialogDesigner = new DialogTreeDesigner(this.playerIndex, (Window) this.canvas, new BehaviourTree(BehaviourTreeType.Dialog, false)
      {
        Name = "Dialog" + (object) Globals1.BehaviourTrees.Count + (object) 1
      }, new Action(this.ExitDialogDesignerScreen));
      this.dialogDesigner.LoadContent();
      this.canvas.AdjustSizeToContainAllChildrenDeep(new Func<Window, Point?>(this.ShouldAdjustSizeToContainAllChildren));
      this.canvas.SlidingScroll = true;
      this.canvas.SetMouse(Point.Zero);
    }

    private void ClickDialogMode(object sender, WindowEventArgs e)
    {
      if (this.dialogModeWin.Text == "Mode: Edit")
        this.dialogModeWin.Text = "Mode: Copy";
      else if (this.dialogModeWin.Text == "Mode: Copy")
      {
        this.dialogModeWin.Text = "Mode: Delete";
        this.dialogModeWin.Colors = (Window.ColorProfile) Colors.ButtonWarnColors;
      }
      else
      {
        this.dialogModeWin.Text = "Mode: Edit";
        this.dialogModeWin.Colors = (Window.ColorProfile) Colors.ButtonColors;
      }
    }

    private void ClickDialogExport(object sender, WindowEventArgs e)
    {
      if (!this.IsExportEnabled())
        return;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
      saveFileDialog1.FileName = "Dialog.db";
      saveFileDialog1.Title = "Select Dialog Export file name";
      saveFileDialog1.ValidateNames = true;
      saveFileDialog1.Filter = "Dialog Database (*.db)|*.db";
      saveFileDialog1.FilterIndex = 0;
      saveFileDialog1.OverwritePrompt = true;
      SaveFileDialog saveFileDialog2 = saveFileDialog1;
      if (saveFileDialog2.ShowDialog() != DialogResult.OK)
        return;
      this.ExportDialogs(saveFileDialog2.FileName);
    }

    private void ExitAIDesignerScreen()
    {
      if (this.aiDesigner == null)
        return;
      this.canvas.RemoveChild((Node) this.aiDesigner.BaseWindow);
      this.canvas.AddChild((Node) this.mainMenuContainer);
      this.canvas.SetMouse(Point.Zero);
      this.ResetCanvasTabData();
      this.LoadListBox(BehaviourTreeType.AI, this.behaviourPath);
      this.aiDesigner = (AIBehaviourTreeDesigner) null;
    }

    private void ExitDialogDesignerScreen()
    {
      if (this.dialogDesigner == null)
        return;
      this.canvas.RemoveChild((Node) this.dialogDesigner.BaseWindow);
      this.canvas.AddChild((Node) this.mainMenuContainer);
      this.canvas.SetMouse(Point.Zero);
      this.ResetCanvasTabData();
      this.LoadListBox(BehaviourTreeType.Dialog, this.dialogPath);
      this.dialogDesigner = (DialogTreeDesigner) null;
    }

    private void ClickBehaviourPathBack(object sender, WindowEventArgs args)
    {
      if (!this.behaviourPath.IsNotEmpty())
        return;
      int num = this.behaviourPath.Substring(0, this.behaviourPath.Length - 1).LastIndexOf('\\');
      if (num < 0)
        this.LoadListBox(BehaviourTreeType.AI, "");
      else
        this.LoadListBox(BehaviourTreeType.AI, this.behaviourPath.Substring(0, num + 1));
    }

    private void ClickDialogPathBack(object sender, WindowEventArgs args)
    {
      if (!this.dialogPath.IsNotEmpty())
        return;
      int num = this.dialogPath.Substring(0, this.dialogPath.Length - 1).LastIndexOf('\\');
      if (num < 0)
        this.LoadListBox(BehaviourTreeType.Dialog, "");
      else
        this.LoadListBox(BehaviourTreeType.Dialog, this.dialogPath.Substring(0, num + 1));
    }

    private void ClickBehaviourName(object sender, WindowEventArgs args)
    {
      if (this.behaviourListbox.Text.EndsWith("\\"))
        this.LoadListBox(BehaviourTreeType.AI, this.behaviourPath + this.behaviourListbox.Text);
      else if (this.behaviourModeWin.Text == "Mode: Delete")
      {
        Globals1.DeleteBehaviourTree(BehaviourTreeType.AI, this.behaviourPath + this.behaviourListbox.Text);
        this.LoadListBox(BehaviourTreeType.AI, this.behaviourPath);
      }
      else
      {
        BehaviourTree behaviour = Globals1.GetBehaviour(BehaviourTreeType.AI, this.behaviourPath + this.behaviourListbox.Text);
        if (behaviour == null)
          return;
        this.canvas.RemoveChild((Node) this.mainMenuContainer);
        BehaviourTree tree = behaviour.Clone((INPCBehaviour) null);
        if (this.behaviourModeWin.Text == "Mode: Copy")
          tree.Name = "Copy_" + tree.Name;
        this.aiDesigner = new AIBehaviourTreeDesigner(this.playerIndex, (Window) this.canvas, tree, new Action(this.ExitAIDesignerScreen));
        this.aiDesigner.LoadContent();
        this.canvas.AdjustSizeToContainAllChildrenDeep(new Func<Window, Point?>(this.ShouldAdjustSizeToContainAllChildren));
        this.canvas.SlidingScroll = true;
        this.canvas.SetMouse(Point.Zero);
      }
    }

    private void ClickDialogName(object sender, WindowEventArgs args)
    {
      if (this.dialogListbox.Text.EndsWith("\\"))
        this.LoadListBox(BehaviourTreeType.Dialog, this.dialogPath + this.dialogListbox.Text);
      else if (this.dialogModeWin.Text == "Mode: Delete")
      {
        Globals1.DeleteBehaviourTree(BehaviourTreeType.Dialog, this.dialogPath + this.dialogListbox.Text);
        this.LoadListBox(BehaviourTreeType.Dialog, this.dialogPath);
      }
      else
      {
        BehaviourTree behaviour = Globals1.GetBehaviour(BehaviourTreeType.Dialog, this.dialogPath + this.dialogListbox.Text);
        if (behaviour == null)
          return;
        this.canvas.RemoveChild((Node) this.mainMenuContainer);
        BehaviourTree tree = behaviour.Clone((INPCBehaviour) null);
        if (this.dialogModeWin.Text == "Mode: Copy")
          tree.Name = "Copy_" + tree.Name;
        this.dialogDesigner = new DialogTreeDesigner(this.playerIndex, (Window) this.canvas, tree, new Action(this.ExitDialogDesignerScreen));
        this.dialogDesigner.LoadContent();
        this.canvas.AdjustSizeToContainAllChildrenDeep(new Func<Window, Point?>(this.ShouldAdjustSizeToContainAllChildren));
        this.canvas.SlidingScroll = true;
        this.canvas.SetMouse(Point.Zero);
      }
    }

    private Point? ShouldAdjustSizeToContainAllChildren(Window win)
    {
      if (win.Tag is DesignerNode)
        return new Point?();
      if (win is Canvas)
        return new Point?(new Point(this.screenRect.Width, this.screenRect.Height));
      return new Point?(Point.Zero);
    }

    private void LoadListBox(BehaviourTreeType treeType, string path)
    {
      if (path == null)
        path = "";
      StudioForge.Engine.GUI.ListBox listBox;
      if (treeType == BehaviourTreeType.Dialog)
      {
        this.dialogPath = path;
        this.dialogPathWin.Text = "Path: " + path;
        listBox = this.dialogListbox;
      }
      else
      {
        this.behaviourPath = path;
        this.behaviourPathWin.Text = "Path: " + path;
        listBox = this.behaviourListbox;
      }
      listBox.ClearItems();
      foreach (BehaviourTree behaviourTree in Globals1.BehaviourTrees)
      {
        if (behaviourTree.TreeType == treeType && behaviourTree.Name.StartsWith(path))
        {
          string s = behaviourTree.Name.Substring(path.Length);
          int num = s.IndexOf('\\');
          if (num >= 0)
          {
            string str = s.Substring(0, num + 1);
            if (!listBox.Contains(str))
              listBox.AddItem(str);
          }
          else
            listBox.AddItem(s);
        }
      }
      listBox.SortItems(new Comparison<string>(Globals2.SortNamesWithFoldersAtTop));
    }

    private void ExportBehaviours(string filename)
    {
      Globals1.ExportBehaviourTrees(BehaviourTreeType.AI, filename, this.behaviourPath);
    }

    private void ExportDialogs(string filename)
    {
      Globals1.ExportBehaviourTrees(BehaviourTreeType.Dialog, filename, this.dialogPath);
    }
  }
}
