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
1 Minute Bars Aggressive Setting
All times are Pacific Time (PST)

Use_Sound FALSE
Flat_on_Startup TRUE
OK_Long TRUE
OK_Short TRUE
Trade_Only_with_SAR TRUE
Slip_on_Exit FALSE
Time_Open 730
Time_EOD 1245
Time_Session_Starts 1500
Flat_Time_1 1700
Flat_Over_1 1715
Flat_Time_2 1700
Flat_Over_2 1715
Qty_1 1
Qty_2 1
Qty_3 1
Bars_to_Hold_Entry 1
Min_Bars_Between_Trades 1
Entry_Tick_B1 2
Entry_Tick_B2 1
Entry_Tick_B3 1
Entry_Tick_S1 2
Entry_Tick_S2 1
Entry_Tick_S3 1
Stop_Loss 500
Profit_Target 90000
Max_Daily_Loss 1000
Max_Daily_Gain 90000
Day_of_Week 7
DTSAR_Back 3
DTSAR_Hold 5
DTSAR_Min_Gap 1
ATOC_Control FALSE
ATOC_Buy_Rate 100
ATOC_Buy_Test 600
ATOC_Sell_Rate 100
ATOC_Sell_Test 650

 

1 Minute Bars Balanced Setting
All times are Pacific Time (PST)

Use_Sound FALSE
Flat_on_Startup TRUE
OK_Long TRUE
OK_Short TRUE
Trade_Only_with_SAR FALSE
Slip_on_Exit FALSE
Time_Open 730
Time_EOD 1245
Time_Session_Starts 1500
Flat_Time_1 1700
Flat_Over_1 1715
Flat_Time_2 1700
Flat_Over_2 1715
Qty_1 1
Qty_2 1
Qty_3 1
Bars_to_Hold_Entry 1
Min_Bars_Between_Trades 1
Entry_Tick_B1 2
Entry_Tick_B2 1
Entry_Tick_B3 1
Entry_Tick_S1 2
Entry_Tick_S2 1
Entry_Tick_S3 1
Stop_Loss 500
Profit_Target 90000
Max_Daily_Loss 1000
Max_Daily_Gain 90000
Day_of_Week 7
DTSAR_Back 3
DTSAR_Hold 5
DTSAR_Min_Gap 1
ATOC_Control TRUE
ATOC_Buy_Rate 100
ATOC_Buy_Test 600
ATOC_Sell_Rate 100
ATOC_Sell_Test 650

 

5 Minute Bars Setting
All times are Pacific Time (PST)

