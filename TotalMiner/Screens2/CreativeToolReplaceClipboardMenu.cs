// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.CreativeToolReplaceClipboardMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.Screens;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class CreativeToolReplaceClipboardMenu : CreativeToolMenu
  {
    private TextBox blockWin;
    private TextBox block1Win;

    protected override string SelectBlockText
    {
      get
      {
        return "Select the Block to Replace";
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return true;
      }
    }

    private string BlockID1Text
    {
      get
      {
        return ItemData2.ForDisplay(this.instance, (Item) this.data.BlockID1);
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeReplace;
      }
    }

    public CreativeToolReplaceClipboardMenu(
      PauseMenuScreen2 parentScreen,
      GameInstance instance,
      Player player,
      Action onApplied)
      : base(parentScreen, instance, player, CreativeToolReplaceClipboardMenu.GetCommandData(instance, player), onApplied)
    {
      this.data.Map = player.ClipboardModel.Map;
      this.data.Min = GlobalPoint3D.Zero;
      this.data.Max = this.data.Map.MapSize - GlobalPoint3D.One;
      this.data.XMin = this.data.XMax = GlobalPoint3D.MaxValue;
    }

    private static CreativeOperationData GetCommandData(
      GameInstance instance,
      Player player)
    {
      CreativeOperationData clipboardCommandData = instance.CreativeModeHelper.GetReplaceClipboardCommandData(player);
      clipboardCommandData.Map = player.ClipboardModel.Map;
      clipboardCommandData.Min = GlobalPoint3D.Zero;
      clipboardCommandData.Max = clipboardCommandData.Map.MapSize - GlobalPoint3D.One;
      clipboardCommandData.XMin = clipboardCommandData.XMax = GlobalPoint3D.MaxValue;
      clipboardCommandData.OnCompletion = new Action<CreativeOperationData>(CreativeToolReplaceClipboardMenu.OnClipboardReplaceComplete);
      return clipboardCommandData;
    }

    private static void OnClipboardReplaceComplete(CreativeOperationData op)
    {
      op.Map.Regions[0].Chunks[0].LoadMesh(true, true);
    }

    protected override int InitWindowsExtra(
      Window container,
      int x,
      int y,
      int w,
      int w2,
      int h,
      int g,
      float scale)
    {
      Window window1 = (Window) new TextBox("Block to Replace:", x, y, w, h, scale);
      window1.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window1);
      Window window2 = (Window) (this.blockWin = new TextBox(this.BlockIDText, x + w + 1, y, w2, h, scale));
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(((CreativeToolMenu) this).ClickBlockID);
      container.AddChild((Node) window2);
      y += h + g;
      Window window3 = (Window) new TextBox("Replace with Block:", x, y, w, h, scale);
      window3.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window3);
      Window window4 = (Window) (this.block1Win = new TextBox(this.BlockID1Text, x + w + 1, y, w2, h, scale));
      window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window4.ClickHandler += new Window.WindowHandler(this.ClickBlockID1);
      container.AddChild((Node) window4);
      y += h + g;
      return y;
    }

    protected override void RefreshWindowTextCore()
    {
      this.blockWin.Text = this.BlockIDText;
      this.block1Win.Text = this.BlockID1Text;
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeReplaceClipboardDefaults(this.data);
    }

    protected override void BuildMessageText()
    {
      if (!this.player.IsClipboardEquipped)
      {
        this.statusText = "Must have clipboard equipped";
        this.data.IsValid = false;
      }
      else
      {
        if ((int) this.data.BlockID != (int) this.data.BlockID1)
          return;
        this.statusText = "Must specify a different replacement block";
        this.data.IsValid = false;
      }
    }

    protected void ClickBlockID1(object sender, WindowEventArgs e)
    {
      BlockSelectionScreen blockSelectionScreen = new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.OnBlock1Selected), "Select Replacement Block", this.BlockSelectMode);
      blockSelectionScreen.IsPopup = true;
      this.instance.AddScreen((GameScreen) blockSelectionScreen, this.player);
    }

    private bool OnBlock1Selected(Player player, Block block)
    {
      this.data.BlockID1 = (byte) block;
      this.RefreshWindowText();
      return true;
    }

    protected override bool OnExecuteCore()
    {
      if (this.data.Seed == 0)
      {
        this.data.IsCustomSeed = false;
        this.data.Seed = this.instance.Random.Next();
      }
      this.instance.CreativeCommandQueue.Execute(this.data, true);
      return true;
    }
  }
}
