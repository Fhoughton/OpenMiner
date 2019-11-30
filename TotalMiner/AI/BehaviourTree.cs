// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.BehaviourTree
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Core;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public class BehaviourTree : NodeTree
  {
    public bool Immutable;
    private BehaviourTreeType treeType;

    public string Name { get; set; }

    public BehaviourTreeType TreeType
    {
      get
      {
        return this.treeType;
      }
    }

    public BehaviourTrackType TrackType { get; set; }

    public BehaviourTree(BehaviourTreeType treeType)
      : this(treeType, false)
    {
    }

    public BehaviourTree(BehaviourTreeType treeType, bool immutable)
    {
      this.treeType = treeType;
      this.Immutable = immutable;
    }

    private void SetNPC(INPCBehaviour npc)
    {
      BehaviourTreeNode root = this.root as BehaviourTreeNode;
      if (root == null)
        return;
      for (BehaviourTreeNode behaviourTreeNode = root; behaviourTreeNode != null; behaviourTreeNode = behaviourTreeNode.NextSibling as BehaviourTreeNode)
        behaviourTreeNode.SetNPC(npc);
    }

    public BehaviourTree Clone(INPCBehaviour npc)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          this.WriteState(writer);
          memoryStream.Position = 0L;
          using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
          {
            BehaviourTree behaviourTree = new BehaviourTree(this.treeType, false);
            behaviourTree.ReadState(reader, int.MaxValue);
            behaviourTree.SetNPC(npc);
            return behaviourTree;
          }
        }
      }
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.ReadStateCore(reader, version);
    }

    protected virtual void ReadStateCore(BinaryReader reader, int version)
    {
      this.Name = reader.ReadString();
      this.treeType = (BehaviourTreeType) reader.ReadByte();
      if (version < 254)
      {
        if (!reader.ReadBoolean())
          return;
        this.SetRoot(Node.Deserialize(reader, version));
      }
      else
      {
        int num = reader.ReadInt32();
        if (num <= 0)
          return;
        Node node = Node.Deserialize(reader, version);
        while (--num > 0)
          node.AddSibling(Node.Deserialize(reader, version));
        this.SetRoot(node);
      }
    }

    public void WriteState(BinaryWriter writer)
    {
      this.WriteStateCore(writer);
    }

    protected virtual void WriteStateCore(BinaryWriter writer)
    {
      writer.Write(this.Name != null ? this.Name : "");
      writer.Write((byte) this.treeType);
      writer.Write(this.ChildCount);
      for (BehaviourTreeNode behaviourTreeNode = this.root as BehaviourTreeNode; behaviourTreeNode != null; behaviourTreeNode = behaviourTreeNode.NextSibling as BehaviourTreeNode)
        behaviourTreeNode.WriteState(writer);
    }
  }
}
