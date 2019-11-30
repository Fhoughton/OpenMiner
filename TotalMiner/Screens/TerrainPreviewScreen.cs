// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TerrainPreviewScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Game;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Threading;

namespace StudioForge.TotalMiner.Screens
{
  internal class TerrainPreviewScreen : GameScreen
  {
    private int zMod = 16;
    private bool hasDrawCalled;
    private Texture2D texture;
    private GameProperties gameProperties;
    private Thread buildThread;
    private HeightField field;
    private Color[] colorData;
    private Rectangle texRect;
    private Color[] heightColors;
    private BiomeType biome;
    private TerrainGeneratorBase generator;
    private Action onRefresh;

    public TerrainPreviewScreen(GameProperties gameProperties, Action onRefresh)
    {
      this.gameProperties = gameProperties;
      this.onRefresh = onRefresh;
      this.biome = gameProperties.BiomeType;
    }

    public override void LoadContent()
    {
      this.Font = this.ScreenManager.GameFont;
      base.LoadContent();
      this.borderColor = Color.White;
    }

    public override bool HandleInput(InputState input)
    {
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, GuiInput.ExitScreen))
      {
        this.ExitScreen();
        return true;
      }
      if (InputManager1.IsInputReleasedNew(this.ControllingPlayer, PlayerInput.EventScriptX))
      {
        if (this.gameProperties.IsNewMap && this.texture != null && !this.buildThread.IsAlive)
        {
          this.texture.Dispose();
          this.texture = (Texture2D) null;
          this.gameProperties.SaveGame.Header.MapSeed = new PcgRandom(this.gameProperties.SaveGame.Header.MapSeed).Next();
          this.onRefresh();
        }
        return true;
      }
      if (this.gameProperties.IsNewMap && this.texture != null && !this.buildThread.IsAlive)
      {
        float num1 = 0.0f;
        float num2 = 0.0f;
        GamePadState gamepadState = InputManager.GetGamepadState(this.ControllingPlayer.Value);
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorLeft))
          num1 = 50f;
        else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorRight))
          num1 = -50f;
        if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorUp))
          num2 = 50f;
        else if (InputManager1.IsInputPressedNew(this.ControllingPlayer, GuiInput.CursorDown))
          num2 = -50f;
        if ((double) gamepadState.ThumbSticks.Left.X != 0.0)
          num1 = gamepadState.ThumbSticks.Left.X * -50f;
        if ((double) gamepadState.ThumbSticks.Left.Y != 0.0)
          num2 = gamepadState.ThumbSticks.Left.Y * 50f;
        if (input.IsButtonDown(Buttons.LeftTrigger, this.ControllingPlayer.Value))
        {
          num1 *= 0.1f;
          num2 *= 0.1f;
        }
        else if (input.IsButtonDown(Buttons.RightTrigger, this.ControllingPlayer.Value))
        {
          num1 *= 10f;
          num2 *= 10f;
        }
        bool flag = (double) num1 != 0.0 || (double) num2 != 0.0;
        float num3 = (double) gamepadState.ThumbSticks.Right.Y != 0.0 ? gamepadState.ThumbSticks.Right.Y : (float) InputManager.GetMouseWheelDelta(this.ControllingPlayer.Value) / 100f;
        if ((double) num3 != 0.0)
        {
          float num4 = (double) num3 < 0.0 ? 0.9f : 1.111111f;
          BiomeParams biomeParams = this.gameProperties.SaveGame.Header.BiomeParams;
          biomeParams.BigDetailNoise *= num4;
          biomeParams.MediumDetailNoise *= num4;
          biomeParams.FineDetailNoise *= num4;
          biomeParams.OffsetX = (int) ((double) biomeParams.OffsetX * (double) num4);
          biomeParams.OffsetZ = (int) ((double) biomeParams.OffsetZ * (double) num4);
          flag = true;
        }
        if (flag)
        {
          this.texture.Dispose();
          this.texture = (Texture2D) null;
          this.gameProperties.SaveGame.Header.BiomeParams.OffsetX += (int) num1;
          this.gameProperties.SaveGame.Header.BiomeParams.OffsetZ += (int) num2;
          this.onRefresh();
          return true;
        }
      }
      return base.HandleInput(input);
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (!this.hasDrawCalled || this.texture != null)
        return;
      this.texture = new Texture2D(CoreGlobals.GraphicsDevice, 512, 512, false, SurfaceFormat.Color);
      this.buildThread = new Thread(new ThreadStart(this.BuildTextureThreaded));
      this.buildThread.CurrentCulture = Globals1.CultureInfo;
      this.buildThread.CurrentUICulture = Globals1.CultureInfo;
      this.buildThread.Start();
    }

    private void BuildTextureThreaded()
    {
      int width = this.texture.Width;
      int height = this.texture.Height;
      this.field = new HeightField(width, height, this.biome == BiomeType.DigDeep ? 3071f : 511f);
      this.colorData = new Color[width * this.zMod];
      this.texRect = new Rectangle(0, height - this.zMod, width, this.zMod);
      this.generator = this.GetBiomeGenerator(this.gameProperties.SaveGame.Header.TerrainData.Biome);
      if (this.generator == null)
        return;
      SaveMapHead header = this.gameProperties.SaveGame.Header;
      this.generator.GenerateToHeightMap(this.field, header.BiomeParams, header.TerrainData.SeaLevel, this.gameProperties.SaveGame.Header.MapSeed, this.gameProperties.SaveGame.Header.MapWidth / width, this.gameProperties.SaveGame.Header.MapWidth / height, new Action<int>(this.RowGenerated));
    }

    private TerrainGeneratorBase GetBiomeGenerator(BiomeType biomeType)
    {
      switch (biomeType)
      {
        case BiomeType.Desert:
          return (TerrainGeneratorBase) new DesertBiome();
        case BiomeType.Grasslands:
          return (TerrainGeneratorBase) new GrasslandsBiome();
        case BiomeType.SemiAlphine:
          return (TerrainGeneratorBase) new SemiAlpineBiome();
        case BiomeType.DigDeep:
          return (TerrainGeneratorBase) new DigDeepBiome2();
        default:
          return (TerrainGeneratorBase) null;
      }
    }

    private void RowGenerated(int row)
    {
      if ((row + 1) % this.zMod != 0)
        return;
      if (this.heightColors == null)
        this.heightColors = this.generator.GetColorTable(this.gameProperties.SaveGame.Header.MapHeight);
      int width = this.texture.Width;
      int height1 = this.texture.Height;
      int num1 = 0;
      int num2 = this.zMod - 1;
      while (num1 < this.zMod)
      {
        int num3 = (this.zMod - num2 - 1) * width;
        int x = 0;
        int num4 = width - 1;
        while (x < width)
        {
          float height2 = this.field.GetHeight(x, row - num1);
          this.colorData[num4 + num3] = this.heightColors[(int) height2];
          ++x;
          --num4;
        }
        ++num1;
        --num2;
      }
      do
        ;
      while (!BaseGame.IsUpdating);
      this.texture.SetData<Color>(0, new Rectangle?(this.texRect), this.colorData, 0, width * this.zMod);
      this.texRect.Y -= this.zMod;
    }

    protected override void DrawCore()
    {
      this.hasDrawCalled = true;
      Rectangle boxRect = new Rectangle(384, 104, 512, 542);
      this.SpriteBatch.DrawBlockBox(GraphicStatics.WindowBorderTiles, boxRect, this.TransitionAlphaFloat * this.clientBackAlpha, true, this.borderWidth, this.borderColor, this.clientBackColor, this.Matrix);
      this.SpriteBatch.End();
      this.SpriteBatch.Begin();
      if (this.texture != null)
      {
        Rectangle destinationRectangle1 = new Rectangle(384, 104, 512, 512);
        this.SpriteBatch.Draw(this.texture, destinationRectangle1, Color.White);
        Rectangle destinationRectangle2 = new Rectangle(destinationRectangle1.X, destinationRectangle1.Y + destinationRectangle1.Height, 512, 1);
        this.SpriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle2, Color.White);
        this.SpriteBatch.DrawString(CoreGlobals.GameFont, "Seed: " + this.gameProperties.SaveGame.Header.MapSeed.ToString(), new Vector2((float) (destinationRectangle2.X + 6), (float) (destinationRectangle2.Y + 5)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
        if (this.gameProperties.IsNewMap)
        {
          GraphicStatics.DrawInputIcon(this.SpriteBatch, PlayerInput.EventScriptX, new Rectangle(destinationRectangle2.X + 360, destinationRectangle2.Y + 5, 20, 20));
          this.SpriteBatch.DrawString(CoreGlobals.GameFont, "New Seed", new Vector2((float) (destinationRectangle2.X + 388), (float) (destinationRectangle2.Y + 5)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
        }
      }
      this.SpriteBatch.End();
    }
  }
}
