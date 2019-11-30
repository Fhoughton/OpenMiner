// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Colors
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.GUI;

namespace StudioForge.TotalMiner
{
  public static class Colors
  {
    public static TextBox.ColorProfile LabelColors;
    public static TextBox.ColorProfile LabelLowAlphaColors;
    public static TextBox.ColorProfile ButtonColors;
    public static TextBox.ColorProfile ButtonConstColors;
    public static TextBox.ColorProfile ButtonAltColors;
    public static TextBox.ColorProfile ButtonWarnColors;
    public static DataField.ColorProfile DataFieldColors;
    public static ListBox.ColorProfile ListBoxColors;
    public static TextBox.ColorProfile StatusGreen;
    public static TextBox.ColorProfile StatusRed;
    public static TextBox.ColorProfile Heading1;
    public static TextBox.ColorProfile Heading2;
    public static Window.ColorProfile PauseMenuTop;
    public static TextBox.ColorProfile PauseMenuTab;
    public static Window.ColorProfile PauseMenuTabHighlight;
    public static Window.ColorProfile PauseMenuLine1;
    public static Window.ColorProfile PauseMenuLine2;
    public static TextBox.ColorProfile PauseMenuKeyboard;
    public static Window.ColorProfile IconColors;
    public static TextBox.ColorProfile InvIcon;
    public static TextBox.ColorProfile InvIconInvalidHover;
    public static Window.ColorProfile GrayTrack;
    public static Window.ColorProfile RedTrack;
    public static Window.ColorProfile GreenTrack;
    public static Window.ColorProfile BlueTrack;
    public static TextBox.ColorProfile BlackText;
    public static TextBox.ColorProfile NodeDesignerButton;
    public static TextBox.ColorProfile NodeHeader;
    public static TextBox.ColorProfile NodeType;
    public static Window.ColorProfile NodeContainer;
    public static DataField.ColorProfile NodeTree;
    public static TextBox.ColorProfile NodeDropButton;
    public static TextBox.ColorProfile NodeLogic;
    public static TextBox.ColorProfile NodeConditional;
    public static TextBox.ColorProfile NodeAction;
    public static TextBox.ColorProfile NodeDisabled;
    public static TextBox.ColorProfile NodeLine;
    public static TextBox.ColorProfile NodeLineEnd;
    public static TextBox.ColorProfile DialogSilver;
    public static TextBox.ColorProfile DialogGold;

