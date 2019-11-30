// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SunMoon
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner
{
  internal class SunMoon : GameObjectBase
  {
    public const int VertexCount = 16;
    public const float DayNightChangeOverRange = 0.3926991f;
    public const float DayNightChangeOverStart = 1.374447f;
    public const float DayNightChangeOverEnd = 1.767146f;
    public const float MorningSunRotation = -1.274447f;
    public float Rotation;
    public float RotationSpeed;
    public float GlobalLight;
    public Vector2 SunEffectUV;
    public Vector2 MoonEffectUV;
    public Vector3 SunPosition;
    public Vector3 MoonPosition;
    public SunMoon.SunEffect CurrentEffect;
    public VertexBuffer VertexBuffer;
    private float dayNightSpeed;
    private float distance;
    private PcgRandom rand;
    private Vector2 sunEffectUVStart;
    private Vector2 sunEffectUVEnd;
    private Vector2 moonEffectUVStart;
    private Vector2 moonEffectUVEnd;
    private bool nightSoundPlayed;
    private int seasonChangeFrequencyInDays;

    public event EventHandler SunriseStarted;

    public event EventHandler SunriseEnded;

    public event EventHandler SunsetStarted;

    public event EventHandler SunsetEnded;

    public event EventHandler SeasonChange;

    private void Raise_SeasonChange()
    {
      if (this.SeasonChange == null)
        return;
      this.SeasonChange((object) this, EventArgs.Empty);
    }

    private void Raise_SunriseStarted()
    {
      if (this.SunriseStarted == null)
        return;
      this.SunriseStarted((object) this, EventArgs.Empty);
    }

    private void Raise_SunriseEnded()
    {
      if (this.SunriseEnded == null)
        return;
      this.SunriseEnded((object) this, EventArgs.Empty);
    }

    private void Raise_SunsetStarted()
    {
      if (this.SunsetStarted == null)
        return;
      this.SunsetStarted((object) this, EventArgs.Empty);
    }

    private void Raise_SunsetEnded()
    {
      if (this.SunsetEnded == null)
        return;
      this.SunsetEnded((object) this, EventArgs.Empty);
    }

    public bool IsDayTime
    {
      get
      {
        if ((double) this.Rotation > -1.37444686889648)
          return (double) this.Rotation < 1.37444686889648;
        return false;
      }
    }

    public bool IsNightTime
    {
      get
      {
        if ((double) this.Rotation >= -1.37444686889648)
          return (double) this.Rotation > 1.76714587211609;
        return true;
      }
    }

    public float CurrentHour
    {
      get
      {
        return 24f - (float) ((3.14159274101257 - (double) this.Rotation) / 6.28318548202515 * 24.0);
      }
    }

    public int DaysIntoSeason
    {
      get
      {
        return this.DaysIntoGame % this.seasonChangeFrequencyInDays;
      }
    }

    private EnvManager EnvManager
    {
      get
      {
        return GameInstance.Instance.MapStrategyTM.EnvManager;
      }
    }

    public int DaysIntoGame
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.DaysIntoGame;
      }
      set
      {
        Globals2.GameProperties.SaveGame.Header.DaysIntoGame = value;
      }
    }

    public SeasonType Season
    {
      get
      {
        return (SeasonType) (this.DaysIntoGame / this.seasonChangeFrequencyInDays % 4);
      }
    }

    public SunMoon(float dayNightSpeed, int seasonChangeFrequencyInDays)
    {
      this.dayNightSpeed = dayNightSpeed;
      this.seasonChangeFrequencyInDays = seasonChangeFrequencyInDays;
      this.rand = new PcgRandom(new Random().Next());
    }

    public void LoadGeometry(Map map, float maxFarClip)
    {
      this.LoadGeometry(maxFarClip + 100f);
    }

    private void LoadGeometry(float distance)
    {
      Vector2 textureCoordinate1 = MapChunkContent.TexCoords3[289];
      Vector2 vector2_1 = MapChunkContent.TexCoords2[289];
      Vector2 textureCoordinate2 = MapChunkContent.TexCoords3[290];
      Vector2 vector2_2 = MapChunkContent.TexCoords2[290];
      Vector2 textureCoordinate3 = MapChunkContent.TexCoords3[291];
      Vector2 vector2_3 = MapChunkContent.TexCoords2[291];
      Vector2 textureCoordinate4 = MapChunkContent.TexCoords3[292];
      Vector2 vector2_4 = MapChunkContent.TexCoords2[292];
      this.distance = distance;
      float num1 = 36f;
      float num2 = 25f;
      float num3 = num1 * 3f;
      float num4 = num2 * 3f;
      VertexPositionTexture[] data = new VertexPositionTexture[16]
      {
        new VertexPositionTexture(new Vector3(-num3, distance, num3), textureCoordinate3),
        new VertexPositionTexture(new Vector3(num3, distance, num3), new Vector2(textureCoordinate3.X, vector2_3.Y)),
        new VertexPositionTexture(new Vector3(num3, distance, -num3), new Vector2(vector2_3.X, vector2_3.Y)),
        new VertexPositionTexture(new Vector3(-num3, distance, -num3), new Vector2(vector2_3.X, textureCoordinate3.Y)),
        new VertexPositionTexture(new Vector3(-num4, -distance, -num4), textureCoordinate4),
        new VertexPositionTexture(new Vector3(num4, -distance, -num4), new Vector2(textureCoordinate4.X, vector2_4.Y)),
        new VertexPositionTexture(new Vector3(num4, -distance, num4), new Vector2(vector2_4.X, vector2_4.Y)),
        new VertexPositionTexture(new Vector3(-num4, -distance, num4), new Vector2(vector2_4.X, textureCoordinate4.Y)),
        new VertexPositionTexture(new Vector3(-num1, distance, num1), textureCoordinate1),
        new VertexPositionTexture(new Vector3(num1, distance, num1), new Vector2(textureCoordinate1.X, vector2_1.Y)),
        new VertexPositionTexture(new Vector3(num1, distance, -num1), new Vector2(vector2_1.X, vector2_1.Y)),
        new VertexPositionTexture(new Vector3(-num1, distance, -num1), new Vector2(vector2_1.X, textureCoordinate1.Y)),
        new VertexPositionTexture(new Vector3(-num2, -distance, -num2), textureCoordinate2),
        new VertexPositionTexture(new Vector3(num2, -distance, -num2), new Vector2(textureCoordinate2.X, vector2_2.Y)),
        new VertexPositionTexture(new Vector3(num2, -distance, num2), new Vector2(vector2_2.X, vector2_2.Y)),
        new VertexPositionTexture(new Vector3(-num2, -distance, num2), new Vector2(vector2_2.X, textureCoordinate2.Y))
      };
      if (this.VertexBuffer == null)
        this.VertexBuffer = new VertexBuffer(CoreGlobals.GraphicsDevice, VertexPositionTexture.VertexDeclaration, 16, BufferUsage.WriteOnly);
      this.VertexBuffer.SetData<VertexPositionTexture>(data);
    }

    public void StartPlay()
    {
      if ((double) this.Rotation <= -1.37444686889648)
      {
        if ((double) this.Rotation > -1.76714587211609)
          this.Raise_SunriseStarted();
        else
          this.Raise_SunsetEnded();
      }
      else if ((double) this.Rotation >= 1.37444686889648)
      {
        if ((double) this.Rotation < 1.76714587211609)
          this.Raise_SunsetStarted();
        else
          this.Raise_SunsetEnded();
      }
      else
        this.Raise_SunriseEnded();
    }

    protected override void UpdateCore(UpdateState state)
    {
      this.RotationSpeed = 0.0f;
      if (Globals2.GameProperties.SaveGame.Header.DayNightActive)
      {
        this.RotationSpeed = this.GetRotaionSpeedModifier();
        this.Rotation = MyMathHelper.WrapAngle(this.Rotation + Services.ElapsedTime * this.dayNightSpeed * this.RotationSpeed);
      }
      this.SunPosition = Vector3.Transform(new Vector3(0.0f, this.distance, 0.0f), Matrix.CreateRotationZ(-this.Rotation));
      this.MoonPosition = -this.SunPosition;
      this.CalcGlobalLight();
    }

    private void CalcGlobalLight()
    {
      this.GlobalLight = 1f;
      if ((double) this.Rotation <= -1.37444686889648)
      {
        this.GlobalLight = 0.0f;
        if ((double) this.Rotation <= -1.76714587211609)
          return;
        this.GlobalLight = (float) (1.0 - (-(double) this.Rotation - 1.37444686889648) / 0.392699092626572);
        if ((double) this.GlobalLight > 1.0)
          this.GlobalLight = 1f;
        if (this.CurrentEffect == SunMoon.SunEffect.None)
        {
          this.CalcNewSunriseEffect();
          this.CurrentEffect = SunMoon.SunEffect.Sunrise;
          this.Raise_SunriseStarted();
        }
        this.SunEffectUV = Vector2.Lerp(this.sunEffectUVStart, this.sunEffectUVEnd, this.GlobalLight);
        this.MoonEffectUV = Vector2.Lerp(this.moonEffectUVStart, this.moonEffectUVEnd, this.GlobalLight);
      }
      else if ((double) this.Rotation >= 1.37444686889648)
      {
        this.GlobalLight = 0.0f;
        if ((double) this.Rotation < 1.76714587211609)
        {
          this.GlobalLight = (float) (1.0 - ((double) this.Rotation - 1.37444686889648) / 0.392699092626572);
          if ((double) this.GlobalLight > 1.0)
            this.GlobalLight = 1f;
          if (this.CurrentEffect == SunMoon.SunEffect.None)
          {
            this.CalcNewSunsetEffect();
            this.CurrentEffect = SunMoon.SunEffect.Sunset;
            this.Raise_SunsetStarted();
          }
          else if (this.CurrentEffect == SunMoon.SunEffect.Sunset && !this.nightSoundPlayed && (double) this.GlobalLight < 0.800000011920929)
          {
            this.nightSoundPlayed = true;
            Sounds.PlaySound(ItemSoundGroup.EnvNightfall);
          }
          this.SunEffectUV = Vector2.Lerp(this.sunEffectUVStart, this.sunEffectUVEnd, 1f - this.GlobalLight);
          this.MoonEffectUV = Vector2.Lerp(this.moonEffectUVStart, this.moonEffectUVEnd, 1f - this.GlobalLight);
        }
        else
        {
          if (this.CurrentEffect != SunMoon.SunEffect.Sunset)
            return;
          this.CurrentEffect = SunMoon.SunEffect.None;
          this.Raise_SunsetEnded();
          SeasonType season = this.Season;
          ++this.DaysIntoGame;
          if (season == this.Season)
            return;
          this.Raise_SeasonChange();
        }
      }
      else if (this.CurrentEffect == SunMoon.SunEffect.Sunrise)
      {
        this.CurrentEffect = SunMoon.SunEffect.None;
        this.Raise_SunriseEnded();
      }
      else
      {
        this.CurrentEffect = SunMoon.SunEffect.None;
        this.nightSoundPlayed = false;
      }
    }

    private void CalcNewSunriseEffect()
    {
      Vector2 range1 = MapChunkContent.TexCoords4[293] - MapChunkContent.TexCoords1[293];
      range1.X *= 0.5f;
      this.sunEffectUVStart = MapChunkContent.TexCoords1[293] + this.GetRandomUV(range1);
      this.sunEffectUVEnd = MapChunkContent.TexCoords1[293] + this.GetRandomUV(range1);
      Vector2 range2 = MapChunkContent.TexCoords4[294] - MapChunkContent.TexCoords1[294];
      range2.X *= 0.5f;
      this.moonEffectUVStart = MapChunkContent.TexCoords1[294] + this.GetRandomUV(range2);
      this.moonEffectUVEnd = MapChunkContent.TexCoords1[294] + this.GetRandomUV(range2);
      MapTM map = GameInstance.Instance.Map;
      if (!map.IsHost || map.Random.Next(3) != 0 || this.EnvManager.FogCount != 0)
        return;
      GlobalPoint3D globalPoint3D1 = (map.MapBound.Max - map.MapBound.Min) / 2;
      GlobalPoint3D globalPoint3D2 = map.MapBound.Min + globalPoint3D1;
      Color.Lerp(new Color((int) byte.MaxValue, 249, 225, (int) byte.MaxValue), Color.White, (float) map.Random.NextDouble());
      map.Random.NextDouble();
      map.Random.Next(70, 170);
    }

    private void CalcNewSunsetEffect()
    {
      Vector2 range1 = MapChunkContent.TexCoords4[293] - MapChunkContent.TexCoords1[293];
      range1.X *= 0.5f;
      this.sunEffectUVStart = MapChunkContent.TexCoords1[293] + new Vector2(range1.X, 0.0f) + this.GetRandomUV(range1);
      this.sunEffectUVEnd = MapChunkContent.TexCoords1[293] + new Vector2(range1.X, 0.0f) + this.GetRandomUV(range1);
      Vector2 range2 = MapChunkContent.TexCoords4[294] - MapChunkContent.TexCoords1[294];
      range2.X *= 0.5f;
      this.moonEffectUVStart = MapChunkContent.TexCoords1[294] + new Vector2(range2.X, 0.0f) + this.GetRandomUV(range2);
      this.moonEffectUVEnd = MapChunkContent.TexCoords1[294] + new Vector2(range2.X, 0.0f) + this.GetRandomUV(range2);
      MapTM map = GameInstance.Instance.Map;
      if (!map.IsHost || map.Random.Next(3) != 0 || this.EnvManager.FogCount != 0)
        return;
      GlobalPoint3D globalPoint3D1 = (map.MapBound.Max - map.MapBound.Min) / 2;
      GlobalPoint3D globalPoint3D2 = map.MapBound.Min + globalPoint3D1;
      Color.Lerp(new Color((int) byte.MaxValue, 157, (int) sbyte.MaxValue, (int) byte.MaxValue), new Color((int) byte.MaxValue, 235, 158, (int) byte.MaxValue), (float) map.Random.NextDouble());
      map.Random.NextDouble();
      map.Random.Next(100, 170);
    }

    private Vector2 GetRandomUV(Vector2 range)
    {
      return new Vector2((float) (this.rand.NextDouble() * (double) range.X * 0.899999976158142 + (double) range.X * 0.0500000007450581), (float) (this.rand.NextDouble() * (double) range.Y * 0.899999976158142 + (double) range.Y * 0.0500000007450581));
    }

    private float GetRotaionSpeedModifier()
    {
      float num1 = 1.3f;
      float num2 = 0.05f;
      if ((double) this.Rotation < -1.76714587211609 - (double) num2)
        return num1;
      float num3 = 0.3f;
      float num4 = 1f;
      float num5 = num2 * 2f;
      if ((double) this.Rotation < -1.76714587211609)
        return MathHelper.Lerp(num3, num1, (-1.767146f - this.Rotation) / num2);
      if ((double) this.Rotation <= -1.37444686889648 - (double) num2)
        return num3;
      if ((double) this.Rotation <= (double) num2 - 1.37444686889648)
        return MathHelper.Lerp(num4, num3, (num2 - 1.374447f - this.Rotation) / num5);
      if ((double) this.Rotation < 1.37444686889648 - (double) num2)
        return num4;
      if ((double) this.Rotation < 1.37444686889648 + (double) num2)
        return MathHelper.Lerp(num3, num4, (1.374447f + num2 - this.Rotation) / num5);
      if ((double) this.Rotation <= 1.76714587211609)
        return num3;
      if ((double) this.Rotation <= 1.76714587211609 + (double) num2)
        return MathHelper.Lerp(num1, num3, (1.767146f + num2 - this.Rotation) / num2);
      return num1;
    }

    public enum SunEffect
    {
      None,
      Sunrise,
      Sunset,
    }
  }
}
