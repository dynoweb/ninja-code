// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Triple Simple Moving Average
    /// </summary>
    [Description("Triple Weighted Moving Average")]
	[Gui.Design.DisplayName("TWMA (Triple Weighted Moving Average)")]
    public class TWMA : Indicator
    {
        #region Variables
        private int period = 14;
		private WMA wma1;
        private WMA wma2;
        private WMA wma3;

        #endregion

        protected override void OnStartUp()
        {
            wma1 = WMA(Inputs[0], Period);
            wma2 = WMA(wma1, Period);
            wma3 = WMA(wma2, Period);
        }

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "TWMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Value.Set(3 * wma1[0] - 3 * wma2[0] + wma3[0]);
        }

        #region Properties

        [Description("Number of bars used for calculations")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
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
        private TWMA[] cacheTWMA = null;

        private static TWMA checkTWMA = new TWMA();

        /// <summary>
        /// Triple Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public TWMA TWMA(int period)
        {
            return TWMA(Input, period);
        }

        /// <summary>
        /// Triple Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public TWMA TWMA(Data.IDataSeries input, int period)
        {
            if (cacheTWMA != null)
                for (int idx = 0; idx < cacheTWMA.Length; idx++)
                    if (cacheTWMA[idx].Period == period && cacheTWMA[idx].EqualsInput(input))
                        return cacheTWMA[idx];

            lock (checkTWMA)
            {
                checkTWMA.Period = period;
                period = checkTWMA.Period;

                if (cacheTWMA != null)
                    for (int idx = 0; idx < cacheTWMA.Length; idx++)
                        if (cacheTWMA[idx].Period == period && cacheTWMA[idx].EqualsInput(input))
                            return cacheTWMA[idx];

                TWMA indicator = new TWMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                TWMA[] tmp = new TWMA[cacheTWMA == null ? 1 : cacheTWMA.Length + 1];
                if (cacheTWMA != null)
                    cacheTWMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTWMA = tmp;
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
        /// Triple Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TWMA TWMA(int period)
        {
            return _indicator.TWMA(Input, period);
        }

        /// <summary>
        /// Triple Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.TWMA TWMA(Data.IDataSeries input, int period)
        {
            return _indicator.TWMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Triple Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TWMA TWMA(int period)
        {
            return _indicator.TWMA(Input, period);
        }

        /// <summary>
        /// Triple Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.TWMA TWMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TWMA(input, period);
        }
    }
}
#endregion
