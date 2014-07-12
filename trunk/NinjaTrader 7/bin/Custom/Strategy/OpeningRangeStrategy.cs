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
    /// Opening Range Breakout Strategy
    /// </summary>
    [Description("Opening Range Breakout Strategy")]
    public class OpeningRangeStrategy : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int enableStops = 0; 
        private int tradeSize = 50000; 
        private int useTarget2 = 1; 
		
        // User defined variables (add any user defined variables below)
		private OpeningRangeBreakout orb = null;
		private bool enableTrade = false;
		IOrder entryLongOrder = null;
		IOrder entryShortOrder = null;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(OpeningRangeBreakout(5));
			
			// Triggers the exit on close function 30 seconds prior to session end 
    		ExitOnClose = true;
    		ExitOnCloseSeconds = 60;
			
			// Applies to managed trades only
			//EntriesPerDirection = 2;
			//EntryHandling = EntryHandling.UniqueEntries;
			
			Unmanaged = true;
			TimeInForce = Cbi.TimeInForce.Day;

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// This should be true when the primary bar is being udpated
			if (BarsInProgress != 0)
		    	return;
			
			// If it's Friday, do not trade.
//		    if (Time[0].DayOfWeek == DayOfWeek.Friday)
//        		return;

			// Don't enter new trades after 9am CST
			if (ToTime(Time[0]) >= 90000)
			{
				if (entryLongOrder  != null) CancelOrder(entryLongOrder);
				if (entryShortOrder != null) CancelOrder(entryShortOrder);
				entryLongOrder = null;
				entryShortOrder = null;
				return;
			}
			
			if (Bars.FirstBarOfSession) 
			{
				//Print(Time + " first bar of session");
				enableTrade = true;
				entryLongOrder = null;
				entryShortOrder = null;
				return; // wait until second bar before considering a trade
			}
			
			// Buy Limit Order
			// A Buy Limit Order is an order to buy a specified number of shares of a 
			// stock at a designated price or lower, at a price that is below the current 
			// market price. Your limit price, in other words, is the maximum price you 
			// are willing to pay to purchase your shares.
			
			// Buy Stop Order
			// A Buy Stop Order is an order to buy a stock at a price above the current market 
			// price. Once a stock's price trades at or above the price you have specified, 
			// it becomes a Market Order to buy.
			
			orb = OpeningRangeBreakout(5);
			
			if (Position.MarketPosition == MarketPosition.Flat && enableTrade)
			{
				int shares = calcShares(TradeSize);

				if (entryLongOrder == null && (Time[0].DayOfWeek == DayOfWeek.Friday || Time[0].DayOfWeek == DayOfWeek.Tuesday))
				{
					if (GetCurrentBid() < orb.LongEntry[0])
					{
						entryLongOrder = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, shares, orb.LongEntry[0], orb.LongEntry[0], "dayTrade", "LongStopLimit");
					} 
					else
					{
						entryLongOrder = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, shares, orb.LongEntry[0], 0, "dayTrade", "LongLimit");
					}
				}
				
				if (entryShortOrder == null && (Time[0].DayOfWeek == DayOfWeek.Monday || Time[0].DayOfWeek == DayOfWeek.Tuesday))
				{
					if (GetCurrentBid() > orb.ShortEntry[0])
					{
						//this.DrawDot(CurrentBar + "shortstop", true, 0, orb.ShortEntry[0], Color.Cyan);
						entryShortOrder = SubmitOrder(0, OrderAction.Sell, OrderType.StopLimit, shares, orb.ShortEntry[0], orb.ShortEntry[0], "dayTrade", "ShortStopLimit");
					}
					else
					{		
						//this.DrawDot(CurrentBar + "shortlimit", true, 0, orb.ShortEntry[0], Color.Black);
						entryShortOrder = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, shares, orb.ShortEntry[0], 0, "dayTrade", "ShortStopLimit");
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
				//Print(Time + " -->> OnExecution.Order is null");
				return;
			}
			
			
			
			if (execution.Order.OrderState == OrderState.Filled)
			{
				enableTrade = false; // limit 1 trade / day
				if (execution.Order.OrderAction == OrderAction.Buy)
				{
					stopPrice = orb.LongStop[0];
					limitPrice = orb.LongTarget1[0];
					if (UseTarget2 == 1)
						limitPrice = orb.LongTarget2[0];
					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, 0, "closeDayTrade", "Close Long Limit");
					if (EnableStops == 1)
						SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 0, stopPrice, "closeDayTrade", "Close Long Stop");
				}
				
				if (execution.Order.OrderAction == OrderAction.Sell)
				{
					stopPrice = orb.ShortStop[0];
					limitPrice = orb.ShortTarget1[0];
					if (UseTarget2 == 1)
						limitPrice = orb.ShortTarget2[0];
					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, limitPrice, 0, "closeDayTrade", "Close Short Limit");
					if (EnableStops == 1)
						SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 0, stopPrice, "closeDayTrade", "Close Short Stop");
				}
			} 
			else 
			{
				Print(Time + " execution.Order: " + execution.Order.ToString());
			}
				
		}
		
		private int calcShares(int investment)
		{
			int shares = (int) Math.Floor(investment/Close[0]);
			//if (debugOn) Print(Time + " shares: " + shares);
			return shares; 
		}
		
        #region Properties
        [Description("Set to 1 to enable")]
        [GridCategory("Parameters")]
        public int EnableStops
        {
            get { return enableStops; }
            set { enableStops = Math.Max(0, value); }
        }

        [Description("Used to calculate number of shares")]
        [GridCategory("Parameters")]
        public int TradeSize
        {
            get { return tradeSize; }
            set { tradeSize = Math.Max(1, value); }
        }
		
        [Description("Set to 0 to use target 1, set to 1 to use target 2")]
        [GridCategory("Parameters")]
        public int UseTarget2
        {
            get { return useTarget2; }
            set { useTarget2 = Math.Max(0, value); }
        }

        #endregion
    }
}
