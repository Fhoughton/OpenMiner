// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Extensions
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner
{
  internal static class Extensions
  {
    public static bool IsSpecialSeed(int seed)
    {
      if (seed != 666 && seed != 7201969)
        return seed == 20071969;
      return true;
    }

    public static bool IsMoonSeed(this Map map)
    {
      if (map.Seed != 7201969)
        return map.Seed == 20071969;
      return true;
    }

    public static bool IsMoonSeed(this MapOld map)
    {
      if (map.Seed != 7201969)
        return map.Seed == 20071969;
      return true;
    }

    public static bool IsHellSeed(this Map map)
    {
      return map.Seed == 666;
    }

    public static bool IsHellSeed(this MapOld map)
    {
      return map.Seed == 666;
    }

    public static bool IsNightSeed(this MapOld map)
    {
      return map.Seed == 2400;
    }

    public static void DrawBlockBox(
      this SpriteBatchSafe spriteBatch,
      Texture2D texture,
      Rectangle boxRect,
      float alpha,
      bool beginAndSetupStatesButDontEndBatch,
      int borderWidth,
      Color borderColor,
      Color innerColor,
      Matrix matrix)
    {
      spriteBatch.DrawBlockBox(texture, boxRect, alpha, beginAndSetupStatesButDontEndBatch, borderWidth, borderColor, innerColor, true, matrix);
    }

    public static void DrawBlockBox(
      this SpriteBatchSafe spriteBatch,
      Texture2D texture,
      Rectangle rect,
      float alpha,
      bool beginAndSetupStatesButDontEndBatch,
      int borderWidth,
      Color borderColor,
      Color innerColor,
      bool drawInner,
      Matrix matrix)
    {
      if (beginAndSetupStatesButDontEndBatch)
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, matrix);
      spriteBatch.DrawBlockBox(texture, rect, borderColor, innerColor);
    }

    public static void DrawBlockBox(
      this SpriteBatchSafe spriteBatch,
      Texture2D texture,
      Rectangle boxRect,
      Color color,
      Color bkgdColor)
    {
      if (texture == null)
        return;
      int num1 = texture.Width / 3;
      int num2 = texture.Height / 3;
      Rectangle rectangle1 = new Rectangle(0, 0, num1, num2);
      Rectangle rectangle2 = new Rectangle(num1 * 2, 0, num1, num2);
      Rectangle rectangle3 = new Rectangle(0, num2 * 2, num1, num2);
      Rectangle rectangle4 = new Rectangle(num1 * 2, num2 * 2, num1, num2);
      Rectangle destinationRectangle = new Rectangle(boxRect.X - num1, boxRect.Y - num2, num1, num2);
      spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle1), color);
      destinationRectangle.X = boxRect.X + boxRect.Width;
      spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle2), color);
      destinationRectangle.Y = boxRect.Y + boxRect.Height;
      spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle4), color);
      destinationRectangle.X = boxRect.X - num1;
      spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle3), color);
      Rectangle rectangle5 = new Rectangle(num1, 0, num1, num2);
      Rectangle rectangle6 = new Rectangle(num1, num2 * 2, num1, num2);
      for (destinationRectangle.X = boxRect.X; destinationRectangle.X < boxRect.X + boxRect.Width; destinationRectangle.X += num1)
      {
        if (destinationRectangle.X + num1 > boxRect.X + boxRect.Width)
        {
          int num3 = destinationRectangle.X + num1 - (boxRect.X + boxRect.Width);
          destinationRectangle.Width -= num3;
          rectangle5.Width -= num3;
          rectangle6.Width -= num3;
        }
        destinationRectangle.Y = boxRect.Y - num2;
        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle5), color);
        destinationRectangle.Y = boxRect.Y + boxRect.Height;
        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle6), color);
      }
      destinationRectangle.Width = num1;
      Rectangle rectangle7 = new Rectangle(0, num2, num1, num2);
      Rectangle rectangle8 = new Rectangle(num1 * 2, num2, num1, num2);
      for (destinationRectangle.Y = boxRect.Y; destinationRectangle.Y < boxRect.Y + boxRect.Height; destinationRectangle.Y += num2)
      {
        if (destinationRectangle.Y + num2 > boxRect.Y + boxRect.Height)
        {
          int num3 = destinationRectangle.Y + num2 - (boxRect.Y + boxRect.Height);
          destinationRectangle.Height -= num3;
          rectangle7.Height -= num3;
          rectangle8.Height -= num3;
        }
        destinationRectangle.X = boxRect.X - num1;
        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle7), color);
        destinationRectangle.X = boxRect.X + boxRect.Width;
        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(rectangle8), color);
      }
      spriteBatch.Draw(CoreGlobals.BlankTexture, boxRect, bkgdColor);
    }

    public static void BeginTM(this SpriteBatchSafe spriteBatch, Matrix matrix)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, (SamplerState) null, (DepthStencilState) null, (RasterizerState) null, (Effect) null, matrix);
    }

    public static void BeginTM(
      this SpriteBatchSafe spriteBatch,
      SamplerState samplerState,
      Matrix matrix)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, samplerState, (DepthStencilState) null, (RasterizerState) null, (Effect) null, matrix);
    }

    public static void BeginTM(
      this SpriteBatchSafe spriteBatch,
      RasterizerState rasterState,
      Matrix matrix)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, (SamplerState) null, (DepthStencilState) null, rasterState, (Effect) null, matrix);
    }

    public static void DrawGradient(
      this SpriteBatchSafe spriteBatch,
      Rectangle boxRect,
      int leftWidth,
      int rightWidth,
      Color color,
      Matrix matrix)
    {
      int width = boxRect.Width;
      boxRect.Width = leftWidth;
      spriteBatch.Draw(GraphicStatics.GradientTexture, boxRect, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
      boxRect.X += boxRect.Width;
      boxRect.Width = width - leftWidth - rightWidth;
      spriteBatch.Draw(CoreGlobals.BlankTexture, boxRect, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
      boxRect.X += boxRect.Width;
      boxRect.Width = rightWidth;
      spriteBatch.Draw(GraphicStatics.GradientTexture, boxRect, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
    }

    public static bool IsBlockBoxInFrustum(
      this BoundingFrustum frustum,
      GameInstance instance,
      GlobalPoint3D p)
    {
      return frustum.Contains(instance.GetBlockBox(p)) != ContainmentType.Disjoint;
    }
  }
}
