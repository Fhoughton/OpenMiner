// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.InteropHelp
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace StudioForge.Engine.Net
{
  internal class InteropHelp
  {
    public static string IntPtrToUTF8(IntPtr ptr)
    {
      if (ptr == IntPtr.Zero)
        return string.Empty;
      int ofs = 0;
      while (Marshal.ReadByte(ptr, ofs) != (byte) 0)
        ++ofs;
      if (ofs == 0)
        return string.Empty;
      byte[] numArray = new byte[ofs];
      Marshal.Copy(ptr, numArray, 0, numArray.Length);
      return Encoding.UTF8.GetString(numArray);
    }
  }
}
