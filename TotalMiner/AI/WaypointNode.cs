// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.WaypointNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Waypoint", BehaviourTreeNodeType.Action)]
  internal class WaypointNode : MoveNode
  {
    public List<Vector3> Waypoints = new List<Vector3>();
    private int currentWaypoint = -1;
    public WaypointType WaypointType;
    public int WanderTime;
    public float WanderDistance;
    private bool backEnumeration;
    private int currentWanderTime;
    private long wanderTimer;
    private WanderNode wanderNode;
    private Vector3 nextPosition;

    public WaypointNode()
    {
    }

    public WaypointNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    public void AddWaypoint(Vector3 v)
    {
      if (this.Waypoints == null)
        this.Waypoints = new List<Vector3>();
      this.Waypoints.Add(v);
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
        else if (this.wanderTimer > 0L)
        {
          this.wanderNode.Update(engine);
          if ((int) (Globals1.ElapsedWatch.ElapsedMilliseconds - this.wanderTimer) >= this.currentWanderTime)
          {
            this.Position = this.nextPosition;
            this.wanderTimer = 0L;
          }
        }
        else
        {
          if (this.currentWaypoint < 0)
            this.SetNextPosition();
          this.npc.LookAt(CoordType.Absolute, this.Position, false);
          base.UpdateCore(engine);
          if (!this.isMoving)
            this.SetNextPosition();
        }
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    private void SetNextPosition()
    {
      if (this.backEnumeration)
      {
        if (this.currentWaypoint > 0)
        {
          --this.currentWaypoint;
        }
        else
        {
          this.backEnumeration = false;
          this.currentWaypoint = 1;
        }
      }
      else if (++this.currentWaypoint >= this.Waypoints.Count)
      {
        if (this.WaypointType == WaypointType.Backtrack)
        {
          this.currentWaypoint = this.Waypoints.Count - 1;
          this.backEnumeration = true;
        }
        else
          this.currentWaypoint = 0;
      }
      this.nextPosition = this.Waypoints[this.currentWaypoint];
      if (this.WanderTime > 0)
      {
        int max = this.WanderTime / 2;
        this.currentWanderTime = GameInstance.Instance.Random.Next(max) + max;
        if ((double) this.currentWanderTime >= 2.0)
        {
          if (this.wanderNode == null)
          {
            WanderNode wanderNode = new WanderNode(this.npc);
            wanderNode.CanJump = true;
            wanderNode.Distance = this.WanderDistance;
            wanderNode.PositionType = CoordType.Absolute;
            wanderNode.VelocityModifier = this.VelocityModifier;
            this.wanderNode = wanderNode;
          }
          this.wanderNode.Position = this.npc.Position;
          this.wanderTimer = Globals1.ElapsedWatch.ElapsedMilliseconds;
        }
        else
          this.Position = this.nextPosition;
      }
      else
      {
        this.Position = this.nextPosition;
        this.SetNextMoveType();
      }
    }

    protected override string ToStringCore(string propertyName, object data)
    {
      if (!(propertyName == "Waypoints"))
        return base.ToStringCore(propertyName, data);
      StringBuilder stringBuilder = new StringBuilder();
      if (this.Waypoints != null)
      {
        for (int index = 0; index < this.Waypoints.Count; ++index)
        {
          Vector3 waypoint = this.Waypoints[index];
          stringBuilder.AppendFormat("[{0},{1},{2}]", (object) waypoint.X, (object) waypoint.Y, (object) waypoint.Z);
          if (index < this.Waypoints.Count - 1)
            stringBuilder.Append(", ");
        }
      }
      return stringBuilder.ToString();
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.WaypointType = (WaypointType) reader.ReadByte();
      this.Waypoints.Clear();
      ushort num = reader.ReadUInt16();
      for (int index = 0; index < (int) num; ++index)
        this.Waypoints.Add(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
      this.WanderTime = reader.ReadInt32();
      this.WanderDistance = reader.ReadSingle();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.WaypointType);
      writer.Write((ushort) this.Waypoints.Count);
      foreach (Vector3 waypoint in this.Waypoints)
      {
        writer.Write(waypoint.X);
        writer.Write(waypoint.Y);
        writer.Write(waypoint.Z);
      }
      writer.Write(this.WanderTime);
      writer.Write(this.WanderDistance);
    }
  }
}
