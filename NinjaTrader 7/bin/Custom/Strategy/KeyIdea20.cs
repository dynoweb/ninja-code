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
	/// Buy next bar at the highest high of last X bars
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
    [Description("30 min GC bars - Kevin Davey's Gold Trade")]
    public class KeyIdea20 : Strategy
    {
        #region Variables
        private int profitTarget = 4000;
		private int stopLoss = 500; // Note: over the long run, moving this out to 300 gives the best return
        private int period1 = 60; // Default setting for Period1
		
//		// ATRTrailing parms
//		ATRTrailing atr = null;
//		double atrTimes = 10; // 3.5 default; 
//		int atrPeriod = 20; // 5 default; 
		double ratched = 0.00;
//		
//		//  KeltnerChannel parms
//		KeltnerChannel kc = null;
//		double offsetMultiplier = 1.5;
//		int keltnerPeriod = 10; 
		
		// DonchianChannel parms
		DonchianChannel dc = null;
		int donchianPeriod = 3;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
        /// </summary>
        protected override void Initialize()
        {
			dc = DonchianChannel(Period1);
			dc.Displacement = 1;
			dc.PaintPriceMarkers = false;
			Add(dc);
			
            SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
            SetStopLoss(CalculationMode.Ticks, StopLoss);
            //SetTrailStop(CalculationMode.Ticks, Iparm2);

			Unmanaged = false;
			BarsRequired = 10;
            CalculateOnBarClose = true;
			ExitOnClose = false;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
			// If it's not Monday, do not trade.
//    		if (Time[0].DayOfWeek != DayOfWeek.Monday)
//				return;

//			if (Bars.FirstBarOfSession)
//				tradesInSession = 0;

//			if (ToTime(Time[0]) == 93000)
//				prevHigh = High[HighestBar(High, 3)];

			
			//Print(Time + " atr.Upper: " + atr.Upper[0] + " atr.Lower: " + atr.Lower[0] + " atr.Upper.ContainsValue(0): " + atr.Upper.ContainsValue(0));
//			if (isFlat() && enableTrade)
//			{
				// long trade
//				if (atr.Upper.ContainsValue(0))
//				{
//					//DrawDot(CurrentBar + "Long", false, 0, Typical[0], Color.Cyan);
//				}
				
			if (Close[0] > dc.Upper[1])
				this.EnterLong();
			else if (Close[0] < dc.Lower[1])
				this.EnterShort();
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
				//tradesInSession++;
				//enableTrade = false;
			}
		}

		
		
        #region Properties

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period1
        {
            get { return period1; }
            set { period1 = Math.Max(1, value); }
        }


        #endregion
    }
}
