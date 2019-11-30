// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.DataScrambler
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner
{
  internal static class DataScrambler
  {
    public const int ScrambleMethodCount = 4;
    private static byte test;

    public static byte RandomScramble(byte[] data)
    {
      byte num = (byte) new PcgRandom(new Random().Next()).Next(4);
      if (DataScrambler.test++ == (byte) 4)
        DataScrambler.test = (byte) 0;
      byte test = DataScrambler.test;
      switch (test)
      {
        case 1:
          DataScrambler.Scramble4thBitSwap(data);
          DataScrambler.ScrambleOppositeEndsSwap(data);
          DataScrambler.ScramblePairSwap(data);
          break;
        case 2:
          DataScrambler.Scramble6thBitSwap(data);
          DataScrambler.ScrambleOppositeEndsSwap(data);
          DataScrambler.ScramblePairSwap(data);
          break;
        case 3:
          DataScrambler.Scramble6thBitSwap(data);
          DataScrambler.ScramblePairSwap(data);
          DataScrambler.ScrambleOppositeEndsSwap(data);
          break;
        default:
          DataScrambler.Scramble4thBitSwap(data);
          DataScrambler.ScramblePairSwap(data);
          DataScrambler.ScrambleOppositeEndsSwap(data);
          break;
      }
      return test;
    }

    public static void Unscramble(byte[] data, byte scrambleID)
    {
      switch (scrambleID)
      {
        case 1:
          DataScrambler.ScramblePairSwap(data);
          DataScrambler.ScrambleOppositeEndsSwap(data);
          DataScrambler.Scramble4thBitSwap(data);
          break;
        case 2:
          DataScrambler.ScramblePairSwap(data);
          DataScrambler.ScrambleOppositeEndsSwap(data);
          DataScrambler.Scramble6thBitSwap(data);
          break;
        case 3:
          DataScrambler.ScrambleOppositeEndsSwap(data);
          DataScrambler.ScramblePairSwap(data);
          DataScrambler.Scramble6thBitSwap(data);
          break;
        default:
          DataScrambler.ScrambleOppositeEndsSwap(data);
          DataScrambler.ScramblePairSwap(data);
          DataScrambler.Scramble4thBitSwap(data);
          break;
      }
    }

    public static void ScramblePairSwap(byte[] data)
    {
      for (int index = 0; index < data.Length - 1; index += 2)
      {
        byte num = data[index];
        data[index] = data[index + 1];
        data[index + 1] = num;
      }
    }

    public static void ScrambleOppositeEndsSwap(byte[] data)
    {
      int index1 = 0;
      int index2 = data.Length - 1;
      while (index1 < data.Length / 2)
      {
        byte num = data[index1];
        data[index1] = data[index2];
        data[index2] = num;
        ++index1;
        --index2;
      }
    }

    public static void Scramble4thBitSwap(byte[] data)
    {
      for (int index = 0; index < data.Length - 1; ++index)
      {
        byte num = data[index];
        if (((int) num & 8) > 0)
        {
          if (((int) num & 4) > 0)
            num &= (byte) 251;
          else
            num |= (byte) 4;
        }
        data[index] = num;
      }
    }

    public static void Scramble6thBitSwap(byte[] data)
    {
      for (int index = 0; index < data.Length - 1; ++index)
      {
        byte num = data[index];
        if (((int) num & 32) > 0)
        {
          if (((int) num & 16) > 0)
            num &= (byte) 239;
          else
            num |= (byte) 16;
        }
        data[index] = num;
      }
    }
  }
}
