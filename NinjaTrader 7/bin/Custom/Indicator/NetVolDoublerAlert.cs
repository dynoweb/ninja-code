#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

namespace NinjaTrader.Indicator
{    
	/// <summary>
    /// Gom Delta Volume
    /// </summary>
    [Description("Gom Delta Divergence")]
	public class NetVolDoublerAlert : GomDeltaIndicator
	{
		#region Variables

		int totalvolume, upvolume, downvolume, deltavolume;
		
		private double	multiplier 			= 2.0;
		private int		highVolume			= 1000;
		
		//Text Alert
		private bool	textWarnings		= false;
		private Font 	textFont			= new Font("Arial", 10f, FontStyle.Bold);
		private int 	textOffset			= 4;

		private Color 	textColor1			= Color.Black;
		private Color 	textColor2			= Color.Black;
				
		private Color 	outlineColor1		= Color.DarkBlue;
		private Color 	outlineColor2		= Color.DarkRed;
				
		private Color 	areaColor1			= Color.Blue;
		private Color 	areaColor2			= Color.Red;
				
		private int 	zopacity1			= 3;
		private int 	zopacity2			= 3;
		
		//Sound Alert
		private bool	soundAlert			= true;
		private string	downFile			= "Alert3.wav";
		private string	upFile				= "Alert4.wav";
		private int		rearmTime			= 151;
		
		//Alert Box
		private bool 	alertBox			= true;
		private 		TextPosition tpos	= TextPosition.Center;
		private Font 	textFont1			= new Font("Arial", 15f, FontStyle.Regular);
		
		private Color 	textColor3			= Color.Black;
		private Color 	textColor4			= Color.Black;
		
		private Color 	outlineColor3		= Color.DarkBlue;
		private Color 	outlineColor4		= Color.DarkRed;
		
		private Color 	areaColor3			= Color.Blue;
		private Color 	areaColor4			= Color.Red;
		
		private int 	zopacity3			= 3;
		private int 	zopacity4			= 3;
		
		#endregion

		protected override void GomInitialize()
		{
			Add(new Plot(new Pen(Color.Transparent, 3), PlotStyle.Bar, "UpVolume"));
			Add(new Plot(new Pen(Color.Transparent, 3), PlotStyle.Bar, "DownVolume"));
			Add(new Plot(new Pen(Color.Transparent, 3), PlotStyle.Bar, "TotalVolume"));

			Overlay 				= true;
			PriceTypeSupported 		= false;
			DrawOnPricePanel 		= true;
			PlotsConfigurable		= false;
		}

