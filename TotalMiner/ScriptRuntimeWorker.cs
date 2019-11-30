// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ScriptRuntimeWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StudioForge.TotalMiner
{
  internal class ScriptRuntimeWorker : IThreadWorkItem
  {
    private Pool<ScriptInstance> instancePool = new Pool<ScriptInstance>();
    private List<ScriptInstance> waitingOnResult = new List<ScriptInstance>();
    private List<ScriptRuntimeWorker.InputResultData> inputResults = new List<ScriptRuntimeWorker.InputResultData>();
    private List<ScriptRuntimeWorker.IntersectResultData> intersectResults = new List<ScriptRuntimeWorker.IntersectResultData>();
    private object waitingSemaphore = new object();
    private MapTM map;
    private GameInstance instance;
    private Stopwatch clock;
    private PriorityLevel priority;
    private ScriptInstance lastExecutedScript;
    private ScriptInstance currentlyExecutingScriptInstance;
    private Queue<ScriptInstance> scriptQueue;
    private ScriptRuntime scriptRuntime;

    public string Name
    {
      get
      {
        return "ScriptRuntime";
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public int ScriptQueueCount
    {
      get
      {
        return this.scriptQueue.Count + this.waitingOnResult.Count;
      }
    }

    public ScriptRuntimeWorker(GameInstance instance, MapTM map, PriorityLevel priority)
    {
      this.instance = instance;
      this.map = map;
      this.priority = priority;
      this.clock = new Stopwatch();
      this.clock.Start();
      this.scriptQueue = new Queue<ScriptInstance>();
      this.scriptRuntime = new ScriptRuntime(instance, this.clock);
    }

    public void QueueScript(Script script, ScriptExecuteData data)
    {
      int next = this.instancePool.GetNext();
      ScriptInstance scriptInstance = this.instancePool.List[next];
      scriptInstance.Script = script;
      scriptInstance.Instance = this.instance;
      scriptInstance.MapTM = this.map;
      scriptInstance.CmdIndex = 0;
      scriptInstance.ScriptOffset = data.ScriptOffset;
      scriptInstance.BlockOffset = data.BlockOffset;
      scriptInstance.BlockOffsetString = (string) null;
      scriptInstance.Random = data.Random;
      scriptInstance.ConditionState = ScriptConditionState.Normal;
      scriptInstance.Method = UpdateBlockMethod.Player;
      scriptInstance.OnComplete = data.OnComplete;
      scriptInstance.Parent = data.Parent;
      scriptInstance.ParentVars = data.PassedVars;
      scriptInstance.VarCount = 0;
      scriptInstance.OrigActor = data.Actor;
      scriptInstance.Target = data.Target;
      scriptInstance.Killer = data.Killer;
      scriptInstance.WaitTime = 0L;
      scriptInstance.Seed = data.Seed;
      if (scriptInstance.Seed == 0)
        scriptInstance.Seed = this.instance.Random.Next();
      scriptInstance.NeedsCommit = false;
      scriptInstance.IsComplete = false;
      scriptInstance.IsCancelled = false;
      scriptInstance.InstancePoolHandle = next;
      scriptInstance.PC = 0L;
      scriptInstance.UpdatePC = true;
      scriptInstance.WaitingOnResult = 0L;
      scriptInstance.NegativeResult = false;
      scriptInstance.IntersectTarget = new ScriptInstanceCallback(this.GetIntersectTarget);
      scriptInstance.InputTarget = new ScriptInstanceCallback(this.GetInputTarget);
      scriptInstance.DefaultContext = data.Context;
      scriptInstance.Context = ScriptContext.None;
      scriptInstance.SetContext(data.Context == ScriptContext.None ? ScriptContext.PlayerDefault : data.Context);
      lock (this.scriptQueue)
        this.scriptQueue.Enqueue(scriptInstance);
    }

    public bool CancelScript(Script script, Actor actor)
    {
      bool flag = false;
      lock (this.scriptQueue)
      {
        foreach (ScriptInstance script1 in this.scriptQueue)
        {
          if (this.scriptRuntime.CancelScript(script1, script, actor))
            flag = true;
        }
        if (this.scriptRuntime.CancelScript(this.currentlyExecutingScriptInstance, script, actor))
          flag = true;
        if (this.scriptRuntime.CancelScript(this.lastExecutedScript, script, actor))
          flag = true;
      }
      lock (this.waitingSemaphore)
      {
        foreach (ScriptInstance si in this.waitingOnResult)
        {
          if (this.scriptRuntime.CancelScript(si, script, actor))
            flag = true;
        }
      }
      return flag;
    }

    public List<Script> GetListOfQueuedScripts()
    {
      List<Script> scriptList = new List<Script>(this.scriptQueue.Count + 1);
      lock (this.scriptQueue)
      {
        foreach (ScriptInstance script in this.scriptQueue)
        {
          if (script != null && script.Script != null && !script.IsCancelled)
            scriptList.Add(script.Script);
        }
        if (this.currentlyExecutingScriptInstance != null)
        {
          if (this.currentlyExecutingScriptInstance.Script != null)
          {
            if (!this.currentlyExecutingScriptInstance.IsCancelled)
              scriptList.Add(this.currentlyExecutingScriptInstance.Script);
          }
        }
      }
      lock (this.waitingSemaphore)
      {
        foreach (ScriptInstance scriptInstance in this.waitingOnResult)
          scriptList.Add(scriptInstance.Script);
      }
      return scriptList;
    }

    public void Update()
    {
      try
      {
        if (!this.instance.IsMapActiveIgnoreGuide)
          return;
        this.UpdateCore();
      }
      finally
      {
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
      }
    }

    private void UpdateCore()
    {
      this.ProcessReceivedResults();
      ScriptInstance si = (ScriptInstance) null;
      lock (this.scriptQueue)
      {
        if (this.scriptQueue.Count > 0)
        {
          si = this.scriptQueue.Dequeue();
          if (si != null)
          {
            if (si.WaitTime > this.clock.ElapsedMilliseconds)
            {
              this.scriptQueue.Enqueue(si);
              si = (ScriptInstance) null;
            }
          }
        }
      }
      if (si == null)
        return;
      this.ExecuteScript(si);
    }

    private void ExecuteScript(ScriptInstance si)
    {
      try
      {
        if (si.IsComplete || si.IsCancelled)
          return;
        this.currentlyExecutingScriptInstance = si;
        this.scriptRuntime.ExecuteScript(si);
      }
      finally
      {
        this.lastExecutedScript = this.currentlyExecutingScriptInstance;
        this.currentlyExecutingScriptInstance = (ScriptInstance) null;
        if (si.IsComplete || si.IsCancelled)
          this.instancePool.Release(si.InstancePoolHandle);
        else if (si.WaitingOnResult > 0L)
        {
          lock (this.waitingSemaphore)
            this.waitingOnResult.Add(si);
        }
        else
        {
          lock (this.scriptQueue)
            this.scriptQueue.Enqueue(si);
        }
      }
    }

    public void PostInputResult(string name, GamerID gamerID, double? val)
    {
      ScriptRuntimeWorker.InputResultData inputResultData = new ScriptRuntimeWorker.InputResultData()
      {
        ScriptName = name,
        GamerID = gamerID,
        Value = val,
        Timer = Globals1.ElapsedWatch.ElapsedMilliseconds + 5000L
      };
      lock (this.waitingSemaphore)
        this.inputResults.Add(inputResultData);
    }

    public void PostIntersectResult(string name, GamerID gamerID, GamerID targetID)
    {
      ScriptRuntimeWorker.IntersectResultData intersectResultData = new ScriptRuntimeWorker.IntersectResultData()
      {
        ScriptName = name,
        GamerID = gamerID,
        TargetID = targetID,
        Timer = Globals1.ElapsedWatch.ElapsedMilliseconds + 5000L
      };
      lock (this.waitingSemaphore)
        this.intersectResults.Add(intersectResultData);
    }

    public void ProcessReceivedResults()
    {
      lock (this.waitingSemaphore)
      {
        for (int index1 = this.waitingOnResult.Count - 1; index1 >= 0; --index1)
        {
          ScriptInstance scriptInstance = this.waitingOnResult[index1];
          bool flag = false;
          for (int index2 = 0; index2 < this.inputResults.Count; ++index2)
          {
            ScriptRuntimeWorker.InputResultData inputResult = this.inputResults[index2];
            if (scriptInstance.Script.Name.Equals(inputResult.ScriptName, StringComparison.OrdinalIgnoreCase) && scriptInstance.Player != null && scriptInstance.Player.GamerID == inputResult.GamerID)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            for (int index2 = 0; index2 < this.intersectResults.Count; ++index2)
            {
              ScriptRuntimeWorker.IntersectResultData intersectResult = this.intersectResults[index2];
              if (scriptInstance.Script.Name.Equals(intersectResult.ScriptName, StringComparison.OrdinalIgnoreCase) && scriptInstance.Player != null && scriptInstance.Player.GamerID == intersectResult.GamerID)
              {
                flag = true;
                break;
              }
            }
          }
          if (flag)
          {
            scriptInstance.WaitingOnResult = 0L;
            this.waitingOnResult.RemoveAt(index1);
            this.scriptQueue.Enqueue(scriptInstance);
          }
        }
      }
    }

    private bool GetInputTarget(ScriptInstance si)
    {
      bool flag = false;
      lock (this.waitingSemaphore)
      {
        for (int index = 0; index < this.inputResults.Count; ++index)
        {
          ScriptRuntimeWorker.InputResultData inputResult = this.inputResults[index];
          if (si.Script.Name.Equals(inputResult.ScriptName, StringComparison.OrdinalIgnoreCase) && si.Player != null && si.Player.GamerID == inputResult.GamerID)
          {
            si.InputResult = inputResult.Value;
            inputResult.Invalid = flag = true;
            this.inputResults[index] = inputResult;
            break;
          }
        }
        if (flag)
        {
          for (int index = this.inputResults.Count - 1; index >= 0; --index)
          {
            if (this.inputResults[index].Invalid)
              this.inputResults.RemoveAt(index);
          }
        }
      }
      return flag;
    }

    private bool GetIntersectTarget(ScriptInstance si)
    {
      bool flag = false;
      lock (this.waitingSemaphore)
      {
        for (int index = 0; index < this.intersectResults.Count; ++index)
        {
          ScriptRuntimeWorker.IntersectResultData intersectResult = this.intersectResults[index];
          if (si.Script.Name.Equals(intersectResult.ScriptName, StringComparison.OrdinalIgnoreCase) && si.Player != null && si.Player.GamerID == intersectResult.GamerID)
          {
            si.NegativeResult = !intersectResult.TargetID.IsGamer;
            si.Target = si.NegativeResult ? (Actor) null : si.Instance.GetCharacter(intersectResult.TargetID);
            intersectResult.Invalid = flag = true;
            this.intersectResults[index] = intersectResult;
            break;
          }
        }
        if (flag)
        {
          for (int index = this.intersectResults.Count - 1; index >= 0; --index)
          {
            if (this.intersectResults[index].Invalid)
              this.intersectResults.RemoveAt(index);
          }
        }
      }
      return flag;
    }

    private enum ExecuteState
    {
      FirstRun,
      Waiting,
      Complete,
    }

    private struct InputResultData
    {
      public string ScriptName;
      public GamerID GamerID;
      public double? Value;
      public long Timer;
      public bool Invalid;
    }

    private struct IntersectResultData
    {
      public string ScriptName;
      public GamerID GamerID;
      public GamerID TargetID;
      public long Timer;
      public bool Invalid;
    }
  }
}
