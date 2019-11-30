// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Interpolator`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public abstract class Interpolator<T>
  {
    public bool IsActive;
    protected double currentDuration;
    protected double totalDuration;
    protected bool smoothStep;
    protected T value1;
    protected T value2;
    private T currentValue;

    public T Value1
    {
      get
      {
        return this.value1;
      }
    }

    public T Value2
    {
      get
      {
        return this.value2;
      }
    }

    public T CurrentValue
    {
      get
      {
        return this.currentValue;
      }
    }

    public abstract float AmountToLerp { get; }

    public double CurrentDuration
    {
      get
      {
        return this.currentDuration;
      }
    }

    public double TotalDuration
    {
      get
      {
        return this.totalDuration;
      }
    }

    public void Start(T value1, T value2, double totalDuration)
    {
      this.Start(value1, value2, totalDuration, false);
    }

    public void Start(T value1, T value2, double totalDuration, bool smoothStep)
    {
      this.value1 = value1;
      this.value2 = value2;
      this.totalDuration = totalDuration;
      this.smoothStep = smoothStep;
      this.Restart();
    }

    public void Restart()
    {
      this.IsActive = true;
      this.currentDuration = 0.0;
      this.currentValue = this.value1;
    }

    public T Update()
    {
      this.currentDuration += (double) Services.ElapsedTime;
      if (this.currentDuration > this.totalDuration)
      {
        this.currentDuration = this.totalDuration;
        this.IsActive = false;
      }
      return this.currentValue = this.Interpolate();
    }

    public T PeekValue()
    {
      double currentDuration = this.currentDuration;
      bool isActive = this.IsActive;
      T obj = this.Update();
      this.currentDuration = currentDuration;
      this.IsActive = isActive;
      return obj;
    }

    public void Reset()
    {
      this.IsActive = false;
      this.currentDuration = 0.0;
      this.totalDuration = 0.0;
    }

    public void Reset(T value)
    {
      this.IsActive = false;
      this.currentDuration = 0.0;
      this.totalDuration = 0.0;
      this.currentValue = value;
    }

    protected abstract T Interpolate();
  }
}
