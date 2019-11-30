// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.UserControls
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Input;

namespace StudioForge.TotalMiner
{
  internal class UserControls
  {
    public UserControlSetting Scheme;
    public Buttons Jump;
    public Buttons OpenInventory;
    public Buttons OpenCraftScreen;
    public Buttons Prospect;
    public Buttons DropItem;
    public Buttons Crouch;
    public Buttons CrouchHold;
    public Buttons FlyToggle;
    public Buttons FlyAscend;
    public Buttons FlyDescend;
    public Buttons LeftHand;
    public Buttons RightHand;
    public Buttons HotBarLeft;
    public Buttons HotBarRight;
    public Buttons PauseMenu;
    public Buttons OpenTopMap;
    public Buttons SpecialKey;
    public Buttons ProspectPickItem;
    public Buttons ProspectStockItem;
    public Buttons TimeFFwd;
    public Buttons TimeRev;
    public Buttons NoClipToggle;
    public Buttons PasteMerge;
    public Buttons PasteNoOverwrite;
    public Buttons ClipboardZoomIn;
    public Buttons ClipboardZoomOut;
    public Buttons ClipboardZoomRotateLeft;
    public Buttons ClipboardZoomRotateRight;
    public Buttons SpectatePlayerLeft;
    public Buttons SpectatePlayerRight;
    public Buttons TextMessageShortcut;
    public Buttons InvEquipLeftItem;
    public Buttons InvEquipRightItem;
    public Buttons InvTransferItem;
    public Buttons InvDropItem;
    public Buttons InvLiftItem;
    public Buttons InvLiftSingleItem;
    public Buttons InvExamineItem;
    public Buttons SwitchTabLeft;
    public Buttons SwitchTabRight;
    public Buttons CraftEquipLeftItem;
    public Buttons CraftEquipRightItem;
    public Buttons CraftEasyCraft;
    public Buttons CreativeBlocks;
    public Buttons CreativeItems;
    public Buttons CreativeMenu;

    public bool CraftScreenFromInventoryScreen
    {
      get
      {
        return this.OpenCraftScreen != this.InvLiftSingleItem;
      }
    }

    public void Initialize(UserControlSetting setting)
    {
      this.Scheme = setting;
      switch (setting)
      {
        case UserControlSetting.TotalMiner2_0:
          this.InitSetupTotalMiner2_0();
          break;
        case UserControlSetting.TotalMiner2_0_Alt:
          this.InitSetupTotalMiner2_0_Alt();
          break;
        case UserControlSetting.TotalMiner1_9:
          this.InitSetupTotalMiner1_9();
          break;
        case UserControlSetting.Shoulders2_0:
          this.InitSetupShoulders2_0();
          break;
        case UserControlSetting.MC360:
          this.InitMinecraft();
          break;
      }
    }

