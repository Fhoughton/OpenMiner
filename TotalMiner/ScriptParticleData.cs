// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptParticleData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  internal struct ScriptParticleData
  {
    public ScriptSingle EmitFreq;
    public ScriptSingle Duration;
    public ScriptSingle Rotation;
    public ScriptVector3 Velocity;
    public ScriptVector3 VelocityVariance;
    public ScriptVector3 EmitPosOffset;
    public ScriptVector3 EmitPosVariance;
    public ScriptVector4 Size;
    public ScriptSingle WindFactor;
    public ScriptSingle Gravity;
    public ScriptColor StartColor;
    public ScriptColor EndColor;
    public ScriptCoordType VelocityType;
    public ScriptSingle Proximity;
  }
}
