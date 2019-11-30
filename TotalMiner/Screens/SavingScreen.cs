// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SavingScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class SavingScreen : GameScreen, IProgressBar
  {
    private float progressValue;
    private float progressFactor;
    private MapSaveWorker saver;
    private GameInstance instance;
    private ProgressTag progressTag;
    private SpriteBatchSafe spriteBatchPoint;
    private Action<bool, bool> callback;

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

    string IProgressBar.Text { get; set; }

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

    public SavingScreen(GameInstance instance, Player player, Action<bool, bool> callback)
    {
      this.instance = instance;
      this.callback = callback;
      this.TransitionOnTime = TimeSpan.Zero;
      this.progressValue = 0.0f;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.Font = CoreGlobals.GameFont;
      this.spriteBatchPoint = this.ScreenManager.SpriteBatch;
      this.saver = new MapSaveWorker(this.instance, false, (IProgressBar) this, new Action<bool, bool>(this.OnSaveComplete));
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.saver, false, PriorityLevel.Priority);
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
    }

    private void OnSaveComplete(bool success, bool anotherSaveInProgress)
    {
      this.ExitScreen();
      if (this.callback == null)
        return;
      this.callback(success, anotherSaveInProgress);
    }

    protected override void DrawCore()
    {
      SpriteBatchSafe spriteBatch = this.ScreenManager.SpriteBatch;
      Rectangle boxRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, 768, 240);
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, boxRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      spriteBatch.End();
      spriteBatch.BeginTM(this.Matrix);
      int x = boxRect.X + 96;
      int num = boxRect.Y + 80;
      Vector2 position = new Vector2((float) x, (float) (num - 10));
      Color color = Color.White * this.TransitionAlphaFloat;
      spriteBatch.DrawString(CoreGlobals.GameFont, "Saving...", position, color);
      spriteBatch.DrawString(CoreGlobals.GameFont, Globals2.GameProperties.SaveGame.Header.MapName, position + new Vector2(144f, 5f), color, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      int height = 18;
      int width = boxRect.Width - 96 - 96;
      spriteBatch.DrawBox(new Rectangle(x, num + 70, width, height), 2, Color.White * this.TransitionAlphaFloat, 0.0f);
      spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(x + 2, num + 72, (int) ((double) (width - 4) * (double) Math.Min(1f, this.progressValue)), height - 4), Color.Green * this.TransitionAlphaFloat);
      spriteBatch.End();
      ++CoreGlobals.FrameRateCounter.SpriteCalls;
    }
  }
}
