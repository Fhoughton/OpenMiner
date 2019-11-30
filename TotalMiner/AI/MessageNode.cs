// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.MessageNode
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.GUI;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  [BehaviourTreeNode("Message", BehaviourTreeNodeType.Action)]
  internal class MessageNode : IsNpcTypeQueryNode
  {
    public float Duration = 1f;
    public MessageAction Action;
    public MessageType Type;
    public ushort Distance;

    public override string ToStringParms
    {
      get
      {
        return this.Action.ToString() + " " + this.Type.ToString() + " " + base.ToStringParms;
      }
    }

    public MessageNode()
    {
    }

    public MessageNode(INPCBehaviour npc)
      : base(npc)
    {
    }

    protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
    {
      if (this.npc == null)
      {
        this.Status = BehaviourTreeNodeStatus.Failure;
      }
      else
      {
        switch (this.Action)
        {
          case MessageAction.Send:
            this.SendMessage(engine.Tree.TreeType);
            break;
          case MessageAction.Receive:
            this.ReceiveMessage();
            break;
        }
        this.Status = BehaviourTreeNodeStatus.Success;
      }
    }

    private void SendMessage(BehaviourTreeType treeType)
    {
      MessagePacket packet = (MessagePacket) null;
      if (this.Type == MessageType.IsTargeted)
      {
        INPCBehaviour target = this.GetTarget(treeType, this.CompareTarget);
        if (target == null)
          return;
        packet = (MessagePacket) new MessagePacketUnderAttack()
        {
          Target = target
        };
      }
      if (packet == null)
        return;
      packet.Type = this.Type;
      packet.Distance = this.Distance;
      packet.Position = this.npc.Position;
      packet.Recipients = this.SearchTypes;
      if (packet.Recipients != null && packet.Recipients.Count == 0)
        packet.Recipients = (List<ActorType>) null;
      packet.Exclude = this.ExcludeTypes;
      if (packet.Exclude != null && packet.Exclude.Count == 0)
        packet.Exclude = (List<ActorType>) null;
      packet.Duration = this.Duration;
      MessageQueue.SendMessage(this.npc, packet);
    }

    private void ReceiveMessage()
    {
      List<MessagePacket> messagePacketList = MessageQueue.RecieveMessages(this.Type);
      if (messagePacketList == null)
        return;
      foreach (MessagePacket msg in messagePacketList)
      {
        if (msg.Type == this.Type && (msg.Recipients == null || msg.Recipients.Contains(this.npc.ActorType)) && ((msg.Exclude == null || !msg.Exclude.Contains(this.npc.ActorType)) && (msg.Distance == (ushort) 0 || (double) Vector3.DistanceSquared(msg.Position, this.npc.Position) <= (double) ((int) msg.Distance * (int) msg.Distance))))
          this.ProcessMessage(msg);
      }
    }

    private bool ProcessMessage(MessagePacket msg)
    {
      if (msg.Type != MessageType.IsTargeted)
        return false;
      MessagePacketUnderAttack packetUnderAttack = msg as MessagePacketUnderAttack;
      if (packetUnderAttack != null && packetUnderAttack.Target != null && packetUnderAttack.Target.IsAlive)
        TargetingSystem.Target(packetUnderAttack.Target, this.npc, 1200, NpcQueryPreference.Agressive);
      return true;
    }

    public override bool IsPropertyEnabled(string propertyName)
    {
      switch (propertyName)
      {
        case "Duration":
        case "Distance":
        case "CompareTarget":
          return this.Action == MessageAction.Send;
        default:
          return base.IsPropertyEnabled(propertyName);
      }
    }

    public override void SetPropertyEditorDefaults(string name, Window win)
    {
      base.SetPropertyEditorDefaults(name, win);
      switch (name)
      {
        case "Duration":
          win.SetToolTip("The amount of time in seconds the message is kept alive.\n\nThis property is only used for Sent messages");
          break;
        case "Distance":
          win.SetToolTip("The distance from the senders position that the message can be received. Zero = any distance.\n\nThis property is only used for Sent messages");
          break;
        case "CompareTarget":
          win.SetToolTip("The entity the receivers of the message should target.\n\nThis property is only used for Sent messages");
          break;
        case "SearchTypes":
          win.SetToolTip("If the message is being sent, it will only be sent to these types.\n\nIf the message is being received, only messages sent by these types can be received.\n\nLeave empty for all types");
          break;
        case "ExcludeTypes":
          win.SetToolTip("If the message is being sent, it will not be sent to these types.\n\nIf the message is being received, messages sent by these types will be ignored.");
          break;
      }
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Action = (MessageAction) reader.ReadByte();
      this.Type = (MessageType) reader.ReadByte();
      this.Duration = version <= 270 ? (float) reader.ReadByte() / 60f : reader.ReadSingle();
      if (version > 267)
        this.Distance = reader.ReadUInt16();
      else
        this.Distance = (ushort) 0;
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write((byte) this.Action);
      writer.Write((byte) this.Type);
      writer.Write(this.Duration);
      writer.Write(this.Distance);
    }
  }
}
