// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.CoreGlobals
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System;
using System.Windows.Forms;

namespace StudioForge.Engine
{
  public static class CoreGlobals
  {
    public static int DebugVerbosity;
    public static SpriteFont GameFont;
    public static SpriteFont MenuFont;
    public static Texture2D ButtonTextureA;
    public static Texture2D ButtonTextureB;
    public static Texture2D ButtonTextureX;
    public static Texture2D ButtonTextureY;
    public static Texture2D ButtonTextureLB;
    public static Texture2D ButtonTextureRB;
    public static Texture2D ButtonTextureBack;
    public static Texture2D ButtonTextureStart;
    public static Texture2D ControllerFrontOrtho;
    public static Exception ThreadException;
    private static Microsoft.Xna.Framework.Game game;
    private static IFrameRateCounter frameRateCounter;
    private static IMessageDisplay messageDisplay;
    private static IAudioManager audioManager;
    private static ICamera camera;
    private static IContentManager contentManager;
    private static INotificationManager notificationManager;
    private static Texture2D blankTexture;
    private static GraphicsDevice graphicsDevice;
    private static SpriteBatchSafe spriteBatch;

    public static void ClearReferenceCache()
    {
      CoreGlobals.game = (Microsoft.Xna.Framework.Game) null;
      CoreGlobals.frameRateCounter = (IFrameRateCounter) null;
      CoreGlobals.messageDisplay = (IMessageDisplay) null;
      CoreGlobals.audioManager = (IAudioManager) null;
      CoreGlobals.camera = (ICamera) null;
      CoreGlobals.contentManager = (IContentManager) null;
      CoreGlobals.notificationManager = (INotificationManager) null;
      CoreGlobals.blankTexture = (Texture2D) null;
      CoreGlobals.graphicsDevice = (GraphicsDevice) null;
      CoreGlobals.spriteBatch = (SpriteBatchSafe) null;
    }

    public static Microsoft.Xna.Framework.Game Game
    {
      get
      {
        return CoreGlobals.game ?? (CoreGlobals.game = Services.GetService<Microsoft.Xna.Framework.Game>());
      }
    }

    public static IFrameRateCounter FrameRateCounter
    {
      get
      {
        return CoreGlobals.frameRateCounter ?? (CoreGlobals.frameRateCounter = Services.GetService<IFrameRateCounter>());
      }
    }

    public static IMessageDisplay Message
    {
      get
      {
        return CoreGlobals.messageDisplay ?? (CoreGlobals.messageDisplay = Services.GetService<IMessageDisplay>());
      }
    }

    public static IAudioManager AudioManager
    {
      get
      {
        return CoreGlobals.audioManager ?? (CoreGlobals.audioManager = Services.GetService<IAudioManager>());
      }
    }

    public static ICamera Camera
    {
      get
      {
        return CoreGlobals.camera ?? (CoreGlobals.camera = Services.GetService<ICamera>());
      }
    }

    public static IContentManager Content
    {
      get
      {
        return CoreGlobals.contentManager ?? (CoreGlobals.contentManager = Services.GetService<IContentManager>());
      }
    }

    public static INotificationManager NotificationManager
    {
      get
      {
        return CoreGlobals.notificationManager ?? (CoreGlobals.notificationManager = Services.GetService<INotificationManager>());
      }
    }

    public static Texture2D BlankTexture
    {
      get
      {
        return CoreGlobals.blankTexture ?? (CoreGlobals.blankTexture = Services.GetService<Texture2D>());
      }
    }

    public static GraphicsDevice GraphicsDevice
    {
      get
      {
        return CoreGlobals.graphicsDevice ?? (CoreGlobals.graphicsDevice = Services.GetService<IGraphicsDeviceService>().GraphicsDevice);
      }
    }

    public static SpriteBatchSafe SpriteBatch
    {
      get
      {
        return CoreGlobals.spriteBatch ?? (CoreGlobals.spriteBatch = Services.GetService<SpriteBatchSafe>());
      }
    }

    public static void LogInfoMessage(string header, string msg)
    {
      int num = (int) MessageBox.Show(msg, header, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    public static void LogWarningMessage(string header, string msg)
    {
      int num = (int) MessageBox.Show(msg, header, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public static void LogErrorMessage(string header, string msg)
    {
      int num = (int) MessageBox.Show(msg, header, MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }
  }
}
