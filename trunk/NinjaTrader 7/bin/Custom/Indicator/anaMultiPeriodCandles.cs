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

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// This indicator colors bars. Reversal bars are also identified.
    /// </summary>
    // Version 1.1, May 6, 2011: Bug fixed, period of first candle after session start was short by one bar on minute charts
	[Description("The indicator regroups single bars to diplsay Multi Period Candles")]
    [Gui.Design.DisplayName("anaMultiPeriodCandles")]
	public class anaMultiPeriodCandles : Indicator
    {
		#region Variables
		private int 		period				= 12;
		private int			count				= 0;
		private int			tag					= 0;
		private int 		width				= 2;
		private int			opacity				= 5;
		private double 		open				= 0.0;
		private double 		high				= 0.0;
		private double 		low					= 0.0;
		private double 		close				= 0.0;
		private Color		upColor				= Color.Green;	
		private Color		downColor			= Color.DarkRed;	
		private Color		candleColor			= Color.Empty; 
		private Color		outlineColor 		= Color.LightSlateGray;
		private Color		dojiColor			= Color.Transparent;
		private DateTime	sessionStart		= Cbi.Globals.MinDate;
		private DateTime	sessionEnd			= Cbi.Globals.MinDate;
		private DateTime 	candleStartTime		= Cbi.Globals.MinDate;
		private DateTime 	nextStartTime		= Cbi.Globals.MinDate;
		private bool		fixedPeriod			= true;
		private bool		fixedPeriodMinutes	= true;
		private bool		useCandleTime		= true;
		private Font		textFont			= new Font("Arial", 12);
		private string		errorData			= "Multi-Period Candles can only be displayed on intraday charts.";
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= false;
			PaintPriceMarkers	= false;
			ZOrder				= -1;
       	}

		protected override void OnStartUp()
		{
			if (Bars.Period.Id == PeriodType.Minute || Bars.Period.Id == PeriodType.Second)
			{
				fixedPeriod = true;
				if (Bars.Period.Id == PeriodType.Minute)
					fixedPeriodMinutes = true;
				else
					fixedPeriodMinutes = false;
			}
			else
				fixedPeriod = false;
		}
		
		/// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (!Data.BarsType.GetInstance(Bars.Period.Id).IsIntraday)
			{
				DrawTextFixed("errortag", errorData, TextPosition.Center, ChartControl.AxisColor, textFont, Color.Transparent,Color.Transparent,0);
				return;
			}
			
			if (FirstTickOfBar)
			{
				if (fixedPeriod)
				{
					if (Bars.FirstBarOfSession)
					{
						count = 0;
						Bars.Session.GetNextBeginEnd(Time[0], out sessionStart, out sessionEnd);	
						if (fixedPeriodMinutes)
							nextStartTime = sessionStart.AddMinutes(Period * Bars.Period.Value);
						else 
							nextStartTime = sessionStart.AddSeconds(Period * Bars.Period.Value);
					}
					else
					{
						count = count +1;
						if (Time[0]> nextStartTime)
						{
							do
							{
								candleStartTime = nextStartTime;
								if (fixedPeriodMinutes)
									nextStartTime = candleStartTime.AddMinutes(Period * Bars.Period.Value);
								else 
									nextStartTime = candleStartTime.AddSeconds(Period * Bars.Period.Value);
							}
							while (nextStartTime <= Time[0]);
							count = 0;
						}
					}
				}
				else
				{
					if (Bars.FirstBarOfSession)
						count = 0;
					else 
						count = count +1;
				}
			}
			
			int lookback = count % period + 1;
			if (lookback == 1)
				tag = CurrentBar;
			open = Open[lookback - 1];
			close = Close[0];
			high = MAX(High,lookback)[0];
			low = MIN(Low,lookback)[0];
			
			int candleIndex = 0;
			DateTime candleTime = Cbi.Globals.MinDate;
			TimeSpan candlePeriod = new TimeSpan(0,0,0);
			if(fixedPeriod)
			{
				candlePeriod 	= Time[0].Subtract(Time[lookback-1]);
				candlePeriod 	= new TimeSpan(candlePeriod.Ticks/2);
				candleTime 		= Time[lookback-1].Add(candlePeriod);
				useCandleTime 	= true;
			}
			else if (lookback % 2 == 1)
			{
				candleIndex = (lookback - 1)/2;
				useCandleTime = false;
			}
			else
			{
				candleIndex = lookback/2 - 1;
				candlePeriod = Time[candleIndex].Subtract(Time[candleIndex+1]);
				candlePeriod = new TimeSpan(candlePeriod.Ticks/2);
				candleTime = Time[candleIndex+1].Add(candlePeriod);
				if (candlePeriod > new TimeSpan(0,0,0))
					useCandleTime = true;
				else
					useCandleTime = false;
			}
				
			double upper = Math.Max(open,close);
			double lower = Math.Min(open,close);
			if (open>close)
			{	
				candleColor = downColor;
				dojiColor = Color.Transparent;
			}
			else if (open<close)
			{	
				candleColor = upColor;
				dojiColor = Color.Transparent;
			}
			else
				dojiColor = outlineColor;
				
			DrawLine("doji" + tag, true, lookback-1, close, 0, close, dojiColor, DashStyle.Solid, Width+1);
			DrawRectangle("box" + tag, true, lookback-1, open, 0 , close, outlineColor, candleColor, opacity);
			if (useCandleTime)
			{
				DrawLine("upperwig" + tag, true, candleTime, high, candleTime, upper, outlineColor, DashStyle.Solid, Width);
				DrawLine("lowerwig" + tag, true, candleTime, lower, candleTime, low, outlineColor, DashStyle.Solid, Width);
			}	
			else
			{
				DrawLine("upperwig" + tag, true, candleIndex, high, candleIndex, upper, outlineColor, DashStyle.Solid, Width);
				DrawLine("lowerwig" + tag, true, candleIndex, lower, candleIndex, low, outlineColor, DashStyle.Solid, Width);
			}
		}
			
        #region Properties
		
 		/// <summary>
		/// </summary>
		[Description("Candle Period")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Bar Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("UpColor")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
			set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Select Bar Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("DownColor")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColor); }
			set { downColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Select Candle Outline Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Candle Outline")]
		public Color OutlineColor
		{
			get { return outlineColor; }
			set { outlineColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string OutlineColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(outlineColor); }
			set { outlineColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
 		
		/// <summary>
		/// </summary>
		[Description("Opacity of Candles ")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Candle Opacity")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = Math.Min(10, Math.Max(0, value)); }
		}

 		/// <summary>
		/// </summary>
		[Description("Width for Candle Wigs ")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Width of Wigs")]
		public int Width
		{
			get { return width; }
			set { width = Math.Min(10, Math.Max(1, value)); }
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
        private anaMultiPeriodCandles[] cacheanaMultiPeriodCandles = null;

        private static anaMultiPeriodCandles checkanaMultiPeriodCandles = new anaMultiPeriodCandles();

        /// <summary>
        /// The indicator regroups single bars to diplsay Multi Period Candles
        /// </summary>
        /// <returns></returns>
        public anaMultiPeriodCandles anaMultiPeriodCandles(int opacity, int period, int width)
        {
            return anaMultiPeriodCandles(Input, opacity, period, width);
        }

        /// <summary>
        /// The indicator regroups single bars to diplsay Multi Period Candles
        /// </summary>
        /// <returns></returns>
        public anaMultiPeriodCandles anaMultiPeriodCandles(Data.IDataSeries input, int opacity, int period, int width)
        {
            if (cacheanaMultiPeriodCandles != null)
                for (int idx = 0; idx < cacheanaMultiPeriodCandles.Length; idx++)
                    if (cacheanaMultiPeriodCandles[idx].Opacity == opacity && cacheanaMultiPeriodCandles[idx].Period == period && cacheanaMultiPeriodCandles[idx].Width == width && cacheanaMultiPeriodCandles[idx].EqualsInput(input))
                        return cacheanaMultiPeriodCandles[idx];

            lock (checkanaMultiPeriodCandles)
            {
                checkanaMultiPeriodCandles.Opacity = opacity;
                opacity = checkanaMultiPeriodCandles.Opacity;
                checkanaMultiPeriodCandles.Period = period;
                period = checkanaMultiPeriodCandles.Period;
                checkanaMultiPeriodCandles.Width = width;
                width = checkanaMultiPeriodCandles.Width;

                if (cacheanaMultiPeriodCandles != null)
                    for (int idx = 0; idx < cacheanaMultiPeriodCandles.Length; idx++)
                        if (cacheanaMultiPeriodCandles[idx].Opacity == opacity && cacheanaMultiPeriodCandles[idx].Period == period && cacheanaMultiPeriodCandles[idx].Width == width && cacheanaMultiPeriodCandles[idx].EqualsInput(input))
                            return cacheanaMultiPeriodCandles[idx];

                anaMultiPeriodCandles indicator = new anaMultiPeriodCandles();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Opacity = opacity;
                indicator.Period = period;
                indicator.Width = width;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaMultiPeriodCandles[] tmp = new anaMultiPeriodCandles[cacheanaMultiPeriodCandles == null ? 1 : cacheanaMultiPeriodCandles.Length + 1];
                if (cacheanaMultiPeriodCandles != null)
                    cacheanaMultiPeriodCandles.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaMultiPeriodCandles = tmp;
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
        /// The indicator regroups single bars to diplsay Multi Period Candles
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMultiPeriodCandles anaMultiPeriodCandles(int opacity, int period, int width)
        {
            return _indicator.anaMultiPeriodCandles(Input, opacity, period, width);
        }

        /// <summary>
        /// The indicator regroups single bars to diplsay Multi Period Candles
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMultiPeriodCandles anaMultiPeriodCandles(Data.IDataSeries input, int opacity, int period, int width)
        {
            return _indicator.anaMultiPeriodCandles(input, opacity, period, width);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The indicator regroups single bars to diplsay Multi Period Candles
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMultiPeriodCandles anaMultiPeriodCandles(int opacity, int period, int width)
        {
            return _indicator.anaMultiPeriodCandles(Input, opacity, period, width);
        }

        /// <summary>
        /// The indicator regroups single bars to diplsay Multi Period Candles
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMultiPeriodCandles anaMultiPeriodCandles(Data.IDataSeries input, int opacity, int period, int width)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaMultiPeriodCandles(input, opacity, period, width);
        }
    }
}
#endregion
