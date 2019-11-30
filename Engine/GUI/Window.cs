// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.Window
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
  public class Window : Node
  {
    public static Window.ColorProfile DefaultColorProfile = new Window.ColorProfile()
    {
      ForeColor = Color.White,
      BackColor = new Color(192, 192, 192),
      BackHoverColor = new Color(224, 224, 224),
      BackClickColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue),
      BackDisabledColor = new Color(160, 160, 160),
      BorderColor = Color.Black
    };
    public static Window.ColorProfile TransparentColorProfile = new Window.ColorProfile()
    {
      ForeColor = Color.Transparent,
      BackColor = Color.Transparent,
      BackHoverColor = Color.Transparent,
      BackClickColor = Color.Transparent,
      BackDisabledColor = Color.Transparent,
      BorderColor = Color.Transparent
    };
    public static float DefaultDragEnablePressTime = 0.35f;
    public static bool DefaultBorderRounded = false;
    public Window.ColorProfile Colors;
    public bool IsVisible;
    public Vector2 Position;
    public Point Size;
    public float Scale;
    public float Alpha;
    public float DragEnablePressTime;
    public int BorderThickness;
    public TextureData Texture;
    public string Name;
    public object Tag;
    public ToolTip ToolTip;
    public RenderProfile RenderProfile;
    public Func<bool> IsEnabledFunc;
    internal Window.WinFlags Flags;
    private bool isEnabled;

    public event Window.WindowHandler ClickHandler;

    public event Window.WindowHandler ClickDownHandler;

    public event Window.WindowHandler RightClickHandler;

    public event Window.WindowHandler RightClickDownHandler;

    public event Window.WindowHandler MiddleClickHandler;

    public event Window.WindowHandler MiddleClickDownHandler;

    public event Window.WindowHandler HoverStartHandler;

    public event Window.WindowHandler HoverHandler;

    public event Window.WindowHandler HoverEndHandler;

    public event Window.WindowDragHandler DragStartHandler;

    public event Window.WindowDragHandler DragHandler;

    public event Window.WindowDragHandler DragEndHandler;

    internal void RaiseClickHandler(WindowEventArgs e)
    {
      if (this.ClickHandler == null)
        return;
      this.ClickHandler((object) this, e);
    }

    internal void RaiseClickDownHandler(WindowEventArgs e)
    {
      if (this.ClickDownHandler == null)
        return;
      this.ClickDownHandler((object) this, e);
    }

    internal void RaiseRightClickHandler(WindowEventArgs e)
    {
      if (this.RightClickHandler == null)
        return;
      this.RightClickHandler((object) this, e);
    }

    internal void RaiseRightDownClickHandler(WindowEventArgs e)
    {
      if (this.RightClickDownHandler == null)
        return;
      this.RightClickDownHandler((object) this, e);
    }

    internal void RaiseMiddleClickHandler(WindowEventArgs e)
    {
      if (this.MiddleClickHandler == null)
        return;
      this.MiddleClickHandler((object) this, e);
    }

    internal void RaiseMiddleDownClickHandler(WindowEventArgs e)
    {
      if (this.MiddleClickDownHandler == null)
        return;
      this.MiddleClickDownHandler((object) this, e);
    }

    internal void RaiseHoverStartHandler(WindowEventArgs e)
    {
      if (this.HoverStartHandler == null)
        return;
      this.HoverStartHandler((object) this, e);
    }

    internal void RaiseHoverHandler(WindowEventArgs e)
    {
      if (this.HoverHandler == null)
        return;
      this.HoverHandler((object) this, e);
    }

    internal void RaiseHoverEndHandler(WindowEventArgs e)
    {
      if (this.HoverEndHandler == null)
        return;
      this.HoverEndHandler((object) this, e);
    }

    internal void RaiseDragStartHandler(WindowDragEventArgs e)
    {
      if (this.DragStartHandler == null)
        return;
      this.DragStartHandler((object) this, e);
    }

    internal void RaiseDragHandler(WindowDragEventArgs e)
    {
      if (this.DragHandler == null)
        return;
      this.DragHandler((object) this, e);
    }

    internal void RaiseDragEndHandler(WindowDragEventArgs e)
    {
      if (this.DragEndHandler == null)
        return;
      this.DragEndHandler((object) this, e);
    }

    protected virtual Window.ColorProfile InitColorProfile()
    {
      return Window.DefaultColorProfile;
    }

    public virtual Point SizeScaled(float worldScale)
    {
      return new Point((int) (((double) this.Size.X + 0.5) * (double) worldScale * (double) this.Scale), (int) (((double) this.Size.Y + 0.5) * (double) worldScale * (double) this.Scale));
    }

    public Vector2 WorldPosition
    {
      get
      {
        Vector2 vector2 = this.Position;
        Window parent = this.parent as Window;
        for (; parent != null; parent = parent.parent as Window)
        {
          vector2 = vector2 * parent.Scale + parent.Position;
          Canvas canvas = parent as Canvas;
          if (canvas != null)
            vector2 += canvas.Offset;
        }
        return vector2;
      }
    }

    public float WorldScale
    {
      get
      {
        float scale = this.Scale;
        for (Window parent = this.parent as Window; parent != null; parent = parent.parent as Window)
          scale *= parent.Scale;
        return scale;
      }
    }

    public bool IsEnabled
    {
      get
      {
        if (this.IsEnabledFunc != null)
          return this.IsEnabledFunc();
        return this.isEnabled;
      }
      set
      {
        this.isEnabled = value;
      }
    }

    public bool IsWorldEnabled
    {
      get
      {
        bool isEnabled = this.IsEnabled;
        for (Window parent = this.parent as Window; parent != null && isEnabled; parent = parent.parent as Window)
          isEnabled = parent.IsEnabled;
        return isEnabled;
      }
    }

    public bool HasHoverOrClickHandler
    {
      get
      {
        if (this.HoverHandler == null && this.HoverStartHandler == null && (this.HoverEndHandler == null && !this.HasLeftClickHandler) && !this.HasRightClickHandler)
          return this.HasMiddleClickHandler;
        return true;
      }
    }

    public bool HasLeftClickHandler
    {
      get
      {
        if (this.ClickHandler == null)
          return this.ClickDownHandler != null;
        return true;
      }
    }

    public bool HasRightClickHandler
    {
      get
      {
        if (this.RightClickHandler == null)
          return this.RightClickDownHandler != null;
        return true;
      }
    }

    public bool HasMiddleClickHandler
    {
      get
      {
        if (this.MiddleClickHandler == null)
          return this.MiddleClickDownHandler != null;
        return true;
      }
    }

    public Rectangle WinRect
    {
      get
      {
        return new Rectangle((int) this.Position.X, (int) this.Position.X, this.Size.X, this.Size.Y);
      }
    }

    public virtual bool IsKeyNavigable
    {
      get
      {
        if (this.IsEnabled && this.IsVisible)
          return this.ClickHandler != null;
        return false;
      }
    }

    public virtual Window GetNavigable()
    {
      return this;
    }

    public virtual Window GetDragProxy(WindowDragEventArgs e)
    {
      return this;
    }

    public Window()
      : this(0, 0, 10, 10)
    {
    }

    public Window(int x, int y, int width, int height)
      : this((string) null, x, y, width, height)
    {
    }

    public Window(string name, int x, int y, int width, int height)
    {
      this.Name = name;
      this.isEnabled = true;
      this.IsVisible = true;
      this.Scale = 1f;
      this.Alpha = 1f;
      this.Position = new Vector2((float) x, (float) y);
      this.Size = new Point(width, height);
      this.DragEnablePressTime = Window.DefaultDragEnablePressTime;
      if (Window.DefaultBorderRounded)
        this.AddFlags(Window.WinFlags.BorderRounded);
      this.Colors = this.InitColorProfile();
    }

    public Window(Window win)
    {
      this.isEnabled = win.isEnabled;
      this.IsEnabledFunc = win.IsEnabledFunc;
      this.IsVisible = win.IsVisible;
      this.Position = win.Position;
      this.Size = win.Size;
      this.Scale = win.Scale;
      this.Alpha = win.Alpha;
      this.DragEnablePressTime = win.DragEnablePressTime;
      this.BorderThickness = win.BorderThickness;
      this.Texture = win.Texture;
      this.Name = win.Name;
      this.Tag = win.Tag;
      this.ToolTip = win.ToolTip;
      this.RenderProfile = win.RenderProfile;
      this.Colors = win.Colors;
      this.Flags = win.Flags;
    }

    public bool HasFlag(Window.WinFlags flags)
    {
      return (this.Flags & flags) == flags;
    }

    public bool HasFlagAny(Window.WinFlags flags)
    {
      return (this.Flags & flags) > Window.WinFlags.None;
    }

    public void AddFlags(Window.WinFlags flags)
    {
      this.Flags |= flags;
    }

    public void ClearFlags()
    {
      this.Flags = Window.WinFlags.None;
    }

    public void ClearFlags(Window.WinFlags flags)
    {
      this.Flags &= ~flags;
    }

    protected virtual bool EqualsInputWindowCore(Window win)
    {
      return win == this;
    }

    public Window FindChild(string name)
    {
      if (name == null || name.Length < 1)
        return (Window) null;
      for (Window window = this.firstChild as Window; window != null; window = window.nextSibling as Window)
      {
        if (window.Name != null && string.Equals(window.Name, name, StringComparison.OrdinalIgnoreCase))
          return window;
      }
      return (Window) null;
    }

    public void AdjustSizeToContainAllChildren()
    {
      this.AdjustSizeToContainAllChildrenCore(this, Point.Zero);
    }

    public void AdjustSizeToContainAllChildren(Point min)
    {
      this.AdjustSizeToContainAllChildrenCore(this, min);
    }

    public void AdjustSizeToContainAllChildren(Rectangle min)
    {
      this.AdjustSizeToContainAllChildrenCore(this, new Point(min.Width, min.Height));
    }

    public void AdjustSizeToContainAllChildrenDeep()
    {
      this.AdjustSizeToContainAllChildrenDeep((Func<Window, Point?>) null);
    }

    public void AdjustSizeToContainAllChildrenDeep(Func<Window, Point?> test)
    {
      List<Node> nodeList = new List<Node>();
      Node node = this.firstChild;
      while (node != null)
      {
        if (node.HasChildren && !nodeList.Contains(node))
        {
          node = node.FirstChild;
        }
        else
        {
          Node parent = node.Parent;
          if (parent == null)
            break;
          Point? nullable = test == null ? new Point?(Point.Zero) : test(parent as Window);
          if (nullable.HasValue)
            this.AdjustSizeToContainAllChildrenCore(parent as Window, nullable.Value);
          if (parent == this)
            break;
          nodeList.Add(parent);
          node = parent.NextSibling == null ? parent.Parent : parent.NextSibling;
        }
      }
    }

    private void AdjustSizeToContainAllChildrenCore(Window win, Point min)
    {
      if (win == null || !win.IsVisible)
        return;
      win.Size.X = min.X;
      win.Size.Y = min.Y;
      for (Window window = win.firstChild as Window; window != null; window = window.nextSibling as Window)
      {
        if (window.IsVisible)
        {
          int num1 = (int) ((double) window.Position.X + (double) window.Size.X);
          if (num1 > win.Size.X)
            win.Size.X = num1;
          int num2 = (int) ((double) window.Position.Y + (double) window.Size.Y);
          if (num2 > win.Size.Y)
            win.Size.Y = num2;
        }
      }
    }

    public void LoadTexture(string asset)
    {
      this.LoadTexture(asset, false, false, 1f);
    }

    public void LoadTexture(string asset, bool isBackground)
    {
      this.LoadTexture(asset, isBackground, true, 1f);
    }

    public void LoadTexture(string asset, bool isBackground, bool stretchToFit, float scale)
    {
      if (asset.IsNotEmpty())
      {
        this.Texture = new TextureData()
        {
          Texture = CoreGlobals.Content.Load<Texture2D>(asset),
          IsBackground = isBackground,
          StretchToFit = stretchToFit,
          Scale = scale
        };
        this.ResetBackgroundTexture();
      }
      else
        this.Texture = (TextureData) null;
    }

    public void LoadTexture(Texture2D texture)
    {
      this.LoadTexture(texture, false, false, 1f);
    }

    public void LoadTexture(Texture2D texture, bool isBackground)
    {
      this.LoadTexture(texture, isBackground, true, 1f);
    }

    public void LoadTexture(Texture2D texture, bool isBackground, bool toFit, float scale)
    {
      if (texture != null)
      {
        this.Texture = new TextureData()
        {
          Texture = texture,
          IsBackground = isBackground,
          StretchToFit = toFit,
          Scale = scale
        };
        this.ResetBackgroundTexture();
      }
      else
        this.Texture = (TextureData) null;
    }

    private void ResetBackgroundTexture()
    {
      if (this.Texture == null || !this.Texture.IsBackground || this.Texture.StretchToFit)
        return;
      this.Texture.DestRect = new Rectangle?(new Rectangle((int) ((double) this.Size.X * 0.5 - (double) this.Texture.Texture.Width * 0.5), (int) ((double) this.Size.Y * 0.5 - (double) this.Texture.Texture.Height * 0.5), this.Texture.Texture.Width, this.Texture.Texture.Height));
    }

    public bool HandleInput(WindowEventArgs e)
    {
      return this.HandleInputCore(e);
    }

    protected virtual bool HandleInputCore(WindowEventArgs e)
    {
      return false;
    }

    public void OnHover(WindowEventArgs e)
    {
      this.OnHoverCore(e);
    }

    protected virtual void OnHoverCore(WindowEventArgs e)
    {
    }

    public void OnClick(WindowEventArgs e)
    {
      this.OnClickCore(e);
    }

    protected virtual void OnClickCore(WindowEventArgs e)
    {
    }

    public void OnClickDown(WindowEventArgs e)
    {
      this.OnClickDownCore(e);
    }

    protected virtual void OnClickDownCore(WindowEventArgs e)
    {
    }

    public void OnRightClick(WindowEventArgs e)
    {
      this.OnRightClickCore(e);
    }

    protected virtual void OnRightClickCore(WindowEventArgs e)
    {
    }

    public void OnRightClickDown(WindowEventArgs e)
    {
      this.OnRightClickDownCore(e);
    }

    protected virtual void OnRightClickDownCore(WindowEventArgs e)
    {
    }

    public void OnMiddleClick(WindowEventArgs e)
    {
      this.OnMiddleClickCore(e);
    }

    protected virtual void OnMiddleClickCore(WindowEventArgs e)
    {
    }

    public void OnMiddleClickDown(WindowEventArgs e)
    {
      this.OnMiddleClickDownCore(e);
    }

    protected virtual void OnMiddleClickDownCore(WindowEventArgs e)
    {
    }

    public void OnMouseWheelDelta(WindowEventArgs e, int delta)
    {
      this.OnMouseWheelDeltaCore(e, delta);
    }

    protected virtual void OnMouseWheelDeltaCore(WindowEventArgs e, int delta)
    {
    }

    public bool OnKeyPress(WindowEventArgs e, Keys[] keys)
    {
      return this.OnKeyPressCore(e, keys);
    }

    protected virtual bool OnKeyPressCore(WindowEventArgs e, Keys[] keys)
    {
      return false;
    }

    public bool OnKeyRelease(WindowEventArgs e, Keys[] keys)
    {
      return this.OnKeyReleaseCore(e, keys);
    }

    protected virtual bool OnKeyReleaseCore(WindowEventArgs e, Keys[] keys)
    {
      return false;
    }

    public void SetToolTip(string tip)
    {
      this.SetToolTip(tip, ToolTip.DefaultTooltipDelay);
    }

    public void SetToolTip(string tip, float delay)
    {
      if (this.ToolTip == null)
      {
        this.ToolTip = new ToolTip(CoreGlobals.GameFont)
        {
          Text = tip,
          Delay = delay
        };
      }
      else
      {
        this.ToolTip.Text = tip;
        this.ToolTip.Delay = delay;
      }
    }

    public void EnableToolTips(bool enable)
    {
      this.EnableToolTipsCore(this, enable);
    }

    private void EnableToolTipsCore(Window win, bool enable)
    {
      if (win.ToolTip != null)
        win.ToolTip.IsEnabled = enable;
      for (Window win1 = win.firstChild as Window; win1 != null; win1 = win1.nextSibling as Window)
        this.EnableToolTipsCore(win1, enable);
    }

    public virtual Color GetBackColorOverride(Color color)
    {
      return color;
    }

    public virtual void Draw(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      bool isEnabled)
    {
    }

    public virtual void DrawPreChild()
    {
    }

    public virtual void DrawingChild(Rectangle bound, float scale)
    {
    }

    public virtual void DrawBackground(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      Color backColor)
    {
      if (backColor.A > (byte) 0)
        spriteBatch.Draw(CoreGlobals.BlankTexture, bound, backColor * alpha);
      if (this.Texture == null || this.Texture.Texture == null)
        return;
      if (this.Texture.StretchToFit)
        spriteBatch.Draw(this.Texture.Texture, bound, this.Texture.SrRect, this.Texture.TintColor * alpha, this.Texture.Rotation, this.Texture.Origin, SpriteEffects.None, 0.0f);
      else if (this.Texture.DestRect.HasValue)
      {
        Rectangle destinationRectangle = this.Texture.DestRect.Value;
        destinationRectangle.X = (int) ((double) destinationRectangle.X * (double) scale + (double) bound.X);
        destinationRectangle.Y = (int) ((double) destinationRectangle.Y * (double) scale + (double) bound.Y);
        destinationRectangle.Width = (int) ((double) destinationRectangle.Width * (double) scale);
        destinationRectangle.Height = (int) ((double) destinationRectangle.Height * (double) scale);
        spriteBatch.Draw(this.Texture.Texture, destinationRectangle, this.Texture.SrRect, this.Texture.TintColor * alpha, this.Texture.Rotation, this.Texture.Origin, SpriteEffects.None, 0.0f);
      }
      else
      {
        Vector2 position = this.Texture.Position;
        position.X += (float) bound.X;
        position.Y += (float) bound.Y;
        spriteBatch.Draw(this.Texture.Texture, position, this.Texture.SrRect, this.Texture.TintColor * alpha, this.Texture.Rotation, this.Texture.Origin, this.Texture.Scale, SpriteEffects.None, 0.0f);
      }
    }

    public void DrawBorder(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      float z)
    {
      Color color = this.Colors.BorderColor * alpha;
      if (this.BorderThickness <= 0 || color.A <= (byte) 0)
        return;
      int borderThickness = this.BorderThickness;
      int num = borderThickness * 2;
      if (!this.HasFlag(Window.WinFlags.BorderRounded))
      {
        bound.Expand(borderThickness);
        spriteBatch.DrawBox(CoreGlobals.BlankTexture, bound, this.BorderThickness, color, z);
      }
      else
      {
        Rectangle destinationRectangle1 = new Rectangle(bound.X - borderThickness, bound.Y + borderThickness, borderThickness, bound.Height - num);
        Rectangle destinationRectangle2 = new Rectangle(bound.X + borderThickness, bound.Y - borderThickness, bound.Width - num, borderThickness);
        Rectangle destinationRectangle3 = new Rectangle(bound.X + bound.Width, bound.Y + borderThickness, borderThickness, bound.Height - num);
        Rectangle destinationRectangle4 = new Rectangle(bound.X + borderThickness, bound.Y + bound.Height, bound.Width - num, borderThickness);
        spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle1, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
        spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle2, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
        spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle3, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
        spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle4, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
        spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(bound.X, bound.Y, borderThickness, borderThickness), new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
        spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(bound.X + bound.Width - borderThickness, bound.Y, borderThickness, borderThickness), new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
        spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(bound.X, bound.Y + bound.Height - borderThickness, borderThickness, borderThickness), new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
        spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(bound.X + bound.Width - borderThickness, bound.Y + bound.Height - borderThickness, borderThickness, borderThickness), new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
      }
    }

    [System.Flags]
    public enum WinFlags : ushort
    {
      None = 0,
      IsDragable = 1,
      ClipChildren = 2,
      BorderRounded = 4,
      UseHoverColorIfDraggedOver = 8,
      DeferDraw = 16, // 0x0010
      LeftClickOnPress = 32, // 0x0020
      HideNavBorder = 64, // 0x0040
      IsNotHoverable = 128, // 0x0080
      OwnsMouseWheel = 256, // 0x0100
      DragCopy = 512, // 0x0200
      DragRightButton = 1024, // 0x0400
      KeepFocus = 2048, // 0x0800
      KeepItemsSorted = 4096, // 0x1000
      DynamicHeight = 8192, // 0x2000
      FilteringEnabled = 16384, // 0x4000
      TrapMouse = 32768, // 0x8000
    }

    public delegate void WindowHandler(object sender, WindowEventArgs args);

    public delegate void WindowEnabledHandler(object sender, bool enabled);

    public delegate void WindowDragHandler(object sender, WindowDragEventArgs args);

    public class ColorProfile
    {
      public Color ForeColor;
      public Color BackColor;
      public Color BorderColor;
      public Color BackHoverColor;
      public Color BackClickColor;
      public Color BackDisabledColor;

      public virtual Window.ColorProfile Copy(Window.ColorProfile result)
      {
        result.ForeColor = this.ForeColor;
        result.BackColor = this.BackColor;
        result.BorderColor = this.BorderColor;
        result.BackHoverColor = this.BackHoverColor;
        result.BackClickColor = this.BackClickColor;
        result.BackDisabledColor = this.BackDisabledColor;
        return result;
      }
    }
  }
}
