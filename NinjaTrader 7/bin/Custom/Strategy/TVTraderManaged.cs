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
    /// CL 3 min bars
	/// 
	/// This strategy came from Ben Coggins, DTI subscription on 7/9/2015.  Here are the rules. 
	///
	/// Uses the period from 5 PM to 5:30 PM CST to establish a price channel. If the price at 5:30 is in the top 1/3 of the price channel range, open a short
	/// position at the first sell target below the top 1/3 range of the price channel.  If it's in the bottom 1/3 of the price channel range, open
	/// a long position at the first buy target price.
	/// 
	/// Long open target prices *.11, .31, .61, .81
	/// Short open target prices *.89 .69 .39 .19
	/// 
	/// Close target is 12-29 cents
	/// Stop is at 22 cents
	/// 
	/// 
    /// </summary>
    [Description("CL 1 min bars")]
    public class TVTraderManaged : Strategy
    {
        #region Variables
        
//		double atrFactorChannelOut = 1.5;
//		int channelStartPeriodSize = 30;
		
        int contracts = 1; 
//		double maxAtr = 0.29;
//		double breakEvenPercentOfTarget = 0.70;
//		double breakEvenPlus = 0.30;
		int minRange = 6;
		int stopLoss = 22;
		int profitTarget = 12;
		int trailStop = 0;
//		int tradeStartTime = 1700;	// 5:00 PM Central
//		int tradeEndTime = 2000;	// 8:00 PM
//		bool tradeHiAndLow = true;
		
		double channelHigh = 0;
		double channelLow = 0;
		double channelSize = 0;
		double bottomThird = 0;
		double topThird = 0;
		double channelClose = 0;
		bool closedInTop = false;
		bool closedInBottom = false;
		double buyPrice = 0;
		double sellPrice = 0;
		int startOfSessionTradeCount = 0;
		
		double upperTrigger = 0;
		double lowerTrigger = 0;
		double upperStopLoss = 0;
		double lowerStopLoss = 0;
		double longTarget = 0;
		double shortTarget = 0;
//		double atr = 0;		

//		IOrder buyOrder1 = null;
//		IOrder sellOrder1 = null;
//		IOrder closeLongOrderLimit1 = null;
//		IOrder closeLongOrderStop1 = null;
//		IOrder closeShortOrderLimit1 = null;
//		IOrder closeShortOrderStop1 = null;
		
		double limitPrice = 0;
		double stopPrice = 0;
		
		string orderPrefix = "TV"; 
		bool fullTrace=false;		
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//ClearOutputWindow();
            Unmanaged = false;				// Use unmanaged order methods
			
			SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
			// it will use one or the other, but not both
			if (StopLoss != 0)  SetStopLoss(CalculationMode.Ticks, StopLoss);
			if (TrailStop != 0) SetTrailStop(CalculationMode.Ticks, TrailStop);
			
			//Slippage = 2;
			BarsRequired = 6;
            CalculateOnBarClose = true;		// Onbar update happens only on the start of a new bar vrs each tick
			ExitOnClose = true;				// Closes open positions at the end of the session
			IncludeCommission = true;		// Commissions are used in the calculation of the profit/loss
			TraceOrders = false;			// Trace orders in the output window, used for debugging, normally false
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (fullTrace) Print(Time + " OnBarUpdate start");

			// Return if historical--this sample utilizes tick information, so is necessary to run in real-time.
			if (BarsPeriod.Id != PeriodType.Minute)
				return;
						
			// reset variables at the start of each day
			// or if (Bars.FirstBarOfSession)
			if (Bars.BarsSinceSession == 0)
			{
				//Print(Time + " 1 - resetting channel size - BarsSinceSession: " + Bars.BarsSinceSession);
				channelHigh = 0;
				channelLow = 0;
				channelSize = 0;
				upperTrigger = 0;
				lowerTrigger = 0;
				startOfSessionTradeCount = Performance.AllTrades.Count;
			}
			
			if (fullTrace) Print(Time + " OnBarUpdate CP03");
		
			if (channelSize == 0 && ToTime(Time[0]) == 173000)
			{
				EstablishOpeningChannel();
			}

			if (channelSize != 0)
			{
				if (fullTrace) Print(Time + " OnBarUpdate CP04");
				
				if (isFlat() & Bars.BarsSinceSession < 300)   // 420 for min  && ToTime(Time[0]) >= 173000
				{
					if (fullTrace) Print(Time + " OnBarUpdate CP05");
					//Print(Time + " CurrentBar Data High: " + High[0] + " Low: " + Low[0] + " Close: " + Close[0]);
					
					if (closedInBottom)
					{
						upperTrigger = buyPrice;
						longTarget = upperTrigger + ProfitTarget * TickSize;
						upperStopLoss = upperTrigger - StopLoss * TickSize;
								
						EnterLongStop(Contracts, upperTrigger, "LE");						
						
						//Print(Time + " placing buy order for limitPrice: " + limitPrice + " and stopPrice: " + stopPrice);
						//Print(Time + " placing Buy stop order to: " + upperTrigger);
						
						DrawLine(CurrentBar + "longTarget", 0, longTarget, -5, longTarget, Color.Green);				
						DrawLine(CurrentBar + "upperTrigger", 0, upperTrigger, -5, upperTrigger, Color.Blue);
						DrawLine(CurrentBar + "upperStopLoss", 0, upperStopLoss, -5, upperStopLoss, Color.Red);
					}
					
						
					if (closedInTop)
					{
						lowerTrigger = sellPrice;
						shortTarget = lowerTrigger - ProfitTarget * TickSize;
						lowerStopLoss = lowerTrigger + StopLoss * TickSize;

						EnterShortStop(Contracts, lowerTrigger, "SE");
						
						//Print(Time + " placing sell order for limitPrice: " + limitPrice + " sellOrder1 is null " + (sellOrder1 == null));
						//Print(Time + " placing SellShort stop order to: " + lowerTrigger);
						
						DrawLine(CurrentBar + "shortTarget", 0, shortTarget, -5, shortTarget, Color.Green);
						DrawLine(CurrentBar + "lowerTrigger", 0, lowerTrigger, -5, lowerTrigger, Color.Blue);
						DrawLine(CurrentBar + "lowerStopLoss", 0, lowerStopLoss, -5, lowerStopLoss, Color.Red);
					}
				}
			}				
			
//			if (Performance.AllTrades.Count > startOfSessionTradeCount)
//			{
//				if (BarsSinceEntry() == 10)
//				{
//					if (isLong()) ExitLong();
//					if (isShort()) ExitShort();
//				}
//			}
			
			if (fullTrace) Print(Time + " OnBarUpdate end");
        }
		
		protected override void OnOrderUpdate(IOrder order)
		{
			//Print(Time + " " + order.ToString());
			if (order.OrderState == OrderState.Filled)
			{
				//Print(Time + " 2 - resetting channel size - BarsSinceSession: " + Bars.BarsSinceSession);
				channelSize = 0;
			}
		}
		
		private double GetBuyPrice()
		{
			double r = Instrument.MasterInstrument.Round2TickSize(channelClose - Math.Truncate(channelClose));
			double result = 0;

			if (r < 0.11)
				result = Math.Truncate(channelClose) + 0.11;
			else if (r < 0.31)
				result = Math.Truncate(channelClose) + 0.31;
			else if (r < 0.61)
				result = Math.Truncate(channelClose) + 0.61;
			else if (r < 0.81)
				result = Math.Truncate(channelClose) + 0.81;
			else if (r >= 0.81)
				result = Math.Truncate(channelClose) + 1.11;
				
			//Print(Time + " channelClose: " + channelClose + " r: " + r + " BuyPrice: " + result);
			return result;
		}

		private double GetSellPrice()
		{
			double r = Instrument.MasterInstrument.Round2TickSize(channelClose - Math.Truncate(channelClose));
			double result = 0;

			if (r > 0.89)
				result = Math.Truncate(channelClose) + 0.89;
			else if (r > 0.69)
				result = Math.Truncate(channelClose) + 0.69;
			else if (r > 0.39)
				result = Math.Truncate(channelClose) + 0.39;
			else if (r > 0.19)
				result = Math.Truncate(channelClose) + 0.19;
			else if (r <= 0.19)
				result = Math.Truncate(channelClose) - 0.11;
				
			//Print(Time + " channelClose: " + channelClose + " r: " + r + " SellPrice: " + result);
			return result;
		}
		
		private void EstablishOpeningChannel()
		{
			if (fullTrace) Print(Time + " OnBarUpdate CP02");
//			int barsToEstablishChannel = channelStartPeriodSize/BarsPeriod.Value;
//			if (fullTrace) Print(Time + " Time of startChannel " + Time[barsToEstablishChannel]);
//			Print(Time + " Bars.BarsSinceSession: " + Bars.BarsSinceSession);
			
			channelHigh = MAX(High, Bars.BarsSinceSession + 1)[0];
			channelLow = MIN(Low, Bars.BarsSinceSession + 1)[0];
			if ((channelHigh - channelLow) < minRange * TickSize)
				return;
			channelSize = channelHigh - channelLow;
//			int endHour = tradeEndTime/100;
//			int endMin = tradeEndTime - endHour * 100;
//			int barsToTrade = (endHour * 60 + endMin - (Time[0].Hour * 60 + Time[0].Minute)) / BarsPeriod.Value; 
			//Print(Time +  " barsToTrade: " + barsToTrade + " tradeEndTime: " + tradeEndTime +  " Time[0]: " + Time[0] + " endHour: " + endHour + " endMin: " + endMin);
			//DrawRectangle(CurrentBar + "channel", false, Bars.BarsSinceSession, channelHigh, -barsToTrade, channelLow, Color.Teal, Color.Teal, 4);
			bottomThird = Instrument.MasterInstrument.Round2TickSize(channelLow + channelSize * 0.3333);
			topThird = Instrument.MasterInstrument.Round2TickSize(channelHigh - channelSize * 0.3333);
			DrawRectangle(CurrentBar + "channel3", false, 0, topThird, -250, bottomThird, Color.Teal, Color.Red, 4);
			DrawRectangle(CurrentBar + "channels", false, Bars.BarsSinceSession, channelHigh, 0, channelLow, Color.Teal, Color.Teal, 6);
			
			channelClose = Close[0];
			closedInTop = false;
			closedInBottom = false;
			if (channelClose >= topThird) 
			{
				closedInTop = true;
				DrawArrowDown(CurrentBar + "DN", 2, channelHigh + 2 * TickSize, Color.Red);
				sellPrice = GetSellPrice();
			}
			if (channelClose <= bottomThird) 
			{
				closedInBottom = true;
				DrawArrowUp(CurrentBar + "UP", 2, channelLow - 2 * TickSize, Color.Green);
				buyPrice = GetBuyPrice();
			}
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
		
//        [Description("ATR multiplier out of channel")]
//        [GridCategory("Parameters")]
//        public double AtrFactorChannelOut
//        {
//            get { return atrFactorChannelOut; }
//            set { atrFactorChannelOut = value; }
//        }
//		
//        [Description("Maximum allowed ATR value for trade in Points")]
//        [GridCategory("Parameters")]
//        public double MaxAtr
//        {
//            get { return maxAtr; }
//            set { maxAtr = value; }
//        }
//		
//        [Description("Percent of target which the price moved that triggers the stop to be moved to Break/Even")]
//        [GridCategory("Parameters")]
//        public double BreakEvenPercentOfTarget
//        {
//            get { return breakEvenPercentOfTarget; }
//            set { breakEvenPercentOfTarget = value; }
//        }
//		
//		[Description("After break-even target is hit, move to break-even plus x percent")]
//        [GridCategory("Parameters")]
//        public double BreakEvenPlus
//        {
//            get { return breakEvenPlus; }
//            set { breakEvenPlus = Math.Max(0, value); }
//        }
//		
//		[Description("Period to determine channel size in min")]
//        [GridCategory("Parameters")]
//        public int ChannelStartPeriodSize
//        {
//            get { return channelStartPeriodSize; }
//            set { channelStartPeriodSize = Math.Max(15, value); }
//        }
		
		[Description("Number of future contracts traded")]
        [GridCategory("Parameters")]
        public int Contracts
        {
            get { return contracts; }
            set { contracts = Math.Max(1, value); }
        }
		
//        [Description("Stop Loss as a Percent of ATR ex 0.9 as 90% or 1.5 as 150%")]
//        [GridCategory("Parameters")]
//        public double StopLossPercent
//        {
//            get { return stopLossPercent; }
//            set { stopLossPercent = value; }
//        }
//				
//        [Description("Trade both directions anytime")]
//        [GridCategory("Parameters")]
//        public bool TradeHiAndLow
//        {
//            get { return tradeHiAndLow; }
//            set { tradeHiAndLow = value; }
//        }				
//		
//        [Description("Target Profit Percent of ATR ex 0.5 or 1.25")]
//        [GridCategory("Parameters")]
//        public double TargetProfitPercent
//        {
//            get { return targetProfitPercent; }
//            set { targetProfitPercent = value; }
//        }		
		
        [Description("Minimum Range for first 30 min of the session in ticks")]
        [GridCategory("Parameters")]
        public int MinRange
        {
            get { return minRange; }
            set { minRange = Math.Max(0, value); }
        }

        [Description("Stop Loss in Ticks")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = value; }
        }
				
        [Description("Profit Target in Ticks")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = value; }
        }				
				
        [Description("Trailing stop in Ticks")]
        [GridCategory("Parameters")]
        public int TrailStop
        {
            get { return trailStop; }
            set { trailStop = value; }
        }		
//        [Description("Start Time in HHMM format, leading zeros are not needed")]
//        [GridCategory("Parameters")]
//        public int TradeEndTime
//        {
//            get { return tradeEndTime; }
//            set { tradeEndTime = value; }
//        }
//
//        [Description("Start Time in HHMM format, leading zeros are not needed")]
//        [GridCategory("Parameters")]
//        public int TradeStartTime
//        {
//            get { return tradeStartTime; }
//            set { tradeStartTime = value; }
//        }

        #endregion
    }
}
