// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.WaypointSystem`1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public class WaypointSystem<TNode>
  {
    private int currentWaypoint;
    private int waypointDirection;
    private List<TNode> waypoints;
    private bool isActive;
    private bool isRepeat;

    public WaypointSystem()
    {
      this.isActive = false;
      this.isRepeat = false;
      this.currentWaypoint = 0;
      this.waypointDirection = 1;
      this.waypoints = new List<TNode>();
    }

    public TNode[] Waypoints
    {
      get
      {
        return this.waypoints.ToArray();
      }
    }

    public TNode CurrentWaypoint
    {
      get
      {
        return this.waypoints[this.currentWaypoint];
      }
    }

    public bool IsActive
    {
      get
      {
        return this.isActive;
      }
    }

    public bool IsRepeat
    {
      get
      {
        return this.isRepeat;
      }
      set
      {
        this.isRepeat = value;
      }
    }

    public int Count
    {
      get
      {
        return this.waypoints.Count;
      }
    }

    public void AddWaypoint(TNode wp)
    {
      this.waypoints.Add(wp);
      this.isActive = true;
    }

    public void ClearWaypoints()
    {
      this.waypoints.Clear();
      this.waypointDirection = 1;
      this.currentWaypoint = 0;
      this.isActive = false;
    }

    public void MoveNext()
    {
      if (this.waypoints.Count > 1)
      {
        int num = this.currentWaypoint + this.waypointDirection;
        if (num < 0 || num >= this.waypoints.Count)
        {
          if (this.isRepeat)
          {
            this.waypointDirection = -this.waypointDirection;
            num = this.currentWaypoint + this.waypointDirection;
          }
          else
            this.isActive = false;
        }
        this.currentWaypoint = num;
      }
      else
      {
        this.currentWaypoint = 0;
        this.isActive = false;
      }
    }

    public bool Contains(TNode node)
    {
      return this.waypoints.Contains(node);
    }
  }
}
