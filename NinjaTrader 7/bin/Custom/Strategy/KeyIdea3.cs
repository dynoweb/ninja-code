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
	/// 	limited testing - check if there's any merit to the entry idea
	/// 		test against only 10-20% of data
	/// 			Entry Testing - pg 104
	/// 				usefulness test - without commission/slippage goal > 52% profitable
	/// 					Fixed-Stop and Target Exit
	/// 						SetProfitTarget
	/// 						SetStopLoss
	/// 					Fixed-Bar Exit
	/// 					Random Exit
	/// 
	/// Testing - uses 5 Range CL with SMA 9 bar high and lows, if close in scaple 5 ticks
	/// 	close if reverse signal found or 8 tick stop
    /// </summary>
    [Description("Used to test new key trading ideas - idea, Hi Low trend band entry on CL using 5 Range Bar")]
    public class KeyIdea3 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double dparm1 = 0.009; // Default setting for Dparm1
        private double dparm2 = 0.009; // Default setting for Dparm2
        private int iparm1 = 15; // Default setting for Iparm1
        private int iparm2 = 15; // Default setting for Iparm2
        private int period1 = 23; // Default setting for Period1
        private int period2 = 14; // Default setting for Period2
        // User defined variables (add any user defined variables below)
		
		private int tradeCount = 0;
		
		//SMA smaLow;
		//SMA smaHigh;
		
		HMA smaLow;
		HMA smaHigh;
		
		HallMaColored hallMa; 
		HallMaColored hallMaFast;
		DonchianChannel dc;
		
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//this.HallMaColored
			SetProfitTarget(CalculationMode.Ticks, iparm1);
			SetStopLoss(CalculationMode.Ticks, iparm1);
			//SetTrailStop("", CalculationMode.Percent, dparm2, false);
			
			smaLow = HMA(Low, period1);
			smaHigh = HMA(High, period1);
			//Add(smaLow);
			//Add(smaHigh);
			smaHigh.Displacement = 0;
			smaLow.Displacement = 0;
	
			hallMa = HallMaColored(Typical, period1);
			//hallMaFast = HallMaColored(Low, period2);
			Add(hallMa);
			//Add(hallMaFast);
			dc = DonchianChannel(period2);
			Add(dc);
			
            CalculateOnBarClose = true;
			ExitOnClose = true;
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
				tradeCount = 0;
			}
			
			if (isFlat() && tradeCount < 1000) 
			{
				if (isBull())
				{
					//if (iparm2 == 0)
						double target = Close[0];  //smaLow[0] - (smaLow[1] - smaLow[0]);
						//EnterLongLimit(hallMa[0]);
					EnterLong();
					//else
					//	EnterShort();
					
					tradeCount++;
//						SetProfitTarget("", CalculationMode.Ticks, adjustedChannelSize + (Close[0]-channelHigh) / TickSize);
//						SetStopLoss("", CalculationMode.Ticks, adjustedChannelSize + (Close[0]-channelHigh) / TickSize, false);						
				}
				if (isBear())
				{
					//if (iparm2 == 0)
					double target = smaHigh[0] + (smaHigh[1] - smaHigh[0]);
					//EnterShortLimit(target);
					//else
					//	EnterLong();

					tradeCount++;
//						SetProfitTarget("", CalculationMode.Ticks, adjustedChannelSize + (channelLow - Close[0]) / TickSize);
//						SetStopLoss("", CalculationMode.Ticks, adjustedChannelSize + (channelLow - Close[0]) / TickSize, false);						
				}
			} 
//			else
//				if (string s == "testingCloseBasedOnLowestOfPriorThreeBars")
//				{
//				}
			else 
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
			else
			{
				if (isLong() && Falling(hallMa))
					ExitLong();
			}

        }

		private bool isBull() 
		{
			bool bull = false;
			if (//hallMa[1] < hallMa[0] // rising
				//&& hallMa[0] < hallMaFast[0] // accelerated
				Close[0] > dc.Upper[1]
				&& dc.Upper[0] > dc.Upper[1]
				&& Rising(hallMa)
				//&& Close[0] == High[0]
				//&& Open[0] < Close[0]
				//&& High[1] < smaLow[1]
				)
			{
				bull = true;
			}
			return bull;
		}

		private bool isBear() 
		{
			bool bear = false;
			if (Close[0] > smaHigh[0] 
				&& Close[0] == Low[0]
				&& Open[1] > Close[0]
				//&& Low[1] > smaHigh[1]
				)
			{
				bear = true;
			}
			return bear;
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
