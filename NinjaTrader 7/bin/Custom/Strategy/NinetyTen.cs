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
    /// This strategy is designed to identify stocks which have reached a 90 day high, then pulled back to a 10 day low, but closed in the upper 80-100 % range between the 10 day low and the 90 day high.
	/// 
	/// The following is a similar concept
	/// http://www.marketgeeks.com/swing-trading-stocks-strategies
	/// 
    /// </summary>
    [Description("This strategy is designed to identify stocks which have reached a 90 day high, then pulled back to a 10 day low, but closed in the upper 80-100 % range between the 10 day low and the 90 day high.")]
    public class NinetyTen : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// Daily - This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			int highestBar = HighestBar(High, 90);
			// has the highbar occured between the last 10-20 days
			if (highestBar >= 10 && highestBar <= 20) {
				// is high bar, the highest bar in the last 90 days before high 90 bar 
				// highest bar will be 10-20 days ago
				int ninetyBarHigh = HighestBar(High, 90 + highestBar);
				if (ninetyBarHigh == highestBar) {
					DrawDot(CurrentBar + "90", false, highestBar, High[highestBar], Color.Purple);	
					// high bar is qualified, now does the current bar qualify
					// it's required to close in the upper 20% of it's range
					// and it's low the 10 day low
					if (LowestBar(Low, 10) == 0) {
						// qualifies as having the 10 day low
						DrawDot(CurrentBar + "Low", false, 0, Low[0], Color.Red);	
						if (Close[0] >= (Range()[0] * 0.8 + Low[0])) {
							// qualifies as closing in the top 20% of the bar
							DrawDot(CurrentBar + "20%", false, 0, Close[0], Color.Green);	
						}
					}
				}
			}
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
        }
        #endregion
    }
}
