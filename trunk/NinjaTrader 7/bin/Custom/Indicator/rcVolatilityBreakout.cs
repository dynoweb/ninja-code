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
    private int length = 20;
    private double number_2_0 = 2.0;
    private double number_1_5 = 1.5;
    private int number_20 = 20;
    private Color red = Color.Red;	// 1
    private Color gray = Color.Gray; // 1
    private Color lime = Color.Lime; // 1
    private Color darkGreen = Color.DarkGreen; // 1
    private Color red2 = Color.Red; // 1
    private Color maroon = Color.Maroon; // 1
    private Color darkGreen2 = Color.DarkGreen; // 1
    private Color darkRed = Color.DarkRed; // 1
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
    private int c84334302133d3436d40fc5c5c9cd118a;
    private int c703c1b4d798d0f3074ba94d947ccca39;
    private ToolStripDropDownButton toolStripDropDownButton;
    private int barsSinceSqueeze;
    private Font font;
    private Color color1;
    private Color color2;
    private Image image;
    private rcVolatilityBreakout.hitPanel myHitPanel;
    private int breakoutRecNumb;
    private int aBarNumber;
    private int cdcf0ce7396813d4bd126a0b9fa8fea53;
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
    private int c13d059c56853219888b6d45fccc6b9e1;
    private bool shortLogo;
    private bool extendedMiddleVisible;
    private bool multiAlert;
	private static string LEFT = "Left";
	private static string RIGHT = "Right";
	private static string BOTH = "Both";
	private static string DISABLED = "Disabled";

	protected override void Initialize()      
	{
      Add(new Plot(Color.Transparent, (PlotStyle) 3, "Squeeze"));  // 0 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(839)
      Add(new Plot(Color.Transparent, (PlotStyle) 3, "SqueezeOn"));  // 1 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(854)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "PMomentumUp"));  // 2 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(873)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "PMomentumDown"));  // 3 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(896)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "NMomentumUp"));  // 4 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(923)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "NMomentumDown"));  // 5 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(946)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "CurrentValue"));  // 6 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(973)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "BarsSinceSqueeze"));  // 7 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(998)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeLength"));  // 8 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1031)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeHigh"));  // 9 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1058)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeLow"));  // 10 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1081)
      dataSeries1 = new DataSeries((IndicatorBase) this);
      dataSeries2 = new DataSeries((IndicatorBase) this);
      CalculateOnBarClose = false;
      Overlay = true;
      PriceTypeSupported = false;
      PlotsConfigurable = false;
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
            if (num3 <= 1.0)
            {
                  if (this.Squeeze[1] != 0.0)
                  {
                        this.aBarNumber = CurrentBar;
                        this.squeezeHigh = High[0];
                        this.squeezeLow = Low[0];
                        ++this.breakoutRecNumb;
                  }
                  if (this.Squeeze[1] == 0.0)
                  {
                        if (High[0] > this.squeezeHigh)
                        {
                              this.squeezeHigh = High[0];
                        }
                        if (Low[0] < this.squeezeLow)
                        {
                              this.squeezeLow = Low[0];
                        }
                        this.cdcf0ce7396813d4bd126a0b9fa8fea53 = CurrentBar;
                        if (this.aBarNumber != this.cdcf0ce7396813d4bd126a0b9fa8fea53)
                        {
                          this.squeezeMidPoint = (this.squeezeHigh + this.squeezeLow) / 2.0;
                          this.squeezeRange = this.squeezeHigh - this.squeezeLow;
                          this.iRectangle = this.DrawRectangle(CurrentBar + "rectangle" + this.breakoutRecNumb, true, CurrentBar - this.aBarNumber, this.squeezeHigh, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeLow, this.boxOutline.Pen.Color, this.boxAreaColor, this.boxAreaOpacity);
                          ((IShape) this.iRectangle).Pen.DashStyle = this.boxOutline.Pen.DashStyle;
                          ((IShape) this.iRectangle).Pen.Width = this.boxOutline.Pen.Width;
                          if (this.extendedHighVisible)
                          {
                                this.iLine = this.DrawLine(CurrentBar + "extendedHigh" + this.breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeHigh, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeHigh, this.extendedHigh.Pen.Color, this.extendedHigh.Pen.DashStyle, (int) this.extendedHigh.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                  this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                if (!this.showLines)
                                {
                                  this.iLine.Pen.Color = Color.Transparent;
                                }
                          }
                          if (this.extendedLowVisible)
                          {
                                this.iLine = this.DrawLine(CurrentBar + "extendedLow" + this.breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeLow, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeLow, this.extendedLow.Pen.Color, this.extendedLow.Pen.DashStyle, (int) this.extendedLow.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                          }
                          if (this.extendedMiddleVisible)
                          {
                                this.iLine = this.DrawLine(CurrentBar + "extendedMiddle" + this.breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeMidPoint, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedMiddleLineLength, this.squeezeMidPoint, this.extendedMiddle.Pen.Color, this.extendedMiddle.Pen.DashStyle, (int) this.extendedMiddle.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                          }
                          if (this.extendedLevel1Visible)
                          {
                                double num4 = this.squeezeRange * (double) this.extendedLevel1 / 100.0;
                                this.iLine = this.DrawLine(CurrentBar + "extendedLevel1High" + this.breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeHigh + num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeHigh + num4, this.extendedLineLevel1.Pen.Color, this.extendedLineLevel1.Pen.DashStyle, (int) this.extendedLineLevel1.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
								{
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                                this.iLine = this.DrawLine(CurrentBar + "extendedLevel1Low" + this.breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeLow - num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeLow - num4, this.extendedLineLevel1.Pen.Color, this.extendedLineLevel1.Pen.DashStyle, (int) this.extendedLineLevel1.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }                            
                          }
						
                          if (this.extendedLevel2Visible)
                          {
                                double num4 = this.squeezeRange * (double) this.extendedLevel2 / 100.0;
                                this.iLine = this.DrawLine(CurrentBar + "extendedLevel2High" + this.breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeHigh + num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeHigh + num4, this.extendedLineLevel2.Pen.Color, this.extendedLineLevel2.Pen.DashStyle, (int) this.extendedLineLevel2.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                                this.iLine = this.DrawLine(CurrentBar + "extendedLevel2Low" + this.breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeLow - num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeLow - num4, this.extendedLineLevel2.Pen.Color, this.extendedLineLevel2.Pen.DashStyle, (int) this.extendedLineLevel2.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }                            
                          }
						
                          if (this.extendedLevel3Visible)
                          {
                                double num4 = this.squeezeRange * (double) this.extendedLevel3 / 100.0;
                                Plot plot = this.extendedLineLevel3;
                                this.iLine = this.DrawLine(CurrentBar + "extendedLevel3High" + breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeHigh + num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeHigh + num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                                this.iLine = this.DrawLine(CurrentBar + "extendedLevel3Low" + breakoutRecNumb, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeLow - num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.squeezeLow - num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                          }
                          
                          if (this.tickHeightVisible)
                          {
                                int num4 = Convert.ToInt32((this.squeezeHigh - this.squeezeLow) / TickSize);
                                if (this.tickHeightPosition == RIGHT || this.tickHeightPosition == BOTH)
                                {
                                  this.iText = this.DrawText(CurrentBar + "LText" + breakoutRecNumb, true, num4.ToString(), CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.squeezeMidPoint, 0, this.tickHeightColor, this.font, StringAlignment.Near, Color.Transparent, Color.Transparent, 1);
                                  if (!this.dictionary2.ContainsKey(this.iText))
                                  {
                                        this.dictionary2.Add(this.iText, this.iText.TextColor);
                                  }
                                  if (!this.extendedVisible)
                                  {
                                        this.iText.TextColor = Color.Transparent;
                                  }
                                }
                                if (!(this.tickHeightPosition == LEFT))
                                {
                                      if (this.tickHeightPosition == BOTH)
                                      {
                                      }
                                }
                                this.iText = this.DrawText(CurrentBar + "RText" + breakoutRecNumb, true, num4.ToString(), CurrentBar - this.aBarNumber, this.squeezeMidPoint, 0, this.tickHeightColor, this.font, StringAlignment.Far, Color.Transparent, Color.Transparent, 1);
								
                                if (!dictionary2.ContainsKey(iText))
                                {
                                      dictionary2.Add(iText, iText.TextColor);
								}
                                if (!extendedVisible)
                                {
                                	iText.TextColor = Color.Transparent;
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
                  }
                  this.Squeeze.Set(0.0);
                  this.SqueezeOn.Reset();
				
              
            }
            else
            {
              this.Squeeze.Reset();
              this.SqueezeOn.Set(0.0);
            }
            this.dataSeries2.Set(Input[0] - (this.DonchianChannel(Input, this.number_20).Mean.Get(CurrentBar) + this.EMA(Input, this.number_20)[0]) / 2.0);
            this.dataSeries1.Set(this.LinReg((IDataSeries) this.dataSeries2, this.number_20)[0]);
            this.PMomentumUp.Set(0.0);
            this.PMomentumDown.Set(0.0);
            this.NMomentumUp.Set(0.0);
            this.NMomentumDown.Set(0.0);
            if (this.dataSeries1[0] > 0.0)
            {
                  if (this.dataSeries1[0] > this.dataSeries1[1])
                  {
                        this.PMomentumUp.Set(this.dataSeries1[0]);
                  }
                  else
                  {
                    this.PMomentumDown.Set(dataSeries1[0]);
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
        
      
      this.processAlert2(this.SoundHigh, 0, this.squeezeHigh, "squeezeHigh");
      this.processAlert2(this.SoundLevel1, 1, this.squeezeHigh + this.squeezeRange * (double) this.extendedLevel1 / 100.0, "extendedLevel1");
      this.processAlert2(this.SoundLevel2, 2, this.squeezeHigh + this.squeezeRange * (double) this.extendedLevel2 / 100.0, "extendedLevel2");
      this.processAlert2(this.SoundLevel3, 3, this.squeezeHigh + this.squeezeRange * (double) this.extendedLevel3 / 100.0, "extendedLevel3");
      this.processAlerts(this.SoundLow, 4, this.squeezeLow, "squeezeLow");
      this.processAlerts(this.SoundLevel1, 5, this.squeezeLow - this.squeezeRange * (double) this.extendedLevel1 / 100.0, "extendedLevel1");
      this.processAlerts(this.SoundLevel2, 6, this.squeezeLow - this.squeezeRange * (double) this.extendedLevel2 / 100.0, "extendedLevel2");
      this.processAlerts(this.SoundLevel3, 7, this.squeezeLow - this.squeezeRange * (double) this.extendedLevel3 / 100.0, "extendedLevel3");
			
      CurrentValue.Set(dataSeries1[0]);
      if (c13d059c56853219888b6d45fccc6b9e1 != CurrentBar)
      {
            c13d059c56853219888b6d45fccc6b9e1 = CurrentBar;
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

	
	
    private void processAlert2(string alertWaveFile, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string message)
    {
//      if (!(alertWaveFile != DISABLED))
//        return;
//	
//      if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar || this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.cdcf0ce7396813d4bd126a0b9fa8fea53)
//          if (Close[1] >= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] < ca2bb1244f73ff9a012a77d9ca61f445d || CurrentBar < this.cdcf0ce7396813d4bd126a0b9fa8fea53)
//            return;
//      if (CurrentBar >= this.cdcf0ce7396813d4bd126a0b9fa8fea53 + this.extendedLineLength)
//        return;
//      if (!this.multiAlert)
//      {
//            this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.cdcf0ce7396813d4bd126a0b9fa8fea53;
//      }
//      this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
//      this.sendEmail(message);
//      this.PlaySound(Core.get_InstallDir() + "sounds" + alertWaveFile);
//      return;
    }

	
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

    private void processAlerts(string alertWaveFile, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string message)
    {
//      if (!(alertWaveFile != DISABLED))
//        return;
//      if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar)
//      {
//          if (this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.cdcf0ce7396813d4bd126a0b9fa8fea53)
//            return;
//          if (Close[1] <= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] > ca2bb1244f73ff9a012a77d9ca61f445d)
//            return;
//          if (CurrentBar < this.cdcf0ce7396813d4bd126a0b9fa8fea53 || CurrentBar >= this.cdcf0ce7396813d4bd126a0b9fa8fea53 + this.extendedLineLength)
//            return;
//          if (!this.multiAlert)
//          {
//                this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.cdcf0ce7396813d4bd126a0b9fa8fea53;
//          }
//          this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
//          this.sendEmail(message);
//          this.PlaySound(Core.get_InstallDir() +"sounds" + alertWaveFile);
//          return;
//      }
    }

    private string convertArgbToColor(Color color)
    {
      return color.ToArgb().ToString();
    }

    private Color convertColorFromArgb(string rgb)
    {
      return Color.FromArgb(Convert.ToInt32(rgb));
    }

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
