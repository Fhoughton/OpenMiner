// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptVector2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  internal struct ScriptVector2
  {
    public ScriptSingle X;
    public ScriptSingle Y;

    public static explicit operator Vector2(ScriptVector2 p)
    {
      return new Vector2(p.X.I, p.Y.I);
    }

    public static implicit operator ScriptVector2(Vector2 p)
    {
      return new ScriptVector2()
      {
        X = {
          I = p.X
        },
        Y = {
          I = p.Y
        }
      };
    }
  }
}
