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
	/// Confidential Information - Owned by Rick Cromer and Chris Waite
	/// 
	/// Problems 
	/// - initial qty
	/// - trades continuing to end of session
	/// 
	/// Testing
	/// 
	/// Changes
	///  01/19/2013 - Changing this to reflect the trading rules given to me by Chris
    /// </summary>
    [Description("Uses CL to jump in a breakout from a Range using unmanaged order methods. Currently based on 5 Min Periods")]
    public class BangBang : Strategy
    {

	#region Variables

		// Exposed variables
        private int abeProfitTrigger = 0; // ATM Strategy Variables - sets stop loss to B/E when price moves to this target
		private bool breakEvenLast = false;
		private int hour = 8;
		private int maxReversals = 3;		
		private int minute = 0;
		private int period = 3;
			
		// channel
		private double channelHigh = 0;
		private double channelLow = 0;
		private int adjustedChannelSize = 0;		

        // User defined variables (add any user defined variables below)
        private int startQty = 1; // Start Order Qty, converts to new Qty after reversal
		private int qty = 1;
		private int tradeCount = 0;
		private double shortStop = 0;
		private double longStop = 0;
		private double profitPoints = 0;
		private bool enableTraceOrders = false;
		double breakEvenPrice = 0;
		
		private IOrder entryOrderLong = null;
		private IOrder entryOrderShort = null;
		private IOrder closeOrderLongLimit = null;
		private IOrder closeOrderLongStop = null;
		private IOrder closeOrderShortLimit = null;
		private IOrder closeOrderShortStop = null;
		private IOrder closeOrderLong = null;	
		private IOrder closeOrderShort = null;	
		
		private string orderPrefix = "BngB";
		private bool enableDebug = false;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = false;
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
			
			DateTime exp = new DateTime(2015, 5, 1);
			if (BarsPeriod.Id != PeriodType.Minute || ToDay(Time[0]) > ToDay(exp))
				return;

			// Drawfloating box
			DrawDiamond("MyDiamond", true, 0, High[0] + 2 * TickSize, Color.Blue);	
			
			
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
			
			// do this when in a trade
			if (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short)
			{
				OrderManagement();
			}
			
			// Run on the trigger bar to calculate daily range
			if (ToTime(Time[0]) == ToTime(hour, minute, 0)
				&& adjustedChannelSize == 0
				)
			{
				adjustedChannelSize = calcAdjustedChannelSize();
				
				if (enableDebug) DrawHorizontalLine("channelHigh", channelHigh, Color.Blue);
				if (enableDebug) DrawHorizontalLine("channelLow", channelLow, Color.Cyan);
			}
			
			if (adjustedChannelSize != 0 
				&& tradeCount == 0
				&& Position.MarketPosition == MarketPosition.Flat
				&& profitPoints == 0
				) 
			{
				if (entryOrderLong == null && entryOrderShort == null) 
				{					
					stopPrice = channelHigh + 2 * TickSize;
					limitPrice = stopPrice + 0 * TickSize;
					DrawDot(CurrentBar + "Long_stopPrice", false, 0, stopPrice, Color.White);
					DrawDot(CurrentBar + "Long_limitPrice", false, 0, limitPrice, Color.Black);
					entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Long");
					
					stopPrice = channelLow - 2 * TickSize;
					limitPrice = stopPrice - 0 * TickSize;
					DrawDot(CurrentBar + "Short_stopPrice", false, 0, stopPrice, Color.White);
					DrawDot(CurrentBar + "Short_limitPrice", false, 0, limitPrice, Color.Black);
					entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Short");
				}				
			}			
        }

		private void OrderManagement()
		{
			double limitPrice = 0;
			double stopPrice = 0;
			/*
			// Raise stop loss to breakeven when you are at least abeProfitTrigger ticks in profit
    		if (closeOrderLongStop != null 
				&& closeOrderLongStop.StopPrice < Position.AvgPrice 
				&& Close[0] >= Position.AvgPrice + abeProfitTrigger * TickSize
				)
			{
         		ChangeOrder(closeOrderLongStop, closeOrderLongStop.Quantity, closeOrderLongStop.LimitPrice, Position.AvgPrice);
			}
			*/

			
			//if (Close[0] < channelLow)
			//{
			//	Print(Time + " Closed below channel,     entryOrderLong -- " + entryOrderLong);
			//	Print(Time + " Closed below channel, closeOrderLongStop -- " + entryOrderLong);
			//}
			
			// Change long close order to close if stop trigger (closed below channel)
    		if (null != closeOrderLongStop
			//	&& closeOrderLongStop.StopPrice < Position.AvgPrice 
				&& closeOrderLong == null
				&& Close[0] < channelLow 
				)
			{
				if (enableDebug) Print(Time + " Closed below channel on a long, selling position for " + entryOrderLong);
				closeOrderLong = SubmitOrder(0, OrderAction.Sell, OrderType.Market, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Mkt");
			}
			
			// Change short close order to close if stop trigger (closed above channel)
    		if (null != closeOrderShortStop 
			//	&& closeOrderStopStop.StopPrice < Position.AvgPrice 
				&& closeOrderShort == null
				&& Close[0] > channelHigh
				)
			{
				if (enableDebug) Print(Time + " Closed above channel on a short, buying to cover position");
				closeOrderShort = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Mkt");
				//Print(Time + " Submitted order to buy to cover position");
			}

			/*
			if (breakEvenLast 
				&& Close[0] <= breakEvenPrice 
				&& (tradeCount == maxReversals)
				)
			{
				Print(Time + " Close[0]: " + Close[0] + " Closed below breakEvenPrice: " + breakEvenPrice);
			}
			*/


			/*
			// if on the last reversal, move stop up to averagePrice for a single leg B/E
    		if (closeOrderLongLimit != null 
				&& tradeCount == maxReversals
				//&& closeOrderShortStop.StopPrice > Position.AvgPrice 
				//&& Close[0] >= Position.AvgPrice + abeProfitTrigger * TickSize
				)
			{
				Print(Time + " Changing Order to B/E -- breakEvenPrice: " + breakEvenPrice + " tradeCount: " + tradeCount);
         		ChangeOrder(closeOrderLongLimit, closeOrderLongLimit.Quantity, breakEvenPrice, channelHigh + 3 * TickSize);
			}
			
			// if on the last reversal, close limit will be the B/E point
    		if (closeOrderShortLimit != null 
				&& tradeCount == maxReversals
				//&& closeOrderShortStop.StopPrice > Position.AvgPrice 
				//&& Close[0] >= Position.AvgPrice + abeProfitTrigger * TickSize
				)
			{
				Print(Time + " Changing Order to B/E -- breakEvenPrice: " + breakEvenPrice + " tradeCount: " + tradeCount);
         		ChangeOrder(closeOrderShortLimit, closeOrderShortLimit.Quantity, breakEvenPrice, channelHigh + 3 * TickSize);
			}
			*/
		}
		
		private int calcAdjustedChannelSize()
		{
			int checkPeriod = Math.Min(period, Bars.BarsSinceSession);  // Bars.PercentComplete
			
			channelHigh = High[HighestBar(High, checkPeriod)];
			channelLow = Low[LowestBar(Low, checkPeriod)];
			BackColor = Color.Coral;

			double channelSize = (Instrument.MasterInstrument.Round2TickSize(channelHigh - channelLow))/TickSize;
			int adjustedChannelSize = (int) channelSize;
			
			//if (channelSize < ChannelMin || channelSize > ChannelMax) 
			//{
			//	adjustedChannelSize = 0;
			//}
			
			
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
				// don't trade if channelSize > 55
				adjustedChannelSize = 0;
			}
						
			if (enableDebug) Print(Time + " channelLow: " + channelLow + " channelHigh: " + channelHigh + " channelSize: " + channelSize + " adjustedChannelSize: " + adjustedChannelSize);
			//if (adjustedChannelSize > 25) 
			//	adjustedChannelSize = 25;													
			
			// Calculate end of channel rectangle
			//	 1 min - 240 bars to get 4 hours
			//	 2 min - 120 bars
            //     5 min - 48 bars
			//	15 min - 16 bars
			int endRectangle = 0;
			if (BarsPeriod.Id == PeriodType.Minute) {
				endRectangle = 240/BarsPeriod.Value;
			}			
			if (enableDebug) DrawRectangle(CurrentBar + "rectangle", checkPeriod - 1, channelLow, -endRectangle, channelHigh, Color.DarkBlue);
			
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
			
			if (enableDebug) Print(Time + " execution.Order: " + execution.Order.ToString());
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (entryOrderLong != null && entryOrderLong == execution.Order)
			{
				//Print(Time + " entryOrderLong: " + entryOrderLong);
				if (closeOrderLongLimit == null) 
				{
					// the first time this is called (1st trade) it will shrink the channel biased to 1st entry side
//					channelLow = channelHigh - adjustedChannelSize * TickSize;
					
					limitPrice = 0;
					stopPrice = channelLow - 20 * TickSize;	
					//stopPrice = channelLow - 1 * TickSize;
					
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderLongStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Stop");

					stopPrice = 0; 
					breakEvenPrice = entryOrderLong.AvgFillPrice + Math.Abs(profitPoints/entryOrderLong.Quantity); 
					DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);
					
					if (breakEvenLast && tradeCount == maxReversals)
					{
						Print(Time + " Setting Limit at BreakEven");
						//Print(Time + " closeOrderLongStop: " + closeOrderLongStop);
						limitPrice = breakEvenPrice;
					} else {
						limitPrice = breakEvenPrice + (adjustedChannelSize) * TickSize;
						DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					}
					
					closeOrderLongLimit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Limit");
					
					//DrawText(CurrentBar + "stopText", "" + stopPrice, 0, stopPrice, Color.White);
					//Print(Time + " channelLow: " + channelLow + " channelHigh: " + channelHigh + " adjustedChannelSize: " + adjustedChannelSize);
				} 
				else
				{
					if (enableDebug) Print(Time + " OnExecution has been fired and stop and limits for long have already been submitted");
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
					// after thinking about this, they keep the channel size the same, so losses will
					// be based on the full channel, the profit target will used the adjustedChannelSize.
//					channelHigh = channelLow + adjustedChannelSize * TickSize;
					
					limitPrice = 0;
					stopPrice = channelHigh + 20 * TickSize;	
					//stopPrice = channelLow + (adjustedChannelSize + 3) * TickSize;
					
					//Print("Setting Stop");
					//stopPrice = entryOrderShort.AvgFillPrice + (adjustedChannelSize + 3) * TickSize;
					//Print("stopPrice: " + stopPrice);
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderShortStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Stop");
					
					stopPrice = 0; 
					breakEvenPrice = entryOrderShort.AvgFillPrice - Math.Abs(profitPoints/entryOrderShort.Quantity);
					DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);

					if (breakEvenLast && tradeCount == maxReversals)
					{
						Print(Time + " Setting Limit at BreakEven");
						limitPrice = breakEvenPrice;
					} else {
						limitPrice = breakEvenPrice - (adjustedChannelSize) * TickSize;
						DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					}					
					
					closeOrderShortLimit = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Limit");
					
					//DrawText(CurrentBar + "stopText", "" + stopPrice, 0, stopPrice, Color.White);
					//Print(Time + " stopPrice: " + stopPrice + " adjustedChannelSize: " + adjustedChannelSize);
				} 
				else
				{
					if (enableDebug) Print(Time + " OnExecution has been fired and stop and limits for Short have already been submitted");
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
				int closeQty = entryOrderLong.Quantity;
				
				if (closeOrderLongStop == execution.Order)
				{
					//closeQty = closeOrderLongStop.Quantity;
					profitPoints += (-0.005 + closeQty * (closeOrderLongStop.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				else if (closeOrderLong == execution.Order)
				{
					//closeQty = closeOrderLong.Quantity;
					profitPoints += (-0.005 + closeOrderLong.Quantity * (closeOrderLong.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				
				if (enableDebug) 
				{
					Print(Time + " Long Close Profit: " + profitPoints);				
					Print("------------------------");
				}
				
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
					limitPrice = 0;

					entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, 2 * closeQty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Short");

					// TODO - may want to set stop here too, and revise after it's filled
				}
			}			
			
			// ===================================================
			//   Trade hit Long limit, no more trades for the day
			// ===================================================
			if (closeOrderLongLimit != null && closeOrderLongLimit == execution.Order)
			{
				profitPoints += (-0.005 + closeOrderLongLimit.Quantity * (closeOrderLongLimit.AvgFillPrice - entryOrderLong.AvgFillPrice));
				if (enableDebug) Print(Time + " closeOrderLongLimit tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				ResetTrade();
			}			

			// ==================================================
			//   Trade hit Short stop or reversal, create reversed trade Long
			// ==================================================
			if ((closeOrderShortStop != null || closeOrderShort != null)
				&& (execution.Order == closeOrderShortStop || execution.Order == closeOrderShort))
			{
				int closeQty = entryOrderShort.Quantity;
				
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
				
				if (enableDebug) 
				{
					Print(Time + " Short Close Profit: " + profitPoints);
					Print("------------------------");
				}
				
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
					limitPrice = 0;
					
					if (enableDebug) Print(Time + " -- Submitting a reversal trade long market");
					entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 2 * closeQty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Long");
					
					// may want to set stop here too, and revise after it's filled
				}
			}			
			
			// ===================================================
			//   Trade hit Short limit, no more trades for the day
			// ===================================================
			if (closeOrderShortLimit != null && closeOrderShortLimit == execution.Order)
			{
				profitPoints += (-0.005 + closeOrderShortLimit.Quantity * (entryOrderShort.AvgFillPrice - closeOrderShortLimit.AvgFillPrice));
				if (enableDebug) Print(Time + " closeOrderShortLimit tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				ResetTrade();
			}			
		}
		
		private void ResetTrade() {
			
			if (enableDebug) Print("--------------- ResetTrade --------------");
			
			entryOrderLong = null;
			entryOrderShort = null;

			closeOrderLongStop = null;
			closeOrderLongLimit = null;
			closeOrderShortStop = null;
			closeOrderShortLimit = null;
			closeOrderLong = null;
			closeOrderShort = null;
		
			adjustedChannelSize = 0;
		}
		
        #region Properties
		
        [Description("The number of bars to include before opening pit session to build the range")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
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
		
        [Description("On the last trade, close at break even")]
        [GridCategory("Parameters")]
        public bool BreakEvenLast
        {
            get { return breakEvenLast; }
            set { breakEvenLast = value; }
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
            set { abeProfitTrigger = Math.Max(0, value); }
        }

        [Description("Start Order Quantity")]
        [GridCategory("Parameters")]
        public int StartQty
        {
            get { return startQty; }
            set { startQty = Math.Max(1, value); }
        }

		// ===========================================================
		// End ATM Stop Strategy Settings
		// ===========================================================
		
        #endregion
    }
}
