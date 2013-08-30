// 
// Copyright (C) 2008, NinjaTrader LLC <www.ninjatrader.com>.
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
    /// Double Exponential Moving Average
    /// </summary>
    [Description("Double Simple Moving Average")]
	[Gui.Design.DisplayName("DSMA (Double Simple Moving Average)")]
    public class DSMA : Indicator
    {
        #region Variables
            private int period = 14;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "DSMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			Value.Set(2 * SMA(Inputs[0], Period)[0] -  SMA(SMA(Inputs[0], Period), Period)[0]);
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
        private DSMA[] cacheDSMA = null;

        private static DSMA checkDSMA = new DSMA();

        /// <summary>
        /// Double Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public DSMA DSMA(int period)
        {
            return DSMA(Input, period);
        }

        /// <summary>
        /// Double Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public DSMA DSMA(Data.IDataSeries input, int period)
        {
            if (cacheDSMA != null)
                for (int idx = 0; idx < cacheDSMA.Length; idx++)
                    if (cacheDSMA[idx].Period == period && cacheDSMA[idx].EqualsInput(input))
                        return cacheDSMA[idx];

            lock (checkDSMA)
            {
                checkDSMA.Period = period;
                period = checkDSMA.Period;

                if (cacheDSMA != null)
                    for (int idx = 0; idx < cacheDSMA.Length; idx++)
                        if (cacheDSMA[idx].Period == period && cacheDSMA[idx].EqualsInput(input))
                            return cacheDSMA[idx];

                DSMA indicator = new DSMA();
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

                DSMA[] tmp = new DSMA[cacheDSMA == null ? 1 : cacheDSMA.Length + 1];
                if (cacheDSMA != null)
                    cacheDSMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDSMA = tmp;
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
        /// Double Simple Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DSMA DSMA(int period)
        {
            return _indicator.DSMA(Input, period);
        }

        /// <summary>
        /// Double Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DSMA DSMA(Data.IDataSeries input, int period)
        {
            return _indicator.DSMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Double Simple Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DSMA DSMA(int period)
        {
            return _indicator.DSMA(Input, period);
        }

        /// <summary>
        /// Double Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DSMA DSMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DSMA(input, period);
        }
    }
}
#endregion
