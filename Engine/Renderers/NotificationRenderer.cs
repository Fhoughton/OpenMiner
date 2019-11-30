// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.NotificationRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.Renderers
{
  public class NotificationRenderer : IHasContent, IHasDraw
  {
    private string[] text = new string[20];
    private float[] age = new float[20];
    private Color[] backColor = new Color[20];
    private float initialAge = 4f;
    private int currentCount;
    private SpriteFont font;
    private SpriteBatchSafe spriteBatch;
    private int laylowCounter;

    public bool IsVisible { get; set; }

    public void LoadContent(InitState state)
    {
      this.font = CoreGlobals.GameFont;
      this.spriteBatch = new SpriteBatchSafe(CoreGlobals.GraphicsDevice);
    }

    public void UnloadContent()
    {
    }

    public void AddNotification(string msgText)
    {
      this.AddNotification(msgText, Color.Red);
    }

    public void AddNotification(string msgText, Color backColor)
    {
      if (this.currentCount >= this.text.Length || msgText == null || msgText.Length <= 0)
        return;
      this.text[this.currentCount] = msgText;
      this.age[this.currentCount] = this.initialAge;
      this.backColor[this.currentCount] = backColor;
      ++this.currentCount;
    }

    public void Draw(DrawState state)
    {
      if (--this.laylowCounter < 0)
      {
        try
        {
          if (this.currentCount > this.age.Length)
            this.currentCount = this.age.Length;
          this.spriteBatch.Begin();
          for (int i = 0; i < this.currentCount; ++i)
            this.DrawNotification(i);
          this.spriteBatch.End();
          ++CoreGlobals.FrameRateCounter.SpriteCalls;
        }
        catch (InvalidOperationException ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(-4, (Exception) ex);
          this.spriteBatch = new SpriteBatchSafe(CoreGlobals.GraphicsDevice);
          this.laylowCounter = 300;
        }
        catch (OutOfMemoryException ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(-5, (Exception) ex);
          this.laylowCounter = 100;
        }
      }
      if (this.currentCount <= 0 || (double) this.age[0] > 0.0)
        return;
      for (int index = 1; index < this.currentCount; ++index)
      {
        this.text[index - 1] = this.text[index];
        this.age[index - 1] = this.age[index];
        this.backColor[index - 1] = this.backColor[index];
      }
      --this.currentCount;
    }

    private void DrawNotification(int i)
    {
      float scale = 0.6f;
      float num = 1f;
      this.age[i] -= Services.ElapsedTime;
      if ((double) this.age[i] < 0.5)
        num = this.age[i] * 2f;
      else if ((double) this.age[i] > (double) this.initialAge - 0.5)
        num = (float) (((double) this.initialAge - (double) this.age[i]) * 2.0);
      Color color1 = Color.Black * num;
      Color color2 = Color.White * num;
      Color fillColor = this.backColor[i] * num * 0.2f;
      string text = this.text[i];
      if (text == null || text.Length <= 0)
        return;
      Vector2 vector2 = this.font.MeasureString(text) * scale;
      Rectangle rect = new Rectangle(101, 71 + i * 30, (int) vector2.X + 22, 29);
      this.spriteBatch.DrawBox(rect, 1, color2, 0.0f);
      ++rect.X;
      ++rect.Y;
      rect.Width -= 2;
      rect.Height -= 2;
      this.spriteBatch.DrawFilledBox(rect, 1, color1, fillColor);
      this.spriteBatch.DrawString(this.font, text, new Vector2((float) (rect.X + 11), (float) (rect.Y + 2)), color1, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      this.spriteBatch.DrawString(this.font, text, new Vector2((float) (rect.X + 10), (float) (rect.Y + 1)), color2, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
    }
  }
}
