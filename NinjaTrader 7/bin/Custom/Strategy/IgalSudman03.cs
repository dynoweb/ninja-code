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
	/// 1. x ATR for stop loss rounded up even if its 7.1 we go to 8
	/// 2. profit target % of atr rounded down , even if its 7.9 we go to 7 but we are able to change the target to anything 75% to 110%
	/// 3. BE is a % of profit rounded down can be 50 or 90 before we go to BE
	/// 4. another BE PLUS is when we get a certain % of profit 70-80-90 whatever we put to go to BE plus ??
	/// 5. trail stop when we get to % profit we trail by ( parameter input ) ex. if we get to 80-90% of profit target for every tick up we trail by 1 tick or for every 2 ticks we trail by 1
	/// 6. we may input the profit target to be 150% in order to let the trail work because if we are right we may only lose 1 tick on the pullback fter hitting 80% of profit but have the opportunity to trail 3-4-5 ticks up which makes a difference on the profit PNL but does not really change what we give up ( at most 1-2 ticks )
	/// 7. parameter for time in market , we will not trade during news or when market closes for ex in Europe
	/// 
	/// 
	/// then we will have to be aggressive with it .. probably only move when we are at min 80-90 % of target
	/// I want to be able to set the run so if you do it as a percentage parameter we can put it as 200% target when the atr is 15 ticks - profit will be 30 so then when we hit 40-50% we can go to BE plus something then when it hits 100% go to BE plus 12 and then for every tick or two move up a tick or two
	/// I think you are right about hitting stops more then targets which is OK ,but want to take runs and let the market move to your target without chocking it
	/// going out - let me know if you want to work on things tomorrow but will probably have to wait for you to adjust things , if you want to email me some of your other systems or strategies i can look them over as well , love learning new things
	/// some conclusions - looks like 15 ticks is the right number we get 138 trades and loss of 370$
	/// of those there are 62 winner and 72 loser but if we move the break-even to closer to profit target and not do the 50% to BE then they move to 74 winners and 64 lossers with now being Profit of approx 400$ , if we are able to either get an extra 1-2 ticks or trail better beacuse most moves do have more of a follow then we have an additonal 74 *1-2 ticks , we are up 1500-2500$ for the period , looking at news will help as well ... all looks good and the number of trades is more in order of what i am seeing over the last month or so 4-5 per day and not 8-9
	/// 
    /// </summary>
    [Description("CL 3 min bars")]
    public class IgalSudman03 : Strategy
    {
        #region Variables
        
        int contracts = 1; 
		double maxAtr = 0.29;
		double breakEvenPercentOfTarget = 0.50;
		int breakEvenPlus = 0;
		double stopLossPercent = 1.0;
		double targetProfitPercent = 0.75;
		int tradeStartTime = 800;	// 8 AM Central
		int tradeEndTime = 1430;	// 2:30 PM
		
		double channelHigh = 0;
		double channelLow = 0;
		double channelSize = 0;
		
		double upperTrigger = 0;
		double lowerTrigger = 0;
		double upperStopLoss = 0;
		double lowerStopLoss = 0;
		double atr = 0;		

		IOrder buyOrder1 = null;
		IOrder sellOrder1 = null;
		IOrder closeLongOrderLimit1 = null;
		IOrder closeLongOrderStop1 = null;
		//IOrder closeLongOrder1 = null;
		IOrder closeShortOrderLimit1 = null;
		IOrder closeShortOrderStop1 = null;
		//IOrder closeShortOrder1 = null;
		
		double limitPrice = 0;
		double stopPrice = 0;
		
		string orderPrefix = "Ig3"; 
		
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();
            Unmanaged = true;				// Use unmanaged order methods
			
			Add(anaMultiPeriodBoxes(3, 5));
			
			// Local time zone: (UTC-06:00) Central Time (US & Canada)
//			Print("Local time zone: " + TimeZoneInfo.Local.DisplayName);
//			if (TimeZoneInfo.Local.DisplayName.Contains("Eastern Time"))
//			{
//				tradeStartTime = 900;
//			}
			
			//Slippage = 2;
			BarsRequired = 10;
            CalculateOnBarClose = false;		// Onbar update happens only on the start of a new bar vrs each tick
			ExitOnClose = true;				// Closes open positions at the end of the session
			IncludeCommission = true;		// Commissions are used in the calculation of the profit/loss
			TraceOrders = false;			// Trace orders in the output window, used for debugging, normally false
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// do this realtime when in a trade
			//if (!Historical && (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short))
			if (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short)
			{
				OrderManagement();
			}
			
			if (FirstTickOfBar)
			{
				// Return if historical--this sample utilizes tick information, so is necessary to run in real-time.
				if (BarsPeriod.Id != PeriodType.Minute)
					return;
							
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
			
				// Channel measurement happens 30 min after the start time
				if (channelSize == 0 && ToTime(Time[0]) == (tradeStartTime * 100) + 3300)
				{
					EstablishOpeningChannel();
				}

				if (channelSize != 0)
				{
					if (Time[0].Minute % 15 == 3 && isFlat())
					{
						atr = CalcAtr();
						Print(Time + " Order entry or adjustment - atr: " + atr + " maxAtr: " +  maxAtr);
						DrawText(CurrentBar + "atr", atr.ToString(), 2, MAX(High, 5)[1] + 0.3 * atr, Color.Black);
						if (atr <= maxAtr)
						{
							upperTrigger = MAX(High, 5)[1] + Instrument.MasterInstrument.Round2TickSize(atr);
							lowerTrigger = MIN(Low, 5)[1] - Instrument.MasterInstrument.Round2TickSize(atr);
							
							if (upperTrigger < channelLow || upperTrigger > channelHigh)
							{
								upperTrigger = MAX(High, 5)[1] + Instrument.MasterInstrument.Round2TickSize(atr * 1.5);
							}
							if (lowerTrigger < channelLow || lowerTrigger > channelHigh)
							{
								lowerTrigger = MIN(Low, 5)[1] - Instrument.MasterInstrument.Round2TickSize(atr * 1.5);
							}
							
							DrawLine(CurrentBar + "upperTrigger", 0, upperTrigger, -5, upperTrigger, Color.Blue);
							DrawLine(CurrentBar + "lowerTrigger", 0, lowerTrigger, -5, lowerTrigger, Color.Blue);
							
							upperStopLoss = upperTrigger + Instrument.MasterInstrument.Round2TickSize(atr * StopLossPercent);
							lowerStopLoss = lowerTrigger - Instrument.MasterInstrument.Round2TickSize(atr * StopLossPercent);
							
							DrawLine(CurrentBar + "upperStopLoss", 0, upperStopLoss, -5, upperStopLoss, Color.Red);
							DrawLine(CurrentBar + "lowerStopLoss", 0, lowerStopLoss, -5, lowerStopLoss, Color.Red);

							if (lowerTrigger != 0)
							{
								limitPrice = lowerTrigger;
								stopPrice = limitPrice;
								//Print(Time + " placing buy order for limitPrice: " + limitPrice + " and stopPrice: " + stopPrice);
								
								if (buyOrder1 == null)
								{
									Print(Time + " placing Buy Limit order to: " + limitPrice);
									buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, Contracts, limitPrice, stopPrice, 
										orderPrefix + "oco1", "B1");
								}
								else  if (buyOrder1.OrderState == OrderState.Working || buyOrder1.OrderState == OrderState.Accepted)
								{
									Print(Time + " changing Buy Limit order to: " + limitPrice);
									ChangeOrder(buyOrder1, buyOrder1.Quantity, limitPrice, stopPrice);
								}
								else								
								{
									Print(Time + " ***** buyOrder1.OrderState: " + buyOrder1.OrderState);
								}

								double target = lowerTrigger + Instrument.MasterInstrument.Round2TickSize(atr * TargetProfitPercent);
								DrawLine(CurrentBar+"longTarget", 0, target, -5, target, Color.Green);				
							}
							
							if (upperTrigger != 0)
							{
								limitPrice = upperTrigger;
								stopPrice = limitPrice;
								Print(Time + " placing sell order for limitPrice: " + limitPrice + " sellOrder1 is null " + (sellOrder1 == null));
								
								if (sellOrder1 == null)
								{
									Print(Time + " placing SellShort stop order to: " + limitPrice + " and stopPrice: " + stopPrice);
									sellOrder1 = SubmitOrder(0, OrderAction.SellShort, OrderType.Limit, Contracts, limitPrice, stopPrice, 
										orderPrefix + "oco1", "S1");
								} 
								else if (sellOrder1.OrderState == OrderState.Working || sellOrder1.OrderState == OrderState.Accepted)
								{
									Print(Time + " changing Sell to Open order to: " + limitPrice + " and stopPrice: " + stopPrice);
									ChangeOrder(sellOrder1, sellOrder1.Quantity, limitPrice, stopPrice);
								}
								else
								{
									Print(Time + " ********* sellOrder1.OrderState: " + sellOrder1.OrderState);
								}
								
								double target = upperTrigger - Instrument.MasterInstrument.Round2TickSize(atr * TargetProfitPercent);
								DrawLine(CurrentBar+"shortTarget", 0, target, -5, target, Color.Green);
							}
						}
						else
						{
							CloseWorkingOrders();
						}
					}
				}
			}
        }
		
		private void OrderManagement()
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			//double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
			//double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];

			
			// Change long stop order
    		if (closeLongOrderStop1 != null && closeLongOrderLimit1 != null)
			{
				//Print(Time + " CurrentAsk: " + GetCurrentAsk() + " 50% trigger: " + Instrument.MasterInstrument.Round2TickSize(buyOrder1.AvgFillPrice + (closeLongOrderLimit1.LimitPrice - buyOrder1.AvgFillPrice)*breakEvenPercentOfTarget));
				// change to B/E if 50% of target
				if (GetCurrentAsk() > buyOrder1.AvgFillPrice + (closeLongOrderLimit1.LimitPrice - buyOrder1.AvgFillPrice)*breakEvenPercentOfTarget)
				{
					// moving the stop up
					if (closeLongOrderStop1.StopPrice < buyOrder1.AvgFillPrice)
					{
						Print(Time + " moving stop to B/E");
						stopPrice = buyOrder1.AvgFillPrice + BreakEvenPlus * TickSize;
						ChangeOrder(closeLongOrderStop1, closeLongOrderStop1.Quantity, closeLongOrderStop1.LimitPrice, stopPrice);
					}
				}
			}
			
			// Change short stop order
			if (closeShortOrderStop1 != null && closeShortOrderLimit1 != null)
			{
				//Print(Time + "------------------------------------------------------");
				//Print(Time + " CurrentAsk: " + GetCurrentAsk() + "  50% trigger: " + Instrument.MasterInstrument.Round2TickSize(sellOrder1.AvgFillPrice - (sellOrder1.AvgFillPrice - closeShortOrderLimit1.LimitPrice)*breakEvenPercentOfTarget));
				
				// change to B/E if 50% of target
				if (GetCurrentAsk() < Instrument.MasterInstrument.Round2TickSize(sellOrder1.AvgFillPrice - (sellOrder1.AvgFillPrice - closeShortOrderLimit1.LimitPrice)*breakEvenPercentOfTarget))
				{
					// moving the stop down
					if (closeShortOrderStop1.StopPrice > sellOrder1.AvgFillPrice)
					{
						Print(Time + " moving stop to B/E");
						stopPrice = sellOrder1.AvgFillPrice - BreakEvenPlus * TickSize;
						ChangeOrder(closeShortOrderStop1, closeShortOrderStop1.Quantity, closeShortOrderStop1.LimitPrice, stopPrice);
					}
				}
			}
