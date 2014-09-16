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
    /// This is a strategy to determine if a intrument & the time frame trade with the trend or against it. 
	/// Strategy based on Chapter 4 of the book Building Reliable Trading Systems
	/// 
	/// Note: currently this strategy is worthless, 
	/// I was using concepts in chapt 4 trying to replicate the results in the book.
	/// Chapter 3 is where trend/contra trend talk it about
	/// 
    /// </summary>
    [Description("This is a strategy to determine if a intrument & the time frame trade with the trend or against it. Strategy based on Chapter 2 of the book Building Reliable Trading Systems")]
    public class TrendOrContra : Strategy
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		int rsiSmooth = 3;
		int rsiPeriod = 80;
		int barsToHold = 35;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();
			// Order Handling enforce only 1 position long or short at any time
			EntriesPerDirection = 200; 
			EntryHandling = EntryHandling.AllEntries;
			ExitOnClose = false;

			CalculateOnBarClose = true;
			//BarsRequired = maPeriod + periodsBack;
			
			Add(RSI(rsiPeriod, rsiSmooth));
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
//			if (Position.MarketPosition == MarketPosition.Flat)
//			{
				if (RSI(rsiPeriod, rsiSmooth).Avg[0] > 53)
				{
					EnterLong(CurrentBar.ToString());
				}
//			} 
//			else
//			{
				//if (BarsSinceEntry() > barsToHold)
				
				try
				{
					ExitLong((CurrentBar - barsToHold).ToString());		
					double tradeHigh = High[HighestBar(High, barsToHold)];
					Print("Max Profit," + (tradeHigh - Open[barsToHold]));
				}
				catch 
				{
				}
//			}
        }

        #region Properties
        [Description("RSI Period")]
        [GridCategory("Parameters")]
        public int RsiPeriod
        {
            get { return rsiPeriod; }
            set { rsiPeriod = Math.Max(1, value); }
        }

        [Description("RSI Smooth")]
        [GridCategory("Parameters")]
        public int RsiSmooth
        {
            get { return rsiSmooth; }
            set { rsiSmooth = Math.Max(0, value); }
        }

        [Description("Number of bars to hold position")]
        [GridCategory("Parameters")]
        public int BarsToHold
        {
            get { return barsToHold; }
            set { barsToHold = Math.Max(1, value); }
        }
		
        #endregion
    }
}
