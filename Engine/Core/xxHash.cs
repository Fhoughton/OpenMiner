// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.xxHash
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;

namespace StudioForge.Engine.Core
{
  public class xxHash
  {
    private const uint PRIME32_1 = 2654435761;
    private const uint PRIME32_2 = 2246822519;
    private const uint PRIME32_3 = 3266489917;
    private const uint PRIME32_4 = 668265263;
    private const uint PRIME32_5 = 374761393;
    protected xxHash.XXH_State _state;

    public static uint CalculateHash(byte[] buf, int len, uint seed)
    {
      int index1 = 0;
      if (len == -1)
        len = buf.Length;
      uint num1;
      if (len >= 16)
      {
        int num2 = len - 16;
        uint num3 = (uint) ((int) seed - 1640531535 - 2048144777);
        uint num4 = seed + 2246822519U;
        uint num5 = seed;
        uint num6 = seed - 2654435761U;
        do
        {
          num3 = xxHash.CalcSubHash(num3, buf, index1);
          int index2 = index1 + 4;
          num4 = xxHash.CalcSubHash(num4, buf, index2);
          int index3 = index2 + 4;
          num5 = xxHash.CalcSubHash(num5, buf, index3);
          int index4 = index3 + 4;
          num6 = xxHash.CalcSubHash(num6, buf, index4);
          index1 = index4 + 4;
        }
        while (index1 <= num2);
        num1 = xxHash.RotateLeft(num3, 1) + xxHash.RotateLeft(num4, 7) + xxHash.RotateLeft(num5, 12) + xxHash.RotateLeft(num6, 18);
      }
      else
        num1 = seed + 374761393U;
      uint num7 = num1 + (uint) len;
      for (; index1 <= len - 4; index1 += 4)
        num7 = xxHash.RotateLeft(num7 + BitConverter.ToUInt32(buf, index1) * 3266489917U, 17) * 668265263U;
      for (; index1 < len; ++index1)
        num7 = xxHash.RotateLeft(num7 + (uint) buf[index1] * 374761393U, 11) * 2654435761U;
      uint num8 = (num7 ^ num7 >> 15) * 2246822519U;
      uint num9 = (num8 ^ num8 >> 13) * 3266489917U;
      return num9 ^ num9 >> 16;
    }

    public void Init(uint seed)
    {
      this._state.seed = seed;
      this._state.v1 = (uint) ((int) seed - 1640531535 - 2048144777);
      this._state.v2 = seed + 2246822519U;
      this._state.v3 = seed;
      this._state.v4 = seed - 2654435761U;
      this._state.total_len = 0UL;
      this._state.memsize = 0;
      this._state.memory = new byte[16];
    }

    public bool Update(byte[] input, int len)
    {
      int num1 = 0;
      this._state.total_len += (ulong) (uint) len;
      if (this._state.memsize + len < 16)
      {
        Array.Copy((Array) input, 0, (Array) this._state.memory, this._state.memsize, len);
        this._state.memsize += len;
        return true;
      }
      if (this._state.memsize > 0)
      {
        Array.Copy((Array) input, 0, (Array) this._state.memory, this._state.memsize, 16 - this._state.memsize);
        this._state.v1 = xxHash.CalcSubHash(this._state.v1, this._state.memory, num1);
        int index1 = num1 + 4;
        this._state.v2 = xxHash.CalcSubHash(this._state.v2, this._state.memory, index1);
        int index2 = index1 + 4;
        this._state.v3 = xxHash.CalcSubHash(this._state.v3, this._state.memory, index2);
        int index3 = index2 + 4;
        this._state.v4 = xxHash.CalcSubHash(this._state.v4, this._state.memory, index3);
        int num2 = index3 + 4;
        num1 = 0;
        this._state.memsize = 0;
      }
      if (num1 <= len - 16)
      {
        int num2 = len - 16;
        uint num3 = this._state.v1;
        uint num4 = this._state.v2;
        uint num5 = this._state.v3;
        uint num6 = this._state.v4;
        do
        {
          num3 = xxHash.CalcSubHash(num3, input, num1);
          int index1 = num1 + 4;
          num4 = xxHash.CalcSubHash(num4, input, index1);
          int index2 = index1 + 4;
          num5 = xxHash.CalcSubHash(num5, input, index2);
          int index3 = index2 + 4;
          num6 = xxHash.CalcSubHash(num6, input, index3);
          num1 = index3 + 4;
        }
        while (num1 <= num2);
        this._state.v1 = num3;
        this._state.v2 = num4;
        this._state.v3 = num5;
        this._state.v4 = num6;
      }
      if (num1 < len)
      {
        Array.Copy((Array) input, num1, (Array) this._state.memory, 0, len - num1);
        this._state.memsize = len - num1;
      }
      return true;
    }

    public uint Digest()
    {
      int startIndex = 0;
      uint num1 = (this._state.total_len < 16UL ? this._state.seed + 374761393U : xxHash.RotateLeft(this._state.v1, 1) + xxHash.RotateLeft(this._state.v2, 7) + xxHash.RotateLeft(this._state.v3, 12) + xxHash.RotateLeft(this._state.v4, 18)) + (uint) this._state.total_len;
      for (; startIndex <= this._state.memsize - 4; startIndex += 4)
        num1 = xxHash.RotateLeft(num1 + BitConverter.ToUInt32(this._state.memory, startIndex) * 3266489917U, 17) * 668265263U;
      for (; startIndex < this._state.memsize; ++startIndex)
        num1 = xxHash.RotateLeft(num1 + (uint) this._state.memory[startIndex] * 374761393U, 11) * 2654435761U;
      uint num2 = (num1 ^ num1 >> 15) * 2246822519U;
      uint num3 = (num2 ^ num2 >> 13) * 3266489917U;
      return num3 ^ num3 >> 16;
    }

    private static uint CalcSubHash(uint value, byte[] buf, int index)
    {
      uint uint32 = BitConverter.ToUInt32(buf, index);
      value += uint32 * 2246822519U;
      value = xxHash.RotateLeft(value, 13);
      value *= 2654435761U;
      return value;
    }

    private static uint RotateLeft(uint value, int count)
    {
      return value << count | value >> 32 - count;
    }

    public struct XXH_State
    {
      public ulong total_len;
      public uint seed;
      public uint v1;
      public uint v2;
      public uint v3;
      public uint v4;
      public int memsize;
      public byte[] memory;
    }
  }
}
