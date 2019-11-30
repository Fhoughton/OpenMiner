// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.MyExtensions
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace StudioForge.Engine.Core
{
  public static class MyExtensions
  {
    private static Texture2D blankTexture;
    private static Texture2D roundedEdgeTexture;
    private static Texture2D roundedLineEdgeTexture;

    public static bool IsEmpty(this string s)
    {
      if (s != null)
        return s.Length == 0;
      return true;
    }

    public static bool IsNotEmpty(this string s)
    {
      if (s != null)
        return s.Length > 0;
      return false;
    }

    public static bool HasUpperChars(this string s)
    {
      foreach (char c in s)
      {
        if (char.IsUpper(c))
          return true;
      }
      return false;
    }

    public static bool IsEmpty(this GamePadState pad)
    {
      if (pad.Buttons.A == ButtonState.Released && pad.Buttons.B == ButtonState.Released && (pad.Buttons.X == ButtonState.Released && pad.Buttons.Y == ButtonState.Released) && (pad.Buttons.Back == ButtonState.Released && pad.Buttons.BigButton == ButtonState.Released && (pad.Buttons.LeftShoulder == ButtonState.Released && pad.Buttons.LeftStick == ButtonState.Released)) && (pad.Buttons.RightShoulder == ButtonState.Released && pad.Buttons.RightStick == ButtonState.Released && (pad.Buttons.Start == ButtonState.Released && pad.DPad.Down == ButtonState.Released) && (pad.DPad.Left == ButtonState.Released && pad.DPad.Right == ButtonState.Released && (pad.DPad.Up == ButtonState.Released && (double) pad.ThumbSticks.Left.X < 0.00999999977648258))) && ((double) pad.ThumbSticks.Left.X > -0.00999999977648258 && (double) pad.ThumbSticks.Left.Y < 0.00999999977648258 && ((double) pad.ThumbSticks.Left.Y > -0.00999999977648258 && (double) pad.ThumbSticks.Right.X < 0.00999999977648258) && ((double) pad.ThumbSticks.Right.X > -0.00999999977648258 && (double) pad.ThumbSticks.Right.Y < 0.00999999977648258 && ((double) pad.ThumbSticks.Right.Y > -0.00999999977648258 && (double) pad.Triggers.Left < 0.00999999977648258))))
        return (double) pad.Triggers.Right < 0.00999999977648258;
      return false;
    }

    public static bool IsEmpty(this Point p)
    {
      if (p.X == 0)
        return p.Y == 0;
      return false;
    }

    public static Point Add(this Point p, Point o)
    {
      return new Point(p.X + o.X, p.Y + o.Y);
    }

    public static Rectangle Rectangle(this Viewport vp)
    {
      return new Rectangle()
      {
        X = vp.X,
        Y = vp.Y,
        Width = vp.Width,
        Height = vp.Height
      };
    }

    public static Rectangle Expand(this Rectangle rect, int i)
    {
      rect.X -= i;
      rect.Y -= i;
      int num = i * 2;
      rect.Width += num;
      rect.Height += num;
      return rect;
    }

    public static Rectangle Merge(this Rectangle rect, Rectangle other)
    {
      if (other.X < rect.X)
      {
        rect.Width += rect.X - other.X;
        rect.X = other.X;
      }
      if (other.Y < rect.Y)
      {
        rect.Height += rect.Y - other.Y;
        rect.Y = other.Y;
      }
      if (other.Width > rect.Width)
        rect.Width = other.Width;
      if (other.Height > rect.Height)
        rect.Height = other.Height;
      if (other.X + other.Width > rect.X + rect.Width)
        rect.Width += other.X + other.Width - (rect.X + rect.Width);
      if (other.Y + other.Height > rect.Y + rect.Height)
        rect.Height += other.Y + other.Height - (rect.Y + rect.Height);
      return rect;
    }

    public static void ResetRenderStates(this SpriteBatchSafe spriteBatch)
    {
    }

    public static void DrawBox(
      this SpriteBatchSafe spriteBatch,
      Rectangle rect,
      int thickness,
      Color color,
      float z)
    {
      spriteBatch.DrawBox(MyExtensions.BlankTexture, rect, thickness, color, z);
    }

    public static void DrawBox(
      this SpriteBatchSafe spriteBatch,
      Texture2D texture,
      Rectangle rect,
      int thickness,
      Color color,
      float z)
    {
      Rectangle destinationRectangle1 = new Rectangle(rect.X, rect.Y, thickness, rect.Height);
      Rectangle destinationRectangle2 = new Rectangle(rect.X + thickness, rect.Y, rect.Width - thickness - thickness, thickness);
      Rectangle destinationRectangle3 = new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height);
      Rectangle destinationRectangle4 = new Rectangle(rect.X + thickness, rect.Y + rect.Height - thickness, rect.Width - thickness - thickness, thickness);
      spriteBatch.Draw(texture, destinationRectangle1, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
      spriteBatch.Draw(texture, destinationRectangle2, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
      spriteBatch.Draw(texture, destinationRectangle3, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
      spriteBatch.Draw(texture, destinationRectangle4, new Rectangle?(), color, 0.0f, Vector2.Zero, SpriteEffects.None, z);
    }

    public static void DrawFilledBox(
      this SpriteBatchSafe spriteBatch,
      Rectangle rect,
      int thickness,
      Color color,
      Color fillColor)
    {
      spriteBatch.DrawFilledBox(MyExtensions.BlankTexture, rect, thickness, color, fillColor);
    }

    public static void DrawFilledBox(
      this SpriteBatchSafe spriteBatch,
      Texture2D texture,
      Rectangle rect,
      int thickness,
      Color color,
      Color fillColor)
    {
      spriteBatch.DrawBox(texture, rect, thickness, color, 0.0f);
      Rectangle destinationRectangle = new Rectangle(rect.X + thickness, rect.Y + thickness, rect.Width - thickness - thickness, rect.Height - thickness - thickness);
      spriteBatch.Draw(texture, destinationRectangle, fillColor);
    }

    public static void DrawRoundedEdgeBox(
      this SpriteBatchSafe spriteBatch,
      Rectangle rect,
      int thickness,
      Color color)
    {
      Texture2D roundedLineEdgeTexture = MyExtensions.RoundedLineEdgeTexture;
      Rectangle destinationRectangle1 = new Rectangle(rect.X + 1, rect.Y + 1, 11, 11);
      Rectangle destinationRectangle2 = new Rectangle(rect.X + rect.Width - 12, rect.Y + 1, 11, 11);
      Rectangle destinationRectangle3 = new Rectangle(rect.X + 1, rect.Y + rect.Height - 12, 11, 11);
      Rectangle destinationRectangle4 = new Rectangle(rect.X + rect.Width - 12, rect.Y + rect.Height - 12, 11, 11);
      Vector2 zero = Vector2.Zero;
      spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle1, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally, 1f);
      spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle2, color);
      spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle3, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 1f);
      spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle4, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipVertically, 1f);
      if (thickness > 1)
      {
        for (int index = 1; index < thickness; ++index)
        {
          ++destinationRectangle1.X;
          --destinationRectangle2.X;
          ++destinationRectangle3.X;
          --destinationRectangle4.X;
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle1, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally, 1f);
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle2, color);
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle3, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 1f);
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle4, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipVertically, 1f);
          ++destinationRectangle1.Y;
          ++destinationRectangle2.Y;
          --destinationRectangle3.Y;
          --destinationRectangle4.Y;
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle1, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally, 1f);
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle2, color);
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle3, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 1f);
          spriteBatch.Draw(roundedLineEdgeTexture, destinationRectangle4, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipVertically, 1f);
        }
      }
      Rectangle destinationRectangle5 = new Rectangle(rect.X, rect.Y + 12, thickness, rect.Height - 24);
      Rectangle destinationRectangle6 = new Rectangle(rect.X + 12, rect.Y, rect.Width - 24, thickness);
      Rectangle destinationRectangle7 = new Rectangle(rect.X + rect.Width - thickness, rect.Y + 12, thickness, rect.Height - 24);
      Rectangle destinationRectangle8 = new Rectangle(rect.X + 12, rect.Y + rect.Height - thickness, rect.Width - 24, thickness);
      spriteBatch.Draw(MyExtensions.BlankTexture, destinationRectangle5, color);
      spriteBatch.Draw(MyExtensions.BlankTexture, destinationRectangle6, color);
      spriteBatch.Draw(MyExtensions.BlankTexture, destinationRectangle7, color);
      spriteBatch.Draw(MyExtensions.BlankTexture, destinationRectangle8, color);
    }

    public static void DrawRoundedBox(
      this SpriteBatchSafe spriteBatch,
      Rectangle rect,
      Color color)
    {
      Texture2D roundedEdgeTexture = MyExtensions.RoundedEdgeTexture;
      Rectangle destinationRectangle1 = new Rectangle(rect.X + 1, rect.Y + 1, 11, 11);
      Rectangle destinationRectangle2 = new Rectangle(rect.X + rect.Width - 12, rect.Y + 1, 11, 11);
      Rectangle destinationRectangle3 = new Rectangle(rect.X + 1, rect.Y + rect.Height - 12, 11, 11);
      Rectangle destinationRectangle4 = new Rectangle(rect.X + rect.Width - 12, rect.Y + rect.Height - 12, 11, 11);
      Rectangle destinationRectangle5 = new Rectangle(rect.X + 12, rect.Y, rect.Width - 24, 12);
      Rectangle destinationRectangle6 = new Rectangle(rect.X + 12, rect.Y + rect.Height - 12, rect.Width - 24, 12);
      Rectangle destinationRectangle7 = new Rectangle(rect.X, rect.Y + 12, rect.Width, rect.Height - 24);
      Vector2 zero = Vector2.Zero;
      spriteBatch.Draw(roundedEdgeTexture, destinationRectangle1, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally, 1f);
      spriteBatch.Draw(roundedEdgeTexture, destinationRectangle2, color);
      spriteBatch.Draw(roundedEdgeTexture, destinationRectangle3, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 1f);
      spriteBatch.Draw(roundedEdgeTexture, destinationRectangle4, new Rectangle?(), color, 0.0f, zero, SpriteEffects.FlipVertically, 1f);
      spriteBatch.Draw(MyExtensions.BlankTexture, destinationRectangle5, color);
      spriteBatch.Draw(MyExtensions.BlankTexture, destinationRectangle6, color);
      spriteBatch.Draw(MyExtensions.BlankTexture, destinationRectangle7, color);
    }

    public static void DrawRoundedFilledBox(
      this SpriteBatchSafe spriteBatch,
      Rectangle rect,
      int thickness,
      Color color,
      Color fillColor)
    {
      spriteBatch.DrawRoundedBox(rect, fillColor);
      spriteBatch.DrawRoundedEdgeBox(rect, thickness, color);
    }

    public static Rectangle ViewportRect(this SpriteBatchSafe spriteBatch)
    {
      return spriteBatch.GraphicsDevice.Viewport.Rectangle();
    }

    public static void DrawStringCentered(
      this SpriteBatchSafe spriteBatch,
      SpriteFont font,
      string text,
      float y,
      Color color,
      float scale)
    {
      Vector2 vector2 = font.MeasureString(text) * scale;
      float x = (float) (((double) spriteBatch.GraphicsDevice.Viewport.Width - (double) vector2.X) / 2.0);
      spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
    }

    public static void DrawStringCentered(
      this SpriteBatchSafe spriteBatch,
      SpriteFont font,
      string text,
      Rectangle rect,
      Color color,
      float scale)
    {
      Vector2 vector2 = font.MeasureString(text) * scale;
      float x = (float) (((double) rect.Width - (double) vector2.X) / 2.0) + (float) rect.X;
      spriteBatch.DrawString(font, text, new Vector2(x, (float) rect.Y), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
    }

    public static Texture2D BlankTexture
    {
      get
      {
        if (MyExtensions.blankTexture == null)
        {
          MyExtensions.blankTexture = new Texture2D(CoreGlobals.GraphicsDevice, 1, 1);
          MyExtensions.blankTexture.SetData<Color>(new Color[1]
          {
            Color.White
          });
        }
        return MyExtensions.blankTexture;
      }
    }

    public static Texture2D RoundedEdgeTexture
    {
      get
      {
        if (MyExtensions.roundedEdgeTexture == null)
        {
          MyExtensions.roundedEdgeTexture = new Texture2D(CoreGlobals.GraphicsDevice, 11, 11);
          Color[] data = MyExtensions.FillRoundedEdgeTexture();
          MyExtensions.roundedEdgeTexture.SetData<Color>(data);
        }
        return MyExtensions.roundedEdgeTexture;
      }
    }

    public static Texture2D RoundedLineEdgeTexture
    {
      get
      {
        if (MyExtensions.roundedLineEdgeTexture == null)
        {
          MyExtensions.roundedLineEdgeTexture = new Texture2D(CoreGlobals.GraphicsDevice, 11, 11);
          Color[] data = MyExtensions.FillRoundedLineEdgeTexture();
          MyExtensions.roundedLineEdgeTexture.SetData<Color>(data);
        }
        return MyExtensions.roundedLineEdgeTexture;
      }
    }

    private static Color[] FillRoundedEdgeTexture()
    {
      Color[] colorArray = new Color[121];
      int num1 = 0;
      for (int index = 0; index < 3; ++index)
        colorArray[index + num1 * 11] = Color.White;
      int num2 = num1 + 1;
      for (int index = 0; index < 5; ++index)
        colorArray[index + num2 * 11] = Color.White;
      int num3 = num2 + 1;
      for (int index = 0; index < 7; ++index)
        colorArray[index + num3 * 11] = Color.White;
      int num4 = num3 + 1;
      for (int index = 0; index < 8; ++index)
        colorArray[index + num4 * 11] = Color.White;
      int num5 = num4 + 1;
      for (int index = 0; index < 9; ++index)
        colorArray[index + num5 * 11] = Color.White;
      int num6 = num5 + 1;
      for (int index = 0; index < 9; ++index)
        colorArray[index + num6 * 11] = Color.White;
      int num7 = num6 + 1;
      for (int index = 0; index < 10; ++index)
        colorArray[index + num7 * 11] = Color.White;
      int num8 = num7 + 1;
      for (int index = 0; index < 10; ++index)
        colorArray[index + num8 * 11] = Color.White;
      int num9 = num8 + 1;
      for (int index = 0; index < 11; ++index)
        colorArray[index + num9 * 11] = Color.White;
      int num10 = num9 + 1;
      for (int index = 0; index < 11; ++index)
        colorArray[index + num10 * 11] = Color.White;
      int num11 = num10 + 1;
      for (int index = 0; index < 11; ++index)
        colorArray[index + num11 * 11] = Color.White;
      return colorArray;
    }

    private static Color[] FillRoundedLineEdgeTexture()
    {
      Color[] colorArray = new Color[121];
      int num1 = 0;
      for (int index = 0; index < 3; ++index)
        colorArray[index + num1 * 11] = Color.White;
      int num2 = num1 + 1;
      for (int index = 3; index < 5; ++index)
        colorArray[index + num2 * 11] = Color.White;
      int num3 = num2 + 1;
      for (int index = 5; index < 7; ++index)
        colorArray[index + num3 * 11] = Color.White;
      int num4 = num3 + 1;
      colorArray[7 + num4 * 11] = Color.White;
      int num5 = num4 + 1;
      colorArray[8 + num5 * 11] = Color.White;
      int num6 = num5 + 1;
      colorArray[8 + num6 * 11] = Color.White;
      int num7 = num6 + 1;
      colorArray[9 + num7 * 11] = Color.White;
      int num8 = num7 + 1;
      colorArray[9 + num8 * 11] = Color.White;
      int num9 = num8 + 1;
      colorArray[10 + num9 * 11] = Color.White;
      int num10 = num9 + 1;
      colorArray[10 + num10 * 11] = Color.White;
      int num11 = num10 + 1;
      colorArray[10 + num11 * 11] = Color.White;
      return colorArray;
    }

    public static void DrawLine(
      this SpriteBatchSafe spriteBatch,
      Texture2D texture,
      float width,
      Color color,
      Vector2 point1,
      Vector2 point2)
    {
      float rotation = (float) Math.Atan2((double) point2.Y - (double) point1.Y, (double) point2.X - (double) point1.X);
      float x = Vector2.Distance(point1, point2);
      spriteBatch.Draw(texture, point1, new Rectangle?(), color, rotation, Vector2.Zero, new Vector2(x, width), SpriteEffects.None, 0.0f);
    }

    public static Texture2D Transfer<T>(this Texture2D srcTexture, Rectangle srcRect) where T : struct
    {
      T[] data = new T[srcRect.Width * srcRect.Height];
      srcTexture.GetData<T>(0, new Rectangle?(srcRect), data, 0, data.Length);
      Texture2D texture2D = new Texture2D(srcTexture.GraphicsDevice, srcRect.Width, srcRect.Height);
      texture2D.SetData<T>(data);
      return texture2D;
    }

    public static Texture2D Copy<T>(this Texture2D srcTexture) where T : struct
    {
      T[] data = new T[srcTexture.Width * srcTexture.Height];
      srcTexture.GetData<T>(data);
      Texture2D texture2D = new Texture2D(srcTexture.GraphicsDevice, srcTexture.Width, srcTexture.Height);
      texture2D.SetData<T>(data);
      return texture2D;
    }

    public static string TrimTime(this TimeSpan ts)
    {
      return ts.TrimTime(true, true);
    }

    public static string TrimTime(this TimeSpan ts, bool showHours, bool plural)
    {
      string str = "";
      int minutes = ts.Minutes;
      if (showHours)
      {
        if (ts.Hours > 0)
          str = str + ts.Hours.ToString() + (ts.Hours == 1 || !plural ? " hour" : " hours");
      }
      else
        minutes += ts.Hours * 60;
      if (minutes > 0)
        str = str + (str.Length == 0 ? "" : " ") + minutes.ToString() + (minutes == 1 || !plural ? " minute" : " minutes");
      if (ts.Seconds > 0)
        str = str + (str.Length == 0 ? "" : " ") + ts.Seconds.ToString() + (ts.Seconds == 1 || !plural ? " second" : " seconds");
      return str;
    }

    public static T GetService<T>(this GameComponent component)
    {
      return (T) component.Game.Services.GetService(typeof (T));
    }

    public static T GetService<T>(this GameServiceContainer service)
    {
      return (T) service.GetService(typeof (T));
    }

    public static bool IsSetOnDevice(this GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer)
    {
      /*
            VertexBufferBinding[] vertexBuffers = graphicsDevice.getVertexBuffer();
      if (vertexBuffers != null)
      {
        for (int index = 0; index < vertexBuffers.Length; ++index)
        {
          if (vertexBuffers[index].VertexBuffer == vertexBuffer)
            return true;
        }
      }
      */
      return false;
    }

    public static bool IsSetOnDevice(this GraphicsDevice graphicsDevice, Texture2D texture)
    {
      TextureCollection textures = graphicsDevice.Textures;
      if (textures != null)
      {
        try
        {
          for (int index = 0; index < 2; ++index)
          {
            if (textures[index] == texture)
              return true;
          }
        }
        catch (Exception ex)
        {
        }
      }
      return false;
    }

    public static bool FastIntersect(this BoundingFrustum frustum, ref BoundingBox box)
    {
      float x1 = box.Min.X;
      float y1 = box.Min.Y;
      float z1 = box.Min.Z;
      float x2 = box.Max.X;
      float y2 = box.Max.Y;
      float z2 = box.Max.Z;
      Vector3 normal1 = frustum.Near.Normal;
      Vector3 vector3;
      vector3.X = (double) normal1.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal1.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal1.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Near.D + (double) normal1.X * (double) vector3.X + (double) normal1.Y * (double) vector3.Y + (double) normal1.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal2 = frustum.Left.Normal;
      vector3.X = (double) normal2.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal2.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal2.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Left.D + (double) normal2.X * (double) vector3.X + (double) normal2.Y * (double) vector3.Y + (double) normal2.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal3 = frustum.Right.Normal;
      vector3.X = (double) normal3.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal3.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal3.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Right.D + (double) normal3.X * (double) vector3.X + (double) normal3.Y * (double) vector3.Y + (double) normal3.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal4 = frustum.Bottom.Normal;
      vector3.X = (double) normal4.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal4.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal4.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Bottom.D + (double) normal4.X * (double) vector3.X + (double) normal4.Y * (double) vector3.Y + (double) normal4.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal5 = frustum.Top.Normal;
      vector3.X = (double) normal5.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal5.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal5.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Top.D + (double) normal5.X * (double) vector3.X + (double) normal5.Y * (double) vector3.Y + (double) normal5.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal6 = frustum.Far.Normal;
      vector3.X = (double) normal6.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal6.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal6.Z >= 0.0 ? z1 : z2;
      return (double) frustum.Far.D + (double) normal6.X * (double) vector3.X + (double) normal6.Y * (double) vector3.Y + (double) normal6.Z * (double) vector3.Z <= 0.0;
    }

    public static bool FastSideIntersect(this BoundingFrustum frustum, ref BoundingBox box)
    {
      float x1 = box.Min.X;
      float y1 = box.Min.Y;
      float z1 = box.Min.Z;
      float x2 = box.Max.X;
      float y2 = box.Max.Y;
      float z2 = box.Max.Z;
      Vector3 normal1 = frustum.Near.Normal;
      Vector3 vector3;
      vector3.X = (double) normal1.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal1.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal1.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Near.D + (double) normal1.X * (double) vector3.X + (double) normal1.Y * (double) vector3.Y + (double) normal1.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal2 = frustum.Left.Normal;
      vector3.X = (double) normal2.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal2.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal2.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Left.D + (double) normal2.X * (double) vector3.X + (double) normal2.Y * (double) vector3.Y + (double) normal2.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal3 = frustum.Right.Normal;
      vector3.X = (double) normal3.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal3.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal3.Z >= 0.0 ? z1 : z2;
      if ((double) frustum.Right.D + (double) normal3.X * (double) vector3.X + (double) normal3.Y * (double) vector3.Y + (double) normal3.Z * (double) vector3.Z > 0.0)
        return false;
      Vector3 normal4 = frustum.Far.Normal;
      vector3.X = (double) normal4.X >= 0.0 ? x1 : x2;
      vector3.Y = (double) normal4.Y >= 0.0 ? y1 : y2;
      vector3.Z = (double) normal4.Z >= 0.0 ? z1 : z2;
      return (double) frustum.Far.D + (double) normal4.X * (double) vector3.X + (double) normal4.Y * (double) vector3.Y + (double) normal4.Z * (double) vector3.Z <= 0.0;
    }

    public static bool FastUpDownIntersect(this BoundingFrustum frustum, ref BoundingBox box)
    {
      Vector3 normal = frustum.Bottom.Normal;
      Vector3 vector3;
      vector3.X = (double) normal.X >= 0.0 ? box.Min.X : box.Max.X;
      vector3.Y = (double) normal.Y >= 0.0 ? box.Min.Y : box.Max.Y;
      vector3.Z = (double) normal.Z >= 0.0 ? box.Min.Z : box.Max.Z;
      if ((double) frustum.Bottom.D + (double) normal.X * (double) vector3.X + (double) normal.Y * (double) vector3.Y + (double) normal.Z * (double) vector3.Z > 0.0)
        return false;
      normal = frustum.Top.Normal;
      vector3.X = (double) normal.X >= 0.0 ? box.Min.X : box.Max.X;
      vector3.Y = (double) normal.Y >= 0.0 ? box.Min.Y : box.Max.Y;
      vector3.Z = (double) normal.Z >= 0.0 ? box.Min.Z : box.Max.Z;
      return (double) frustum.Top.D + (double) normal.X * (double) vector3.X + (double) normal.Y * (double) vector3.Y + (double) normal.Z * (double) vector3.Z <= 0.0;
    }

    public static Rectangle CenterOfViewport(int rectWidth, int rectHeight)
    {
      return MyExtensions.CenterOfViewport(CoreGlobals.GraphicsDevice.Viewport, rectWidth, rectHeight);
    }

    public static Rectangle CenterOfViewport(Viewport vp, int rectWidth, int rectHeight)
    {
      return MyExtensions.CenterOfOuterRectangle(vp.Rectangle(), rectWidth, rectHeight);
    }

    public static Rectangle CenterOfOuterRectangle(
      Rectangle rect,
      int rectWidth,
      int rectHeight)
    {
      return new Rectangle(0, 0, rectWidth, rectHeight)
      {
        X = (rect.Width - rectWidth) / 2 + rect.X,
        Y = (rect.Height - rectHeight) / 2 + rect.Y
      };
    }

    public static Vector3 CenterOfBox(BoundingBox box)
    {
      return (box.Max - box.Min) * 0.5f + box.Min;
    }

    public static void ClearGamePadVibrations()
    {
      GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
      GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
      GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
      GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
    }

    public static long BufferSize(this VertexBuffer vb)
    {
      return (long) (vb.VertexCount * vb.VertexDeclaration.VertexStride);
    }

    public static long BufferSize(this IndexBuffer ib)
    {
      return (long) (ib.IndexCount * (ib.IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4));
    }

    public static bool RandomChance(this Random random, double chance)
    {
      return random.NextDouble() <= chance;
    }

    public static bool RandomChanceTime(this Random random, double seconds)
    {
      return random.Next((int) (seconds * 60.0)) == 0;
    }
  }
}
