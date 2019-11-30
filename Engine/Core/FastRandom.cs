// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.FastRandom
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.Core
{
  public class FastRandom : IRandom
  {
    private uint bitMask = 1;
    private const double REAL_UNIT_INT = 4.65661287307739E-10;
    private const double REAL_UNIT_UINT = 2.3283064365387E-10;
    private const uint Y = 842502087;
    private const uint Z = 3579807591;
    private const uint W = 273326509;
    private uint x;
    private uint y;
    private uint z;
    private uint w;
    private uint bitBuffer;

    public FastRandom()
    {
      this.Reinitialize(Environment.TickCount);
    }

    public FastRandom(int seed)
    {
      this.Reinitialize(seed);
    }

    public void Reinitialize()
    {
      this.Reinitialize(Environment.TickCount);
    }

    public void Reinitialize(int seed)
    {
      this.x = (uint) seed;
      this.y = 842502087U;
      this.z = 3579807591U;
      this.w = 273326509U;
    }

    public int Next()
    {
      uint num1 = this.x ^ this.x << 11;
      this.x = this.y;
      this.y = this.z;
      this.z = this.w;
      this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num1 ^ (int) (num1 >> 8)));
      uint num2 = this.w & (uint) int.MaxValue;
      if (num2 == (uint) int.MaxValue)
        return this.Next();
      return (int) num2;
    }

    public int Next(int upperBound)
    {
      if (upperBound < 0)
        throw new ArgumentOutOfRangeException(nameof (upperBound));
      uint num = this.x ^ this.x << 11;
      this.x = this.y;
      this.y = this.z;
      this.z = this.w;
      return (int) (4.65661287307739E-10 * (double) (int.MaxValue & (int) (this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num ^ (int) (num >> 8))))) * (double) upperBound);
    }

    public int Next(int lowerBound, int upperBound)
    {
      if (lowerBound > upperBound)
        throw new ArgumentOutOfRangeException(nameof (upperBound));
      uint num1 = this.x ^ this.x << 11;
      this.x = this.y;
      this.y = this.z;
      this.z = this.w;
      int num2 = upperBound - lowerBound;
      if (num2 < 0)
        return lowerBound + (int) (2.3283064365387E-10 * (double) (this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num1 ^ (int) (num1 >> 8)))) * (double) ((long) upperBound - (long) lowerBound));
      return lowerBound + (int) (4.65661287307739E-10 * (double) (int.MaxValue & (int) (this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num1 ^ (int) (num1 >> 8))))) * (double) num2);
    }

    public double NextDouble()
    {
      uint num = this.x ^ this.x << 11;
      this.x = this.y;
      this.y = this.z;
      this.z = this.w;
      return 4.65661287307739E-10 * (double) (int.MaxValue & (int) (this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num ^ (int) (num >> 8)))));
    }

    public void NextBytes(byte[] buffer)
    {
      uint num1 = this.x;
      uint num2 = this.y;
      uint num3 = this.z;
      uint num4 = this.w;
      int num5 = 0;
      int num6 = buffer.Length - 3;
      while (num5 < num6)
      {
        uint num7 = num1 ^ num1 << 11;
        num1 = num2;
        num2 = num3;
        num3 = num4;
        num4 = (uint) ((int) num4 ^ (int) (num4 >> 19) ^ ((int) num7 ^ (int) (num7 >> 8)));
        byte[] numArray1 = buffer;
        int index1 = num5;
        int num8 = index1 + 1;
        int num9 = (int) (byte) num4;
        numArray1[index1] = (byte) num9;
        byte[] numArray2 = buffer;
        int index2 = num8;
        int num10 = index2 + 1;
        int num11 = (int) (byte) (num4 >> 8);
        numArray2[index2] = (byte) num11;
        byte[] numArray3 = buffer;
        int index3 = num10;
        int num12 = index3 + 1;
        int num13 = (int) (byte) (num4 >> 16);
        numArray3[index3] = (byte) num13;
        byte[] numArray4 = buffer;
        int index4 = num12;
        num5 = index4 + 1;
        int num14 = (int) (byte) (num4 >> 24);
        numArray4[index4] = (byte) num14;
      }
      if (num5 < buffer.Length)
      {
        uint num7 = num1 ^ num1 << 11;
        num1 = num2;
        num2 = num3;
        num3 = num4;
        num4 = (uint) ((int) num4 ^ (int) (num4 >> 19) ^ ((int) num7 ^ (int) (num7 >> 8)));
        byte[] numArray1 = buffer;
        int index1 = num5;
        int num8 = index1 + 1;
        int num9 = (int) (byte) num4;
        numArray1[index1] = (byte) num9;
        if (num8 < buffer.Length)
        {
          byte[] numArray2 = buffer;
          int index2 = num8;
          int num10 = index2 + 1;
          int num11 = (int) (byte) (num4 >> 8);
          numArray2[index2] = (byte) num11;
          if (num10 < buffer.Length)
          {
            byte[] numArray3 = buffer;
            int index3 = num10;
            int index4 = index3 + 1;
            int num12 = (int) (byte) (num4 >> 16);
            numArray3[index3] = (byte) num12;
            if (index4 < buffer.Length)
              buffer[index4] = (byte) (num4 >> 24);
          }
        }
      }
      this.x = num1;
      this.y = num2;
      this.z = num3;
      this.w = num4;
    }

    public uint NextUInt()
    {
      uint num = this.x ^ this.x << 11;
      this.x = this.y;
      this.y = this.z;
      this.z = this.w;
      return this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num ^ (int) (num >> 8)));
    }

    public int NextInt()
    {
      uint num = this.x ^ this.x << 11;
      this.x = this.y;
      this.y = this.z;
      this.z = this.w;
      return int.MaxValue & (int) (this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num ^ (int) (num >> 8))));
    }

    public bool NextBool()
    {
      if (this.bitMask != 1U)
        return ((int) this.bitBuffer & (int) (this.bitMask >>= 1)) == 0;
      uint num = this.x ^ this.x << 11;
      this.x = this.y;
      this.y = this.z;
      this.z = this.w;
      this.bitBuffer = this.w = (uint) ((int) this.w ^ (int) (this.w >> 19) ^ ((int) num ^ (int) (num >> 8)));
      this.bitMask = 2147483648U;
      return ((int) this.bitBuffer & (int) this.bitMask) == 0;
    }
  }
}
