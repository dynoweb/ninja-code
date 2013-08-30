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

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// anaSuperTrend Indicator
    /// </summary>
    [Description("Modified SuperTrend Indicator based on a Moving Median")]
    public class anaSuperTrend : Indicator
    {
        #region Variables
			private int periodMedian		= 3; // Default setting for Median Period
            private int periodATR			= 3; // Default setting for Range Period
            private double multiplier 		= 1; // Default setting for Multiplier
			private bool showArrows 		= true;
			private bool showStopLine		= true;
			private bool paintBars 			= false;
			private bool currentUpTrend		= true;
			private bool priorUpTrend		= true;
			private bool gap				= false;
			private double offset			= 0;
			private double newStop			= 0;
			private double priorStop		= 0;
			private int plot0Width 			= 2;
			private PlotStyle plot0Style	= PlotStyle.Line;
			private DashStyle dash0Style	= DashStyle.Dash;
			private Color upColor			= Color.Blue;
			private Color downColor			= Color.Red;
			private Color neutralColor 		= Color.Transparent;
			private Color priorColor		= Color.Empty;
			private BoolSeries upTrend;
			private anaMovingMedian MM;
			private ATR MAE;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Gray, PlotStyle.Line, "StopLine"));
			CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			PlotsConfigurable = false;
			upTrend = new BoolSeries(this);
        }

		protected override void OnStartUp()
		{
			Plots[0].Pen.Width = plot0Width;
			Plots[0].PlotStyle = plot0Style;
			Plots[0].Pen.DashStyle = dash0Style;
			gap = (plot0Style == PlotStyle.Line)||(plot0Style == PlotStyle.Square);
			if (ShowStopLine)
				Plots[0].Pen.Color = Color.Gray;
			else 
				Plots[0].Pen.Color = Color.Transparent;
			MM = anaMovingMedian(Medians[0], periodMedian);
			MAE = ATR(Closes[0],periodATR);
		}        
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		protected override void OnBarUpdate()
        {
			if (CurrentBar == 0)
			{ 
				upTrend.Set(true);
				StopLine.Set (Close[0]); 
				PlotColors[0][0] = neutralColor;
				return; 
			}
			if (FirstTickOfBar)
			{
				priorUpTrend = upTrend[1];
				priorStop = StopLine[1];
				priorColor = PlotColors[0][1];
				offset = MAE[1];
			}
			if (Close[0] > priorStop)
			{
				upTrend.Set(true);
				newStop = MM[0] - Multiplier * offset;
				currentUpTrend = true;
				if (!priorUpTrend) // trend change up
					StopLine.Set(newStop);
				else
					StopLine.Set(Math.Max(newStop, priorStop));
			}
			else if (Close[0] < priorStop)
			{
				upTrend.Set(false);
				newStop = MM[0]+ Multiplier * offset;
				currentUpTrend = false;
				if (priorUpTrend) // trend change down
					StopLine.Set(newStop);
				else
					StopLine.Set(Math.Min(newStop,priorStop));
			}
			else
			{
				upTrend.Set(priorUpTrend);
				currentUpTrend = priorUpTrend;
				StopLine.Set(priorStop);
			}
			
			if(PaintBars)
			{
				if (currentUpTrend)
				{
					CandleOutlineColor = upColor;
					if(Open[0] < Close[0] && ChartControl.ChartStyleType == ChartStyleType.CandleStick ) 
					BarColor  = Color.Transparent;
				else
					BarColor  = upColor;
				}
				else
				{
					CandleOutlineColor = downColor;
					if(Open[0] < Close[0] && ChartControl.ChartStyleType == ChartStyleType.CandleStick ) 
					BarColor  = Color.Transparent;
				else
					BarColor  = downColor;
				}
			}
			if(ShowArrows)
			{
				if(currentUpTrend && !priorUpTrend)
					DrawArrowUp("arrow" + CurrentBar, true, 0, newStop - 0.5*offset, upColor);
				else if(!currentUpTrend && priorUpTrend)
					DrawArrowDown("arrow" + CurrentBar, true, 0, newStop + 0.5*offset, downColor);
				else
					RemoveDrawObject("arrow" + CurrentBar);
			}
			if(ShowStopLine)
			{
				if(currentUpTrend && !priorUpTrend)
				{
					if (gap)
						PlotColors[0][0]= neutralColor;
					else
						PlotColors[0][0] = upColor;
				}
				else if (currentUpTrend)
					PlotColors[0][0] = upColor;
				else if(!currentUpTrend && priorUpTrend)
				{
					if (gap)
						PlotColors[0][0]= neutralColor;
					else
						PlotColors[0][0] = downColor;
				}
				else
					PlotColors[0][0] = downColor;
			}
        }

        #region Properties
        [Browsable(false)]	
        [XmlIgnore()]		
        public DataSeries StopLine
        {
            get { return Values[0]; }
        }

		[Browsable(false)]
        [XmlIgnore()]		
        public BoolSeries UpTrend
        {
            get { return upTrend; }
        }

		[Description("Median Period")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("Median Period")]
        public int PeriodMedian
        {
            get { return periodMedian; }
            set { periodMedian = Math.Max(1, value); }
        }

		[Description("ATR Period")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("ATR Period")]
        public int PeriodATR
        {
            get { return periodATR; }
            set { periodATR = Math.Max(1, value); }
        }

        [Description("ATR Multiplier")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("ATR Multiplier")]
        public double Multiplier
        {
            get { return multiplier; }
            set { multiplier = Math.Max(0.0, value); }
        }
		
		[Description("Show Arrows when Trendline is violated?")]
        [Category("Options")]
		[Gui.Design.DisplayName ("Show Arrows")]
        public bool ShowArrows
        {
            get { return showArrows; }
            set { showArrows = value; }
        }

		[Description("Show Stop Line")]
        [Category("Options")]
		[Gui.Design.DisplayName ("Show Stop Line")]
        public bool ShowStopLine
        {
            get { return showStopLine; }
            set { showStopLine = value; }
        }

		[Description("Color the bars in the direction of the trend?")]
        [Category("Options")]
		[Gui.Design.DisplayName ("Paint Bars")]
        public bool PaintBars
        {
            get { return paintBars; }
            set { paintBars = value; }
        }

		/// <summary>
		/// </summary>
		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
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
		[Description("Select color for downtrend")]
		[Category("Plot Colors")]
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
		[Description("Width for Stop Line Line.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width Stop Line")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for Stop Line Line.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Stop Line")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for Stop Line Line.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Stop Line")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
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
        private anaSuperTrend[] cacheanaSuperTrend = null;

        private static anaSuperTrend checkanaSuperTrend = new anaSuperTrend();

        /// <summary>
        /// Modified SuperTrend Indicator based on a Moving Median
        /// </summary>
        /// <returns></returns>
        public anaSuperTrend anaSuperTrend(double multiplier, int periodATR, int periodMedian)
        {
            return anaSuperTrend(Input, multiplier, periodATR, periodMedian);
        }

        /// <summary>
        /// Modified SuperTrend Indicator based on a Moving Median
        /// </summary>
        /// <returns></returns>
        public anaSuperTrend anaSuperTrend(Data.IDataSeries input, double multiplier, int periodATR, int periodMedian)
        {
            if (cacheanaSuperTrend != null)
                for (int idx = 0; idx < cacheanaSuperTrend.Length; idx++)
                    if (Math.Abs(cacheanaSuperTrend[idx].Multiplier - multiplier) <= double.Epsilon && cacheanaSuperTrend[idx].PeriodATR == periodATR && cacheanaSuperTrend[idx].PeriodMedian == periodMedian && cacheanaSuperTrend[idx].EqualsInput(input))
                        return cacheanaSuperTrend[idx];

            lock (checkanaSuperTrend)
            {
                checkanaSuperTrend.Multiplier = multiplier;
                multiplier = checkanaSuperTrend.Multiplier;
                checkanaSuperTrend.PeriodATR = periodATR;
                periodATR = checkanaSuperTrend.PeriodATR;
                checkanaSuperTrend.PeriodMedian = periodMedian;
                periodMedian = checkanaSuperTrend.PeriodMedian;

                if (cacheanaSuperTrend != null)
                    for (int idx = 0; idx < cacheanaSuperTrend.Length; idx++)
                        if (Math.Abs(cacheanaSuperTrend[idx].Multiplier - multiplier) <= double.Epsilon && cacheanaSuperTrend[idx].PeriodATR == periodATR && cacheanaSuperTrend[idx].PeriodMedian == periodMedian && cacheanaSuperTrend[idx].EqualsInput(input))
                            return cacheanaSuperTrend[idx];

                anaSuperTrend indicator = new anaSuperTrend();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Multiplier = multiplier;
                indicator.PeriodATR = periodATR;
                indicator.PeriodMedian = periodMedian;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaSuperTrend[] tmp = new anaSuperTrend[cacheanaSuperTrend == null ? 1 : cacheanaSuperTrend.Length + 1];
                if (cacheanaSuperTrend != null)
                    cacheanaSuperTrend.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaSuperTrend = tmp;
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
        /// Modified SuperTrend Indicator based on a Moving Median
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaSuperTrend anaSuperTrend(double multiplier, int periodATR, int periodMedian)
        {
            return _indicator.anaSuperTrend(Input, multiplier, periodATR, periodMedian);
        }

        /// <summary>
        /// Modified SuperTrend Indicator based on a Moving Median
        /// </summary>
        /// <returns></returns>
        public Indicator.anaSuperTrend anaSuperTrend(Data.IDataSeries input, double multiplier, int periodATR, int periodMedian)
        {
            return _indicator.anaSuperTrend(input, multiplier, periodATR, periodMedian);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Modified SuperTrend Indicator based on a Moving Median
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaSuperTrend anaSuperTrend(double multiplier, int periodATR, int periodMedian)
        {
            return _indicator.anaSuperTrend(Input, multiplier, periodATR, periodMedian);
        }

        /// <summary>
        /// Modified SuperTrend Indicator based on a Moving Median
        /// </summary>
        /// <returns></returns>
        public Indicator.anaSuperTrend anaSuperTrend(Data.IDataSeries input, double multiplier, int periodATR, int periodMedian)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaSuperTrend(input, multiplier, periodATR, periodMedian);
        }
    }
}
#endregion
