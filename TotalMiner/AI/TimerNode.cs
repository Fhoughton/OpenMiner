// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.TimerNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GUI;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Timer", BehaviourTreeNodeType.Action)]
  internal class TimerNode : BehaviourTreeNode
  {
    public int MilliSeconds;
    private long timer;

    public override string ToStringParms
    {
      get
      {
        return this.MilliSeconds.ToString();
      }
    }

    public TimerNode()
    {
      this.MilliSeconds = 1000;
    }

    public TimerNode(INPCBehaviour npc)
      : base(npc)
    {
      this.MilliSeconds = 1000;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
        this.Status = BehaviourTreeNodeStatus.Failure;
      else if (this.timer == 0L)
      {
        this.timer = Globals1.ElapsedWatch.ElapsedMilliseconds;
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else if ((int) (Globals1.ElapsedWatch.ElapsedMilliseconds - this.timer) >= this.MilliSeconds)
      {
        this.Status = BehaviourTreeNodeStatus.Success;
        this.timer = 0L;
      }
      else
        this.Status = BehaviourTreeNodeStatus.Failure;
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "MilliSeconds":
          win.SetToolTip("The node will fail until this amount of time in milli seconds has passed. The timer is then reset");
          break;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.MilliSeconds = reader.ReadInt32();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.MilliSeconds);
    }
  }
}
