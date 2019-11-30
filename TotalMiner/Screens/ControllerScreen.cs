// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ControllerScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ControllerScreen : BlockMenuScreen
  {
    private UserControlSetting setting;

    public ControllerScreen(UserControlSetting setting)
      : base("Controls", (Player) null)
    {
      this.setting = setting;
      this.InitItems();
    }

    private void InitItems()
    {
      this.MenuEntries.Clear();
      UserControls userControls = new UserControls();
      userControls.Initialize(this.setting);
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, "Setup: " + (object) userControls.Scheme);
      blockMenuEntry1.ColorSelected = Color.Yellow;
      blockMenuEntry1.ColorUnselected = Color.Yellow;
      BlockMenuEntry blockMenuEntry2 = blockMenuEntry1;
      blockMenuEntryList2.Add(blockMenuEntry2);
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Move: LeftStick"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Look: RightStick"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Jump: " + (object) userControls.Jump));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Open Inventory: " + (object) userControls.OpenInventory));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Use Left Hand (Build): " + (object) userControls.LeftHand));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Use Right Hand (Dig): " + (object) userControls.RightHand));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Prospect/Interact: " + (object) userControls.Prospect));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Prospect Pick: " + (object) userControls.ProspectPickItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Crouch: " + (object) userControls.Crouch));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Crouch Hold: " + (object) userControls.CrouchHold));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fly Toggle: " + (object) userControls.FlyToggle));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fly Ascend: " + (object) userControls.FlyAscend));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fly Descend: " + (object) userControls.FlyDescend));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry3 = new BlockMenuEntry((BlockMenuScreen) this, "General");
      blockMenuEntry3.ColorSelected = Color.Yellow;
      blockMenuEntry3.ColorUnselected = Color.Yellow;
      BlockMenuEntry blockMenuEntry4 = blockMenuEntry3;
      blockMenuEntryList3.Add(blockMenuEntry4);
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "HotBar Left: " + (object) userControls.HotBarLeft));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "HotBar Right: " + (object) userControls.HotBarRight));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Pause Menu: " + (object) userControls.PauseMenu));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Open Top Map: " + (object) userControls.OpenTopMap));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry5 = new BlockMenuEntry((BlockMenuScreen) this, "Inventory + Crafting Screens");
      blockMenuEntry5.ColorSelected = Color.Yellow;
      blockMenuEntry5.ColorUnselected = Color.Yellow;
      BlockMenuEntry blockMenuEntry6 = blockMenuEntry5;
      blockMenuEntryList4.Add(blockMenuEntry6);
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Inventory Equip Lefthand Item: " + (object) userControls.InvEquipLeftItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Inventory Equip Righthand Item: " + (object) userControls.InvEquipRightItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Open Craft Screen: " + (object) userControls.OpenCraftScreen));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Crafting Equip Lefthand Item: " + (object) userControls.CraftEquipLeftItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Crafting Equip Righthand Item: " + (object) userControls.CraftEquipRightItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Lift Stack: " + (object) userControls.InvLiftItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Lift Half Stack: " + (object) userControls.InvLiftSingleItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Transfer Item: " + (object) userControls.InvTransferItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Examine Item: " + (object) userControls.InvExamineItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Drop/Discard Item: " + (object) userControls.InvDropItem));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Open Easy Craft/Smelt: " + (object) userControls.CraftEasyCraft));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Switch Page/Tab Left: " + (object) userControls.SwitchTabLeft));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Switch Page/Tab Right: " + (object) userControls.SwitchTabRight));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry7 = new BlockMenuEntry((BlockMenuScreen) this, "Creative Features");
      blockMenuEntry7.ColorSelected = Color.Yellow;
      blockMenuEntry7.ColorUnselected = Color.Yellow;
      BlockMenuEntry blockMenuEntry8 = blockMenuEntry7;
      blockMenuEntryList5.Add(blockMenuEntry8);
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Creative Menu Shortcut: " + (object) userControls.CreativeMenu));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Creative Block Shop: " + (object) userControls.CreativeBlocks));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Creative Item Shop: " + (object) userControls.CreativeItems));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry9 = new BlockMenuEntry((BlockMenuScreen) this, "Special Key Features");
      blockMenuEntry9.ColorSelected = Color.Yellow;
      blockMenuEntry9.ColorUnselected = Color.Yellow;
      BlockMenuEntry blockMenuEntry10 = blockMenuEntry9;
      blockMenuEntryList6.Add(blockMenuEntry10);
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Special Key: " + (object) userControls.SpecialKey));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Text Message Shortcut: Special Key + " + (object) userControls.TextMessageShortcut));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Time FFwd: Special Key + " + (object) userControls.TimeFFwd));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Time Rev: Special Key + " + (object) userControls.TimeRev));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "No Clip Toggle: Special Key + " + (object) userControls.NoClipToggle));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Paste Merge: Special Key + " + (object) userControls.PasteMerge));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Paste No Overwrite: Special Key + " + (object) userControls.PasteNoOverwrite));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clipboard Zoom In: Special Key + RightThumbstickDown"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clipboard Zoom Out: Special Key + RightThumbstickUp"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clipboard Rotate Left: Special Key + RightThumbstickLeft"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clipboard Rotate Right: Special Key + RightThumbstickRight"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Spectate Player Left: Special Key + " + (object) userControls.SpectatePlayerLeft));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Spectate Player Right: Special Key + " + (object) userControls.SpectatePlayerRight));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList1[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.SchemeSelected);
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
    }

    private void SchemeSelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.setting == UserControlSetting.MC360)
        this.setting = UserControlSetting.TotalMiner2_0;
      else
        ++this.setting;
      this.InitItems();
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 560;
      this.ItemHeight = 20;
      this.ItemGapY = 2;
      this.ItemTextScale = 0.5f;
      this.ItemsPerPage = 26;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    private enum ShowController
    {
      GameplayControls,
      InventoryControls,
      CraftScreenControls,
      OtherControls,
      None,
    }
  }
}
