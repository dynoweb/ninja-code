// Type: NinjaTrader.Indicator.rcVolatilityBreakout
// Assembly: rcVolatilityBreakout, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 44D1D67D-B887-419E-93CB-54BC561A67EC
// Assembly location: C:\Users\rcromer\Documents\NinjaTrader 7\bin\Custom\rcVolatilityBreakout.dll

//using A;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NinjaTrader.Indicator
{
  [Description(".")]
  public class rcVolatilityBreakout : Indicator
  {
	#region variables
    private int length = 20;
    private double number_2_0 = 2.0;
    private double number_1_5 = 1.5;
    private int number_20 = 20;
//    private Color red = Color.Red;	// 1
//    private Color gray = Color.Gray; // 1
//    private Color lime = Color.Lime; // 1
//    private Color darkGreen = Color.DarkGreen; // 1
//    private Color red2 = Color.Red; // 1
//    private Color maroon = Color.Maroon; // 1
//    private Color darkGreen2 = Color.DarkGreen; // 1
//    private Color darkRed = Color.DarkRed; // 1
    private int number_2 = 2;
    private int number_5 = 5;
    private string aString = "aString";
    private List<IRectangle> list_IRectangle = new List<IRectangle>();
    private Dictionary<ILine, Color> dictionary1 = new Dictionary<ILine, Color>();
    private Dictionary<IText, Color> dictionary2 = new Dictionary<IText, Color>();
    private int[] intArray8_1 = new int[8];
    private int[] intArray8_2 = new int[8];
    [XmlIgnore]
    private Plot boxOutline = new Plot(new Pen(Color.Orchid, 1f), "boxOutline");
    private string email = "";
    private int barsLookback = 30;
    private bool extendedVisible = true;
    private bool showLines = true;
    [XmlIgnore]
    private Plot extendedHigh = new Plot(new Pen(Color.Green, 1f), "extendedHigh");
    private bool extendedHighVisible = true;
    [XmlIgnore]
    private Plot extendedLow = new Plot(new Pen(Color.Red, 1f), "extendedLow");
    private bool extendedLowVisible = true;
    [XmlIgnore]
    private Plot extendedMiddle = new Plot(new Pen(Color.Gray, 1f), "extendedMiddle");
    private int extendedMiddleLineLength = 6;
    private int extendedLineLength = 20;
    private Color boxAreaColor = Color.Blue;
    private int boxAreaOpacity = 1;
    private bool tickHeightVisible = true;
    private string tickHeightPosition = LEFT;
    private Color tickHeightColor = Color.Black;
    private string fontSize = "7";
    private bool extendedLevel1Visible = true;
    private bool extendedLevel2Visible = true;
    private bool extendedLevel3Visible = true;
    private int extendedLevel1 = 50;
    private int extendedLevel2 = 100;
    private int extendedLevel3 = 200;
    [XmlIgnore]
    private Plot extendedLineLevel1 = new Plot(new Pen(Color.Gray, 1f), "extendedLineLevel1");
    [XmlIgnore]
    private Plot extendedLineLevel2 = new Plot(new Pen(Color.Gray, 1f), "extendedLineLevel2");
    [XmlIgnore]
    private Plot extendedLineLevel3 = new Plot(new Pen(Color.Gray, 1f), "extendedLineLevel3");
    private string soundHigh = "Alert1.wav";
    private string soundLow = "Alert1.wav";
    private string soundLevel1 = "Alert1.wav";
    private string soundLevel2 = "Alert1.wav";
    private string soundLevel3 = "Alert1.wav";
    private DataSeries dataSeries1;
    private DataSeries dataSeries2;
//    private int c84334302133d3436d40fc5c5c9cd118a;
//    private int c703c1b4d798d0f3074ba94d947ccca39;
    private ToolStripDropDownButton toolStripDropDownButton;
    private int barsSinceSqueeze;
    private Font font;
    private Color color1;
    private Color color2;
    private Image image;
    private rcVolatilityBreakout.hitPanel myHitPanel;
    private int breakoutRecNumb = 0;
    private int aBarNumber = 0;
    private int tempCurrentBar = 0;
    private double squeezeHigh;
    private double squeezeLow;
    private ToolStripSeparator toolStripSeparator;
    private ToolStripButton toolStripButton;
    private IRectangle iRectangle;
    private ILine iLine;
    private IText iText;
    private int squeezeLength;
    private double squeezeMidPoint;
    private double squeezeRange;
    private int tempCurrentBar2;
    private bool shortLogo;
    private bool extendedMiddleVisible;
    private bool multiAlert;
	private static string LEFT = "Left";
	private static string RIGHT = "Right";
	private static string BOTH = "Both";
	private static string DISABLED = "Disabled";
	#endregion

	protected override void Initialize()      
	{
		Add(new Plot(Color.Transparent, (PlotStyle) 3, "Squeeze"));  
		Add(new Plot(Color.Transparent, (PlotStyle) 3, "SqueezeOn")); 
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "PMomentumUp"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "PMomentumDown"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "NMomentumUp"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "NMomentumDown"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "CurrentValue"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "BarsSinceSqueeze"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeLength"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeHigh"));
		Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeLow"));
		
		dataSeries1 = new DataSeries((IndicatorBase) this);
		dataSeries2 = new DataSeries((IndicatorBase) this);
		
		boxOutline.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
		
		CalculateOnBarClose = true;
		Overlay = true;
		PriceTypeSupported = false;
		//PlotsConfigurable = false;
	}
	
    protected override void OnStartUp()
    {
        //ToolStrip toolStrip = ((Control) this.ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
        //this.toolStripSeparator = new ToolStripSeparator();
        //this.toolStripDropDownButton = new ToolStripDropDownButton(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629((int) byte.MaxValue));
        //this.toolStripDropDownButton.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(278);
        //ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(291));
        //toolStripMenuItem1.Checked = this.extendedVisible;
        //toolStripMenuItem1.Click += new EventHandler(this.toggleShowBoxes);
        //this.toolStripDropDownButton.DropDownItems.Add((ToolStripItem) toolStripMenuItem1);
        //ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(302));
        //toolStripMenuItem2.Checked = this.showLines;
        //toolStripMenuItem2.Click += new EventHandler(this.toggleShowLines);
        //this.toolStripDropDownButton.DropDownItems.Add((ToolStripItem) toolStripMenuItem2);
        //toolStrip.Items.Add((ToolStripItem) this.toolStripSeparator);
        //toolStrip.Items.Add((ToolStripItem) this.toolStripDropDownButton);
        //this.aString = ((object) this).GetType().Name;
        //this.font = new Font(((Control) this.get_ChartControl()).Font.FontFamily, (float) Convert.ToInt16(this.fontSize));
    }

	protected override void OnBarUpdate()
	{
		// Are there enough bars
		if (CurrentBar < Length) return;

		if (CurrentBar != 0)
		{
		
			double num1 = ATR(length)[0];
			double num2 = StdDev(Close, length)[0];
			double num3;
			
			if (number_1_5 * num1 == 0.0)
			{
				num3 = 1.0;
			}
			else
				num3 = number_2_0 * num2 / (number_1_5 * num1);
			
			// DEBUG ONLY, IT"S NOT SUPPOSED TO BE HERE
			//BarsSinceSqueeze.Set(num3);

			if (num3 <= 1.0)
			{
				if (Squeeze[1] != 0.0)
				{
					Print(Time + " in != 0.0 - Squeeze[1] " + Squeeze[1] + " ");
					aBarNumber = CurrentBar;
					squeezeHigh = High[0];
					squeezeLow = Low[0];
					breakoutRecNumb++;
				}
				
				if (Squeeze[1] == 0.0)
				{
					Print(Time + " in == 0.0 - Squeeze[1] " + Squeeze[1] + " ");
					if (High[0] > squeezeHigh)
					{
						squeezeHigh = High[0];
					}
					if (Low[0] < squeezeLow)
					{
						squeezeLow = Low[0];
					}
					
					tempCurrentBar = CurrentBar;
					
					if (aBarNumber != tempCurrentBar)
					{
						squeezeMidPoint = (squeezeHigh + squeezeLow) / 2.0;
						squeezeRange = squeezeHigh - squeezeLow;
						iRectangle = DrawRectangle("rectangle" + breakoutRecNumb, AutoScale, CurrentBar - aBarNumber, squeezeHigh, CurrentBar - tempCurrentBar, squeezeLow, boxOutline.Pen.Color, boxAreaColor, boxAreaOpacity);
						((IShape) iRectangle).Pen.DashStyle = boxOutline.Pen.DashStyle;
						((IShape) iRectangle).Pen.Width = boxOutline.Pen.Width;
						
						if (extendedHighVisible)
						{
							iLine = DrawLine("extendedHigh" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeHigh, CurrentBar - tempCurrentBar - extendedLineLength, squeezeHigh, extendedHigh.Pen.Color, extendedHigh.Pen.DashStyle, (int) extendedHigh.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
								dictionary1.Add(iLine, iLine.Pen.Color);
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
						}
						
						if (extendedLowVisible)
						{
							iLine = DrawLine("extendedLow" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeLow, CurrentBar - tempCurrentBar - extendedLineLength, squeezeLow, extendedLow.Pen.Color, extendedLow.Pen.DashStyle, (int) extendedLow.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
						}
						
						if (extendedMiddleVisible)
						{
							iLine = DrawLine("extendedMiddle" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeMidPoint, CurrentBar - tempCurrentBar - extendedMiddleLineLength, squeezeMidPoint, extendedMiddle.Pen.Color, extendedMiddle.Pen.DashStyle, (int) extendedMiddle.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
						}
						
						if (extendedLevel1Visible)
						{
							double num4 = squeezeRange * (double) extendedLevel1 / 100.0;
							iLine = DrawLine("extendedLevel1High" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeHigh + num4, CurrentBar - tempCurrentBar - extendedLineLength, squeezeHigh + num4, extendedLineLevel1.Pen.Color, extendedLineLevel1.Pen.DashStyle, (int) extendedLineLevel1.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							iLine = DrawLine("extendedLevel1Low" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeLow - num4, CurrentBar - tempCurrentBar - extendedLineLength, squeezeLow - num4, extendedLineLevel1.Pen.Color, extendedLineLevel1.Pen.DashStyle, (int) extendedLineLevel1.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}                            
						}
					
						if (extendedLevel2Visible)
						{
							double num4 = squeezeRange * (double) extendedLevel2 / 100.0;
							iLine = DrawLine("extendedLevel2High" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeHigh + num4, CurrentBar - tempCurrentBar - extendedLineLength, squeezeHigh + num4, extendedLineLevel2.Pen.Color, extendedLineLevel2.Pen.DashStyle, (int) extendedLineLevel2.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							iLine = DrawLine("extendedLevel2Low" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeLow - num4, CurrentBar - tempCurrentBar - extendedLineLength, squeezeLow - num4, extendedLineLevel2.Pen.Color, extendedLineLevel2.Pen.DashStyle, (int) extendedLineLevel2.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}                            
						}
					
						if (extendedLevel3Visible)
						{
							double num4 = squeezeRange * (double) extendedLevel3 / 100.0;
							Plot plot = extendedLineLevel3;
							iLine = DrawLine("extendedLevel3High" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeHigh + num4, CurrentBar - tempCurrentBar - extendedLineLength, squeezeHigh + num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
							iLine = DrawLine("extendedLevel3Low" + breakoutRecNumb, AutoScale, CurrentBar - tempCurrentBar, squeezeLow - num4, CurrentBar - tempCurrentBar - extendedLineLength, squeezeLow - num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
							if (!dictionary1.ContainsKey(iLine))
							{
								dictionary1.Add(iLine, iLine.Pen.Color);
							}
							if (!showLines)
							{
								iLine.Pen.Color = Color.Transparent;
							}
						}
					
						if (tickHeightVisible)
						{
							int num4 = Convert.ToInt32((squeezeHigh - squeezeLow) / TickSize);
							if (tickHeightPosition == RIGHT || tickHeightPosition == BOTH)
							{
								iText = DrawText("LText" + breakoutRecNumb, AutoScale, num4.ToString(), CurrentBar - tempCurrentBar, squeezeMidPoint, 0, tickHeightColor, font, StringAlignment.Near, Color.Transparent, Color.Transparent, 1);
								if (!dictionary2.ContainsKey(iText))
								{
									dictionary2.Add(iText, iText.TextColor);
								}
								if (!extendedVisible)
								{
									iText.TextColor = Color.Transparent;
								}
							}
							
							if (tickHeightPosition == LEFT || tickHeightPosition == BOTH)
							{
								iText = DrawText("RText" + breakoutRecNumb, AutoScale, num4.ToString(), CurrentBar - aBarNumber, squeezeMidPoint, 0, tickHeightColor, font, StringAlignment.Far, Color.Transparent, Color.Transparent, 1);
								if (!dictionary2.ContainsKey(iText))
								{
									dictionary2.Add(iText, iText.TextColor);
								}
								if (!extendedVisible)
								{
									iText.TextColor = Color.Transparent;
								}
							}
						}

						if (!extendedVisible)
						{
							color1 = ((IShape) iRectangle).AreaColor;
							color2 = ((IShape) iRectangle).Pen.Color;
							((IShape) iRectangle).AreaColor = Color.Transparent;
							((IShape) iRectangle).Pen.Color = Color.Transparent;
						}
						if (!this.list_IRectangle.Contains(this.iRectangle))
						{
							list_IRectangle.Add(iRectangle);
						}
					}
				}
				Squeeze.Set(0.0);
				SqueezeOn.Reset();
			}
			else
			{
				Squeeze.Reset();
				SqueezeOn.Set(0.0);
				//Print(Time + " in else - Squeeze[0] " + Squeeze[0] + " ");
			}
		
			dataSeries2.Set(Input[0] - (DonchianChannel(Input, number_20).Mean[CurrentBar] + EMA(Input, number_20)[0]) / 2.0);
			dataSeries1.Set(LinReg((IDataSeries) dataSeries2, number_20)[0]);
			
			// DEBUG ONLY, IT"S NOT SUPPOSED TO BE HERE
			//BarsSinceSqueeze.Set(dataSeries1[0]);
			//Squeeze.Set(dataSeries2[0]);
			//Print(Time + " DonchianChannel Mean: " + DonchianChannel(Input, number_20).Mean[CurrentBar]);
			
			PMomentumUp.Set(0.0);
			PMomentumDown.Set(0.0);
			NMomentumUp.Set(0.0);
			NMomentumDown.Set(0.0);
			
			if (dataSeries1[0] > 0.0)
			{
				if (dataSeries1[0] > dataSeries1[1])
				{
					PMomentumUp.Set(dataSeries1[0]);
				}
				else
				{
					PMomentumDown.Set(dataSeries1[0]);
				}
			}
			else if (dataSeries1[0] > dataSeries1[1])
			{
				NMomentumUp.Set(dataSeries1[0]);
			}
			else
			{
				NMomentumDown.Set(dataSeries1[0]);
			}
		}
      
		processLongAlert(SoundHigh, 0, squeezeHigh, "squeezeHigh");
		processLongAlert(SoundLevel1, 1, squeezeHigh + squeezeRange * (double) extendedLevel1 / 100.0, "extendedLevel1");
		processLongAlert(SoundLevel2, 2, squeezeHigh + squeezeRange * (double) extendedLevel2 / 100.0, "extendedLevel2");
		processLongAlert(SoundLevel3, 3, squeezeHigh + squeezeRange * (double) extendedLevel3 / 100.0, "extendedLevel3");
		processShortAlert(SoundLow, 4, squeezeLow, "squeezeLow");
		processShortAlert(SoundLevel1, 5, squeezeLow - squeezeRange * (double) extendedLevel1 / 100.0, "extendedLevel1");
		processShortAlert(SoundLevel2, 6, squeezeLow - squeezeRange * (double) extendedLevel2 / 100.0, "extendedLevel2");
		processShortAlert(SoundLevel3, 7, squeezeLow - squeezeRange * (double) extendedLevel3 / 100.0, "extendedLevel3");
			
		CurrentValue.Set(dataSeries1[0]);
			
		if (tempCurrentBar2 != CurrentBar)
		{
			tempCurrentBar2 = CurrentBar;
			if (SqueezeOn[0] == 0.0)
			{
				++barsSinceSqueeze;
			}
			else
				barsSinceSqueeze = 0;
			
			if (Squeeze[0] == 0.0)
			{
				++squeezeLength;
			}
			else
			{
				squeezeLength = 0;
			}
		}

		if (barsSinceSqueeze > barsLookback)
		{
			BarsSinceSqueeze.Set(0.0);
		}
		else
			BarsSinceSqueeze.Set((double) barsSinceSqueeze);
		
		SqueezeLength.Set((double) squeezeLength);
		SqueezeHigh.Set(squeezeHigh);
		SqueezeLow.Set(squeezeLow);
	}

	
	
	#region UnusedCode
    private void toggleShowLines(object myObject, EventArgs eventArgs)
    {
//      ToolStripMenuItem toolStripMenuItem = myObject as ToolStripMenuItem;
//      if (toolStripMenuItem.Checked)
//      {
//        using (Dictionary<ILine, Color>.Enumerator enumerator = this.dictionary1.GetEnumerator())
//        {
//          while (enumerator.MoveNext())
//            enumerator.Current.Key.Pen.Color = Color.Transparent;
//        }
//        ((Control) this.get_ChartControl()).Refresh();
//        this.myHitPanel.Refresh();
//        toolStripMenuItem.Checked = false;
//        this.showLines = false;
//      }
//      else
//      {
//        if (toolStripMenuItem.Checked)
//          return;
//            using (Dictionary<ILine, Color>.Enumerator enumerator = this.dictionary1.GetEnumerator())
//            {
//              while (enumerator.MoveNext())
//              {
//                KeyValuePair<ILine, Color> current = enumerator.Current;
//                current.Key.Pen.Color = current.Value;
//              }
//            }
//            ((Control) this.get_ChartControl()).Refresh();
//            this.myHitPanel.Refresh();
//            toolStripMenuItem.Checked = true;
//            this.showLines = true;
//      }
    }

    private void toggleShowBoxes(object myObject, EventArgs eventArgs)
    {
//      ToolStripMenuItem toolStripMenuItem = myObject as ToolStripMenuItem;
//      if (toolStripMenuItem.Checked)
//      {
//            using (List<IRectangle>.Enumerator enumerator = this.list_IRectangle.GetEnumerator())
//            {
//              while (enumerator.MoveNext())
//              {
//                IRectangle current = enumerator.Current;
//                this.color1 = ((IShape) current).get_AreaColor();
//                this.color2 = ((IShape) current).Pen.Color;
//                ((IShape) current).set_AreaColor(Color.Transparent);
//                ((IShape) current).Pen.Color = Color.Transparent;
//              }
//            }
//            using (Dictionary<IText, Color>.Enumerator enumerator = this.dictionary2.GetEnumerator())
//            {
//              while (enumerator.MoveNext())
//                enumerator.Current.Key.set_TextColor(Color.Transparent);
//            }
//            ((Control) this.get_ChartControl()).Refresh();
//            this.myHitPanel.Refresh();
//            toolStripMenuItem.Checked = false;
//            this.extendedVisible = false;
//      }
//      else
//      {
//        if (toolStripMenuItem.Checked)
//          return;
//		
//        using (List<IRectangle>.Enumerator enumerator = this.list_IRectangle.GetEnumerator())
//        {
//          while (enumerator.MoveNext())
//          {
//            IRectangle current = enumerator.Current;
//            ((IShape) current).set_AreaColor(this.color1);
//            ((IShape) current).Pen.Color = this.color2;
//          }
//        }
//		
//        using (Dictionary<IText, Color>.Enumerator enumerator = this.dictionary2.GetEnumerator())
//        {
//          while (enumerator.MoveNext())
//          {
//            KeyValuePair<IText, Color> current = enumerator.Current;
//            current.Key.set_TextColor(current.Value);
//          }
//        }
//		
//        ((Control) this.get_ChartControl()).Refresh();
//        this.myHitPanel.Refresh();
//        toolStripMenuItem.Checked = true;
//        this.extendedVisible = true;
//      }
    }

    protected virtual void OnTermination()
    {
//      try
//      {
//          this.myHitPanel.Click -= new EventHandler(this.cd7409d0fdab3b2ac0617894a0a49bbf0);
//          this.get_ChartControl().get_ChartPanel().Controls.Remove((Control) this.myHitPanel);
//          ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
//          toolStrip.Items.Remove((ToolStripItem) this.toolStripSeparator);
//          toolStrip.Items.Remove((ToolStripItem) this.toolStripDropDownButton);
//      }
//      catch (Exception ex)
//      {
//      }
    }

    public void logoSetup()
    {
		/*
      string str1 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(313);
      string str2;
      if (this.shortLogo)
      {
            str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(332);
      }
      else
      {
        str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(313);
      }
      try
      {
        if (System.IO.File.Exists(str2))
        {
              System.IO.File.GetCreationTime(str2);
        }
        else
        {
          WebClient webClient = new WebClient();
          if (this.shortLogo)
          {
                webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(359), str2);
          }
          else
            webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(478), str2);
        }
        this.image = Image.FromFile(str2);
		
        this.myHitPanel = new rcVolatilityBreakout.hitPanel();
        this.myHitPanel.Height = this.image.Height;
        this.myHitPanel.Width = this.image.Width;
        this.myHitPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.myHitPanel.Top = this.get_ChartControl().get_ChartPanel().Bounds.Bottom - this.image.Height - 80;
        this.myHitPanel.Left = 1;
        this.myHitPanel.Click += new EventHandler(this.cd7409d0fdab3b2ac0617894a0a49bbf0);
        this.get_ChartControl().get_ChartPanel().Controls.Add((Control) this.myHitPanel);
      }
      catch (Exception ex)
      {
        this.Print(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(589));
      }
		*/
    }

    public virtual void Plot(Graphics g, Rectangle bounds, double min, double max)
    {
//      g.DrawImage(this.image, 2, bounds.Bottom - this.image.Height - 18, this.image.Width, this.image.Height);
    }

    private void cd7409d0fdab3b2ac0617894a0a49bbf0(object myObject, EventArgs eventArgs)
    {
//      Process.Start(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(622), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(647));
    }

    private void sendEmail(string message)
    {
//      if (this.email.Length <= 0)
//        return;
//	
//      this.SendMail("from rcVolatilityBreakout Indicator", this.email, this.get_Name() + ", ", message + (object) ", " + (string) (object) DateTime.Now + ", " + this.get_Instrument().get_MasterInstrument().get_Name() + ", " + (string) (object) Input[0]);
    }

    private void processShortAlert(string alertWaveFile, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string message)
    {
//      if (!(alertWaveFile != DISABLED))
//        return;
//      if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar)
//      {
//          if (this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.tempCurrentBar)
//            return;
//          if (Close[1] <= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] > ca2bb1244f73ff9a012a77d9ca61f445d)
//            return;
//          if (CurrentBar < this.tempCurrentBar || CurrentBar >= this.tempCurrentBar + this.extendedLineLength)
//            return;
//          if (!this.multiAlert)
//          {
//                this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.tempCurrentBar;
//          }
//          this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
//          this.sendEmail(message);
//          this.PlaySound(Core.get_InstallDir() +"sounds" + alertWaveFile);
//          return;
//      }
    }
	
    private void processLongAlert(string alertWaveFile, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string message)
    {
//      if (!(alertWaveFile != DISABLED))
//        return;
//	
//      if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar || this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.tempCurrentBar)
//          if (Close[1] >= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] < ca2bb1244f73ff9a012a77d9ca61f445d || CurrentBar < this.tempCurrentBar)
//            return;
//      if (CurrentBar >= this.tempCurrentBar + this.extendedLineLength)
//        return;
//      if (!this.multiAlert)
//      {
//            this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.tempCurrentBar;
//      }
//      this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
//      this.sendEmail(message);
//      this.PlaySound(Core.get_InstallDir() + "sounds" + alertWaveFile);
//      return;
    }


    private string convertArgbToColor(Color color)
    {
      return color.ToArgb().ToString();
    }

    private Color convertColorFromArgb(string rgb)
    {
      return Color.FromArgb(Convert.ToInt32(rgb));
    }
	#endregion

	#region SupportingClasses
    public class hitPanel : Panel
    {
      protected override CreateParams CreateParams
      {
        get
        {
          CreateParams createParams = base.CreateParams;
          createParams.ExStyle |= 32;
          return createParams;
        }
      }

      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
      }
    }

	// used for the background color of the box, but I don't think you have to do it this way
    public class XmlColor
    {
      private Color cf961a6779df9914187bdd86c52824c74 = Color.Black;

      [XmlAttribute]
      public string Web
      {
        get
        {
          return ColorTranslator.ToHtml(this.cf961a6779df9914187bdd86c52824c74);
        }
        set
        {
          try
          {
            if ((int) this.Alpha == (int) byte.MaxValue)
            {
                  this.cf961a6779df9914187bdd86c52824c74 = ColorTranslator.FromHtml(value);
            }
            else
              this.cf961a6779df9914187bdd86c52824c74 = Color.FromArgb((int) this.Alpha, ColorTranslator.FromHtml(value));
          }
          catch (Exception ex)
          {
            this.cf961a6779df9914187bdd86c52824c74 = Color.Black;
          }
        }
      }

      [XmlAttribute]
      public byte Alpha
      {
        get { return this.cf961a6779df9914187bdd86c52824c74.A; }
        set
        {
          if ((int) value == (int) this.cf961a6779df9914187bdd86c52824c74.A)
            return;
          this.cf961a6779df9914187bdd86c52824c74 = Color.FromArgb((int) value, this.cf961a6779df9914187bdd86c52824c74);
        }
      }

      public XmlColor()
      {
      }

      public XmlColor(Color c)
      {
        this.cf961a6779df9914187bdd86c52824c74 = c;
      }

      public static implicit operator Color(rcVolatilityBreakout.XmlColor x)
      {
        return x.ToColor();
      }

      public static implicit operator rcVolatilityBreakout.XmlColor(Color c)
      {
        return new rcVolatilityBreakout.XmlColor(c);
      }

      public Color ToColor()
      {
        return this.cf961a6779df9914187bdd86c52824c74;
      }

      public void FromColor(Color c)
      {
        this.cf961a6779df9914187bdd86c52824c74 = c;
      }

      public bool ShouldSerializeAlpha()
      {
        return (int) this.Alpha < (int) byte.MaxValue;
      }
    }

	// class used to load wave files
    public class WaveConverter : TypeConverter
    {
      private static string[] stringArray;

      static WaveConverter()
      {
      }

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

		// returns a list of alert wav files
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			string[] array;
			if (stringArray == null)
			{
				string[] files = Directory.GetFiles(Core.InstallDir + "sounds", "*.wav"); //SearchOption.TopDirectoryOnly);
				StringCollection stringCollection = new StringCollection();
				for (int index = 0; index < files.Length; ++index)
					stringCollection.Add(Path.GetFileName(files[index]));
				array = new string[stringCollection.Count + 1];
				stringCollection.CopyTo(array, 1);
				array[0] = DISABLED;
				stringArray = array;
			}
			else
			array = stringArray;
			return new TypeConverter.StandardValuesCollection((ICollection) array);
		}
	}

    private class cb7fd7594b63cf924b3e645a1b96661fe : UITypeEditor
    {
      public override bool GetPaintValueSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override void PaintValue(PaintValueEventArgs e)
      {
        if (e.Value == null)
          return;
		
        SolidBrush solidBrush = new SolidBrush(((Plot) e.Value).Pen.Color);
        e.Graphics.FillRectangle((Brush) solidBrush, e.Bounds);
        solidBrush.Dispose();
      }
	}

	// for possible positions for the height of the squeeze
    public class positionModeConverter : TypeConverter
    {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        return new TypeConverter.StandardValuesCollection((ICollection) new string[4] { LEFT, RIGHT, BOTH, DISABLED });
      }
    }

	// used for font size 
    public class sizeConverter : TypeConverter
    {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        return new TypeConverter.StandardValuesCollection((ICollection) new string[6] { "5","6","7","8","9","10" });
      }
    }
	#endregion
	
	#region Properties
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Category("Visual")]
    [Description("")]
    public Plot BoxOutline
    {
      get { return this.boxOutline; }
      set { this.boxOutline = value; }
    }

    [GridCategory("\tNinjacators")]
    [Description("")]
    public bool ShortLogo
    {
      get { return this.shortLogo; }
      set { this.shortLogo = value; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries Squeeze
    {
      get { return Values[0]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries SqueezeOn
    {
      get { return Values[1]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries PMomentumUp
    {
      get { return Values[2]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries PMomentumDown
    {
      get { return Values[3]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries NMomentumUp
    {
      get { return Values[4]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries NMomentumDown
    {
      get { return Values[5]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries CurrentValue
    {
      get { return Values[6]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries BarsSinceSqueeze
    {
      get { return Values[7]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeLength
    {
      get { return Values[8]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeHigh
    {
      get { return Values[9]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeLow
    {
      get { return Values[10]; }
    }

    [Description("Volatility Strength")]
    [GridCategory("Consolidation box")]
    public int Length
    {
      get { return this.length; }
      set { this.length = Math.Max(1, value); }
    }

    [Description("")]
    [Category("Alert")]
    public string Email
    {
      get { return this.email; }
      set { this.email = value; }
    }

    [Description("")]
    [Category("Global")]
    public int BarsLookback
    {
      get { return this.barsLookback; }
      set { this.barsLookback = value; }
    }

    [Category("Global")]
    [Description("ShowBoxes")]
    public bool ExtendedVisible
    {
      get { return this.extendedVisible; }
      set { this.extendedVisible = value; }
    }

    [Description("ShowLines")]
    [Category("Global")]
    public bool ShowLines
    {
      get { return this.showLines; }
      set { this.showLines = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedHigh
    {
      get { return this.extendedHigh; }
      set { this.extendedHigh = value; }
    }

    [Category("Settings")]
    public bool ExtendedHighVisible
    {
      get { return this.extendedHighVisible; }
      set { this.extendedHighVisible = value; }
    }

    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Category("Visual")]
    [Description("")]
    public Plot ExtendedLow
    {
      get { return this.extendedLow; }
      set { this.extendedLow = value; }
    }

    [Category("Settings")]
    public bool ExtendedLowVisible
    {
      get { return this.extendedLowVisible; }
      set { this.extendedLowVisible = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedMiddle
    {
      get { return this.extendedMiddle; }
      set { this.extendedMiddle = value; }
    }

    [Category("Settings")]
    public bool ExtendedMiddleVisible
    {
      get { return this.extendedMiddleVisible; }
      set { this.extendedMiddleVisible = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedMiddleLineLength
    {
      get { return this.extendedMiddleLineLength; }
      set { this.extendedMiddleLineLength = value; }
    }

    [Description("")]
    [Category("Settings")]
    public int ExtendedLineLength
    {
      get { return this.extendedLineLength; }
      set { this.extendedLineLength = value; }
    }

    [Category("Visual")]
    [Description("")]
    [XmlElement(Type = typeof (rcVolatilityBreakout.XmlColor))]
    public Color BoxAreaColor
    {
      get { return this.boxAreaColor; }
      set { this.boxAreaColor = value; }
    }

    [Description("")]
    [Category("Visual")]
    public int BoxAreaOpacity
    {
      get { return this.boxAreaOpacity; }
      set { this.boxAreaOpacity = value; }
    }

    [Category("Visual")]
    [Description("TickHeightVisible")]
    public bool TickHeightVisible
    {
      get { return this.tickHeightVisible; }
      set { this.tickHeightVisible = value; }
    }

    [Category("Visual")]
    [TypeConverter(typeof (rcVolatilityBreakout.positionModeConverter))]
    [Description("TickHeightPosition")]
    public string TickHeightPosition
    {
      get { return this.tickHeightPosition; }
      set { this.tickHeightPosition = value; }
    }

    [Category("Visual")]
    [XmlElement(Type = typeof (rcVolatilityBreakout.XmlColor))]
    [Description("")]
    public Color TickHeightColor
    {
      get { return this.tickHeightColor; }
      set { this.tickHeightColor = value; }
    }

    [Description("FontSize")]
    [Category("Settings")]
    [TypeConverter(typeof (rcVolatilityBreakout.sizeConverter))]
    public string FontSize
    {
      get { return this.fontSize; }
      set { this.fontSize = value; }
    }

    [Category("Settings")]
    public bool ExtendedLevel1Visible
    {
      get { return this.extendedLevel1Visible; }
      set { this.extendedLevel1Visible = value; }
    }

    [Category("Settings")]
    public bool ExtendedLevel2Visible
    {
      get { return this.extendedLevel2Visible; }
      set { this.extendedLevel2Visible = value; }
    }

    [Category("Settings")]
    public bool ExtendedLevel3Visible
    {
      get { return this.extendedLevel3Visible; }
      set { this.extendedLevel3Visible = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedLevel1
    {
      get { return this.extendedLevel1; }
      set { this.extendedLevel1 = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedLevel2
    {
      get { return this.extendedLevel2; }
      set { this.extendedLevel2 = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedLevel3
    {
      get { return this.extendedLevel3; }
      set { this.extendedLevel3 = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedLineLevel1
    {
      get { return this.extendedLineLevel1; }
      set { this.extendedLineLevel1 = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedLineLevel2
    {
      get { return this.extendedLineLevel2; }
      set { this.extendedLineLevel2 = value; }
    }

    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    [Category("Visual")]
    public Plot ExtendedLineLevel3
    {
      get { return this.extendedLineLevel3; }
      set { this.extendedLineLevel3 = value; }
    }

    [Category("Alert")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Description("")]
    public string SoundHigh
    {
      get { return this.soundHigh; }
      set { this.soundHigh = value; }
    }

    [Description("")]
    [Category("Alert")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    public string SoundLow
    {
      get { return this.soundLow; }
      set { this.soundLow = value; }
    }

    [Description("")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Category("Alert")]
    public string SoundLevel1
    {
      get { return this.soundLevel1; }
      set { this.soundLevel1 = value; }
    }

    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Category("Alert")]
    [Description("")]
    public string SoundLevel2
    {
      get { return this.soundLevel2; }
      set { this.soundLevel2 = value; }
    }

    [Description("")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Category("Alert")]
    public string SoundLevel3
    {
      get { return this.soundLevel3; }
      set { this.soundLevel3 = value; }
    }

    [Description("")]
    [Category("Alert")]
    public bool MultiAlert
    {
      get { return this.multiAlert; }
      set { this.multiAlert = value; }
    }
	#endregion
  }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private rcVolatilityBreakout[] cachercVolatilityBreakout = null;

        private static rcVolatilityBreakout checkrcVolatilityBreakout = new rcVolatilityBreakout();

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public rcVolatilityBreakout rcVolatilityBreakout(int length, bool shortLogo)
        {
            return rcVolatilityBreakout(Input, length, shortLogo);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length, bool shortLogo)
        {
            if (cachercVolatilityBreakout != null)
                for (int idx = 0; idx < cachercVolatilityBreakout.Length; idx++)
                    if (cachercVolatilityBreakout[idx].Length == length && cachercVolatilityBreakout[idx].ShortLogo == shortLogo && cachercVolatilityBreakout[idx].EqualsInput(input))
                        return cachercVolatilityBreakout[idx];

            lock (checkrcVolatilityBreakout)
            {
                checkrcVolatilityBreakout.Length = length;
                length = checkrcVolatilityBreakout.Length;
                checkrcVolatilityBreakout.ShortLogo = shortLogo;
                shortLogo = checkrcVolatilityBreakout.ShortLogo;

                if (cachercVolatilityBreakout != null)
                    for (int idx = 0; idx < cachercVolatilityBreakout.Length; idx++)
                        if (cachercVolatilityBreakout[idx].Length == length && cachercVolatilityBreakout[idx].ShortLogo == shortLogo && cachercVolatilityBreakout[idx].EqualsInput(input))
                            return cachercVolatilityBreakout[idx];

                rcVolatilityBreakout indicator = new rcVolatilityBreakout();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                indicator.ShortLogo = shortLogo;
                Indicators.Add(indicator);
                indicator.SetUp();

                rcVolatilityBreakout[] tmp = new rcVolatilityBreakout[cachercVolatilityBreakout == null ? 1 : cachercVolatilityBreakout.Length + 1];
                if (cachercVolatilityBreakout != null)
                    cachercVolatilityBreakout.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachercVolatilityBreakout = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(int length, bool shortLogo)
        {
            return _indicator.rcVolatilityBreakout(Input, length, shortLogo);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length, bool shortLogo)
        {
            return _indicator.rcVolatilityBreakout(input, length, shortLogo);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(int length, bool shortLogo)
        {
            return _indicator.rcVolatilityBreakout(Input, length, shortLogo);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length, bool shortLogo)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.rcVolatilityBreakout(input, length, shortLogo);
        }
    }
}
#endregion
