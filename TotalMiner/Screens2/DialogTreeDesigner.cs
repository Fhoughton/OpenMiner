// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.DialogTreeDesigner
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.AI;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens2
{
  internal class DialogTreeDesigner : BehaviourTreeDesigner
  {
    protected override bool OneToolBarRowPerType
    {
      get
      {
        return false;
      }
    }

    public DialogTreeDesigner(
      PlayerIndex playerIndex,
      Window parent,
      BehaviourTree tree,
      Action exitScreen)
      : base(playerIndex, parent, tree, exitScreen)
    {
    }

    protected override void InitTooltips()
    {
      this.tooltips = new Dictionary<string, string>();
      this.tooltips.Add("Dialog", "A single dialog option");
      this.tooltips.Add("Equip", "Equip a specified item into a specified hand or body slot");
      this.tooltips.Add("Health", "Manipulate current health");
      this.tooltips.Add("Message", "This node has an undefined tool tip");
      this.tooltips.Add("Properties", "Change one or more properties that define an NPCs behaviour");
      this.tooltips.Add("Script", "Execute an existing script or embed one or more script commands directly into the node");
      this.tooltips.Add("HasHistory", "Query history");
      this.tooltips.Add("HasInventory", "Query inventory");
      this.tooltips.Add("IsAge", "Query current age");
      this.tooltips.Add("IsEquipType", "Query the type of item equipped");
      this.tooltips.Add("IsHealth", "Query current health");
      this.tooltips.Add("IsRandom", "Randomize the dialogs control flow");
      this.tooltips.Add("Exit", "Exit the dialog");
      this.tooltips.Add("Proxy", "Import dialog from other dialog trees");
    }

    protected override TreeDesignWindow GetNewTreeDesignWindow()
    {
      DialogTreeDesignWindow treeDesignWindow = new DialogTreeDesignWindow(this.playerIndex, this.screenRect.X, this.screenRect.Y, this.screenRect.Width, this.screenRect.Height, this.tree, this.iconTextures, new Point(320, 300));
      treeDesignWindow.Name = "DesignWin";
      treeDesignWindow.Colors = Window.TransparentColorProfile;
      return (TreeDesignWindow) treeDesignWindow;
    }

    protected override bool IsValidNodeType(Type t)
    {
      if (BehaviourTreeNode.GetNodeTypeEnum(t) != BehaviourTreeNodeType.Dialog && !(t == typeof (HasHistoryNode)) && (!(t == typeof (HasInventoryNode)) && !(t == typeof (HealthNode))) && (!(t == typeof (IsAgeNode)) && !(t == typeof (IsEquipTypeNode)) && (!(t == typeof (IsHealthNode)) && !(t == typeof (IsRandomNode)))) && (!(t == typeof (ExitNode)) && !(t == typeof (ProxyNode)) && (!(t == typeof (LoadNode)) && !(t == typeof (EquipNode))) && (!(t == typeof (MessageNode)) && !(t == typeof (PropertiesNode)))))
        return t == typeof (ScriptNode);
      return true;
    }
  }
}
