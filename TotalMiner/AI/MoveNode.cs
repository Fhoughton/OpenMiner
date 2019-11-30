// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.MoveNode
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.GUI;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Move", BehaviourTreeNodeType.Action)]
  public class MoveNode : BehaviourTreeNode
  {
    public float Distance = 0.2f;
    public float VelocityModifier = 1f;
    public bool CanJump = true;
    public Vector3 Position;
    public CoordType PositionType;
    public bool Instant;
    protected float distanceSqFromPosition;
    protected bool isMoving;
    protected MoveType moveType;

    public override string ToStringParms
    {
      get
      {
        return this.ShortPosType(this.PositionType) + " " + this.Distance.ToString();
      }
    }

    public MoveNode()
    {
    }

    public MoveNode(INPCBehaviour npc)
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
        if (this.npc.CurrentDialog != null && this.npc.CurrentDialog.StopMoving)
        {
          this.npc.StandStill();
          this.npc.LookAt(CoordType.Absolute, this.npc.CurrentDialogTarget.EyePosition, false);
        }
        else
        {
          Vector3 finalPosition = this.npc.GetFinalPosition(this.PositionType, this.Position);
          if (this.Instant)
          {
            this.npc.TeleportTo(finalPosition);
            this.distanceSqFromPosition = 0.0f;
            this.isMoving = false;
          }
          else
          {
            this.distanceSqFromPosition = Vector2.DistanceSquared(new Vector2(this.npc.Position.X, this.npc.Position.Z), new Vector2(finalPosition.X, finalPosition.Z));
            if ((double) this.distanceSqFromPosition > (double) this.Distance * (double) this.Distance)
            {
              this.npc.LookAt(CoordType.Absolute, finalPosition, false);
              this.npc.MoveTo(finalPosition, this.VelocityModifier, this.CanJump, this.moveType);
              this.isMoving = true;
            }
            else
            {
              this.npc.StandStill();
              this.isMoving = false;
            }
          }
        }
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    protected void SetNextMoveType()
    {
      this.moveType = this.npc.Properties.MoveType.HasValue ? this.npc.Properties.MoveType.Value : MoveType.Default;
      if (!this.npc.Properties.MoveTypePercent.HasValue || this.npc.Random.Next(100) < (int) this.npc.Properties.MoveTypePercent.Value)
        return;
      this.moveType = MoveType.Default;
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "Distance":
          win.SetToolTip("Once the NPC is within this distance of the destination, it will stop moving");
          break;
        case "VelocityModifier":
          win.SetToolTip("This value is multiplied against the NPCs current velocity to calculate a final velocity. A value < 1 will slow the NPC down. A value > 1 will speed the NPC up");
          break;
        case "Instant":
          win.SetToolTip("If TRUE the NPC will move to the destination instantly (like a teleport).");
          break;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Position.X = reader.ReadSingle();
      this.Position.Y = reader.ReadSingle();
      this.Position.Z = reader.ReadSingle();
      this.Distance = reader.ReadSingle();
      this.PositionType = (CoordType) reader.ReadByte();
      this.Instant = reader.ReadBoolean();
      this.VelocityModifier = reader.ReadSingle();
      this.CanJump = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Position.X);
      writer.Write(this.Position.Y);
      writer.Write(this.Position.Z);
      writer.Write(this.Distance);
      writer.Write((byte) this.PositionType);
      writer.Write(this.Instant);
      writer.Write(this.VelocityModifier);
      writer.Write(this.CanJump);
    }
  }
}
