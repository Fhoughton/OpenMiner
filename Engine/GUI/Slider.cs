// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.Slider
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine.Core;

namespace StudioForge.Engine.GUI
{
  public class Slider : TextBox
  {
    private static TextBox.ColorProfile WidgetColorProfile;
    private Window widget;

    public event Window.WindowDragHandler DragSliderHandler;

    internal void RaiseDragSliderHandler(WindowDragEventArgs e)
    {
      if (this.DragSliderHandler == null)
        return;
      this.DragSliderHandler((object) this, e);
    }

    public override bool IsKeyNavigable
    {
      get
      {
        return true;
      }
    }

    public Slider()
    {
    }

    public Slider(int x, int y, int width, int height)
      : base((string) null, x, y, width, height)
    {
      this.Initialize();
    }

    public Slider(int x, int y, int width, int height, float textScale)
      : base((string) null, x, y, width, height, textScale)
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.AddFlags(Window.WinFlags.OwnsMouseWheel);
      this.TextAlignX = WinTextAlignX.Center;
      this.widget = new Window(this.Size.X / 2, 2, 10, this.Size.Y - 4);
      this.widget.IsEnabled = false;
      this.widget.Colors = (Window.ColorProfile) Slider.WidgetColorProfile;
      this.widget.BorderThickness = 1;
      this.AddChild((Node) this.widget);
    }

    public void SetValue(float normal)
    {
      normal = MathHelper.Clamp(normal, 0.0f, 1f);
      this.Text = ((int) ((double) normal * 100.0)).ToString() + "%";
      int borderThickness = this.widget.BorderThickness;
      int num = this.Size.X - (this.widget.Size.X + this.widget.BorderThickness);
      this.widget.Position.X = (float) borderThickness + (float) (num - borderThickness) * normal;
    }

    private void SetSliderPosition(float x)
    {
      int borderThickness = this.widget.BorderThickness;
      int num = this.Size.X - (this.widget.Size.X + this.widget.BorderThickness);
      this.widget.Position.X = (float) (int) MathHelper.Clamp(x, (float) borderThickness, (float) num);
      x = this.widget.Position.X - (float) borderThickness;
      this.RaiseDragSliderHandler(new WindowDragEventArgs((WindowManager) null, (Window) this, (Window) this, (Window) null, Point.Zero, false)
      {
        Tag = (object) (float) ((double) x / (double) (num - borderThickness))
      });
    }

    protected override void OnMouseWheelDeltaCore(WindowEventArgs e, int delta)
    {
      this.SetSliderPosition(this.widget.Position.X - (float) delta / 100f);
    }

    protected override void OnClickDownCore(WindowEventArgs args)
    {
      base.OnClickDownCore(args);
      this.SetSliderPosition((float) (args.MousePosition.X - this.widget.Size.X / 2));
    }

    protected override bool OnKeyPressCore(WindowEventArgs e, Keys[] keys)
    {
      switch (keys[0])
      {
        case Keys.Left:
          this.SetSliderPosition(this.widget.Position.X - (float) this.Size.X / 100f);
          break;
        case Keys.Right:
          this.SetSliderPosition(this.widget.Position.X + (float) this.Size.X / 100f);
          break;
      }
      return base.OnKeyPressCore(e, keys);
    }

    static Slider()
    {
      TextBox.ColorProfile colorProfile = new TextBox.ColorProfile();
      colorProfile.BackDisabledColor = Window.DefaultColorProfile.BackHoverColor * 0.4f;
      colorProfile.BackClickColor = Color.Transparent;
      colorProfile.BackColor = Color.Transparent;
      colorProfile.BackHoverColor = Color.Transparent;
      colorProfile.BorderColor = Color.White;
      colorProfile.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      Slider.WidgetColorProfile = colorProfile;
    }
  }
}
