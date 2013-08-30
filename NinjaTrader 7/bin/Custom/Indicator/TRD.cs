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
    /// True Range Double - true range for two days
    /// </summary>
    [Description("True Range Double - true range for two days")]
    public class TRD : Indicator
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
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "ATRD"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 2)
				return;
			
			double highestHigh = Math.Max( Math.Max(High[0], High[1]), Close[2]);
			double lowestLow   = Math.Min( Math.Min(Low[0],  Low[1]),  Close[2]);
			double trueRange   = highestHigh - lowestLow;
			
			ATRD.Set(trueRange);
			//ATRD.Set(((Math.Min(CurrentBar + 1, Period) - 1 ) * Value[1] + trueRange) / Math.Min(CurrentBar + 1, Period));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ATRD
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
        private TRD[] cacheTRD = null;

        private static TRD checkTRD = new TRD();

        /// <summary>
        /// True Range Double - true range for two days
        /// </summary>
        /// <returns></returns>
        public TRD TRD(int period)
        {
            return TRD(Input, period);
        }

        /// <summary>
        /// True Range Double - true range for two days
        /// </summary>
        /// <returns></returns>
        public TRD TRD(Data.IDataSeries input, int period)
        {
            if (cacheTRD != null)
                for (int idx = 0; idx < cacheTRD.Length; idx++)
                    if (cacheTRD[idx].Period == period && cacheTRD[idx].EqualsInput(input))
                        return cacheTRD[idx];

            lock (checkTRD)
            {
                checkTRD.Period = period;
                period = checkTRD.Period;

                if (cacheTRD != null)
                    for (int idx = 0; idx < cacheTRD.Length; idx++)
                        if (cacheTRD[idx].Period == period && cacheTRD[idx].EqualsInput(input))
                            return cacheTRD[idx];

                TRD indicator = new TRD();
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

                TRD[] tmp = new TRD[cacheTRD == null ? 1 : cacheTRD.Length + 1];
                if (cacheTRD != null)
                    cacheTRD.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTRD = tmp;
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
        /// True Range Double - true range for two days
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TRD TRD(int period)
        {
            return _indicator.TRD(Input, period);
        }

        /// <summary>
        /// True Range Double - true range for two days
        /// </summary>
        /// <returns></returns>
        public Indicator.TRD TRD(Data.IDataSeries input, int period)
        {
            return _indicator.TRD(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// True Range Double - true range for two days
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TRD TRD(int period)
        {
            return _indicator.TRD(Input, period);
        }

        /// <summary>
        /// True Range Double - true range for two days
        /// </summary>
        /// <returns></returns>
        public Indicator.TRD TRD(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TRD(input, period);
        }
    }
}
#endregion
