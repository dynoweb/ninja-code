// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. 
	/// The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. 
	/// The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and 
	/// selling when fast moves below slow.
	/// </summary>
	[Description("The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.")]
	public class StochasticsCycles : Indicator
	{
		#region Variables
		private int				periodD	= 3;	// SlowDperiod
		private int				periodK	= 5;	// Kperiod
		private int				smooth	= 2;	// SlowKperiod
		private DataSeries		den;
		private DataSeries		nom;
        private DataSeries      fastK;
		//private int 			trendBars = 50;	// SMA Period
		private int 			trendBars = 100; // HMA Period
		Stochastics stocMT = null;	// Mid Term Stochastics
		int threshold = 60;
		
		bool showHighsAndLows = false;
		bool showHigherTimeframe = false;
		int timeFrameMultiplier = 3;
		
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Plot dPlot = new Plot(Color.Blue, "D");
			dPlot.Pen.Width = 2;
			Add(dPlot);
			Add(new Plot(Color.Black, "K"));
            //Add(new Plot(Color.Black, PlotStyle.Line, "Line1"));
            //Add(new Plot(Color.Black, PlotStyle.Line, "Line2"));

			Add(new Line(Color.Red, 45, "Lower"));
			Add(new Line(Color.Red, 55, "Upper"));
			
			Pen dashPen = new Pen(Color.Black);
			dashPen.DashStyle = DashStyle.Dash;
			Add(new Line(dashPen, 80, "Over"));			
			Add(new Line(dashPen, 20, "Under"));
				
			den			= new DataSeries(this);
			nom			= new DataSeries(this);
            fastK       = new DataSeries(this);
			
			// BarsPeriod.Value, PeriodType.Tick
			
			if (showHigherTimeframe && (BarsPeriod.Id == PeriodType.Tick || BarsPeriod.Id == PeriodType.Minute || BarsPeriod.Id == PeriodType.Volume))
			{
				Print("Instrument.FullName: " + Instrument.FullName + " BarsPeriod.Id: " + BarsPeriod.Id + " BarsPeriod.Value: " + BarsPeriod.Value);
				Add(Instrument.FullName, BarsPeriod.Id, BarsPeriod.Value * timeFrameMultiplier);
			}
			//stocMT = Stochastics(BarsArray[1], periodD, periodK, smooth); 
			//Add(Stochastics(stocMT[1], periodD, periodK, smooth));
			//Add(Stochastics(BarsArray[1], periodD, periodK, smooth)); 
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			//Print(Time + " BarsInProgress " + BarsInProgress );
			

			// Check which Bars object is calling the OnBarUpdate() method 
			if (BarsInProgress == 1) 
	        {					
				if (!showHigherTimeframe || CurrentBars[1] <= BarsRequired)
					return;
				
				stocMT = Stochastics(BarsArray[1], periodD, periodK, smooth); 
				if (stocMT.K[0] <= stocMT.K[1])
					BackColor = Color.Pink;
				if (stocMT.K[0] <= stocMT.K[1] && stocMT.D[0] <= stocMT.D[1])
					BackColor = Color.Red;
				if (stocMT.K[0] >= stocMT.K[1])
					BackColor = Color.Lime;
				if (stocMT.K[0] >= stocMT.K[1] && stocMT.D[0] >= stocMT.D[1])
					BackColor = Color.Green;
				
				return;
			}
			
		    // Checks to ensure all Bars objects contain enough bars before beginning
			if (BarsInProgress != 0 || CurrentBars[0] <= BarsRequired)
				return;
			
            nom.Set(Close[0] - MIN(Low, PeriodK)[0]);
            den.Set(MAX(High, PeriodK)[0] - MIN(Low, PeriodK)[0]);

            if (den[0].Compare(0, 0.000000000001) == 0)
                fastK.Set(CurrentBar == 0 ? 50 : fastK[1]);
            else
                fastK.Set(Math.Min(100, Math.Max(0, 100 * nom[0] / den[0])));

            // Slow %K == Fast %D
            K.Set(SMA(fastK, Smooth)[0]);
            D.Set(SMA(K, PeriodD)[0]);

			if (CurrentBar < 50)
				return;
			
			/// Here’s how we define Cycle highs and lows:
			/// A Cycle high is the highest high in price after %D goes above 45 and before it gets back below 55.
			/// A Cycle low is the lowest low in price after %D goes below 55 and before it gets back above 45.
		
			if (showHighsAndLows)
			{
				// Mark Cycle High
				if (CrossBelow(D, 55, 1)) 
				{
					//BackColor = Color.LimeGreen;
					int i = 2;
					while (CurrentBar - i > 0 && !CrossAbove(D, 45, i)) 
					{
						i++;
					}
					int barsAgo = HighestBar(High, i);
					DrawDot(CurrentBar + "high", false, barsAgo, High[barsAgo] + 5 * TickSize, Color.Green);
				}			
						
				// Mark Cycle Low
				if (CrossAbove(D, 45, 1)) 
				{
					//BackColor = Color.Yellow;
					int i = 2;
					while (CurrentBar - i > 0 && !CrossBelow(D, 55, i)) 
					{
						i++;
					}
					int barsAgo = LowestBar(Low, i);
					DrawDot(CurrentBar + "low", false, barsAgo, Low[barsAgo] - 5 * TickSize, Color.Red);
				}
			}
			
			if (Falling(HMARick(TrendBars,70)) && HMARick(TrendBars,threshold).TrendSet[0] != 0)
			{
				if (K[0] <= K[1] && K[1] >= 80)
				{
					DrawArrowDown(CurrentBar + "validShort", 0, High[0] + 2 * TickSize, Color.Pink);
				}
				if (D[0] <= D[1] && D[1] >= 80)
				{
					DrawArrowDown(CurrentBar + "validShort", 0, High[0] + 2 * TickSize, Color.Red);
				}
			}
			if (Rising(HMARick(TrendBars,70)) && HMARick(TrendBars,threshold).TrendSet[0] != 0)
			{
				if (K[0] >= K[1] && K[1] <= 20)
				{
					DrawArrowUp(CurrentBar + "validLong", 0, Low[0] - 2 * TickSize, Color.LightGreen);
				}
				if (D[0] >= D[1] && D[1] <= 20)
				{
					DrawArrowUp(CurrentBar + "validLong", 0, Low[0] - 2 * TickSize, Color.Green);
				}
			}
		}

		#region Properties
		/// <summary>
		/// Gets the slow D value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries D
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// Gets the slow K value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries K
		{
			get { return Values[1]; }
		}
		
		[Description("Show cycle highs and lows")]
        [GridCategory("Annotations")]
        public bool ShowHighsAndLows
        {
            get { return showHighsAndLows;}
            set { showHighsAndLows = value; }
        }

		[Description("Show colors from higher timeframe stochastics")]
        [GridCategory("Annotations")]
        public bool ShowHigherTimeframe
        {
            get { return showHigherTimeframe;}
            set { showHigherTimeframe = value; }
        }

		[Description("Higher timeframe multiplier")]
        [GridCategory("Annotations")]
        public int TimeFrameMultiplier
        {
            get { return timeFrameMultiplier;}
            set { timeFrameMultiplier = Math.Max(1, value); }
        }

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for moving average over K values")]
		[GridCategory("Parameters")]
		public int PeriodD
		{
			get { return periodD; }
			set { periodD = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculating the K values")]
		[GridCategory("Parameters")]
		public int PeriodK
		{
			get { return periodK; }
			set { periodK = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing the slow K values")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Number of bars for SMA or HAM trend")]
		[GridCategory("Parameters")]
		public int TrendBars
		{
			get { return trendBars; }
			set { trendBars = Math.Max(1, value); }
		}
		
		/// </summary>
		[Description("Neutral Slope Threshold as percentage of average true range, used in HMARick")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Neutral Threshold")]
		public int Threshold 
		{
			get { return threshold; }
			set { threshold = Math.Max(0, value); }
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
        private StochasticsCycles[] cacheStochasticsCycles = null;

        private static StochasticsCycles checkStochasticsCycles = new StochasticsCycles();

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public StochasticsCycles StochasticsCycles(int periodD, int periodK, bool showHigherTimeframe, bool showHighsAndLows, int smooth, int threshold, int timeFrameMultiplier, int trendBars)
        {
            return StochasticsCycles(Input, periodD, periodK, showHigherTimeframe, showHighsAndLows, smooth, threshold, timeFrameMultiplier, trendBars);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public StochasticsCycles StochasticsCycles(Data.IDataSeries input, int periodD, int periodK, bool showHigherTimeframe, bool showHighsAndLows, int smooth, int threshold, int timeFrameMultiplier, int trendBars)
        {
            if (cacheStochasticsCycles != null)
                for (int idx = 0; idx < cacheStochasticsCycles.Length; idx++)
                    if (cacheStochasticsCycles[idx].PeriodD == periodD && cacheStochasticsCycles[idx].PeriodK == periodK && cacheStochasticsCycles[idx].ShowHigherTimeframe == showHigherTimeframe && cacheStochasticsCycles[idx].ShowHighsAndLows == showHighsAndLows && cacheStochasticsCycles[idx].Smooth == smooth && cacheStochasticsCycles[idx].Threshold == threshold && cacheStochasticsCycles[idx].TimeFrameMultiplier == timeFrameMultiplier && cacheStochasticsCycles[idx].TrendBars == trendBars && cacheStochasticsCycles[idx].EqualsInput(input))
                        return cacheStochasticsCycles[idx];

            lock (checkStochasticsCycles)
            {
                checkStochasticsCycles.PeriodD = periodD;
                periodD = checkStochasticsCycles.PeriodD;
                checkStochasticsCycles.PeriodK = periodK;
                periodK = checkStochasticsCycles.PeriodK;
                checkStochasticsCycles.ShowHigherTimeframe = showHigherTimeframe;
                showHigherTimeframe = checkStochasticsCycles.ShowHigherTimeframe;
                checkStochasticsCycles.ShowHighsAndLows = showHighsAndLows;
                showHighsAndLows = checkStochasticsCycles.ShowHighsAndLows;
                checkStochasticsCycles.Smooth = smooth;
                smooth = checkStochasticsCycles.Smooth;
                checkStochasticsCycles.Threshold = threshold;
                threshold = checkStochasticsCycles.Threshold;
                checkStochasticsCycles.TimeFrameMultiplier = timeFrameMultiplier;
                timeFrameMultiplier = checkStochasticsCycles.TimeFrameMultiplier;
                checkStochasticsCycles.TrendBars = trendBars;
                trendBars = checkStochasticsCycles.TrendBars;

                if (cacheStochasticsCycles != null)
                    for (int idx = 0; idx < cacheStochasticsCycles.Length; idx++)
                        if (cacheStochasticsCycles[idx].PeriodD == periodD && cacheStochasticsCycles[idx].PeriodK == periodK && cacheStochasticsCycles[idx].ShowHigherTimeframe == showHigherTimeframe && cacheStochasticsCycles[idx].ShowHighsAndLows == showHighsAndLows && cacheStochasticsCycles[idx].Smooth == smooth && cacheStochasticsCycles[idx].Threshold == threshold && cacheStochasticsCycles[idx].TimeFrameMultiplier == timeFrameMultiplier && cacheStochasticsCycles[idx].TrendBars == trendBars && cacheStochasticsCycles[idx].EqualsInput(input))
                            return cacheStochasticsCycles[idx];

                StochasticsCycles indicator = new StochasticsCycles();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodD = periodD;
                indicator.PeriodK = periodK;
                indicator.ShowHigherTimeframe = showHigherTimeframe;
                indicator.ShowHighsAndLows = showHighsAndLows;
                indicator.Smooth = smooth;
                indicator.Threshold = threshold;
                indicator.TimeFrameMultiplier = timeFrameMultiplier;
                indicator.TrendBars = trendBars;
                Indicators.Add(indicator);
                indicator.SetUp();

                StochasticsCycles[] tmp = new StochasticsCycles[cacheStochasticsCycles == null ? 1 : cacheStochasticsCycles.Length + 1];
                if (cacheStochasticsCycles != null)
                    cacheStochasticsCycles.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStochasticsCycles = tmp;
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
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StochasticsCycles StochasticsCycles(int periodD, int periodK, bool showHigherTimeframe, bool showHighsAndLows, int smooth, int threshold, int timeFrameMultiplier, int trendBars)
        {
            return _indicator.StochasticsCycles(Input, periodD, periodK, showHigherTimeframe, showHighsAndLows, smooth, threshold, timeFrameMultiplier, trendBars);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticsCycles StochasticsCycles(Data.IDataSeries input, int periodD, int periodK, bool showHigherTimeframe, bool showHighsAndLows, int smooth, int threshold, int timeFrameMultiplier, int trendBars)
        {
            return _indicator.StochasticsCycles(input, periodD, periodK, showHigherTimeframe, showHighsAndLows, smooth, threshold, timeFrameMultiplier, trendBars);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StochasticsCycles StochasticsCycles(int periodD, int periodK, bool showHigherTimeframe, bool showHighsAndLows, int smooth, int threshold, int timeFrameMultiplier, int trendBars)
        {
            return _indicator.StochasticsCycles(Input, periodD, periodK, showHigherTimeframe, showHighsAndLows, smooth, threshold, timeFrameMultiplier, trendBars);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticsCycles StochasticsCycles(Data.IDataSeries input, int periodD, int periodK, bool showHigherTimeframe, bool showHighsAndLows, int smooth, int threshold, int timeFrameMultiplier, int trendBars)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.StochasticsCycles(input, periodD, periodK, showHigherTimeframe, showHighsAndLows, smooth, threshold, timeFrameMultiplier, trendBars);
        }
    }
}
#endregion
