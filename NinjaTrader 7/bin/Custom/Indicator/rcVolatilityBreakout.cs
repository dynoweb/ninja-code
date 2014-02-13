// Type: NinjaTrader.Indicator.rcVolatilityBreakout
// Assembly: rcVolatilityBreakout, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 44D1D67D-B887-419E-93CB-54BC561A67EC
// Assembly location: C:\Users\rcromer\Documents\NinjaTrader 7\bin\Custom\rcVolatilityBreakout.dll

using A;
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
    private Plot boxOutline = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1974));
    private string email = "";
    private int barsLookback = 30;
    private bool extendedVisible = true;
    private bool showLines = true;
    [XmlIgnore]
    private Plot extendedHigh = new Plot(new Pen(Color.Green, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1995));
    private bool extendedHighVisible = true;
    [XmlIgnore]
    private Plot extendedLow = new Plot(new Pen(Color.Red, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2010));
    private bool extendedLowVisible = true;
    [XmlIgnore]
    private Plot extendedMiddle = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2023));
    private int extendedMiddleLineLength = 6;
    private int extendedLineLength = 20;
    private Color boxAreaColor = Color.Orange;
    private int boxAreaOpacity = 1;
    private bool tickHeightVisible = true;
    private string tickHeightPosition = LEFT;
    private Color tickHeightColor = Color.Black;
    private string fontSize = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2042);
    private bool extendedLevel1Visible = true;
    private bool extendedLevel2Visible = true;
    private bool extendedLevel3Visible = true;
    private int extendedLevel1 = 50;
    private int extendedLevel2 = 100;
    private int extendedLevel3 = 200;
    [XmlIgnore]
    private Plot extendedLineLevel1 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2045));
    [XmlIgnore]
    private Plot extendedLineLevel2 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2082));
    [XmlIgnore]
    private Plot extendedLineLevel3 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2119));
    private string soundHigh = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string soundLow = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string soundLevel1 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string soundLevel2 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string soundLevel3 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private DataSeries dataSeries1;
    private DataSeries dataSeries2;
    private double ce532046b6b6b789cfb4e83dd4ded26e8;
    private double c4401083d3066e091774c61d40eed01ad;
    private double c3989dca8716e205f1ce35948cff57ac9;
    private double ce0ad2a035fced4d4b79ef3da6ec927ae;
    private double c5e0958df8f76b18b2ab9293382ae3d14;
    private double cb5ff3849b5f0c4d175ca3bba67c0b947;
    private double c37c8eb5e2c1b5ca77a4987ec1f116d56;
    private double cd6bc814fcea78d4e2c478b3e42c0a050;
    private double ca5ad4acedee8387a0d09b08e94ffd438;
    private double c95e8e518faa568cb6142f92d98772c66;
    private double cb638b9cc732de8bf65b6dbeeb52281d1;
    private double c98884df07a511384f7ab4afade318e44;
    private int c84334302133d3436d40fc5c5c9cd118a;
    private int c703c1b4d798d0f3074ba94d947ccca39;
    private ToolStripDropDownButton toolStripDropDownButton;
    private int ced563a5e9c3c71140c2e62f63afc998a;
    private Font font;
    private Color color1;
    private Color color2;
    private Image image;
    private rcVolatilityBreakout.hitPanel myHitPanel;
    private int c70fc78f80bfde9358a1dc14ea234aa72;
    private int aBarNumber;
    private int cdcf0ce7396813d4bd126a0b9fa8fea53;
    private double cfb105a9d24e167ceec6191571a1a8e43;
    private double c739d1f8b1109fe6f2de5bc1ec3c7d232;
    private ToolStripSeparator toolStripSeparator;
    private ToolStripButton toolStripButton;
    private IRectangle iRectangle;
    private ILine iLine;
    private IText iText;
    private int c6c9ad6a793905e213eacafa1be1fb516;
    private double c133da4068b26ce316e9f7ed309997e74;
    private double c950bfc49088216da2d12e2c56be386df;
    private int c13d059c56853219888b6d45fccc6b9e1;
    private bool shortLogo;
    private bool extendedMiddleVisible;
    private bool multiAlert;
	private string LEFT = "Left";
	private string RIGHT = "Right";
	private string BOTH = "Both";
	private string DISABLED = "Disabled";

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
//      if (this.get_ChartControl() == null)
//      {
//label_1:
//        switch (1)
//        {
//          case 0:
//            goto label_1;
//          default:
//            if (1 != 0)
//              break;
//            // ISSUE: method reference
//            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.OnStartUp);
//            break;
//        }
//      }
//      else
//      {
        //ToolStrip toolStrip = ((Control) this.ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
        //this.toolStripSeparator = new ToolStripSeparator();
        //this.toolStripDropDownButton = new ToolStripDropDownButton(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629((int) byte.MaxValue));
        //this.toolStripDropDownButton.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(278);
        //ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(291));
        //toolStripMenuItem1.Checked = this.extendedVisible;
        //toolStripMenuItem1.Click += new EventHandler(this.ce3690bd0a1e9603192f7898e782b1f4f);
        //this.toolStripDropDownButton.DropDownItems.Add((ToolStripItem) toolStripMenuItem1);
        //ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(302));
        //toolStripMenuItem2.Checked = this.showLines;
        //toolStripMenuItem2.Click += new EventHandler(this.toggleShowLines);
        //this.toolStripDropDownButton.DropDownItems.Add((ToolStripItem) toolStripMenuItem2);
        //toolStrip.Items.Add((ToolStripItem) this.toolStripSeparator);
        //toolStrip.Items.Add((ToolStripItem) this.toolStripDropDownButton);
        //this.aString = ((object) this).GetType().Name;
        //this.font = new Font(((Control) this.get_ChartControl()).Font.FontFamily, (float) Convert.ToInt16(this.fontSize));
