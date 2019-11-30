// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.ExecutionEngine
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.AI
{
  internal class ExecutionEngine : ITMBehaviourExecutionEngine
  {
    private Stack<BehaviourTreeNode> ExecutionStack = new Stack<BehaviourTreeNode>();
    private ITMWorld world;
    private BehaviourTree tree;

    ITMWorld ITMBehaviourExecutionEngine.World
    {
      get
      {
        return this.world;
      }
    }

    BehaviourTree ITMBehaviourExecutionEngine.Tree
    {
      get
      {
        return this.tree;
      }
    }

    public void SetNode(BehaviourTreeNode node)
    {
      this.ExecutionStack.Clear();
      this.AddNode(node);
    }

    public void AddNode(BehaviourTreeNode node)
    {
      if (node == null)
        return;
      node.Status = BehaviourTreeNodeStatus.Running;
      this.ExecutionStack.Push(node);
    }

    public void Update(ITMWorld world, BehaviourTree tree)
    {
      if (this.ExecutionStack.Count <= 0)
        return;
      this.world = world;
      this.tree = tree;
      BehaviourTreeNode behaviourTreeNode = this.ExecutionStack.Peek();
      if (behaviourTreeNode.CanExecute)
        behaviourTreeNode.Update((ITMBehaviourExecutionEngine) this);
      behaviourTreeNode.NPC.LastNode = behaviourTreeNode;
      if (behaviourTreeNode is ExitNode)
      {
        if (!behaviourTreeNode.IsEnabled)
          return;
        while (this.ExecutionStack.Count > 0 && !this.ExecutionStack.Peek().CanExecute)
          this.ExecutionStack.Pop();
      }
      else
      {
        if (!behaviourTreeNode.IsComplete)
          return;
        if (behaviourTreeNode.NPC.BehaviourTree != tree)
        {
          this.SetNode(tree.Root as BehaviourTreeNode);
        }
        else
        {
          BehaviourTreeNode node;
          if (behaviourTreeNode.IsEnabled && behaviourTreeNode.Status == BehaviourTreeNodeStatus.Success)
          {
            node = behaviourTreeNode.FirstChild as BehaviourTreeNode;
            if (node == null && behaviourTreeNode.Continue)
            {
              node = behaviourTreeNode.NextSibling as BehaviourTreeNode;
              this.ExecutionStack.Pop();
            }
          }
          else
          {
            node = behaviourTreeNode.NextSibling as BehaviourTreeNode;
            this.ExecutionStack.Pop();
          }
          if (node == null)
          {
            if (behaviourTreeNode.Status == BehaviourTreeNodeStatus.Failure || behaviourTreeNode.Continue)
            {
              while ((node == null || !node.CanExecute) && (this.ExecutionStack.Count > 0 && !this.ExecutionStack.Peek().CanExecute))
                node = this.ExecutionStack.Pop().NextSibling as BehaviourTreeNode;
            }
            else
            {
              while (this.ExecutionStack.Count > 0 && !this.ExecutionStack.Peek().CanExecute)
                this.ExecutionStack.Pop();
            }
          }
          if (node == null)
            return;
          this.AddNode(node);
        }
      }
    }
  }
}
