// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Parser
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioForge.TotalMiner
{
  public class Parser
  {
    private StringBuilder builder = new StringBuilder(500);
    private static string[] skillEnumList;

    public Parser.Token GetNextToken(string command, int index)
    {
      return this.GetNextToken(command, index, '[', ']');
    }

    public Parser.Token GetNextToken(
      string command,
      int index,
      char startDelim,
      char endDelim)
    {
      while (index < command.Length && (char.IsWhiteSpace(command[index]) || startDelim != char.MinValue && (int) command[index] != (int) startDelim))
        ++index;
      if (index < command.Length && (int) command[index] == (int) startDelim)
        ++index;
      this.builder.Length = 0;
      int num1 = index;
      int num2 = 0;
      for (; index < command.Length; ++index)
      {
        char ch = command[index];
        if ((int) ch != (int) endDelim || num2-- > 0)
        {
          if ((int) ch == (int) startDelim)
            ++num2;
          this.builder.Append(ch);
        }
        else
          break;
      }
      return new Parser.Token()
      {
        StartIndex = num1,
        EndIndex = index,
        Lexeme = this.builder.ToString()
      };
    }

    public Item? GetItemIDFromToken(Parser.Token token)
    {
      return this.GetItemIDFromToken(token.Lexeme, false, false, true);
    }

    public Item? GetItemIDFromToken(
      string token,
      bool includeHand,
      bool includeDisabled,
      bool convertBlockIDToItemID)
    {
      if (token != null)
      {
        if (token.IndexOf(' ') >= 0)
          token = token.Replace(" ", "");
        if (token.Length > 0)
        {
          foreach (ItemDataXML itemDataXml in Globals1.ItemData)
          {
            if ((includeDisabled || includeHand && itemDataXml.ItemID == Item.Hand || itemDataXml.IsEnabled && itemDataXml.ItemID != Item.Bedrock) && itemDataXml.IDString.Equals(token, StringComparison.OrdinalIgnoreCase))
              return new Item?(convertBlockIDToItemID ? ItemData.ConvertBlockIDToItemID(itemDataXml.ItemID) : itemDataXml.ItemID);
          }
        }
      }
      return new Item?();
    }

    public SkillType? GetSkillFromToken(Parser.Token token)
    {
      return this.GetSkillFromToken(token.Lexeme);
    }

    public SkillType? GetSkillFromToken(string token)
    {
      if (token.Length > 0)
      {
        if (Parser.skillEnumList == null)
          Parser.skillEnumList = Utils.BuildEnumStringArray<SkillType>((string) null, true);
        for (int index = 0; index < Parser.skillEnumList.Length; ++index)
        {
          if (Parser.skillEnumList[index] == token)
            return new SkillType?((SkillType) index);
        }
      }
      return new SkillType?();
    }

    public ItemAction? GetActionFromToken(Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "mine":
        case "mined":
          return new ItemAction?(ItemAction.Mined);
        case "use":
        case "used":
          return new ItemAction?(ItemAction.Used);
        case "craft":
        case "crafted":
          return new ItemAction?(ItemAction.Crafted);
        case "collect":
        case "collected":
          return new ItemAction?(ItemAction.Collected);
        default:
          return new ItemAction?();
      }
    }

    public ZoneType? GetZoneTypeFromToken(Parser.Token token)
    {
      ZoneType zoneType = ZoneType.None;
      string lexeme = token.Lexeme;
      char[] chArray = new char[1]{ '|' };
      foreach (string str in lexeme.Split(chArray))
      {
        switch (str)
        {
          case "spawn":
            zoneType |= ZoneType.Spawn;
            break;
          case "noedit":
            zoneType |= ZoneType.NoEdit;
            break;
          case "nopvp":
            zoneType |= ZoneType.NoCombat;
            break;
          case "nofly":
            zoneType |= ZoneType.NoFly;
            break;
          case "nomobs":
            zoneType |= ZoneType.NoMobs;
            break;
          case "noescape":
            zoneType |= ZoneType.NoEscape;
            break;
        }
      }
      return new ZoneType?(zoneType);
    }

    public Permissions? GetPermissionFromToken(Parser.Token token, bool includeAdmin)
    {
      switch (token.Lexeme)
      {
        case "none":
          return new Permissions?(Permissions.None);
        case "adventure":
          return new Permissions?(Permissions.Adventure);
        case "edit":
          return new Permissions?(Permissions.Edit);
        case "creative":
          return new Permissions?(Permissions.Creative);
        case "fly":
          return new Permissions?(Permissions.Fly);
        case "map":
          return new Permissions?(Permissions.Map);
        case "chat":
        case "voicechat":
          return new Permissions?(Permissions.VoiceChat);
        case "spectate":
          return new Permissions?(Permissions.Spectate);
        case "shops":
          return new Permissions?(Permissions.SystemShops);
        case "viewscripts":
          return new Permissions?(Permissions.ViewScripts);
        case "textchat":
          return new Permissions?(Permissions.TextChat);
        default:
          if (includeAdmin)
          {
            switch (token.Lexeme)
            {
              case "save":
                return new Permissions?(Permissions.Save);
              case "admin":
                return new Permissions?(Permissions.Admin);
              case "grief":
                return new Permissions?(Permissions.Grief);
            }
          }
          return new Permissions?();
      }
    }

    public int? GetIntFromToken(Parser.Token token)
    {
      return this.GetIntFromToken(token.Lexeme);
    }

    public int? GetIntFromToken(string token)
    {
      int result;
      if (int.TryParse(token, out result))
        return new int?(result);
      return new int?();
    }

    public int? GetIntOrPercentFromToken(Parser.Token token, out bool isPercent)
    {
      isPercent = false;
      if (token.Lexeme.Length > 0 && token.Lexeme[token.Lexeme.Length - 1] == '%')
      {
        int result;
        if (int.TryParse(token.Lexeme.Substring(0, token.Lexeme.Length - 1), out result))
        {
          isPercent = true;
          return new int?(result);
        }
      }
      else
      {
        int result;
        if (int.TryParse(token.Lexeme, out result))
          return new int?(result);
      }
      return new int?();
    }

    public int? GetIntFromNamedToken(string token, string name)
    {
      int result;
      if (token.Length > name.Length + 1 && token.StartsWith(name) && (token[name.Length] == '=' && int.TryParse(token.Substring(name.Length + 1), out result)))
        return new int?(result);
      return new int?();
    }

    public float? GetFloatFromToken(Parser.Token token)
    {
      float result;
      if (float.TryParse(token.Lexeme, out result))
        return new float?(result);
      return new float?();
    }

    public double? GetDoubleFromToken(Parser.Token token)
    {
      double result;
      if (double.TryParse(token.Lexeme, out result))
        return new double?(result);
      return new double?();
    }

    public BlockFace? GetFaceFromToken(Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "left":
          return new BlockFace?(BlockFace.Left);
        case "forward":
          return new BlockFace?(BlockFace.Forward);
        case "right":
          return new BlockFace?(BlockFace.Right);
        case "backward":
          return new BlockFace?(BlockFace.Backward);
        case "up":
          return new BlockFace?(BlockFace.Up);
        case "down":
          return new BlockFace?(BlockFace.Down);
        default:
          return new BlockFace?();
      }
    }

    public BlockFace? GetDirFromToken(Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "n":
          return new BlockFace?(BlockFace.Backward);
        case "s":
          return new BlockFace?(BlockFace.Forward);
        case "e":
          return new BlockFace?(BlockFace.Left);
        case "w":
          return new BlockFace?(BlockFace.Right);
        default:
          return new BlockFace?();
      }
    }

    public Color? GetColor4FromToken(Parser.Token token)
    {
      return this.GetColor4FromToken(token.Lexeme);
    }

    public Color? GetColor4FromToken(string token)
    {
      if (token.Length > 6)
      {
        int num1 = token.IndexOf(',');
        if (num1 >= 0)
        {
          int num2 = token.IndexOf(',', num1 + 1);
          if (num2 >= 0 && token.IndexOf(',', num2 + 1) >= 0)
          {
            Parser.Token nextToken = this.GetNextToken(token, 0, char.MinValue, ',');
            byte result1;
            byte.TryParse(nextToken.Lexeme, out result1);
            nextToken = this.GetNextToken(token, nextToken.EndIndex + 1, char.MinValue, ',');
            byte result2;
            byte.TryParse(nextToken.Lexeme, out result2);
            nextToken = this.GetNextToken(token, nextToken.EndIndex + 1, char.MinValue, ',');
            byte result3;
            byte.TryParse(nextToken.Lexeme, out result3);
            nextToken = this.GetNextToken(token, nextToken.EndIndex + 1, char.MinValue, ',');
            byte result4;
            byte.TryParse(nextToken.Lexeme, out result4);
            return new Color?(new Color((int) result1, (int) result2, (int) result3, (int) result4));
          }
        }
      }
      return new Color?();
    }

    public Rectangle? GetRectFromToken(Parser.Token token)
    {
      return this.GetRectFromToken(token.Lexeme);
    }

    public Rectangle? GetRectFromToken(string token)
    {
      if (token.Length > 6)
      {
        int num1 = token.IndexOf(',');
        if (num1 >= 0)
        {
          int num2 = token.IndexOf(',', num1 + 1);
          if (num2 >= 0 && token.IndexOf(',', num2 + 1) >= 0)
          {
            Parser.Token nextToken = this.GetNextToken(token, 0, char.MinValue, ',');
            ushort result1;
            ushort.TryParse(nextToken.Lexeme, out result1);
            nextToken = this.GetNextToken(token, nextToken.EndIndex + 1, char.MinValue, ',');
            ushort result2;
            ushort.TryParse(nextToken.Lexeme, out result2);
            nextToken = this.GetNextToken(token, nextToken.EndIndex + 1, char.MinValue, ',');
            ushort result3;
            ushort.TryParse(nextToken.Lexeme, out result3);
            nextToken = this.GetNextToken(token, nextToken.EndIndex + 1, char.MinValue, ',');
            ushort result4;
            ushort.TryParse(nextToken.Lexeme, out result4);
            return new Rectangle?(new Rectangle((int) result1, (int) result2, (int) result3, (int) result4));
          }
        }
      }
      return new Rectangle?();
    }

    public Vector2? GetVector2FromToken(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length <= 2 || lexeme.IndexOf(',') < 0)
        return new Vector2?();
      Parser.Token nextToken = this.GetNextToken(lexeme, 0, char.MinValue, ',');
      float result1;
      float.TryParse(nextToken.Lexeme, out result1);
      float result2;
      float.TryParse(this.GetNextToken(lexeme, nextToken.EndIndex + 1, char.MinValue, ',').Lexeme, out result2);
      return new Vector2?(new Vector2(result1, result2));
    }

    public Vector3? GetVector3FromToken(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length > 4)
      {
        int num = lexeme.IndexOf(',');
        if (num >= 0 && lexeme.IndexOf(',', num + 1) >= 0)
        {
          Parser.Token nextToken1 = this.GetNextToken(lexeme, 0, char.MinValue, ',');
          float result1;
          float.TryParse(nextToken1.Lexeme, out result1);
          Parser.Token nextToken2 = this.GetNextToken(lexeme, nextToken1.EndIndex + 1, char.MinValue, ',');
          float result2;
          float.TryParse(nextToken2.Lexeme, out result2);
          float result3;
          float.TryParse(this.GetNextToken(lexeme, nextToken2.EndIndex + 1, char.MinValue, ',').Lexeme, out result3);
          return new Vector3?(new Vector3(result1, result2, result3));
        }
      }
      return new Vector3?();
    }

    public Vector4? GetVector4FromToken(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length > 6)
      {
        int num1 = lexeme.IndexOf(',');
        if (num1 >= 0)
        {
          int num2 = lexeme.IndexOf(',', num1 + 1);
          if (num2 >= 0 && lexeme.IndexOf(',', num2 + 1) >= 0)
          {
            Parser.Token nextToken1 = this.GetNextToken(lexeme, 0, char.MinValue, ',');
            float result1;
            float.TryParse(nextToken1.Lexeme, out result1);
            Parser.Token nextToken2 = this.GetNextToken(lexeme, nextToken1.EndIndex + 1, char.MinValue, ',');
            float result2;
            float.TryParse(nextToken2.Lexeme, out result2);
            Parser.Token nextToken3 = this.GetNextToken(lexeme, nextToken2.EndIndex + 1, char.MinValue, ',');
            float result3;
            float.TryParse(nextToken3.Lexeme, out result3);
            float result4;
            float.TryParse(this.GetNextToken(lexeme, nextToken3.EndIndex + 1, char.MinValue, ',').Lexeme, out result4);
            return new Vector4?(new Vector4(result1, result2, result3, result4));
          }
        }
      }
      return new Vector4?();
    }

    public void GetNamedToken(Parser.Token token, string name, out int indexStart, out int length)
    {
      this.GetNamedToken(token.Lexeme, name, out indexStart, out length);
    }

    public void GetNamedToken(string token, string name, out int indexStart, out int length)
    {
      indexStart = -1;
      length = 0;
      if (name.Length <= 0 || token.Length < name.Length + 2 || !token.StartsWith(name) || token[name.Length] != '=' && token[name.Length] != ':')
        return;
      indexStart = name.Length + 1;
      length = token.Length - indexStart;
    }

    public Parser.CompareState? GetCompareFromToken(Parser.Token token)
    {
      switch (token.Lexeme)
      {
        case "=":
          return new Parser.CompareState?(Parser.CompareState.Equal);
        case "!=":
        case "<>":
          return new Parser.CompareState?(Parser.CompareState.NotEqual);
        case "<":
          return new Parser.CompareState?(Parser.CompareState.LessThan);
        case "=<":
        case "<=":
          return new Parser.CompareState?(Parser.CompareState.LessThanOrEqual);
        case ">":
          return new Parser.CompareState?(Parser.CompareState.GreaterThan);
        case "=>":
        case ">=":
          return new Parser.CompareState?(Parser.CompareState.GreaterThanOrEqual);
        case "mod":
          return new Parser.CompareState?(Parser.CompareState.Modulus);
        default:
          return new Parser.CompareState?();
      }
    }

    public static string GetShortCompareString(Parser.CompareState state)
    {
      switch (state)
      {
        case Parser.CompareState.Equal:
          return "=";
        case Parser.CompareState.NotEqual:
          return "<>";
        case Parser.CompareState.LessThan:
          return "<";
        case Parser.CompareState.LessThanOrEqual:
          return "<=";
        case Parser.CompareState.GreaterThan:
          return ">";
        case Parser.CompareState.GreaterThanOrEqual:
          return ">=";
        case Parser.CompareState.Modulus:
          return "%";
        default:
          return (string) null;
      }
    }

    public static bool Compare(int actual, int compareWith, Parser.CompareState compare)
    {
      switch (compare)
      {
        case Parser.CompareState.Equal:
          return actual == compareWith;
        case Parser.CompareState.NotEqual:
          return actual != compareWith;
        case Parser.CompareState.LessThan:
          return actual < compareWith;
        case Parser.CompareState.LessThanOrEqual:
          return actual <= compareWith;
        case Parser.CompareState.GreaterThan:
          return actual > compareWith;
        case Parser.CompareState.GreaterThanOrEqual:
          return actual >= compareWith;
        case Parser.CompareState.Modulus:
          return actual % compareWith == 0;
        default:
          return false;
      }
    }

    public static bool Compare(float actual, float compareWith, Parser.CompareState compare)
    {
      switch (compare)
      {
        case Parser.CompareState.Equal:
          return (double) actual == (double) compareWith;
        case Parser.CompareState.NotEqual:
          return (double) actual != (double) compareWith;
        case Parser.CompareState.LessThan:
          return (double) actual < (double) compareWith;
        case Parser.CompareState.LessThanOrEqual:
          return (double) actual <= (double) compareWith;
        case Parser.CompareState.GreaterThan:
          return (double) actual > (double) compareWith;
        case Parser.CompareState.GreaterThanOrEqual:
          return (double) actual >= (double) compareWith;
        case Parser.CompareState.Modulus:
          return (double) actual % (double) compareWith == 0.0;
        default:
          return false;
      }
    }

    public static bool Compare(double actual, double compareWith, Parser.CompareState compare)
    {
      switch (compare)
      {
        case Parser.CompareState.Equal:
          return actual == compareWith;
        case Parser.CompareState.NotEqual:
          return actual != compareWith;
        case Parser.CompareState.LessThan:
          return actual < compareWith;
        case Parser.CompareState.LessThanOrEqual:
          return actual <= compareWith;
        case Parser.CompareState.GreaterThan:
          return actual > compareWith;
        case Parser.CompareState.GreaterThanOrEqual:
          return actual >= compareWith;
        case Parser.CompareState.Modulus:
          return actual % compareWith == 0.0;
        default:
          return false;
      }
    }

    public string[] GetTokens(string command, char delim)
    {
      string[] strArray = (string[]) null;
      if (command.Length > 0)
      {
        int num = 0;
        for (int index = 0; index < command.Length; ++index)
        {
          if ((int) command[index] == (int) delim)
            ++num;
        }
        strArray = new string[num + 1];
        int index1 = 0;
        int startIndex = 0;
        for (int index2 = command.IndexOf(delim); index2 >= 0; index2 = command.IndexOf(delim, startIndex))
        {
          strArray[index1++] = command.Substring(startIndex, index2 - startIndex);
          startIndex = index2 + 1;
        }
        strArray[index1] = command.Substring(startIndex);
      }
      return strArray;
    }

    protected void SetMinMax(ref GlobalPoint3D min, ref GlobalPoint3D max)
    {
      if (min.X > max.X)
      {
        int x = min.X;
        min.X = max.X;
        max.X = x;
      }
      if (min.Y > max.Y)
      {
        int y = min.Y;
        min.Y = max.Y;
        max.Y = y;
      }
      if (min.Z <= max.Z)
        return;
      int z = min.Z;
      min.Z = max.Z;
      max.Z = z;
    }

    public static List<string> Split(string s, char delim, char quotes)
    {
      List<string> stringList = new List<string>();
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 0;
      int num2 = s.IndexOf(quotes);
      bool flag = num2 >= 0 && s.IndexOf(quotes, num2 + 1) > num2;
      foreach (char ch in s)
      {
        if (num1 == 0 && (int) ch == (int) delim)
        {
          if (stringBuilder.Length > 0)
            stringList.Add(stringBuilder.ToString());
          stringBuilder.Clear();
        }
        else if (flag && (int) ch == (int) quotes)
        {
          if (num1 == 0)
          {
            ++num1;
          }
          else
          {
            if (stringBuilder.Length > 0)
              stringList.Add(stringBuilder.ToString());
            stringBuilder.Clear();
            num1 = 0;
          }
        }
        else
          stringBuilder.Append(ch);
      }
      if (stringBuilder.Length > 0)
        stringList.Add(stringBuilder.ToString());
      return stringList;
    }

    public static List<string> Split(string s, char delim, char startquote, char endquote)
    {
      List<string> stringList = new List<string>();
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (char ch in s)
      {
        if (num == 0 && (int) ch == (int) delim)
        {
          if (stringBuilder.Length > 0)
            stringList.Add(stringBuilder.ToString());
          stringBuilder.Clear();
        }
        else
        {
          if ((int) ch == (int) startquote)
            ++num;
          else if ((int) ch == (int) endquote)
            --num;
          stringBuilder.Append(ch);
        }
      }
      if (stringBuilder.Length > 0)
        stringList.Add(stringBuilder.ToString());
      return stringList;
    }

    public static List<string> Split(
      string s,
      char[] delim,
      char[] startquotes,
      char[] endquotes)
    {
      List<string> stringList = new List<string>();
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (char c in s)
      {
        if (num == 0 && Parser.InArray(c, delim))
        {
          if (stringBuilder.Length > 0)
            stringList.Add(stringBuilder.ToString());
          stringBuilder.Clear();
        }
        else
        {
          if (Parser.InArray(c, startquotes))
            ++num;
          else if (Parser.InArray(c, endquotes))
            --num;
          stringBuilder.Append(c);
        }
      }
      if (stringBuilder.Length > 0)
        stringList.Add(stringBuilder.ToString());
      return stringList;
    }

    private static bool InArray(char c, char[] a)
    {
      if (a == null)
        return false;
      foreach (char ch in a)
      {
        if ((int) c == (int) ch)
          return true;
      }
      return false;
    }

    public struct Token
    {
      public string Lexeme;
      public int StartIndex;
      public int EndIndex;

      public bool IsEmpty
      {
        get
        {
          return this.StartIndex == this.EndIndex;
        }
      }
    }

    public enum CompareState : byte
    {
      None,
      Binary,
      Equal,
      NotEqual,
      LessThan,
      LessThanOrEqual,
      GreaterThan,
      GreaterThanOrEqual,
      Modulus,
    }
  }
}