//      }
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
                        this.cfb105a9d24e167ceec6191571a1a8e43 = High[0];
                        this.c739d1f8b1109fe6f2de5bc1ec3c7d232 = Low[0];
                        ++this.c70fc78f80bfde9358a1dc14ea234aa72;
                  }
                  if (this.Squeeze[1] == 0.0)
                  {
                        if (High[0] > this.cfb105a9d24e167ceec6191571a1a8e43)
                        {
                              this.cfb105a9d24e167ceec6191571a1a8e43 = High[0];
                        }
                        if (Low[0] < this.c739d1f8b1109fe6f2de5bc1ec3c7d232)
                        {
                              this.c739d1f8b1109fe6f2de5bc1ec3c7d232 = Low[0];
                        }
                        this.cdcf0ce7396813d4bd126a0b9fa8fea53 = CurrentBar;
                        if (this.aBarNumber != this.cdcf0ce7396813d4bd126a0b9fa8fea53)
                        {
                          this.c133da4068b26ce316e9f7ed309997e74 = (this.cfb105a9d24e167ceec6191571a1a8e43 + this.c739d1f8b1109fe6f2de5bc1ec3c7d232) / 2.0;
                          this.c950bfc49088216da2d12e2c56be386df = this.cfb105a9d24e167ceec6191571a1a8e43 - this.c739d1f8b1109fe6f2de5bc1ec3c7d232;
                          this.iRectangle = this.DrawRectangle(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1102) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.aBarNumber, this.cfb105a9d24e167ceec6191571a1a8e43, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.boxOutline.Pen.Color, this.boxAreaColor, this.boxAreaOpacity);
                          ((IShape) this.iRectangle).Pen.DashStyle = this.boxOutline.Pen.DashStyle;
                          ((IShape) this.iRectangle).Pen.Width = this.boxOutline.Pen.Width;
                          if (this.extendedHighVisible)
                          {
                                this.iLine = this.DrawLine(CurrentBar + "292", true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43, this.extendedHigh.Pen.Color, this.extendedHigh.Pen.DashStyle, (int) this.extendedHigh.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                  this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                if (!this.showLines)
                                {
                                  this.iLine.Pen.Color = Color.Transparent;
                                }
                          }
                          if (this.extendedLowVisible)
                          {
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1114) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.extendedLow.Pen.Color, this.extendedLow.Pen.DashStyle, (int) this.extendedLow.Pen.Width);
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
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1119) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c133da4068b26ce316e9f7ed309997e74, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedMiddleLineLength, this.c133da4068b26ce316e9f7ed309997e74, this.extendedMiddle.Pen.Color, this.extendedMiddle.Pen.DashStyle, (int) this.extendedMiddle.Pen.Width);
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
                                double num4 = this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel1 / 100.0;
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1126) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.extendedLineLevel1.Pen.Color, this.extendedLineLevel1.Pen.DashStyle, (int) this.extendedLineLevel1.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
								{
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1135) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.extendedLineLevel1.Pen.Color, this.extendedLineLevel1.Pen.DashStyle, (int) this.extendedLineLevel1.Pen.Width);
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
                                double num4 = this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel2 / 100.0;
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1144) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.extendedLineLevel2.Pen.Color, this.extendedLineLevel2.Pen.DashStyle, (int) this.extendedLineLevel2.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1153) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.extendedLineLevel2.Pen.Color, this.extendedLineLevel2.Pen.DashStyle, (int) this.extendedLineLevel2.Pen.Width);
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
                                double num4 = this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel3 / 100.0;
                                Plot plot = this.extendedLineLevel3;
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1162) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
                                if (!this.dictionary1.ContainsKey(this.iLine))
                                {
                                      this.dictionary1.Add(this.iLine, this.iLine.Pen.Color);
                                }
                                if (!this.showLines)
                                {
                                      this.iLine.Pen.Color = Color.Transparent;
                                }
                                this.iLine = this.DrawLine(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1171) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, plot.Pen.Color, plot.Pen.DashStyle, (int) plot.Pen.Width);
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
                                int num4 = Convert.ToInt32((this.cfb105a9d24e167ceec6191571a1a8e43 - this.c739d1f8b1109fe6f2de5bc1ec3c7d232) / this.get_TickSize());
                                if (this.tickHeightPosition == cee5e96d25be00bb50a036ae3849574cc.RIGHT || this.tickHeightPosition == cee5e96d25be00bb50a036ae3849574cc.BOTH)
                                {
                                  this.iText = this.DrawText(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1200) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, num4.ToString(), CurrentBar - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c133da4068b26ce316e9f7ed309997e74, 0, this.tickHeightColor, this.font, StringAlignment.Near, Color.Transparent, Color.Transparent, 1);
                                  if (!this.dictionary2.ContainsKey(this.iText))
                                  {
                                        this.dictionary2.Add(this.iText, this.iText.get_TextColor());
                                  }
                                  if (!this.extendedVisible)
                                  {
                                        this.iText.set_TextColor(Color.Transparent);
                                  }
                                }
                                if (!(this.tickHeightPosition == LEFT))
                                {
                                      if (this.tickHeightPosition == cee5e96d25be00bb50a036ae3849574cc.BOTH)
                                      {
                                      }
                                }
                                this.iText = this.DrawText(this.aString + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1220) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, num4.ToString(), CurrentBar - this.aBarNumber, this.c133da4068b26ce316e9f7ed309997e74, 0, this.tickHeightColor, this.font, StringAlignment.Far, Color.Transparent, Color.Transparent, 1);
								
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
        
      
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundHigh, 0, this.cfb105a9d24e167ceec6191571a1a8e43, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1229));
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundLevel1, 1, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel1 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1308));
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundLevel2, 2, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel2 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1401));
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundLevel3, 3, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel3 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1494));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLow, 4, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1587));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLevel1, 5, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel1 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1664));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLevel2, 6, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel2 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1755));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLevel3, 7, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df * (double) this.extendedLevel3 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1846));
			
      this.CurrentValue.Set(this.dataSeries1[0]);
      if (this.c13d059c56853219888b6d45fccc6b9e1 != CurrentBar)
      {
            this.c13d059c56853219888b6d45fccc6b9e1 = CurrentBar;
            if (this.SqueezeOn[0] == 0.0)
            {
                  ++this.ced563a5e9c3c71140c2e62f63afc998a;
            }
            else
              this.ced563a5e9c3c71140c2e62f63afc998a = 0;
            if (this.Squeeze[0] == 0.0)
            {
                  ++this.c6c9ad6a793905e213eacafa1be1fb516;
            }
            else
            {
              this.c6c9ad6a793905e213eacafa1be1fb516 = 0;
            }
        }
      }
      if (this.ced563a5e9c3c71140c2e62f63afc998a > this.barsLookback)
      {
            this.BarsSinceSqueeze.Set(0.0);
      }
      else
        this.BarsSinceSqueeze.Set((double) this.ced563a5e9c3c71140c2e62f63afc998a);
      this.SqueezeLength.Set((double) this.c6c9ad6a793905e213eacafa1be1fb516);
      this.SqueezeHigh.Set(this.cfb105a9d24e167ceec6191571a1a8e43);
      this.SqueezeLow.Set(this.c739d1f8b1109fe6f2de5bc1ec3c7d232);
    }

	
	
    private void cc9a41bd2d71b489e8f4134a31abfa2e6(string cc411ee82ad01410449696e9d303287f0, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string c549929c7d4f914e2cdb9a099d7a7eb21)
    {
      if (!(cc411ee82ad01410449696e9d303287f0 != cee5e96d25be00bb50a036ae3849574cc.DISABLED))
        return;
	
      if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar || this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.cdcf0ce7396813d4bd126a0b9fa8fea53)
          if (Close[1] >= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] < ca2bb1244f73ff9a012a77d9ca61f445d || CurrentBar < this.cdcf0ce7396813d4bd126a0b9fa8fea53)
            return;
      if (CurrentBar >= this.cdcf0ce7396813d4bd126a0b9fa8fea53 + this.extendedLineLength)
        return;
      if (!this.multiAlert)
      {
            this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.cdcf0ce7396813d4bd126a0b9fa8fea53;
      }
      this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
      this.c013129a8956cc8e16ff7ec447af66499(c549929c7d4f914e2cdb9a099d7a7eb21);
      this.PlaySound(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1954) + cc411ee82ad01410449696e9d303287f0);
      return;
    }

	
    private void toggleShowLines(object myObject, EventArgs eventArgs)
    {
      ToolStripMenuItem toolStripMenuItem = myObject as ToolStripMenuItem;
      if (toolStripMenuItem.Checked)
      {
        using (Dictionary<ILine, Color>.Enumerator enumerator = this.dictionary1.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Key.Pen.Color = Color.Transparent;
        }
        ((Control) this.get_ChartControl()).Refresh();
        this.myHitPanel.Refresh();
        toolStripMenuItem.Checked = false;
        this.showLines = false;
      }
      else
      {
        if (toolStripMenuItem.Checked)
          return;
            using (Dictionary<ILine, Color>.Enumerator enumerator = this.dictionary1.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<ILine, Color> current = enumerator.Current;
                current.Key.Pen.Color = current.Value;
              }
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.myHitPanel.Refresh();
            toolStripMenuItem.Checked = true;
            this.showLines = true;
      }
    }

    private void ce3690bd0a1e9603192f7898e782b1f4f(object myObject, EventArgs eventArgs)
    {
      ToolStripMenuItem toolStripMenuItem = myObject as ToolStripMenuItem;
      if (toolStripMenuItem.Checked)
      {
            using (List<IRectangle>.Enumerator enumerator = this.list_IRectangle.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IRectangle current = enumerator.Current;
                this.color1 = ((IShape) current).get_AreaColor();
                this.color2 = ((IShape) current).Pen.Color;
                ((IShape) current).set_AreaColor(Color.Transparent);
                ((IShape) current).Pen.Color = Color.Transparent;
              }
            }
            using (Dictionary<IText, Color>.Enumerator enumerator = this.dictionary2.GetEnumerator())
            {
              while (enumerator.MoveNext())
                enumerator.Current.Key.set_TextColor(Color.Transparent);
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.myHitPanel.Refresh();
            toolStripMenuItem.Checked = false;
            this.extendedVisible = false;
      }
      else
      {
        if (toolStripMenuItem.Checked)
          return;
		
        using (List<IRectangle>.Enumerator enumerator = this.list_IRectangle.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            IRectangle current = enumerator.Current;
            ((IShape) current).set_AreaColor(this.color1);
            ((IShape) current).Pen.Color = this.color2;
          }
        }
		
        using (Dictionary<IText, Color>.Enumerator enumerator = this.dictionary2.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<IText, Color> current = enumerator.Current;
            current.Key.set_TextColor(current.Value);
          }
        }
		
        ((Control) this.get_ChartControl()).Refresh();
        this.myHitPanel.Refresh();
        toolStripMenuItem.Checked = true;
        this.extendedVisible = true;
      }
    }

    protected virtual void OnTermination()
    {
      try
      {
          this.myHitPanel.Click -= new EventHandler(this.cd7409d0fdab3b2ac0617894a0a49bbf0);
          this.get_ChartControl().get_ChartPanel().Controls.Remove((Control) this.myHitPanel);
          ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
          toolStrip.Items.Remove((ToolStripItem) this.toolStripSeparator);
          toolStrip.Items.Remove((ToolStripItem) this.toolStripDropDownButton);
      }
      catch (Exception ex)
      {
      }
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
      g.DrawImage(this.image, 2, bounds.Bottom - this.image.Height - 18, this.image.Width, this.image.Height);
    }

    private void cd7409d0fdab3b2ac0617894a0a49bbf0(object myObject, EventArgs eventArgs)
    {
      Process.Start(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(622), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(647));
    }

    private void c013129a8956cc8e16ff7ec447af66499(string iText)
    {
      if (this.email.Length <= 0)
        return;
label_1:
      switch (5)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
            // ISSUE: method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.c013129a8956cc8e16ff7ec447af66499);
          }
          this.SendMail(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(700), this.email, this.get_Name() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(747), iText + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(760) + (string) (object) DateTime.Now + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(795) + this.get_Instrument().get_MasterInstrument().get_Name() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(822) + (string) (object) Input[0]);
          break;
      }
    }

    private void c03c1af8a08fc173794566a08a468a918(string cc411ee82ad01410449696e9d303287f0, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string c549929c7d4f914e2cdb9a099d7a7eb21)
    {
      if (!(cc411ee82ad01410449696e9d303287f0 != cee5e96d25be00bb50a036ae3849574cc.DISABLED))
        return;
label_1:
      switch (6)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
            // ISSUE: method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.c03c1af8a08fc173794566a08a468a918);
          }
          if (this.intArray8_1[cefea6044484c3257df671c52a39f09b2] == CurrentBar)
            break;