//				buyOrder1.AvgFillPrice
//				closeLongOrderLimit1.LimitPrice
//				closeLongOrderStop1.StopPrice
//				
		}

		/// <summary>
		/// Called before OnExecution
		/// 
		/// An order goes through these states
		/// - PendingChange
		/// - Accepted
		/// - Working
		/// 
		/// </summary>
		/// <param name="order"></param>
		protected override void OnOrderUpdate(IOrder order) 
		{ 
			// Rejection handling 
			if (order.OrderState == OrderState.Rejected) 
			{
				// Stop loss order was rejected !!!! 
				// Do something about it here 
				Print(Time + " >>>> order rejected <<<< " + order);
			} 
			else
			{
				//Print(Time + " orderUpdate: " + order);
				if (order.OrderState == OrderState.Cancelled && order == sellOrder1)
				{
					Print(Time + " Sell order for " + sellOrder1.LimitPrice + " cancelled");
					sellOrder1 = null;
				}
				if (order.OrderState == OrderState.Cancelled && order == buyOrder1)
				{
					Print(Time + " Buy order for " + buyOrder1.LimitPrice + " cancelled");
					buyOrder1 = null;
				}
			}
		}

		protected override void OnExecution(IExecution execution)
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			if (execution.Order == null)
			{
				Print(Time + " executed/reset on close");
				return;
			}
			
			Print(Time + " --- execution.Order: " + execution.Order);
			
			if (TraceOrders)
			{
				//Print(Time + " execution: " + execution.ToString());
				//Print(Time + " execution.Order: " + execution.Order.ToString());
			}
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (buyOrder1 != null && buyOrder1 == execution.Order)
			{
				//Print(Time + " buyOrder1: " + buyOrder1);
				
				if (closeLongOrderLimit1 == null) 
				{
					limitPrice = 0;
					stopPrice = buyOrder1.AvgFillPrice - Instrument.MasterInstrument.Round2TickSize(atr * StopLossPercent);
					//DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeLongOrderStop1 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CSB1");
					//Print(Time + " stopPrice: " + stopPrice);

					stopPrice = 0;
					limitPrice = buyOrder1.AvgFillPrice + Instrument.MasterInstrument.Round2TickSize(atr * TargetProfitPercent);
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeLongOrderLimit1 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CLB1");
					//Print(Time + " limitPrice: " + limitPrice);
				} 
			} 


			// ============================================
			// New short order placed, now set stops/limits
			// ============================================
			if (sellOrder1 != null && sellOrder1 == execution.Order)
			{
				//Print(Time + " sellOrder1: " + sellOrder1);
				
				if (closeShortOrderLimit1 == null) 
				{
					limitPrice = 0;
					stopPrice = sellOrder1.AvgFillPrice + Instrument.MasterInstrument.Round2TickSize(atr * StopLossPercent);
					//DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeShortOrderStop1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS1", "CSS1");

					stopPrice = 0;
					limitPrice = sellOrder1.AvgFillPrice - Instrument.MasterInstrument.Round2TickSize(atr * TargetProfitPercent);
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeShortOrderLimit1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS1", "CLS1");
				} 
			} 
			
			// ===================================================
			//   Trade hit Long limit
			// ===================================================
			if (closeLongOrderLimit1 != null && closeLongOrderLimit1 == execution.Order)
			{
				if (closeLongOrderLimit1.OrderState != OrderState.Filled)
				{
					Print(Time + " --> not filled: " + execution.Order);
				}
				else
					ResetTrades();
			}
			
			// ===================================================
			//   Trade hit Long stop 
			// ===================================================
			if (closeLongOrderStop1 != null && closeLongOrderStop1 == execution.Order)
			{
				ResetTrades();
			}
			
			// ===================================================
			//   Trade hit Long limit
			// ===================================================
			if (closeShortOrderLimit1 != null && closeShortOrderLimit1 == execution.Order)
			{
				ResetTrades();
			}
			
			// ===================================================
			//   Trade hit Long stop 
			// ===================================================
			if (closeShortOrderStop1 != null && closeShortOrderStop1 == execution.Order)
			{
				ResetTrades();
			}
			
		}
		
		private void CloseWorkingOrders()
		{
			//Print(Time + " Closing Working Orders");
			CloseBuyOrder();
			CloseSellOrder();
		}	
		
		private void CloseBuyOrder()
		{
			if (buyOrder1 != null && (buyOrder1.OrderState == OrderState.Working || buyOrder1.OrderState == OrderState.Accepted))
			{
				CancelOrder(buyOrder1);
			}
		}
		
		private void CloseSellOrder()
		{
			if (sellOrder1 != null && (sellOrder1.OrderState == OrderState.Working || sellOrder1.OrderState == OrderState.Accepted))
			{
				CancelOrder(sellOrder1);
			}					
		}	
		
		private void ResetTrades()
		{
			//Print(Time + " ----------- RESET ----------");
			closeLongOrderStop1 = null;
			closeLongOrderLimit1 = null;
			closeShortOrderLimit1 = null;
			closeShortOrderStop1 = null;
			buyOrder1 = null;
			sellOrder1 = null;
			lowerTrigger = 0;
			upperTrigger = 0;
		}
			
		private void EstablishOpeningChannel()
		{
			channelHigh = MAX(High, 10)[1];
			channelLow = MIN(Low, 10)[1];
			channelSize = channelHigh - channelLow;
			DrawRectangle(CurrentBar + "channel", false, 10, channelHigh, -((int) ((tradeEndTime - tradeStartTime) * 0.20 - 10)), channelLow, Color.Teal, Color.Teal, 4);
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
		
        [Description("Maximum allowed ATR value for trade in Points")]
        [GridCategory("Parameters")]
        public double MaxAtr
        {
            get { return maxAtr; }
            set { maxAtr = value; }
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
		
		[Description("Number of future contracts traded")]
        [GridCategory("Parameters")]
        public int Contracts
        {
            get { return contracts; }
            set { contracts = Math.Max(1, value); }
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
