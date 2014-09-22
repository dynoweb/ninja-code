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
	/// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
	/// </summary>
	[Description("The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.")]
	public class StochasticsCyclesColors : Indicator
	{
		#region Variables
		private int				periodD	= 3;	// SlowDperiod
		private int				periodK	= 5;	// Kperiod
		private int				smooth	= 2;	// SlowKperiod
		private DataSeries		den;
		private DataSeries		nom;
        private DataSeries      fastK;
		private int 			trendBars = 50;	// SMA Period
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

			//Add(new Line(Color.Red, 45, "Lower"));
			//Add(new Line(Color.Red, 55, "Upper"));
			
			Pen dashPen = new Pen(Color.Black);
			dashPen.DashStyle = DashStyle.Dash;
			Add(new Line(dashPen, 80, "Over"));			
			Add(new Line(dashPen, 20, "Under"));
				
			den			= new DataSeries(this);
			nom			= new DataSeries(this);
            fastK       = new DataSeries(this);
			
			//Print("Type: " + BarsPeriod.BasePeriodType + " value: " + BarsPeriods[1].Value);
			Add(PeriodType.Tick, BarsPeriods[1].Value);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			// Do not calculate if we don't have enough bars 
			if (CurrentBar < trendBars) return;
			
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
/*		
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
*/			
/*			if (Falling(SMA(TrendBars)))
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
*//*			if (Rising(SMA(TrendBars)))
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
*/						
			if (BarsInProgress == 1)
			{
				if (Falling(SMA(BarsArray[1], TrendBars)))
				{
					if (K[0] <= K[1])
						BackColor = Color.Pink;
					if (D[0] <= D[1])
						BackColor = Color.Red;
				}
				if (Rising(SMA(BarsArray[1], TrendBars)))
				{
					if (K[0] >= K[1])
						BackColor = Color.LightGreen;
					if (D[0] >= D[1])
						BackColor = Color.Green;
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
		[Description("Number of bars for SMA trend")]
		[GridCategory("Parameters")]
		public int TrendBars
		{
			get { return trendBars; }
			set { trendBars = Math.Max(1, value); }
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
        private StochasticsCyclesColors[] cacheStochasticsCyclesColors = null;

        private static StochasticsCyclesColors checkStochasticsCyclesColors = new StochasticsCyclesColors();

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public StochasticsCyclesColors StochasticsCyclesColors(int periodD, int periodK, int smooth, int trendBars)
        {
            return StochasticsCyclesColors(Input, periodD, periodK, smooth, trendBars);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public StochasticsCyclesColors StochasticsCyclesColors(Data.IDataSeries input, int periodD, int periodK, int smooth, int trendBars)
        {
            if (cacheStochasticsCyclesColors != null)
                for (int idx = 0; idx < cacheStochasticsCyclesColors.Length; idx++)
                    if (cacheStochasticsCyclesColors[idx].PeriodD == periodD && cacheStochasticsCyclesColors[idx].PeriodK == periodK && cacheStochasticsCyclesColors[idx].Smooth == smooth && cacheStochasticsCyclesColors[idx].TrendBars == trendBars && cacheStochasticsCyclesColors[idx].EqualsInput(input))
                        return cacheStochasticsCyclesColors[idx];

            lock (checkStochasticsCyclesColors)
            {
                checkStochasticsCyclesColors.PeriodD = periodD;
                periodD = checkStochasticsCyclesColors.PeriodD;
                checkStochasticsCyclesColors.PeriodK = periodK;
                periodK = checkStochasticsCyclesColors.PeriodK;
                checkStochasticsCyclesColors.Smooth = smooth;
                smooth = checkStochasticsCyclesColors.Smooth;
                checkStochasticsCyclesColors.TrendBars = trendBars;
                trendBars = checkStochasticsCyclesColors.TrendBars;

                if (cacheStochasticsCyclesColors != null)
                    for (int idx = 0; idx < cacheStochasticsCyclesColors.Length; idx++)
                        if (cacheStochasticsCyclesColors[idx].PeriodD == periodD && cacheStochasticsCyclesColors[idx].PeriodK == periodK && cacheStochasticsCyclesColors[idx].Smooth == smooth && cacheStochasticsCyclesColors[idx].TrendBars == trendBars && cacheStochasticsCyclesColors[idx].EqualsInput(input))
                            return cacheStochasticsCyclesColors[idx];

                StochasticsCyclesColors indicator = new StochasticsCyclesColors();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodD = periodD;
                indicator.PeriodK = periodK;
                indicator.Smooth = smooth;
                indicator.TrendBars = trendBars;
                Indicators.Add(indicator);
                indicator.SetUp();

                StochasticsCyclesColors[] tmp = new StochasticsCyclesColors[cacheStochasticsCyclesColors == null ? 1 : cacheStochasticsCyclesColors.Length + 1];
                if (cacheStochasticsCyclesColors != null)
                    cacheStochasticsCyclesColors.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStochasticsCyclesColors = tmp;
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
        public Indicator.StochasticsCyclesColors StochasticsCyclesColors(int periodD, int periodK, int smooth, int trendBars)
        {
            return _indicator.StochasticsCyclesColors(Input, periodD, periodK, smooth, trendBars);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticsCyclesColors StochasticsCyclesColors(Data.IDataSeries input, int periodD, int periodK, int smooth, int trendBars)
        {
            return _indicator.StochasticsCyclesColors(input, periodD, periodK, smooth, trendBars);
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
        public Indicator.StochasticsCyclesColors StochasticsCyclesColors(int periodD, int periodK, int smooth, int trendBars)
        {
            return _indicator.StochasticsCyclesColors(Input, periodD, periodK, smooth, trendBars);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticsCyclesColors StochasticsCyclesColors(Data.IDataSeries input, int periodD, int periodK, int smooth, int trendBars)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.StochasticsCyclesColors(input, periodD, periodK, smooth, trendBars);
        }
    }
}
#endregion
