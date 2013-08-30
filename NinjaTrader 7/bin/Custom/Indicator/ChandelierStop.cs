// Chuck LeBeau's Chandelier Stop
// see http://www.traderclub.com/toolkit.htm#chandelier
// Coded by Alex Matulich, August 2008

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
    /// Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier
    /// </summary>
    [Description("Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier")]
    public class ChandelierStop : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double rangeMultiple = 3.000; // Default setting for RangeMultiple
            private double eMALength = 14.000; // Default setting for EMALength
        // User defined variables (add any user defined variables below)
			private double wt, avrng1;
			public double hbound, lbound; // public for external visibility
			public int dir = 1; // externally-visible value indicating current trend direction
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DeepPink), PlotStyle.Line, "ChandelierHi"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DeepPink), PlotStyle.Line, "ChandelierLo"));
            CalculateOnBarClose	= true; // WILL NOT WORK IF SET TO FALSE
            Overlay				= true;
            PriceTypeSupported	= false;
			wt = 2.0/(eMALength + 1.0); // EMA weight
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double TH, TL, avrng, stopdistance;
			if (CurrentBar == 0) {
				hbound = TH = High[0];
				lbound = TL = Low[0];
				avrng = avrng1 = TH-TL;
			}
			else {
				TH = Math.Max(Close[1], High[0]);  // true high
				TL = Math.Min(Close[1], Low[0]);   // true low
				avrng = wt * (TH-TL - avrng1) + avrng1; // EMA of true range
				avrng1 = avrng; // save last value for next bar
			}
			stopdistance = rangeMultiple * avrng;
			if (High[0] >= hbound) {
				hbound = TL + stopdistance;
				if (dir < 0) dir = 1; // market is up
			}
			else hbound = Math.Min(hbound, TL+stopdistance);
			if (Low[0] <= lbound) {
				lbound = TH - stopdistance;
				if (dir > 0) dir = -1; // market is down
			}
			else lbound = Math.Max(lbound, TH-stopdistance);
			ChandelierHi.Set(hbound);
			ChandelierLo.Set(lbound);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ChandelierHi
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ChandelierLo
        {
            get { return Values[1]; }
        }

        [Description("Size of stop in average range multiples")]
        [Category("Parameters")]
        public double RangeMultiple
        {
            get { return rangeMultiple; }
            set { rangeMultiple = Math.Max(0.000, value); }
        }

        [Description("Length of moving average of ranges")]
        [Category("Parameters")]
        public double EMALength
        {
            get { return eMALength; }
            set { eMALength = Math.Max(1, value); }
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
        private ChandelierStop[] cacheChandelierStop = null;

        private static ChandelierStop checkChandelierStop = new ChandelierStop();

        /// <summary>
        /// Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier
        /// </summary>
        /// <returns></returns>
        public ChandelierStop ChandelierStop(double eMALength, double rangeMultiple)
        {
            return ChandelierStop(Input, eMALength, rangeMultiple);
        }

        /// <summary>
        /// Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier
        /// </summary>
        /// <returns></returns>
        public ChandelierStop ChandelierStop(Data.IDataSeries input, double eMALength, double rangeMultiple)
        {
            if (cacheChandelierStop != null)
                for (int idx = 0; idx < cacheChandelierStop.Length; idx++)
                    if (Math.Abs(cacheChandelierStop[idx].EMALength - eMALength) <= double.Epsilon && Math.Abs(cacheChandelierStop[idx].RangeMultiple - rangeMultiple) <= double.Epsilon && cacheChandelierStop[idx].EqualsInput(input))
                        return cacheChandelierStop[idx];

            lock (checkChandelierStop)
            {
                checkChandelierStop.EMALength = eMALength;
                eMALength = checkChandelierStop.EMALength;
                checkChandelierStop.RangeMultiple = rangeMultiple;
                rangeMultiple = checkChandelierStop.RangeMultiple;

                if (cacheChandelierStop != null)
                    for (int idx = 0; idx < cacheChandelierStop.Length; idx++)
                        if (Math.Abs(cacheChandelierStop[idx].EMALength - eMALength) <= double.Epsilon && Math.Abs(cacheChandelierStop[idx].RangeMultiple - rangeMultiple) <= double.Epsilon && cacheChandelierStop[idx].EqualsInput(input))
                            return cacheChandelierStop[idx];

                ChandelierStop indicator = new ChandelierStop();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EMALength = eMALength;
                indicator.RangeMultiple = rangeMultiple;
                Indicators.Add(indicator);
                indicator.SetUp();

                ChandelierStop[] tmp = new ChandelierStop[cacheChandelierStop == null ? 1 : cacheChandelierStop.Length + 1];
                if (cacheChandelierStop != null)
                    cacheChandelierStop.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheChandelierStop = tmp;
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
        /// Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChandelierStop ChandelierStop(double eMALength, double rangeMultiple)
        {
            return _indicator.ChandelierStop(Input, eMALength, rangeMultiple);
        }

        /// <summary>
        /// Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier
        /// </summary>
        /// <returns></returns>
        public Indicator.ChandelierStop ChandelierStop(Data.IDataSeries input, double eMALength, double rangeMultiple)
        {
            return _indicator.ChandelierStop(input, eMALength, rangeMultiple);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChandelierStop ChandelierStop(double eMALength, double rangeMultiple)
        {
            return _indicator.ChandelierStop(Input, eMALength, rangeMultiple);
        }

        /// <summary>
        /// Chuck LeBeau's Chandelier Stop described at www.traderclub.com/toolkit.htm#chandelier
        /// </summary>
        /// <returns></returns>
        public Indicator.ChandelierStop ChandelierStop(Data.IDataSeries input, double eMALength, double rangeMultiple)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ChandelierStop(input, eMALength, rangeMultiple);
        }
    }
}
#endregion
