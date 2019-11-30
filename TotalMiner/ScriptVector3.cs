// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptVector3
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  internal struct ScriptVector3
  {
    public ScriptSingle X;
    public ScriptSingle Y;
    public ScriptSingle Z;

    public static explicit operator Vector3(ScriptVector3 p)
    {
      return new Vector3(p.X.I, p.Y.I, p.Z.I);
    }

    public static implicit operator ScriptVector3(Vector3 p)
    {
      return new ScriptVector3()
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
