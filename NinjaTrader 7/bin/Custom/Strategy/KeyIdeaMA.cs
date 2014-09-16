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
    /// This code is used to test a particular key idea, 
	/// this key idea is bassed on the book Building Profitable Trading System 
	/// using a Reversal Bar	
	/// Bar Type: Daily
	/// Hypothesis: is that a reversal bar often signals a trend reversal in the market
	/// For Shorts: A bar that produces a new 10-day high and then closes below 
	/// 			the close of the previous day. Enter at open of next bar.
	/// For Longs: A bar that produces a new 10-day low and then closes above the close of the
	/// 			previous day. Enter at open of next bar.
    /// </summary>
    [Description("This code is used to test a particular key idea, this key idea is to see the affect of the 200 day ma has on SPY")]
    public class KeyIdeaMA : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int barsInTrade = 5; // Default setting for BarsInTrade
        private int barsAfterMACross = 7; // Default setting for BarsAfterMACross
        private int smaPeriod = 10; // Default setting for MAPeriod
		private int marketPeriod = 200;
        // User defined variables (add any user defined variables below)
		SMA sma;
		SMA marketSma;
		bool krBullish;	// Key reversal bullish
		bool krBearish;	// Key reversal bearish
		bool marketBullish;
		bool marketBearish;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			sma = SMA(SMAPeriod);
			Add(sma);
			marketSma = SMA(Typical, MarketPeriod);
			Add(marketSma);
			
			AccountSize = 20000;
			BarsRequired = marketPeriod;
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar <= BarsRequired)
				return;
			
			krBullish = false;
			krBearish = false;
			marketBullish = false;
			marketBearish = false;
			
			
			int shares = (int) Math.Truncate(AccountSize/Close[0]);
			Print(Time + " balance:" + AccountSize + " shares: " + shares);
			shares = 100;
			
			if (LowestBar(Low, 10) == 0 && Close[0] > Close[1])
			{
				krBullish = true;
			}
			
			if (HighestBar(High, 10) == 0 && Close[0] < Close[1])
			{
				krBearish = true;
			}
			marketBullish = true; //Close[0] > marketSma[0];
			marketBearish = true; //Close[0] < marketSma[0];
			
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				if (krBearish && marketBearish)
				{
					EnterShort(shares);
				}
				if (krBullish && marketBullish)
				{
					EnterLong(shares);
				}
			} 
			
			
			else 
			{
				if (Close[0] > sma[0])
					ExitLong();
				if (Close[0] < sma[0])
					ExitShort();

//				if (BarsSinceEntry() >= barsInTrade) 
//				{
//					if (Position.MarketPosition == MarketPosition.Long) 
//					{
//						ExitLong();
//					}
//					if (Position.MarketPosition == MarketPosition.Short) 
//					{
//						ExitShort();
//					}
//				}				
			}
        }

        #region Properties
        [Description("Number of bars before closing a position")]
        [GridCategory("Parameters")]
        public int BarsInTrade
        {
            get { return barsInTrade; }
            set { barsInTrade = Math.Max(1, value); }
        }

        [Description("Number of days after MA crosses before trading")]
        [GridCategory("Parameters")]
        public int BarsAfterMACross
        {
            get { return barsAfterMACross; }
            set { barsAfterMACross = Math.Max(1, value); }
        }

        [Description("Moving Average Period")]
        [GridCategory("Parameters")]
        public int MarketPeriod
        {
            get { return marketPeriod; }
            set { marketPeriod = Math.Max(1, value); }
        }

        [Description("Moving Average Period")]
        [GridCategory("Parameters")]
        public int SMAPeriod
        {
            get { return smaPeriod; }
            set { smaPeriod = Math.Max(1, value); }
        }
        #endregion
    }
}
