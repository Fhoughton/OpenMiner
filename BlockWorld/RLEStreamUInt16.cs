// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.RLEStreamUInt16
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using StudioForge.Engine.Core;

namespace StudioForge.BlockWorld
{
  internal static class RLEStreamUInt16
  {
    public static CustomArray<ushort> Compress(short[] cache)
    {
      ushort[] cache1 = new ushort[cache.Length];
      for (int index = 0; index < cache.Length; ++index)
        cache1[index] = (ushort) cache[index];
      return RLEStreamUInt16.Compress(cache1);
    }

    public static CustomArray<ushort> Compress(ushort[] cache)
    {
      ushort t1 = 0;
      ushort t2 = 0;
      ushort maxValue = ushort.MaxValue;
      CustomArray<ushort> customArray = new CustomArray<ushort>();
      for (int index = 0; index < cache.Length; ++index)
      {
        t2 = cache[index];
        if (maxValue == ushort.MaxValue || (int) t2 != (int) t1)
        {
          if (index > 0)
          {
            customArray.Add(maxValue);
            customArray.Add(t1);
            t1 = t2;
            maxValue = ushort.MaxValue;
          }
          else
            t1 = t2;
        }
        ++maxValue;
      }
      customArray.Add(maxValue);
      customArray.Add(t2);
      return customArray;
    }

    public static ushort[] Uncompress(ushort[] stream, int cacheLength)
    {
      ushort[] cache = new ushort[cacheLength];
      return RLEStreamUInt16.Uncompress(stream, cache);
    }

    public static ushort[] Uncompress(ushort[] stream, ushort[] cache)
    {
      int index1 = 0;
      int num1 = 0;
      for (; index1 < stream.Length - 2; index1 += 2)
      {
        int num2 = (int) stream[index1];
        ushort num3 = stream[index1 + 1];
        for (int index2 = 0; index2 <= num2; ++index2)
          cache[num1++] = num3;
      }
      ushort num4 = stream[index1 + 1];
      while (num1 < cache.Length)
        cache[num1++] = num4;
      return cache;
    }
  }
}
