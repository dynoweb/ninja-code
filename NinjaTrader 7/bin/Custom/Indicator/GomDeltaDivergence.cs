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
	public class GomDeltaDivergence : GomDeltaIndicator
	{
		#region Variables

		int totalvolume, upvolume, downvolume, deltavolume;
		
		//Text Alert
		private bool	textWarnings		= true;
		private Font 	textFont			= new Font("Arial", 10f, FontStyle.Bold);
		private int 	textOffset			= 2;
		private string 	plotAlert			= "Div";
		
		private Color 	textColor1			= Color.Black;
		private Color 	textColor2			= Color.Black;
				
		private Color 	outlineColor1		= Color.DarkRed;
		private Color 	outlineColor2		= Color.DarkBlue;
				
		private Color 	areaColor1			= Color.Red;
		private Color 	areaColor2			= Color.Blue;
				
		private int 	zopacity1			= 3;
		private int 	zopacity2			= 3;
		
		//Sound Alert
		private bool	soundAlert			= false;
		private string	upFile				= "Alert4.wav";
		private string	downFile			= "Alert3.wav";
		private int		rearmTime			= 44;
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
			if (deltavolume > 0)
			{
				UpVolume.Set(deltavolume);
				DownVolume.Set(0);
								
				if (textWarnings)
				{
					if (Close[0] < Open[0])
						DrawText("Delta" + CurrentBar, true, Convert.ToString(plotAlert), 0, High[0] + textOffset*TickSize, 0, textColor1, 
    					textFont, StringAlignment.Center, outlineColor1, areaColor1, Zopacity1);
						
					else if (Close[0] >= Open[0]) 
						DrawText("Delta" + CurrentBar, true, Convert.ToString("NEUTRAL"), 0, High[0] + textOffset*TickSize, 0, Color.Transparent,
						textFont, StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
				}
				
				if (SoundAlert)
				{
					if (Close[0] < Open[0])
					{
						try
						{
							Alert("Delta",NinjaTrader.Cbi.Priority.Medium,"Div Down "+ Bars.Period.Value + " " + Values[0] + " " + CurrentBar,downFile,rearmTime,Color.Navy,Color.Magenta);
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
					if (Close[0] > Open[0])  
						DrawText("Delta" + CurrentBar, true, Convert.ToString(plotAlert), 0, Low[0] - textOffset*TickSize, 0, textColor2, 
    					textFont, StringAlignment.Center, outlineColor2, areaColor2, Zopacity2);
						
					else if (Close[0] <= Open[0])
						DrawText("Delta" + CurrentBar, true, Convert.ToString("NEUTRAL"), 0, Low[0] - textOffset*TickSize, 0, Color.Transparent,
						textFont, StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
				}
				
				if (SoundAlert)
				{
					if (Close[0] > Open[0])
					{
						try
						{
							Alert("Delta",NinjaTrader.Cbi.Priority.Medium,"Div Up "+ Bars.Period.Value + " " + Values[0] + " " + CurrentBar,upFile,rearmTime,Color.Navy,Color.Magenta);
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

		//Text Alert
		[Description("Text marker")]
		[Gui.Design.DisplayName ("01.Show Text")]
        [Category("Text")]
        public bool TextWarnings
        {
            get { return textWarnings; }
            set { textWarnings = value; }
        }
		
		[XmlIgnore()]
		[Description("Text Font")]
        [Category("Text")]
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
        [Category("Text")]
		public int TextOffset
        {
            get { return textOffset; }
            set {textOffset = Math.Max(0, value); }
        }	
		
		/// <summary>
		/// </summary>
		[Description("User defined alert label")]
		[Category("Text")]
		[Gui.Design.DisplayNameAttribute("04.Text Label")]
		public string PlotAlert
		{
			get { return plotAlert; }
			set { plotAlert = value; }
		}
						
		// Text Positive
		[XmlIgnore]
        [Description("Text Color")]
        [Category("Text Positive")]
        [Gui.Design.DisplayNameAttribute("01.Text Color")]
        public Color TextColor2
        {
            get { return textColor2; }
            set { textColor2 = value; }
        }
		
		[Browsable(false)]
        public string textColor2Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(textColor2); }
            set { textColor2 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Area Color")]
        [Category("Text Positive")]
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
        [Category("Text Positive")]
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
		[Category("Text Positive")]
		[Gui.Design.DisplayNameAttribute("04.Text Box Opacity")]
		public int Zopacity2
		{
			get { return zopacity2; }
			set { zopacity2 = value; }
		}
		
		// Text Negative
		[XmlIgnore]
        [Description("Text Color")]
        [Category("Text Negative")]
        [Gui.Design.DisplayNameAttribute("01.Text Color")]
        public Color TextColor1
        {
            get { return textColor1; }
            set { textColor1 = value; }
        }
		
		[Browsable(false)]
        public string textColor1Serialize
        {
            get { return Gui.Design.SerializableColor.ToString(textColor1); }
            set { textColor1 = Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore]
        [Description("Text Box Area Color")]
        [Category("Text Negative")]
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
        [Category("Text Negative")]
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
		[Category("Text Negative")]
		[Gui.Design.DisplayNameAttribute("04.Text Box Opacity")]
		public int Zopacity1
		{
			get { return zopacity1; }
			set { zopacity1 = value; }
		}
		
		//Sound Alert
		[Description("Sound Alerts?")]
        [Category("Sound Alerts")]
		[Gui.Design.DisplayName("01.Sound Alert")]
        public bool SoundAlert
        {
            get { return soundAlert; }
            set { soundAlert = value; }
        }
		
		[Description("Sound File for pos div")]
		[Category("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("02.Pos Div File")]
		public string UpFile
		{
		get { return upFile; }
		set { upFile = value; }
		}
		
		[Description("Sound File for neg div")]
		[Category("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("03.Neg Div File")]
		public string DownFile
		{
		get { return downFile; }
		set { downFile = value; }
		}
		
		[Description("Rearm time for alert in seconds")]
		[Category("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("04.Rearm Time (sec)")]
		public int RearmTime
		{
		get { return rearmTime; }
		set { rearmTime = value; }
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
        private GomDeltaDivergence[] cacheGomDeltaDivergence = null;

        private static GomDeltaDivergence checkGomDeltaDivergence = new GomDeltaDivergence();

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public GomDeltaDivergence GomDeltaDivergence()
        {
            return GomDeltaDivergence(Input);
        }

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public GomDeltaDivergence GomDeltaDivergence(Data.IDataSeries input)
        {
            if (cacheGomDeltaDivergence != null)
                for (int idx = 0; idx < cacheGomDeltaDivergence.Length; idx++)
                    if (cacheGomDeltaDivergence[idx].EqualsInput(input))
                        return cacheGomDeltaDivergence[idx];

            lock (checkGomDeltaDivergence)
            {
                if (cacheGomDeltaDivergence != null)
                    for (int idx = 0; idx < cacheGomDeltaDivergence.Length; idx++)
                        if (cacheGomDeltaDivergence[idx].EqualsInput(input))
                            return cacheGomDeltaDivergence[idx];

                GomDeltaDivergence indicator = new GomDeltaDivergence();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomDeltaDivergence[] tmp = new GomDeltaDivergence[cacheGomDeltaDivergence == null ? 1 : cacheGomDeltaDivergence.Length + 1];
                if (cacheGomDeltaDivergence != null)
                    cacheGomDeltaDivergence.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomDeltaDivergence = tmp;
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
        public Indicator.GomDeltaDivergence GomDeltaDivergence()
        {
            return _indicator.GomDeltaDivergence(Input);
        }

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaDivergence GomDeltaDivergence(Data.IDataSeries input)
        {
            return _indicator.GomDeltaDivergence(input);
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
        public Indicator.GomDeltaDivergence GomDeltaDivergence()
        {
            return _indicator.GomDeltaDivergence(Input);
        }

        /// <summary>
        /// Gom Delta Divergence
        /// </summary>
        /// <returns></returns>
        public Indicator.GomDeltaDivergence GomDeltaDivergence(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomDeltaDivergence(input);
        }
    }
}
#endregion
