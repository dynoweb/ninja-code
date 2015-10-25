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

namespace NinjaTrader.Strategy
{
    /// <summary>
	/// Strategy written by Rick Cromer (dynoweb)
	/// 
    /// Uses CL to jump in a breakout based on an opening Range. Requires Minute Period Type
	/// 
	/// CL Pit opens 09:00 ET closes 14:30 ET after which it goes Globex, 
	/// CME stops Globex at 17:15 and resumes it at 18:00
	/// 
	/// Press F5 to compile
	/// 
	/// Problems 
	/// - initial qty
	/// - trades continuing to end of session
	/// 
	/// Testing
	/// - 1/14/2013 - Replay testing from CL 02-13, 1 min bars, from 11/17/2012 - 1/12/2013 - 20,true,35,10,7,2,0,30, $3,470.00
	/// - 3/23/2013 - YM 8/18 TF 8/13 ES 5/12 channel curve fitted
	/// 
	/// Update Release
	/// - 8/3/2014 - increased the starting contracts to 5 and decreased the number of reversals to 1. Big Mike released.
	/// 
	/// Changes
    /// </summary>
    [Description("Uses CL to jump in a breakout from a Range using unmanaged order methods. Currently based on 20, 1 Min Periods")]
    public class Dynoweb : Strategy
    {
        #region Variables

		// Exposed variables
       // ATM Strategy Variables - sets stop loss to B/E when price moves to this target
		private bool breakEvenLast = false;
		private int channelMax = 40;
		private int channelMin = 5;
		private bool enableSummary = false;
		private int extendPeriod = 0;	
		private int hour = 5; // CST
		private int maxReversals = 1;		
		private int minute = 0;
		private int period = 60;
		private int sessionHighLow = 0;
		private double stdDevMax = 0.08;
		private bool useTrailing = false;
		
       // private int abeProfitTrigger = 20; // ATM Strategy Variables - sets stop loss to B/E when price moves to this target
		/*private bool breakEvenLast = false;
		private int channelMax = 45;
		private int channelMin = 15;
		private bool enableSummary = true;
		private int extendPeriod = 1;	// 0 == false, 1 == true
		private int hour = 7; // CST
		private int maxReversals = 4;		
		private int minute = 0;
		private int period = 18;
		private int sessionHighLow = 0;
		private bool useTrailing = false;
		private double stdDevMax = 0.06;
		*//*
		// Optimized for FDAX 1 min
		private bool breakEvenLast = false;
		private int channelMax = 60;
		private int channelMin = 15;
		private bool enableSummary = true;
		private int extendPeriod = 0;	// 0 == false, 1 == true
		private int hour = 5; // CST
		private int maxReversals = 2;		
		private int minute = 0;
		private int period = 50;
		private int sessionHighLow = 0;
		private bool useTrailing = false;
		*/
		
		// channel
		private double channelHigh = 0;
		private double channelLow = 0;
		private double startingChannelHigh = 0;
		private double startingChannelLow = 0;
		private int adjustedChannelSize = 0;		

		// Trade Summary
		private double lastcumprofit = 0;
		private	Font textFont = new Font("Arial",8,FontStyle.Regular);
		
        // User defined variables (add any user defined variables below)
        private static int startQty = 5; // Starting Order Qty for the day, converts to new Qty after reversal
		private int qty = 5;
		private int tradeCount = 0;
		private double shortStop = 0;
		private double longStop = 0;
		private double profitPoints = 0;
		private bool enableTraceOrders = false;
		private double breakEvenPrice = 0;
		private int barNumberOfOrder = 0;		
		private int barNumberSinceFilled  = 0;
		
		//Stochastics stoc;
		//Stochastics stocX3;
		int stocD = 3;
		int stocK = 5;
		int stocS = 2;
		
		private int maPeriod = 200;
		HMA ma = null;
		
		private IOrder entryOrderLong = null;
		private IOrder entryOrderShort = null;
		private IOrder closeOrderLongLimit = null;
		private IOrder closeOrderLongStop = null;
		private IOrder closeOrderShortLimit = null;
		private IOrder closeOrderShortStop = null;
		private IOrder closeOrderLong = null;	
		private IOrder closeOrderShort = null;	
		
		private string orderPrefix = "Dynoweb";
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			// re-trigger on each tick - may not be required with unmanaged order entry, yet to be determined
            CalculateOnBarClose = false;
			// Use Unmanaged order methods
    		Unmanaged = true;
			
