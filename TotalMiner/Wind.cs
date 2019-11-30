// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Wind
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner
{
  internal class Wind
  {
    private const float maxWindSpeed = 10f;
    public Vector3 WindVelocity;
    public Vector3 ShaderWindDirection;
    public float ShaderWindAmount;
    public float ShaderWindTime;
    private FloatInterpolator speedChange;
    private FloatInterpolator angleChange;
    private PcgRandom random;
    private Vector3 windVelNorm;

    public float WindFactor
    {
      get
      {
        return Globals2.GameProperties.SaveGame.Header.WindFactor;
      }
      set
      {
        Globals2.GameProperties.SaveGame.Header.WindFactor = value;
        if ((double) Globals2.GameProperties.SaveGame.Header.WindFactor > 0.0)
          return;
        this.WindVelocity = Vector3.Zero;
        this.windVelNorm = Vector3.Zero;
        this.ShaderWindDirection = Vector3.Zero;
        this.ShaderWindAmount = 0.0f;
        this.speedChange.Reset();
        this.angleChange.Reset();
      }
    }

    public Wind(PcgRandom random)
    {
      this.random = random;
      this.speedChange = new FloatInterpolator();
      this.angleChange = new FloatInterpolator();
    }

    public void Update()
    {
      if ((double) this.WindFactor <= 0.0)
        return;
      bool flag = false;
      if (this.angleChange.IsActive)
      {
        double num = (double) this.angleChange.Update();
        flag = true;
      }
      if (this.speedChange.IsActive)
      {
        double num = (double) this.speedChange.Update();
        flag = true;
      }
      float num1 = this.speedChange.CurrentValue * 10f * this.WindFactor;
      if (flag)
      {
        Vector2 vector2 = MyMathHelper.RotateVector2ByAngle(new Vector2(0.0f, 1f), this.angleChange.CurrentValue);
        this.WindVelocity = new Vector3(vector2.X, 0.0f, vector2.Y);
        this.WindVelocity.Normalize();
        this.windVelNorm = this.WindVelocity;
        this.WindVelocity.X *= num1;
        this.WindVelocity.Z *= num1;
        GraphicStatics.ParticleShader.Wind.SetValue(this.WindVelocity);
      }
      this.ShaderWindDirection = this.windVelNorm;
      this.ShaderWindAmount = 0.25f * (float) (((double) num1 * 0.949999988079071 + 0.5) / 10.0);
      float num2 = (float) (1.0 + (double) this.ShaderWindAmount * 20.0);
      this.ShaderWindTime += Services.ElapsedTime * num2;
      if (this.random.Next(1000) == 0 || (double) this.angleChange.Value2 == 0.0)
        this.angleChange.Start(this.angleChange.CurrentValue, (float) (this.random.NextDouble() * 6.28318548202515 - 3.14159274101257), this.random.NextDouble() * 10.0 + 10.0, true);
      if (this.random.Next(1000) != 0 && (double) this.speedChange.Value2 != 0.0)
        return;
      double num3 = this.random.NextDouble();
      float num4 = num3 >= 0.100000001490116 ? (num3 >= 0.300000011920929 ? (float) this.random.NextDouble() * 0.6f : (float) this.random.NextDouble() * 0.1f) : (float) (this.random.NextDouble() * 0.200000002980232 + 0.800000011920929);
      this.speedChange.Start(this.speedChange.CurrentValue, num4, this.random.NextDouble() * 4.0 + (double) Math.Abs(num4 - this.speedChange.CurrentValue) * 6.0, true);
    }
  }
}
