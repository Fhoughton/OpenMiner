// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.Canvas
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;

namespace StudioForge.Engine.GUI
{
  public class Canvas : Window
  {
    public float MinScale = 0.2f;
    public float MaxScale = 4f;
    public bool SlidingScroll = true;
    public ushort ResetScaleInput = ushort.MaxValue;
    public Vector2 Offset;
    public Point OffsetMin;
    public Point OffsetMax;
    public Vector4 InnerBound;
    public Vector4 ChildBound;
    public bool PanUsesViewportEdge;
    protected float cursorOnEdgeTimer;

    public event Window.WindowHandler ZoomHandler;

    protected void RaiseZoomHandler(WindowEventArgs e)
    {
      if (this.ZoomHandler == null)
        return;
      this.ZoomHandler((object) this, e);
    }

    public override Point SizeScaled(float worldScale)
    {
      return new Point((int) (((double) this.Size.X + 0.5) * (double) worldScale), (int) (((double) this.Size.Y + 0.5) * (double) worldScale));
    }

    public Canvas()
      : this((string) null, 0, 0, 10, 10)
    {
    }

    public Canvas(string name, int x, int y, int width, int height)
      : base(name, x, y, width, height)
    {
      this.Scale = 1f;
      this.ChildBound = new Vector4((float) x, (float) y, (float) (x + width), (float) (y + height));
      this.OffsetMin = new Point(-100, -100);
      this.OffsetMax = new Point(100, 100);
    }

    protected override bool HandleInputCore(WindowEventArgs e)
    {
      if (this.ResetScaleInput < (ushort) 0 || !InputManager.IsInputReleasedNew(PlayerIndex.One, this.ResetScaleInput))
        return base.HandleInputCore(e);
      if ((double) this.Scale != 1.0)
      {
        this.Scale = 1f;
        this.RaiseZoomHandler(e);
      }
      return true;
    }

    protected override void OnHoverCore(WindowEventArgs e)
    {
      if (!this.SlidingScroll)
        return;
      bool flag = false;
      if (this == e.Hovered || e.Hovered == null || !e.Hovered.HasFlagAny(Window.WinFlags.OwnsMouseWheel))
      {
        int num1 = (int) MathHelper.Clamp((float) InputManager.GetMouseWheelDelta(e.PlayerIndex), -500f, 500f);
        if (num1 == 0)
          num1 = (int) ((double) InputManager.GetGamepadRightStick(e.PlayerIndex).Y * 25.0);
        if (num1 != 0)
        {
          float num2 = 0.0004f * this.Scale;
          float scale = this.Scale;
          this.Scale = MathHelper.Clamp(this.Scale + (float) num1 * num2, this.MinScale, this.MaxScale);
          flag = (double) this.Scale != (double) scale;
        }
      }
      this.SetOffsetForMousePos(e.MousePosition);
      if (!flag)
        return;
      this.RaiseZoomHandler(e);
    }

    public override void DrawPreChild()
    {
      this.ChildBound = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
    }

    public override void DrawingChild(Rectangle bound, float scale)
    {
      if ((double) this.ChildBound.X > (double) bound.X)
        this.ChildBound.X = (float) bound.X;
      if ((double) this.ChildBound.Y > (double) bound.Y)
        this.ChildBound.Y = (float) bound.Y;
      if ((double) this.ChildBound.Z < (double) (bound.X + bound.Width))
        this.ChildBound.Z = (float) (bound.X + bound.Width);
      if ((double) this.ChildBound.W >= (double) (bound.Y + bound.Height))
        return;
      this.ChildBound.W = (float) (bound.Y + bound.Height);
    }

    public void SetOffsetForMousePos(Point mousePos)
    {
      int x1 = mousePos.X;
      int y1 = mousePos.Y;
      int num1 = this.OffsetMax.X - this.OffsetMin.X + this.Size.X;
      int num2 = this.OffsetMax.Y - this.OffsetMin.X + this.Size.Y;
      Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
      int num3 = viewport.Width - num1;
      int num4 = viewport.Height - num2;
      int x2 = this.OffsetMax.X;
      int y2 = this.OffsetMax.Y;
      float width = (float) viewport.Width;
      float height = (float) viewport.Height;
      this.Offset.X = (float) (int) MathHelper.Lerp((float) x2, (float) num3, (float) x1 / width);
      this.Offset.Y = (float) (int) MathHelper.Lerp((float) y2, (float) num4, (float) y1 / height);
    }

    public Point GetMousePosFor(Point offset)
    {
      int num1 = this.OffsetMax.X - this.OffsetMin.X + this.Size.X;
      int num2 = this.OffsetMax.Y - this.OffsetMin.X + this.Size.Y;
      Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
      int num3 = viewport.Width - num1;
      int num4 = viewport.Height - num2;
      int x = this.OffsetMax.X;
      int y = this.OffsetMax.Y;
      float width = (float) viewport.Width;
      float height = (float) viewport.Height;
      float num5 = (float) (offset.X - num3) / (float) (x - num3) * width;
      float num6 = (float) (offset.Y - num4) / (float) (y - num4) * height;
      return new Point((int) ((double) width - (double) num5), (int) ((double) height - (double) num6));
    }

    public void SetMouse(Point offset)
    {
      Point mousePosFor = this.GetMousePosFor(offset);
      InputManager.SetMousePos(mousePosFor.X, mousePosFor.Y);
    }
  }
}
