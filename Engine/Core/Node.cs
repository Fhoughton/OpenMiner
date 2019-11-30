// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Node
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.IO;
using System.Reflection;

namespace StudioForge.Engine.Core
{
  public class Node
  {
    protected Node parent;
    protected Node firstChild;
    protected Node prevSibling;
    protected Node nextSibling;

    public Node Parent
    {
      get
      {
        return this.parent;
      }
    }

    public Node FirstChild
    {
      get
      {
        return this.firstChild;
      }
    }

    public Node NextSibling
    {
      get
      {
        return this.nextSibling;
      }
    }

    public Node PrevSibling
    {
      get
      {
        return this.prevSibling;
      }
    }

    public bool IsLeafNode
    {
      get
      {
        return this.firstChild == null;
      }
    }

    public bool HasChildren
    {
      get
      {
        return this.firstChild != null;
      }
    }

    public int ChildCount
    {
      get
      {
        int num = 0;
        for (Node node = this.firstChild; node != null; node = node.nextSibling)
          ++num;
        return num;
      }
    }

    public int SiblingCount
    {
      get
      {
        int num = 0;
        for (Node prevSibling = this.prevSibling; prevSibling != null && prevSibling != this; prevSibling = prevSibling.prevSibling)
          ++num;
        return num;
      }
    }

    public Node GetSibling(int count)
    {
      Node node;
      for (node = this; count > 0 && node != null; --count)
        node = node.nextSibling;
      return node;
    }

    public bool IsChildOf(Node node)
    {
      for (Node parent = this.parent; parent != null; parent = parent.parent)
      {
        if (parent == node)
          return true;
      }
      return false;
    }

    public void ChangeParent(Node newParent)
    {
      this.RemoveSelf();
      newParent?.AddChild(this);
    }

    public void AddChild(Node child)
    {
      if (child == null || this.IsChildOf(child))
        return;
      child.parent = this;
      child.nextSibling = child.prevSibling = (Node) null;
      if (this.firstChild == null)
        this.firstChild = child;
      else if (this.firstChild.prevSibling != null)
      {
        child.prevSibling = this.firstChild.prevSibling;
        this.firstChild.prevSibling.nextSibling = child;
        this.firstChild.prevSibling = child;
      }
      else
      {
        this.firstChild.nextSibling = this.firstChild.prevSibling = child;
        child.prevSibling = this.firstChild;
      }
    }

    public void AddSibling(Node node)
    {
      if (node == null)
        return;
      node.parent = this.parent;
      node.nextSibling = (Node) null;
      if (this.prevSibling == null)
      {
        node.prevSibling = this;
        this.nextSibling = this.prevSibling = node;
      }
      else
      {
        Node node1 = this;
        Node prevSibling;
        for (prevSibling = this.prevSibling; prevSibling.nextSibling != null; prevSibling = prevSibling.prevSibling)
          node1 = prevSibling;
        node.prevSibling = prevSibling;
        prevSibling.nextSibling = node;
        node1.prevSibling = node;
      }
    }

    public void InsertNode(Node prevSibling, Node node)
    {
      if (node == null)
        return;
      node.parent = this;
      if ((node.prevSibling = prevSibling) != null)
      {
        if (prevSibling.nextSibling != null)
          prevSibling.nextSibling.prevSibling = node;
        node.nextSibling = prevSibling.nextSibling;
        prevSibling.nextSibling = node;
      }
      else
      {
        node.nextSibling = this.firstChild;
        node.prevSibling = this.firstChild.prevSibling;
        this.firstChild.prevSibling = node;
        this.firstChild = node;
      }
    }

