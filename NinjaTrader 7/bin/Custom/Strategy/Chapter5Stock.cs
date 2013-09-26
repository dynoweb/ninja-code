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
	/// Stock Strategy based on Chapter 5 of the book Building Reliable Trading Systems
	/// Trades daily time period stocks
    /// </summary>
    [Description("Trades daily time period stocks, based on Chapter 5 of the book Building Reliable Trading Systems. Note when running live make sure to set the the 'Stop & target submission' property to ByStrategyPosition")]
    public class Chapter5Stock : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int donchianPeriod = 8; // Default setting for DonchianPeriod
        private bool longOnly = true; // Default setting for LongOnly
        private double stopLoss = 0.05; // Default setting for StopLoss
        private int exitAfterXBars = 7; // Default setting for ExitAfterXBars
        private double profitTarget = 300.00; // Default setting for ProfitTarget
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
			
			Add(DonchianChannel(donchianPeriod));			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
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
				// using reverse entry signals
				if (Close[0] < MIN(Close, DonchianPeriod)[1])
				{
					EnterLong(calcShares(5000));
				}
//				if (!LongOnly && Close[0] > MAX(Close, DonchianPeriod)[1])
//				{
//					EnterShort(calcShares(5000));
//				}
			}
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

        [Description("Trade only longs")]
        [GridCategory("Parameters")]
        public bool LongOnly
        {
            get { return longOnly; }
            set { longOnly = value; }
        }

        [Description("Loss in dollars of a $5000 investment")]
        [GridCategory("Parameters")]
        public double StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(0.000, value); }
        }

        [Description("Closes the trade after x bars, use 0 to ignore")]
        [GridCategory("Parameters")]
        public int ExitAfterXBars
        {
            get { return exitAfterXBars; }
            set { exitAfterXBars = Math.Max(0, value); }
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
