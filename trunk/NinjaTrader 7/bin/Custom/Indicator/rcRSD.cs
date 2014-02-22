// Type: NinjaTrader.Indicator.rcRSD
// Assembly: rcRSD, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null
// MVID: 76EF490D-8811-4D13-9DF9-124F0A1DC5E4
// Assembly location: C:\Users\rcromer\Documents\NinjaTrader 7\bin\Custom\rcRSD.dll

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
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Xml.Serialization;

namespace NinjaTrader.Indicator
{
  [Description("Enter the description of your new custom indicator here")]
  public class rcRSD : Indicator
  {
	#region Variables
    private string c04f04f23b8324aa9890b66f92020cca2 = "";
    private bool cbb678590d50e8aad333db21722c4c357 = true;
    private bool recolorRetouch = true;
    private bool ccc375f661d9037d189efe7ef0b91fa94 = true;
    private bool noWeakZones = true;
    private bool drawEdgePrice = true;
    private int number_2 = 2;
    private bool c24bb7194511c35c641cecf85d8a99f85 = true;
    private string alertOnDemandSound = "Alert1.wav";
    private Color supplyStrong = Color.Red;
    private Color supplyWeak = Color.PaleVioletRed;
    private Color supplyRetouch = Color.Coral;
    private Color demandRetouch = Color.Chartreuse;
    private Color demandStrong = Color.Green;
    private Color demandWeak = Color.Olive;
    private Color colorDogerBllue = Color.DodgerBlue;
    private Color colorNavy = Color.Navy;
    private Color colorSeaGreen = Color.SeaGreen;
    private Color colorCrimson = Color.Crimson;
    private Color colorDarkGray = Color.DarkGray;
    private Color colorRed = Color.Red;
    private Color colorDarkSlateGray = Color.DarkSlateGray;
    //private int global_int_v100 = 100;
    //private int global_int_v500 = 500;
    private double[] c747169f75a51d045c253bfc20678f7d0 = new double[4];
    private double[] cdb18564d76ec49197cb5acea61dd5a75 = new double[4];
    private int global_int_v112 = 112;
    private int global_int_v113 = 113;
    //private Font font1 = new Font(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1034), 20f);
    //private Font font2 = new Font(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1061), 12f);
    private Font font3 = new Font("Arial", 8f);
    //private Font font4 = new Font(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1082), 40f);
    private string c8e48df600e2eec7b45e17eae02afeebd = "";
    private List<rcRSD.props> listOfProperties = new List<rcRSD.props>();
    private bool c97d2cedb53c91ff0bd0cf330fc9e63e4 = true;
    [XmlIgnore]
    private Dictionary<string, double> highZones = new Dictionary<string, double>();
    [XmlIgnore]
    private Dictionary<string, double> lowZones = new Dictionary<string, double>();
	private Dictionary<string, int> periodTypeStringToNumberDictionary;
    private int extendMinutes = 60;
    private string alertOnSupplySound = "Alert1.wav";
    private Color textColor = Color.Black;
    private Color supplyZoneBorder = Color.Silver;
    private Color demandZoneBorder = Color.Silver;
    private string myPeriodType = "ChartPeriod";
    //private const int A_BIG_INT = 999999;
    private Image myImage;
    private int timeframePeriod;
    private bool cff627c81c2b2c7d92ceedddb396d8a34;
    private bool c48f0c1747cdba1671630acd164e5479a;
    private bool recolorWeakRetouch;
    private bool showFibonacci = true;	// do fib retracements?
    //private int global_int_2;
    private bool c14f7f326a6c723aa5a14f115c4692513;
    private int supplyDemandBarSeries;
    private DataSeries dataSeries1;
    private DataSeries dataSeries2;
    private DataSeries dataSeries3;
    private string c850d2e969cc14b3e2045165601daa1b5;
    private Color cdab4fdd69c04ef4182a2ced6d43f9a92;
    private double cc431d32139d823e354ffc329f78c46a9;
    private double c18bb8b8c797593b820367034f7a2f763;
    private bool validPeriodType;   // maybe valid var name
    private int global_int_1;
    private int global_int_2;
    private List<rcRSD.RSD_Drawings> list_Of_RSD_Drawings;
    private int global_int_3;
    private DateTime c70b275c21878ae5c45a5206582435006;
    private double c3c4d0cacab89b52c96d1032861422a89;
    private IText iText;
    private int global_int_4;
    private bool shortLogo;
    private bool drawFarEdgePrice;
    private bool drawZoneHeight;
    private bool extendZone;
	#endregion

	#region Properties
	
    [Category("Parameters")]
    [Browsable(false)]
    [XmlIgnore]
    [Description("")]
    public List<double> HighZones
    {
      get { 
        this.Update();
        return new List<double>((IEnumerable<double>) this.highZones.Values);
      }
    }

    [Browsable(false)]
    [Category("Parameters")]
    [XmlIgnore]
    [Description("")]
    public List<double> LowZones
    {
      get
      {
        this.Update();
        return new List<double>((IEnumerable<double>) this.lowZones.Values);
      }
    }

    [GridCategory("Parameters")]
    [Gui.Design.DisplayName("Timeframe Period")]
    [Description("")]
    public int TimeframePeriod
    {
      get { return this.timeframePeriod; }
      set { this.timeframePeriod = Math.Max(0, value); }
    }

    [Category("DrawText")]
    [Gui.Design.DisplayName("MainEdgePrice")]
    [Description("")]
    public bool Draw_edge_price
    {
      get { return this.drawEdgePrice; }
      set { this.drawEdgePrice = value; }
    }

    [Description("")]
    [Category("DrawText")]
    [Gui.Design.DisplayName("FarEdgePrice")]
    public bool DrawFarEdgePrice
    {
      get { return this.drawFarEdgePrice; }
      set { this.drawFarEdgePrice = value; }
    }

    [Description("")]
    [Category("DrawText")]
    [Gui.Design.DisplayName("ZoneHeight")]
    public bool DrawZoneHeight
    {
      get { return this.drawZoneHeight; }
      set { this.drawZoneHeight = value; }
    }

    [Gui.Design.DisplayName("RecolorRetouch")]
    [Description("")]
    [Category("Parameters")]
    public bool Recolor_retouch
    {
      get { return this.recolorRetouch; }
      set { this.recolorRetouch = value; }
    }

    [Gui.Design.DisplayName("RecolorWeakRetouch")]
    [Description("")]
    [Category("Parameters")]
    public bool Recolor_weak_retouch
    {
      get { return this.recolorWeakRetouch; }
      set { this.recolorWeakRetouch = value; }
    }

    [Gui.Design.DisplayName("NoWeakZones")]
    [Description("")]
    [Category("Parameters")]
    public bool No_weak_zones
    {
      get { return this.noWeakZones; }
      set { this.noWeakZones = value; }
    }

    [Gui.Design.DisplayName("ExtendZone")]
    [Description("")]
    [Category("Parameters")]
    public bool ExtendZone
    {
      get { return this.extendZone; }
      set { this.extendZone = value; }
    }

    [Description("")]
    [Category("Parameters")]
    [Gui.Design.DisplayName("ExtendZoneMinutes")]
    public int ExtendMinutes
    {
      get { return this.extendMinutes; }
      set { this.extendMinutes = value; }
    }

    [TypeConverter(typeof (rcRSD.WaveConverter))]
    [Description("")]
    [Category("Alert")]
    [Gui.Design.DisplayName("AlertOnDemandSound")]
    public string AlertOnDemandSound
    {
      get { return this.alertOnDemandSound; }
      set { this.alertOnDemandSound = value; }
    }

    [Gui.Design.DisplayName("AlertOnSupplySound")]
    [Description("")]
    [TypeConverter(typeof (rcRSD.WaveConverter))]
    [Category("Alert")]
    public string AlertOnSupplySound
    {
      get { return this.alertOnSupplySound; }
      set { this.alertOnSupplySound = value; }
    }

