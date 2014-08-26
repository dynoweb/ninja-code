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
            private int eMA1Len = 14; // Default setting for EMA1Len
            private int eMA2Len = 55; // Default setting for EMA2Len
		    private int entryOffsetTicks = 2;
		    private int stopOffsetTicks = 1;
		    private int lengthFast = 2;
		    private int lengthSlow = 6;
            private double tgt1X = 1; // Default setting for Tgt1X
            private double tgt2X = 1.5; // Default setting for Tgt2X
            private double tgt3X = 2; // Default setting for Tgt3X
        // User defined variables (add any user defined variables below)
			int crossedAboveBarNumber = 0;
			int crossedBelowBarNumber = 0;
			bool lookingLong = false;
			bool lookingShort = false;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "LongTgt3"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "LongTgt2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "LongTgt1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Cross, "LongEntry"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "LongStop"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "ShortStop"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Cross, "ShortEntry"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "ShortTgt1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "ShortTgt2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Dot, "ShortTgt3"));

			Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "SlowLine"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Line, "FastLine"));
			
			Add(new Plot(Color.Fuchsia, PlotStyle.Line, "TrailingStop"));
			Add(new Plot(Color.MediumBlue, PlotStyle.Line, "TrendFilter"));
			
			Plots[10].Pen.DashStyle = DashStyle.Dash;
			Plots[11].Pen.DashStyle = DashStyle.Dash;
			
			Plots[10].Pen.Width = 2;
			
			PaintPriceMarkers = false;
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			FastLine.Set(Instrument.MasterInstrument.Round2TickSize(DonchianChannel(LengthFast)[0]));
			SlowLine.Set(Instrument.MasterInstrument.Round2TickSize(DonchianChannel(LengthSlow)[0]));
			TrailingStop.Set(EMA(eMA1Len)[0]);
			//TrendFilter.Set(EMA(eMA2Len)[0]);
			
			if (CrossAbove(FastLine, SlowLine, 1)
				&& crossedAboveBarNumber <= crossedBelowBarNumber)
			{
				crossedAboveBarNumber = CurrentBar;
				lookingLong = true;
				lookingShort = false;
			}
			else if (CrossBelow(FastLine, SlowLine, 1)
				&& crossedAboveBarNumber >= crossedBelowBarNumber)
			{
				crossedBelowBarNumber = CurrentBar;
				lookingLong = false;
				lookingShort = true;
			}
			
			// Cancel trade under thesse conditions
			if (lookingLong && High[0] + entryOffsetTicks * TickSize < TrailingStop[0])
				lookingLong = false;
			if (lookingShort && Low[0] - entryOffsetTicks * TickSize > TrailingStop[0])
				lookingShort = false;

			//Print(Time + " " + CurrentBar + " " + crossedAboveBarNumber + " " + crossedBelowBarNumber);
			
			double barHeight = Range()[0]; //High[0] - Low[0]; // 
			
			PlotColors[0][0] = Color.DarkBlue;
			PlotColors[1][0] = Color.DarkBlue;
			PlotColors[2][0] = Color.DarkBlue;
			PlotColors[3][0] = Color.DarkBlue;
			PlotColors[4][0] = Color.DarkBlue;
			PlotColors[5][0] = Color.DarkBlue;
			PlotColors[6][0] = Color.DarkBlue;
			PlotColors[7][0] = Color.DarkBlue;
			PlotColors[8][0] = Color.DarkBlue;
			PlotColors[9][0] = Color.DarkBlue;
			
			if (isLongSignal())
			{				
				LongEntry.Set(High[0] + entryOffsetTicks * TickSize);
				LongStop.Set(Low[0] - stopOffsetTicks * TickSize);
				LongTgt1.Set(High[0] + Instrument.MasterInstrument.Round2TickSize(tgt1X * barHeight));
				LongTgt2.Set(High[0] + Instrument.MasterInstrument.Round2TickSize(tgt2X * barHeight));
				LongTgt3.Set(High[0] + Instrument.MasterInstrument.Round2TickSize(tgt3X * barHeight));
			}
			if (isShortSignal())
			{
				ShortEntry.Set(Low[0] - entryOffsetTicks * TickSize);
				ShortStop.Set(High[0] + stopOffsetTicks * TickSize);
				ShortTgt1.Set(Low[0] - Instrument.MasterInstrument.Round2TickSize(tgt1X * barHeight));
				ShortTgt2.Set(Low[0] - Instrument.MasterInstrument.Round2TickSize(tgt2X * barHeight));
				ShortTgt3.Set(Low[0] - Instrument.MasterInstrument.Round2TickSize(tgt3X * barHeight));
			}

		}
		
		private bool isLongSignal() {
			
			if (lookingLong
				&& Close[0] >= Open[0]
				&& Close[0] > SlowLine[0]
				//&& TrailingStop[0] > TrailingStop[1]
				&& FastLine[0] >= FastLine[1]
				//&& (Low[0] + 1 * TickSize) > TrailingStop[0]
				)
			{
				lookingLong = false;
				return true;
			}
			else
				return false;
		}

		private bool isShortSignal() {
			
			if (lookingShort
				&& Close[0] <= Open[0]
				&& Close[0] < SlowLine[0]
				//&& TrailingStop[0] < TrailingStop[1]
				//&& FastLine[0] <= FastLine[1]
				
				)
			{
				lookingShort = false;
				return true;
			}
			else
				return false;
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongTgt3
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
        public DataSeries LongTgt1
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
        public DataSeries ShortStop
        {
            get { return Values[5]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortEntry
        {
            get { return Values[6]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTgt1
        {
            get { return Values[7]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTgt2
        {
            get { return Values[8]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTgt3
        {
            get { return Values[9]; }
        }

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries SlowLine
		{
			get { return Values[10]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries FastLine
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
        public int LengthSlow
        {
            get { return lengthSlow; }
            set { lengthSlow = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int LengthFast
        {
            get { return lengthFast; }
            set { lengthFast = Math.Max(1, value); }
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
        public TrendJumper TrendJumper(int eMA1Len, int eMA2Len, int lengthFast, int lengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return TrendJumper(Input, eMA1Len, eMA2Len, lengthFast, lengthSlow, tgt1X, tgt2X, tgt3X);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public TrendJumper TrendJumper(Data.IDataSeries input, int eMA1Len, int eMA2Len, int lengthFast, int lengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            if (cacheTrendJumper != null)
                for (int idx = 0; idx < cacheTrendJumper.Length; idx++)
                    if (cacheTrendJumper[idx].EMA1Len == eMA1Len && cacheTrendJumper[idx].EMA2Len == eMA2Len && cacheTrendJumper[idx].LengthFast == lengthFast && cacheTrendJumper[idx].LengthSlow == lengthSlow && Math.Abs(cacheTrendJumper[idx].Tgt1X - tgt1X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt2X - tgt2X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt3X - tgt3X) <= double.Epsilon && cacheTrendJumper[idx].EqualsInput(input))
                        return cacheTrendJumper[idx];

            lock (checkTrendJumper)
            {
                checkTrendJumper.EMA1Len = eMA1Len;
                eMA1Len = checkTrendJumper.EMA1Len;
                checkTrendJumper.EMA2Len = eMA2Len;
                eMA2Len = checkTrendJumper.EMA2Len;
                checkTrendJumper.LengthFast = lengthFast;
                lengthFast = checkTrendJumper.LengthFast;
                checkTrendJumper.LengthSlow = lengthSlow;
                lengthSlow = checkTrendJumper.LengthSlow;
                checkTrendJumper.Tgt1X = tgt1X;
                tgt1X = checkTrendJumper.Tgt1X;
                checkTrendJumper.Tgt2X = tgt2X;
                tgt2X = checkTrendJumper.Tgt2X;
                checkTrendJumper.Tgt3X = tgt3X;
                tgt3X = checkTrendJumper.Tgt3X;

                if (cacheTrendJumper != null)
                    for (int idx = 0; idx < cacheTrendJumper.Length; idx++)
                        if (cacheTrendJumper[idx].EMA1Len == eMA1Len && cacheTrendJumper[idx].EMA2Len == eMA2Len && cacheTrendJumper[idx].LengthFast == lengthFast && cacheTrendJumper[idx].LengthSlow == lengthSlow && Math.Abs(cacheTrendJumper[idx].Tgt1X - tgt1X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt2X - tgt2X) <= double.Epsilon && Math.Abs(cacheTrendJumper[idx].Tgt3X - tgt3X) <= double.Epsilon && cacheTrendJumper[idx].EqualsInput(input))
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
                indicator.LengthFast = lengthFast;
                indicator.LengthSlow = lengthSlow;
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
        public Indicator.TrendJumper TrendJumper(int eMA1Len, int eMA2Len, int lengthFast, int lengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return _indicator.TrendJumper(Input, eMA1Len, eMA2Len, lengthFast, lengthSlow, tgt1X, tgt2X, tgt3X);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendJumper TrendJumper(Data.IDataSeries input, int eMA1Len, int eMA2Len, int lengthFast, int lengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return _indicator.TrendJumper(input, eMA1Len, eMA2Len, lengthFast, lengthSlow, tgt1X, tgt2X, tgt3X);
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
        public Indicator.TrendJumper TrendJumper(int eMA1Len, int eMA2Len, int lengthFast, int lengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            return _indicator.TrendJumper(Input, eMA1Len, eMA2Len, lengthFast, lengthSlow, tgt1X, tgt2X, tgt3X);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendJumper TrendJumper(Data.IDataSeries input, int eMA1Len, int eMA2Len, int lengthFast, int lengthSlow, double tgt1X, double tgt2X, double tgt3X)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TrendJumper(input, eMA1Len, eMA2Len, lengthFast, lengthSlow, tgt1X, tgt2X, tgt3X);
        }
    }
}
#endregion
