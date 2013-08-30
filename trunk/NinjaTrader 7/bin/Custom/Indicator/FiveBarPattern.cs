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
    /// Added markers to indicate 5 bar pattern resistance support
    /// </summary>
    [Description("Added markers to indicate 5 bar pattern resistance support")]
    public class FiveBarPattern : Indicator
    {
        #region Variables
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Dot, "Upper"));
            Add(new Plot(Color.Red, PlotStyle.Dot, "Lower"));
            
			Overlay				= true;			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (High[0] < High[2] && High[1] < High[2] && High[3] < High[2] && High[4] < High[2]) {
				//Plots[0].Pen = new Pen(Color.Green);
				Values[0][2] = High[2] + 2 * TickSize;
				
			}
			
			if (Low[0] > Low[2] && Low[1] > Low[2] && Low[3] > Low[2] && Low[4] > Low[2]) {
				Values[1][2] = Low[2] - 2 * TickSize;
			}
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot1
        {
            get { return Values[1]; }
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
        private FiveBarPattern[] cacheFiveBarPattern = null;

        private static FiveBarPattern checkFiveBarPattern = new FiveBarPattern();

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public FiveBarPattern FiveBarPattern()
        {
            return FiveBarPattern(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public FiveBarPattern FiveBarPattern(Data.IDataSeries input)
        {
            if (cacheFiveBarPattern != null)
                for (int idx = 0; idx < cacheFiveBarPattern.Length; idx++)
                    if (cacheFiveBarPattern[idx].EqualsInput(input))
                        return cacheFiveBarPattern[idx];

            lock (checkFiveBarPattern)
            {
                if (cacheFiveBarPattern != null)
                    for (int idx = 0; idx < cacheFiveBarPattern.Length; idx++)
                        if (cacheFiveBarPattern[idx].EqualsInput(input))
                            return cacheFiveBarPattern[idx];

                FiveBarPattern indicator = new FiveBarPattern();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                FiveBarPattern[] tmp = new FiveBarPattern[cacheFiveBarPattern == null ? 1 : cacheFiveBarPattern.Length + 1];
                if (cacheFiveBarPattern != null)
                    cacheFiveBarPattern.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheFiveBarPattern = tmp;
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
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FiveBarPattern FiveBarPattern()
        {
            return _indicator.FiveBarPattern(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarPattern FiveBarPattern(Data.IDataSeries input)
        {
            return _indicator.FiveBarPattern(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FiveBarPattern FiveBarPattern()
        {
            return _indicator.FiveBarPattern(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarPattern FiveBarPattern(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.FiveBarPattern(input);
        }
    }
}
#endregion
