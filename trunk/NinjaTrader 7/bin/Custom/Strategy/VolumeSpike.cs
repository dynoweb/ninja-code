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
    /// Convert from @Bounty's bot tradestation to NinjaTrade by Rick Cromer (dynoweb)  Based on 15 min CL bars
    /// </summary>
    [Description("Convert from @Bounty's bot tradestation to NinjaTrade by Rick Cromer (dynoweb)  Based on 15 min CL bars")]
    public class VolumeSpike : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int lookback = 48; // Default setting for Lookback
        private int maxDailyEntries = 1; // Default setting for MaxDailyEntries
        private int stopTime = 150000; // Default setting for StopTime
        private double vpFactor = 0.800; // Default setting for VpFactor
        // User defined variables (add any user defined variables below)
		double threshold = 0;
		int vpHigh = 0;
		int vpLow = 999999;
		int tradeCount = 0;
		int orderQty = 1;
		// misc
		IOrder entryLongOrder = null;
		IOrder entryShortOrder = null;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(VOL());
			
			ClearOutputWindow();
			
			EntriesPerDirection = 1; 
			EntryHandling = EntryHandling.AllEntries;
			
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
				
				if (entryLongOrder != null)
				{
//					CancelOrder(entryLongOrder);
//					entryLongOrder = null;
				}
				if (entryShortOrder != null)
				{
//					CancelOrder(entryShortOrder);
//					entryShortOrder = null;
				}
			}
			
			threshold = VpFactor * MAX(Volume, Lookback)[0];
			//Print(Time + " threshold: " +  threshold + " MaxVol: " + MAX(Volume, Lookback)[0]);
			
			// collect values
			if (Volume[0] < Volume[1] 
				&& Volume[1] > threshold 
				&& tradeCount < MaxDailyEntries
				&& ToTime(Time[0]) < stopTime) 
			{
				if (Position.MarketPosition == MarketPosition.Flat)
				{
					Print(Time + " GetCurrentBid: " + GetCurrentBid() 
						+ " MAX: " + MAX(High, 2)[0]
						+ " MIN: " + MIN(Low, 2)[0]);
					
					entryLongOrder = EnterLongStop(0, false, orderQty, MAX(High, 2)[0], "LE");
					DrawDot(CurrentBar + "long", true, 0, MAX(High, 2)[0], Color.Blue);
					
					if (GetCurrentBid() <= MIN(Low, 2)[0])
					{
						entryShortOrder = EnterShort(0, orderQty, "SE");
						DrawDot(CurrentBar + "short", true, 0, MIN(Low, 2)[0], Color.Pink);
					} 
					else
					{
						entryShortOrder = EnterShortStop(0, false, orderQty, MIN(Low, 2)[0], "SE");
						//entryShortOrder = EnterShortStop(orderQty, MIN(Low, 2)[0], "SE");
						DrawDot(CurrentBar + "short", true, 0, MIN(Low, 2)[0], Color.Red);
					}
				}
			}
			
			if (ToTime(Time[0]) >= stopTime)
			{
				if (Position.MarketPosition == MarketPosition.Long)
					ExitLong(orderQty, "Time Exit", "LE");	
				if (Position.MarketPosition == MarketPosition.Short)
					ExitShort(orderQty, "Time Exit", "SE");	
			}
        }
		
		protected override void OnOrderUpdate(IOrder order)
		{
			if (entryLongOrder != null && entryLongOrder == order)
			{
				//Print(Time + " " + order);
				if (order.OrderState == OrderState.Filled)
				{
					if (entryShortOrder != null)
						CancelOrder(entryShortOrder);
					//entryShortOrder = null;
					//entryLongOrder = null;
					tradeCount++;
				}
			}
			
			if (entryShortOrder != null && entryShortOrder == order)
			{
				//Print(Time + " " + order);
				if (order.OrderState == OrderState.Filled)
				{
					if (entryLongOrder != null)
						CancelOrder(entryLongOrder);
					//entryShortOrder = null;
					//entryLongOrder = null;
					tradeCount++;
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
