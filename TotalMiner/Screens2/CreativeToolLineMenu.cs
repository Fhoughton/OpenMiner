// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.CreativeToolLineMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class CreativeToolLineMenu : CreativeToolMenu
  {
    private TextBox blockWin;

    protected override string SelectBlockText
    {
      get
      {
        return "Select a Block for the Line";
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return false;
      }
    }

    public CreativeToolLineMenu(
      PauseMenuScreen2 parentScreen,
      GameInstance instance,
      Player player,
      Action onApplied)
      : base(parentScreen, instance, player, instance.CreativeModeHelper.GetLineCommandData(player), onApplied)
    {
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
      Window window1 = (Window) new TextBox("Block:", x, y, w, h, scale);
      window1.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window1);
      Window window2 = (Window) (this.blockWin = new TextBox(this.BlockIDText, x + w + 1, y, w2, h, scale));
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(((CreativeToolMenu) this).ClickBlockID);
      container.AddChild((Node) window2);
      y += h + g;
      Window window3 = (Window) new TextBox("Width:", x, y, w, h, scale);
      window3.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window3);
      DataField dataField1;
      Window window4 = (Window) (dataField1 = new DataField(this.data.BlockID1.ToString(), x + w + 1, y, w2, h, scale));
      window4.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField1).OnValidateInput = new Action<ITextInputWindow>(this.ValidateWidth);
      container.AddChild((Node) window4);
      y += h + g;
      Window window5 = (Window) new TextBox("Height:", x, y, w, h, scale);
      window5.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window5);
      DataField dataField2;
      Window window6 = (Window) (dataField2 = new DataField(this.data.BlockID2.ToString(), x + w + 1, y, w2, h, scale));
      window6.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateHeight);
      container.AddChild((Node) window6);
      y += h + g;
      return y;
    }

    protected override void RefreshWindowTextCore()
    {
      this.blockWin.Text = this.BlockIDText;
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeLineDefaults(this.data);
    }

    protected override void BuildMessageText()
    {
      if (this.markerCount >= 2)
        return;
      this.statusText = "Must have at least 2 markers";
      this.data.IsValid = false;
    }

    private void ValidateWidth(ITextInputWindow win)
    {
      int result;
      if (int.TryParse(win.Text, out result))
        this.data.BlockID1 = (byte) MyMathHelper.Clamp(result, 1, 16);
      win.Text = this.data.BlockID1.ToString();
      this.RefreshWindowText();
    }

    private void ValidateHeight(ITextInputWindow win)
    {
      int result;
      if (int.TryParse(win.Text, out result))
        this.data.BlockID2 = (byte) MyMathHelper.Clamp(result, 1, 16);
      win.Text = this.data.BlockID2.ToString();
      this.RefreshWindowText();
    }

    protected override bool OnExecuteCore()
    {
      this.instance.CreativeCommandQueue.Execute(this.data, true);
      this.SendNetworkCommand();
      return true;
    }
  }
}
