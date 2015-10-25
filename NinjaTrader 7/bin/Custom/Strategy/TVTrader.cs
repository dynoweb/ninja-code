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
    public class TVTrader : Strategy
    {
        #region Variables
        
		double atrFactorChannelOut = 1.5;
		int channelStartPeriodSize = 30;
		
        int contracts = 1; 
		double maxAtr = 0.29;
		double breakEvenPercentOfTarget = 0.70;
		double breakEvenPlus = 0.30;
		double stopLossPercent = 1.0;
		double targetProfitPercent = 1.0;
		int tradeStartTime = 1700;	// 5:00 PM Central
		int tradeEndTime = 2000;	// 8:00 PM
		bool tradeHiAndLow = true;
		
		double channelHigh = 0;
		double channelLow = 0;
		double channelSize = 0;
		int channelStartBar = 0;
		double bottomThird = 0;
		double topThird = 0;
		
		double upperTrigger = 0;
		double lowerTrigger = 0;
		double upperStopLoss = 0;
		double lowerStopLoss = 0;
		double longTarget = 0;
		double shortTarget = 0;
		double atr = 0;		

		IOrder buyOrder1 = null;
		IOrder sellOrder1 = null;
		IOrder closeLongOrderLimit1 = null;
		IOrder closeLongOrderStop1 = null;
		IOrder closeShortOrderLimit1 = null;
		IOrder closeShortOrderStop1 = null;
		
		double limitPrice = 0;
		double stopPrice = 0;
		
		string orderPrefix = "TVT"; 
		bool fullTrace=true;		
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//ClearOutputWindow();
            Unmanaged = true;				// Use unmanaged order methods
			
			Add(anaMultiPeriodBoxes(3, 5));
			
			//Slippage = 2;
			BarsRequired = 30;
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
			// do this realtime when in a trade
			//if (!Historical && (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short))
			if (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short)
			{
				OrderManagement();
			}
			if (fullTrace) Print(Time + " OnBarUpdate CP01");
			
//			if (FirstTickOfBar)
//			{
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
//				if (ToTime(Time[0]) < (tradeStartTime * 100) || ToTime(Time[0]) > (tradeEndTime*100))
//				{
//					if (buyOrder1 != null || sellOrder1 != null)
//					{
//						Print(Time + " out of trading time slot, closing all orders");
//						CloseWorkingOrders();
//						Print(Time + " closed off session orders");
//					}
//					return;
//				}
				if (fullTrace) Print(Time + " OnBarUpdate CP03");
			
				Print(Time + " " + (tradeStartTime + channelStartPeriodSize) * 100);
				// Channel measurement happens 30 min after the start time
				if (channelSize == 0 && ToTime(Time[0]) == (1730 * 100))
				{
					DrawDot(CurrentBar + "cp", false, 0, MAX(High, 5)[0] + 5 * TickSize, Color.Blue);
					EstablishOpeningChannel();
					DrawDot(CurrentBar + "cpG", false, 0, High[0], Color.Green);
					DrawDot(CurrentBar + "cpR", false, 0, Low[0], Color.Red);
					channelStartBar = CurrentBar - Bars.BarsSinceSession;
				}

				if (channelSize != 0)
				{
					if (isFlat() & Bars.BarsSinceSession < 420)   //  && ToTime(Time[0]) >= 173000
					{
						//atr = CalcAtr();
						//Print(Time + " Order entry or adjustment - atr: " + atr + " maxAtr: " +  maxAtr);
						//DrawText(CurrentBar + "atr", atr.ToString(), 2, MAX(High, 5)[1] + 0.3 * atr, Color.Black);
						//if (atr <= maxAtr)
						if (Close[0] <= bottomThird)
						{
							//Print(Time + " High: " + High[0] + " Low: " + Low[0] + " Close: " + Close[0]);
							// channelStartBar
//							if (MAX(High, 5)[1] >= (MAX(High, CurrentBar - channelStartBar)[6]))
//							{
								upperTrigger = bottomThird + 3 * TickSize;
								longTarget = upperTrigger + 12 * TickSize;
								upperStopLoss = upperTrigger - 22 * TickSize;
									
								//if (upperTrigger > channelHigh || TradeHiAndLow)
								//{
//									DrawLine(CurrentBar + "longTarget", 0, longTarget, -40, longTarget, Color.Green);				
//									DrawLine(CurrentBar + "upperTrigger", 0, upperTrigger, -40, upperTrigger, Color.Blue);
//									DrawLine(CurrentBar + "upperStopLoss", 0, upperStopLoss, -40, upperStopLoss, Color.Red);
								//}
								//else
							//	{
							//		longTarget = 0;
							//		upperTrigger = 0;
							//		lowerTrigger = 0;
							//	}
//							}
							if (fullTrace) Print(Time + " OnBarUpdate CP05");
							
							//Print(Time + " MIN(Low, 5)[1] = " + MIN(Low, 5)[1]);
//							shortTarget = topThird - (3 + 12) * TickSize;
//							lowerTrigger = shortTarget + 3 * TickSize;
//							lowerStopLoss = lowerTrigger + 22 * TickSize;
							
							if (lowerTrigger < channelLow || TradeHiAndLow)
							{
//								DrawLine(CurrentBar + "shortTarget", 0, shortTarget, -4, shortTarget, Color.Green);
//								DrawLine(CurrentBar + "lowerTrigger", 0, lowerTrigger, -4, lowerTrigger, Color.Blue);
//								DrawLine(CurrentBar + "lowerStopLoss", 0, lowerStopLoss, -4, lowerStopLoss, Color.Red);
							}
							else
							{
								shortTarget = 0;
								lowerTrigger = 0;
								lowerStopLoss = 0;
							}
							
							if (fullTrace) Print(Time + " OnBarUpdate CP07");
							if (upperTrigger == 0)
							{
								Print(Time + " upperTrigger == 0, closing buy order");
								CloseBuyOrder();	
							}
							
							if (lowerTrigger == 0)
							{
								Print(Time + " lowerTrigger == 0, closing sell order");
								CloseSellOrder();	
							}
							

							if (upperTrigger != 0)
							{
								limitPrice = upperTrigger;
								stopPrice = limitPrice;
								//Print(Time + " placing buy order for limitPrice: " + limitPrice + " and stopPrice: " + stopPrice);
								
								if (buyOrder1 == null || (buyOrder1 != null && buyOrder1.OrderState == OrderState.Filled))
								{
									Print(Time + " placing Buy stop order to: " + limitPrice);
//									ResetLongTrade();
									buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, Contracts, limitPrice, stopPrice, 
										orderPrefix + "long", "B1");
								}
								else  if (buyOrder1.OrderState == OrderState.Working || buyOrder1.OrderState == OrderState.Accepted)
								{
									Print(Time + " changing Buy stop order to: " + limitPrice);
									ChangeOrder(buyOrder1, buyOrder1.Quantity, limitPrice, stopPrice);
								}
								else								
								{
									Print(Time + " ***** buyOrder1.OrderState: " + buyOrder1.OrderState);
								}
							}
								
							if (fullTrace) Print(Time + " OnBarUpdate CP20");
							
							if (lowerTrigger != 0)
							{
								limitPrice = lowerTrigger;
								stopPrice = limitPrice;
								Print(Time + " placing sell order for limitPrice: " + limitPrice + " sellOrder1 is null " + (sellOrder1 == null));
								
								if (sellOrder1 == null)
								{
									Print(Time + " placing SellShort stop order to: " + limitPrice + " and stopPrice: " + stopPrice);
//									ResetShortTrade();
									sellOrder1 = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, Contracts, limitPrice, stopPrice, 
										orderPrefix + "short", "S1");
								} 
								else if (sellOrder1.OrderState == OrderState.Working || sellOrder1.OrderState == OrderState.Accepted)
								{
									Print(Time + " changing SellShort stop order to: " + limitPrice + " and stopPrice: " + stopPrice);
									ChangeOrder(sellOrder1, sellOrder1.Quantity, limitPrice, stopPrice);
								}
								else
								{
									Print(Time + " ********* sellOrder1.OrderState: " + sellOrder1.OrderState);
								}
							}

							
						}
						else
						{
							Print(Time + " max ATR out of range, closing orders");
						//	CloseWorkingOrders();
						}
					}
				}				
