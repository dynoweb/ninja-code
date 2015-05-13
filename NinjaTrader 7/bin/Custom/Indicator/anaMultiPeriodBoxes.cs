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
    /// TThe indicator regroups single bars to display them as Multi Period Boxes.
    /// </summary>
    // Version 1.1, May 6, 2011: Bug fixed, period of first candle after session start was short by one bar on minute charts
	[Description("The indicator regroups single bars to display them as Multi Period Boxes")]
    [Gui.Design.DisplayName("anaMultiPeriodBoxes")]
	public class anaMultiPeriodBoxes : Indicator
    {
		#region Variables
		private int 		period				= 12;
		private int			count				= 0;
		private int			tag					= 0;
		private int			opacity				= 5;
		private int			lookback			= 0;
		private double 		open				= 0.0;
		private double 		high				= 0.0;
		private double 		low					= 0.0;
		private double 		close				= 0.0;
		private Color		upColor				= Color.Green;	
		private Color		downColor			= Color.DarkRed;	
		private Color		candleColor			= Color.Empty; 
		private Color		outlineColor 		= Color.LightSlateGray;
		private DateTime	sessionStart		= Cbi.Globals.MinDate;
		private DateTime	sessionEnd			= Cbi.Globals.MinDate;
		private DateTime 	candleStartTime		= Cbi.Globals.MinDate;
		private DateTime 	nextStartTime		= Cbi.Globals.MinDate;
		private bool		fixedPeriod			= true;
		private bool		fixedPeriodMinutes	= true;
		private Font		textFont			= new Font("Arial", 12);
		private string		errorData			= "Multi-Period Boxes can only be displayed on intraday charts.";
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
			DrawOnPricePanel	= false;
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
				lookback = count % period + 1;
				if (lookback == 1)
				{
					tag = CurrentBar;
					open = Open[0];
				}
				high = MAX(High,lookback)[0];
				low = MIN(Low,lookback)[0];
			}
			close = Close[0];
			high = Math.Max(close, high);
			low = Math.Min(close, low);
			
			if (open > close)
				candleColor = downColor;
			else if (open <= close)
				candleColor = upColor;
				
			DrawRectangle("box" + tag, true, lookback-1, low, 0 , high, outlineColor, candleColor, opacity);
		}
			
        #region Properties
		
 		/// <summary>
		/// </summary>
		[Description("Period for Composite Candles, please select the period > 2")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(2, value); }
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

     	#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private anaMultiPeriodBoxes[] cacheanaMultiPeriodBoxes = null;

        private static anaMultiPeriodBoxes checkanaMultiPeriodBoxes = new anaMultiPeriodBoxes();

        /// <summary>
        /// The indicator regroups single bars to display them as Multi Period Boxes
        /// </summary>
        /// <returns></returns>
        public anaMultiPeriodBoxes anaMultiPeriodBoxes(int opacity, int period)
        {
            return anaMultiPeriodBoxes(Input, opacity, period);
        }

        /// <summary>
        /// The indicator regroups single bars to display them as Multi Period Boxes
        /// </summary>
        /// <returns></returns>
        public anaMultiPeriodBoxes anaMultiPeriodBoxes(Data.IDataSeries input, int opacity, int period)
        {
            if (cacheanaMultiPeriodBoxes != null)
                for (int idx = 0; idx < cacheanaMultiPeriodBoxes.Length; idx++)
                    if (cacheanaMultiPeriodBoxes[idx].Opacity == opacity && cacheanaMultiPeriodBoxes[idx].Period == period && cacheanaMultiPeriodBoxes[idx].EqualsInput(input))
                        return cacheanaMultiPeriodBoxes[idx];

            lock (checkanaMultiPeriodBoxes)
            {
                checkanaMultiPeriodBoxes.Opacity = opacity;
                opacity = checkanaMultiPeriodBoxes.Opacity;
                checkanaMultiPeriodBoxes.Period = period;
                period = checkanaMultiPeriodBoxes.Period;

                if (cacheanaMultiPeriodBoxes != null)
                    for (int idx = 0; idx < cacheanaMultiPeriodBoxes.Length; idx++)
                        if (cacheanaMultiPeriodBoxes[idx].Opacity == opacity && cacheanaMultiPeriodBoxes[idx].Period == period && cacheanaMultiPeriodBoxes[idx].EqualsInput(input))
                            return cacheanaMultiPeriodBoxes[idx];

                anaMultiPeriodBoxes indicator = new anaMultiPeriodBoxes();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Opacity = opacity;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaMultiPeriodBoxes[] tmp = new anaMultiPeriodBoxes[cacheanaMultiPeriodBoxes == null ? 1 : cacheanaMultiPeriodBoxes.Length + 1];
                if (cacheanaMultiPeriodBoxes != null)
                    cacheanaMultiPeriodBoxes.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaMultiPeriodBoxes = tmp;
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
        /// The indicator regroups single bars to display them as Multi Period Boxes
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMultiPeriodBoxes anaMultiPeriodBoxes(int opacity, int period)
        {
            return _indicator.anaMultiPeriodBoxes(Input, opacity, period);
        }

        /// <summary>
        /// The indicator regroups single bars to display them as Multi Period Boxes
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMultiPeriodBoxes anaMultiPeriodBoxes(Data.IDataSeries input, int opacity, int period)
        {
            return _indicator.anaMultiPeriodBoxes(input, opacity, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The indicator regroups single bars to display them as Multi Period Boxes
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMultiPeriodBoxes anaMultiPeriodBoxes(int opacity, int period)
        {
            return _indicator.anaMultiPeriodBoxes(Input, opacity, period);
        }

        /// <summary>
        /// The indicator regroups single bars to display them as Multi Period Boxes
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMultiPeriodBoxes anaMultiPeriodBoxes(Data.IDataSeries input, int opacity, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaMultiPeriodBoxes(input, opacity, period);
        }
    }
}
#endregion
