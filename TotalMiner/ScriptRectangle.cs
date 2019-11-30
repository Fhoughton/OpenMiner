// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptRectangle
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  internal struct ScriptRectangle
  {
    public ScriptInt32 X;
    public ScriptInt32 Y;
    public ScriptInt32 W;
    public ScriptInt32 H;

    public static explicit operator Rectangle(ScriptRectangle p)
    {
      return new Rectangle(p.X.I, p.Y.I, p.W.I, p.H.I);
    }

    public static implicit operator ScriptRectangle(Rectangle p)
    {
      return new ScriptRectangle()
      {
        X = {
          I = p.X
        },
        Y = {
          I = p.Y
        },
        W = {
          I = p.Width
        },
        H = {
          I = p.Height
        }
      };
    }
  }
}