    [Category("Colors")]
    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    [Description("")]
    public Color SupplyStrong
    {
      get { return this.supplyStrong; }
      set { this.supplyStrong = value; }
    }

    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    [Category("Colors")]
    [Description("")]
    public Color SupplyWeak
    {
      get { return this.supplyWeak; }
      set { this.supplyWeak = value; }
    }

    [Category("Colors")]
    [Description("")]
    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    public Color SupplyRetouch
    {
      get { return this.supplyRetouch; }
      set { this.supplyRetouch = value; }
    }

    [Category("Colors")]
    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    [Description("")]
    public Color DemandStrong
    {
      get { return this.demandStrong; }
      set { this.demandStrong = value; }
    }

    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    [Category("Colors")]
    [Description("")]
    public Color DemandWeak
    {
      get { return this.demandWeak; }
      set { this.demandWeak = value; }
    }

    [Category("Colors")]
    [Description("")]
    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    public Color DemandRetouch
    {
      get { return this.demandRetouch; }
      set { this.demandRetouch = value; }
    }

    [Category("Colors")]
    [Description("")]
    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    public Color TextColor
    {
      get { return this.textColor; }
      set { this.textColor = value; }
    }

    [Description("")]
    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    [Category("Colors")]
    public Color SupplyZoneBorder
    {
      get { return this.supplyZoneBorder; }
      set { this.supplyZoneBorder = value; }
    }

    [Category("Colors")]
    [XmlElement(Type = typeof (rcRSD.XmlColor))]
    [Description("")]
    public Color DemandZoneBorder
    {
      get { return this.demandZoneBorder; }
      set { this.demandZoneBorder = value; }
    }

    [GridCategory("Parameters")]
    [Gui.Design.DisplayName("TimeframeType")]
    [TypeConverter(typeof (rcRSD.periodConverter))]
    public string MyPeriodType
    {
      get { return this.myPeriodType; }
      set { this.myPeriodType = value; }
    }
	#endregion
	

    protected override void Initialize()
    {
      if (this.myPeriodType != "ChartPeriod")
      {
            if (this.timeframePeriod <= 0)
            {
              this.validPeriodType = false;
              return;
            }
            else
            {
				try {
              supplyDemandBarSeries = 1;
              PeriodType periodType = PeriodType.Minute;
              int periods = timeframePeriod;
				
              if (myPeriodType == "Minute")
              {
                    periodType = PeriodType.Minute;
              }
              else if (myPeriodType == "Hour")
              {
                periodType = PeriodType.Minute;
                periods = timeframePeriod * 60;
              }
              else if (myPeriodType == "Day")
              {
                    periodType = PeriodType.Day;
              }
              else if (myPeriodType == "Week")
			{
                periodType = PeriodType.Week;
			}
              else if (myPeriodType == "Month")
              {
                    periodType = PeriodType.Month;
              }
			
              Add(periodType, periods);	// add new bar series
              validPeriodType = true;
              this.c8e48df600e2eec7b45e17eae02afeebd = this.ca47bf0e564bff4420f70c86585f8a040(this.myPeriodType) + (object) this.timeframePeriod;
				} catch (Exception ex)
				{
					Print("Exception: " + ex.InnerException);
				}
            }
      }
      else
      {
        if (BarsPeriod.Id != PeriodType.Minute)		// PeriodType.Minute = 4
        {
              if (BarsPeriod.Id != PeriodType.Day)		// ? PeriodType.Day = 5
              {
                    if (BarsPeriod.Id != PeriodType.Week)		// ? PeriodType.Week = 6
                    {
                          if (BarsPeriod.Id != PeriodType.Month)			// ? PeriodType.Month = 7
                          {
                                this.validPeriodType = false;
                          }
                    }
              }
        }
        this.c8e48df600e2eec7b45e17eae02afeebd = this.periodTypeToString(BarsPeriod.Id) + (object) BarsPeriod.Value;
        this.validPeriodType = true;
      }

      Overlay = true;
      CalculateOnBarClose = false;
      this.dataSeries1 = new DataSeries((IndicatorBase) this, (MaximumBarsLookBack) 1);
      this.dataSeries2 = new DataSeries((IndicatorBase) this, (MaximumBarsLookBack) 1);
      this.dataSeries3 = new DataSeries((IndicatorBase) this, (MaximumBarsLookBack) 1);
      this.list_Of_RSD_Drawings = new List<rcRSD.RSD_Drawings>();
    }

    protected override void OnStartUp()
    {
      if (!this.validPeriodType)
      {
            this.DrawTextFixed("TimeframeError", "Wrong timeframe setting", (TextPosition) 2);
      }
    }


	// line 802
    protected override void OnBarUpdate()
    {
      if (!this.validPeriodType)
      {
      }
      else
      {
        if (this.supplyDemandBarSeries > 0)
        {
              if (BarsInProgress != this.supplyDemandBarSeries)
                return;
        }
		// switched logic, empty code block - line 835
        if (CurrentBars[0] >= 3)
        {
          int num = CurrentBars[this.supplyDemandBarSeries];
          if (num < 3)
          {
                this.dataSeries2.Set(0.0);
                this.dataSeries3.Set(0.0);
          }
          else
          {
            if (this.global_int_3 != num)
            {
              this.global_int_3 = num;
              this.global_int_1 = 1;
              this.global_int_2 = 1;
            }
            if (CurrentBar <= Bars.Count - 3)
              return;
			
            this.CountZZ(3, 2, 2);
            this.initializeTimeAndDataSeries();
            this.caf6fc3b163d246ab61c6fed47b34a1f4();
			// this doesn't look right - line 879
//            if (!(this.alertOnDemandSound != "Alert1.wav"))
//            {
//              if (!(this.alertOnSupplySound != "Alert1.wav"))
//				this.c38d9d98f35374f31b1d1fc5a2242e326();
//			}
          }
        }
      }
    }

    private void c38d9d98f35374f31b1d1fc5a2242e326()
    {
      rcRSD.RSD_Drawings rsdDrawings_1 = (rcRSD.RSD_Drawings) null;
      rcRSD.RSD_Drawings rsdDrawings_2 = (rcRSD.RSD_Drawings) null;
      for (int index = 0; index < this.list_Of_RSD_Drawings.Count; ++index)
      {
        rcRSD.RSD_Drawings rsdDrawings_3 = this.list_Of_RSD_Drawings[index];
        if (rsdDrawings_3 != null)
        {
              if (rsdDrawings_3.iDrawObject_1 != null)
              {
                    if (rsdDrawings_3.bool_1)
                    {
                      if (rsdDrawings_3.myEnumInstance == myEnum.myEnumerated_2 && rsdDrawings_3.int_2 == this.global_int_1)
                      {
                            rsdDrawings_1 = rsdDrawings_3;
                      }
                      if (rsdDrawings_3.myEnumInstance == myEnum.myEnumerated_3)
                      {
                            if (rsdDrawings_3.int_2 == this.global_int_2)
                            {
                                  rsdDrawings_2 = rsdDrawings_3;
                            }
                      }
                    }
              }
        }
        if (rsdDrawings_1 != null)
        {
              if (rsdDrawings_2 == null)
              {
                    continue;
              }
        }
      }

		if (rsdDrawings_1 != null)
		{
			double endY = ((IRectangle) rsdDrawings_1.iDrawObject_1).EndY;
			if (Closes[0][0] > endY)
			{
				if (endY > 0.0)
				{
					if (this.alertOnSupplySound != "Disabled")
					{
						this.PlaySound(this.alertOnSupplySound);
						++this.global_int_1;
					}
				}
			}
		}
	
		if (rsdDrawings_2 != null)
		{
			if (Closes[0][0] >= ((IRectangle) rsdDrawings_2.iDrawObject_1).EndY)
			{
				if (this.alertOnDemandSound != "Disabled")
				{				
					this.PlaySound(this.alertOnDemandSound);
					++this.global_int_2;
				}      
			}
		}
    }