		void PlotChart()
		{
			if (CurrentBar < 1)
				return;
			
			if (deltavolume > 0)
			{
				UpVolume.Set(deltavolume);
				DownVolume.Set(0);
								
				if (textWarnings)
				{
					if (Values[0][0] >= (multiplier * Values[0][1]) && Values[0][0] >= HighVolume && Values[0][1] > 0)
					
						DrawText("Delta" + CurrentBar, true, Convert.ToString("!X!"), 0, Low[0] - textOffset*TickSize, 0, textColor1, 
    					textFont, StringAlignment.Center, outlineColor1, areaColor1, zopacity1);
						
					else
						DrawText("Delta" + CurrentBar, true, Convert.ToString("NEUTRAL"), 0, Low[0] - textOffset*TickSize, 0, Color.Transparent,
						textFont, StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
				}
				
				if (alertBox)
				{
					if (Values[0][0] >= (multiplier * Values[0][1]) && Values[0][0] >= HighVolume && Values[0][1] > 0)
					
						DrawTextFixed("AlertBox", (Instrument.MasterInstrument.Name) + Convert.ToString(Values[0][0]), tpos, textColor3, textFont1, outlineColor3, areaColor3, zopacity3);
						
					else
						DrawTextFixed("AlertBox", "Alert", tpos, Color.Transparent, textFont1, Color.Transparent, Color.Transparent, 0);
				}
				
				if (SoundAlert)
				{
					if (Values[0][0] >= (multiplier * Values[0][1]) && Values[0][0] >= HighVolume && Values[0][1] > 0)
					{
						try
						{
							Alert("Delta",NinjaTrader.Cbi.Priority.Medium,"DeltaX UP " + Bars.Period.Value + " " + Values[0] + " " + CurrentBar,upFile,rearmTime,Color.Navy,Color.Magenta);
						}
							catch {}
					}
				}
			}
			
			else if (deltavolume < 0)
			{
				UpVolume.Set(0);
				DownVolume.Set(-deltavolume);
				
				if (textWarnings)
				{
					if (Values[1][0] >= (multiplier * Values[1][1]) && Values[1][0] >= HighVolume && Values[1][1] > 0)
					
						DrawText("Delta" + CurrentBar, true, Convert.ToString("!X!"), 0, High[0] + textOffset*TickSize, 0, textColor2, 
    					textFont, StringAlignment.Center, outlineColor2, areaColor2, zopacity2);
						
					else
						DrawText("Delta" + CurrentBar, true, Convert.ToString("NEUTRAL"), 0, High[0] + textOffset*TickSize, 0, Color.Transparent,
						textFont, StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
				}
				
				if (AlertBox)
				{
					if (Values[1][0] >= (multiplier * Values[1][1]) && Values[1][0] >= HighVolume && Values[1][1] > 0)
					
						DrawTextFixed("AlertBox", (Instrument.MasterInstrument.Name) + Convert.ToString(Values[1][0]), tpos, textColor4, textFont1, outlineColor4, areaColor4, zopacity4);
						
					else
						DrawTextFixed("AlertBox", "Alert", tpos, Color.Transparent, textFont1, Color.Transparent, Color.Transparent, 0);
				}
				
				if (SoundAlert)
				{
					if (Values[1][0] >= (multiplier * Values[1][1]) && Values[1][0] >= HighVolume && Values[1][1] > 0)
					{
						try
						{
							Alert("Delta",NinjaTrader.Cbi.Priority.Medium,"DeltaX DOWN " + Bars.Period.Value + " " + Values[0] + " " + CurrentBar,downFile,rearmTime,Color.Magenta,Color.Navy);
						}
							catch {}
					}
				}
			}
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void GomOnBarUpdate()
		{
			if (FirstTickOfBar)
			{
				totalvolume = 0;
				upvolume = 0;
				downvolume = 0;
				deltavolume = 0;
			}
		}

		protected override void GomOnMarketData(Gom.MarketDataType e)
		{
			int delta = CalcDelta(e);

			totalvolume += e.Volume;

			if (delta > 0)
				upvolume += delta;
			if (delta < 0)
				downvolume += delta; 

			deltavolume += delta;
		}

		protected override void GomOnBarUpdateDone()
		{
			PlotChart();
		}

		#region Properties
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries TotalVolume
		{
			get { return Values[2]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries UpVolume
		{
			get { return Values[0]; }
		}

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries DownVolume
		{
			get { return Values[1]; }
		}
		
		[Description("volume threshold")]
        [Category("0.0 Parameters")]
		[Gui.Design.DisplayNameAttribute("Threshold")]
        public int HighVolume
        {
            get { return highVolume; }
            set { highVolume = Math.Max(1, value); }
        }
		
		[Description("volume multiplier")]
		[Category("0.0 Parameters")]
		[Gui.Design.DisplayNameAttribute("Multiplier previous bar")]
		public double Multiplier
		{
			get { return multiplier; }
			set { multiplier = Math.Max(1, value); }
		}

		//Text Alert
		[Description("Text marker")]
		[Gui.Design.DisplayName ("01.Show Text")]
        [Category("2.0 Text")]
        public bool TextWarnings
        {
            get { return textWarnings; }
            set { textWarnings = value; }
        }
		
		[XmlIgnore()]
		[Description("Text Font")]
        [Category("2.0 Text")]
        [Gui.Design.DisplayNameAttribute("02.Text Font")]
        public Font TextFont
        {
            get { return textFont; }
            set { textFont = value; }
        }
		
		[Browsable(false)]
        public string textFontSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableFont.ToString(textFont); }
            set { textFont = NinjaTrader.Gui.Design.SerializableFont.FromString(value); }
        }
		
		[Description("Text offset in ticks")]
		[Gui.Design.DisplayName("03.Text Offset")]
        [Category("2.0 Text")]
		public int TextOffset
        {
            get { return textOffset; }
            set {textOffset = Math.Max(0, value); }
        }		
						
		// Text Negative
		[XmlIgnore]
        [Description("Text Color")]
        [Category("2.2 Text Nagative")]
        [Gui.Design.DisplayNameAttribute("01.Text Color")]
        public Color TextColor2
        {
            get { return textColor2; }
            set { textColor2 = value; }
        }
		
		[Browsable(false)]
        public string TextColor2Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(textColor2); }
            set { textColor2 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Area Color")]
        [Category("2.2 Text Nagative")]
        [Gui.Design.DisplayNameAttribute("02.Text Box Area Color")]
        public Color AreaColor2
        {
            get { return areaColor2; }
            set { areaColor2 = value; }
        }

        [Browsable(false)]
        public string AreaColor2Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(areaColor2); }
            set { areaColor2 = Gui.Design.SerializableColor.FromString(value); }
        }        
		
		[XmlIgnore]
        [Description("Text Box Outline Color")]
        [Category("2.2 Text Nagative")]
        [Gui.Design.DisplayNameAttribute("03.Text Box Outline Color")]
        public Color OutlineColor2
        {
            get { return outlineColor2; }
            set { outlineColor2 = value; }
        }

        [Browsable(false)]
        public string OutlineColor2Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(outlineColor2); }
            set { outlineColor2 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[Description("Zone Opacity")]
		[Category("2.2 Text Nagative")]
		[Gui.Design.DisplayNameAttribute("04.Text Box Opacity")]
		public int Zopacity2
		{
			get { return zopacity2; }
			set { zopacity2 = value; }
		}
		
		// Text Positive
		[XmlIgnore]
        [Description("Text Color")]
        [Category("2.1 Text Positive")]
        [Gui.Design.DisplayNameAttribute("01.Text Color")]
        public Color TextColor1
        {
            get { return textColor1; }
            set { textColor1 = value; }
        }
		
		[Browsable(false)]
        public string TextColor1Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(textColor1); }
            set { textColor1 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Area Color")]
        [Category("2.1 Text Positive")]
        [Gui.Design.DisplayNameAttribute("02.Text Box Area Color")]
        public Color AreaColor1
        {
            get { return areaColor1; }
            set { areaColor1 = value; }
        }

        [Browsable(false)]
        public string AreaColor1Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(areaColor1); }
            set { areaColor1 = Gui.Design.SerializableColor.FromString(value); }
        }        
		