label_5:
          switch (6)
          {
            case 0:
              goto label_5;
            default:
              if (this.intArray8_2[cefea6044484c3257df671c52a39f09b2] == this.cdcf0ce7396813d4bd126a0b9fa8fea53)
                return;
label_7:
              switch (2)
              {
                case 0:
                  goto label_7;
                default:
                  if (Close[1] <= ca2bb1244f73ff9a012a77d9ca61f445d || Close[0] > ca2bb1244f73ff9a012a77d9ca61f445d)
                    return;
label_9:
                  switch (2)
                  {
                    case 0:
                      goto label_9;
                    default:
                      if (CurrentBar < this.cdcf0ce7396813d4bd126a0b9fa8fea53 || CurrentBar >= this.cdcf0ce7396813d4bd126a0b9fa8fea53 + this.extendedLineLength)
                        return;
label_11:
                      switch (7)
                      {
                        case 0:
                          goto label_11;
                        default:
                          if (!this.multiAlert)
                          {
label_13:
                            switch (6)
                            {
                              case 0:
                                goto label_13;
                              default:
                                this.intArray8_2[cefea6044484c3257df671c52a39f09b2] = this.cdcf0ce7396813d4bd126a0b9fa8fea53;
                                break;
                            }
                          }
                          this.intArray8_1[cefea6044484c3257df671c52a39f09b2] = CurrentBar;
                          this.c013129a8956cc8e16ff7ec447af66499(c549929c7d4f914e2cdb9a099d7a7eb21);
                          this.PlaySound(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1954) + cc411ee82ad01410449696e9d303287f0);
                          return;
                      }
                  }
              }
          }
      }
    }

    private string c4152da8a89098d52e196178975c6ae4f(Color c4544fc12a8cffe1b31db6f02f2dd887e)
    {
      return c4544fc12a8cffe1b31db6f02f2dd887e.ToArgb().ToString();
    }

    private Color c8c61fd528f31c4e9976bfec210e23950(string c4544fc12a8cffe1b31db6f02f2dd887e)
    {
      return Color.FromArgb(Convert.ToInt32(c4544fc12a8cffe1b31db6f02f2dd887e));
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
label_1:
              switch (7)
              {
                case 0:
                  goto label_1;
                default:
                  if (1 == 0)
                  {
                    RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.XmlColor.set_Web);
                  }
                  this.cf961a6779df9914187bdd86c52824c74 = ColorTranslator.FromHtml(value);
                  break;
              }
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
        get
        {
          return this.cf961a6779df9914187bdd86c52824c74.A;
        }
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

    public class WaveConverter : TypeConverter
    {
      private static string[] c881dc1ea8e7199b694d684a64842fb7d;

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
        if (rcVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d == null)
        {
              string[] files = Directory.GetFiles(Core.get_InstallDir() + "sounds", "*.wav"); //SearchOption.TopDirectoryOnly);
              StringCollection stringCollection = new StringCollection();
              for (int index = 0; index < files.Length; ++index)
                stringCollection.Add(Path.GetFileName(files[index]));
              array = new string[stringCollection.Count + 1];
              stringCollection.CopyTo(array, 1);
              array[0] = cee5e96d25be00bb50a036ae3849574cc.DISABLED;
              rcVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d = array;
        }
        else
          array = rcVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d;
        return new TypeConverter.StandardValuesCollection((ICollection) array);
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
        return new TypeConverter.StandardValuesCollection((ICollection) new string[6]
        {
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2207),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2210),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2042),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2213),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2216),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2219)
        });
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
