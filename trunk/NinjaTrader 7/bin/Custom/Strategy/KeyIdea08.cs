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
    [Description("Test SSO Daily trading")]
    public class KeyIdea08 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double dparm1 = 0.92; // Default setting for Dparm1
        private double dparm2 = 0.82; // Default setting for Dparm2
        private double dparm3 = 0.75; // Default setting for Dparm2
        
		private int iparm1 = 100; // Default setting for Iparm1  
        private int iparm2 = 100; // Default setting for Iparm2
        private int iparm3 = 100; // Default setting for Iparm2

        private int period1 = 0; // Default setting for Period1
        private int period2 = 30; // Default setting for Period2
        private int period3 = 15; // Default setting for Period2

		private double ratched = 0.68;
		private bool isMo = true;
		private bool is2nd = false;
        // User defined variables (add any user defined variables below)
		double monthHigh;
		double monthLow;
		IOrder buyOrder1 = null;
		IOrder buyOrder2 = null;
		IOrder buyOrder3 = null;
		IOrder closeOrder1Limit = null;
		IOrder closeOrder2Limit = null;
		IOrder closeOrder3Limit = null;
		
		double limitPrice = 0;
		double stopPrice = 0;
		int qty = 100;
		
		int x = 0;
		int y = 0;
		string orderPrefix = "KI8"; //"KeyIdea08";
		
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
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
		/// Rules:
		///  - if 8% below 22 day high, add to position
		///  - if 16% below 22 day high, add 3x to position
		///  - if 10% profit, close 50%, if 20% close 50% again
        /// </summary>
        protected override void Initialize()
        {
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
			
			// Use Unmanaged order methods
    		Unmanaged = true;

			//EntriesPerDirection = 5;
			//EntryHandling = EntryHandling.UniqueEntries;
			BarsRequired = 22;
            CalculateOnBarClose = true;
			ExitOnClose = false;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
			monthHigh = High[HighestBar(High, 22)];
			monthLow = Low[LowestBar(Low, 22)];

		/// Rules:
		///  - if 8% below 22 day high, add to position
		///  - if 16% below 22 day high, add 3x to position
		///  - if 10% profit, close 50%, if 20% close 50% again

			qty = Iparm1;
			stopPrice = 0;

			if (buyOrder1 == null) // || buyOrder1.OrderState != OrderState.Filled)
			{
				limitPrice = Instrument.MasterInstrument.Round2TickSize(monthHigh * Dparm1);
				DrawDot(CurrentBar + "Long_limitPrice", false, 0, limitPrice, Color.Black);
				if (Close[0] <= limitPrice) 
					buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy1", "Buy1");
				else
				{
					buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy1", "Buy1");
				}
			} 
			else
			{
				if (buyOrder1.OrderState != OrderState.Filled)
				{
					limitPrice = monthHigh * Dparm1;
					if (Close[0] <= limitPrice) 
						buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy1", "Buy1");
					else
					{
						if (limitPrice != buyOrder1.LimitPrice) 
						{
							DrawDot(CurrentBar + "Long_limitPrice", false, 0, limitPrice, Color.Black);
							ChangeOrder(buyOrder1, buyOrder1.Quantity, limitPrice, stopPrice);
						}
					}
				}
			}
			

			qty = Iparm2;
			limitPrice = monthHigh * Dparm2;
			
			if (buyOrder2 == null) 
			{
				DrawDot(CurrentBar + "Long_limitPriceX", false, 0, limitPrice, Color.Gray);
				if (Close[0] <= limitPrice) 
					buyOrder2 = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy2", "Buy2");
				else
				{
					buyOrder2 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy2", "Buy2");
				}
			} 
			else
			{
				if (buyOrder2.OrderState != OrderState.Filled)
				{
					if (Close[0] <= limitPrice) 
						buyOrder2 = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy2", "Buy2");
					else
					{
						if (limitPrice != buyOrder2.LimitPrice) 
						{
							DrawDot(CurrentBar + "Long_limitPriceX", false, 0, limitPrice, Color.Gray);
							ChangeOrder(buyOrder2, buyOrder2.Quantity, limitPrice, stopPrice);
						}
					}
				}
			}
			
			qty = Iparm3;
			limitPrice = monthHigh * Dparm3;
			
			if (buyOrder3 == null) 
			{
				DrawDot(CurrentBar + "Long_limitPrice3", false, 0, limitPrice, Color.Yellow);
				if (Close[0] <= limitPrice) 
					buyOrder3 = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy3", "Buy3");
				else
				{
					buyOrder3 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy3", "Buy3");
				}
			} 
			else
			{
				if (buyOrder3.OrderState != OrderState.Filled)
				{
					if (Close[0] <= limitPrice) 
						buyOrder3 = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoBuy3", "Buy3");
					else
					{
						if (limitPrice != buyOrder3.LimitPrice) 
						{
							DrawDot(CurrentBar + "Long_limitPrice3", false, 0, limitPrice, Color.Yellow);
							ChangeOrder(buyOrder3, buyOrder3.Quantity, limitPrice, stopPrice);
						}
					}
				}
			}
			
        }


		
		protected override void OnExecution(IExecution execution)
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			if (execution.Order == null)
			{
				//Print(Time + " -->> OnExecution.Order is null");
				return;
			}
			
			Print(Time + " execution.Order: " + execution.Order.ToString());
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (buyOrder1 != null && buyOrder1 == execution.Order)
			{
				//Print(Time + " buyOrder1: " + buyOrder1);
				if (closeOrder1Limit == null) 
				{
					stopPrice = 0;
					limitPrice = (buyOrder1.AvgFillPrice/Dparm1) * (1.0 + Period1/100.0);
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrder1Limit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose1", "Close Long Limit1");
					
					//barNumberSinceFilled = CurrentBar;
				} 
			} 

			if (buyOrder2 != null && buyOrder2 == execution.Order)
			{
				//Print(Time + " buyOrder2: " + buyOrder2);
				if (closeOrder2Limit == null) 
				{
					stopPrice = 0;
					limitPrice = (buyOrder2.AvgFillPrice/Dparm2) * (1.0 + Period2/100.0);
					DrawDot(CurrentBar + "limitPricex", false, 0, limitPrice, Color.Lime);
					closeOrder2Limit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose2", "Close Long Limit2");
					
					//barNumberSinceFilled = CurrentBar;
				} 
			} 

			if (buyOrder3 != null && buyOrder3 == execution.Order)
			{
				//Print(Time + " buyOrder3: " + buyOrder3);
				if (closeOrder3Limit == null) 
				{
					stopPrice = 0;
					limitPrice = (buyOrder3.AvgFillPrice/Dparm3) * (1.0 + Period3/100.0);
					DrawDot(CurrentBar + "limitPricex", false, 0, limitPrice, Color.Lime);
					closeOrder3Limit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, stopPrice, orderPrefix + "ocoClose3", "Close Long Limit3");
					
					//barNumberSinceFilled = CurrentBar;
				} 
			} 

			// ===================================================
			//   Trade hit Long limit, no more trades for the day
			// ===================================================
			if (closeOrder1Limit != null && closeOrder1Limit == execution.Order)
			{
				//ResetTrade();
				buyOrder1 = null;
				closeOrder1Limit = null;
			}			

			if (closeOrder2Limit != null && closeOrder2Limit == execution.Order)
			{
				//ResetTrade();
				buyOrder2 = null;
				closeOrder2Limit = null;
			}			

			if (closeOrder3Limit != null && closeOrder3Limit == execution.Order)
			{
				//ResetTrade();
				buyOrder3 = null;
				closeOrder3Limit = null;
			}			

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

		
        #endregion

    }
    
}
