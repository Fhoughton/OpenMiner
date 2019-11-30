// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.PacketReader
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using System.IO;

namespace StudioForge.Engine.Net
{
  public class PacketReader : BinaryReader
  {
    internal byte[] Data
    {
      get
      {
        return ((MemoryStream) this.BaseStream).GetBuffer();
      }
      set
      {
        this.BaseStream.Write(value, 0, value.Length);
      }
    }

    public int Length
    {
      get
      {
        return (int) this.BaseStream.Length;
      }
    }

    public int Position
    {
      get
      {
        return (int) this.BaseStream.Position;
      }
      set
      {
        this.BaseStream.Position = (long) value;
      }
    }

    public PacketReader()
      : this(0)
    {
    }

    public PacketReader(int capacity)
      : base((Stream) new MemoryStream(capacity))
    {
    }

    public PacketReader(Stream stream)
      : base(stream)
    {
    }

    public GamerID ReadGamerID()
    {
      return new GamerID(this.ReadInt16());
    }

    public Color ReadColor()
    {
      return new Color() { PackedValue = this.ReadUInt32() };
    }

    public override double ReadDouble()
    {
      return base.ReadDouble();
    }

    public Matrix ReadMatrix()
    {
      return new Matrix()
      {
        M11 = this.ReadSingle(),
        M12 = this.ReadSingle(),
        M13 = this.ReadSingle(),
        M14 = this.ReadSingle(),
        M21 = this.ReadSingle(),
        M22 = this.ReadSingle(),
        M23 = this.ReadSingle(),
        M24 = this.ReadSingle(),
        M31 = this.ReadSingle(),
        M32 = this.ReadSingle(),
        M33 = this.ReadSingle(),
        M34 = this.ReadSingle(),
        M41 = this.ReadSingle(),
        M42 = this.ReadSingle(),
        M43 = this.ReadSingle(),
        M44 = this.ReadSingle()
      };
    }

    public Quaternion ReadQuaternion()
    {
      return new Quaternion()
      {
        X = this.ReadSingle(),
        Y = this.ReadSingle(),
        Z = this.ReadSingle(),
        W = this.ReadSingle()
      };
    }

    public Vector2 ReadVector2()
    {
      return new Vector2()
      {
        X = this.ReadSingle(),
        Y = this.ReadSingle()
      };
    }

    public Vector3 ReadVector3()
    {
      return new Vector3()
      {
        X = this.ReadSingle(),
        Y = this.ReadSingle(),
        Z = this.ReadSingle()
      };
    }

    public Vector4 ReadVector4()
    {
      return new Vector4()
      {
        X = this.ReadSingle(),
        Y = this.ReadSingle(),
        Z = this.ReadSingle(),
        W = this.ReadSingle()
      };
    }

    internal void Reset(int size)
    {
      MemoryStream baseStream = (MemoryStream) this.BaseStream;
      baseStream.SetLength((long) size);
      baseStream.Position = 0L;
    }
  }
}
