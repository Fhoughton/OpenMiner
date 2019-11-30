// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.DialogHandler
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class DialogHandler
  {
    public const float IndirectSpeechScale = 0.6f;
    public const float DirectSpeechScale = 0.6f;
    public const float SpeechMenuScale = 0.6f;
    public const float SpeechMenuNameScale = 0.7f;
    public Player Player;
    public NpcBase Npc;
    public DialogNode CurrentDialog;
    public DialogNode CurrentDialogFirstChild;
    public int SelectedNodeIndex;
    public bool CanTalkCached;
    public string ReticleText;
    public string SpeechText;
    public Rectangle ReticleTextRect;
    public Rectangle SpeechTextRect;
    public Rectangle SpeechMenuRect;
    public string SpeechTextNpcName;
    private Stack<int> dialogStack;
    private BehaviourTree dialogTree;
    private bool menuButtonTemp;
    private float indirectSpeechTimer;
    private int currentNodeChildCount;
    private Actor lastActorInReticle;
    private NpcBase indirectSpeechNpc;
    private DialogHandler.Engine engine;

    private DialogHandler.Engine GetEngine(ITMWorld world, BehaviourTree tree)
    {
      if (this.engine == null)
        this.engine = new DialogHandler.Engine()
        {
          Tree = tree,
          World = world
        };
      this.engine.Tree = tree;
      return this.engine;
    }

    public bool IsActive
    {
      get
      {
        if (this.CurrentDialog == null && this.SpeechText == null)
          return this.CanTalkToNPC;
        return true;
      }
    }

    public bool InConversation
    {
      get
      {
        if (this.CurrentDialog == null)
          return (double) this.indirectSpeechTimer > 0.0;
        return true;
      }
    }

    public bool CanTalkToNPC
    {
      get
      {
        if (this.Npc == null)
          return false;
        BehaviourTree behaviourTree = this.Npc.DialogTree;
        if (behaviourTree == null)
        {
          if (this.Npc.SpawnBlock == null || this.Npc.SpawnBlock.DialogTree == null)
            return false;
          behaviourTree = Globals1.GetBehaviour(BehaviourTreeType.Dialog, this.Npc.SpawnBlock.DialogTree);
        }
        if (behaviourTree != null)
        {
          BehaviourTreeNode behaviourTreeNode = behaviourTree.Root as BehaviourTreeNode;
          if (behaviourTreeNode != null)
          {
            if (!(behaviourTreeNode is DialogNode))
            {
              BehaviourTree tree = behaviourTree.Clone((INPCBehaviour) this.Npc);
              this.Npc.CurrentDialogTarget = (INPCBehaviour) this.Player;
              behaviourTreeNode = this.FindDialogNodeExecutingOtherNodesOnTheWay(tree, tree.Root as BehaviourTreeNode, (INPCBehaviour) this.Npc, true);
            }
            if (behaviourTreeNode != null)
              return StudioForge.Engine.Core.Node.FindFirstChild(typeof (DialogNode), (StudioForge.Engine.Core.Node) behaviourTreeNode) is DialogNode;
            return false;
          }
        }
        return false;
      }
    }

    public bool DrawIndirectSpeechBackButton
    {
      get
      {
        if ((double) this.indirectSpeechTimer > 3.0 && this.CurrentDialog == null)
          return this.SpeechTextNpcName.IsNotEmpty();
        return false;
      }
    }

    public DialogNode CurrentDialogParent
    {
      get
      {
        return StudioForge.Engine.Core.Node.GetParent(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialog) as DialogNode;
      }
    }

    public DialogHandler(Player player)
    {
      this.Player = player;
    }

    public bool HandleInput()
    {
      if (this.CurrentDialog == null)
      {
        if (this.Npc != null && InputManager1.IsInputReleasedNew(this.Player.PlayerIndex, PlayerInput.Interact))
        {
          if (this.dialogStack == null)
            this.dialogStack = new Stack<int>();
          else
            this.dialogStack.Clear();
          this.SelectedNodeIndex = 0;
          this.CurrentDialog = this.FindFirstDialogNodeExecutingOtherNodesOnTheWay() as DialogNode;
          if (this.CurrentDialog != null)
          {
            this.CurrentDialogFirstChild = this.CurrentDialog.FirstChild as DialogNode;
            if (this.CurrentDialogFirstChild == null && this.CurrentDialog.FirstChild != null && !(this.CurrentDialog.FirstChild is DialogNode))
              this.CurrentDialogFirstChild = this.FindDialogChildExecutingOtherNodesOnTheWay((BehaviourTreeNode) this.CurrentDialog) as DialogNode;
            this.BuildDirectSpeechCache(this.CurrentDialog.Text);
            this.Npc.EnteredDirectDialog(this.Player, this.CurrentDialog);
          }
          return this.menuButtonTemp = true;
        }
      }
      else
      {
        if (InputManager1.IsInputReleasedNew(this.Player.PlayerIndex, PlayerInput.BackButton) || InputManager.IsMouseButtonReleasedNew(this.Player.PlayerIndex, StudioForge.Engine.Integration.MouseButtons.RightButton))
        {
          DialogNode currentDialogParent = this.CurrentDialogParent;
          if (currentDialogParent == null || !currentDialogParent.DisableBackButton)
            this.BackButtonPressed();
          return this.menuButtonTemp = true;
        }
        if (InputManager1.IsInputPressedNew(this.Player.PlayerIndex, GuiInput.CursorUp))
        {
          int siblingCount = StudioForge.Engine.Core.Node.GetSiblingCount(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialogFirstChild);
          if (--this.SelectedNodeIndex < 0)
            this.SelectedNodeIndex = siblingCount;
          return this.menuButtonTemp = true;
        }
        if (InputManager1.IsInputPressedNew(this.Player.PlayerIndex, GuiInput.CursorDown))
        {
          if (++this.SelectedNodeIndex > StudioForge.Engine.Core.Node.GetSiblingCount(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialogFirstChild))
            this.SelectedNodeIndex = 0;
          return this.menuButtonTemp = true;
        }
        if (InputManager1.IsInputReleasedNew(this.Player.PlayerIndex, GuiInput.SelectItem) || InputManager.IsMouseButtonReleasedNew(this.Player.PlayerIndex, StudioForge.Engine.Integration.MouseButtons.RightButton))
        {
          this.SelectDialogButtonPressed();
          return this.menuButtonTemp = true;
        }
        int num = (int) ((double) InputManager.GetMouseWheelDelta(this.Player.PlayerIndex) * 0.00999999977648258);
        if (num != 0)
        {
          int siblingCount = StudioForge.Engine.Core.Node.GetSiblingCount(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialogFirstChild);
          this.SelectedNodeIndex -= num;
          if (this.SelectedNodeIndex < 0)
            this.SelectedNodeIndex = 0;
          else if (this.SelectedNodeIndex > siblingCount)
            this.SelectedNodeIndex = siblingCount;
          return this.menuButtonTemp = true;
        }
      }
      bool menuButtonTemp = this.menuButtonTemp;
      if (InputManager1.IsInputReleased(this.Player.PlayerIndex, PlayerInput.Interact) && InputManager1.IsInputReleased(this.Player.PlayerIndex, PlayerInput.BackButton) && (InputManager1.IsInputReleased(this.Player.PlayerIndex, GuiInput.CursorUp) && InputManager1.IsInputReleased(this.Player.PlayerIndex, GuiInput.CursorDown)) && (InputManager.IsMouseButtonReleased(this.Player.PlayerIndex, StudioForge.Engine.Integration.MouseButtons.LeftButton) && InputManager.IsMouseButtonReleased(this.Player.PlayerIndex, StudioForge.Engine.Integration.MouseButtons.RightButton)))
        this.menuButtonTemp = false;
      if (!menuButtonTemp)
        return this.menuButtonTemp;
      return true;
    }

    public void Update()
    {
      if (this.CurrentDialog != null)
        return;
      if ((double) this.indirectSpeechTimer > 0.0)
      {
        this.indirectSpeechTimer -= Services.ElapsedTime;
        if ((double) this.indirectSpeechTimer <= 2.0)
          this.ClearIndirectSpeech();
      }
      this.BuildReticleCache();
    }

    private void ClearIndirectSpeech()
    {
      this.indirectSpeechTimer = 0.0f;
      this.SpeechText = (string) null;
      this.SpeechTextNpcName = (string) null;
      if (this.indirectSpeechNpc == null)
        return;
      this.indirectSpeechNpc.StoppedTalking();
      this.indirectSpeechNpc = (NpcBase) null;
    }

    private void SelectDialogButtonPressed()
    {
      this.CurrentDialog.DisableBackButton = false;
      if (this.CurrentDialogFirstChild != null)
      {
        DialogNode nextSibling = StudioForge.Engine.Core.Node.FindNextSibling(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialogFirstChild, this.SelectedNodeIndex) as DialogNode;
        if (nextSibling != null && (!nextSibling.IsRead || !nextSibling.DisableOnceRead))
        {
          nextSibling.IsRead = true;
          nextSibling.UpdateFromHandler((ITMPlayer) this.Player);
          BehaviourTreeNode otherNodesOnTheWay1 = this.FindDialogChildExecutingOtherNodesOnTheWay((BehaviourTreeNode) nextSibling);
          if (otherNodesOnTheWay1 != null)
          {
            DialogNode dialogNode = otherNodesOnTheWay1 as DialogNode;
            if (dialogNode != null)
            {
              this.dialogStack.Push(this.SelectedNodeIndex);
              this.SelectedNodeIndex = 0;
              this.CurrentDialog = dialogNode;
              dialogNode.UpdateFromHandler((ITMPlayer) this.Player);
              this.BuildNpcSpeechTextCache(this.Npc, this.CurrentDialog.Text);
              this.Npc.EnteredDirectDialog(this.Player, this.CurrentDialog);
              BehaviourTreeNode otherNodesOnTheWay2 = this.FindDialogChildExecutingOtherNodesOnTheWay((BehaviourTreeNode) this.CurrentDialog);
              if (otherNodesOnTheWay2 is ExitNode)
              {
                this.EndConversation();
                return;
              }
              this.CurrentDialogFirstChild = otherNodesOnTheWay2 as DialogNode;
              return;
            }
            if (otherNodesOnTheWay1 is ExitNode)
            {
              this.EndConversation();
              return;
            }
          }
        }
      }
      this.BackButtonPressed();
    }

    private void BackButtonPressed()
    {
      do
      {
        this.CurrentDialog = StudioForge.Engine.Core.Node.GetParent(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialog) as DialogNode;
        if (this.CurrentDialog != null)
          this.CurrentDialogFirstChild = this.GetDialogChild(this.CurrentDialog);
        else
          break;
      }
      while (this.CurrentDialog.MustGoBack);
      if (this.CurrentDialog != null)
      {
        this.CurrentDialog = StudioForge.Engine.Core.Node.GetParent(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialog) as DialogNode;
        this.CurrentDialogFirstChild = this.GetDialogChild(this.CurrentDialog);
      }
      if (this.CurrentDialog == null)
      {
        this.EndConversation();
      }
      else
      {
        if (this.Npc == null)
          return;
        this.SelectedNodeIndex = this.dialogStack.Pop();
        this.BuildDirectSpeechCache((string) null);
        this.Npc.EnteredDirectDialog(this.Player, this.CurrentDialog);
      }
    }

    private DialogNode GetDialogChild(DialogNode dialog)
    {
      if (dialog == null)
        return dialog;
      return this.FindDialogNodeExecutingOtherNodesOnTheWay(this.dialogTree, dialog.FirstChild as BehaviourTreeNode, (INPCBehaviour) this.Npc, false) as DialogNode;
    }

    public void EndConversation()
    {
      if (this.Npc != null)
        this.Npc.StoppedTalking();
      this.CurrentDialog = (DialogNode) null;
      this.dialogTree = (BehaviourTree) null;
    }

    public DialogNode FindOpeningLine(NpcBase npc)
    {
      if (npc != null)
      {
        BehaviourTree behaviourTree = this.LoadDialog(npc, false);
        if (behaviourTree != null)
        {
          BehaviourTreeNode behaviourTreeNode = behaviourTree.Root as BehaviourTreeNode;
          if (behaviourTreeNode != null)
          {
            if (!(behaviourTreeNode is DialogNode))
            {
              BehaviourTree tree = behaviourTree.Clone((INPCBehaviour) npc);
              behaviourTreeNode = this.FindDialogNodeExecutingOtherNodesOnTheWay(tree, tree.Root as BehaviourTreeNode, (INPCBehaviour) npc, false);
            }
            if (behaviourTreeNode != null)
            {
              int siblingCount = StudioForge.Engine.Core.Node.GetSiblingCount(typeof (DialogNode), (StudioForge.Engine.Core.Node) behaviourTreeNode);
              if (siblingCount > 0)
                behaviourTreeNode = StudioForge.Engine.Core.Node.FindPrevSibling(typeof (DialogNode), (StudioForge.Engine.Core.Node) behaviourTreeNode, this.Player.GameInstance.Random.Next(siblingCount + 1)) as BehaviourTreeNode;
              DialogNode dialogNode = behaviourTreeNode as DialogNode;
              dialogNode?.UpdateFromHandler((ITMPlayer) this.Player);
              return dialogNode;
            }
          }
        }
      }
      return (DialogNode) null;
    }

    private BehaviourTreeNode FindFirstDialogNodeExecutingOtherNodesOnTheWay()
    {
      if (this.dialogTree == null)
        this.dialogTree = this.LoadDialog(this.Npc, true);
      if (this.dialogTree != null)
        return this.FindDialogNodeExecutingOtherNodesOnTheWay(this.dialogTree, this.dialogTree.Root as BehaviourTreeNode, (INPCBehaviour) this.Npc);
      return (BehaviourTreeNode) null;
    }

    private BehaviourTree LoadDialog(NpcBase npc, bool clone)
    {
      BehaviourTree behaviourTree = npc.DialogTree;
      if (behaviourTree == null && npc.SpawnBlock != null)
      {
        if (npc.SpawnBlock.DialogText.IsNotEmpty())
        {
          if (npc.SpawnBlock.DialogTextCache == null)
          {
            npc.SpawnBlock.DialogTextCache = new BehaviourTree(BehaviourTreeType.Dialog, true);
            DialogNode dialogNode = new DialogNode();
            dialogNode.Text = npc.SpawnBlock.DialogText;
            dialogNode.SetNPC((INPCBehaviour) npc);
            npc.SpawnBlock.DialogTextCache.AddChild((StudioForge.Engine.Core.Node) dialogNode);
          }
          behaviourTree = npc.SpawnBlock.DialogTextCache;
        }
        else
        {
          BehaviourTree behaviour = Globals1.GetBehaviour(BehaviourTreeType.Dialog, npc.SpawnBlock.DialogTree);
          if (behaviour != null)
            behaviourTree = clone ? behaviour.Clone((INPCBehaviour) npc) : behaviour;
        }
      }
      return behaviourTree;
    }

    private BehaviourTreeNode FindDialogChildExecutingOtherNodesOnTheWay(
      BehaviourTreeNode node)
    {
      return this.FindDialogNodeExecutingOtherNodesOnTheWay(this.dialogTree, (BehaviourTreeNode) node?.FirstChild, (INPCBehaviour) this.Npc, true);
    }

    private BehaviourTreeNode FindDialogNodeExecutingOtherNodesOnTheWay(
      BehaviourTree tree,
      BehaviourTreeNode node,
      INPCBehaviour npc)
    {
      return this.FindDialogNodeExecutingOtherNodesOnTheWay(tree, node, npc, true);
    }

    private BehaviourTreeNode FindDialogNodeExecutingOtherNodesOnTheWay(
      BehaviourTree tree,
      BehaviourTreeNode node,
      INPCBehaviour npc,
      bool executeScripts)
    {
      for (; node != null; node = node.Status == BehaviourTreeNodeStatus.Success ? node.FirstChild as BehaviourTreeNode : node.NextSibling as BehaviourTreeNode)
      {
        if (node is DialogNode || node is ExitNode)
          return node;
        ScriptNode scriptNode = node as ScriptNode;
        if (scriptNode != null)
        {
          if (executeScripts)
            scriptNode.ExecuteScript(npc, (INPCBehaviour) this.Player, (Action<Script, Actor>) null);
          node.Status = BehaviourTreeNodeStatus.Success;
        }
        else
          node.Update((ITMBehaviourExecutionEngine) this.GetEngine((ITMWorld) this.Player.GameInstance, tree));
      }
      return (BehaviourTreeNode) null;
    }

    private void BuildReticleCache()
    {
      if (this.Player.ActorInReticle == this.lastActorInReticle)
        return;
      this.lastActorInReticle = this.Player.ActorInReticle;
      this.Npc = this.lastActorInReticle as NpcBase;
      if (this.Npc != null && this.Npc.Name != "" && this.Npc.Name != "<Unnamed>")
      {
        this.ReticleText = this.Npc.Name;
        this.CanTalkCached = this.CanTalkToNPC;
        if (this.CanTalkCached)
          this.ReticleText += ":    Talk";
        Vector2 vector2 = CoreGlobals.GameFont.MeasureString(this.ReticleText) * 0.6f;
        this.ReticleTextRect.X = (int) ((double) (this.Player.Viewport.Width / 2) - (double) vector2.X / 2.0 - 30.0);
        this.ReticleTextRect.Y = 396;
        this.ReticleTextRect.Width = (int) vector2.X + 60;
        this.ReticleTextRect.Height = (int) vector2.Y + 10;
        if (this.SpeechText == null || this.ReticleTextRect.Y + this.ReticleTextRect.Height <= this.SpeechTextRect.Y - 20)
          return;
        this.ReticleTextRect.Y = Math.Max(GraphicStatics.HUDPos().Y + 4, Math.Min(290, this.SpeechTextRect.Y - this.ReticleTextRect.Height - 16));
      }
      else
        this.ReticleText = (string) null;
    }

    private void BuildNpcSpeechTextCache(NpcBase npc, string text)
    {
      Rectangle rectangle = GraphicStatics.HUDPos();
      text = Globals2.SubstituteGamertag(text, this.Player);
      this.SpeechText = Utils.InsertNewLines(CoreGlobals.GameFont, rectangle.Width - 560, 0.6f, text, true);
      if (this.SpeechText == null)
        return;
      Vector2 vector2 = CoreGlobals.GameFont.MeasureString(this.SpeechText) * 0.6f;
      this.SpeechTextRect.X = (int) ((double) (this.Player.Viewport.Width / 2) - (double) vector2.X / 2.0 - 60.0);
      this.SpeechTextRect.Width = (int) vector2.X + 120;
      this.SpeechTextRect.Height = (int) vector2.Y + 16;
      this.SpeechTextRect.Y = (int) ((double) (rectangle.Height - 528) * 0.5 + 438.0 - (double) this.SpeechTextRect.Height * 0.5);
      if (this.SpeechTextRect.Y + this.SpeechTextRect.Height > rectangle.Height - 112)
        this.SpeechTextRect.Y = rectangle.Height - 112 - this.SpeechTextRect.Height;
      if (this.ReticleTextRect.Y + this.ReticleTextRect.Height > this.SpeechTextRect.Y - 20)
        this.ReticleTextRect.Y = Math.Max(GraphicStatics.HUDPos().Y + 4, Math.Min(290, this.SpeechTextRect.Y - this.ReticleTextRect.Height - 16));
      this.SpeechTextNpcName = npc.Name;
      this.indirectSpeechTimer = (float) Math.Max(4, this.SpeechText.Length / 15);
      this.indirectSpeechNpc = npc;
    }

    private void BuildDirectSpeechCache(string text)
    {
      this.SpeechMenuRect = GraphicStatics.HUDPos();
      this.SpeechMenuRect.Y += 200;
      this.SpeechMenuRect.Height -= 400;
      int num1 = this.SpeechMenuRect.Height / 2 + this.SpeechMenuRect.Y;
      this.SpeechMenuRect.Height = 174;
      this.currentNodeChildCount = StudioForge.Engine.Core.Node.GetChildCount(typeof (DialogNode), (StudioForge.Engine.Core.Node) this.CurrentDialog);
      if (this.currentNodeChildCount == 0)
        this.currentNodeChildCount = 1;
      this.SpeechMenuRect.Y = num1 - this.SpeechMenuRect.Height / 2;
      this.BuildNpcSpeechTextCache(this.Npc, text);
      int num2 = this.SpeechMenuRect.Y + this.SpeechMenuRect.Height + 20;
      if (this.SpeechTextRect.Y <= num2)
        return;
      this.SpeechTextRect.Y = (int) ((double) (this.SpeechTextRect.Y + this.SpeechTextRect.Height - num2) * 0.5 + (double) num2 - (double) this.SpeechTextRect.Height * 0.5);
    }

    public void NpcSaidSomething(NpcBase npc, DialogNode node)
    {
      if (npc == null || npc.SpawnBlock == null || (node == null || !node.Text.IsNotEmpty()) || (this.SpeechText != null || (double) this.indirectSpeechTimer > 0.0))
        return;
      this.BuildNpcSpeechTextCache(npc, npc.Name + ": " + node.Text);
    }

    private class Engine : ITMBehaviourExecutionEngine
    {
      public ITMWorld World { get; set; }

      public BehaviourTree Tree { get; set; }

      public void AddNode(BehaviourTreeNode node)
      {
      }
    }
  }
}
