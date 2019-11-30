// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptExecuteData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal struct ScriptExecuteData
  {
    public Actor Actor;
    public Actor Target;
    public Actor Killer;
    public ScriptContext Context;
    public int Seed;
    public GlobalPoint3D? ScriptOffset;
    public GlobalPoint3D? BlockOffset;
    public PcgRandom Random;
    public int Delay;
    public bool TempScript;
    public Action<Script, Player> OnComplete;
    public ScriptInstance Parent;
    public ushort[] PassedVars;
  }
}
