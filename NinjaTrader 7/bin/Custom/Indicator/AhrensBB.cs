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
	/// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
	/// </summary>
	[Description("Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.")]
	public class AhrensBB : Indicator
	{
		#region Variables
		private	double		numStdDev	= 2;
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "Upper band"));
			Add(new Plot(Color.Orange, "Middle band"));
			Add(new Plot(Color.Orange, "Lower band"));

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
		    double smaValue    = AhrensMA(period).AhrensHL[0];
		    double stdDevValue = StdDev(Period)[0];
            Upper.Set(smaValue + NumStdDev * stdDevValue);
            Middle.Set(smaValue);
            Lower.Set(smaValue - NumStdDev * stdDevValue);
		}

		#region Properties
		/// <summary>
		/// Gets the lower value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// Get the middle value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Middle
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Description("Number of standard deviations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("# of std. dev.")]
		public double NumStdDev
		{
			get { return numStdDev; }
			set { numStdDev = Math.Max(0, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// Get the upper value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[0]; }
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
        private AhrensBB[] cacheAhrensBB = null;

        private static AhrensBB checkAhrensBB = new AhrensBB();

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public AhrensBB AhrensBB(double numStdDev, int period)
        {
            return AhrensBB(Input, numStdDev, period);
        }

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public AhrensBB AhrensBB(Data.IDataSeries input, double numStdDev, int period)
        {
            if (cacheAhrensBB != null)
                for (int idx = 0; idx < cacheAhrensBB.Length; idx++)
                    if (Math.Abs(cacheAhrensBB[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheAhrensBB[idx].Period == period && cacheAhrensBB[idx].EqualsInput(input))
                        return cacheAhrensBB[idx];

            lock (checkAhrensBB)
            {
                checkAhrensBB.NumStdDev = numStdDev;
                numStdDev = checkAhrensBB.NumStdDev;
                checkAhrensBB.Period = period;
                period = checkAhrensBB.Period;

                if (cacheAhrensBB != null)
                    for (int idx = 0; idx < cacheAhrensBB.Length; idx++)
                        if (Math.Abs(cacheAhrensBB[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheAhrensBB[idx].Period == period && cacheAhrensBB[idx].EqualsInput(input))
                            return cacheAhrensBB[idx];

                AhrensBB indicator = new AhrensBB();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.NumStdDev = numStdDev;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                AhrensBB[] tmp = new AhrensBB[cacheAhrensBB == null ? 1 : cacheAhrensBB.Length + 1];
                if (cacheAhrensBB != null)
                    cacheAhrensBB.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAhrensBB = tmp;
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
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AhrensBB AhrensBB(double numStdDev, int period)
        {
            return _indicator.AhrensBB(Input, numStdDev, period);
        }

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.AhrensBB AhrensBB(Data.IDataSeries input, double numStdDev, int period)
        {
            return _indicator.AhrensBB(input, numStdDev, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AhrensBB AhrensBB(double numStdDev, int period)
        {
            return _indicator.AhrensBB(Input, numStdDev, period);
        }

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.AhrensBB AhrensBB(Data.IDataSeries input, double numStdDev, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AhrensBB(input, numStdDev, period);
        }
    }
}
#endregion
