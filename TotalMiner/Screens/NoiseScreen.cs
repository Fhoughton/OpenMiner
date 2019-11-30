// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NoiseScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class NoiseScreen : GameScreen
  {
    private Color[] ncolor = new Color[4]
    {
      Color.White,
      Color.Red,
      Color.Yellow,
      Color.Green
    };
    protected Rectangle screenRect;
    protected SpriteBatchSafe spriteBatch;
    private Map map;
    private PcgRandom random;
    private int[] perm1;
    private int[] perm2;
    private int[] perm3;
    private int[] perm4;
    private int[] perm5;
    private bool genWhenNoPress;
    private int noiseCount;
    private float[] noise;
    private byte[][] b;
    private Color[][] c;
    private Texture2D[] tex;

    public NoiseScreen(Map map)
    {
      this.map = map;
      this.random = new PcgRandom(new Random().Next());
      this.noiseCount = 1;
      this.perm1 = SimplexNoise1.GetSimplexNoisePermTable(this.random.Next());
      this.perm2 = SimplexNoise1.GetSimplexNoisePermTable(this.random.Next());
      this.perm3 = SimplexNoise1.GetSimplexNoisePermTable(this.random.Next());
      this.perm4 = SimplexNoise1.GetSimplexNoisePermTable(this.random.Next());
      this.perm5 = SimplexNoise1.GetSimplexNoisePermTable(this.random.Next());
    }

    public override void LoadContent()
    {
      this.spriteBatch = this.ScreenManager.SpriteBatch;
      this.Font = this.ScreenManager.GameFont;
      this.screenRect = this.GraphicsDevice.Viewport.Rectangle();
      this.noise = new float[this.noiseCount];
      this.b = new byte[this.noiseCount][];
      this.c = new Color[this.noiseCount][];
      this.tex = new Texture2D[this.noiseCount];
      for (int index = 0; index < this.noiseCount; ++index)
      {
        this.b[index] = new byte[1052676];
        this.c[index] = new Color[1048576];
        this.tex[index] = new Texture2D(this.ScreenManager.GraphicsDevice, 1024, 1024);
        this.noise[index] = (float) this.random.Next(100, 500);
      }
      this.Generate();
    }

    private void Generate()
    {
      this.Generate1((int) this.noise[0]);
    }

    private void Generate1(int scale)
    {
      float num1 = float.MaxValue;
      float num2 = float.MinValue;
      int num3 = 1024;
      int num4 = num3 + 2;
      int num5 = 1024;
      int index1 = 0;
      int num6 = 1000000;
      for (int index2 = -1; index2 < num5 + 1; ++index2)
      {
        for (int index3 = -1; index3 < num3 + 1; ++index3)
        {
          float num7 = (float) scale;
          float num8 = (float) (((double) SimplexNoise1.noise((float) index3 / num7, (float) index2 / num7, this.perm1) + (double) SimplexNoise1.noise((float) (index2 + num6) / num7, (float) (index3 + num6) / num7, this.perm1)) / 2.0);
          if ((double) num8 < (double) num1)
            num1 = num8;
          if ((double) num8 > (double) num2)
            num2 = num8;
          int index4 = index3 + 1 + (index2 + 1) * num4;
          this.b[index1][index4] = (byte) ((double) num8 * (double) byte.MaxValue);
        }
      }
      for (int z = 0; z < num5; ++z)
      {
        for (int x = 0; x < num3; ++x)
        {
          int index2 = x + 1 + (z + 1) * num4;
          int index3 = x + z * num3;
          int num7 = (int) this.b[index1][index2];
          float noise2 = this.GetNoise2(x, z, scale);
          this.c[index1][index3] = num7 >= 105 ? (num7 >= 110 ? (num7 >= 165 ? (num7 >= 185 ? (num7 >= 200 ? new Color(num7, num7, num7, (int) byte.MaxValue) : new Color(50, 50, 50, (int) byte.MaxValue)) : this.GetLowLandsColor(noise2)) : this.GetGrassColor(noise2)) : this.GetBeachColor(noise2)) : this.GetWaterColor(noise2);
          this.c[index1][index3] = (double) noise2 >= 0.600000023841858 ? Color.White : Color.Black;
        }
      }
      this.tex[index1].SetData<Color>(this.c[index1]);
    }

    private Color GetWaterColor(float temperature)
    {
      return Color.Blue;
    }

    private Color GetBeachColor(float temperature)
    {
      if ((double) temperature != 1.0)
        return Color.Yellow;
      return Color.LightGreen;
    }

    private Color GetGrassColor(float temperature)
    {
      if ((double) temperature == 0.0)
        return Color.Yellow;
      if ((double) temperature != 1.0)
        return Color.Green;
      return Color.DarkGreen;
    }

    private Color GetLowLandsColor(float temperature)
    {
      return Color.Chocolate;
    }

    private void Generate2(int scale)
    {
      float num1 = float.MaxValue;
      float num2 = float.MinValue;
      int num3 = 1024;
      int num4 = num3 + 2;
      int num5 = 1024;
      int index1 = 0;
      for (int z = -1; z < num5 + 1; ++z)
      {
        for (int x = -1; x < num3 + 1; ++x)
        {
          float noise2 = this.GetNoise2(x, z, scale);
          if ((double) noise2 < (double) num1)
            num1 = noise2;
          if ((double) noise2 > (double) num2)
            num2 = noise2;
          int index2 = x + 1 + (z + 1) * num4;
          this.b[index1][index2] = (byte) ((double) noise2 * (double) byte.MaxValue);
        }
      }
      for (int index2 = 0; index2 < num5; ++index2)
      {
        for (int index3 = 0; index3 < num3; ++index3)
        {
          int index4 = index3 + 1 + (index2 + 1) * num4;
          int index5 = index3 + index2 * num3;
          int num6 = (int) this.b[index1][index4];
          this.c[index1][index5] = new Color(num6, num6, num6, (int) byte.MaxValue);
        }
      }
      this.tex[index1].SetData<Color>(this.c[index1]);
    }

    private float GetNoise2(int x, int z, int scale)
    {
      float num = (float) (scale * 2);
      return SimplexNoise1.noise((float) x / num, (float) z / num, this.perm2);
    }

    public override bool HandleInput(InputState input)
    {
      PlayerIndex playerIndex;
      if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
        this.ExitScreen();
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      int index = 0;
      if (currentGamePadState.Buttons.A == ButtonState.Pressed)
        index = 1;
      else if (currentGamePadState.Buttons.X == ButtonState.Pressed)
        index = 2;
      else if (currentGamePadState.Buttons.Y == ButtonState.Pressed)
        index = 3;
      float num = this.noise[index];
      if (currentGamePadState.DPad.Left == ButtonState.Pressed)
        this.noise[index] -= 0.1f;
      else if (currentGamePadState.DPad.Right == ButtonState.Pressed)
        this.noise[index] += 0.1f;
      else if (currentGamePadState.DPad.Up == ButtonState.Pressed)
        --this.noise[index];
      else if (currentGamePadState.DPad.Down == ButtonState.Pressed)
        ++this.noise[index];
      if ((double) this.noise[index] != (double) num)
      {
        this.genWhenNoPress = true;
        return true;
      }
      if (!this.genWhenNoPress)
        return base.HandleInput(input);
      this.Generate();
      this.genWhenNoPress = false;
      return true;
    }

    protected override void DrawCore()
    {
      this.spriteBatch.Begin();
      this.spriteBatch.DrawFilledBox(this.screenRect, 2, new Color(0.1f, 0.1f, 0.1f), Color.Black);
      this.spriteBatch.End();
      this.spriteBatch.Begin();
      for (int index = 0; index < this.noiseCount; ++index)
        this.spriteBatch.Draw(this.tex[index], new Rectangle(this.screenRect.X + 2, this.screenRect.Y + 2, this.screenRect.Width - 4, this.screenRect.Height - 4), this.ncolor[index]);
      this.spriteBatch.End();
      this.spriteBatch.Begin();
      for (int index = 0; index < this.noiseCount; ++index)
        this.spriteBatch.DrawString(CoreGlobals.GameFont, this.noise[index].ToString(), new Vector2((float) (this.screenRect.X + 8), (float) (this.screenRect.Y + 8 + index * 20)), Color.White);
      this.spriteBatch.End();
    }
  }
}
