// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.MapTopViewScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Renderers;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class MapTopViewScreen : MinerToolScreen
  {
    private Vector3[] corners = new Vector3[8];
    private GameInstance instance;
    private Texture2D cursorTex;
    private Texture2D waypointTex;
    private float zoom;
    private Map map;
    private Rectangle rtRect;
    private Vector2 rtRectPos;
    private Texture2D markerTex;
    private Texture2D graveyardTex;
    private Texture2D playerTex;
    private int mapSizeX;
    private int mapSizeZ;
    private GlobalPoint3D currentMapPoint;
    private string coordText;
    private int highlightedMarker;
    private static float rtDisposeCounter;
    private static RenderTarget2D rt;

    private Vector2 CursorPoint
    {
      get
      {
        Point mousePos = InputManager.GetMousePos(this.player.PlayerIndex);
        return new Vector2((float) mousePos.X, (float) mousePos.Y);
      }
    }

    public static int StaticMemorySizeInBytesUnmanaged
    {
      get
      {
        if (MapTopViewScreen.rt == null || MapTopViewScreen.rt.IsDisposed)
          return 0;
        return MapTopViewScreen.rt.Width * MapTopViewScreen.rt.Height * 2;
      }
    }

    public static void DisposeRT(GameInstance instance, bool force)
    {
      if (MapTopViewScreen.rt == null)
        return;
      if (force)
      {
        if (!MapTopViewScreen.rt.IsDisposed)
          MapTopViewScreen.rt.Dispose();
        MapTopViewScreen.rt = (RenderTarget2D) null;
      }
      else
      {
        if (instance.TopViewMapOpenCount != 0)
          return;
        MapTopViewScreen.rtDisposeCounter += Services.ElapsedTime;
        if ((double) MapTopViewScreen.rtDisposeCounter <= 10.0)
          return;
        if (!MapTopViewScreen.rt.IsDisposed)
          MapTopViewScreen.rt.Dispose();
        MapTopViewScreen.rt = (RenderTarget2D) null;
      }
    }

    private bool CanAddMarker
    {
      get
      {
        return this.player.HasPermissionAny(Permissions.Creative | Permissions.Admin);
      }
    }

    private bool CanTeleport
    {
      get
      {
        if (this.player.IsGodOrTester)
          return true;
        if (this.player.IsAdmin)
          return this.instance.IsCreativeMode;
        return false;
      }
    }

    public MapTopViewScreen(GameInstance instance, Player player)
      : base(player)
    {
      this.instance = instance;
      this.map = (Map) instance.Map;
      player.IsViewingMainMap = true;
      this.mapSizeX = this.map.MapSize.X;
      this.mapSizeZ = this.map.MapSize.Z;
    }

    public override void LoadContent()
    {
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.screenRect = GraphicStatics.DefaultViewport.Rectangle();
      base.LoadContent();
      this.zoom = 1f;
      this.RebuildCoordText();
      this.cursorTex = this.content.Load<Texture2D>("Textures\\Cursor2");
      this.playerTex = this.content.Load<Texture2D>("Textures\\Stickman");
      this.markerTex = this.content.Load<Texture2D>("Textures\\CheckboxOff");
      this.graveyardTex = this.content.Load<Texture2D>("Textures\\Graveyard");
      this.waypointTex = this.content.Load<Texture2D>("Textures\\waypoint");
      this.rtRect = new Rectangle(0, 0, 1280, 720);
      MapTopViewScreen.rtDisposeCounter = 0.0f;
    }

    private void RebuildCoordText()
    {
      if (MapTopViewScreen.rt == null)
        return;
      GlobalPoint3D map = this.ConvertScreenPointToMap(ref this.screenRect, this.CursorPoint);
      if (!(map != this.currentMapPoint))
        return;
      this.currentMapPoint = map;
      this.coordText = string.Format("({0}, {1})", (object) map.X, (object) map.Z);
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      this.player.IsViewingMainMap = false;
    }

    public override bool HandleInput(InputState input)
    {
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      if (MapTopViewScreen.rt != null)
      {
        float num1 = 6f;
        Vector2 cursorPoint = this.CursorPoint;
        if ((double) cursorPoint.X < (double) this.screenRect.Width * 0.100000001490116 && (double) this.rtRectPos.X > 0.0)
        {
          this.rtRectPos.X -= (float) ((double) num1 * (double) this.zoom * 2.0);
          this.ClampDrawOffsetX();
        }
        else if ((double) cursorPoint.X > (double) (this.screenRect.X + this.screenRect.Width) - (double) this.screenRect.Width * 0.100000001490116 && (double) this.rtRectPos.X + (double) this.rtRect.Width < (double) MapTopViewScreen.rt.Width)
        {
          this.rtRectPos.X += (float) ((double) num1 * (double) this.zoom * 2.0);
          this.ClampDrawOffsetX();
        }
        if ((double) cursorPoint.Y < (double) this.screenRect.Height * 0.100000001490116 && (double) this.rtRectPos.Y > 0.0)
        {
          this.rtRectPos.Y -= (float) ((double) num1 * (double) this.zoom * 2.0);
          this.ClampDrawOffsetY();
        }
        else if ((double) cursorPoint.Y > (double) (this.screenRect.Y + this.screenRect.Height) - (double) this.screenRect.Height * 0.100000001490116 && (double) this.rtRectPos.Y + (double) this.rtRect.Height < (double) MapTopViewScreen.rt.Height)
        {
          this.rtRectPos.Y += (float) ((double) num1 * (double) this.zoom * 2.0);
          this.ClampDrawOffsetY();
        }
        float num2 = currentGamePadState.ThumbSticks.Right.Y;
        if ((double) num2 == 0.0)
          num2 = (float) InputManager.GetMouseWheelDelta(this.ControllingPlayer) * 0.02f;
        if ((double) num2 != 0.0)
          this.MoveZoom(-num2);
        this.ClampCursorPoint();
        this.RebuildCoordText();
        this.highlightedMarker = this.instance.GetMapMarkerIndex(this.ConvertScreenPointToMap(ref this.screenRect, this.CursorPoint));
        if (this.ControllingPlayer.HasValue)
        {
          if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.SelectItem))
          {
            if (this.CanTeleport)
            {
              this.player.TeleportTo(this.ConvertScreenPointToMap(ref this.screenRect, this.CursorPoint), true);
              this.ExitScreen();
            }
          }
          else if (input.IsNewButtonPress(Buttons.X, this.ControllingPlayer.Value))
          {
            if (this.CanAddMarker)
            {
              this.AddMarker();
              return true;
            }
          }
          else if (input.IsNewButtonPress(Buttons.RightTrigger, this.ControllingPlayer.Value))
          {
            GlobalPoint3D map = this.ConvertScreenPointToMap(ref this.screenRect, this.CursorPoint);
            if (!this.player.Waypoint.HasValue)
              this.player.Waypoint = new GlobalPoint3D?(map);
            else if (map.X >= this.player.Waypoint.Value.X - 5 && map.X <= this.player.Waypoint.Value.X + 5 && (map.Z >= this.player.Waypoint.Value.Z - 5 && map.Z <= this.player.Waypoint.Value.Z + 5))
              this.player.Waypoint = new GlobalPoint3D?();
            else
              this.player.Waypoint = new GlobalPoint3D?(map);
          }
          else if (this.highlightedMarker > -1)
          {
            MapMarker mapMarker = this.instance.MapMarkers[this.highlightedMarker];
            if (input.IsNewButtonPress(Buttons.Y, this.ControllingPlayer.Value))
            {
              if (this.player.HasPermission(Permissions.Creative) && mapMarker.Type == MapMarkerType.X || this.player.IsAdmin)
              {
                this.RemoveMarker();
                return true;
              }
            }
            else if (input.IsNewButtonPress(Buttons.LeftTrigger, this.ControllingPlayer.Value) && this.player.IsAdmin)
            {
              if (mapMarker.Type == MapMarkerType.X)
              {
                mapMarker.Type = MapMarkerType.AdminX;
                this.instance.MapMarkers[this.highlightedMarker] = mapMarker;
              }
              else if (mapMarker.Type == MapMarkerType.AdminX)
              {
                mapMarker.Type = MapMarkerType.X;
                this.instance.MapMarkers[this.highlightedMarker] = mapMarker;
              }
            }
          }
        }
      }
      return base.HandleInput(input);
    }

    private void MoveZoom(float stickZoom)
    {
      float num1 = (float) this.screenRect.Width / (float) MapTopViewScreen.rt.Width;
      float num2 = (float) this.screenRect.Height / (float) MapTopViewScreen.rt.Height;
      Vector2 cursorPoint = this.CursorPoint;
      Vector2 vector2_1 = new Vector2(this.rtRectPos.X + cursorPoint.X / num1 * this.zoom, this.rtRectPos.Y + cursorPoint.Y / num2 * this.zoom);
      Vector2 vector2_2 = new Vector2((float) this.screenRect.Width / cursorPoint.X / num1, (float) this.screenRect.Height / cursorPoint.Y / num2);
      this.zoom += stickZoom * 0.01f;
      if ((double) this.zoom < 3.0 / 16.0)
        this.zoom = 3f / 16f;
      else if ((double) this.zoom > 1.0)
        this.zoom = 1f;
      this.rtRect.Width = (int) ((double) MapTopViewScreen.rt.Width * (double) this.zoom);
      this.rtRect.Height = (int) ((double) MapTopViewScreen.rt.Height * (double) this.zoom);
      this.rtRectPos.X = vector2_1.X - (float) this.rtRect.Width / num1 / vector2_2.X;
      this.rtRectPos.Y = vector2_1.Y - (float) this.rtRect.Height / num2 / vector2_2.Y;
      this.ClampDrawOffsetX();
      this.ClampDrawOffsetY();
    }

    private void MoveScale(float stickScale)
    {
    }

    private bool ClampDrawOffsetX()
    {
      bool flag = false;
      if ((double) this.rtRectPos.X < 0.0)
      {
        this.rtRectPos.X = 0.0f;
        flag = true;
      }
      else if ((double) this.rtRectPos.X + (double) this.rtRect.Width > (double) MapTopViewScreen.rt.Width)
      {
        this.rtRectPos.X = (float) (MapTopViewScreen.rt.Width - this.rtRect.Width);
        flag = true;
      }
      return flag;
    }

    private bool ClampDrawOffsetY()
    {
      bool flag = false;
      if ((double) this.rtRectPos.Y < 0.0)
      {
        this.rtRectPos.Y = 0.0f;
        flag = true;
      }
      else if ((double) this.rtRectPos.Y + (double) this.rtRect.Height > (double) MapTopViewScreen.rt.Height)
      {
        this.rtRectPos.Y = (float) (MapTopViewScreen.rt.Height - this.rtRect.Height);
        flag = true;
      }
      return flag;
    }

    private void AddMarker()
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter Marker Text", "Enter a maximum of 25 characters", "", new AsyncCallback(this.MarkerTextEntered), (object) null);
    }

    private void MarkerTextEntered(IAsyncResult ar)
    {
      string text = Globals2.StripBadChars(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (text.Length <= 0)
        return;
      if (text.Length > 25)
        text = text.Substring(0, 25);
      this.instance.AddMapMarker(this.ConvertScreenPointToMap(ref this.screenRect, this.CursorPoint), text, MapMarkerType.X, true);
    }

    private void RemoveMarker()
    {
      this.instance.RemoveMapMarker(this.highlightedMarker, true);
    }

    private void ClampCursorPoint()
    {
      Vector2 cursorPoint = this.CursorPoint;
      bool flag = false;
      if ((double) cursorPoint.X < (double) this.screenRect.X)
      {
        cursorPoint.X = (float) this.screenRect.X;
        flag = true;
      }
      if ((double) cursorPoint.X > (double) (this.screenRect.X + this.screenRect.Width - 1))
      {
        cursorPoint.X = (float) (this.screenRect.X + this.screenRect.Width - 1);
        flag = true;
      }
      if ((double) cursorPoint.Y < (double) this.screenRect.Y)
      {
        cursorPoint.Y = (float) this.screenRect.Y;
        flag = true;
      }
      if ((double) cursorPoint.Y > (double) (this.screenRect.Y + this.screenRect.Height - 1))
      {
        cursorPoint.Y = (float) (this.screenRect.Y + this.screenRect.Height - 1);
        flag = true;
      }
      if (!flag)
        return;
      InputManager.SetMousePos((int) cursorPoint.X, (int) cursorPoint.Y);
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.drawFrameCount != 1)
        return;
      if (MapTopViewScreen.rt != null)
        return;
      try
      {
        MapTopViewScreen.rt = new RenderTarget2D(CoreGlobals.GraphicsDevice, this.rtRect.Width, this.rtRect.Height, false, SurfaceFormat.Bgr565, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        MapTopViewRenderer mapTopViewRenderer = new MapTopViewRenderer(this.instance, this.instance.Map);
        mapTopViewRenderer.Initialize();
        mapTopViewRenderer.LoadContent();
        CoreGlobals.GraphicsDevice.SetRenderTarget(MapTopViewScreen.rt);
        CoreGlobals.GraphicsDevice.Clear(GraphicStatics.TexturePack.SkyColor);
        mapTopViewRenderer.Draw(this.player, this.rtRect, true);
        mapTopViewRenderer.Draw(this.player, this.rtRect, false);
        CoreGlobals.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
        mapTopViewRenderer.UnloadContent();
      }
      catch (InvalidOperationException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(106, (Exception) ex);
        MapTopViewScreen.rt = (RenderTarget2D) null;
        this.ExitScreen();
      }
    }

    protected override void DrawCore()
    {
      if (MapTopViewScreen.rt == null)
      {
        this.spriteBatch.Begin();
        this.spriteBatch.Draw(CoreGlobals.BlankTexture, GraphicStatics.DefaultViewport.Rectangle(), GraphicStatics.TexturePack.SkyColor);
        this.spriteBatch.DrawStringCentered(CoreGlobals.GameFont, "Building Map. Please wait...", (float) (GraphicStatics.DefaultViewport.Height / 2 - 10), Color.White, 1f);
        this.spriteBatch.End();
      }
      else
      {
        this.rtRect.X = (int) this.rtRectPos.X;
        this.rtRect.Y = (int) this.rtRectPos.Y;
        this.spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null);
        this.spriteBatch.Draw((Texture2D) MapTopViewScreen.rt, this.screenRect, new Rectangle?(this.rtRect), Color.White);
        this.DrawDebug();
        this.spriteBatch.End();
        this.DrawMarkers();
        this.spriteBatch.Begin();
        this.DrawPlayerTags();
        if (this.player.Waypoint.HasValue)
        {
          GlobalPoint3D p = this.player.Waypoint.Value;
          this.spriteBatch.Draw(this.waypointTex, this.ConvertMapPointToScreen(ref this.screenRect, ref p) - new Vector2(4f, 2f), new Rectangle?(), Color.White, 0.0f, new Vector2((float) (this.waypointTex.Width / 2), (float) (this.waypointTex.Height / 2)), 1f, SpriteEffects.None, 0.0f);
        }
        this.DrawHUD(this.spriteBatch);
        this.DrawCursor(this.spriteBatch);
        this.spriteBatch.End();
      }
    }

    private Vector2 GetScreenVector(Vector3 v1)
    {
      GlobalPoint3D point = this.map.GetPoint(v1);
      return this.ConvertMapPointToScreen(ref this.screenRect, ref point);
    }

    private void DrawRect(SpriteBatchSafe spriteBatch, int x, int y, int w, int h, Color color)
    {
      spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(x, y, w, h), color);
    }

    private void DrawHUD(SpriteBatchSafe spriteBatch)
    {
      float scale = 0.6f;
      Color orangeRed = Color.OrangeRed;
      Rectangle destinationRectangle = new Rectangle(this.screenRect.X + this.screenRect.Width / 10, this.screenRect.Y + this.screenRect.Height - this.screenRect.Height / 10 - 24, 24, 24);
      this.DrawRect(spriteBatch, destinationRectangle.X - 8, destinationRectangle.Y - 4, 144, 34, Color.Black * 0.75f);
      spriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.RightTrigger), destinationRectangle, Color.White);
      destinationRectangle.X += 32;
      spriteBatch.DrawString(this.Font, "Waypoint", new Vector2((float) destinationRectangle.X, (float) (destinationRectangle.Y + 4)) + TMFont.yVec, orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      destinationRectangle.X += 116;
      if (this.CanAddMarker)
      {
        this.DrawRect(spriteBatch, destinationRectangle.X - 8, destinationRectangle.Y - 4, 172, 34, Color.Black * 0.75f);
        spriteBatch.Draw(CoreGlobals.ButtonTextureX, destinationRectangle, Color.White);
        destinationRectangle.X += 30;
        spriteBatch.DrawString(this.Font, "Add Marker", new Vector2((float) destinationRectangle.X, (float) (destinationRectangle.Y + 4)) + TMFont.yVec, orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        destinationRectangle.X += 146;
        this.DrawRect(spriteBatch, destinationRectangle.X - 8, destinationRectangle.Y - 4, 212, 34, Color.Black * 0.75f);
        spriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
        destinationRectangle.X += 32;
        spriteBatch.DrawString(this.Font, "Remove Marker", new Vector2((float) destinationRectangle.X, (float) (destinationRectangle.Y + 4)) + TMFont.yVec, orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        destinationRectangle.X += 184;
      }
      if (this.CanTeleport)
      {
        this.DrawRect(spriteBatch, destinationRectangle.X - 8, destinationRectangle.Y - 4, 170, 34, Color.Black * 0.75f);
        spriteBatch.Draw(CoreGlobals.ButtonTextureA, destinationRectangle, Color.White);
        destinationRectangle.X += 32;
        spriteBatch.DrawString(this.Font, "Teleport To", new Vector2((float) destinationRectangle.X, (float) (destinationRectangle.Y + 4)) + TMFont.yVec, orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        destinationRectangle.X += 142;
      }
      if (this.player.IsAdmin)
      {
        this.DrawRect(spriteBatch, destinationRectangle.X - 8, destinationRectangle.Y - 4, 186, 34, Color.Black * 0.75f);
        spriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.LeftTrigger), destinationRectangle, Color.White);
        destinationRectangle.X += 32;
        spriteBatch.DrawString(this.Font, "Toggle Admin", new Vector2((float) destinationRectangle.X, (float) (destinationRectangle.Y + 4)) + TMFont.yVec, orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        destinationRectangle.X += 160;
      }
      if (this.coordText == null)
        return;
      this.DrawRect(spriteBatch, destinationRectangle.X - 10, destinationRectangle.Y - 4, 158, 34, Color.Black * 0.75f);
      spriteBatch.DrawString(this.Font, this.coordText, new Vector2((float) destinationRectangle.X, (float) (destinationRectangle.Y + 4)) + TMFont.yVec, orangeRed, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
    }

    private void DrawCursor(SpriteBatchSafe spriteBatch)
    {
      Vector2 cursorPoint = this.CursorPoint;
      Rectangle destinationRectangle = new Rectangle((int) cursorPoint.X - this.cursorTex.Width / 2, (int) cursorPoint.Y - this.cursorTex.Height / 2, this.cursorTex.Width, this.cursorTex.Height);
      spriteBatch.Draw(this.cursorTex, destinationRectangle, Color.White);
    }

    private void DrawMarkers()
    {
      if (this.instance.MapMarkers.Count <= 0 && this.instance.GraveMarkers.Count <= 0)
        return;
      this.spriteBatch.Begin();
      int num = 0;
      bool isAdmin = this.player.IsAdmin;
      for (int index = this.instance.MapMarkers.Count - 1; index >= 0 && num < 500; ++num)
      {
        MapMarker mapMarker = this.instance.MapMarkers[index];
        if (isAdmin || mapMarker.Type != MapMarkerType.AdminX)
        {
          Color color = index == this.highlightedMarker ? Color.Orange : (mapMarker.Type == MapMarkerType.AdminX ? Color.Yellow : Color.White);
          this.DrawMarker(mapMarker, color);
        }
        --index;
      }
      for (int index = this.instance.GraveMarkers.Count - 1; index >= 0 && num < 500; ++num)
      {
        this.DrawMarker(this.instance.GraveMarkers[index], Color.White);
        --index;
      }
      this.spriteBatch.End();
    }

    private void DrawMarker(MapMarker marker, Color color)
    {
      if (marker.Label == null || marker.Label.Length <= 0)
        return;
      Vector2 screen = this.ConvertMapPointToScreen(ref this.screenRect, ref marker.Point);
      int num = 10;
      Texture2D texture = this.markerTex;
      if (marker.Type == MapMarkerType.Graveyard)
      {
        num = 16;
        texture = this.graveyardTex;
      }
      Rectangle destinationRectangle1 = new Rectangle((int) screen.X - num / 2, (int) screen.Y - num / 2, num, num);
      float scale = 0.5f;
      Vector2 vector2 = this.Font.MeasureString(marker.Label) * scale;
      screen.X -= vector2.X * 0.5f;
      screen.Y -= (float) ((double) vector2.Y * 0.5 + 9.0) + (float) (num / 2);
      Rectangle destinationRectangle2 = new Rectangle((int) ((double) screen.X - 4.0), (int) ((double) screen.Y - 1.0), (int) ((double) vector2.X + 8.0), (int) ((double) vector2.Y + 3.0));
      if (destinationRectangle2.X < 1)
      {
        screen.X += (float) (1 - destinationRectangle2.X);
        destinationRectangle2.X = 1;
      }
      else if (destinationRectangle2.X + destinationRectangle2.Width > this.screenRect.X + this.screenRect.Width - 2)
      {
        screen.X -= (float) (destinationRectangle2.X + destinationRectangle2.Width - (this.screenRect.X + this.screenRect.Width) + 1);
        destinationRectangle2.X = this.screenRect.X + this.screenRect.Width - destinationRectangle2.Width - 1;
      }
      if (destinationRectangle2.Y < 1)
      {
        screen.Y += (float) (1 - destinationRectangle2.Y);
        destinationRectangle2.Y = 1;
      }
      else if (destinationRectangle2.Y + destinationRectangle2.Height > this.screenRect.Y + this.screenRect.Height - 2)
      {
        screen.Y -= (float) (destinationRectangle2.Y + destinationRectangle2.Height - (this.screenRect.Y + this.screenRect.Height) + 1);
        destinationRectangle2.Y = this.screenRect.Y + this.screenRect.Height - destinationRectangle2.Height - 1;
      }
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle2, Color.Black * 0.3f);
      this.spriteBatch.DrawString(this.Font, marker.Label, screen + new Vector2(1f, 1f), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      this.spriteBatch.DrawString(this.Font, marker.Label, screen, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      this.spriteBatch.Draw(texture, destinationRectangle1, Color.White);
    }

    private void DrawPlayerTags()
    {
      if (this.player.Settings.Nameplates == NamePlateSetting.None)
        return;
      foreach (Gamer allEnabledGamer in this.instance.NetworkManager.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null)
          this.DrawPlayerTag(tag);
      }
    }

    private void DrawPlayerTag(Player player)
    {
      NamePlateSetting namePlateSetting = (NamePlateSetting) Math.Min((int) player.Settings.Nameplates, (int) this.player.Settings.Nameplates);
      if (namePlateSetting == NamePlateSetting.None && player != this.player)
        return;
      float num = namePlateSetting == NamePlateSetting.Far ? 1000f : 40f;
      if ((double) Vector3.Distance(player.Position, this.player.Position) >= (double) num)
        return;
      Vector3 v1 = player.Position - new Vector3(2.5f, 0.0f, 12f) * this.zoom + player.ViewDirection * this.zoom * 10f;
      Vector2 screenVector1 = this.GetScreenVector(v1);
      Vector2 screenVector2 = this.GetScreenVector(v1 + player.ViewDirection * 40f);
      this.spriteBatch.DrawLine(CoreGlobals.BlankTexture, 2f, Color.Yellow, screenVector1, screenVector2);
      Vector3 vector3 = v1 + player.ViewDirection * 40f;
      this.spriteBatch.DrawLine(CoreGlobals.BlankTexture, 2f, Color.Yellow, this.GetScreenVector(vector3 + Vector3.Transform(-player.ViewDirection * 10f, Matrix.CreateRotationY(0.7853982f))), screenVector2);
      this.spriteBatch.DrawLine(CoreGlobals.BlankTexture, 2f, Color.Yellow, this.GetScreenVector(vector3 + Vector3.Transform(-player.ViewDirection * 10f, Matrix.CreateRotationY(-0.7853982f))), screenVector2);
      GlobalPoint3D point = this.map.GetPoint(player.Position);
      Vector2 screen = this.ConvertMapPointToScreen(ref this.screenRect, ref point);
      Rectangle destinationRectangle = new Rectangle((int) screen.X, (int) screen.Y, 10, 20);
      float scale = 0.5f;
      Color color = player.IsGod ? Color.LightGoldenrodYellow : (player.Gamer.IsHost ? Color.Blue : (player.IsAdmin ? Color.Cyan : Color.Yellow));
      string displayGamertag = player.DisplayGamertag;
      Vector2 vector2 = this.Font.MeasureString(displayGamertag) * scale;
      screen.X -= vector2.X * 0.5f;
      screen.Y -= (float) ((double) vector2.Y * 0.5 + 14.0);
      this.spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle((int) ((double) screen.X - 4.0), (int) ((double) screen.Y - 1.0), (int) ((double) vector2.X + 8.0), (int) ((double) vector2.Y + 3.0)), Color.Black * 0.3f);
      this.spriteBatch.DrawString(this.Font, displayGamertag, screen, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      this.spriteBatch.Draw(this.playerTex, destinationRectangle, Color.White);
    }

    public Vector2 ConvertMapPointToScreen(ref Rectangle rect, ref GlobalPoint3D p)
    {
      float num1 = (float) rect.Width / (float) MapTopViewScreen.rt.Width;
      float num2 = (float) rect.Height / (float) MapTopViewScreen.rt.Height;
      float num3 = (float) (this.mapSizeX - (p.X - this.map.MapBound.Min.X));
      float num4 = (float) (this.mapSizeZ - (p.Z - this.map.MapBound.Min.Z));
      return new Vector2((float) ((double) num3 / (double) this.mapSizeX * (double) rect.Width - (double) this.rtRectPos.X * (double) num1) / this.zoom, (float) ((double) num4 / (double) this.mapSizeZ * (double) rect.Height - (double) this.rtRectPos.Y * (double) num2) / this.zoom);
    }

    public GlobalPoint3D ConvertScreenPointToMap(ref Rectangle rect, Vector2 v)
    {
      float num1 = (float) rect.Width / (float) MapTopViewScreen.rt.Width;
      float num2 = (float) rect.Height / (float) MapTopViewScreen.rt.Height;
      float num3 = (float) this.mapSizeX - (v.X * this.zoom + this.rtRectPos.X) * num1 * (float) this.mapSizeX / (float) this.screenRect.Width + (float) this.map.MapBound.Min.X;
      float num4 = (float) this.mapSizeZ - (v.Y * this.zoom + this.rtRectPos.Y) * num2 * (float) this.mapSizeZ / (float) this.screenRect.Height + (float) this.map.MapBound.Min.Z;
      if ((double) num3 < 0.0)
        num3 = 0.0f;
      else if ((double) num3 >= (double) this.mapSizeX)
        num3 = (float) (this.mapSizeX - 1);
      if ((double) num4 < 0.0)
        num4 = 0.0f;
      else if ((double) num4 >= (double) this.mapSizeZ)
        num4 = (float) (this.mapSizeZ - 1);
      return new GlobalPoint3D((int) (num3 + (float) this.map.MapBound.Min.X), 0, (int) (num4 + (float) this.map.MapBound.Min.Z));
    }

    private void DrawDebug()
    {
    }
  }
}
