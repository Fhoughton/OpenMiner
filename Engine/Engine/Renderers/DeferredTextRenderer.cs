// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.DeferredTextRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Renderers
{
  public static class DeferredTextRenderer
  {
    private static List<DeferredTextRenderer.TextObject> textObjects = new List<DeferredTextRenderer.TextObject>();
    private static GraphicsDevice graphicsDevice;
    private static SpriteBatchSafe spriteBatch;
    private static SpriteFont font;

    public static bool IsVisible { get; set; }

    public static void LoadContent()
    {
      DeferredTextRenderer.graphicsDevice = CoreGlobals.GraphicsDevice;
      DeferredTextRenderer.spriteBatch = new SpriteBatchSafe(DeferredTextRenderer.graphicsDevice);
      DeferredTextRenderer.font = CoreGlobals.GameFont;
    }

    public static void AddText(string text, Vector2 pos)
    {
      DeferredTextRenderer.AddText(text, pos, Color.White, 3.0, 0.5f);
    }

    public static void AddText(string text, Vector2 pos, Color color)
    {
      DeferredTextRenderer.AddText(text, pos, color, 3.0, 0.5f);
    }

    public static void AddText(string text, Vector2 pos, Color color, float scale)
    {
      DeferredTextRenderer.AddText(text, pos, color, 3.0, scale);
    }

    public static void AddText(string text, Vector2 pos, Color color, double age)
    {
      DeferredTextRenderer.AddText(text, pos, color, age, 0.5f);
    }

    public static void AddText(string text, Vector3 pos, Color color, double age, float scale)
    {
      ICamera camera = CoreGlobals.Camera;
      if (camera == null)
        return;
      if (DeferredTextRenderer.graphicsDevice == null)
        DeferredTextRenderer.LoadContent();
      Vector3 vector3 = DeferredTextRenderer.graphicsDevice.Viewport.Project(pos, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
      DeferredTextRenderer.AddText(text, new Vector2(vector3.X, vector3.Y), color, age, scale);
    }

    public static void AddText(string text, Vector2 pos, Color color, double age, float scale)
    {
      DeferredTextRenderer.TextObject textObject = new DeferredTextRenderer.TextObject()
      {
        Text = text,
        Age = age,
        Scale = scale,
        Color = color,
        Position = pos
      };
      DeferredTextRenderer.textObjects.Add(textObject);
    }

    public static void Draw()
    {
      if (DeferredTextRenderer.graphicsDevice == null)
        DeferredTextRenderer.LoadContent();
      DeferredTextRenderer.spriteBatch.Begin();
      for (int index = DeferredTextRenderer.textObjects.Count - 1; index >= 0; --index)
      {
        DeferredTextRenderer.TextObject textObject = DeferredTextRenderer.textObjects[index];
        DeferredTextRenderer.spriteBatch.DrawString(DeferredTextRenderer.font, textObject.Text, textObject.Position, textObject.Color, 0.0f, Vector2.Zero, textObject.Scale, SpriteEffects.None, 1f);
        textObject.Age -= (double) Services.ElapsedTime;
        if (textObject.Age <= 0.0)
          DeferredTextRenderer.textObjects.RemoveAt(index);
        else
          DeferredTextRenderer.textObjects[index] = textObject;
      }
      DeferredTextRenderer.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }

    private struct TextObject
    {
      public string Text;
      public Color Color;
      public double Age;
      public Vector2 Position;
      public float Scale;
    }
  }
}
