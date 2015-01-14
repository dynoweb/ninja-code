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
	/// The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
	/// </summary>
	[Description("The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.")]
	public class StochasticRSI : Indicator
	{
		#region Variables
		private int			periodRSI = 14;
		private int			periodK	  =  8;	// Kperiod
		private int			periodD	  =  5;	// SlowDperiod
		private int			smooth	  =  3;
		private DataSeries      fastK;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "StochasticRSI"));

			Add(new Line(Color.Red,  80, "Overbought"));
			//Add(new Line(Color.Blue, 0.5, "Neutral"));
			Add(new Line(Color.Red,  20, "Oversold"));

			fastK       = new DataSeries(this);
			Overlay				= false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double rsi  = RSI(Inputs[0], periodRSI, 0)[0];
			double rsiL = MIN(RSI(Inputs[0], periodRSI, 0), periodK)[0];
			double rsiH = MAX(RSI(Inputs[0], periodRSI, 0), periodK)[0];

			if (rsi != rsiL && rsiH != rsiL)
			{
				//Value.Set((rsi - rsiL) / (rsiH - rsiL));
				fastK.Set(100*(rsi - rsiL) / (rsiH - rsiL));
				Value.Set(SMA(fastK, Smooth)[0]);
			}
			else
				Value.Set(0);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int PeriodRSI
		{
			get { return periodRSI; }
			set { periodRSI = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for moving average over D values")]
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
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private StochasticRSI[] cacheStochasticRSI = null;

        private static StochasticRSI checkStochasticRSI = new StochasticRSI();

        /// <summary>
        /// The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public StochasticRSI StochasticRSI(int periodD, int periodK, int periodRSI, int smooth)
        {
            return StochasticRSI(Input, periodD, periodK, periodRSI, smooth);
        }

        /// <summary>
        /// The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public StochasticRSI StochasticRSI(Data.IDataSeries input, int periodD, int periodK, int periodRSI, int smooth)
        {
            if (cacheStochasticRSI != null)
                for (int idx = 0; idx < cacheStochasticRSI.Length; idx++)
                    if (cacheStochasticRSI[idx].PeriodD == periodD && cacheStochasticRSI[idx].PeriodK == periodK && cacheStochasticRSI[idx].PeriodRSI == periodRSI && cacheStochasticRSI[idx].Smooth == smooth && cacheStochasticRSI[idx].EqualsInput(input))
                        return cacheStochasticRSI[idx];

            lock (checkStochasticRSI)
            {
                checkStochasticRSI.PeriodD = periodD;
                periodD = checkStochasticRSI.PeriodD;
                checkStochasticRSI.PeriodK = periodK;
                periodK = checkStochasticRSI.PeriodK;
                checkStochasticRSI.PeriodRSI = periodRSI;
                periodRSI = checkStochasticRSI.PeriodRSI;
                checkStochasticRSI.Smooth = smooth;
                smooth = checkStochasticRSI.Smooth;

                if (cacheStochasticRSI != null)
                    for (int idx = 0; idx < cacheStochasticRSI.Length; idx++)
                        if (cacheStochasticRSI[idx].PeriodD == periodD && cacheStochasticRSI[idx].PeriodK == periodK && cacheStochasticRSI[idx].PeriodRSI == periodRSI && cacheStochasticRSI[idx].Smooth == smooth && cacheStochasticRSI[idx].EqualsInput(input))
                            return cacheStochasticRSI[idx];

                StochasticRSI indicator = new StochasticRSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodD = periodD;
                indicator.PeriodK = periodK;
                indicator.PeriodRSI = periodRSI;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                StochasticRSI[] tmp = new StochasticRSI[cacheStochasticRSI == null ? 1 : cacheStochasticRSI.Length + 1];
                if (cacheStochasticRSI != null)
                    cacheStochasticRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStochasticRSI = tmp;
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
        /// The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StochasticRSI StochasticRSI(int periodD, int periodK, int periodRSI, int smooth)
        {
            return _indicator.StochasticRSI(Input, periodD, periodK, periodRSI, smooth);
        }

        /// <summary>
        /// The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticRSI StochasticRSI(Data.IDataSeries input, int periodD, int periodK, int periodRSI, int smooth)
        {
            return _indicator.StochasticRSI(input, periodD, periodK, periodRSI, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StochasticRSI StochasticRSI(int periodD, int periodK, int periodRSI, int smooth)
        {
            return _indicator.StochasticRSI(Input, periodD, periodK, periodRSI, smooth);
        }

        /// <summary>
        /// The StochasticRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochasticRSI uses RSI values. The StochasticRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochasticRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticRSI StochasticRSI(Data.IDataSeries input, int periodD, int periodK, int periodRSI, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.StochasticRSI(input, periodD, periodK, periodRSI, smooth);
        }
    }
}
#endregion
