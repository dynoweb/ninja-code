#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
	/// ES - 5 min
    /// Opening gap trading based on 
	/// https://www.investiquant.com/product/iq-gap-trader 
	/// https://www.investiquant.com/news/category/daily-iq
	/// 
	/// Should work for any instrument but tested using ES
	/// 		
	/// Zones are based on location of open relative to closing bar
	///		For an up bar
	///		1 - above the high
	///		2 - above the close
	///		3 - between the open and close
	///		4 - below the open and low
	///		5 - below the low
	///
	/// Gap Sizes
	/// Large Up Gaps - Prior Close + 40% of 5 day ATR
	/// Small Up Gaps - less than Large up gaps
	/// Small Down Gaps - less than large down gaps
	/// Large Down Gaps - Prior Close - 40% of 5 day ATR
	/// 
	/// 
    /// </summary>
    [Description("Opening gap trading based on https://www.investiquant.com/product/iq-gap-trader product. Should work for any instrument but tested using ES")]
    public class IQGapTrader : Strategy
    {
        #region Variables
			// Parametric variables
			private int customFilters = 0; // Default setting for CustomFilters
			private double gapSizeMax = 4.0; // Default setting for GapSizeMax
			private double gapSizeMin = 0.75; // Default setting for GapSizeMin
			private int marketCondition = 1; // Default setting for MarketCondition
			private int openingZone = 1; // Default setting for OpeningZone
			private double stopPctATR = 0.0;
			private double stopPctGapSize = 0.0;
			private int stopsPoint = 14;
			private int stopsTime = 0; // Default setting for TimeStops
			private double targetPctATR = 1; // Default setting for TargetPctATR
			private double targetPctGapSize = 1; // Default setting for TargetPctGapSize
	        // Other variables
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            SetProfitTarget("", CalculationMode.Ticks, 0);
			if (StopsPoint != 0) {
            	SetStopLoss("", CalculationMode.Ticks, StopsPoint, false);
			}

			Add(PitColor(Color.Blue, 83000, 15, 160000));
			
//			Unmanaged = false;
//			BarsRequired = 10;
            CalculateOnBarClose = true;
//			ExitOnClose = false;
//			IncludeCommission = true;        
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			
			if (Bars.FirstBarOfSession) {
				int lineSize = 0;
				if (BarsPeriod.Id == PeriodType.Minute) {					
					lineSize = 20 * 60/BarsPeriod.Value;	// 20 hours
				}
				
				// This open is based on the session open not the open pit open
				DrawLine(CurrentBar + "O", 0, PriorDayOHLC().PriorOpen[0], -lineSize, PriorDayOHLC().PriorOpen[0], Color.Green); 
				DrawLine(CurrentBar + "H", 0, PriorDayOHLC().PriorHigh[0], -lineSize, PriorDayOHLC().PriorHigh[0], Color.Blue); 
				DrawLine(CurrentBar + "L", 0, PriorDayOHLC().PriorLow[0], -lineSize, PriorDayOHLC().PriorLow[0], Color.Blue); 
				DrawLine(CurrentBar + "C", 0, PriorDayOHLC().PriorClose[0], -lineSize, PriorDayOHLC().PriorClose[0], Color.Red); 
			}
			
			// If it's Friday, do not trade.
		    //if (Time[0].DayOfWeek == DayOfWeek.Tuesday)
        	//	return;
			
			
			int hour = 8;
			int minute = 30;
			
			if (ToTime(Time[0]) == ToTime(hour, minute, 0)) {

				bool all = OpeningZone == 0 ? true : false;
				bool openingZone1 = OpeningZone == 1 || all ? true : false;
				bool openingZone2 = OpeningZone == 2 || all ? true : false;
						
				// Up bar
				if (PriorDayOHLC().PriorClose[0] > PriorDayOHLC().PriorOpen[0]) {
					double gapSize = Close[0] - PriorDayOHLC().PriorClose[0];
					// max/min gap check
					if (gapSize >= GapSizeMin && gapSize <= GapSizeMax) {

						// Zone 1 open above prior days high
						if (openingZone1 && Close[0] >= PriorDayOHLC().PriorHigh[0]) {
							EnterShortLimit(Close[0], "Zone 1");
							SetProfitTarget(CalculationMode.Price, PriorDayOHLC().PriorClose[0]);
						}
						// Zone 2 between the high and close
						if (openingZone2 && Close[0] > PriorDayOHLC().PriorClose[0] && Close[0] <= PriorDayOHLC().PriorHigh[0]) {
							EnterShortLimit(Close[0], "Zone 2");
							SetProfitTarget(CalculationMode.Price, PriorDayOHLC().PriorClose[0]);
						}
					}
				}
				
				// Down bar
				if (PriorDayOHLC().PriorClose[0] < PriorDayOHLC().PriorOpen[0]) {
					double gapSize = PriorDayOHLC().PriorClose[0] - Close[0];
					// max/min gap check
					if (gapSize >= GapSizeMin && gapSize <= GapSizeMax) {

						SetProfitTarget(CalculationMode.Price, PriorDayOHLC().PriorClose[0]);
						
						// Zone 1 open below prior days low
						if (openingZone1 && Close[0] <= PriorDayOHLC().PriorLow[0]) {
							EnterLongLimit(Close[0], "Zone 1");
						}
						// Zone 2 between the close and low
						if (openingZone2 && Close[0] < PriorDayOHLC().PriorClose[0] && Close[0] >= PriorDayOHLC().PriorLow[0]) {
							EnterLongLimit(Close[0], "Zone 2");
						}
					}
				}
			}
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public double GapSizeMin
        {
            get { return gapSizeMin; }
            set { gapSizeMin = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double GapSizeMax
        {
            get { return gapSizeMax; }
            set { gapSizeMax = Math.Max(0.500, value); }
        }

        [Description("0 - means ignore, 1 - means enable (day or week filter)")]
        [GridCategory("Parameters")]
        public int MarketCondition
        {
            get { return marketCondition; }
            set { marketCondition = Math.Max(0, value); }
        }

		/*
		Zones are based on location of open relative to closing bar
			For an up bar
			1 - above the high
			2 - above the close
			3 - between the open and close
			4 - below the open and low
			5 - below the low
		*/
        [Description("Zones [1..5] - 0 implies all zones")]
        [GridCategory("Parameters")]
        public int OpeningZone
        {
            get { return openingZone; }
            set { openingZone = Math.Max(0, value); }
        }

        [Description("Binary number to enable trade filters")]
        [GridCategory("Parameters")]
        public int CustomFilters
        {
            get { return customFilters; }
            set { customFilters = Math.Max(0, value); }
        }

        [Description("% of Average True Range (ATR) -useful for setting volatility-adjusted stops. The more volatile the market, the bigger your stop and vice versa. (note: # of days for calculating ATR can be specified)")]
        [GridCategory("Parameters")]
        public double TargetPctATR
        {
            get { return targetPctATR; }
            set { targetPctATR = Math.Max(0.000, value); }
        }

        [Description("great for testing and setting stops for large gap. Does today’s large gap need a large stop or it is more likely to reverse and fill quickly? Stop guessing. Test and know.")]
        [GridCategory("Parameters")]
        public double TargetPctGapSize
        {
            get { return targetPctGapSize; }
            set { targetPctGapSize = Math.Max(0.000, value); }
        }

        [Description("% of Average True Range (ATR) - 0 value is ignored")]
        [GridCategory("Parameters")]
        public double StopPctATR
        {
            get { return stopPctATR; }
            set { stopPctATR = Math.Max(0.000, value); }
        }

        [Description("Stop based on percent of Gap size - 0 value is ignored")]
        [GridCategory("Parameters")]
        public double StopPctGapSize
        {
            get { return stopPctGapSize; }
            set { stopPctGapSize = Math.Max(0.000, value); }
        }

        [Description("Stop x bars after open - 0 value is ignored")]
        [GridCategory("Parameters")]
        public int StopsTime
        {
            get { return stopsTime; }
            set { stopsTime = Math.Max(0, value); }
        }
		
        [Description("Stop based on points - 0 value is ignored")]
        [GridCategory("Parameters")]
        public int StopsPoint
        {
            get { return stopsPoint; }
            set { stopsPoint = Math.Max(0, value); }
        }
        #endregion
    }
}
