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
    /// SPY-EOD This strategy is based from a purchased e-book named Building Profitable Trading Systems. 
	/// http://www.systemtradersuccess.com
	/// 
	/// The testing period will be between 09/01/1997 and 12/31/2011
	/// I used 9/1 instead of 1/1 because the pdf file had a 200 day Min Bars Req
	/// 
	/// The sanity test gain was 7795.17, slightly better than buy and hold
	/// The buy and hold gain during this same period is 7536.93
	/// 
    /// </summary>
    [Description("SPY-EOD This strategy is based on Building Profitable Trading Systems")]
    public class KeyReversalETF : Strategy
    {
        #region Variables
			// Wizard generated variables
			private int myInput0 = 1; // Default setting for MyInput0
			// User defined variables (add any user defined variables below)
			double accountSize = 20000.00;
			int lookBackPeriod = 10;
			int smaPeriod = 10;
			int shares = 0;	// shares to trade
			bool isBearish = false;
			bool isBullish = false;

			// DonchianChannel parms
			DonchianChannel dc = null;
			SMA sma = null;
		
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
//            SetProfitTarget(20000);
//            SetProfitTarget("", CalculationMode.Percent, 0);
//            SetStopLoss("", CalculationMode.Price, 0, false);
//            SetTrailStop("", CalculationMode.Percent, 0, false);

			dc = DonchianChannel(LookBackPeriod);
			dc.Displacement = 0;
			dc.PaintPriceMarkers = false;
			dc.Plots[0].Pen.Color = Color.Transparent;	// mean
			dc.Plots[1].Pen.Color = Color.Black;		// top plot
			dc.Plots[2].Pen.Color = Color.Black;		// bottom plot
			Add(dc);
			
			sma = SMA(SmaPeriod);
			sma.Plots[0].Pen.Color = Color.Blue;
			Add(sma);

			Slippage = 0.04;
			IncludeCommission = true;
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			isBullish = ((Low[0] < MIN(Low, LookBackPeriod)[1]) && (Close[0] >= Close[1]));
			//Print(Time + " Low[0] " + Low[0] + " MIN: " + MIN(Low, lookBack)[1] + " isBullish? " + isBullish);
			isBearish = ((High[0] > MAX(High, LookBackPeriod)[1]) && (Close[0] <= Close[1]));
			//Print(Time + " High[0] " + High[0] + " MAX: " + MAX(High, lookBack)[1] + " isBearish? " + isBearish);
			shares = (int) Math.Floor(accountSize/Close[0]);
			
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				double currentAccountSize = accountSize + Performance.AllTrades.TradesPerformance.GrossProfit + Performance.AllTrades.TradesPerformance.GrossLoss;
				shares = (int) Math.Floor(currentAccountSize/Close[0]);
				Print(Time + " GrossProfit: " + Performance.AllTrades.TradesPerformance.GrossProfit + "GrossLoss " + Performance.AllTrades.TradesPerformance.GrossLoss);
				if (isBullish) {
					EnterLong(shares, "KR_Long");
				} else if (isBearish) {
					//Print(Time + " High[0] " + High[0] + " MAX: " + MAX(High, lookBack)[1]);
					EnterShort(shares, "KR_Short");
				}
			}
			
			if (Position.MarketPosition == MarketPosition.Long && Close[0] > sma[0]) {
				ExitLong("SMA Exit", "KR_Long");
			}
			if (Position.MarketPosition == MarketPosition.Short && Close[0] < sma[0]) {
				ExitShort("SMA Exit", "KR_Short");
			}
        }

        #region Properties
        [Description("The look-back period for the key reversal entry")]
        [GridCategory("Parameters")]
        public int LookBackPeriod
        {
            get { return lookBackPeriod; }
            set { lookBackPeriod = Math.Max(1, value); }
        }
		
        [Description("The look-back period for our exit calculation")]
        [GridCategory("Parameters")]
        public int SmaPeriod
        {
            get { return smaPeriod; }
            set { smaPeriod = Math.Max(1, value); }
        }
        #endregion
    }
}














