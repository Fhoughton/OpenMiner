// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.NPCSpawnBlockScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Achievements;
using StudioForge.TotalMiner.AI;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Screens;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class NPCSpawnBlockScreen : NewGuiMenu2
  {
    private GameProperties gameProperties;
    private SaveMapHead header;
    private NpcSpawnBlock dataBlock;
    private TextBox winTexture;
    private TextBox winActorType;
    private TextBox winActorName;
    private TextBox winBehaviour;
    private TextBox winDialogText;
    private TextBox winDialogTree;

    public override string Name
    {
      get
      {
        return "NPC Spawn";
      }
    }

    private bool IsCustomStats
    {
      get
      {
        if (this.dataBlock.ActorType != ActorType.None)
          return !this.dataBlock.CombatStats.IsEqual(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) this.dataBlock.ActorType].LevelType]);
        return false;
      }
    }

    private string CombatStatsText
    {
      get
      {
        return string.Format("{0}  [Level: {1}]", this.IsCustomStats ? (object) "Custom" : (object) "Default", (object) SkillData.CombatLevel(this.dataBlock.CombatStats));
      }
    }

    private string LootTableText
    {
      get
      {
        return this.dataBlock.LootTable.Count <= 0 ? "Default" : "Custom";
      }
    }

    private string InventoryText
    {
      get
      {
        return this.dataBlock.Inventory == null || !this.dataBlock.Inventory.HasItems() ? "Empty" : "Custom";
      }
    }

    private string TextureText
    {
      get
      {
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.NPCSpawn, (int) this.instance.Map.GetAuxHighDataNoCache(this.dataBlock.Point));
        if (textureIdForDrawing != Block.None)
          return textureIdForDrawing.ToString();
        return "None";
      }
    }

    public NPCSpawnBlockScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base(instance, player)
    {
      this.dataBlock = instance.MapStrategyTM.GetDataBlock(p) as NpcSpawnBlock;
      if (this.dataBlock == null)
        this.dataBlock = instance.MapStrategyTM.NewDataBlock(p, Block.NPCSpawn, player.GamerID) as NpcSpawnBlock;
      this.gameProperties = Globals2.GameProperties;
      this.header = this.gameProperties.SaveGame.Header;
    }

    public override void OnParentExit()
    {
      if (this.dataBlock.ActorType != ActorType.None)
        this.instance.MapStrategyTM.AddDataBlock((DataBlock) this.dataBlock, UpdateBlockMethod.Player, true);
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.dataBlock, this.dataBlock.ActorType == ActorType.None);
      base.OnParentExit();
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    private void InitMainContainer()
    {
      Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -100;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int x1 = 120;
      int y1 = 110;
      int width1 = 200;
      int width2 = 320;
      int height1 = 34;
      int num1 = 4;
      int num2 = 13;
      int height2 = height1 * num2 + num1 * (num2 - 1);
      float textScale = 0.6f;
      Window window1 = new Window((string) null, x1, y1, width1 + 1 + width2, height2)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((Node) window1);
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int y2;
      int x2 = y2 = 0;
      Window window2 = (Window) new TextBox("Type:", x2, y2, width1, height1, textScale);
      window2.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window2);
      Window window3;
      this.initialNavigable = window3 = (Window) (this.winActorType = new TextBox(Globals1.NpcTypeData[(int) this.dataBlock.ActorType].IDString, x2 + width1 + 1, y2, width2, height1, textScale));
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window3.ClickHandler += new Window.WindowHandler(this.ClickActorType);
      window1.AddChild((Node) window3);
      int y3 = y2 + (height1 + num1);
      Window window4 = (Window) new TextBox("Name:", x2, y3, width1, height1, textScale);
      window4.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window4);
      DataField dataField1 = new DataField(this.dataBlock.Name, x2 + width1 + 1, y3, width2, height1, textScale);
      Window window5 = (Window) (this.winActorName = (TextBox) dataField1);
      window5.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField1).OnValidateInput = new Action<ITextInputWindow>(this.ValidateName);
      window1.AddChild((Node) window5);
      int y4 = y3 + (height1 + num1);
      Window window6 = (Window) new TextBox("Behaviour:", x2, y4, width1, height1, textScale);
      window6.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window6);
      Window window7 = (Window) (this.winBehaviour = new TextBox(this.dataBlock.BehaviourTree, x2 + width1 + 1, y4, width2, height1, textScale));
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window7.ClickHandler += new Window.WindowHandler(this.ClickBehaviour);
      window1.AddChild((Node) window7);
      int y5 = y4 + (height1 + num1);
      Window window8 = (Window) new TextBox("Dialog Text:", x2, y5, width1, height1, textScale);
      window8.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window8);
      DataField dataField2;
      DataField dataField3 = dataField2 = new DataField(this.dataBlock.DialogText, x2 + width1 + 1, y5, width2, height1, textScale);
      TextBox textBox1 = (TextBox) dataField2;
      this.winDialogText = (TextBox) dataField2;
      Window window9 = (Window) textBox1;
      window9.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField3).OnValidateInput = new Action<ITextInputWindow>(this.ValidateDialogText);
      window9.SetToolTip("You can enter some dialog text directly here without creating a full Dialog Tree. The field overrides any selected Dialog Tree, i.e. the Dialog Tree will not execute if this field has text.");
      window1.AddChild((Node) window9);
      int y6 = y5 + (height1 + num1);
      Window window10 = (Window) new TextBox("Dialog Tree:", x2, y6, width1, height1, textScale);
      window10.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window10);
      Window window11 = (Window) (this.winDialogTree = new TextBox(this.dataBlock.DialogTree, x2 + width1 + 1, y6, width2, height1, textScale));
      window11.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window11.ClickHandler += new Window.WindowHandler(this.ClickDialogTree);
      window1.AddChild((Node) window11);
      int y7 = y6 + (height1 + num1);
      Window window12 = (Window) new TextBox("Dialog Delay:", x2, y7, width1, height1, textScale);
      window12.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window12);
      DataField dataField4 = new DataField(this.dataBlock.DialogDelay.ToString(), x2 + width1 + 1, y7, width2, height1, textScale)
      {
        IsNumeric = true
      };
      Window window13 = (Window) dataField4;
      window13.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField4).OnValidateInput = new Action<ITextInputWindow>(this.ValidateDialogDelay);
      window13.SetToolTip("Change this value if you want the NPC to wait a shorter or longer period of time before it talks to a player again");
      window1.AddChild((Node) window13);
      int y8 = y7 + (height1 + num1);
      Window window14 = (Window) new TextBox("Max Instances:", x2, y8, width1, height1, textScale);
      window14.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window14);
      DataField dataField5 = new DataField(this.dataBlock.MaxActiveInstances.ToString(), x2 + width1 + 1, y8, width2, height1, textScale)
      {
        IsNumeric = true
      };
      Window window15 = (Window) dataField5;
      window15.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField5).OnValidateInput = new Action<ITextInputWindow>(this.ValidateMaxInstances);
      window15.SetToolTip("This is the maximum number of instances of the NPC that can be currently active within the proximity of the spawn block");
      window1.AddChild((Node) window15);
      int y9 = y8 + (height1 + num1);
      Window window16 = (Window) new TextBox("Timer:", x2, y9, width1, height1, textScale);
      window16.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window16);
      DataField dataField6 = new DataField(this.dataBlock.SpawnFrequency.ToString(), x2 + width1 + 1, y9, width2, height1, textScale)
      {
        IsNumeric = true
      };
      Window window17 = (Window) dataField6;
      window17.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField6).OnValidateInput = new Action<ITextInputWindow>(this.ValidateSpawnFrequency);
      window17.SetToolTip("The amount of time in seconds between spawns");
      window1.AddChild((Node) window17);
      int y10 = y9 + (height1 + num1);
      Window window18 = (Window) new TextBox("Proximity:", x2, y10, width1, height1, textScale);
      window18.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window18);
      DataField dataField7 = new DataField(this.dataBlock.Proximity.ToString(), x2 + width1 + 1, y10, width2, height1, textScale)
      {
        IsNumeric = true
      };
      Window window19 = (Window) dataField7;
      window19.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField7).OnValidateInput = new Action<ITextInputWindow>(this.ValidateProximity);
      window19.SetToolTip("NPCs will not spawn from this block unless at least one player is within this proximity of the block (measured in blocks)");
      window1.AddChild((Node) window19);
      int y11 = y10 + (height1 + num1);
      Window window20 = (Window) new TextBox("Combat Stats:", x2, y11, width1, height1, textScale);
      window20.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window20);
      TextBox textBox2 = new TextBox(this.CombatStatsText, x2 + width1 + 1, y11, width2, height1, textScale);
      textBox2.Name = "combatstats";
      Window window21 = (Window) textBox2;
      window21.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window21.ClickHandler += new Window.WindowHandler(this.ClickCombatStats);
      window1.AddChild((Node) window21);
      int y12 = y11 + (height1 + num1);
      Window window22 = (Window) new TextBox("Loot Table:", x2, y12, width1, height1, textScale);
      window22.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window22);
      TextBox textBox3 = new TextBox(this.LootTableText, x2 + width1 + 1, y12, width2, height1, textScale);
      textBox3.Name = "loottable";
      Window window23 = (Window) textBox3;
      window23.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window23.ClickHandler += new Window.WindowHandler(this.ClickLootTable);
      window23.IsEnabled = this.instance.IsCreativeMode;
      window1.AddChild((Node) window23);
      int y13 = y12 + (height1 + num1);
      Window window24 = (Window) new TextBox("Inventory:", x2, y13, width1, height1, textScale);
      window24.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window24);
      TextBox textBox4 = new TextBox(this.InventoryText, x2 + width1 + 1, y13, width2, height1, textScale);
      textBox4.Name = "inventory";
      Window window25 = (Window) textBox4;
      window25.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window25.ClickHandler += new Window.WindowHandler(this.ClickInventory);
      window1.AddChild((Node) window25);
      int y14 = y13 + (height1 + num1);
      Window window26 = (Window) new TextBox("Kill Script:", x2, y14, width1, height1, textScale);
      window26.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window26);
      TextBox textBox5 = new TextBox(this.dataBlock.KillScript, x2 + width1 + 1, y14, width2, height1, textScale);
      textBox5.Name = "killscript";
      Window window27 = (Window) textBox5;
      window27.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window27.SetToolTip("The selected script will be executed when the NPC is killed");
      window27.ClickHandler += new Window.WindowHandler(this.ClickKillScript);
      window1.AddChild((Node) window27);
      int y15 = y14 + (height1 + num1);
      Window window28 = (Window) new TextBox("Requires Power:", x2, y15, width1, height1, textScale);
      window28.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window28);
      Window window29 = (Window) new TextBox(this.OnOff(this.dataBlock.RequiresPower), x2 + width1 + 1, y15, width2, height1, textScale);
      window29.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window29.ClickHandler += new Window.WindowHandler(this.ClickRequiresPower);
      window1.AddChild((Node) window29);
      int y16 = y15 + (height1 + num1);
      Window window30 = (Window) new TextBox("Show Owner:", x2, y16, width1, height1, textScale);
      window30.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window30);
      Window window31 = (Window) new TextBox(this.OnOff(this.dataBlock.ShowOwnerData), x2 + width1 + 1, y16, width2, height1, textScale);
      window31.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window31.ClickHandler += new Window.WindowHandler(this.ClickShowOwner);
      window1.AddChild((Node) window31);
      int y17 = y16 + (height1 + num1);
      Window window32 = (Window) new TextBox("Texture:", x2, y17, width1, height1, textScale);
      window32.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((Node) window32);
      Window window33 = (Window) (this.winTexture = new TextBox(this.TextureText, x2 + width1 + 1, y17, width2, height1, textScale));
      window33.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window33.ClickHandler += new Window.WindowHandler(this.ClickTexture);
      window1.AddChild((Node) window33);
      int num3 = y17 + (height1 + num1);
    }

    private void ClickActorType(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new UnlockablesScreen(this.instance, this.player, (Unlockable) null, false, true, new Action<Player, ActorType>(this.OnActorTypeSelected), this.dataBlock.ActorType, (GameScreen) null), new PlayerIndex?(this.playerIndex));
    }

    private void OnActorTypeSelected(Player player, ActorType actorType)
    {
      if (player == null || this.dataBlock.ActorType == actorType)
        return;
      this.dataBlock.SetActorType(actorType);
      string idString = Globals1.NpcTypeData[(int) actorType].IDString;
      this.dataBlock.Name = "";
      this.dataBlock.ShowOwnerData = false;
      this.dataBlock.DialogText = (string) null;
      this.dataBlock.DialogTree = "System\\Dialog\\" + idString;
      this.dataBlock.OwnerGamertag = player.Gamertag;
      Unlockable unlockable = player.Unlockables != null ? player.Unlockables.GetUnlockable(actorType) : (Unlockable) null;
      this.dataBlock.OwnerHasAvatarUnlocked = unlockable == null || !unlockable.IsUnlocked ? UnlockType.Locked : UnlockType.Unlocked;
      this.winActorType.Text = idString;
      this.winActorName.Text = this.dataBlock.Name;
      this.winDialogTree.Text = this.dataBlock.DialogTree;
      this.winDialogText.IsEnabled = false;
    }

    private void ValidateName(ITextInputWindow win)
    {
      if (win.Text.IsNotEmpty())
        this.dataBlock.Name = win.Text;
      win.Text = this.dataBlock.Name;
    }

    private void ValidateDialogText(ITextInputWindow win)
    {
      this.dataBlock.DialogText = win.Text;
      this.dataBlock.DialogTextCache = (BehaviourTree) null;
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
        localEnabledPlayer.DialogHandler.EndConversation();
    }

    private void ValidateDialogDelay(ITextInputWindow win)
    {
      int result;
      if (win.Text.IsNotEmpty() && int.TryParse(win.Text, out result))
        this.dataBlock.DialogDelay = (ushort) MyMathHelper.Clamp(result, 0, (int) ushort.MaxValue);
      win.Text = this.dataBlock.DialogDelay.ToString();
    }

    private void ValidateMaxInstances(ITextInputWindow win)
    {
      int result;
      if (win.Text.IsNotEmpty() && int.TryParse(win.Text, out result))
        this.dataBlock.MaxActiveInstances = MyMathHelper.Clamp(result, 0, this.instance.NpcManager.CurrentMaxNpcCount);
      win.Text = this.dataBlock.MaxActiveInstances.ToString();
    }

    private void ClickBehaviour(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new BehaviourListMenuScreen(this.instance, this.player, BehaviourTreeType.AI, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnBehaviourSelectedForUse)), new PlayerIndex?(this.playerIndex));
    }

    private bool OnBehaviourSelectedForUse(MenuEntry item)
    {
      string str = (string) item.Tag + item.Text;
      this.dataBlock.BehaviourTree = str != "None" ? str : (string) null;
      this.winBehaviour.Text = this.dataBlock.BehaviourTree;
      return true;
    }

    private void ClickDialogTree(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new BehaviourListMenuScreen(this.instance, this.player, BehaviourTreeType.Dialog, (string) null, new ListBoxScreen.OnMenuItemSelected(this.OnDialogSelectedForUse), (Action) null, false, true), new PlayerIndex?(this.playerIndex));
    }

    private bool OnDialogSelectedForUse(MenuEntry item)
    {
      if (item == null)
      {
        this.dataBlock.DialogTree = (string) null;
      }
      else
      {
        string str = (string) item.Tag + item.Text;
        this.dataBlock.DialogTree = str != "None" ? str : (string) null;
      }
      this.winDialogTree.Text = this.dataBlock.DialogTree;
      return true;
    }

    private void ValidateSpawnFrequency(ITextInputWindow win)
    {
      float result;
      if (win.Text.IsNotEmpty() && float.TryParse(win.Text, out result))
        this.dataBlock.SpawnFrequency = MathHelper.Clamp(result, 0.1f, 10000f);
      win.Text = this.dataBlock.SpawnFrequency.ToString();
    }

    private void ValidateProximity(ITextInputWindow win)
    {
      int result;
      if (win.Text.IsNotEmpty() && int.TryParse(win.Text, out result))
        this.dataBlock.Proximity = MyMathHelper.Clamp(result, 10, 300);
      win.Text = this.dataBlock.Proximity.ToString();
    }

    private void ClickCombatStats(object sender, WindowEventArgs e)
    {
      CombatStats reset = new CombatStats();
      reset.SetFromXML(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) this.dataBlock.ActorType].LevelType]);
      CombatStatsEntryScreen statsEntryScreen = new CombatStatsEntryScreen(this.player, this.dataBlock.CombatStats, reset, new Action<CombatStats>(this.OnCombatStatsEntered));
      statsEntryScreen.IsPopup = true;
      this.screenManager.AddScreen((GameScreen) statsEntryScreen, new PlayerIndex?(this.playerIndex));
    }

    private void OnCombatStatsEntered(CombatStats stats)
    {
      this.dataBlock.CombatStats = stats;
      ((TextBox) this.canvas.FindChild("mainContainer").FindChild("combatStats")).Text = this.CombatStatsText;
    }

    private void ClickLootTable(object sender, WindowEventArgs e)
    {
      LootTableScreen lootTableScreen = new LootTableScreen(this.player, this.dataBlock.LootTable, new Action(this.OnLootTableEntered));
      lootTableScreen.IsPopup = true;
      this.screenManager.AddScreen((GameScreen) lootTableScreen, new PlayerIndex?(this.playerIndex));
    }

    private void OnLootTableEntered()
    {
      ((TextBox) this.canvas.FindChild("mainContainer").FindChild("loottable")).Text = this.LootTableText;
    }

    private void ClickInventory(object sender, WindowEventArgs e)
    {
      if (this.dataBlock.Inventory == null)
        this.dataBlock.Inventory = new Inventory(10, 7, 0);
      ShopScreen shopScreen = new ShopScreen(this.instance, this.player, this.dataBlock.Inventory, new Action(this.OnInventoryEntered));
      shopScreen.IsPopup = true;
      this.screenManager.AddScreen((GameScreen) shopScreen, new PlayerIndex?(this.playerIndex));
    }

    private void OnInventoryEntered()
    {
      ((TextBox) this.canvas.FindChild("mainContainer").FindChild("inventory")).Text = this.InventoryText;
    }

    private void ClickKillScript(object sender, WindowEventArgs e)
    {
      ScriptListMenuScreen scriptListMenuScreen = new ScriptListMenuScreen(this.instance, this.player, this.dataBlock.KillScript, new ListBoxScreen.OnMenuItemSelected(this.OnKillScriptEntered), false, true);
      scriptListMenuScreen.IsPopup = true;
      this.screenManager.AddScreen((GameScreen) scriptListMenuScreen, new PlayerIndex?(this.playerIndex));
    }

    private bool OnKillScriptEntered(MenuEntry item)
    {
      if (this.player.IsAdmin)
      {
        this.dataBlock.KillScript = item == null ? (string) null : (string) item.Tag + item.Text;
        ((TextBox) this.canvas.FindChild("mainContainer").FindChild("killscript")).Text = this.dataBlock.KillScript;
      }
      return true;
    }

    private void ClickRequiresPower(object sender, WindowEventArgs e)
    {
      this.dataBlock.RequiresPower = !this.dataBlock.RequiresPower;
      ((TextBox) e.Window).Text = this.OnOff(this.dataBlock.RequiresPower);
    }

    private void ClickShowOwner(object sender, WindowEventArgs e)
    {
      this.dataBlock.ShowOwnerData = !this.dataBlock.ShowOwnerData;
      ((TextBox) e.Window).Text = this.OnOff(this.dataBlock.ShowOwnerData);
    }

    private void ClickTexture(object sender, WindowEventArgs e)
    {
      this.parentScreen.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.TextureCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.NPCSpawn, (int) this.instance.Map.GetAuxHighDataNoCache(this.dataBlock.Point)), this.parentScreen.ControllingPlayer);
    }

    private bool TextureCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.dataBlock.Point, Block.NPCSpawn, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
        return false;
      this.winTexture.Text = this.TextureText;
      return true;
    }
  }
}
