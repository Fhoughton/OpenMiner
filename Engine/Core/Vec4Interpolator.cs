// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Vec4Interpolator
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Core
{
  public class Vec4Interpolator : Interpolator<Vector4>
  {
    public override float AmountToLerp
    {
      get
      {
        float num1 = this.value1.LengthSquared();
        float num2 = this.value2.LengthSquared();
        if ((double) num2 <= (double) num1)
          return (float) (((double) this.CurrentValue.LengthSquared() - (double) num2) / ((double) num1 - (double) num2));
        return (float) (((double) this.CurrentValue.LengthSquared() - (double) num1) / ((double) num2 - (double) num1));
      }
    }

    protected override Vector4 Interpolate()
    {
      if (!this.smoothStep)
        return Vector4.Lerp(this.value1, this.value2, (float) (this.currentDuration / this.totalDuration));
      return Vector4.SmoothStep(this.value1, this.value2, (float) (this.currentDuration / this.totalDuration));
    }
  }
}
