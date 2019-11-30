// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Timer
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public struct Timer
  {
    public double CurrentDuration;
    public double TotalDuration;

    public bool IsActive
    {
      get
      {
        return this.CurrentDuration < this.TotalDuration;
      }
    }

    public bool IsComplete
    {
      get
      {
        return this.CurrentDuration == this.TotalDuration;
      }
    }

    public float AmountToLerp
    {
      get
      {
        return (float) (this.CurrentDuration / this.TotalDuration);
      }
    }

    public void Start(double seconds)
    {
      this.CurrentDuration = 0.0;
      this.TotalDuration = seconds;
    }

    public void Update()
    {
      this.CurrentDuration += (double) Services.ElapsedTime;
      if (this.CurrentDuration <= this.TotalDuration)
        return;
      this.CurrentDuration = this.TotalDuration;
    }
  }
}
