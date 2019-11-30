// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptColor
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  internal struct ScriptColor
  {
    public ScriptInt32 R;
    public ScriptInt32 G;
    public ScriptInt32 B;
    public ScriptInt32 A;

    public static explicit operator Color(ScriptColor p)
    {
      return new Color(p.R.I, p.G.I, p.B.I, p.A.I);
    }

    public static implicit operator ScriptColor(Color p)
    {
      return new ScriptColor()
      {
        R = {
          I = (int) p.R
        },
        G = {
          I = (int) p.G
        },
        B = {
          I = (int) p.B
        },
        A = {
          I = (int) p.A
        }
      };
    }
  }
}