    public bool SwapNode(Node node1, Node node2)
    {
      if (node1 == null || node2 == null || node1 == node2)
        return false;
      if (node1 != this.firstChild && node1.prevSibling != node2)
        node1.prevSibling.nextSibling = node2;
      if (node2 != this.firstChild && node2.prevSibling != node1)
        node2.prevSibling.nextSibling = node1;
      if (node1.nextSibling != null && node1.nextSibling != node2)
        node1.nextSibling.prevSibling = node2;
      if (node2.nextSibling != null && node2.nextSibling != node1)
        node2.nextSibling.prevSibling = node1;
      Node prevSibling = node1.prevSibling;
      Node nextSibling = node1.nextSibling;
      node1.prevSibling = node2.prevSibling;
      node1.nextSibling = node2.nextSibling;
      node2.prevSibling = prevSibling;
      node2.nextSibling = nextSibling;
      if (node1.prevSibling == node1)
        node1.prevSibling = node2;
      if (node2.prevSibling == node2)
        node2.prevSibling = node1;
      if (node1.nextSibling == node1)
        node1.nextSibling = node2;
      if (node2.nextSibling == node2)
        node2.nextSibling = node1;
      if (this.firstChild == node1)
        this.firstChild = node2;
      else if (this.firstChild == node2)
        this.firstChild = node1;
      return true;
    }

    public void RemoveChild(Node child)
    {
      if (child == null || child.parent != this)
        return;
      if (this.firstChild == child)
      {
        this.firstChild = child.nextSibling;
        if (this.firstChild != null && (this.firstChild.prevSibling = child.prevSibling) == this.firstChild)
          this.firstChild.prevSibling = (Node) null;
      }
      else
      {
        if (child.prevSibling != null)
          child.prevSibling.nextSibling = child.nextSibling;
        if (child.nextSibling != null)
          child.nextSibling.prevSibling = child.prevSibling;
        if (this.firstChild.prevSibling == child && (this.firstChild.prevSibling = child.prevSibling) == this.firstChild)
          this.firstChild.prevSibling = (Node) null;
      }
      child.prevSibling = (Node) null;
      child.nextSibling = (Node) null;
      child.parent = (Node) null;
    }

    public void RemoveSelf()
    {
      if (this.parent == null)
        return;
      this.parent.RemoveChild(this);
    }

    public void RemoveAllChildren()
    {
      this.firstChild = (Node) null;
    }

    public void ClearPointers()
    {
      if (this.firstChild != null)
        this.firstChild.ClearPointers();
      Node node = this.nextSibling;
      this.nextSibling = (Node) null;
      this.prevSibling = (Node) null;
      this.parent = (Node) null;
      Node nextSibling;
      for (; node != null; node = nextSibling)
      {
        nextSibling = node.nextSibling;
        if (node.firstChild != null)
          node.firstChild.ClearPointers();
        node.prevSibling = (Node) null;
        node.nextSibling = (Node) null;
        node.parent = (Node) null;
      }
    }

    public void ReplaceWith(Node node)
    {
      if (node == null)
        return;
      node.parent = this.parent;
      if (this.parent.firstChild == this)
      {
        node.prevSibling = this.parent.firstChild.prevSibling;
        if ((node.nextSibling = this.nextSibling) != null)
          this.nextSibling.prevSibling = node;
        this.parent.firstChild = node;
      }
      else
      {
        if ((node.prevSibling = this.prevSibling) != null)
          this.prevSibling.nextSibling = node;
        if ((node.nextSibling = this.nextSibling) == null)
          return;
        this.nextSibling.prevSibling = node;
      }
    }

