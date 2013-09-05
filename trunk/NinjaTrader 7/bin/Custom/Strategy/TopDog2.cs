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
	/// Written by Rick Cromer (dynoweb)
	/// 
	/// Strategy:
    /// Code based from the dynoweb strategy code that jump in a breakout based on an opening Range. 
	/// So that it can reuse the unmanage trade code
	///  
	/// The strategy will be based on the following rules:
	/// 1 - Only trade with the trend - this will not try to identify trend reversals
	/// 	a) look for continuation patterns, like slo-mo 
	///     b) look for strong break outs of a range
	/// 2 - Confirm entries using these indicators
	///     a) MACD dad or mom with trend
	///     b) stochastic leaving over bought/sold zones
	/// 3 - Exit when MACD dad changes color or stochastic crosses back across mid line
	/// 4 - Max trades in one trend is 2    
	///
	/// NQ $20/pt 0.25pt/tick $5/tick
	/// 
	/// Press F5 to compile
	/// 
	/// Problems 
	/// 
	/// Testing
	/// 
	/// Changes
    /// </summary>
    [Description("Uses NQ 450 tick and unmanaged order methods")]
    public class TopDog2 : Strategy
    {
        #region Variables

		// Exposed variables
       // private int abeProfitTrigger = 20; // ATM Strategy Variables - sets stop loss to B/E when price moves to this target
		private bool enableSummary = false;
		private int sessionHighLow = 0;
		private bool useTrailing = true;
		private bool tradeLong = true;
		private bool tradeShort = true;

		// Trade Summary
		private double lastcumprofit = 0;
		private	Font textFont = new Font("Arial",8,FontStyle.Regular);
		
        // User defined variables (add any user defined variables below)
        private int startQty = 1; // Start Order Qty, converts to new Qty after reversal
		private int qty = 1;
		private int tradeCount = 0;
		private double shortStop = 0;
		private double longStop = 0;
		private double profitPoints = 0;
		private bool enableTraceOrders = false;
		private double breakEvenPrice = 0;
		private int barNumberOfOrder = 0;		
		private int barNumberSinceFilled  = 0;
		private bool debug = true;

		static int FALLING = -1;
		static int RISING = 1;
		static int NEUTRAL = 0;
		
		// 
		// indicator variables
		// 
		
		// Stochastic indicator settings
		int stocD = 3;
		int stocK = 5;
		int stocS = 2;		
		
		// MACD indicator settings
		int macdFast = 5;
		int macdSlow = 20;
		int macdSmooth = 30;

		// used for long term trend, when signal crosses look to open
		// long band is when signal is above red slowSignalPeriod - slow signal
        private int signalLinePeriod = 10; // Default setting for SignalLinePeriod
		
		//int trendBand = 120;
        // User defined variables (add any user defined variables below)
		int trendPlotPeriod = 100;		// HMA liked 30 long 48 short, SMA liked 6, entry price trigger
		int threshold = 50;
		
		int slowSignalPeriod = 14;	// slow signal
		int HmaPeriod2 = 50;

		private MAType selectedMAType = MAType.HMARick; 
		HMARick trendPlot;
		//private MAType selectedMAType = MAType.SMARick; 
		//SMARick trendPlot;
		
//		HMARick signal;				// DimGray
//		HMARick dynamicTrend;
//		EMA slowSignal;
		StochasticsCycles stoc;
		MACDRick2 macd;

		// 
		// order variables
		// 
		private IOrder entryOrderLong = null;
		private IOrder entryOrderShort = null;
		private IOrder closeOrderLongLimit = null;
		private IOrder closeOrderLongStop = null;
		private IOrder closeOrderShortLimit = null;
		private IOrder closeOrderShortStop = null;
		private IOrder closeOrderLong = null;	
		private IOrder closeOrderShort = null;	
		
		private string orderPrefix = "TopDog2";
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();
			// re-trigger on each tick - may not be required with unmanaged order entry, yet to be determined
            CalculateOnBarClose = false;
			// Use Unmanaged order methods
    		Unmanaged = true;
			
			// Triggers the exit on close function 60 min prior to session end 
			// Note: This property is a real-time only property.
			ExitOnClose = true;
			ExitOnCloseSeconds = 3600;	// 60 min before session close
			TimeInForce = Cbi.TimeInForce.Day;
			
			Add(PitColor(Color.Black, 83000, 25, 161500));
			Add(TickCounter(true, false));
			
			switch (selectedMAType)
			{
				case MAType.HMARick: 
				{
					trendPlot = HMARick(TrendPlotPeriod, Threshold);
					trendPlot.DownColor = Color.Purple;
					trendPlot.UpColor = Color.DarkCyan;
					trendPlot.NeutralColor = Color.WhiteSmoke;
					trendPlot.Plots[0].Pen.DashStyle = DashStyle.Solid;
					trendPlot.Plots[0].Pen.Width = 2;
					trendPlot.PaintPriceMarkers = false;
					Add(trendPlot);
					break;
				}
//				case MAType.SMARick: 
//				{
//					trendPlot = SMARick(TrendPlotPeriod);
//					trendPlot.PaintPriceMarkers = false;
//					Add(trendPlot);
//					break;
//				}
			}					
			
			
//			signal = HMARick(signalLinePeriod, 60);
//			signal.Plots[0].Pen.Color = Color.DimGray;
//			signal.Plots[0].Pen.DashStyle = DashStyle.Solid;
//			signal.Plots[0].Pen.Width = 2;
//			signal.PaintPriceMarkers = false;
//			Add(signal);

//			slowSignal = EMA(slowSignalPeriod);
//			slowSignal.Plots[0].Pen.Color = Color.DimGray;
//			slowSignal.PaintPriceMarkers = false;
//			Add(slowSignal);

			//MAType maType = SMARick.class;
			stoc = StochasticsCycles(stocD, stocK, MAType.HMARick, false, false, stocS, Threshold, 3, TrendPlotPeriod);
            Add(stoc);
			
			macd = MACDRick2(macdFast, macdSlow, macdSmooth);
			macd.Lines[0].Pen.Width = 10;
			Add(macd);

			
//
//			Add(HMARick(70, 70));


//			stoc = StochasticsCycles(stocD, stocK, false, false, stocS, Threshold, 3, TrendPlotPeriod);
//			stoc.Plots[0].Pen.Color = Color.Blue; // D color
//			stoc.Plots[0].Pen.Width = 2;
//			stoc.Lines[0].Pen.Color = Color.Black; // Lower
//        	stoc.Lines[0].Pen.DashStyle = DashStyle.Dot;
//			stoc.Lines[0].Pen.Width = 2;
//			stoc.Plots[1].Pen.Color = Color.Black; // K color
//			stoc.Lines[1].Pen.Color = Color.Black; // Upper
//			stoc.Lines[1].Pen.DashStyle = DashStyle.Dot;
//			stoc.Lines[1].Pen.Width = 2;
//          Add(stoc);
			
    		TraceOrders = enableTraceOrders; 
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Do not calculate if we don't have enough bars 
			//if (CurrentBar < Period) return;
			
			double limitPrice = 0;
			double stopPrice = 0;
			
			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				if (debug) Print("=======================");
				tradeCount = 0;
				profitPoints = 0;
				qty = startQty;
				ResetTrade();
				if (debug) Print(Time + " vars reset");
			}
			
			// do this when in a trade
			if (Position.MarketPosition == MarketPosition.Long || Position.MarketPosition == MarketPosition.Short)
			{
				OrderManagement();
			}
			// If more than x bars has elapsed, cancel the entry order
			else if (FirstTickOfBar && CurrentBar > barNumberOfOrder) 	
			{				
				CancelAllOrders();
			}

			if (				
//				&& tradeCount <= maxReversals
//				&& tradeCount == 0
				Position.MarketPosition == MarketPosition.Flat
				&& entryOrderLong == null 
				&& entryOrderShort == null
//				&& profitPoints == 0
				//&& ToTime(Time[0]) >= (ToTime(hour, minute, 0) + 10000) // add an hour before we start the trades
				) 
			{
				//if (trendPlot.TrendSet[0] == RISING
				//if (trendPlot.Plots[0] > trendPlot.Plots[1]
				if (tradeLong 
					//&& Rising(trendPlot)
					&& trendPlot.TrendSet[0] == RISING
					&& stoc.K[0] >= stoc.K[1] 
					&& stoc.K[1] <= 20
					&& Math.Abs(macd.Mom[0]) > 1
					//&& stoc.D[0] >= stoc.D[1] 
					//&& stoc.D[1] <= 20
					) 
				{					
					int barsAgo = LowestBar(Low, 5);
					double lowStopPrice = Low[barsAgo] - 2 * TickSize;					
					
					stopPrice = High[0] + 1 * TickSize;
					limitPrice = High[0] + 1 * TickSize;
					//DrawDot(CurrentBar + "Long_stopPrice", false, 0, stopPrice, Color.White);
					//DrawDot(CurrentBar + "Long_limitPrice", false, 0, limitPrice, Color.Black);
					
					// check to make sure that my stop won't be more than 3 pts
					if (limitPrice - lowStopPrice <= 10 * TickSize)
					{
						entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Long");						
						barNumberOfOrder = CurrentBar;
						if (debug) Print(Time + " macd.Avg: " + macd.Avg[0] + " macd.Default: " + macd.Default[0] + " macd.Diff: " + macd.Diff[0] + " macd.Mom: " + macd.Mom[0]);
					}
					else
					{
						DrawDot(CurrentBar + "Long_limitPrice_disqualified", false, 0, limitPrice, Color.Yellow);
					}
				}
				
				if (tradeShort 
					//&& Falling(trendPlot)
					&& trendPlot.TrendSet[0] == FALLING
					&& stoc.K[0] <= stoc.K[1] 
					&& stoc.K[1] >= 80
					
					//&& stoc.D[0] <= stoc.D[1] 
					//&& stoc.D[1] >= 80
					) 
				{
					int barsAgo = HighestBar(High, 5);
					double highStopPrice = High[barsAgo] + 2 * TickSize;					
					DrawDot(CurrentBar + "Short_highStopPrice", false, 0, highStopPrice, Color.Cyan);
					
					stopPrice = Low[0] - 1 * TickSize;
					limitPrice = Low[0] - 1 * TickSize;
					//if (debug) Print(Time + " highStopPrice - limitPrice = " + (highStopPrice - limitPrice));
					//DrawDot(CurrentBar + "Short_stopPrice", false, 0, stopPrice, Color.White);
					//DrawDot(CurrentBar + "Short_limitPrice", false, 0, limitPrice, Color.Black);

					// check to make sure that my stop won't be more than 3 (pts) x 4 ticks = 12
					if (highStopPrice - limitPrice <= (10 * TickSize))
					{
						entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Enter Short");						
						barNumberOfOrder = CurrentBar;
					}
					else
					{
						DrawDot(CurrentBar + "Short_limitPrice_disqualified", false, 0, limitPrice, Color.Yellow);
					}
				}				
			}			
			
			if (enableSummary)
			{
				if (lastcumprofit != Performance.AllTrades.TradesPerformance.Currency.CumProfit)
				{
					Print(Time[0] + " " + Performance.LongTrades.TradesPerformance.Currency.CumProfit.ToString("0") + " + " + Performance.ShortTrades.TradesPerformance.Currency.CumProfit.ToString("0") + " = " + Performance.AllTrades.TradesPerformance.Currency.CumProfit.ToString("0"));
					lastcumprofit = Performance.AllTrades.TradesPerformance.Currency.CumProfit;
				}
			
				string last = "";
			    if (Performance.AllTrades.Count > 1)
			    {
					Trade lastTrade = Performance.AllTrades[Performance.AllTrades.Count - 1];
					last = "\nLast: " + lastTrade.Quantity + " x " + Instrument.MasterInstrument.Round2TickSize(lastTrade.ProfitPoints) + " points";
 				}

				// http://msdn.microsoft.com/en-us/library/system.string.format.aspx
				DrawTextFixed("pnl","All: " + Performance.AllTrades.TradesCount + " W:" + (Performance.AllTrades.WinningTrades.Count) + " L: " + (Performance.AllTrades.LosingTrades.Count) +
					" PNL: $" + (int) Performance.AllTrades.TradesPerformance.Currency.CumProfit +
					"\nLongs: " + Performance.LongTrades.TradesCount + " W: " + (Performance.LongTrades.WinningTrades.Count) + " L: " + (Performance.LongTrades.LosingTrades.Count) +
					" PNL: $" + (int) Performance.LongTrades.TradesPerformance.Currency.CumProfit +
					"\nShorts: " + Performance.ShortTrades.TradesCount + " W: " + (Performance.ShortTrades.WinningTrades.Count) + " L: " + (Performance.ShortTrades.LosingTrades.Count) +
					" PNL: $" + (int) Performance.ShortTrades.TradesPerformance.Currency.CumProfit +
					"\nAccount: " + Account.Name +
					"\nAccount Balance: $" + GetAccountValue(AccountItem.CashValue) + last 
					,TextPosition.TopLeft,Color.Black,textFont,Color.Green,Color.White,80);
			}
        }
		
		private double calcLongStop()
		{
			// find low since the last high
			int recentHighBarsAgo = recentHigh();
			int barsAgo = LowestBar(Low, recentHighBarsAgo);
			//DrawDot(CurrentBar + "longStop", false, barsAgo, Low[barsAgo] - 2 * TickSize, Color.Red);
			return Low[barsAgo] - 2;
		}
		
		/// <summary>
		/// Use stochastic D to find the recent high
		/// 
		/// returns bars ago since the high
		/// </summary>
		int recentHigh()
		{
			// find where D went positive (start of cycle high)
			int i = 2;
			while (CurrentBar - i >  0 && !CrossAbove(stoc.D, 45, i))
			{
				i++;
			}
			int barsAgo = HighestBar(High, i);
			//DrawDot(CurrentBar + "high", false, barsAgo, High[barsAgo] + 5 * TickSize, Color.Green);
			return barsAgo;
		}
		

		protected override void OnOrderUpdate(IOrder order) 
		{ 
			//if (stopLossOrder != null && stopLossOrder == order) 
			//{
				// Rejection handling 
				if (order.OrderState == OrderState.Rejected) 
				{
					// Stop loss order was rejected !!!! 
					// Do something about it here 
					Print(Time + " Order Rejected -- " + order);
				} 
			//}
		}

		
		/// <summary>
		/// 
		/// </summary>
		private void OrderManagement()
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
			double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];

			if (false)
			{
			
			// Change long close order to close if stop trigger (closed below channel)
    		if (null != closeOrderLongStop)
			{
				if (closeOrderLong == null
					&& barNumberSinceFilled != barNumberSinceFilled
					)
				{
					//if (debug) Print(Time + " Closed below channel on a long, selling position");
					closeOrderLong = SubmitOrder(0, OrderAction.Sell, OrderType.Market, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long");
				} 

				// keep from bouncing on one bar
				if (barNumberSinceFilled != CurrentBar && stopPrice == 0)
				{
//					stopPrice = channelLow - 1 * TickSize;
					
					//if (closeOrderLongStop.StopPrice != stopPrice)
					if ((int) (closeOrderLongStop.StopPrice * 100) != (int) (stopPrice * 100))
					{
						ChangeOrder(closeOrderLongStop, closeOrderLongStop.Quantity, closeOrderLongStop.LimitPrice, stopPrice);
						if (debug) DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					}
				}
			}
			
			// Change short close order to close if stop trigger (closed above channel)
    		if (null != closeOrderShortStop)
			{
				if (closeOrderShort == null
					&& barNumberSinceFilled != barNumberSinceFilled
					)
				{
					//if (debug) Print(Time + " Closed above channel on a short, buying to cover position");
					closeOrderShort = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Market, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short");
					//if (debug) Print(Time + " Submitted order to buy to cover position");
				}
				
				// keep from bouncing on one bar
				if (barNumberSinceFilled != CurrentBar)
				{
//					stopPrice = channelHigh + 1 * TickSize;
					
					// rounding numbers, doubles wouldn't match
					//int sp = (int) (stopPrice * 100);
					if ((int) (closeOrderShortStop.StopPrice * 100) != (int) (stopPrice * 100))
					{
						//if (debug) Print(Time + " closeOrderShortStop.StopPrice: " + closeOrderShortStop.StopPrice + " stopPrice: " + stopPrice);
						ChangeOrder(closeOrderShortStop, closeOrderShortStop.Quantity, closeOrderShortStop.LimitPrice, stopPrice);
						if (debug) DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
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
				Print(Time + " -->> OnExecution.Order is null");
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
					
					limitPrice = 0;					
//					stopPrice = entryOrderLong.AvgFillPrice - 10 * TickSize;					
					int barsAgo = LowestBar(Low, 5);
					stopPrice = Math.Min(Low[barsAgo] - 2 * TickSize, entryOrderLong.AvgFillPrice - 10 * TickSize);					
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderLongStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Stop");
					
					limitPrice = entryOrderLong.AvgFillPrice + 1.5 * (entryOrderLong.AvgFillPrice - stopPrice);
					stopPrice = 0; 
					if (debug) Print(Time + " limitPrice: " + limitPrice);
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrderLongLimit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Close Long Limit");
					
					barNumberSinceFilled = CurrentBar;
				} 
				else
				{
					Print(Time + " OnExecution has been fired and stop and limits for long have already been submitted");
					if (debug) Print(Time + " closeOrderLongStop: " + closeOrderLongStop);
					if (debug) Print(Time + " closeOrderLongLimit: " + closeOrderLongLimit);
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
					
					limitPrice = 0;
					stopPrice = entryOrderShort.AvgFillPrice + 10 * TickSize;
					//int barsAgo = HighestBar(High, 5);
					//stopPrice = High[barsAgo] + 2 * TickSize;				
					
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeOrderShortStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Stop");
					
					//limitPrice = entryOrderShort.AvgFillPrice - 1 * (stopPrice - entryOrderShort.AvgFillPrice);
					limitPrice = entryOrderShort.AvgFillPrice - 12 * TickSize;
					stopPrice = 0; 
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeOrderShortLimit = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Close Short Limit");
					
					barNumberSinceFilled = CurrentBar;
				} 
				else
				{
					Print(Time + " OnExecution has been fired and stop and limits for Short have already been submitted");
					if (debug) Print(Time + " closeOrderShortStop: " + closeOrderShortStop);
					if (debug) Print(Time + " closeOrderShortLimit: " + closeOrderShortLimit);
				}
			} 

			// ===================================================
			//   Trade hit Long stop or reversal close, create reversed trade Short
			// ===================================================
			if (
				   (closeOrderLongStop != null && closeOrderLongStop == execution.Order)
				|| (closeOrderLong != null && closeOrderLong == execution.Order)
				|| (closeOrderLongLimit != null && closeOrderLongLimit == execution.Order)
				)
			{
				int closeQty = execution.Quantity;
				
				if (closeOrderLongStop == execution.Order)
				{
					//closeQty = closeOrderLongStop.Quantity;
					profitPoints += (-0.005 + closeQty * (closeOrderLongStop.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				else if (closeOrderLong == execution.Order)
				{
					//closeQty = closeOrderLong.Quantity;
					profitPoints += (-0.005 + closeQty * (closeOrderLong.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				else if (closeOrderLongLimit == execution.Order)
				{
					profitPoints += (-0.005 + closeOrderLongLimit.Quantity * (closeOrderLongLimit.AvgFillPrice - entryOrderLong.AvgFillPrice));
				}
				
				Print(Time + " Long Close Profit: " + profitPoints);
				Print(Time + " tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				
				ResetTrade();
				tradeCount++;
			}			
			
			// ==================================================
			//   Trade hit Short stop or reversal, create reversed trade Long
			// ==================================================
			if (   (closeOrderShortStop != null && closeOrderShortStop == execution.Order)
				|| (closeOrderShort != null && closeOrderShort != execution.Order)
				|| (closeOrderShortLimit != null && closeOrderShort != execution.Order)
				)
			{
				int closeQty = execution.Quantity;
				
				profitPoints += (-0.005 + closeQty * (entryOrderShort.AvgFillPrice - execution.Order.AvgFillPrice));
				
				Print(Time + " tradeCount: " + tradeCount + " Profit: " + Instrument.MasterInstrument.Round2TickSize(profitPoints));
				Print(Time + " Short Close Profit: " + profitPoints);
				ResetTrade();

				tradeCount++;
			}			
		}
		
		private void ResetTrade() {
			
			Print("------------------------");
			
			CancelAllOrders();

			closeOrderLongStop = null;
			closeOrderLongLimit = null;
			closeOrderShortStop = null;
			closeOrderShortLimit = null;
			closeOrderLong = null;
			closeOrderShort = null;
		}
		
		private void CancelAllOrders()
		{
			if (entryOrderLong != null)
			{
				CancelOrder(entryOrderLong);
				if (debug) Print(Time + " Canceling long order ");
				entryOrderLong = null;
			}
			if (entryOrderShort != null)
			{
				CancelOrder(entryOrderShort);
				if (debug) Print(Time + " Canceling short order ");
				entryOrderShort = null;
			}
		}
		
        #region Properties
		
		/// </summary>
		[Description("Slope threshold as percentage of average true range")]
		[GridCategory("Trend Parameters")]
		[Gui.Design.DisplayNameAttribute("Neutral Threshold")]
		public int Threshold 
		{
			get { return threshold; }
			set { threshold = Math.Max(0, value); }
		}

		[Description("Number of Periods used to determine the Trend Plot")]
		[GridCategory("Trend Parameters")]
		[Gui.Design.DisplayNameAttribute("Trend Plot Period")]
		public int TrendPlotPeriod 
		{
			get { return trendPlotPeriod; }
			set { trendPlotPeriod = Math.Max(1, value); }
		}

		[Description("Moving Average Type")]
		[GridCategory("Trend Parameters")]
		[Gui.Design.DisplayNameAttribute("Type for MA")]
		public MAType SelectedMAType
		{
			get { return selectedMAType; }
			set { selectedMAType = value; }
		}
		
        [Description("Enables the display of summary information, best turn off during backtesting")]
        [GridCategory("Parameters")]
        public bool EnableSummary
        {
            get { return enableSummary; }
            set { enableSummary = value; }
        }
		
        [Description("Enables extending channel to reach session high or lows, 0 ==  false, 1 == true")]
        [GridCategory("Parameters")]
        public int SessionHighLow
        {
            get { return sessionHighLow; }
            set { sessionHighLow = value; }
        }
		
        [Description("On the last trade, if BreakEventLast, trail from day b/e")]
        [GridCategory("Parameters")]
        public bool UseTrailing
        {
            get { return useTrailing; }
            set { useTrailing = value; }
        }
		
        [Description("Enables long trades")]
        [GridCategory("Parameters")]
        public bool TradeLong
        {
            get { return tradeLong; }
            set { tradeLong = value; }
        }
		
        [Description("Enables short trades")]
        [GridCategory("Parameters")]
        public bool TradeShort
        {
            get { return tradeShort; }
            set { tradeShort = value; }
        }
		

		
		// ===========================================================
		// Begin ATM Stop Strategy Settings
		// ===========================================================
		
		/*
        [Description("Auto Break Even Profit Trigger")]
        [GridCategory("Parameters")]
        public int AbeProfitTrigger
        {
            get { return abeProfitTrigger; }
            set { abeProfitTrigger = Math.Max(1, value); }
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
