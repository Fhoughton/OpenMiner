// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptVector4
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  internal struct ScriptVector4
  {
    public ScriptSingle X;
    public ScriptSingle Y;
    public ScriptSingle Z;
    public ScriptSingle W;

    public static explicit operator Vector4(ScriptVector4 p)
    {
      return new Vector4(p.X.I, p.Y.I, p.Z.I, p.W.I);
    }

    public static implicit operator ScriptVector4(Vector4 p)
    {
      return new ScriptVector4()
      {
        X = {
          I = p.X
        },
        Y = {
          I = p.Y
        },
        Z = {
          I = p.Z
        },
        W = {
          I = p.W
        }
      };
    }
  }
}
