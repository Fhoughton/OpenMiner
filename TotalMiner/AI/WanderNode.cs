// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.WanderNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using System;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Wander", BehaviourTreeNodeType.Action)]
  internal class WanderNode : MoveNode
  {
    private float waitTime;
    private float moveTime;
    private float expectedMoveTime;
    private float lastDiffSq;

    public bool IsWaiting
    {
      get
      {
        return (double) this.waitTime > 0.0;
      }
    }

    public WanderNode()
    {
    }

    public WanderNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    public override void SetNPC(INPCBehaviour npc)
    {
      base.SetNPC(npc);
      if (npc == null)
        return;
      this.waitTime = 0.1f;
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        if (this.npc.CurrentDialog != null && this.npc.CurrentDialog.StopMoving || (double) this.Distance == 0.0)
        {
          this.npc.StandStill();
          if (this.npc.CurrentDialogTarget != null)
            this.npc.LookAt(CoordType.Absolute, this.npc.CurrentDialogTarget.EyePosition, false);
        }
        else if ((double) this.waitTime > 0.0)
        {
          this.waitTime -= Services.ElapsedTime;
          if ((double) this.waitTime <= 0.0)
            this.SetNextPosition(false);
        }
        else
        {
          Vector3 finalPosition = this.npc.GetFinalPosition(this.PositionType, this.Position);
          this.npc.LookAt(CoordType.Absolute, finalPosition, false);
          this.npc.MoveTo(finalPosition, this.VelocityModifier, this.CanJump, this.moveType);
          float num = new Vector2(finalPosition.X - this.npc.Position.X, finalPosition.Z - this.npc.Position.Z).LengthSquared();
          if ((double) num <= 0.200000002980232)
            this.SetNextPosition(true);
          else if ((double) this.waitTime < -10.0 || (double) Math.Abs(num - this.lastDiffSq) < 0.0199999995529652)
          {
            this.SetNextPosition(false);
          }
          else
          {
            this.lastDiffSq = num;
            this.moveTime += Services.ElapsedTime;
            if ((double) this.moveTime > (double) this.expectedMoveTime * 1.79999995231628)
              this.SetNextPosition(true);
          }
        }
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    private void SetNextPosition(bool canStandStill)
    {
      this.waitTime = 0.0f;
      if ((double) this.Distance <= 0.0)
        return;
      if (canStandStill && this.npc.Random.Next(2) == 1)
      {
        this.waitTime = (float) GameInstance.Instance.Random.Next(1, 4) * 0.5f;
        this.npc.StandStill();
      }
      else
      {
        if (this.PositionType == CoordType.SpawnRelative && this.npc.SpawnPoint != GlobalPoint3D.Zero)
        {
          Vector3 finalPosition;
          Vector3 blockCenter;
          do
          {
            this.Position.X = (float) (this.npc.Random.NextDouble() * (double) this.Distance * 2.0) - this.Distance;
            this.Position.Y = 0.5f;
            this.Position.Z = (float) (this.npc.Random.NextDouble() * (double) this.Distance * 2.0) - this.Distance;
            finalPosition = this.npc.GetFinalPosition(this.PositionType, this.Position);
            blockCenter = this.npc.Map.GetBlockCenter(this.npc.SpawnPoint);
          }
          while ((double) Vector2.DistanceSquared(new Vector2(finalPosition.X, finalPosition.Z), new Vector2(blockCenter.X, blockCenter.Z)) >= (double) this.Distance * (double) this.Distance);
        }
        else
        {
          this.PositionType = CoordType.Absolute;
          this.Position = this.npc.GetRandomPositionNearPoint(this.npc.Position, (float) (this.npc.Random.NextDouble() * (double) this.Distance + 1.0));
        }
        this.moveTime = 0.0f;
        this.expectedMoveTime = Vector3.Distance(this.Position, this.npc.Position) / (float) ((double) this.npc.MoveSpeed * (double) this.VelocityModifier * 60.0);
        this.SetNextMoveType();
      }
    }

    public override void SetPropertyDefaults()
    {
      base.SetPropertyDefaults();
      this.PositionType = CoordType.SpawnRelative;
      this.Distance = 5f;
      this.VelocityModifier = 0.6f;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
    }
  }
}
