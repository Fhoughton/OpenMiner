// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.BehaviourTreeNodeAttribute
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System;

namespace StudioForge.TotalMiner.AI
{
  [AttributeUsage(AttributeTargets.Class)]
  public class BehaviourTreeNodeAttribute : Attribute
  {
    public string Name;
    public BehaviourTreeNodeType Type;
    public bool IsImplemented;

    public BehaviourTreeNodeAttribute(string name, BehaviourTreeNodeType type)
    {
      this.Name = name;
      this.Type = type;
      this.IsImplemented = true;
    }
  }
}
