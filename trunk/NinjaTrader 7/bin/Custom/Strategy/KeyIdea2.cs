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
    /// Used to test new key trading ideas - this should be used only as a template.
	/// 
	/// Step 1: KeyIdea or trade idea 
	/// 		pseudo code entry
	/// 		example entry: if close > highest(close, x) then buy next bar at market
	/// 		exit: reverse position when entry reverses
	/// 		   or technical-based - support/resistance, moving average 
	///                 needs to work with entry to not give conflicting signals
	///            breakeven stops, stop-loss, profit targets, trailing stops 
	/// 		market selection
	/// 			oil, index etc
	/// 			over night multi-day 
	/// 			day trading
	/// 		time frame/bar size
	/// 			BetterRenko - 10 using 0.01 PT/SL
	/// 			Range
	/// 			Tick
	/// 			Time
	/// 		data considerations 
	/// 			not much can be done with ninja - continuous adjusted historical
	/// 	limited texting - check if there's any merit to the entry idea
	/// 		test against only 10-20% of data
	/// 			Entry Testing - pg 104
	/// 				usefullness test - without commission/slippage goal > 52% profitable
	/// 					Fixed-Stop and Target Exit
	/// 						SetProfitTarget
	/// 						SetStopLoss
	/// 					Fixed-Bar Exit
	/// 					Random Exit
	/// 
	/// Testing Range BO like Dynoweb uses
    /// </summary>
    [Description("Used to test new key trading ideas - idea, trend entry on CL using Range Bar")]
    public class KeyIdea2 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int iparm1 = 0; // Default setting for Iparm1
        private int iparm2 = 1; // Default setting for Iparm2
        private double dparm1 = 0.009; // Default setting for Dparm1
        private double dparm2 = 0.009; // Default setting for Dparm2
        private int period1 = 20; // Default setting for Period1
        private int period2 = 20; // Default setting for Period2
        // User defined variables (add any user defined variables below)
		
		private int channelMax = 40;
		private int channelMin = 5;
//		private bool enableSummary = true;
		private int extendPeriod = 0;	
		private int hour = 5; // CST
//		private int maxReversals = 1;		
		private int minute = 0;
		private int period = 60;
		private int sessionHighLow = 0;
