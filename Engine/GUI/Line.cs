// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.Line
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Renderers;

namespace StudioForge.Engine.GUI
{
  public class Line : Window
  {
    public override Point SizeScaled(float worldScale)
    {
      Point point = base.SizeScaled(worldScale);
      return new Point(this.Size.X != 1 ? point.X : 1, this.Size.Y != 1 ? point.Y : 1);
    }

    public Line(string name, int x1, int y1, int x2, int y2)
      : base(name, x1, y1, x2 - x1 + 1, y2 - y1 + 1)
    {
    }

    public override void DrawBackground(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      Color backColor)
    {
      if (spriteBatch == null)
        return;
      LineRenderer2D.DrawLine(spriteBatch, CoreGlobals.BlankTexture, backColor, new Vector2((float) bound.X, (float) bound.Y), new Vector2((float) (bound.X + bound.Width - 1), (float) (bound.Y + bound.Height - 1)));
    }
  }
}