    protected void CountZZ(int ExtDepth, int ExtDeviation, int ExtBackstep)
    {
      double tickSize = TickSize;
      IDataSeries input1 = Highs[this.supplyDemandBarSeries];
      IDataSeries input2 = Lows[this.supplyDemandBarSeries];
      double num1 = 0.0;
      double num2 = 0.0;
      int num3 = Math.Min(CurrentBars[0], CurrentBars[this.supplyDemandBarSeries]) - ExtDepth;
      for (int index1 = num3; index1 >= 0; --index1)
      {
        double num4 = this.MIN(input2, ExtDepth)[index1];
        if (num4 == num2)
        {
          num4 = 0.0;
        }
        else
        {
          num2 = num4;
          if (input2[index1] - num4 > (double) ExtDeviation * tickSize)
          {
            num4 = 0.0;
          }
          else
          {
            for (int index2 = 1; index2 <= ExtBackstep; ++index2)
            {
              if (index1 + index2 < num3)
              {
                    double num5 = this.dataSeries3[index1 + index2];
                    if (num5 != 0.0 && num5 > num4)
                    {
                          this.dataSeries3.Set(index1 + index2, 0.0);
                    }
              }
            }
          }
        }
        this.dataSeries3.Set(index1, num4);
        double num6 = this.MAX(input1, ExtDepth)[index1];
        if (num6 == num1)
        {
              num6 = 0.0;
        }
        else
        {
          num1 = num6;
          if (num6 - input1[index1] > (double) ExtDeviation * tickSize)
          {
                num6 = 0.0;
          }
          else
          {
            for (int index2 = 1; index2 <= ExtBackstep; ++index2)
            {
              if (index1 + index2 < num3)
              {
                    double num5 = this.dataSeries2[index1 + index2];
                    if (num5 != 0.0)
                    {
                          if (num5 < num6)
                          {
                                this.dataSeries2.Set(index1 + index2, 0.0);
                          }
                    }
              }
            }
          }
        }
        dataSeries2.Set(index1, num6);
      }

		double num7 = -1.0;
		int num8 = -1;
		double num9 = -1.0;
		int num10 = -1;
		for (int index = num3; index >= 0; --index)
		{
            double num4 = this.dataSeries3[index];
            double num5 = this.dataSeries2[index];
            if (num4 == 0.0)
            {
                  if (num5 != 0.0)
                  {
                  }
            }
            if (num5 != 0.0)
            {
                  if (num7 > 0.0)
                  {
                        if (num7 < num5)
                        {
                              this.dataSeries2.Set(num8, 0.0);
                        }
                        else
                        {
                          this.dataSeries2.Set(index, 0.0);
                        }
                  }
                  
                  if (num7 >= num5)
                  {
                        if (num7 < 0.0)
                        {
                        }
                  }
                  num7 = num5;
                  num8 = index;
            }
            if (num4 != 0.0)
            {
                  if (num9 > 0.0)
                  {
                        if (num9 > num4)
                        {
                              this.dataSeries3.Set(num10, 0.0);
                        }
                        else
                        {
                          this.dataSeries3.Set(index, 0.0);
                        }
                  }
					// code doesn't look right
//                  if (num4 >= num9)
//                  {
//                        if (num9 >= 0.0)
//                  }
                  num9 = num4;
                  num10 = index;
                  num7 = -1.0;
            }
        }
      
    }

    private void initializeTimeAndDataSeries()
    {
      IDataSeries idataSeries1 = Highs[this.supplyDemandBarSeries];
      IDataSeries idataSeries2 = Lows[this.supplyDemandBarSeries];
      IDataSeries idataSeries3 = Opens[this.supplyDemandBarSeries];
      IDataSeries idataSeries4 = Closes[this.supplyDemandBarSeries];
      ITimeSeries itimeSeries = Times[this.supplyDemandBarSeries];
      double num1 = 0.0;
      double num2 = 0.0;
      int val1_1 = 0;
      int val2 = 0;
      double num3 = 0.0;
      double num4 = 0.0;
      int num5 = Math.Min(CurrentBars[0], CurrentBars[this.supplyDemandBarSeries]);
      for (int index = 0; index < num5; ++index)
      {
        if (this.dataSeries3[index] > 0.0)
        {
              num1 = this.dataSeries3[index];
              num3 = this.dataSeries3[index];
        }
      }
      for (int index = 0; index < num5; ++index)
      {
        if (this.dataSeries2[index] > 0.0)
        {
          num2 = this.dataSeries2[index];
          num4 = this.dataSeries2[index];
        }
      }

      for (int index = 0; index < num5; ++index)
      {
        if (this.dataSeries2[index] >= num4)
        {
              num4 = this.dataSeries2[index];
              val2 = index;
        }
        else
          this.dataSeries2.Set(index, 0.0);
        if (this.dataSeries2[index] <= num2)
        {
              if (this.dataSeries3[index] > 0.0)
              {
                    this.dataSeries2.Set(index, 0.0);
              }
        }
        if (this.dataSeries3[index] <= num3 && this.dataSeries3[index] > 0.0)
        {
          num3 = this.dataSeries3[index];
          val1_1 = index;
        }
        else
          this.dataSeries3.Set(index, 0.0);
		  
        if (this.dataSeries3[index] > num1)
          this.dataSeries3.Set(index, 0.0);
      }
	  
        double num6 = Math.Min(idataSeries3[val2], idataSeries4[val2]);
        double num7 = Math.Max(idataSeries3[val1_1], idataSeries4[val1_1]);
        for (int index = Math.Max(val1_1, val2); index >= 0; --index)
        {
            if (this.dataSeries2[index] > num6)
            {
                  if (this.dataSeries2[index] != num4)
                  {
                    this.dataSeries2.Set(index, 0.0);
                  }
            }
            if (this.dataSeries2[index] > 0.0)
            {
                  num4 = this.dataSeries2[index];
                  double val1_2 = Math.Min(idataSeries4[index], idataSeries3[index]);
                  if (index > 0)
                  {
                        val1_2 = Math.Max(val1_2, Math.Max(idataSeries2[index - 1], idataSeries2[index + 1]));
                  }
                  if (index > 0)
                    val1_2 = Math.Max(val1_2, Math.Min(idataSeries3[index - 1], idataSeries4[index - 1]));
                  num6 = Math.Max(val1_2, Math.Min(idataSeries3[index + 1], idataSeries4[index + 1]));
            }
			
            if (this.dataSeries3[index] <= num7)
            {
                  if (this.dataSeries3[index] > 0.0)
                  {
                        if (this.dataSeries3[index] != num3)
                        {
                              this.dataSeries3.Set(index, 0.0);
                        }
                  }
            }
            if (this.dataSeries3[index] > 0.0)
            {
                  num3 = this.dataSeries3[index];
                  double val1_2 = Math.Max(idataSeries4[index], idataSeries3[index]);
                  if (index > 0)
                  {
                        val1_2 = Math.Min(val1_2, Math.Min(idataSeries1[index + 1], idataSeries1[index - 1]));
                  }
                  if (index > 0)
                    val1_2 = Math.Min(val1_2, Math.Max(idataSeries3[index - 1], idataSeries4[index - 1]));
                  num7 = Math.Min(val1_2, Math.Max(idataSeries3[index + 1], idataSeries4[index + 1]));
            }
        }
    }

