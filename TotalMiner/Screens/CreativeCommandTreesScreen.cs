// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandTreesScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandTreesScreen : CreativeCommandScreen
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

    private string CountText
    {
      get
      {
        return "Tree Count: " + this.treeData.TreeCount.ToString();
      }
    }

    public CreativeCommandTreesScreen(GameInstance instance, Player player)
      : base(instance, player, instance.CreativeModeHelper.GetTreesCommandData(player))
    {
    }

    private string GetItemText(int i)
    {
      return (this.treeData.CompsSelected[i] ? "[x] " : "[ ] ") + this.treeTypes[i];
    }

    protected override void AddParamItems(List<BlockMenuEntry> items)
    {
      this.treeData = this.data.Data as CreativeGenerateTreeData;
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.CountText));
      items[this.baseItemCount].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEnterCount);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, "------------------- Select Trees -----------------------------"));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GetItemText(0)));
      items[this.baseItemCount + 2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ToggleItem(0));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GetItemText(1)));
      items[this.baseItemCount + 3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ToggleItem(1));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GetItemText(2)));
      items[this.baseItemCount + 4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ToggleItem(2));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GetItemText(3)));
      items[this.baseItemCount + 5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ToggleItem(3));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GetItemText(4)));
      items[this.baseItemCount + 6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ToggleItem(4));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GetItemText(5)));
      items[this.baseItemCount + 7].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ToggleItem(5));
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GetItemText(6)));
      items[this.baseItemCount + 8].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => this.ToggleItem(6));
    }

    private void ToggleItem(int i)
    {
      this.treeData.CompsSelected[i] = !this.treeData.CompsSelected[i];
      this.RefreshItemText();
    }

    protected override void RefreshItemTextCore()
    {
      this.MenuEntries[this.baseItemCount].Text = this.CountText;
      this.MenuEntries[this.baseItemCount + 2].Text = this.GetItemText(0);
      this.MenuEntries[this.baseItemCount + 3].Text = this.GetItemText(1);
      this.MenuEntries[this.baseItemCount + 4].Text = this.GetItemText(2);
      this.MenuEntries[this.baseItemCount + 5].Text = this.GetItemText(3);
      this.MenuEntries[this.baseItemCount + 6].Text = this.GetItemText(4);
      this.MenuEntries[this.baseItemCount + 7].Text = this.GetItemText(5);
      this.MenuEntries[this.baseItemCount + 8].Text = this.GetItemText(6);
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

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeTreesDefaults(this.data);
    }

    private void OnEnterCount(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnCountEntered), this.treeData.TreeCount, false), this.ControllingPlayer);
    }

    private void OnCountEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.treeData.TreeCount = MyMathHelper.Clamp((int) value, 1, 5000);
      this.RefreshItemText();
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