			// Triggers the exit on close function 60 min prior to session end 
			// Note: This property is a real-time only property.
			ExitOnClose = true;
			ExitOnCloseSeconds = 3600;	// 60 min before session close
			TimeInForce = Cbi.TimeInForce.Day;
			
			//Add(ATR(period));
			//ATR(period).Plots[0].Pen.Color = Color.DarkGreen;
			
			ma = HMA(MaPeriod);
			Add(ma);
			
//            Add(Stochastics(stocD, stocK, stocS));
//			Stochastics(stocD, stocK, stocS).Plots[0].Pen.Color = Color.Blue; // D color
//			Stochastics(stocD, stocK, stocS).Plots[0].Pen.Width = 2;
//			Stochastics(stocD, stocK, stocS).Lines[0].Pen.Color = Color.Black; // Lower
//			Stochastics(stocD, stocK, stocS).Lines[0].Pen.DashStyle = DashStyle.Dot;
//			Stochastics(stocD, stocK, stocS).Lines[0].Pen.Width = 2;
//			Stochastics(stocD, stocK, stocS).Plots[1].Pen.Color = Color.Black; // K color
//			Stochastics(stocD, stocK, stocS).Lines[1].Pen.Color = Color.Black; // Upper
//			Stochastics(stocD, stocK, stocS).Lines[1].Pen.DashStyle = DashStyle.Dot;
//			Stochastics(stocD, stocK, stocS).Lines[1].Pen.Width = 2;
//
			ClearOutputWindow();
    		TraceOrders = enableTraceOrders; 
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double limitPrice = 0;
			double stopPrice = 0;
			
			DateTime exp = new DateTime(2020, 1, 1);
			if (BarsPeriod.Id != PeriodType.Minute || ToDay(Time[0]) > ToDay(exp))
				return;

			// If it's Friday, do not trade.
		    //if (Time[0].DayOfWeek == DayOfWeek.Tuesday)
        	//	return;

			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				Print("=======================");
				channelHigh = 0;
				channelLow = 0;
				tradeCount = 0;
				profitPoints = 0;
				adjustedChannelSize = 0;
				qty = startQty;
				ResetTrade();
			}
			
			// do this when in a trade
			if (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short)
			{
				OrderManagement();
			}
			
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
					startingChannelHigh = channelHigh;
					startingChannelLow = channelLow;
					
