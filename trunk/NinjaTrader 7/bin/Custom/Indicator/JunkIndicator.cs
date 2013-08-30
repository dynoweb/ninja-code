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
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class JunkIndicator : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 20; // Default setting for Period
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "RicksPlot"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            RicksPlot.Set(Close[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RicksPlot
        {
            get { return Values[0]; }
        }

        [Description("")]
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
        private JunkIndicator[] cacheJunkIndicator = null;

        private static JunkIndicator checkJunkIndicator = new JunkIndicator();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public JunkIndicator JunkIndicator(int period)
        {
            return JunkIndicator(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public JunkIndicator JunkIndicator(Data.IDataSeries input, int period)
        {
            if (cacheJunkIndicator != null)
                for (int idx = 0; idx < cacheJunkIndicator.Length; idx++)
                    if (cacheJunkIndicator[idx].Period == period && cacheJunkIndicator[idx].EqualsInput(input))
                        return cacheJunkIndicator[idx];

            lock (checkJunkIndicator)
            {
                checkJunkIndicator.Period = period;
                period = checkJunkIndicator.Period;

                if (cacheJunkIndicator != null)
                    for (int idx = 0; idx < cacheJunkIndicator.Length; idx++)
                        if (cacheJunkIndicator[idx].Period == period && cacheJunkIndicator[idx].EqualsInput(input))
                            return cacheJunkIndicator[idx];

                JunkIndicator indicator = new JunkIndicator();
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

                JunkIndicator[] tmp = new JunkIndicator[cacheJunkIndicator == null ? 1 : cacheJunkIndicator.Length + 1];
                if (cacheJunkIndicator != null)
                    cacheJunkIndicator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheJunkIndicator = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JunkIndicator JunkIndicator(int period)
        {
            return _indicator.JunkIndicator(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.JunkIndicator JunkIndicator(Data.IDataSeries input, int period)
        {
            return _indicator.JunkIndicator(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JunkIndicator JunkIndicator(int period)
        {
            return _indicator.JunkIndicator(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.JunkIndicator JunkIndicator(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JunkIndicator(input, period);
        }
    }
}
#endregion
