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
using PriceActionSwingOscillator.Utility;
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
	/// 			range
	/// 			tick
	/// 			time
	/// 		data considerations 
	/// 			not much can be done with ninja - continuous adjusted historical
	/// 	limited texting - check if there's any merit to the entry idea
	/// 		test against only 10-20% of data
	/// 			Entry Testing
	/// 				usefullness test - without commission/slippage goal > 52% profitable
	/// 					Fixed-Stop and Target Exit
	/// 						SetProfitTarget
	/// 						SetStopLoss
	/// 					Fixed-Bar Exit
	/// 					Random Exit
	/// 	
    /// </summary>
    [Description("CL 3 min - Use 15 min bars - Created to backtest IgalSudman's ATR strategies")]
    public class KeyIdea16 : Strategy
    {
        #region Variables
		
		double atrFactorChannelIn = 1.5;
		double atrFactorChannelOut = 1.5;
		int channelStartPeriodSize = 30;
		
        int contracts = 1; 
		double maxAtr = 0.25;
		double breakEvenPercentOfTarget = 0.50;
		int breakEvenPlus = 5;
		double stopLossPercent = 1.0;
		double targetProfitPercent = 1.0;
		int tradeStartTime = 800;	// 8 AM Central
		int tradeEndTime = 1500;	// 12:00 PM
		
		double channelHigh = 0;
		double channelLow = 0;
		double channelSize = 0;
		
		double upperTrigger = 0;
		double lowerTrigger = 0;
		double upperStopLoss = 0;
		double lowerStopLoss = 0;
		double upperTarget = 0;
		double lowerTarget = 0;
		double atr = 0;		
		
		bool isTraded = false;

		string orderPrefix = "KI16"; 
		
		
		
		// ATRTrailing parms
//		ATRTrailing atr = null;
//		double atrTimes = 3.5; 
//		int atrPeriod = 10; 
		
		//  KeltnerChannel parms
		KeltnerChannel kc = null;
		double offsetMultiplier = 1.5;
		int keltnerPeriod = 10; 
		
		// DonchianChannel parms
		DonchianChannel dc = null;
		int donchianPeriod = 20;
				
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
        /// </summary>
        protected override void Initialize()
        {
			//int dtbStrength = Iparm1;
			//int swingSize = Iparm2;
			//atrTimes = dparm1;
			//atrPeriod = period1;
			
			//atr = ATRTrailing(atrTimes, Period1, Ratched);
			//Add(atr);
			
			Add(anaMultiPeriodBoxes(3, 1));
			
			Add(this.PitColor(Color.Black, 80000, 15, 150000));
			
            //SetProfitTarget("", CalculationMode.Ticks, dparm1);
            //SetStopLoss("", CalculationMode.Ticks, dparm2, false);
            //SetTrailStop("", CalculationMode.Percent, dparm2, false);

			BarsRequired = 2;
            CalculateOnBarClose = true;
			ExitOnClose = true;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
//			// If it's Monday, do not trade.
//    		//if (Time[0].DayOfWeek == DayOfWeek.Thursday)
//
//			if (ToTime(Time[0]) < 90000
//				|| ToTime(Time[0]) > 150000)
//			{
//				if (isShort())
//				{
//					ExitShort();
//				}
//				if (isLong())
//				{
//					ExitLong();
//				}
//				return;
//			}
//
//				//SetStopLoss(CalculationMode.Ticks, 1000);
			
			
			//Test concept
			//For each bar that breaks out, record how far it breaks out and how far it retraces
			
			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				channelHigh = 0;
				channelLow = 0;
				channelSize = 0;
				upperTrigger = 0;
				lowerTrigger = 0;
				ResetTrades();
			}
			
			// Open pit session trading only
			if (ToTime(Time[0]) < (tradeStartTime * 100) || ToTime(Time[0]) > (tradeEndTime*100))
			{
				CloseWorkingOrders();
				return;
			}

			Print(Time + "," + Open[0] + "," + High[0] + "," + Low[0] + "," + Close[0] + "," + ATR(5)[0]);
			
			/*
			// Channel measurement happens 30 min after the start time
			if (channelSize == 0 && ToTime(Time[0]) == (tradeStartTime + channelStartPeriodSize + BarsPeriod.Value) * 100)
			{
				EstablishOpeningChannel();
			}

			if (channelSize != 0)
			{
				// % 15 is the higher timeframe, the == 1 is the number of min per bar
				if (Time[0].Minute % 15 == 1 && isFlat())
				{
					isTraded = false;
					atr = CalcAtr();
					Print(Time + " Order entry or adjustment - atr: " + atr + " maxAtr: " +  maxAtr);
					//DrawText(CurrentBar + "atr", atr.ToString(), 2, MAX(High, 5)[1] + 0.3 * atr, Color.Black);
					if (atr <= maxAtr)
					{
						upperTrigger = MAX(High, 15)[1] + Instrument.MasterInstrument.Round2TickSize(atr) * atrFactorChannelIn;
						lowerTrigger = MIN(Low, 15)[1] - Instrument.MasterInstrument.Round2TickSize(atr) * atrFactorChannelIn;
						
						if (upperTrigger < channelLow || upperTrigger > channelHigh)
						{
							upperTrigger = MAX(High, 15)[1] + Instrument.MasterInstrument.Round2TickSize(atr * atrFactorChannelOut);
						}
						if (lowerTrigger < channelLow || lowerTrigger > channelHigh)
						{
							lowerTrigger = MIN(Low, 15)[1] - Instrument.MasterInstrument.Round2TickSize(atr * atrFactorChannelOut);
						}
						
						DrawLine(CurrentBar + "upperTrigger", 0, upperTrigger, -14, upperTrigger, Color.Blue);
						DrawLine(CurrentBar + "lowerTrigger", 0, lowerTrigger, -14, lowerTrigger, Color.Blue);
						
						upperStopLoss = upperTrigger + Instrument.MasterInstrument.Round2TickSize(atr * StopLossPercent);
						lowerStopLoss = lowerTrigger - Instrument.MasterInstrument.Round2TickSize(atr * StopLossPercent);
						
						DrawLine(CurrentBar + "upperStopLoss", 0, upperStopLoss, -14, upperStopLoss, Color.Red);
						DrawLine(CurrentBar + "lowerStopLoss", 0, lowerStopLoss, -14, lowerStopLoss, Color.Red);

						lowerTarget = lowerTrigger + Instrument.MasterInstrument.Round2TickSize(atr * TargetProfitPercent);
						upperTarget = upperTrigger - Instrument.MasterInstrument.Round2TickSize(atr * TargetProfitPercent);
						
						DrawLine(CurrentBar + "shortTarget", 0, upperTarget, -14, upperTarget, Color.Green);
						DrawLine(CurrentBar + "longTarget", 0, lowerTarget, -14, lowerTarget, Color.Green);				
					}
				}

				if (isFlat() && !isTraded)
				{
					if (lowerTrigger != 0)
					{
						//limitPrice = lowerTrigger;
						//stopPrice = limitPrice;
						EnterLongLimit(lowerTrigger);
					}
					
					if (upperTrigger != 0)
					{
						//limitPrice = upperTrigger;
						//stopPrice = limitPrice;
						//target = upperTarget;

						//EnterLongStop(upperTrigger);
						//EnterShortStop(upperTrigger);
						//EnterShortLimit(upperTrigger);
						//ExitShortStop(upperStopLoss);
					}
				} 
				else if (isLong())
				{
					isTraded = true;
					ExitLongLimit(lowerTarget);
					if (Close[0] < lowerStopLoss)
						ExitLong();
					else
						ExitLongStop(lowerStopLoss);
					DrawDot(CurrentBar + "lowerTarget", false, 0, lowerTarget, Color.LightGreen);
					DrawDot(CurrentBar + "lowerStopLoss", false, 0, lowerStopLoss, Color.LightPink);
					if (BarsSinceEntry() > BreakEvenPlus)
						ExitLong();
				}
				else if (isShort())
				{
					isTraded = true;
					ExitShortLimit(upperTarget);
//					//SetStopLoss(CalculationMode.Ticks, atr * StopLossPercent/TickSize);
//					ExitShortStop(upperStopLoss);
					DrawDot(CurrentBar + "upperTarget", false, 0, upperTarget, Color.LightGreen);
					DrawDot(CurrentBar + "lowerTarget", false, 0, upperStopLoss, Color.LightPink);
					if (BarsSinceEntry() > BreakEvenPlus)
						ExitShort();
				}
			}
			*/
        }
		
		protected override void OnTermination()
		{
		}

		
		protected override void OnOrderUpdate(IOrder order)
		{
//			if (entryOrder != null && entryOrder == order)
//			{
//				Print(order.ToString());
//				if (order.OrderState == OrderState.Filled)
//					entryOrder = null;
//			}
		}
		
		private void CloseWorkingOrders()
		{
			//Print(Time + " Closing Working Orders");
//			CloseBuyOrder();
//			CloseSellOrder();
		}	
		
		private void CloseBuyOrder()
		{
//			if (buyOrder1 != null && (buyOrder1.OrderState == OrderState.Working || buyOrder1.OrderState == OrderState.Accepted))
//			{
//				CancelOrder(buyOrder1);
//			}
		}
		
		private void CloseSellOrder()
		{
//			if (sellOrder1 != null && (sellOrder1.OrderState == OrderState.Working || sellOrder1.OrderState == OrderState.Accepted))
//			{
//				CancelOrder(sellOrder1);
//			}					
		}	
		
		private void ResetTrades()
		{
			//Print(Time + " ----------- RESET ----------");
			lowerTrigger = 0;
			upperTrigger = 0;
		}
			
		private void EstablishOpeningChannel()
		{
			int barsToEstablishChannel = channelStartPeriodSize/BarsPeriod.Value;
			channelHigh = MAX(High, barsToEstablishChannel)[1];
			channelLow = MIN(Low, barsToEstablishChannel)[1];
			channelSize = channelHigh - channelLow;
			int endHour = tradeEndTime/100;
			int endMin = tradeEndTime - endHour * 100;
			int barsToTrade = (endHour * 60 + endMin - (Time[0].Hour * 60 + Time[0].Minute)) / BarsPeriod.Value; 
			//Print(Time +  " barsToTrade: " + barsToTrade + " tradeEndTime: " + tradeEndTime +  " Time[0]: " + Time[0] + " endHour: " + endHour + " endMin: " + endMin);
			DrawRectangle(CurrentBar + "channel", false, barsToEstablishChannel, channelHigh, -barsToTrade, channelLow, Color.Teal, Color.Teal, 4);
		}
		
		private double CalcAtr()
		{
			double _atr = 0;
			for (int i = 0; i < 10; i++) 
			{
				_atr += ATR(10)[i];
			}
			for (int i = 0; i < 5; i++) 
			{
				_atr += ATR(5)[i];
			}
			return Instrument.MasterInstrument.Round2TickSize(_atr/15);
		}
		
		private bool isBull()
		{
			return false;
		}
		
		private bool isBear()
		{
			return false;
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
		
        [Description("ATR multiplier in channel")]
        [GridCategory("Parameters")]
        public double AtrFactorChannelIn
        {
            get { return atrFactorChannelIn; }
            set { atrFactorChannelIn = value; }
        }
		
        [Description("ATR multiplier out of channel")]
        [GridCategory("Parameters")]
        public double AtrFactorChannelOut
        {
            get { return atrFactorChannelOut; }
            set { atrFactorChannelOut = value; }
        }
		
        [Description("Percent of target which the price moved that triggers the stop to be moved to Break/Even")]
        [GridCategory("Parameters")]
        public double BreakEvenPercentOfTarget
        {
            get { return breakEvenPercentOfTarget; }
            set { breakEvenPercentOfTarget = value; }
        }
		
		[Description("After break-even target is hit, move to break-even plus x ticks")]
        [GridCategory("Parameters")]
        public int BreakEvenPlus
        {
            get { return breakEvenPlus; }
            set { breakEvenPlus = Math.Max(0, value); }
        }
		
		[Description("Period to determine channel size in min")]
        [GridCategory("Parameters")]
        public int ChannelStartPeriodSize
        {
            get { return channelStartPeriodSize; }
            set { channelStartPeriodSize = Math.Max(15, value); }
        }
		
		[Description("Number of future contracts traded")]
        [GridCategory("Parameters")]
        public int Contracts
        {
            get { return contracts; }
            set { contracts = Math.Max(1, value); }
        }
		
        [Description("Maximum allowed ATR value for trade in Points")]
        [GridCategory("Parameters")]
        public double MaxAtr
        {
            get { return maxAtr; }
            set { maxAtr = value; }
        }
		
        [Description("Stop Loss as a Percent of ATR ex 0.9 as 90% or 1.5 as 150%")]
        [GridCategory("Parameters")]
        public double StopLossPercent
        {
            get { return stopLossPercent; }
            set { stopLossPercent = value; }
        }
		
        [Description("Target Profit Percent of ATR ex 0.5 or 1.25")]
        [GridCategory("Parameters")]
        public double TargetProfitPercent
        {
            get { return targetProfitPercent; }
            set { targetProfitPercent = value; }
        }		
		
        [Description("Start Time in HHMM format, leading zeros are not needed")]
        [GridCategory("Parameters")]
        public int TradeEndTime
        {
            get { return tradeEndTime; }
            set { tradeEndTime = value; }
        }

        [Description("Start Time in HHMM format, leading zeros are not needed")]
        [GridCategory("Parameters")]
        public int TradeStartTime
        {
            get { return tradeStartTime; }
            set { tradeStartTime = value; }
        }

		
		
        #endregion
    }
}
