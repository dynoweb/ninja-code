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
    [Description("Triple Simple Moving Average")]
	[Gui.Design.DisplayName("TSMA (Triple Simple Moving Average)")]
    public class TSMA : Indicator
    {
        #region Variables
        private int period = 14;
		private SMA sma1;
        private SMA sma2;
        private SMA sma3;

        #endregion

        protected override void OnStartUp()
        {
            sma1 = SMA(Inputs[0], Period);
            sma2 = SMA(sma1, Period);
            sma3 = SMA(sma2, Period);
        }

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "TSMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Value.Set(3 * sma1[0] - 3 * sma2[0] + sma3[0]);
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
        private TSMA[] cacheTSMA = null;

        private static TSMA checkTSMA = new TSMA();

        /// <summary>
        /// Triple Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public TSMA TSMA(int period)
        {
            return TSMA(Input, period);
        }

        /// <summary>
        /// Triple Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public TSMA TSMA(Data.IDataSeries input, int period)
        {
            if (cacheTSMA != null)
                for (int idx = 0; idx < cacheTSMA.Length; idx++)
                    if (cacheTSMA[idx].Period == period && cacheTSMA[idx].EqualsInput(input))
                        return cacheTSMA[idx];

            lock (checkTSMA)
            {
                checkTSMA.Period = period;
                period = checkTSMA.Period;

                if (cacheTSMA != null)
                    for (int idx = 0; idx < cacheTSMA.Length; idx++)
                        if (cacheTSMA[idx].Period == period && cacheTSMA[idx].EqualsInput(input))
                            return cacheTSMA[idx];

                TSMA indicator = new TSMA();
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

                TSMA[] tmp = new TSMA[cacheTSMA == null ? 1 : cacheTSMA.Length + 1];
                if (cacheTSMA != null)
                    cacheTSMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTSMA = tmp;
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
        /// Triple Simple Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSMA TSMA(int period)
        {
            return _indicator.TSMA(Input, period);
        }

        /// <summary>
        /// Triple Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.TSMA TSMA(Data.IDataSeries input, int period)
        {
            return _indicator.TSMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Triple Simple Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSMA TSMA(int period)
        {
            return _indicator.TSMA(Input, period);
        }

        /// <summary>
        /// Triple Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.TSMA TSMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TSMA(input, period);
        }
    }
}
#endregion
