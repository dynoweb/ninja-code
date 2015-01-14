// Type NinjaTrader.Indicator.ncatVolatilityBreakout
// Assembly ncatVolatilityBreakout, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null
// MVID 44D1D67D-B887-419E-93CB-54BC561A67EC
// Assembly location CUsersrcromerAppDataLocalTempKuvezedb8de101b15ncatVolatilityBreakout.dll

#region Using declarations
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
#endregion

namespace NinjaTrader.Indicator
{
  [Description("Dissassembled code")]
  public class NCVolatilityBreakout : Indicator
  {
	
	#region Variables
    private int length;
    private double theNumber2pt0;
    private double theNumber1pt5;
    private int theNumber20;
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
    private Color extendedLowColor;
    private Color GrayColor;
    private Color LimeColor;
    private Color DarkGreenColor;
    private Color anotherRedColor;
    private Color MaroonColor;
    private Color DarkGreenColor2;
    private Color aDarkRedColor;
    private int theNumber2_;
    private int theNumber5_;
    private ToolStripDropDownButton toolStripDropDownButton;
    private int ced563a5e9c3c71140c2e62f63afc998a;
    private Font myFont;
    private string typeName;
    private Color color_x;
    private Color color_x1;
    private Image image;
    private ncatVolatilityBreakout.hitPanel cfd0f789956fddac7d881844489c6bb27;
    private int c70fc78f80bfde9358a1dc14ea234aa72;
    private int currentBar;
    private int currentBar_;
    private double cfb105a9d24e167ceec6191571a1a8e43;
    private double c739d1f8b1109fe6f2de5bc1ec3c7d232;
    private ToolStripSeparator toolStripSeparator;
    private ToolStripButton toolStripButton;
    private IRectangle iRectangle;
    //private ListIRectangle listIRectangle;
    private ILine iLine;
    private Dictionary<ILine, Color> dictionaryILine;
    private IText c63b68670261102fc9a5683b8ef26ddae;
    private Dictionary<IText, Color> dictionaryIText;
    private int c6c9ad6a793905e213eacafa1be1fb516;
    private double c133da4068b26ce316e9f7ed309997e74;
    private double c950bfc49088216da2d12e2c56be386df;
    private int[] intArrayOf8;
    private int[] intArrayOf8_;
    private int c13d059c56853219888b6d45fccc6b9e1;
    [XmlIgnore]
    private Plot boxOutline;
    private bool shortLogo;
    private string email;
    private int barsLookback;
    private bool extendedVisible;
    private bool showLines;
    [XmlIgnore]
    private Plot plot;
    private bool extendedHighVisible;
    [XmlIgnore]
    private Plot extendedLow;
    private bool extendedLowVisible;
    [XmlIgnore]
    private Plot extendedMiddle;
    private bool extendedMiddleVisible;
    private int extendedMiddleLineLength;
    private int extendedLineLength;
    private Color boxAreaColor;
    private int boxAreaOpacity;
    private bool tickHeightVisible;
    private string tickHeightPosition;
    private Color tickHeightColor;
    private string fontSize;
    private bool extendedLevel1Visible;
    private bool extendedLevel2Visible;
    private bool extendedLevel3Visible;
    private int extendedLevel1;
    private int extendedLevel2;
    private int extendedLevel3;
    [XmlIgnore]
    private Plot extendedLineLevel1;
    [XmlIgnore]
    private Plot extendedLineLevel2;
    [XmlIgnore]
    private Plot extendedLineLevel3;
    private string soundHigh;
    private string soundLow;
    private string soundLevel1;
    private string soundLevel2;
    private string soundLevel3;
    private bool multiAlert;
	#endregion

