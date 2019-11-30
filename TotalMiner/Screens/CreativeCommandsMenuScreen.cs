// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandsMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandsMenuScreen : BlockMenuScreen
  {
    private GameInstance instance;

    public CreativeCommandsMenuScreen(GameInstance instance, Player player)
      : base("Creative Commands", player)
    {
      this.instance = instance;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Measure"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Remove Markers"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Fill"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Clear"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Replace"));
      if (!instance.IsAvatarDesigner)
      {
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Replace Texture"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Line"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Sphere"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Wall / Path"));
        blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Trees"));
      }
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Copy"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Flood"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Abort Active Floods"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      bool clipboardEquipped = player.IsClipboardEquipped;
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.MeasureMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.RemoveMarkersMenuEntrySelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.FillMenuEntrySelected);
      blockMenuEntryList1[num4 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(this.ClearMenuEntrySelected);
      blockMenuEntryList1[num5 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.ReplaceMenuEntrySelected);
      if (clipboardEquipped)
        blockMenuEntryList1[num6 - 1].Text = "Replace (clipboard)";
      blockMenuEntryList1[num6 - 1].IsEnabled = player.HasPermission(Permissions.Creative);
      if (!instance.IsAvatarDesigner)
      {
        List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
        int index6 = num6;
        int num7 = index6 + 1;
        blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(this.ReplaceTextureMenuEntrySelected);
        blockMenuEntryList1[num7 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
        List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
        int index7 = num7;
        int num8 = index7 + 1;
        blockMenuEntryList8[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.LineMenuEntrySelected);
        blockMenuEntryList1[num8 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
        List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
        int index8 = num8;
        int num9 = index8 + 1;
        blockMenuEntryList9[index8].Selected += new EventHandler<PlayerIndexEventArgs>(this.SphereMenuEntrySelected);
        blockMenuEntryList1[num9 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
        List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
        int index9 = num9;
        int num10 = index9 + 1;
        blockMenuEntryList10[index9].Selected += new EventHandler<PlayerIndexEventArgs>(this.WallMenuEntrySelected);
        blockMenuEntryList1[num10 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
        List<BlockMenuEntry> blockMenuEntryList11 = blockMenuEntryList1;
        int index10 = num10;
        num6 = index10 + 1;
        blockMenuEntryList11[index10].Selected += new EventHandler<PlayerIndexEventArgs>(this.TreesMenuEntrySelected);
        blockMenuEntryList1[num6 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
      }
      List<BlockMenuEntry> blockMenuEntryList12 = blockMenuEntryList1;
      int index11 = num6;
      int num11 = index11 + 1;
      blockMenuEntryList12[index11].Selected += new EventHandler<PlayerIndexEventArgs>(this.CopyMenuEntrySelected);
      blockMenuEntryList1[num11 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Creative);
      List<BlockMenuEntry> blockMenuEntryList13 = blockMenuEntryList1;
      int index12 = num11;
      int num12 = index12 + 1;
      blockMenuEntryList13[index12].Selected += new EventHandler<PlayerIndexEventArgs>(this.FloodMenuEntrySelected);
      blockMenuEntryList1[num12 - 1].IsEnabled = !clipboardEquipped && player.HasPermission(Permissions.Grief);
      List<BlockMenuEntry> blockMenuEntryList14 = blockMenuEntryList1;
      int index13 = num12;
      int num13 = index13 + 1;
      blockMenuEntryList14[index13].Selected += new EventHandler<PlayerIndexEventArgs>(this.AbortFloodMenuEntrySelected);
      blockMenuEntryList1[num13 - 1].IsEnabled = player.HasActiveFloods && player.HasPermission(Permissions.Grief);
      List<BlockMenuEntry> blockMenuEntryList15 = blockMenuEntryList1;
      int index14 = num13;
      int num14 = index14 + 1;
      blockMenuEntryList15[index14].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 432;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    private void RemoveMarkersMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.instance.CreativeModeHelper.RemoveMarkers(this.player.GamerID, true);
      this.ExitScreen();
    }

    private void MeasureMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.instance.CreativeModeHelper.Measure(this.player);
      this.ExitScreen();
    }

    private void CopyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.instance.CreativeModeHelper.CopyToClipboard(this.player.GamerID, Map.CopyAccess.Restricted);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void ClearMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandClearScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void FillMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandFillScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void ReplaceMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        if (this.player.IsClipboardEquipped)
          this.ScreenManager.AddScreen((GameScreen) new CreativeCommandReplaceClipboardScreen(this.instance, this.player), this.ControllingPlayer);
        else
          this.ScreenManager.AddScreen((GameScreen) new CreativeCommandReplaceScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void ReplaceTextureMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandReplaceTextureScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void FloodMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Grief, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandFloodScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void AbortFloodMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Grief, true))
      {
        this.instance.CreativeModeHelper.AbortFlood(this.player);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void LineMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandLineScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void SphereMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandSphereScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void WallMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandWallScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    private void TreesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player.HasPermission(Permissions.Creative, true))
      {
        this.ScreenManager.AddScreen((GameScreen) new CreativeCommandTreesScreen(this.instance, this.player), this.ControllingPlayer);
        this.ExitScreen();
      }
      else
        TotalMinerGame.ShowNoPermissionScreen(this.ScreenManager, this.ControllingPlayer, this.player);
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      base.OnCancel(playerIndex);
      this.ScreenManager.AddScreen((GameScreen) new CreativeMenuScreen(this.instance, this.player), this.ControllingPlayer);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
