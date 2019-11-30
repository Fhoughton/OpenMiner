// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScreenMatrix
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;

namespace StudioForge.TotalMiner
{
  internal static class ScreenMatrix
  {
    public static Matrix GetScreenMatrix(
      GameInstance instance,
      Player player,
      Rectangle rect)
    {
      float scale;
      Vector3 pos;
      ScreenMatrix.GetScreenOffsetAndScale(instance, player, rect, out scale, out pos);
      return Matrix.CreateScale(scale) * Matrix.CreateTranslation(pos);
    }

    public static void GetScreenOffsetAndScale(
      GameInstance instance,
      Player player,
      Rectangle rect,
      out float scale,
      out Vector3 pos)
    {
      scale = 1f;
      pos = Vector3.Zero;
      if (instance == null || player == null || !instance.IsSplitScreen)
        return;
      Rectangle rectangle1 = GraphicStatics.DefaultViewport.Rectangle();
      Rectangle rectangle2 = player.Viewport.Rectangle();
      int num1 = (int) ((double) rectangle1.Width * 0.0750000029802322);
      int num2 = (int) ((double) rectangle1.Height * 0.0750000029802322);
      if (rectangle2.Width == rectangle1.Width)
        rectangle2.Width -= num1;
      if (rectangle2.Height == rectangle1.Height)
        rectangle2.Height -= num2;
      rectangle2.Width -= num1;
      rectangle2.Height -= num2;
      if (rectangle2.X == 0)
        rectangle2.X += num1;
      else
        num1 = 0;
      if (rectangle2.Y == 0)
        rectangle2.Y += num2;
      else
        num2 = 0;
      if (rect.Width > rectangle2.Width || rect.Height > rectangle2.Height)
      {
        float num3 = (float) rectangle2.Width / (float) rect.Width;
        float num4 = (float) rectangle2.Height / (float) rect.Height;
        scale = (double) num3 >= (double) num4 ? num4 : num3;
      }
      pos.X = (float) (((double) rectangle2.Width - (double) rect.Width * (double) scale) * 0.5) + (float) num1;
      pos.Y = (float) (((double) rectangle2.Height - (double) rect.Height * (double) scale) * 0.5) + (float) num2;
      pos.X = (float) (int) ((double) pos.X - (double) rect.X * (double) scale);
      pos.Y = (float) (int) ((double) pos.Y - (double) rect.Y * (double) scale);
    }

    public static Matrix GetScreenMatrix(
      GameInstance instance,
      Player player,
      ScreenForScale screen)
    {
      float scale;
      Vector3 pos;
      ScreenMatrix.GetScreenOffset(instance, player, screen, out scale, out pos);
      return Matrix.CreateScale(scale) * Matrix.CreateTranslation(pos);
    }

    public static void GetScreenOffset(
      GameInstance instance,
      Player player,
      ScreenForScale screen,
      out float scale,
      out Vector3 pos)
    {
      scale = 1f;
      pos = Vector3.Zero;
      if (instance == null || player == null || !instance.IsSplitScreen || instance.LocalPlayerCount <= 2 && !Globals2.GameSettings.SplitScreenVertical)
        return;
      scale = 0.5f;
    }

    public static Rectangle TransformRectangle(Rectangle rect, Matrix matrix)
    {
      Vector4 zero = Vector4.Zero;
      zero.X = (float) rect.X;
      zero.Y = (float) rect.Y;
      zero.Z = (float) (rect.X + rect.Width);
      zero.W = (float) (rect.Y + rect.Height);
      Vector4 vector4 = Vector4.Transform(zero, matrix);
      rect.X = (int) vector4.X;
      rect.Y = (int) vector4.Y;
      rect.Width = (int) ((double) vector4.Z - (double) vector4.X);
      rect.Height = (int) ((double) vector4.W - (double) vector4.Y);
      return rect;
    }
  }
}
