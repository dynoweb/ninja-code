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
    /// Uses CL to jump in a breakout
	/// 
	/// Press F5 to compile
	/// 
	/// Problems 
	/// 
	/// Testing
	/// - 01/13/13 Replay testing CL 02-13, 1 min bars, from 11/17/2012 - 1/12/2013 - 20,true,35,10,7,2,0,30, $3,470.00
	/// - v1.9 2012 back test - 0,0,false,35,10,7,3,0,30  68,525 (3r), 54,040 (2r), 3,120 (1r), -360 (0r)
	/// 
	/// Bugs
	/// 
	/// Change Log
	///  01/23/13 - 
	/// 
	/// Backlog
	///		shift the period start and end later (fixed bar count on range) in time until the periods satisfy the range conditions
	/// 	shift the start time forward if not within channel range limits (expanded bar count)
	///		also, start tracking from session open until it got three points wide, then start the trade. 
	///		add an option to scale the range smaller on larger ranges
	///		modify range inclusion on channel range condition (change less than to less than or equal)
	/// 	on failed breakout, expand range to include reversal
	/// 
    /// </summary>
    [Description("Uses CL to jump in a breakout from a Range using unmanaged order methods 1 min bars")]
    public class SlidingRange : Strategy
    {
        #region Variables

		// Exposed properties
        private int abeProfitTrigger = 0; // ATM Strategy Variables - sets stop loss to B/E when price moves to this target
		private int atStopLoss1 = 18; // Auto Trail AtStopLoss		
		private int channelMax = 26;
		private int channelMin = 1;
		private bool enablePeriodExpansion = true;
		private bool enablePeriodCompression = true;
		private int period = 180;
		private int profitTarget = 15; // Closes once reached, if 0 will be ignored ProfitTarget
		//private int atProfitTrigger1 = 10; // Auto Trail AtProfitTrigger
        private int startQty = 5; // Start Order Qty, converts to new Qty after reversal
		
		// channel
		private double channelHigh = 0;
		private double channelLow = 0;
		private int adjustedChannelSize = 0;		

        // User defined variables (add any user defined variables below)
		private int qty = 1;
		private int tradeCount = 0;
		private double shortStop = 0;
		private double longStop = 0;
		private double profitPoints = 0;
		private bool enableTraceOrders = false;
		private double breakEvenPrice = 0;
		private bool specialStop = false;

		// order management		
		private IOrder entryOrderLong = null;
		private IOrder entryOrderShort = null;
		private IOrder closeOrderLongLimit = null;
		private IOrder closeOrderLongStop = null;
		private IOrder closeOrderShortLimit = null;
		private IOrder closeOrderShortStop = null;
		private IOrder closeOrderLong = null;	
		private IOrder closeOrderShort = null;	
		
		private string orderPrefix = "Slide";
		
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
			TimeInForce = Cbi.TimeInForce.Day;
			
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
			// Do not calculate if we don't have enough bars 
			if (CurrentBar < Period) return;

			double limitPrice = 0;
			double stopPrice = 0;
			
			DateTime exp = new DateTime(2013, 4, 15);
			if (BarsPeriod.Id != PeriodType.Minute || ToDay(Time[0]) > ToDay(exp))
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
			
			// do this when in a trade
			if (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short)
			{
				OrderManagement();
			}
			
			QualifyRange();			
			
			if (adjustedChannelSize != 0 
				&& Position.MarketPosition == MarketPosition.Flat
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
		
		private void QualifyRange()
		{
			int channelRangeX = channelMax;
			if (adjustedChannelSize == 0 && Bars.BarsSinceSession >= period) 
			{
				channelHigh = High[HighestBar(High, period)];
				channelLow = Low[LowestBar(Low, period)];
				if (channelHigh-channelLow <= channelRangeX * TickSize)
				{
					DrawRectangle(CurrentBar + "rectangleMew", false, period, channelLow, 0, channelHigh, Color.DarkBlue, Color.DarkBlue, 5);		
					double channelSize = (Instrument.MasterInstrument.Round2TickSize(channelHigh - channelLow))/TickSize;
					adjustedChannelSize = (int) channelSize;
					
					DrawDot(CurrentBar + "channelHigh", false, 0, channelHigh, Color.BlueViolet);
					DrawDot(CurrentBar + "channelLow", false, 0, channelLow, Color.Chartreuse);
					DrawHorizontalLine("channelHigh", channelHigh, Color.Blue);
					DrawHorizontalLine("channelLow", channelLow, Color.Cyan);
				} 
				//Print(Time + " qualifying range adjustedChannelSize: " + adjustedChannelSize); 
			}
		}

		
		private void OrderManagement()
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			// Change long close order to close if stop trigger (closed below channel)
    		if (null != closeOrderLongStop)
			{
				if (closeOrderLong == null
					&& Close[0] < channelLow 
					)
				{
					Print(Time + " Closed below channel on a long, selling position for " + entryOrderLong);
					closeOrderLong = SubmitOrder(0, OrderAction.Sell, OrderType.Market, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Mkt");
				} 
				else 
				{					
					double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
					if (atStopLoss1 > 0) // Trailing Stop
					{
						stopPrice = Math.Max(closeOrderLongStop.StopPrice, highSinceOpen - (atStopLoss1 * TickSize));
						ChangeOrder(closeOrderLongStop, closeOrderLongStop.Quantity, closeOrderLongStop.LimitPrice, stopPrice);
						specialStop = true;
					} 
					else if (abeProfitTrigger > 0)	// sets stop loss to B/E when price moves to this target
					{
						if ((highSinceOpen - Position.AvgPrice) >= abeProfitTrigger * TickSize)
						{
							//stopPrice = Math.Max(closeOrderLongStop.StopPrice, highSinceOpen - (atStopLoss1 * TickSize));
							stopPrice = Position.AvgPrice;
							ChangeOrder(closeOrderLongStop, closeOrderLongStop.Quantity, closeOrderLongStop.LimitPrice, stopPrice);
							specialStop = true;
						}
					}
				}
			}
			
			// Change short close order to close if stop trigger (closed above channel)
    		if (null != closeOrderShortStop)
			{
				if (closeOrderShort == null
					&& Close[0] > channelHigh
					)
				{
					Print(Time + " Closed above channel on a short, buying to cover position");
					closeOrderShort = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Mkt");
				}
				else
				{
					double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];
					if (atStopLoss1 > 0) // Trailing Stop)
					{
						stopPrice = Math.Min(closeOrderShortStop.StopPrice, lowSinceOpen + (atStopLoss1 * TickSize));
						ChangeOrder(closeOrderShortStop, closeOrderShortStop.Quantity, closeOrderShortStop.LimitPrice, stopPrice);
						specialStop = true;
					}
					else if (abeProfitTrigger > 0)	// sets stop loss to B/E when price moves to this target
					{
						if ((Position.AvgPrice - lowSinceOpen) >= abeProfitTrigger * TickSize)
						{
							//stopPrice = Math.Max(closeOrderLongStop.StopPrice, highSinceOpen - (atStopLoss1 * TickSize));
							stopPrice = Position.AvgPrice;
							ChangeOrder(closeOrderShortStop, closeOrderShortStop.Quantity, closeOrderShortStop.LimitPrice, stopPrice);
							specialStop = true;
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
				if (execution.Name == "Exit on close")
				{
					Print(Time + " " + execution);
					if (execution.MarketPosition == MarketPosition.Short)	// not sure why, but if I was long, close on exit is showing short
					{
						profitPoints += execution.Quantity * (execution.Price - entryOrderLong.AvgFillPrice - 0.005);
						Print(Time + " trade profit " + execution.Quantity * (execution.Price - entryOrderLong.AvgFillPrice - 0.005));
					}
					if (execution.MarketPosition == MarketPosition.Long)
						profitPoints += execution.Quantity * (entryOrderShort.AvgFillPrice - execution.Price - 0.005);
					Print(Time + " tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
					ResetTrade();

				} 
				else
				{
					Print(Time + " -->> OnExecution.Order is null");
					Print(Time + " -->> " + execution);
				}
				return;
			}
			
			Print(Time + " execution.Order: " + execution.Order.ToString());
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (entryOrderLong != null && entryOrderLong == execution.Order)
			{
				//Print(Time + " entryOrderLong: " + entryOrderLong);
				if (closeOrderLongLimit == null) 
				{
					// the first time this is called (1st trade) it will shrink the channel biased to 1st entry side
					//channelLow = channelHigh - adjustedChannelSize * TickSize;
					
					limitPrice = 0;
					//stopPrice = channelHigh - (adjustedChannelSize + 3) * TickSize;
					stopPrice = channelLow - 2 * TickSize;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderLongStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Stop");

					stopPrice = 0; 
					//breakEvenPrice = entryOrderLong.AvgFillPrice + Math.Abs(profitPoints/entryOrderLong.Quantity); 
					//DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);
					
					limitPrice = entryOrderLong.AvgFillPrice + profitTarget * TickSize;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					
					closeOrderLongLimit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Limit");
					
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
					// after thinking about this, they keep the channel size the same, so losses will
					// be based on the full channel, the profit target will used the adjustedChannelSize.
					//channelHigh = channelLow + adjustedChannelSize * TickSize;
					
					limitPrice = 0;
					//stopPrice = channelLow + (adjustedChannelSize + 3) * TickSize;
					stopPrice = channelHigh + 2 * TickSize;
					//Print("Setting Stop");
					//stopPrice = entryOrderShort.AvgFillPrice + (adjustedChannelSize + 3) * TickSize;
					//Print("stopPrice: " + stopPrice);
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderShortStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Stop");
					
					stopPrice = 0; 
					breakEvenPrice = entryOrderShort.AvgFillPrice; // - Math.Abs(profitPoints/entryOrderShort.Quantity);
					//DrawDot(CurrentBar + "breakEvenPrice", false, 0, breakEvenPrice, Color.Yellow);

					limitPrice = entryOrderShort.AvgFillPrice - profitTarget * TickSize;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					//}					
					
					closeOrderShortLimit = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Limit");
					
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

			// ===================================================
			//   Trade hit Long stop or reversal close, create reversed trade Short
			// ===================================================
			if ((closeOrderLongStop != null || closeOrderLong != null) 
				&& (closeOrderLongStop == execution.Order || closeOrderLong == execution.Order))
			{
				int closeQty = entryOrderLong.Quantity;
				
				if (closeOrderLongStop == execution.Order)
				{
					Print(Time + " xx_ " + closeOrderLongStop.AvgFillPrice);
					//Print(Time + " xx_ " + entryOrderLong.AvgFillPrice);
					profitPoints += (-0.005 + closeQty * (closeOrderLongStop.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				else if (closeOrderLong == execution.Order)
				{
					profitPoints += (-0.005 + closeQty * (closeOrderLong.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				
				Print(Time + " Long Close Profit: " + profitPoints);
				
				ResetTrade();
			}			
			
			// ===================================================
			//   Trade hit Long limit, no more trades for the day
			// ===================================================
			if (closeOrderLongLimit != null && closeOrderLongLimit == execution.Order)
			{
				profitPoints += closeOrderLongLimit.Quantity * (closeOrderLongLimit.AvgFillPrice - entryOrderLong.AvgFillPrice - 0.005);
				Print(Time + " closeOrderLongLimit tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				ResetTrade();
			}			

			// ==================================================
			//   Trade hit Short stop or reversal, create reversed trade Long
			// ==================================================
			if ((closeOrderShortStop != null || closeOrderShort != execution.Order)
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
				
				Print(Time + " Short Close Profit: " + profitPoints);

				ResetTrade();
				
			}			
			
			// ===================================================
			//   Trade hit Short limit, no more trades for the day
			// ===================================================
			if (closeOrderShortLimit != null && closeOrderShortLimit == execution.Order)
			{
				profitPoints += closeOrderShortLimit.Quantity * (entryOrderShort.AvgFillPrice - closeOrderShortLimit.AvgFillPrice - 0.005);
				Print(Time + " closeOrderShortLimit tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				ResetTrade();
			}			
		}
		
		private void ResetTrade() {
			
			Print("--------------- ResetTrade --------------");
			entryOrderLong = null;
			entryOrderShort = null;

			closeOrderLongStop = null;
			closeOrderLongLimit = null;
			closeOrderShortStop = null;
			closeOrderShortLimit = null;
			closeOrderLong = null;
			closeOrderShort = null;
			specialStop = false;
		
			adjustedChannelSize = 0;
		}
		
        #region Properties
		
        [Description("Minimum Channel Size in ticks")]
        [GridCategory("Parameters")]
        public int ChannelMin
        {
            get { return channelMin; }
            set { channelMin = Math.Max(1, value); }
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
		
        [Description("Increases the period to get to a min channel height")]
        [GridCategory("Parameters")]
        public bool EnablePeriodExpansion
        {
            get { return enablePeriodExpansion; }
            set { enablePeriodExpansion = value; }
        }
		
        [Description("Decreases the period to get to a max channel height")]
        [GridCategory("Parameters")]
        public bool EnablePeriodCompression
        {
            get { return enablePeriodCompression; }
            set { enablePeriodCompression = value; }
        }
		
		// ===========================================================
		// Begin ATM Stop Strategy Settings
		// ===========================================================
		
        [Description("Auto Break Even Profit Trigger, ignored if AbeProfitTrigger = 0")]
        [GridCategory("Parameters")]
        public int AbeProfitTrigger
        {
            get { return abeProfitTrigger; }
            set { abeProfitTrigger = Math.Max(0, value); }
        }
		
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
/*
        [Description("Stop Loss")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
        }
		*/
        [Description("Auto trail Stop loss, ignored if AtStopLoss1 = 0")]
        [GridCategory("Parameters")]
        public int AtStopLoss1
        {
            get { return atStopLoss1; }
            set { atStopLoss1 = Math.Max(0, value); }
        }
		/*
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
