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
    /// Uses CL to jump in a breakout based on an opening Range. Requires Minute Period Type
	/// 
	/// Press F5 to compile
	/// 
	/// Problems 
	/// - initial qty
	/// - trades continuing to next session
	/// 
    /// </summary>
    [Description("Uses CL to jump in a breakout from a Range using session open.")]
    public class CLOpen : Strategy
    {
        #region Variables

		// Exposed variables
        private int abeProfitTrigger = 20; // ATM Strategy Variables - sets stop loss to B/E when price moves to this target
		private bool allowReversal = true;
		private int channelMax = 25;
		private int channelMin = 10;
		private int hour = 7;
		private int maxReversals = 2;		
		private int minute = 0;
		private int barNumberStart = 1;
		private int period = 30;
		private int periodMin = 15;
		
		// channel
		private double channelHigh = 0;
		private double channelLow = 0;
		private int adjustedChannelSize = 0;		

        // User defined variables (add any user defined variables below)
        private int startQty = 1; // Start Order Qty, converts to new Qty after reversal
		private int qty = 1;
		//private int nextQty = 0;
		//private double channelSize = 0;
		private int tradeCount = 0;
		private double shortStop = 0;
		private double longStop = 0;
		//private MarketPosition lastMarketPosition = MarketPosition.Flat;
		private double profitPoints = 0;
		private bool enableTraceOrders = false;
		
		private IOrder entryOrderLong = null;
		private IOrder entryOrderShort = null;
		private IOrder closeOrderLongLimit = null;
		private IOrder closeOrderLongStop = null;
		private IOrder closeOrderShortLimit = null;
		private IOrder closeOrderShortStop = null;
		
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
			
			if (BarsPeriod.Id != PeriodType.Minute) 
				return;

			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				Print("=======================");
				channelHigh = 0;
				channelLow = 0;
				tradeCount = 0;
				profitPoints = 0;
				qty = startQty;
				ResetTrade();
			}
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				
			}
			
			if (Position.MarketPosition == MarketPosition.Short) 
			{
			}
			
			// Run on the trigger bar to calculate session range
			if (Bars.BarsSinceSession == barNumberStart + period
				&& adjustedChannelSize == 0
				//&& ToTime(Time[0]) < ToTime(hour, minute + BarsPeriod.Value, 0))
				)
			{
				adjustedChannelSize = calcAdjustedChannelSize();
			}
			
			if (adjustedChannelSize != 0 
				//&& tradeCount <= maxReversals
				&& tradeCount == 0
				&& Position.MarketPosition == MarketPosition.Flat
				&& profitPoints == 0
				//&& ToTime(Time[0]) >= (ToTime(hour, minute, 0) + 10000) // add an hour before we start the trades
				) 
			{
				if (entryOrderLong == null) 
				{					
					stopPrice = channelHigh + 2 * TickSize;
					limitPrice = stopPrice + 4 * TickSize;
					DrawDot(CurrentBar + "Long_stopPrice", false, 0, stopPrice, Color.White);
					DrawDot(CurrentBar + "Long_limitPrice", false, 0, limitPrice, Color.Black);
					entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, qty, limitPrice, stopPrice, "ocoEnter", "Enter Long");
					
					stopPrice = channelLow - 2 * TickSize;
					limitPrice = stopPrice - 4 * TickSize;
					DrawDot(CurrentBar + "Short_stopPrice", false, 0, stopPrice, Color.White);
					DrawDot(CurrentBar + "Short_limitPrice", false, 0, limitPrice, Color.Black);
					entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.StopLimit, qty, limitPrice, stopPrice, "ocoEnter", "Enter Short");
				}
				
			}
			
        }

		private int calcAdjustedChannelSize()
		{
			double channelSize = 0;
			int optimizedPeriod = period;
			int adjustedChannelSize = 0;
			
			while (adjustedChannelSize == 0 && optimizedPeriod >= periodMin) 
			{
				channelHigh = High[HighestBar(High, optimizedPeriod)];
				channelLow = Low[LowestBar(Low, optimizedPeriod)];
				channelSize = (Instrument.MasterInstrument.Round2TickSize(channelHigh - channelLow))/TickSize;
				if (channelSize <= channelMax) 
				{
					adjustedChannelSize = (int) channelSize;
				}
				else
				{
					optimizedPeriod--;
				}
			}
			
			BackColor = Color.Coral;
			
				
			/*
			if (channelSize < ChannelMin || channelSize > ChannelMax) 
			{
				adjustedChannelSize = 0;
			}
			*/
			
			/*
			if (channelSize <= 10) 
			{
				adjustedChannelSize = 0;
			}
			else if (channelSize <= 30)
			{		
				adjustedChannelSize = (int) channelSize;
			}			
			else if (channelSize <= 40)
			{		
				adjustedChannelSize = (int) Instrument.MasterInstrument.Round2TickSize(channelSize * 0.9);
			}			
			else if (channelSize <= 50)
			{		
				adjustedChannelSize = (int) Instrument.MasterInstrument.Round2TickSize(channelSize * 0.8);
			}			
			else if (channelSize <= 55)
			{		
				adjustedChannelSize = (int) Instrument.MasterInstrument.Round2TickSize(channelSize * 0.7);
			}
			else
			{
				// don't trade if acs > 55
				adjustedChannelSize = 0;
			}
			*/
			
			DrawDot(CurrentBar + "channelHigh", false, 0, channelHigh, Color.Blue);
			DrawDot(CurrentBar + "channelLow", false, 0, channelLow, Color.Cyan);
			DrawHorizontalLine("channelHigh", channelHigh, Color.Blue);
			DrawHorizontalLine("channelLow", channelLow, Color.Cyan);
			
			DrawRectangle(CurrentBar + "rectangle", optimizedPeriod, channelLow, -3 * optimizedPeriod, channelHigh, Color.DarkBlue);
			
			Print(Time + " channelLow: " + channelLow + " channelHigh: " + channelHigh + " channelSize: " + channelSize + " adjustedChannelSize: " + adjustedChannelSize);
			//if (adjustedChannelSize > 25) 
			//	adjustedChannelSize = 25;
						
			return adjustedChannelSize;
		}
		
		
		protected override void OnExecution(IExecution execution)
		{
			double limitPrice = 0;
			double stopPrice = 0;
			double breakEvenPrice = 0;
			
			if (execution.Order == null)
			{
				Print(Time + " -->> OnExecution.Order is null");
				return;
			}
			
			//Print(Time + " execution.Order: " + execution.Order.ToString());
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (entryOrderLong != null && entryOrderLong == execution.Order)
			{
				//Print(Time + " entryOrderLong: " + entryOrderLong);
				if (closeOrderLongLimit == null) 
				{
					// the first time this is called (1st trade) it will shrink the channel biased to 1st entry side
					channelLow = channelHigh - adjustedChannelSize * TickSize;
					
					limitPrice = 0;
					//stopPrice = channelHigh - (adjustedChannelSize + 3) * TickSize;
					stopPrice = channelLow - 3 * TickSize;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderLongStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, entryOrderLong.Quantity, limitPrice, stopPrice, "ocoLongClose", "Close Long Stop");
					
					stopPrice = 0; 
					breakEvenPrice = entryOrderLong.AvgFillPrice + Math.Abs(profitPoints/entryOrderLong.Quantity); 
					DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);
					limitPrice = breakEvenPrice + adjustedChannelSize * TickSize;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrderLongLimit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, entryOrderLong.Quantity, limitPrice, stopPrice, "ocoLongClose", "Close Long Limit");
					
					//DrawText(CurrentBar + "stopText", "" + stopPrice, 0, stopPrice, Color.White);
					Print(Time + " channelLow: " + channelLow + " channelHigh: " + channelHigh + " adjustedChannelSize: " + adjustedChannelSize);
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
					channelHigh = channelLow + adjustedChannelSize * TickSize;
					
					limitPrice = 0;
					//stopPrice = channelLow + (adjustedChannelSize + 3) * TickSize;
					stopPrice = channelHigh + 3 * TickSize;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderShortStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, entryOrderShort.Quantity, limitPrice, stopPrice, "ocoShortClose", "Close Short Stop");
					
					stopPrice = 0; 
					breakEvenPrice = entryOrderShort.AvgFillPrice - Math.Abs(profitPoints/entryOrderShort.Quantity);
					DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);
					limitPrice = breakEvenPrice - adjustedChannelSize * TickSize;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrderShortLimit = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, entryOrderShort.Quantity, limitPrice, stopPrice, "ocoShortClose", "Close Short Limit");
					
					//DrawText(CurrentBar + "stopText", "" + stopPrice, 0, stopPrice, Color.White);
					//Print(Time + " stopPrice: " + stopPrice + " adjustedChannelSize: " + adjustedChannelSize);
				} 
				else
				{
					Print(Time + " OnExecution has been fired and stop and limits for Short have already been submitted");
					//Print(Time + " closeOrderShortStop: " + closeOrderShortStop);
					//Print(Time + " closeOrderShortLimit: " + closeOrderShortLimit);
				}
			} 

			// ========================
			//  In trade exists
			// ========================
			if (closeOrderLongStop != null && closeOrderLongStop == execution.Order)
			{
				profitPoints += (-0.005 + closeOrderLongStop.Quantity * (closeOrderLongStop.AvgFillPrice - entryOrderLong.AvgFillPrice));
				Print(Time + " closeOrderLongStop Profit: " + profitPoints);
				
				Print("------------------------");
				entryOrderLong = null;
				entryOrderShort = null;
				closeOrderShortStop = null;
				closeOrderShortLimit = null;
				tradeCount++;
				
				if (tradeCount <= maxReversals
					//&& ToTime(Time[0]) < (ToTime(9, 30, 0))
					) 
				{
					// Reverse to Short order
					stopPrice = 0;
					limitPrice = 0;
					entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.Market,2 * closeOrderLongStop.Quantity, limitPrice, stopPrice, "ocoEnter", "Enter Short");
					// may want to set stop here too, and revise after it's filled
				}
			}			
			
			if (closeOrderLongLimit != null && closeOrderLongLimit == execution.Order)
			{
				profitPoints += (-0.005 + closeOrderLongLimit.Quantity * (closeOrderLongLimit.AvgFillPrice - entryOrderLong.AvgFillPrice));
				Print(Time + " closeOrderLongLimit tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				ResetTrade();
			}			

			if (closeOrderShortStop != null && closeOrderShortStop == execution.Order)
			{
				profitPoints += (-0.005 + closeOrderShortStop.Quantity * (entryOrderShort.AvgFillPrice - closeOrderShortStop.AvgFillPrice));
				Print(Time + " closeOrderShortStop Profit: " + profitPoints);

				Print("------------------------");
				entryOrderLong = null;
				entryOrderShort = null;
				closeOrderLongStop = null;
				closeOrderLongLimit = null;
				tradeCount++;

				if (tradeCount <= maxReversals
					//&& ToTime(Time[0]) < (ToTime(9, 30, 0)) 
					)
				{
					// Reverse to Long order
					stopPrice = 0;
					limitPrice = 0;
					entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 2 * closeOrderShortStop.Quantity, limitPrice, stopPrice, "ocoEnter", "Enter Long");
					// may want to set stop here too, and revise after it's filled
				}
			}			
			
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
			
				adjustedChannelSize = 0;
		}
		
        #region Properties
		
        [Description("Minimum Channel Size in ticks")]
        [GridCategory("Parameters")]
        public int ChannelMin
        {
            get { return channelMin; }
            set { channelMin = Math.Max(5, value); }
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
		
        [Description("The min number of bars to include before opening pit session to build the range")]
        [GridCategory("Parameters")]
        public int PeriodMin
        {
            get { return periodMin; }
            set { periodMin = Math.Max(4, value); }
        }
		
        [Description("The bar number where the range starts")]
        [GridCategory("Parameters")]
        public int BarNumberStart
        {
            get { return barNumberStart; }
            set { barNumberStart = Math.Max(0, value); }
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
		
        [Description("Reverse the trade if it hits the opposite limit")]
        [GridCategory("Parameters")]
        public bool AllowReversal
        {
            get { return allowReversal; }
            set { allowReversal = value; }
        }
		
        [Description("The number of reversals before closing trade")]
        [GridCategory("Parameters")]
        public int MaxReversals
        {
            get { return maxReversals; }
            set { maxReversals = Math.Max(0, value); }
        }		
		
		// ===========================================================
		// Begin ATM Stop Strategy Settings
		// ===========================================================
		
        [Description("Auto Break Even Profit Trigger")]
        [GridCategory("Parameters")]
        public int AbeProfitTrigger
        {
            get { return abeProfitTrigger; }
            set { abeProfitTrigger = Math.Max(1, value); }
        }
		/*
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
