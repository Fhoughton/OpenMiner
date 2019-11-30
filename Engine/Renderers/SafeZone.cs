// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.SafeZone
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StudioForge.Engine.Renderers
{
  public static class SafeZone
  {
    private static readonly Dictionary<float, Rectangle> prevValues = new Dictionary<float, Rectangle>();

    public static Rectangle GetTitleSafeArea(GraphicsDevice device, float percent)
    {
      Rectangle rectangle;
      if (SafeZone.prevValues.TryGetValue(percent, out rectangle))
        return rectangle;
      rectangle = new Rectangle(device.Viewport.X, device.Viewport.Y, device.Viewport.Width, device.Viewport.Height);
      float num = (float) ((1.0 - (double) percent) / 2.0);
      rectangle.X = (int) ((double) num * (double) rectangle.Width);
      rectangle.Y = (int) ((double) num * (double) rectangle.Height);
      rectangle.Width = (int) ((double) percent * (double) rectangle.Width);
      rectangle.Height = (int) ((double) percent * (double) rectangle.Height);
      SafeZone.prevValues.Add(percent, rectangle);
      return rectangle;
    }
  }
}
