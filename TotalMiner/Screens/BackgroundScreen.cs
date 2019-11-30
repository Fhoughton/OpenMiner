// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BackgroundScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class BackgroundScreen : StudioForge.Engine.GameState.BackgroundScreen
  {
    private static bool[] shown = new bool[113];
    private static string[] digDeepHints = new string[13]
    {
      "Be sure to chop some wood before your last axe degrades.",
      "Buy your equipment, or craft it. You choose.",
      "In the Dig Deep game mode, you can only craft the items you have a blueprint for.",
      "Collect scrolls of wisdom to increase your... wisdom.",
      "Be wary of the dark places.",
      "How deep can you dig?",
      "Always carry a healthy supply of wood and torches.",
      "Blueprint Finder spin speed indicates distance.",
      "Hint: Cavein's will not destroy your structures.",
      "Items are locked until you find their blueprint.",
      "Rocks are locked until you either prospect them or pick them up.",
      "Scripts can only be used in Dig Deep once the Script block blueprint has been found.",
      "Store your blueprints, scrolls and books in your book cases."
    };
    private static string[] creativeHints = new string[8]
    {
      "It's time to get creative!",
      "In flight mode, press A to ascend and press/hold Left Stick to descend.",
      "Place Marker blocks to mark out extents for creative operations.",
      "Use the One Hit Wonder - Sledge Hammer to destroy any block with one hit.",
      "Use Creative Flooding to fill in irregular spaces.",
      "Once you have copied to the clipboard you can save the clipboard as a component.",
      "Press DPad Down for a shortcut to the Creative Menu.",
      "Mobs do not drop loot if Finite Resources is Off in Creative worlds."
    };
    private static string[] generalHints = new string[36]
    {
      "Save often.",
      "Use left trigger to open shops, chests, workbenches, furnaces, etc.",
      "Sentry Turrets require ammunition AND a weapon to function.",
      "Visit: www.youtube.com/totalminer to see what others are doing in Total Miner.",
      "Cook food in the furnace.",
      "Store your items in crates, chests, bookcases, locked chests and safes.",
      "A red target frame indicates you can't place a block there. But you can still mine there.",
      "Obsidian has magical properties.",
      "If you die, your items will remain where you died for 7 minutes.",
      "If you die, a graveyard marker will appear on your surface map, at the place of death.",
      "Use the Escape option if you are trapped.",
      "Place a torch on explosives to detonate them.",
      "Press A to jump. Double tap A to double jump. Press X to toggle fly mode.",
      "Press Y to prospect, interact with, or setup a block. Many blocks have interaction/setup options.",
      "Warning: Jumping into a one block high gap can be fatal!",
      "Press Left Bumper to examine an item in your inventory.",
      "Press Right Bumper to throw away an item in your inventory.",
      "If you've invested many hours, periodically copy the Total Miner Data file to a USB stick for peace of mind.",
      "You can turn Item Bobbing on or off in Player Options.",
      "You can set Single Hand Wielding or Dual Wielding in Player Options.",
      "The red teleport channel is reserved for admins.",
      "You can assign keys to locked doors and locked chests.",
      "Many blocks, such as stairs, can use up to 16 different textures.",
      "Shops can be economized (player owned).",
      "Examine items to see their stats.",
      "Press A while crouching to stay crouched. Press A again to stand up.",
      "Use crouch to move past mobs stealthily (silently).",
      "When creating scripts, using \"New Script From Change Log\" is a good way to get started.",
      "Paste Overwrite and Paste Merge are essential when using the System Terrain Packs.",
      "If you don't have an HDMI cable, get one.",
      "An enemy might not be able to see you, but can it hear you?. Walking around and hitting things is noisy.",
      "There are 300+ items that can be crafted, smelted, smithed, or cooked.",
      "Admins are unaffected by zones.",
      "Go to Pause->Game->How To->Hotbar to understand exactly how the Hotbar works.",
      "You can now hold multiple clipboards in your inventory.",
      "Click Terrain on the lobby screen to edit parameters that affect how terrain is generated."
    };
    private static string[] newHints = new string[5]
    {
      "Unequip and reequip the decal applicator to change it's decal.",
      "The decal applicator can remove decals too.",
      "Set an NPC to ShowOwner and no one but you can change it's message.",
      "The Particle and ParticleEmitter velocity parameter supports view relative direction.",
      "Use Game -> Options -> Item Options to enable or disable item use in your world."
    };
    private static string[] conversionHints = new string[3]
    {
      "This is a one time conversion for this world.",
      "After conversion the file size may change. This is due to our new data format.",
      "Pre 1.8 map files will not be deleted. You may delete them when you're ready."
    };
    private float fadeTime = 5f;
    private float charScale = 1.2f;
    private const int planeCount = 113;
    private const int backCharCount = 12;
    private const int hintCount = 1;
    private static FloatInterpolator fader;
    private static int fadeType;
    private Texture2D logoTexture;
    private bool fadeLogo;
    private Rectangle logoRect;
    private SpriteBatchSafe logoSpriteBatch;
    private BasicEffect effect;
    private short[] indices;
    private VertexPositionColorTexture[] vertices;
    private RasterizerState rasterState;
    private DepthStencilState depthState;
    private bool drawChar;
    private static int planeIndex;
    private Texture2D backPlaneTex;
    private Texture2D backCharTex;
    private static Vector2 charPos;
    private static Vector2 charDir;
    private PcgRandom random;
    private static int backPlaneTimer;
    private static Vector3 yprStart;
    private static Vector3 yprVel;
    private static Vec3Interpolator ypr;
    private static Vector2[] hintPos;
    private static string[] hintText;
    private static float[] hintLength;
    private static float[] hintSpeed;
    private static float[] hintTimer;
    private static float[] hintTime;
    private float bannerDuration;
    private float bannerTimer;
    private float bannerScaleDir;
    private float bannerScale;
    private int bannerIndex;
    private string bannerText;

    public override void LoadContent()
    {
      base.LoadContent();
      bool flag = BackgroundScreen.fader == null;
      if (flag)
      {
        BackgroundScreen.fader = new FloatInterpolator();
        BackgroundScreen.ypr = new Vec3Interpolator();
      }
      GraphicStatics.GradientTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\gradient");
      this.logoSpriteBatch = GraphicStatics.SpriteBatchPool.GetNextItem();
      this.logoTexture = this.content.Load<Texture2D>("Textures\\totalminerlogo2");
      this.logoRect = MyExtensions.CenterOfViewport(this.logoTexture.Width, this.logoTexture.Height);
      this.logoRect.Y = 90;
      this.Font = CoreGlobals.GameFont;
      float x = 5f;
      float y = 2.8f;
      this.vertices = new VertexPositionColorTexture[4];
      VertexPositionColorTexture positionColorTexture = new VertexPositionColorTexture();
      positionColorTexture.Position = new Vector3(-x, -y, 0.0f);
      positionColorTexture.Color = Color.White * 1f;
      positionColorTexture.TextureCoordinate = new Vector2(0.0f, 1f);
      this.vertices[0] = positionColorTexture;
      positionColorTexture.Position = new Vector3(-x, y, 0.0f);
      positionColorTexture.TextureCoordinate = new Vector2(0.0f, 0.0f);
      this.vertices[1] = positionColorTexture;
      positionColorTexture.Position = new Vector3(x, y, 0.0f);
      positionColorTexture.TextureCoordinate = new Vector2(1f, 0.0f);
      this.vertices[2] = positionColorTexture;
      positionColorTexture.Position = new Vector3(x, -y, 0.0f);
      positionColorTexture.TextureCoordinate = new Vector2(1f, 1f);
      this.vertices[3] = positionColorTexture;
      this.indices = new short[6]
      {
        (short) 0,
        (short) 1,
        (short) 2,
        (short) 0,
        (short) 2,
        (short) 3
      };
      this.effect = new BasicEffect(this.GraphicsDevice);
      this.effect.TextureEnabled = true;
      this.effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), this.GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
      this.effect.AmbientLightColor = Vector3.One;
      this.effect.DiffuseColor = Vector3.One;
      this.effect.Alpha = 1f;
      this.effect.LightingEnabled = false;
      this.effect.CurrentTechnique = this.effect.Techniques[0];
      this.rasterState = new RasterizerState()
      {
        CullMode = CullMode.None
      };
      this.depthState = DepthStencilState.Default;
      this.random = new PcgRandom(new Random().Next());
      if (BackgroundScreen.hintPos == null)
      {
        BackgroundScreen.hintPos = new Vector2[1];
        BackgroundScreen.hintText = new string[1];
        BackgroundScreen.hintLength = new float[1];
        BackgroundScreen.hintSpeed = new float[1];
        BackgroundScreen.hintTimer = new float[1];
        BackgroundScreen.hintTime = new float[1];
        for (int index = 0; index < 1; ++index)
          BackgroundScreen.hintTimer[index] = (float) this.random.Next(10);
      }
      if (flag)
      {
        this.LoadPlane();
      }
      else
      {
        this.backPlaneTex = this.content.Load<Texture2D>("Textures\\backgroundscreen" + (BackgroundScreen.planeIndex + 1).ToString());
        this.backCharTex = this.content.Load<Texture2D>("Textures\\backchar" + (BackgroundScreen.planeIndex % 12 + 1).ToString());
        this.effect.Texture = this.backPlaneTex;
      }
    }

    private void LoadPlane()
    {
      int num = 0;
      for (int index = 0; index < 113; ++index)
      {
        if (BackgroundScreen.shown[index])
          ++num;
      }
      if (num == 113)
      {
        for (int index = 0; index < 113; ++index)
          BackgroundScreen.shown[index] = false;
      }
      int index1 = this.random.Next(113);
      while (BackgroundScreen.shown[index1])
        index1 = this.random.Next(113);
      BackgroundScreen.shown[index1] = true;
      this.LoadPlane(index1);
    }

    private void LoadPlane(int index)
    {
      BackgroundScreen.planeIndex = index;
      if (BackgroundScreen.planeIndex >= 113)
        BackgroundScreen.planeIndex = 0;
      int num1 = BackgroundScreen.planeIndex % 12 + 1;
      this.backPlaneTex = this.content.Load<Texture2D>("Textures\\backgroundscreen" + (BackgroundScreen.planeIndex + 1).ToString());
      this.backCharTex = this.content.Load<Texture2D>("Textures\\backchar" + num1.ToString());
      this.effect.Texture = this.backPlaneTex;
      this.StartFadeIn();
      bool flag = num1 == 1 || num1 == 10 || num1 == 12;
      BackgroundScreen.charPos = new Vector2(flag ? 400f : (float) (1300.0 - (double) this.backCharTex.Width * (double) this.charScale), (float) (this.GraphicsDevice.Viewport.Height + 120) - (float) this.backCharTex.Height * this.charScale);
      BackgroundScreen.charDir = new Vector2(flag ? 0.13f : -0.13f, 0.0f);
      BackgroundScreen.backPlaneTimer = 0;
      this.drawChar = this.random.Next(7) == 0;
      float num2 = 0.24f;
      float num3 = num2 * 0.5f;
      float x = (float) this.random.NextDouble() * num2 - num3;
      float y = (float) this.random.NextDouble() * num2 - num3;
      float num4 = (float) this.random.NextDouble() * num3 + num3;
      float num5 = (float) this.random.NextDouble() * num3 + num3;
      float num6 = (double) x < 0.0 ? Math.Abs(num4) : -Math.Abs(num4);
      float num7 = (double) y < 0.0 ? Math.Abs(num5) : -Math.Abs(num5);
      BackgroundScreen.yprStart = new Vector3(x, y, 0.0f);
      BackgroundScreen.yprVel = new Vector3(num6 * 0.5f, num7 * 0.5f, 0.0f);
      BackgroundScreen.ypr.Start(BackgroundScreen.yprStart, BackgroundScreen.yprStart + BackgroundScreen.yprVel, 10.0, true);
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
      GraphicStatics.SpriteBatchPool.Release(this.logoSpriteBatch);
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      TotalMinerGame.Instance.BackgroundScreenRemoved();
    }

    protected override Texture2D LoadBackgroundTexture()
    {
      return (Texture2D) null;
    }

    public void StartFadeIn()
    {
      BackgroundScreen.fadeType = 1;
      BackgroundScreen.fader.Start(this.userFade, 1f, (double) this.fadeTime);
    }

    public void StartFadeOut(bool fadeLogo)
    {
      BackgroundScreen.fadeType = 2;
      this.fadeLogo = fadeLogo;
      BackgroundScreen.fader.Start(this.userFade, 0.0f, (double) this.fadeTime);
    }

    public override bool HandleInput(InputState input)
    {
      if (!input.CurrentKeyboardStates[0].IsKeyDown(Keys.Space) || !input.LastKeyboardStates[0].IsKeyUp(Keys.Space))
        return base.HandleInput(input);
      this.StartFadeOut(false);
      return true;
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      coveredByOtherScreen = false;
      base.UpdateCore(coveredByOtherScreen);
      BackgroundScreen.charPos += BackgroundScreen.charDir;
      BackgroundScreen.ypr.Update();
      if (!BackgroundScreen.ypr.IsActive)
      {
        if (BackgroundScreen.ypr.CurrentValue == BackgroundScreen.yprStart)
          BackgroundScreen.ypr.Start(BackgroundScreen.ypr.CurrentValue, BackgroundScreen.yprStart + BackgroundScreen.yprVel, 10.0, true);
        else
          BackgroundScreen.ypr.Start(BackgroundScreen.ypr.CurrentValue, BackgroundScreen.yprStart, 10.0, true);
      }
      if (BackgroundScreen.fadeType == 1 || BackgroundScreen.fadeType == 2)
      {
        double num = (double) BackgroundScreen.fader.Update();
        this.userFade = BackgroundScreen.fader.CurrentValue;
        if (!BackgroundScreen.fader.IsActive)
        {
          if (BackgroundScreen.fadeType == 2)
          {
            this.LoadPlane();
          }
          else
          {
            BackgroundScreen.fadeType = 0;
            this.fadeLogo = true;
          }
        }
      }
      ++BackgroundScreen.backPlaneTimer;
      if (BackgroundScreen.backPlaneTimer > 1200 && this.random.Next(200) == 0)
        this.StartFadeOut(false);
      if (Globals1.IsCuePlaying(CoreGlobals.AudioManager.CurrentCue))
        return;
      string[] cues = AmbientMusicWorker.Cues;
      CoreGlobals.AudioManager.PlaySong(cues[this.random.Next(cues.Length)]);
    }

    protected override void DrawOverlay()
    {
      this.logoSpriteBatch.Begin();
      this.logoSpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.Black);
      this.logoSpriteBatch.End();
      this.GraphicsDevice.RasterizerState = this.rasterState;
      this.GraphicsDevice.DepthStencilState = this.depthState;
      this.effect.VertexColorEnabled = true;
      this.effect.World = Matrix.CreateFromYawPitchRoll(BackgroundScreen.ypr.CurrentValue.X, BackgroundScreen.ypr.CurrentValue.Y, BackgroundScreen.ypr.CurrentValue.Z) * Matrix.CreateTranslation(0.0f, 0.25f, 0.0f);
      this.effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 6f), Vector3.Zero, Vector3.Up);
      this.effect.Alpha = this.userFade;
      this.effect.Techniques[0].Passes[0].Apply();
      this.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.vertices, 0, 4, this.indices, 0, 2, VertexPositionColorTexture.VertexDeclaration);
      this.logoSpriteBatch.BeginTM(this.Matrix);
      if (this.drawChar)
        this.logoSpriteBatch.Draw(this.backCharTex, BackgroundScreen.charPos, new Rectangle?(), new Color(this.userFade, this.userFade, this.userFade), 0.0f, Vector2.Zero, this.charScale, SpriteEffects.None, 0.0f);
      this.logoRect.X = 122;
      this.logoRect.Y = this.GraphicsDevice.Viewport.Height - 420;
      Color white = Color.White;
      Color black = Color.Black;
      if (this.fadeLogo)
      {
        white *= this.userFade;
        Color color = black * this.userFade;
      }
      this.logoSpriteBatch.DrawGradient(new Rectangle(this.logoRect.X - 80, this.logoRect.Y - 10, this.logoRect.Width + 160, this.logoRect.Height + 20), 80, 80, Color.Black * 0.5f, Matrix.Identity);
      this.logoSpriteBatch.Draw(this.logoTexture, this.logoRect, white);
      int num1 = this.GraphicsDevice.Viewport.Height - 78;
      this.logoSpriteBatch.DrawString(this.Font, "Visit us at www.TotalMinerForums.net", new Vector2(146f, (float) (num1 + 1)), Color.Black, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      this.logoSpriteBatch.DrawString(this.Font, "Visit us at www.TotalMinerForums.net", new Vector2(145f, (float) num1), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      int num2 = 27302;
      string text = string.Format("V{0}.{1}.{2:D3}  R{3}", (object) (num2 / 10000), (object) (num2 % 10000 / 1000), (object) (num2 % 1000), (object) "15.03.18");
      Vector2 vector2 = this.Font.MeasureString(text) * 0.5f;
      int num3 = this.GraphicsDevice.Viewport.Width - 480;
      this.logoSpriteBatch.DrawString(this.Font, text, new Vector2((float) num3, (float) (num1 + 1)), Color.Black, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      this.logoSpriteBatch.DrawString(this.Font, text, new Vector2((float) (num3 - 1), (float) num1), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
      this.DrawHints();
      this.DrawBanner();
      this.logoSpriteBatch.End();
    }

    protected override void DrawCore2(float fade)
    {
    }

    protected override Vector2 DrawPosition
    {
      get
      {
        return new Vector2(0.0f, 9f);
      }
    }

    protected override bool DrawByRectangle
    {
      get
      {
        return false;
      }
    }

    private void DrawBanner()
    {
      if (Globals2.BannerList == null || Globals2.BannerList.Count <= 0)
        return;
      float num1 = 0.75f;
      float num2 = 0.05f;
      float num3 = 0.5f;
      this.bannerTimer += Services.ElapsedTime;
      if ((double) this.bannerTimer > (double) this.bannerDuration)
      {
        GameScreen topActiveScreen1 = this.ScreenManager.GetTopActiveScreen(this.ControllingPlayer);
        GameScreen topActiveScreen2 = this.ScreenManager.GetTopActiveScreen(new PlayerIndex?());
        if (topActiveScreen1 is LoadingScreenBase || topActiveScreen1 is LobbyScreen || (topActiveScreen2 is LoadingScreenBase || topActiveScreen2 is LobbyScreen))
        {
          this.bannerText = (string) null;
          this.bannerDuration = 20f;
        }
        else
        {
          ++this.bannerIndex;
          if (this.bannerIndex % 2 == 1)
          {
            this.bannerText = Utils.InsertNewLines(this.Font, 360, (float) ((double) num1 + (double) num2), Globals2.BannerList[this.bannerIndex / 2 % Globals2.BannerList.Count], true, new char[1]
            {
              ' '
            });
            this.bannerDuration = 10f;
          }
          else
          {
            this.bannerText = (string) null;
            this.bannerDuration = 4f;
          }
        }
        this.bannerScale = num1 - num2;
        this.bannerScaleDir = (float) ((double) num2 * 2.0 * (double) num3 / 60.0);
        this.bannerTimer = 0.0f;
      }
      if (this.bannerIndex <= 0 || this.bannerText == null)
        return;
      Vector2 vector2 = this.Font.MeasureString(this.bannerText) * this.bannerScale;
      this.logoSpriteBatch.DrawString(this.Font, this.bannerText, new Vector2(220f, 180f), Color.Yellow * this.TransitionAlphaFloat, 0.0f, new Vector2(vector2.X * 0.5f, vector2.Y * 0.5f), this.bannerScale, SpriteEffects.None, 0.0f);
      this.bannerScale += this.bannerScaleDir;
      if ((double) this.bannerScale > (double) num1 + (double) num2)
      {
        this.bannerScaleDir = (float) (-(double) num2 * 2.0 * (double) num3 / 60.0);
      }
      else
      {
        if ((double) this.bannerScale >= (double) num1 - (double) num2)
          return;
        this.bannerScaleDir = (float) ((double) num2 * 2.0 * (double) num3 / 60.0);
      }
    }

    private void DrawHints()
    {
      int num = 36;
      for (int index = 0; index < 1; ++index)
      {
        BackgroundScreen.hintTimer[index] -= Services.ElapsedTime;
        if ((double) BackgroundScreen.hintTimer[index] < 0.0)
        {
          BackgroundScreen.hintText[index] = this.GetNewHint();
          BackgroundScreen.hintLength[index] = this.Font.MeasureString(BackgroundScreen.hintText[index]).X * 0.7f;
          BackgroundScreen.hintSpeed[index] = 1.8f;
          BackgroundScreen.hintTime[index] = BackgroundScreen.hintTimer[index] = Math.Max(10f, BackgroundScreen.hintLength[index] / 70f);
          BackgroundScreen.hintPos[index] = new Vector2((float) (this.GraphicsDevice.Viewport.Width - 100 - this.random.Next(20)), (float) (50 + index * num));
        }
        if (BackgroundScreen.hintText[index] != null)
        {
          BackgroundScreen.hintPos[index].X -= BackgroundScreen.hintSpeed[index];
          float scale = 0.7f;
          if ((double) BackgroundScreen.hintTimer[index] > (double) BackgroundScreen.hintTime[index] - 4.0)
            scale = MathHelper.Lerp(0.7f, 0.0f, (float) (((double) BackgroundScreen.hintTimer[index] - ((double) BackgroundScreen.hintTime[index] - 4.0)) / 4.0));
          else if ((double) BackgroundScreen.hintTimer[index] < 4.0)
            scale = MathHelper.Lerp(0.0f, 0.7f, BackgroundScreen.hintTimer[index] / 4f);
          this.logoSpriteBatch.DrawString(this.Font, BackgroundScreen.hintText[index], BackgroundScreen.hintPos[index] + Vector2.One + TMFont.yVec, Color.Black * this.TransitionAlphaFloat * scale, 0.0f, new Vector2(BackgroundScreen.hintLength[index] * 0.5f, 1f), scale, SpriteEffects.None, 0.0f);
          this.logoSpriteBatch.DrawString(this.Font, BackgroundScreen.hintText[index], BackgroundScreen.hintPos[index] + TMFont.yVec, Color.White * this.TransitionAlphaFloat * scale, 0.0f, new Vector2(BackgroundScreen.hintLength[index] * 0.5f, 1f), scale, SpriteEffects.None, 0.0f);
        }
      }
    }

    private string GetNewHint()
    {
      if (Globals2.GameProperties != null && Globals2.GameProperties.SaveGame != null && Globals2.GameProperties.SaveGame.Header != null && Globals2.GameProperties.SaveGame.Header.GameMode == GameMode.DigDeep)
      {
        int index = this.random.Next(BackgroundScreen.digDeepHints.Length + BackgroundScreen.generalHints.Length + BackgroundScreen.newHints.Length);
        if (index >= BackgroundScreen.digDeepHints.Length + BackgroundScreen.generalHints.Length)
          return BackgroundScreen.newHints[index - BackgroundScreen.digDeepHints.Length - BackgroundScreen.generalHints.Length];
        if (index < BackgroundScreen.digDeepHints.Length)
          return BackgroundScreen.digDeepHints[index];
        return BackgroundScreen.generalHints[index - BackgroundScreen.digDeepHints.Length];
      }
      int index1 = this.random.Next(BackgroundScreen.creativeHints.Length + BackgroundScreen.generalHints.Length + BackgroundScreen.newHints.Length);
      if (index1 >= BackgroundScreen.creativeHints.Length + BackgroundScreen.generalHints.Length)
        return BackgroundScreen.newHints[index1 - BackgroundScreen.creativeHints.Length - BackgroundScreen.generalHints.Length];
      if (index1 < BackgroundScreen.creativeHints.Length)
        return BackgroundScreen.creativeHints[index1];
      return BackgroundScreen.generalHints[index1 - BackgroundScreen.creativeHints.Length];
    }
  }
}
