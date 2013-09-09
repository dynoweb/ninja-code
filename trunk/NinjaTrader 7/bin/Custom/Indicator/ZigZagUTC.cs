/*
ZigZagUTC - another zig zag indicator
by Alex Matulich, anachronist@gmail.com
Unicorn Research Corporation, September 2008

I derived this from the verbal description on page 39 of
_The Ultimate Trading Guide_ by John R. Hill, George Pruitt, and Lundy Hill
published (2000) by John Wiley and Sons, Inc, ISBN 0-471-38135-7

Instead of using an arbitrary pullback amount to identify a swing high
or a swing low, this zigzag indicator uses an objective definition:
A swing high bar is the highest high prior to penetration of the low
of that highest high bar. A swing low bar is the lowest low prior to
penetration of the high of that lowest low bar.

This indicator goes a bit further by introducing a 'span' parameter.
When span=1, the indicator behaves as described in the book. When
span>1, the highs and lows are calculated as the highest and lowest in
a sliding window 'span' bars wide. Increasing the size of 'span' causes
the indicator to ignore the smaller swings. When span=6, this indicator
matches Larry Williams' zigzag description in his "Money Tree" course.

If you set "Use Highs and Lows" to false, then only input prices (such
as closes) will be used. In this case, you should increase span to 2 or
higher to avoid identifying nearly every price as a swing.

This indicator has a number of public properties that can be accessed
from other indicators.
*/
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
    /// ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.
    /// </summary>
    [Description("ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.")]
    public class ZigZagUTC : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int span = 2; // Default setting for Span
			private bool useHiLo = true; // Default setting for UseHiLo
			private Color zigZagColor = Color.Green; // Default setting for zigZagColor
			private int show = 3; // Default setting for Show
        // User defined variables (add any user defined variables below)
			private DataSeries hi, lo;
			private bool drawlines, showdots;
			private int linewidth = 2;
		// Useful public properties
			public int dir;         // direction of current zigzag trend, 1=up, -1=down
			public int lasthibar;   // bar number of last swing high (bars ago = CurrentBar-lasthibar)
			public int lastlobar;   // bar number of last swing low
			public double lasthi,prevhi;   // value of last swing high, previous is the one prior to the last
			public double lastlo,prevlo;   // value of last swing low, previous is the one prior to the last
			
			string longRelationship = "";
			string shortRelationship = "";
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.DarkOrchid), 2), PlotStyle.Dot, "ZigZagDot"));
            CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= true;

			hi = new DataSeries(this);
			lo = new DataSeries(this);
			dir = 0;
			drawlines = ((show & 1) > 0);
			showdots = ((show & 2) > 0);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar == 0) { // first bar initialization
				lasthi = hi[0] = useHiLo ? High[0] : Input[0];
				lastlo = lo[0] = useHiLo ? Low[0] : Input[0];
				lasthibar = lastlobar = 0;
				return;
			}

			// get high and low for last 'span' number of bars
			Hi.Set(MAX(useHiLo ? High : Input, span)[0]);
			Lo.Set(MIN(useHiLo ? Low : Input, span)[0]);

			// set initial trend direction (dir will become non-zero after the first couple of bars)
			if (dir == 0) { // set initial direction
				if (hi[0] > lasthi) {
					lasthi = hi[0]; lasthibar = CurrentBar;
					if (lo[0] > lo[1]) dir = 1;
				}
				if (lo[0] < lastlo) {
					lastlo = lo[0]; lastlobar = CurrentBar;
					if (hi[0] < hi[1]) dir = -1;
				}
			}

			// look for swing points and draw lines
			if (dir > 0) { // trend is up, look for new swing high
				if (hi[0] > lasthi) { // found a higher high
					lasthi = hi[0]; lasthibar = CurrentBar;
					if (drawlines) // draw/re-draw upward (current trend) line from lastlobar
						DrawLine(lastlobar.ToString(), AutoScale, CurrentBar-lastlobar, lastlo, CurrentBar-lasthibar, lasthi, zigZagColor, DashStyle.Solid, linewidth); 
				}
				else if (hi[0] < lasthi && lo[0] < lo[1]) { // found a swing high
					if (drawlines) // redraw the upward line from lastlobar to new swing high
						DrawLine(lastlobar.ToString(), AutoScale, CurrentBar-lastlobar, lastlo, CurrentBar-lasthibar, lasthi, zigZagColor, DashStyle.Solid, linewidth); 
					dir = -1;                               // trend direction is now down
					lastlo = lo[0]; lastlobar = CurrentBar; // now seeking new lows
					if (drawlines) // start new trendline from new swing high to most recent low
						DrawLine(lasthibar.ToString(), AutoScale, CurrentBar-lasthibar, lasthi, CurrentBar-lastlobar, lastlo, zigZagColor, DashStyle.Solid, linewidth);
					if (showdots) 
						ZigZagDot.Set(CurrentBar-lasthibar, lasthi);
					
					// The following added by Rick
					int dtbStrength = 15;
					double dtbOffset = ATR(14)[CurrentBar - lasthibar] * dtbStrength / 100;
					if (lasthi > prevhi - dtbOffset && lasthi < prevhi + dtbOffset)
					{  
						shortRelationship = "DT";
			            DrawText(lasthibar + "txt", "DT", CurrentBar-lasthibar, lasthi+1*TickSize, Color.Orange);
					} else if (lasthi > prevhi) {
						shortRelationship = "HH";
						DrawText(lasthibar + "txt", "HH", CurrentBar-lasthibar, lasthi+1*TickSize, Color.Green);
					} else {
						shortRelationship = "LH";
						DrawText(lasthibar + "txt", "LH", CurrentBar-lasthibar, lasthi+1*TickSize, Color.Red);
					}
					prevhi = lasthi;
					
				}

			} else { // dir < 0, trend is down, look for new swing low
				if (lo[0] < lastlo) { // found a lower low
					lastlo = lo[0]; lastlobar = CurrentBar;
					if (drawlines) // draw/re-draw downward (current trend) line from lasthibar
						DrawLine(lasthibar.ToString(), AutoScale, CurrentBar-lasthibar, lasthi, CurrentBar-lastlobar, lastlo, zigZagColor, DashStyle.Solid, linewidth);
				}
				else if (lo[0] > lastlo && hi[0] > hi[1]) { // found a swing low
					if (drawlines) // redraw the downward line from lastlobar to new swing low
						DrawLine(lasthibar.ToString(), AutoScale, CurrentBar-lasthibar, lasthi, CurrentBar-lastlobar, lastlo, zigZagColor, DashStyle.Solid, linewidth);
					dir = 1;                                // trend direction is now up
					lasthi = hi[0]; lasthibar = CurrentBar; // now seeking new highs
					if (drawlines) {// start new trendline from new swing low to most recent high
						DrawLine(lastlobar.ToString(), AutoScale, CurrentBar-lastlobar, lastlo, CurrentBar-lasthibar, lasthi, zigZagColor, DashStyle.Solid, linewidth);
					}
					if (showdots) 
						ZigZagDot.Set(CurrentBar-lastlobar, lastlo);
					
					// The following added by Rick
					int dtbStrength = 15;
		            double dtbOffset = ATR(14)[CurrentBar - lastlobar] * dtbStrength / 100;
		            if (lastlo > prevlo - dtbOffset && lastlo < prevlo + dtbOffset) {
						longRelationship = "DB";
        		        DrawText(lastlobar + "txt", "DB", CurrentBar-lastlobar, lastlo-1*TickSize, Color.Orange);
					} else if (lastlo < prevlo) {
						longRelationship = "LL";
						DrawText(lastlobar + "txt", "LL", CurrentBar-lastlobar, lastlo-1*TickSize, Color.Red);
					} else {
						longRelationship = "HL";
						DrawText(lastlobar + "txt", "HL", CurrentBar-lastlobar, lastlo-1*TickSize, Color.Green);
					}
					prevlo = lastlo;
				}
			}
			// Looking for a short entry (LL then LH)
			if (longRelationship == "LL" && shortRelationship == "LH" 
				&& lasthibar < lastlobar
				&& High[CurrentBar-lasthibar] > EMA(13)[CurrentBar-lasthibar]
				&& Close[0] <= Low[CurrentBar-lasthibar]
				)
			{
				// CurrentBar-lastlobar
				DrawArrowDown(CurrentBar + "dnArrow", 0, High[0] + 1 * TickSize, Color.Red);
				DrawDot(CurrentBar + "shortEntry", true, 0, Close[0], Color.Magenta);
			}
        }

        #region Properties
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ZigZagDot
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Hi
        {
            get { return hi; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Lo
        {
            get { return lo; }
        }

        [Description("Bar span to consider for highest and lowest values")]
        [Category("Parameters")]
        public int Span
        {
            get { return span; }
            set { span = Math.Max(1, value); }
        }

		[Description("Color of zigzag lines")]
		[Category("Parameters")]
		public Color ZigZagColor
		{
			get { return zigZagColor; }
			set { zigZagColor = value; }
		}
		[Browsable(false)]
		public string PaintColorSerialize
		{
     		get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zigZagColor); }
     		set { zigZagColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("true = use Highs and Lows for swings, false = use price input")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("Use high and low")]
		[RefreshProperties(RefreshProperties.All)]
        public bool UseHiLo
        {
            get { return useHiLo; }
            set { useHiLo = value; PriceTypeSupported = !value; }
        }

		[Description("What to show: 1=zigzag lines, 2=swing dots, 3=both")]
        [Category("Parameters")]
        public int Show
        {
            get { return show; }
            set { show = Math.Max(1, Math.Min(3, value)); }
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
        private ZigZagUTC[] cacheZigZagUTC = null;

        private static ZigZagUTC checkZigZagUTC = new ZigZagUTC();

        /// <summary>
        /// ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.
        /// </summary>
        /// <returns></returns>
        public ZigZagUTC ZigZagUTC(int show, int span, bool useHiLo, Color zigZagColor)
        {
            return ZigZagUTC(Input, show, span, useHiLo, zigZagColor);
        }

        /// <summary>
        /// ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.
        /// </summary>
        /// <returns></returns>
        public ZigZagUTC ZigZagUTC(Data.IDataSeries input, int show, int span, bool useHiLo, Color zigZagColor)
        {
            if (cacheZigZagUTC != null)
                for (int idx = 0; idx < cacheZigZagUTC.Length; idx++)
                    if (cacheZigZagUTC[idx].Show == show && cacheZigZagUTC[idx].Span == span && cacheZigZagUTC[idx].UseHiLo == useHiLo && cacheZigZagUTC[idx].ZigZagColor == zigZagColor && cacheZigZagUTC[idx].EqualsInput(input))
                        return cacheZigZagUTC[idx];

            lock (checkZigZagUTC)
            {
                checkZigZagUTC.Show = show;
                show = checkZigZagUTC.Show;
                checkZigZagUTC.Span = span;
                span = checkZigZagUTC.Span;
                checkZigZagUTC.UseHiLo = useHiLo;
                useHiLo = checkZigZagUTC.UseHiLo;
                checkZigZagUTC.ZigZagColor = zigZagColor;
                zigZagColor = checkZigZagUTC.ZigZagColor;

                if (cacheZigZagUTC != null)
                    for (int idx = 0; idx < cacheZigZagUTC.Length; idx++)
                        if (cacheZigZagUTC[idx].Show == show && cacheZigZagUTC[idx].Span == span && cacheZigZagUTC[idx].UseHiLo == useHiLo && cacheZigZagUTC[idx].ZigZagColor == zigZagColor && cacheZigZagUTC[idx].EqualsInput(input))
                            return cacheZigZagUTC[idx];

                ZigZagUTC indicator = new ZigZagUTC();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Show = show;
                indicator.Span = span;
                indicator.UseHiLo = useHiLo;
                indicator.ZigZagColor = zigZagColor;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZigZagUTC[] tmp = new ZigZagUTC[cacheZigZagUTC == null ? 1 : cacheZigZagUTC.Length + 1];
                if (cacheZigZagUTC != null)
                    cacheZigZagUTC.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZigZagUTC = tmp;
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
        /// ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZigZagUTC ZigZagUTC(int show, int span, bool useHiLo, Color zigZagColor)
        {
            return _indicator.ZigZagUTC(Input, show, span, useHiLo, zigZagColor);
        }

        /// <summary>
        /// ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZigZagUTC ZigZagUTC(Data.IDataSeries input, int show, int span, bool useHiLo, Color zigZagColor)
        {
            return _indicator.ZigZagUTC(input, show, span, useHiLo, zigZagColor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZigZagUTC ZigZagUTC(int show, int span, bool useHiLo, Color zigZagColor)
        {
            return _indicator.ZigZagUTC(Input, show, span, useHiLo, zigZagColor);
        }

        /// <summary>
        /// ZigZag indicator from the Ultimate Trading Guide by Hill, Pruitt, and Hill. A swing high occurs when price takes out the low of the highest bar. A swing low occurs when price takes out the high of the lowest bar. Span setting determines how many bars to look back for highest highs and lowest lows; i.e. span=5 on daily bars find weekly swings.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZigZagUTC ZigZagUTC(Data.IDataSeries input, int show, int span, bool useHiLo, Color zigZagColor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZigZagUTC(input, show, span, useHiLo, zigZagColor);
        }
    }
}
#endregion
