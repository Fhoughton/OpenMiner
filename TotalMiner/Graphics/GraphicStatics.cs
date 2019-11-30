// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.GraphicStatics
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Renderers;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Graphics
{
  internal static class GraphicStatics
  {
    public static ChunkRenderType ChunkRenderType = ChunkRenderType.UnsortedLeavesOctree;
    public static int SplinterCount = 8;
    public static bool IsWireFrame = false;
    public static bool DrawCuboidModels = false;
    public static Vector3 FullLightColor1 = new Vector3(1f, 0.9607844f, 0.8078432f);
    public static Vector3 FullLightColor2 = new Vector3(0.9647059f, 0.7607844f, 0.4078432f);
    public static Vector3 FullLightColor3 = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
    public static Vector3 LightDirection1 = new Vector3(-0.5265408f, -0.5735765f, -0.6275069f);
    public static Vector3 LightDirection2 = new Vector3(0.7198464f, 0.3420201f, 0.6040227f);
    public static Vector3 LightDirection3 = new Vector3(0.4545195f, -0.7660444f, 0.4545195f);
    public static Vector3 AmbientColor = new Vector3(0.05f, 0.05f, 0.05f);
    public static Vec3Interpolator CustomTintColor = new Vec3Interpolator();
    public static Vec4Interpolator CustomSkyColor = new Vec4Interpolator();
    public static float PlateHeight = 0.06f;
    public static Color WindowBorderColor = Color.White;
    public static Color WindowClientColor = new Color(0.25f, 0.25f, 0.28f) * 0.95f;
    public static Vector3[] TorchParticlesOffset = new Vector3[6]
    {
      new Vector3(0.25f, 0.28f, 0.0f),
      new Vector3(0.0f, 0.28f, 0.25f),
      new Vector3(-0.25f, 0.28f, 0.0f),
      new Vector3(0.0f, 0.28f, -0.25f),
      new Vector3(0.0f, 0.2f, 0.0f),
      Vector3.Zero
    };
    private static int texturePacksToUnloadCounter = 0;
    private static Queue<TexturePack> texturePacksToUnload = new Queue<TexturePack>();
    private static Rectangle hudPos = new Rectangle(128, 72, 1152, 648);
    public static TexturePack TexturePack;
    public static bool IsLoadingTexturePack;
    public static PhotoData PhotoData;
    public static Texture2D AvatarPalette;
    public static Texture2D WindowBorderTiles;
    public static Texture2D GradientTexture;
    public static Texture2D ClanBanners;
    public static Texture2D KeysTexture;
    public static Texture2D LockedTexture;
    public static SpriteFont ItemTextFont;
    public static SpriteFont InvadersFont;
    public static SpriteFont SignTextFont;
    public static SpriteFont DebugFont;
    public static SpriteBatchPool SpriteBatchPool;
    public static Viewport DefaultViewport;

    public static Rectangle HUDPos(Player player)
    {
      if (player == null)
        return GraphicStatics.hudPos;
      Rectangle rectangle = new Rectangle();
      rectangle.X = GraphicStatics.hudPos.X;
      rectangle.Y = GraphicStatics.hudPos.Y;
      rectangle.Width = GraphicStatics.hudPos.Width;
      rectangle.Height = GraphicStatics.hudPos.Height;
      Viewport viewport = player.Viewport;
      int num = 4;
      bool flag1 = viewport.Width == GraphicStatics.DefaultViewport.Width;
      bool flag2 = viewport.Height == GraphicStatics.DefaultViewport.Height;
      if (!flag1)
      {
        if (viewport.X == 0)
          rectangle.X *= 2;
        else
          rectangle.X = num * 2;
        if (flag2)
        {
          rectangle.Y *= 2;
          rectangle.Height *= 2;
        }
      }
      if (!flag2)
      {
        if (viewport.Y == 0)
        {
          if (viewport.Width < GraphicStatics.DefaultViewport.Width)
          {
            rectangle.Y *= 2;
            rectangle.Height = GraphicStatics.DefaultViewport.Height - num * 2;
          }
          else
            rectangle.Height = viewport.Height - num;
        }
        else if (viewport.Width < GraphicStatics.DefaultViewport.Width)
        {
          rectangle.Y = num * 2;
          rectangle.Height = GraphicStatics.DefaultViewport.Height - GraphicStatics.hudPos.Y * 2;
        }
        else
        {
          rectangle.Y = num;
          rectangle.Height = viewport.Height - GraphicStatics.hudPos.Y;
        }
      }
      return rectangle;
    }

    public static Rectangle HUDPos()
    {
      return GraphicStatics.hudPos;
    }

    public static void SetHUDPos(int x, int y)
    {
      GraphicStatics.hudPos.X = x;
      GraphicStatics.hudPos.Y = y;
      GraphicStatics.hudPos.Width = GraphicStatics.DefaultViewport.Width - x;
      GraphicStatics.hudPos.Height = GraphicStatics.DefaultViewport.Height - y;
    }

    public static Rectangle GetClanBannerRect(byte bannerID)
    {
      return new Rectangle(((int) bannerID - 1) % 16 * 16, ((int) bannerID - 1) / 16 * 16, 16, 16);
    }

    public static void Construct()
    {
      if (GraphicStatics.SpriteBatchPool != null)
        return;
      GraphicStatics.SpriteBatchPool = new SpriteBatchPool(4);
    }

    public static void Initialize()
    {
      if (GraphicStatics.SignTextFont != null)
        return;
      GraphicStatics.InitShaders();
      SpriteFont spriteFont;
      GraphicStatics.DebugFont = spriteFont = CoreGlobals.Content.Load<SpriteFont>("Fonts\\CourierNewSmall");
      GraphicStatics.InvadersFont = spriteFont;
      GraphicStatics.SignTextFont = spriteFont;
      GraphicStatics.DebugFont = CoreGlobals.Content.Load<SpriteFont>("Fonts\\CourierNew");
      GraphicStatics.ItemTextFont = CoreGlobals.Content.Load<SpriteFont>("Fonts\\DefaultBold");
      GraphicStatics.AvatarPalette = CoreGlobals.Content.Load<Texture2D>("Textures\\tp_AvatarPalette");
      GraphicStatics.GradientTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\gradient");
      GraphicStatics.LockedTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\locked");
      InputManager.KeysTexture = GraphicStatics.KeysTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\Keys");
      GraphicStatics.ClanBanners = CoreGlobals.Content.Load<Texture2D>("Textures\\ClanBanners");
      GraphicStatics.AvatarShader.Texture.SetValue((Texture) GraphicStatics.AvatarPalette);
      GraphicStatics.PhotoData = new PhotoData();
      GraphicStatics.LoadTexturePack((MapTM) null, "Original HD", false, false);
    }

    public static void InitializeForTest()
    {
      GraphicStatics.TexturePack = (TexturePack) new TestTexturePack();
    }

    public static bool LoadWindowBorder(string id)
    {
      try
      {
        GraphicStatics.WindowBorderTiles = CoreGlobals.Content.Load<Texture2D>("Textures\\WindowBorder" + id);
        return true;
      }
      catch (ContentLoadException ex)
      {
        return false;
      }
      catch (ArgumentException ex)
      {
        return false;
      }
    }

    public static void GameInstanceCleanup()
    {
      GraphicStatics.PhotoData.ClearPhotoThumbnailColorData();
    }

    public static void Update()
    {
      GraphicStatics.UnloadOldTexturePacks();
    }

    private static void UnloadOldTexturePacks()
    {
      TexturePack texturePack = (TexturePack) null;
      lock (GraphicStatics.texturePacksToUnload)
      {
        if (GraphicStatics.texturePacksToUnload.Count > 0)
        {
          if (--GraphicStatics.texturePacksToUnloadCounter <= 0)
          {
            texturePack = GraphicStatics.texturePacksToUnload.Dequeue();
            GraphicStatics.texturePacksToUnloadCounter = 3;
          }
        }
      }
      if (texturePack == null || texturePack.Content == null)
        return;
      texturePack.Content.Unload();
    }

    private static void InitShaders()
    {
      GraphicStatics.GlobalShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\Standalone\\GlobalShaderPPL");
      GraphicStatics.GlobalShader.World = GraphicStatics.GlobalShader.Effect.Parameters["World"];
      GraphicStatics.GlobalShader.ViewProjection = GraphicStatics.GlobalShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.GlobalShader.CameraPosition = GraphicStatics.GlobalShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.GlobalShader.Texture = GraphicStatics.GlobalShader.Effect.Parameters["Texture1"];
      GraphicStatics.GlobalShader.LightColor1 = GraphicStatics.GlobalShader.Effect.Parameters["LightColor1"];
      GraphicStatics.GlobalShader.LightColor2 = GraphicStatics.GlobalShader.Effect.Parameters["LightColor2"];
      GraphicStatics.GlobalShader.LightColor3 = GraphicStatics.GlobalShader.Effect.Parameters["LightColor3"];
      GraphicStatics.GlobalShader.LightDirection1 = GraphicStatics.GlobalShader.Effect.Parameters["LightDirection1"];
      GraphicStatics.GlobalShader.LightDirection2 = GraphicStatics.GlobalShader.Effect.Parameters["LightDirection2"];
      GraphicStatics.GlobalShader.LightDirection3 = GraphicStatics.GlobalShader.Effect.Parameters["LightDirection3"];
      GraphicStatics.GlobalShader.AmbientColor = GraphicStatics.GlobalShader.Effect.Parameters["AmbientColor"];
      GraphicStatics.GlobalShader.TintColor = GraphicStatics.GlobalShader.Effect.Parameters["TintColor"];
      GraphicStatics.GlobalShader.LanturnColor = GraphicStatics.GlobalShader.Effect.Parameters["LanturnColor"];
      GraphicStatics.GlobalShader.LanturnRange = GraphicStatics.GlobalShader.Effect.Parameters["LanturnRange"];
      GraphicStatics.GlobalShader.LightColor1.SetValue(GraphicStatics.FullLightColor1);
      GraphicStatics.GlobalShader.LightColor2.SetValue(GraphicStatics.FullLightColor2);
      GraphicStatics.GlobalShader.LightColor3.SetValue(GraphicStatics.FullLightColor3);
      GraphicStatics.GlobalShader.LightDirection1.SetValue(GraphicStatics.LightDirection1);
      GraphicStatics.GlobalShader.LightDirection2.SetValue(GraphicStatics.LightDirection2);
      GraphicStatics.GlobalShader.LightDirection3.SetValue(GraphicStatics.LightDirection3);
      GraphicStatics.GlobalShader.AmbientColor.SetValue(GraphicStatics.AmbientColor);
      GraphicStatics.RainShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\RainShaderPPL");
      GraphicStatics.RainShader.World = GraphicStatics.RainShader.Effect.Parameters["World"];
      GraphicStatics.RainShader.ViewProjection = GraphicStatics.RainShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.RainShader.CameraPosition = GraphicStatics.RainShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.RainShader.CurrentTime = GraphicStatics.RainShader.Effect.Parameters["CurrentTime"];
      GraphicStatics.RainShader.MaxDistance = GraphicStatics.RainShader.Effect.Parameters["MaxDistance"];
      GraphicStatics.RainShader.LightValue = GraphicStatics.RainShader.Effect.Parameters["LightValue"];
      GraphicStatics.HailShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\HailShaderPPL");
      GraphicStatics.HailShader.World = GraphicStatics.HailShader.Effect.Parameters["World"];
      GraphicStatics.HailShader.ViewProjection = GraphicStatics.HailShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.HailShader.CameraPosition = GraphicStatics.HailShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.HailShader.CurrentTime = GraphicStatics.HailShader.Effect.Parameters["CurrentTime"];
      GraphicStatics.HailShader.MaxDistance = GraphicStatics.HailShader.Effect.Parameters["MaxDistance"];
      GraphicStatics.HailShader.LightValue = GraphicStatics.HailShader.Effect.Parameters["LightValue"];
      GraphicStatics.ParticleShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\ParticleShaderPPL");
      GraphicStatics.ParticleShader.World = GraphicStatics.ParticleShader.Effect.Parameters["World"];
      GraphicStatics.ParticleShader.ViewProjection = GraphicStatics.ParticleShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.ParticleShader.CameraPosition = GraphicStatics.ParticleShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.ParticleShader.TintColor = GraphicStatics.ParticleShader.Effect.Parameters["TintColor"];
      GraphicStatics.ParticleShader.FarClip = GraphicStatics.ParticleShader.Effect.Parameters["FarClip"];
      GraphicStatics.ParticleShader.FadeStart = GraphicStatics.ParticleShader.Effect.Parameters["FadeStart"];
      GraphicStatics.ParticleShader.FogStart = GraphicStatics.ParticleShader.Effect.Parameters["FogStart"];
      GraphicStatics.ParticleShader.FogEnd = GraphicStatics.ParticleShader.Effect.Parameters["FogEnd"];
      GraphicStatics.ParticleShader.FogColor = GraphicStatics.ParticleShader.Effect.Parameters["FogColor"];
      GraphicStatics.ParticleShader.Wind = GraphicStatics.ParticleShader.Effect.Parameters["Wind"];
      GraphicStatics.ParticleShader.CurrentTime = GraphicStatics.ParticleShader.Effect.Parameters["CurrentTime"];
      GraphicStatics.ParticleShader.LightValue = GraphicStatics.ParticleShader.Effect.Parameters["LightValue"];
      GraphicStatics.MapShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\MapShaderPPL");
      GraphicStatics.MapShader.World = GraphicStatics.MapShader.Effect.Parameters["World"];
      GraphicStatics.MapShader.ViewProjection = GraphicStatics.MapShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.MapShader.LightViewProjection = GraphicStatics.MapShader.Effect.Parameters["LightViewProjection"];
      GraphicStatics.MapShader.CameraPosition = GraphicStatics.MapShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.MapShader.TintColor = GraphicStatics.MapShader.Effect.Parameters["TintColor"];
      GraphicStatics.MapShader.Alpha = GraphicStatics.MapShader.Effect.Parameters["Alpha"];
      GraphicStatics.MapShader.FarClip = GraphicStatics.MapShader.Effect.Parameters["FarClip"];
      GraphicStatics.MapShader.FadeStart = GraphicStatics.MapShader.Effect.Parameters["FadeStart"];
      GraphicStatics.MapShader.FogStart = GraphicStatics.MapShader.Effect.Parameters["FogStart"];
      GraphicStatics.MapShader.FogEnd = GraphicStatics.MapShader.Effect.Parameters["FogEnd"];
      GraphicStatics.MapShader.FogColor = GraphicStatics.MapShader.Effect.Parameters["FogColor"];
      GraphicStatics.MapShader.WindAmount = GraphicStatics.MapShader.Effect.Parameters["WindAmount"];
      GraphicStatics.MapShader.WindDirection = GraphicStatics.MapShader.Effect.Parameters["WindDirection"];
      GraphicStatics.MapShader.WindTime = GraphicStatics.MapShader.Effect.Parameters["WindTime"];
      GraphicStatics.MapShader.RayDistance = GraphicStatics.MapShader.Effect.Parameters["RayDistance"];
      GraphicStatics.MapShader.SunDirection = GraphicStatics.MapShader.Effect.Parameters["SunDirection"];
      GraphicStatics.MapShader.SunPosition = GraphicStatics.MapShader.Effect.Parameters["SunPosition"];
      GraphicStatics.MapShader.MoonPosition = GraphicStatics.MapShader.Effect.Parameters["MoonPosition"];
      GraphicStatics.MapShader.SunEffectColorTextCoords = GraphicStatics.MapShader.Effect.Parameters["SunEffectColorTextCoords"];
      GraphicStatics.MapShader.MoonEffectColorTextCoords = GraphicStatics.MapShader.Effect.Parameters["MoonEffectColorTextCoords"];
      GraphicStatics.MapShader.SunSideNormal = GraphicStatics.MapShader.Effect.Parameters["SunSideNormal"];
      GraphicStatics.MapShader.FullLODDistance = GraphicStatics.MapShader.Effect.Parameters["FullLODDistance"];
      GraphicStatics.MapShader.TextureCoordMovement = GraphicStatics.MapShader.Effect.Parameters["TextureCoordMovement"];
      GraphicStatics.MapShader.TextureCoordFireOffset = GraphicStatics.MapShader.Effect.Parameters["TextureCoordFireOffset"];
      GraphicStatics.MapShader.TextureCoordLavaOffset = GraphicStatics.MapShader.Effect.Parameters["TextureCoordLavaOffset"];
      GraphicStatics.MapShader.TextureCoordWaterOffset = GraphicStatics.MapShader.Effect.Parameters["TextureCoordWaterOffset"];
      GraphicStatics.MapShader.Texture = GraphicStatics.MapShader.Effect.Parameters["Texture"];
      GraphicStatics.MapShader.TextureLOD = GraphicStatics.MapShader.Effect.Parameters["TextureLOD"];
      GraphicStatics.MapShader.LightCycle = GraphicStatics.MapShader.Effect.Parameters["LightCycle"];
      GraphicStatics.MapShader.LanturnColor = GraphicStatics.MapShader.Effect.Parameters["LanturnColor"];
      GraphicStatics.MapShader.LanturnRange = GraphicStatics.MapShader.Effect.Parameters["LanturnRange"];
      GraphicStatics.MapShader.LightMapTexture = GraphicStatics.MapShader.Effect.Parameters["LightMapTexture"];
      GraphicStatics.MapShader.NightLightMapTexture = GraphicStatics.MapShader.Effect.Parameters["NightLightMapTexture"];
      GraphicStatics.MapShader.ShadowMapTexture = GraphicStatics.MapShader.Effect.Parameters["ShadowMapTexture"];
      GraphicStatics.EntityShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\EntityShader");
      GraphicStatics.EntityShader.World = GraphicStatics.EntityShader.Effect.Parameters["World"];
      GraphicStatics.EntityShader.ViewProjection = GraphicStatics.EntityShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.EntityShader.CameraPosition = GraphicStatics.EntityShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.EntityShader.TintColor = GraphicStatics.EntityShader.Effect.Parameters["TintColor"];
      GraphicStatics.EntityShader.Alpha = GraphicStatics.EntityShader.Effect.Parameters["Alpha"];
      GraphicStatics.EntityShader.FarClip = GraphicStatics.EntityShader.Effect.Parameters["FarClip"];
      GraphicStatics.EntityShader.FadeStart = GraphicStatics.EntityShader.Effect.Parameters["FadeStart"];
      GraphicStatics.EntityShader.FogStart = GraphicStatics.EntityShader.Effect.Parameters["FogStart"];
      GraphicStatics.EntityShader.FogEnd = GraphicStatics.EntityShader.Effect.Parameters["FogEnd"];
      GraphicStatics.EntityShader.FogColor = GraphicStatics.EntityShader.Effect.Parameters["FogColor"];
      GraphicStatics.EntityShader.RayDistance = GraphicStatics.EntityShader.Effect.Parameters["RayDistance"];
      GraphicStatics.EntityShader.SunDirection = GraphicStatics.EntityShader.Effect.Parameters["SunDirection"];
      GraphicStatics.EntityShader.SunPosition = GraphicStatics.EntityShader.Effect.Parameters["SunPosition"];
      GraphicStatics.EntityShader.MoonPosition = GraphicStatics.EntityShader.Effect.Parameters["MoonPosition"];
      GraphicStatics.EntityShader.SunEffectColorTextCoords = GraphicStatics.EntityShader.Effect.Parameters["SunEffectColorTextCoords"];
      GraphicStatics.EntityShader.MoonEffectColorTextCoords = GraphicStatics.EntityShader.Effect.Parameters["MoonEffectColorTextCoords"];
      GraphicStatics.EntityShader.SunSideNormal = GraphicStatics.EntityShader.Effect.Parameters["SunSideNormal"];
      GraphicStatics.EntityShader.MaxLight = GraphicStatics.EntityShader.Effect.Parameters["MaxLight"];
      GraphicStatics.EntityShader.Texture = GraphicStatics.EntityShader.Effect.Parameters["Texture1"];
      GraphicStatics.EntityShader.LightCycle = GraphicStatics.EntityShader.Effect.Parameters["LightCycle"];
      GraphicStatics.EntityShader.LanturnColor = GraphicStatics.EntityShader.Effect.Parameters["LanturnColor"];
      GraphicStatics.EntityShader.LanturnRange = GraphicStatics.EntityShader.Effect.Parameters["LanturnRange"];
      GraphicStatics.EntityShader.LightMapTexture = GraphicStatics.EntityShader.Effect.Parameters["LightMapTexture"];
      GraphicStatics.EntityShader.NightLightMapTexture = GraphicStatics.EntityShader.Effect.Parameters["NightLightMapTexture"];
      GraphicStatics.AvatarShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\AvatarShader");
      GraphicStatics.AvatarShader.World = GraphicStatics.AvatarShader.Effect.Parameters["World"];
      GraphicStatics.AvatarShader.ViewProjection = GraphicStatics.AvatarShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.AvatarShader.CameraPosition = GraphicStatics.AvatarShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.AvatarShader.TintColor = GraphicStatics.AvatarShader.Effect.Parameters["TintColor"];
      GraphicStatics.AvatarShader.Alpha = GraphicStatics.AvatarShader.Effect.Parameters["Alpha"];
      GraphicStatics.AvatarShader.FarClip = GraphicStatics.AvatarShader.Effect.Parameters["FarClip"];
      GraphicStatics.AvatarShader.FadeStart = GraphicStatics.AvatarShader.Effect.Parameters["FadeStart"];
      GraphicStatics.AvatarShader.FogStart = GraphicStatics.AvatarShader.Effect.Parameters["FogStart"];
      GraphicStatics.AvatarShader.FogEnd = GraphicStatics.AvatarShader.Effect.Parameters["FogEnd"];
      GraphicStatics.AvatarShader.FogColor = GraphicStatics.AvatarShader.Effect.Parameters["FogColor"];
      GraphicStatics.AvatarShader.RayDistance = GraphicStatics.AvatarShader.Effect.Parameters["RayDistance"];
      GraphicStatics.AvatarShader.SunDirection = GraphicStatics.AvatarShader.Effect.Parameters["SunDirection"];
      GraphicStatics.AvatarShader.SunPosition = GraphicStatics.AvatarShader.Effect.Parameters["SunPosition"];
      GraphicStatics.AvatarShader.MoonPosition = GraphicStatics.AvatarShader.Effect.Parameters["MoonPosition"];
      GraphicStatics.AvatarShader.SunEffectColorTextCoords = GraphicStatics.AvatarShader.Effect.Parameters["SunEffectColorTextCoords"];
      GraphicStatics.AvatarShader.MoonEffectColorTextCoords = GraphicStatics.AvatarShader.Effect.Parameters["MoonEffectColorTextCoords"];
      GraphicStatics.AvatarShader.SunSideNormal = GraphicStatics.AvatarShader.Effect.Parameters["SunSideNormal"];
      GraphicStatics.AvatarShader.MaxLight = GraphicStatics.AvatarShader.Effect.Parameters["MaxLight"];
      GraphicStatics.AvatarShader.Texture = GraphicStatics.AvatarShader.Effect.Parameters["Texture1"];
      GraphicStatics.AvatarShader.LightCycle = GraphicStatics.AvatarShader.Effect.Parameters["LightCycle"];
      GraphicStatics.AvatarShader.LanturnColor = GraphicStatics.AvatarShader.Effect.Parameters["LanturnColor"];
      GraphicStatics.AvatarShader.LanturnRange = GraphicStatics.AvatarShader.Effect.Parameters["LanturnRange"];
      GraphicStatics.AvatarShader.LightMapTexture = GraphicStatics.AvatarShader.Effect.Parameters["LightMapTexture"];
      GraphicStatics.AvatarShader.NightLightMapTexture = GraphicStatics.AvatarShader.Effect.Parameters["NightLightMapTexture"];
      GraphicStatics.CubeAvatarShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\CubeAvatarShader");
      GraphicStatics.CubeAvatarShader.World = GraphicStatics.CubeAvatarShader.Effect.Parameters["World"];
      GraphicStatics.CubeAvatarShader.ViewProjection = GraphicStatics.CubeAvatarShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.ItemInHandShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\ItemInHand");
      GraphicStatics.ItemInHandShader.World = GraphicStatics.ItemInHandShader.Effect.Parameters["World"];
      GraphicStatics.ItemInHandShader.View = GraphicStatics.ItemInHandShader.Effect.Parameters["View"];
      GraphicStatics.ItemInHandShader.Projection = GraphicStatics.ItemInHandShader.Effect.Parameters["Projection"];
      GraphicStatics.ItemInHandShader.CameraPosition = GraphicStatics.ItemInHandShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.ItemInHandShader.TintColor = GraphicStatics.ItemInHandShader.Effect.Parameters["TintColor"];
      GraphicStatics.ItemInHandShader.Alpha = GraphicStatics.ItemInHandShader.Effect.Parameters["Alpha"];
      GraphicStatics.ItemInHandShader.Sunlight = GraphicStatics.ItemInHandShader.Effect.Parameters["Sunlight"];
      GraphicStatics.ItemInHandShader.Blocklight = GraphicStatics.ItemInHandShader.Effect.Parameters["Blocklight"];
      GraphicStatics.ItemInHandShader.FogStart = GraphicStatics.ItemInHandShader.Effect.Parameters["FogStart"];
      GraphicStatics.ItemInHandShader.FogEnd = GraphicStatics.ItemInHandShader.Effect.Parameters["FogEnd"];
      GraphicStatics.ItemInHandShader.FogColor = GraphicStatics.ItemInHandShader.Effect.Parameters["FogColor"];
      GraphicStatics.ItemInHandShader.Texture = GraphicStatics.ItemInHandShader.Effect.Parameters["Texture1"];
      GraphicStatics.ItemInHandShader.LightCycle = GraphicStatics.ItemInHandShader.Effect.Parameters["LightCycle"];
      GraphicStatics.ItemInHandShader.LanturnColor = GraphicStatics.ItemInHandShader.Effect.Parameters["LanturnColor"];
      GraphicStatics.ItemInHandShader.LanturnRange = GraphicStatics.ItemInHandShader.Effect.Parameters["LanturnRange"];
      GraphicStatics.ItemInHandShader.LightMapTexture = GraphicStatics.ItemInHandShader.Effect.Parameters["LightMapTexture"];
      GraphicStatics.ItemInHandShader.NightLightMapTexture = GraphicStatics.ItemInHandShader.Effect.Parameters["NightLightMapTexture"];
      GraphicStatics.StarfieldShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\MayNeedOptimize\\Starfield");
      GraphicStatics.StarfieldShader.World = GraphicStatics.StarfieldShader.Effect.Parameters["World"];
      GraphicStatics.StarfieldShader.ViewProjection = GraphicStatics.StarfieldShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.StarfieldShader.HorizY = GraphicStatics.StarfieldShader.Effect.Parameters["HorizY"];
      GraphicStatics.StarfieldShader.Alpha = GraphicStatics.StarfieldShader.Effect.Parameters["Alpha"];
      GraphicStatics.StarfieldShader.FogColor = GraphicStatics.StarfieldShader.Effect.Parameters["FogColor"];
      GraphicStatics.StarfieldShader.World.SetValue(Matrix.Identity);
      GraphicStatics.StarfieldShader.Effect.CurrentTechnique = GraphicStatics.StarfieldShader.Effect.Techniques["StarfieldShader"];
      GraphicStatics.SkyCurtainShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\MayNeedOptimize\\SkyCurtain");
      GraphicStatics.SkyCurtainShader.World = GraphicStatics.SkyCurtainShader.Effect.Parameters["World"];
      GraphicStatics.SkyCurtainShader.ViewProjection = GraphicStatics.SkyCurtainShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.SkyCurtainShader.LightCycle = GraphicStatics.SkyCurtainShader.Effect.Parameters["LightCycle"];
      GraphicStatics.SkyCurtainShader.RayDistance = GraphicStatics.SkyCurtainShader.Effect.Parameters["RayDistance"];
      GraphicStatics.SkyCurtainShader.SunPosition = GraphicStatics.SkyCurtainShader.Effect.Parameters["SunPosition"];
      GraphicStatics.SkyCurtainShader.MoonPosition = GraphicStatics.SkyCurtainShader.Effect.Parameters["MoonPosition"];
      GraphicStatics.SkyCurtainShader.SunEffectColorTextCoords = GraphicStatics.SkyCurtainShader.Effect.Parameters["SunEffectColorTextCoords"];
      GraphicStatics.SkyCurtainShader.MoonEffectColorTextCoords = GraphicStatics.SkyCurtainShader.Effect.Parameters["MoonEffectColorTextCoords"];
      GraphicStatics.SkyCurtainShader.FogColor = GraphicStatics.SkyCurtainShader.Effect.Parameters["FogColor"];
      GraphicStatics.SkyCurtainShader.CustomColor = GraphicStatics.SkyCurtainShader.Effect.Parameters["CustomColor"];
      GraphicStatics.SkyCurtainShader.TintColor = GraphicStatics.SkyCurtainShader.Effect.Parameters["TintColor"];
      GraphicStatics.SkyCurtainShader.MapBound = GraphicStatics.SkyCurtainShader.Effect.Parameters["MapBound"];
      GraphicStatics.SkyCurtainShader.FloorY = GraphicStatics.SkyCurtainShader.Effect.Parameters["FloorY"];
      GraphicStatics.SkyCurtainShader.Texture = GraphicStatics.SkyCurtainShader.Effect.Parameters["Texture1"];
      GraphicStatics.SkyCurtainShader.World.SetValue(Matrix.Identity);
      GraphicStatics.SkyCurtainShader.CustomColor.SetValue(Vector4.Zero);
      GraphicStatics.SkyCurtainShader.Effect.CurrentTechnique = GraphicStatics.SkyCurtainShader.Effect.Techniques["SkyCurtainShader"];
      GraphicStatics.SignTextShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\MayNeedOptimize\\SignTextShaderPPL");
      GraphicStatics.SignTextShader.World = GraphicStatics.SignTextShader.Effect.Parameters["World"];
      GraphicStatics.SignTextShader.ViewProjection = GraphicStatics.SignTextShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.SignTextShader.CameraPosition = GraphicStatics.SignTextShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.SignTextShader.FogStart = GraphicStatics.SignTextShader.Effect.Parameters["FogStart"];
      GraphicStatics.SignTextShader.FogEnd = GraphicStatics.SignTextShader.Effect.Parameters["FogEnd"];
      GraphicStatics.SignTextShader.FogColor = GraphicStatics.SignTextShader.Effect.Parameters["FogColor"];
      GraphicStatics.SignTextShader.Texture = GraphicStatics.SignTextShader.Effect.Parameters["Texture1"];
      GraphicStatics.SignTextShader.Effect.CurrentTechnique = GraphicStatics.SignTextShader.Effect.Techniques["SignTextShader"];
      GraphicStatics.CloudShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\CloudShaderPPL");
      GraphicStatics.CloudShader.World = GraphicStatics.CloudShader.Effect.Parameters["World"];
      GraphicStatics.CloudShader.ViewProjection = GraphicStatics.CloudShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.CloudShader.CameraPosition = GraphicStatics.CloudShader.Effect.Parameters["CameraPosition"];
      GraphicStatics.CloudShader.TintColor = GraphicStatics.CloudShader.Effect.Parameters["TintColor"];
      GraphicStatics.CloudShader.FarClip = GraphicStatics.CloudShader.Effect.Parameters["FarClip"];
      GraphicStatics.CloudShader.FadeStart = GraphicStatics.CloudShader.Effect.Parameters["FadeStart"];
      GraphicStatics.CloudShader.FogStart = GraphicStatics.CloudShader.Effect.Parameters["FogStart"];
      GraphicStatics.CloudShader.FogEnd = GraphicStatics.CloudShader.Effect.Parameters["FogEnd"];
      GraphicStatics.CloudShader.FogColor = GraphicStatics.CloudShader.Effect.Parameters["FogColor"];
      GraphicStatics.CloudShader.LightCycle = GraphicStatics.CloudShader.Effect.Parameters["LightCycle"];
      GraphicStatics.CloudShader.RayDistance = GraphicStatics.CloudShader.Effect.Parameters["RayDistance"];
      GraphicStatics.CloudShader.SunDirection = GraphicStatics.CloudShader.Effect.Parameters["SunDirection"];
      GraphicStatics.CloudShader.SunPosition = GraphicStatics.CloudShader.Effect.Parameters["SunPosition"];
      GraphicStatics.CloudShader.MoonPosition = GraphicStatics.CloudShader.Effect.Parameters["MoonPosition"];
      GraphicStatics.CloudShader.SunEffectColorTextCoords = GraphicStatics.CloudShader.Effect.Parameters["SunEffectColorTextCoords"];
      GraphicStatics.CloudShader.MoonEffectColorTextCoords = GraphicStatics.CloudShader.Effect.Parameters["MoonEffectColorTextCoords"];
      GraphicStatics.CloudShader.SunSideNormal = GraphicStatics.CloudShader.Effect.Parameters["SunSideNormal"];
      GraphicStatics.CloudShader.Alpha = GraphicStatics.CloudShader.Effect.Parameters["Alpha"];
      GraphicStatics.CloudShader.Texture = GraphicStatics.CloudShader.Effect.Parameters["Texture1"];
      GraphicStatics.CloudShader.LightMapTexture = GraphicStatics.CloudShader.Effect.Parameters["LightMapTexture"];
      GraphicStatics.CloudShader.NightLightMapTexture = GraphicStatics.CloudShader.Effect.Parameters["NightLightMapTexture"];
      GraphicStatics.CloudShader.Effect.CurrentTechnique = GraphicStatics.CloudShader.Effect.Techniques["CloudShader"];
      GraphicStatics.SunMoonShader.Effect = CoreGlobals.Content.Load<Effect>("Effects\\MayNeedOptimize\\SunMoon");
      GraphicStatics.SunMoonShader.World = GraphicStatics.SunMoonShader.Effect.Parameters["World"];
      GraphicStatics.SunMoonShader.ViewProjection = GraphicStatics.SunMoonShader.Effect.Parameters["ViewProjection"];
      GraphicStatics.SunMoonShader.HorizY = GraphicStatics.SunMoonShader.Effect.Parameters["HorizY"];
      GraphicStatics.SunMoonShader.Intensity = GraphicStatics.SunMoonShader.Effect.Parameters["Intensity"];
      GraphicStatics.SunMoonShader.SunEffectColorTextCoords = GraphicStatics.SunMoonShader.Effect.Parameters["SunEffectColorTextCoords"];
      GraphicStatics.SunMoonShader.MoonEffectColorTextCoords = GraphicStatics.SunMoonShader.Effect.Parameters["MoonEffectColorTextCoords"];
      GraphicStatics.SunMoonShader.Texture = GraphicStatics.SunMoonShader.Effect.Parameters["Texture1"];
      GraphicStatics.SunMoonShader.Effect.CurrentTechnique = GraphicStatics.SunMoonShader.Effect.Techniques["SunShader"];
    }

    public static void LoadTexturePack(
      MapTM map,
      string name,
      bool copyPaintingData,
      bool isReload)
    {
      if (name == null)
        name = "Original HD";
      if (!isReload && GraphicStatics.TexturePack != null && name == GraphicStatics.TexturePack.Name)
        return;
      GraphicStatics.IsLoadingTexturePack = true;
      while (true)
      {
        TexturePack texPack = new TexturePack()
        {
          Name = name
        };
        try
        {
          texPack.Content = new StudioForge.Engine.Core.ContentManager(Services.GetService<IServiceProvider>(), "Content");
          texPack.BlockTexture = texPack.Content.Load<Texture2D>("Textures\\tp_" + name);
          texPack.ItemTexture = texPack.Content.Load<Texture2D>("Textures\\tpi_" + name);
          try
          {
            texPack.BlockTextureLOD = texPack.Content.Load<Texture2D>("Textures\\tp4_" + name);
          }
          catch (ContentLoadException ex)
          {
          }
          texPack.LoadTexturePack();
          if (copyPaintingData)
            GraphicStatics.PhotoData.LoadPaintingsIntoTPImmediate(map, texPack);
          GraphicStatics.SetTexturePacksOnDevice(texPack);
          if (GraphicStatics.TexturePack != null)
          {
            GraphicStatics.texturePacksToUnload.Enqueue(GraphicStatics.TexturePack);
            GraphicStatics.texturePacksToUnloadCounter = 3;
          }
          GraphicStatics.TexturePack = texPack;
          break;
        }
        catch (ContentLoadException ex1)
        {
          try
          {
            texPack.BlockTextureLOD = (Texture2D) null;
            texPack.BlockTexture = GraphicStatics.LoadTextureFromFile(texPack.Content.RootDirectory + "\\Textures\\tp_" + name + ".png");
            texPack.ItemTexture = GraphicStatics.LoadTextureFromFile(texPack.Content.RootDirectory + "\\Textures\\tpi_" + name + ".png");
            if (texPack.BlockTexture != null)
            {
              try
              {
                texPack.BlockTextureLOD = GraphicStatics.LoadTextureFromFile(texPack.Content.RootDirectory + "\\Textures\\tp4_" + name + ".png");
              }
              catch (FileNotFoundException ex2)
              {
              }
              texPack.LoadTexturePack();
              if (copyPaintingData)
                GraphicStatics.PhotoData.LoadPaintingsIntoTPImmediate(map, texPack);
              GraphicStatics.SetTexturePacksOnDevice(texPack);
              if (GraphicStatics.TexturePack != null)
              {
                GraphicStatics.texturePacksToUnload.Enqueue(GraphicStatics.TexturePack);
                GraphicStatics.texturePacksToUnloadCounter = 3;
              }
              GraphicStatics.TexturePack = texPack;
              break;
            }
            name = "Original HD";
            texPack.Content.Unload();
          }
          catch (OutOfMemoryException ex2)
          {
            Services.ExceptionReporter.ReportExceptionCaught(111, (Exception) ex2);
            break;
          }
        }
        finally
        {
          GraphicStatics.IsLoadingTexturePack = false;
        }
      }
    }

    public static Texture2D LoadTextureFromFile(string filename)
    {
      Texture2D texture2D = (Texture2D) null;
      try
      {
        using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
          texture2D = Texture2D.FromStream(CoreGlobals.GraphicsDevice, (Stream) fileStream);
      }
      catch (FileNotFoundException ex)
      {
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(109, ex);
      }
      return texture2D;
    }

    private static void SetTexturePacksOnDevice(TexturePack texPack)
    {
      MapChunkContent.TexCoords1 = texPack.TexCoords1;
      MapChunkContent.TexCoords2 = texPack.TexCoords2;
      MapChunkContent.TexCoords3 = texPack.TexCoords3;
      MapChunkContent.TexCoords4 = texPack.TexCoords4;
      MapChunkContent.TexOffsets = texPack.TexOffsets;
      GraphicStatics.MapShader.Texture.SetValue((Texture) texPack.BlockTexture);
      GraphicStatics.MapShader.TextureLOD.SetValue((Texture) texPack.BlockTextureLOD);
      GraphicStatics.EntityShader.Texture.SetValue((Texture) texPack.BlockTexture);
      GraphicStatics.ItemInHandShader.Texture.SetValue((Texture) texPack.BlockTexture);
      GraphicStatics.SkyCurtainShader.Texture.SetValue((Texture) texPack.BlockTexture);
      GraphicStatics.SunMoonShader.Texture.SetValue((Texture) texPack.BlockTexture);
      GraphicStatics.CloudShader.Texture.SetValue((Texture) texPack.BlockTexture);
      GraphicStatics.GlobalShader.Texture.SetValue((Texture) texPack.BlockTexture);
    }

    public static void SetLightMaps(Texture2D lightMapTexture, Texture2D nightLightMapTexture)
    {
      GraphicStatics.MapShader.LightMapTexture.SetValue((Texture) lightMapTexture);
      GraphicStatics.AvatarShader.LightMapTexture.SetValue((Texture) lightMapTexture);
      GraphicStatics.EntityShader.LightMapTexture.SetValue((Texture) lightMapTexture);
      GraphicStatics.CloudShader.LightMapTexture.SetValue((Texture) lightMapTexture);
      GraphicStatics.ItemInHandShader.LightMapTexture.SetValue((Texture) lightMapTexture);
      GraphicStatics.MapShader.NightLightMapTexture.SetValue((Texture) nightLightMapTexture);
      GraphicStatics.AvatarShader.NightLightMapTexture.SetValue((Texture) nightLightMapTexture);
      GraphicStatics.EntityShader.NightLightMapTexture.SetValue((Texture) nightLightMapTexture);
      GraphicStatics.CloudShader.NightLightMapTexture.SetValue((Texture) nightLightMapTexture);
      GraphicStatics.ItemInHandShader.NightLightMapTexture.SetValue((Texture) nightLightMapTexture);
    }

    public static void InitIndices(int[] indices)
    {
      int num = 0;
      for (int index = 0; index < indices.Length; index += 6)
      {
        indices[index] = num;
        indices[index + 1] = num + 1;
        indices[index + 2] = num + 2;
        indices[index + 3] = num;
        indices[index + 4] = num + 2;
        indices[index + 5] = num + 3;
        num += 4;
      }
    }

    public static void InitIndices(CustomArray<int> indices, int count)
    {
      int t = indices.Count > 0 ? indices.Array[indices.Count - 1] + 1 : 0;
      while (indices.Count < count)
      {
        indices.Add(t);
        indices.Add(t + 1);
        indices.Add(t + 2);
        indices.Add(t);
        indices.Add(t + 2);
        indices.Add(t + 3);
        t += 4;
      }
    }

    public static void DrawCursor(SpriteBatchSafe spriteBatch, Rectangle rect, Color color)
    {
      rect.X -= 2;
      rect.Y -= 2;
      rect.Width += 4;
      rect.Height += 4;
      spriteBatch.DrawBox(CoreGlobals.BlankTexture, rect, 1, Color.Black, 0.0f);
      ++rect.X;
      ++rect.Y;
      rect.Width -= 2;
      rect.Height -= 2;
      spriteBatch.DrawBox(CoreGlobals.BlankTexture, rect, 4, color, 0.0f);
      rect.X += 4;
      rect.Y += 4;
      rect.Width -= 8;
      rect.Height -= 8;
      spriteBatch.DrawBox(CoreGlobals.BlankTexture, rect, 1, Color.Black, 0.0f);
    }

    public static void DrawMessageBoxBackground(
      SpriteBatchSafe spriteBatch,
      Rectangle rect,
      float alpha,
      Matrix matrix)
    {
      rect.X -= 16;
      rect.Y -= 10;
      rect.Width += 32;
      rect.Height += 20;
      spriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, rect, alpha, true, 8, GraphicStatics.WindowBorderColor, GraphicStatics.WindowClientColor, true, matrix);
      spriteBatch.End();
    }

    public static void DrawItem(
      SpriteBatchSafe spriteBatch,
      SpriteBatchSafe spriteBatchPoint,
      SpriteBatchSafe spriteBatchText,
      int x,
      int y,
      InventoryItem item,
      bool drawData)
    {
      GraphicStatics.DrawItem(spriteBatch, spriteBatchPoint, spriteBatchText, x, y, item, drawData, false, 1f);
    }

    public static void DrawItem(
      SpriteBatchSafe spriteBatch,
      SpriteBatchSafe spriteBatchPoint,
      SpriteBatchSafe spriteBatchText,
      int x,
      int y,
      InventoryItem item,
      bool drawData,
      bool forceDrawCount,
      float alpha)
    {
      Rectangle slotRect = new Rectangle(x + 9, y + 9, 32, 32);
      GraphicStatics.DrawItem(spriteBatch, spriteBatchPoint, spriteBatchText, slotRect, item, drawData, forceDrawCount, alpha);
    }

    public static void DrawItem(
      SpriteBatchSafe spriteBatch,
      SpriteBatchSafe spriteBatchPoint,
      Rectangle slotRect,
      Item item)
    {
      GraphicStatics.DrawItem(spriteBatch, spriteBatchPoint, (SpriteBatchSafe) null, slotRect, new InventoryItem(item, 1), true, false, 1f);
    }

    private static void DrawItem(
      SpriteBatchSafe spriteBatch,
      SpriteBatchSafe spriteBatchPoint,
      SpriteBatchSafe spriteBatchText,
      Rectangle slotRect,
      InventoryItem item,
      bool drawData,
      bool forceDrawCount,
      float alpha)
    {
      spriteBatchPoint.Draw(GraphicStatics.TexturePack.GetTexureForItem(item.ItemID), slotRect, new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(item.ItemID)), Color.White * alpha);
      if (!drawData)
        return;
      GraphicStatics.DrawItemData(spriteBatch, spriteBatchPoint, spriteBatchText, slotRect, item, GraphicStatics.ShowDurabilityBar(item), forceDrawCount, alpha);
    }

    public static void DrawItemData(
      SpriteBatchSafe spriteBatch,
      SpriteBatchSafe spriteBatchPoint,
      SpriteBatchSafe spriteBatchText,
      Rectangle slotRect,
      InventoryItem item,
      bool drawDurability,
      bool forceDrawCount,
      float alpha)
    {
      if (drawDurability)
      {
        int num = slotRect.Height - 2;
        slotRect.Y += num;
        slotRect.Height = 4;
        spriteBatch.Draw(CoreGlobals.BlankTexture, slotRect, Color.Gray * alpha);
        slotRect.Width = (int) ((double) slotRect.Width * (double) Math.Min(1f, (float) item.Durability / (float) item.MaxDurability));
        spriteBatchPoint.Draw(CoreGlobals.BlankTexture, slotRect, Color.Green * alpha);
        slotRect.Y -= num;
      }
      if (item.Count <= 1 && !forceDrawCount || spriteBatchText == null)
        return;
      string itemCountString = Globals2.GetItemCountString(item.Count);
      float x = GraphicStatics.ItemTextFont.MeasureString(itemCountString).X;
      Vector2 position = new Vector2((float) (slotRect.X + slotRect.Width + 1) - x, (float) (slotRect.Y + 20));
      if ((double) x > (double) (slotRect.Width + 4))
        position.X = (float) ((double) slotRect.X + (double) slotRect.Width / 2.0 - (double) x / 2.0);
      spriteBatchText.DrawString(GraphicStatics.ItemTextFont, itemCountString, position + new Vector2(2f, 2f), Color.Black * alpha, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
      spriteBatchText.DrawString(GraphicStatics.ItemTextFont, itemCountString, position, Color.White * alpha, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
    }

    private static bool ShowDurabilityBar(InventoryItem item)
    {
      return item.ShowDurabilityBar;
    }

    public static void DrawInputIcon(
      SpriteBatchSafe spriteBatch,
      PlayerInput input,
      Rectangle rect)
    {
      GraphicStatics.DrawInputIcon(spriteBatch, (ushort) input, rect, Color.White);
    }

    public static void DrawInputIcon(
      SpriteBatchSafe spriteBatch,
      PlayerInput input,
      Rectangle rect,
      Color color)
    {
      GraphicStatics.DrawInputIcon(spriteBatch, (ushort) input, rect, color);
    }

    public static void DrawInputIcon(SpriteBatchSafe spriteBatch, GuiInput input, Rectangle rect)
    {
      GraphicStatics.DrawInputIcon(spriteBatch, (ushort) input, rect, Color.White);
    }

    public static void DrawInputIcon(
      SpriteBatchSafe spriteBatch,
      GuiInput input,
      Rectangle rect,
      Color color)
    {
      GraphicStatics.DrawInputIcon(spriteBatch, (ushort) input, rect, color);
    }

    public static void DrawInputIcon(
      SpriteBatchSafe spriteBatch,
      ushort input,
      Rectangle rect,
      Color color)
    {
      InputItem inputItem = InputManager.GetInputItem(PlayerIndex.One, input);
      if (InputManager.IsUsingGamePad)
      {
        spriteBatch.Draw(GraphicStatics.ButtonTexture(inputItem.Button), rect, color);
      }
      else
      {
        Rectangle rectangle = new Rectangle(0, 0, 24, 24);
        if (rect.Width >= 48)
        {
          rectangle.X = 60;
          rectangle.Width = 64;
          rectangle.Height = 64;
        }
        else if (rect.Width > 24)
        {
          rectangle.X = 26;
          rectangle.Width = 32;
          rectangle.Height = 32;
        }
        spriteBatch.Draw(GraphicStatics.KeysTexture, rect, new Rectangle?(rectangle), color);
        string text = (string) null;
        float scale = 0.5f;
        rectangle.X = -1;
        switch (inputItem.Key)
        {
          case Keys.Escape:
            text = "Esc";
            scale = 0.4f;
            break;
          case Keys.PageUp:
            text = "PgUp";
            scale = 0.3f;
            break;
          case Keys.PageDown:
            text = "PgDn";
            scale = 0.3f;
            break;
          case Keys.End:
            text = "End";
            scale = 0.4f;
            break;
          case Keys.Home:
            text = "Home";
            scale = 0.3f;
            break;
          case Keys.LeftShift:
          case Keys.RightShift:
            text = "Shft";
            scale = 0.3f;
            break;
          case Keys.LeftControl:
          case Keys.RightControl:
            text = "Ctrl";
            scale = 0.3f;
            break;
          case Keys.LeftAlt:
          case Keys.RightAlt:
            text = "Alt";
            scale = 0.4f;
            break;
          case Keys.OemSemicolon:
          case Keys.OemPlus:
          case Keys.OemComma:
          case Keys.OemMinus:
          case Keys.OemPeriod:
          case Keys.OemQuestion:
          case Keys.OemTilde:
          case Keys.OemOpenBrackets:
          case Keys.OemCloseBrackets:
          case Keys.OemQuotes:
          case Keys.OemBackslash:
            text = inputItem.Key.ToString();
            break;
          default:
            if (inputItem.Key >= Keys.A && inputItem.Key <= Keys.Z || inputItem.Key >= Keys.D0 && inputItem.Key <= Keys.D9)
            {
              text = inputItem.Key.ToString();
              break;
            }
            rectangle = GraphicStatics.GetKeySrcRect(inputItem);
            break;
        }
        if (text != null)
        {
          Vector2 vector2 = CoreGlobals.GameFont.MeasureString(text) * scale;
          Vector2 position = new Vector2((float) rect.X + ((float) (rect.Width / 2) - vector2.X * 0.5f), (float) ((double) rect.Y + ((double) (rect.Height / 2) - (double) vector2.Y * 0.5) + 1.0));
          spriteBatch.DrawString(CoreGlobals.GameFont, text, position, Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }
        else
        {
          if (rectangle.X < 0)
            return;
          float num1 = (float) rect.Width * 0.75f;
          float num2 = (float) rect.Width * 0.75f;
          rect.X = (int) ((double) rect.X + (double) rect.Width * 0.125);
          rect.Y = (int) ((double) rect.Y + (double) rect.Height * 0.125);
          rect.Width = (int) num1;
          rect.Height = (int) num2;
          spriteBatch.Draw(GraphicStatics.KeysTexture, rect, new Rectangle?(rectangle), color);
        }
      }
    }

    public static Rectangle GetKeySrcRect(InputItem item)
    {
      switch (item.Key)
      {
        case Keys.Back:
          return new Rectangle(31, 34, 27, 18);
        case Keys.Enter:
          return new Rectangle(0, 26, 18, 12);
        default:
          return new Rectangle(-1, 0, 0, 0);
      }
    }

    public static Texture2D ButtonTexture(Buttons button)
    {
      switch (button)
      {
        case Buttons.DPadUp:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonDpadUp");
        case Buttons.DPadDown:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonDpadDown");
        case Buttons.DPadLeft:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonDpadLeft");
        case Buttons.DPadRight:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonDpadRight");
        case Buttons.Start:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonStart");
        case Buttons.Back:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonBack");
        case Buttons.LeftStick:
        case Buttons.LeftThumbstickLeft:
        case Buttons.LeftThumbstickUp:
        case Buttons.LeftThumbstickDown:
        case Buttons.LeftThumbstickRight:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonLeftStick");
        case Buttons.RightStick:
        case Buttons.RightThumbstickUp:
        case Buttons.RightThumbstickDown:
        case Buttons.RightThumbstickRight:
        case Buttons.RightThumbstickLeft:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonRightStick");
        case Buttons.LeftShoulder:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonLB");
        case Buttons.RightShoulder:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonRB");
        case Buttons.A:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonA");
        case Buttons.B:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonB");
        case Buttons.X:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonX");
        case Buttons.Y:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonY");
        case Buttons.RightTrigger:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonRightTrigger");
        case Buttons.LeftTrigger:
          return CoreGlobals.Content.Load<Texture2D>("Textures\\IconButtonLeftTrigger");
        default:
          return CoreGlobals.BlankTexture;
      }
    }

    public static long BufferSize()
    {
      long num = 0;
      if (GraphicStatics.TexturePack.Content != null)
        num = GraphicStatics.TexturePack.Content.BufferSize + (long) (65536 * Globals2.DeviceVirtualization);
      return num;
    }

    public static void AddIcon(Window parent, Texture2D texture, Rectangle rect)
    {
      Window window = new Window((string) null, rect.X, rect.Y, rect.Width, rect.Height);
      window.LoadTexture(texture, true, true, 1f);
      window.Colors = Colors.IconColors;
      window.IsEnabled = false;
      parent.AddChild((StudioForge.Engine.Core.Node) window);
    }

    public static class GlobalShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter Texture;
      public static EffectParameter LightColor1;
      public static EffectParameter LightColor2;
      public static EffectParameter LightColor3;
      public static EffectParameter LightDirection1;
      public static EffectParameter LightDirection2;
      public static EffectParameter LightDirection3;
      public static EffectParameter AmbientColor;
      public static EffectParameter TintColor;
      public static EffectParameter LanturnColor;
      public static EffectParameter LanturnRange;
    }

    public static class RainShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter CurrentTime;
      public static EffectParameter MaxDistance;
      public static EffectParameter LightValue;
    }

    public static class HailShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter CurrentTime;
      public static EffectParameter MaxDistance;
      public static EffectParameter LightValue;
    }

    public static class ParticleShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter TintColor;
      public static EffectParameter FarClip;
      public static EffectParameter FadeStart;
      public static EffectParameter FogStart;
      public static EffectParameter FogEnd;
      public static EffectParameter FogColor;
      public static EffectParameter Wind;
      public static EffectParameter CurrentTime;
      public static EffectParameter LightValue;
    }

    public static class MapShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter LightViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter TintColor;
      public static EffectParameter Alpha;
      public static EffectParameter FarClip;
      public static EffectParameter FadeStart;
      public static EffectParameter FogStart;
      public static EffectParameter FogEnd;
      public static EffectParameter FogColor;
      public static EffectParameter WindDirection;
      public static EffectParameter WindAmount;
      public static EffectParameter WindTime;
      public static EffectParameter RayDistance;
      public static EffectParameter SunDirection;
      public static EffectParameter SunPosition;
      public static EffectParameter MoonPosition;
      public static EffectParameter SunEffectColorTextCoords;
      public static EffectParameter MoonEffectColorTextCoords;
      public static EffectParameter SunSideNormal;
      public static EffectParameter FullLODDistance;
      public static EffectParameter TextureCoordMovement;
      public static EffectParameter TextureCoordFireOffset;
      public static EffectParameter TextureCoordLavaOffset;
      public static EffectParameter TextureCoordWaterOffset;
      public static EffectParameter Texture;
      public static EffectParameter TextureLOD;
      public static EffectParameter LightCycle;
      public static EffectParameter LanturnColor;
      public static EffectParameter LanturnRange;
      public static EffectParameter LightMapTexture;
      public static EffectParameter NightLightMapTexture;
      public static EffectParameter ShadowMapTexture;
    }

    public static class EntityShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter TintColor;
      public static EffectParameter Alpha;
      public static EffectParameter MaxLight;
      public static EffectParameter FarClip;
      public static EffectParameter FadeStart;
      public static EffectParameter FogStart;
      public static EffectParameter FogEnd;
      public static EffectParameter FogColor;
      public static EffectParameter RayDistance;
      public static EffectParameter SunDirection;
      public static EffectParameter SunPosition;
      public static EffectParameter MoonPosition;
      public static EffectParameter SunEffectColorTextCoords;
      public static EffectParameter MoonEffectColorTextCoords;
      public static EffectParameter SunSideNormal;
      public static EffectParameter Texture;
      public static EffectParameter LightCycle;
      public static EffectParameter LanturnColor;
      public static EffectParameter LanturnRange;
      public static EffectParameter LightMapTexture;
      public static EffectParameter NightLightMapTexture;
    }

    public static class AvatarShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter TintColor;
      public static EffectParameter Alpha;
      public static EffectParameter MaxLight;
      public static EffectParameter FarClip;
      public static EffectParameter FadeStart;
      public static EffectParameter FogStart;
      public static EffectParameter FogEnd;
      public static EffectParameter FogColor;
      public static EffectParameter RayDistance;
      public static EffectParameter SunDirection;
      public static EffectParameter SunPosition;
      public static EffectParameter MoonPosition;
      public static EffectParameter SunEffectColorTextCoords;
      public static EffectParameter MoonEffectColorTextCoords;
      public static EffectParameter SunSideNormal;
      public static EffectParameter Texture;
      public static EffectParameter LightCycle;
      public static EffectParameter LanturnColor;
      public static EffectParameter LanturnRange;
      public static EffectParameter LightMapTexture;
      public static EffectParameter NightLightMapTexture;
    }

    public static class CubeAvatarShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
    }

    public static class ItemInHandShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter View;
      public static EffectParameter Projection;
      public static EffectParameter CameraPosition;
      public static EffectParameter TintColor;
      public static EffectParameter Alpha;
      public static EffectParameter FogStart;
      public static EffectParameter FogEnd;
      public static EffectParameter FogColor;
      public static EffectParameter Sunlight;
      public static EffectParameter Blocklight;
      public static EffectParameter Texture;
      public static EffectParameter LightCycle;
      public static EffectParameter LanturnColor;
      public static EffectParameter LanturnRange;
      public static EffectParameter LightMapTexture;
      public static EffectParameter NightLightMapTexture;
    }

    public static class StarfieldShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter HorizY;
      public static EffectParameter Alpha;
      public static EffectParameter FogColor;
    }

    public static class SkyCurtainShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter LightCycle;
      public static EffectParameter RayDistance;
      public static EffectParameter SunPosition;
      public static EffectParameter MoonPosition;
      public static EffectParameter SunEffectColorTextCoords;
      public static EffectParameter MoonEffectColorTextCoords;
      public static EffectParameter FogColor;
      public static EffectParameter CustomColor;
      public static EffectParameter TintColor;
      public static EffectParameter MapBound;
      public static EffectParameter FloorY;
      public static EffectParameter Texture;
    }

    public static class SignTextShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter FogStart;
      public static EffectParameter FogEnd;
      public static EffectParameter FogColor;
      public static EffectParameter Texture;
    }

    public static class CloudShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter CameraPosition;
      public static EffectParameter TintColor;
      public static EffectParameter LightCycle;
      public static EffectParameter RayDistance;
      public static EffectParameter SunDirection;
      public static EffectParameter SunPosition;
      public static EffectParameter MoonPosition;
      public static EffectParameter SunEffectColorTextCoords;
      public static EffectParameter MoonEffectColorTextCoords;
      public static EffectParameter SunSideNormal;
      public static EffectParameter FarClip;
      public static EffectParameter FadeStart;
      public static EffectParameter FogStart;
      public static EffectParameter FogEnd;
      public static EffectParameter FogColor;
      public static EffectParameter Texture;
      public static EffectParameter LightMapTexture;
      public static EffectParameter NightLightMapTexture;
      public static EffectParameter Alpha;
    }

    public static class SunMoonShader
    {
      public static Effect Effect;
      public static EffectParameter World;
      public static EffectParameter ViewProjection;
      public static EffectParameter HorizY;
      public static EffectParameter Intensity;
      public static EffectParameter SunEffectColorTextCoords;
      public static EffectParameter MoonEffectColorTextCoords;
      public static EffectParameter Texture;
    }
  }
}
