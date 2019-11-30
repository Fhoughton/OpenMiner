// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Renderers.DialogRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;
using System;

namespace StudioForge.TotalMiner.Renderers
{
  internal class DialogRenderer
  {
    private Color backColor = new Color(0.2f, 0.2f, 0.2f) * 0.8f;
    private DialogHandler dialog;
    private SpriteBatchSafe spriteBatch;
    private SpriteBatchSafe spriteBatchPoint;
    private SpriteBatchSafe spriteBatchText;
    private Texture2D gradient;
    private RasterizerState rasterState;

    public void LoadContent()
    {
      this.spriteBatch = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchText = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.gradient = CoreGlobals.Content.Load<Texture2D>("Textures\\gradient");
      this.rasterState = new RasterizerState()
      {
        ScissorTestEnable = true
      };
    }

    public void UnloadContent()
    {
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchText);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchPoint);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatch);
    }

    public void Draw(Player player, Player virtualPlayer)
    {
      this.dialog = virtualPlayer.DialogHandler;
      if (this.dialog == null)
        return;
      if (this.dialog.CurrentDialog != null)
      {
        this.DrawDirectDialog(player);
      }
      else
      {
        if (this.dialog.SpeechText == null && this.dialog.ReticleText == null)
          return;
        this.DrawIndirectDialog(player);
      }
    }

    private void DrawIndirectDialog(Player player)
    {
      Matrix screenMatrix = player.GetScreenMatrix(ScreenForScale.Hud);
      this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, screenMatrix);
      this.spriteBatchText.BeginTM(screenMatrix);
      if (this.dialog.ReticleText.IsNotEmpty())
      {
        Rectangle reticleTextRect = this.dialog.ReticleTextRect;
        --reticleTextRect.Y;
        reticleTextRect.Width /= 2;
        reticleTextRect.Height = 1;
        this.spriteBatchPoint.Draw(this.gradient, reticleTextRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
        ++reticleTextRect.Y;
        reticleTextRect.Height = this.dialog.ReticleTextRect.Height;
        this.spriteBatchPoint.Draw(this.gradient, reticleTextRect, new Rectangle?(), this.backColor, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
        reticleTextRect.Y += this.dialog.ReticleTextRect.Height;
        reticleTextRect.Height = 1;
        this.spriteBatchPoint.Draw(this.gradient, reticleTextRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
        reticleTextRect.X += reticleTextRect.Width;
        this.spriteBatchPoint.Draw(this.gradient, reticleTextRect, Color.White);
        reticleTextRect.Height = this.dialog.ReticleTextRect.Height;
        reticleTextRect.Y -= reticleTextRect.Height;
        this.spriteBatchPoint.Draw(this.gradient, reticleTextRect, this.backColor);
        reticleTextRect.Height = 1;
        --reticleTextRect.Y;
        this.spriteBatchPoint.Draw(this.gradient, reticleTextRect, new Rectangle?(), Color.White);
        Rectangle rect = new Rectangle(0, this.dialog.ReticleTextRect.Y + 5, player.Viewport.Width, 0);
        this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, this.dialog.ReticleText, rect, Color.White, 0.6f);
        if (this.dialog.CanTalkCached)
        {
          int num = (int) ((double) CoreGlobals.GameFont.MeasureString(this.dialog.Npc.Name).X * 0.600000023841858);
          rect.X = (rect.Width - num) / 2 + rect.X + num - 25;
          rect.Y = this.dialog.ReticleTextRect.Y + 8;
          rect.Width = rect.Height = 20;
          GraphicStatics.DrawInputIcon(this.spriteBatchPoint, PlayerInput.Interact, rect);
        }
      }
      if (this.dialog.SpeechText.IsNotEmpty())
        this.DrawNpcSpeech();
      this.spriteBatchPoint.End();
      this.spriteBatchText.End();
    }

    private void DrawDirectDialog(Player player)
    {
      Matrix screenMatrix = player.GetScreenMatrix(ScreenForScale.Hud);
      this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, screenMatrix);
      this.spriteBatchText.BeginTM(this.rasterState, screenMatrix);
      Rectangle speechMenuRect = this.dialog.SpeechMenuRect;
      if (this.dialog.SpeechText != null && speechMenuRect.Y + speechMenuRect.Height > this.dialog.SpeechTextRect.Y - 8)
        speechMenuRect.Y = Math.Max(this.dialog.SpeechTextRect.Y - 8 - speechMenuRect.Height, GraphicStatics.HUDPos().Y + 4);
      this.spriteBatchText.GraphicsDevice.ScissorRectangle = speechMenuRect;
      speechMenuRect.Width /= 2;
      speechMenuRect.Y -= 2;
      this.spriteBatchPoint.Draw(this.gradient, speechMenuRect, new Rectangle?(), this.backColor, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
      speechMenuRect.X += speechMenuRect.Width;
      this.spriteBatchPoint.Draw(this.gradient, speechMenuRect, this.backColor);
      this.spriteBatchPoint.Draw(CoreGlobals.BlankTexture, new Rectangle(this.dialog.SpeechMenuRect.Width / 2 + this.dialog.SpeechMenuRect.X, speechMenuRect.Y, 1, speechMenuRect.Height), Color.White);
      speechMenuRect.Y += 2;
      Vector2 vector2 = CoreGlobals.GameFont.MeasureString(this.dialog.Npc.Name) * 0.7f;
      int num1 = (int) ((double) speechMenuRect.Height * 0.5 - (double) vector2.Y * 0.5 + (double) speechMenuRect.Y + 1.0);
      Vector2 position = new Vector2((float) ((double) (this.dialog.SpeechMenuRect.Width / 2 - 24) - (double) vector2.X + (double) this.dialog.SpeechMenuRect.X + 1.0), (float) (num1 + 1));
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, this.dialog.Npc.Name, position + Vector2.One, Color.Black, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, this.dialog.Npc.Name, position, Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f);
      float num2 = 1f;
      float num3 = 0.3f;
      int num4 = 0;
      int num5 = num1;
      int maxWidth = (GraphicStatics.HUDPos().Width - GraphicStatics.HUDPos().X) / 2 - 60;
      DialogNode dialogFirstChild = this.dialog.CurrentDialogFirstChild;
      DialogNode nextSibling;
      DialogNode dialogNode1;
      for (dialogNode1 = nextSibling = Node.FindNextSibling(typeof (DialogNode), (Node) dialogFirstChild, this.dialog.SelectedNodeIndex) as DialogNode; nextSibling != null && num4 < 3; ++num4)
      {
        vector2 = CoreGlobals.GameFont.MeasureString(nextSibling.Text) * 0.6f;
        if ((double) vector2.X > (double) maxWidth)
          nextSibling.Text = Utils.InsertNewLines(CoreGlobals.GameFont, maxWidth, 0.6f, nextSibling.Text, true);
        if (num4 == 0)
          this.spriteBatchPoint.Draw(this.gradient, new Rectangle(speechMenuRect.Width + 1 + this.dialog.SpeechMenuRect.X, num5 - 1, speechMenuRect.Width - 2, (int) vector2.Y + 4), Color.White * 0.3f);
        position = new Vector2((float) (speechMenuRect.Width + 16 + this.dialog.SpeechMenuRect.X), (float) (num5 + 1));
        this.spriteBatchText.DrawString(CoreGlobals.GameFont, nextSibling.Text, position + Vector2.One, Color.Black * num2, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        if (!nextSibling.IsRead)
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, nextSibling.Text, position, Color.White * num2, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        num5 += (int) ((double) vector2.Y + 8.0);
        nextSibling = Node.FindNextSibling(typeof (DialogNode), (Node) nextSibling) as DialogNode;
        num2 -= num3;
        num3 += 0.1f;
      }
      if (num4 == 0)
      {
        this.spriteBatchPoint.Draw(CoreGlobals.BlankTexture, new Rectangle(this.dialog.SpeechMenuRect.Width / 2 + 1 + this.dialog.SpeechMenuRect.X, num5 - 1, this.dialog.SpeechMenuRect.Width / 2 - 2, (int) vector2.Y), Color.White * 0.3f);
        position = new Vector2((float) (this.dialog.SpeechMenuRect.Width / 2 + 16 + this.dialog.SpeechMenuRect.X), (float) (num5 + 1));
        this.spriteBatchText.DrawString(CoreGlobals.GameFont, "Back", position + Vector2.One, Color.Black, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.spriteBatchText.DrawString(CoreGlobals.GameFont, "Back", position, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      }
      else
      {
        float num6 = 0.6f;
        int num7 = num1;
        int num8 = Math.Min(2, this.dialog.SelectedNodeIndex);
        DialogNode dialogNode2 = dialogNode1;
        for (; num8 > 0; --num8)
        {
          dialogNode2 = Node.FindPrevSibling(typeof (DialogNode), (Node) dialogNode2) as DialogNode;
          if (dialogNode2 != null)
          {
            vector2 = CoreGlobals.GameFont.MeasureString(dialogNode2.Text) * 0.6f;
            num7 -= (int) ((double) vector2.Y + 8.0);
            position = new Vector2((float) (this.dialog.SpeechMenuRect.Width / 2 + 16 + this.dialog.SpeechMenuRect.X), (float) (num7 + 1));
            this.spriteBatchText.DrawString(CoreGlobals.GameFont, dialogNode2.Text, position + Vector2.One, Color.Black * num6, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
            if (!dialogNode2.IsRead)
              this.spriteBatchText.DrawString(CoreGlobals.GameFont, dialogNode2.Text, position, Color.White * num6, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
            num6 -= 0.2f;
          }
          else
            break;
        }
      }
      this.spriteBatchPoint.End();
      this.spriteBatchText.End();
      this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, screenMatrix);
      this.spriteBatchText.BeginTM(screenMatrix);
      if (this.dialog.SpeechText.IsNotEmpty())
        this.DrawNpcSpeech();
      DialogNode currentDialogParent = this.dialog.CurrentDialogParent;
      if (currentDialogParent == null || !currentDialogParent.DisableBackButton)
      {
        string text = currentDialogParent == null ? "Exit" : "Back";
        vector2 = CoreGlobals.GameFont.MeasureString(text);
        speechMenuRect.X = speechMenuRect.Width * 2 - 64 - (int) ((double) vector2.X * 0.600000023841858);
        speechMenuRect.Y += speechMenuRect.Height + 4;
        speechMenuRect.Width = speechMenuRect.Height = 24;
        GraphicStatics.DrawInputIcon(this.spriteBatchPoint, PlayerInput.BackButton, speechMenuRect);
        speechMenuRect.X += speechMenuRect.Width + 6;
        this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, text, new Vector2((float) (speechMenuRect.X + 1), (float) speechMenuRect.Y), Color.Black, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, text, new Vector2((float) speechMenuRect.X, (float) (speechMenuRect.Y - 1)), Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      }
      this.spriteBatchPoint.End();
      this.spriteBatchText.End();
    }

    private void DrawNpcSpeech()
    {
      Rectangle speechTextRect = this.dialog.SpeechTextRect;
      speechTextRect.Width /= 2;
      this.spriteBatchPoint.Draw(this.gradient, speechTextRect, new Rectangle?(), this.backColor, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
      speechTextRect.X += speechTextRect.Width;
      this.spriteBatchPoint.Draw(this.gradient, speechTextRect, this.backColor);
      speechTextRect.X = 1;
      speechTextRect.Y = this.dialog.SpeechTextRect.Y + 9;
      speechTextRect.Width = this.dialog.Player.Viewport.Width;
      this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, this.dialog.SpeechText, speechTextRect, Color.Black, 0.6f);
      --speechTextRect.X;
      speechTextRect.Y = this.dialog.SpeechTextRect.Y + 8;
      this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, this.dialog.SpeechText, speechTextRect, Color.White, 0.6f);
      if (!this.dialog.DrawIndirectSpeechBackButton)
        return;
      float scale = 0.45f;
      int width = GraphicStatics.HUDPos().Width;
      Vector2 vector2 = CoreGlobals.GameFont.MeasureString(this.dialog.SpeechTextNpcName) * scale;
      Vector2 position = new Vector2((float) width - Math.Max(80f, vector2.X), (float) (this.dialog.SpeechTextRect.Y + this.dialog.SpeechTextRect.Height + 4));
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, this.dialog.SpeechTextNpcName, position + Vector2.One, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, this.dialog.SpeechTextNpcName, position, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      position.Y += vector2.Y + 2f;
      speechTextRect.X = (int) position.X;
      speechTextRect.Y = (int) position.Y + 1;
      speechTextRect.Width = speechTextRect.Height = 16;
      position.X += 20f;
      GraphicStatics.DrawInputIcon(this.spriteBatchPoint, PlayerInput.BackButton, speechTextRect);
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, "Close", position + Vector2.One, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, "Close", position, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
    }
  }
}