//			} // first tick of bar
			if (fullTrace) Print(Time + " OnBarUpdate end");
        }
		
		private void OrderManagement()
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			//double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
			//double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];

			if (fullTrace) Print(Time + " OrderManagement Start");			
			// Change long stop order
    		if (closeLongOrderStop1 != null && closeLongOrderLimit1 != null && buyOrder1 != null)
			{
				if (fullTrace) Print(Time + " OrderManagement CP1");			
				//Print(Time + " CurrentAsk: " + GetCurrentAsk() + " 50% trigger: " + Instrument.MasterInstrument.Round2TickSize(buyOrder1.AvgFillPrice + (closeLongOrderLimit1.LimitPrice - buyOrder1.AvgFillPrice)*breakEvenPercentOfTarget));
				// change to B/E if 50% of target
				Print(Time + "------------------------------------------------------");
				Print(Time + " buyOrder1: " + buyOrder1 + " breakEvenPercentOfTarget: " + breakEvenPercentOfTarget);
				if (High[0] > buyOrder1.AvgFillPrice + (closeLongOrderLimit1.LimitPrice - buyOrder1.AvgFillPrice) * breakEvenPercentOfTarget)
				{
					Print(Time + " checking for long stop adjustment");
					if (fullTrace) Print(Time + " OrderManagement CP2");			
					// moving the stop up
					if (closeLongOrderStop1.StopPrice < buyOrder1.AvgFillPrice)
					{
						if (fullTrace) Print(Time + " OrderManagement CP3");			
						Print(Time + " moving stop to B/E");
						stopPrice = buyOrder1.AvgFillPrice + (closeLongOrderLimit1.LimitPrice - buyOrder1.AvgFillPrice) * BreakEvenPlus;
						if (fullTrace) Print(Time + " OrderManagement CP4");			
						ChangeOrder(closeLongOrderStop1, closeLongOrderStop1.Quantity, closeLongOrderStop1.LimitPrice, stopPrice);
					}
				}
			}
			if (fullTrace) Print(Time + " OrderManagement CP5");			
			
			// Change short stop order
			if (closeShortOrderStop1 != null && closeShortOrderLimit1 != null)
			{
				//Print(Time + "------------------------------------------------------");
				//Print(Time + " CurrentAsk: " + GetCurrentAsk() + "  50% trigger: " + Instrument.MasterInstrument.Round2TickSize(sellOrder1.AvgFillPrice - (sellOrder1.AvgFillPrice - closeShortOrderLimit1.LimitPrice)*breakEvenPercentOfTarget));
				
				// change to B/E if 50% of target
				if (Low[0] < Instrument.MasterInstrument.Round2TickSize(sellOrder1.AvgFillPrice - (sellOrder1.AvgFillPrice - closeShortOrderLimit1.LimitPrice)*breakEvenPercentOfTarget))
				{
					// moving the stop down
					if (closeShortOrderStop1.StopPrice > sellOrder1.AvgFillPrice)
					{
						Print(Time + " moving stop to B/E");
						stopPrice = sellOrder1.AvgFillPrice - (sellOrder1.AvgFillPrice - closeShortOrderLimit1.LimitPrice) * BreakEvenPlus;
						ChangeOrder(closeShortOrderStop1, closeShortOrderStop1.Quantity, closeShortOrderStop1.LimitPrice, stopPrice);
					}
				}
			}
