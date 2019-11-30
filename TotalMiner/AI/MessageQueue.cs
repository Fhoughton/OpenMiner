// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.MessageQueue
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.AI
{
  internal static class MessageQueue
  {
    private static Dictionary<MessageType, List<MessagePacket>> messages;

    public static int MessageCount
    {
      get
      {
        int num = 0;
        if (MessageQueue.messages != null)
        {
          foreach (KeyValuePair<MessageType, List<MessagePacket>> message in MessageQueue.messages)
            num += message.Value.Count;
        }
        return num;
      }
    }

    public static void Initialize()
    {
      if (MessageQueue.messages == null)
        MessageQueue.messages = new Dictionary<MessageType, List<MessagePacket>>();
      else
        MessageQueue.messages.Clear();
    }

    public static void Update()
    {
      float elapsedTime = Services.ElapsedTime;
      foreach (KeyValuePair<MessageType, List<MessagePacket>> message in MessageQueue.messages)
      {
        List<MessagePacket> messagePacketList = message.Value;
        if (messagePacketList != null && messagePacketList.Count > 0)
        {
          for (int index = messagePacketList.Count - 1; index >= 0; --index)
          {
            MessagePacket messagePacket = messagePacketList[index];
            messagePacket.Duration -= elapsedTime;
            if ((double) messagePacket.Duration <= 0.0)
              messagePacketList.RemoveAt(index);
            else
              messagePacketList[index] = messagePacket;
          }
        }
      }
    }

    public static void SendMessage(INPCBehaviour sender, MessagePacket packet)
    {
      if (sender == null || packet == null)
        return;
      packet.Sender = sender;
      List<MessagePacket> messagePacketList;
      if (!MessageQueue.messages.TryGetValue(packet.Type, out messagePacketList))
      {
        messagePacketList = new List<MessagePacket>();
        messagePacketList.Add(packet);
        MessageQueue.messages.Add(packet.Type, messagePacketList);
      }
      else
      {
        bool flag = false;
        for (int index = messagePacketList.Count - 1; index >= 0; --index)
        {
          if (messagePacketList[index].Sender == sender)
          {
            messagePacketList[index] = packet;
            flag = true;
            break;
          }
        }
        if (flag)
          return;
        messagePacketList.Add(packet);
      }
    }

    public static List<MessagePacket> RecieveMessages(MessageType type)
    {
      List<MessagePacket> messagePacketList = (List<MessagePacket>) null;
      MessageQueue.messages.TryGetValue(type, out messagePacketList);
      return messagePacketList;
    }
  }
}