    protected override void Initialize()
    {									// (PlotStyle) 3 changed to PlatStyle.Dot
      this.Add(new Plot(Color.Transparent, PlotStyle.Dot, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(839)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 3, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(854)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(873)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(896)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(923)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(946)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(973)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(998)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1031)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1058)));
      this.Add(new Plot(Color.Transparent, (PlotStyle) 0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1081)));
      this.dataSeries1 = new DataSeries((IndicatorBase) this);
      this.dataSeries2 = new DataSeries((IndicatorBase) this);
      this.set_CalculateOnBarClose(false);
      this.set_Overlay(true);
      this.set_PriceTypeSupported(false);
      this.set_PlotsConfigurable(false);
    }

	protected override void OnStartUp()
    {
      if (this.get_ChartControl() == null)
      {
		
label_1;
        switch (1)
        {
			case 0:
            goto label_1;
         default:
            if (1 != 0)
              break;
        //     ISSUE method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (NCVolatilityBreakout.OnStartUp);
            break;
        }
      }
      else
      {
        this.logoSetup();
        ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
        this.toolStripSeparator = new ToolStripSeparator();
        this.toolStripDropDownButton = new ToolStripDropDownButton(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629((int) byte.MaxValue));
        this.toolStripDropDownButton.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(278);
        ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(291));
        toolStripMenuItem1.Checked = this.extendedVisible;
       //  ISSUE method pointer
        toolStripMenuItem1.Click += new EventHandler((object) this, __methodptr(ce3690bd0a1e9603192f7898e782b1f4f));
        this.toolStripDropDownButton.DropDownItems.Add((ToolStripItem) toolStripMenuItem1);
        ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(302));
        toolStripMenuItem2.Checked = this.showLines;
        // ISSUE method pointer
        toolStripMenuItem2.Click += new EventHandler((object) this, __methodptr(c08cc5298d38e534db5d45a770a9e27d2));
        this.toolStripDropDownButton.DropDownItems.Add((ToolStripItem) toolStripMenuItem2);
        toolStrip.Items.Add((ToolStripItem) this.toolStripSeparator);
        toolStrip.Items.Add((ToolStripItem) this.toolStripDropDownButton);
        this.typeName = ((object) this).GetType().Name;
        this.myFont = new Font(((Control) this.get_ChartControl()).Font.FontFamily, (float) Convert.ToInt16(this.fontSize));
      }
    }

    protected override void OnBarUpdate()
    {
		/*
      if (this.get_CurrentBar() != 0)
      {
label_1:
        switch (6)
        {
			case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
             //  ISSUE method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.OnBarUpdate);
            }
            double num1 = this.ATR(this.length).get_Item(0);
            double num2 = this.StdDev(this.get_Close(), this.length).get_Item(0);
            double num3;
            if (this.theNumber1pt5 + num1 == 0.0)
            {
label_5:
              switch (6)
              {
                case 0:
                  goto label_5;
                default:
                  num3 = 1.0;
                  break;
              }
            }
            else
              num3 = this.theNumber2pt0 + num2 + (this.theNumber1pt5  + num1);
            if (num3 = 1.0)
            {
label_9:
              switch (5)
              {
                case 0:
                  goto label_9;
                default:
                  if (this.Squeeze.get_Item(1) != 0.0)
                  {
label_11:
                    switch (1)
                    {
                      case 0:
                        goto label_11;
                      default:
                        this.currentBar = this.get_CurrentBar();
                        this.cfb105a9d24e167ceec6191571a1a8e43 = this.get_High().get_Item(0);
                        this.c739d1f8b1109fe6f2de5bc1ec3c7d232 = this.get_Low().get_Item(0);
                        ++this.c70fc78f80bfde9358a1dc14ea234aa72;
                        break;
                    }
                  }
                  if (this.Squeeze.get_Item(1) == 0.0)
                  {
label_14:
                    switch (4)
                    {
                      case 0:
                        goto label_14;
                      default:
                        if (this.get_High().get_Item(0) > this.cfb105a9d24e167ceec6191571a1a8e43)
                        {
label_16:
                          switch (3)
                          {
                            case 0:
                              goto label_16;
                            default:
                              this.cfb105a9d24e167ceec6191571a1a8e43 = this.get_High().get_Item(0);
                              break;
                          }
                        }
                        if (this.get_Low().get_Item(0)  this.c739d1f8b1109fe6f2de5bc1ec3c7d232)
                        {
label_19:
                          switch (6)
                          {
                            case 0:
                              goto label_19;
                            default:
                              this.c739d1f8b1109fe6f2de5bc1ec3c7d232 = this.get_Low().get_Item(0);
                              break;
                          }
                        }
                        this.currentBar_ = this.get_CurrentBar();
                        if (this.currentBar != this.currentBar_)
                        {
                          this.c133da4068b26ce316e9f7ed309997e74 = (this.cfb105a9d24e167ceec6191571a1a8e43 + this.c739d1f8b1109fe6f2de5bc1ec3c7d232)  2.0;
                          this.c950bfc49088216da2d12e2c56be386df = this.cfb105a9d24e167ceec6191571a1a8e43 - this.c739d1f8b1109fe6f2de5bc1ec3c7d232;
                          this.iRectangle = this.DrawRectangle(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1102) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar, this.cfb105a9d24e167ceec6191571a1a8e43, this.get_CurrentBar() - this.currentBar_, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.boxOutline.get_Pen().Color, this.boxAreaColor, this.boxAreaOpacity);
                          ((IShape) this.iRectangle).get_Pen().DashStyle = this.boxOutline.get_Pen().DashStyle;
                          ((IShape) this.iRectangle).get_Pen().Width = this.boxOutline.get_Pen().Width;
                          if (this.extendedHighVisible)
                          {
label_23
                            switch (3)
                            {
                              case 0
                                goto label_23;
                              default
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1109) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.cfb105a9d24e167ceec6191571a1a8e43, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43, this.plot.get_Pen().Color, this.plot.get_Pen().DashStyle, (int) this.plot.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                  this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                if (!this.showLines)
                                {
                                  this.iLine.get_Pen().Color = Color.Transparent;
                                  break;
                                }
                                else
                                  break;
                            }
                          }
                          if (this.extendedLowVisible)
                          {
label_29
                            switch (7)
                            {
                              case 0
                                goto label_29;
                              default
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1114) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.extendedLow.get_Pen().Color, this.extendedLow.get_Pen().DashStyle, (int) this.extendedLow.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_31
                                  switch (6)
                                  {
                                    case 0
                                      goto label_31;
                                    default
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_34
                                  switch (4)
                                  {
                                    case 0
                                      goto label_34;
                                    default
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.extendedMiddleVisible)
                          {
label_37
                            switch (4)
                            {
                              case 0
                                goto label_37;
                              default
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1119) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.c133da4068b26ce316e9f7ed309997e74, this.get_CurrentBar() - this.currentBar_ - this.extendedMiddleLineLength, this.c133da4068b26ce316e9f7ed309997e74, this.extendedMiddle.get_Pen().Color, this.extendedMiddle.get_Pen().DashStyle, (int) this.extendedMiddle.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_39
                                  switch (1)
                                  {
                                    case 0
                                      goto label_39;
                                    default
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_42
                                  switch (4)
                                  {
                                    case 0
                                      goto label_42;
                                    default
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.extendedLevel1Visible)
                          {
label_45:
                            switch (7)
                            {
                              case 0:
                                goto label_45;
                              default:
                                double num4 = this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel1  100.0;
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1126) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.extendedLineLevel1.get_Pen().Color, this.extendedLineLevel1.get_Pen().DashStyle, (int) this.extendedLineLevel1.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_47:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_47;
                                    default:
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_50:
                                  switch (2)
                                  {
                                    case 0:
                                      goto label_50;
                                    default:
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1135) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.extendedLineLevel1.get_Pen().Color, this.extendedLineLevel1.get_Pen().DashStyle, (int) this.extendedLineLevel1.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_53:
                                  switch (3)
                                  {
                                    case 0:
                                      goto label_53;
                                    default:
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_56:
                                  switch (6)
                                  {
                                    case 0:
                                      goto label_56;
                                    default:
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.extendedLevel2Visible)
                          {
label_59:
                            switch (2)
                            {
                              case 0:
                                goto label_59;
                              default:
                                double num4 = this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel2  100.0;
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1144) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.extendedLineLevel2.get_Pen().Color, this.extendedLineLevel2.get_Pen().DashStyle, (int) this.extendedLineLevel2.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_61:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_61;
                                    default:
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_64:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_64;
                                    default:
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1153) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.extendedLineLevel2.get_Pen().Color, this.extendedLineLevel2.get_Pen().DashStyle, (int) this.extendedLineLevel2.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_67
                                  switch (1)
                                  {
                                    case 0
                                      goto label_67;
                                    default
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_70:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_70;
                                    default:
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.extendedLevel2Visible)
                          {
label_73:
                            switch (1)
                            {
                              case 0:
                                goto label_73;
                              default:
                                double num4 = this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel3  100.0;
                                Plot plot = this.extendedLineLevel3;
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1162) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, plot.get_Pen().Color, plot.get_Pen().DashStyle, (int) plot.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_75:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_75;
                                    default:
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_78:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_78;
                                    default:
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                this.iLine = this.DrawLine(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1171) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.currentBar_, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.get_CurrentBar() - this.currentBar_ - this.extendedLineLength, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, plot.get_Pen().Color, plot.get_Pen().DashStyle, (int) plot.get_Pen().Width);
                                if (!this.dictionaryILine.ContainsKey(this.iLine))
                                {
label_81:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_81;
                                    default:
                                      this.dictionaryILine.Add(this.iLine, this.iLine.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.showLines)
                                {
label_84:
                                  switch (7)
                                  {
                                    case 0:
                                      goto label_84;
                                    default:
                                      this.iLine.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.tickHeightVisible)
                          {
label_87:
                            switch (7)
                            {
                              case 0:
                                goto label_87;
                              default:
                                int num4 = Convert.ToInt32((this.cfb105a9d24e167ceec6191571a1a8e43 - this.c739d1f8b1109fe6f2de5bc1ec3c7d232)  this.get_TickSize());
                                if (this.tickHeightPosition == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1180)  this.tickHeightPosition == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1191))
                                {
                                  this.c63b68670261102fc9a5683b8ef26ddae = this.DrawText(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1200) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, num4.ToString(), this.get_CurrentBar() - this.currentBar_, this.c133da4068b26ce316e9f7ed309997e74, 0, this.tickHeightColor, this.myFont, StringAlignment.Near, Color.Transparent, Color.Transparent, 1);
                                  if (!this.dictionaryIText.ContainsKey(this.c63b68670261102fc9a5683b8ef26ddae))
                                  {
label_90:
                                    switch (2)
                                    {
                                      case 0:
                                        goto label_90;
                                      default:
                                        this.dictionaryIText.Add(this.c63b68670261102fc9a5683b8ef26ddae, this.c63b68670261102fc9a5683b8ef26ddae.get_TextColor());
                                        break;
                                    }
                                  }
                                  if (!this.extendedVisible)
                                  {
label_93:
                                    switch (7)
                                    {
                                      case 0:
                                        goto label_93;
                                      default:
                                        this.c63b68670261102fc9a5683b8ef26ddae.set_TextColor(Color.Transparent);
                                        break;
                                    }
                                  }
                                }
                                if (!(this.tickHeightPosition == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1211)))
                                {
label_96:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_96;
                                    default:
                                      if (this.tickHeightPosition == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1191))
                                      {
label_98:
                                        switch (4)
                                        {
                                          case 0:
                                            goto label_98;
                                        }
                                      }
                                      else
                                        goto label_105;
                                  }
                                }
                                this.c63b68670261102fc9a5683b8ef26ddae = this.DrawText(this.typeName + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1220) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, num4.ToString(), this.get_CurrentBar() - this.currentBar, this.c133da4068b26ce316e9f7ed309997e74, 0, this.tickHeightColor, this.myFont, StringAlignment.Far, Color.Transparent, Color.Transparent, 1);
                                if (!this.dictionaryIText.ContainsKey(this.c63b68670261102fc9a5683b8ef26ddae))
                                {
label_100:
                                  switch (7)
                                  {
                                    case 0:
                                      goto label_100;
                                    default:
                                      this.dictionaryIText.Add(this.c63b68670261102fc9a5683b8ef26ddae, this.c63b68670261102fc9a5683b8ef26ddae.get_TextColor());
                                      break;
                                  }
                                }
                                if (!this.extendedVisible)
                                {
label_103:
                                  switch (7)
                                  {
                                    case 0:
                                      goto label_103;
                                    default:
                                      this.c63b68670261102fc9a5683b8ef26ddae.set_TextColor(Color.Transparent);
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
label_105:
                          if (!this.extendedVisible)
                          {
label_106:
                            switch (6)
                            {
                              case 0:
                                goto label_106;
                              default:
                                this.color_x = ((IShape) this.iRectangle).get_AreaColor();
                                this.color_x1 = ((IShape) this.iRectangle).get_Pen().Color;
                                ((IShape) this.iRectangle).set_AreaColor(Color.Transparent);
                                ((IShape) this.iRectangle).get_Pen().Color = Color.Transparent;
                                break;
                            }
                          }
                          if (!this.listIRectangle.Contains(this.iRectangle))
                          {
label_109:
                            switch (7)
                            {
                              case 0:
                                goto label_109;
                              default:
                                this.listIRectangle.Add(this.iRectangle);
                                break;
                            }
                          }
                          else
                            break;
                        }
                        else
                          break;
                    }
                  }
                  this.Squeeze.Set(0.0);
                  this.SqueezeOn.Reset();
                  break;
              }
            }
            else
            {
              this.Squeeze.Reset();
              this.SqueezeOn.Set(0.0);
            }
            this.dataSeries2.Set(this.get_Input().get_Item(0) - (this.DonchianChannel(this.get_Input(), this.theNumber20).Mean.Get(this.get_CurrentBar()) + this.EMA(this.get_Input(), this.theNumber20).get_Item(0))  2.0);
            this.dataSeries1.Set(this.LinReg((IDataSeries) this.dataSeries2, this.theNumber20).get_Item(0));
            this.PMomentumUp.Set(0.0);
            this.PMomentumDown.Set(0.0);
            this.NMomentumUp.Set(0.0);
            this.NMomentumDown.Set(0.0);
            if (this.dataSeries1.get_Item(0)  0.0)
            {
label_114:
              switch (3)
              {
                case 0:
                  goto label_114;
                default:
                  if (this.dataSeries1.get_Item(0)  this.dataSeries1.get_Item(1))
                  {
label_116:
                    switch (5)
                    {
                      case 0:
                        goto label_116;
                      default:
                        this.PMomentumUp.Set(this.dataSeries1.get_Item(0));
                        break;
                    }
                  }
                  else
                  {
                    this.PMomentumDown.Set(this.dataSeries1.get_Item(0));
                    break;
                  }
              }
            }
            else if (this.dataSeries1.get_Item(0)  this.dataSeries1.get_Item(1))
            {
              this.NMomentumUp.Set(this.dataSeries1.get_Item(0));
              break;
            }
            else
            {
              this.NMomentumDown.Set(this.dataSeries1.get_Item(0));
              break;
            }
        }
      }
      this.myMethod2(this.SoundHigh, 0, this.cfb105a9d24e167ceec6191571a1a8e43, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1229));
      this.myMethod2(this.SoundLevel1, 1, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel1  100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1308));
      this.myMethod2(this.SoundLevel2, 2, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel2  100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1401));
      this.myMethod2(this.SoundLevel3, 3, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel3  100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1494));
      this.myMethod(this.SoundLow, 4, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1587));
      this.myMethod(this.SoundLevel1, 5, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel1  100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1664));
      this.myMethod(this.SoundLevel2, 6, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel2  100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1755));
      this.myMethod(this.SoundLevel3, 7, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df  (double) this.extendedLevel3  100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1846));
      this.CurrentValue.Set(this.dataSeries1.get_Item(0));
      if (this.c13d059c56853219888b6d45fccc6b9e1 != this.get_CurrentBar())
      {
label_123:
        switch (5)
        {
          case 0:
            goto label_123;
          default:
            this.c13d059c56853219888b6d45fccc6b9e1 = this.get_CurrentBar();
            if (this.SqueezeOn.get_Item(0) == 0.0)
            {
label_125:
              switch (4)
              {
                case 0:
                  goto label_125;
                default:
                  ++this.ced563a5e9c3c71140c2e62f63afc998a;
                  break;
              }
            }
            else
              this.ced563a5e9c3c71140c2e62f63afc998a = 0;
            if (this.Squeeze.get_Item(0) == 0.0)
            {
label_129:
              switch (2)
              {
                case 0:
                  goto label_129;
                default:
                  ++this.c6c9ad6a793905e213eacafa1be1fb516;
                  break;
              }
            }
            else
            {
              this.c6c9ad6a793905e213eacafa1be1fb516 = 0;
              break;
            }
        }
      }
      if (this.ced563a5e9c3c71140c2e62f63afc998a  this.barsLookback)
      {
label_133:
        switch (5)
        {
          case 0:
            goto label_133;
          default:
            this.BarsSinceSqueeze.Set(0.0);
            break;
        }
      }
      else
        this.BarsSinceSqueeze.Set((double) this.ced563a5e9c3c71140c2e62f63afc998a);
      this.SqueezeLength.Set((double) this.c6c9ad6a793905e213eacafa1be1fb516);
      this.SqueezeHigh.Set(this.cfb105a9d24e167ceec6191571a1a8e43);
      this.SqueezeLow.Set(this.c739d1f8b1109fe6f2de5bc1ec3c7d232);
    }

    private void myMethod2(string str_arg_1, int int_arg_1, double double_arg_1, string c549929c7d4f914e2cdb9a099d7a7eb21)
    {
      if (!(string_arg_1 != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937)))
        return;
label_1:
      switch (1)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
             //ISSUE method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (NCVolatilityBreakout.myMethod2);
          }
          if (this.intArrayOf8[int_arg_1] == this.get_CurrentBar()  this.intArrayOf8_[int_arg_1] == this.currentBar_)
            break;
label_5:
          switch (1)
          {
            case 0:
              goto label_5;
            default:
              if (this.get_Close().get_Item(1) = double_arg_1  this.get_Close().get_Item(0)  double_arg_1  this.get_CurrentBar()  this.currentBar_)
                return;
label_7:
              switch (5)
              {
                case 0:
                  goto label_7;
                default:
                  if (this.get_CurrentBar() = this.currentBar_ + this.extendedLineLength)
                    return;
label_9:
                  switch (2)
                  {
                    case 0:
                      goto label_9;
                    default:
                      if (!this.multiAlert)
                      {
label_11:
                        switch (7)
                        {
                          case 0:
                            goto label_11;
                          default:
                            this.intArrayOf8_[int_arg_1] = this.currentBar_;
                            break;
                        }
                      }
                      this.intArrayOf8[int_arg_1] = this.get_CurrentBar();
                      this.c013129a8956cc8e16ff7ec447af66499(c549929c7d4f914e2cdb9a099d7a7eb21);
                      this.PlaySound(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1954) + string_arg_1);
                      return;
                  }
              }
          }
      }
		*/
    }
	
	

	
	
    public NCVolatilityBreakout()
    {
      this.length = 20;
      this.theNumber2pt0 = 2.0;
      this.theNumber1pt5 = 1.5;
      this.theNumber20 = 20;
      this.extendedLowColor = Color.Red;
      this.GrayColor = Color.Gray;
      this.LimeColor = Color.Lime;
      this.DarkGreenColor = Color.DarkGreen;
      this.anotherRedColor = Color.Red;
      this.MaroonColor = Color.Maroon;
      this.DarkGreenColor2 = Color.DarkGreen;
      this.aDarkRedColor = Color.DarkRed;
      this.theNumber2_ = 2;
      this.theNumber5_ = 5;
      this.typeName = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1971);
      this.listIRectangle = new ListIRectangle();
      this.dictionaryILine = new DictionaryILine, Color();
      this.dictionaryIText = new DictionaryIText, Color();
      this.intArrayOf8 = new int[8];
      this.intArrayOf8_ = new int[8];
      this.boxOutline = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1974));
      this.email = ;
      this.barsLookback = 30;
      this.extendedVisible = true;
      this.showLines = true;
      this.plot = new Plot(new Pen(Color.Green, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1995));
      this.extendedHighVisible = true;
      this.extendedLow = new Plot(new Pen(Color.Red, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2010));
      this.extendedLowVisible = true;
      this.extendedMiddle = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2023));
      this.extendedMiddleLineLength = 6;
      this.extendedLineLength = 20;
      this.boxAreaColor = Color.Orange;
      this.boxAreaOpacity = 1;
      this.tickHeightVisible = true;
      this.tickHeightPosition = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1211);
      this.tickHeightColor = Color.Black;
      this.fontSize = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2042);
      this.extendedLevel1Visible = true;
      this.extendedLevel2Visible = true;
      this.extendedLevel3Visible = true;
      this.extendedLevel1 = 50;
      this.extendedLevel2 = 100;
      this.extendedLevel3 = 200;
      this.extendedLineLevel1 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2045));
      this.extendedLineLevel2 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2082));
      this.extendedLineLevel3 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2119));
      this.soundHigh = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
      this.soundLow = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
      this.soundLevel1 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
      this.soundLevel2 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
      this.soundLevel3 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
      base.u002Ector();
    }

    private void c08cc5298d38e534db5d45a770a9e27d2(object myObject, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      ToolStripMenuItem toolStripMenuItem = myObject as ToolStripMenuItem;
      if (toolStripMenuItem.Checked)
      {
        using (DictionaryILine, Color.Enumerator enumerator = this.dictionaryILine.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Key.get_Pen().Color = Color.Transparent;
label_5:
          switch (1)
          {
            case 0:
              goto label_5;
            default:
              if (1 == 0)
              {
                 //ISSUE method reference
                RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.c08cc5298d38e534db5d45a770a9e27d2);
                break;
              }
              else
                break;
          }
        }
        ((Control) this.get_ChartControl()).Refresh();
        this.cfd0f789956fddac7d881844489c6bb27.Refresh();
        toolStripMenuItem.Checked = false;
        this.showLines = false;
      }
      else
      {
        if (toolStripMenuItem.Checked)
          return;
label_11:
        switch (6)
        {
			case 0:
            goto label_11;
          default:
            using (DictionaryILine, Color.Enumerator enumerator = this.dictionaryILine.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePairILine, Color current = enumerator.Current;
                current.Key.get_Pen().Color = current.Value;
              }
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.cfd0f789956fddac7d881844489c6bb27.Refresh();
            toolStripMenuItem.Checked = true;
            this.showLines = true;
            break;
        }
      }
    }

    private void ce3690bd0a1e9603192f7898e782b1f4f(object myObject, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      ToolStripMenuItem toolStripMenuItem = myObject as ToolStripMenuItem;
      if (toolStripMenuItem.Checked)
      {
label_1:
        switch (4)
        {
			case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
              // ISSUE method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.ce3690bd0a1e9603192f7898e782b1f4f);
            }
            using (ListIRectangle.Enumerator enumerator = this.listIRectangle.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IRectangle current = enumerator.Current;
                this.color_x = ((IShape) current).get_AreaColor();
                this.color_x1 = ((IShape) current).get_Pen().Color;
                ((IShape) current).set_AreaColor(Color.Transparent);
                ((IShape) current).get_Pen().Color = Color.Transparent;
              }
            }
            using (DictionaryIText, Color.Enumerator enumerator = this.dictionaryIText.GetEnumerator())
            {
              while (enumerator.MoveNext())
                enumerator.Current.Key.set_TextColor(Color.Transparent);
label_13:
              switch (1)
              {
                case 0:
                  goto label_13;
              }
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.cfd0f789956fddac7d881844489c6bb27.Refresh();
            toolStripMenuItem.Checked = false;
            this.extendedVisible = false;
            break;
        }
      }
      else
      {
        if (toolStripMenuItem.Checked)
          return;
label_17:
        switch (2)
        {
			case 0:
            goto label_17;
          default
            using (ListIRectangle.Enumerator enumerator = this.listIRectangle.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IRectangle current = enumerator.Current;
                ((IShape) current).set_AreaColor(this.color_x);
                ((IShape) current).get_Pen().Color = this.color_x1;
              }
label_22:
              switch (1)
              {
                case 0:
                  goto label_22;
              }
            }
            using (DictionaryIText, Color.Enumerator enumerator = this.dictionaryIText.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePairIText, Color current = enumerator.Current;
                current.Key.set_TextColor(current.Value);
              }
label_28:
              switch (1)
              {
                case 0:
                  goto label_28;
              }
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.cfd0f789956fddac7d881844489c6bb27.Refresh();
            toolStripMenuItem.Checked = true;
            this.extendedVisible = true;
            break;
        }
      }
    }

    protected virtual void OnTermination()
    {
      try
      {
        if (this.get_ChartControl() == null)
        {
label_1:
          switch (4)
          {
            case 0:
              goto label_1;
            default:
              if (1 != 0)
                break;
             //  ISSUE method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.OnTermination);
              break;
          }
        }
        else
        {
           //ISSUE method pointer
          this.cfd0f789956fddac7d881844489c6bb27.Click -= new EventHandler((object) this, __methodptr(cd7409d0fdab3b2ac0617894a0a49bbf0));
          this.get_ChartControl().get_ChartPanel().Controls.Remove((Control) this.cfd0f789956fddac7d881844489c6bb27);
          ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
          toolStrip.Items.Remove((ToolStripItem) this.toolStripSeparator);
          toolStrip.Items.Remove((ToolStripItem) this.toolStripDropDownButton);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public void logoSetup()
    {
      string str1 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(313);
      string str2;
      if (this.shortLogo)
      {
label_1:
        switch (5)
        {
			case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
             //  ISSUE method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.logoSetup);
            }
            str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(332);
            break;
        }
      }
      else
        str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(313);
      try
      {
        if (System.IO.File.Exists(str2))
        {
label_7:
          switch (5)
          {
            case 0:
              goto label_7;
            default:
              System.IO.File.GetCreationTime(str2);
              break;
          }
        }
        else
        {
          WebClient webClient = new WebClient();
          if (this.shortLogo)
          {
label_10:
            switch (5)
            {
				case 0:
                goto label_10;
              default:
                webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(359), str2);
                break;
            }
          }
          else
            webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(478), str2);
        }
        this.image = Image.FromFile(str2);
        this.cfd0f789956fddac7d881844489c6bb27 = new ncatVolatilityBreakout.hitPanel();
        this.cfd0f789956fddac7d881844489c6bb27.Height = this.image.Height;
        this.cfd0f789956fddac7d881844489c6bb27.Width = this.image.Width;
        this.cfd0f789956fddac7d881844489c6bb27.Anchor = AnchorStyles.Bottom  AnchorStyles.Left;
        this.cfd0f789956fddac7d881844489c6bb27.Top = this.get_ChartControl().get_ChartPanel().Bounds.Bottom - this.image.Height - 80;
        this.cfd0f789956fddac7d881844489c6bb27.Left = 1;
         //ISSUE method pointer
        this.cfd0f789956fddac7d881844489c6bb27.Click += new EventHandler((object) this, __methodptr(cd7409d0fdab3b2ac0617894a0a49bbf0));
        this.get_ChartControl().get_ChartPanel().Controls.Add((Control) this.cfd0f789956fddac7d881844489c6bb27);
      }
      catch (Exception ex)
      {
        this.Print(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(589));
      }
    }

    public virtual void Plot(Graphics g, Rectangle bounds, double min, double max)
    {
      g.DrawImage(this.image, 2, bounds.Bottom - this.image.Height - 18, this.image.Width, this.image.Height);
    }

    private void cd7409d0fdab3b2ac0617894a0a49bbf0(object myObject, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      Process.Start(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(622), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(647));
    }

    private void c013129a8956cc8e16ff7ec447af66499(string c63b68670261102fc9a5683b8ef26ddae)
    {
      if (this.email.Length = 0)
        return;
label_1:
      switch (5)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
            // ISSUE method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.c013129a8956cc8e16ff7ec447af66499);
          }
          this.SendMail(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(700), this.email, this.get_Name() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(747), c63b68670261102fc9a5683b8ef26ddae + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(760) + (string) (object) DateTime.Now + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(795) + this.get_Instrument().get_MasterInstrument().get_Name() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(822) + (string) (object) this.get_Input().get_Item(0));
          break;
      }
    }



    private void myMethod(string string_arg_1, int int_arg_1, double double_arg_1, string c549929c7d4f914e2cdb9a099d7a7eb21)
    {
      if (!(string_arg_1 != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937)))
        return;
