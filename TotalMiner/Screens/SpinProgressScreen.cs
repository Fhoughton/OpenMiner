// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SpinProgressScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner.Screens
{
  internal class SpinProgressScreen : GameScreen
  {
    private Texture2D spinTexture;
    private SpriteBatchSafe spriteBatch;
    private float rotation;
    private Vector2 off;
    private string text;
    private float scale;

    public string Text
    {
      get
      {
        return this.text;
      }
      set
      {
        this.text = value;
      }
    }

    public SpinProgressScreen(string text, float scale)
    {
      this.IsPopup = true;
      this.text = text;
      this.scale = scale;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.Font = CoreGlobals.GameFont;
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.spinTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\spin");
      this.off = new Vector2((float) (this.spinTexture.Width / 2), (float) (this.spinTexture.Height / 2));
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
    }

    protected override void DrawCore()
    {
      Vector2 vector2 = this.Font.MeasureString(this.text) * this.scale;
      Rectangle boxRect = MyExtensions.CenterOfViewport(this.GraphicsDevice.Viewport, (int) vector2.X + 96, (int) vector2.Y + 96);
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, boxRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.spriteBatch.End();
      this.spriteBatch.Begin();
      this.spriteBatch.DrawString(this.Font, this.text, new Vector2((float) (boxRect.X + 48), (float) (boxRect.Y + 48)), Color.WhiteSmoke, 0.0f, Vector2.Zero, this.scale, SpriteEffects.None, 0.0f);
      this.spriteBatch.Draw(this.spinTexture, new Vector2((float) (boxRect.X + 48 + (int) vector2.X - this.spinTexture.Width), (float) (boxRect.Y + 48 + (int) vector2.Y - this.spinTexture.Height - 4)), new Rectangle?(), Color.White, this.rotation, this.off, 2f, SpriteEffects.None, 0.0f);
      this.spriteBatch.End();
      this.rotation += 0.05f;
    }
  }
}
