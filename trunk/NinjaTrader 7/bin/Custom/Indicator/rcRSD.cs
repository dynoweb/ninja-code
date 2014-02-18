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
    private string c04f04f23b8324aa9890b66f92020cca2 = "";
    private bool cbb678590d50e8aad333db21722c4c357 = true;
    private bool recolorRetouch = true;
    private bool ccc375f661d9037d189efe7ef0b91fa94 = true;
    private bool noWeakZones = true;
    private bool drawEdgePrice = true;
    private int number_2 = 2;
    private bool c24bb7194511c35c641cecf85d8a99f85 = true;
    private string alertOnDemandSound = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(901);
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
    private int c9f3136213d8eee8968673aa04e061bfd = 100;
    private int cb1bc769603738a1705086a5160497663 = 500;
    private double[] c747169f75a51d045c253bfc20678f7d0 = new double[4];
    private double[] cdb18564d76ec49197cb5acea61dd5a75 = new double[4];
    private int c6f4cbf3b07e142fd13e2afb457be39d1 = 112;
    private int cf2c2e319a306d42570a68dd163a03bea = 113;
    private Font font1 = new Font(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1034), 20f);
    private Font font2 = new Font(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1061), 12f);
    private Font font3 = new Font(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1061), 8f);
    private Font font4 = new Font(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1082), 40f);
    private string c8e48df600e2eec7b45e17eae02afeebd = "";
    private List<rcRSD.props> listOfProperties = new List<rcRSD.props>();
    private bool c97d2cedb53c91ff0bd0cf330fc9e63e4 = true;
    [XmlIgnore]
    private Dictionary<string, double> highZones = new Dictionary<string, double>();
    [XmlIgnore]
    private Dictionary<string, double> lowZones = new Dictionary<string, double>();
    private int extendMinutes = 60;
    private string alertOnSupplySound = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(901);
    private Color textColor = Color.Black;
    private Color supplyZoneBorder = Color.Silver;
    private Color demandZoneBorder = Color.Silver;
    private string myPeriodType = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(627);
    private const int c4134f3c1f5b723e6eaf24e8f758fedfc = 999999;
    private Image myImage;
    private int _periodValue;
    private bool cff627c81c2b2c7d92ceedddb396d8a34;
    private bool c48f0c1747cdba1671630acd164e5479a;
    private bool recolorWeakRetouch;
    private bool ce2384e40d6da05074ed92eac2372f952;
    private int c2f8fb3b7a36b40b493e767d9ef62a52e;
    private bool c14f7f326a6c723aa5a14f115c4692513;
    private int c8c1734a714810a78b4d22f7dad581c53;
    private DataSeries dataSeries1;
    private DataSeries dataSeries2;
    private DataSeries dataSeries3;
    private string c850d2e969cc14b3e2045165601daa1b5;
    private Color cdab4fdd69c04ef4182a2ced6d43f9a92;
    private double cc431d32139d823e354ffc329f78c46a9;
    private double c18bb8b8c797593b820367034f7a2f763;
    private bool c7a004ada5cc889b2e562160532bbd3cd;
    private int c7e731ab1165e586f24aac409bd5448e7;
    private int cc16f7f79ebca5054a8751af72d4fa952;
    private List<rcRSD.c73d7e688e3f79162747ac07a87d9d0d5> ce98848ac05b3eb8a4526be6ad0a15bd2;
    private int cb3cffe51ffa0a2c5bac5f35f6959306f;
    private DateTime c70b275c21878ae5c45a5206582435006;
    private double c3c4d0cacab89b52c96d1032861422a89;
    private IText c2b61c1ccafe508ae3804f8b1cfe9c2da;
    private int cf71da5f113c2fe6afc3148e31ca45827;
    private bool shortLogo;
    private bool drawFarEdgePrice;
    private bool drawZoneHeight;
    private bool extendZone;

    [Category("\tParameters")]
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
    [Category("\tParameters")]
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

    [Description("")]
    [Category("\tNinjacators")]
    public bool ShortLogo
    {
      get { return this.shortLogo; }
      set { this.shortLogo = value; }
    }

    [GridCategory("\tParameters")]
    [DisplayName("\tTimeframeValue")]
    [Description("")]
    public int periodValue
    {
      get { return this._periodValue; }
      set { this._periodValue = Math.Max(0, value); }
    }

    [Category("DrawText")]
    [DisplayName("MainEdgePrice")]
    [Description("")]
    public bool Draw_edge_price
    {
      get { return this.drawEdgePrice; }
      set { this.drawEdgePrice = value; }
    }

    [Description("")]
    [Category("DrawText")]
    [DisplayName("FarEdgePrice")]
    public bool DrawFarEdgePrice
    {
      get { return this.drawFarEdgePrice; }
      set { this.drawFarEdgePrice = value; }
    }

    [Description("")]
    [Category("DrawText")]
    [DisplayName("ZoneHeight")]
    public bool DrawZoneHeight
    {
      get { return this.drawZoneHeight; }
      set { this.drawZoneHeight = value; }
    }

    [DisplayName("RecolorRetouch")]
    [Description("")]
    [Category("\tParameters")]
    public bool Recolor_retouch
    {
      get { return this.recolorRetouch; }
      set { this.recolorRetouch = value; }
    }

    [DisplayName("RecolorWeakRetouch")]
    [Description("")]
    [Category("\tParameters")]
    public bool Recolor_weak_retouch
    {
      get { return this.recolorWeakRetouch; }
      set { this.recolorWeakRetouch = value; }
    }

    [DisplayName("NoWeakZones")]
    [Description("")]
    [Category("\tParameters")]
    public bool No_weak_zones
    {
      get { return this.noWeakZones; }
      set { this.noWeakZones = value; }
    }

    [DisplayName("ExtendZone")]
    [Description("")]
    [Category("\tParameters")]
    public bool ExtendZone
    {
      get { return this.extendZone; }
      set { this.extendZone = value; }
    }

    [Description("")]
    [Category("\tParameters")]
    [DisplayName("ExtendZoneMinutes")]
    public int ExtendMinutes
    {
      get { return this.extendMinutes; }
      set { this.extendMinutes = value; }
    }

    [TypeConverter(typeof (rcRSD.WaveConverter))]
    [Description("")]
    [Category("Alert")]
    [DisplayName("AlertOnDemandSound")]
    public string AlertOnDemandSound
    {
      get { return this.alertOnDemandSound; }
      set { this.alertOnDemandSound = value; }
    }

    [DisplayName("AlertOnSupplySound")]
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
    public Color 
    {
      get { return this.; }
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

    [GridCategory("\tParameters")]
    [DisplayName("\t\tTimeframeType")]
    [TypeConverter(typeof (rcRSD.periodConverter))]
    public string MyPeriodType
    {
      get { return this.myPeriodType; }
      set { this.myPeriodType = value; }
    }

    public void logoSetup()
    {
      string str1 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240);
      string str2;
      if (this.shortLogo)
      {
            str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(259);
      }
      else
        str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240);
      try
      {
        if (!System.IO.File.Exists(str2))
        {
          WebClient webClient = new WebClient();
          if (this.shortLogo)
            webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(286), str2);
          else
            webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(405), str2);
        }
        this.myImage = Image.FromFile(str2);
      }
      catch (Exception ex)
      {
        this.Print(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(516));
      }
    }

    private void cd7409d0fdab3b2ac0617894a0a49bbf0(object c113fd96257592fe24a96acf1688040b5, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      Process.Start(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(549), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(574));
    }

    protected virtual void Initialize()
    {
      if (this.myPeriodType != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(627))
      {
            if (this._periodValue <= 0)
            {
              this.c7a004ada5cc889b2e562160532bbd3cd = false;
              return;
            }
            else
            {
              this.c8c1734a714810a78b4d22f7dad581c53 = 1;
              PeriodType periodType = (PeriodType) 4;
              int num = this._periodValue;
              if (this.myPeriodType == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(650))
              {
                    periodType = (PeriodType) 4;
              }
              else if (this.myPeriodType == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(663))
              {
                periodType = (PeriodType) 4;
                num = this._periodValue * 60;
              }
              else if (this.myPeriodType == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(672))
              {
                    periodType = (PeriodType) 5;
              }
              else if (this.myPeriodType == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(679))
                periodType = (PeriodType) 6;
              else if (this.myPeriodType == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(688))
              {
                    periodType = (PeriodType) 7;
              }
              this.Add(periodType, num);
              this.c7a004ada5cc889b2e562160532bbd3cd = true;
              this.c8e48df600e2eec7b45e17eae02afeebd = this.ca47bf0e564bff4420f70c86585f8a040(this.myPeriodType) + (object) this._periodValue;
              break;
            }
        }
      }
      else
      {
        if (this.get_BarsPeriod().get_Id() != 4)
        {
              if (this.get_BarsPeriod().get_Id() != 5)
              {
                    if (this.get_BarsPeriod().get_Id() != 6)
                    {
                          if (this.get_BarsPeriod().get_Id() != 7)
                          {
                                this.c7a004ada5cc889b2e562160532bbd3cd = false;
                          }
                          else
                    }
                    else
              }
              else
        }
        this.c8e48df600e2eec7b45e17eae02afeebd = this.c97621f0266292349d47b023891c086d5(this.get_BarsPeriod().get_Id()) + (object) this.get_BarsPeriod().get_Value();
        this.c7a004ada5cc889b2e562160532bbd3cd = true;
      }

      this.set_Overlay(true);
      this.set_CalculateOnBarClose(false);
      this.dataSeries1 = new DataSeries((IndicatorBase) this, (MaximumBarsLookBack) 1);
      this.dataSeries2 = new DataSeries((IndicatorBase) this, (MaximumBarsLookBack) 1);
      this.dataSeries3 = new DataSeries((IndicatorBase) this, (MaximumBarsLookBack) 1);
      this.ce98848ac05b3eb8a4526be6ad0a15bd2 = new List<rcRSD.c73d7e688e3f79162747ac07a87d9d0d5>();
    }

    protected virtual void OnStartUp()
    {
      this.logoSetup();
      if (!this.c7a004ada5cc889b2e562160532bbd3cd)
      {
            this.DrawTextFixed(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(699), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(710), (TextPosition) 2);
      }
      else
      {
        if (this.get_ChartControl() == null)
          return;
	  
		ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(757)] as ToolStrip;
		ToolStripSeparator toolStripSeparator1 = new ToolStripSeparator();
		toolStripSeparator1.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(772);
		ToolStripButton toolStripButton = new ToolStripButton(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(789));
		toolStripButton.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(814);
		toolStripButton.Click += new EventHandler(this.c34d62e2fefbfb9d5838fb95bc422dd2f);
		string str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(837);
		if (this._periodValue != 0)
		{
			  str = this._periodValue.ToString() + str;
		}
		ToolStripLabel toolStripLabel = new ToolStripLabel(str + this.myPeriodType);
		toolStripLabel.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(840);
		toolStripLabel.ForeColor = Color.DarkGray;
		ToolStripSeparator toolStripSeparator2 = new ToolStripSeparator();
		toolStripSeparator2.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(859);
		toolStrip.Items.Add((ToolStripItem) toolStripSeparator1);
		toolStrip.Items.Add((ToolStripItem) toolStripButton);
		toolStrip.Items.Add((ToolStripItem) toolStripSeparator2);
		toolStrip.Items.Add((ToolStripItem) toolStripLabel);
      }
    }

    private void c34d62e2fefbfb9d5838fb95bc422dd2f(object c113fd96257592fe24a96acf1688040b5, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      ToolStripButton toolStripButton = c113fd96257592fe24a96acf1688040b5 as ToolStripButton;
      if (toolStripButton.Text.EndsWith(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(878)))
      {
            toolStripButton.Text = toolStripButton.Text.Replace(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(885), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(894));
            this.c97d2cedb53c91ff0bd0cf330fc9e63e4 = false;
            using (List<rcRSD.c73d7e688e3f79162747ac07a87d9d0d5>.Enumerator enumerator = this.ce98848ac05b3eb8a4526be6ad0a15bd2.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 current = enumerator.Current;
                if (current.cf97ac0bed167fcb539cde5088e6c6961.get_DrawType() == 18)
                {
                  ChartRectangle chartRectangle = current.cf97ac0bed167fcb539cde5088e6c6961 as ChartRectangle;
                  ((ChartShape) chartRectangle).set_AreaOpacity(0);
                  ((ChartShape) chartRectangle).get_Pen().Color = Color.Transparent;
                  ((ChartShape) chartRectangle).set_AreaColor(Color.Transparent);
                }
                if (current.c064a1026d9a70d5b924a69fb41c7db32.get_DrawType() == 23)
                {
                      ChartText chartText = current.c064a1026d9a70d5b924a69fb41c7db32 as ChartText;
                      chartText.set_AreaOpacity(0);
                      chartText.set_TextColor(Color.Transparent);
                      chartText.get_Pen().Color = Color.Transparent;
                      chartText.set_AreaColor(Color.Transparent);
                }
              }
            }
        
		}
		else
		{
			toolStripButton.Text = toolStripButton.Text.Replace(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(894), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(885));
			this.c97d2cedb53c91ff0bd0cf330fc9e63e4 = true;
			using (List<rcRSD.c73d7e688e3f79162747ac07a87d9d0d5>.Enumerator enumerator = this.ce98848ac05b3eb8a4526be6ad0a15bd2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 current = enumerator.Current;
					if (current.cf97ac0bed167fcb539cde5088e6c6961.get_DrawType() == 18)
					{
						  ChartRectangle chartRectangle = current.cf97ac0bed167fcb539cde5088e6c6961 as ChartRectangle;
						  ((ChartShape) chartRectangle).set_AreaOpacity(current.c5ae42acd29515b888cddbd96e6918f00);
						  ((ChartShape) chartRectangle).get_Pen().Color = current.cc19b4e075526200d7cda803485f968fb;
						  ((ChartShape) chartRectangle).set_AreaColor(current.c74092c112ee4cab79306f14f3564db3f);
					}
					if (current.c064a1026d9a70d5b924a69fb41c7db32.get_DrawType() == 23)
					  (current.c064a1026d9a70d5b924a69fb41c7db32 as ChartText).set_TextColor(this.textColor);
				}
			}
		}
      ((Control) this.get_ChartControl()).Refresh();
    }

    protected virtual void OnBarUpdate()
    {
      if (!this.c7a004ada5cc889b2e562160532bbd3cd)
      {
      }
      else
      {
        if (this.c8c1734a714810a78b4d22f7dad581c53 > 0)
        {
              if (this.get_BarsInProgress() != this.c8c1734a714810a78b4d22f7dad581c53)
                return;
        }
        if (this.get_CurrentBars()[0] < 3)
        {
        }
        else
        {
          int num = this.get_CurrentBars()[this.c8c1734a714810a78b4d22f7dad581c53];
          if (num < 3)
          {
                this.dataSeries2.Set(0.0);
                this.dataSeries3.Set(0.0);
                break;
            }
          }
          else
          {
            if (this.cb3cffe51ffa0a2c5bac5f35f6959306f != num)
            {
              this.cb3cffe51ffa0a2c5bac5f35f6959306f = num;
              this.c7e731ab1165e586f24aac409bd5448e7 = 1;
              this.cc16f7f79ebca5054a8751af72d4fa952 = 1;
            }
            if (this.get_CurrentBar() <= this.get_Bars().get_Count() - 3)
              return;
                this.CountZZ(3, 2, 2);
                this.c65bf1e4ae6cc5de46ab39a04d80cac0d();
                this.caf6fc3b163d246ab61c6fed47b34a1f4();
                if (!(this.alertOnDemandSound != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(901)))
                {
                  if (!(this.alertOnSupplySound != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(901)))
					this.c38d9d98f35374f31b1d1fc5a2242e326();
				}
          }
        }
      
    }

    private void c38d9d98f35374f31b1d1fc5a2242e326()
    {
      rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 c73d7e688e3f79162747ac07a87d9d0d5_1 = (rcRSD.c73d7e688e3f79162747ac07a87d9d0d5) null;
      rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 c73d7e688e3f79162747ac07a87d9d0d5_2 = (rcRSD.c73d7e688e3f79162747ac07a87d9d0d5) null;
      for (int index = 0; index < this.ce98848ac05b3eb8a4526be6ad0a15bd2.Count; ++index)
      {
        rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 c73d7e688e3f79162747ac07a87d9d0d5_3 = this.ce98848ac05b3eb8a4526be6ad0a15bd2[index];
        if (c73d7e688e3f79162747ac07a87d9d0d5_3 != null)
        {
              if (c73d7e688e3f79162747ac07a87d9d0d5_3.cf97ac0bed167fcb539cde5088e6c6961 != null)
              {
                    if (c73d7e688e3f79162747ac07a87d9d0d5_3.c5be6d778377fcf6785cd885e6f57fd9a)
                    {
                      if (c73d7e688e3f79162747ac07a87d9d0d5_3.cde3a95a32edd1347240184201e1a8fc4 == rcRSD.cadc1cbe590d50cdf2b50adee29668645.c550219c4ee6a522e23562e31465d2162 && c73d7e688e3f79162747ac07a87d9d0d5_3.c9d17b946231eaa00a035163f1e7972c8 == this.c7e731ab1165e586f24aac409bd5448e7)
                      {
                            c73d7e688e3f79162747ac07a87d9d0d5_1 = c73d7e688e3f79162747ac07a87d9d0d5_3;
                      }
                      if (c73d7e688e3f79162747ac07a87d9d0d5_3.cde3a95a32edd1347240184201e1a8fc4 == rcRSD.cadc1cbe590d50cdf2b50adee29668645.c80b3f0659d533013831363f110f16b30)
                      {
                            if (c73d7e688e3f79162747ac07a87d9d0d5_3.c9d17b946231eaa00a035163f1e7972c8 == this.cc16f7f79ebca5054a8751af72d4fa952)
                            {
                                  c73d7e688e3f79162747ac07a87d9d0d5_2 = c73d7e688e3f79162747ac07a87d9d0d5_3;
                            }
                      }
                    }
              }
        }
        if (c73d7e688e3f79162747ac07a87d9d0d5_1 != null)
        {
              if (c73d7e688e3f79162747ac07a87d9d0d5_2 == null)
              {
                    continue;
              }
        }
      }

      if (c73d7e688e3f79162747ac07a87d9d0d5_1 != null)
      {
            double endY = ((IRectangle) c73d7e688e3f79162747ac07a87d9d0d5_1.cf97ac0bed167fcb539cde5088e6c6961).get_EndY();
            if (this.get_Closes()[0].get_Item(0) > endY)
            {
                  if (endY > 0.0)
                  {
                        if (this.alertOnSupplySound != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(901))
                        {
                          this.PlaySound(this.alertOnSupplySound);
                          ++this.c7e731ab1165e586f24aac409bd5448e7;
                        }
                  }
              }
      }
      if (c73d7e688e3f79162747ac07a87d9d0d5_2 == null)
        return;
          if (this.get_Closes()[0].get_Item(0) >= ((IRectangle) c73d7e688e3f79162747ac07a87d9d0d5_2.cf97ac0bed167fcb539cde5088e6c6961).get_EndY())
              if (!(this.alertOnDemandSound != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(901)))
                return;
                  this.PlaySound(this.alertOnDemandSound);
                  ++this.cc16f7f79ebca5054a8751af72d4fa952;
                  return;
      
    }

    protected void CountZZ(int ExtDepth, int ExtDeviation, int ExtBackstep)
    {
      double tickSize = this.get_TickSize();
      IDataSeries input1 = this.get_Highs()[this.c8c1734a714810a78b4d22f7dad581c53];
      IDataSeries input2 = this.get_Lows()[this.c8c1734a714810a78b4d22f7dad581c53];
      double num1 = 0.0;
      double num2 = 0.0;
      int num3 = Math.Min(this.get_CurrentBars()[0], this.get_CurrentBars()[this.c8c1734a714810a78b4d22f7dad581c53]) - ExtDepth;
      for (int index1 = num3; index1 >= 0; --index1)
      {
        double num4 = this.MIN(input2, ExtDepth).get_Item(index1);
        if (num4 == num2)
        {
          num4 = 0.0;
        }
        else
        {
          num2 = num4;
          if (input2.get_Item(index1) - num4 > (double) ExtDeviation * tickSize)
          {
            num4 = 0.0;
          }
          else
          {
            for (int index2 = 1; index2 <= ExtBackstep; ++index2)
            {
              if (index1 + index2 < num3)
              {
                    double num5 = this.dataSeries3.get_Item(index1 + index2);
                    if (num5 != 0.0 && num5 > num4)
                    {
                          this.dataSeries3.Set(index1 + index2, 0.0);
                    }
              }
            }
          }
        }
        this.dataSeries3.Set(index1, num4);
        double num6 = this.MAX(input1, ExtDepth).get_Item(index1);
        if (num6 == num1)
        {
              num6 = 0.0;
        }
        else
        {
          num1 = num6;
          if (num6 - input1.get_Item(index1) > (double) ExtDeviation * tickSize)
          {
                num6 = 0.0;
          }
          else
          {
            for (int index2 = 1; index2 <= ExtBackstep; ++index2)
            {
              if (index1 + index2 < num3)
              {
                    double num5 = this.dataSeries2.get_Item(index1 + index2);
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
        this.dataSeries2.set_Item(index1, num6);
      }

	  double num7 = -1.0;
          int num8 = -1;
          double num9 = -1.0;
          int num10 = -1;
          for (int index = num3; index >= 0; --index)
          {
            double num4 = this.dataSeries3.get_Item(index);
            double num5 = this.dataSeries2.get_Item(index);
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
                        else
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
                  if (num4 >= num9)
                  {
                        if (num9 >= 0.0)
                  }
                  num9 = num4;
                  num10 = index;
                  num7 = -1.0;
            }
        }
      
    }

    private void c65bf1e4ae6cc5de46ab39a04d80cac0d()
    {
      IDataSeries idataSeries1 = this.get_Highs()[this.c8c1734a714810a78b4d22f7dad581c53];
      IDataSeries idataSeries2 = this.get_Lows()[this.c8c1734a714810a78b4d22f7dad581c53];
      IDataSeries idataSeries3 = this.get_Opens()[this.c8c1734a714810a78b4d22f7dad581c53];
      IDataSeries idataSeries4 = this.get_Closes()[this.c8c1734a714810a78b4d22f7dad581c53];
      ITimeSeries itimeSeries = this.get_Times()[this.c8c1734a714810a78b4d22f7dad581c53];
      double num1 = 0.0;
      double num2 = 0.0;
      int val1_1 = 0;
      int val2 = 0;
      double num3 = 0.0;
      double num4 = 0.0;
      int num5 = Math.Min(this.get_CurrentBars()[0], this.get_CurrentBars()[this.c8c1734a714810a78b4d22f7dad581c53]);
      for (int index = 0; index < num5; ++index)
      {
        if (this.dataSeries3.get_Item(index) > 0.0)
        {
              num1 = this.dataSeries3.get_Item(index);
              num3 = this.dataSeries3.get_Item(index);
        }
      }
      for (int index = 0; index < num5; ++index)
      {
        if (this.dataSeries2.get_Item(index) > 0.0)
        {
          num2 = this.dataSeries2.get_Item(index);
          num4 = this.dataSeries2.get_Item(index);
        }
      }

      for (int index = 0; index < num5; ++index)
      {
        if (this.dataSeries2.get_Item(index) >= num4)
        {
              num4 = this.dataSeries2.get_Item(index);
              val2 = index;
        }
        else
          this.dataSeries2.Set(index, 0.0);
        if (this.dataSeries2.get_Item(index) <= num2)
        {
              if (this.dataSeries3.get_Item(index) > 0.0)
              {
                    this.dataSeries2.Set(index, 0.0);
              }
        }
        if (this.dataSeries3.get_Item(index) <= num3 && this.dataSeries3.get_Item(index) > 0.0)
        {
          num3 = this.dataSeries3.get_Item(index);
          val1_1 = index;
        }
        else
          this.dataSeries3.Set(index, 0.0);
		  
        if (this.dataSeries3.get_Item(index) > num1)
          this.dataSeries3.set_Item(index, 0.0);
      }
	  
        double num6 = Math.Min(idataSeries3.get_Item(val2), idataSeries4.get_Item(val2));
        double num7 = Math.Max(idataSeries3.get_Item(val1_1), idataSeries4.get_Item(val1_1));
        for (int index = Math.Max(val1_1, val2); index >= 0; --index)
        {
            if (this.dataSeries2.get_Item(index) > num6)
            {
                  if (this.dataSeries2.get_Item(index) != num4)
                  {
                    this.dataSeries2.Set(index, 0.0);
                    goto label_46;
                  }
            }
            if (this.dataSeries2.get_Item(index) > 0.0)
            {
                  num4 = this.dataSeries2.get_Item(index);
                  double val1_2 = Math.Min(idataSeries4.get_Item(index), idataSeries3.get_Item(index));
                  if (index > 0)
                  {
                        val1_2 = Math.Max(val1_2, Math.Max(idataSeries2.get_Item(index - 1), idataSeries2.get_Item(index + 1)));
                  }
                  if (index > 0)
                    val1_2 = Math.Max(val1_2, Math.Min(idataSeries3.get_Item(index - 1), idataSeries4.get_Item(index - 1)));
                  num6 = Math.Max(val1_2, Math.Min(idataSeries3.get_Item(index + 1), idataSeries4.get_Item(index + 1)));
            }
			
            if (this.dataSeries3.get_Item(index) <= num7)
            {
                  if (this.dataSeries3.get_Item(index) > 0.0)
                  {
                        if (this.dataSeries3.get_Item(index) != num3)
                        {
                              this.dataSeries3.Set(index, 0.0);
                        }
                  }
            }
            if (this.dataSeries3.get_Item(index) > 0.0)
            {
                  num3 = this.dataSeries3.get_Item(index);
                  double val1_2 = Math.Max(idataSeries4.get_Item(index), idataSeries3.get_Item(index));
                  if (index > 0)
                  {
                        val1_2 = Math.Min(val1_2, Math.Min(idataSeries1.get_Item(index + 1), idataSeries1.get_Item(index - 1)));
                  }
                  if (index > 0)
                    val1_2 = Math.Min(val1_2, Math.Max(idataSeries3.get_Item(index - 1), idataSeries4.get_Item(index - 1)));
                  num7 = Math.Min(val1_2, Math.Max(idataSeries3.get_Item(index + 1), idataSeries4.get_Item(index + 1)));
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
      IDataSeries idataSeries1 = this.get_Highs()[this.c8c1734a714810a78b4d22f7dad581c53];
      IDataSeries idataSeries2 = this.get_Lows()[this.c8c1734a714810a78b4d22f7dad581c53];
      IDataSeries idataSeries3 = this.get_Opens()[this.c8c1734a714810a78b4d22f7dad581c53];
      IDataSeries idataSeries4 = this.get_Closes()[this.c8c1734a714810a78b4d22f7dad581c53];
      ITimeSeries itimeSeries = this.get_Times()[this.c8c1734a714810a78b4d22f7dad581c53];
      int num8 = Math.Min(this.get_CurrentBars()[0], this.get_CurrentBars()[this.c8c1734a714810a78b4d22f7dad581c53]);
      for (int index3 = 1; index3 < num8 - 1; ++index3)
      {
        int num9 = num8 - index3;
        rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 c73d7e688e3f79162747ac07a87d9d0d5_1 = (rcRSD.c73d7e688e3f79162747ac07a87d9d0d5) null;
        rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 c73d7e688e3f79162747ac07a87d9d0d5_2 = (rcRSD.c73d7e688e3f79162747ac07a87d9d0d5) null;
        int index4 = -1;
        int index5 = -1;
        if (this.dataSeries2.get_Item(index3) == 0.0)
        {
            if (this.dataSeries3.get_Item(index3) == 0.0)
			{
				for (int index6 = this.ce98848ac05b3eb8a4526be6ad0a15bd2.Count - 1; index6 >= 0; --index6)
				{
					rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 c73d7e688e3f79162747ac07a87d9d0d5_3 = this.ce98848ac05b3eb8a4526be6ad0a15bd2[index6];
					if (c73d7e688e3f79162747ac07a87d9d0d5_3 != null)
					{
						if (c73d7e688e3f79162747ac07a87d9d0d5_3.cf97ac0bed167fcb539cde5088e6c6961 != null && c73d7e688e3f79162747ac07a87d9d0d5_3.c5be6d778377fcf6785cd885e6f57fd9a)
						{
							if (c73d7e688e3f79162747ac07a87d9d0d5_3.cc7e06f1bdf7afedb4899f1fe08f4a254 == num9)
							{
								this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_3.cf97ac0bed167fcb539cde5088e6c6961);
								if (c73d7e688e3f79162747ac07a87d9d0d5_3.c064a1026d9a70d5b924a69fb41c7db32 != null)
								{
									if (this.highZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_3.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
									{
										this.highZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_3.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
									}
									if (this.lowZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_3.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
									{
										this.lowZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_3.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
									}
									this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_3.c064a1026d9a70d5b924a69fb41c7db32);
								}
								if (c73d7e688e3f79162747ac07a87d9d0d5_3.ce2dced7ddb532f032178192718ea35cf != null)
								  this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_3.ce2dced7ddb532f032178192718ea35cf);
								if (c73d7e688e3f79162747ac07a87d9d0d5_3.c2b61c1ccafe508ae3804f8b1cfe9c2da != null)
								{
									this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_3.c2b61c1ccafe508ae3804f8b1cfe9c2da);
								}
								this.ce98848ac05b3eb8a4526be6ad0a15bd2.RemoveAt(index6);
								c73d7e688e3f79162747ac07a87d9d0d5_3.c5be6d778377fcf6785cd885e6f57fd9a = false;
							}
						}
					}
				}
			}
        }
		// from line 1778
        for (int index6 = 0; index6 < this.ce98848ac05b3eb8a4526be6ad0a15bd2.Count; ++index6)
        {
            rcRSD.c73d7e688e3f79162747ac07a87d9d0d5 c73d7e688e3f79162747ac07a87d9d0d5_3 = this.ce98848ac05b3eb8a4526be6ad0a15bd2[index6];
            if (c73d7e688e3f79162747ac07a87d9d0d5_3 != null && c73d7e688e3f79162747ac07a87d9d0d5_3.cf97ac0bed167fcb539cde5088e6c6961 != null)
            {
                if (c73d7e688e3f79162747ac07a87d9d0d5_3.c5be6d778377fcf6785cd885e6f57fd9a)
                {
                    if (c73d7e688e3f79162747ac07a87d9d0d5_3.cc7e06f1bdf7afedb4899f1fe08f4a254 == num9)
                    {
                            if (this.dataSeries2.get_Item(index3) != 0.0)
                            {
                                  if (c73d7e688e3f79162747ac07a87d9d0d5_3.cde3a95a32edd1347240184201e1a8fc4 == rcRSD.cadc1cbe590d50cdf2b50adee29668645.c550219c4ee6a522e23562e31465d2162)
                                  {
                                    c73d7e688e3f79162747ac07a87d9d0d5_1 = c73d7e688e3f79162747ac07a87d9d0d5_3;
                                    index4 = index6;
                                  }
                            }
                            if (this.dataSeries3.get_Item(index3) != 0.0)
                            {
                                  if (c73d7e688e3f79162747ac07a87d9d0d5_3.cde3a95a32edd1347240184201e1a8fc4 == rcRSD.cadc1cbe590d50cdf2b50adee29668645.c80b3f0659d533013831363f110f16b30)
                                  {
                                        c73d7e688e3f79162747ac07a87d9d0d5_2 = c73d7e688e3f79162747ac07a87d9d0d5_3;
                                        index5 = index6;
                                  }
                            }
                            if (c73d7e688e3f79162747ac07a87d9d0d5_1 != null)
                            {
                                  if (c73d7e688e3f79162747ac07a87d9d0d5_2 != null)
                            }
                    }
                }
            }
        }
		// from line 1882
        if (this.dataSeries2.get_Item(index3) != 0.0)
        {
              bool flag1 = false;
              bool flag2 = false;
              DateTime dateTime1 = itimeSeries.get_Item(index3);
              DateTime dateTime2 = this.get_Times()[0].get_Item(0);
              double val1 = Math.Min(idataSeries4.get_Item(index3), idataSeries3.get_Item(index3));
              if (index3 > 0)
              {
                    val1 = Math.Max(val1, Math.Max(idataSeries2.get_Item(index3 - 1), idataSeries2.get_Item(index3 + 1)));
              }
              if (index3 > 0)
              {
                    val1 = Math.Max(val1, Math.Min(idataSeries3.get_Item(index3 - 1), idataSeries4.get_Item(index3 - 1)));
              }
              double num10 = Math.Max(val1, Math.Min(idataSeries3.get_Item(index3 + 1), idataSeries4.get_Item(index3 + 1)));
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
                      if (idataSeries1.get_Item(index6) < num10)
                      {
                        flag4 = true;
                      }
                }
                if (flag4)
                {
                      if (idataSeries1.get_Item(index6) > num10)
                      {
                            flag1 = true;
                            if (this.ce2384e40d6da05074ed92eac2372f952)
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

              if (c73d7e688e3f79162747ac07a87d9d0d5_1 != null && index4 >= 0)
              {
                    if (flag3)
                    {
                          ((IRectangle) c73d7e688e3f79162747ac07a87d9d0d5_1.cf97ac0bed167fcb539cde5088e6c6961).get_EndY();
                    }
                    else
                    {
                      this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.cf97ac0bed167fcb539cde5088e6c6961);
                      if (c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32 != null)
                      {
                            if (this.highZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                            {
                                  this.highZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                            }
                            if (this.lowZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                            {
                                  this.lowZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                            }
                            this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32);
                      }
                      if (c73d7e688e3f79162747ac07a87d9d0d5_1.ce2dced7ddb532f032178192718ea35cf != null)
                      {
                            this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.ce2dced7ddb532f032178192718ea35cf);
                      }
                      if (c73d7e688e3f79162747ac07a87d9d0d5_1.c2b61c1ccafe508ae3804f8b1cfe9c2da != null)
                      {
                            this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.c2b61c1ccafe508ae3804f8b1cfe9c2da);
                      }
                      this.ce98848ac05b3eb8a4526be6ad0a15bd2.RemoveAt(index4);
                      c73d7e688e3f79162747ac07a87d9d0d5_1.c5be6d778377fcf6785cd885e6f57fd9a = false;
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
                              if (idataSeries1.get_Item(index6 + 1) < num10)
                              {
                                    ++num11;
                              }
                              if (idataSeries1.get_Item(index6 + 1) > this.dataSeries2.get_Item(index3))
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
                          if (c73d7e688e3f79162747ac07a87d9d0d5_1 != null)
                          {
                                if (index4 >= 0)
                                {
                                      if (!flag3)
                                      {
                                            this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.cf97ac0bed167fcb539cde5088e6c6961);
                                            if (c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32 != null)
                                            {
                                                  if (this.highZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                                                    this.highZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                                                  if (this.lowZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                                                    this.lowZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                                                  this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32);
                                            }
                                            if (c73d7e688e3f79162747ac07a87d9d0d5_1.ce2dced7ddb532f032178192718ea35cf != null)
                                            {
                                                  this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.ce2dced7ddb532f032178192718ea35cf);
                                            }
                                            if (c73d7e688e3f79162747ac07a87d9d0d5_1.c2b61c1ccafe508ae3804f8b1cfe9c2da != null)
                                              this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_1.c2b61c1ccafe508ae3804f8b1cfe9c2da);
                                            this.ce98848ac05b3eb8a4526be6ad0a15bd2.RemoveAt(index4);
                                            c73d7e688e3f79162747ac07a87d9d0d5_1.c5be6d778377fcf6785cd885e6f57fd9a = false;
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
                                  string key = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(918) + (object) num9;
                                  Color color2 = this.textColor;
                                  if (!this.c97d2cedb53c91ff0bd0cf330fc9e63e4)
                                  {
                                        color1 = Color.Transparent;
                                        color2 = color1;
                                  }
                                  this.c70b275c21878ae5c45a5206582435006 = this.get_Times()[0].get_Item(0);
                                  if (this.extendZone)
                                  {
                                        this.c70b275c21878ae5c45a5206582435006 = this.c70b275c21878ae5c45a5206582435006.AddMinutes((double) this.extendMinutes);
                                  }
                                  IText itext1 = this.DrawText(key, false, string.Concat((object) num10), this.c70b275c21878ae5c45a5206582435006, num10, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                  if (c73d7e688e3f79162747ac07a87d9d0d5_1 == null)
                                  {
                                        c73d7e688e3f79162747ac07a87d9d0d5_1 = new rcRSD.c73d7e688e3f79162747ac07a87d9d0d5();
                                        c73d7e688e3f79162747ac07a87d9d0d5_1.cde3a95a32edd1347240184201e1a8fc4 = rcRSD.cadc1cbe590d50cdf2b50adee29668645.c550219c4ee6a522e23562e31465d2162;
                                        c73d7e688e3f79162747ac07a87d9d0d5_1.c74092c112ee4cab79306f14f3564db3f = color1;
                                        c73d7e688e3f79162747ac07a87d9d0d5_1.cc19b4e075526200d7cda803485f968fb = color1;
                                        c73d7e688e3f79162747ac07a87d9d0d5_1.c5ae42acd29515b888cddbd96e6918f00 = itext1.get_AreaOpacity();
                                        this.ce98848ac05b3eb8a4526be6ad0a15bd2.Add(c73d7e688e3f79162747ac07a87d9d0d5_1);
                                        this.highZones.Add(key, num10);
                                  }
                                  c73d7e688e3f79162747ac07a87d9d0d5_1.c064a1026d9a70d5b924a69fb41c7db32 = (IDrawObject) itext1;
                                  if (this.drawFarEdgePrice)
                                  {
                                    IText itext2 = this.DrawText(key + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(935), false, string.Concat((object) this.dataSeries2.get_Item(index3)), this.c70b275c21878ae5c45a5206582435006, this.dataSeries2.get_Item(index3), 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                    c73d7e688e3f79162747ac07a87d9d0d5_1.ce2dced7ddb532f032178192718ea35cf = (IDrawObject) itext2;
                                  }
                                  if (this.drawZoneHeight)
                                  {
                                        this.c3c4d0cacab89b52c96d1032861422a89 = (this.dataSeries2.get_Item(index3) + num10) / 2.0;
                                        this.cf71da5f113c2fe6afc3148e31ca45827 = Convert.ToInt32(Math.Abs(this.dataSeries2.get_Item(index3) - num10) / this.get_TickSize());
                                        this.c2b61c1ccafe508ae3804f8b1cfe9c2da = this.DrawText(key + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(938), false, string.Concat((object) this.cf71da5f113c2fe6afc3148e31ca45827), this.c70b275c21878ae5c45a5206582435006, this.c3c4d0cacab89b52c96d1032861422a89, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                        c73d7e688e3f79162747ac07a87d9d0d5_1.c2b61c1ccafe508ae3804f8b1cfe9c2da = (IDrawObject) this.c2b61c1ccafe508ae3804f8b1cfe9c2da;
                                  }
                                }
                                string str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(947) + (object) num9;
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
                                  IRectangle irectangle = this.DrawRectangle(str, false, dateTime1, this.dataSeries2.get_Item(index3), this.c70b275c21878ae5c45a5206582435006, num10, color3, color1, 5);
                                  if (c73d7e688e3f79162747ac07a87d9d0d5_1 == null)
                                  {
                                        c73d7e688e3f79162747ac07a87d9d0d5_1 = new rcRSD.c73d7e688e3f79162747ac07a87d9d0d5();
                                        c73d7e688e3f79162747ac07a87d9d0d5_1.cde3a95a32edd1347240184201e1a8fc4 = rcRSD.cadc1cbe590d50cdf2b50adee29668645.c550219c4ee6a522e23562e31465d2162;
                                        this.ce98848ac05b3eb8a4526be6ad0a15bd2.Add(c73d7e688e3f79162747ac07a87d9d0d5_1);
                                  }
                                  c73d7e688e3f79162747ac07a87d9d0d5_1.c74092c112ee4cab79306f14f3564db3f = color1;
                                  c73d7e688e3f79162747ac07a87d9d0d5_1.cc19b4e075526200d7cda803485f968fb = this.supplyZoneBorder;
                                  c73d7e688e3f79162747ac07a87d9d0d5_1.c5ae42acd29515b888cddbd96e6918f00 = ((IShape) irectangle).get_AreaOpacity();
                                  c73d7e688e3f79162747ac07a87d9d0d5_1.cf97ac0bed167fcb539cde5088e6c6961 = (IDrawObject) irectangle;
                                  c73d7e688e3f79162747ac07a87d9d0d5_1.cc7e06f1bdf7afedb4899f1fe08f4a254 = num9;
                                  c73d7e688e3f79162747ac07a87d9d0d5_1.c9d17b946231eaa00a035163f1e7972c8 = num4;
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
        } // if (this.c88e5ec7985d343b005eb3a0b337af79f.get_Item(index3) != 0.0) line 1882
		// from line 2557
        if (this.dataSeries3.get_Item(index3) != 0.0)
        {
// label_201:			
              bool flag1 = false;
              bool flag2 = false;
              DateTime dateTime1 = itimeSeries.get_Item(index3);
              DateTime dateTime2 = this.get_Times()[0].get_Item(0);
              double val1 = Math.Max(idataSeries4.get_Item(index3), idataSeries3.get_Item(index3));
              if (index3 > 0)
              {
                    val1 = Math.Min(val1, Math.Min(idataSeries1.get_Item(index3 + 1), idataSeries1.get_Item(index3 - 1)));
              }
              if (index3 > 0)
                val1 = Math.Min(val1, Math.Max(idataSeries3.get_Item(index3 - 1), idataSeries4.get_Item(index3 - 1)));
              double num10 = Math.Min(val1, Math.Max(idataSeries3.get_Item(index3 + 1), idataSeries4.get_Item(index3 + 1)));
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
                    default:
                      if (!flag4)
                      {
                            flag3 = false;
                      }
                }
                if (!flag4)
                {
                      if (idataSeries2.get_Item(index6) > num10)
                      {
                            flag4 = true;
                      }
                }
                if (flag4)
                {
                      if (idataSeries2.get_Item(index6) < num10)
                      {
                            flag1 = true;
                            if (this.ce2384e40d6da05074ed92eac2372f952)
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
              if (c73d7e688e3f79162747ac07a87d9d0d5_2 != null)
              {
                  default:
                    if (index5 >= 0)
                    {
                          if (flag3)
                          {
                            ((IRectangle) c73d7e688e3f79162747ac07a87d9d0d5_2.cf97ac0bed167fcb539cde5088e6c6961).get_EndY();
                          }
                          else
                          {
                            this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.cf97ac0bed167fcb539cde5088e6c6961);
                            if (c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32 != null)
                            {
                                  if (this.highZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                                  {
                                        this.highZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                                  }
                                  if (this.lowZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                                    this.lowZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                                  this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32);
                            }
                            if (c73d7e688e3f79162747ac07a87d9d0d5_2.ce2dced7ddb532f032178192718ea35cf != null)
                            {
                                  this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.ce2dced7ddb532f032178192718ea35cf);
                            }
                            if (c73d7e688e3f79162747ac07a87d9d0d5_2.c2b61c1ccafe508ae3804f8b1cfe9c2da != null)
                            {
                                  this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.c2b61c1ccafe508ae3804f8b1cfe9c2da);
                            }
                            this.ce98848ac05b3eb8a4526be6ad0a15bd2.RemoveAt(index5);
                            index5 = -1;
                            c73d7e688e3f79162747ac07a87d9d0d5_2.c5be6d778377fcf6785cd885e6f57fd9a = false;
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
                                  if (idataSeries2.get_Item(index6 + 1) > num10)
                                    ++num11;
                                  if (idataSeries2.get_Item(index6 + 1) < this.dataSeries3.get_Item(index3))
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

                          if (c73d7e688e3f79162747ac07a87d9d0d5_2 != null)
                          {

						  if (index5 >= 0)
                                {

								if (!flag3)
                                      {
                                            this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.cf97ac0bed167fcb539cde5088e6c6961);
                                            if (c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32 != null)
                                            {
                                              if (this.highZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                                                this.highZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                                              if (this.lowZones.ContainsKey(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag()))
                                              {
                                                    this.lowZones.Remove(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32.get_Tag());
                                              }
                                              this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32);
                                            }
                                            if (c73d7e688e3f79162747ac07a87d9d0d5_2.ce2dced7ddb532f032178192718ea35cf != null)
                                              this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.ce2dced7ddb532f032178192718ea35cf);
                                            if (c73d7e688e3f79162747ac07a87d9d0d5_2.c2b61c1ccafe508ae3804f8b1cfe9c2da != null)
                                            {
                                                  this.RemoveDrawObject(c73d7e688e3f79162747ac07a87d9d0d5_2.c2b61c1ccafe508ae3804f8b1cfe9c2da);
                                            }
                                            this.ce98848ac05b3eb8a4526be6ad0a15bd2.RemoveAt(index5);
                                            c73d7e688e3f79162747ac07a87d9d0d5_2.c5be6d778377fcf6785cd885e6f57fd9a = false;
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
                                            else
                                      }
                                }
                                ++num5;
                                if (num5 == 1)
                                {
                                      this.c18bb8b8c797593b820367034f7a2f763 = num10;
                                }
                                if (this.drawEdgePrice)
                                {
                                  string key = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(962) + (object) num9;
                                  Color color2 = this.textColor;
                                  if (!this.c97d2cedb53c91ff0bd0cf330fc9e63e4)
                                  {
                                        color1 = Color.Transparent;
                                        color2 = color1;
                                  }
                                  this.c70b275c21878ae5c45a5206582435006 = this.get_Times()[0].get_Item(0);
                                  if (this.extendZone)
                                    this.c70b275c21878ae5c45a5206582435006 = this.c70b275c21878ae5c45a5206582435006.AddMinutes((double) this.extendMinutes);
                                  IText itext1 = this.DrawText(key, false, string.Concat((object) num10), this.c70b275c21878ae5c45a5206582435006, num10, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                  if (c73d7e688e3f79162747ac07a87d9d0d5_2 == null)
                                  {
                                        c73d7e688e3f79162747ac07a87d9d0d5_2 = new rcRSD.c73d7e688e3f79162747ac07a87d9d0d5();
                                        c73d7e688e3f79162747ac07a87d9d0d5_2.cde3a95a32edd1347240184201e1a8fc4 = rcRSD.cadc1cbe590d50cdf2b50adee29668645.c80b3f0659d533013831363f110f16b30;
                                        c73d7e688e3f79162747ac07a87d9d0d5_2.c74092c112ee4cab79306f14f3564db3f = color1;
                                        c73d7e688e3f79162747ac07a87d9d0d5_2.cc19b4e075526200d7cda803485f968fb = color1;
                                        c73d7e688e3f79162747ac07a87d9d0d5_2.c5ae42acd29515b888cddbd96e6918f00 = itext1.get_AreaOpacity();
                                        this.ce98848ac05b3eb8a4526be6ad0a15bd2.Add(c73d7e688e3f79162747ac07a87d9d0d5_2);
                                        this.lowZones.Add(key, num10);
                                  }
                                  c73d7e688e3f79162747ac07a87d9d0d5_2.c064a1026d9a70d5b924a69fb41c7db32 = (IDrawObject) itext1;
                                  if (this.drawFarEdgePrice)
                                  {
                                        IText itext2 = this.DrawText(key + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(935), false, string.Concat((object) this.dataSeries3.get_Item(index3)), this.c70b275c21878ae5c45a5206582435006, this.dataSeries3.get_Item(index3), 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                        c73d7e688e3f79162747ac07a87d9d0d5_2.ce2dced7ddb532f032178192718ea35cf = (IDrawObject) itext2;
                                  }
                                  if (this.drawZoneHeight)
                                  {
                                        this.c3c4d0cacab89b52c96d1032861422a89 = (this.dataSeries3.get_Item(index3) + num10) / 2.0;
                                        this.cf71da5f113c2fe6afc3148e31ca45827 = Convert.ToInt32(Math.Abs(this.dataSeries3.get_Item(index3) - num10) / this.get_TickSize());
                                        this.c2b61c1ccafe508ae3804f8b1cfe9c2da = this.DrawText(key + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(938), false, string.Concat((object) this.cf71da5f113c2fe6afc3148e31ca45827), this.c70b275c21878ae5c45a5206582435006, this.c3c4d0cacab89b52c96d1032861422a89, 0, color2, this.font3, StringAlignment.Near, Color.Transparent, Color.Transparent, 0);
                                        c73d7e688e3f79162747ac07a87d9d0d5_2.c2b61c1ccafe508ae3804f8b1cfe9c2da = (IDrawObject) this.c2b61c1ccafe508ae3804f8b1cfe9c2da;
                                  }
                                }
                                string str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(979) + (object) num9;
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
                                  IRectangle irectangle = this.DrawRectangle(str, false, dateTime1, this.dataSeries3.get_Item(index3), this.c70b275c21878ae5c45a5206582435006, num10, color3, color1, 5);
                                  if (c73d7e688e3f79162747ac07a87d9d0d5_2 == null)
                                  {
                                    c73d7e688e3f79162747ac07a87d9d0d5_2 = new rcRSD.c73d7e688e3f79162747ac07a87d9d0d5();
                                    c73d7e688e3f79162747ac07a87d9d0d5_2.cde3a95a32edd1347240184201e1a8fc4 = rcRSD.cadc1cbe590d50cdf2b50adee29668645.c80b3f0659d533013831363f110f16b30;
                                    this.ce98848ac05b3eb8a4526be6ad0a15bd2.Add(c73d7e688e3f79162747ac07a87d9d0d5_2);
                                  }
                                  c73d7e688e3f79162747ac07a87d9d0d5_2.c74092c112ee4cab79306f14f3564db3f = color1;
                                  c73d7e688e3f79162747ac07a87d9d0d5_2.cc19b4e075526200d7cda803485f968fb = this.demandZoneBorder;
                                  c73d7e688e3f79162747ac07a87d9d0d5_2.c5ae42acd29515b888cddbd96e6918f00 = ((IShape) irectangle).get_AreaOpacity();
                                  c73d7e688e3f79162747ac07a87d9d0d5_2.cf97ac0bed167fcb539cde5088e6c6961 = (IDrawObject) irectangle;
                                  c73d7e688e3f79162747ac07a87d9d0d5_2.cc7e06f1bdf7afedb4899f1fe08f4a254 = num9;
                                  c73d7e688e3f79162747ac07a87d9d0d5_2.c9d17b946231eaa00a035163f1e7972c8 = num5;
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
        }	// if (this.dataSeries3.get_Item(index3) != 0.0) from line 2557
		// line 3222
        if (!this.get_Historical())
        {
              string str = "";
              using (Dictionary<string, double>.ValueCollection.Enumerator enumerator = this.highZones.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  double current = enumerator.Current;
                  str = str + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(994) + current.ToString();
                }
              }
              using (Dictionary<string, double>.ValueCollection.Enumerator enumerator = this.lowZones.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  double current = enumerator.Current;
                  str = str + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(994) + current.ToString();
                }
              }
          
        }
      }  // line 3263 for (int index3 = 1; index3 < num8 - 1; ++index3) line 1651
          int num12 = 0;
          int num13 = 0;
          int num14 = 0;
          int num15 = 0;
          if (!this.ce2384e40d6da05074ed92eac2372f952)
          {
                if (!this.c24bb7194511c35c641cecf85d8a99f85)
          }
          for (int index3 = 0; index3 < num8; ++index3)
          {
            if (idataSeries1.get_Item(index3) > num6)
            {
                  if (num13 == 0)
                  {
                        num13 = index3;
                  }
            }
            if (idataSeries1.get_Item(index3) > this.c747169f75a51d045c253bfc20678f7d0[0])
            {
                  if (num15 == 0)
                  {
                        num15 = index3;
                  }
            }
            if (idataSeries2.get_Item(index3) < num7)
            {
                  if (num12 == 0)
                  {
                    num12 = index3;
                  }
            }
            if (idataSeries2.get_Item(index3) < this.cdb18564d76ec49197cb5acea61dd5a75[0] && num14 == 0)
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
          if (this.ce2384e40d6da05074ed92eac2372f952)
          {
                double num9;
                double num10;
                if (num12 < num13)
                {
                      num9 = num7;
                      num10 = this.c747169f75a51d045c253bfc20678f7d0[0];
                }
                else
                {
                  num9 = num6;
                  num10 = this.cdb18564d76ec49197cb5acea61dd5a75[0];
                }
                IFibonacciRetracements ifibonacciRetracements = this.DrawFibonacciRetracements(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(997), false, this.get_Times()[0].get_Item(0), num10, this.get_Times()[0].get_Item(0), num9);
                ifibonacciRetracements.set_ExtendRight(true);
                ifibonacciRetracements.set_ShowText(true);
          }
          if (!this.c24bb7194511c35c641cecf85d8a99f85)
			break;
              double num16;
              double num17;
              if (num14 < num15)
              {
                    num16 = this.cdb18564d76ec49197cb5acea61dd5a75[0];
                    num17 = this.c747169f75a51d045c253bfc20678f7d0[0];
                    this.c850d2e969cc14b3e2045165601daa1b5 = ((char) this.c6f4cbf3b07e142fd13e2afb457be39d1).ToString();
                    this.cdab4fdd69c04ef4182a2ced6d43f9a92 = this.colorSeaGreen;
              }
              else
              {
                num16 = this.c747169f75a51d045c253bfc20678f7d0[0];
                num17 = this.cdb18564d76ec49197cb5acea61dd5a75[0];
                this.c850d2e969cc14b3e2045165601daa1b5 = ((char) this.cf2c2e319a306d42570a68dd163a03bea).ToString();
                this.cdab4fdd69c04ef4182a2ced6d43f9a92 = this.colorCrimson;
                return;
              }
          
      
    }

    private string ca47bf0e564bff4420f70c86585f8a040(string c28d164b35276c984a6d18e95b394415a)
    {
      string key1;
      string str;
      if ((key1 = c28d164b35276c984a6d18e95b394415a) != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (c084497bfbb8c31ec060910a94dfb4cf4.cd430f7ce3dc110608f59482c5cedaa4c == null)
        {
          Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
          string key2 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(650);
          int num1 = 0;
          // ISSUE: explicit non-virtual call
          __nonvirtual (dictionary.Add(key2, num1));
          string key3 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(663);
          int num2 = 1;
          // ISSUE: explicit non-virtual call
          __nonvirtual (dictionary.Add(key3, num2));
          string key4 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(672);
          int num3 = 2;
          // ISSUE: explicit non-virtual call
          __nonvirtual (dictionary.Add(key4, num3));
          string key5 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(679);
          int num4 = 3;
          // ISSUE: explicit non-virtual call
          __nonvirtual (dictionary.Add(key5, num4));
          string key6 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(688);
          int num5 = 4;
          // ISSUE: explicit non-virtual call
          __nonvirtual (dictionary.Add(key6, num5));
          string key7 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(627);
          int num6 = 5;
          // ISSUE: explicit non-virtual call
          __nonvirtual (dictionary.Add(key7, num6));
          // ISSUE: reference to a compiler-generated field
          c084497bfbb8c31ec060910a94dfb4cf4.cd430f7ce3dc110608f59482c5cedaa4c = dictionary;
        }
        int num;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: explicit non-virtual call
        if (__nonvirtual (c084497bfbb8c31ec060910a94dfb4cf4.cd430f7ce3dc110608f59482c5cedaa4c.TryGetValue(key1, out num)))
        {
label_4:
          switch (7)
          {
            case 0:
              goto label_4;
            default:
              if (1 == 0)
              {
                // ISSUE: method reference
                RuntimeMethodHandle runtimeMethodHandle = __methodref (rcRSD.ca47bf0e564bff4420f70c86585f8a040);
              }
              switch (num)
              {
                case 0:
                  str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1006);
                  goto label_15;
                case 1:
                  str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1009);
                  goto label_15;
                case 2:
                  str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1012);
                  goto label_15;
                case 3:
                  str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1015);
                  goto label_15;
                case 4:
                  str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1018);
                  goto label_15;
                case 5:
                  str = "";
                  goto label_15;
              }
          }
        }
      }
      str = "";
label_15:
      return str;
    }

    private string c97621f0266292349d47b023891c086d5(PeriodType c28d164b35276c984a6d18e95b394415a)
    {
      string str;
      switch (c28d164b35276c984a6d18e95b394415a - 4)
      {
        case 0:
          str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1006);
          break;
        case 1:
          str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1012);
          break;
        case 2:
          str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1015);
          break;
        case 3:
          str = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1018);
          break;
        default:
          str = "";
          break;
      }
      return str;
    }

    protected virtual void OnTermination()
    {
      if (this.get_ChartControl() == null)
        return;
label_1:
      switch (2)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
            // ISSUE: method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcRSD.OnTermination);
          }
          ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(757)] as ToolStrip;
          List<ToolStripItem> list = new List<ToolStripItem>();
          foreach (ToolStripItem toolStripItem in (ArrangedElementCollection) toolStrip.Items)
          {
            if (toolStripItem.Name.StartsWith(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1023)))
            {
label_7:
              switch (6)
              {
                case 0:
                  goto label_7;
                default:
                  list.Add(toolStripItem);
                  continue;
              }
            }
          }
          using (List<ToolStripItem>.Enumerator enumerator = list.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ToolStripItem current = enumerator.Current;
              toolStrip.Items.Remove(current);
            }
            break;
          }
      }
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

    private enum cadc1cbe590d50cdf2b50adee29668645
    {
      cb4d7c60ce8ce73fa8ade64e895631781,
      c550219c4ee6a522e23562e31465d2162,
      c80b3f0659d533013831363f110f16b30,
    }

    private class c73d7e688e3f79162747ac07a87d9d0d5
    {
      public bool c5be6d778377fcf6785cd885e6f57fd9a = true;
      public IDrawObject cf97ac0bed167fcb539cde5088e6c6961;
      public IDrawObject c064a1026d9a70d5b924a69fb41c7db32;
      public IDrawObject ce2dced7ddb532f032178192718ea35cf;
      public IDrawObject c2b61c1ccafe508ae3804f8b1cfe9c2da;
      public int cc7e06f1bdf7afedb4899f1fe08f4a254;
      public int c9d17b946231eaa00a035163f1e7972c8;
      public rcRSD.cadc1cbe590d50cdf2b50adee29668645 cde3a95a32edd1347240184201e1a8fc4;
      public Color c74092c112ee4cab79306f14f3564db3f;
      public Color cc19b4e075526200d7cda803485f968fb;
      public int c5ae42acd29515b888cddbd96e6918f00;
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
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(627),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(650),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(663),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(672),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(679),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(688)
        });
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

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        string[] array;
        if (rcRSD.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d == null)
        {
label_1:
          switch (4)
          {
            case 0:
              goto label_1;
            default:
              if (1 == 0)
              {
                // ISSUE: method reference
                RuntimeMethodHandle runtimeMethodHandle = __methodref (rcRSD.WaveConverter.GetStandardValues);
              }
              string[] files = Directory.GetFiles(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1105), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1124));
              StringCollection stringCollection = new StringCollection();
              for (int index = 0; index < files.Length; ++index)
                stringCollection.Add(Path.GetFileName(files[index]));
label_7:
              switch (2)
              {
                case 0:
                  goto label_7;
                default:
                  array = new string[stringCollection.Count + 1];
                  stringCollection.CopyTo(array, 1);
                  array[0] = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(901);
                  rcRSD.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d = array;
                  break;
              }
          }
        }
        else
          array = rcRSD.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d;
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
label_1:
          switch (2)
          {
            case 0:
              goto label_1;
            default:
              if (1 == 0)
              {
                RuntimeMethodHandle runtimeMethodHandle = __methodref (rcRSD.XmlColor.set_Alpha);
              }
              this.c1fedf72583b28720f8114971a173cf52 = Color.FromArgb((int) value, this.c1fedf72583b28720f8114971a173cf52);
              break;
          }
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
label_1:
              switch (6)
              {
                case 0:
                  goto label_1;
                default:
                  if (1 == 0)
                  {
                    RuntimeMethodHandle runtimeMethodHandle = __methodref (rcRSD.XmlColor.set_Web);
                  }
                  this.c1fedf72583b28720f8114971a173cf52 = ColorTranslator.FromHtml(value);
                  break;
              }
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
