// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.AVParams
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.API
{
  public struct AVParams
  {
    public float X;
    public float Y;
    public float Z;
    public int Face;
    public byte BlockID;
    public byte Aux;
    public NormalizedShort2 TC;
    public GlobalPoint3D Point;
    public Vector3 Pos1;
    public Vector3 Pos2;
    public bool IsCorner;
    public bool UseOwnLight;
    public bool WindAffected;
    public int WindUniformWaveRandomness;
    public int WindUniformWaveRandomnessHash;

    public void SetFrom(TempAVParams p)
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
