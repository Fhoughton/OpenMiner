// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.LineRenderer2D
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Renderers
{
  public class LineRenderer2D
  {
    public static void DrawLine(
      SpriteBatchSafe batch,
      Texture2D texture,
      Color color,
      Vector2 point1,
      Vector2 point2,
      Point viewportSize)
    {
      if ((double) point1.X < 0.0 && (double) point2.X < 0.0 || (double) point1.X > (double) viewportSize.X && (double) point2.X > (double) viewportSize.X || ((double) point1.Y < 0.0 && (double) point2.Y < 0.0 || (double) point1.Y > (double) viewportSize.Y && (double) point2.Y > (double) viewportSize.Y))
        return;
      LineRenderer2D.DrawLine(batch, texture, color, point1, point2);
    }

    public static void DrawLine(
      SpriteBatchSafe batch,
      Texture2D texture,
      Color color,
      Vector4 line)
    {
      LineRenderer2D.DrawLine(batch, texture, color, new Vector2(line.X, line.Y), new Vector2(line.Z, line.W));
    }

    public static void DrawLine(
      SpriteBatchSafe batch,
      Texture2D texture,
      Color color,
      Vector2 point1,
      Vector2 point2)
    {
      float rotation = (float) Math.Atan2((double) point2.Y - (double) point1.Y, (double) point2.X - (double) point1.X);
      float x = (point2 - point1).Length();
      batch.Draw(texture, point1, new Rectangle?(), color, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, 0.0f);
    }

    public static void DrawPoint(
      SpriteBatchSafe batch,
      Texture2D texture,
      Color color,
      Vector2 point,
      Vector2 pointSize,
      Point viewportSize)
    {
      Vector2 vector2 = new Vector2(pointSize.X * 0.5f, pointSize.Y * 0.5f);
      Rectangle rect = new Rectangle((int) ((double) point.X - (double) vector2.X), (int) ((double) point.Y - (double) vector2.Y), (int) pointSize.X, (int) pointSize.Y);
      LineRenderer2D.DrawPoint(batch, texture, color, rect, viewportSize);
    }

    public static void DrawPoint(
      SpriteBatchSafe batch,
      Texture2D texture,
      Color color,
      Rectangle rect,
      Point viewportSize)
    {
      Rectangle rectangle = new Rectangle(0, 0, viewportSize.X, viewportSize.Y);
      if (!rect.Intersects(rectangle))
        return;
      batch.Draw(texture, rect, color);
    }
  }
}