	// from line 1634
    private void caf6fc3b163d246ab61c6fed47b34a1f4()
    {
      int num1 = 0;
      int num2 = 0;
      int index1 = 0;
      int index2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      double num6 = 0.0;
      double num7 = 0.0;
      IDataSeries idataSeries1 = Highs[this.supplyDemandBarSeries];
      IDataSeries idataSeries2 = Lows[this.supplyDemandBarSeries];
      IDataSeries idataSeries3 = Opens[this.supplyDemandBarSeries];
      IDataSeries idataSeries4 = Closes[this.supplyDemandBarSeries];
      ITimeSeries itimeSeries = Times[this.supplyDemandBarSeries];
      int num8 = Math.Min(CurrentBars[0], CurrentBars[this.supplyDemandBarSeries]);
      for (int index3 = 1; index3 < num8 - 1; ++index3)
      {
        int num9 = num8 - index3;
        rcRSD.RSD_Drawings rsdDrawings_1 = (rcRSD.RSD_Drawings) null;
        rcRSD.RSD_Drawings rsdDrawings_2 = (rcRSD.RSD_Drawings) null;
        int index4 = -1;
        int index5 = -1;
        if (this.dataSeries2[index3] == 0.0)
        {
            if (this.dataSeries3[index3] == 0.0)
			{
				for (int index6 = this.list_Of_RSD_Drawings.Count - 1; index6 >= 0; --index6)
				{
					rcRSD.RSD_Drawings rsdDrawings_3 = this.list_Of_RSD_Drawings[index6];
					if (rsdDrawings_3 != null)
					{
						if (rsdDrawings_3.iDrawObject_1 != null && rsdDrawings_3.bool_1)
						{
							if (rsdDrawings_3.int_1 == num9)
							{
								this.RemoveDrawObject(rsdDrawings_3.iDrawObject_1);
								if (rsdDrawings_3.iDrawObject_2 != null)
								{
									if (this.highZones.ContainsKey(rsdDrawings_3.iDrawObject_2.Tag))
									{
										this.highZones.Remove(rsdDrawings_3.iDrawObject_2.Tag);
									}
									if (this.lowZones.ContainsKey(rsdDrawings_3.iDrawObject_2.Tag))
									{
										this.lowZones.Remove(rsdDrawings_3.iDrawObject_2.Tag);
									}
									this.RemoveDrawObject(rsdDrawings_3.iDrawObject_2);
								}
								if (rsdDrawings_3.iDrawObject_3 != null)
								  this.RemoveDrawObject(rsdDrawings_3.iDrawObject_3);
								if (rsdDrawings_3.iText != null)
								{
									this.RemoveDrawObject(rsdDrawings_3.iText);
								}
								this.list_Of_RSD_Drawings.RemoveAt(index6);
								rsdDrawings_3.bool_1 = false;
							}
						}
					}
				}
			}
        }
		// from line 1778
        for (int index6 = 0; index6 < this.list_Of_RSD_Drawings.Count; ++index6)
        {
            rcRSD.RSD_Drawings rsdDrawings_3 = this.list_Of_RSD_Drawings[index6];
            if (rsdDrawings_3 != null && rsdDrawings_3.iDrawObject_1 != null)
            {
                if (rsdDrawings_3.bool_1)
                {
                    if (rsdDrawings_3.int_1 == num9)
                    {
                            if (this.dataSeries2[index3] != 0.0)
                            {
                                  if (rsdDrawings_3.myEnumInstance == myEnum.myEnumerated_2)
                                  {
                                    rsdDrawings_1 = rsdDrawings_3;
                                    index4 = index6;
                                  }
                            }
                            if (this.dataSeries3[index3] != 0.0)
                            {
                                  if (rsdDrawings_3.myEnumInstance == myEnum.myEnumerated_3)
                                  {
                                        rsdDrawings_2 = rsdDrawings_3;
                                        index5 = index6;
                                  }
                            }
							// line 1848
//                            if (rsdDrawings_1 != null)
//                            {
//                                  if (rsdDrawings_2 != null)
//                            }
                    }
                }
            }
        }
		// from line 1882
        if (this.dataSeries2[index3] != 0.0)
        {
              bool flag1 = false;
              bool flag2 = false;
              DateTime dateTime1 = itimeSeries[index3];
              DateTime dateTime2 = Times[0][0];
              double val1 = Math.Min(idataSeries4[index3], idataSeries3[index3]);
              if (index3 > 0)
              {
                    val1 = Math.Max(val1, Math.Max(idataSeries2[index3 - 1], idataSeries2[index3 + 1]));
              }
              if (index3 > 0)
              {
                    val1 = Math.Max(val1, Math.Min(idataSeries3[index3 - 1], idataSeries4[index3 - 1]));
              }
              double num10 = Math.Max(val1, Math.Min(idataSeries3[index3 + 1], idataSeries4[index3 + 1]));
              bool flag3 = true;
              if (!this.recolorRetouch)
              {
                    if (!this.c48f0c1747cdba1671630acd164e5479a)
                    {
                    }
              }
              bool flag4 = false;
              for (int index6 = index3; index6 >= 0; --index6)
              {
                if (index6 == 0)
                {
                      if (!flag4)
                      {
                        flag3 = false;
					  }
                }
                if (!flag4)
                {
                      if (idataSeries1[index6] < num10)
                      {
                        flag4 = true;
                      }
                }
                if (flag4)
                {
                      if (idataSeries1[index6] > num10)
                      {
                            flag1 = true;
                            if (this.showFibonacci)
                            {
                                  if (num1 == 0)
                                  {
                                        num6 = num10;
                                        num1 = index6;
                                  }
                            }
                      }
                }
              }

              if (rsdDrawings_1 != null && index4 >= 0)
              {
                    if (flag3)
                    {
						// line 2047 ?
                          //((IRectangle) rsdDrawings_1.iDrawObject_1).EndY;
                    }
                    else
                    {
                      this.RemoveDrawObject(rsdDrawings_1.iDrawObject_1);
                      if (rsdDrawings_1.iDrawObject_2 != null)
                      {
                            if (this.highZones.ContainsKey(rsdDrawings_1.iDrawObject_2.Tag))
                            {
                                  this.highZones.Remove(rsdDrawings_1.iDrawObject_2.Tag);
                            }
                            if (this.lowZones.ContainsKey(rsdDrawings_1.iDrawObject_2.Tag))
                            {
                                  this.lowZones.Remove(rsdDrawings_1.iDrawObject_2.Tag);
                            }
                            this.RemoveDrawObject(rsdDrawings_1.iDrawObject_2);
                      }
                      if (rsdDrawings_1.iDrawObject_3 != null)
                      {
                            this.RemoveDrawObject(rsdDrawings_1.iDrawObject_3);
                      }
                      if (rsdDrawings_1.iText != null)
                      {
                            this.RemoveDrawObject(rsdDrawings_1.iText);
                      }
                      this.list_Of_RSD_Drawings.RemoveAt(index4);
                      rsdDrawings_1.bool_1 = false;
                      index4 = -1;
                }
              }
              if (this.cbb678590d50e8aad333db21722c4c357)
              {
					// line 2129
                    if (flag3)
                    {
                          flag2 = true;
                          Color color1 = this.supplyStrong;
                          if (this.ccc375f661d9037d189efe7ef0b91fa94)
                          {
                            if (!flag1)
                            {
                                  if (!this.recolorRetouch)
                                  {
								  //goto label_114;
                                  }
                            }
                            int num11 = 0;
                            num3 = 0;
                            for (int index6 = index3; index6 < num8 - 1; ++index6)
                            {
                              if (idataSeries1[index6 + 1] < num10)
                              {
                                    ++num11;
                              }
                              if (idataSeries1[index6 + 1] > this.dataSeries2[index3])
                              {
                                    ++num3;
                              }
                              if (num11 <= 1)
                              {
                                    if (num3 > 1)
                                    {
                                          color1 = this.supplyWeak;
                                          if (this.noWeakZones)
                                          {
                                                flag3 = false;
                                          }
                                    }
                              }
							}
                          }

						  // line 2238
                          if (rsdDrawings_1 != null)
                          {
                                if (index4 >= 0)
                                {
                                      if (!flag3)
                                      {
                                            this.RemoveDrawObject(rsdDrawings_1.iDrawObject_1);
                                            if (rsdDrawings_1.iDrawObject_2 != null)
                                            {
                                                  if (this.highZones.ContainsKey(rsdDrawings_1.iDrawObject_2.Tag))
                                                    this.highZones.Remove(rsdDrawings_1.iDrawObject_2.Tag);
                                                  if (this.lowZones.ContainsKey(rsdDrawings_1.iDrawObject_2.Tag))
                                                    this.lowZones.Remove(rsdDrawings_1.iDrawObject_2.Tag);
                                                  this.RemoveDrawObject(rsdDrawings_1.iDrawObject_2);
                                            }
                                            if (rsdDrawings_1.iDrawObject_3 != null)
                                            {
                                                  this.RemoveDrawObject(rsdDrawings_1.iDrawObject_3);
                                            }
                                            if (rsdDrawings_1.iText != null)
                                              this.RemoveDrawObject(rsdDrawings_1.iText);
                                            this.list_Of_RSD_Drawings.RemoveAt(index4);
                                            rsdDrawings_1.bool_1 = false;
                                      }
                                }
                          }
						  // line 2306
                          if (flag3)
                          {
                                if (this.recolorRetouch)
                                {
                                      if (flag1 && num3 < 2)
                                      {
                                            color1 = this.supplyRetouch;
                                      }
                                }
                                if (this.recolorWeakRetouch)
                                {
                                      if (flag1)
                                      {
                                            if (num3 > 1)
                                            {
                                                  color1 = this.supplyRetouch;
                                            }
                                      }
                                }
                                ++num4;
                                if (num4 == 1)
                                {
                                      this.cc431d32139d823e354ffc329f78c46a9 = num10;
                                }
                                if (this.drawEdgePrice)
                                {
                                  string key = "EdgePrice" + (object) num9;
                                  Color color2 = this.textColor;
                                  if (!this.c97d2cedb53c91ff0bd0cf330fc9e63e4)
                                  {
                                        color1 = Color.Transparent;
                                        color2 = color1;
                                  }
                                  this.c70b275c21878ae5c45a5206582435006 = Times[0][0];
                                  if (this.extendZone)
                                  {
                                        this.c70b275c21878ae5c45a5206582435006 = this.c70b275c21878ae5c45a5206582435006.AddMinutes((double) this.extendMinutes);
                                  }
                                  IText itext1 = this.DrawText(key, false, string.Concat((object) num10), this.c70b275c21878ae5c45a5206582435006, num10, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                  if (rsdDrawings_1 == null)
                                  {
                                        rsdDrawings_1 = new rcRSD.RSD_Drawings();
                                        rsdDrawings_1.myEnumInstance = myEnum.myEnumerated_2;
                                        rsdDrawings_1.color_1 = color1;
                                        rsdDrawings_1.color_2 = color1;
                                        rsdDrawings_1.int_3 = itext1.AreaOpacity;
                                        this.list_Of_RSD_Drawings.Add(rsdDrawings_1);
                                        this.highZones.Add(key, num10);
                                  }
                                  rsdDrawings_1.iDrawObject_2 = (IDrawObject) itext1;
                                  if (this.drawFarEdgePrice)
                                  {
                                    IText itext2 = this.DrawText(key + "drawFarEdgePrice", false, string.Concat((object) this.dataSeries2[index3]), this.c70b275c21878ae5c45a5206582435006, this.dataSeries2[index3], 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                    rsdDrawings_1.iDrawObject_3 = (IDrawObject) itext2;
                                  }
                                  if (this.drawZoneHeight)
                                  {
                                        this.c3c4d0cacab89b52c96d1032861422a89 = (this.dataSeries2[index3] + num10) / 2.0;
                                        this.global_int_4 = Convert.ToInt32(Math.Abs(this.dataSeries2[index3] - num10) / TickSize);
                                        this.iText = this.DrawText(key + "drawZoneHeight", false, string.Concat((object) this.global_int_4), this.c70b275c21878ae5c45a5206582435006, this.c3c4d0cacab89b52c96d1032861422a89, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                        rsdDrawings_1.iText = (IDrawObject) this.iText;
                                  }
                                }
                                string str = "rectTag" + (object) num9;
                                Color color3 = this.supplyZoneBorder;
                                if (!this.c97d2cedb53c91ff0bd0cf330fc9e63e4)
                                {
                                      color1 = Color.Transparent;
                                      color3 = color1;
                                }
								
								try
								{
                                  this.c70b275c21878ae5c45a5206582435006 = dateTime2;
                                  if (this.extendZone)
                                    this.c70b275c21878ae5c45a5206582435006 = this.c70b275c21878ae5c45a5206582435006.AddMinutes((double) this.extendMinutes);
                                  IRectangle irectangle = this.DrawRectangle(str, false, dateTime1, this.dataSeries2[index3], this.c70b275c21878ae5c45a5206582435006, num10, color3, color1, 5);
                                  if (rsdDrawings_1 == null)
                                  {
                                        rsdDrawings_1 = new rcRSD.RSD_Drawings();
                                        rsdDrawings_1.myEnumInstance = myEnum.myEnumerated_2;
                                        this.list_Of_RSD_Drawings.Add(rsdDrawings_1);
                                  }
                                  rsdDrawings_1.color_1 = color1;
                                  rsdDrawings_1.color_2 = this.supplyZoneBorder;
                                  rsdDrawings_1.int_3 = ((IShape) irectangle).AreaOpacity;
                                  rsdDrawings_1.iDrawObject_1 = (IDrawObject) irectangle;
                                  rsdDrawings_1.int_1 = num9;
                                  rsdDrawings_1.int_2 = num4;
								 } 
								 catch (Exception ex)
                                 {
                                    break;
                                 }
                            }	// if flag3 line 2511
                    }
              } // line 2515 if (this.cbb678590d50e8aad333db21722c4c357)
              
              if (flag3 && index1 < 4)
              {
                    if (this.c24bb7194511c35c641cecf85d8a99f85)
                    {
                          if (flag2)
                          {
                                this.c747169f75a51d045c253bfc20678f7d0[index1] = num10;
                                ++index1;
                          }
                    }
              }
        } // if (this.c88e5ec7985d343b005eb3a0b337af79f[index3] != 0.0) line 1882
		// from line 2557
        if (this.dataSeries3[index3] != 0.0)
        {
// label_201:			
              bool flag1 = false;
              bool flag2 = false;
              DateTime dateTime1 = itimeSeries[index3];
              DateTime dateTime2 = Times[0][0];
              double val1 = Math.Max(idataSeries4[index3], idataSeries3[index3]);
              if (index3 > 0)
              {
                    val1 = Math.Min(val1, Math.Min(idataSeries1[index3 + 1], idataSeries1[index3 - 1]));
              }
              if (index3 > 0)
                val1 = Math.Min(val1, Math.Max(idataSeries3[index3 - 1], idataSeries4[index3 - 1]));
              double num10 = Math.Min(val1, Math.Max(idataSeries3[index3 + 1], idataSeries4[index3 + 1]));
              Color color1 = this.demandStrong;
              bool flag3 = true;
              if (!this.recolorRetouch)
              {
                    if (!this.c48f0c1747cdba1671630acd164e5479a)
                    {
                    }
              }
              bool flag4 = false;
              for (int index6 = index3; index6 >= 0; --index6)
              {
                if (index6 == 0)
                {
                      if (!flag4)
                      {
                            flag3 = false;
                      }
                }
                if (!flag4)
                {
                      if (idataSeries2[index6] > num10)
                      {
                            flag4 = true;
                      }
                }
                if (flag4)
                {
                      if (idataSeries2[index6] < num10)
                      {
                            flag1 = true;
                            if (this.showFibonacci)
                            {
                                  if (num2 == 0)
                                  {
                                        num7 = num10;
                                        num2 = index6;
                                  }
                            }
                      }
                }
              }
              if (rsdDrawings_2 != null)
              {
                    if (index5 >= 0)
                    {
                          if (flag3)
                          {
							// not sure about this one
                            //((IRectangle) rsdDrawings_2.iDrawObject_1).EndY;
                          }
                          else
                          {
                            this.RemoveDrawObject(rsdDrawings_2.iDrawObject_1);
                            if (rsdDrawings_2.iDrawObject_2 != null)
                            {
                                  if (this.highZones.ContainsKey(rsdDrawings_2.iDrawObject_2.Tag))
                                  {
                                        this.highZones.Remove(rsdDrawings_2.iDrawObject_2.Tag);
                                  }
                                  if (this.lowZones.ContainsKey(rsdDrawings_2.iDrawObject_2.Tag))
                                    this.lowZones.Remove(rsdDrawings_2.iDrawObject_2.Tag);
                                  this.RemoveDrawObject(rsdDrawings_2.iDrawObject_2);
                            }
                            if (rsdDrawings_2.iDrawObject_3 != null)
                            {
                                  this.RemoveDrawObject(rsdDrawings_2.iDrawObject_3);
                            }
                            if (rsdDrawings_2.iText != null)
                            {
                                  this.RemoveDrawObject(rsdDrawings_2.iText);
                            }
                            this.list_Of_RSD_Drawings.RemoveAt(index5);
                            index5 = -1;
                            rsdDrawings_2.bool_1 = false;
                      }
                    }
              }
              if (this.cbb678590d50e8aad333db21722c4c357)
              {
                    if (flag3)
                    {
                          flag2 = true;
                          if (this.ccc375f661d9037d189efe7ef0b91fa94)
                          {
                                if (!flag1)
                                {
                                      if (!this.recolorRetouch)
                                      {
                                      }
                                }
                                int num11 = 0;
                                num3 = 0;
                                for (int index6 = index3; index6 < num8 - 1; ++index6)
                                {
                                  if (idataSeries2[index6 + 1] > num10)
                                    ++num11;
                                  if (idataSeries2[index6 + 1] < this.dataSeries3[index3])
                                  {
                                        ++num3;
                                  }
                                  if (num11 <= 1)
                                  {
                                        if (num3 > 1)
                                        {
                                              if (this.noWeakZones)
                                                flag3 = false;
                                              color1 = this.demandWeak;
                                        }
                                  }
								}
                          }

                          if (rsdDrawings_2 != null)
                          {

						  if (index5 >= 0)
                                {

								if (!flag3)
                                      {
                                            this.RemoveDrawObject(rsdDrawings_2.iDrawObject_1);
                                            if (rsdDrawings_2.iDrawObject_2 != null)
                                            {
                                              if (this.highZones.ContainsKey(rsdDrawings_2.iDrawObject_2.Tag))
                                                this.highZones.Remove(rsdDrawings_2.iDrawObject_2.Tag);
                                              if (this.lowZones.ContainsKey(rsdDrawings_2.iDrawObject_2.Tag))
                                              {
                                                    this.lowZones.Remove(rsdDrawings_2.iDrawObject_2.Tag);
                                              }
                                              this.RemoveDrawObject(rsdDrawings_2.iDrawObject_2);
                                            }
                                            if (rsdDrawings_2.iDrawObject_3 != null)
                                              this.RemoveDrawObject(rsdDrawings_2.iDrawObject_3);
                                            if (rsdDrawings_2.iText != null)
                                            {
                                                  this.RemoveDrawObject(rsdDrawings_2.iText);
                                            }
                                            this.list_Of_RSD_Drawings.RemoveAt(index5);
                                            rsdDrawings_2.bool_1 = false;
                                      }
                                }
                          }
                          if (flag3)
                          {
                                if (this.recolorRetouch)
                                {
                                      if (flag1 && num3 < 2)
                                      {
                                        color1 = this.demandRetouch;
                                      }
                                }
                                if (this.recolorWeakRetouch)
                                {
                                      if (flag1)
                                      {
                                            if (num3 > 1)
                                            {
                                                  color1 = this.demandRetouch;
                                            }
                                      }
                                }
                                ++num5;
                                if (num5 == 1)
                                {
                                      this.c18bb8b8c797593b820367034f7a2f763 = num10;
                                }
                                if (this.drawEdgePrice)
                                {
                                  string key = "textTag" + (object) num9;
                                  Color color2 = this.textColor;
                                  if (!this.c97d2cedb53c91ff0bd0cf330fc9e63e4)
                                  {
                                        color1 = Color.Transparent;
                                        color2 = color1;
                                  }
                                  this.c70b275c21878ae5c45a5206582435006 = Times[0][0];
                                  if (this.extendZone)
                                    this.c70b275c21878ae5c45a5206582435006 = this.c70b275c21878ae5c45a5206582435006.AddMinutes((double) this.extendMinutes);
                                  IText itext1 = this.DrawText(key, false, string.Concat((object) num10), this.c70b275c21878ae5c45a5206582435006, num10, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                  if (rsdDrawings_2 == null)
                                  {
                                        rsdDrawings_2 = new rcRSD.RSD_Drawings();
                                        rsdDrawings_2.myEnumInstance = myEnum.myEnumerated_3;
                                        rsdDrawings_2.color_1 = color1;
                                        rsdDrawings_2.color_2 = color1;
                                        rsdDrawings_2.int_3 = itext1.AreaOpacity;
                                        this.list_Of_RSD_Drawings.Add(rsdDrawings_2);
                                        this.lowZones.Add(key, num10);
                                  }
                                  rsdDrawings_2.iDrawObject_2 = (IDrawObject) itext1;
                                  if (this.drawFarEdgePrice)
                                  {
                                        IText itext2 = this.DrawText(key + "drawFarEdgePrice", false, string.Concat((object) this.dataSeries3[index3]), this.c70b275c21878ae5c45a5206582435006, this.dataSeries3[index3], 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                        rsdDrawings_2.iDrawObject_3 = (IDrawObject) itext2;
                                  }
                                  if (this.drawZoneHeight)
                                  {
                                        this.c3c4d0cacab89b52c96d1032861422a89 = (this.dataSeries3[index3] + num10) / 2.0;
                                        this.global_int_4 = Convert.ToInt32(Math.Abs(this.dataSeries3[index3] - num10) / TickSize);
                                        this.iText = this.DrawText(key + "drawZoneHeight", false, string.Concat((object) this.global_int_4), this.c70b275c21878ae5c45a5206582435006, this.c3c4d0cacab89b52c96d1032861422a89, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                        rsdDrawings_2.iText = (IDrawObject) this.iText;
                                  }
                                }
                                string str = "rectTag_2" + (object) num9;
                                Color color3 = this.demandZoneBorder;
                                if (!this.c97d2cedb53c91ff0bd0cf330fc9e63e4)
                                {
                                  color1 = Color.Transparent;
                                  color3 = color1;
                                }
                                try
                                {
                                  this.c70b275c21878ae5c45a5206582435006 = dateTime2;
                                  if (this.extendZone)
                                  {
                                        this.c70b275c21878ae5c45a5206582435006 = this.c70b275c21878ae5c45a5206582435006.AddMinutes((double) this.extendMinutes);
                                  }
                                  IRectangle irectangle = this.DrawRectangle(str, false, dateTime1, this.dataSeries3[index3], this.c70b275c21878ae5c45a5206582435006, num10, color3, color1, 5);
                                  if (rsdDrawings_2 == null)
                                  {
                                    rsdDrawings_2 = new rcRSD.RSD_Drawings();
                                    rsdDrawings_2.myEnumInstance = myEnum.myEnumerated_3;
                                    this.list_Of_RSD_Drawings.Add(rsdDrawings_2);
                                  }
                                  rsdDrawings_2.color_1 = color1;
                                  rsdDrawings_2.color_2 = this.demandZoneBorder;
                                  rsdDrawings_2.int_3 = ((IShape) irectangle).AreaOpacity;
                                  rsdDrawings_2.iDrawObject_1 = (IDrawObject) irectangle;
                                  rsdDrawings_2.int_1 = num9;
                                  rsdDrawings_2.int_2 = num5;
                                }
								catch (Exception ex)
								{
									Print(Time + " -- 1400 -- " + ex.Message);
								}
                            }
                          
                    }
              }
              if (flag3)
              {
                    if (index2 < 4)
                    {
                          if (this.c24bb7194511c35c641cecf85d8a99f85)
                          {
                                if (flag2)
                                {
                                      this.cdb18564d76ec49197cb5acea61dd5a75[index2] = num10;
                                      ++index2;
                                }
                          }
                    }
              }
        }	// if (this.dataSeries3[index3] != 0.0) from line 2557
		// line 3222
        if (!Historical)
        {
			// it doesn't look like it does anything with str
              string str = "";
              using (Dictionary<string, double>.ValueCollection.Enumerator enumerator = this.highZones.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  double current = enumerator.Current;
                  str = str + "unknownStr" + current.ToString();
                }
              }
              using (Dictionary<string, double>.ValueCollection.Enumerator enumerator = this.lowZones.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  double current = enumerator.Current;
                  str = str + "unknownStr" + current.ToString();
                }
              }
          
        }
      }  // line 3263 for (int index3 = 1; index3 < num8 - 1; ++index3) line 1651
          int num12 = 0;
          int num13 = 0;
          int num14 = 0;
          int num15 = 0;
          if (!this.showFibonacci)
          {
                if (!this.c24bb7194511c35c641cecf85d8a99f85)
				{
					// do nothing - line 3282
				}
          }
          for (int index3 = 0; index3 < num8; ++index3)
          {
            if (idataSeries1[index3] > num6)
            {
                  if (num13 == 0)
                  {
                        num13 = index3;
                  }
            }
            if (idataSeries1[index3] > this.c747169f75a51d045c253bfc20678f7d0[0])
            {
                  if (num15 == 0)
                  {
                        num15 = index3;
                  }
            }
            if (idataSeries2[index3] < num7)
            {
                  if (num12 == 0)
                  {
                    num12 = index3;
                  }
            }
            if (idataSeries2[index3] < this.cdb18564d76ec49197cb5acea61dd5a75[0] && num14 == 0)
            {
                  num14 = index3;
            }
            if (num13 != 0)
            {
                  if (num15 != 0)
                  {
                        if (num12 != 0)
                        {
                              if (num14 == 0)
                              {
                                    continue;
                              }
                        }
                  }
            }
          }
		  // line 3416
          if (this.showFibonacci)
          {
                double endY;	// endY
                double startY;	// startY
                if (num12 < num13)
                {
                      endY = num7;
                      startY = this.c747169f75a51d045c253bfc20678f7d0[0];
                }
                else
                {
                  endY = num6;
                  startY = this.cdb18564d76ec49197cb5acea61dd5a75[0];
                }
                IFibonacciRetracements ifibonacciRetracements = this.DrawFibonacciRetracements("FibRetrace", false, Times[0][0], startY, Times[0][0], endY);
                ifibonacciRetracements.ExtendRight = true;
                ifibonacciRetracements.ShowText = true;
          }
          if (!this.c24bb7194511c35c641cecf85d8a99f85)
		{	//?????  I added the { }
			
              double num16;
              double num17;
              if (num14 < num15)
              {
                    num16 = this.cdb18564d76ec49197cb5acea61dd5a75[0];
                    num17 = this.c747169f75a51d045c253bfc20678f7d0[0];
                    this.c850d2e969cc14b3e2045165601daa1b5 = ((char) this.global_int_v112).ToString();
                    this.cdab4fdd69c04ef4182a2ced6d43f9a92 = this.colorSeaGreen;
              }
              else
              {
                num16 = this.c747169f75a51d045c253bfc20678f7d0[0];
                num17 = this.cdb18564d76ec49197cb5acea61dd5a75[0];
                this.c850d2e969cc14b3e2045165601daa1b5 = ((char) this.global_int_v113).ToString();
                this.cdab4fdd69c04ef4182a2ced6d43f9a92 = this.colorCrimson;
              }
         } 
      
    }

	// something to do with periodTypes
    private string ca47bf0e564bff4420f70c86585f8a040(string localPeriodType)
    {
      string key1;
      string str;
      if ((key1 = localPeriodType) != null)
      {
        if (periodTypeStringToNumberDictionary == null)
        {
          Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
          string key2 = "Minute";
          int num1 = 0;
          dictionary.Add(key2, num1);
          string key3 = "Hour";
          int num2 = 1;
          dictionary.Add(key3, num2);
          string key4 = "Day";
          int num3 = 2;
          dictionary.Add(key4, num3);
          string key5 = "Week";
          int num4 = 3;
          dictionary.Add(key5, num4);
          string key6 = "Month";
          int num5 = 4;
          dictionary.Add(key6, num5);
          string key7 = "ChartPeriod";
          int num6 = 5;
          dictionary.Add(key7, num6);
          periodTypeStringToNumberDictionary = dictionary;
        }
        int num;
        if (periodTypeStringToNumberDictionary.TryGetValue(key1, out num))
        {
              switch (num)
              {
                case 0:
                  str = "Minute";
                  break;
                case 1:
                  str = "Hour";		// I don't think this is correct, since Hour isn't an option on Period
                  break;
                case 2:
                  str = "Day";
                  break;
                case 3:
                  str = "Week";
                  break;
                case 4:
                  str = "Month";
                  break;
                case 5:
                  str = "";
                  break;
              }
        }
      }
      str = "";
      return str;
    }

    private string periodTypeToString(PeriodType localPeriodType)
    {
      string str;
      switch (((int) localPeriodType) - 4)
      {
        case 0:
          str = "Minute";
          break;
        case 1:
          str = "Day";
          break;
        case 2:
          str = "Week";
          break;
        case 3:
          str = "Month";
          break;
        default:
          str = "";
          break;
      }
      return str;
    }

    protected virtual void OnTermination()
    {
//      if (this.get_ChartControl() == null)
//        return;
//		
//          ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(757)] as ToolStrip;
//          List<ToolStripItem> list = new List<ToolStripItem>();
//          foreach (ToolStripItem toolStripItem in (ArrangedElementCollection) toolStrip.Items)
//          {
//            if (toolStripItem.Name.StartsWith(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1023)))
//            {
//                  list.Add(toolStripItem);
//            }
//          }
//          using (List<ToolStripItem>.Enumerator enumerator = list.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              ToolStripItem current = enumerator.Current;
//              toolStrip.Items.Remove(current);
//            }
//          }
    }

    public virtual void Plot(Graphics g, Rectangle bounds, double min, double max)
    {
      g.DrawImage(this.myImage, 2, bounds.Bottom - this.myImage.Height - 18, this.myImage.Width, this.myImage.Height);
      base.Plot(g, bounds, min, max);
    }

    public class props
    {
      public IDrawObject draw;
      public Color mainColor;
      public Color bordColor;
      public int alpha;

      public props(IDrawObject d, Color m, Color b, int a)
      {
        this.draw = d;
        this.mainColor = m;
        this.bordColor = b;
        this.alpha = a;
      }
    }

    private enum myEnum
    {
      myEnumerated_1,
      myEnumerated_2,
      myEnumerated_3,
    }

    private class RSD_Drawings
    {
      public bool bool_1 = true;
      public IDrawObject iDrawObject_1;
      public IDrawObject iDrawObject_2;
      public IDrawObject iDrawObject_3;
      public IDrawObject iText;
      public int int_1;
      public int int_2;
      public myEnum myEnumInstance;
      public Color color_1;
      public Color color_2;
      public int int_3;
    }

    public class periodConverter : TypeConverter
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
          "ChartPeriod", // cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(627),
          "Minute", // cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(650),
          "Hour", // cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(663),
          "Day", // cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(672),
          "Week", // cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(679),
          "Month"  // cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(688)
        });
      }
    }

