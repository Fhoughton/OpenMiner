// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.FloatInterpolator
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Core
{
  public class FloatInterpolator : Interpolator<float>
  {
    public override float AmountToLerp
    {
      get
      {
        if ((double) this.value2 <= (double) this.value1)
          return (float) (((double) this.CurrentValue - (double) this.value2) / ((double) this.value1 - (double) this.value2));
        return (float) (((double) this.CurrentValue - (double) this.value1) / ((double) this.value2 - (double) this.value1));
      }
    }

    protected override float Interpolate()
    {
      if (!this.smoothStep)
        return MathHelper.Lerp(this.value1, this.value2, (float) (this.currentDuration / this.totalDuration));
      return MathHelper.SmoothStep(this.value1, this.value2, (float) (this.currentDuration / this.totalDuration));
    }
  }
}
