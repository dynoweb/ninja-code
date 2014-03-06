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
    /// Opening Range Breakout Strategy
    /// </summary>
    [Description("Opening Range Breakout Strategy")]
    public class OpeningRangeStrategy : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int enableStops = 0; 
        private int useTarget2 = 1; 
        private int tradeSize = 50000; 
        // User defined variables (add any user defined variables below)
		private bool enableTrade = false;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//Add(OpeningRangeBreakout(5));
			
			// Triggers the exit on close function 30 seconds prior to session end 
    		ExitOnClose = true;
    		ExitOnCloseSeconds = 60;

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// This should be true when the primary bar is being udpated
			if (BarsInProgress != 0)
		    	return;
			
			//OpeningRangeBreakout orb = OpeningRangeBreakout(5);
			ATR(5)[0];
			
			if (Bars.FirstBarOfSession) 
				enableTrade = true;
			
			double longEntry = CurrentPrice() + (ATR(5)[0] * 0.5);
			double shortEntry = CurrentPrice() - (ATR(5)[0] * 0.5);
			//Print(Time + " orb.LongEntry[0]: " + orb.LongEntry[0]);
			 
			// Buy Limit Order
			// A Buy Limit Order is an order to buy a specified number of shares of a 
			// stock at a designated price or lower, at a price that is below the current 
			// market price. Your limit price, in other words, is the maximum price you 
			// are willing to pay to purchase your shares.
			
			// Buy Stop Order
			// A Buy Stop Order is an order to buy a stock at a price above the current market 
			// price. Once a stock's price trades at or above the price you have specified, 
			// it becomes a Market Order to buy.
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				int shares = calcShares(TradeSize);

				Print(Time + "  GetCurrentBid() @ " + GetCurrentBid() 
					+ "  EnterLongStop @ " + orb.LongEntry[0] 
					+ "  EnterShortStop @ " + orb.ShortEntry[0]);
				
				if (GetCurrentBid() < orb.LongEntry[0])
				{
					this.EnterLongStop(shares, orb.LongEntry[0]);
				}
				if (GetCurrentBid() > orb.ShortEntry[0])
				{
					this.EnterShortStop(shares, orb.ShortEntry[0]);
				}
			}
			else
			{
				enableTrade = false;
//				if (Position.MarketPosition == MarketPosition.Long)
//				{
//					this.ExitLongLimit(orb.LongTarget1[0]);
//					if (UseTarget2 == 1)
//						this.ExitLongLimit(orb.LongTarget2[0]);
//					if (EnableStops == 1)
//						this.ExitLongStop(orb.LongStop[0]);
//				} 
//				else
//				{
//					this.ExitShortLimit(orb.ShortTarget1[0]);
//					if (UseTarget2 == 1)
//						this.ExitShortLimit(orb.ShortTarget2[0]);
//					if (EnableStops == 1)
//						this.ExitShortStop(orb.ShortStop[0]);
//				}
			}
			
        }
		
		private int calcShares(int investment)
		{
			int shares = (int) Math.Floor(investment/Close[0]);
			//if (debugOn) Print(Time + " shares: " + shares);
			return shares; 
		}
		
        #region Properties
        [Description("Set to 1 to enable")]
        [GridCategory("Parameters")]
        public int EnableStops
        {
            get { return enableStops; }
            set { enableStops = Math.Max(0, value); }
        }

        [Description("Used to calculate number of shares")]
        [GridCategory("Parameters")]
        public int TradeSize
        {
            get { return tradeSize; }
            set { tradeSize = Math.Max(1, value); }
        }
		
        [Description("Set to 0 to use target 1, set to 1 to use target 2")]
        [GridCategory("Parameters")]
        public int UseTarget2
        {
            get { return useTarget2; }
            set { useTarget2 = Math.Max(0, value); }
        }

        #endregion
    }
}