    public class WaveConverter : TypeConverter
    {
      private static string[] waveFiles;

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

	  // line 3725
      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        string[] array;
        if (WaveConverter.waveFiles == null)
        {
              string[] files = Directory.GetFiles(Core.InstallDir + "sounds", "*.wav");
              StringCollection stringCollection = new StringCollection();
              for (int index = 0; index < files.Length; ++index)
                stringCollection.Add(Path.GetFileName(files[index]));

				  array = new string[stringCollection.Count + 1];
                  stringCollection.CopyTo(array, 1);
                  array[0] = "Alert1.wav";
                  WaveConverter.waveFiles = array;

              
        }
        else
          array = WaveConverter.waveFiles;
        return new TypeConverter.StandardValuesCollection((ICollection) array);
      }
    }

    public class XmlColor
    {
      private Color c1fedf72583b28720f8114971a173cf52;

      [XmlAttribute]
      public byte Alpha
      {
        get
        {
          return this.c1fedf72583b28720f8114971a173cf52.A;
        }
        set
        {
          if ((int) value == (int) this.c1fedf72583b28720f8114971a173cf52.A)
            return;
              this.c1fedf72583b28720f8114971a173cf52 = Color.FromArgb((int) value, this.c1fedf72583b28720f8114971a173cf52);
        }
      }

