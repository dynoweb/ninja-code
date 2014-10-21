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
    /// Based on 15 min CL bars. Converted from @Bounty's tradestation BOT to NinjaTrader by Rick Cromer (dynoweb)  
	/// 
	/// The following are comments made by @Bounty
	/// I'm trading in CET which is EST+6. That means regular US stock markets open at 1530 and 
	/// closes at 2200 for me.  
	/// 
	/// Concerning the volume spike strategy, I checked the code again 
	/// and there is no explicit start time. So if the threshold is rather low, it can be triggered 
	/// by low "premarket" volume. When I started developing this strategy, I originally wanted to 
	/// check for the highest per-bar-volume of the previous session (previous day) and derive the 
	/// threshold from that value, but I didn't know how to do that. So I chose the route via highest 
	/// volume within the lookback period multiplied with a certain factor (preferably slighly below 1).
	/// 
	/// The Volume spike itself then consists of the volume of two consecutive bars: The first bar must 
	/// exceed the threshold, and the second bar must be smaller than the first bar, in oder to confirm 
	/// the spike. An outbreak from the combined price range of those to bars then generates a buy or sell signal.
	/// 
	/// I had planned to use the variables VP_High and VP_Low to calculate their difference 
	/// (the aforementioned combined range of the two consecutive bars), because I wanted to calculate 
	/// simple stops and targets that way, but again, I couldn't make it work. So I chose a time exit 
	/// which I optimised on CL data from 2013 and made an out of sample test with 2014.
	/// 
	/// The reasoning behind this is: When the volume spikes, sometimes a significant move 
	/// (or reversal of a previous move) follows.
	/// 
    /// </summary>
    [Description("Convert from @Bounty's bot tradestation to NinjaTrade by Rick Cromer (dynoweb)  Based on 15 min CL bars")]
    public class VolumeSpikeNT : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int lookback = 10; // Default setting for Lookback
        private int maxDailyEntries = 1; // Default setting for MaxDailyEntries
        private int stopTime = 145000; // Default setting for StopTime -- 2:50 PM CST
		private int startTime = 170000; // 05:00 PM CST
		private double vpFactor = 0.8; // Default setting for VpFactor
        // User defined variables (add any user defined variables below)
		double threshold = 0;
		int vpHigh = 0;
		int vpLow = 999999;
		int tradeCount = 0;
		int orderQty = 1;
		// misc
		IOrder longOrder = null;
		IOrder shortOrder = null;
		IOrder closeOrder = null;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(VOL());
			
			ClearOutputWindow();
			
			// to allow both long and short open orders
			Unmanaged = true;
			
			TraceOrders = true;
			
			ExitOnClose = true;
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				vpHigh = 0;
				vpLow = 999999;
				tradeCount = 0;
			}
			
			threshold = VpFactor * MAX(Volume, Lookback)[2];

			// note: The original Tradestation code traded midnight - 21:50 EST+6 
			// or 6 PM EST to 3:50 PM EST <-- note 6 is PM not AM
			// that would be 5 PM to 2:50 PM CST 
			if (Volume[0] < Volume[1] 
				&& Volume[1] > threshold 
				&& tradeCount < MaxDailyEntries
				&& ((ToTime(Time[0]) > startTime) || (ToTime(Time[0]) < stopTime)))
			{
				if (Position.MarketPosition == MarketPosition.Flat)
				{
					Print(Time + " GetCurrentBid: " + GetCurrentBid() 
						+ " MAX: " + MAX(High, 2)[0]
						+ " MIN: " + MIN(Low, 2)[0]);
					
					shortOrder = 
						SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, orderQty, 0,  MIN(Low, 2)[0], "OCO_ID", "SE");  
					DrawDot(CurrentBar + "short", true, 0, MIN(Low, 2)[0], Color.Red);
					
					longOrder = 
						SubmitOrder(0, OrderAction.Buy, OrderType.Stop, orderQty, 0,  MAX(High, 2)[0], "OCO_ID", "LE");  
					DrawDot(CurrentBar + "long", true, 0, MAX(High, 2)[0], Color.Blue);
					
				}
			}
			
			if (ToTime(Time[0]) >= stopTime && ToTime(Time[0]) <= startTime)
			{
				if (longOrder != null && longOrder.OrderState == OrderState.Filled)
				{
					closeOrder = SubmitOrder(0, OrderAction.Sell, OrderType.Market, longOrder.Quantity, 0,0, "", "LE");
				}
					
				if (shortOrder != null && shortOrder.OrderState == OrderState.Filled)
				{
					closeOrder = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, shortOrder.Quantity, 0,0, "", "SE");
				}
			}
        }
		
		protected override void OnOrderUpdate(IOrder order)
		{
			if (longOrder != null && longOrder == order)
			{
				if (order.OrderState == OrderState.Filled)
				{
					tradeCount++;
				}
			}
			
			if (shortOrder != null && shortOrder == order)
			{
				if (order.OrderState == OrderState.Filled)
				{
					tradeCount++;
				}
			}
			
			if (closeOrder != null && closeOrder == order)
			{
				if (order.OrderState == OrderState.Filled)
				{
					closeOrder = null;
					longOrder = null;
					shortOrder = null;
				}
			}
		}
		

        #region Properties

        [Description("")]
        [GridCategory("Parameters")]
        public int Lookback
        {
            get { return lookback; }
            set { lookback = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int MaxDailyEntries
        {
            get { return maxDailyEntries; }
            set { maxDailyEntries = Math.Max(1, value); }
        }

        [Description("HHMMSS (CST) 170000 = 5PM CST")]
        [GridCategory("Parameters")]
        public int StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        [Description("HHMMSS (CST) 145000 = 2:50PM CST")]
        [GridCategory("Parameters")]
        public int StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public double VpFactor
        {
            get { return vpFactor; }
            set { vpFactor = Math.Max(0.000, value); }
        }

        #endregion
    }
}