Use_Sound FALSE
Flat_on_Startup TRUE
OK_Long TRUE
OK_Short TRUE
Trade_Only_with_SAR FALSE
Slip_on_Exit FALSE
Time_Open 740
Time_EOD 1130
Time_Session_Starts 1500
Flat_Time_1 1700
Flat_Over_1 1715
Flat_Time_2 1700
Flat_Over_2 1715
Qty_1 1
Qty_2 1
Qty_3 1
Bars_to_Hold_Entry 1
Min_Bars_Between_Trades 1
Entry_Tick_B1 2
Entry_Tick_B2 1
Entry_Tick_B3 1
Entry_Tick_S1 2
Entry_Tick_S2 1
Entry_Tick_S3 1
Stop_Loss 1500
Profit_Target 90000
Max_Daily_Loss 1000
Max_Daily_Gain 90000
Day_of_Week 7
DTSAR_Back 3
DTSAR_Hold 5
DTSAR_Min_Gap 1
ATOC_Control TRUE
ATOC_Buy_Rate 100
ATOC_Buy_Test 825
ATOC_Sell_Rate 100
ATOC_Sell_Test 925
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
    [Description("ES 5 min")]
    public class KeyIdea09 : Strategy
    {
        #region Variables
        // Wizard generated variables
		private int entryB1 = 2;  // Buy x ticks pullback
		private int entryB2 = 1;  // Buy x ticks pullback
		private int entryB3 = 1;  // Buy x ticks pullback

		private int entryS1 = 2;  // Sell x ticks pullback
		private int entryS2 = 1;  // Sell x ticks pullback
		private int entryS3 = 1;  // Sell x ticks pullback

		private double dparm1 = 0.92; // Default setting for Dparm1
        private double dparm2 = 0.82; // Default setting for Dparm2
        private double dparm3 = 0.75; // Default setting for Dparm2
        
		private int iparm1 = 100; // Default setting for Iparm1  
        private int iparm2 = 100; // Default setting for Iparm2
        private int iparm3 = 100; // Default setting for Iparm2

        private int period1 = 30; // Default setting for Period1
        private int period2 = 30; // Default setting for Period2
        private int period3 = 15; // Default setting for Period2

		private int startTime = 730;  // start of trading hhmm
		private int stopTime = 1230;  // end of trading hhmm
		
		private double ratched = 0.68;
		private bool isMo = true;
		private bool is2nd = false;
		
		int qty1 = 1;
		int qty2 = 1;
		int qty3 = 1;

		// User defined variables (add any user defined variables below)
		IOrder buyOrder1 = null;
		IOrder buyOrder2 = null;
		IOrder buyOrder3 = null;

		IOrder sellOrder1 = null;
		IOrder sellOrder2 = null;
		IOrder sellOrder3 = null;

		IOrder closeOrderLimit1 = null;
		IOrder closeOrderLimit2 = null;
		IOrder closeOrderLimit3 = null;
		
		IOrder closeOrderLongStop1 = null;
		IOrder closeOrderLongStop2 = null;
		IOrder closeOrderLongStop3 = null;
		
		IOrder closeOrderLong1 = null;
		IOrder closeOrderLong2 = null;
		IOrder closeOrderLong3 = null;
		
		double limitPrice = 0;
		double stopPrice = 0;
		
		string orderPrefix = "KI9"; //"KeyIdea09";
		
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
		
		HMARick hma = null;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
		/// E-Mini Scalper: Trying to simulate trade2live.com or eminiTradingStrategy.com autotrader
		/// 
		/// Puts on positions on a pull back of a strong trend. 
		///  
		///  
        /// </summary>
        protected override void Initialize()
        {
			hma = HMARick(Period1, 200);
			Add(hma);
			
			Add(PitColor(Color.Black, 83000, 25, 161500));
			
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
			
            //SetProfitTarget("", CalculationMode.Percent, dparm2);
            //SetStopLoss("", CalculationMode.Percent, dparm2, false);
            //SetTrailStop("", CalculationMode.Percent, dparm2, false);
			
			// Use unmanaged order methods
    		Unmanaged = true;

			// set to true to trace orders in the output window, used for debugging, normally set this to false
			TraceOrders = false;
			EntriesPerDirection = 2;
			//EntryHandling = EntryHandling.UniqueEntries;
			BarsRequired = 22;
            CalculateOnBarClose = true;
			ExitOnClose = true;
			IncludeCommission = true;
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
				ResetTrade();
			}
			

			if ((ToTime(Time[0]) < StartTime * 100) || (ToTime(Time[0]) > StopTime * 100))
			{
				if (buyOrder1 != null)
				{
					if (TraceOrders == true)
						Print(Time + " off hours buyOrder1: " + buyOrder1);
					if (buyOrder1.OrderState == OrderState.Filled && closeOrderLong1 == null)
					{
						if (TraceOrders == true)
							Print(Time + " delayed closing ");
						if (ToTime(Time[0]) > (StopTime + 15) * 100)	// wait 15 more min before closing
						{
							closeOrderLong1 = SubmitOrder(0, OrderAction.Sell, OrderType.Market, buyOrder1.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose1", "CTB1");
						}
					}
					else if (buyOrder1.OrderState == OrderState.Working)
					{
						if (TraceOrders == true)
							Print(Time + " Cancelling off hours working order: " + buyOrder1);
						CancelOrder(buyOrder1);
						buyOrder1 = null;
					} 
					else 
					{
						if (TraceOrders == true)
							Print(Time + " else " + buyOrder1);
					}
				}
				return;
			}
			
			// No Trade zone
			if ((ToTime(Time[0]) >= 800 * 100) && (ToTime(Time[0]) <= 900 * 100))
			{
				if (buyOrder1 != null && buyOrder1.OrderState == OrderState.Working)
				{
					Print(Time + " #0 " + buyOrder1);
					CancelOrder(buyOrder1);
					Print(Time + " cancel buyOrder1");
					Print(Time + " #1 " + buyOrder1);
					buyOrder1 = null;
					ResetTrade();
				} 
				if (TraceOrders == true)
				{
					if (buyOrder1 != null && buyOrder1.OrderState != OrderState.Working)
						Print(Time + " #2 " + buyOrder1);
				}
				
				return;		// no trade time
			}
			
			if (hma.TrendSet[0] == 1) //  uptrend
			{
				limitPrice = Low[0] - EntryB1 * TickSize;
				stopPrice = limitPrice;
				
				if (buyOrder1 != null && buyOrder1.OrderState == OrderState.Working)
				{
					DrawDot(CurrentBar + "b1c", false, 0, limitPrice, Color.DarkGray);
					ChangeOrder(buyOrder1, buyOrder1.Quantity, limitPrice, stopPrice);
					if (TraceOrders == true)
						Print(Time + " changeOrder - limitPrice: " + limitPrice);
				}
				else if (buyOrder1 == null)
				{
					DrawDot(CurrentBar + "b1", false, 0, limitPrice, Color.LightGray);
					buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, Qty1, limitPrice, stopPrice, orderPrefix + "oco1", "B1");
					if (TraceOrders == true)
						Print(Time + " submitOrder: " + buyOrder1);
				}
				else
				{
					if (TraceOrders == true)
						Print(Time + " OrderState: " + buyOrder1.OrderState);
				}
			}
			
			if (hma.TrendSet[0] == 0) // consolidation
			{
				if (buyOrder1 != null && buyOrder1.OrderState == OrderState.Working)
				{
					CancelOrder(buyOrder1);
					buyOrder1 = null;
				}
				if (sellOrder1 != null && sellOrder1.OrderState == OrderState.Working)
				{
					CancelOrder(sellOrder1);
					sellOrder1 = null;
				}
			}
				
//				if (hma.TrendSet[0] == -1) //  downtrend
//				{
//					limitPrice = High[0] + EntryS1 * TickSize;
//					stopPrice = limitPrice;
//					
//					if (order1 != null && Position.MarketPosition == MarketPosition.Flat)  //  (order1.OrderState == OrderState.Working || order1.OrderState == OrderState.Cancelled))
//					{
//						DrawDot(CurrentBar + "s1", false, 0, limitPrice, Color.DarkBlue);
//						ChangeOrder(order1, order1.Quantity, limitPrice, stopPrice);
//					}
//					else if (order1 == null)
//					{
//						DrawDot(CurrentBar + "s1", false, 0, limitPrice, Color.LightBlue);
//						order1 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, Qty1, limitPrice, stopPrice, orderPrefix + "oco1", "S1");
//					}
//					else
//						Print(Time + " OrderState: " +order1.OrderState);
//				}
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
				
			}
        //} 


		
		protected override void OnExecution(IExecution execution)
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			if (execution.Order == null)
			{
				Print(Time + " -->> OnExecution.Order is null");
				
				buyOrder1 = null;
				closeOrderLimit1 = null;
				closeOrderLongStop1 = null;
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
				
				if (closeOrderLimit1 == null) 
				{
					limitPrice = 0;
					stopPrice = buyOrder1.AvgFillPrice - 10 * TickSize;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderLongStop1 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose1", "CSB1");

					stopPrice = 0;
					limitPrice = buyOrder1.AvgFillPrice + 5 * TickSize;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrderLimit1 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose1", "CLB1");
				} 
			} 

			if (buyOrder2 != null && buyOrder2 == execution.Order)
			{
				//Print(Time + " buyOrder2: " + buyOrder2);
				if (closeOrderLimit2 == null) 
				{
					stopPrice = 0;
					limitPrice = (buyOrder2.AvgFillPrice/Dparm2) * (1.0 + Period2/100.0);
					DrawDot(CurrentBar + "limitPricex", false, 0, limitPrice, Color.Lime);
					closeOrderLimit2 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose2", "Close Long Limit2");
					
					//barNumberSinceFilled = CurrentBar;
				} 
			} 

			if (buyOrder3 != null && buyOrder3 == execution.Order)
			{
				//Print(Time + " buyOrder3: " + buyOrder3);
				if (closeOrderLimit3 == null) 
				{
					stopPrice = 0;
					limitPrice = (buyOrder3.AvgFillPrice/Dparm3) * (1.0 + Period3/100.0);
					DrawDot(CurrentBar + "limitPricex", false, 0, limitPrice, Color.Lime);
					closeOrderLimit3 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose3", "Close Long Limit3");
					
					//barNumberSinceFilled = CurrentBar;
				} 
			} 

			// ===================================================
			//   Trade hit Long limit, no more trades for the day
			// ===================================================
			if (closeOrderLimit1 != null && closeOrderLimit1 == execution.Order)
			{
				//ResetTrade();
				buyOrder1 = null;
				closeOrderLimit1 = null;
				closeOrderLongStop1 = null;
				Print(Time + " closeOrderLimit1 executed/reset");
			}			
				
			if (closeOrderLongStop1 != null && closeOrderLongStop1 == execution.Order)
			{
				//ResetTrade();
				buyOrder1 = null;
				closeOrderLimit1 = null;
				closeOrderLongStop1 = null;
				Print(Time + " closeOrderLongStop1 executed/reset");
			}			
			
			if (closeOrderLong1 != null && closeOrderLong1 == execution.Order)
			{
				//ResetTrade();
				buyOrder1 = null;
				closeOrderLimit1 = null;
				closeOrderLongStop1 = null;
				closeOrderLong1 = null;
				Print(Time + " closeOrderLong1 executed/reset");
			}			

			if (closeOrderLimit2 != null && closeOrderLimit2 == execution.Order)
			{
				//ResetTrade();
				buyOrder2 = null;
				closeOrderLimit2 = null;
			}			

			if (closeOrderLimit3 != null && closeOrderLimit3 == execution.Order)
			{
				//ResetTrade();
				buyOrder3 = null;
				closeOrderLimit3 = null;
			}			

		}
		
		private void ResetTrade()
		{
			//buyOrder1 = null;
			closeOrderLimit1 = null;
			closeOrderLongStop1 = null;
			closeOrderLong1 = null;
		}
		
        #region Properties
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

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm3
        {
            get { return dparm3; }
            set { dparm3 = Math.Max(0.000, value); }
        }

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
