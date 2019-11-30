// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.BookData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Storage;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class BookData
  {
    public const int MaxBooks = 65535;
    public ushort ID;
    public string Title;
    public string[] Text;

    public void SetText(string text, int page)
    {
      if (this.Text == null)
        this.Text = new string[page + 1];
      int num = page % 2;
      if (this.Text.Length < page - num + 2)
      {
        string[] strArray = new string[page - num + 2];
        for (int index = 0; index < this.Text.Length; ++index)
          strArray[index] = this.Text[index];
        this.Text = strArray;
      }
      this.Text[page] = Utils.StripChars(text, 32, 160);
    }

    public void ReadState(BinaryReader reader, int version)
    {
      if (version < 173)
      {
        reader.ReadInt32();
        int num = (int) reader.ReadInt16();
        reader.ReadInt32();
      }
      this.ID = version > 169 ? reader.ReadUInt16() : (ushort) reader.ReadByte();
      this.Title = reader.ReadString();
      this.Text = new string[reader.ReadInt32()];
      for (int index = 0; index < this.Text.Length; ++index)
        this.Text[index] = reader.ReadString();
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.ID);
      writer.Write(this.Title);
      writer.Write(this.Text.Length);
      for (int index = 0; index < this.Text.Length; ++index)
        writer.Write(this.Text[index] == null ? "" : this.Text[index]);
    }

    public void LoadFromSaveData(SaveBookState state)
    {
      this.ID = state.ID;
      this.Title = state.Title;
      this.Text = state.Text;
    }

    public void WriteToScript(List<string> commands)
    {
      commands.Add("var [x] = [0] // set as required");
      commands.Add("var [y] = [0] // set as required");
      commands.Add("var [z] = [0] // set as required");
      commands.Add("SetText [x,y,z] [" + this.Title + "] [name]");
      for (int index = 0; index < this.Text.Length; ++index)
      {
        if (this.Text[index].IsNotEmpty())
          commands.Add("SetText [x,y,z] [" + this.Text[index] + "] [" + (object) (index + 1) + "]");
      }
    }
  }
}
