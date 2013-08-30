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

#region Global Enums

public enum anaSuperTrendU11BaseType {Median, ADXVMA, Butterworth_2, Butterworth_3, DEMA, DSMA, DTMA, DWMA, Ehlers, EMA, Gauss_2, Gauss_3, Gauss_4, 
			HMA, HoltEMA, LinReg, SMA, SuperSmoother_2, SuperSmoother_3, TEMA, TMA, TSMA, TWMA, VWMA, WMA, ZeroLagHATEMA, ZeroLagTEMA, ZLEMA}
public enum anaSuperTrendU11OffsetType {Default, Median, ADXVMA, Butterworth_2, Butterworth_3, DEMA, DSMA, DTMA, DWMA, Ehlers, EMA, Gauss_2, Gauss_3, Gauss_4, 
			HMA, HoltEMA, LinReg, SMA, SuperSmoother_2, SuperSmoother_3, TEMA, TMA, TSMA, TWMA, VWMA, WMA, ZeroLagHATEMA, ZeroLagTEMA, ZLEMA}
public enum	anaSuperTrendU11VolaType {Simple_Range, True_Range, Standard_Deviation}

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// anaSuperTrendU11 Indicator
    /// </summary>
    [Description("SuperTrend")]
    public class anaSuperTrendU11 : Indicator
    {
        #region Variables
			private int 						basePeriod			= 3; // Default setting for Median Period
            private int 						rangePeriod			= 15; // Default setting for Range Period
			private double 						multiplier 			= 2.5; // Default setting for Multiplier
			private anaSuperTrendU11BaseType 	thisBaseType		= anaSuperTrendU11BaseType.Median; 
			private anaSuperTrendU11OffsetType 	thisOffsetType		= anaSuperTrendU11OffsetType.Default; 
			private anaSuperTrendU11VolaType 	thisVolaType		= anaSuperTrendU11VolaType.True_Range; 
			private bool						candles				= false;
			private bool 						gap					= false;
			private bool						reverseIntraBar		= false;
			private bool 						showArrows 			= true;
			private bool 						showPaintBars 		= false;
			private bool 						showStopLine		= true;
			private bool						soundAlert			= false;
			private bool 						currentUpTrend		= true;
			private bool 						priorUpTrend		= true;
			private bool						stoppedOut			= false;
			private double						movingBase			= 0.0;
			private double 						offset				= 0.0;
			private double						trailingAmount		= 0.0;
			private double						currentStopLong		= 0.0;
			private double						currentStopShort	= 0.0;
			private double						margin				= 0.0;
			private int							displacement		= 0;
			private int							opacity				= 3;
			private int							alpha				= 0;
			private int 						plot0Width 			= 1;
			private PlotStyle 					plot0Style			= PlotStyle.Dot;
			private DashStyle					dash0Style			= DashStyle.Dot;
			private int 						plot1Width 			= 1;
			private PlotStyle 					plot1Style			= PlotStyle.Line;
			private DashStyle 					dash1Style			= DashStyle.Solid;
			private Color 						upColor				= Color.DodgerBlue;
			private Color 						downColor			= Color.Red;
			private Color						trendColor			= Color.Empty;
			private int 						rearmTime			= 30;
			private string 						confirmedUpTrend	= "newuptrend.wav";
			private string 						confirmedDownTrend	= "newdowntrend.wav";
			private string 						potentialUpTrend	= "potentialuptrend.wav";
			private string 						potentialDownTrend	= "potentialdowntrend.wav";
			private DataSeries					reverseDot;			
			private BoolSeries 					upTrend;
			private IDataSeries		 			baseline;
			private IDataSeries					rangeSeries;
			private IDataSeries 				offsetSeries;
			private ATR							volatility;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Gray, PlotStyle.Dot, "StopDot"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "StopLine"));
            Overlay				= true;
			PlotsConfigurable 	= false;
			reverseDot			= new DataSeries(this);
			upTrend 			= new BoolSeries(this);
        }

		protected override void OnStartUp()
		{
			switch (thisVolaType)
			{
				case anaSuperTrendU11VolaType.Simple_Range:
					rangeSeries = Range();
					break;
				case anaSuperTrendU11VolaType.True_Range:
					rangeSeries = ATR(Close,1);
					break;
				case anaSuperTrendU11VolaType.Standard_Deviation:
					thisOffsetType = anaSuperTrendU11OffsetType.Default; 
					rangeSeries = StdDev(Close, rangePeriod);
					offsetSeries = StdDev(Close, rangePeriod);
					break;
			}
			switch (thisBaseType)
			{
				case anaSuperTrendU11BaseType.Median: 
					baseline = anaMovingMedian(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.ADXVMA: 
					baseline = anaADXVMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.Butterworth_2: 
					baseline = anaButterworthFilter(Input, basePeriod, 2);
					break;
				case anaSuperTrendU11BaseType.Butterworth_3: 
					baseline = anaButterworthFilter(Input, basePeriod, 3);
					break;
				case anaSuperTrendU11BaseType.DEMA: 
					baseline = DEMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.DSMA: 
					baseline = DSMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.DTMA: 
					baseline = DTMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.DWMA: 
					baseline = DWMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.Ehlers: 
					baseline = anaEhlersFilter(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.EMA: 
					baseline = EMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.Gauss_2: 
					baseline = anaGaussianFilter(Input, basePeriod, 2);
					break;
				case anaSuperTrendU11BaseType.Gauss_3: 
					baseline = anaGaussianFilter(Input, basePeriod, 3);
					break;
				case anaSuperTrendU11BaseType.Gauss_4: 
					baseline = anaGaussianFilter(Input, basePeriod, 4);
					break;
				case anaSuperTrendU11BaseType.HMA: 
					baseline = HMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.HoltEMA: 
					baseline = anaHoltEMA(Input, basePeriod, 2*basePeriod);
					break;
				case anaSuperTrendU11BaseType.LinReg: 
					baseline = LinReg(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.SMA: 
					baseline = SMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.SuperSmoother_2: 
					baseline = anaSuperSmootherFilter(Input, basePeriod, 2);
					break;
				case anaSuperTrendU11BaseType.SuperSmoother_3: 
					baseline = anaSuperSmootherFilter(Input, basePeriod, 3);
					break;
				case anaSuperTrendU11BaseType.TEMA: 
					baseline = TEMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.TMA: 
					baseline = TMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.TSMA: 
					baseline = TSMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.TWMA: 
					baseline = TWMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.VWMA: 
					baseline = VWMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.WMA: 
					baseline = WMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.ZeroLagHATEMA: 
					baseline = anaZeroLagHATEMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.ZeroLagTEMA: 
					baseline = ZeroLagTEMA(Input, basePeriod);
					break;
				case anaSuperTrendU11BaseType.ZLEMA: 
					baseline = ZLEMA(Input, basePeriod);
					break;
			}
			if (thisVolaType != anaSuperTrendU11VolaType.Standard_Deviation)
			{
				switch (thisOffsetType)
				{
					case anaSuperTrendU11OffsetType.Default: 
						offsetSeries = EMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.Median: 
						offsetSeries = anaMovingMedian(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.ADXVMA: 
						offsetSeries = anaADXVMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.Butterworth_2: 
						offsetSeries = anaButterworthFilter(rangeSeries, rangePeriod, 2);
						break;
					case anaSuperTrendU11OffsetType.Butterworth_3: 
						offsetSeries = anaButterworthFilter(rangeSeries, rangePeriod, 3);
						break;
					case anaSuperTrendU11OffsetType.DEMA: 
						offsetSeries = DEMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.DSMA: 
						offsetSeries = DSMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.DTMA: 
						offsetSeries = DTMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.DWMA: 
						offsetSeries = DWMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.Ehlers: 
						offsetSeries = anaEhlersFilter(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.EMA: 
						offsetSeries = EMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.Gauss_2: 
						offsetSeries = anaGaussianFilter(rangeSeries, rangePeriod, 2);
						break;
					case anaSuperTrendU11OffsetType.Gauss_3: 
						offsetSeries = anaGaussianFilter(rangeSeries, rangePeriod, 3);
						break;
					case anaSuperTrendU11OffsetType.Gauss_4: 
						offsetSeries = anaGaussianFilter(rangeSeries, rangePeriod, 4);
						break;
					case anaSuperTrendU11OffsetType.HMA: 
						offsetSeries = HMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.HoltEMA: 
						offsetSeries = anaHoltEMA(rangeSeries, rangePeriod, 2*rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.LinReg: 
						offsetSeries = LinReg(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.SMA: 
						offsetSeries = SMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.SuperSmoother_2: 
						offsetSeries = anaSuperSmootherFilter(rangeSeries, rangePeriod, 2);
						break;
					case anaSuperTrendU11OffsetType.SuperSmoother_3: 
						offsetSeries = anaSuperSmootherFilter(rangeSeries, rangePeriod, 3);
						break;
					case anaSuperTrendU11OffsetType.TEMA: 
						offsetSeries = TEMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.TMA: 
						offsetSeries = TMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.TSMA: 
						offsetSeries = TSMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.TWMA: 
						offsetSeries = TWMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.VWMA: 
						offsetSeries = VWMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.WMA: 
						offsetSeries = WMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.ZeroLagHATEMA: 
						offsetSeries = anaZeroLagHATEMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.ZeroLagTEMA: 
						offsetSeries = ZeroLagTEMA(rangeSeries, rangePeriod);
						break;
					case anaSuperTrendU11OffsetType.ZLEMA: 
						offsetSeries = ZLEMA(rangeSeries, rangePeriod);
						break;
				}
			}
			volatility = ATR(Close, 256); 
				
			Plots[0].Pen.Width = plot0Width;
			Plots[0].PlotStyle = plot0Style;
			Plots[0].Pen.DashStyle = dash0Style;
			Plots[1].Pen.Width = plot1Width;
			Plots[1].PlotStyle = plot1Style;
			Plots[1].Pen.DashStyle = dash1Style;
			if (ChartControl != null && ChartControl.ChartStyleType == ChartStyleType.CandleStick)
			{
				candles = true;
				alpha = 25*opacity;
			}
			else
				candles = false;
			gap = (plot1Style == PlotStyle.Line)||(plot1Style == PlotStyle.Square);
		}        
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		protected override void OnBarUpdate()
        {
			if (CurrentBar == 0)
			{ 
				displacement = Math.Max(Displacement, -CurrentBar);
				priorUpTrend = true;
				currentUpTrend = true;
				upTrend.Set(true);
				StopDot.Set (Close[0]);
				StopLine.Set (Close[0]); 
				PlotColors[0][-displacement] = Color.Transparent;
				PlotColors[1][-displacement] = Color.Transparent;
				return; 
			}
			if (FirstTickOfBar)
			{
				displacement = Math.Max(Displacement, -CurrentBar);
				movingBase = baseline[1];
				offset = Math.Max(TickSize, offsetSeries[1]);
				trailingAmount = multiplier * offset;
				margin = volatility[1];
				if(currentUpTrend)
				{
					currentStopShort = movingBase + trailingAmount;
					if(priorUpTrend)
						currentStopLong = Math.Max(currentStopLong, movingBase - trailingAmount);
					else
						currentStopLong = movingBase - trailingAmount;
					StopDot.Set(currentStopLong);
					ReverseDot.Set(currentStopShort);
					PlotColors[0][-displacement] = upColor;

					if(showStopLine)
					{	
						StopLine.Set(currentStopLong);
						if(gap && !priorUpTrend)
							PlotColors[1][-displacement]= Color.Transparent;
						else
							PlotColors[1][-displacement] = upColor;
					}
					else
						StopLine.Reset();
				}
				else	
				{	
					currentStopLong = movingBase - trailingAmount;
					if(!priorUpTrend)
						currentStopShort = Math.Min(currentStopShort, movingBase + trailingAmount);
					else
						currentStopShort = movingBase + trailingAmount;
					StopDot.Set(currentStopShort);
					ReverseDot.Set(currentStopLong);
					PlotColors[0][-displacement] = downColor;
					if(showStopLine)
					{	
						StopLine.Set(currentStopShort);
						if(gap && priorUpTrend)
							PlotColors[1][-displacement]= Color.Transparent;
						else
							PlotColors[1][-displacement] = downColor;
					}
					else
						StopLine.Reset();
				}
				if(showPaintBars)
				{
					if (currentUpTrend)
						trendColor = upColor;
					else
						trendColor = downColor;
					CandleOutlineColorSeries[-displacement] = trendColor;
					BarColorSeries[-displacement] = trendColor;
				}
				if(showArrows)
				{
					if(currentUpTrend && !priorUpTrend)
						DrawArrowUp("arrow" + CurrentBar, true, -displacement, currentStopLong - 0.5 * margin, upColor);
					else if(!currentUpTrend && priorUpTrend)
						DrawArrowDown("arrow" + CurrentBar, true, -displacement, currentStopShort + 0.5 * margin, downColor);
				}
				priorUpTrend = currentUpTrend;
				stoppedOut = false;
			}
			
			if(reverseIntraBar) // only one trend change per bar is permitted
			{
				if(!stoppedOut)
				{
					if (priorUpTrend && Low[0] < currentStopLong)
					{
						currentUpTrend = false;
						stoppedOut = true;
					}	
					else if (!priorUpTrend && High[0] > currentStopShort)
					{
						currentUpTrend = true;
						stoppedOut = true;
					}
				}
			}
			else 
			{
				if (priorUpTrend && Close[0] < currentStopLong)
					currentUpTrend = false;
				else if (!priorUpTrend && Close[0] > currentStopShort)
					currentUpTrend = true;
				else
					currentUpTrend = priorUpTrend;
			}
			
			// this information can be accessed by a strategy
			if(CalculateOnBarClose)
				upTrend.Set(currentUpTrend);
			else if(FirstTickOfBar && !reverseIntraBar)
				upTrend.Set(priorUpTrend);
			else if(reverseIntraBar)
				upTrend.Set(currentUpTrend);
			
			if(showPaintBars && candles)
			{
				if(Open[0] < Close[0]) 
					BarColorSeries[-displacement] = Color.FromArgb(alpha, trendColor);
			}
				
			if (soundAlert && !Historical && IsConnected() && (CalculateOnBarClose || reverseIntraBar))
			{
				if(currentUpTrend && !priorUpTrend)		
				{
					try
					{
							Alert("NewUpTrend", NinjaTrader.Cbi.Priority.Medium,"NewUpTrend", confirmedUpTrend, rearmTime, Color.Black, upColor);
					}
					catch{}
				}
				else if(!currentUpTrend && priorUpTrend)	 
				{
					try
					{
							Alert("NewDownTrend", NinjaTrader.Cbi.Priority.Medium,"NewDownTrend", confirmedDownTrend, rearmTime, Color.Black, downColor);
					}
					catch{}
				}
			}				
			if (soundAlert && !Historical && IsConnected() && !CalculateOnBarClose && !reverseIntraBar)
			{
				if(currentUpTrend && !priorUpTrend)		
				{
					try
					{
							Alert("PotentialUpTrend", NinjaTrader.Cbi.Priority.Medium,"PotentialUpTrend", potentialUpTrend, rearmTime, Color.Black, upColor);
					}
					catch{}
				}
				else if(!currentUpTrend && priorUpTrend)	 
				{
					try
					{
							Alert("PotentialDownTrend", NinjaTrader.Cbi.Priority.Medium,"PotentialDownTrend", potentialDownTrend, rearmTime, Color.Black, downColor);
					}
					catch{}
				}
			}	
        }

        #region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries StopDot
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries StopLine
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
        [XmlIgnore()]		
        public DataSeries ReverseDot
        {
            get { return reverseDot; }
        }

		[Browsable(false)]
        [XmlIgnore()]		
        public BoolSeries UpTrend
        {
            get { return upTrend; }
        }

		[Description("Median period")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Baseline period")]
        public int BasePeriod
        {
            get { return basePeriod; }
            set { basePeriod = Math.Max(1, value); }
        }

		[Description("ATR period")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Offset period")]
        public int RangePeriod
        {
            get { return rangePeriod; }
            set { rangePeriod = Math.Max(1, value); }
        }

       [Description("ATR multiplier")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Offset multiplier")]
        public double Multiplier
        {
            get { return multiplier; }
            set { multiplier = Math.Max(0.0, value); }
        }
		
		/// <summary>
		/// </summary>
		[Description("Moving average type for baseline")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Baseline smoothing")]
		public anaSuperTrendU11BaseType ThisBaseType
		{
			get { return thisBaseType; }
			set { thisBaseType = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Moving average type for volatility estimator")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Offset smoothing")]
		public anaSuperTrendU11OffsetType ThisOffsetType
		{
			get { return thisOffsetType; }
			set { thisOffsetType = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Simple or True Range")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Offset type")]
		public anaSuperTrendU11VolaType ThisRangeType
		{
			get { return thisVolaType; }
			set { thisVolaType = value; }
		}

		[Description("Reverse intra-bar")]
        [GridCategory("Options")]
		[Gui.Design.DisplayName ("Reverse intra-bar")]
        public bool ReverseIntraBar
        {
            get { return reverseIntraBar; }
            set { reverseIntraBar = value; }
        }

		[Description("Show arrows when trendline is violated?")]
        [GridCategory("Display and Sound Options")]
		[Gui.Design.DisplayName ("Show arrows")]
        public bool ShowArrows
        {
            get { return showArrows; }
            set { showArrows = value; }
        }

		[Description("Color the bars in the direction of the trend?")]
        [GridCategory("Display and Sound Options")]
		[Gui.Design.DisplayName ("Show paintbars")]
        public bool ShowPaintBars
        {
            get { return showPaintBars; }
            set { showPaintBars = value; }
        }

		[Description("Show stop line")]
        [GridCategory("Display and Sound Options")]
		[Gui.Design.DisplayName ("Show stop line")]
        public bool ShowStopLine
        {
            get { return showStopLine; }
            set { showStopLine = value; }
        }

		[Description("Sound alerts activated")]
        [GridCategory("Display and Sound Options")]
		[Gui.Design.DisplayName("Sound alert active")]
        public bool SoundAlert
        {
            get { return soundAlert; }
            set { soundAlert = value; }
        }
		
		/// <summary>
		/// </summary>
        [XmlIgnore()]		
		[Description("Select color for uptrend")]
		[GridCategory("Plot Colors")]
		[Gui.Design.DisplayName("Uptrend")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
			set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
        [XmlIgnore()]		
		[Description("Select color for downtrend")]
		[GridCategory("Plot Colors")]
		[Gui.Design.DisplayName("Downtrend")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColor); }
			set { downColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Width for stop dots.")]
		[GridCategory("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Width stop dots")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for stop dots.")]
		[GridCategory("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Plot style stop dots")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for stop dots.")]
		[GridCategory("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style stop dots")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Width for stop line.")]
		[GridCategory("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Width stop line")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for stop line.")]
		[GridCategory("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Plot style stop line")]
		public PlotStyle Plot1Style
		{
			get { return plot1Style; }
			set { plot1Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for stop line.")]
		[GridCategory("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style stop line")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
		} 

		[Description("When paint bars are activated, this parameter sets the opacity of the upclose bars")]
		[GridCategory("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Upclose opacity")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = value; }
		}
		
		[Description("Sound file for confirmed new uptrend")]
		[GridCategory("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("New uptrend")]
		public string ConfirmedUpTrend
		{
			get { return confirmedUpTrend; }
			set { confirmedUpTrend = value; }
		}
		
		[Description("Sound file for confirmed new downtrend")]
		[GridCategory("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("New downtrend")]
		public string ConfirmedDownTrend
		{
			get { return confirmedDownTrend; }
			set { confirmedDownTrend = value; }
		}
		
		[Description("Sound file for potential new uptrend")]
		[GridCategory("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("Potential uptrend")]
		public string PotentialUpTrend
		{
			get { return potentialUpTrend; }
			set { potentialUpTrend = value; }
		}
		
		[Description("Sound file for potential new downtrend")]
		[GridCategory("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("Potential downtrend")]
		public string PotentialDownTrend
		{
			get { return potentialDownTrend; }
			set { potentialDownTrend = value; }
		}
		
		[Description("Rearm time for alert in seconds")]
		[GridCategory("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("Rearm time (sec)")]
		public int RearmTime
		{
			get { return rearmTime; }
			set { rearmTime = value; }
		}
		#endregion
		
		
		#region Miscellaneous
		private bool IsConnected()
        {
			if ( Bars.MarketData != null
					&& Bars.MarketData.Connection.PriceStatus == Cbi.ConnectionStatus.Connected
					&& Bars.Session.InSession(Now, Bars.Period, true))
				return true;
			else
            	return false;
        }		
		
		private DateTime Now
		{
			get 
			{ 
				DateTime now = (Bars.MarketData.Connection.Options.Provider == Cbi.Provider.Replay ? Bars.MarketData.Connection.Now : DateTime.Now); 

				if (now.Millisecond > 0)
					now = Cbi.Globals.MinDate.AddSeconds((long) System.Math.Floor(now.Subtract(Cbi.Globals.MinDate).TotalSeconds));

				return now;
			}
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
        private anaSuperTrendU11[] cacheanaSuperTrendU11 = null;

        private static anaSuperTrendU11 checkanaSuperTrendU11 = new anaSuperTrendU11();

        /// <summary>
        /// SuperTrend
        /// </summary>
        /// <returns></returns>
        public anaSuperTrendU11 anaSuperTrendU11(int basePeriod, string confirmedDownTrend, string confirmedUpTrend, DashStyle dash0Style, DashStyle dash1Style, Color downColor, double multiplier, int opacity, PlotStyle plot0Style, int plot0Width, PlotStyle plot1Style, int plot1Width, string potentialDownTrend, string potentialUpTrend, int rangePeriod, int rearmTime, bool reverseIntraBar, bool showArrows, bool showPaintBars, bool showStopLine, bool soundAlert, anaSuperTrendU11BaseType thisBaseType, anaSuperTrendU11OffsetType thisOffsetType, anaSuperTrendU11VolaType thisRangeType, Color upColor)
        {
            return anaSuperTrendU11(Input, basePeriod, confirmedDownTrend, confirmedUpTrend, dash0Style, dash1Style, downColor, multiplier, opacity, plot0Style, plot0Width, plot1Style, plot1Width, potentialDownTrend, potentialUpTrend, rangePeriod, rearmTime, reverseIntraBar, showArrows, showPaintBars, showStopLine, soundAlert, thisBaseType, thisOffsetType, thisRangeType, upColor);
        }

        /// <summary>
        /// SuperTrend
        /// </summary>
        /// <returns></returns>
        public anaSuperTrendU11 anaSuperTrendU11(Data.IDataSeries input, int basePeriod, string confirmedDownTrend, string confirmedUpTrend, DashStyle dash0Style, DashStyle dash1Style, Color downColor, double multiplier, int opacity, PlotStyle plot0Style, int plot0Width, PlotStyle plot1Style, int plot1Width, string potentialDownTrend, string potentialUpTrend, int rangePeriod, int rearmTime, bool reverseIntraBar, bool showArrows, bool showPaintBars, bool showStopLine, bool soundAlert, anaSuperTrendU11BaseType thisBaseType, anaSuperTrendU11OffsetType thisOffsetType, anaSuperTrendU11VolaType thisRangeType, Color upColor)
        {
            if (cacheanaSuperTrendU11 != null)
                for (int idx = 0; idx < cacheanaSuperTrendU11.Length; idx++)
                    if (cacheanaSuperTrendU11[idx].BasePeriod == basePeriod && cacheanaSuperTrendU11[idx].ConfirmedDownTrend == confirmedDownTrend && cacheanaSuperTrendU11[idx].ConfirmedUpTrend == confirmedUpTrend && cacheanaSuperTrendU11[idx].Dash0Style == dash0Style && cacheanaSuperTrendU11[idx].Dash1Style == dash1Style && cacheanaSuperTrendU11[idx].DownColor == downColor && Math.Abs(cacheanaSuperTrendU11[idx].Multiplier - multiplier) <= double.Epsilon && cacheanaSuperTrendU11[idx].Opacity == opacity && cacheanaSuperTrendU11[idx].Plot0Style == plot0Style && cacheanaSuperTrendU11[idx].Plot0Width == plot0Width && cacheanaSuperTrendU11[idx].Plot1Style == plot1Style && cacheanaSuperTrendU11[idx].Plot1Width == plot1Width && cacheanaSuperTrendU11[idx].PotentialDownTrend == potentialDownTrend && cacheanaSuperTrendU11[idx].PotentialUpTrend == potentialUpTrend && cacheanaSuperTrendU11[idx].RangePeriod == rangePeriod && cacheanaSuperTrendU11[idx].RearmTime == rearmTime && cacheanaSuperTrendU11[idx].ReverseIntraBar == reverseIntraBar && cacheanaSuperTrendU11[idx].ShowArrows == showArrows && cacheanaSuperTrendU11[idx].ShowPaintBars == showPaintBars && cacheanaSuperTrendU11[idx].ShowStopLine == showStopLine && cacheanaSuperTrendU11[idx].SoundAlert == soundAlert && cacheanaSuperTrendU11[idx].ThisBaseType == thisBaseType && cacheanaSuperTrendU11[idx].ThisOffsetType == thisOffsetType && cacheanaSuperTrendU11[idx].ThisRangeType == thisRangeType && cacheanaSuperTrendU11[idx].UpColor == upColor && cacheanaSuperTrendU11[idx].EqualsInput(input))
                        return cacheanaSuperTrendU11[idx];

            lock (checkanaSuperTrendU11)
            {
                checkanaSuperTrendU11.BasePeriod = basePeriod;
                basePeriod = checkanaSuperTrendU11.BasePeriod;
                checkanaSuperTrendU11.ConfirmedDownTrend = confirmedDownTrend;
                confirmedDownTrend = checkanaSuperTrendU11.ConfirmedDownTrend;
                checkanaSuperTrendU11.ConfirmedUpTrend = confirmedUpTrend;
                confirmedUpTrend = checkanaSuperTrendU11.ConfirmedUpTrend;
                checkanaSuperTrendU11.Dash0Style = dash0Style;
                dash0Style = checkanaSuperTrendU11.Dash0Style;
                checkanaSuperTrendU11.Dash1Style = dash1Style;
                dash1Style = checkanaSuperTrendU11.Dash1Style;
                checkanaSuperTrendU11.DownColor = downColor;
                downColor = checkanaSuperTrendU11.DownColor;
                checkanaSuperTrendU11.Multiplier = multiplier;
                multiplier = checkanaSuperTrendU11.Multiplier;
                checkanaSuperTrendU11.Opacity = opacity;
                opacity = checkanaSuperTrendU11.Opacity;
                checkanaSuperTrendU11.Plot0Style = plot0Style;
                plot0Style = checkanaSuperTrendU11.Plot0Style;
                checkanaSuperTrendU11.Plot0Width = plot0Width;
                plot0Width = checkanaSuperTrendU11.Plot0Width;
                checkanaSuperTrendU11.Plot1Style = plot1Style;
                plot1Style = checkanaSuperTrendU11.Plot1Style;
                checkanaSuperTrendU11.Plot1Width = plot1Width;
                plot1Width = checkanaSuperTrendU11.Plot1Width;
                checkanaSuperTrendU11.PotentialDownTrend = potentialDownTrend;
                potentialDownTrend = checkanaSuperTrendU11.PotentialDownTrend;
                checkanaSuperTrendU11.PotentialUpTrend = potentialUpTrend;
                potentialUpTrend = checkanaSuperTrendU11.PotentialUpTrend;
                checkanaSuperTrendU11.RangePeriod = rangePeriod;
                rangePeriod = checkanaSuperTrendU11.RangePeriod;
                checkanaSuperTrendU11.RearmTime = rearmTime;
                rearmTime = checkanaSuperTrendU11.RearmTime;
                checkanaSuperTrendU11.ReverseIntraBar = reverseIntraBar;
                reverseIntraBar = checkanaSuperTrendU11.ReverseIntraBar;
                checkanaSuperTrendU11.ShowArrows = showArrows;
                showArrows = checkanaSuperTrendU11.ShowArrows;
                checkanaSuperTrendU11.ShowPaintBars = showPaintBars;
                showPaintBars = checkanaSuperTrendU11.ShowPaintBars;
                checkanaSuperTrendU11.ShowStopLine = showStopLine;
                showStopLine = checkanaSuperTrendU11.ShowStopLine;
                checkanaSuperTrendU11.SoundAlert = soundAlert;
                soundAlert = checkanaSuperTrendU11.SoundAlert;
                checkanaSuperTrendU11.ThisBaseType = thisBaseType;
                thisBaseType = checkanaSuperTrendU11.ThisBaseType;
                checkanaSuperTrendU11.ThisOffsetType = thisOffsetType;
                thisOffsetType = checkanaSuperTrendU11.ThisOffsetType;
                checkanaSuperTrendU11.ThisRangeType = thisRangeType;
                thisRangeType = checkanaSuperTrendU11.ThisRangeType;
                checkanaSuperTrendU11.UpColor = upColor;
                upColor = checkanaSuperTrendU11.UpColor;

                if (cacheanaSuperTrendU11 != null)
                    for (int idx = 0; idx < cacheanaSuperTrendU11.Length; idx++)
                        if (cacheanaSuperTrendU11[idx].BasePeriod == basePeriod && cacheanaSuperTrendU11[idx].ConfirmedDownTrend == confirmedDownTrend && cacheanaSuperTrendU11[idx].ConfirmedUpTrend == confirmedUpTrend && cacheanaSuperTrendU11[idx].Dash0Style == dash0Style && cacheanaSuperTrendU11[idx].Dash1Style == dash1Style && cacheanaSuperTrendU11[idx].DownColor == downColor && Math.Abs(cacheanaSuperTrendU11[idx].Multiplier - multiplier) <= double.Epsilon && cacheanaSuperTrendU11[idx].Opacity == opacity && cacheanaSuperTrendU11[idx].Plot0Style == plot0Style && cacheanaSuperTrendU11[idx].Plot0Width == plot0Width && cacheanaSuperTrendU11[idx].Plot1Style == plot1Style && cacheanaSuperTrendU11[idx].Plot1Width == plot1Width && cacheanaSuperTrendU11[idx].PotentialDownTrend == potentialDownTrend && cacheanaSuperTrendU11[idx].PotentialUpTrend == potentialUpTrend && cacheanaSuperTrendU11[idx].RangePeriod == rangePeriod && cacheanaSuperTrendU11[idx].RearmTime == rearmTime && cacheanaSuperTrendU11[idx].ReverseIntraBar == reverseIntraBar && cacheanaSuperTrendU11[idx].ShowArrows == showArrows && cacheanaSuperTrendU11[idx].ShowPaintBars == showPaintBars && cacheanaSuperTrendU11[idx].ShowStopLine == showStopLine && cacheanaSuperTrendU11[idx].SoundAlert == soundAlert && cacheanaSuperTrendU11[idx].ThisBaseType == thisBaseType && cacheanaSuperTrendU11[idx].ThisOffsetType == thisOffsetType && cacheanaSuperTrendU11[idx].ThisRangeType == thisRangeType && cacheanaSuperTrendU11[idx].UpColor == upColor && cacheanaSuperTrendU11[idx].EqualsInput(input))
                            return cacheanaSuperTrendU11[idx];

                anaSuperTrendU11 indicator = new anaSuperTrendU11();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BasePeriod = basePeriod;
                indicator.ConfirmedDownTrend = confirmedDownTrend;
                indicator.ConfirmedUpTrend = confirmedUpTrend;
                indicator.Dash0Style = dash0Style;
                indicator.Dash1Style = dash1Style;
                indicator.DownColor = downColor;
                indicator.Multiplier = multiplier;
                indicator.Opacity = opacity;
                indicator.Plot0Style = plot0Style;
                indicator.Plot0Width = plot0Width;
                indicator.Plot1Style = plot1Style;
                indicator.Plot1Width = plot1Width;
                indicator.PotentialDownTrend = potentialDownTrend;
                indicator.PotentialUpTrend = potentialUpTrend;
                indicator.RangePeriod = rangePeriod;
                indicator.RearmTime = rearmTime;
                indicator.ReverseIntraBar = reverseIntraBar;
                indicator.ShowArrows = showArrows;
                indicator.ShowPaintBars = showPaintBars;
                indicator.ShowStopLine = showStopLine;
                indicator.SoundAlert = soundAlert;
                indicator.ThisBaseType = thisBaseType;
                indicator.ThisOffsetType = thisOffsetType;
                indicator.ThisRangeType = thisRangeType;
                indicator.UpColor = upColor;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaSuperTrendU11[] tmp = new anaSuperTrendU11[cacheanaSuperTrendU11 == null ? 1 : cacheanaSuperTrendU11.Length + 1];
                if (cacheanaSuperTrendU11 != null)
                    cacheanaSuperTrendU11.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaSuperTrendU11 = tmp;
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
        /// SuperTrend
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaSuperTrendU11 anaSuperTrendU11(int basePeriod, string confirmedDownTrend, string confirmedUpTrend, DashStyle dash0Style, DashStyle dash1Style, Color downColor, double multiplier, int opacity, PlotStyle plot0Style, int plot0Width, PlotStyle plot1Style, int plot1Width, string potentialDownTrend, string potentialUpTrend, int rangePeriod, int rearmTime, bool reverseIntraBar, bool showArrows, bool showPaintBars, bool showStopLine, bool soundAlert, anaSuperTrendU11BaseType thisBaseType, anaSuperTrendU11OffsetType thisOffsetType, anaSuperTrendU11VolaType thisRangeType, Color upColor)
        {
            return _indicator.anaSuperTrendU11(Input, basePeriod, confirmedDownTrend, confirmedUpTrend, dash0Style, dash1Style, downColor, multiplier, opacity, plot0Style, plot0Width, plot1Style, plot1Width, potentialDownTrend, potentialUpTrend, rangePeriod, rearmTime, reverseIntraBar, showArrows, showPaintBars, showStopLine, soundAlert, thisBaseType, thisOffsetType, thisRangeType, upColor);
        }

        /// <summary>
        /// SuperTrend
        /// </summary>
        /// <returns></returns>
        public Indicator.anaSuperTrendU11 anaSuperTrendU11(Data.IDataSeries input, int basePeriod, string confirmedDownTrend, string confirmedUpTrend, DashStyle dash0Style, DashStyle dash1Style, Color downColor, double multiplier, int opacity, PlotStyle plot0Style, int plot0Width, PlotStyle plot1Style, int plot1Width, string potentialDownTrend, string potentialUpTrend, int rangePeriod, int rearmTime, bool reverseIntraBar, bool showArrows, bool showPaintBars, bool showStopLine, bool soundAlert, anaSuperTrendU11BaseType thisBaseType, anaSuperTrendU11OffsetType thisOffsetType, anaSuperTrendU11VolaType thisRangeType, Color upColor)
        {
            return _indicator.anaSuperTrendU11(input, basePeriod, confirmedDownTrend, confirmedUpTrend, dash0Style, dash1Style, downColor, multiplier, opacity, plot0Style, plot0Width, plot1Style, plot1Width, potentialDownTrend, potentialUpTrend, rangePeriod, rearmTime, reverseIntraBar, showArrows, showPaintBars, showStopLine, soundAlert, thisBaseType, thisOffsetType, thisRangeType, upColor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// SuperTrend
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaSuperTrendU11 anaSuperTrendU11(int basePeriod, string confirmedDownTrend, string confirmedUpTrend, DashStyle dash0Style, DashStyle dash1Style, Color downColor, double multiplier, int opacity, PlotStyle plot0Style, int plot0Width, PlotStyle plot1Style, int plot1Width, string potentialDownTrend, string potentialUpTrend, int rangePeriod, int rearmTime, bool reverseIntraBar, bool showArrows, bool showPaintBars, bool showStopLine, bool soundAlert, anaSuperTrendU11BaseType thisBaseType, anaSuperTrendU11OffsetType thisOffsetType, anaSuperTrendU11VolaType thisRangeType, Color upColor)
        {
            return _indicator.anaSuperTrendU11(Input, basePeriod, confirmedDownTrend, confirmedUpTrend, dash0Style, dash1Style, downColor, multiplier, opacity, plot0Style, plot0Width, plot1Style, plot1Width, potentialDownTrend, potentialUpTrend, rangePeriod, rearmTime, reverseIntraBar, showArrows, showPaintBars, showStopLine, soundAlert, thisBaseType, thisOffsetType, thisRangeType, upColor);
        }

        /// <summary>
        /// SuperTrend
        /// </summary>
        /// <returns></returns>
        public Indicator.anaSuperTrendU11 anaSuperTrendU11(Data.IDataSeries input, int basePeriod, string confirmedDownTrend, string confirmedUpTrend, DashStyle dash0Style, DashStyle dash1Style, Color downColor, double multiplier, int opacity, PlotStyle plot0Style, int plot0Width, PlotStyle plot1Style, int plot1Width, string potentialDownTrend, string potentialUpTrend, int rangePeriod, int rearmTime, bool reverseIntraBar, bool showArrows, bool showPaintBars, bool showStopLine, bool soundAlert, anaSuperTrendU11BaseType thisBaseType, anaSuperTrendU11OffsetType thisOffsetType, anaSuperTrendU11VolaType thisRangeType, Color upColor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaSuperTrendU11(input, basePeriod, confirmedDownTrend, confirmedUpTrend, dash0Style, dash1Style, downColor, multiplier, opacity, plot0Style, plot0Width, plot1Style, plot1Width, potentialDownTrend, potentialUpTrend, rangePeriod, rearmTime, reverseIntraBar, showArrows, showPaintBars, showStopLine, soundAlert, thisBaseType, thisOffsetType, thisRangeType, upColor);
        }
    }
}
#endregion
