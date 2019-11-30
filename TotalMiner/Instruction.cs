// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Instruction
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner
{
  internal class Instruction : DrawableGameObjectBase
  {
    private float fadeLength = 0.5f;
    private Instruction.Anchor[] anchors = new Instruction.Anchor[30]
    {
      Instruction.Anchor.Center,
      Instruction.Anchor.Center,
      Instruction.Anchor.Center,
      Instruction.Anchor.Center,
      Instruction.Anchor.Center,
      Instruction.Anchor.Center,
      Instruction.Anchor.Top,
      Instruction.Anchor.Top,
      Instruction.Anchor.Top,
      Instruction.Anchor.Top,
      Instruction.Anchor.Center,
      Instruction.Anchor.Center,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.Center,
      Instruction.Anchor.Top,
      Instruction.Anchor.Top,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.RightTop,
      Instruction.Anchor.Center
    };
    private string[] instructionText = new string[31]
    {
      "Hello Gamer! Welcome to GameName!\r\n\r\nI am your guide and I will guide you on the basics of the game\r\nto get you started.\r\n\r\nSurrounding you is a world full of discovery. \r\n\r\nExplore it and you will discover many things both material and \r\nmythological, both precious and priceless.\r\n\r\nYour untold fortune is here to be made!\r\n\r\nPress the A button for me to continue.",
      "At anytime during my rambling, you can press the Right Shoulder\r\nbutton to skip forward to the next instruction.\r\n\r\nAnd pressing the Left Shoulder button will skip back to the previous.\r\n\r\nIf I have disappeared for some reason, and you've forgotten what \r\nI've asked you to do, then press either the Left or Right Shoulder\r\nbuttons to make me reappear.\r\n\r\nPress the Y button at any time to end my instruction.\r\n\r\nPress the A button for me to continue.",
      "Ok, lets get on with it.\r\n\r\nRight now you are standing on the surface somewhere\r\nnear the center of the world.\r\n\r\nNow use the Right Stick to take a look around you.",
      "Looking good. \r\n\r\nNow use the Left Stick to walk around.\r\n\r\nYou'll notice the terrain is blocky. If a block is in your path\r\nwhile walking, it will block you and you'll stop moving. \r\n\r\nIf this happens, press the A button while walking to jump up \r\nand onto any block in your path, so you can continue walking \r\nin that direction.",
      "Walkin good.\r\n\r\nThis is an exploration mining game. The main objective is to \r\nexplore the world by digging or mining down into the earth, \r\nfinding precious metals and other treasures.\r\n\r\nYou need tools to mine or dig. The most important tool is the \r\nPickaxe. The Pickaxe allows you to dig earth and mine rock.\r\n\r\nPress the A button to continue.",
      "There is no shop here to buy tools. You must craft your \r\ntools with your own hands using the materials you gather.\r\n\r\nYou need wood to make most tools.\r\n\r\nWalk over to a tree now and we'll make a new tool to get\r\nyou started.",
      "Excellent. You've found a tree. \r\n\r\nI've just given you a Hatchet so you can chop the tree \r\neasier. You should see it now on the right hand side of \r\nthe screen.\r\n\r\nPress the A Button to continue.",
      "Now aim the cursor that's in the center of the screen\r\n(the little white box) at the tree. Look for a white \r\nwireframe around a section of the tree where the cursor is.\r\nThe wireframe indicates which section of a block is targeted.\r\n\r\nPress the A Button to continue.",
      "Now press and hold the Right Trigger to swing your hatchet at \r\nthe tree. The hatchet will strike the section indicated by the \r\nwireframe. Hold the trigger pressed for a few seconds until the \r\ntree is chopped.\r\n\r\nAs the hatchet swings, you'll see the tree trunk start to splinter\r\nand bits of wood fly off the tree if your targeting and striking it\r\nproperly.\r\n\r\nPress the A button to close this text if it is obscuring your view",
      "You've just chopped your first tree!\r\n\r\nYou should now see a small piece of tree trunk bobbing just above\r\nthe ground. This is a pickup. \r\n\r\nNow walk close to it, and it will fly into you. When it hits you \r\nit will be placed into your inventory.\r\n",
      "You've picked up the piece of tree trunk and it has been added\r\nto your inventory.\r\n\r\nYou should now see a row of boxes at the bottom of the screen. \r\n\r\nThis is your inventory panel.\r\n\r\nYou'll see a hatchet in the first slot and a tree trunk in the\r\nsecond.\r\n\r\nPress the A button to continue.",
      "On your inventory panel you will see a highlight box \r\naround the hatchet. This indicates the hatchet is selected. \r\n\r\nPressing Left or Right on the DPad changes the current \r\nselection.\r\n\r\nNow press the X button to bring up the inventory screen.",
      "Good. This is your inventory screen.\r\n\r\nYour inventory can hold upto 30 items\r\nor stacks of items at once. Most items\r\ncan be stacked. A stack can have up to\r\n100 items. More than that will require\r\nanother stack and therefore another \r\ninventory slot.\r\n\r\nThe first 10 slots at the bottom of \r\nthe inventory screen are the slots \r\nthat you can select from during game\r\nplay.\r\n\r\nPress the A button to continue.",
      "You can move the highlight box around \r\nthe slots using either the Left Stick,\r\nRight Stick or the Dpad. This highlight\r\nbox is used to move items from one\r\nslot to another.\r\n\r\nIf the highlight box is yellow, then no\r\nitem is selected. If it is red then an\r\nitem or a stack of items is selected.\r\n\r\nNow move the highlight box over the \r\nTree item and press the A button.",
      "You will see the highlight box has \r\nturned red. This means you have \r\nselected or grabbed that item. When\r\nyou move the highlight box around, \r\nthe item will move with it. This is \r\nhow you move items around your \r\ninventory.\r\n\r\nNow move the highlight box over to \r\nan empty slot and press the A button.",
      "Good. You have moved the Tree item\r\nto a different inventory slot.\r\n\r\nThe highlight box has changed back\r\nto yellow.\r\n\r\nIn the top left of the Inventory screen\r\nyou will see 4 slots on their own. These\r\nare the crafting slots and are used to\r\ncraft new tools.\r\n\r\nNow select the Tree item and place it \r\ninto the bottom left crafting slot.\r\n",
      "You will now see 4 Wood Planks appear\r\nin the slot to the right of the crafting\r\nslots. This slot holds the finished \r\nproduct.\r\n\r\nCrafting is done by placing one or more\r\nitems into the crafting slots. But they\r\nmust be placed into the correct slots \r\nand in the correct combination to \r\nproduce a final product. \r\n\r\nPlacing a tree trunk into the bottom\r\nleft crafting slot produces wood planks.\r\nYou get 4 planks from each tree trunk.\r\n\r\nPress the A button to continue.",
      "Any item or tool you need can be\r\ncrafted by placing items in this way.\r\n\r\nA full list of all craftable items and\r\nhow to craft them can be found on the\r\nin game pause menu. Access this menu\r\nby pressing the Back or Start button\r\nduring game play.\r\n\r\nNow move the highlight box over the \r\nstack of 4 Wood Planks and press the\r\nA button to select them.",
      "The highlight box should now be \r\nred, indicating the stack of Wood \r\nPlanks are selected.\r\n\r\nNow move the Planks over the \r\nbottom left crafting slot and \r\npress the B button once.",
      "Pressing the B button will place a\r\nsingle item from a stack into the\r\nhighlighted slot while continuing\r\nto hold the remaining items.\r\n\r\nNow place a Wood Plank into each\r\nof the remaining 3 craft slots \r\nusing the B button.",
      "Well done. You have crafted a Workbench.\r\n\r\nNow take the Workbench and place it into\r\none of the empty slots on the bottom row \r\nso that you can select it in game.",
      "Now press the X button to close your\r\ninventory screen",
      "Ok. Make sure the Workbench is selected in your inventory panel\r\nand then target an empty area of ground with the cursor.\r\n\r\nNow press the Left Trigger to place the Workbench on the ground.",
      "Your inventory screen only has a 2x2 crafting grid (4 slots).\r\n\r\nBut most items need a 3x3 crafting grid (9 slots). This is what\r\nthe Workbench is for. It gives you a 3x3 crafting grid so that\r\nyou can make any item in the game.\r\n\r\nNow target the Workbench with your cursor and press the Left\r\nTrigger to use the Workbench.",
      "Great. You will now see a 3x3 \r\ncrafting grid. This screen works\r\nexactly the same as the inventory\r\nscreen. The only difference is the\r\nsize of the crafting grid.\r\n\r\nPress the A button to continue.",
      "Now we will make a Pickaxe.\r\n\r\nPlace a single Wooden Plank \r\ninto the bottom left crafting \r\nslot. This will craft 4 sticks.",
      "Now take the 4 sticks and place one\r\nin the bottom center craft slot and \r\none in the middle craft slot.",
      "Now take 3 Wood Planks and place \r\none in each of the 3 top craft slots.\r\n\r\nIf you need more Planks, go and chop\r\nanother tree trunk and craft more \r\nplanks.",
      "Congratulations, you have made\r\na Wodden Pickaxe!\r\n\r\nNow you have the means to dig\r\nand mine your way to untold \r\nriches!\r\n\r\nWell not quite... but it will\r\nget you started.\r\n\r\nPlace the Pickaxe into your \r\ninventory and press the X button\r\nto close the Workbench.",
      "Ok, that's all for now.\r\n\r\nWhat are you waiting for? Start Exploring!\r\n\r\nThis instruction is over for now.",
      "\r\n"
    };
    private InstructionState state;
    private GameInstance instance;
    private Player player;
    private bool isLeftStickEnabled;
    private bool isRightStickEnabled;
    private bool isInventoryEnabled;
    private bool isCursorEnabled;
    private int rightStickPressedCount;
    private int rightStickPressedTarget;
    private int leftStickPressedCount;
    private int leftStickPressedTarget;
    private int treeTargetedCount;
    private int treeTargetedTarget;
    private float fadeElapsed;
    private bool fadeIn;
    private bool draw;

    public Instruction(GameInstance instance, Player player)
    {
      this.instance = instance;
      this.fadeIn = true;
      this.draw = false;
      this.isRightStickEnabled = true;
      this.isLeftStickEnabled = false;
      this.leftStickPressedTarget = 200;
      this.rightStickPressedTarget = 200;
      this.treeTargetedTarget = 40;
      this.player = player;
      this.state = InstructionState.Idle;
      this.InitializeState();
    }

    protected override void LoadContentCore(InitState state)
    {
      base.LoadContentCore(state);
    }

    protected override void UnloadContentCore()
    {
      base.UnloadContentCore();
    }

    public bool IsUnderInstruction
    {
      get
      {
        if (this.state != InstructionState.Idle)
          return this.state != InstructionState.Complete;
        return false;
      }
    }

    public bool IsLeftStickEnabled
    {
      get
      {
        if (!this.IsUnderInstruction)
          return true;
        return this.isLeftStickEnabled;
      }
    }

    public bool IsRightStickEnabled
    {
      get
      {
        if (!this.IsUnderInstruction)
          return true;
        return this.isRightStickEnabled;
      }
    }

    public bool IsInventoryEnabled
    {
      get
      {
        if (!this.IsUnderInstruction)
          return true;
        return this.isInventoryEnabled;
      }
    }

    public bool IsCursorEnabled
    {
      get
      {
        if (!this.IsUnderInstruction)
          return true;
        return this.isCursorEnabled;
      }
    }

    private void RegisterRightStickPress(object sender, EventArgs e)
    {
      ++this.rightStickPressedCount;
      if (this.rightStickPressedCount != 30)
        return;
      this.CloseWindow();
    }

    private void RegisterRightStickRelease(object sender, EventArgs e)
    {
      if (this.rightStickPressedCount < this.rightStickPressedTarget)
        return;
      this.IncrementState();
    }

    private void RegisterLeftStickRelease(object sender, EventArgs e)
    {
      if (this.leftStickPressedCount < this.leftStickPressedTarget)
        return;
      this.IncrementState();
    }

    private void RegisterLeftStickPress(object sender, EventArgs e)
    {
      ++this.leftStickPressedCount;
      if (this.leftStickPressedCount != 30)
        return;
      this.CloseWindow();
    }

    private void RegisterBlockTargeted(object sender, BlockEventArgs e)
    {
      if (this.state != InstructionState.C_MakeFirstTool_FIndTree || e.BlockID != Block.Wood)
        return;
      this.RegisterTreeTargeted();
    }

    private void RegisterBlockCleared(object sender, BlockEventArgs e)
    {
      if (this.state != InstructionState.D_ChopTree || e.BlockID != Block.Wood)
        return;
      this.IncrementState();
    }

    private void RegisterBlockPlaced(object sender, BlockEventArgs e)
    {
      if (this.state != InstructionState.Q_PlaceWorkbench || e.BlockID != Block.Workbench)
        return;
      this.IncrementState();
    }

    private void RegisterItemPickup(object sender, ItemEventArgs e)
    {
      switch (this.state)
      {
        case InstructionState.D_ChopTree:
        case InstructionState.E_FindWoodPickup:
          if (e.ItemID != Item.Wood)
            break;
          this.IncrementState();
          break;
      }
    }

    private void RegisterTreeTargeted()
    {
      ++this.treeTargetedCount;
      if (this.treeTargetedCount != this.treeTargetedTarget)
        return;
      this.IncrementState();
    }

    private void RegisterInventoryOpened(object sender, EventArgs e)
    {
      this.IncrementState();
    }

    private void RegisterWorkbenchOpened(object sender, EventArgs e)
    {
      this.IncrementState();
    }

    private void RegisterWorkbenchClosed(object sender, EventArgs e)
    {
      this.IncrementState();
    }

    private void RegisterInventoryClosed(object sender, EventArgs e)
    {
      this.IncrementState();
    }

    private void RegisterInventoryItemSelected(object sender, InventoryItemEventArgs e)
    {
      if (this.state == InstructionState.I_InventoryScreenSelectItem && e.Item.ItemID == Item.Wood)
        this.IncrementState();
      this.IncrementState();
    }

    private void RegisterInventoryItemPlaced(object sender, InventoryItemEventArgs e)
    {
      if (this.state == InstructionState.J_InventoryScreenPlaceItem && e.Item.ItemID == Item.Wood)
        this.IncrementState();
      else if (this.state == InstructionState.K_InventoryScreenCraftingIntro && this.player.GetCraftResult().ItemID == Item.WoodPlank)
        this.IncrementState();
      else if (this.state == InstructionState.N_InventoryScreenTakeWoodenPlanks && this.player.craftItem1.ItemID == Item.WoodPlank)
        this.IncrementState();
      else if (this.state == InstructionState.O_InventoryScreenMakeWorkbench && this.player.GetCraftResult().ItemID == Item.Workbench)
        this.IncrementState();
      else if (this.state == InstructionState.P_InventoryScreenTakeWorkbench && e.Item.ItemID == Item.Workbench && e.SlotID < 10)
        this.IncrementState();
      else if (this.state == InstructionState.S_MakePickAxe && e.Item.ItemID == Item.WoodPlank && this.player.GetCraftResult().ItemID == Item.Stick)
        this.IncrementState();
      else if (this.state == InstructionState.S_MakePickAxeStage2 && this.player.craftItem2.ItemID == Item.Stick && this.player.craftItem5.ItemID == Item.Stick)
      {
        this.IncrementState();
      }
      else
      {
        if (this.state != InstructionState.S_MakePickAxeStage3 || this.player.GetCraftResult().ItemID != Item.WoodPickaxe)
          return;
        this.IncrementState();
      }
    }

    private void IncrementState()
    {
      if (this.state == InstructionState.Complete)
        return;
      this.draw = false;
      this.fadeIn = true;
      this.fadeElapsed = 0.0f;
      this.CleanupState();
      ++this.state;
      this.InitializeState();
    }

    private void DecrementState()
    {
      if (this.state == InstructionState.A_Introduction)
        return;
      this.draw = false;
      this.fadeIn = true;
      this.fadeElapsed = 0.0f;
      this.CleanupState();
      --this.state;
      this.InitializeState();
    }

    private void CleanupState()
    {
      switch (this.state)
      {
        case InstructionState.A_RightStick:
          this.player.RightStickPressed -= new EventHandler(this.RegisterRightStickPress);
          this.player.RightStickReleased -= new EventHandler(this.RegisterRightStickRelease);
          break;
        case InstructionState.B_LeftStick:
          this.player.LeftStickPressed -= new EventHandler(this.RegisterLeftStickPress);
          this.player.LeftStickReleased -= new EventHandler(this.RegisterLeftStickRelease);
          this.leftStickPressedCount = 0;
          break;
        case InstructionState.C_MakeFirstTool_FIndTree:
          this.player.LeftStickPressed -= new EventHandler(this.RegisterLeftStickPress);
          break;
        case InstructionState.D_TreeFound:
          this.player.LeftStickPressed -= new EventHandler(this.RegisterLeftStickPress);
          this.player.BlockTargeted -= new BlockEventHandler(this.RegisterBlockTargeted);
          break;
        case InstructionState.D_ChopTree:
          this.player.BlockCleared -= new BlockEventHandler(this.RegisterBlockCleared);
          break;
        case InstructionState.E_FindWoodPickup:
          this.player.ItemPickup -= new ItemEventHandler(this.RegisterItemPickup);
          break;
        case InstructionState.G_InventoryPanel2:
          this.player.InventoryOpened -= new EventHandler(this.RegisterInventoryOpened);
          break;
        case InstructionState.I_InventoryScreenSelectItem:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemSelected);
          break;
        case InstructionState.J_InventoryScreenPlaceItem:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.K_InventoryScreenCraftingIntro:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.M_InventoryScreenCraftingContinued:
          this.player.InventoryScreenItemSelected -= new InventoryEventHandler(this.RegisterInventoryItemSelected);
          break;
        case InstructionState.N_InventoryScreenTakeWoodenPlanks:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.O_InventoryScreenMakeWorkbench:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.P_InventoryScreenTakeWorkbench:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.P_InventoryScreenClose:
          this.player.InventoryClosed -= new EventHandler(this.RegisterInventoryClosed);
          break;
        case InstructionState.Q_PlaceWorkbench:
          this.player.BlockPlaced -= new BlockEventHandler(this.RegisterBlockPlaced);
          break;
        case InstructionState.R_OpenWorkBench:
          this.player.WorkBenchOpened -= new EventHandler(this.RegisterWorkbenchOpened);
          break;
        case InstructionState.S_MakePickAxe:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.S_MakePickAxeStage2:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.S_MakePickAxeStage3:
          this.player.InventoryScreenItemPlaced -= new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.S_MakePickAxeDone:
          this.player.WorkBenchClosed -= new EventHandler(this.RegisterWorkbenchClosed);
          break;
      }
    }

    private void InitializeState()
    {
      switch (this.state)
      {
        case InstructionState.A_RightStick:
          this.player.RightStickPressed += new EventHandler(this.RegisterRightStickPress);
          this.player.RightStickReleased += new EventHandler(this.RegisterRightStickRelease);
          this.rightStickPressedCount = 0;
          break;
        case InstructionState.B_LeftStick:
          this.player.LeftStickPressed += new EventHandler(this.RegisterLeftStickPress);
          this.player.LeftStickReleased += new EventHandler(this.RegisterLeftStickRelease);
          this.leftStickPressedCount = 0;
          break;
        case InstructionState.C_MakeFirstTool_FIndTree:
          this.player.LeftStickPressed += new EventHandler(this.RegisterLeftStickPress);
          this.player.BlockTargeted += new BlockEventHandler(this.RegisterBlockTargeted);
          this.leftStickPressedCount = 0;
          break;
        case InstructionState.D_TreeFound:
          this.isCursorEnabled = false;
          if (this.player.HasItem(Item.WoodHatchet))
            break;
          this.player.AddToInventory(Item.WoodHatchet);
          break;
        case InstructionState.D_TreeFound1:
          this.isCursorEnabled = true;
          break;
        case InstructionState.D_ChopTree:
          this.player.BlockCleared += new BlockEventHandler(this.RegisterBlockCleared);
          this.player.ItemPickup += new ItemEventHandler(this.RegisterItemPickup);
          break;
        case InstructionState.E_FindWoodPickup:
          this.isInventoryEnabled = false;
          break;
        case InstructionState.F_GotSomeWoodNowExplainInventoryPanel:
          this.isInventoryEnabled = true;
          break;
        case InstructionState.G_InventoryPanel2:
          this.player.InventoryOpened += new EventHandler(this.RegisterInventoryOpened);
          break;
        case InstructionState.I_InventoryScreenSelectItem:
          this.player.InventoryScreenItemSelected += new InventoryEventHandler(this.RegisterInventoryItemSelected);
          break;
        case InstructionState.J_InventoryScreenPlaceItem:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.K_InventoryScreenCraftingIntro:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.M_InventoryScreenCraftingContinued:
          this.player.InventoryScreenItemSelected += new InventoryEventHandler(this.RegisterInventoryItemSelected);
          break;
        case InstructionState.N_InventoryScreenTakeWoodenPlanks:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.O_InventoryScreenMakeWorkbench:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.P_InventoryScreenTakeWorkbench:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.P_InventoryScreenClose:
          this.player.InventoryClosed += new EventHandler(this.RegisterInventoryClosed);
          break;
        case InstructionState.Q_PlaceWorkbench:
          this.player.BlockPlaced += new BlockEventHandler(this.RegisterBlockPlaced);
          break;
        case InstructionState.R_OpenWorkBench:
          this.player.WorkBenchOpened += new EventHandler(this.RegisterWorkbenchOpened);
          break;
        case InstructionState.S_MakePickAxe:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.S_MakePickAxeStage2:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.S_MakePickAxeStage3:
          this.player.InventoryScreenItemPlaced += new InventoryEventHandler(this.RegisterInventoryItemPlaced);
          break;
        case InstructionState.S_MakePickAxeDone:
          this.player.WorkBenchClosed += new EventHandler(this.RegisterWorkbenchClosed);
          break;
      }
    }

    protected override bool HandleInputCore(InputState input, PlayerIndex playerIndex)
    {
      if (this.IsUnderInstruction && this.draw && input.IsNewButtonPress(Buttons.Y))
      {
        this.CleanupState();
        this.state = InstructionState.Complete;
        this.InitializeState();
        return true;
      }
      if (this.IsUnderInstruction && !this.draw && (input.IsNewButtonPress(Buttons.RightShoulder) || input.IsNewButtonPress(Buttons.LeftShoulder)))
      {
        this.draw = true;
        this.fadeElapsed = 0.0f;
        this.fadeIn = false;
        return true;
      }
      if (this.IsUnderInstruction && input.IsNewButtonPress(Buttons.RightShoulder))
      {
        this.IncrementState();
        return true;
      }
      if (this.IsUnderInstruction && input.IsNewButtonPress(Buttons.LeftShoulder))
      {
        this.DecrementState();
        return true;
      }
      switch (this.state)
      {
        case InstructionState.A_Introduction:
        case InstructionState.A_Introduction2:
        case InstructionState.C_ALittleBitAboutTheGame:
        case InstructionState.D_TreeFound:
        case InstructionState.D_TreeFound1:
        case InstructionState.F_GotSomeWoodNowExplainInventoryPanel:
        case InstructionState.H_InventoryScreenIntro:
        case InstructionState.L_InventoryScreenCraftingPlanks:
        case InstructionState.S_UseWorkBench:
          if (input.IsNewButtonPress(Buttons.A))
          {
            this.IncrementState();
            return true;
          }
          break;
        case InstructionState.D_ChopTree:
          if (input.IsNewButtonPress(Buttons.A) && this.draw)
          {
            this.CloseWindow();
            return true;
          }
          break;
      }
      return base.HandleInputCore(input, playerIndex);
    }

    private void CloseWindow()
    {
      this.fadeElapsed = this.fadeLength;
      this.fadeIn = false;
      this.draw = false;
    }

    protected override void DrawCore(DrawState drawState)
    {
      if (this.state == InstructionState.Idle || this.state == InstructionState.Complete)
        return;
      this.HandleFade();
      if (!this.draw && (double) this.fadeElapsed <= 0.0)
        return;
      float scale = 1f;
      string text = this.instructionText[(int) this.state];
      Rectangle textRect = this.GetTextRect(text, scale);
      float num = this.draw ? 1f : this.fadeElapsed / this.fadeLength;
      CoreGlobals.SpriteBatch.Begin();
      CoreGlobals.SpriteBatch.DrawRoundedFilledBox(textRect, 2, Color.White * num, Color.DarkBlue * 0.8f * num);
      CoreGlobals.SpriteBatch.DrawString(CoreGlobals.GameFont, text, new Vector2((float) (textRect.X + 20), (float) (textRect.Y + 20)), Color.White * num, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      CoreGlobals.SpriteBatch.End();
    }

    private Rectangle GetTextRect(string text, float scale)
    {
      Vector2 vector2 = CoreGlobals.GameFont.MeasureString(text) * scale;
      Viewport viewport = CoreGlobals.GraphicsDevice.Viewport;
      int width = (int) vector2.X + 40;
      int height = (int) vector2.Y + 40;
      switch (this.anchors[(int) this.state])
      {
        case Instruction.Anchor.Top:
          return new Rectangle((viewport.Width - width) / 2, 50, width, height);
        case Instruction.Anchor.RightTop:
          return new Rectangle(viewport.Width - width - 50, 50, width, height);
        default:
          return new Rectangle((viewport.Width - width) / 2, (viewport.Height - height) / 2, width, height);
      }
    }

    private void HandleFade()
    {
      if (this.fadeIn)
      {
        this.fadeElapsed += Services.ElapsedTime;
        if ((double) this.fadeElapsed < (double) this.fadeLength)
          return;
        this.fadeElapsed = this.fadeLength;
        this.fadeIn = false;
        this.draw = true;
      }
      else
      {
        if ((double) this.fadeElapsed <= 0.0)
          return;
        this.fadeElapsed -= Services.ElapsedTime;
        if ((double) this.fadeElapsed > 0.0)
          return;
        this.fadeElapsed = 0.0f;
      }
    }

    private enum Anchor
    {
      Center,
      Top,
      RightTop,
    }
  }
}
