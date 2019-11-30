// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptSingle
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  internal struct ScriptSingle
  {
    public float I;
    public ScriptValueType T;

    public bool IsZero
    {
      get
      {
        if (this.T == ScriptValueType.NumLiterial)
          return (double) this.I == 0.0;
        return false;
      }
    }

    public static implicit operator ScriptSingle(float p)
    {
      return new ScriptSingle() { I = p };
    }
  }
}
