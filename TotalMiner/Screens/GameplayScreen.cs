// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GameplayScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class GameplayScreen : StudioForge.Engine.GameState.GameplayScreen
  {
    public static int OperationsStartY = 62;
    public static GameplayScreen ScreenInstance;
    public ArcadeMachineRenderer ArcadeMachineRenderer;
    private SpriteFont font;
    private SpriteBatchSafe spritebatch;
    private SpriteBatchSafe spritebatchPoint;
    private bool instanceUpdated;
    private GameInstance instance;
    private RenderTarget2D[] sceneRenderTargets;
    private RenderTarget2D[] bpfRenderTargets;
    private RenderTarget2D photoThumbnail16x16;
    private RenderTarget2D photoThumbnail64x64;
    private Texture2D logoTexture;

    public GameInstance GameInstance
    {
      get
      {
        return this.instance;
      }
    }

    public RenderTarget2D GetBlueprintFinderRenderTarget(int playerScreenID)
    {
      if (playerScreenID < 0)
        return (RenderTarget2D) null;
      return this.bpfRenderTargets[playerScreenID];
    }

    public GameplayScreen(GameInstance instance)
    {
      this.instance = instance;
      GameplayScreen.ScreenInstance = this;
      this.TransitionOnTime = TimeSpan.FromSeconds(2.0);
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.font = CoreGlobals.GameFont;
      this.spritebatch = this.ScreenManager.SpriteBatch;
      this.spritebatchPoint = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.logoTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\totalminerlogo2");
      this.ArcadeMachineRenderer = new ArcadeMachineRenderer(this.instance);
      this.ArcadeMachineRenderer.LoadContent((InitState) null);
      this.sceneRenderTargets = new RenderTarget2D[4];
      this.bpfRenderTargets = new RenderTarget2D[4];
      this.SetupRenderTargets();
      this.SetupViewports();
      this.photoThumbnail16x16 = new RenderTarget2D(this.GraphicsDevice, 16, 16, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
      this.photoThumbnail64x64 = new RenderTarget2D(this.GraphicsDevice, 64, 64, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
    }

    public void SetupViewports()
    {
      List<NetworkGamer> localGamers = this.instance.NetworkManager.LocalGamers;
      lock (localGamers)
      {
        foreach (Gamer gamer in localGamers)
        {
          Player tag = gamer.Tag as Player;
          if (tag != null)
            this.ScreenManager.SetViewport(new PlayerIndex?(tag.PlayerIndex), tag.Viewport);
        }
      }
    }

    public void SetupRenderTargets()
    {
      SurfaceFormat preferredFormat = SurfaceFormat.Color;
      List<NetworkGamer> localGamers = this.instance.NetworkManager.LocalGamers;
      lock (localGamers)
      {
        foreach (Gamer gamer in localGamers)
        {
          Player tag = gamer.Tag as Player;
          if (tag != null)
          {
            Viewport viewport = tag.Viewport;
            int screenId = tag.ScreenID;
            if (this.sceneRenderTargets[screenId] == null || this.sceneRenderTargets[screenId].Width != viewport.Width || this.sceneRenderTargets[screenId].Height != viewport.Height)
            {
              if (this.sceneRenderTargets[screenId] != null)
                this.sceneRenderTargets[screenId].Dispose();
              this.sceneRenderTargets[screenId] = new RenderTarget2D(this.GraphicsDevice, viewport.Width, viewport.Height, false, preferredFormat, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            }
            if (this.bpfRenderTargets[screenId] == null)
              this.bpfRenderTargets[screenId] = new RenderTarget2D(this.GraphicsDevice, 64, 60, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
          }
        }
      }
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      if (this.sceneRenderTargets != null)
      {
        foreach (RenderTarget2D sceneRenderTarget in this.sceneRenderTargets)
          sceneRenderTarget?.Dispose();
        foreach (RenderTarget2D bpfRenderTarget in this.bpfRenderTargets)
          bpfRenderTarget?.Dispose();
        this.sceneRenderTargets = (RenderTarget2D[]) null;
        this.bpfRenderTargets = (RenderTarget2D[]) null;
      }
      if (this.ArcadeMachineRenderer != null)
      {
        this.ArcadeMachineRenderer.UnloadContent();
        this.ArcadeMachineRenderer = (ArcadeMachineRenderer) null;
      }
      if (this.instance != null)
      {
        this.instance.UnloadContent();
        this.instance = (GameInstance) null;
      }
      GraphicStatics.SpriteBatchPool.Release(this.spritebatchPoint);
      GameplayScreen.ScreenInstance = (GameplayScreen) null;
    }

    public override bool HandleInput(InputState input)
    {
      if (this.instance == null || !this.instance.IsEnabled)
        return true;
      this.instance.HandleInput(input);
      return base.HandleInput(input);
    }

    private bool IsGamePadDisconnected(InputState input, PlayerIndex playerIndex)
    {
      if (this.instance != null)
      {
        Player localPlayer = this.instance.GetLocalPlayer(playerIndex);
        if (localPlayer != null && localPlayer.IsBot)
          return false;
      }
      return !GamePad.GetState(playerIndex).IsConnected;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(false);
      if (this.instance == null)
        return;
      this.instance.Update((UpdateState) null);
      this.instanceUpdated = true;
    }

    protected override void DrawCore()
    {
      try
      {
        if (GraphicStatics.TexturePack.NeedLightMap)
          GraphicStatics.TexturePack.LoadLightMap();
        if (this.instance == null || !this.instanceUpdated || (!this.instance.IsEnabledField || !this.instance.IsVisible))
          return;
        this.DrawInstance();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(100, ex);
      }
    }

    private void DrawInstance()
    {
      if (this.instance == null)
        return;
      if (this.instance.NetworkManager == null)
        return;
      try
      {
        this.DrawSetupAndPreparation();
        this.DrawToPlayerRenderTargets();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(118, ex);
      }
      this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      this.spritebatch.Begin();
      this.spritebatchPoint.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointWrap, (DepthStencilState) null, (RasterizerState) null);
      this.DrawPlayerRenderTargets();
      this.DrawOperationMessages();
      this.spritebatchPoint.End();
      this.spritebatch.End();
    }

    private void DrawSetupAndPreparation()
    {
      if (this.instance.TopViewMapOpenCount >= this.instance.NetworkManager.LocalGamerCount)
        return;
      this.instance.NpcManager?.PrepareForDraw();
      for (int index = this.instance.ArcadeMachines.Count - 1; index >= 0; --index)
        this.ArcadeMachineRenderer.Draw(this.instance.ArcadeMachines[index]);
    }

    private void DrawToPlayerRenderTargets()
    {
      this.instance.ChunkSortCount = 0;
      this.instance.DrawChunksMillisecs = 0;
      this.instance.DrawChunkSearchMillisecs = 0;
      foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
        this.DrawPlayer(localGamer.Tag as Player);
    }

    private void DrawPlayer(Player player)
    {
      if (player == null || !player.IsEnabledField || (player.IsViewingMainMap || player.IsViewLoading))
        return;
      Player virtualPlayer = player.VirtualPlayer;
      if (virtualPlayer == null)
        return;
      try
      {
        this.DrawPlayerCore(player, virtualPlayer);
      }
      catch (InvalidOperationException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(36, (Exception) ex);
      }
    }

    private void DrawPlayerCore(Player player, Player virtualPlayer)
    {
      this.SetPlayerCameraFrame(player);
      this.DrawPlayerBPFinder(player, virtualPlayer);
      this.DrawPlayerMiniMap(player, virtualPlayer);
      this.GraphicsDevice.SetRenderTarget(this.sceneRenderTargets[player.ScreenID]);
      this.instance.Draw(player, virtualPlayer);
      this.TakePlayerPhoto(player);
      this.TransitionPlayerScreenOn(player);
    }

    private void DrawPlayerBPFinder(Player player, Player virtualPlayer)
    {
      if (!player.Settings.BlueprintFinderVisible)
        return;
      this.GraphicsDevice.SetRenderTarget(this.bpfRenderTargets[player.ScreenID]);
      this.GraphicsDevice.Clear(Color.Black * 0.5f);
      this.instance.MapRenderer.DrawBlueprintFinder(player, virtualPlayer);
    }

    private void DrawPlayerMiniMap(Player player, Player virtualPlayer)
    {
      if (player.MiniMapRenderer == null || !player.MiniMapRenderer.IsEnabled || !player.Settings.MapVisible)
        return;
      player.MiniMapRenderer.Draw(virtualPlayer);
    }

    private void TransitionPlayerScreenOn(Player player)
    {
      if ((double) player.CurrentScreenTransitionOnTime <= 0.0)
        return;
      player.CurrentScreenTransitionOnTime -= Services.ElapsedTime;
      if ((double) player.CurrentScreenTransitionOnTime < 0.0)
        player.CurrentScreenTransitionOnTime = 0.0f;
      this.ScreenManager.FadeBackBufferToBlack((int) ((double) player.CurrentScreenTransitionOnTime / (double) player.ScreenTransitionOnTime * (double) byte.MaxValue));
    }

    private void DrawPlayerRenderTargets()
    {
      foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null)
        {
          Viewport viewport = this.instance.Viewports[tag.ScreenID];
          if (this.sceneRenderTargets != null)
            this.spritebatch.Draw((Texture2D) this.sceneRenderTargets[tag.ScreenID], viewport.Rectangle(), Color.White);
          if (--tag.NewVisitorTimer > -120 && tag.NewVisitorMsg != null)
          {
            float scale = tag.NewVisitorTimer > 0 ? MathHelper.SmoothStep(0.0f, 1.5f, (float) (1.0 / ((double) tag.NewVisitorTimer * 0.0500000007450581))) : 1.5f;
            this.spritebatch.DrawStringCentered(CoreGlobals.GameFont, tag.NewVisitorMsg, (float) (viewport.Height / 2 - 10 + viewport.Y), Color.White, scale);
            if (tag.NewVisitorTimer == -120)
              tag.NewVisitorMsg = (string) null;
          }
        }
      }
      if (this.instance.NetworkManager.LocalGamerCount == 3)
        this.DrawPlayerFourFiller();
      if (!this.instance.IsSplitScreen)
        return;
      this.DrawViewportBorders();
    }

    private void DrawOperationMessages()
    {
      this.instance.CreativeCommandQueue.Draw();
    }

    private void DrawViewportBorders()
    {
      Texture2D blankTexture = CoreGlobals.BlankTexture;
      Color gray = Color.Gray;
      Viewport viewport = this.GraphicsDevice.Viewport;
      if (this.instance.LocalPlayerCount == 2)
      {
        if (Globals2.GameSettings.SplitScreenVertical)
        {
          Rectangle destinationRectangle = new Rectangle(viewport.Width / 2 - 1, 0, 2, viewport.Height);
          this.spritebatch.Draw(blankTexture, destinationRectangle, gray);
        }
        else
        {
          Rectangle destinationRectangle = new Rectangle(0, viewport.Height / 2 - 1, viewport.Width, 2);
          this.spritebatch.Draw(blankTexture, destinationRectangle, gray);
        }
      }
      else
      {
        if (this.instance.LocalPlayerCount != 3 && this.instance.LocalPlayerCount != 4)
          return;
        Rectangle destinationRectangle = new Rectangle(viewport.Width / 2 - 1, 0, 2, viewport.Height);
        this.spritebatch.Draw(blankTexture, destinationRectangle, gray);
        destinationRectangle = new Rectangle(0, viewport.Height / 2 - 1, viewport.Width, 2);
        this.spritebatch.Draw(blankTexture, destinationRectangle, gray);
      }
    }

    private void DrawPlayerFourFiller()
    {
      Rectangle destinationRectangle = new Rectangle(0, 0, 48, 48);
      for (destinationRectangle.Y = 361; destinationRectangle.Y < 718; destinationRectangle.Y += 48)
      {
        for (destinationRectangle.X = 641; destinationRectangle.X < 1278; destinationRectangle.X += 48)
          this.spritebatchPoint.Draw(GraphicStatics.TexturePack.BlockTexture, destinationRectangle, new Rectangle?(GraphicStatics.TexturePack.BlockSrcRects[42]), Color.White);
      }
      destinationRectangle.X = 960 - this.logoTexture.Width / 2;
      destinationRectangle.Y = 540 - this.logoTexture.Height / 2;
      destinationRectangle.Width = this.logoTexture.Width;
      destinationRectangle.Height = this.logoTexture.Height;
      this.spritebatch.Draw(this.logoTexture, destinationRectangle, Color.White);
    }

    private void TakePlayerPhoto(Player player)
    {
      if (!player.IsTakingPhoto)
        return;
      player.IsTakingPhoto = false;
      Color[] photo1;
      Color[] photo2;
      Color[] photo3;
      try
      {
        int screenId = player.ScreenID;
        photo1 = this.TakePhoto(this.photoThumbnail16x16, this.sceneRenderTargets[screenId], player.CamRect, PhotoFileType.SDThumbnail);
        photo2 = this.TakePhoto(this.photoThumbnail64x64, this.sceneRenderTargets[screenId], player.CamRect, PhotoFileType.HDThumbnail);
        photo3 = this.TakePhoto(this.sceneRenderTargets[screenId], this.sceneRenderTargets[screenId], player.CamRect, PhotoFileType.PhotoImage);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(102, ex);
        player.IsAssemblingPhoto = false;
        return;
      }
      player.TakePhotoWorker.Initialize(player, photo3, photo2, photo1);
      ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) player.TakePhotoWorker, false, PriorityLevel.Priority);
    }

    private Color[] TakePhoto(
      RenderTarget2D rt,
      RenderTarget2D texture,
      Rectangle camRect,
      PhotoFileType type)
    {
      Rectangle destinationRectangle;
      if (type == PhotoFileType.HDThumbnail || type == PhotoFileType.SDThumbnail)
      {
        destinationRectangle = new Rectangle(0, 0, rt.Width, rt.Height);
        this.GraphicsDevice.SetRenderTarget(rt);
        this.spritebatch.Begin();
        this.spritebatch.Draw((Texture2D) texture, destinationRectangle, new Rectangle?(camRect), Color.White);
        this.spritebatch.End();
        this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      }
      else
        destinationRectangle = camRect;
      Color[] data = new Color[destinationRectangle.Width * destinationRectangle.Height];
      rt.GetData<Color>(0, new Rectangle?(destinationRectangle), data, 0, data.Length);
      return data;
    }

    private void SetPlayerCameraFrame(Player player)
    {
      if (!player.Inventory.IsEquippedInHand(Item.Camera))
        return;
      Rectangle rectangle1 = player.Viewport.Rectangle();
      Rectangle rectangle2 = new Rectangle()
      {
        Y = (int) ((double) rectangle1.Height * 0.100000001490116)
      };
      rectangle2.Height = rectangle1.Height - rectangle2.Y * 2;
      rectangle2.X = rectangle1.Width / 2 - rectangle2.Height / 2;
      rectangle2.Width = rectangle2.Height;
      rectangle2.Y /= 2;
      player.CamRect = rectangle2;
    }
  }
}
