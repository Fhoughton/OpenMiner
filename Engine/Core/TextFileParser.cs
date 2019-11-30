// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.TextFileParser
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;
using System.IO;

namespace StudioForge.Engine.Core
{
  public static class TextFileParser
  {
    public static StreamReader GetReader(string filename)
    {
      return new StreamReader((Stream) File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read));
    }

    public static string GetValue(StreamReader reader, string id)
    {
      reader.BaseStream.Seek(0L, SeekOrigin.Begin);
      while (!reader.EndOfStream)
      {
        string str = reader.ReadLine();
        if (str.StartsWith(id, StringComparison.OrdinalIgnoreCase))
        {
          int num = str.IndexOf('=');
          return str.Substring(num + 1).Trim();
        }
      }
      return (string) null;
    }

    public static string ReadString(StreamReader reader, string id)
    {
      return TextFileParser.ReadString(reader, id, (string) null);
    }

    public static string ReadString(StreamReader reader, string id, string defaultValue)
    {
      return TextFileParser.GetValue(reader, id) ?? defaultValue;
    }

    public static string ReadString(string filename, string id, string defaultValue)
    {
      using (StreamReader reader = TextFileParser.GetReader(filename))
        return TextFileParser.ReadString(reader, id, defaultValue);
    }

    public static bool ReadBool(StreamReader reader, string id)
    {
      return TextFileParser.ReadBool(reader, id, false);
    }

    public static bool ReadBool(StreamReader reader, string id, bool defaultValue)
    {
      string str = TextFileParser.GetValue(reader, id);
      if (str != null)
      {
        bool result;
        if (bool.TryParse(str, out result))
          return result;
        if (str.Equals("on", StringComparison.OrdinalIgnoreCase) || str.Equals("true", StringComparison.OrdinalIgnoreCase) || str.Equals("yes", StringComparison.OrdinalIgnoreCase))
          return true;
        if (str.Equals("off", StringComparison.OrdinalIgnoreCase) || str.Equals("false", StringComparison.OrdinalIgnoreCase) || str.Equals("no", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return defaultValue;
    }

    public static bool ReadBool(string filename, string id, bool defaultValue)
    {
      using (StreamReader reader = TextFileParser.GetReader(filename))
        return TextFileParser.ReadBool(reader, id, defaultValue);
    }

    public static int ReadInt(StreamReader reader, string id)
    {
      return TextFileParser.ReadInt(reader, id, 0);
    }

    public static int ReadInt(StreamReader reader, string id, int defaultValue)
    {
      string s = TextFileParser.GetValue(reader, id);
      int result;
      if (s != null && int.TryParse(s, out result))
        return result;
      return defaultValue;
    }

    public static int ReadInt(string filename, string id, int defaultValue)
    {
      using (StreamReader reader = TextFileParser.GetReader(filename))
        return TextFileParser.ReadInt(reader, id, defaultValue);
    }

    public static void WriteValue(string filename, string id, object v)
    {
      using (FileStream fileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
      {
        byte[] buffer = new byte[fileStream.Length];
        fileStream.Read(buffer, 0, (int) fileStream.Length);
        using (MemoryStream memoryStream = new MemoryStream(buffer))
        {
          using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
          {
            if (buffer.Length > 0)
              fileStream.SetLength(0L);
            using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
            {
              bool flag = false;
              while (!streamReader.EndOfStream)
              {
                string str = streamReader.ReadLine();
                if (str.StartsWith(id))
                {
                  streamWriter.WriteLine(id + "=" + v.ToString());
                  flag = true;
                }
                else
                  streamWriter.WriteLine(str);
              }
              if (flag)
                return;
              streamWriter.WriteLine(id + "=" + v.ToString());
            }
          }
        }
      }
    }

    public static void WriteString(string filename, string id, string v)
    {
      TextFileParser.WriteValue(filename, id, (object) v);
    }

    public static void WriteBool(string filename, string id, bool v)
    {
      TextFileParser.WriteValue(filename, id, (object) v);
    }

    public static void WriteInt(string filename, string id, int v)
    {
      TextFileParser.WriteValue(filename, id, (object) v);
    }
  }
}
