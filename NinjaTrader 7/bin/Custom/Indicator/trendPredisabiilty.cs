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
    public class trendPredisabiilty : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int tEST1 = 1; // Default setting for TEST1
            private double tEST12 = 1; // Default setting for TEST12
            private double vTEST3 = 1; // Default setting for VTEST3
            private double zEST1 = 1; // Default setting for ZEST1
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Wseweeee"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "Weeegg"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Line, "Fggg"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            Plot0.Set(Close[0]);
            Wseweeee.Set(Close[0]);
            Weeegg.Set(Close[0]);
            Fggg.Set(Close[0]);
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
        public DataSeries Wseweeee
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Weeegg
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Fggg
        {
            get { return Values[3]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TEST1
        {
            get { return tEST1; }
            set { tEST1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double TEST12
        {
            get { return tEST12; }
            set { tEST12 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double VTEST3
        {
            get { return vTEST3; }
            set { vTEST3 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ZEST1
        {
            get { return zEST1; }
            set { zEST1 = Math.Max(1, value); }
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
        private trendPredisabiilty[] cachetrendPredisabiilty = null;

        private static trendPredisabiilty checktrendPredisabiilty = new trendPredisabiilty();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public trendPredisabiilty trendPredisabiilty(int tEST1, double tEST12, double vTEST3, double zEST1)
        {
            return trendPredisabiilty(Input, tEST1, tEST12, vTEST3, zEST1);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public trendPredisabiilty trendPredisabiilty(Data.IDataSeries input, int tEST1, double tEST12, double vTEST3, double zEST1)
        {
            if (cachetrendPredisabiilty != null)
                for (int idx = 0; idx < cachetrendPredisabiilty.Length; idx++)
                    if (cachetrendPredisabiilty[idx].TEST1 == tEST1 && Math.Abs(cachetrendPredisabiilty[idx].TEST12 - tEST12) <= double.Epsilon && Math.Abs(cachetrendPredisabiilty[idx].VTEST3 - vTEST3) <= double.Epsilon && Math.Abs(cachetrendPredisabiilty[idx].ZEST1 - zEST1) <= double.Epsilon && cachetrendPredisabiilty[idx].EqualsInput(input))
                        return cachetrendPredisabiilty[idx];

            lock (checktrendPredisabiilty)
            {
                checktrendPredisabiilty.TEST1 = tEST1;
                tEST1 = checktrendPredisabiilty.TEST1;
                checktrendPredisabiilty.TEST12 = tEST12;
                tEST12 = checktrendPredisabiilty.TEST12;
                checktrendPredisabiilty.VTEST3 = vTEST3;
                vTEST3 = checktrendPredisabiilty.VTEST3;
                checktrendPredisabiilty.ZEST1 = zEST1;
                zEST1 = checktrendPredisabiilty.ZEST1;

                if (cachetrendPredisabiilty != null)
                    for (int idx = 0; idx < cachetrendPredisabiilty.Length; idx++)
                        if (cachetrendPredisabiilty[idx].TEST1 == tEST1 && Math.Abs(cachetrendPredisabiilty[idx].TEST12 - tEST12) <= double.Epsilon && Math.Abs(cachetrendPredisabiilty[idx].VTEST3 - vTEST3) <= double.Epsilon && Math.Abs(cachetrendPredisabiilty[idx].ZEST1 - zEST1) <= double.Epsilon && cachetrendPredisabiilty[idx].EqualsInput(input))
                            return cachetrendPredisabiilty[idx];

                trendPredisabiilty indicator = new trendPredisabiilty();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.TEST1 = tEST1;
                indicator.TEST12 = tEST12;
                indicator.VTEST3 = vTEST3;
                indicator.ZEST1 = zEST1;
                Indicators.Add(indicator);
                indicator.SetUp();

                trendPredisabiilty[] tmp = new trendPredisabiilty[cachetrendPredisabiilty == null ? 1 : cachetrendPredisabiilty.Length + 1];
                if (cachetrendPredisabiilty != null)
                    cachetrendPredisabiilty.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachetrendPredisabiilty = tmp;
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
        public Indicator.trendPredisabiilty trendPredisabiilty(int tEST1, double tEST12, double vTEST3, double zEST1)
        {
            return _indicator.trendPredisabiilty(Input, tEST1, tEST12, vTEST3, zEST1);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.trendPredisabiilty trendPredisabiilty(Data.IDataSeries input, int tEST1, double tEST12, double vTEST3, double zEST1)
        {
            return _indicator.trendPredisabiilty(input, tEST1, tEST12, vTEST3, zEST1);
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
        public Indicator.trendPredisabiilty trendPredisabiilty(int tEST1, double tEST12, double vTEST3, double zEST1)
        {
            return _indicator.trendPredisabiilty(Input, tEST1, tEST12, vTEST3, zEST1);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.trendPredisabiilty trendPredisabiilty(Data.IDataSeries input, int tEST1, double tEST12, double vTEST3, double zEST1)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.trendPredisabiilty(input, tEST1, tEST12, vTEST3, zEST1);
        }
    }
}
#endregion
