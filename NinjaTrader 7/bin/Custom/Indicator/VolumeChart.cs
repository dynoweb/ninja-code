#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Tracks the ticks bought or sold at bid or ask
    /// </summary>
    [Description("Tracks the ticks bought or sold at bid or ask")]
    public class VolumeChart : Indicator
    {
        #region Variables
			internal struct VolumeInfo
			{
				public double up;
				public double down;
				public double total;
			}

		// Wizard generated variables
			private int ticksPerBarLine = 1;	// Currently only 1 supported
            private int volBarSize = 10; 	// Default setting for VolBarSize
            private int ticksPerColumn = 4000; // Default setting for ticksPerColumn
            private int colorThreshold = 75; // Bid/Ask delta color step
			private Color tickbarColor = Color.White;
		// User defined variables (add any user defined variables below)
			private SolidBrush		barBrush;
			private int				barSpacing		= 0;
			private int				transparency	= 20;  // started off at 80
			private int activeBar = -1;
			private int barWidth = 100;

			private bool firstPaint = true;
			private double previousVol = 0;
		
			private int ticks = 0;
			private static int alpha = 0; 
		
			// ref: http://msdn.microsoft.com/en-us/library/system.collections.hashtable.aspx
			// ref: http://msdn.microsoft.com/en-us/library/xfhwa508.aspx
			private Dictionary<double, VolumeInfo> volMap = new Dictionary<double, VolumeInfo>();
			private Dictionary<double, VolumeInfo> volMapPrior = new Dictionary<double, VolumeInfo>();
			
			private Brush textBrush = new SolidBrush(Color.Black);
			private Brush fillBrush = new SolidBrush(Color.WhiteSmoke);
			private Font stringFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			ChartOnly				= true;
			Overlay					= true;
			ZOrder					= +1;
			CalculateOnBarClose		= false;
			//RecalculateColors();
			alpha = (int) (255.0 * ((100.0 - transparency) / 100.0));		
			ChartControl.BarMarginLeft = barWidth*2;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			ticks++;
			
			if (CurrentBar < activeBar)
			{
				Print("CurrentBar < activeBar");
			    return;
			}
			
			
			if (CurrentBar != activeBar) {
				//Print("CurrentBar  != activeBar");
				previousVol = Volume[0];
				activeBar = CurrentBar;
			}
			
			
			//Print("ticks: " + ticks);
			//if (CurrentBar != activeBar)
			if (ticks > ticksPerColumn)
			{
				//previousVol = 0;
				activeBar = CurrentBar;
				//Print("------------ ticks: " + ticks + " ---------");				
				ticks = 0;
				Print("------------ NEW VOL BAR 2 ---------");				
				if (volMap.Count < 30) {
					foreach(KeyValuePair<double, VolumeInfo> kvp in volMap) {
				//		Print(kvp.Key + ", " + kvp.Value.down + ", " + kvp.Value.up);	
					}
				}
				volMapPrior = new Dictionary<double, VolumeInfo>(volMap);
				volMap = new Dictionary<double, VolumeInfo>();
			}

			if (firstPaint)
				firstPaint = false;
			else
			{
				VolumeInfo volInfo = new VolumeInfo();	
				if (volMap.TryGetValue(Close[0], out volInfo)) {
					//Print("tradeVol:  " + tradeVol + " Close[0]: " + Close[0] + " volInfo.up: " + volInfo.up + " volInfo.down: " + volInfo.down);
					volInfo = updateVolumeInfo(volInfo);
					volMap[Close[0]] = volInfo;
				} else {
					//Print("tradeVol:  " + tradeVol + " Close[0]: " + Close[0]);
					volInfo.down = 0;
					volInfo.up = 0;
					volInfo = updateVolumeInfo(volInfo);
					volMap.Add(Close[0], volInfo);
				}
			}

        }

		private VolumeInfo updateVolumeInfo(VolumeInfo vi) {
		
			double tradeVol = previousVol == 0 ? Volume[0] : Volume[0] - previousVol;
			previousVol = Volume[0];
			
			if (Close[0] >= GetCurrentAsk())
				vi.up += tradeVol;
			else if (Close[0] <= GetCurrentBid())
				vi.down += tradeVol;
			
			//Print("Volume[0]: " + Volume[0] + " Close[0]: " + GetCurrentAsk() + " GetCurrentAsk(): " + GetCurrentAsk() + " GetCurrentBid(): " + GetCurrentBid() + " buys: " + buys + " sells: " + sells);			
			//Print("tradeVol  " + tradeVol + " Close[0]  " + Close[0] + " vi.up  " + vi.up + " vi.down  " + vi.down);
			return vi;
		}
		
		private int GetYPos(double price, Rectangle bounds, double min, double max)
		{
			return ChartControl.GetYByValue(this, price);
		}

		private void RecalculateColors(double delta) 
		{
			Color barColor;
			int colorIndex = 0;
			
			int	alpha = (int) (255.0 * ((100.0 - transparency) / 100.0));		
			
			// ref - http://msdn.microsoft.com/en-us/library/system.array
			Color[] posColors = new Color[3] {
				Color.FromArgb(192,255,192),
				Color.LightGreen, 
				Color.Green };
			
			Color[] negColors = new Color[3] {
				Color.Pink, 
				Color.LightCoral,
				Color.Red };
			
			// ref - http://msdn.microsoft.com/en-us/library/system.math
			colorIndex = (int) Math.Abs(delta/colorThreshold);
			//if (colorIndex > 2)
			//	Print("colorIndex: " + colorIndex + " delta/colorThreshold: " + delta + "/" + colorThreshold);
			colorIndex = Math.Min(colorIndex, 2);
			
			if (delta < 0) {
				barColor = negColors[colorIndex];
			} else {
				barColor = posColors[colorIndex];
			}
			//Print("colorIndex: " + colorIndex + " barColor: " + barColor);
			
			if (barBrush != null)
				barBrush.Dispose();
			barBrush   = new SolidBrush(Color.FromArgb(alpha, barColor.R, barColor.G, barColor.B));
		}
		
		/**
		*
		* min, max is generated by the chart and indicates the price range on the chart that is in view
		*/
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			int x;
			int y;
			int height;
			Brush deltaBrush;
			double delta = 0;
			
			//Print("should be showing the ticks");
			//Brush tickBrush = new SolidBrush(Color.FromArgb(alpha, tickbarColor.R, tickbarColor.G, tickbarColor.B));
			
			//Print("volMap Count: " + volMap.Count);
			foreach(KeyValuePair<double, VolumeInfo> kvp in volMap) {
				//Print(kvp.Key + ", " + kvp.Value.down + ", " + kvp.Value.up);	
				double change = kvp.Value.up - kvp.Value.down;
				RecalculateColors(change);

				double priceUpper = kvp.Key + ticksPerBarLine * TickSize;
				double priceLower = kvp.Key;
				int yUpper = GetYPos(priceUpper, bounds, min, max) - barSpacing;
				int yLower = GetYPos(priceLower, bounds, min, max);
				height = yLower - yUpper;
				y = (int) yUpper + height / 2;	// perfect
				int yText = (int) yUpper + height * 3/4;
				
				if (change != 0) {
					graphics.FillRectangle(barBrush, new Rectangle(
						bounds.X + barWidth, y, 
						barWidth, height));

					// ref http://msdn.microsoft.com/en-us/library/system.string.format.aspx
					string text = String.Format("{0,-9:N2}{1,9:N0}", kvp.Key, (kvp.Value.up-kvp.Value.down));
					//Print(text);
					graphics.DrawString(text, stringFont, textBrush, bounds.X + barWidth, yText);
				}
				delta += change;	
			}
			
			//if (tickBrush != null)
			//	tickBrush.Dispose();
			
			x = bounds.X + barWidth + 10;
			y = 20;
			height = 20;
			//new Rectangle( x , y, width, height);
			graphics.FillRectangle(fillBrush, new Rectangle(x, y, barWidth-20, height));
			deltaBrush = delta > 0 ? new SolidBrush(Color.Blue) : new SolidBrush(Color.Red);
			// DrawString(string s, Font font, Brush brush, float x, float y);
			graphics.DrawString("change: " + delta, stringFont, deltaBrush, x, y);
			delta = 0;

			//Print("volMap Count: " + volMap.Count);
			foreach(KeyValuePair<double, VolumeInfo> kvp in volMapPrior) {
				//Print(kvp.Key + ", " + kvp.Value.down + ", " + kvp.Value.up);	
				double change = kvp.Value.up - kvp.Value.down;
				RecalculateColors(kvp.Value.up - kvp.Value.down);

				double priceUpper = kvp.Key + ticksPerBarLine * TickSize;
				double priceLower = kvp.Key;
				int yUpper = GetYPos(priceUpper, bounds, min, max) - barSpacing;
				int yLower = GetYPos(priceLower, bounds, min, max);				
				height = Math.Abs(yUpper - yLower);
				y = (int) yUpper + height / 2;
				int yText = (int) yUpper + height * 3/4;
				
				if (change != 0) {
					graphics.FillRectangle(barBrush, new Rectangle(
						bounds.X, y, 
						barWidth, height));
					
					//Print("new Rectangle(" + bounds.X + ", " + y + ", " + barWidth + ", " + height + ")" + ChartControl.Height); 
					// ref http://msdn.microsoft.com/en-us/library/system.string.format.aspx
					string text = String.Format("{0,-9:N2}{1,9:N0}", kvp.Key, (kvp.Value.up-kvp.Value.down));
					//Print(text);
					graphics.DrawString(text, stringFont, textBrush, bounds.X, yText);
				}
				delta += change;						
			}
			
			x = bounds.X + 10;
			y = 20;
			height = 20;
			//new Rectangle( x , y, width, height);
			graphics.FillRectangle(fillBrush, new Rectangle(x, y, barWidth-20, height));
			deltaBrush = delta > 0 ? new SolidBrush(Color.Blue) : new SolidBrush(Color.Red);
			// DrawString(string s, Font font, Brush brush, float x, float y);
			graphics.DrawString("change: " + delta, stringFont, deltaBrush, x, y);

			
		}
		
		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{/*
			if (barBrush != null) 
				barBrush.Dispose();
			if (textBrush != null)
				textBrush.Dispose();
			ChartControl.BarMarginLeft = 0;
		*/}

		
		
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

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot2
        {
            get { return Values[2]; }
        }

        [Description("Tick pre price range")]
        [GridCategory("Parameters")]
        public int TicksPerBarLine
        {
            get { return ticksPerBarLine; }
            set { ticksPerBarLine = Math.Max(1, value); }
        }

        [Description("Number of ticks before starting a new Vol Bar")]
        [GridCategory("Parameters")]
        public int TicksPerColumn
        {
            get { return ticksPerColumn; }
            set { ticksPerColumn = Math.Max(1, value); }
        }

        [Description("Bid/Ask delta color step")]
        [GridCategory("Parameters")]
        public int ColorThreshold
        {
            get { return colorThreshold; }
            set { colorThreshold = Math.Max(1, value); }
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
        private VolumeChart[] cacheVolumeChart = null;

        private static VolumeChart checkVolumeChart = new VolumeChart();

        /// <summary>
        /// Tracks the ticks bought or sold at bid or ask
        /// </summary>
        /// <returns></returns>
        public VolumeChart VolumeChart(int colorThreshold, int ticksPerBarLine, int ticksPerColumn)
        {
            return VolumeChart(Input, colorThreshold, ticksPerBarLine, ticksPerColumn);
        }

        /// <summary>
        /// Tracks the ticks bought or sold at bid or ask
        /// </summary>
        /// <returns></returns>
        public VolumeChart VolumeChart(Data.IDataSeries input, int colorThreshold, int ticksPerBarLine, int ticksPerColumn)
        {
            if (cacheVolumeChart != null)
                for (int idx = 0; idx < cacheVolumeChart.Length; idx++)
                    if (cacheVolumeChart[idx].ColorThreshold == colorThreshold && cacheVolumeChart[idx].TicksPerBarLine == ticksPerBarLine && cacheVolumeChart[idx].TicksPerColumn == ticksPerColumn && cacheVolumeChart[idx].EqualsInput(input))
                        return cacheVolumeChart[idx];

            lock (checkVolumeChart)
            {
                checkVolumeChart.ColorThreshold = colorThreshold;
                colorThreshold = checkVolumeChart.ColorThreshold;
                checkVolumeChart.TicksPerBarLine = ticksPerBarLine;
                ticksPerBarLine = checkVolumeChart.TicksPerBarLine;
                checkVolumeChart.TicksPerColumn = ticksPerColumn;
                ticksPerColumn = checkVolumeChart.TicksPerColumn;

                if (cacheVolumeChart != null)
                    for (int idx = 0; idx < cacheVolumeChart.Length; idx++)
                        if (cacheVolumeChart[idx].ColorThreshold == colorThreshold && cacheVolumeChart[idx].TicksPerBarLine == ticksPerBarLine && cacheVolumeChart[idx].TicksPerColumn == ticksPerColumn && cacheVolumeChart[idx].EqualsInput(input))
                            return cacheVolumeChart[idx];

                VolumeChart indicator = new VolumeChart();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ColorThreshold = colorThreshold;
                indicator.TicksPerBarLine = ticksPerBarLine;
                indicator.TicksPerColumn = ticksPerColumn;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeChart[] tmp = new VolumeChart[cacheVolumeChart == null ? 1 : cacheVolumeChart.Length + 1];
                if (cacheVolumeChart != null)
                    cacheVolumeChart.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeChart = tmp;
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
        /// Tracks the ticks bought or sold at bid or ask
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeChart VolumeChart(int colorThreshold, int ticksPerBarLine, int ticksPerColumn)
        {
            return _indicator.VolumeChart(Input, colorThreshold, ticksPerBarLine, ticksPerColumn);
        }

        /// <summary>
        /// Tracks the ticks bought or sold at bid or ask
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeChart VolumeChart(Data.IDataSeries input, int colorThreshold, int ticksPerBarLine, int ticksPerColumn)
        {
            return _indicator.VolumeChart(input, colorThreshold, ticksPerBarLine, ticksPerColumn);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Tracks the ticks bought or sold at bid or ask
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeChart VolumeChart(int colorThreshold, int ticksPerBarLine, int ticksPerColumn)
        {
            return _indicator.VolumeChart(Input, colorThreshold, ticksPerBarLine, ticksPerColumn);
        }

        /// <summary>
        /// Tracks the ticks bought or sold at bid or ask
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeChart VolumeChart(Data.IDataSeries input, int colorThreshold, int ticksPerBarLine, int ticksPerColumn)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeChart(input, colorThreshold, ticksPerBarLine, ticksPerColumn);
        }
    }
}
#endregion
