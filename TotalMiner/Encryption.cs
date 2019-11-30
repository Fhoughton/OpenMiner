// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Encryption
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.IO;

namespace StudioForge.TotalMiner
{
  public static class Encryption
  {
    public static byte[] GetMessageHash(byte[] message, int length, int version)
    {
      byte[] hash = new byte[64];
      Encryption.BuildMessageHash(message, length, hash, version);
      return hash;
    }

    public static byte[] GetMessageHash(Stream message, int length, int version)
    {
      byte[] hash = new byte[64];
      Encryption.BuildMessageHash(message, length, hash, version);
      return hash;
    }

    private static void BuildMessageHash(byte[] message, int length, byte[] hash, int version)
    {
      int length1 = hash.Length;
      if (length > message.Length)
        length = message.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        byte num = message[index1];
        int index2 = index1 % length1;
        hash[index2] += num;
      }
      long num1 = 0;
      int num2 = version < 130 ? 1 : (version < 197 ? 13 : 17);
      int num3 = version < 130 ? 2000 : (version < 197 ? 3591 : 1849);
      int num4 = version < 130 ? 1000 : (version < 197 ? 1901 : 1656);
      for (int index = 0; index < length1; ++index)
        num1 += (long) (((int) hash[index] + num2) * num3);
      byte num5 = (byte) ((ulong) num1 / (ulong) num4 & (ulong) byte.MaxValue);
      for (int index = 0; index < length1; ++index)
        hash[index] += num5;
    }

    private static void BuildMessageHash(Stream message, int length, byte[] hash, int version)
    {
      int length1 = hash.Length;
      if ((long) length > message.Length)
        length = (int) message.Length;
      message.Position = 0L;
      for (int index1 = 0; index1 < length; ++index1)
      {
        byte num = (byte) message.ReadByte();
        int index2 = index1 % length1;
        hash[index2] += num;
      }
      long num1 = 0;
      int num2 = version < 130 ? 1 : (version < 197 ? 13 : 17);
      int num3 = version < 130 ? 2000 : (version < 197 ? 3591 : 1849);
      int num4 = version < 130 ? 1000 : (version < 197 ? 1901 : 1656);
      for (int index = 0; index < length1; ++index)
        num1 += (long) (((int) hash[index] + num2) * num3);
      byte num5 = (byte) ((ulong) num1 / (ulong) num4 & (ulong) byte.MaxValue);
      for (int index = 0; index < length1; ++index)
        hash[index] += num5;
    }
  }
}
