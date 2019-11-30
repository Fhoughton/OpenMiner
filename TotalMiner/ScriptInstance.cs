// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptInstance
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal class ScriptInstance
  {
    public PcgRandom OrigRandom = new PcgRandom();
    public long PC;
    public long CurentCmdPC;
    public bool UpdatePC;
    public long WaitTime;
    public long WaitingOnResult;
    public bool IsCancelled;
    public Player Player;
    public Actor Actor;
    public GameInstance Instance;
    public MapTM MapTM;
    public int CmdIndex;
    public long BeginPC;
    public int VarCount;
    public double[] Vars;
    public string[] VarNames;
    public bool NeedsCommit;
    public GlobalPoint3D? ScriptOffset;
    public GlobalPoint3D? BlockOffset;
    public ScriptConditionState ConditionState;
    public UpdateBlockMethod Method;
    public Actor Target;
    public Actor Killer;
    public Actor OrigActor;
    public ScriptContext Context;
    public ScriptContext DefaultContext;
    public Script Script;
    public PcgRandom Random;
    public string BlockOffsetString;
    public bool NegativeResult;
    public ScriptInstanceCallback IntersectTarget;
    public ScriptInstanceCallback InputTarget;
    public double? InputResult;
    public Action<Script, Player> OnComplete;
    public ScriptInstance Parent;
    public GlobalPoint3D LastRayHit;
    public ushort[] ParentVars;
    public bool IsComplete;
    public int InstancePoolHandle;
    public int Seed;

    public void SetContext(ScriptContext context)
    {
      if (context == this.Context)
        return;
      this.Context = context;
      switch (context)
      {
        case ScriptContext.PlayerDefault:
          this.Player = this.OrigActor as Player;
          this.Actor = this.OrigActor;
          break;
        case ScriptContext.PlayerTarget:
          this.Player = this.Target as Player;
          this.Actor = this.Target;
          break;
        case ScriptContext.PlayerKiller:
          this.Player = this.Killer as Player;
          this.Actor = this.Killer;
          break;
      }
    }

    public double GetVarValue(ushort varIndex)
    {
      if (varIndex < (ushort) 0 || (int) varIndex >= this.VarCount)
        return 0.0;
      return this.Vars[(int) varIndex];
    }

    public double GetVarValue(string name)
    {
      int varIndex = this.GetVarIndex(name);
      if (varIndex < 0 || varIndex >= this.VarCount)
        return 0.0;
      return this.Vars[varIndex];
    }

    private int GetVarIndex(string name)
    {
      if (this.VarNames != null)
      {
        for (int index = 0; index < this.VarCount; ++index)
        {
          if (this.VarNames[index] == name)
            return index;
        }
      }
      return -1;
    }
  }
}
