// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.ListBox
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.GUI
{
  public class ListBox : TextBox
  {
    public static ListBox.ColorProfile DefaultColorProfile;
    public Comparison<string> SortComparison;
    protected float indexAtTop;
    protected int localMouseIndex;
    protected int localKeyIndex;
    protected List<string> fullList;
    protected List<string> drawList;
    protected string filter;
    protected string keyText;
    protected int maxHeight;
    private bool disableKeyNavigation;

    protected override Window.ColorProfile InitColorProfile()
    {
      return (Window.ColorProfile) ListBox.DefaultColorProfile;
    }

    public event Window.WindowHandler ItemSelectedHandler;

    public event Window.WindowHandler EscapePressedHandler;

    internal void RaiseItemSelected(WindowEventArgs e)
    {
      if (this.ItemSelectedHandler == null)
        return;
      this.ItemSelectedHandler((object) this, e);
    }

    internal void RaiseEscapePressed(WindowEventArgs e)
    {
      if (this.EscapePressedHandler == null)
        return;
      this.EscapePressedHandler((object) this, e);
    }

    public Window DragProxyWin { get; set; }

    protected int mouseIndex
    {
      get
      {
        return (int) this.indexAtTop + this.localMouseIndex;
      }
    }

    protected int keyIndex
    {
      get
      {
        return (int) this.indexAtTop + this.localKeyIndex;
      }
    }

    public override bool IsKeyNavigable
    {
      get
      {
        if (this.IsEnabled && this.IsVisible)
          return !this.disableKeyNavigation;
        return false;
      }
    }

    public int ItemCount
    {
      get
      {
        return this.fullList.Count;
      }
    }

    public int Spacing
    {
      get
      {
        return (int) (3.0 + (double) this.TextScale * (this.Font != null ? (double) this.Font.MeasureString("A").Y : 1.0));
      }
    }

    public string MouseHighlightedItem
    {
      get
      {
        if (this.mouseIndex >= this.drawList.Count)
          return (string) null;
        return this.drawList[this.mouseIndex];
      }
    }

    public string KeyHighlightedItem
    {
      get
      {
        if (this.keyIndex >= this.drawList.Count)
          return (string) null;
        return this.drawList[this.keyIndex];
      }
    }

    public bool Contains(string i)
    {
      return this.fullList.Contains(i);
    }

    public override Window GetDragProxy(WindowDragEventArgs e)
    {
      if (this.DragProxyWin != null)
        return this.DragProxyWin;
      return (Window) this;
    }

    public void ClearItems()
    {
      this.fullList.Clear();
      this.drawList.Clear();
      this.SetIndex(0);
    }

    public void SetIndex(int index)
    {
      if (this.drawList.Count < 1)
        index = 0;
      if (index >= this.drawList.Count)
        index = this.drawList.Count - 1;
      if (index < 0)
        index = 0;
      this.indexAtTop = 0.0f;
      this.localMouseIndex = this.localKeyIndex = index;
      this.Text = this.drawList.Count > this.mouseIndex ? this.drawList[this.mouseIndex] : "";
      this.keyText = this.drawList.Count > this.keyIndex ? this.drawList[this.keyIndex] : "";
    }

    public void SetKeyNavigation(bool enable)
    {
      this.disableKeyNavigation = !enable;
    }

    private void SetKeyIndex(int index)
    {
      if (this.drawList.Count < 1)
        index = 0;
      index = MyMathHelper.Clamp(index, 0, Math.Max(0, this.drawList.Count - 1));
      if (index == this.keyIndex)
        return;
      if ((double) index < (double) this.indexAtTop)
      {
        this.indexAtTop = (float) index;
      }
      else
      {
        int num = this.Size.Y / this.Spacing;
        if (index >= this.drawList.Count)
        {
          this.indexAtTop = (float) (this.drawList.Count - num);
          this.localKeyIndex = index - (int) this.indexAtTop;
        }
        else if ((double) index - (double) this.indexAtTop >= (double) num)
          this.indexAtTop = (float) (index - num + 1);
      }
      this.localKeyIndex = index - (int) this.indexAtTop;
      this.keyText = this.drawList.Count > this.keyIndex ? this.drawList[this.keyIndex] : "";
    }

    public ListBox(string name, int x, int y, int width, int maxHeight)
      : base(name, x, y, width, 0)
    {
      this.maxHeight = maxHeight;
      this.AddFlags(Window.WinFlags.HideNavBorder | Window.WinFlags.OwnsMouseWheel | Window.WinFlags.FilteringEnabled);
      this.drawList = new List<string>();
      this.fullList = new List<string>();
      this.filter = "";
    }

    protected void RecalculateHeight()
    {
      if (!this.HasFlag(Window.WinFlags.DynamicHeight))
        return;
      int spacing = this.Spacing;
      int num1 = this.maxHeight - this.maxHeight % spacing;
      if (num1 == 0)
        num1 = spacing;
      int num2 = spacing * this.drawList.Count;
      if (num2 > num1)
        num2 = num1;
      this.Size.Y = num2;
    }

    public void AddItem(string s)
    {
      this.fullList.Add(s);
      if (s.StartsWith(this.filter, StringComparison.OrdinalIgnoreCase))
        this.drawList.Add(s);
      this.SortItemsCore();
      this.RecalculateHeight();
    }

    public void AddRange(IEnumerable<string> s)
    {
      this.fullList.AddRange(s);
      if (this.filter.Length < 1)
      {
        this.drawList.AddRange(s);
      }
      else
      {
        foreach (string str in s)
        {
          if (str.StartsWith(this.filter, StringComparison.OrdinalIgnoreCase))
            this.drawList.Add(str);
        }
      }
      this.SortItemsCore();
      this.RecalculateHeight();
    }

    public void RemoveItem()
    {
      string text = this.Text;
      if (this.mouseIndex < 0 || this.mouseIndex >= this.drawList.Count)
        return;
      this.drawList.RemoveAt(this.mouseIndex);
      this.fullList.Remove(text);
    }

    public void SortItems()
    {
      this.SortItemsCore((Comparison<string>) null);
    }

    public void SortItems(Comparison<string> compare)
    {
      this.SortItemsCore(compare);
    }

    protected void SortItemsCore()
    {
      if (!this.HasFlag(Window.WinFlags.KeepItemsSorted))
        return;
      this.SortItemsCore(this.SortComparison);
    }

    private void SortItemsCore(Comparison<string> compare)
    {
      if (compare != null)
      {
        this.fullList.Sort(compare);
        this.drawList.Sort(compare);
      }
      else
      {
        this.fullList.Sort();
        this.drawList.Sort();
      }
    }

    protected override bool HandleInputCore(WindowEventArgs e)
    {
      float y = InputManager.GetGamepadRightStick(e.PlayerIndex).Y;
      if ((double) y != 0.0)
      {
        this.indexAtTop -= y * 0.3f;
        if ((double) this.indexAtTop > (double) (this.drawList.Count - 1))
          this.indexAtTop = (float) (this.drawList.Count - 1);
        if ((double) this.indexAtTop < 0.0)
          this.indexAtTop = 0.0f;
      }
      return base.HandleInputCore(e);
    }

    protected override void OnHoverCore(WindowEventArgs args)
    {
      float num = (float) this.Spacing * this.WorldScale;
      this.localMouseIndex = Math.Max(0, (double) num > 0.0 ? (int) ((double) args.MousePosition.Y / (double) num) : 0);
      if (this.localMouseIndex < 0)
        this.localMouseIndex = 0;
      int mouseIndex = this.mouseIndex;
      this.Text = this.drawList.Count > mouseIndex ? this.drawList[mouseIndex] : "";
    }

    protected override void OnClickCore(WindowEventArgs args)
    {
      if (this.mouseIndex >= this.drawList.Count)
        return;
      this.RaiseItemSelected(args);
    }

    protected override bool OnKeyPressCore(WindowEventArgs e, Keys[] keys)
    {
      switch (keys[0])
      {
        case Keys.Back:
          if (this.filter.Length < 1)
            return true;
          this.filter = this.filter.Substring(0, this.filter.Length - 1);
          break;
        case Keys.End:
          this.SetKeyIndex(this.fullList.Count);
          return true;
        case Keys.Home:
          this.SetKeyIndex(0);
          return true;
        case Keys.Up:
          this.SetKeyIndex(this.keyIndex - 1);
          return true;
        case Keys.Down:
          this.SetKeyIndex(this.keyIndex + 1);
          return true;
        default:
          if (!this.HasFlag(Window.WinFlags.FilteringEnabled) || (keys[0] < Keys.A || keys[0] > Keys.Z) && (keys[0] < Keys.NumPad0 || keys[0] > Keys.NumPad9))
            return false;
          this.filter += (string) (object) keys[0];
          break;
      }
      this.drawList.Clear();
      if (this.filter.Length < 1)
      {
        this.drawList.AddRange((IEnumerable<string>) this.fullList);
      }
      else
      {
        foreach (string full in this.fullList)
        {
          if (full.StartsWith(this.filter, StringComparison.OrdinalIgnoreCase))
            this.drawList.Add(full);
        }
      }
      this.SetIndex(0);
      this.RecalculateHeight();
      return true;
    }

    protected override bool OnKeyReleaseCore(WindowEventArgs e, Keys[] keys)
    {
      switch (keys[0])
      {
        case Keys.Enter:
          this.Text = this.keyText;
          this.RaiseItemSelected(e);
          return true;
        case Keys.Escape:
          this.RaiseEscapePressed(e);
          return true;
        default:
          return false;
      }
    }

    protected override void OnMouseWheelDeltaCore(WindowEventArgs args, int delta)
    {
      this.indexAtTop -= (float) delta * 0.01f;
      if ((double) this.indexAtTop > (double) (this.drawList.Count - 1))
        this.indexAtTop = (float) (this.drawList.Count - 1);
      if ((double) this.indexAtTop >= 0.0)
        return;
      this.indexAtTop = 0.0f;
    }

    public override void Draw(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      bool isEnabled)
    {
      Vector2 zero = Vector2.Zero;
      zero.X = (float) (bound.X + 1);
      zero.Y = (float) (bound.Y + 1);
      ListBox.ColorProfile colors = (ListBox.ColorProfile) this.Colors;
      Color color1 = colors.TextColor * alpha;
      Color color2 = colors.ForeHighlightColor * alpha;
      int height = (int) ((double) this.Spacing * (double) scale);
      int num1 = this.localMouseIndex * height - 1;
      Rectangle rectangle = new Rectangle(bound.X, bound.Y + num1, bound.Width, height);
      int num2 = rectangle.Y + rectangle.Height - (bound.Y + bound.Height);
      if (num2 > 0)
        rectangle.Height -= num2;
      spriteBatch.Draw(CoreGlobals.BlankTexture, rectangle, colors.BackHighlightColor);
      if (this.IsKeyNavigable)
      {
        int num3 = this.localKeyIndex * height;
        rectangle.Y = bound.Y + num3;
        int num4 = rectangle.Y + rectangle.Height - (bound.Y + bound.Height);
        if (num4 > 0)
          rectangle.Height -= num4;
        spriteBatch.DrawBox(CoreGlobals.BlankTexture, rectangle, 2, colors.NavigableColor * alpha, 0.0f);
      }
      int indexAtTop = (int) this.indexAtTop;
      int mouseIndex = this.mouseIndex;
      int keyIndex = this.keyIndex;
      for (int index = indexAtTop; index < this.drawList.Count; ++index)
      {
        string draw = this.drawList[index];
        if (draw != null)
        {
          spriteBatch.DrawString(this.Font, draw, zero, index == mouseIndex || index == keyIndex ? color2 : color1, 0.0f, Vector2.Zero, this.TextScale * scale, SpriteEffects.None, 0.0f);
          zero.Y += (float) height;
          if ((double) zero.Y > (double) (bound.Y + bound.Height))
            break;
        }
      }
    }

    static ListBox()
    {
      ListBox.ColorProfile colorProfile = new ListBox.ColorProfile();
      colorProfile.BackDisabledColor = new Color(160, 160, 160);
      colorProfile.BackColor = new Color(192, 192, 192);
      colorProfile.BackHoverColor = new Color(192, 192, 192);
      colorProfile.BackClickColor = new Color(192, 192, 192);
      colorProfile.BorderColor = Color.Black;
      colorProfile.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile.TextColor = Color.Black;
      colorProfile.BackHighlightColor = Color.LightBlue;
      colorProfile.ForeHighlightColor = Color.Black;
      colorProfile.NavigableColor = WindowManager.NavigableColor;
      ListBox.DefaultColorProfile = colorProfile;
    }

    public class ColorProfile : TextBox.ColorProfile
    {
      public Color BackHighlightColor;
      public Color ForeHighlightColor;
      public Color NavigableColor;

      public override Window.ColorProfile Copy(Window.ColorProfile result)
      {
        ListBox.ColorProfile colorProfile = base.Copy(result) as ListBox.ColorProfile;
        if (colorProfile != null)
        {
          colorProfile.BackHighlightColor = this.BackHighlightColor;
          colorProfile.ForeHighlightColor = this.ForeHighlightColor;
          colorProfile.NavigableColor = this.NavigableColor;
        }
        return result;
      }
    }
  }
}
