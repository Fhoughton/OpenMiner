// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.ParticleData
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using System.IO;

namespace StudioForge.TotalMiner.Graphics
{
  public struct ParticleData
  {
    public string Name;
    public int EmitFreq;
    public ushort Duration;
    public float Rotation;
    public Vector3 Velocity;
    public Vector3 VelocityVariance;
    public Vector3 EmitPosOffset;
    public Vector3 EmitPosVariance;
    public Vector4 Size;
    public float WindFactor;
    public short Gravity;
    public Color StartColor;
    public Color EndColor;
    public ScriptCoordType VelocityType;
    public float Proximity;

    public override bool Equals(object obj)
    {
      ParticleData particleData = (ParticleData) obj;
      if (this.EmitFreq == particleData.EmitFreq && (int) this.Duration == (int) particleData.Duration && (this.Velocity == particleData.Velocity && this.VelocityType == particleData.VelocityType) && (this.VelocityVariance == particleData.VelocityVariance && this.EmitPosOffset == particleData.EmitPosOffset && (this.EmitPosVariance == particleData.EmitPosVariance && (double) this.Rotation == (double) particleData.Rotation)) && ((double) this.WindFactor == (double) particleData.WindFactor && (int) this.Gravity == (int) particleData.Gravity && (this.Size == particleData.Size && this.StartColor == particleData.StartColor)))
        return this.EndColor == particleData.EndColor;
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public void CopyFrom(ref ParticleData from)
    {
      this.EmitFreq = from.EmitFreq;
      this.Duration = from.Duration;
      this.Velocity = from.Velocity;
      this.VelocityType = from.VelocityType;
      this.VelocityVariance = from.VelocityVariance;
      this.EmitPosOffset = from.EmitPosOffset;
      this.EmitPosVariance = from.EmitPosVariance;
      this.Rotation = from.Rotation;
      this.WindFactor = from.WindFactor;
      this.Gravity = from.Gravity;
      this.Size = from.Size;
      this.StartColor.PackedValue = from.StartColor.PackedValue;
      this.EndColor.PackedValue = from.EndColor.PackedValue;
      this.Proximity = from.Proximity;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.EmitFreq = reader.ReadInt32();
      this.Duration = reader.ReadUInt16();
      this.Rotation = reader.ReadSingle();
      this.Velocity.X = reader.ReadSingle();
      this.Velocity.Y = reader.ReadSingle();
      this.Velocity.Z = reader.ReadSingle();
      this.VelocityVariance.X = reader.ReadSingle();
      this.VelocityVariance.Y = reader.ReadSingle();
      this.VelocityVariance.Z = reader.ReadSingle();
      this.EmitPosOffset.X = reader.ReadSingle();
      this.EmitPosOffset.Y = reader.ReadSingle();
      this.EmitPosOffset.Z = reader.ReadSingle();
      this.EmitPosVariance.X = reader.ReadSingle();
      this.EmitPosVariance.Y = reader.ReadSingle();
      this.EmitPosVariance.Z = reader.ReadSingle();
      this.Size.X = reader.ReadSingle();
      this.Size.Y = reader.ReadSingle();
      this.Size.Z = reader.ReadSingle();
      this.Size.W = reader.ReadSingle();
      double num1 = (double) reader.ReadSingle();
      double num2 = (double) reader.ReadSingle();
      this.WindFactor = reader.ReadSingle();
      this.Gravity = version > 230 ? reader.ReadInt16() : (short) 0;
      this.StartColor.PackedValue = reader.ReadUInt32();
      this.EndColor.PackedValue = reader.ReadUInt32();
      this.Proximity = version > 238 ? reader.ReadSingle() : 0.0f;
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.EmitFreq);
      writer.Write(this.Duration);
      writer.Write(this.Rotation);
      writer.Write(this.Velocity.X);
      writer.Write(this.Velocity.Y);
      writer.Write(this.Velocity.Z);
      writer.Write(this.VelocityVariance.X);
      writer.Write(this.VelocityVariance.Y);
      writer.Write(this.VelocityVariance.Z);
      writer.Write(this.EmitPosOffset.X);
      writer.Write(this.EmitPosOffset.Y);
      writer.Write(this.EmitPosOffset.Z);
      writer.Write(this.EmitPosVariance.X);
      writer.Write(this.EmitPosVariance.Y);
      writer.Write(this.EmitPosVariance.Z);
      writer.Write(this.Size.X);
      writer.Write(this.Size.Y);
      writer.Write(this.Size.Z);
      writer.Write(this.Size.W);
      writer.Write(0.0f);
      writer.Write(0.0f);
      writer.Write(this.WindFactor);
      writer.Write(this.Gravity);
      writer.Write(this.StartColor.PackedValue);
      writer.Write(this.EndColor.PackedValue);
      writer.Write(this.Proximity);
    }
  }
}