      [XmlAttribute]
      public string Web
      {
        get
        {
          return ColorTranslator.ToHtml(this.c1fedf72583b28720f8114971a173cf52);
        }
        set
        {
          try
          {
            if ((int) this.Alpha == (int) byte.MaxValue)
            {
                  this.c1fedf72583b28720f8114971a173cf52 = ColorTranslator.FromHtml(value);
            }
            else
              this.c1fedf72583b28720f8114971a173cf52 = Color.FromArgb((int) this.Alpha, ColorTranslator.FromHtml(value));
          }
          catch (Exception ex)
          {
            this.c1fedf72583b28720f8114971a173cf52 = Color.Black;
          }
        }
      }

      public XmlColor()
      {
        this.c1fedf72583b28720f8114971a173cf52 = Color.Black;
      }

      public XmlColor(Color c)
      {
        this.c1fedf72583b28720f8114971a173cf52 = Color.Black;
        this.c1fedf72583b28720f8114971a173cf52 = c;
      }

      public static implicit operator Color(rcRSD.XmlColor x)
      {
        return x.ToColor();
      }

      public static implicit operator rcRSD.XmlColor(Color c)
      {
        return new rcRSD.XmlColor(c);
      }

      public void FromColor(Color c)
      {
        this.c1fedf72583b28720f8114971a173cf52 = c;
      }

