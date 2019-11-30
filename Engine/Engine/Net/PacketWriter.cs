// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.PacketWriter
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using System.IO;

namespace StudioForge.Engine.Net
{
  public class PacketWriter : BinaryWriter
  {
    internal byte[] Data
    {
      get
      {
        return ((MemoryStream) this.BaseStream).GetBuffer();
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

    public PacketWriter()
      : this(0)
    {
    }

    public PacketWriter(int capacity)
      : base((Stream) new MemoryStream(capacity))
    {
    }

    public void WriteGamerID(Gamer gamer)
    {
      this.Write(gamer.ID.ID);
    }

    public void WriteGamerID(GamerID id)
    {
      this.Write(id.ID);
    }

    public void Write(Color value)
    {
      this.Write(value.PackedValue);
    }

    public override void Write(double value)
    {
      base.Write(value);
    }

    public void Write(Matrix value)
    {
      base.Write(value.M11);
      base.Write(value.M12);
      base.Write(value.M13);
      base.Write(value.M14);
      base.Write(value.M21);
      base.Write(value.M22);
      base.Write(value.M23);
      base.Write(value.M24);
      base.Write(value.M31);
      base.Write(value.M32);
      base.Write(value.M33);
      base.Write(value.M34);
      base.Write(value.M41);
      base.Write(value.M42);
      base.Write(value.M43);
      base.Write(value.M44);
    }

    public void Write(Quaternion value)
    {
      base.Write(value.X);
      base.Write(value.Y);
      base.Write(value.Z);
      base.Write(value.W);
    }

    public override void Write(float value)
    {
      base.Write(value);
    }

    public void Write(Vector2 value)
    {
      base.Write(value.X);
      base.Write(value.Y);
    }

    public void Write(Vector3 value)
    {
      base.Write(value.X);
      base.Write(value.Y);
      base.Write(value.Z);
    }

    public void Write(Vector4 value)
    {
      base.Write(value.X);
      base.Write(value.Y);
      base.Write(value.Z);
      base.Write(value.W);
    }

    internal void Reset()
    {
      MemoryStream baseStream = (MemoryStream) this.BaseStream;
      baseStream.SetLength(0L);
      baseStream.Position = 0L;
    }
  }
}
