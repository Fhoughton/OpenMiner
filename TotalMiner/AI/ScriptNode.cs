// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ScriptNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Script", BehaviourTreeNodeType.Action)]
  internal class ScriptNode : BehaviourTreeNode
  {
    public string ScriptName;
    public string EmbeddedScript;
    public BehaviourTreeNodeCompareTarget Target;
    public bool WaitForCompletion;
    private bool waiting;
    private Script script;
    private Script embeddedScript;
    private bool scriptLoaded;

    public override string ToStringParms
    {
      get
      {
        if (this.ScriptName == null)
          return this.CutString(this.EmbeddedScript, 8);
        return this.CutString(this.ScriptName, 8);
      }
    }

    public ScriptNode()
    {
    }

    public ScriptNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.waiting)
        return;
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        this.Status = this.ExecuteScript(this.npc, this.npc.AITarget, new Action<Script, Actor>(this.OnComplete)) ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
        if (this.WaitForCompletion)
          return;
        this.waiting = false;
      }
    }

    public bool ExecuteScript(
      INPCBehaviour npc,
      INPCBehaviour target,
      Action<Script, Actor> onComplete)
    {
      Actor target1 = this.Target == BehaviourTreeNodeCompareTarget.Target ? target as Actor : npc as Actor;
      if (target1 == null)
        return false;
      if (this.embeddedScript != null)
        this.ExecuteScript(this.embeddedScript, target1, onComplete);
      else if (this.EmbeddedScript.IsNotEmpty())
      {
        this.embeddedScript = new Script("temp", 1);
        this.embeddedScript.Commands = Parser.Split(this.EmbeddedScript, ',', '[', ']');
        this.ExecuteScript(this.embeddedScript, target1, onComplete);
      }
      if (this.script != null)
        this.ExecuteScript(this.script, target1, onComplete);
      else if (!this.scriptLoaded && this.ScriptName.IsNotEmpty())
      {
        this.script = GameInstance.Instance.GetScript(this.ScriptName);
        this.scriptLoaded = true;
        if (this.script != null)
          this.ExecuteScript(this.script, target1, onComplete);
      }
      return true;
    }

    private void ExecuteScript(Script script, Actor target, Action<Script, Actor> onComplete)
    {
      GlobalPoint3D? nullable = new GlobalPoint3D?();
      if (this.npc.SpawnPoint != GlobalPoint3D.Zero)
        nullable = new GlobalPoint3D?(this.npc.SpawnPoint);
      ScriptExecuteData data = new ScriptExecuteData()
      {
        BlockOffset = nullable,
        Actor = target,
        OnComplete = (Action<Script, Player>) onComplete
      };
      GameInstance.Instance.ExecuteScript(script, data, true);
      this.waiting = true;
    }

    private void OnComplete(Script script, Actor actor)
    {
      this.Status = BehaviourTreeNodeStatus.Success;
      this.waiting = false;
    }

    public override bool IsPropertyEnabled(string propertyName)
    {
      switch (propertyName)
      {
        case "WaitForCompletion":
          return false;
        default:
          return base.IsPropertyEnabled(propertyName);
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.ScriptName = reader.ReadString();
      if (this.ScriptName.Length == 0)
        this.ScriptName = (string) null;
      this.EmbeddedScript = reader.ReadString();
      if (this.EmbeddedScript.Length == 0)
        this.EmbeddedScript = (string) null;
      this.Target = (BehaviourTreeNodeCompareTarget) reader.ReadByte();
      this.WaitForCompletion = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.ScriptName != null ? this.ScriptName : "");
      writer.Write(this.EmbeddedScript != null ? this.EmbeddedScript : "");
      writer.Write((byte) this.Target);
      writer.Write(this.WaitForCompletion);
    }
  }
}
