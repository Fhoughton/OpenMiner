// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Renderers.HudRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Renderers
{
  internal class HudRenderer : DrawableGameObjectBase
  {
    private float barAlpha = 0.5f;
    private string lastLabel = "";
    private int lastTargetIndex = -1;
    private List<NpcBase> mobs = new List<NpcBase>();
    private Map map;
    private Texture2D cursorTexture;
    private Texture2D maniconTexture;
    private Texture2D circleTexture;
    private Texture2D waypointTexture;
    private Texture2D graveyardTexture;
    private Texture2D talkingTexture;
    private SpriteBatchSafe spriteBatch;
    private SpriteBatchSafe spriteBatchPoint;
    private SpriteBatchSafe spriteBatchText;
    private Vector2 curOrigin;
    private GameInstance instance;
    private Rectangle healthBorderRect;
    private Rectangle healthBarRect;
    private Rectangle oxygenBorderRect;
    private Rectangle oxygenBarRect;
    private Rectangle talkRect;
    private Rectangle hudPos;
    private PropertyToString<int> gamerCount;
    private Actor lastTarget;

    public HudRenderer(GameInstance instance)
    {
      this.instance = instance;
      this.map = (Map) instance.Map;
    }

    protected override void LoadContentCore(InitState state)
    {
      this.spriteBatch = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.spriteBatchText = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.cursorTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\Cursor2");
      this.maniconTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\manicon");
      this.circleTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\circlefull");
      this.waypointTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\waypoint");
      this.graveyardTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\Graveyard");
      this.talkingTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\Talking");
      this.talkRect = new Rectangle(0, 0, this.talkingTexture.Width, this.talkingTexture.Height);
      this.curOrigin = new Vector2((float) this.cursorTexture.Width / 2f, (float) this.cursorTexture.Height / 2f);
    }

    protected override void UnloadContentCore()
    {
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatch);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchPoint);
      GraphicStatics.SpriteBatchPool.Release(this.spriteBatchText);
      base.UnloadContentCore();
    }

    protected override void DrawCore(DrawState state)
    {
    }

    public void Draw(Player player, Player virtualPlayer)
    {
      this.hudPos = GraphicStatics.HUDPos(player);
      if (virtualPlayer.Settings.HudVisible)
        this.DrawEnabledHUD(player, virtualPlayer);
      else
        this.DrawDisabledHUD(player, virtualPlayer);
    }

    private void DrawDisabledHUD(Player player, Player virtualPlayer)
    {
      if (virtualPlayer == player || !player.Settings.HudVisible)
        return;
      this.spriteBatchText.BeginTM(player.GetScreenMatrix(ScreenForScale.Hud));
      string text = !player.IsCCTVView ? virtualPlayer.DisplayGamertag : "[CCTV Press    to Exit]";
      Vector2 position = new Vector2((float) (player.Viewport2.Width / 2) - (float) ((double) CoreGlobals.GameFont.MeasureString(text).X * 0.800000011920929 * 0.5), (float) (player.Viewport.TitleSafeArea.Y + 20));
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, text, position, Color.Black, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, text, position + Vector2.One, Color.Yellow, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
      if (player.IsCCTVView)
        GraphicStatics.DrawInputIcon(this.spriteBatchText, PlayerInput.BackButton, new Rectangle((int) ((double) position.X + 194.0), (int) position.Y + 6, 24, 24));
      this.spriteBatchText.End();
    }

    private void DrawEnabledHUD(Player player, Player virtualPlayer)
    {
      Matrix screenMatrix = player.GetScreenMatrix(ScreenForScale.Hud);
      if (!player.IsAssemblingPhoto)
      {
        this.healthBorderRect = new Rectangle(this.hudPos.X, this.hudPos.Height - (this.instance.IsCreativeMode ? 72 : 52), 250, 20);
        this.healthBarRect = new Rectangle(this.healthBorderRect.X + 2, this.healthBorderRect.Y + 2, 0, this.healthBorderRect.Height - 4);
        this.oxygenBorderRect = new Rectangle(this.healthBorderRect.X, this.healthBorderRect.Y - this.healthBorderRect.Height - 6, this.healthBorderRect.Width, this.healthBorderRect.Height);
        this.oxygenBarRect = new Rectangle(this.oxygenBorderRect.X + 2, this.oxygenBorderRect.Y + 2, 0, this.oxygenBorderRect.Height - 4);
        this.spriteBatchText.BeginTM(screenMatrix);
        this.spriteBatchPoint.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, screenMatrix);
        this.spriteBatch.BeginTM(screenMatrix);
        Color white = Color.White;
        Vector2 vector2 = new Vector2((float) (this.healthBarRect.X + 2), (float) (this.healthBarRect.Y + this.healthBarRect.Height + 10));
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.healthBorderRect.X, this.healthBorderRect.Y + this.healthBorderRect.Height + 4, this.healthBorderRect.Width, this.instance.IsCreativeMode ? 48 : 28), Color.Black * 0.4f);
        if (this.instance.IsDigDeepMode)
        {
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, virtualPlayer.DepthString.ToString(), vector2 + TMFont.yVec, white, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
          float num = CoreGlobals.GameFont.MeasureString(virtualPlayer.DepthString.ToString()).X * 0.7f;
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, virtualPlayer.PosString, vector2 + new Vector2(num + 16f, 0.0f) + TMFont.yVec, white, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
          vector2.Y += 25f;
        }
        else
        {
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, virtualPlayer.PosString, vector2 + TMFont.yVec, white, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
          if (this.instance.IsCreativeMode)
          {
            vector2.Y += 25f;
            this.spriteBatchText.DrawString(CoreGlobals.GameFont, virtualPlayer.CursorString, vector2 + TMFont.yVec, white, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
          }
        }
        bool flag = !this.instance.IsGamePaused;
        if (this.instance.IsCombatEnabled)
        {
          this.DrawHealthBar(virtualPlayer);
          this.DrawOxygenBar(virtualPlayer);
        }
        if ((double) virtualPlayer.BloodTint > 0.0)
          this.spriteBatchText.Draw(CoreGlobals.BlankTexture, new Rectangle(0, 0, virtualPlayer.Viewport.Width, virtualPlayer.Viewport.Height + 20), Color.DarkRed * virtualPlayer.BloodTint);
        Rectangle rect = new Rectangle(this.hudPos.X + 2, virtualPlayer.IsUnderWater ? this.oxygenBorderRect.Y - 70 : this.healthBorderRect.Y - 70, 0, 0);
        int num1 = rect.Y + 70;
        if (virtualPlayer.Settings.BlueprintFinderVisible)
        {
          RenderTarget2D finderRenderTarget = GameplayScreen.ScreenInstance.GetBlueprintFinderRenderTarget(player.ScreenID);
          if (finderRenderTarget != null)
          {
            rect.Width = finderRenderTarget.Width;
            rect.Height = finderRenderTarget.Height;
            num1 = rect.Y;
            this.DrawBox(this.spriteBatch, rect, 2, Color.White);
            this.spriteBatch.Draw((Texture2D) finderRenderTarget, rect, Color.White);
            rect.X += rect.Width + 10;
          }
        }
        rect.Y += 12;
        rect.Width = 48;
        rect.Height = 48;
        if (virtualPlayer.ClanBannerID > 0)
        {
          this.DrawBox(this.spriteBatch, rect, 2, Color.White);
          Rectangle clanBannerRect = GraphicStatics.GetClanBannerRect((byte) virtualPlayer.ClanBannerID);
          this.spriteBatchPoint.Draw(GraphicStatics.ClanBanners, rect, new Rectangle?(clanBannerRect), Color.White);
          rect.X += rect.Width + 10;
          if (num1 > rect.Y)
            num1 = rect.Y;
        }
        if (virtualPlayer.LeftHand.HasItem)
        {
          this.DrawFilledBox(this.spriteBatch, rect, 2, Color.White, Color.Black * 0.7f);
          --rect.X;
          GraphicStatics.DrawItem(this.spriteBatch, this.spriteBatchPoint, this.spriteBatchText, rect.X, rect.Y, virtualPlayer.Inventory.LeftHand, true, false, 1f);
          rect.X += rect.Width + 10;
          if (num1 > rect.Y)
            num1 = rect.Y;
        }
        if (virtualPlayer.RightHand.HasItem)
        {
          this.DrawFilledBox(this.spriteBatch, rect, 2, Color.White, Color.Black * 0.7f);
          --rect.X;
          GraphicStatics.DrawItem(this.spriteBatch, this.spriteBatchPoint, this.spriteBatchText, rect.X, rect.Y, virtualPlayer.Inventory.RightHand, true, false, 1f);
          rect.X += rect.Width + 10;
          if (num1 > rect.Y)
            num1 = rect.Y;
        }
        this.DrawHUDElements(player, virtualPlayer);
        this.DrawTextMessages(rect.Y - (virtualPlayer.Settings.BlueprintFinderVisible ? 16 : 4));
        if (this.instance.IsSkillsEnabled)
        {
          if (num1 > rect.Y)
            num1 = rect.Y;
          this.DrawSkillData(virtualPlayer, ref rect, virtualPlayer.SkillsData.MostRecentlyUsedSkill);
        }
        if (this.instance.IsMultiplayer)
        {
          this.gamerCount.Value = NetworkManager.Instance.AllGamerCount;
          int x = this.healthBorderRect.X;
          int y = num1 - this.maniconTexture.Height - 5;
          this.spriteBatch.Draw(this.maniconTexture, new Rectangle(x, y, this.maniconTexture.Width, this.maniconTexture.Height), Color.White);
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, this.gamerCount.ToString(), new Vector2((float) (x + this.maniconTexture.Width + 6), (float) (y + 4)) + TMFont.yVec * 0.6f, white, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        }
        if (player.HasPermission(Permissions.Adventure))
          this.DrawCompass(player, virtualPlayer);
        this.talkRect.X = this.hudPos.Width - this.talkRect.Width;
        this.talkRect.Y = this.hudPos.Height - this.talkRect.Height;
        foreach (NetworkGamer allGamer in this.instance.NetworkManager.AllGamers)
        {
          if (allGamer.IsTalking)
          {
            Player tag = allGamer.Tag as Player;
            if (tag != null && tag.HasPermission(Permissions.VoiceChat))
            {
              this.spriteBatch.Draw(this.talkingTexture, this.talkRect, Color.White);
              float num2 = CoreGlobals.GameFont.MeasureString(allGamer.Gamertag).X * 0.5f;
              this.spriteBatch.DrawString(CoreGlobals.GameFont, allGamer.Gamertag, new Vector2((float) (this.talkRect.X - 8) - num2, (float) (this.talkRect.Y + 1)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
              this.talkRect.Y -= this.talkRect.Height + 4;
            }
          }
        }
        if (this.instance.MiniGame != null)
        {
          float endTime = this.instance.MiniGame.EndTime;
          TimeSpan timeSpan = TimeSpan.FromSeconds((double) endTime > 0.0 ? (double) endTime - (double) this.instance.MiniGame.Elapsed : (double) this.instance.MiniGame.Elapsed);
          this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, "Time: " + string.Format("{0:D2}:{1:D2}", (object) timeSpan.Minutes, (object) timeSpan.Seconds), 80f, Color.White, 1f);
          if (this.instance.MiniGame.Leader != null)
            this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, "Leader: " + this.instance.MiniGame.Leader.DisplayGamertag, 115f, Color.Yellow, 0.7f);
        }
        if (virtualPlayer != player)
        {
          int num2 = player.Settings.CompassTop ? this.hudPos.Y + 42 : this.hudPos.Y + 10;
          this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, virtualPlayer.DisplayGamertag, (float) (num2 + 1), Color.Black, 0.8f);
          this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, virtualPlayer.DisplayGamertag, (float) num2, Color.Yellow, 0.8f);
        }
        else if (player.ActionRequests.Count > 0)
        {
          int num2 = GraphicStatics.HUDPos(player).Height - 150;
          double totalSeconds = Globals1.ElapsedWatch.Elapsed.TotalSeconds;
          for (int index = player.ActionRequests.Count - 1; index >= 0; --index)
          {
            Player.ActionRequest actionRequest = player.ActionRequests[index];
            if (actionRequest.Seconds + actionRequest.SecondsHidden < totalSeconds)
            {
              this.spriteBatchText.DrawStringCentered(CoreGlobals.GameFont, actionRequest.Message, (float) num2, Color.DeepSkyBlue, 0.7f);
              num2 -= 26;
            }
          }
        }
        float Y = (float) (player.Viewport.Height / 2 + 20);
        Player.ButtonScript? buttonScript1 = virtualPlayer.GetButtonScript(Buttons.X);
        if (buttonScript1.HasValue && buttonScript1.Value.Text.IsNotEmpty())
          Y = this.DrawButtonText(player, virtualPlayer, PlayerInput.EventScriptX, Y, buttonScript1.Value);
        Player.ButtonScript? buttonScript2 = virtualPlayer.GetButtonScript(Buttons.Y);
        if (buttonScript2.HasValue && buttonScript2.Value.Text.IsNotEmpty())
          Y = this.DrawButtonText(player, virtualPlayer, PlayerInput.EventScriptY, Y, buttonScript2.Value);
        Player.ButtonScript? buttonScript3 = virtualPlayer.GetButtonScript(Buttons.B);
        if (buttonScript3.HasValue && buttonScript3.Value.Text.IsNotEmpty())
          this.DrawButtonText(player, virtualPlayer, PlayerInput.EventScriptB, Y, buttonScript3.Value);
        this.spriteBatch.End();
        this.spriteBatchPoint.End();
        this.spriteBatchText.End();
        if (player.MiniMapRenderer != null && player.MiniMapRenderer.IsEnabled && player.Settings.MapVisible)
        {
          Matrix matrix = screenMatrix;
          int num2 = 1;
          int num3 = 1;
          if (this.instance.LocalPlayerCount == 2 && !Globals2.GameSettings.SplitScreenVertical)
          {
            matrix.M11 = 0.5f;
            matrix.M22 = 0.5f;
            matrix.M33 = 0.5f;
            Viewport viewport = player.Viewport;
            if (viewport.X == 0)
              num2 = 2;
            if (viewport.Y == 0)
              num3 = 2;
          }
          this.spriteBatch.BeginTM(matrix);
          RenderTarget2D renderTarget = player.MiniMapRenderer.RenderTarget;
          if (renderTarget != null)
          {
            Rectangle winRect = player.MiniMapRenderer.WinRect;
            winRect.X = this.hudPos.X * num2;
            winRect.Y = this.hudPos.Y * num3;
            this.spriteBatch.Draw((Texture2D) renderTarget, winRect, Color.White);
          }
          this.spriteBatch.End();
        }
        if (flag)
        {
          this.spriteBatch.Begin(screenMatrix);
          this.spriteBatch.Draw(this.cursorTexture, new Vector2((float) player.Viewport.Width / 2f, (float) player.Viewport.Height / 2f), new Rectangle?(), Color.White, 0.0f, this.curOrigin, 1f, SpriteEffects.None, 0.0f);
          this.spriteBatch.End();
        }
      }
      if (!player.Inventory.IsEquippedInHand(Item.Camera))
        return;
      this.spriteBatch.BeginTM(screenMatrix);
      this.spriteBatch.DrawBox(player.CamRect.Expand(1), 2, Color.White, 0.0f);
      this.spriteBatch.End();
    }

    private float DrawButtonText(
      Player player,
      Player virtualPlayer,
      PlayerInput input,
      float Y,
      Player.ButtonScript script)
    {
      float scale = script.Scale.HasValue ? script.Scale.Value : 0.7f;
      Vector2 vector2 = CoreGlobals.GameFont.MeasureString(script.Text);
      vector2.X *= scale;
      vector2.Y *= scale;
      Y += (float) ((double) vector2.Y * 0.5 + 1.0);
      Rectangle rect = new Rectangle();
      if (script.Pos.HasValue)
      {
        rect.X = (int) script.Pos.Value.X;
        rect.Y = (int) script.Pos.Value.Y;
      }
      else
      {
        rect.X = (int) ((double) (player.Viewport.Width / 2) - (double) vector2.X * 0.5 - 14.0);
        rect.Y = (int) Y;
      }
      rect.Width = 20;
      rect.Height = 20;
      GraphicStatics.DrawInputIcon(this.spriteBatchPoint, input, rect);
      rect.X += 28;
      rect.Y = (int) ((double) (rect.Y + 10) - (double) vector2.Y * 0.5 + 3.0);
      Vector2 position = new Vector2((float) rect.X, (float) rect.Y);
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, script.Text, position, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      --position.X;
      --position.Y;
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, script.Text, position, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      return (float) ((double) Y + (double) vector2.Y * 0.5 + 1.0);
    }

    private void DrawBox(SpriteBatchSafe spriteBatch, Rectangle rect, int thickness, Color color)
    {
      rect.X -= thickness;
      rect.Y -= thickness;
      rect.Width += thickness + thickness;
      rect.Height += thickness + thickness;
      spriteBatch.DrawBox(rect, thickness, color, 0.0f);
    }

    private void DrawFilledBox(
      SpriteBatchSafe spriteBatch,
      Rectangle rect,
      int thickness,
      Color color1,
      Color color2)
    {
      rect.X -= thickness;
      rect.Y -= thickness;
      rect.Width += thickness + thickness;
      rect.Height += thickness + thickness;
      spriteBatch.DrawFilledBox(rect, thickness, color1, color2);
    }

    private void DrawHUDElements(Player player, Player virtualPlayer)
    {
      this.DrawHUDElements(player, this.instance.HUDElementManager.HUDElements);
      this.DrawHUDElements(player, virtualPlayer.HUDElementManager.HUDElements);
    }

    private void DrawHUDElements(Player player, List<HUDElement> hudElements)
    {
      for (int index = 0; index < hudElements.Count; ++index)
      {
        HUDElement hudElement = hudElements[index];
        HUDRect element1 = hudElement as HUDRect;
        if (element1 != null)
        {
          this.DrawHUDRect(player, element1);
        }
        else
        {
          HUDProgressBar element2 = hudElement as HUDProgressBar;
          if (element2 != null)
          {
            this.DrawHUDBar(player, element2);
          }
          else
          {
            HUDCounter element3 = hudElement as HUDCounter;
            if (element3 != null)
            {
              this.DrawHUDCounter(player, element3);
            }
            else
            {
              HUDText element4 = hudElement as HUDText;
              if (element4 != null)
                this.DrawHUDText(player, element4);
            }
          }
        }
      }
    }

    private Rectangle GetElementRect(Rectangle rect)
    {
      Viewport defaultViewport = GraphicStatics.DefaultViewport;
      if (rect.X <= defaultViewport.Width / 2)
        rect.X += this.hudPos.X;
      else
        rect.X = this.hudPos.Width - (defaultViewport.Width - rect.X);
      if (rect.Y <= defaultViewport.Height / 2)
        rect.Y += this.hudPos.Y;
      else
        rect.Y = this.hudPos.Height - (defaultViewport.Height - rect.Y);
      return rect;
    }

    private Vector2 GetElementPos(Vector2 pos)
    {
      Viewport defaultViewport = GraphicStatics.DefaultViewport;
      if ((double) pos.X <= (double) (defaultViewport.Width / 2))
        pos.X += (float) this.hudPos.X;
      else
        pos.X = (float) this.hudPos.Width - ((float) defaultViewport.Width - pos.X);
      if ((double) pos.Y <= (double) (defaultViewport.Height / 2))
        pos.Y += (float) this.hudPos.Y;
      else
        pos.Y = (float) this.hudPos.Height - ((float) defaultViewport.Height - pos.Y);
      return pos;
    }

    private void DrawHUDRect(Player player, HUDRect element)
    {
      this.spriteBatchPoint.Draw(CoreGlobals.BlankTexture, (element.Props & HUDElementProps.Absolute) > HUDElementProps.None ? element.Rect : this.GetElementRect(element.Rect), element.Color);
    }

    private void DrawHUDText(Player player, HUDText element)
    {
      bool flag1 = (element.Props & HUDElementProps.Vertical) > HUDElementProps.None;
      bool flag2 = (element.Props & HUDElementProps.RightJustify) > HUDElementProps.None;
      Vector2 position = (element.Props & HUDElementProps.Absolute) > HUDElementProps.None ? element.Position : this.GetElementPos(element.Position);
      float scale = element.Scale;
      float rotation = element.Rotation;
      if (flag1)
        rotation += 1.570796f;
      Vector2 vector2 = CoreGlobals.GameFont.MeasureString(element.HUDString) * scale;
      Vector2 origin = new Vector2(0.0f, 8f);
      if (flag2)
      {
        if (flag1)
          position.X -= 20f;
        else
          position.X -= vector2.X - 1f;
      }
      this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, element.HUDString, position + Vector2.One, Color.Black, rotation, origin, scale, SpriteEffects.None, 0.0f);
      this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, element.HUDString, position, element.Color, rotation, origin, scale, SpriteEffects.None, 0.0f);
    }

    private void DrawHUDCounter(Player player, HUDCounter element)
    {
      bool flag = (element.Props & HUDElementProps.ShowLabel) > HUDElementProps.None;
      Vector2 position = (element.Props & HUDElementProps.Absolute) > HUDElementProps.None ? element.Position : this.GetElementPos(element.Position);
      float scale = element.Scale;
      Vector2 origin = new Vector2(0.0f, 0.0f);
      long history = element.History.GetHistory(element.HistoryKey);
      if (element.HUDString == null || (long) element.LastValue != history)
      {
        if (flag)
          element.HUDString = string.Format("{0}: {1}", (object) element.Name, (object) (int) history);
        else
          element.HUDString = string.Format("{0}", (object) (int) history);
        element.LastValue = (int) history;
      }
      this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, element.HUDString, position + Vector2.One, Color.Black, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
      this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, element.HUDString, position, element.Color, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
    }

    private void DrawHUDBar(Player player, HUDProgressBar element)
    {
      bool flag1 = (element.Props & HUDElementProps.Vertical) > HUDElementProps.None;
      bool flag2 = (element.Props & HUDElementProps.RightJustify) > HUDElementProps.None;
      bool flag3 = (element.Props & HUDElementProps.ShowLabel) > HUDElementProps.None;
      bool flag4 = (element.Props & HUDElementProps.ShowNumbers) > HUDElementProps.None;
      Rectangle rectangle1 = (element.Props & HUDElementProps.Absolute) > HUDElementProps.None ? element.Rect : this.GetElementRect(element.Rect);
      this.spriteBatchPoint.DrawFilledBox(rectangle1, 2, Color.White, Color.Black * 0.5f);
      Rectangle rectangle2 = rectangle1;
      rectangle1.X += 2;
      rectangle1.Y += 2;
      rectangle1.Width -= 4;
      rectangle1.Height -= 4;
      float num1 = MathHelper.Clamp((float) element.History.GetHistory(element.HistoryKey), 0.0f, (float) element.MaxValue);
      if (flag1)
      {
        int num2 = (int) ((double) num1 / (double) element.MaxValue * (double) rectangle1.Height);
        if (flag2)
          rectangle1.Y += rectangle1.Height - num2;
        rectangle1.Height = num2;
      }
      else
      {
        int num2 = (int) ((double) num1 / (double) element.MaxValue * (double) rectangle1.Width);
        if (flag2)
          rectangle1.X += rectangle1.Width - num2;
        rectangle1.Width = num2;
      }
      if (rectangle1.Width > 0 && rectangle1.Height > 0)
        this.spriteBatchPoint.Draw(CoreGlobals.BlankTexture, rectangle1, element.Color);
      float rotation = flag1 ? (flag2 ? -1.570796f : 1.570796f) : 0.0f;
      if (flag4)
      {
        if (element.HUDString == null || (double) element.LastValue != (double) num1)
        {
          element.HUDString = string.Format("{0} / {1}", (object) (int) num1, (object) element.MaxValue);
          element.LastValue = (int) num1;
        }
        float scale = 0.4f;
        Vector2 vector2 = CoreGlobals.GameFont.MeasureString(element.HUDString) * scale;
        Vector2 origin = flag1 ? new Vector2(0.0f, 0.0f) : new Vector2(0.0f, 0.0f);
        Vector2 position = flag1 ? new Vector2((float) rectangle2.X + (float) ((double) rectangle2.Width * 0.5 + (double) vector2.Y * 0.5 - 1.0), (float) ((double) rectangle2.Y + ((double) rectangle2.Height * 0.5 - (double) vector2.X * 0.5) + 1.0)) : new Vector2((float) rectangle2.X + (float) ((double) rectangle2.Width * 0.5 - (double) vector2.X * 0.5), (float) ((double) rectangle2.Y + ((double) rectangle2.Height * 0.5 - (double) vector2.Y * 0.5) + 1.0));
        if (flag1 && flag2)
        {
          position.X -= 16f;
          position.Y = (float) (rectangle2.Y + rectangle2.Height) - (float) (((double) rectangle2.Height - (double) vector2.X) / 2.0);
        }
        this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, element.HUDString, position, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
      }
      if (!flag3)
        return;
      float scale1 = 0.5f;
      Vector2 position1 = flag1 ? new Vector2((float) (rectangle2.X + rectangle2.Width + 20), (float) rectangle2.Y) : new Vector2((float) rectangle2.X, (float) (rectangle2.Y - 21));
      if (flag2)
      {
        Vector2 vector2 = CoreGlobals.GameFont.MeasureString(element.Name) * scale1;
        if (flag1)
        {
          position1.X = (float) (rectangle2.X - 20);
          position1.Y = (float) (rectangle2.Y + rectangle2.Height);
        }
        else
          position1.X += (float) ((double) rectangle2.Width - (double) vector2.X - 1.0);
      }
      Vector2 origin1 = flag1 ? new Vector2(0.0f, 0.0f) : new Vector2(0.0f, 0.0f);
      this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, element.Name, position1 + Vector2.One, Color.Black, rotation, origin1, scale1, SpriteEffects.None, 0.0f);
      this.spriteBatchPoint.DrawString(CoreGlobals.GameFont, element.Name, position1, Color.White, rotation, origin1, scale1, SpriteEffects.None, 0.0f);
    }

    private void DrawTextMessages(int y)
    {
      if (this.instance.TextMessages.Count <= 0)
        return;
      Rectangle rectangle = new Rectangle(this.hudPos.X + 1, y - 5, 350, 15);
      Vector2 vector2 = new Vector2(0.0f, (float) (rectangle.Y + 2));
      float scale = 0.5f;
      for (int index = this.instance.TextMessages.Count - 1; index >= 0; --index)
      {
        TextMessage textMessage = this.instance.TextMessages[index];
        if ((double) textMessage.Measure.X == 0.0)
        {
          this.BuildTxtMsgDrawData(ref textMessage, rectangle, vector2, scale);
          this.instance.TextMessages[index] = textMessage;
        }
        int num = rectangle.Y - (int) ((double) textMessage.Measure.Y + (double) textMessage.Measure2.Y);
        if (num >= this.hudPos.Y + rectangle.Height + 10)
        {
          rectangle.Height = rectangle.Y - num;
          rectangle.Y = num;
          vector2.X = (float) (rectangle.X + 4);
          vector2.Y = (float) (num + 1);
          this.spriteBatchText.Draw(CoreGlobals.BlankTexture, rectangle, textMessage.BackColor);
          if (textMessage.ClanID > (byte) 0)
          {
            Rectangle clanBannerRect = GraphicStatics.GetClanBannerRect(textMessage.ClanID);
            this.spriteBatchText.Draw(GraphicStatics.ClanBanners, new Rectangle((int) vector2.X, (int) vector2.Y + 3, 16, 16), new Rectangle?(clanBannerRect), Color.White);
            vector2.X += 20f;
          }
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, textMessage.Header, vector2 + Vector2.One, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, textMessage.Header, vector2, textMessage.Color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          vector2.X += textMessage.Measure2.X + 8f;
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, textMessage.MessageLine1, vector2 + Vector2.One, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          this.spriteBatchText.DrawString(CoreGlobals.GameFont, textMessage.MessageLine1, vector2, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          if (textMessage.MessageRemaining != null)
          {
            vector2.X = (float) (rectangle.X + 4);
            vector2.Y += textMessage.Measure.Y;
            this.spriteBatchText.DrawString(CoreGlobals.GameFont, textMessage.MessageRemaining, vector2 + Vector2.One, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            this.spriteBatchText.DrawString(CoreGlobals.GameFont, textMessage.MessageRemaining, vector2, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
          }
        }
        else
          break;
      }
      --rectangle.X;
      --rectangle.Y;
      rectangle.Width += 2;
      rectangle.Height = y - 4 - rectangle.Y;
      this.spriteBatchText.DrawBox(rectangle, 1, Color.White, 0.0f);
    }

    private void BuildTxtMsgDrawData(
      ref TextMessage msg,
      Rectangle rect,
      Vector2 pos,
      float scale)
    {
      msg.Measure = CoreGlobals.GameFont.MeasureString(msg.Message) * scale;
      msg.Measure2.X = CoreGlobals.GameFont.MeasureString(msg.Header).X * scale;
      if (8.0 + (double) msg.Measure2.X + 8.0 + (double) msg.Measure.X + (msg.ClanID > (byte) 0 ? 20.0 : 0.0) > (double) rect.Width)
      {
        int num1 = (int) msg.Measure2.X + 8 + (msg.ClanID > (byte) 0 ? 20 : 0);
        int maxWidth = rect.Width - 8 - num1;
        int num2 = Utils.InsertNewLines(CoreGlobals.GameFont, maxWidth, scale, msg.Message, true).IndexOf('\n');
        if (num2 >= 0)
        {
          msg.MessageLine1 = msg.Message.Substring(0, num2);
          if (num2 >= msg.Message.Length)
            return;
          msg.MessageRemaining = Utils.InsertNewLines(CoreGlobals.GameFont, rect.Width - 8, scale, msg.Message.Substring(num2), true);
          msg.Measure2.Y = CoreGlobals.GameFont.MeasureString(msg.MessageRemaining).Y * scale;
          msg.Measure.Y -= 3f;
          return;
        }
      }
      msg.MessageLine1 = msg.Message;
    }

    private void DrawCompass(Player player, Player virtualPlayer)
    {
      int width = player.Settings.CompassTop ? 300 : 466;
      int height = 20;
      Rectangle rect = new Rectangle((GraphicStatics.DefaultViewport.Width - width) / 2, player.Settings.CompassTop ? this.hudPos.Y : this.hudPos.Height - height - 16, width, height);
      this.spriteBatch.DrawFilledBox(rect, 1, Color.White, Color.Black * 0.5f);
      float num1 = 1f;
      HudRenderer.CompassData data = new HudRenderer.CompassData();
      data.Bound = new Rectangle(rect.X + 2, rect.X + rect.Width - 16, rect.Width / 2, rect.Width / 2 + rect.X);
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(data.Bound.Height, rect.Y, 1, rect.Height), Color.White * 0.5f);
      float y = virtualPlayer.Position.Y;
      Vector3 position = virtualPlayer.Position;
      data.Y = (int) ((double) rect.Y + (double) rect.Height * 0.5);
      data.Tex = this.circleTexture;
      data.Origin = new Vector2((float) data.Tex.Width * 0.5f, (float) data.Tex.Height * 0.5f);
      data.Bound.X += 4;
      data.Bound.Y += 10;
      int index1 = -1;
      int num2 = int.MaxValue;
      int num3 = 0;
      int num4 = rect.Width / 4;
      int num5 = 1000000;
      int num6 = 2000000;
      int num7 = 3000000;
      data.Color = Color.Yellow * 0.7f;
      data.Scale = 0.5f;
      data.Distance = 0.0f;
      lock (this.instance.MapMarkers)
      {
        for (int index2 = 0; index2 < this.instance.MapMarkers.Count; ++index2)
        {
          MapMarker mapMarker = this.instance.MapMarkers[index2];
          data.Pos = this.map.GetPosition(mapMarker.Point);
          int num8 = this.DrawCompassTexture(virtualPlayer, ref data);
          int num9 = Math.Abs(num8 - data.Bound.Height);
          if (num9 < num2 && num9 < num4)
          {
            num2 = num9;
            num3 = num8;
            index1 = index2;
          }
        }
      }
      data.Tex = this.graveyardTexture;
      data.Origin = new Vector2((float) data.Tex.Width * 0.5f, (float) data.Tex.Height * 0.5f);
      data.Color = Color.White * 0.8f;
      lock (this.instance.GraveMarkers)
      {
        for (int index2 = 0; index2 < this.instance.GraveMarkers.Count; ++index2)
        {
          MapMarker graveMarker = this.instance.GraveMarkers[index2];
          data.Pos = this.map.GetPosition(graveMarker.Point);
          int num8 = this.DrawCompassTexture(virtualPlayer, ref data);
          int num9 = Math.Abs(num8 - data.Bound.Height);
          if (num9 < num2 && num9 < num4)
          {
            num2 = num9;
            num3 = num8;
            index1 = index2 + num5;
          }
        }
      }
      data.Distance = 3600f;
      data.Scale = 0.6f;
      data.Tex = this.circleTexture;
      data.Origin = new Vector2((float) data.Tex.Width * 0.5f, (float) data.Tex.Height * 0.5f);
      if (this.instance.NpcManager != null)
      {
        data.Color = Color.DarkRed * 0.9f;
        for (int index2 = 0; index2 < this.mobs.Count; ++index2)
        {
          NpcBase mob = this.mobs[index2];
          if (mob != null && !mob.IsInactiveOrDisabled)
          {
            data.Pos = mob.Position;
            int num8 = this.DrawCompassTexture(virtualPlayer, ref data);
            if (this.instance.IsEasyDifficulty)
            {
              int num9 = Math.Abs(num8 - data.Bound.Height);
              if (num9 < num2 && num9 < num4)
              {
                num2 = num9;
                num3 = num8;
                index1 = index2 + num7;
              }
            }
          }
        }
      }
      data.Color = Color.Green * 0.7f;
      List<NetworkGamer> allEnabledGamers = this.instance.NetworkManager.AllEnabledGamers;
      for (int index2 = 0; index2 < allEnabledGamers.Count; ++index2)
      {
        NetworkGamer networkGamer = allEnabledGamers[index2];
        if (networkGamer.ID != virtualPlayer.GamerID)
        {
          Player tag = networkGamer.Tag as Player;
          NamePlateSetting namePlateSetting = (NamePlateSetting) Math.Min((int) virtualPlayer.Settings.Nameplates, (int) tag.Settings.Nameplates);
          if (namePlateSetting != NamePlateSetting.None)
          {
            bool flag = namePlateSetting == NamePlateSetting.Far;
            data.Distance = flag ? 1000000f : 1600f;
            data.Pos = tag.Position;
            int num8 = this.DrawCompassTexture(virtualPlayer, ref data);
            int num9 = Math.Abs(num8 - data.Bound.Height);
            if (num9 < num2 && num9 < num4)
            {
              num2 = num9;
              num3 = num8;
              index1 = index2 + num6;
            }
          }
        }
      }
      if (index1 >= 0)
      {
        Color color;
        if (index1 < num6)
        {
          if (index1 != this.lastTargetIndex)
            this.lastLabel = index1 >= num5 ? this.instance.GraveMarkers[index1 - num5].Label : this.instance.MapMarkers[index1].Label;
          color = Color.Yellow;
          this.lastTarget = (Actor) null;
        }
        else if (index1 < num7)
        {
          Player tag = (Player) allEnabledGamers[index1 - num6].Tag;
          if (tag != this.lastTarget)
          {
            this.lastLabel = tag.DisplayGamertag;
            this.lastTarget = (Actor) tag;
          }
          color = Color.Green;
        }
        else
        {
          NpcBase mob = this.mobs[index1 - num7];
          if (mob != this.lastTarget)
          {
            this.lastLabel = mob.ActorType.ToString();
            this.lastTarget = (Actor) mob;
          }
          color = Color.Red;
        }
        float num8 = CoreGlobals.GameFont.MeasureString(this.lastLabel).X * 0.5f;
        this.spriteBatchText.DrawString(CoreGlobals.GameFont, this.lastLabel, new Vector2((float) num3 - num8 * 0.5f, (float) (rect.Y + rect.Height)), color * MathHelper.SmoothStep(0.1f, 1f, (float) (num4 - num2) / (float) num4), 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      }
      this.lastTargetIndex = index1;
      this.mobs.Clear();
      data.Bound.X -= 4;
      data.Bound.Y -= 10;
      data.Y = rect.Y;
      data.Scale = 0.5f;
      data.Color = Color.White * 0.75f;
      data.Distance = 0.0f;
      data.Pos = new Vector3(0.0f, y, -num1);
      this.DrawCompassText(virtualPlayer, "N", ref data);
      data.Pos = new Vector3(-num1, y, 0.0f);
      this.DrawCompassText(virtualPlayer, "E", ref data);
      data.Pos = new Vector3(0.0f, y, num1);
      this.DrawCompassText(virtualPlayer, "S", ref data);
      data.Bound.Y -= 4;
      data.Pos = new Vector3(num1, y, 0.0f);
      this.DrawCompassText(virtualPlayer, "W", ref data);
      if (!virtualPlayer.Waypoint.HasValue)
        return;
      data.Bound.X += 4;
      data.Bound.Y += 10;
      data.Origin = new Vector2((float) (this.waypointTexture.Width / 2), (float) (this.waypointTexture.Height / 2));
      data.Tex = this.waypointTexture;
      data.Y = rect.Y + 10;
      data.Scale = 1f;
      data.Pos = this.map.GetPosition(virtualPlayer.Waypoint.Value);
      this.DrawCompassTexture(virtualPlayer, ref data);
    }

    private void DrawCompassText(
      Player virtualPlayer,
      string text,
      ref HudRenderer.CompassData data)
    {
      if ((double) data.Distance != 0.0 && (double) Vector3.DistanceSquared(virtualPlayer.Position, data.Pos) > (double) data.Distance)
        return;
      Vector2 compassPos = this.GetCompassPos(virtualPlayer, new Vector2(data.Pos.X, data.Pos.Z));
      if ((double) compassPos.Y <= 0.0)
        return;
      int num = (int) ((double) compassPos.X * (double) data.Bound.Width + (double) data.Bound.Height);
      if (num < data.Bound.X || num > data.Bound.Y)
        return;
      this.spriteBatchText.DrawString(CoreGlobals.GameFont, text, new Vector2((float) num, (float) data.Y), data.Color, 0.0f, Vector2.Zero, data.Scale, SpriteEffects.None, 0.0f);
    }

    private int DrawCompassTexture(Player virtualPlayer, ref HudRenderer.CompassData data)
    {
      if ((double) data.Distance == 0.0 || (double) Vector2.DistanceSquared(new Vector2(virtualPlayer.Position.X, virtualPlayer.Position.Z), new Vector2(data.Pos.X, data.Pos.Z)) <= (double) data.Distance)
      {
        Vector3 position = virtualPlayer.Position;
        Vector2 dir = Vector2.Normalize(new Vector2(data.Pos.X - position.X, position.Z - data.Pos.Z));
        Vector2 compassPos = this.GetCompassPos(virtualPlayer, dir);
        if ((double) compassPos.Y > 0.0)
        {
          int num = (int) ((double) compassPos.X * (double) data.Bound.Width + (double) data.Bound.Height);
          if (num >= data.Bound.X && num <= data.Bound.Y)
          {
            this.spriteBatch.Draw(data.Tex, new Vector2((float) num, (float) data.Y), new Rectangle?(), data.Color, 0.0f, data.Origin, data.Scale, SpriteEffects.None, 0.0f);
            return num;
          }
        }
      }
      return 0;
    }

    private Vector2 GetCompassPos(Player virtualPlayer, Vector2 dir)
    {
      Matrix rotationY = Matrix.CreateRotationY(virtualPlayer.ViewAngle.X);
      Vector3 vector3 = Vector3.Transform(new Vector3(dir.X, 0.0f, dir.Y), rotationY);
      return new Vector2(vector3.X, vector3.Z);
    }

    private void DrawSkillData(Player player, ref Rectangle rect, SkillType skillType)
    {
      this.DrawFilledBox(this.spriteBatch, rect, 2, Color.White, Color.Black * 0.7f);
      rect.X += 8;
      rect.Y += 6;
      rect.Width = rect.Height = 32;
      Item itemID = (Item) ((byte) 204 + skillType);
      this.spriteBatchPoint.Draw(GraphicStatics.TexturePack.ItemTexture, rect, new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(itemID)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
      int count = skillType == SkillType.None ? player.CombatLevel : player.SkillsData[(int) skillType].Level;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, Globals2.GetItemCountString(count), new Vector2((float) (rect.X + rect.Width + 12), (float) (rect.Y + 12)), Color.White, 0.0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0.0f);
      if (skillType == SkillType.None)
        return;
      SkillData skillData = player.SkillsData[(int) skillType];
      rect.Y += 33;
      long xp = SkillData.GetXP(skillData.Level);
      float num = ((float) skillData.CurrentXP - (float) xp) / (float) (SkillData.GetXP(skillData.Level + 1) - xp);
      rect.Height = 4;
      Rectangle destinationRectangle = new Rectangle(rect.X, rect.Y, (int) ((double) rect.Width * (double) num), rect.Height);
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Green);
      destinationRectangle.X += destinationRectangle.Width;
      destinationRectangle.Width = 32 + rect.X - destinationRectangle.X;
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.Red);
    }

    private void DrawHealthBar(Player virtualPlayer)
    {
      this.spriteBatch.DrawFilledBox(this.healthBorderRect, 2, Color.White, Color.Black * this.barAlpha);
      float num1 = virtualPlayer.MaxHealth;
      if ((double) num1 < (double) virtualPlayer.Health)
        num1 = virtualPlayer.Health;
      this.healthBarRect.Width = (int) ((double) (this.healthBorderRect.Width - 4) * ((double) virtualPlayer.Health / (double) num1));
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, this.healthBarRect, Color.Red * this.barAlpha);
      if ((double) virtualPlayer.FreezeImmunityTimer > 0.0)
      {
        this.healthBarRect.Width = (int) ((double) (this.healthBorderRect.Width - 4) * ((double) virtualPlayer.FreezeImmunityTimer / 4.0));
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, this.healthBarRect, Color.Green * this.barAlpha);
      }
      if (virtualPlayer.IceEffectActive)
      {
        this.healthBarRect.Width = (int) ((double) (this.healthBorderRect.Width - 4) * ((double) virtualPlayer.FreezeTimer / 3.0));
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, this.healthBarRect, Color.Blue * this.barAlpha);
      }
      virtualPlayer.SetHealthHudString();
      float num2 = CoreGlobals.GameFont.MeasureString(virtualPlayer.HealthHUDString).X * 0.5f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, virtualPlayer.HealthHUDString, new Vector2((float) this.healthBorderRect.X + (float) ((double) this.healthBorderRect.Width * 0.5 - (double) num2 * 0.5), (float) (this.healthBarRect.Y - 2)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
    }

    private void DrawOxygenBar(Player virtualPlayer)
    {
      if (!virtualPlayer.IsUnderWater)
        return;
      this.spriteBatch.DrawFilledBox(this.oxygenBorderRect, 2, Color.White, Color.Black * this.barAlpha);
      this.oxygenBarRect.Width = (int) ((double) (this.oxygenBorderRect.Width - 4) * ((double) virtualPlayer.Oxygen / (double) virtualPlayer.MaxOxygen));
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, this.oxygenBarRect, Color.Blue * this.barAlpha);
      virtualPlayer.SetOxygenHudString();
      float num = CoreGlobals.GameFont.MeasureString(virtualPlayer.OxygenHUDString).X * 0.5f;
      this.spriteBatch.DrawString(CoreGlobals.GameFont, virtualPlayer.OxygenHUDString, new Vector2((float) this.oxygenBorderRect.X + (float) ((double) this.oxygenBorderRect.Width * 0.5 - (double) num * 0.5), (float) (this.oxygenBarRect.Y - 2)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
    }

    private struct CompassData
    {
      public Texture2D Tex;
      public Vector3 Pos;
      public Rectangle Bound;
      public Color Color;
      public float Distance;
      public float Scale;
      public Vector2 Origin;
      public int Y;
    }
  }
}
