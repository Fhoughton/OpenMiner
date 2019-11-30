// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.DeactivateNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Deactivate", BehaviourTreeNodeType.Action)]
  internal class DeactivateNode : BehaviourTreeNode
  {
    private static List<ActorType> searchTypes = new List<ActorType>()
    {
      ActorType.Player
    };
    public float Distance;

    public override string ToStringParms
    {
      get
      {
        return this.Distance.ToString();
      }
    }

    public DeactivateNode()
    {
    }

    public DeactivateNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        INPCBehaviour actor = this.npc.FindActor(NpcQueryPreference.Closest, 0.0f, DeactivateNode.searchTypes, (List<ActorType>) null);
        if ((actor != null ? (double) Vector3.DistanceSquared(this.npc.Position, actor.Position) : (double) this.Distance * (double) this.Distance) < (double) this.Distance * (double) this.Distance)
        {
          this.Status = BehaviourTreeNodeStatus.Failure;
        }
        else
        {
          this.npc.ChangeState(ActorState.InActive);
          this.Status = BehaviourTreeNodeStatus.Success;
        }
      }
    }

    public override bool IsPropertyEnabled(string propertyName)
    {
      switch (propertyName)
      {
        case "Continue":
          return false;
        default:
          return base.IsPropertyEnabled(propertyName);
      }
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.Distance = 50f;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Distance = reader.ReadSingle();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Distance);
    }
  }
}
