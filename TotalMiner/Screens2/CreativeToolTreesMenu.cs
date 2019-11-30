// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.CreativeToolTreesMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class CreativeToolTreesMenu : CreativeToolMenu
  {
    private string[] treeTypes = new string[7]
    {
      "Acacia",
      "Jungle",
      "Maple",
      "Oak",
      "Original",
      "Pine",
      "Bonemeal"
    };
    private CreativeGenerateTreeData treeData;

    protected override bool IsPercentUsed
    {
      get
      {
        return false;
      }
    }

    protected override bool IsSeedUsed
    {
      get
      {
        return true;
      }
    }

    protected override bool IsClearMarkersUsed
    {
      get
      {
        return true;
      }
    }

    private string GetTreeText(int i)
    {
      return !this.treeData.CompsSelected[i] ? "----" : "Use";
    }

    public CreativeToolTreesMenu(
      PauseMenuScreen2 parentScreen,
      GameInstance instance,
      Player player,
      Action onApplied)
      : base(parentScreen, instance, player, instance.CreativeModeHelper.GetTreesCommandData(player), onApplied)
    {
      this.treeData = this.data.Data as CreativeGenerateTreeData;
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
      Window window1 = (Window) new TextBox("Tree Count:", x, y, w, h, scale);
      window1.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window1);
      DataField dataField;
      Window window2 = (Window) (dataField = new DataField(this.treeData.TreeCount.ToString(), x + w + 1, y, w2, h, scale));
      window2.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField).OnValidateInput = new Action<ITextInputWindow>(this.ValidateTreeCount);
      container.AddChild((Node) window2);
      y += h + g;
      y += g;
      container.AddChild((Node) new Window((string) null, x, y, w + 1 + w2, g)
      {
        IsEnabled = false
      });
      y += g + g + g;
      for (int i = 0; i < this.treeTypes.Length; ++i)
      {
        Window window3 = (Window) new TextBox(this.treeTypes[i], x, y, w, h, scale);
        window3.Colors = (Window.ColorProfile) Colors.LabelColors;
        container.AddChild((Node) window3);
        Window window4 = (Window) new TextBox(this.GetTreeText(i), x + w + 1, y, w2, h, scale);
        window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window4.ClickHandler += new Window.WindowHandler(this.ClickTreeType);
        window4.Tag = (object) i;
        container.AddChild((Node) window4);
        y += h + g;
      }
      return y;
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeTreesDefaults(this.data);
    }

    protected override void BuildMessageText()
    {
      if (this.markerCount < 2)
      {
        this.statusText = "Must have at least 2 markers";
        this.data.IsValid = false;
      }
      else if (this.RegionSizeInBlocks2D * 5 > CreativeModeHelper.MaxRegionBlocks && !this.player.IsGod)
      {
        this.statusText = "Region exceeds maximum area";
        this.data.IsValid = false;
      }
      else
      {
        bool flag1 = false;
        foreach (bool flag2 in this.treeData.CompsSelected)
        {
          if (flag2)
          {
            flag1 = true;
            break;
          }
        }
        if (flag1)
          return;
        this.statusText = "You have not selected any Tree Types";
        this.data.IsValid = false;
      }
    }

    private void ValidateTreeCount(ITextInputWindow win)
    {
      int result;
      if (int.TryParse(win.Text, out result))
        this.treeData.TreeCount = MyMathHelper.Clamp(result, 1, 5000);
      win.Text = this.treeData.TreeCount.ToString();
      this.RefreshWindowText();
    }

    protected void ClickTreeType(object sender, WindowEventArgs e)
    {
      int tag = (int) e.Window.Tag;
      this.treeData.CompsSelected[tag] = !this.treeData.CompsSelected[tag];
      ((TextBox) e.Window).Text = this.GetTreeText(tag);
      this.RefreshWindowText();
    }

    protected override bool OnExecuteCore()
    {
      if (this.data.Seed == 0)
      {
        this.data.IsCustomSeed = false;
        this.data.Seed = this.instance.Random.Next();
      }
      this.treeData.TreeModels = CreativeModeHelper.GetTreeModelsToUseForGenerateTrees(this.treeData.CompsSelected);
      this.instance.CreativeCommandQueue.Execute(this.data, true);
      this.SendNetworkCommand();
      return true;
    }
  }
}