    private void InitSetupTotalMiner2_0()
    {
      this.Jump = Buttons.A;
      this.OpenInventory = Buttons.B;
      this.OpenCraftScreen = Buttons.Start;
      this.Prospect = Buttons.Y;
      this.DropItem = Buttons.RightStick;
      this.Crouch = Buttons.LeftStick;
      this.CrouchHold = Buttons.A;
      this.FlyToggle = Buttons.X;
      this.FlyAscend = Buttons.A;
      this.FlyDescend = Buttons.LeftStick;
      this.LeftHand = Buttons.LeftTrigger;
      this.RightHand = Buttons.RightTrigger;
      this.HotBarLeft = Buttons.LeftShoulder;
      this.HotBarRight = Buttons.RightShoulder;
      this.PauseMenu = Buttons.Start;
      this.OpenTopMap = Buttons.Back;
      this.SpecialKey = Buttons.DPadUp;
      this.ProspectPickItem = Buttons.Y;
      this.ProspectStockItem = Buttons.X;
      this.TimeFFwd = Buttons.RightThumbstickUp;
      this.TimeRev = Buttons.RightThumbstickDown;
      this.NoClipToggle = Buttons.X;
      this.PasteMerge = Buttons.LeftTrigger;
      this.PasteNoOverwrite = Buttons.RightTrigger;
      this.ClipboardZoomIn = Buttons.RightThumbstickDown;
      this.ClipboardZoomOut = Buttons.RightThumbstickUp;
      this.ClipboardZoomRotateLeft = Buttons.RightThumbstickLeft;
      this.ClipboardZoomRotateRight = Buttons.RightThumbstickRight;
      this.SpectatePlayerLeft = Buttons.LeftShoulder;
      this.SpectatePlayerRight = Buttons.RightShoulder;
      this.TextMessageShortcut = Buttons.A;
      this.InvEquipLeftItem = Buttons.LeftTrigger;
      this.InvEquipRightItem = Buttons.RightTrigger;
      this.CraftEquipLeftItem = Buttons.LeftTrigger;
      this.CraftEquipRightItem = Buttons.RightTrigger;
      this.InvTransferItem = Buttons.Y;
      this.InvLiftItem = Buttons.A;
      this.InvLiftSingleItem = Buttons.X;
      this.InvExamineItem = Buttons.LeftShoulder;
      this.InvDropItem = Buttons.RightShoulder;
      this.CraftEasyCraft = Buttons.Start;
      this.SwitchTabLeft = Buttons.LeftTrigger;
      this.SwitchTabRight = Buttons.RightTrigger;
      this.CreativeBlocks = Buttons.DPadLeft;
      this.CreativeItems = Buttons.DPadRight;
      this.CreativeMenu = Buttons.DPadDown;
    }

    private void InitSetupTotalMiner2_0_Alt()
    {
      this.Jump = Buttons.A;
      this.OpenInventory = Buttons.Y;
      this.Prospect = Buttons.B;
      this.DropItem = Buttons.RightStick;
      this.Crouch = Buttons.RightStick;
      this.CrouchHold = Buttons.DPadUp;
      this.FlyToggle = Buttons.X;
      this.FlyAscend = Buttons.A;
      this.FlyDescend = Buttons.RightStick;
      this.LeftHand = Buttons.LeftTrigger;
      this.RightHand = Buttons.RightTrigger;
      this.HotBarLeft = Buttons.LeftShoulder;
      this.HotBarRight = Buttons.RightShoulder;
      this.PauseMenu = Buttons.Start;
      this.OpenTopMap = Buttons.Back;
      this.SpecialKey = Buttons.LeftStick;
      this.ProspectPickItem = Buttons.Y;
      this.ProspectStockItem = Buttons.X;
      this.TimeFFwd = Buttons.RightThumbstickUp;
      this.TimeRev = Buttons.RightThumbstickDown;
      this.NoClipToggle = Buttons.X;
      this.PasteMerge = Buttons.LeftTrigger;
      this.PasteNoOverwrite = Buttons.RightTrigger;
      this.ClipboardZoomIn = Buttons.RightThumbstickDown;
      this.ClipboardZoomOut = Buttons.RightThumbstickUp;
      this.ClipboardZoomRotateLeft = Buttons.RightThumbstickLeft;
      this.ClipboardZoomRotateRight = Buttons.RightThumbstickRight;
      this.SpectatePlayerLeft = Buttons.LeftShoulder;
      this.SpectatePlayerRight = Buttons.RightShoulder;
      this.TextMessageShortcut = Buttons.A;
      this.InvEquipLeftItem = Buttons.LeftShoulder;
      this.InvEquipRightItem = Buttons.RightShoulder;
      this.CraftEquipLeftItem = Buttons.LeftShoulder;
      this.CraftEquipRightItem = Buttons.RightShoulder;
      this.InvTransferItem = Buttons.Y;
      this.InvLiftItem = Buttons.A;
      this.InvLiftSingleItem = Buttons.X;
      this.InvExamineItem = Buttons.LeftTrigger;
      this.InvDropItem = Buttons.RightTrigger;
      this.CraftEasyCraft = Buttons.Start;
      this.OpenCraftScreen = Buttons.Start;
      this.SwitchTabLeft = Buttons.LeftShoulder;
      this.SwitchTabRight = Buttons.RightShoulder;
      this.CreativeBlocks = Buttons.DPadLeft;
      this.CreativeItems = Buttons.DPadRight;
      this.CreativeMenu = Buttons.DPadDown;
    }

