// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.Utils
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace StudioForge.Engine.Core
{
  public static class Utils
  {
    public static T[] ConvertArray<T>(T[,] data)
    {
      T[] objArray = new T[data.Length];
      int length = data.GetLength(0);
      for (int index1 = 0; index1 < data.GetLength(1); ++index1)
      {
        for (int index2 = 0; index2 < length; ++index2)
          objArray[index2 + index1 * length] = data[index2, index1];
      }
      return objArray;
    }

    public static T[,] ConvertArray<T>(T[] data, int sizeX)
    {
      T[,] objArray = new T[sizeX, data.Length / sizeX];
      for (int index = 0; index < data.Length; ++index)
        objArray[index % sizeX, index / sizeX] = data[index];
      return objArray;
    }

    public static IEnumerable<T> GetValues<T>()
    {
      return ((IEnumerable<FieldInfo>) typeof (T).GetFields(BindingFlags.Static | BindingFlags.Public)).Select<FieldInfo, T>((Func<FieldInfo, T>) (x => (T) x.GetValue((object) null)));
    }

    public static IEnumerable GetValues(Type type)
    {
      return (IEnumerable) ((IEnumerable<FieldInfo>) type.GetFields(BindingFlags.Static | BindingFlags.Public)).Select<FieldInfo, object>((Func<FieldInfo, object>) (x => x.GetValue((object) null)));
    }

    public static string[] BuildEnumStringArray<T>()
    {
      return Utils.BuildEnumStringArray<T>((string) null, false);
    }

    public static string[] BuildEnumStringArray<T>(string filter)
    {
      return Utils.BuildEnumStringArray<T>(filter, false);
    }

    public static string[] BuildEnumStringArray<T>(string filter, bool convertToLowerCase)
    {
      List<string> stringList = new List<string>();
      foreach (T obj in Utils.GetValues<T>())
      {
        string lower = obj.ToString();
        if (filter == null || lower.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
        {
          if (convertToLowerCase)
            lower = lower.ToLower();
          stringList.Add(lower);
        }
      }
      return stringList.ToArray();
    }

    public static string[] BuildEnumStringArray(Type type)
    {
      return Utils.BuildEnumStringArray(type, (string) null);
    }

    public static string[] BuildEnumStringArray(Type type, string filter)
    {
      List<string> stringList = new List<string>();
      foreach (object obj in Utils.GetValues(type))
      {
        string str = obj.ToString();
        if (filter == null || str.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
          stringList.Add(str.ToString());
      }
      return stringList.ToArray();
    }

    public static T? GetEnumFromString<T>(string name) where T : struct
    {
      foreach (T obj in Utils.GetValues<T>())
      {
        if (obj.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
          return new T?(obj);
      }
      return new T?();
    }

    public static object GetEnumFromString(Type type, string name)
    {
      foreach (object obj in Utils.GetValues(type))
      {
        if (obj.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
          return obj;
      }
      return (object) null;
    }

    public static string InsertSpacesBeforeCapitals(string s)
    {
      if (s == null || s.Length < 1)
        return s;
      string str = string.Empty + (object) s[0];
      bool flag1 = true;
      bool flag2 = true;
      char ch = '_';
      for (int index = 1; index < s.Length; ++index)
      {
        char c = s[index];
        bool flag3 = char.IsUpper(c);
        bool flag4 = char.IsDigit(c);
        if (ch != ' ' && (flag3 && !flag1 || flag4 && !flag2))
          str += (string) (object) ' ';
        flag1 = flag3;
        flag2 = flag4;
        ch = c;
        str += (string) (object) c;
      }
      return str;
    }

    public static string ReplaceDelims(string text, char[] delimeters, char with)
    {
      if (text != null && text.Length > 0 && (delimeters != null && delimeters.Length > 0))
      {
        foreach (char delimeter in delimeters)
          text = text.Replace(delimeter, with);
      }
      return text;
    }

    public static string[] BreakIntoLines(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      bool wordWrap)
    {
      return Utils.BreakIntoLines(font, maxWidth, scale, text, wordWrap, (char[]) null, true);
    }

    public static string[] BreakIntoLines(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      bool wordWrap,
      char[] delims)
    {
      return Utils.BreakIntoLines(font, maxWidth, scale, text, wordWrap, delims, true);
    }

    public static string[] BreakIntoLines(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      bool wordWrap,
      char[] delims,
      bool trimLeadingSpaces)
    {
      if (!wordWrap)
        return Utils.BreakIntoLinesNoWordWrap(font, maxWidth, scale, text, delims);
      return Utils.BreakIntoLinesWordWrap(font, maxWidth, scale, text, delims, trimLeadingSpaces);
    }

    private static string[] BreakIntoLinesWordWrap(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      char[] delims,
      bool trimLeadingSpaces)
    {
      if (text == null || text.Length == 0)
        return new string[1]{ "" };
      text = Utils.ReplaceDelims(text, delims, '\n');
      List<string> stringList = new List<string>();
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        if (ch == '\n')
        {
          stringList.Add(stringBuilder.ToString());
          stringBuilder.Remove(0, stringBuilder.Length);
        }
        else if (stringBuilder.Length > 0 || !trimLeadingSpaces || ch != ' ')
        {
          if (ch == ' ')
            startIndex = stringBuilder.Length;
          stringBuilder.Append(ch);
          if ((double) (font.MeasureString(stringBuilder.ToString()) * scale).X > (double) maxWidth || ch == '\n')
          {
            if (startIndex == 0)
              startIndex = stringBuilder.Length - 1;
            string str = stringBuilder.ToString().Substring(startIndex + 1, stringBuilder.Length - startIndex - 1);
            stringBuilder.Remove(startIndex, stringBuilder.Length - startIndex);
            stringList.Add(stringBuilder.ToString());
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append(str);
          }
        }
      }
      stringList.Add(stringBuilder.ToString());
      return stringList.ToArray();
    }

    private static string[] BreakIntoLinesNoWordWrap(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      char[] delims)
    {
      if (text == null || text.Length == 0)
        return new string[1]{ "" };
      List<string> stringList = new List<string>();
      StringBuilder text1 = new StringBuilder();
      for (int index = 0; index < text.Length; ++index)
      {
        char c = text[index];
        if (delims != null && delims.Length > 0 && ((IEnumerable<char>) delims).Any<char>((Func<char, bool>) (k => (int) k == (int) c)))
        {
          stringList.Add(text1.ToString());
          text1.Remove(0, text1.Length);
        }
        else
        {
          text1.Append(c);
          if ((double) font.MeasureString(text1).X * (double) scale > (double) maxWidth)
          {
            text1.Remove(text1.Length - 1, 1);
            stringList.Add(text1.ToString());
            text1.Remove(0, text1.Length);
            text1.Append(c);
          }
        }
      }
      stringList.Add(text1.ToString());
      return stringList.ToArray();
    }

    public static string InsertNewLines(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      bool wordWrap)
    {
      return Utils.InsertNewLines(font, maxWidth, scale, text, wordWrap, (char[]) null);
    }

    public static string InsertNewLines(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      bool wordWrap,
      char[] delims)
    {
      if (!wordWrap)
        return Utils.InsertNewLinesNoWordWrap(font, maxWidth, scale, text, delims);
      return Utils.InsertNewLinesWordWrap(font, maxWidth, scale, text, delims);
    }

    private static string InsertNewLinesWordWrap(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      char[] delims)
    {
      if (text == null || text.Length < 1)
        return text;
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        if (ch == ' ')
          startIndex = stringBuilder.Length;
        if (delims != null && ((IEnumerable<char>) delims).Contains<char>(ch))
          stringBuilder.Append('\n');
        else
          stringBuilder.Append(ch);
        if ((double) (font.MeasureString(stringBuilder.ToString()) * scale).X > (double) maxWidth)
        {
          if (startIndex == 0)
          {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append('\n');
            if (ch != ' ')
              stringBuilder.Append(ch);
          }
          else
          {
            string str = stringBuilder.ToString().Substring(startIndex + 1);
            stringBuilder.Remove(startIndex, stringBuilder.Length - startIndex);
            stringBuilder.Append('\n');
            stringBuilder.Append(str);
          }
          startIndex = 0;
        }
      }
      return stringBuilder.ToString();
    }

    private static string InsertNewLinesNoWordWrap(
      SpriteFont font,
      int maxWidth,
      float scale,
      string text,
      char[] delims)
    {
      if (text == null || text.Length < 1)
        return text;
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder text1 = new StringBuilder();
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        text1.Append(ch);
        if ((double) font.MeasureString(text1).X * (double) scale > (double) maxWidth && (delims == null || ((IEnumerable<char>) delims).Contains<char>(ch)))
        {
          stringBuilder.Append('\n');
          if (delims == null)
            stringBuilder.Append(ch);
          text1.Remove(0, text1.Length);
        }
        else
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }

    public static Vector2 MeasureText(SpriteFont font, string[] text, float scale)
    {
      Vector2 zero = Vector2.Zero;
      if (text != null)
      {
        foreach (string text1 in text)
        {
          Vector2 vector2 = font.MeasureString(text1);
          if ((double) vector2.X > (double) zero.X)
            zero.X = vector2.X;
          zero.Y += vector2.Y;
        }
      }
      return zero * scale;
    }

    public static string StripChars(string s, int from, int to)
    {
      return Utils.StripChars(s, from, to, (char[]) null);
    }

    public static string StripChars(string s, int from, int to, char[] exceptions)
    {
      string str = "";
      if (s != null)
      {
        for (int index = 0; index < s.Length; ++index)
        {
          char ch = s[index];
          if ((int) ch >= from && (int) ch <= to || exceptions != null && ((IEnumerable<char>) exceptions).Contains<char>(ch))
            str += (string) (object) ch;
        }
      }
      return str;
    }

    public static byte[] EncryptString(string str, int key)
    {
      byte[] numArray = new byte[str.Length];
      for (int index = 0; index < str.Length; ++index)
        numArray[index] = (byte) ((uint) str[index] + (uint) key);
      return numArray;
    }

    public static string UnencryptString(byte[] data, int key)
    {
      string str = (string) null;
      if (data != null && data.Length > 0)
      {
        for (int index = 0; index < data.Length; ++index)
          str += (string) (object) (char) ((uint) data[index] - (uint) key);
      }
      return str;
    }

    public static void CopyStream(Stream src, Stream dest, int fromSrcPos, int fromDestPos)
    {
      Utils.CopyStream(src, dest, fromSrcPos, fromDestPos, (int) (src.Length - (long) fromSrcPos));
    }

    public static void CopyStream(
      Stream src,
      Stream dest,
      int fromSrcPos,
      int fromDestPos,
      int maxCopyBufferSize)
    {
      src.Position = (long) fromSrcPos;
      dest.Position = (long) fromDestPos;
      src.CopyTo(dest);
    }

    public static T Deserialize<T>(string filename) where T : new()
    {
      using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (XmlReader xmlReader = XmlReader.Create((Stream) fileStream))
          return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
      }
    }

    public static T Deserialize1<T>(string filename)
    {
      using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (XmlReader xmlReader = XmlReader.Create((Stream) fileStream))
          return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
      }
    }

    public static T Deserialize<T>(Stream stream) where T : new()
    {
      using (XmlReader xmlReader = XmlReader.Create(stream))
        return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
    }

    public static void Serialize<T>(T data, string filename)
    {
      using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
        new XmlSerializer(typeof (T)).Serialize((Stream) fileStream, (object) data);
    }

    public static long DateToBinary(DateTime date)
    {
      return date.ToBinary();
    }

    public static DateTime DateFromBinary(long date)
    {
      if (date == 0L)
        return DateTime.MinValue;
      return DateTime.FromBinary(date);
    }

    public static List<T> ValidateTypeList<T>(string text, out string finalText)
    {
      string[] strArray = text.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      IEnumerable<T> values = Utils.GetValues<T>();
      List<T> objList = new List<T>(strArray.Length);
      finalText = "";
      foreach (string str in strArray)
      {
        foreach (T obj in values)
        {
          if (obj.ToString().Equals(str.Trim(), StringComparison.OrdinalIgnoreCase))
          {
            if (objList.Count > 0)
              finalText += ", ";
            finalText += obj.ToString();
            objList.Add(obj);
            break;
          }
        }
      }
      return objList;
    }

    public static int SortStringNoneAtTop(string s1, string s2)
    {
      if (s1.Equals("none", StringComparison.OrdinalIgnoreCase))
        return "a".CompareTo("b");
      if (s2.Equals("none", StringComparison.OrdinalIgnoreCase))
        return "b".CompareTo("a");
      return s1.CompareTo(s2);
    }

    public static int SortStringOffAtTop(string s1, string s2)
    {
      if (s1.Equals("off", StringComparison.OrdinalIgnoreCase))
        return "a".CompareTo("b");
      if (s2.Equals("off", StringComparison.OrdinalIgnoreCase))
        return "b".CompareTo("a");
      return s1.CompareTo(s2);
    }
  }
}