label_1:
      switch (6)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
          //   ISSUE method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (NCVolatilityBreakout.myMethod);
          }
          if (this.intArrayOf8[int_arg_1] == this.get_CurrentBar())
            break;
label_5:
          switch (6)
          {
            case 0:
              goto label_5;
            default:
              if (this.intArrayOf8_[int_arg_1] == this.currentBar_)
                return;
label_7:
              switch (2)
              {
                case 0:
                  goto label_7;
                default:
                  if (Close[1] == double_arg_1 && Close[0] ==  double_arg_1)
                    return;
label_9:
                  switch (2)
                  {
                    case 0:
                      goto label_9;
                    default:
                      if (this.get_CurrentBar()  this.currentBar_  this.get_CurrentBar() = this.currentBar_ + this.extendedLineLength)
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
                                this.intArrayOf8_[int_arg_1] = this.currentBar_;
                                break;
                            }
                          }
                          this.intArrayOf8[int_arg_1] = this.get_CurrentBar();
                          this.c013129a8956cc8e16ff7ec447af66499(c549929c7d4f914e2cdb9a099d7a7eb21);
                          this.PlaySound(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1954) + string_arg_1);
                          return;
                      }
                  }
              }
          }
      }
    }

    private string myMethod3(Color c4544fc12a8cffe1b31db6f02f2dd887e)
    {
      return c4544fc12a8cffe1b31db6f02f2dd887e.ToArgb().ToString();
    }

    private Color myMethod4(string c4544fc12a8cffe1b31db6f02f2dd887e)
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
          createParams.ExStyle = 32;
          return createParams;
        }
      }

      public hitPanel()
      {
        base.u002Ector();
      }

      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
      }
    }

    public class XmlColor
    {
      private Color cf961a6779df9914187bdd86c52824c74;

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
                    RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.XmlColor.set_Web);
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
        this.cf961a6779df9914187bdd86c52824c74 = Color.Black;
        base.u002Ector();
      }

      public XmlColor(Color c)
      {
        this.cf961a6779df9914187bdd86c52824c74 = Color.Black;
        base.u002Ector();
        this.cf961a6779df9914187bdd86c52824c74 = c;
      }

      public static implicit operator Color(ncatVolatilityBreakout.XmlColor x)
      {
        return x.ToColor();
      }

      public static implicit operator ncatVolatilityBreakout.XmlColor(Color c)
      {
        return new ncatVolatilityBreakout.XmlColor(c);
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
        return (int) this.Alpha  (int) byte.MaxValue;
      }
    }

    public class WaveConverter  TypeConverter
    {
      private static string[] c881dc1ea8e7199b694d684a64842fb7d;

      static WaveConverter()
      {
      }

      public WaveConverter()
      {
        base.u002Ector();
      }

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
        string[] array;
        if (ncatVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d == null)
        {
label_1:
          switch (3)
          {
            case 0:
              goto label_1;
            default:
              if (1 == 0)
              {
                // ISSUE method reference
                RuntimeMethodHandle runtimeMethodHandle = __methodref (NCVolatilityBreakout.WaveConverter.GetStandardValues);
              }
              string[] files = Directory.GetFiles(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2177), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2196));
              StringCollection stringCollection = new StringCollection();
              for (int index = 0; index  files.Length; ++index)
                stringCollection.Add(Path.GetFileName(files[index]));
label_7:
              switch (5)
              {
                case 0:
                  goto label_7;
                default:
                  array = new string[stringCollection.Count + 1];
                  stringCollection.CopyTo(array, 1);
                  array[0] = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937);
                  ncatVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d = array;
                  break;
              }
          }
        }
        else
          array = ncatVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d;
        return new TypeConverter.StandardValuesCollection((ICollection) array);
      }
    }

    private class cb7fd7594b63cf924b3e645a1b96661fe : UITypeEditor
    {
      public cb7fd7594b63cf924b3e645a1b96661fe()
      {
        base.u002Ector();
      }

      public override bool GetPaintValueSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override void PaintValue(PaintValueEventArgs e)
      {
        if (e.Value == null)
          return;
label_1:
        switch (4)
        {
          case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
              // ISSUE method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe.PaintValue);
            }
            SolidBrush solidBrush = new SolidBrush(((Plot) e.Value).get_Pen().Color);
            e.Graphics.FillRectangle((Brush) solidBrush, e.Bounds);
            solidBrush.Dispose();
            break;
        }
      }
    }

    public class positionModeConverter : TypeConverter
    {
      public positionModeConverter()
      {
        base.u002Ector();
      }

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
        return new TypeConverter.StandardValuesCollection((ICollection) new string[4]
        {
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1211),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1180),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1191),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937)
        });
      }
    }

    public class sizeConverter : TypeConverter
    {
      public sizeConverter()
      {
        base.u002Ector();
      }

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
	
	
	#region properties
	
    [Editor(typeof (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Category(Visual)]
    [Description()]
    public Plot BoxOutline
    {
      get { return this.boxOutline; }
      set { this.boxOutline = value; }
    }

    [GridCategory(tNinjacators)]
    [Description()]
    public bool ShortLogo
    {
      get { return this.shortLogo; }
      set { this.shortLogo = value; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries Squeeze
    {
      get { return this.get_Values()[0];}
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries SqueezeOn
    {
      get { return this.get_Values()[1]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries PMomentumUp
    {
      get { return this.get_Values()[2]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries PMomentumDown
    {
      get { return this.get_Values()[3]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries NMomentumUp
    {
      get { return this.get_Values()[4]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries NMomentumDown
    {
      get { return this.get_Values()[5]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries CurrentValue
    {
      get { return this.get_Values()[6]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries BarsSinceSqueeze
    {
      get { return this.get_Values()[7]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeLength
    {
      get { return this.get_Values()[8]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeHigh
    {
      get { return this.get_Values()[9]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeLow
    {
      get { return this.get_Values()[10]; }
    }

    [Description("Volatility Strength")]
    [GridCategory("Consolidation box")]
    public int Length
    {
      get { return this.length; }
      set { this.length = Math.Max(1, value);}
    }

    [Description()]
    [Category(Alert)]
    public string Email
    {
      get { return this.email; }
      set { this.email = value; }
    }

    [Description()]
    [Category(Global)]
    public int BarsLookback
    {
      get { return this.barsLookback; }
      set { this.barsLookback = value; }
    }

    [Category(Global)]
    [Description(ShowBoxes)]
    public bool ExtendedVisible
    {
      get { return this.extendedVisible; }
      set { this.extendedVisible = value; }
    }

    [Description(ShowLines)]
    [Category(Global)]
    public bool ShowLines
    {
      get { return this.showLines; }
      set { this.showLines = value; }
    }

    [Category(Visual)]
    [Editor(typeof (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description()]
    public Plot ExtendedHigh
    {
      get { return this.plot; }
      set { this.plot = value; }
    }

    [Category(Settings)]
    public bool ExtendedHighVisible
    {
      get { return this.extendedHighVisible; }
      set { this.extendedHighVisible = value; }
    }

    [Editor(typeof (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Category(Visual)]
    [Description()]
    public Plot ExtendedLow
    {
      get { return this.extendedLow; }
      set { this.extendedLow = value; }
    }

    [Category(Settings)]
    public bool ExtendedLowVisible
    {
      get { return this.extendedLowVisible; }
      set { this.extendedLowVisible = value; }
    }

    [Category(Visual)]
    [Editor(typeof (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description()]
    public Plot ExtendedMiddle
    {
      get { return this.extendedMiddle; }
      set { this.extendedMiddle = value; }
    }

    [Category(Settings)]
    public bool ExtendedMiddleVisible
    {
      get { return this.extendedMiddleVisible; }
      set { this.extendedMiddleVisible = value; }
    }

    [Category(Settings)]
    [Description()]
    public int ExtendedMiddleLineLength
    {
      get { return this.extendedMiddleLineLength; }
      set { this.extendedMiddleLineLength = value; }
    }

    [Description()]
    [Category(Settings)]
    public int ExtendedLineLength
    {
      get { return this.extendedLineLength; }
      set { this.extendedLineLength = value; }
    }

    [Category(Visual)]
    [Description()]
    [XmlElement(Type = typeof (ncatVolatilityBreakout.XmlColor))]
    public Color BoxAreaColor
    {
      get { return this.boxAreaColor; }
      set { this.boxAreaColor = value; }
    }

    [Description()]
    [Category(Visual)]
    public int BoxAreaOpacity
    {
      get { return this.boxAreaOpacity; }
      set { this.boxAreaOpacity = value; }
    }

    [Category(Visual)]
    [Description(TickHeightVisible)]
    public bool TickHeightVisible
    {
      get { return this.tickHeightVisible; }
      set { this.tickHeightVisible = value; }
    }

    [Category(Visual)]
    [Description()]
    [TypeConverter(typeof (ncatVolatilityBreakout.positionModeConverter))]
    public string TickHeightPosition
    {
      get { return this.tickHeightPosition; }
      set { this.tickHeightPosition = value; }
    }

    [Category(Visual)]
    [XmlElement(Type = typeof (ncatVolatilityBreakout.XmlColor))]
    [Description()]
    public Color TickHeightColor
    {
      get { return this.tickHeightColor; }
      set { this.tickHeightColor = value; }
    }

    [Description(FontSize)]
    [Category(Settings)]
    [TypeConverter(typeof (ncatVolatilityBreakout.sizeConverter))]
    public string FontSize
    {
      get { return this.fontSize; }
      set { this.fontSize = value; }
    }

    [Category(Settings)]
    public bool ExtendedLevel1Visible
    {
      get { return this.extendedLevel1Visible; }
      set { this.extendedLevel1Visible = value; }
    }

    [Category(Settings)]
    public bool ExtendedLevel2Visible
    {
      get { return this.extendedLevel2Visible; }
      set { this.extendedLevel2Visible = value; }
    }

    [Category(Settings)]
    public bool ExtendedLevel3Visible
    {
      get {return this.extendedLevel3Visible; }
      set { this.extendedLevel3Visible = value; }
    }

    [Category(Settings)]
    [Description()]
    public int ExtendedLevel1
    {
      get { return this.extendedLevel1; }
      set { this.extendedLevel1 = value; }
    }

    [Category(Settings)]
    [Description()]
    public int ExtendedLevel2
    {
      get { return this.extendedLevel2; }
      set { this.extendedLevel2 = value; }
    }

    [Category(Settings)]
    [Description()]
    public int ExtendedLevel3
    {
      get { return this.extendedLevel3; }
      set { this.extendedLevel3 = value; }
    }

    [Category(Visual)]
    [Editor(typeof (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description()]
    public Plot ExtendedLineLevel1
    {
      get { return this.extendedLineLevel1; }
      set { this.extendedLineLevel1 = value; }
    }

    [Category(Visual)]
    [Editor(typeof (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description()]
    public Plot ExtendedLineLevel2
    {
      get { return this.extendedLineLevel2; }
      set { this.extendedLineLevel2 = value; }
    }

    [Editor(typeof (ncatVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description()]
    [Category(Visual)]
    public Plot ExtendedLineLevel3
    {
      get { return this.extendedLineLevel3; }
      set { this.extendedLineLevel3 = value; }
    }

    [Category(Alert)]
    [TypeConverter(typeof (ncatVolatilityBreakout.WaveConverter))]
    [Description()]
    public string SoundHigh
    {
      get { return this.soundHigh; }
      set { this.soundHigh = value; }
    }

    [Description()]
    [Category(Alert)]
    [TypeConverter(typeof (ncatVolatilityBreakout.WaveConverter))]
    public string SoundLow
    {
      get { return this.soundLow; }
      set { this.soundLow = value; }
    }

    [Description()]
    [TypeConverter(typeof (ncatVolatilityBreakout.WaveConverter))]
    [Category(Alert)]
    public string SoundLevel1
    {
      get { return this.soundLevel1; }
      set { this.soundLevel1 = value; }
    }

    [TypeConverter(typeof (ncatVolatilityBreakout.WaveConverter))]
    [Category(Alert)]
    [Description()]
    public string SoundLevel2
    {
      get { return this.soundLevel2; }
      set { this.soundLevel2 = value; }
    }

    [Description()]
    [TypeConverter(typeof (ncatVolatilityBreakout.WaveConverter))]
    [Category(Alert)]
    public string SoundLevel3
    {
      get { return this.soundLevel3; }
      set { this.soundLevel3 = value; }
    }

    [Description()]
    [Category(Alert)]
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
        private NCVolatilityBreakout[] cacheNCVolatilityBreakout = null;

        private static NCVolatilityBreakout checkNCVolatilityBreakout = new NCVolatilityBreakout();

        /// <summary>
        /// Dissassembled code
        /// </summary>
        /// <returns></returns>
        public NCVolatilityBreakout NCVolatilityBreakout(int length)
        {
            return NCVolatilityBreakout(Input, length);
        }

        /// <summary>
        /// Dissassembled code
        /// </summary>
        /// <returns></returns>
        public NCVolatilityBreakout NCVolatilityBreakout(Data.IDataSeries input, int length)
        {
            if (cacheNCVolatilityBreakout != null)
                for (int idx = 0; idx < cacheNCVolatilityBreakout.Length; idx++)
                    if (cacheNCVolatilityBreakout[idx].Length == length && cacheNCVolatilityBreakout[idx].EqualsInput(input))
                        return cacheNCVolatilityBreakout[idx];

            lock (checkNCVolatilityBreakout)
            {
                checkNCVolatilityBreakout.Length = length;
                length = checkNCVolatilityBreakout.Length;

                if (cacheNCVolatilityBreakout != null)
                    for (int idx = 0; idx < cacheNCVolatilityBreakout.Length; idx++)
                        if (cacheNCVolatilityBreakout[idx].Length == length && cacheNCVolatilityBreakout[idx].EqualsInput(input))
                            return cacheNCVolatilityBreakout[idx];

                NCVolatilityBreakout indicator = new NCVolatilityBreakout();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                NCVolatilityBreakout[] tmp = new NCVolatilityBreakout[cacheNCVolatilityBreakout == null ? 1 : cacheNCVolatilityBreakout.Length + 1];
                if (cacheNCVolatilityBreakout != null)
                    cacheNCVolatilityBreakout.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheNCVolatilityBreakout = tmp;
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
        /// Dissassembled code
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NCVolatilityBreakout NCVolatilityBreakout(int length)
        {
            return _indicator.NCVolatilityBreakout(Input, length);
        }

        /// <summary>
        /// Dissassembled code
        /// </summary>
        /// <returns></returns>
        public Indicator.NCVolatilityBreakout NCVolatilityBreakout(Data.IDataSeries input, int length)
        {
            return _indicator.NCVolatilityBreakout(input, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Dissassembled code
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NCVolatilityBreakout NCVolatilityBreakout(int length)
        {
            return _indicator.NCVolatilityBreakout(Input, length);
        }

        /// <summary>
        /// Dissassembled code
        /// </summary>
        /// <returns></returns>
        public Indicator.NCVolatilityBreakout NCVolatilityBreakout(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.NCVolatilityBreakout(input, length);
        }
    }
}
#endregion
