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
    /// Slight modification of the DMI indicator
    /// </summary>
    [Description("Slight modification of the DMI indicator")]
    public class rcDMI : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 14; // Default setting for Period
		
        // User defined variables (add any user defined variables below)
			private DataSeries			dmPlus;
			private DataSeries			dmMinus;
			private DataSeries			sumDmPlus;
			private DataSeries			sumDmMinus;
			private DataSeries			sumTr;
			private DataSeries			tr;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkSlateGray), PlotStyle.Line, "ADX"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "DIPlus"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "DIMinus"));
			
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 20, "ADXLimit"));
			
			dmPlus		= new DataSeries(this);
			dmMinus		= new DataSeries(this);
			sumDmPlus	= new DataSeries(this);
			sumDmMinus	= new DataSeries(this);
			sumTr		= new DataSeries(this);
			tr			= new DataSeries(this);
			
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double trueRange = High[0] - Low[0];
			if (CurrentBar == 0)
			{
				tr.Set(trueRange);
				dmPlus.Set(0);
				dmMinus.Set(0);
				sumTr.Set(tr[0]);
				sumDmPlus.Set(dmPlus[0]);
				sumDmMinus.Set(dmMinus[0]);
				Value.Set(50);
			}
			else
			{
				tr.Set(Math.Max(Math.Abs(Low[0] - Close[1]), Math.Max(trueRange, Math.Abs(High[0] - Close[1]))));
				dmPlus.Set(High[0] - High[1] > Low[1] - Low[0] ? Math.Max(High[0] - High[1], 0) : 0);
				dmMinus.Set(Low[1] - Low[0] > High[0] - High[1] ? Math.Max(Low[1] - Low[0], 0) : 0);

				if (CurrentBar < Period)
				{
					sumTr.Set(sumTr[1] + tr[0]);
					sumDmPlus.Set(sumDmPlus[1] + dmPlus[0]);
					sumDmMinus.Set(sumDmMinus[1] + dmMinus[0]);
				}
				else
				{
					sumTr.Set(sumTr[1] - sumTr[1] / Period + tr[0]);
					sumDmPlus.Set(sumDmPlus[1] - sumDmPlus[1] / Period + dmPlus[0]);
					sumDmMinus.Set(sumDmMinus[1] - sumDmMinus[1] / Period + dmMinus[0]);
				}

				double diPlus	= 100 * (sumTr[0] == 0 ? 0 : sumDmPlus[0] / sumTr[0]);
				double diMinus	= 100 * (sumTr[0] == 0 ? 0 : sumDmMinus[0] / sumTr[0]);
				double diff		= Math.Abs(diPlus - diMinus);
				double sum		= diPlus + diMinus;

				Value.Set(sum == 0 ? 50 : ((Period - 1) * Value[1] + 100 * diff / sum) / Period);
				DIPlus.Set(diPlus);
				DIMinus.Set(diMinus);
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ADX
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DIPlus
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DIMinus
        {
            get { return Values[2]; }
        }

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
        private rcDMI[] cachercDMI = null;

        private static rcDMI checkrcDMI = new rcDMI();

        /// <summary>
        /// Slight modification of the DMI indicator
        /// </summary>
        /// <returns></returns>
        public rcDMI rcDMI(int period)
        {
            return rcDMI(Input, period);
        }

        /// <summary>
        /// Slight modification of the DMI indicator
        /// </summary>
        /// <returns></returns>
        public rcDMI rcDMI(Data.IDataSeries input, int period)
        {
            if (cachercDMI != null)
                for (int idx = 0; idx < cachercDMI.Length; idx++)
                    if (cachercDMI[idx].Period == period && cachercDMI[idx].EqualsInput(input))
                        return cachercDMI[idx];

            lock (checkrcDMI)
            {
                checkrcDMI.Period = period;
                period = checkrcDMI.Period;

                if (cachercDMI != null)
                    for (int idx = 0; idx < cachercDMI.Length; idx++)
                        if (cachercDMI[idx].Period == period && cachercDMI[idx].EqualsInput(input))
                            return cachercDMI[idx];

                rcDMI indicator = new rcDMI();
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

                rcDMI[] tmp = new rcDMI[cachercDMI == null ? 1 : cachercDMI.Length + 1];
                if (cachercDMI != null)
                    cachercDMI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachercDMI = tmp;
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
        /// Slight modification of the DMI indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcDMI rcDMI(int period)
        {
            return _indicator.rcDMI(Input, period);
        }

        /// <summary>
        /// Slight modification of the DMI indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.rcDMI rcDMI(Data.IDataSeries input, int period)
        {
            return _indicator.rcDMI(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Slight modification of the DMI indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcDMI rcDMI(int period)
        {
            return _indicator.rcDMI(Input, period);
        }

        /// <summary>
        /// Slight modification of the DMI indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.rcDMI rcDMI(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.rcDMI(input, period);
        }
    }
}
#endregion
