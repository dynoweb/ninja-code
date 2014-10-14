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
    /// </summary>
    [Description("Convert from @Bounty's bot tradestation to NinjaTrade by Rick Cromer (dynoweb)  Based on 15 min CL bars")]
    public class VolumeSpikeNT : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int lookback = 10; // Default setting for Lookback
        private int maxDailyEntries = 1; // Default setting for MaxDailyEntries
        private int stopTime = 145000; // Default setting for StopTime -- 2:50 PM CST
		private int startTime = 080000; // 8:00 AM CST
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

			if (Volume[0] < Volume[1] 
				&& Volume[1] > threshold 
				&& tradeCount < MaxDailyEntries
				&& ToTime(Time[0]) < stopTime) 
			{
				if (Position.MarketPosition == MarketPosition.Flat
					&& ToTime(Time[0]) <= stopTime && ToTime(Time[0]) >= startTime)
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
			
			if (ToTime(Time[0]) >= stopTime)
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

        [Description("HHMM (CST)")]
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
