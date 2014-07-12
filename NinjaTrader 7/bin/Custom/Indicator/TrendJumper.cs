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
    public class TrendJumper : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double tgt1X = 1; // Default setting for Tgt1X
            private double tgt2X = 1.500; // Default setting for Tgt2X
            private double tgt3X = 2; // Default setting for Tgt3X
            private int eMA1Len = 15; // Default setting for EMA1Len
            private int eMA2Len = 55; // Default setting for EMA1Len
		    private int jLLengthFast = 4;
		    private int jLLengthSlow = 24;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Dot, "LongTgt1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Dot, "LongTgt2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Dot, "LongTgt3"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Cross, "LongEntry"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Dot, "LongStop"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Dot, "ShortTgt1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Dot, "ShortTgt2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Dot, "ShortTgt3"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Cross, "ShortEntry"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Dot, "ShortStop"));

			Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "JumpLineSlow"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Line, "JumpLineFast"));
			
			Add(new Plot(Color.DarkGray, PlotStyle.Line, "TrailingStop"));
			Add(new Plot(Color.MediumBlue, PlotStyle.Line, "TrendFilter"));
			
			Plots[10].Pen.DashStyle = DashStyle.Dash;
			Plots[11].Pen.DashStyle = DashStyle.Dash;
			
			PaintPriceMarkers = false;
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			JumpLineFast.Set(Instrument.MasterInstrument.Round2TickSize(DonchianChannel(JLLengthFast)[0]));
			JumpLineSlow.Set(Instrument.MasterInstrument.Round2TickSize(DonchianChannel(JLLengthSlow)[0]));
			TrailingStop.Set(EMA(eMA1Len)[0]);
			//TrendFilter.Set(EMA(eMA2Len)[0]);
			
			if (
				CrossAbove(JumpLineFast, JumpLineSlow, 1) 
				&& !CrossBelow(JumpLineFast, JumpLineSlow, 10) 
				&& Close[0] == High[0]
				)
			{
				LongEntry.Set(High[0] + 5 * TickSize);
				LongStop.Set(Low[0] - 10 * TickSize);
				LongTgt1.Set(High[0] + Instrument.MasterInstrument.Round2TickSize(tgt1X * 20 * TickSize));
				LongTgt2.Set(High[0] + Instrument.MasterInstrument.Round2TickSize(tgt2X * 20 * TickSize));
				LongTgt3.Set(High[0] + Instrument.MasterInstrument.Round2TickSize(tgt3X * 20 * TickSize));
			}
			if (//CrossBelow(JumpLineFast, JumpLineSlow, 1) 
				   JumpLineFast[0] < JumpLineSlow[0]
				&& JumpLineFast[1] > JumpLineSlow[1]
				&& Close[0] == Low[0])
			{
				ShortEntry.Set(Low[0] - 5 * TickSize);
				ShortStop.Set(High[0] + 10 * TickSize);
				ShortTgt1.Set(Low[0] - Instrument.MasterInstrument.Round2TickSize(tgt1X * 20 * TickSize));
				ShortTgt2.Set(Low[0] - Instrument.MasterInstrument.Round2TickSize(tgt2X * 20 * TickSize));
				ShortTgt3.Set(Low[0] - Instrument.MasterInstrument.Round2TickSize(tgt3X * 20 * TickSize));
			}

		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongTgt1
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongTgt2
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongTgt3
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongEntry
        {
            get { return Values[3]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongStop
        {
            get { return Values[4]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTgt1
        {
            get { return Values[5]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTgt2
        {
            get { return Values[6]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTgt3
        {
            get { return Values[7]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortEntry
        {
            get { return Values[8]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortStop
        {
            get { return Values[9]; }
        }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries JumpLineSlow
		{
			get { return Values[10]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries JumpLineFast
		{
			get { return Values[11]; }
		}		

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries TrailingStop
		{
			get { return Values[12]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries TrendFilter
		{
			get { return Values[13]; }
		}
		
        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt1X
        {
            get { return tgt1X; }
            set { tgt1X = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt2X
        {
            get { return tgt2X; }
            set { tgt2X = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt3X
        {
            get { return tgt3X; }
            set { tgt3X = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EMA1Len
        {
            get { return eMA1Len; }
            set { eMA1Len = Math.Max(1, value); }
        }
				
        [Description("")]
        [GridCategory("Parameters")]
        public int EMA2Len
        {
            get { return eMA2Len; }
            set { eMA2Len = Math.Max(1, value); }
        }
				
        [Description("")]
        [GridCategory("Parameters")]
        public int JLLengthSlow
        {
            get { return jLLengthSlow; }
            set { jLLengthSlow = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int JLLengthFast
        {
            get { return jLLengthFast; }
            set { jLLengthFast = Math.Max(1, value); }
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
        private TrendJumper[] cacheTrendJumper = null;

        private static TrendJumper checkTrendJumper = new TrendJumper();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public TrendJumper TrendJumper(int eMA1Len, int eMA2Len, int jLLengthFast, int jLLengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return TrendJumper(Input, eMA1Len, eMA2Len, jLLengthFast, jLLengthSlow, tgt1X, tgt2X, tgt3X);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public TrendJumper TrendJumper(Data.IDataSeries input, int eMA1Len, int eMA2Len, int jLLengthFast, int jLLengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            if (cacheTrendJumper != null)
                for (int idx = 0; idx < cacheTrendJumper.Length; idx++)
                    if (cacheTrendJumper[idx].EMA1Len == eMA1Len && cacheTrendJumper[idx].EMA2Len == eMA2Len && cacheTrendJumper[idx].JLLengthFast == jLLengthFast && cacheTrendJumper[idx].JLLengthSlow == jLLengthSlow && Math.Abs(cacheTrendJumper[idx].Tgt1X - tgt1X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt2X - tgt2X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt3X - tgt3X) <= double.Epsilon && cacheTrendJumper[idx].EqualsInput(input))
                        return cacheTrendJumper[idx];

            lock (checkTrendJumper)
            {
                checkTrendJumper.EMA1Len = eMA1Len;
                eMA1Len = checkTrendJumper.EMA1Len;
                checkTrendJumper.EMA2Len = eMA2Len;
                eMA2Len = checkTrendJumper.EMA2Len;
                checkTrendJumper.JLLengthFast = jLLengthFast;
                jLLengthFast = checkTrendJumper.JLLengthFast;
                checkTrendJumper.JLLengthSlow = jLLengthSlow;
                jLLengthSlow = checkTrendJumper.JLLengthSlow;
                checkTrendJumper.Tgt1X = tgt1X;
                tgt1X = checkTrendJumper.Tgt1X;
                checkTrendJumper.Tgt2X = tgt2X;
                tgt2X = checkTrendJumper.Tgt2X;
                checkTrendJumper.Tgt3X = tgt3X;
                tgt3X = checkTrendJumper.Tgt3X;

                if (cacheTrendJumper != null)
                    for (int idx = 0; idx < cacheTrendJumper.Length; idx++)
                        if (cacheTrendJumper[idx].EMA1Len == eMA1Len && cacheTrendJumper[idx].EMA2Len == eMA2Len && cacheTrendJumper[idx].JLLengthFast == jLLengthFast && cacheTrendJumper[idx].JLLengthSlow == jLLengthSlow && Math.Abs(cacheTrendJumper[idx].Tgt1X - tgt1X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt2X - tgt2X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt3X - tgt3X) <= double.Epsilon && cacheTrendJumper[idx].EqualsInput(input))
                            return cacheTrendJumper[idx];

                TrendJumper indicator = new TrendJumper();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EMA1Len = eMA1Len;
                indicator.EMA2Len = eMA2Len;
                indicator.JLLengthFast = jLLengthFast;
                indicator.JLLengthSlow = jLLengthSlow;
                indicator.Tgt1X = tgt1X;
                indicator.Tgt2X = tgt2X;
                indicator.Tgt3X = tgt3X;
                Indicators.Add(indicator);
                indicator.SetUp();

                TrendJumper[] tmp = new TrendJumper[cacheTrendJumper == null ? 1 : cacheTrendJumper.Length + 1];
                if (cacheTrendJumper != null)
                    cacheTrendJumper.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTrendJumper = tmp;
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
        public Indicator.TrendJumper TrendJumper(int eMA1Len, int eMA2Len, int jLLengthFast, int jLLengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return _indicator.TrendJumper(Input, eMA1Len, eMA2Len, jLLengthFast, jLLengthSlow, tgt1X, tgt2X, tgt3X);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendJumper TrendJumper(Data.IDataSeries input, int eMA1Len, int eMA2Len, int jLLengthFast, int jLLengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return _indicator.TrendJumper(input, eMA1Len, eMA2Len, jLLengthFast, jLLengthSlow, tgt1X, tgt2X, tgt3X);
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
        public Indicator.TrendJumper TrendJumper(int eMA1Len, int eMA2Len, int jLLengthFast, int jLLengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return _indicator.TrendJumper(Input, eMA1Len, eMA2Len, jLLengthFast, jLLengthSlow, tgt1X, tgt2X, tgt3X);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendJumper TrendJumper(Data.IDataSeries input, int eMA1Len, int eMA2Len, int jLLengthFast, int jLLengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TrendJumper(input, eMA1Len, eMA2Len, jLLengthFast, jLLengthSlow, tgt1X, tgt2X, tgt3X);
        }
    }
}
#endregion
