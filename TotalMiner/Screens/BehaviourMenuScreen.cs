// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.BehaviourMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class BehaviourMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private NpcSpawnBlock block;

    public BehaviourMenuScreen(GameInstance instance, Player player, NpcSpawnBlock block)
      : base("Behaviour Menu", player)
    {
      this.instance = instance;
      this.block = block;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      if (block != null)
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Use Behaviour"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "New Behaviour"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Edit Behaviour"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "----------------"));
      if (block != null)
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Use Dialog"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "New Dialog"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Edit Dialog"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "----------------"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Close"));
      int num1 = 0;
      if (block != null)
        blockMenuEntryList1[num1++].Selected += new EventHandler<PlayerIndexEventArgs>(this.UseBehaviourMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.NewBehaviourMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.EditBehaviourMenuEntrySelected);
      int num4 = num3 + 1;
      if (block != null)
        blockMenuEntryList1[num4++].Selected += new EventHandler<PlayerIndexEventArgs>(this.UseDialogMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num4;
      int num5 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.NewDialogMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num5;
      int num6 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.EditDialogMenuEntrySelected);
      int num7 = num6 + 1;
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 376;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void UseBehaviourMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BehaviourListMenuScreen(this.instance, this.player, BehaviourTreeType.AI, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnBehaviourSelectedForUse)), this.ControllingPlayer);
    }

    private void UseDialogMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BehaviourListMenuScreen(this.instance, this.player, BehaviourTreeType.Dialog, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnDialogSelectedForUse)), this.ControllingPlayer);
    }

    private void NewBehaviourMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      new BehaviourTree(BehaviourTreeType.AI, false).Name = "Behaviour" + (object) Globals1.BehaviourTrees.Count + (object) 1;
    }

    private void NewDialogMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      new BehaviourTree(BehaviourTreeType.Dialog, false).Name = "Dialog" + (object) Globals1.BehaviourTrees.Count + (object) 1;
    }

    private void EditBehaviourMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BehaviourListMenuScreen(this.instance, this.player, BehaviourTreeType.AI, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnBehaviourSelectedForEdit), (Action) null, true, false), this.ControllingPlayer);
    }

    private void EditDialogMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BehaviourListMenuScreen(this.instance, this.player, BehaviourTreeType.Dialog, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnDialogSelectedForEdit), (Action) null, true, false), this.ControllingPlayer);
    }

    private bool OnBehaviourSelectedForUse(MenuEntry item)
    {
      string str = (string) item.Tag + item.Text;
      this.block.BehaviourTree = str != "None" ? str : (string) null;
      this.ExitScreen();
      return true;
    }

    private bool OnDialogSelectedForUse(MenuEntry item)
    {
      string str = (string) item.Tag + item.Text;
      this.block.DialogTree = str != "None" ? str : (string) null;
      this.ExitScreen();
      return true;
    }

    private bool OnBehaviourSelectedForEdit(MenuEntry item)
    {
      BehaviourTree behaviour = Globals1.GetBehaviour(BehaviourTreeType.AI, (string) item.Tag + item.Text);
      if (behaviour == null)
        return false;
      behaviour.Clone((INPCBehaviour) null);
      return true;
    }

    private bool OnDialogSelectedForEdit(MenuEntry item)
    {
      BehaviourTree behaviour = Globals1.GetBehaviour(BehaviourTreeType.Dialog, (string) item.Tag + item.Text);
      if (behaviour == null)
        return false;
      behaviour.Clone((INPCBehaviour) null);
      return true;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
