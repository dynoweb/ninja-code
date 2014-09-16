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
    /// This strategy was created after going to the eftreplay website and testing SPY starting in 2000 on daily bars. The trade was based on a 200 day SMA, If the price closes the month below the MA close the position. Enter when above 200 day MA.
    /// </summary>
    [Description("This strategy was created after going to the eftreplay website and testing SPY starting in 2000 on daily bars. The trade was based on a 200 day SMA, If the price closes the month below the MA close the position. Enter when above 200 day MA.")]
    public class SimpleMA : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int days = 1; // Default setting for Days
        private int mAPeriod = 200; // Default setting for MAPeriod
        // User defined variables (add any user defined variables below)
		SMA ma;
		bool newMonth = false;
		int currentMonth = 1;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ma = this.SMA(MAPeriod);
			Add(ma);
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Time[0].Year < 2000)
			{
				return;
			}
			
			if (currentMonth != Time[0].Month) 
			{
				currentMonth = Time[0].Month;
				newMonth = true;
			}
			
			if (newMonth)
			{
				if (Close[0] > ma[0])
				{
					if (Position.MarketPosition == MarketPosition.Short) 
						ExitShort();
				
					EnterLong();
				} 
				if (Close[0] < ma[0])
				{
					if (Position.MarketPosition == MarketPosition.Long) 
						ExitLong();
					EnterShort();
				} 
			}
			newMonth = false;
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Days
        {
            get { return days; }
            set { days = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MAPeriod
        {
            get { return mAPeriod; }
            set { mAPeriod = Math.Max(1, value); }
        }
        #endregion
    }
}
