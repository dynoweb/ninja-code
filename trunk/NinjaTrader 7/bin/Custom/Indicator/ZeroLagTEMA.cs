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
    /// Zero-Lagging TEMA
    /// </summary>
    [Description("Zero-Lagging TEMA")]
    public class ZeroLagTEMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 14; // Default setting for Period
			private TEMA tema1;
			private TEMA tema2;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Orange, PlotStyle.Line, "Zero-Lag TEMA"));
            Overlay				= true;
            PriceTypeSupported	= true;
        }

        protected override void OnStartUp()
        {
            tema1 = TEMA(Inputs[0], Period);
            tema2 = TEMA(tema1, Period);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Value.Set( 2* tema1[0] - tema2[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ZeroTEMA
        {
            get { return Values[0]; }
        }

        [Description("Numbers of bars used for calculations")]
        [Category("Parameters")]
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
        private ZeroLagTEMA[] cacheZeroLagTEMA = null;

        private static ZeroLagTEMA checkZeroLagTEMA = new ZeroLagTEMA();

        /// <summary>
        /// Zero-Lagging TEMA
        /// </summary>
        /// <returns></returns>
        public ZeroLagTEMA ZeroLagTEMA(int period)
        {
            return ZeroLagTEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging TEMA
        /// </summary>
        /// <returns></returns>
        public ZeroLagTEMA ZeroLagTEMA(Data.IDataSeries input, int period)
        {
            if (cacheZeroLagTEMA != null)
                for (int idx = 0; idx < cacheZeroLagTEMA.Length; idx++)
                    if (cacheZeroLagTEMA[idx].Period == period && cacheZeroLagTEMA[idx].EqualsInput(input))
                        return cacheZeroLagTEMA[idx];

            lock (checkZeroLagTEMA)
            {
                checkZeroLagTEMA.Period = period;
                period = checkZeroLagTEMA.Period;

                if (cacheZeroLagTEMA != null)
                    for (int idx = 0; idx < cacheZeroLagTEMA.Length; idx++)
                        if (cacheZeroLagTEMA[idx].Period == period && cacheZeroLagTEMA[idx].EqualsInput(input))
                            return cacheZeroLagTEMA[idx];

                ZeroLagTEMA indicator = new ZeroLagTEMA();
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

                ZeroLagTEMA[] tmp = new ZeroLagTEMA[cacheZeroLagTEMA == null ? 1 : cacheZeroLagTEMA.Length + 1];
                if (cacheZeroLagTEMA != null)
                    cacheZeroLagTEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZeroLagTEMA = tmp;
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
        /// Zero-Lagging TEMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZeroLagTEMA ZeroLagTEMA(int period)
        {
            return _indicator.ZeroLagTEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging TEMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ZeroLagTEMA ZeroLagTEMA(Data.IDataSeries input, int period)
        {
            return _indicator.ZeroLagTEMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Zero-Lagging TEMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZeroLagTEMA ZeroLagTEMA(int period)
        {
            return _indicator.ZeroLagTEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging TEMA
        /// </summary>
        /// <returns></returns>
        public Indicator.ZeroLagTEMA ZeroLagTEMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZeroLagTEMA(input, period);
        }
    }
}
#endregion
