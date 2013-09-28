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
	/// Written by Rick Cromer
	/// Stock Strategy based on Chapter 6 of the book Building Reliable Trading Systems
	/// Trades daily time period stocks
    /// </summary>
    [Description("Trades daily time period stocks, based on Chapter 5 of the book Building Reliable Trading Systems. Note when running live make sure to set the the 'Stop & target submission' property to ByStrategyPosition")]
    public class Chapter6Stock : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int donchianPeriod = 8; // Default setting for DonchianPeriod
        private int exitAfterXBars = 8; // Default setting for ExitAfterXBars
        private int longerTermPeriod = 70; // Default setting for LongerTermPeriod
        private bool longOnly = true; // Default setting for LongOnly
		private int lowVolatilityThreshold = 3;
        private double profitTarget = 300.00; // Default setting for ProfitTarget, based on a $5000 investment
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			// Order Handling enforce only 1 position long or short at any time
			EntriesPerDirection = 1; 
			EntryHandling = EntryHandling.AllEntries;
			ClearOutputWindow();
			ExitOnClose = true;
			CalculateOnBarClose = true;
				
			// if this is set without a CalculationMode then it's a dollar value
			// for the entire trade (targetPrice = fillPrice + profitTarget/positionQuantity)
			if (profitTarget != 0) SetProfitTarget(profitTarget);
			
			Add(DonchianChannelClose(donchianPeriod));			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			// Checks to make sure we have at least x or more bars
			if (CurrentBar < LongerTermPeriod)
        		return;

			if (Position.MarketPosition == MarketPosition.Long)
			{					
				if (ExitAfterXBars != 0 && BarsSinceEntry() >= ExitAfterXBars)
					ExitLong();
				
				if (ExitAfterXBars == 0 && DonchianPeriod != 0 
					&& Close[0] > MAX(Close, DonchianPeriod)[1])
						ExitLong();								
			}
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{				
				if (Close[0] < MIN(Close, DonchianPeriod)[1]  // using reverse entry signals
					&& (LongerTermPeriod == 0 || Close[0] > Close[LongerTermPeriod])
					&& (LowVolatilityThreshold == 0 || !lowVolatility(LowVolatilityThreshold))
					)
				{
					EnterLong(calcShares(5000));
				}
//				if (!LongOnly && Close[0] > MAX(Close, DonchianPeriod)[1])
//				{
//					EnterShort(calcShares(5000));
//				}
			}
        }
		
		/// <summary>
		/// Detects if the price volativity is too low to trade.
		/// 
		/// This will calculate the standard deviation of 20 closing prices, if current
		/// standard deviation is less than minPercent of the close, return true. 
		/// </summary>
		/// <param name="investment"></param>
		/// <returns>true if the stock is trading in a very tight range</returns>
		private bool lowVolatility(int minPercent)
		{
			int Period = 20;
			double stdDevValue = StdDev(Close, Period)[0];
			Print (Time + " stdDev percent of price " + ((stdDevValue/Close[0]) * 100));
			return (stdDevValue/Close[0]) * 100 < minPercent ? true : false;
		}
		
		private int calcShares(int investment)
		{
			return (int) Math.Floor(investment/Close[0]);
//			Print(Time + " shares: " + i);
//			return 1; 
		}

		protected override void OnExecution(IExecution execution)
		{
			// Remember to check the underlying IOrder object for null before trying to access its properties
			if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
			{
				if (execution.Order.OrderAction == OrderAction.Buy)
				{
					//Print(Time + " OnExecution " + execution.ToString());
					//Print(Time + " OnExecution     " + execution.Order.ToString());
					if (ProfitTarget != 0)
					{
						//double profitPerShare = profitTarget/execution.Order.Quantity;
						//double target = Position.AvgPrice + profitPerShare;
						//Print(Time + " target " + target);
						//SetProfitTarget(profitTarget);
					}
				}
				if (execution.Order.OrderAction == OrderAction.Sell)
				{
					// Reset profit target to never hit value
					//if (Position.MarketPosition == MarketPosition.Flat)
					//	SetProfitTarget(99999999);
				}
			} 
		}
		
        #region Properties
        [Description("Donchian channel period, use 0 to ignore")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(0, value); }
        }

        [Description("Closes the trade after x bars, use 0 to ignore")]
        [GridCategory("Parameters")]
        public int ExitAfterXBars
        {
            get { return exitAfterXBars; }
            set { exitAfterXBars = Math.Max(0, value); }
        }

		[Description("Determines trend by price longerTermPeriod bars ago")]
        [GridCategory("Parameters")]
        public int LongerTermPeriod
        {
            get { return longerTermPeriod; }
            set { longerTermPeriod = Math.Max(0, value); }
        }
		
        [Description("Low Volatility Threshold, use 0 to ignore. If 1 stddev < x percent don't trade")]
        [GridCategory("Parameters")]
        public int LowVolatilityThreshold
        {
            get { return lowVolatilityThreshold; }
            set { lowVolatilityThreshold = Math.Max(0, value); }
        }

        [Description("Trade only longs")]
        [GridCategory("Parameters")]
        public bool LongOnly
        {
            get { return longOnly; }
            set { longOnly = value; }
        }

        [Description("Profit taget, use 0 to ignore")]
        [GridCategory("Parameters")]
        public double ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0, value); }
        }

        #endregion
    }
}
