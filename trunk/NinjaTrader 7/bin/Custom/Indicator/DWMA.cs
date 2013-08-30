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
    [Description("Double Weighted Moving Average")]
	[Gui.Design.DisplayName("DWMA (Double Weighted Moving Average)")]
    public class DWMA : Indicator
    {
        #region Variables
            private int period = 14;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "DWMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			Value.Set(2 * WMA(Inputs[0], Period)[0] -  WMA(WMA(Inputs[0], Period), Period)[0]);
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
        private DWMA[] cacheDWMA = null;

        private static DWMA checkDWMA = new DWMA();

        /// <summary>
        /// Double Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public DWMA DWMA(int period)
        {
            return DWMA(Input, period);
        }

        /// <summary>
        /// Double Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public DWMA DWMA(Data.IDataSeries input, int period)
        {
            if (cacheDWMA != null)
                for (int idx = 0; idx < cacheDWMA.Length; idx++)
                    if (cacheDWMA[idx].Period == period && cacheDWMA[idx].EqualsInput(input))
                        return cacheDWMA[idx];

            lock (checkDWMA)
            {
                checkDWMA.Period = period;
                period = checkDWMA.Period;

                if (cacheDWMA != null)
                    for (int idx = 0; idx < cacheDWMA.Length; idx++)
                        if (cacheDWMA[idx].Period == period && cacheDWMA[idx].EqualsInput(input))
                            return cacheDWMA[idx];

                DWMA indicator = new DWMA();
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

                DWMA[] tmp = new DWMA[cacheDWMA == null ? 1 : cacheDWMA.Length + 1];
                if (cacheDWMA != null)
                    cacheDWMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDWMA = tmp;
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
        /// Double Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DWMA DWMA(int period)
        {
            return _indicator.DWMA(Input, period);
        }

        /// <summary>
        /// Double Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DWMA DWMA(Data.IDataSeries input, int period)
        {
            return _indicator.DWMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Double Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DWMA DWMA(int period)
        {
            return _indicator.DWMA(Input, period);
        }

        /// <summary>
        /// Double Weighted Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DWMA DWMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DWMA(input, period);
        }
    }
}
#endregion
