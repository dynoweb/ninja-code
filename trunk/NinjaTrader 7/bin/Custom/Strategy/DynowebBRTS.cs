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
	/// Refer to my Chapter5Stock.cs strategy for details
    /// </summary>
    [Description("Optimized for CL Trading")]
    public class DynowebBRTS : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int donchianPeriod = 450; // Default setting for DonchianPeriod
        private bool longOnly = false; // Default setting for LongOnly
        private double stopLoss = 0; // Default setting for StopLoss
        private int exitAfterXBars = 0; // Default setting for ExitAfterXBars
        private double profitTarget = 0; // Default setting for ProfitTarget
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
			ExitOnClose = false;
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

//			if (Position.MarketPosition == MarketPosition.Long)
//			{					
//				
//				if (ExitAfterXBars == 0 && DonchianPeriod != 0 
//					&& Close[0] > MAX(Close, DonchianPeriod)[1])
//						ExitLong();								
//			}
//			if (Position.MarketPosition == MarketPosition.Short)
//			{					
//				
//				if (ExitAfterXBars == 0 && DonchianPeriod != 0 
//					&& Close[0] < MIN(Close, DonchianPeriod)[1])
//						ExitShort();								
//			}
//			
//			
//			if (Position.MarketPosition == MarketPosition.Flat)
//			{				
				// using reverse entry signals
				if (Close[0] > MAX(Close, DonchianPeriod)[1])
				{
					if (Position.MarketPosition == MarketPosition.Short)
						ExitShort();
					if (enterFilter()) EnterLong();
				}
				if (LongOnly)
				{			
					if (Position.MarketPosition == MarketPosition.Long
						&& Close[0] < MIN(Close, DonchianPeriod)[1])
						ExitLong();								
				}
				else 
					if (Close[0] < MIN(Close, DonchianPeriod)[1])
					{
						if (Position.MarketPosition == MarketPosition.Long)
							ExitLong();
						if (enterFilter()) EnterShort();
					}
//			} 
			
			if (Position.MarketPosition != MarketPosition.Flat)
			{
				if (ExitAfterXBars != 0 && BarsSinceEntry() >= ExitAfterXBars)
					ExitLong();
			}				
        }
		
		private bool enterFilter() 
		{
			if (Time[0].DayOfWeek == DayOfWeek.Wednesday
				&& ToTime(Time[0]) >= 20000 
				&& ToTime(Time[0]) <= 133000)
		        return true;
			else
				return false;
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
