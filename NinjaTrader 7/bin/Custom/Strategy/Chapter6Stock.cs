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
	/// 
	/// 9/28/2013 - For some reason, my numbers don't match the books.  I do more trades and am not making as much profit.
	/// 
	/// Test - Using 9/30/99 - 12/31/2011 12 years to allow indicators to get historical data
	/// 9/28/13 - SVN revision 57
	/// Conditions: dP=4, eAXB=9, lTP=40, lO=true, lVT=3, pT=0
	/// Results: PF=1.57, Net=532,920.23, Trades=6523, AvgTrade=81.70, MaxDD=3,268.91 - No Commission
	/// 
	/// Conditions: dP=8, eAXB=8, lTP=70, lO=true, lVT=3, pT=300
	/// Results: PF=1.61, Net=309,836.31, Trades=4907, AvgTrade=63.14, MaxDD=1,839.72 - No Commission
	/// 
	/// Test - Using 9/30/99 - 12/31/2011 12 years to allow indicators to get historical data
	/// 10/24/13 - SVN revision 60
	/// Conditions: dP=8, eAXB=9, lTP=70, lO=true, lVT=3, pT=0, vP=8
	/// Results: PF=1.5, Net=205,592.26, Trades=2422, AvgTrade=84.89, MaxDD=2,264.56 - w/ $48k Commission
	/// 
	/// Test - Using 9/15/99 - 10/29/2013 - NASDAQ 100
	/// /// SVN rev 61 - 
	/// Conditions: dP=10, eAXB=6, lTP=23, lO=true, lVT=5, pT=250, vP=10 - 5 day period
	/// Results: PF=1.82, Net=68,714.09, Trades=779, AvgTrade=88.21, MaxDD=905.46 - w/ $15.5k Commission
	/// 
	/// Test - Using 9/15/99 - 1/1/2013 - NASDAQ 100 - no exit if close above DC
	/// /// SVN rev 62 - 11/15/2013
	/// Conditions: dP=8, eAXB=9, lTP=70, lO=true, lVT=3.2, pT=0, vP=14 - 1 day period
	/// Results: PF=1.57, Net=281,577.08, Trades=3085, AvgTrade=91.27, MaxDD=2514.09 - includes $61.638.30 Commission
	/// Conditions: dP=8, eAXB=9, lTP=70, lO=true, lVT=3.8, pT=0, vP=16 - 1 day period
	/// Results: PF=1.63, Net=264,427.02, Trades=2610, AvgTrade=101.31, MaxDD=2442.50 - includes $52,147.80 Commission
	/// w/ exit if close above DC was added 
	/// Results: PF=1.59, Net=229,884.14, Trades=2642, AvgTrade=87.01, MaxDD=2325.23 - includes $57,787.16 Commission
	/// w/ exit if close above DC was added and optimized for 250,300,350 profit (300 best with results below)
	/// Results: PF=1.46, Net=152,689.89, Trades=2856, AvgTrade=53.46, MaxDD=1960.34 - includes $57,062.88 Commission
	/// 
    /// </summary>
    [Description("Trades daily time period stocks, based on Chapter 5 of the book Building Reliable Trading Systems. Note when running live make sure to set the the 'Stop & target submission' property to ByStrategyPosition")]
    public class Chapter6Stock : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int donchianPeriod = 8; // Default setting for DonchianPeriod
        private int exitAfterXBars = 9; // Confirmed - Default setting for ExitAfterXBars
        private int longerTermPeriod = 70; // Default setting for LongerTermPeriod
        private bool longOnly = true; // Default setting for LongOnly
		private double lowVolatilityThreshold = 3.8;  // 2.8 or 2.9 seem more like the book
        private double profitTarget = 0; // Default setting for ProfitTarget, based on a $5000 investment
		private int volPeriod = 16;	// the book showed 20, but 8-9 seems more like his website is using
		
		bool debugOn = false;
		// User defined variables (add any user defined variables below)

		private bool forceReversalShort = false;	// looks for a short signal before enabling the long signal
		private IOrder entryLongOrder = null;
		private IOrder closeReversalOrder = null;
		private IOrder closeXBarsOrder = null;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			// Order Handling enforce only 1 position long or short at any time
			EntriesPerDirection = 1; 
			EntryHandling = EntryHandling.AllEntries;
			//ClearOutputWindow();
			ExitOnClose = false;
			CalculateOnBarClose = true;
			Aggregated = true;
				
			// if this is set without a CalculationMode then it's a dollar value
			// for the entire trade (targetPrice = fillPrice + profitTarget/positionQuantity)
			if (profitTarget != 0) SetProfitTarget(profitTarget);
			
			Add(DonchianChannelClose(donchianPeriod));			
			Add(SMARick(LongerTermPeriod));
			// Instrument.MasterInstrument.Name
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			// Checks to make sure we have at least x or more bars
			if (CurrentBar < LongerTermPeriod)
        		return;
			
			if (CurrentBar == LongerTermPeriod)
				Print("Initializing Strategy for " + Instrument.FullName);

			// EXIT CODE			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				// Exit after x bars
				if (ExitAfterXBars != 0 
					&& BarsSinceEntry() >= ExitAfterXBars
					)
				{
					closeXBarsOrder = ExitLong("AfterXBars", "Reversal");
					Print(Time + " Exit market on open due to time in trade " + Instrument.FullName);
				}
				
				// Exit if short reversal detected
				if (ExitAfterXBars == 0 && DonchianPeriod != 0 
				//if (DonchianPeriod != 0 
					&& Close[0] > MAX(Close, DonchianPeriod)[1]
					)
				{
					closeReversalOrder = ExitLong("Breakout", "Reversal");								
					Print(Time + " closing since prior bar broke above DC");
				}
			}			
			
			// ENTRY CODE
			if (Position.MarketPosition != MarketPosition.Long && !forceReversalShort)
			{				
				if (Close[0] < MIN(Close, DonchianPeriod)[1])  // using reverse entry signals
				{
					//if (LongerTermPeriod == 0 || Close[0] > Close[LongerTermPeriod])
					if (LongerTermPeriod == 0 || Rising(SMARick(LongerTermPeriod)))
					{
						if (LowVolatilityThreshold == 0 || !lowVolatility(LowVolatilityThreshold))
						{
							entryLongOrder = EnterLong(calcShares(5000), "Reversal");
							Print(Time + " " + Instrument.FullName + " Entered long today at open. Enter Good til Canceled sell limit order at 156.82.");
						}
						else
							if (debugOn) Print(Time + " excluded for low volatility " + ((StdDev(Close, volPeriod)[0]/Close[0]) * 100));
					}
					else
						if (debugOn) Print(Time + " excluded for longTermTrend");
				}
			}
			
			if (Close[0] > MAX(Close, DonchianPeriod)[1])
			{
				forceReversalShort = false;
				if (!LongOnly)
				{
					EnterShort(calcShares(5000), "Reversal");
				}
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
		private bool lowVolatility(double minPercent)
		{
			int Period = volPeriod;
			double stdDevValue = StdDev(Close, Period)[0];
			if (debugOn) Print (Time + " stdDev percent of price " + ((stdDevValue/Close[0]) * 100));
			double stdDevPercentOfPrice = (stdDevValue/Close[0]) * 100;
			Color colour = Color.Yellow;
			if (stdDevPercentOfPrice >= 2) 
			{
				colour = Color.Red;
			}
			else if (stdDevPercentOfPrice >= 2.5) 
			{
				colour = Color.Green;
			}
			else if (stdDevPercentOfPrice >= 3.5) 
			{		
				colour = Color.Black;
			}
			DrawDot(CurrentBar + "vol", true, 0, High[0] * 1.05, colour);
			return stdDevPercentOfPrice < minPercent ? true : false;
		}
		
		private int calcShares(int investment)
		{
			int shares = (int) Math.Floor(investment/Close[0]);
			if (debugOn) Print(Time + " shares: " + shares);
			return shares; 
		}

		// OnOrderUpdate() - Called when a strategy generated order changes state
		protected override void OnOrderUpdate(IOrder order)
		{
			if (entryLongOrder != null && entryLongOrder == order)
			{
				if (order.OrderState == OrderState.Filled)
				{
					if (debugOn) Print(Time + " order filled");
					//Print(Time + " entryLongOrder: " + order.ToString());
					//entryLongOrder = null;
				}
			}
			
//1/12/2009 3:00:00 PM entryLongOrder: Order='NT-00226/Backtest' Name='Reversal' State=Filled Instrument='WYNN' Action=Buy Limit price=0 Stop price=0 Quantity=121 Strategy='Chapter6Stock' Type=Market Tif=Gtc Oco='' Filled=121 Fill price=40.82 Token='2e871a63e35e43a5949fbbacec0256e4' Gtd='12/1/2099 12:00:00 AM'
//1/26/2009 3:00:00 PM closeXBarsOrder: Order='NT-00228/Backtest' Name='AfterXBars' State=Filled Instrument='WYNN' Action=Sell Limit price=0 Stop price=0 Quantity=121 Strategy='Chapter6Stock' Type=Market Tif=Gtc Oco='' Filled=121 Fill price=35.76 Token='1746944728aa45c7b226538e6579655f' Gtd='12/1/2099 12:00:00 AM'
			
			if (closeXBarsOrder != null && closeXBarsOrder == order)
			{
				// normal State life cycle PendingSubmit, Accepted, Working, Filled
				if (order.OrderState == OrderState.Filled)
				{
					//Print(Time + " entryLongOrder: " + order.ToString());
					//Print(Time + " closeXBarsOrder: " + order.ToString());
					double profit = entryLongOrder.Quantity * (closeXBarsOrder.AvgFillPrice - entryLongOrder.AvgFillPrice);
					//Print("Profit: " + profit); 
					//closeXBarsOrder = null;
//					if (profit < 0)
//						forceReversalShort = true;
				}
			}
		}
 
		// OnExecution() - Called when a strategy generated order is filled
		protected override void OnExecution(IExecution execution)
		{
			// Remember to check the underlying IOrder object for null before trying to access its properties
			if (execution.Order != null && execution.Order.OrderState == OrderState.Filled)
			{
				if (execution.Order.OrderAction == OrderAction.Buy)
				{
//					Print(Time + " OnExecution Buy " + execution.ToString());
//					Print(Time + " OnExecution Buy     " + execution.Order.ToString());
//					Print(Time + " " + Position.Quantity + "@" + Position.AvgPrice);
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
					
//1/26/2009 3:00:00 PM OnExecution Execution='NT-00155' Instrument='WYNN' Account='Backtest' Name='Sell' Exchange=Default Price=35.76 Quantity=121 Market position=Short Commission=9.99 Order='NT-00228' Time='1/27/2009 3:00:00 PM'
//1/26/2009 3:00:00 PM OnExecution     Order='NT-00228/Backtest' Name='Sell' State=Filled Instrument='WYNN' Action=Sell Limit price=0 Stop price=0 Quantity=121 Strategy='Chapter6Stock' Type=Market Tif=Gtc Oco='' Filled=121 Fill price=35.76 Token='e8c28c3cabef43939b891c67b0c110ad' Gtd='12/1/2099 12:00:00 AM'
					
//					Print(Time + " OnExecution " + execution.ToString());
//					Print(Time + " OnExecution     " + execution.Order.ToString());
//					Print(Time + " Action=" + execution.Order.OrderAction + "  " + execution.Order.Quantity + "@" + execution.Order.AvgFillPrice + " Opened @ " + Position.AvgPrice);
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
        public double LowVolatilityThreshold
        {
            get { return lowVolatilityThreshold; }
            set { lowVolatilityThreshold = Math.Max(0, value); }
        }

        [Description("Debug On")]
        [GridCategory("Parameters")]
        public bool DebugOn
        {
            get { return debugOn; }
            set { debugOn = value; }
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

        [Description("Volatility period.")]
        [GridCategory("Parameters")]
        public int VolPeriod
        {
            get { return volPeriod; }
            set { volPeriod = Math.Max(2, value); }
        }

        #endregion
    }
}
