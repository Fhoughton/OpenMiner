// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Vec3Interpolator
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Core
{
  public class Vec3Interpolator : Interpolator<Vector3>
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

    protected override Vector3 Interpolate()
    {
      if (!this.smoothStep)
        return Vector3.Lerp(this.value1, this.value2, (float) (this.currentDuration / this.totalDuration));
      return Vector3.SmoothStep(this.value1, this.value2, (float) (this.currentDuration / this.totalDuration));
    }
  }
}
