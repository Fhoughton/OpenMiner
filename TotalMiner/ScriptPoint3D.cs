// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptPoint3D
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner
{
  internal struct ScriptPoint3D
  {
    public ScriptInt32 X;
    public ScriptInt32 Y;
    public ScriptInt32 Z;

    public static explicit operator GlobalPoint3D(ScriptPoint3D p)
    {
      return new GlobalPoint3D(p.X.I, p.Y.I, p.Z.I);
    }

    public static implicit operator ScriptPoint3D(GlobalPoint3D p)
    {
      return new ScriptPoint3D()
      {
        X = {
          I = p.X
        },
        Y = {
          I = p.Y
        },
        Z = {
          I = p.Z
        }
      };
    }
  }
}