		[XmlIgnore]
        [Description("Text Box Outline Color")]
        [Category("2.1 Text Positive")]
        [Gui.Design.DisplayNameAttribute("03.Text Box Outline Color")]
        public Color OutlineColor1
        {
            get { return outlineColor1; }
            set { outlineColor1 = value; }
        }

        [Browsable(false)]
        public string OutlineColor1Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(outlineColor1); }
            set { outlineColor1 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[Description("Zone Opacity")]
		[Category("2.1 Text Positive")]
		[Gui.Design.DisplayNameAttribute("04.Text Box Opacity")]
		public int Zopacity1
		{
			get { return zopacity1; }
			set { zopacity1 = value; }
		}
		
		//Sound Alert
		[Description("Sound Alerts?")]
        [Category("3.0 Sound Alerts")]
		[Gui.Design.DisplayName("01.Sound Alert")]
        public bool SoundAlert
        {
            get { return soundAlert; }
            set { soundAlert = value; }
        }
		
		[Description("Sound File for pos div")]
		[Category("3.0 Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("02.Pos Div File")]
		public string UpFile
		{
		get { return upFile; }
		set { upFile = value; }
		}
		
		[Description("Sound File for neg div")]
		[Category("3.0 Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("03.Neg Div File")]
		public string DownFile
		{
		get { return downFile; }
		set { downFile = value; }
		}
		
		[Description("Rearm time for alert in seconds")]
		[Category("3.0 Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("04.Rearm Time (sec)")]
		public int RearmTime
		{
		get { return rearmTime; }
		set { rearmTime = value; }
		}
		
		//Alert Box
		[Description("show alert box")]
		[Gui.Design.DisplayName ("1.Show Fixed Text")]
        [Category("1.0 Fixed Text")]
        public bool AlertBox
        {
            get { return alertBox; }
            set { alertBox = value; }
        }
		
        [Description("box position")]
        [Category("1.0 Fixed Text")]
		[NinjaTrader.Gui.Design.DisplayName("2.Box Position")]
		public TextPosition TPos
        {
            get { return tpos; }
            set { tpos = value; }
        }
		
		[XmlIgnore()]
		[Description("Text Font")]
        [Category("1.0 Fixed Text")]
        [Gui.Design.DisplayNameAttribute("3.Text Font")]
        public Font TextFont1
        {
            get { return textFont1; }
            set { textFont1 = value; }
        }
		
        [Browsable(false)]
        public string TextFont1Serialize
        {
            get { return NinjaTrader.Gui.Design.SerializableFont.ToString(textFont1); }
            set { textFont1 = NinjaTrader.Gui.Design.SerializableFont.FromString(value); }
        }
		
		//Fixed Text Positive
		[XmlIgnore]
        [Description("Text Color")]
        [Category("1.1 Fixed Text Positive")]
        [Gui.Design.DisplayNameAttribute("4.Text Color")]
        public Color TextColor3
        {
            get { return textColor3; }
            set { textColor3 = value; }
        }

        [Browsable(false)]
        public string TextColor3Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(textColor3); }
            set { textColor3 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Outline Color")]
        [Category("1.1 Fixed Text Positive")]
        [Gui.Design.DisplayNameAttribute("5.Text Box Outline Color")]
        public Color OutlineColor3
        {
            get { return outlineColor3; }
            set { outlineColor3 = value; }
        }

        [Browsable(false)]
        public string OutlineColor3Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(outlineColor3); }
            set { outlineColor3 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Area Color")]
        [Category("1.1 Fixed Text Positive")]
        [Gui.Design.DisplayNameAttribute("6.Text Box Area Color")]
        public Color AreaColor3
        {
            get { return areaColor3; }
            set { areaColor3 = value; }
        }

        [Browsable(false)]
        public string AreaColor3Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(areaColor3); }
            set { areaColor3 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[Description("Zone Opacity")]
		[Category("1.1 Fixed Text Positive")]
		[Gui.Design.DisplayNameAttribute("7.Text Box Opacity")]
		public int Zopacity3
		{
			get { return zopacity3; }
			set { zopacity3 = value; }
		}
		
		//Fixed Text Negative
		[XmlIgnore]
        [Description("Text Color")]
        [Category("1.2 Fixed Text Negative")]
        [Gui.Design.DisplayNameAttribute("4.Text Color")]
        public Color TextColor4
        {
            get { return textColor4; }
            set { textColor4 = value; }
        }

        [Browsable(false)]
        public string TextColor4Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(textColor4); }
            set { textColor4 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Outline Color")]
        [Category("1.2 Fixed Text Negative")]
        [Gui.Design.DisplayNameAttribute("5.Text Box Outline Color")]
        public Color OutlineColor4
        {
            get { return outlineColor4; }
            set { outlineColor4 = value; }
        }

        [Browsable(false)]
        public string OutlineColor4Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(outlineColor4); }
            set { outlineColor4 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Area Color")]
        [Category("1.2 Fixed Text Negative")]
        [Gui.Design.DisplayNameAttribute("6.Text Box Area Color")]
        public Color AreaColor4
        {
            get { return areaColor4; }
            set { areaColor4 = value; }
        }

        [Browsable(false)]
        public string AreaColor4Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(areaColor4); }
            set { areaColor4 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[Description("Zone Opacity")]
		[Category("1.2 Fixed Text Negative")]
		[Gui.Design.DisplayNameAttribute("7.Text Box Opacity")]
		public int Zopacity4
		{
			get { return zopacity4; }
			set { zopacity4 = value; }
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
        private NetVolDoublerAlert[] cacheNetVolDoublerAlert = null;

        private static NetVolDoublerAlert checkNetVolDoublerAlert = new NetVolDoublerAlert();

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public NetVolDoublerAlert NetVolDoublerAlert()
        {
            return NetVolDoublerAlert(Input);
        }

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public NetVolDoublerAlert NetVolDoublerAlert(Data.IDataSeries input)
        {
            if (cacheNetVolDoublerAlert != null)
                for (int idx = 0; idx < cacheNetVolDoublerAlert.Length; idx++)
                    if (cacheNetVolDoublerAlert[idx].EqualsInput(input))
                        return cacheNetVolDoublerAlert[idx];

            lock (checkNetVolDoublerAlert)
            {
                if (cacheNetVolDoublerAlert != null)
                    for (int idx = 0; idx < cacheNetVolDoublerAlert.Length; idx++)
                        if (cacheNetVolDoublerAlert[idx].EqualsInput(input))
                            return cacheNetVolDoublerAlert[idx];

                NetVolDoublerAlert indicator = new NetVolDoublerAlert();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                NetVolDoublerAlert[] tmp = new NetVolDoublerAlert[cacheNetVolDoublerAlert == null ? 1 : cacheNetVolDoublerAlert.Length + 1];
                if (cacheNetVolDoublerAlert != null)
                    cacheNetVolDoublerAlert.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheNetVolDoublerAlert = tmp;
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
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NetVolDoublerAlert NetVolDoublerAlert()
        {
            return _indicator.NetVolDoublerAlert(Input);
        }

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public Indicator.NetVolDoublerAlert NetVolDoublerAlert(Data.IDataSeries input)
        {
            return _indicator.NetVolDoublerAlert(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NetVolDoublerAlert NetVolDoublerAlert()
        {
            return _indicator.NetVolDoublerAlert(Input);
        }

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public Indicator.NetVolDoublerAlert NetVolDoublerAlert(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.NetVolDoublerAlert(input);
        }
    }
}
#endregion
