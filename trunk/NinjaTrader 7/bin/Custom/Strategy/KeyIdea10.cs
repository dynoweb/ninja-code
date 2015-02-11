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


#region Notes
/*
 This strategy hasn't been written, the code is a cut from KeyIdea09.cs
 
 The idea of KeyIdea10 is to use the VHF indicator to detect consolidation periods.  Upon breakout from the consolidation
 entry trade either scapling a few ticks or staying in longer.  Two things can be considered to determine if
 you're staying in the trade, 

   1st - Was high volume breaking out of consolidation, if so, usually there's follow-thru
   2nd - If there wasn't high volume on the break out, just scalpe for a few ticks

	It may be possible to look for decreased vol to know when it's time to leave the trade or
    look at the VHF plot for it to fall.

*/
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
	/// This strategy establishes 3 ETF positions based on TrendStregth.  It appears to use about 
	/// a 15% trailing stop and three exit levels.
	/// 
	/// Use SSO as the default ETF
	/// 
    /// Used to test new key trading ideas - this should be used only as a template.
	/// 
	/// Step 1: KeyIdea or trade idea 
	/// 		pseudo code entry
	/// 		example entry: if close > highest(close, x) then buy next bar at market
	/// 		exit: reverse position when entry reverses
	/// 		   or technical-based - support/resistance, moving average 
	///                 needs to work with entry to not give conflicting signals
	///            break even stops, stop-loss, profit targets, trailing stops 
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
	/// 	limited testing - check if there's any merit to the entry idea
	/// 		test against only 10-20% of data
	/// 			Entry Testing
	/// 				usefulness test - without commission/slippage goal > 52% profitable
	/// 					Fixed-Stop and Target Exit
	/// 						SetProfitTarget
	/// 						SetStopLoss
	/// 					Fixed-Bar Exit
	/// 					Random Exit
	/// 	
    /// </summary>
    [Description("ES 1 min")]
    public class KeyIdea10 : Strategy
    {
        #region Variables
        // Wizard generated variables
		private int atrBucket = 3;
		
		private int entryB1 = 2;  // Buy x ticks pullback
		private int entryB2 = 1;  // Buy x ticks pullback
		private int entryB3 = 1;  // Buy x ticks pullback

		private int entryS1 = 2;  // Sell x ticks pullback
		private int entryS2 = 1;  // Sell x ticks pullback
		private int entryS3 = 1;  // Sell x ticks pullback

		private int iparm1 = 6; // Default setting for Target  
        private int iparm2 = 6; // Default setting for Stop
        private int iparm3 = 100; // Default setting for Iparm2

		private bool optimizedTargets = true;
		
        private int period1 = 30; // Default setting for Period1
        private int period2 = 30; // Default setting for Period2
        private int period3 = 15; // Default setting for Period2

		private int startTime = 0;  // 850 // start of trading hhmm
		private int stopTime = 2400;  // 1230 // end of trading hhmm
		
		int qty1 = 1;
		int qty2 = 0;
		int qty3 = 0;

		// User defined variables (add any user defined variables below)
		int target = 0;
		int stop = 0;
		
		IOrder buyOrder1 = null;
		IOrder buyOrder2 = null;
		IOrder buyOrder3 = null;

		IOrder sellOrder1 = null;
		IOrder sellOrder2 = null;
		IOrder sellOrder3 = null;

		IOrder closeLongOrderLimit1 = null;
		IOrder closeLongOrderLimit2 = null;
		IOrder closeLongOrderLimit3 = null;
		
		IOrder closeLongOrderStop1 = null;
		IOrder closeLongOrderStop2 = null;
		IOrder closeLongOrderStop3 = null;
		
		IOrder closeLongOrder1 = null;
		IOrder closeLongOrder2 = null;
		IOrder closeLongOrder3 = null;
		
		IOrder closeShortOrderLimit1 = null;
		IOrder closeShortOrderLimit2 = null;
		IOrder closeShortOrderLimit3 = null;
		
		IOrder closeShortOrderStop1 = null;
		IOrder closeShortOrderStop2 = null;
		IOrder closeShortOrderStop3 = null;
		
		IOrder closeShortOrder1 = null;
		IOrder closeShortOrder2 = null;
		IOrder closeShortOrder3 = null;
		
		double limitPrice = 0;
		double stopPrice = 0;
		
		string orderPrefix = "KI10"; //"KeyIdea10";
		
		// ATRTrailing parms
//		ATRTrailing atr = null;
//		double atrTimes = 3.0; 
//		int atrPeriod = 10; 
//		//double ratched = 0.00;
//		
//		//  KeltnerChannel parms
//		KeltnerChannel kc = null;
//		double offsetMultiplier = 1.5;
//		int keltnerPeriod = 10; 
//		
//		// DonchianChannel parms
//		DonchianChannel dc = null;
//		int donchianPeriod = 20;
		
		// VHF
		double threshold = 0.35;
		int consolidationStartBar;
		int consolidationEndBar;
		double consolidationLow = 0;
		double consolidationHigh = 0;
		
		ATR atr = null;
		HMARick hma = null;
		VHF vhf = null; 
		
        #endregion

        /// <summary>
        //
		/// 
		/// 
		///   Code base currently is just a copy from another idea
		/// 
		///  
		///  
        /// </summary>
        protected override void Initialize()
        {
			// Indicator Setup
			// ---------------
			hma = HMARick(Period1, 200);
			hma.PaintPriceMarkers = false;			
			Add(hma);
			
			vhf = VHF(Period2, threshold);
			Add(vhf);
			
			Add(PitColor(Color.Black, 83000, 25, 161500));
			
			atr = ATR(10);
			//Add(atr);
			
//			atr = ATRTrailing(atrTimes, Period1, Ratched);
//			Add(atr);
//			
//			kc = KeltnerChannel(offsetMultiplier, keltnerPeriod);
//			Add(kc);
//			
//			dc = DonchianChannel(donchianPeriod);
//			dc.Displacement = 2;
//			dc.PaintPriceMarkers = false;
//			Add(dc);
			
    		Unmanaged = true;				// Use unmanaged order methods
			// Methods BarsSinceEntry() and BarsSinceExit() are usuable in Unmanaged orders

			// Managed Properties
			// --------------------
			//EntriesPerDirection = 2;
			//EntryHandling = EntryHandling.UniqueEntries;
            //SetProfitTarget("", CalculationMode.Percent, dparm2);
            //SetStopLoss("", CalculationMode.Percent, dparm2, false);
            //SetTrailStop("", CalculationMode.Percent, dparm2, false);
			
			//Slippage = 2;
			BarsRequired = 22;
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
			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				if (TraceOrders == true)
					Print(Time + " =======================");
				ResetTrades();
			}
			

			if ((ToTime(Time[0]) < StartTime * 100) || (ToTime(Time[0]) > StopTime * 100)
				//|| (atr[0] < AtrBucket * 0.5 || atr[0] > (AtrBucket * 0.5 + 0.5))
				//|| (atr[0] < 0.5 || atr[0] > 6)
				)
			{
				ShutDownOrders();
				return;
			}
			
//			if (OptimizedTargets)
//			{
//				int bucket = (int) Math.Truncate(atr[0]/0.5);
//				switch (bucket) {
//					case 0: // not trading	
//						target = iparm1;
//						stop = iparm2;
//						break;
//					case 1:
//						target = 3;
//						stop = 6;
//						break;
//					case 2:
//						target = 3;
//						stop = 8;
//						break;
//					case 3:
//						target = 3;
//						stop = 12;
//						break;
//					case 4:
//						target = 4;
//						stop = 12;
//						break;
//					default:
//						target = iparm1;
//						stop = iparm2;
//						break;
//				}
//			} 
//			else
//			{
				target = iparm1;
				stop = iparm2;
//			}
			
			// No Trade zone
//			if ((ToTime(Time[0]) >= 800 * 100) && (ToTime(Time[0]) <= 900 * 100))
//			{
//				CancelWorkingOrders();
//				return;		// no trade time
//			}

			// Doing a little clean up here, if this happens, I really should find out where
			// it's not cleaning up the order
			if (buyOrder1 != null)
			{
				if (TraceOrders == true)
				{
					if (buyOrder1.OrderState != OrderState.Working)
						Print(Time + " OrderState: " + buyOrder1.OrderState);
				}
				if (buyOrder1.OrderState == OrderState.Cancelled) 
					buyOrder1 = null;
			}
			
			if (vhf.VHFLine[0] < threshold) //  possible onsolidation?
			{
				// consolidation found
				if (consolidationStartBar == 0) 
				{
					consolidationStartBar = CurrentBar;
				}
				else
				{
					consolidationLow = Low[LowestBar(Low, CurrentBar - consolidationStartBar + 1)];
					consolidationHigh = High[HighestBar(High, CurrentBar - consolidationStartBar + 1)];
					DrawRectangle(consolidationStartBar + "", false, CurrentBar-consolidationStartBar, consolidationLow, 0, 
						consolidationHigh, Color.PaleGreen, Color.PaleGreen, 2);
					consolidationEndBar = CurrentBar;
				}
			} 
			else
			{
				consolidationStartBar = 0;
			}
			
			
			// ============================================
			// New long order placement
			// ============================================
			if ((((CurrentBar - consolidationEndBar) == 0) || (CurrentBar - consolidationEndBar) == 1)
				&& (consolidationEndBar - consolidationEndBar) > 4
				&& sellOrder1 == null) //  uptrend
			{
				
				if (Qty1 > 0)
				{
					limitPrice = consolidationHigh + EntryB1 * TickSize;
					stopPrice = limitPrice;
					
					if (buyOrder1 != null && buyOrder1.OrderState == OrderState.Working)
					{
						//DrawDot(CurrentBar + "b1", false, 0, limitPrice, Color.LightGray);
						ChangeOrder(buyOrder1, buyOrder1.Quantity, limitPrice, stopPrice);
						if (TraceOrders == true)
							Print(Time + " changeOrder - limitPrice: " + limitPrice);
					}
					else if (buyOrder1 == null)
					{
						//DrawDot(CurrentBar + "b1", false, 0, limitPrice, Color.DarkGray);
						buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, Qty1, limitPrice, stopPrice, orderPrefix + "oco1", "B1");
						if (TraceOrders == true)
							Print(Time + " submitOrder: " + buyOrder1);
					}
					else
					{
						if (TraceOrders == true)
							Print(Time + " OrderState: " + buyOrder1.OrderState);
					}
				}
	
				// Second Long Contract 
				if (Qty2 > 0)
				{
					limitPrice = Low[0] - (EntryB1 + EntryB2) * TickSize;
					stopPrice = limitPrice;
					if (buyOrder2 != null && buyOrder2.OrderState == OrderState.Working)
					{
						ChangeOrder(buyOrder2, buyOrder2.Quantity, limitPrice, stopPrice);
					}
					else if (buyOrder2 == null)
					{
						buyOrder2 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, Qty2, limitPrice, stopPrice, orderPrefix + "oco2", "B2");
					}
				}
				
				// Third Long Contract 
				if (Qty3 > 0)
				{
					limitPrice = Low[0] - (EntryB1 + EntryB2 + EntryB3) * TickSize;
					stopPrice = limitPrice;
					if (buyOrder3 != null && buyOrder3.OrderState == OrderState.Working)
					{
						ChangeOrder(buyOrder3, buyOrder3.Quantity, limitPrice, stopPrice);
					}
					else if (buyOrder3 == null)
					{
						buyOrder3 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, Qty3, limitPrice, stopPrice, orderPrefix + "oco3", "B3");
					}
				}
				
			}
			
			// ============================================
			// New Short order placement
			// ============================================
			if (hma.TrendSet[0] == -1 && buyOrder1 == null) //  downtrend
			{
				// First Short Contract 
				if (Qty1 > 0)
				{
					limitPrice = High[0] + EntryS1 * TickSize;
					stopPrice = limitPrice;
					
					if (sellOrder1 != null && sellOrder1.OrderState == OrderState.Working)
					{
						//DrawDot(CurrentBar + "s1", false, 0, limitPrice, Color.LightBlue);
						ChangeOrder(sellOrder1, sellOrder1.Quantity, limitPrice, stopPrice);
					}
					else if (sellOrder1 == null)
					{
						//DrawDot(CurrentBar + "s1", false, 0, limitPrice, Color.DarkBlue);
						sellOrder1 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, Qty1, limitPrice, stopPrice, orderPrefix + "oco1", "S1");
					}
				}
				
				// Second Short Contract 
				if (Qty2 > 0)
				{
					limitPrice = High[0] + (EntryS1 + EntryS2) * TickSize;
					stopPrice = limitPrice;
					
					if (sellOrder2 != null && sellOrder2.OrderState == OrderState.Working)
					{
						ChangeOrder(sellOrder2, sellOrder2.Quantity, limitPrice, stopPrice);
					}
					else if (sellOrder2 == null)
					{
						sellOrder2 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, Qty2, limitPrice, stopPrice, orderPrefix + "oco2", "S2");
					}
				}
				
				// Third Short Contract 
				if (Qty3 > 0)
				{
					limitPrice = High[0] + (EntryS1 + EntryS2 + EntryS3) * TickSize;
					stopPrice = limitPrice;
					
					if (sellOrder3 != null && sellOrder3.OrderState == OrderState.Working)
					{
						ChangeOrder(sellOrder3, sellOrder3.Quantity, limitPrice, stopPrice);
					}
					else if (sellOrder3 == null)
					{
						sellOrder3 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, Qty3, limitPrice, stopPrice, orderPrefix + "oco3", "S3");
					}
				}
			}
			
			// ============================================
			// Market Flat, cancel working orders
			// ============================================
			if (hma.TrendSet[0] == 0) // consolidation
			{
				if (buyOrder1 != null && buyOrder1.OrderState == OrderState.Working)
				{
					CancelOrder(buyOrder1);
					buyOrder1 = null;
				}
				if (buyOrder2 != null && buyOrder2.OrderState == OrderState.Working)
				{
					CancelOrder(buyOrder2);
					buyOrder2 = null;
				}
				if (buyOrder3 != null && buyOrder3.OrderState == OrderState.Working)
				{
					CancelOrder(buyOrder3);
					buyOrder3 = null;
				}
				if (sellOrder1 != null && sellOrder1.OrderState == OrderState.Working)
				{
					CancelOrder(sellOrder1);
					sellOrder1 = null;
				}
				if (sellOrder2 != null && sellOrder2.OrderState == OrderState.Working)
				{
					CancelOrder(sellOrder2);
					sellOrder2 = null;
				}
				if (sellOrder3 != null && sellOrder3.OrderState == OrderState.Working)
				{
					CancelOrder(sellOrder3);
					sellOrder3 = null;
				}
			}
				
		}

		
		protected override void OnExecution(IExecution execution)
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			if (execution.Order == null)
			{
				Print(Time + " -->> OnExecution.Order is null");
				ResetTrades();
				Print(Time + " executed/reset on close");
				return;
			}
			
			if (TraceOrders)
			{
				Print(Time + " execution: " + execution.ToString());
				Print(Time + " execution.Order: " + execution.Order.ToString());
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
					stopPrice = buyOrder1.AvgFillPrice - stop * TickSize;
					//DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeLongOrderStop1 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CSB1");

					stopPrice = 0;
					limitPrice = buyOrder1.AvgFillPrice + target * TickSize;
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeLongOrderLimit1 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CLB1");
				} 
			} 

			if (buyOrder2 != null && buyOrder2 == execution.Order)
			{
				if (closeLongOrderLimit2 == null) 
				{
					limitPrice = 0;
					stopPrice = buyOrder2.AvgFillPrice - (stop - EntryS2) * TickSize;
					closeLongOrderStop2 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB2", "CSB2");

					stopPrice = 0;
					limitPrice = buyOrder2.AvgFillPrice + (target + EntryS2) * TickSize;
					closeLongOrderLimit2 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB2", "CLB2");
				} 
			} 

			if (buyOrder3 != null && buyOrder3 == execution.Order)
			{
				if (closeLongOrderLimit3 == null) 
				{
					limitPrice = 0;
					stopPrice = buyOrder3.AvgFillPrice - (stop - EntryS2 - EntryS3) * TickSize;
					closeLongOrderStop3 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB3", "CSB3");

					stopPrice = 0;
					limitPrice = buyOrder3.AvgFillPrice + (target + EntryS2 + EntryS3) * TickSize;
					closeLongOrderLimit3 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB3", "CLB3");
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
					stopPrice = sellOrder1.AvgFillPrice + stop * TickSize;
					//DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeLongOrderStop1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS1", "CSS1");

					stopPrice = 0;
					limitPrice = sellOrder1.AvgFillPrice - target * TickSize;
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeShortOrderLimit1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS1", "CLS1");
				} 
			} 

			if (sellOrder2 != null && sellOrder2 == execution.Order)
			{
				if (closeShortOrderLimit2 == null) 
				{
					limitPrice = 0;
					stopPrice = sellOrder2.AvgFillPrice + (stop - EntryS2) * TickSize;
					closeLongOrderStop2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS2", "CSS2");

					stopPrice = 0;
					limitPrice = sellOrder2.AvgFillPrice - (target + EntryS2) * TickSize;
					closeShortOrderLimit2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS2", "CLS2");
				} 
			} 

			if (sellOrder3 != null && sellOrder3 == execution.Order)
			{
				if (closeShortOrderLimit3 == null) 
				{
					limitPrice = 0;
					stopPrice = sellOrder3.AvgFillPrice + (stop - EntryS2 - EntryS3) * TickSize;
					closeLongOrderStop3 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS3", "CSS3");

					stopPrice = 0;
					limitPrice = sellOrder3.AvgFillPrice - (target + EntryS2 + EntryS3) * TickSize;
					closeShortOrderLimit3 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS3", "CLS3");
				} 
			} 

			
			// ===================================================
			//   Trade hit Long limit, no more trades for the day
			// ===================================================
			if (closeLongOrderLimit1 != null && closeLongOrderLimit1 == execution.Order)
				ResetTrade1();
			if (closeLongOrderStop1 != null && closeLongOrderStop1 == execution.Order)
				ResetTrade1();
			if (closeLongOrder1 != null && closeLongOrder1 == execution.Order)
				ResetTrade1();

			if (closeLongOrderLimit2 != null && closeLongOrderLimit2 == execution.Order)
				ResetTrade2();
			if (closeLongOrderStop2 != null && closeLongOrderStop2 == execution.Order)
				ResetTrade2();
			if (closeLongOrder2 != null && closeLongOrder2 == execution.Order)
				ResetTrade2();

			if (closeLongOrderLimit3 != null && closeLongOrderLimit3 == execution.Order)
				ResetTrade3();
			if (closeLongOrderStop3 != null && closeLongOrderStop3 == execution.Order)
				ResetTrade3();
			if (closeLongOrder3 != null && closeLongOrder3 == execution.Order)
				ResetTrade3();

			
			if (closeShortOrderLimit1 != null && closeShortOrderLimit1 == execution.Order)
				ResetTrade1();
			if (closeShortOrderStop1 != null && closeShortOrderStop1 == execution.Order)
				ResetTrade1();
			if (closeShortOrder1 != null && closeShortOrder1 == execution.Order)
				ResetTrade1();

			if (closeShortOrderLimit2 != null && closeShortOrderLimit2 == execution.Order)
				ResetTrade2();
			if (closeShortOrderStop2 != null && closeShortOrderStop2 == execution.Order)
				ResetTrade2();
			if (closeShortOrder2 != null && closeShortOrder2 == execution.Order)
				ResetTrade2();

			if (closeShortOrderLimit3 != null && closeShortOrderLimit3 == execution.Order)
				ResetTrade3();
			if (closeShortOrderStop3 != null && closeShortOrderStop3 == execution.Order)
				ResetTrade3();
			if (closeShortOrder3 != null && closeShortOrder3 == execution.Order)
				ResetTrade3();

		}

		private void ShutDownOrders()
		{
			if (buyOrder1 != null)
			{
//				if (TraceOrders == true)
//					Print(Time + " off hours buyOrder1: " + buyOrder1);
				if (buyOrder1.OrderState == OrderState.Filled && closeLongOrder1 != null)
				{
//					if (TraceOrders == true)
//						Print(Time + " delayed closing ");
					if (ToTime(Time[0]) > (StopTime + 15) * 100)	// wait 15 more min before closing
					{
						closeLongOrder1 = SubmitOrder(0, OrderAction.Sell, OrderType.Market, buyOrder1.Quantity, 
							limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CTB1");
					}
				}
				else if (buyOrder1.OrderState == OrderState.Working)
				{
//					if (TraceOrders == true)
//						Print(Time + " Cancelling off hours working order: " + buyOrder1);
					CancelOrder(buyOrder1);
					buyOrder1 = null;
				} 
//				else 
//				{
//					if (TraceOrders == true)
//						Print(Time + " else " + buyOrder1);
//				}
			}
			
			if (buyOrder2 != null)
			{
				if (buyOrder2.OrderState == OrderState.Filled && closeLongOrder2 != null)
				{
					if (ToTime(Time[0]) > (StopTime + 15) * 100)	// wait 15 more min before closing
					{
						closeLongOrder2 = SubmitOrder(0, OrderAction.Sell, OrderType.Market, buyOrder2.Quantity, 
							limitPrice, stopPrice, orderPrefix + "ocoCloseB2", "CTB2");
					}
				}
				else if (buyOrder2.OrderState == OrderState.Working)
				{
					CancelOrder(buyOrder2);
					buyOrder2 = null;
				} 
			}
			
			if (buyOrder3 != null)
			{
				if (buyOrder3.OrderState == OrderState.Filled && closeLongOrder3 != null)
				{
					if (ToTime(Time[0]) > (StopTime + 15) * 100)	// wait 15 more min before closing
					{
						closeLongOrder3 = SubmitOrder(0, OrderAction.Sell, OrderType.Market, buyOrder3.Quantity, 
							limitPrice, stopPrice, orderPrefix + "ocoCloseB3", "CTB3");
					}
				}
				else if (buyOrder3.OrderState == OrderState.Working)
				{
					CancelOrder(buyOrder3);
					buyOrder3 = null;
				} 
			}

			
			if (sellOrder1 != null)
			{
				if (sellOrder1.OrderState == OrderState.Filled && closeLongOrder1 != null)
				{
					if (ToTime(Time[0]) > (StopTime + 15) * 100)	// wait 15 more min before closing
					{
						closeLongOrder1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, sellOrder1.Quantity, 
							limitPrice, stopPrice, orderPrefix + "ocoCloseS1", "CTS1");
					}
				}
				else if (sellOrder1.OrderState == OrderState.Working)
				{
					CancelOrder(sellOrder1);
					sellOrder1 = null;
				} 
			}
			
			if (sellOrder2 != null)
			{
				if (sellOrder2.OrderState == OrderState.Filled && closeLongOrder2 != null)
				{
					if (ToTime(Time[0]) > (StopTime + 15) * 100)	// wait 15 more min before closing
					{
						closeLongOrder2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, sellOrder2.Quantity, 
							limitPrice, stopPrice, orderPrefix + "ocoCloseS2", "CTS2");
					}
				}
				else if (sellOrder2.OrderState == OrderState.Working)
				{
					CancelOrder(sellOrder2);
					sellOrder2 = null;
				} 
			}
			
			if (sellOrder3 != null)
			{
				if (sellOrder3.OrderState == OrderState.Filled && closeLongOrder3 != null)
				{
					if (ToTime(Time[0]) > (StopTime + 15) * 100)	// wait 15 more min before closing
					{
						closeLongOrder3 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, sellOrder3.Quantity, 
							limitPrice, stopPrice, orderPrefix + "ocoCloseS3", "CTS3");
					}
				}
				else if (sellOrder3.OrderState == OrderState.Working)
				{
					CancelOrder(sellOrder3);
					sellOrder3 = null;
				} 
			}
			
		}
		
		protected override void OnOrderUpdate(IOrder order)
		{
			if (buyOrder1 != null && buyOrder1 == order)
			{
				//Print(order.ToString());
				if (order.OrderState == OrderState.Cancelled)
				{
					buyOrder1 = null;
				}
			}
		}

		
		private void CancelWorkingOrders()
		{
			CancelWorkingOrder(buyOrder1);
			CancelWorkingOrder(buyOrder2);
			CancelWorkingOrder(buyOrder3);
			CancelWorkingOrder(sellOrder1);
			CancelWorkingOrder(sellOrder2);
			CancelWorkingOrder(sellOrder3);
			buyOrder1 = null;
			buyOrder2 = null;
			buyOrder3 = null;
			sellOrder1 = null;
			sellOrder2 = null;
			sellOrder3 = null;
			ResetTrades();
		}
		
		/// <summary>
		/// Cancels the IOrder passed into the method, remember to null out IOrder
		/// </summary>
		/// <param name="order"></param>
		private void CancelWorkingOrder(IOrder order)
		{
			if (order != null && order.OrderState == OrderState.Working)
			{
				CancelOrder(order);
			}
		}
		
		private void ResetTrades()
		{
			ResetTrade1();
			ResetTrade2();
			ResetTrade3();
		}
		
		private void ResetTrade1()
		{
			buyOrder1 = null;
			closeLongOrderLimit1 = null;
			closeLongOrderStop1 = null;
			closeLongOrder1 = null;
			
			sellOrder1 = null;
			closeShortOrderLimit1 = null;
			closeShortOrderStop1 = null;
			closeShortOrder1 = null;
		}
		
		private void ResetTrade2()
		{
			buyOrder2 = null;
			closeLongOrderLimit2 = null;
			closeLongOrderStop2 = null;
			closeLongOrder2 = null;
			
			sellOrder2 = null;
			closeShortOrderLimit2 = null;
			closeShortOrderStop2 = null;
			closeShortOrder2 = null;
		}
		
		private void ResetTrade3()
		{
			buyOrder3 = null;
			closeLongOrderLimit3 = null;
			closeLongOrderStop3 = null;
			closeLongOrder3 = null;
			
			sellOrder3 = null;
			closeShortOrderLimit3 = null;
			closeShortOrderStop3 = null;
			closeShortOrder3 = null;
		}
		
		protected override void OnTermination()
		{
			// Clean up your resources here
			Print("ATR Range used: " + (AtrBucket * 0.5) + " to " + (AtrBucket * 0.5 + 0.5));
		}
 
		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int AtrBucket
        {
            get { return atrBucket; }
            set { atrBucket = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EntryB1
        {
            get { return entryB1; }
            set { entryB1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EntryB2
        {
            get { return entryB2; }
            set { entryB2 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EntryB3
        {
            get { return entryB3; }
            set { entryB3 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EntryS1
        {
            get { return entryS1; }
            set { entryS1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EntryS2
        {
            get { return entryS2; }
            set { entryS2 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EntryS3
        {
            get { return entryS3; }
            set { entryS3 = Math.Max(0, value); }
        }

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
        public int Iparm3
        {
            get { return iparm3; }
            set { iparm3 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public bool OptimizedTargets
        {
            get { return optimizedTargets; }
            set { optimizedTargets = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period1
        {
            get { return period1; }
            set { period1 = Math.Max(-50, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period2
        {
            get { return period2; }
            set { period2 = Math.Max(-50, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period3
        {
            get { return period3; }
            set { period3 = Math.Max(-50, value); }
        }

//        [Description("")]
//        [GridCategory("Parameters")]
//        public double Dparm1
//        {
//            get { return dparm1; }
//            set { dparm1 = Math.Max(0.000, value); }
//        }
//
//        [Description("")]
//        [GridCategory("Parameters")]
//        public double Dparm2
//        {
//            get { return dparm2; }
//            set { dparm2 = Math.Max(0.000, value); }
//        }
//
//        [Description("")]
//        [GridCategory("Parameters")]
//        public double Dparm3
//        {
//            get { return dparm3; }
//            set { dparm3 = Math.Max(0.000, value); }
//        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StartTime
        {
            get { return startTime; }
            set { startTime = Math.Max(0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int StopTime
        {
            get { return stopTime; }
            set { stopTime = Math.Max(0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int Qty1
        {
            get { return qty1; }
            set { qty1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Qty2
        {
            get { return qty2; }
            set { qty2 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Qty3
        {
            get { return qty3; }
            set { qty3 = Math.Max(0, value); }
        }

		
        #endregion

    }
    
}
