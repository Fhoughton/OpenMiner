// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Net.SteamCallback`1
// Assembly: StudioForge.Engine.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC512B22-6907-49CA-B98F-0785F8A4B040
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Net.dll

using System;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Net
{
  internal class SteamCallback<T>
  {
    private readonly int structureSize = Marshal.SizeOf(typeof (T));
    private CCallbackBaseVTable vTable;
    private IntPtr vTablePointer;
    private CCallbackBase callback;
    private GCHandle callbackPointer;
    private bool isGameServer;

    private event SteamCallback<T>.DispatchDelegate function;

    public SteamCallback(SteamCallback<T>.DispatchDelegate func, bool isGameServer)
    {
      this.isGameServer = isGameServer;
      this.vTable = new CCallbackBaseVTable(new CCallbackBaseVTable.RunCallbackDelegate(this.OnRunCallback), (CCallbackBaseVTable.RunCallResultDelegate) null, (CCallbackBaseVTable.GetCallbackSizeBytesDelegate) null);
      this.vTablePointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (CCallbackBaseVTable)));
      Marshal.StructureToPtr((object) this.vTable, this.vTablePointer, false);
      this.callback = new CCallbackBase()
      {
        m_vTable = this.vTablePointer,
        m_nCallbackFlags = (byte) 0,
        m_iCallback = CallbackIdentities.GetCallbackIdentity(typeof (T))
      };
      this.callbackPointer = GCHandle.Alloc((object) this.callback, GCHandleType.Pinned);
      if (func == null)
        throw new Exception("Callback function must not be null.");
      if (((int) this.callback.m_nCallbackFlags & 1) == 1)
        this.Unregister();
      if (isGameServer)
        this.callback.m_nCallbackFlags |= (byte) 2;
      this.function = func;
      SteamAPI.RegisterCallback(this.callbackPointer.AddrOfPinnedObject(), CallbackIdentities.GetCallbackIdentity(typeof (T)));
    }

    ~SteamCallback()
    {
      this.Unregister();
      if (this.vTablePointer != IntPtr.Zero)
        Marshal.FreeHGlobal(this.vTablePointer);
      if (!this.callbackPointer.IsAllocated)
        return;
      this.callbackPointer.Free();
    }

    public void Unregister()
    {
      SteamAPI.UnregisterCallback(this.callbackPointer.AddrOfPinnedObject());
    }

    private void OnRunCallback(IntPtr thisPtr, IntPtr pvParam)
    {
      try
      {
        this.function((T) Marshal.PtrToStructure(pvParam, typeof (T)));
      }
      catch (Exception ex)
      {
        CallbackDispatcher.ExceptionHandler(ex);
      }
    }

    public delegate void DispatchDelegate(T param);
  }
}
