#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion


// 
// Added comments, simplied code.	KBJ 30-Oct-2007
//
// For a FULL description of this indicator and how to use it,
// please see:  http://www.kumotrader.com/ichimoku_wiki/
//
// Revised to display the same colors for SenkouSpanA&B as found in above description.	31-Oct-2007
//
// This code was subsequently hacked up by goldfly on 25-Mar-2008

/* ***********************
Revisions and MOdifications: Paul Brenek

04/08/11: Added code to allow selection of up and down band area colors.
*/ //*********************

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("IchiCloud Kinko Hyo - \"Equilibrium Chart at a Glance\" - For a FULL description of this indicator and how to use it, please see:  http://www.kumotrader.com/ichimoku_wiki/")]
    [Gui.Design.DisplayName("IchiCloud Kinko Hyo")]
    public class IchiCloud : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int periodFast = 9;			// Default setting for PeriodFast
            private int periodMedium = 26;		// Default setting for PeriodMedium
            private int periodSlow = 52;		// Default setting for PeriodSlow
        // User defined variables (add any user defined variables below)
		
			private Color bandAreaColorUp = Color.Blue;
			private Color bandAreaColorDown = Color.Red;
			private int bandAreaColorOpacity = 2;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add( new Plot( new Pen(Color.Black, 1),  "SenkouSpanA" ));		// Predicts levels of future support/resistance  (Values[3])
			Add( new Plot( new Pen(Color.Gray, 1),   "SenkouSpanB" ));		// Predicts levels of future support/resistance	 (Values[4])
			
            CalculateOnBarClose	= true;						// Updated once per bar (as opposed to on every tick.)
            Overlay				= true;						// Display on top of the price in panel # 1.
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if ((CurrentBar < PeriodMedium) || (CurrentBar < PeriodFast))
				return;										// Wait until we have enough bars.

			//
			// The following is a "cloud display" that shows what support/resistance values might be based on prior hi/low averages.
			// The cloud is defined by span-A & span-B.  Span-A is the average of the TenkanSen & Kijunsen, but shifted PeriodMedium
			// bars forward in time, and Span-B is the long-term (slow) average of the lowest-low & highest-high, shifted forward as well.
			//
			if ((CurrentBar < PeriodFast+PeriodMedium) || (CurrentBar < PeriodMedium+PeriodMedium) || (CurrentBar < PeriodSlow+PeriodMedium)) return;

			SenkouSpanA.Set( ( MAX(High,PeriodFast)[0] + MIN(Low,PeriodFast)[0]
							 + MAX(High,PeriodMedium)[0] + MIN(Low,PeriodMedium)[0] ) / 4 );

			SenkouSpanB.Set( ( MAX(High,PeriodSlow)[0] + MIN(Low,PeriodSlow)[0] ) / 2 );
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SenkouSpanA
        {
            get { return Values[0]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SenkouSpanB
        {
            get { return Values[1]; }
        }

        [Description("The number 9 represents a week and a half")]
        [Category("Parameters")]
        public int PeriodFast
        {
            get { return periodFast; }
            set { periodFast = Math.Max(2, value); }
        }

        [Description("The number 26 represents standard Japanese work month")]
        [Category("Parameters")]
        public int PeriodMedium
        {
            get { return periodMedium; }
            set { periodMedium = Math.Max(4, value); }
        }

        [Description("The number 52 represents two months")]
        [Category("Parameters")]
        public int PeriodSlow
        {
            get { return periodSlow; }
            set { periodSlow = Math.Max(6, value); }
        }
// PB modification, Apr 8, 2011
		[XmlIgnore()]
        [Description("ColorUp Band Area")]
        [Category("Band Colors")]
		[Gui.Design.DisplayNameAttribute("Up Band Area Color")]
        public Color BandAreaColorUp
        {
            get { return bandAreaColorUp; }
            set { bandAreaColorUp = value; }
        }
		
		[Browsable(false)]
		public string BandAreaColorUpSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bandAreaColorUp); }
			set { bandAreaColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("ColorDown Band Area")]
        [Category("Band Colors")]
		[Gui.Design.DisplayNameAttribute("Down Band Area Color")]
        public Color BandAreaColorDown
        {
            get { return bandAreaColorDown; }
            set { bandAreaColorDown = value; }
        }
		
		[Browsable(false)]
		public string BandAreaColorDownSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bandAreaColorDown); }
			set { bandAreaColorDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
        #endregion
		
		//
		//  This Plot() method has been tweaked so it will color in two different colors
		//  in-between the two plots of Values[3] and Values[4].
		//
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			base.Plot(graphics, bounds, min, max);

			if (Bars == null || ChartControl == null)
				return;

			SolidBrush brush;								// Set current brush color here.

			SolidBrush brushUP	 = new SolidBrush(Color.FromArgb(bandAreaColorOpacity*20,bandAreaColorUp));
			SolidBrush brushDOWN = new SolidBrush(Color.FromArgb(bandAreaColorOpacity*20,bandAreaColorDown));

			int	barWidth = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			GraphicsPath path = new GraphicsPath();

			DataSeries series0 = (DataSeries) Values[0];	// Color in between these two plots.
			DataSeries series1 = (DataSeries) Values[1];

			brush = brushUP;								// Start with the upwards color.
			int barcount = 0;								// Start with leftmost bar.
			bool firstbar = true;							// Plotting the first bar.
			
			while (barcount < ChartControl.BarsPainted)		// Continue until all bars have been painted.
			{
				int count = 0;								// Counter for innner loop.
				for (int seriesCount = 0; seriesCount < 2; seriesCount++)
				{
					int	lastX = -1;
					int	lastY = -1;
					DataSeries	series = (DataSeries) Values[seriesCount];
					Gui.Chart.Plot plot = Plots[seriesCount];
	
					for (count = barcount; count < ChartControl.BarsPainted; count++)
					{
						int idx = ChartControl.LastBarPainted - ChartControl.BarsPainted + 1 + count;
						if (idx < 0 || idx >= Input.Count || (!ChartControl.ShowBarsRequired && idx < BarsRequired))
							continue;

						double val	= series.Get(idx);				// Get next y-value to be plotted.
						if (val == 0)								// If nothing to plot...
							continue;								// ...ignore the enrtry.
	
						int	x = (int) (ChartControl.CanvasRight - ChartControl.BarMarginRight - barWidth / 2
							  + (count - ChartControl.BarsPainted + 1) * ChartControl.BarSpace) + 1;

						int	y = (int) ((bounds.Y + bounds.Height) - ((val - min ) / Gui.Chart.ChartControl.MaxMinusMin(max, min)) * bounds.Height);

						double val0 = series0.Get(idx);
						double val1 = series1.Get(idx);
						if (((val0 > val1) && (brush != brushUP))	// Now going in wrong direction?
						 || ((val0 < val1) && (brush != brushDOWN)))
						{											// Yes.  Done with this loop.
							if (lastX >= 0)							// Was there a last point?
							{										// Yes.  Connect it to the position half-way to this one.
								path.AddLine( lastX - plot.Pen.Width / 2, lastY, (x +lastX - plot.Pen.Width) / 2, (lastY + y)/2);
																	// Plot vertex of cross-over of the lines (1/2 way point).
							}
							break;									// Done, exit inner loop to change color.
						}

						if (firstbar == false)						// Is this the first plotted bar of the chart?
						{											// No.  Plot all bars after the first one.
							if (count == barcount)					// First bar after direction change (and color swap)?
							{										// Yes.  Add line segment for cross-over, 1/2 bar back.
								double valm1 = series.Get(idx-1);	// Get prior y-value to be plotted.
								lastX = x - ChartControl.BarSpace/2;// Back up 1/2 a bar for x-value.
								lastY = (y + (int) ((bounds.Y + bounds.Height) - ((valm1 - min ) / Gui.Chart.ChartControl.MaxMinusMin(max, min)) * bounds.Height))/2;
							}

							path.AddLine(lastX - plot.Pen.Width / 2, lastY, x - plot.Pen.Width / 2, y);	// Connect last point to this one.
						}
						firstbar = false;							// No longer the first bar.
						lastX = x;									// Save current position for next time, so we can connect the dots.
						lastY = y;
					}
					path.Reverse();									// Go back the other direction.
				}
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.FillPath(brush, path);
				path.Reset();										// Eliminate points already colored.

				barcount = count;									// Get ready to process next segment.
				brush = (brush == brushUP) ? brushDOWN : brushUP;	// Switch colors for next segment.
			}
			graphics.SmoothingMode = oldSmoothingMode;				// Restore smoothing mode before exiting.
		}
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private IchiCloud[] cacheIchiCloud = null;

        private static IchiCloud checkIchiCloud = new IchiCloud();

        /// <summary>
        /// IchiCloud Kinko Hyo - please see http://www.kumotrader.com/ichimoku_wiki/index.php?title=Main_Page
        /// </summary>
        /// <returns></returns>
        public IchiCloud IchiCloud(int periodFast, int periodMedium, int periodSlow)
        {
            return IchiCloud(Input, periodFast, periodMedium, periodSlow);
        }

        /// <summary>
        /// IchiCloud Kinko Hyo - please see http://www.kumotrader.com/ichimoku_wiki/index.php?title=Main_Page
        /// </summary>
        /// <returns></returns>
        public IchiCloud IchiCloud(Data.IDataSeries input, int periodFast, int periodMedium, int periodSlow)
        {
            checkIchiCloud.PeriodFast = periodFast;
            periodFast = checkIchiCloud.PeriodFast;
            checkIchiCloud.PeriodMedium = periodMedium;
            periodMedium = checkIchiCloud.PeriodMedium;
            checkIchiCloud.PeriodSlow = periodSlow;
            periodSlow = checkIchiCloud.PeriodSlow;

            if (cacheIchiCloud != null)
                for (int idx = 0; idx < cacheIchiCloud.Length; idx++)
                    if (cacheIchiCloud[idx].PeriodFast == periodFast && cacheIchiCloud[idx].PeriodMedium == periodMedium && cacheIchiCloud[idx].PeriodSlow == periodSlow && cacheIchiCloud[idx].EqualsInput(input))
                        return cacheIchiCloud[idx];

            IchiCloud indicator = new IchiCloud();
            indicator.SetUp();
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.PeriodFast = periodFast;
            indicator.PeriodMedium = periodMedium;
            indicator.PeriodSlow = periodSlow;

            IchiCloud[] tmp = new IchiCloud[cacheIchiCloud == null ? 1 : cacheIchiCloud.Length + 1];
            if (cacheIchiCloud != null)
                cacheIchiCloud.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheIchiCloud = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// IchiCloud Kinko Hyo - please see http://www.kumotrader.com/ichimoku_wiki/index.php?title=Main_Page
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.IchiCloud IchiCloud(int periodFast, int periodMedium, int periodSlow)
        {
            return _indicator.IchiCloud(Input, periodFast, periodMedium, periodSlow);
        }

        /// <summary>
        /// IchiCloud Kinko Hyo - please see http://www.kumotrader.com/ichimoku_wiki/index.php?title=Main_Page
        /// </summary>
        /// <returns></returns>
        public Indicator.IchiCloud IchiCloud(Data.IDataSeries input, int periodFast, int periodMedium, int periodSlow)
        {
            return _indicator.IchiCloud(input, periodFast, periodMedium, periodSlow);
        }

    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// IchiCloud Kinko Hyo - please see http://www.kumotrader.com/ichimoku_wiki/index.php?title=Main_Page
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.IchiCloud IchiCloud(int periodFast, int periodMedium, int periodSlow)
        {
            return _indicator.IchiCloud(Input, periodFast, periodMedium, periodSlow);
        }

        /// <summary>
        /// IchiCloud Kinko Hyo - please see http://www.kumotrader.com/ichimoku_wiki/index.php?title=Main_Page
        /// </summary>
        /// <returns></returns>
        public Indicator.IchiCloud IchiCloud(Data.IDataSeries input, int periodFast, int periodMedium, int periodSlow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.IchiCloud(input, periodFast, periodMedium, periodSlow);
        }

    }
}
#endregion