    public static Node Clone(Node top)
    {
      if (top == null)
        return (Node) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          top.WriteState(writer);
          memoryStream.Position = 0L;
          using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
            return Node.Deserialize(reader, int.MaxValue);
        }
      }
    }

    public static Node Deserialize(BinaryReader reader, int version)
    {
      string typeName = reader.ReadString();
      if (version < 259)
        typeName += ", StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
      else if (version < 280)
        typeName = typeName.Replace("Craig.", "StudioForge.");
      Type type = Type.GetType(typeName);
      if (type == (Type) null)
      {
        if (typeName.Contains(", StudioForge.TotalMiner, "))
        {
          typeName = typeName.Replace(", StudioForge.TotalMiner, ", ", StudioForge.TotalMiner.API, ");
          type = Type.GetType(typeName);
        }
        if (type == (Type) null)
        {
          type = Node.GetType(typeName);
          if (type == (Type) null)
            return (Node) null;
        }
      }
      Node instance = Activator.CreateInstance(type) as Node;
      if (instance == null)
        return (Node) null;
      instance.ReadState(reader, version);
      return instance;
    }

    private static Type GetType(string typeName)
    {
      int length = typeName.IndexOf(',');
      if (length > 0)
      {
        string aname = typeName.Substring(length + 1).Trim();
        Assembly assembly = Array.Find<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), (Predicate<Assembly>) (i => i.FullName == aname));
        if (assembly != (Assembly) null)
        {
          string tname = typeName.Substring(0, length).Trim();
          return Array.Find<Type>(assembly.GetTypes(), (Predicate<Type>) (i => i.FullName == tname));
        }
      }
      return (Type) null;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.ReadStateCore(reader, version);
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
        this.AddChild(Node.Deserialize(reader, version));
    }

    protected virtual void ReadStateCore(BinaryReader reader, int version)
    {
    }

    public void WriteState(BinaryWriter writer)
    {
      string assemblyQualifiedName = this.GetType().AssemblyQualifiedName;
      writer.Write(assemblyQualifiedName);
      this.WriteStateCore(writer);
      if (this.firstChild != null && this.ShouldWriteChildren)
      {
        writer.Write(this.ChildCount);
        for (Node node = this.firstChild; node != null; node = node.nextSibling)
          node.WriteState(writer);
      }
      else
        writer.Write(0);
    }

    protected virtual bool ShouldWriteChildren
    {
      get
      {
        return true;
      }
    }

    protected virtual void WriteStateCore(BinaryWriter writer)
    {
    }

    public static Node GetParent(Type type, Node node)
    {
      if (node == null)
        return (Node) null;
      node = node.parent;
      while (node != null && type != node.GetType())
        node = node.parent;
      return node;
    }

    public static int GetChildCount(Type type, Node node)
    {
      if (node == null)
        return 0;
      int num = 0;
      for (node = node.firstChild; node != null; node = node.nextSibling)
      {
        if (type == node.GetType())
          ++num;
      }
      return num;
    }

    public static int GetSiblingCount(Type type, Node node)
    {
      if (node == null)
        return 0;
      int num = 0;
      for (Node prevSibling = node.prevSibling; prevSibling != null && prevSibling != node; prevSibling = prevSibling.prevSibling)
      {
        if (type == prevSibling.GetType())
          ++num;
      }
      return num;
    }

    public static Node FindFirst(Type type, Node node)
    {
      if (node == null)
        return (Node) null;
      if (type == node.GetType())
        return node;
      for (node = node.firstChild; node != null; node = node.nextSibling)
      {
        Node first = Node.FindFirst(type, node);
        if (first != null)
          return first;
      }
      return (Node) null;
    }

    public static Node FindFirstChild(Type type, Node node)
    {
      if (node == null)
        return (Node) null;
      for (node = node.firstChild; node != null; node = node.nextSibling)
      {
        Node first = Node.FindFirst(type, node);
        if (first != null)
          return first;
      }
      return (Node) null;
    }

    public static Node FindNextSibling(Type type, Node node)
    {
      return Node.FindNextSibling(type, node, 1);
    }

    public static Node FindNextSibling(Type type, Node node, int count)
    {
      while (node != null && count > 0)
      {
        node = node.nextSibling;
        if (node != null && type == node.GetType())
          --count;
      }
      return node;
    }

    public static Node FindPrevSibling(Type type, Node node)
    {
      return Node.FindPrevSibling(type, node, 1);
    }

    public static Node FindPrevSibling(Type type, Node node, int count)
    {
      while (node != null && count > 0)
      {
        node = node.prevSibling;
        if (node != null && type == node.GetType())
          --count;
      }
      return node;
    }

    public static Node FindSibling(Type type, Node node, int count)
    {
      return Node.FindPrevSibling(type, node, count);
    }
  }
}