//				buyOrder1.AvgFillPrice
//				closeLongOrderLimit1.LimitPrice
//				closeLongOrderStop1.StopPrice
//				
			if (fullTrace) Print(Time + " OrderManagement End");			
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
			if (fullTrace) Print(Time + " OnOrderUpdate start");
			// Rejection handling 
			if (order.OrderState == OrderState.Rejected) 
			{
				// Stop loss order was rejected !!!! 
				// Do something about it here 
				Print(Time + " >>>> order rejected <<<< " + order);
			} 
			else
			{
				Print(Time + " orderUpdate: " + order);
				if (order.OrderState == OrderState.Cancelled && order == sellOrder1)
				{
					Print(Time + " Sell order cancelled");
					sellOrder1 = null;
					Print(Time + " sellOrder1 == null ? " + (sellOrder1 == null));
				}
				if (order.OrderState == OrderState.Cancelled && order == buyOrder1)
				{
					Print(Time + " Buy order cancelled");
					buyOrder1 = null;
					Print(Time + " buyOrder1 == null ? " + (buyOrder1 == null));
				}
			}
			if (fullTrace) Print(Time + " OnOrderUpdate end");
		}

		protected override void OnExecution(IExecution execution)
		{
			if (fullTrace) Print(Time + " OnExecution start");
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
					stopPrice = buyOrder1.AvgFillPrice - 22 * TickSize;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeLongOrderStop1 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CSB1");
					Print(Time + " stopPrice: " + stopPrice);

					stopPrice = 0;
					limitPrice = buyOrder1.AvgFillPrice + 22 * TickSize;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeLongOrderLimit1 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CLB1");
					Print(Time + " limitPrice: " + limitPrice);
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
				{
					Print(Time + " hit limit, closing all orders");
					CloseWorkingOrders();
					buyOrder1 = null;
				}
			}
			
			// ===================================================
			//   Trade hit Long stop 
			// ===================================================
			if (closeLongOrderStop1 != null && closeLongOrderStop1 == execution.Order)
			{
				Print(Time + " hit long stop, closing all orders");
				CloseWorkingOrders();
				buyOrder1 = null;
			}
			
			// ===================================================
			//   Trade hit Short limit
			// ===================================================
			if (closeShortOrderLimit1 != null && closeShortOrderLimit1 == execution.Order)
			{
				Print(Time + " hit limit, closing all orders");
				CloseWorkingOrders();
				sellOrder1 = null;
			}
			
			// ===================================================
			//   Trade hit Short stop 
			// ===================================================
			if (closeShortOrderStop1 != null && closeShortOrderStop1 == execution.Order)
			{
				Print(Time + " hit short stop, closing all orders");
				CloseWorkingOrders();
				sellOrder1 = null;
			}
			if (fullTrace) Print(Time + " OnExecution end");
		}
		
		private void CloseWorkingOrders()
		{
			//Print(Time + " Closing Working Orders");
			CloseBuyOrder();
			CloseSellOrder();
		}	
		
		// TODO: if a trade is on, it doesn't seem to close an active trade
		private void CloseBuyOrder()
		{
			if (fullTrace) Print(Time + " CloseBuyOrder start");
			if (buyOrder1 != null)
			{
				Print(Time + " buyOrder1.OrderState = " + buyOrder1.OrderState);
				if (buyOrder1.OrderState == OrderState.Working || buyOrder1.OrderState == OrderState.Accepted)
				{
					CancelOrder(buyOrder1);
				}
				else if (buyOrder1.OrderState == OrderState.Filled)
				{
					Print(Time + " buyOrder1.OrderState is Filled, setting to null");
					buyOrder1 = null;
				}
				else
				{
					Print(Time + " buyOrder1.OrderState = " + buyOrder1.OrderState);
				}
//				Print(Time + " isNulls - buyOrder1 " + (buyOrder1 == null) + " closeLongOrderLimit1 " + (closeLongOrderLimit1 == null) + " closeLongOrderStop1 " + (closeLongOrderStop1 == null));
			}
			ResetLongTrade();
			if (fullTrace) Print(Time + " CloseBuyOrder end");
		}
		
		private void CloseSellOrder()
		{
			Print(Time + " CloseSellOrder end");
			if (sellOrder1 != null)
			{
				if (sellOrder1.OrderState == OrderState.Working || sellOrder1.OrderState == OrderState.Accepted)
				{
					CancelOrder(sellOrder1);
				}					
				else if (sellOrder1.OrderState == OrderState.Filled)
				{
					Print(Time + " sellOrder1.orderState is Filled, setting to null");
					sellOrder1 = null;
				}
				else
				{
					Print(Time + " sellOrder1.OrderState = " + sellOrder1.OrderState);
				}
//				Print(Time + " isNulls - sellOrder1 " + (sellOrder1 == null) + " closeShortOrderLimit1 " + (closeShortOrderLimit1 == null) + " closeShortOrderStop1 " + (closeShortOrderStop1 == null));
			}
			
			ResetShortTrade();
			if (fullTrace) Print(Time + " CloseSellOrder end");
		}
		
		private void ResetTrades()
		{
			ResetLongTrade();
			ResetShortTrade();
		}
		
		private void ResetLongTrade()
		{
			closeLongOrderStop1 = null;
			closeLongOrderLimit1 = null;
		}

		private void ResetShortTrade()
		{
			closeShortOrderLimit1 = null;
			closeShortOrderStop1 = null;
		}

		private void EstablishOpeningChannel()
		{
			if (fullTrace) Print(Time + " OnBarUpdate CP02");
			int barsToEstablishChannel = channelStartPeriodSize/BarsPeriod.Value;
			if (fullTrace) Print(Time + " Time of startChannel " + Time[barsToEstablishChannel]);
			Print(Time + " Bars.BarsSinceSession: " + Bars.BarsSinceSession);
			channelHigh = MAX(High, Bars.BarsSinceSession + 1)[0];
			channelLow = MIN(Low, Bars.BarsSinceSession + 1)[0];
			channelSize = channelHigh - channelLow;
			int endHour = tradeEndTime/100;
			int endMin = tradeEndTime - endHour * 100;
			int barsToTrade = (endHour * 60 + endMin - (Time[0].Hour * 60 + Time[0].Minute)) / BarsPeriod.Value; 
			Print(Time +  " barsToTrade: " + barsToTrade + " tradeEndTime: " + tradeEndTime +  " Time[0]: " + Time[0] + " endHour: " + endHour + " endMin: " + endMin);
			DrawRectangle(CurrentBar + "channel", false, Bars.BarsSinceSession, channelHigh, -barsToTrade, channelLow, Color.Teal, Color.Teal, 4);
			bottomThird = Instrument.MasterInstrument.Round2TickSize(channelLow + channelSize * 0.3333);
			topThird = Instrument.MasterInstrument.Round2TickSize(channelHigh - channelSize * 0.3333);
			DrawRectangle(CurrentBar + "channel3", false, 0, topThird, -barsToTrade, bottomThird, Color.Teal, Color.Red, 4);
			DrawRectangle(CurrentBar + "channels", false, Bars.BarsSinceSession, channelHigh, 0, channelLow, Color.Teal, Color.Teal, 6);
		}
		
//		protected override void OnPositionUpdate(IPosition position)
//		{ 
//			if (position.MarketPosition == MarketPosition.Flat)
//			{
//				ResetLongTrade();
//				ResetShortTrade();
//				Print(Time + " setting orders to null");
//				buyOrder1 = null;
//				sellOrder1 = null;
//			}
//		}		
		
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
		
        [Description("ATR multiplier out of channel")]
        [GridCategory("Parameters")]
        public double AtrFactorChannelOut
        {
            get { return atrFactorChannelOut; }
            set { atrFactorChannelOut = value; }
        }
		
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
		
		[Description("After break-even target is hit, move to break-even plus x percent")]
        [GridCategory("Parameters")]
        public double BreakEvenPlus
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
		
        [Description("Stop Loss as a Percent of ATR ex 0.9 as 90% or 1.5 as 150%")]
        [GridCategory("Parameters")]
        public double StopLossPercent
        {
            get { return stopLossPercent; }
            set { stopLossPercent = value; }
        }
				
        [Description("Trade both directions anytime")]
        [GridCategory("Parameters")]
        public bool TradeHiAndLow
        {
            get { return tradeHiAndLow; }
            set { tradeHiAndLow = value; }
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