//		private double stdDevMax = 0.09;
//		private bool useTrailing = false;

		// channel
		private double channelHigh = 0;
		private double channelLow = 0;
		private int adjustedChannelSize = 0;		

		private int tradeCount = 0;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			if (Iparm1 == 0)
			{
				SetProfitTarget("", CalculationMode.Percent, dparm1);
				SetStopLoss("", CalculationMode.Percent, dparm1, false);
				//SetTrailStop("", CalculationMode.Percent, dparm2, false);
			}

			//channelMax = channelMin + 5;
			
            CalculateOnBarClose = true;
			ExitOnClose = false;
			IncludeCommission = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {

			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				channelHigh = 0;
				channelLow = 0;
				tradeCount = 0;
				adjustedChannelSize = 0;
			}
			
			if (isFlat() && tradeCount == 0) 
			{
				if (isBear())
				{
					EnterLong();
					tradeCount++;
						SetProfitTarget("", CalculationMode.Ticks, adjustedChannelSize + (Close[0]-channelHigh) / TickSize);
						SetStopLoss("", CalculationMode.Ticks, adjustedChannelSize + (Close[0]-channelHigh) / TickSize, false);						
				}
				if (isBull())
				{
					EnterShort();
					tradeCount++;
						SetProfitTarget("", CalculationMode.Ticks, adjustedChannelSize + (channelLow - Close[0]) / TickSize);
						SetStopLoss("", CalculationMode.Ticks, adjustedChannelSize + (channelLow - Close[0]) / TickSize, false);						
				}
			} 
			else
			{
				if (Iparm1 > 0 && BarsSinceEntry() >= Iparm1)
				{
					if (isLong())
					{
						//ExitLong();
					}
					if (isShort())
					{
						//ExitShort();
					}
				}
			}

        }

		private bool isBear() 
		{
			bool bear = false;
			if (isChannelActive())
			{
				if (Close[0] > channelHigh)
					bear = true;
			}
			return bear;
		}

		private bool isBull() 
		{
			bool bull = false;
			if (isChannelActive())
			{
				if (Close[0] < channelLow)
					bull = true;
			}
			return bull;
		}

		private bool isChannelActive() 
		{
			// Run on the trigger bar to calculate daily range
			if (adjustedChannelSize == 0)
			{
				if (ToTime(Time[0]) == ToTime(hour, minute, 0)
					|| (extendPeriod == 1
						&& ToTime(Time[0]) >= ToTime(hour + (Time[0].DayOfWeek == DayOfWeek.Wednesday ? 0 : 0), minute, 0)
						&& ToTime(Time[0]) <= ToTime(hour + (Time[0].DayOfWeek == DayOfWeek.Wednesday ? 0 : 0), minute + BarsPeriod.Value * period, 0))
					)
				{
					adjustedChannelSize = calcAdjustedChannelSize();
					
					DrawDot(CurrentBar + "channelHigh", false, 0, channelHigh, Color.Blue);
					DrawDot(CurrentBar + "channelLow", false, 0, channelLow, Color.Cyan);
					DrawHorizontalLine("channelHigh", channelHigh, Color.Blue);
					DrawHorizontalLine("channelLow", channelLow, Color.Cyan);
				}
			}
			
			if (adjustedChannelSize != 0)
				return true;
			else
				return false;
		}
		
		private int calcAdjustedChannelSize()
		{
			int checkPeriod = Math.Min(period + (Time[0].DayOfWeek == DayOfWeek.Wednesday ? 0 : 0), Bars.BarsSinceSession);  // Bars.PercentComplete
			
			double sessionHigh = High[HighestBar(High, Bars.BarsSinceSession - 1)];
			double sessionLow = Low[LowestBar(Low, Bars.BarsSinceSession - 1)];
			
			channelHigh = High[HighestBar(High, checkPeriod + 1)];
			channelLow = Low[LowestBar(Low, checkPeriod + 1)];
			BackColor = Color.Coral;

			//	double stdDevHigh = StdDev(High, Bars.BarsSinceSession - 1)[0];
			//	double stdDevLow = StdDev(Low, Bars.BarsSinceSession - 1)[0];
			//	double timeSeriesForcast = TSF(6, 20)[0];
			//	DrawLine(CurrentBar + "SDHigh", 80, High[0] + stdDevHigh, 0, High[0] + stdDevHigh, Color.DarkGoldenrod);
			//	DrawLine(CurrentBar + "SDLow", 80, Low[0] - stdDevLow, 0, Low[0] - stdDevLow, Color.Aquamarine);
			
			double channelSize = (Instrument.MasterInstrument.Round2TickSize(channelHigh - channelLow))/TickSize;
			int adjustedChannelSize = (int) channelSize;
			
			if (channelSize < ChannelMin) 
			{
				adjustedChannelSize = 0;
			}
			else if (channelSize <= ChannelMax)
			{		
				adjustedChannelSize = (int) channelSize;
			}			
			else
			{
				adjustedChannelSize = 0;
			}
			//Print(Time + " adjustedChannelSize = "+adjustedChannelSize+" -- (channelHigh: "+channelHigh+" - channelLow: "+channelLow+")/TickSize: = " + (channelHigh - channelLow)/TickSize);
					
			if (adjustedChannelSize == 0 && sessionHighLow == 1)
			{				
				if ((sessionHigh - channelLow)/TickSize < ChannelMax && (channelHigh - sessionLow)/TickSize >= ChannelMin)
				{
					channelHigh = sessionHigh;
					adjustedChannelSize = Convert.ToInt32(Instrument.MasterInstrument.Round2TickSize(channelHigh - channelLow)/TickSize);
				}
				if ((channelHigh - sessionLow)/TickSize < ChannelMax && (channelHigh - sessionLow)/TickSize >= ChannelMin)
				{
					channelLow = sessionLow;			
					adjustedChannelSize = Convert.ToInt32(Instrument.MasterInstrument.Round2TickSize(channelHigh - channelLow)/TickSize);
				}				
			}
			
			if (adjustedChannelSize != 0)
			{
				Print(Time + " channelLow: " + channelLow + " channelHigh: " + channelHigh + " channelSize: " + channelSize);
				int endRectangle = 0;
				if (BarsPeriod.Id == PeriodType.Minute) {
					endRectangle = 240/BarsPeriod.Value;
				}			
				DrawText(CurrentBar + "adj", adjustedChannelSize.ToString(), 10, channelHigh + 4 * TickSize, Color.Black);
				DrawRectangle(CurrentBar + "rectangle", checkPeriod, channelLow, -endRectangle, channelHigh, Color.DarkBlue);
				
				DrawLine(CurrentBar + "SHigh", 60, sessionHigh, 0, sessionHigh, Color.Green);
				DrawLine(CurrentBar + "SLow", 60, sessionLow, 0, sessionLow, Color.Red);
			}
			return adjustedChannelSize;
		}
		
		
		
		private bool isFlat()
		{
			if (Position.MarketPosition == MarketPosition.Flat)
				return true;
			else
				return false;
		}
		
		private bool isLong()
		{
			if (Position.MarketPosition == MarketPosition.Long)
				return true;
			else
				return false;
		}
		
		private bool isShort()
		{
			if (Position.MarketPosition == MarketPosition.Short)
				return true;
			else
				return false;
		}
		
		
        [Description("Enables extending period to reach channel min 0 ==  false, 1 == true")]
        [GridCategory("Parameters")]
        public int ExtendPeriod
        {
            get { return extendPeriod; }
            set { extendPeriod = value; }
        }
		
        [Description("Minimum Channel Size in ticks")]
        [GridCategory("Parameters")]
        public int ChannelMin
        {
            get { return channelMin; }
            set { channelMin = Math.Max(1, value); }
        }
		
        [Description("Maximum Channel Size in ticks")]
        [GridCategory("Parameters")]
        public int ChannelMax
        {
            get { return channelMax; }
            set { channelMax = Math.Min(500, value); }
        }
		
		
        [Description("The number of bars to include before opening pit session to build the range")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
        [Description("Hour pit opens for this instrument")]
        [GridCategory("Parameters")]
        public int Hour
        {
            get { return hour; }
            set { hour = Math.Max(0, value); }
        }
		
        [Description("Minute pit opens for this instrument")]
        [GridCategory("Parameters")]
        public int Minute
        {
            get { return minute; }
            set { minute = Math.Max(0, value); }
        }
		
		
        [Description("Enables extending channel to reach session high or lows, 0 ==  false, 1 == true")]
        [GridCategory("Parameters")]
        public int SessionHighLow
        {
            get { return sessionHighLow; }
            set { sessionHighLow = value; }
        }
		
		
		

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm1
        {
            get { return iparm1; }
            set { iparm1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm2
        {
            get { return iparm2; }
            set { iparm2 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period1
        {
            get { return period1; }
            set { period1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period2
        {
            get { return period2; }
            set { period2 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm1
        {
            get { return dparm1; }
            set { dparm1 = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm2
        {
            get { return dparm2; }
            set { dparm2 = Math.Max(0.000, value); }
        }
        #endregion
    }
}
