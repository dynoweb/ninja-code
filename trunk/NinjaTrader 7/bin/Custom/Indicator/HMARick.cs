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
	/// <summary>
	/// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
	/// This indicator is based on the reference article found here:
	/// http://www.justdata.com.au/Journals/AlanHull/hull_ma.htm
	/// </summary>
	[Description("The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.")]
    public class HMARick : Indicator
    {
        #region Variables
            private int period = 55; // Default setting for Period
			private DataSeries diffSeries;
		
			private Color upColor			= Color.DarkGreen;
			private Color downColor			= Color.Red;
			private int plot0Width 			= 2;
			private PlotStyle plot0Style	= PlotStyle.Line;
			private DashStyle dash0Style	= DashStyle.Solid;
		
			// added to get neutral slope
			private Color neutralColor	= Color.Gray;
			private int threshold = 50;
			private double middle = 0.0;
			private double neutralSlope	= 0.0;
			private IDataSeries averageTrueRange;
			private DataSeries trendSet;  //  Short == -1, Neutral == 0,  Long == 1

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Gray, PlotStyle.Line, "HmaPlot"));
			PlotsConfigurable = false;	
			
            Overlay				= true;
			diffSeries			= new DataSeries(this);
			trendSet 			= new DataSeries(this);
        }

		protected override void OnStartUp()
		{
			Plots[0].Pen.Width = plot0Width;
			Plots[0].PlotStyle = plot0Style;
			Plots[0].Pen.DashStyle = dash0Style;
			Plots[0].Pen.Color = Color.Gray;
			
			// added to get neutral slope
			averageTrueRange = ATR(20);
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBars[0] <= BarsRequired)
        		return;
			
			double value1 = 2 * WMA(Inputs[0], (int)(Period / 2))[0];
			double value2 = WMA(Inputs[0], Period)[0];
			diffSeries.Set(value1 - value2);

			// added to get neutral slope
			if(FirstTickOfBar)
			{
				neutralSlope =  threshold * averageTrueRange[1] / 1000;
				middle = Value[1] + Value[2];
			}

			double averageValue = WMA(diffSeries, (int) Math.Sqrt(Period))[0];
			//Print(" averageValue: " + averageValue + " middle: " + middle + " neutralSlope: " + neutralSlope);
			
			if (2 * averageValue > middle + 3*neutralSlope)
			{
				PlotColors[0][0] = UpColor;
				TrendSet.Set(1);
			}
			else if (2*averageValue < middle - 3*neutralSlope)
			{
				PlotColors[0][0] = DownColor;
				TrendSet.Set(-1);
			}
			else
			{
				PlotColors[0][0] = NeutralColor;
				TrendSet.Set(0);
			}
				
			//if (Rising(WMA(diffSeries, (int) Math.Sqrt(Period))))
			//{
			//	PlotColors[0][0] = UpColor;
			//}
			//else
			//{
			//	PlotColors[0][0] = downColor;
			//}
			
			Value.Set(WMA(diffSeries, (int) Math.Sqrt(Period))[0]);
		}

		#region Properties

		/// </summary>
		[Description("Slope as percentage of average true range")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Neutral Threshold")]
		public int Threshold 
		{
			get { return threshold; }
			set { threshold = Math.Max(0, value); }
		}

		[Description("Select color for Neutral Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Neutral")]
		public Color NeutralColor
		{
			get { return neutralColor; }
			set { neutralColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string NeutralColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(neutralColor); }
			set { neutralColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
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
		
		[Description("Width for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		[Description("PlotStyle for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Line")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		[Description("DashStyle for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Line")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 

		/// <summary>
		/// Period
		/// </summary>
		[Description("Number of bars used for calculation")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries TrendSet
		{
			get { return trendSet; }
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
        private HMARick[] cacheHMARick = null;

        private static HMARick checkHMARick = new HMARick();

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public HMARick HMARick(int period, int threshold)
        {
            return HMARick(Input, period, threshold);
        }

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public HMARick HMARick(Data.IDataSeries input, int period, int threshold)
        {
            if (cacheHMARick != null)
                for (int idx = 0; idx < cacheHMARick.Length; idx++)
                    if (cacheHMARick[idx].Period == period && cacheHMARick[idx].Threshold == threshold && cacheHMARick[idx].EqualsInput(input))
                        return cacheHMARick[idx];

            lock (checkHMARick)
            {
                checkHMARick.Period = period;
                period = checkHMARick.Period;
                checkHMARick.Threshold = threshold;
                threshold = checkHMARick.Threshold;

                if (cacheHMARick != null)
                    for (int idx = 0; idx < cacheHMARick.Length; idx++)
                        if (cacheHMARick[idx].Period == period && cacheHMARick[idx].Threshold == threshold && cacheHMARick[idx].EqualsInput(input))
                            return cacheHMARick[idx];

                HMARick indicator = new HMARick();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Threshold = threshold;
                Indicators.Add(indicator);
                indicator.SetUp();

                HMARick[] tmp = new HMARick[cacheHMARick == null ? 1 : cacheHMARick.Length + 1];
                if (cacheHMARick != null)
                    cacheHMARick.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHMARick = tmp;
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
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HMARick HMARick(int period, int threshold)
        {
            return _indicator.HMARick(Input, period, threshold);
        }

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public Indicator.HMARick HMARick(Data.IDataSeries input, int period, int threshold)
        {
            return _indicator.HMARick(input, period, threshold);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HMARick HMARick(int period, int threshold)
        {
            return _indicator.HMARick(Input, period, threshold);
        }

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public Indicator.HMARick HMARick(Data.IDataSeries input, int period, int threshold)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HMARick(input, period, threshold);
        }
    }
}
#endregion
