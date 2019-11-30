// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.TextBoxScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;

namespace StudioForge.Engine.GameState
{
  internal class TextBoxScreen : GameScreen
  {
    public Color TextColor = Color.White;
    public Vector2 TextPos;
    protected Rectangle screenRect;
    protected Rectangle scissor;
    private RasterizerState rasterizerState;

    public string Text { get; set; }

    public float TextScale { get; protected set; }

    public TextBoxScreen(string text, float scale, Rectangle rect)
    {
      this.Text = text == null ? "" : text;
      this.TextScale = scale;
      this.screenRect = rect;
      this.scissor = this.screenRect;
      this.scissor.X += 2;
      this.scissor.Y += 2;
      this.scissor.Width -= 4;
      this.scissor.Height -= 4;
      this.TextPos.X = (float) (rect.X + 6);
      this.TextPos.Y = (float) (rect.Y + 4);
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.rasterizerState = new RasterizerState()
      {
        CullMode = CullMode.None,
        ScissorTestEnable = true
      };
    }

    protected override void DrawCore()
    {
      this.SpriteBatch.Begin(this.Matrix);
      this.DrawBackground();
      this.SpriteBatch.End();
      Rectangle scissorRectangle = this.GraphicsDevice.ScissorRectangle;
      this.GraphicsDevice.ScissorRectangle = this.scissor;
      this.SpriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, (SamplerState) null, (DepthStencilState) null, this.rasterizerState, (Effect) null, this.Matrix);
      this.DrawText();
      this.SpriteBatch.End();
      this.GraphicsDevice.ScissorRectangle = scissorRectangle;
    }

    protected virtual void DrawBackground()
    {
      this.SpriteBatch.DrawFilledBox(this.screenRect, 2, this.borderColor, this.backColor);
    }

    protected virtual void DrawText()
    {
      this.SpriteBatch.DrawString(this.Font, this.Text, this.TextPos, this.TextColor, 0.0f, Vector2.Zero, this.TextScale, SpriteEffects.None, 0.0f);
    }
  }
}