    private void InitSetupTotalMiner1_9()
    {
      this.Jump = Buttons.A;
      this.OpenInventory = Buttons.X;
      this.OpenCraftScreen = Buttons.Y;
      this.Prospect = Buttons.Y;
      this.DropItem = Buttons.RightStick;
      this.Crouch = Buttons.LeftStick;
      this.CrouchHold = Buttons.A;
      this.FlyToggle = Buttons.B;
      this.FlyAscend = Buttons.A;
      this.FlyDescend = Buttons.LeftStick;
      this.LeftHand = Buttons.LeftTrigger;
      this.RightHand = Buttons.RightTrigger;
      this.HotBarLeft = Buttons.LeftShoulder;
      this.HotBarRight = Buttons.RightShoulder;
      this.PauseMenu = Buttons.Start;
      this.OpenTopMap = Buttons.Back;
      this.SpecialKey = Buttons.DPadUp;
      this.ProspectPickItem = Buttons.Y;
      this.ProspectStockItem = Buttons.X;
      this.TimeFFwd = Buttons.RightThumbstickUp;
      this.TimeRev = Buttons.RightThumbstickDown;
      this.NoClipToggle = Buttons.X;
      this.PasteMerge = Buttons.LeftTrigger;
      this.PasteNoOverwrite = Buttons.RightTrigger;
      this.ClipboardZoomIn = Buttons.RightThumbstickDown;
      this.ClipboardZoomOut = Buttons.RightThumbstickUp;
      this.ClipboardZoomRotateLeft = Buttons.RightThumbstickLeft;
      this.ClipboardZoomRotateRight = Buttons.RightThumbstickRight;
      this.SpectatePlayerLeft = Buttons.LeftShoulder;
      this.SpectatePlayerRight = Buttons.RightShoulder;
      this.TextMessageShortcut = Buttons.A;
      this.InvEquipLeftItem = Buttons.LeftTrigger;
      this.InvEquipRightItem = Buttons.RightTrigger;
      this.CraftEquipLeftItem = Buttons.LeftTrigger;
      this.CraftEquipRightItem = Buttons.RightTrigger;
      this.InvTransferItem = Buttons.Start;
      this.InvLiftItem = Buttons.A;
      this.InvLiftSingleItem = Buttons.X;
      this.InvExamineItem = Buttons.LeftShoulder;
      this.InvDropItem = Buttons.RightShoulder;
      this.CraftEasyCraft = Buttons.Y;
      this.SwitchTabLeft = Buttons.LeftTrigger;
      this.SwitchTabRight = Buttons.RightTrigger;
      this.CreativeBlocks = Buttons.DPadLeft;
      this.CreativeItems = Buttons.DPadRight;
      this.CreativeMenu = Buttons.DPadDown;
    }

