// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Pulsator
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using System;

namespace StudioForge.Engine.Core
{
  public class Pulsator
  {
    private float v;
    public bool IsActive;
    private float value1;
    private float value2;
    private float seconds;
    private float timer;
    private bool reverse;
    private bool ending;
    private bool smoothStep;

    public float Value
    {
      get
      {
        return this.v;
      }
      set
      {
        if (float.IsNaN(value))
          throw new Exception("nan");
        this.v = value;
      }
    }

    public void Start(float value1, float value2, float seconds)
    {
      this.Start(value1, value2, seconds, false);
    }

    public void Start(float value1, float value2, float seconds, bool smoothStep)
    {
      this.value1 = value1;
      this.value2 = value2;
      this.seconds = seconds;
      this.smoothStep = smoothStep;
      this.reverse = false;
      this.timer = 0.0f;
      this.Value = value1;
      this.IsActive = true;
      this.ending = false;
    }

    public void StartFromCurrent(float value1, float value2, float seconds)
    {
      this.StartFromCurrent(value1, value2, seconds, false);
    }

    public void StartFromCurrent(float value1, float value2, float seconds, bool smoothStep)
    {
      this.value1 = value1;
      this.value2 = value2;
      this.seconds = seconds;
      this.smoothStep = smoothStep;
      this.reverse = false;
      this.timer = 0.0f;
      if ((double) value1 != 0.0 || (double) value2 != 0.0)
        this.timer = seconds - seconds * Math.Abs((float) (((double) value2 - (double) this.Value) / ((double) value2 - (double) value1)));
      this.IsActive = true;
      this.ending = false;
    }

    public void End(float value2)
    {
      this.value1 = this.Value;
      this.value2 = value2;
      this.reverse = false;
      this.timer = 0.0f;
      if ((double) this.value1 != 0.0 || (double) value2 != 0.0)
        this.timer = this.seconds - this.seconds * Math.Abs((float) (((double) value2 - (double) this.Value) / ((double) value2 - (double) this.value1)));
      this.IsActive = true;
      this.ending = true;
    }

    public void Update()
    {
      if ((double) this.seconds <= 0.0)
        return;
      this.timer += Services.ElapsedTime;
      if ((double) this.timer >= (double) this.seconds)
      {
        this.timer -= this.seconds;
        this.reverse = !this.reverse;
        if (this.ending)
          this.IsActive = false;
      }
      if (this.reverse)
        this.Value = this.smoothStep ? MathHelper.SmoothStep(this.value2, this.value1, this.timer / this.seconds) : MathHelper.Lerp(this.value2, this.value1, this.timer / this.seconds);
      else
        this.Value = this.smoothStep ? MathHelper.SmoothStep(this.value1, this.value2, this.timer / this.seconds) : MathHelper.Lerp(this.value1, this.value2, this.timer / this.seconds);
    }
  }
}