      public bool ShouldSerializeAlpha()
      {
        return (int) this.Alpha < (int) byte.MaxValue;
      }

      public Color ToColor()
      {
        return this.c1fedf72583b28720f8114971a173cf52;
      }
    }
  }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private rcRSD[] cachercRSD = null;

        private static rcRSD checkrcRSD = new rcRSD();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public rcRSD rcRSD(int extendMinutes, bool extendZone, string myPeriodType, bool no_weak_zones, bool recolor_retouch, bool recolor_weak_retouch, int timeframePeriod)
        {
            return rcRSD(Input, extendMinutes, extendZone, myPeriodType, no_weak_zones, recolor_retouch, recolor_weak_retouch, timeframePeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public rcRSD rcRSD(Data.IDataSeries input, int extendMinutes, bool extendZone, string myPeriodType, bool no_weak_zones, bool recolor_retouch, bool recolor_weak_retouch, int timeframePeriod)
        {
            if (cachercRSD != null)
                for (int idx = 0; idx < cachercRSD.Length; idx++)
                    if (cachercRSD[idx].ExtendMinutes == extendMinutes && cachercRSD[idx].ExtendZone == extendZone && cachercRSD[idx].MyPeriodType == myPeriodType && cachercRSD[idx].No_weak_zones == no_weak_zones && cachercRSD[idx].Recolor_retouch == recolor_retouch && cachercRSD[idx].Recolor_weak_retouch == recolor_weak_retouch && cachercRSD[idx].TimeframePeriod == timeframePeriod && cachercRSD[idx].EqualsInput(input))
                        return cachercRSD[idx];

            lock (checkrcRSD)
            {
                checkrcRSD.ExtendMinutes = extendMinutes;
                extendMinutes = checkrcRSD.ExtendMinutes;
                checkrcRSD.ExtendZone = extendZone;
                extendZone = checkrcRSD.ExtendZone;
                checkrcRSD.MyPeriodType = myPeriodType;
                myPeriodType = checkrcRSD.MyPeriodType;
                checkrcRSD.No_weak_zones = no_weak_zones;
                no_weak_zones = checkrcRSD.No_weak_zones;
                checkrcRSD.Recolor_retouch = recolor_retouch;
                recolor_retouch = checkrcRSD.Recolor_retouch;
                checkrcRSD.Recolor_weak_retouch = recolor_weak_retouch;
                recolor_weak_retouch = checkrcRSD.Recolor_weak_retouch;
                checkrcRSD.TimeframePeriod = timeframePeriod;
                timeframePeriod = checkrcRSD.TimeframePeriod;

                if (cachercRSD != null)
                    for (int idx = 0; idx < cachercRSD.Length; idx++)
                        if (cachercRSD[idx].ExtendMinutes == extendMinutes && cachercRSD[idx].ExtendZone == extendZone && cachercRSD[idx].MyPeriodType == myPeriodType && cachercRSD[idx].No_weak_zones == no_weak_zones && cachercRSD[idx].Recolor_retouch == recolor_retouch && cachercRSD[idx].Recolor_weak_retouch == recolor_weak_retouch && cachercRSD[idx].TimeframePeriod == timeframePeriod && cachercRSD[idx].EqualsInput(input))
                            return cachercRSD[idx];

                rcRSD indicator = new rcRSD();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ExtendMinutes = extendMinutes;
                indicator.ExtendZone = extendZone;
                indicator.MyPeriodType = myPeriodType;
                indicator.No_weak_zones = no_weak_zones;
                indicator.Recolor_retouch = recolor_retouch;
                indicator.Recolor_weak_retouch = recolor_weak_retouch;
                indicator.TimeframePeriod = timeframePeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                rcRSD[] tmp = new rcRSD[cachercRSD == null ? 1 : cachercRSD.Length + 1];
                if (cachercRSD != null)
                    cachercRSD.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachercRSD = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcRSD rcRSD(int extendMinutes, bool extendZone, string myPeriodType, bool no_weak_zones, bool recolor_retouch, bool recolor_weak_retouch, int timeframePeriod)
        {
            return _indicator.rcRSD(Input, extendMinutes, extendZone, myPeriodType, no_weak_zones, recolor_retouch, recolor_weak_retouch, timeframePeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.rcRSD rcRSD(Data.IDataSeries input, int extendMinutes, bool extendZone, string myPeriodType, bool no_weak_zones, bool recolor_retouch, bool recolor_weak_retouch, int timeframePeriod)
        {
            return _indicator.rcRSD(input, extendMinutes, extendZone, myPeriodType, no_weak_zones, recolor_retouch, recolor_weak_retouch, timeframePeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcRSD rcRSD(int extendMinutes, bool extendZone, string myPeriodType, bool no_weak_zones, bool recolor_retouch, bool recolor_weak_retouch, int timeframePeriod)
        {
            return _indicator.rcRSD(Input, extendMinutes, extendZone, myPeriodType, no_weak_zones, recolor_retouch, recolor_weak_retouch, timeframePeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.rcRSD rcRSD(Data.IDataSeries input, int extendMinutes, bool extendZone, string myPeriodType, bool no_weak_zones, bool recolor_retouch, bool recolor_weak_retouch, int timeframePeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.rcRSD(input, extendMinutes, extendZone, myPeriodType, no_weak_zones, recolor_retouch, recolor_weak_retouch, timeframePeriod);
        }
    }
}
#endregion
