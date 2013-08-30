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
    /// Shows how perdictable the instrument trend
    /// </summary>
    [Description("Shows how perdictable the instrument trend")]
    public class TrendPredictability : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 100; // Default setting for Period
            private int maxConsecutiveBars = 4; // Default setting for MaxConsecutiveBars
        // User defined variables (add any user defined variables below)
			double[] trendPercent = new double[3];
			int[] inTrend;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Bar, "AvgPlot"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "SecondPlot"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 0.50, "Midpoint"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Do not calculate if we don't have enough bars 
			if (CurrentBar < Period + maxConsecutiveBars) return;
			inTrend = new int[maxConsecutiveBars];
			int barsAgo = 0;
			
			// loop for each displayed bar, related to consecutive moves in one direction over a period of time 
			for (barsAgo = 0; barsAgo < maxConsecutiveBars; barsAgo++) 
			{
				// for this ploted bar, count the number of bars in consecutive trend
				for (int p = 0; p < Period; p++) 
				{
					
				}
			}
			
//			for (int barsAgo = 0; barsAgo < maxConsecutiveBars; barsAgo++) 
//			{
				inTrend[barsAgo] = 0;
				for (int x = 0; x < Period; x++)
				{
					bool pTrend = true;
					bool nTrend = true;
					for (int index = 3; index < maxConsecutiveBars && (pTrend || nTrend); index++) 
					{							
						if (Close[x-index] >= Close[x-index+1] && pTrend) 
						{
							pTrend = true;
						} else {
							pTrend = false;
						}
						if (Close[x-index] <= Close[x-index+1] && nTrend) 
						{
							nTrend = true;
						} else {
							nTrend = false;							
						}
						if (nTrend || pTrend)
						{
							Print(CurrentBar + " inTrend[barsAgo]: " + inTrend[barsAgo]);
						}
					}
					if (pTrend) 
					{
						inTrend[barsAgo]++;									
					}
					if (nTrend)
					{
						inTrend[barsAgo]--;									
					}
							
						//if (((Close[x-1] >= Close[x-2]) 
						//	&& Close[x] >= Close[x-1]))
//							|| ((Close[x-1] <= Close[x-2]) && Close[x] <= Close[x-1]))
						//{
						//	inTrend++;
						//} 
					
				}
				double trendPercent = (inTrend[barsAgo] * 1.0) / Period;
				Print(CurrentBar + " inTrend " + inTrend[barsAgo] + " trendPercent " + trendPercent);
				AvgPlot.Set(barsAgo, (inTrend[barsAgo] * 1.0) / Period);
				//AvgPlot.Set(maxBars, (inTrend * 1.0) / Period);
//			}
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            //AvgPlot.Set(trendPercent);
            SecondPlot.Set(0.1);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AvgPlot
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SecondPlot
        {
            get { return Values[1]; }
        }

        [Description("Number of historical bars to consider")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(20, value); }
        }

        [Description("Number of bars in trend to measure")]
        [GridCategory("Parameters")]
        public int MaxConsecutiveBars
        {
            get { return maxConsecutiveBars; }
            set { maxConsecutiveBars = Math.Max(1, value); }
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
        private TrendPredictability[] cacheTrendPredictability = null;

        private static TrendPredictability checkTrendPredictability = new TrendPredictability();

        /// <summary>
        /// Shows how perdictable the instrument trend
        /// </summary>
        /// <returns></returns>
        public TrendPredictability TrendPredictability(int maxConsecutiveBars, int period)
        {
            return TrendPredictability(Input, maxConsecutiveBars, period);
        }

        /// <summary>
        /// Shows how perdictable the instrument trend
        /// </summary>
        /// <returns></returns>
        public TrendPredictability TrendPredictability(Data.IDataSeries input, int maxConsecutiveBars, int period)
        {
            if (cacheTrendPredictability != null)
                for (int idx = 0; idx < cacheTrendPredictability.Length; idx++)
                    if (cacheTrendPredictability[idx].MaxConsecutiveBars == maxConsecutiveBars && cacheTrendPredictability[idx].Period == period && cacheTrendPredictability[idx].EqualsInput(input))
                        return cacheTrendPredictability[idx];

            lock (checkTrendPredictability)
            {
                checkTrendPredictability.MaxConsecutiveBars = maxConsecutiveBars;
                maxConsecutiveBars = checkTrendPredictability.MaxConsecutiveBars;
                checkTrendPredictability.Period = period;
                period = checkTrendPredictability.Period;

                if (cacheTrendPredictability != null)
                    for (int idx = 0; idx < cacheTrendPredictability.Length; idx++)
                        if (cacheTrendPredictability[idx].MaxConsecutiveBars == maxConsecutiveBars && cacheTrendPredictability[idx].Period == period && cacheTrendPredictability[idx].EqualsInput(input))
                            return cacheTrendPredictability[idx];

                TrendPredictability indicator = new TrendPredictability();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MaxConsecutiveBars = maxConsecutiveBars;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                TrendPredictability[] tmp = new TrendPredictability[cacheTrendPredictability == null ? 1 : cacheTrendPredictability.Length + 1];
                if (cacheTrendPredictability != null)
                    cacheTrendPredictability.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTrendPredictability = tmp;
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
        /// Shows how perdictable the instrument trend
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendPredictability TrendPredictability(int maxConsecutiveBars, int period)
        {
            return _indicator.TrendPredictability(Input, maxConsecutiveBars, period);
        }

        /// <summary>
        /// Shows how perdictable the instrument trend
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendPredictability TrendPredictability(Data.IDataSeries input, int maxConsecutiveBars, int period)
        {
            return _indicator.TrendPredictability(input, maxConsecutiveBars, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Shows how perdictable the instrument trend
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendPredictability TrendPredictability(int maxConsecutiveBars, int period)
        {
            return _indicator.TrendPredictability(Input, maxConsecutiveBars, period);
        }

        /// <summary>
        /// Shows how perdictable the instrument trend
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendPredictability TrendPredictability(Data.IDataSeries input, int maxConsecutiveBars, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TrendPredictability(input, maxConsecutiveBars, period);
        }
    }
}
#endregion