    private void InitSetupShoulders2_0()
    {
      this.Jump = Buttons.A;
      this.OpenInventory = Buttons.B;
      this.Prospect = Buttons.Y;
      this.DropItem = Buttons.RightStick;
      this.Crouch = Buttons.LeftStick;
      this.CrouchHold = Buttons.A;
      this.FlyToggle = Buttons.X;
      this.FlyAscend = Buttons.A;
      this.FlyDescend = Buttons.LeftStick;
      this.LeftHand = Buttons.LeftShoulder;
      this.RightHand = Buttons.RightShoulder;
      this.HotBarLeft = Buttons.LeftTrigger;
      this.HotBarRight = Buttons.RightTrigger;
      this.PauseMenu = Buttons.Start;
      this.OpenTopMap = Buttons.Back;
      this.SpecialKey = Buttons.DPadUp;
      this.ProspectPickItem = Buttons.Y;
      this.ProspectStockItem = Buttons.X;
      this.TimeFFwd = Buttons.RightThumbstickUp;
      this.TimeRev = Buttons.RightThumbstickDown;
      this.NoClipToggle = Buttons.X;
      this.PasteMerge = Buttons.LeftShoulder;
      this.PasteNoOverwrite = Buttons.RightShoulder;
      this.ClipboardZoomIn = Buttons.RightThumbstickDown;
      this.ClipboardZoomOut = Buttons.RightThumbstickUp;
      this.ClipboardZoomRotateLeft = Buttons.RightThumbstickLeft;
      this.ClipboardZoomRotateRight = Buttons.RightThumbstickRight;
      this.SpectatePlayerLeft = Buttons.LeftTrigger;
      this.SpectatePlayerRight = Buttons.RightTrigger;
      this.TextMessageShortcut = Buttons.A;
      this.InvEquipLeftItem = Buttons.LeftShoulder;
      this.InvEquipRightItem = Buttons.RightShoulder;
      this.CraftEquipLeftItem = Buttons.LeftShoulder;
      this.CraftEquipRightItem = Buttons.RightShoulder;
      this.InvTransferItem = Buttons.Y;
      this.InvLiftItem = Buttons.A;
      this.InvLiftSingleItem = Buttons.X;
      this.InvExamineItem = Buttons.LeftTrigger;
      this.InvDropItem = Buttons.RightTrigger;
      this.CraftEasyCraft = Buttons.Start;
      this.OpenCraftScreen = Buttons.Start;
      this.SwitchTabLeft = Buttons.LeftShoulder;
      this.SwitchTabRight = Buttons.RightShoulder;
      this.CreativeBlocks = Buttons.DPadLeft;
      this.CreativeItems = Buttons.DPadRight;
      this.CreativeMenu = Buttons.DPadDown;
    }

    private void InitMinecraft()
    {
      this.Jump = Buttons.A;
      this.OpenInventory = Buttons.Y;
      this.Prospect = Buttons.B;
      this.DropItem = Buttons.RightStick;
      this.Crouch = Buttons.RightStick;
      this.CrouchHold = Buttons.LeftStick;
      this.FlyToggle = Buttons.LeftStick;
      this.FlyAscend = Buttons.A;
      this.FlyDescend = Buttons.RightStick;
      this.LeftHand = Buttons.LeftTrigger;
      this.RightHand = Buttons.RightTrigger;
      this.HotBarLeft = Buttons.LeftShoulder;
      this.HotBarRight = Buttons.RightShoulder;
      this.PauseMenu = Buttons.Start;
      this.OpenTopMap = Buttons.Back;
      this.SpecialKey = Buttons.DPadUp;
      this.ProspectPickItem = Buttons.Y;
      this.ProspectStockItem = Buttons.X;
      this.TimeFFwd = Buttons.RightThumbstickUp;
      this.TimeRev = Buttons.RightThumbstickDown;
      this.NoClipToggle = Buttons.X;
      this.PasteMerge = Buttons.LeftTrigger;
      this.PasteNoOverwrite = Buttons.RightTrigger;
      this.ClipboardZoomIn = Buttons.RightThumbstickDown;
      this.ClipboardZoomOut = Buttons.RightThumbstickUp;
      this.ClipboardZoomRotateLeft = Buttons.RightThumbstickLeft;
      this.ClipboardZoomRotateRight = Buttons.RightThumbstickRight;
      this.SpectatePlayerLeft = Buttons.LeftShoulder;
      this.SpectatePlayerRight = Buttons.RightShoulder;
      this.TextMessageShortcut = Buttons.A;
      this.InvEquipLeftItem = Buttons.LeftTrigger;
      this.InvEquipRightItem = Buttons.RightTrigger;
      this.CraftEquipLeftItem = Buttons.LeftTrigger;
      this.CraftEquipRightItem = Buttons.RightTrigger;
      this.InvLiftItem = Buttons.A;
      this.InvLiftSingleItem = Buttons.X;
      this.InvExamineItem = Buttons.LeftStick;
      this.InvDropItem = Buttons.RightStick;
      this.InvTransferItem = Buttons.Y;
      this.CraftEasyCraft = Buttons.Back;
      this.OpenCraftScreen = Buttons.X;
      this.SwitchTabLeft = Buttons.LeftShoulder;
      this.SwitchTabRight = Buttons.RightShoulder;
      this.CreativeBlocks = Buttons.DPadLeft;
      this.CreativeItems = Buttons.DPadRight;
      this.CreativeMenu = Buttons.DPadDown;
    }
  }
}
