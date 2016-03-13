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
    /// Concept copied from John Carter's TTM Squeeze indicator
	/// Written by Rick Cromer
	/// 9/12/2015 - 3/10/2016
    /// </summary>
    [Description("Concept copied from John Carter's TTM Squeeze indicator")]
    public class RCTTMSqueeze : Indicator
    {
        #region Variables
        
            private int period = 20; 
            private double offsetMultiplier = 2.0; 
            private double numStdDev = 2.0; 
            private double alertLine = 1; 
        
			private Color posUpColor			= Color.Cyan;
			private Color posDownColor			= Color.Blue;
			private Color negDownColor			= Color.Red;
			private Color negUpColor			= Color.Gold;
		
			private Color alertColor			= Color.LawnGreen;
			private Color normalColor			= Color.Red;
		
			private DataSeries		diff;
			private DataSeries		mo;
			private BoolSeries  inSqueeze;
		
			// Momentum
			private int				smoothPeriod	= 6;
			private int				moPeriod		= 15;	
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Bar, "Histogram"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Dot, "VolComp"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), PlotStyle.Line, "SqueezeAlert"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "VC"));
            Add(new Line(Color.FromKnownColor(KnownColor.LightCoral), 0, "SA"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 0, "Zero"));			
			
			diff				= new DataSeries(this);
			mo					= new DataSeries(this);
			inSqueeze			= new BoolSeries(this);
			
			Plots[0].Pen.Width = 3;
			Plots[1].Pen.Width = 2;
			
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Bollinger calculations
			//====================================
			double smaValue    = SMA(Period)[0];
		    double stdDevValue = StdDev(Period)[0];
            double bUpper = smaValue + NumStdDev * stdDevValue;
            double bMiddle = smaValue;
            double bLower = smaValue - NumStdDev * stdDevValue;
			double bWidth = bUpper - bLower;
			
			// KeltnerChannel calculations
			//====================================
			diff.Set(High[0] - Low[0]);
			double middle	= SMA(Typical, Period)[0];
			double offset	= SMA(diff, Period)[0] * offsetMultiplier;
			double upper	= middle + offset;
			double lower	= middle - offset;
			double kWidth = upper - lower;
							
			// Set momentum
			double momentum = CurrentBar == 0 ? 0 : Input[0] - Input[Math.Min(CurrentBar, moPeriod)];
			mo.Set(momentum);
			
			//====================================

			double squeezeRatio = bWidth/kWidth;
			//SqueezeAlert.Set(bWidth/kWidth);
			
			// Smoothing out momentum
			double smoothedMo = SMA(mo, smoothPeriod)[0];
			Histogram.Set(smoothedMo);			
			//Value.Set(SMA(mo, Period)[0]);	// alternative syntax

			if (CurrentBar > 1) 
			{
				if (Histogram[0] > 0.0)
				{
					//if (Stochastics(periodD, periodK, smooth).K[0] < 50) 
					if (Histogram[0] > Histogram[1])
					{
						PlotColors[0][0] = PosUpColor;
					}
					else 
					{
						PlotColors[0][0] = PosDownColor;
					}
				}
				else
				{	
					//if (Rising(Stochastics(periodD, periodK, smooth).K)) 
					//if (Stochastics(periodD, periodK, smooth).K[0] > 50) 
					if (Histogram[0] < Histogram[1])
					{
						PlotColors[0][0] = NegDownColor;
					}
					else 
					{
						PlotColors[0][0] = NegUpColor;
					}
				}
				
				VolComp.Set(0);
				inSqueeze.Set((squeezeRatio < alertLine) ? true : false);
				if (inSqueeze[0])
				{
					// in a squeeze
					PlotColors[1][0] = normalColor;
				}
				else
				{
					PlotColors[1][0] = alertColor;
				}
			}
        }

        #region Properties	
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Histogram
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VolComp
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SqueezeAlert
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public BoolSeries InSqueeze
        {
            get { return inSqueeze; }
        }

		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Uptrend")]
		public Color PosUpColor
		{
			get { return posUpColor; }
			set { posUpColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string PosUpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(posUpColor); }
			set { posUpColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Uptrend")]
		public Color PosDownColor
		{
			get { return posDownColor; }
			set { posDownColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string PosDownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(posDownColor); }
			set { posDownColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Downtrend")]
		public Color NegDownColor
		{
			get { return negDownColor; }
			set { negDownColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string NegDownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(negDownColor); }
			set { negDownColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("Select color for Falling Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Downtrend")]
		public Color NegUpColor
		{
			get { return negUpColor; }
			set { negUpColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string NegUpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(negUpColor); }
			set { negUpColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
						
        [Description("")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double OffsetMultiplier
        {
            get { return offsetMultiplier; }
            set { offsetMultiplier = Math.Max(0.25, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double NumStdDev
        {
            get { return numStdDev; }
            set { numStdDev = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double AlertLine
        {
            get { return alertLine; }
            set { alertLine = Math.Max(1, value); }
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
        private RCTTMSqueeze[] cacheRCTTMSqueeze = null;

        private static RCTTMSqueeze checkRCTTMSqueeze = new RCTTMSqueeze();

        /// <summary>
        /// Concept copied from John Carter's TTM Squeeze indicator
        /// </summary>
        /// <returns></returns>
        public RCTTMSqueeze RCTTMSqueeze(double alertLine, double numStdDev, double offsetMultiplier, int period)
        {
            return RCTTMSqueeze(Input, alertLine, numStdDev, offsetMultiplier, period);
        }

        /// <summary>
        /// Concept copied from John Carter's TTM Squeeze indicator
        /// </summary>
        /// <returns></returns>
        public RCTTMSqueeze RCTTMSqueeze(Data.IDataSeries input, double alertLine, double numStdDev, double offsetMultiplier, int period)
        {
            if (cacheRCTTMSqueeze != null)
                for (int idx = 0; idx < cacheRCTTMSqueeze.Length; idx++)
                    if (Math.Abs(cacheRCTTMSqueeze[idx].AlertLine - alertLine) <= double.Epsilon && Math.Abs(cacheRCTTMSqueeze[idx].NumStdDev - numStdDev) <= double.Epsilon && Math.Abs(cacheRCTTMSqueeze[idx].OffsetMultiplier - offsetMultiplier) <= double.Epsilon && cacheRCTTMSqueeze[idx].Period == period && cacheRCTTMSqueeze[idx].EqualsInput(input))
                        return cacheRCTTMSqueeze[idx];

            lock (checkRCTTMSqueeze)
            {
                checkRCTTMSqueeze.AlertLine = alertLine;
                alertLine = checkRCTTMSqueeze.AlertLine;
                checkRCTTMSqueeze.NumStdDev = numStdDev;
                numStdDev = checkRCTTMSqueeze.NumStdDev;
                checkRCTTMSqueeze.OffsetMultiplier = offsetMultiplier;
                offsetMultiplier = checkRCTTMSqueeze.OffsetMultiplier;
                checkRCTTMSqueeze.Period = period;
                period = checkRCTTMSqueeze.Period;

                if (cacheRCTTMSqueeze != null)
                    for (int idx = 0; idx < cacheRCTTMSqueeze.Length; idx++)
                        if (Math.Abs(cacheRCTTMSqueeze[idx].AlertLine - alertLine) <= double.Epsilon && Math.Abs(cacheRCTTMSqueeze[idx].NumStdDev - numStdDev) <= double.Epsilon && Math.Abs(cacheRCTTMSqueeze[idx].OffsetMultiplier - offsetMultiplier) <= double.Epsilon && cacheRCTTMSqueeze[idx].Period == period && cacheRCTTMSqueeze[idx].EqualsInput(input))
                            return cacheRCTTMSqueeze[idx];

                RCTTMSqueeze indicator = new RCTTMSqueeze();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertLine = alertLine;
                indicator.NumStdDev = numStdDev;
                indicator.OffsetMultiplier = offsetMultiplier;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                RCTTMSqueeze[] tmp = new RCTTMSqueeze[cacheRCTTMSqueeze == null ? 1 : cacheRCTTMSqueeze.Length + 1];
                if (cacheRCTTMSqueeze != null)
                    cacheRCTTMSqueeze.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRCTTMSqueeze = tmp;
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
        /// Concept copied from John Carter's TTM Squeeze indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RCTTMSqueeze RCTTMSqueeze(double alertLine, double numStdDev, double offsetMultiplier, int period)
        {
            return _indicator.RCTTMSqueeze(Input, alertLine, numStdDev, offsetMultiplier, period);
        }

        /// <summary>
        /// Concept copied from John Carter's TTM Squeeze indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.RCTTMSqueeze RCTTMSqueeze(Data.IDataSeries input, double alertLine, double numStdDev, double offsetMultiplier, int period)
        {
            return _indicator.RCTTMSqueeze(input, alertLine, numStdDev, offsetMultiplier, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Concept copied from John Carter's TTM Squeeze indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RCTTMSqueeze RCTTMSqueeze(double alertLine, double numStdDev, double offsetMultiplier, int period)
        {
            return _indicator.RCTTMSqueeze(Input, alertLine, numStdDev, offsetMultiplier, period);
        }

        /// <summary>
        /// Concept copied from John Carter's TTM Squeeze indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.RCTTMSqueeze RCTTMSqueeze(Data.IDataSeries input, double alertLine, double numStdDev, double offsetMultiplier, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RCTTMSqueeze(input, alertLine, numStdDev, offsetMultiplier, period);
        }
    }
}
#endregion
