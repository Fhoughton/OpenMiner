// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.AStarPath
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public static class AStarPath
  {
    public static APath<Node> FindPath<Node>(
      Node start,
      Node destination,
      Func<Node, Node, double> distance,
      Func<Node, double> estimate)
      where Node : IHasNeighbours<Node>
    {
      HashSet<Node> nodeSet = new HashSet<Node>();
      PriorityQueue<double, APath<Node>> priorityQueue = new PriorityQueue<double, APath<Node>>();
      priorityQueue.Enqueue(0.0, new APath<Node>(start));
      while (!priorityQueue.IsEmpty)
      {
        APath<Node> apath1 = priorityQueue.Dequeue();
        if (!nodeSet.Contains(apath1.LastStep))
        {
          if (apath1.LastStep.Equals((object) destination))
            return apath1;
          nodeSet.Add(apath1.LastStep);
          if (apath1.LastStep.Neighbours != null)
          {
            foreach (Node neighbour in apath1.LastStep.Neighbours)
            {
              double stepCost = distance(apath1.LastStep, neighbour);
              APath<Node> apath2 = apath1.AddStep(neighbour, stepCost);
              priorityQueue.Enqueue(apath2.TotalCost + estimate(neighbour), apath2);
            }
          }
        }
      }
      return (APath<Node>) null;
    }
  }
}
