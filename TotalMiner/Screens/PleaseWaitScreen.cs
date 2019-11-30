// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.PleaseWaitScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class PleaseWaitScreen : GameScreen
  {
    public string Msg1;
    public string Msg2;
    private SpriteBatchSafe spriteBatch;
    private Action action;
    private int drawCount;

    public PleaseWaitScreen(string msg1, string msg2)
      : this(msg1, msg2, (Action) null)
    {
    }

    public PleaseWaitScreen(string msg1, string msg2, Action action)
      : this(msg1, msg2, action, true)
    {
    }

    public PleaseWaitScreen(string msg1, string msg2, Action action, bool popup)
    {
      this.IsPopup = popup;
      this.Msg1 = msg1;
      this.Msg2 = msg2;
      this.action = action;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.Font = CoreGlobals.GameFont;
    }

    protected override void DrawCore()
    {
      Vector2 vector2_1 = this.Font.MeasureString(this.Msg1);
      Vector2 vector2_2 = this.Font.MeasureString(this.Msg2) * 0.7f;
      Rectangle boxRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, (int) ((double) Math.Max(vector2_1.X, vector2_2.X) + 96.0), (int) ((double) vector2_1.Y + (double) vector2_2.Y + 80.0));
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, boxRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.Begin();
      if (this.Msg1 != null)
        this.spriteBatch.DrawString(this.Font, this.Msg1, new Vector2((float) (boxRect.X + 48), (float) (boxRect.Y + 32)) + TMFont.yVec, Color.WhiteSmoke);
      if (this.Msg2 != null)
        this.spriteBatch.DrawString(this.Font, this.Msg2, new Vector2((float) (boxRect.X + 48), (float) ((double) (boxRect.Y + 32) + (double) vector2_1.Y + 16.0)) + TMFont.yVec, Color.WhiteSmoke, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      this.spriteBatch.End();
      if (++this.drawCount != 2 || this.action == null)
        return;
      this.action();
    }
  }
}
