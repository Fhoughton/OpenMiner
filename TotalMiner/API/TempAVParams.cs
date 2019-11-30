// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.TempAVParams
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace StudioForge.TotalMiner.API
{
  public struct TempAVParams
  {
    public float X;
    public float Y;
    public float Z;
    public NormalizedShort2 TC;
    public bool WindAffected;
    public int WindUniformWaveRandomness;
    public int WindUniformWaveRandomnessHash;

    public TempAVParams(ref AVParams p)
    {
      this.X = p.X;
      this.Y = p.Y;
      this.Z = p.Z;
      this.TC = p.TC;
      this.WindAffected = p.WindAffected;
      this.WindUniformWaveRandomness = p.WindUniformWaveRandomness;
      this.WindUniformWaveRandomnessHash = p.WindUniformWaveRandomnessHash;
    }

    public void SetFrom(ref AVParams p)
    {
      this.X = p.X;
      this.Y = p.Y;
      this.Z = p.Z;
      this.TC = p.TC;
      this.WindAffected = p.WindAffected;
      this.WindUniformWaveRandomness = p.WindUniformWaveRandomness;
      this.WindUniformWaveRandomnessHash = p.WindUniformWaveRandomnessHash;
    }
  }
}