    static Colors()
    {
      TextBox.ColorProfile colorProfile1 = new TextBox.ColorProfile();
      colorProfile1.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile1.BackClickColor = TextBox.DefaultColorProfile.BackColor * 0.65f;
      colorProfile1.BackColor = TextBox.DefaultColorProfile.BackColor * 0.65f;
      colorProfile1.BackHoverColor = TextBox.DefaultColorProfile.BackColor * 0.65f;
      colorProfile1.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile1.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile1.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.LabelColors = colorProfile1;
      TextBox.ColorProfile colorProfile2 = new TextBox.ColorProfile();
      colorProfile2.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile2.BackClickColor = TextBox.DefaultColorProfile.BackColor * 0.35f;
      colorProfile2.BackColor = TextBox.DefaultColorProfile.BackColor * 0.35f;
      colorProfile2.BackHoverColor = TextBox.DefaultColorProfile.BackColor * 0.35f;
      colorProfile2.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile2.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile2.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.LabelLowAlphaColors = colorProfile2;
      TextBox.ColorProfile colorProfile3 = new TextBox.ColorProfile();
      colorProfile3.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile3.BackClickColor = TextBox.DefaultColorProfile.BackClickColor;
      colorProfile3.BackColor = TextBox.DefaultColorProfile.BackColor * 0.8f;
      colorProfile3.BackHoverColor = TextBox.DefaultColorProfile.BackHoverColor;
      colorProfile3.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile3.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile3.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.ButtonColors = colorProfile3;
      TextBox.ColorProfile colorProfile4 = new TextBox.ColorProfile();
      colorProfile4.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile4.BackColor = TextBox.DefaultColorProfile.BackColor * 0.9f;
      colorProfile4.BackHoverColor = TextBox.DefaultColorProfile.BackHoverColor * 0.9f;
      colorProfile4.BackClickColor = TextBox.DefaultColorProfile.BackHoverColor * 0.9f;
      colorProfile4.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile4.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile4.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.ButtonConstColors = colorProfile4;
      TextBox.ColorProfile colorProfile5 = new TextBox.ColorProfile();
      colorProfile5.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile5.BackClickColor = new Color(100, 220, 100, 200);
      colorProfile5.BackColor = new Color(50, 220, 50, 200);
      colorProfile5.BackHoverColor = new Color(75, 220, 75, 200);
      colorProfile5.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile5.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile5.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.ButtonAltColors = colorProfile5;
      TextBox.ColorProfile colorProfile6 = new TextBox.ColorProfile();
      colorProfile6.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile6.BackClickColor = new Color((int) byte.MaxValue, 100, 100, 200);
      colorProfile6.BackColor = new Color((int) byte.MaxValue, 50, 50, 200);
      colorProfile6.BackHoverColor = new Color((int) byte.MaxValue, 75, 75, 200);
      colorProfile6.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile6.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile6.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.ButtonWarnColors = colorProfile6;
      DataField.ColorProfile colorProfile7 = new DataField.ColorProfile();
      colorProfile7.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile7.BackClickColor = DataField.DefaultColorProfile.BackClickColor;
      colorProfile7.BackColor = DataField.DefaultColorProfile.BackColor * 0.8f;
      colorProfile7.BackHoverColor = DataField.DefaultColorProfile.BackHoverColor;
      colorProfile7.BorderColor = DataField.DefaultColorProfile.BorderColor;
      colorProfile7.ForeColor = DataField.DefaultColorProfile.ForeColor;
      colorProfile7.TextColor = DataField.DefaultColorProfile.TextColor;
      colorProfile7.BackInputColor = DataField.DefaultColorProfile.BackInputColor;
      colorProfile7.BackSelectedTextColor = DataField.DefaultColorProfile.BackSelectedTextColor;
      Colors.DataFieldColors = colorProfile7;
      ListBox.ColorProfile colorProfile8 = new ListBox.ColorProfile();
      colorProfile8.BackDisabledColor = ListBox.DefaultColorProfile.BackDisabledColor;
      colorProfile8.BackClickColor = ListBox.DefaultColorProfile.BackClickColor;
      colorProfile8.BackColor = ListBox.DefaultColorProfile.BackColor;
      colorProfile8.BackHoverColor = ListBox.DefaultColorProfile.BackHoverColor;
      colorProfile8.BorderColor = ListBox.DefaultColorProfile.BorderColor;
      colorProfile8.ForeColor = ListBox.DefaultColorProfile.ForeColor;
      colorProfile8.TextColor = ListBox.DefaultColorProfile.TextColor;
      colorProfile8.BackHighlightColor = ListBox.DefaultColorProfile.BackHighlightColor;
      colorProfile8.ForeHighlightColor = ListBox.DefaultColorProfile.ForeHighlightColor;
      colorProfile8.NavigableColor = ListBox.DefaultColorProfile.NavigableColor;
      Colors.ListBoxColors = colorProfile8;
      TextBox.ColorProfile colorProfile9 = new TextBox.ColorProfile();
      colorProfile9.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile9.BackClickColor = new Color(150, (int) byte.MaxValue, 150, 200);
      colorProfile9.BackColor = new Color(150, (int) byte.MaxValue, 150, 200);
      colorProfile9.BackHoverColor = new Color(150, (int) byte.MaxValue, 150, 200);
      colorProfile9.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile9.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile9.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.StatusGreen = colorProfile9;
      TextBox.ColorProfile colorProfile10 = new TextBox.ColorProfile();
      colorProfile10.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile10.BackClickColor = new Color((int) byte.MaxValue, 150, 150, 200);
      colorProfile10.BackColor = new Color((int) byte.MaxValue, 150, 150, 200);
      colorProfile10.BackHoverColor = new Color((int) byte.MaxValue, 150, 150, 200);
      colorProfile10.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile10.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile10.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.StatusRed = colorProfile10;
      TextBox.ColorProfile colorProfile11 = new TextBox.ColorProfile();
      colorProfile11.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile11.BackClickColor = new Color(150, 150, (int) byte.MaxValue, 200);
      colorProfile11.BackColor = new Color(150, 150, (int) byte.MaxValue, 200);
      colorProfile11.BackHoverColor = new Color(150, 150, (int) byte.MaxValue, 200);
      colorProfile11.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile11.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile11.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.Heading1 = colorProfile11;
      TextBox.ColorProfile colorProfile12 = new TextBox.ColorProfile();
      colorProfile12.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile12.BackClickColor = new Color((int) byte.MaxValue, 150, 150, 200);
      colorProfile12.BackColor = new Color((int) byte.MaxValue, 150, 150, 200);
      colorProfile12.BackHoverColor = new Color((int) byte.MaxValue, 150, 150, 200);
      colorProfile12.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile12.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile12.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.Heading2 = colorProfile12;
      Colors.PauseMenuTop = new Window.ColorProfile()
      {
        BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor,
        BackClickColor = new Color(20, 20, 20, 240),
        BackColor = new Color(20, 20, 20, 240),
        BackHoverColor = new Color(20, 20, 20, 240),
        BorderColor = TextBox.DefaultColorProfile.BorderColor,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      TextBox.ColorProfile colorProfile13 = new TextBox.ColorProfile();
      colorProfile13.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile13.BackClickColor = new Color(150, 150, 150, (int) byte.MaxValue);
      colorProfile13.BackColor = new Color(50, 50, 50, (int) byte.MaxValue);
      colorProfile13.BackHoverColor = new Color(100, 100, 100, (int) byte.MaxValue);
      colorProfile13.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile13.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile13.TextColor = new Color(200, 200, 200, (int) byte.MaxValue);
      Colors.PauseMenuTab = colorProfile13;
      Colors.PauseMenuTabHighlight = new Window.ColorProfile()
      {
        BackDisabledColor = Color.White,
        BackClickColor = Color.White,
        BackColor = Color.White,
        BackHoverColor = Color.White,
        BorderColor = TextBox.DefaultColorProfile.BorderColor,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      Colors.PauseMenuLine1 = new Window.ColorProfile()
      {
        BackDisabledColor = new Color(16, 117, 14),
        BackClickColor = new Color(16, 117, 14),
        BackColor = new Color(16, 117, 14),
        BackHoverColor = new Color(16, 117, 14),
        BorderColor = TextBox.DefaultColorProfile.BorderColor,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      Colors.PauseMenuLine2 = new Window.ColorProfile()
      {
        BackDisabledColor = new Color(114, 80, 53),
        BackClickColor = new Color(114, 80, 53),
        BackColor = new Color(114, 80, 53),
        BackHoverColor = new Color(114, 80, 53),
        BorderColor = TextBox.DefaultColorProfile.BorderColor,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      TextBox.ColorProfile colorProfile14 = new TextBox.ColorProfile();
      colorProfile14.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile14.BackClickColor = new Color(20, 20, 20, 240);
      colorProfile14.BackColor = new Color(20, 20, 20, 240);
      colorProfile14.BackHoverColor = new Color(20, 20, 20, 240);
      colorProfile14.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile14.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile14.TextColor = Color.White;
      Colors.PauseMenuKeyboard = colorProfile14;
      Colors.IconColors = new Window.ColorProfile()
      {
        BackDisabledColor = Color.Transparent,
        BackClickColor = Color.Transparent,
        BackColor = Color.Transparent,
        BackHoverColor = Color.Transparent,
        BorderColor = TextBox.DefaultColorProfile.BorderColor,
        ForeColor = Color.White
      };
      TextBox.ColorProfile colorProfile15 = new TextBox.ColorProfile();
      colorProfile15.BackDisabledColor = Color.Transparent;
      colorProfile15.BackClickColor = Color.White * 0.5f;
      colorProfile15.BackColor = Color.White * 0.05f;
      colorProfile15.BackHoverColor = Color.White * 0.3f;
      colorProfile15.BorderColor = Color.White;
      colorProfile15.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile15.TextColor = Color.White;
      Colors.InvIcon = colorProfile15;
      TextBox.ColorProfile colorProfile16 = new TextBox.ColorProfile();
      colorProfile16.BackDisabledColor = Color.Transparent;
      colorProfile16.BackClickColor = Color.White * 0.5f;
      colorProfile16.BackColor = Color.White * 0.05f;
      colorProfile16.BackHoverColor = Color.Red * 0.3f;
      colorProfile16.BorderColor = Color.White;
      colorProfile16.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile16.TextColor = Color.White;
      Colors.InvIconInvalidHover = colorProfile16;
      Colors.GrayTrack = new Window.ColorProfile()
      {
        BackDisabledColor = Color.Transparent,
        BackClickColor = Color.Transparent,
        BackColor = Color.Transparent,
        BackHoverColor = Color.Transparent,
        BorderColor = Color.DarkGray,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      Colors.RedTrack = new Window.ColorProfile()
      {
        BackDisabledColor = Color.Transparent,
        BackClickColor = Color.Transparent,
        BackColor = Color.Transparent,
        BackHoverColor = Color.Transparent,
        BorderColor = Color.Red,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      Colors.GreenTrack = new Window.ColorProfile()
      {
        BackDisabledColor = Color.Transparent,
        BackClickColor = Color.Transparent,
        BackColor = Color.Transparent,
        BackHoverColor = Color.Transparent,
        BorderColor = Color.Green,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      Colors.BlueTrack = new Window.ColorProfile()
      {
        BackDisabledColor = Color.Transparent,
        BackClickColor = Color.Transparent,
        BackColor = Color.Transparent,
        BackHoverColor = Color.Transparent,
        BorderColor = Color.Blue,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      TextBox.ColorProfile colorProfile17 = new TextBox.ColorProfile();
      colorProfile17.BackDisabledColor = Color.Transparent;
      colorProfile17.BackClickColor = Color.Transparent;
      colorProfile17.BackColor = Color.Transparent;
      colorProfile17.BackHoverColor = Color.Transparent;
      colorProfile17.BorderColor = Color.Transparent;
      colorProfile17.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile17.TextColor = Color.Black;
      Colors.BlackText = colorProfile17;
      TextBox.ColorProfile colorProfile18 = new TextBox.ColorProfile();
      colorProfile18.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile18.BackClickColor = new Color(232, 232, 232);
      colorProfile18.BackColor = new Color(186, 186, 186);
      colorProfile18.BackHoverColor = new Color(224, 224, 224);
      colorProfile18.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile18.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile18.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeDesignerButton = colorProfile18;
      TextBox.ColorProfile colorProfile19 = new TextBox.ColorProfile();
      colorProfile19.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile19.BackClickColor = new Color(232, 232, 232);
      colorProfile19.BackColor = new Color(232, 232, 232);
      colorProfile19.BackHoverColor = new Color(232, 232, 232);
      colorProfile19.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile19.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile19.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeHeader = colorProfile19;
      TextBox.ColorProfile colorProfile20 = new TextBox.ColorProfile();
      colorProfile20.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile20.BackClickColor = new Color(232, 232, 232);
      colorProfile20.BackColor = new Color(200, 200, 200);
      colorProfile20.BackHoverColor = new Color(224, 224, 224);
      colorProfile20.BorderColor = TextBox.DefaultColorProfile.BorderColor;
      colorProfile20.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile20.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeType = colorProfile20;
      Colors.NodeContainer = new Window.ColorProfile()
      {
        BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor,
        BackClickColor = Color.Black,
        BackColor = Color.Black,
        BackHoverColor = Color.Black,
        BorderColor = Color.DarkBlue,
        ForeColor = TextBox.DefaultColorProfile.ForeColor
      };
      DataField.ColorProfile colorProfile21 = new DataField.ColorProfile();
      colorProfile21.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile21.BackClickColor = Color.Red;
      colorProfile21.BackColor = Color.Red;
      colorProfile21.BackHoverColor = Color.Red;
      colorProfile21.BorderColor = Color.White;
      colorProfile21.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile21.TextColor = TextBox.DefaultColorProfile.TextColor;
      colorProfile21.BackInputColor = Color.Red;
      colorProfile21.BackSelectedTextColor = DataField.DefaultColorProfile.BackSelectedTextColor;
      Colors.NodeTree = colorProfile21;
      TextBox.ColorProfile colorProfile22 = new TextBox.ColorProfile();
      colorProfile22.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile22.BackClickColor = Color.Transparent;
      colorProfile22.BackColor = Color.Transparent;
      colorProfile22.BackHoverColor = Color.LightGray;
      colorProfile22.BorderColor = Color.White;
      colorProfile22.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile22.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeDropButton = colorProfile22;
      TextBox.ColorProfile colorProfile23 = new TextBox.ColorProfile();
      colorProfile23.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile23.BackClickColor = new Color(0, 0, 224);
      colorProfile23.BackColor = new Color(0, 0, 224);
      colorProfile23.BackHoverColor = new Color(0, 0, 224);
      colorProfile23.BorderColor = Color.White;
      colorProfile23.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile23.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeLogic = colorProfile23;
      TextBox.ColorProfile colorProfile24 = new TextBox.ColorProfile();
      colorProfile24.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile24.BackClickColor = Color.Orange;
      colorProfile24.BackColor = Color.Orange;
      colorProfile24.BackHoverColor = Color.Orange;
      colorProfile24.BorderColor = Color.White;
      colorProfile24.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile24.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeConditional = colorProfile24;
      TextBox.ColorProfile colorProfile25 = new TextBox.ColorProfile();
      colorProfile25.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile25.BackClickColor = new Color(0, 164, 0);
      colorProfile25.BackColor = new Color(0, 164, 0);
      colorProfile25.BackHoverColor = new Color(0, 164, 0);
      colorProfile25.BorderColor = Color.White;
      colorProfile25.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile25.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeAction = colorProfile25;
      TextBox.ColorProfile colorProfile26 = new TextBox.ColorProfile();
      colorProfile26.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile26.BackClickColor = Color.Gray;
      colorProfile26.BackColor = Color.Gray;
      colorProfile26.BackHoverColor = Color.Gray;
      colorProfile26.BorderColor = Color.White;
      colorProfile26.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile26.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeDisabled = colorProfile26;
      TextBox.ColorProfile colorProfile27 = new TextBox.ColorProfile();
      colorProfile27.BackDisabledColor = Color.Yellow;
      colorProfile27.BackClickColor = Color.Yellow;
      colorProfile27.BackColor = Color.Yellow;
      colorProfile27.BackHoverColor = Color.Yellow;
      colorProfile27.BorderColor = Color.Yellow;
      colorProfile27.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile27.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeLine = colorProfile27;
      TextBox.ColorProfile colorProfile28 = new TextBox.ColorProfile();
      colorProfile28.BackDisabledColor = Color.Transparent;
      colorProfile28.BackClickColor = Color.Transparent;
      colorProfile28.BackColor = Color.Transparent;
      colorProfile28.BackHoverColor = Color.Transparent;
      colorProfile28.BorderColor = Color.White;
      colorProfile28.ForeColor = Color.Yellow;
      colorProfile28.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.NodeLineEnd = colorProfile28;
      TextBox.ColorProfile colorProfile29 = new TextBox.ColorProfile();
      colorProfile29.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile29.BackClickColor = Color.Silver;
      colorProfile29.BackColor = Color.Silver;
      colorProfile29.BackHoverColor = Color.Silver;
      colorProfile29.BorderColor = Color.White;
      colorProfile29.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile29.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.DialogSilver = colorProfile29;
      TextBox.ColorProfile colorProfile30 = new TextBox.ColorProfile();
      colorProfile30.BackDisabledColor = TextBox.DefaultColorProfile.BackDisabledColor;
      colorProfile30.BackClickColor = Color.Gold;
      colorProfile30.BackColor = Color.Gold;
      colorProfile30.BackHoverColor = Color.Gold;
      colorProfile30.BorderColor = Color.White;
      colorProfile30.ForeColor = TextBox.DefaultColorProfile.ForeColor;
      colorProfile30.TextColor = TextBox.DefaultColorProfile.TextColor;
      Colors.DialogGold = colorProfile30;
    }
  }
}
