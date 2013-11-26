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
    /// This is a strategy based on concept explored from the book Encylopedia of Trading Strategies
    /// </summary>
    [Description("This is a strategy based on concept explored from the book Encylopedia of Trading Strategies")]
    public class EOTS : Strategy
    {
        #region Variables
        // Wizard generated variables
		private int donchianPeriod = 10;
        private int exitAfterXBars = 5; // Default setting for PeriodInTrade

        // User defined variables (add any user defined variables below)
		Random rnd = new Random();
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Aggregated = true;
			CalculateOnBarClose = true;
			//ClearOutputWindow();
			EntriesPerDirection = 1; // Order Handling enforce only 1 position long or short at any time
			EntryHandling = EntryHandling.AllEntries;
			ExitOnClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Checks to make sure we have at least x or more bars
			if (CurrentBar < BarsRequired)
        		return;			
			
			// EXIT CODE			
			if (Position.MarketPosition != MarketPosition.Flat) 
			{
				// Exit after x bars
				if (BarsSinceEntry() >= ExitAfterXBars)
				{
					if (Position.MarketPosition == MarketPosition.Long)
					{
						ExitLong();
						Print(Time + " Exit market on open due to time in trade " + Instrument.FullName);
					}
					if (Position.MarketPosition == MarketPosition.Short)
					{
						ExitShort();
						Print(Time + " Exit market on open due to time in trade " + Instrument.FullName);
					}
				}
			} 
			
			//if ((BarsSinceExit() == 0) || ((3 % BarsSinceExit()) == rnd.Next(3)))
			//if ((BarsSinceEntry() == 10) || (BarsSinceEntry() == 0))
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				if (Close[0] > MAX(Close, DonchianPeriod)[1])
				{
					EnterLong(5);
				}
				
				if (Close[0] < MIN(Close, DonchianPeriod)[1])
				{
					EnterShort(5);
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

        #endregion
    }
}
