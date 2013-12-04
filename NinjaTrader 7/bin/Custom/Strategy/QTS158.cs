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
    /// Quantitative Trading Strategy, page 158 - 80 day momentum 
	/// Description: Buy if today's close > close of 80 days ago, 
	///              exit if today's close < close of 80 days ago.
    /// </summary>
    [Description("Quantitative Trading Strategy, page 158 - 80 day momentum Description Buy if today's close > close of 80 days ago, and if today's close < close of 80 days ago.")]
    public class QTS158 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int period = 80; // Default setting for Days
        private double investment = 5.000; // Default setting for AmountPerTrade
        // User defined variables (add any user defined variables below)
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
			if (CurrentBar < Period)
        		return;	
			
			if (Close[0] > Close[Period-1])
			{
				EnterLong(calcShares(Investment));
			}
				
			if (Close[0] < Close[Period-1])
			{
				EnterShort(calcShares(Investment));
			}
		}

		private int calcShares(double investment)
		{
			return (int) Math.Floor(investment/Close[0]);
		}
		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Investment
        {
            get { return investment; }
            set { investment = Math.Max(1, value); }
        }
        #endregion
    }
}
