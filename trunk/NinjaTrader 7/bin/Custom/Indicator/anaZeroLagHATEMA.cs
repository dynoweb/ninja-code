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
    /// Zero-Lagging Heiken-Ashi TEMA
    /// </summary>
    [Description("Zero-Lagging Heiken-Ashi TEMA")]
    public class anaZeroLagHATEMA : Indicator
    {
        #region Variables
		private int period = 14; // Default setting for Period
		private DataSeries haC;
		private DataSeries haO;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Orange, PlotStyle.Line, "Zero-Lagging Heiken-Ashi TEMA"));
			
			haC = new DataSeries(this);
			haO = new DataSeries(this);
			
            Overlay				= true;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar == 0)
			{
				haC.Set(Close[0]);
				haO.Set(Open[0]);
				ZeroHATEMA.Set(Input[0]);
				return;
			}
			haO.Set((((Open[1] + High[1] + Low[1] + Close[1]) / 4) + haO[1]) / 2);
			haC.Set((((Open[0] + High[0] + Low[0] + Close[0]) / 4) + haO[0] + Math.Max(High[0], haO[0]) + Math.Min(Low[0], haO[0])) / 4);
			double TEMA1 = TEMA(haC, Period)[0];
			double TEMA2 = TEMA(TEMA(haC, Period), Period)[0];
            ZeroHATEMA.Set(TEMA1 + (TEMA1 - TEMA2));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ZeroHATEMA
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
        private anaZeroLagHATEMA[] cacheanaZeroLagHATEMA = null;

        private static anaZeroLagHATEMA checkanaZeroLagHATEMA = new anaZeroLagHATEMA();

        /// <summary>
        /// Zero-Lagging Heiken-Ashi TEMA
        /// </summary>
        /// <returns></returns>
        public anaZeroLagHATEMA anaZeroLagHATEMA(int period)
        {
            return anaZeroLagHATEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging Heiken-Ashi TEMA
        /// </summary>
        /// <returns></returns>
        public anaZeroLagHATEMA anaZeroLagHATEMA(Data.IDataSeries input, int period)
        {
            if (cacheanaZeroLagHATEMA != null)
                for (int idx = 0; idx < cacheanaZeroLagHATEMA.Length; idx++)
                    if (cacheanaZeroLagHATEMA[idx].Period == period && cacheanaZeroLagHATEMA[idx].EqualsInput(input))
                        return cacheanaZeroLagHATEMA[idx];

            lock (checkanaZeroLagHATEMA)
            {
                checkanaZeroLagHATEMA.Period = period;
                period = checkanaZeroLagHATEMA.Period;

                if (cacheanaZeroLagHATEMA != null)
                    for (int idx = 0; idx < cacheanaZeroLagHATEMA.Length; idx++)
                        if (cacheanaZeroLagHATEMA[idx].Period == period && cacheanaZeroLagHATEMA[idx].EqualsInput(input))
                            return cacheanaZeroLagHATEMA[idx];

                anaZeroLagHATEMA indicator = new anaZeroLagHATEMA();
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

                anaZeroLagHATEMA[] tmp = new anaZeroLagHATEMA[cacheanaZeroLagHATEMA == null ? 1 : cacheanaZeroLagHATEMA.Length + 1];
                if (cacheanaZeroLagHATEMA != null)
                    cacheanaZeroLagHATEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaZeroLagHATEMA = tmp;
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
        /// Zero-Lagging Heiken-Ashi TEMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaZeroLagHATEMA anaZeroLagHATEMA(int period)
        {
            return _indicator.anaZeroLagHATEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging Heiken-Ashi TEMA
        /// </summary>
        /// <returns></returns>
        public Indicator.anaZeroLagHATEMA anaZeroLagHATEMA(Data.IDataSeries input, int period)
        {
            return _indicator.anaZeroLagHATEMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Zero-Lagging Heiken-Ashi TEMA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaZeroLagHATEMA anaZeroLagHATEMA(int period)
        {
            return _indicator.anaZeroLagHATEMA(Input, period);
        }

        /// <summary>
        /// Zero-Lagging Heiken-Ashi TEMA
        /// </summary>
        /// <returns></returns>
        public Indicator.anaZeroLagHATEMA anaZeroLagHATEMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaZeroLagHATEMA(input, period);
        }
    }
}
#endregion
