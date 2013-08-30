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
    [Description("Double Triangular Moving Average")]
	[Gui.Design.DisplayName("DTMA (Double Triangular Moving Average)")]
    public class DTMA : Indicator
    {
        #region Variables
            private int period = 14;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "DTMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			Value.Set(2 * TMA(Inputs[0], Period)[0] -  TMA(TMA(Inputs[0], Period), Period)[0]);
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
        private DTMA[] cacheDTMA = null;

        private static DTMA checkDTMA = new DTMA();

        /// <summary>
        /// Double Triangular Moving Average
        /// </summary>
        /// <returns></returns>
        public DTMA DTMA(int period)
        {
            return DTMA(Input, period);
        }

        /// <summary>
        /// Double Triangular Moving Average
        /// </summary>
        /// <returns></returns>
        public DTMA DTMA(Data.IDataSeries input, int period)
        {
            if (cacheDTMA != null)
                for (int idx = 0; idx < cacheDTMA.Length; idx++)
                    if (cacheDTMA[idx].Period == period && cacheDTMA[idx].EqualsInput(input))
                        return cacheDTMA[idx];

            lock (checkDTMA)
            {
                checkDTMA.Period = period;
                period = checkDTMA.Period;

                if (cacheDTMA != null)
                    for (int idx = 0; idx < cacheDTMA.Length; idx++)
                        if (cacheDTMA[idx].Period == period && cacheDTMA[idx].EqualsInput(input))
                            return cacheDTMA[idx];

                DTMA indicator = new DTMA();
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

                DTMA[] tmp = new DTMA[cacheDTMA == null ? 1 : cacheDTMA.Length + 1];
                if (cacheDTMA != null)
                    cacheDTMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDTMA = tmp;
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
        /// Double Triangular Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DTMA DTMA(int period)
        {
            return _indicator.DTMA(Input, period);
        }

        /// <summary>
        /// Double Triangular Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DTMA DTMA(Data.IDataSeries input, int period)
        {
            return _indicator.DTMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Double Triangular Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DTMA DTMA(int period)
        {
            return _indicator.DTMA(Input, period);
        }

        /// <summary>
        /// Double Triangular Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DTMA DTMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DTMA(input, period);
        }
    }
}
#endregion
