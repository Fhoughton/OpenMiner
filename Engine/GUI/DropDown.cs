// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.DropDown
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.GUI
{
  public class DropDown : DataField
  {
    public static Window.ColorProfile DropDownArrowColorProfile = new Window.ColorProfile()
    {
      BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor,
      BackClickColor = TextBox.DefaultColorProfile.BackClickColor,
      BackColor = TextBox.DefaultColorProfile.BackColor,
      BackHoverColor = TextBox.DefaultColorProfile.BackHoverColor,
      BorderColor = Color.White,
      ForeColor = TextBox.DefaultColorProfile.ForeColor
    };
    public bool IsDropDownEnabled = true;
    public bool HasFlagsAttribute;
    public Comparison<string> SortComparison;
    public Action<Window, List<string>, string> PopulateList;
    private Window arrowWin;
    private ListBox listbox;
    private int lbHeight;
    private List<string> list;

    public event Window.WindowHandler DropDownHandler;

    internal void RaiseDropDownHandler(WindowEventArgs e)
    {
      if (this.DropDownHandler == null)
        return;
      this.DropDownHandler((object) this, e);
    }

    public override Window GetNavigable()
    {
      if (this.listbox == null)
        return (Window) this;
      return this.listbox.GetNavigable();
    }

    public DropDown()
      : base((string) null, 0, 0, 20, 10)
    {
      this.Initialize();
    }

    public DropDown(string text, int x, int y, int width, int height, int lbHeight)
      : this(text, x, y, width, height, lbHeight, 1f)
    {
    }

    public DropDown(
      string text,
      int x,
      int y,
      int width,
      int height,
      int lbHeight,
      float textScale)
      : this(text, x, y, width, height, lbHeight, textScale, TextBox.DefaultTextAlignX, WinTextAlignY.Center)
    {
    }

    public DropDown(
      string text,
      int x,
      int y,
      int width,
      int height,
      int lbHeight,
      float textScale,
      WinTextAlignX alignX,
      WinTextAlignY alignY)
      : base(text, x, y, width, height, textScale, alignX, alignY)
    {
      this.lbHeight = lbHeight;
      this.Initialize();
    }

    private void Initialize()
    {
      this.ClearFlags(Window.WinFlags.LeftClickOnPress);
      this.GetNewInputHandler = new GetTextInputHander(((DataField) this).EmptyInputHander);
      int height = this.Size.Y - 6;
      int width = height % 2 == 1 ? height : height + 1;
      this.arrowWin = new Window((string) null, this.Size.X - 3 - width, 3, width, height)
      {
        BorderThickness = 1
      };
      this.arrowWin.ClickHandler += new Window.WindowHandler(this.OnArrowClick);
      this.arrowWin.Colors = DropDown.DropDownArrowColorProfile;
      this.arrowWin.LoadTexture(InputManager.KeysTexture, false, false, 1f);
      this.arrowWin.Texture.SrRect = new Rectangle?(new Rectangle(126, 0, 7, 8));
      this.arrowWin.Texture.DestRect = new Rectangle?(new Rectangle((this.arrowWin.Size.X - 7) / 2, (this.arrowWin.Size.Y - 8) / 2, 7, 8));
      this.arrowWin.Texture.TintColor = new Color(100, 100, 100, (int) byte.MaxValue);
      this.AddChild((Node) this.arrowWin);
    }

    protected override bool EqualsInputWindowCore(Window win)
    {
      if (win != this && win != this.listbox)
        return win == this.arrowWin;
      return true;
    }

    protected override bool OnKeyReleaseCore(WindowEventArgs e, Keys[] keys)
    {
      return false;
    }

    protected override void OnEndInputCore()
    {
      base.OnEndInputCore();
      this.CloseDropDown();
    }

    private void OnArrowClick(object sender, WindowEventArgs e)
    {
      ListBox listbox = this.listbox;
      ((ITextInputWindow) this).EndInput(false);
      if (listbox == null)
        this.ToggleOpenCloseDropDown(e.WindowManager);
      else
        e.WindowManager.SetNavigable((Window) this);
    }

    protected override void OnClickCore(WindowEventArgs e)
    {
      if (this.GetNewInputHandler == null || this.GetNewInputHandler == new GetTextInputHander(((DataField) this).EmptyInputHander))
        this.ToggleOpenCloseDropDown(e.WindowManager);
      else
        this.CloseDropDown();
    }

    private void ToggleOpenCloseDropDown(WindowManager windowManager)
    {
      if (this.listbox == null)
        this.OpenDropDownCore();
      else
        this.CloseDropDown();
      windowManager.SetNavigable(this.listbox == null ? (Window) this : (Window) this.listbox);
      windowManager.SetInputWindow(this.listbox != null ? (ITextInputWindow) this : (ITextInputWindow) null);
    }

    private void CloseDropDown()
    {
      if (this.listbox == null)
        return;
      this.RaiseDropDownHandler(new WindowEventArgs((WindowManager) null, (Window) this.listbox, (Window) this, Point.Zero, false));
      this.Parent.RemoveChild((Node) this.listbox);
      this.listbox = (ListBox) null;
    }

    private void OpenDropDownCore()
    {
      if (!this.IsEnabled || this.PopulateList == null)
        return;
      if (this.list == null)
        this.list = new List<string>();
      this.PopulateList((Window) this, this.list, (string) null);
      ListBox listBox = new ListBox((string) null, (int) this.Position.X, (int) this.Position.Y + this.Size.Y + 1, this.Size.X, this.lbHeight);
      listBox.TextAlignX = this.TextAlignX;
      listBox.TextScale = this.TextScale;
      listBox.BorderThickness = 1;
      listBox.Tag = this.Tag;
      listBox.SortComparison = this.SortComparison;
      this.listbox = listBox;
      this.listbox.IsEnabled = this.IsDropDownEnabled;
      if (this.HasFlag(Window.WinFlags.KeepItemsSorted))
        this.listbox.AddFlags(Window.WinFlags.KeepItemsSorted);
      this.listbox.ItemSelectedHandler += new Window.WindowHandler(this.OnListClick);
      this.listbox.EscapePressedHandler += new Window.WindowHandler(this.OnListEscape);
      this.listbox.AddFlags(Window.WinFlags.DeferDraw | Window.WinFlags.DynamicHeight);
      this.listbox.AddRange((IEnumerable<string>) this.list);
      this.listbox.SetKeyNavigation(this.GetNewInputHandler == new GetTextInputHander(((DataField) this).EmptyInputHander));
      this.Parent.AddChild((Node) this.listbox);
      this.RaiseDropDownHandler(new WindowEventArgs((WindowManager) null, (Window) this.listbox, (Window) this, Point.Zero, false));
    }

    public void OnListClick(object sender, WindowEventArgs e)
    {
      string str = e.KeyboardRaised ? this.listbox.KeyHighlightedItem : this.listbox.MouseHighlightedItem;
      if (str != null)
      {
        if (this.HasFlagsAttribute && this.Text.IsNotEmpty() && this.Text != "None")
        {
          DropDown dropDown = this;
          dropDown.Text = dropDown.Text + ", " + str;
        }
        else
          this.Text = str;
        if (this.validateInput != null)
          this.validateInput((ITextInputWindow) this);
      }
      this.ToggleOpenCloseDropDown(e.WindowManager);
    }

    public void OnListEscape(object sender, WindowEventArgs e)
    {
      this.ToggleOpenCloseDropDown(e.WindowManager);
    }
  }
}