					DrawDot(CurrentBar + "channelHigh", false, 0, channelHigh, Color.Blue);
					DrawDot(CurrentBar + "channelLow", false, 0, channelLow, Color.Cyan);
					DrawHorizontalLine("channelHigh", channelHigh, Color.Blue);
					DrawHorizontalLine("channelLow", channelLow, Color.Cyan);
				}
			}
			
			if (adjustedChannelSize != 0 
				//&& tradeCount <= maxReversals
				&& tradeCount == 0
				&& Position.MarketPosition == MarketPosition.Flat
				&& profitPoints == 0
				//&& ToTime(Time[0]) >= (ToTime(hour, minute, 0) + 10000) // add an hour before we start the trades
				) 
			{
//				if (StdDev(20)[0] < stdDevMax)
//				{
//					if (Rising(ma) || MaPeriod == 0)
//					{
//						if (entryOrderShort != null && MaPeriod > 0)
//						{
//							CancelOrder(entryOrderShort);
//						}
						if (entryOrderLong == null)
					 	{					
							stopPrice = channelHigh + 1 * TickSize;
							limitPrice = stopPrice + 0 * TickSize;
							if (Close[0] < stopPrice)
							{
								DrawDot(CurrentBar + "Long_stopPrice", false, 0, stopPrice, Color.White);
								DrawDot(CurrentBar + "Long_limitPrice", false, 0, limitPrice, Color.Black);
								entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Long");
						
								barNumberOfOrder = CurrentBar;
							}
						}
//					}
//					if (Falling(ma) || MaPeriod == 0)
//					{
//						if (entryOrderLong != null && MaPeriod > 0)
//						{
//							CancelOrder(entryOrderLong);
//						}
						if (entryOrderShort == null) 
						{					
							//Print(Time + " ATR: " + ATR(period).Plots[0]);
			
							stopPrice = channelLow - 1 * TickSize;
							limitPrice = stopPrice - 0 * TickSize;
							if (Close[0] > stopPrice)
							{
								DrawDot(CurrentBar + "Short_stopPrice", false, 0, stopPrice, Color.White);
								DrawDot(CurrentBar + "Short_limitPrice", false, 0, limitPrice, Color.Black);
								entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Short");
								
								barNumberOfOrder = CurrentBar;
							}
//						}
//					}
				}
//				else
//				{
//					DrawDiamond(CurrentBar + "highSD", false, 0, (channelHigh + channelLow)/2, Color.Red);
//				}
			}			
			
			if (enableSummary)
			{
				if (lastcumprofit != Performance.AllTrades.TradesPerformance.Currency.CumProfit)
				{
					Print(Time[0] + " " + Performance.LongTrades.TradesPerformance.Currency.CumProfit.ToString("0") + " + " + Performance.ShortTrades.TradesPerformance.Currency.CumProfit.ToString("0") + " = " + Performance.AllTrades.TradesPerformance.Currency.CumProfit.ToString("0"));
					lastcumprofit = Performance.AllTrades.TradesPerformance.Currency.CumProfit;
				}
			
				string last = "";
			    if (Performance.AllTrades.Count > 1)
			    {
					Trade lastTrade = Performance.AllTrades[Performance.AllTrades.Count - 1];
					last = "\nLast: " + lastTrade.Quantity + " x " + Instrument.MasterInstrument.Round2TickSize(lastTrade.ProfitPoints) + " points";
 				}

				// http://msdn.microsoft.com/en-us/library/system.string.format.aspx
				DrawTextFixed("pnl","All: " + Performance.AllTrades.TradesCount + " W:" + (Performance.AllTrades.WinningTrades.Count) + " L: " + (Performance.AllTrades.LosingTrades.Count) +
					" PNL: $" + (int) Performance.AllTrades.TradesPerformance.Currency.CumProfit +
					"\nLongs: " + Performance.LongTrades.TradesCount + " W: " + (Performance.LongTrades.WinningTrades.Count) + " L: " + (Performance.LongTrades.LosingTrades.Count) +
					" PNL: $" + (int) Performance.LongTrades.TradesPerformance.Currency.CumProfit +
					"\nShorts: " + Performance.ShortTrades.TradesCount + " W: " + (Performance.ShortTrades.WinningTrades.Count) + " L: " + (Performance.ShortTrades.LosingTrades.Count) +
					" PNL: $" + (int) Performance.ShortTrades.TradesPerformance.Currency.CumProfit +
					"\nAccount: " + Account.Name +
					"\nAccount Balance: $" + GetAccountValue(AccountItem.CashValue) + last 
					,TextPosition.TopLeft,Color.Black,textFont,Color.Green,Color.White,80);
			}
        }

		/// <summary>
		/// 
		/// </summary>
		private void OrderManagement()
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
			double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];

			
			// Change long close order to close if stop trigger (closed below channel)
    		if (null != closeOrderLongStop)
			{
				if (closeOrderLong == null
					&& Low[0] < channelLow
					&& barNumberSinceFilled != barNumberSinceFilled
					)
				{
					//Print(Time + " Closed below channel on a long, selling position");
					closeOrderLong = SubmitOrder(0, OrderAction.Sell, OrderType.Market, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long");
				} 
				else if (breakEvenLast && highSinceOpen > breakEvenPrice && tradeCount == maxReversals)
				{
					stopPrice = Math.Max(closeOrderLongStop.StopPrice, entryOrderLong.AvgFillPrice);
					if (useTrailing)
					{
						stopPrice = Math.Max(closeOrderLongStop.StopPrice, entryOrderLong.AvgFillPrice + (highSinceOpen - breakEvenPrice));
					    DrawDot(CurrentBar + "breakEvenPrice", true, 0, stopPrice, Color.Yellow);
					}
					ChangeOrder(closeOrderLongStop, closeOrderLongStop.Quantity, closeOrderLongStop.LimitPrice, stopPrice);
				}

				// keep from bouncing on one bar
				if (barNumberSinceFilled != CurrentBar)
				{
					Print(Time + "   LimitPrice: " + closeOrderLongLimit.LimitPrice + 
						" startingChannelHigh: " + startingChannelHigh + 
						" adjustedChannelSize: " + adjustedChannelSize * TickSize + 
						" result: " + (closeOrderLongLimit.LimitPrice - startingChannelHigh) * 0.5);
					
					// Slide channel up if price has reached at least 50% of target.
					if (highSinceOpen > (startingChannelHigh + (closeOrderLongLimit.LimitPrice - startingChannelHigh) * 0.5)
						&& tradeCount != maxReversals)
					{
//	RC 7/20/15					channelHigh = startingChannelHigh + (highSinceOpen - startingChannelHigh) * 0.75;
//	RC 7/20/15					channelLow = channelHigh - adjustedChannelSize * TickSize;
						//Print(Time + " new channelLow: " + channelLow);
						DrawDot(CurrentBar + "channelLow", false, 0, channelLow, Color.Yellow);
						//DrawDot(CurrentBar + "channelHigh", false, 0, channelHigh, Color.Yellow);
					}
					
					stopPrice = channelLow - 1 * TickSize;
					
					//if (closeOrderLongStop.StopPrice != stopPrice)
					if ((int) (closeOrderLongStop.StopPrice * 100) != (int) (stopPrice * 100))
					{
						ChangeOrder(closeOrderLongStop, closeOrderLongStop.Quantity, closeOrderLongStop.LimitPrice, stopPrice);
						DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					}
				}
			}
			
			// Change short close order to close if stop trigger (closed above channel)
    		if (null != closeOrderShortStop)
			{
				if (closeOrderShort == null
					&& High[0] > channelHigh
					&& barNumberSinceFilled != barNumberSinceFilled
					)
				{
					//Print(Time + " Closed above channel on a short, buying to cover position");
					closeOrderShort = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short");
					//Print(Time + " Submitted order to buy to cover position");
				}
				else if (breakEvenLast && lowSinceOpen < breakEvenPrice && tradeCount == maxReversals)
				{
					stopPrice = Math.Min(closeOrderShortStop.StopPrice, entryOrderShort.AvgFillPrice);
					if (useTrailing)
					{
						stopPrice = Math.Min(closeOrderShortStop.StopPrice, entryOrderShort.AvgFillPrice - (breakEvenPrice - lowSinceOpen));
					    DrawDot(CurrentBar + "breakEvenPrice", true, 0, stopPrice, Color.Yellow);
					}
					ChangeOrder(closeOrderShortStop, closeOrderShortStop.Quantity, closeOrderShortStop.LimitPrice, stopPrice);
					//DrawDot(CurrentBar + "breakEvenPrice", true, 0, stopPrice, Color.Yellow);
				}
				
				// keep from bouncing on one bar
				if (barNumberSinceFilled != CurrentBar)
				{
					// Slide channel up if price has reached at least 50% of target.
					if (lowSinceOpen < startingChannelLow // - (startingChannelLow - closeOrderShortLimit.LimitPrice) * 0.5)
						&& tradeCount != maxReversals)
					{
// RC 7/20/15						channelLow = startingChannelLow - (startingChannelLow - lowSinceOpen) * 0.50;
						//channelLow = lowSinceOpen;
// RC 7/20/15						channelHigh = channelLow + adjustedChannelSize * TickSize;
						//DrawDot(CurrentBar + "channelLow", false, 0, channelLow, Color.Green);
						DrawDot(CurrentBar + "channelHigh", false, 0, channelHigh, Color.Green);
					}
					
					stopPrice = channelHigh + 1 * TickSize;
					
					// rounding numbers, doubles wouldn't match
					//int sp = (int) (stopPrice * 100);
					if ((int) (closeOrderShortStop.StopPrice * 100) != (int) (stopPrice * 100))
					{
						//Print(Time + " closeOrderShortStop.StopPrice: " + closeOrderShortStop.StopPrice + " stopPrice: " + stopPrice);
						ChangeOrder(closeOrderShortStop, closeOrderShortStop.Quantity, closeOrderShortStop.LimitPrice, stopPrice);
						DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					}
				}
			}
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
			
			DrawText(CurrentBar + "adj", channelSize.ToString(), 3, channelHigh + 4 * TickSize, Color.Black);
			if (adjustedChannelSize != 0)
			{
				Print(Time + " channelLow: " + channelLow + " channelHigh: " + channelHigh + " channelSize: " + channelSize);
				int endRectangle = 0;
				if (BarsPeriod.Id == PeriodType.Minute) {
					endRectangle = 240/BarsPeriod.Value;
				}			
				DrawRectangle(CurrentBar + "rectangle", checkPeriod, channelLow, -endRectangle, channelHigh, Color.DarkBlue);
				
				DrawLine(CurrentBar + "SHigh", 60, sessionHigh, 0, sessionHigh, Color.Green);
				DrawLine(CurrentBar + "SLow", 60, sessionLow, 0, sessionLow, Color.Red);
			}
			return adjustedChannelSize;
		}
		
		
		protected override void OnExecution(IExecution execution)
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			if (execution.Order == null)
			{
				Print(Time + " -->> OnExecution.Order is null");
				return;
			}
			
			Print(Time + " execution.Order: " + execution.Order.ToString());
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (entryOrderLong != null && entryOrderLong == execution.Order)
			{
				//Print(Time + " entryOrderLong: " + entryOrderLong);
				if (closeOrderLongLimit == null) 
				{
					// the first time this is called (1st trade) it will shrink the channel biased to 1st entry side
					
					limitPrice = 0;
					stopPrice = channelLow - 20 * TickSize;
					//stopPrice = channelLow - 1 * TickSize;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderLongStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Stop");
					
					stopPrice = 0; 
					breakEvenPrice = entryOrderLong.AvgFillPrice + Math.Abs(profitPoints/entryOrderLong.Quantity); 
					DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);
					limitPrice = breakEvenPrice + (adjustedChannelSize + 2) * TickSize;
					//if (adjustedChannelSize > 35) limitPrice = limitPrice * 0.70;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrderLongLimit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Limit");
					
					//DrawText(CurrentBar + "stopText", "" + stopPrice, 0, stopPrice, Color.White);
					//Print(Time + " channelLow: " + channelLow + " channelHigh: " + channelHigh + " adjustedChannelSize: " + adjustedChannelSize);
					
					barNumberSinceFilled = CurrentBar;
				} 
				else
				{
					Print(Time + " OnExecution has been fired and stop and limits for long have already been submitted");
					//Print(Time + " closeOrderLongStop: " + closeOrderLongStop);
					//Print(Time + " closeOrderLongLimit: " + closeOrderLongLimit);
				}
			} 

			// ============================================
			// New short order placed, now set stops/limits
			// ============================================
			if (entryOrderShort != null && entryOrderShort == execution.Order)
			{
				//Print(Time + " entryOrderShort: " + entryOrderShort);
				if (closeOrderShortLimit == null) 
				{
					// the first time this is called (1st trade) it will shrink the channel biased to 1st entry side
					
					limitPrice = 0;
					stopPrice = channelHigh + 20 * TickSize;
					//stopPrice = channelHigh + 1 * TickSize;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderShortStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Stop");
					
					stopPrice = 0; 
					breakEvenPrice = entryOrderShort.AvgFillPrice - Math.Abs(profitPoints/entryOrderShort.Quantity);
					DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);
					limitPrice = breakEvenPrice - (adjustedChannelSize + 2) * TickSize;
					//if (adjustedChannelSize > 35) limitPrice = limitPrice * 0.70;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrderShortLimit = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Limit");
					
					//DrawText(CurrentBar + "stopText", "" + stopPrice, 0, stopPrice, Color.White);
					//Print(Time + " stopPrice: " + stopPrice + " adjustedChannelSize: " + adjustedChannelSize);

					barNumberSinceFilled = CurrentBar;
				} 
				else
				{
					Print(Time + " OnExecution has been fired and stop and limits for Short have already been submitted");
					//Print(Time + " closeOrderShortStop: " + closeOrderShortStop);
					//Print(Time + " closeOrderShortLimit: " + closeOrderShortLimit);
				}
			} 

			// ===================================================
			//   Trade hit Long stop or reversal close, create reversed trade Short
			// ===================================================
			if ((closeOrderLongStop != null || closeOrderLong != null) 
				&& (closeOrderLongStop == execution.Order || closeOrderLong == execution.Order))
			{
				int closeQty = execution.Quantity;
				
				if (closeOrderLongStop == execution.Order)
				{
					//closeQty = closeOrderLongStop.Quantity;
					profitPoints += (-0.005 + closeQty * (closeOrderLongStop.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				else if (closeOrderLong == execution.Order)
				{
					//closeQty = closeOrderLong.Quantity;
					profitPoints += (-0.005 + closeQty * (closeOrderLong.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				
				Print(Time + " Long Close Profit: " + profitPoints);
				
				Print("------------------------");
				entryOrderLong = null;
				entryOrderShort = null;
				closeOrderShortStop = null;
				closeOrderShortLimit = null;
				closeOrderLongStop = null;
				closeOrderLong = null;
				tradeCount++;
				
				if (tradeCount <= maxReversals
					//&& ToTime(Time[0]) < (ToTime(9, 30, 0))
					) 
				{
					// Reverse to Short order
					
					/// TODO - Potentially need to not make next trade if the total risk is too high

					stopPrice = 0;
					limitPrice = channelLow - 1 * TickSize;

					entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.Limit, 2 * closeQty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Short");
					DrawDiamond(CurrentBar + "limit", false, 0, limitPrice, Color.Aquamarine);

					// TODO - may want to set stop here too, and revise after it's filled
				}
			}			
			
			// ===================================================
			//   Trade hit Long limit, no more trades for the day
			// ===================================================
			if (closeOrderLongLimit != null && closeOrderLongLimit == execution.Order)
			{
				profitPoints += (-0.005 + closeOrderLongLimit.Quantity * (closeOrderLongLimit.AvgFillPrice - entryOrderLong.AvgFillPrice));
				Print(Time + " closeOrderLongLimit tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				ResetTrade();
			}			

			// ==================================================
			//   Trade hit Short stop or reversal, create reversed trade Long
			// ==================================================
			if ((closeOrderShortStop != null || closeOrderShort != execution.Order)
				&& (execution.Order == closeOrderShortStop || execution.Order == closeOrderShort))
			{
				int closeQty = execution.Quantity;
				
				if (closeOrderShortStop == execution.Order)
				{
				//	closeQty = closeOrderShortStop.Quantity;
					profitPoints += (-0.005 + closeQty * (entryOrderShort.AvgFillPrice - closeOrderShortStop.AvgFillPrice));
				} 
				else if (closeOrderShort == execution.Order)
				{
				//	closeQty = closeOrderShort.Quantity;
					profitPoints += (-0.005 + closeQty * (entryOrderShort.AvgFillPrice - closeOrderShort.AvgFillPrice));
				}
				
				Print(Time + " Short Close Profit: " + profitPoints);

				Print("------------------------");
				entryOrderLong = null;
				entryOrderShort = null;
				closeOrderLongStop = null;
				closeOrderLongLimit = null;
				closeOrderLongStop = null;
				closeOrderLong = null;
				closeOrderShortStop = null;
				closeOrderShort = null;
				
				tradeCount++;

				if (tradeCount <= maxReversals
					//&& ToTime(Time[0]) < (ToTime(9, 30, 0)) 
					)
				{
					// Reverse to Long order
					stopPrice = 0;
					limitPrice = channelHigh + 1 * TickSize;
					
					Print(Time + " -- Submitting a reversal trade long market");
					entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, 2 * closeQty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Long");
					DrawDiamond(CurrentBar + "limit", false, 0, limitPrice, Color.Green);
						
						
					// may want to set stop here too, and revise after it's filled
				}
			}			
			
			// ===================================================
			//   Trade hit Short limit, no more trades for the day
			// ===================================================
			if (closeOrderShortLimit != null && closeOrderShortLimit == execution.Order)
			{
				profitPoints += (-0.005 + closeOrderShortLimit.Quantity * (entryOrderShort.AvgFillPrice - closeOrderShortLimit.AvgFillPrice));
				Print(Time + " closeOrderShortLimit tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				ResetTrade();
			}			
		}
		
		private void ResetTrade() {
			
			Print("------------------------");
			entryOrderLong = null;
			entryOrderShort = null;

			closeOrderLongStop = null;
			closeOrderLongLimit = null;
			closeOrderShortStop = null;
			closeOrderShortLimit = null;
			closeOrderLong = null;
			closeOrderShort = null;
		
			//adjustedChannelSize = 0;
		}
		
        #region Properties
		
        [Description("Enables the display of summary information, best turn off during backtesting")]
        [GridCategory("Parameters")]
        public bool EnableSummary
        {
            get { return enableSummary; }
            set { enableSummary = value; }
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
		
        [Description("Minimum Channel Size in ticks")]
        [GridCategory("Parameters")]
        public double StdDevMax
        {
            get { return stdDevMax; }
            set { stdDevMax = value; }
        }
		
        [Description("Maximum Channel Size in ticks")]
        [GridCategory("Parameters")]
        public int ChannelMax
        {
            get { return channelMax; }
            set { channelMax = Math.Min(100, value); }
        }
		
		
        [Description("The number of bars to include before opening pit session to build the range")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
        [Description("Period of MA trend filter - 0 means to disable filter")]
        [GridCategory("Parameters")]
        public int MaPeriod
        {
            get { return maPeriod; }
            set { maPeriod = Math.Max(0, value); }
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
		
        [Description("The number of reversals before closing trade")]
        [GridCategory("Parameters")]
        public int MaxReversals
        {
            get { return maxReversals; }
            set { maxReversals = Math.Max(0, value); }
        }		
		
        [Description("On the last trade, close at break even")]
        [GridCategory("Parameters")]
        public bool BreakEvenLast
        {
            get { return breakEvenLast; }
            set { breakEvenLast = value; }
        }
		
        [Description("Enables extending channel to reach session high or lows, 0 ==  false, 1 == true")]
        [GridCategory("Parameters")]
        public int SessionHighLow
        {
            get { return sessionHighLow; }
            set { sessionHighLow = value; }
        }
		
        [Description("On the last trade, if BreakEventLast, trail from day b/e")]
        [GridCategory("Parameters")]
        public bool UseTrailing
        {
            get { return useTrailing; }
            set { useTrailing = value; }
        }
		

		
		// ===========================================================
		// Begin ATM Stop Strategy Settings
		// ===========================================================
		
		/*
        [Description("Auto Break Even Profit Trigger")]
        [GridCategory("Parameters")]
        public int AbeProfitTrigger
        {
            get { return abeProfitTrigger; }
            set { abeProfitTrigger = Math.Max(1, value); }
        }
        [Description("Profit Target")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0, value); }
        }

        [Description("Start Order Quantity")]
        [GridCategory("Parameters")]
        public int StartQty
        {
            get { return startQty; }
            set { startQty = Math.Max(1, value); }
        }

        [Description("Stop Loss")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
        }

        [Description("Auto trail Stop loss")]
        [GridCategory("Parameters")]
        public int AtStopLoss1
        {
            get { return atStopLoss1; }
            set { atStopLoss1 = Math.Max(1, value); }
        }

        [Description("Auto trail Frequency")]
        [GridCategory("Parameters")]
        public int AtFrequency1
        {
            get { return atFrequency1; }
            set { atFrequency1 = Math.Max(1, value); }
        }

        [Description("Auto trail Profit Trigger")]
        [GridCategory("Parameters")]
        public int AtProfitTrigger1
        {
            get { return atProfitTrigger1; }
            set { atProfitTrigger1 = Math.Max(0, value); }
        }

        [Description("Auto trail Stop loss")]
        [GridCategory("Parameters")]
        public int AtStopLoss2
        {
            get { return atStopLoss2; }
            set { atStopLoss2 = Math.Max(1, value); }
        }

        [Description("Auto trail Frequency")]
        [GridCategory("Parameters")]
        public int AtFrequency2
        {
            get { return atFrequency2; }
            set { atFrequency2 = Math.Max(1, value); }
        }

        [Description("Auto trail Profit Trigger")]
        [GridCategory("Parameters")]
        public int AtProfitTrigger2
        {
            get { return atProfitTrigger2; }
            set { atProfitTrigger2 = Math.Max(0, value); }
        }

        [Description("Auto trail Stop loss")]
        [GridCategory("Parameters")]
        public int AtStopLoss3
        {
            get { return atStopLoss3; }
            set { atStopLoss3 = Math.Max(1, value); }
        }

        [Description("Auto trail Frequency")]
        [GridCategory("Parameters")]
        public int AtFrequency3
        {
            get { return atFrequency3; }
            set { atFrequency3 = Math.Max(1, value); }
        }

        [Description("Auto trail Profit Trigger")]
        [GridCategory("Parameters")]
        public int AtProfitTrigger3
        {
            get { return atProfitTrigger3; }
            set { atProfitTrigger3 = Math.Max(0, value); }
        }
		*/
		// ===========================================================
		// End ATM Stop Strategy Settings
		// ===========================================================
		
        #endregion
    }
}
