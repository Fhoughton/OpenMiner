// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LoadingScreenBase
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class LoadingScreenBase : MinerToolScreen, IProgressBar
  {
    protected float progressFactor = 1f;
    protected float progressValue;
    protected GameInstance instance;
    protected SpriteBatchSafe spriteBatchPoint;
    protected ProgressTag progressTag;

    string IProgressBar.Text { get; set; }

    float IProgressBar.Progress
    {
      get
      {
        return this.progressValue;
      }
    }

    float IProgressBar.Factor
    {
      get
      {
        return this.progressFactor;
      }
      set
      {
        this.progressFactor = value;
      }
    }

    object IProgressBar.Tag
    {
      get
      {
        return (object) this.progressTag;
      }
      set
      {
        this.progressTag = (ProgressTag) value;
      }
    }

    void IProgressBar.AddProgress(float increment)
    {
      this.progressValue += increment * this.progressFactor;
    }

    void IProgressBar.Reset()
    {
      this.progressValue = 0.0f;
    }

    void IProgressBar.Reset(float value)
    {
      this.progressValue = value;
    }

    public LoadingScreenBase(Player player)
      : base(player)
    {
    }

    public override void LoadContent()
    {
      this.screenRect = this.GraphicsDevice.Viewport.Rectangle();
      base.LoadContent();
      this.Font = CoreGlobals.GameFont;
      this.spriteBatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchPoint);
    }

    protected override void DrawCore()
    {
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      Rectangle rectangle = new Rectangle(145, this.GraphicsDevice.Viewport.Height - 114, this.GraphicsDevice.Viewport.Width - 306, 292);
      this.spriteBatch.BeginTM(this.Matrix);
      Vector2 vector2 = new Vector2((float) rectangle.X, (float) (rectangle.Y - 46));
      Color color = Color.White * this.TransitionAlphaFloat;
      string text = ((IProgressBar) this).Text;
      if (text == null || text.Length < 1)
        text = "Loading...";
      this.spriteBatch.DrawString(this.Font, text, vector2 + new Vector2(2f, 2f) + TMFont.yVec, Color.Black * this.TransitionAlphaFloat, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
      this.spriteBatch.DrawString(this.Font, text, vector2 + new Vector2(0.0f, 0.0f) + TMFont.yVec, color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
      int height = 18;
      this.spriteBatch.DrawFilledBox(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, height), 2, Color.White * this.TransitionAlphaFloat, Color.DarkRed * 0.3f * this.TransitionAlphaFloat);
      float progress = ((IProgressBar) this).Progress;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(rectangle.X + 2, rectangle.Y + 2, (int) ((double) (rectangle.Width - 4) * (double) Math.Min(1f, progress)), height - 4), Color.LawnGreen * this.TransitionAlphaFloat);
      this.spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
