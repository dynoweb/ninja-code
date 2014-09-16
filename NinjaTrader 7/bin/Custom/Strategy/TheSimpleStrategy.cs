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
    /// Based on a book by Markus Heitkoetter 
	/// The Simplae Strategy
	/// A Powerful Day Trading Strategy for Trading Futures, Stocks, ETFs and Forex.   
	/// www.RockwellTrading.com 
	/// 
	/// Uses Range bars - sample settings
	/// ES - 8 ticks (2 points)
	/// YM - 16 ticks	- Current database of data is 9/16/12 to 10/20/13
	/// TF - 16 ticks
	/// 6E - 12 ticks
	/// GC - 20 ticks
	/// CL - 20 ticks
	/// ZB - 4 ticks or 4/32
	/// ZN - 2 ticks or 2/64
	/// 
	/// Testing so far for /ES - 9/16/12 to 10/20/13 longs only with 11 tick range, +1120.00, all other ranges with inferior results but corresponded to distance from 11.
    /// </summary>
    [Description("Based on a book by Markus Heitkoetter - The Simple Strategy, A Powerful Day Trading Strategy for Trading Futures, Stocks, ETFs and Forex.   www.RockwellTrading.com ")]
    public class TheSimpleStrategy : Strategy
    {
        #region Variables
		
        // Property variables
        private int useRSIFilter = 1; 
		double stopFactor = 1.0;
		double limitFactor = 1.0;
		bool shortTrades = true;
		bool longTrades = true;
		bool reverseTrades = false;
			
        // User defined variables
		IOrder entryOrderLong = null;
		IOrder entryOrderShort = null;
		
		IOrder closeOrderLongStop = null;
		IOrder closeOrderLongLimit = null;
		
		IOrder closeOrderShortStop = null;
		IOrder closeOrderShortLimit = null;
		
		string orderPrefix = "SS";
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(Bollinger(2, 12));
			Bollinger(2, 12).Plots[0].Pen.Color = Color.Black;
			Bollinger(2, 12).Plots[0].Pen.DashStyle = DashStyle.Dash;
			Bollinger(2, 12).Plots[1].Pen.Color = Color.Transparent;
			Bollinger(2, 12).Plots[2].Pen.Color = Color.Black;
			Bollinger(2, 12).Plots[2].Pen.DashStyle = DashStyle.Dash;
			
			Add(RcMACD(12, 26, 9));
			
			Add(PitColor(Color.Black, 830000, 20, 161500));
	
			if (useRSIFilter == 1)
			{
				Add(RSI(7, 3));
				RSI(7, 3).Plots[0].Pen.Width = 2;
			}
			else
			{
				Add(Stochastics(3, 5, 2));
			}
			
			// Use Unmanaged order methods
			Unmanaged = true;
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double stopPrice = 0;
			double limitPrice = 0;
			int qty = 1;
			
			if (BarsPeriod.Id != PeriodType.Range)
			{
				DrawTextFixed("error msg", "This Strategy only works on Intraday Range Bars", TextPosition.BottomRight);
				return;
			}
			if (Bars.BarsSinceSession == 1)
			{
				ResetTrade();
			}
			
			RcMACD macd = RcMACD(12, 26, 9);
			RSI rsi = RSI(7, 3);
			Stochastics stoc = Stochastics(3, 5, 2);
			Bollinger bollinger = Bollinger(2, 12);
			
			if (UseRSIFilter == 1)
			{
				if (rsi[0] > 70)
					this.DrawTriangleUp(CurrentBar + "Up", false, 0, Low[0] - 2 * TickSize, Color.Green);
				else if (rsi[0] < 30)
					DrawTriangleDown(CurrentBar + "Down", false, 0, High[0] + 2 * TickSize, Color.Red);
			} 
			else
			{
				if (stoc.D[0] > 80)
					DrawTriangleUp(CurrentBar + "Up", false, 0, Low[0] - 2 * TickSize, Color.Green);
				else if (stoc.D[0] < 20)
					DrawTriangleDown(CurrentBar + "Down", false, 0, High[0] + 2 * TickSize, Color.Red);
			}
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				if (entryOrderLong != null)
				{
					CancelOrder(entryOrderLong);
					entryOrderLong = null;
				} 
				
				else
				
				// Long Entry Conditions
				if (longTrades && entryOrderLong == null
					&& macd.Mom[0] > 0 && macd.Mom[0] > macd.Avg[0]
					&& ((UseRSIFilter == 1 && rsi[0] > 70) || (UseRSIFilter == 0 && stoc.D[0] < 20))
					&& Rising(bollinger.Upper)
					&& Close[0] > (bollinger.Upper[0] - 2 * TickSize)
					//&& Time[0].DayOfWeek != DayOfWeek.Friday
					//&& (ToTime(Time[0]) >= 83000 && ToTime(Time[0]) <= 103000)
					)
				{
					DrawArrowUp(CurrentBar + "Long", false, 0, Low[0] - 8 * TickSize, Color.Green);
					if (Close[0] == High[0])
					{
						//DrawDot(CurrentBar + "marketPrice", false, 1, Close[0], Color.Gray);
						if (!reverseTrades)
							entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Long");
						else
							entryOrderLong = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "!Long");
					}
					else
					{
						stopPrice = Close[0] + 1 * TickSize;
						limitPrice = Close[0] + 1 * TickSize;
						//DrawDot(CurrentBar + "stopPrice", false, 1, stopPrice, Color.DarkGray);
						//DrawDot(CurrentBar + "limitPrice", false, 1, limitPrice, Color.Black);
						if (!reverseTrades)
							entryOrderLong = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Long");
						else
							entryOrderLong = SubmitOrder(0, OrderAction.SellShort, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "!Long");
					}
				} 

				// Short Entry Conditions
				if (shortTrades && entryOrderShort == null
					&& macd.Mom[0] < 0 && macd.Mom[0] < macd.Avg[0]
					&& ((UseRSIFilter == 1 && rsi[0] < 30) || (UseRSIFilter == 0 && stoc.D[0] > 80))
					&& Falling(bollinger.Lower)
					&& Close[0] < (bollinger.Lower[0] + 2 * TickSize))
				{
					DrawArrowDown(CurrentBar + "Short", false, 0, High[0] + 8 * TickSize, Color.Red);
					if (Close[0] == Low[0])
					{
						//DrawDot(CurrentBar + "marketPrice", false, 1, Close[0], Color.Gray);
						if (!reverseTrades)
							entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Short");
						else
							entryOrderShort = SubmitOrder(0, OrderAction.Buy, OrderType.Market, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "!Short");
					}
					else
					{
						stopPrice = Close[0] - 1 * TickSize;
						limitPrice = Close[0] - 1 * TickSize;
						//DrawDot(CurrentBar + "stopPrice", false, 1, stopPrice, Color.DarkGray);
						//DrawDot(CurrentBar + "limitPrice", false, 1, limitPrice, Color.Black);
						if (!reverseTrades)
							entryOrderShort = SubmitOrder(0, OrderAction.SellShort, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "Short");
						else
							entryOrderShort = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, qty, limitPrice, stopPrice, orderPrefix + "ocoEnter", "!Short");
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
				Print(Time + " -->> OnExecution.Order is null, resetting trades since it could be auto end of day close");
				ResetTrade();
				return;
			}
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (execution.Order.OrderState == OrderState.Filled)
			{
         		Print("FILLED: " + execution.ToString());
				
				if (entryOrderLong != null && entryOrderLong == execution.Order
					&& closeOrderLongLimit == null)
				{
					if (!reverseTrades)
					{
						limitPrice = 0;
						stopPrice = entryOrderLong.AvgFillPrice - stopFactor * BarsPeriod.Value * TickSize;
						DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
						closeOrderLongStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Stop");
						
						stopPrice = 0; 
						limitPrice = entryOrderLong.AvgFillPrice + limitFactor * BarsPeriod.Value * TickSize; 
						DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
						closeOrderLongLimit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Target");
					}
					else	// reverse strategy
					{
						limitPrice = 0;
						stopPrice = entryOrderLong.AvgFillPrice + stopFactor * BarsPeriod.Value * TickSize;
						DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
						closeOrderLongStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Stop");
						
						stopPrice = 0; 
						limitPrice = entryOrderLong.AvgFillPrice - limitFactor * BarsPeriod.Value * TickSize; 
						DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
						closeOrderLongLimit = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, entryOrderLong.Quantity, limitPrice, stopPrice, orderPrefix + "ocoLongClose", "Target");
					}
				} 
				else if ((closeOrderLongStop != null && closeOrderLongStop == execution.Order)
					|| (closeOrderLongLimit != null && closeOrderLongLimit == execution.Order))
				{
					ResetTrade();
				} 
				else if (entryOrderShort != null && execution.Order == entryOrderShort
					&& closeOrderShortLimit == null)
				{
					if (!reverseTrades)
					{
						limitPrice = 0;
						stopPrice = entryOrderShort.AvgFillPrice + stopFactor * BarsPeriod.Value * TickSize;
						DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
						closeOrderShortStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Stop");
						
						stopPrice = 0; 
						limitPrice = entryOrderShort.AvgFillPrice - limitFactor * BarsPeriod.Value * TickSize; 
						DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
						closeOrderShortLimit = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Target");
					}
					else	// reverse strategy
					{
						limitPrice = 0;
						stopPrice = entryOrderShort.AvgFillPrice - 1.5 * BarsPeriod.Value * TickSize;
						DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
						closeOrderShortStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Stop");
						
						stopPrice = 0; 
						limitPrice = entryOrderShort.AvgFillPrice + limitFactor * BarsPeriod.Value * TickSize; 
						DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
						closeOrderShortLimit = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, entryOrderShort.Quantity, limitPrice, stopPrice, orderPrefix + "ocoShortClose", "Target");
					}
				} 
				else
				{
					Print(Time + " Filled state but not known order");	
					Print(Time + " execution.Order: " + execution.Order.ToString());
				}
			} 
			else
			{
				Print(Time + " else");	
				Print(Time + " execution.Order: " + execution.Order.ToString());
			}
			
			
		}

		private void ResetTrade() 
		{
			entryOrderLong = null;
			entryOrderShort = null;
			closeOrderLongStop = null;
			closeOrderLongLimit = null;
			closeOrderShortStop = null;
			closeOrderShortLimit = null;
		}
		
        #region Properties
        [Description("Percent of bar size for targets")]
        [GridCategory("Parameters")]
        public double LimitFactor
        {
            get { return limitFactor; }
            set { limitFactor = value; }
        }

        [Description("Percent of bar size for stops")]
        [GridCategory("Parameters")]
        public double StopFactor
        {
            get { return stopFactor; }
            set { stopFactor = value; }
        }

        [Description("Enable Short Trades")]
        [GridCategory("Parameters")]
        public bool ShortTrades
        {
            get { return shortTrades; }
            set { shortTrades = value; }
        }

        [Description("Enable Long Trades")]
        [GridCategory("Parameters")]
        public bool LongTrades
        {
            get { return longTrades; }
            set { longTrades = value; }
        }

        [Description("Counter Trade")]
        [GridCategory("Parameters")]
        public bool ReverseTrades
        {
            get { return reverseTrades; }
            set { reverseTrades = value; }
        }
		
        [Description("1 - Use RSI to Filter Entry Points, 0 - Ignore RSI")]
        [GridCategory("Parameters")]
        public int UseRSIFilter
        {
            get { return useRSIFilter; }
            set { useRSIFilter = Math.Max(0, value); }
        }
        #endregion
    }
}
