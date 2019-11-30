// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandQueue
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandQueue : IThreadWorkItem
  {
    private Queue<CreativeOperationData> items = new Queue<CreativeOperationData>();
    private GameInstance instance;
    private PriorityLevel priority;
    private CreativeOperationData currentItem;
    private CreativeCommandFill commandFill;
    private CreativeCommandMove commandMove;
    private CreativeCommandCopy commandCopy;
    private CreativeCommandLine commandLine;
    private CreativeCommandWall commandWall;
    private CreativeCommandTrees commandTrees;
    private CreativeCommandPaste commandPaste;
    private CreativeCommandSphere commandSphere;
    private CreativeCommandReplace commandReplace;
    private CreativeCommandReplaceTexture commandReplaceTexture;
    private CreativeCommandCopyToClipboard commandCopyToClipboard;

    public string Name
    {
      get
      {
        return nameof (CreativeCommandQueue);
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

    public CreativeCommandQueue(GameInstance instance, PriorityLevel priority)
    {
      this.instance = instance;
      this.priority = priority;
    }

    public void UnloadContent()
    {
    }

    public void Execute(CreativeOperationData item, bool threaded)
    {
      if (item == null)
        return;
      if (threaded)
      {
        lock (this.items)
          this.items.Enqueue(new CreativeOperationData(item));
      }
      else
        this.ExecuteImmediateCore(this.GetImmediateItemExecutor(item), item);
    }

    private void ExecuteImmediateCore(CreativeCommandWorkItem exe, CreativeOperationData item)
    {
      if (exe == null)
        return;
      try
      {
        exe.Initialize(item);
        exe.Update();
      }
      finally
      {
        if (item.OnCompletion != null)
          item.OnCompletion(this.currentItem);
      }
    }

    public void Update()
    {
      try
      {
        lock (this.items)
        {
          if (this.items.Count > 0)
            this.currentItem = this.items.Dequeue();
        }
        if (this.currentItem == null)
          return;
        this.ExecuteImmediateCore(this.GetItemExecutor(this.currentItem), this.currentItem);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(45, ex);
      }
      finally
      {
        this.currentItem = (CreativeOperationData) null;
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this, false, this.priority);
      }
    }

    private CreativeCommandWorkItem GetItemExecutor(
      CreativeOperationData item)
    {
      CreativeCommand creativeCommand = item.Command;
      if (creativeCommand == CreativeCommand.Clear && item.BlockID != (byte) 0)
        creativeCommand = CreativeCommand.Replace;
      switch (creativeCommand)
      {
        case CreativeCommand.Clear:
        case CreativeCommand.Fill:
          return (CreativeCommandWorkItem) this.commandFill ?? (CreativeCommandWorkItem) (this.commandFill = new CreativeCommandFill(this.instance));
        case CreativeCommand.Move:
          return (CreativeCommandWorkItem) this.commandMove ?? (CreativeCommandWorkItem) (this.commandMove = new CreativeCommandMove(this.instance));
        case CreativeCommand.Copy:
          return (CreativeCommandWorkItem) this.commandCopy ?? (CreativeCommandWorkItem) (this.commandCopy = new CreativeCommandCopy(this.instance));
        case CreativeCommand.Paste:
          return (CreativeCommandWorkItem) this.commandPaste ?? (CreativeCommandWorkItem) (this.commandPaste = new CreativeCommandPaste(this.instance));
        case CreativeCommand.Replace:
          return (CreativeCommandWorkItem) this.commandReplace ?? (CreativeCommandWorkItem) (this.commandReplace = new CreativeCommandReplace(this.instance));
        case CreativeCommand.ReplaceTexture:
          return (CreativeCommandWorkItem) this.commandReplaceTexture ?? (CreativeCommandWorkItem) (this.commandReplaceTexture = new CreativeCommandReplaceTexture(this.instance));
        case CreativeCommand.CopyToClipboard:
          return (CreativeCommandWorkItem) this.commandCopyToClipboard ?? (CreativeCommandWorkItem) (this.commandCopyToClipboard = new CreativeCommandCopyToClipboard(this.instance));
        case CreativeCommand.Line:
          return (CreativeCommandWorkItem) this.commandLine ?? (CreativeCommandWorkItem) (this.commandLine = new CreativeCommandLine(this.instance));
        case CreativeCommand.Sphere:
          return (CreativeCommandWorkItem) this.commandSphere ?? (CreativeCommandWorkItem) (this.commandSphere = new CreativeCommandSphere(this.instance));
        case CreativeCommand.Path:
        case CreativeCommand.Wall:
          return (CreativeCommandWorkItem) this.commandWall ?? (CreativeCommandWorkItem) (this.commandWall = new CreativeCommandWall(this.instance));
        case CreativeCommand.Trees:
          return (CreativeCommandWorkItem) this.commandTrees ?? (CreativeCommandWorkItem) (this.commandTrees = new CreativeCommandTrees(this.instance));
        default:
          return (CreativeCommandWorkItem) null;
      }
    }

    private CreativeCommandWorkItem GetImmediateItemExecutor(
      CreativeOperationData item)
    {
      CreativeCommand creativeCommand = item.Command;
      if (creativeCommand == CreativeCommand.Clear && item.BlockID != (byte) 0)
        creativeCommand = CreativeCommand.Replace;
      switch (creativeCommand)
      {
        case CreativeCommand.Clear:
        case CreativeCommand.Fill:
          return (CreativeCommandWorkItem) new CreativeCommandFill(this.instance);
        case CreativeCommand.Move:
          return (CreativeCommandWorkItem) new CreativeCommandMove(this.instance);
        case CreativeCommand.Copy:
          return (CreativeCommandWorkItem) new CreativeCommandCopy(this.instance);
        case CreativeCommand.Paste:
          return (CreativeCommandWorkItem) new CreativeCommandPaste(this.instance);
        case CreativeCommand.Replace:
          return (CreativeCommandWorkItem) new CreativeCommandReplace(this.instance);
        case CreativeCommand.ReplaceTexture:
          return (CreativeCommandWorkItem) new CreativeCommandReplaceTexture(this.instance);
        case CreativeCommand.CopyToClipboard:
          return (CreativeCommandWorkItem) new CreativeCommandCopyToClipboard(this.instance);
        case CreativeCommand.Line:
          return (CreativeCommandWorkItem) new CreativeCommandLine(this.instance);
        case CreativeCommand.Sphere:
          return (CreativeCommandWorkItem) new CreativeCommandSphere(this.instance);
        case CreativeCommand.Path:
        case CreativeCommand.Wall:
          return (CreativeCommandWorkItem) new CreativeCommandWall(this.instance);
        case CreativeCommand.Trees:
          return (CreativeCommandWorkItem) new CreativeCommandTrees(this.instance);
        default:
          return (CreativeCommandWorkItem) null;
      }
    }

    public void Draw()
    {
      this.DrawItem(this.currentItem);
      lock (this.items)
      {
        foreach (CreativeOperationData creativeOperationData in this.items)
          this.DrawItem(creativeOperationData);
      }
    }

    private void DrawItem(CreativeOperationData item)
    {
      if (item == null)
        return;
      ProgressOperation progressOperation = new ProgressOperation()
      {
        Desc = item.Desc,
        Progress = item.Progress
      };
      TotalMinerGame.Instance.Operations.Add(progressOperation);
    }
  }
}
