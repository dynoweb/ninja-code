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
    /// Wilder's average
    /// </summary>
    [Description("Wilder's average")]
    public class Wilder : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 13; // Default setting for Period
        // User defined variables (add any user defined variables below)
			private DataSeries firstAvg;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Purple), PlotStyle.Line, "WAvg"));
            Overlay				= true;
            PriceTypeSupported	= true;
			
			firstAvg = new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			if (CurrentBar == 0 ) 
			{
				firstAvg.Set(0);
				return;
			}
			//if (CurrentBar + 1 == Period )
			{	
				//calculate first average
				firstAvg.Set( SMA(Input, period)[0]);
			}
			  WAvg.Set( (firstAvg[1] * (period -1) + Input[0]) / period);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries WAvg
        {
            get { return Values[0]; }
        }

        [Description("period")]
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
        private Wilder[] cacheWilder = null;

        private static Wilder checkWilder = new Wilder();

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Wilder Wilder(int period)
        {
            return Wilder(Input, period);
        }

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Wilder Wilder(Data.IDataSeries input, int period)
        {
            if (cacheWilder != null)
                for (int idx = 0; idx < cacheWilder.Length; idx++)
                    if (cacheWilder[idx].Period == period && cacheWilder[idx].EqualsInput(input))
                        return cacheWilder[idx];

            lock (checkWilder)
            {
                checkWilder.Period = period;
                period = checkWilder.Period;

                if (cacheWilder != null)
                    for (int idx = 0; idx < cacheWilder.Length; idx++)
                        if (cacheWilder[idx].Period == period && cacheWilder[idx].EqualsInput(input))
                            return cacheWilder[idx];

                Wilder indicator = new Wilder();
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

                Wilder[] tmp = new Wilder[cacheWilder == null ? 1 : cacheWilder.Length + 1];
                if (cacheWilder != null)
                    cacheWilder.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheWilder = tmp;
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
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Wilder Wilder(int period)
        {
            return _indicator.Wilder(Input, period);
        }

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Indicator.Wilder Wilder(Data.IDataSeries input, int period)
        {
            return _indicator.Wilder(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Wilder Wilder(int period)
        {
            return _indicator.Wilder(Input, period);
        }

        /// <summary>
        /// Wilder's average
        /// </summary>
        /// <returns></returns>
        public Indicator.Wilder Wilder(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Wilder(input, period);
        }
    }
}
#endregion
