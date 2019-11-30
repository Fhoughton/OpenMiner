// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.AIBehaviourTreeDesigner
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
  internal class AIBehaviourTreeDesigner : BehaviourTreeDesigner
  {
    public AIBehaviourTreeDesigner(
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
      this.tooltips.Add("Attack", "Force the NPC to attack the entity it is currently targeting");
      this.tooltips.Add("ChangeState", "Change the NPCs state");
      this.tooltips.Add("Deactivate", "Deactivate the NPC if it is a specified distance away from any player");
      this.tooltips.Add("Equip", "Equip exact items, a type of item or a sub type of item into either hand or a body slot");
      this.tooltips.Add("Flee", "Force the NPC to flee from the entity it is currently targeting");
      this.tooltips.Add("Follow", "Force the NPC to follow the entity it is currently targeting");
      this.tooltips.Add("Health", "Manipulate current health");
      this.tooltips.Add("Jump", "Force the NPC to jump");
      this.tooltips.Add("LookAt", "Force the NPC to look at a specified location or entity");
      this.tooltips.Add("Message", "Send and receive messages between NPCs. This node allows structured communication between NPC types");
      this.tooltips.Add("Properties", "Change one or more properties that define an NPCs behaviour");
      this.tooltips.Add("Script", "Execute an existing script or embed one or more script commands directly into the node");
      this.tooltips.Add("StandStill", "Force the NPC to stop moving");
      this.tooltips.Add("SwingHand", "Force the NPC to swing a hand.\n\nThis will activate the item currently in that hand. e.g. If the NPC has food in that hand, it will eat the food.");
      this.tooltips.Add("Timer", "This node will fail until the specified amount of time has elapsed.\n\nOnce the time has elapsed, the node will pass and the timer will be reset so that it will fail until the specified time has elapsed again");
      this.tooltips.Add("Wait", "Force the NPC AI to stop executing for a specified amount of time.\n\nOnce the time has elapsed, the node will pass and execution will continue as normal. The timer will be reset so that the next time it is executed, it will again wait until the specified time has elapsed");
      this.tooltips.Add("Wander", "Have the NPC wander randomly about a specified location");
      this.tooltips.Add("Waypoint", "Have the NPC follow a path of waypoints");
      this.tooltips.Add("FindTarget", "This node has an undefined tool tip");
      this.tooltips.Add("HasHistory", "Query history");
      this.tooltips.Add("HasInventory", "Query inventory");
      this.tooltips.Add("IsAge", "Query current age");
      this.tooltips.Add("IsBlock", "Query the block an entity is currently targeting");
      this.tooltips.Add("IsDistance", "Query the distance between entities or locations");
      this.tooltips.Add("IsEquipType", "Query the type of item equipped");
      this.tooltips.Add("IsHealth", "Query current health");
      this.tooltips.Add("IsInZone", "Query if the NPC is inside a zone or zone type");
      this.tooltips.Add("IsRandom", "Randomize the behaviours control flow");
      this.tooltips.Add("IsTargeted", "Query if the NPC is being targeted by another entity");
      this.tooltips.Add("IsVisible", "Query if the NPC can see an entity");
      this.tooltips.Add("Exit", "Force the behaviour tree to restart");
      this.tooltips.Add("Parallel", "This node has an undefined tool tip");
      this.tooltips.Add("Proxy", "Import behaviour from other behaviour trees");
      this.tooltips.Add("Update", "The main update loop for the NPC behaviour. Each NPC should have only one Update node and it is usually the first node. Any nodes before the Update node will only execute once");
    }

    protected override TreeDesignWindow GetNewTreeDesignWindow()
    {
      AIBehaviourTreeDesignWindow treeDesignWindow = new AIBehaviourTreeDesignWindow(this.playerIndex, this.screenRect.X, this.screenRect.Y, this.screenRect.Width, this.screenRect.Height, this.tree, (NpcBase) null, this.iconTextures, new Point(320, 300));
      treeDesignWindow.Name = "DesignWin";
      treeDesignWindow.Colors = Window.TransparentColorProfile;
      return (TreeDesignWindow) treeDesignWindow;
    }

    protected override void InitNodeListContainer()
    {
      int x1 = 0;
      int y1 = (int) this.mainMenuContainer.Position.Y + this.mainMenuContainer.Size.Y + 4;
      int height = 28;
      int num = 1;
      float textScale = 0.5f;
      DesignerNodeTagType[] nodeTypes = this.GetNodeTypes();
      this.nodeListContainer = new Window((string) null, x1, y1, 1, 1)
      {
        Name = "NodeContainer"
      };
      this.nodeListContainer.Colors = Colors.NodeContainer;
      this.nodeListContainer.BorderThickness = 1;
      this.BaseWindow.AddChild((StudioForge.Engine.Core.Node) this.nodeListContainer);
      int y2;
      int x2 = y2 = 0;
      float dragEnablePressTime = Window.DefaultDragEnablePressTime;
      Window.DefaultDragEnablePressTime = 0.0f;
      foreach (DesignerNodeTagType designerNodeTagType in nodeTypes)
      {
        bool flag = designerNodeTagType.Type == (Type) null;
        if (flag && designerNodeTagType.Name == "Action")
        {
          y2 = 0;
          x2 += this.nodeListContainer.Size.X + 3;
        }
        TextBox textBox = new TextBox(designerNodeTagType.Name, x2, y2, 150, height, textScale);
        textBox.IsEnabled = designerNodeTagType.IsImplemented;
        if (!flag)
          textBox.AddFlags(Window.WinFlags.IsDragable);
        textBox.Colors = flag ? (Window.ColorProfile) Colors.NodeHeader : (Window.ColorProfile) Colors.NodeType;
        textBox.Tag = (object) designerNodeTagType.Type;
        if (!flag)
        {
          textBox.DragStartHandler += new Window.WindowDragHandler(((TreeDesigner) this).OnNodeTypeDragStart);
          textBox.DragEndHandler += new Window.WindowDragHandler(((TreeDesigner) this).OnNodeTypeDragEnd);
          string tip;
          if (this.tooltips != null && this.tooltips.TryGetValue(designerNodeTagType.Name, out tip))
            textBox.SetToolTip(tip, TreeDesigner.ToolTipDelay);
        }
        this.nodeListContainer.AddChild((StudioForge.Engine.Core.Node) textBox);
        y2 += height + num;
        this.nodeListContainer.Size = new Point((int) Math.Max((float) this.nodeListContainer.Size.X, textBox.Position.X + (float) textBox.Size.X), (int) ((double) textBox.Position.Y + (double) textBox.Size.Y));
      }
      Window.DefaultDragEnablePressTime = dragEnablePressTime;
    }

    protected override bool IsValidNodeType(Type t)
    {
      BehaviourTreeNodeType nodeTypeEnum = BehaviourTreeNode.GetNodeTypeEnum(t);
      if (nodeTypeEnum != BehaviourTreeNodeType.None)
        return nodeTypeEnum != BehaviourTreeNodeType.Dialog;
      return false;
    }

    protected override void SaveBehaviours()
    {
      base.SaveBehaviours();
      GameInstance.Instance.NotifyBehaviourChanged(this.tree.Name);
    }
  }
}
