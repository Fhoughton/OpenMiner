// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.NodeTree
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public class NodeTree
  {
    protected Node root;

    public Node Root
    {
      get
      {
        return this.root;
      }
    }

    public virtual void SetRoot(Node node)
    {
      this.root = node;
    }

    public int ChildCount
    {
      get
      {
        int num = 0;
        for (Node node = this.root; node != null; node = node.NextSibling)
          ++num;
        return num;
      }
    }

    public void AddChild(Node child)
    {
      if (child == null)
        return;
      if (this.root == null)
      {
        this.root = child;
      }
      else
      {
        if (child.Parent != null)
          child.ChangeParent((Node) null);
        this.root.AddSibling(child);
      }
    }
  }
}
