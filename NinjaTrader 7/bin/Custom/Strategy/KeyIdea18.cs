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
    [Description("CL 1 min bars - Reserves announcement trader")]
    public class KeyIdea18 : Strategy
    {
        #region Variables
        // Wizard generated variables
//        private int buyAndHold = 0;
        private int tradeLong = 1;
        private int tradeShort = 1;
        private int iparm1 = 24;
		private int iparm2 = 24; // Default setting for Iparm2
        private double dparm1 = 0; // Default setting for Dparm1
        private double dparm2 = 0; // Default setting for Dparm2
//		private double ratched = 0.05;
        private int period1 = 10; // Default setting for Period1
        private int period2 = 27; // Default setting for Period2
//		private bool isMo = true;
//		private bool is2nd = false;
        // User defined variables (add any user defined variables below)
		
		// ATRTrailing parms
		ATRTrailing atr = null;
		double atrTimes = 3.5; 
		int atrPeriod = 10; 
		
		//  KeltnerChannel parms
		KeltnerChannel kc = null;
		double offsetMultiplier = 1.5;
		int keltnerPeriod = 10; 
		
		// DonchianChannel parms
		DonchianChannel dc = null;
		int donchianPeriod = 3;
		
		int tradesInSession = 0;
		double prevHigh = 0;
		
		int closeUpAfterTwoLows = 0;
		int closeUpAfterTwoHighs = 0;
		int closeDownAfterTwoLows = 0;
		int closeDownAfterTwoHighs = 0;
		int twoLows = 0;
		int twoHighs = 0;
		
		//NinjaTrader.Indicator.PriceActionSwingOscillator so = null;
		
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
			
			dc = DonchianChannel(Period1);
			dc.Displacement = 1;
			dc.PaintPriceMarkers = false;
			Add(dc);
			
			Add(PitColor(Color.Blue, 80000, 15, 140000));
			
            SetProfitTarget(CalculationMode.Ticks, Iparm1);
            //SetStopLoss(CalculationMode.Ticks, Iparm2);
            SetTrailStop(CalculationMode.Ticks, Iparm2);

			Unmanaged = false;
			BarsRequired = 10;
            CalculateOnBarClose = true;
			ExitOnClose = true;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
			if (Bars.FirstBarOfSession)
				tradesInSession = 0;

			// If it's not Wednesday, do not trade.
//    		if (Time[0].DayOfWeek != DayOfWeek.Wednesday)
//				return;

//			if (ToTime(Time[0]) == 93000)
//				prevHigh = High[HighestBar(High, 3)];

			if (ToTime(Time[0]) >= 93000
				&& ToTime(Time[0]) <= 110000
				&& tradesInSession < 5)
			{
				if (isFlat())
				{
					if (Close[0] > dc.Upper[1])
						this.EnterLong();
					else if (Close[0] < dc.Lower[1])
						this.EnterShort();
//					EnterLongStop(prevHigh, "EL");
				}
			}
				
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
//				if (isBull())
//				{
//					//DrawDot(CurrentBar + "Bull", true, 0, High[0] + 2 * TickSize, Color.Blue);
//					if (isShort() &&  buyAndHold == 0)
//					{
//						ExitShort();
//					}
//					if (isFlat() && TradeLong == 1)
//					{
//						EnterLong();
//					}					
//				}
//				if (isBear())
//				{
//					//DrawDot(CurrentBar + "Bear", true, 0, Low[0] - 2 * TickSize, Color.Red);
//					if (isLong() && buyAndHold == 0)
//					{
//						ExitLong();
//					}
//					if (isFlat() && TradeShort == 1)
//					{
//						EnterShort();
//					}
//				}
				
        }
		
		protected override void OnTermination()
		{
		}

		/// <summary>
		/// The OnOrderUpdate() method is called everytime an order managed by a strategy changes state. 
		/// An order will change state when a change in order quantity, price or state (working to 
		/// filled) occurs.
		/// </summary>
		/// <param name="order"></param>
		protected override void OnOrderUpdate(IOrder order)
		{
//			if (entryOrder != null && entryOrder == order)
//			{
//				Print(order.ToString());
//				if (order.OrderState == OrderState.Filled)
//					entryOrder = null;
//			}
		}
		
		/// <summary>
		/// The OnExecution() method is called on an incoming execution. An execution 
		/// is another name for a fill of an order.
		/// </summary>
		/// <param name="execution"></param>
		protected override void OnExecution(IExecution execution)
		{

			// Remember to check the underlying IOrder object for null before trying to 
			// access its properties
			if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
			{
				//	Print(execution.ToString());
				tradesInSession++;
			}
		}

		
		private bool isBull()
		{
			if (Close[0] > Close[Iparm2])
			{
				return true;
			}
			return false;
		}
		
		private bool isBear()
		{
			if (Close[0] < Close[Iparm2])
			{
				return true;
			}
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
//        [Description("0 - trade, 1 - buy and hold")]
//        [GridCategory("Parameters")]
//        public int BuyAndHold
//        {
//            get { return buyAndHold; }
//            set { buyAndHold = Math.Max(0, value); }
//        }
//
//        [Description("0 - no long trades, 1 - take long trades")]
//        [GridCategory("Parameters")]
//        public int TradeLong
//        {
//            get { return tradeLong; }
//            set { tradeLong = Math.Max(0, value); }
//        }
//
//        [Description("0 - no short trades, 1 - take short trades")]
//        [GridCategory("Parameters")]
//        public int TradeShort
//        {
//            get { return tradeShort; }
//            set { tradeShort = Math.Max(0, value); }
//        }

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

//        [Description("Mo Entries")]
//        [GridCategory("Parameters")]
//        public bool IsMo
//        {
//            get { return isMo; }
//            set { isMo = value; }
//        }
//
//        [Description("2nd Chance")]
//        [GridCategory("Parameters")]
//        public bool Is2nd
//        {
//            get { return is2nd; }
//            set { is2nd = value; }
//        }
//		
//        [Description("")]
//        [GridCategory("Parameters")]
//        public double Ratched
//        {
//            get { return ratched; }
//            set { ratched = Math.Max(0.000, value); }
//        }

        #endregion
    }
}
