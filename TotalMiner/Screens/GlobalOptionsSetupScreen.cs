// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GlobalOptionsSetupScreen
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
  internal class GlobalOptionsSetupScreen : BlockMenuScreen
  {
    private PlayerIndex playerIndex;
    private GlobalGamerSettings settings;

    public GlobalOptionsSetupScreen(PlayerIndex playerIndex)
      : base("Options", (Player) null)
    {
      this.playerIndex = playerIndex;
      this.settings = Globals2.GamertagData.GetGlobalGamerSettings(playerIndex);
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Player Options"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Game Options"));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.settings.GlobalOverwrite = !this.settings.GlobalOverwrite;
        this.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.PlayerOptionsSelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(this.GameOptionsSelected);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = "Apply: " + (this.settings.GlobalOverwrite ? "Always" : "New World");
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 478;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      this.ValidateDefaultAvatar();
      Globals2.SaveGamertagDataThreaded(false, false);
    }

    private void ValidateDefaultAvatar()
    {
      if (this.IsValidAvatar())
        return;
      this.settings.PlayerSettings.MobType = ActorType.Boy;
    }

    private bool IsValidAvatar()
    {
      return Player.IsActorTypeValidForAvatar(this.settings.PlayerSettings.MobType);
    }

    private void PlayerOptionsSelected(object sender, PlayerIndexEventArgs e)
    {
      if (Globals2.GetSignedInGamer(e.PlayerIndex) != null)
        this.ScreenManager.AddScreen((GameScreen) new GlobalPlayerOptionsScreen(e.PlayerIndex), new PlayerIndex?(e.PlayerIndex));
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("You must be signed in", e.PlayerIndex);
    }

    private void GameOptionsSelected(object sender, PlayerIndexEventArgs e)
    {
      if (Globals2.GetSignedInGamer(e.PlayerIndex) != null)
        this.ScreenManager.AddScreen((GameScreen) new GlobalGameOptionsScreen(e.PlayerIndex), new PlayerIndex?(e.PlayerIndex));
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("You must be signed in", e.PlayerIndex);
    }

    private void ManageModsSelected(object sender, PlayerIndexEventArgs e)
    {
      if (Globals2.GetSignedInGamer(e.PlayerIndex) != null)
        this.ScreenManager.AddScreen((GameScreen) new ModListMenuScreen(), new PlayerIndex?(e.PlayerIndex));
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("You must be signed in", e.PlayerIndex);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
