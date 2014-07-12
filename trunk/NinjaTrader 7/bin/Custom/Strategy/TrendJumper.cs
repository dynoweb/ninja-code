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
    /// This strategy is based on concepts from the TJFreeAUDUSD strategy by Troy TJ Noonan, developer PremierTraderUniversity
	/// http://www.premiertraderuniversity.com/precisionfx/getsystem.html?inf_contact_key=64a92413b3fa2a96dc5cea71bbd722058dc377af26d29bb16f3a9eef7834528d
	/// C:\Users\rcromer\Documents\Trading\PremierTraderUniversity\AUDUSD
	/// 
	/// Buy Limit Order
	/// A Buy Limit Order is an order to buy a specified number of shares of a 
	/// stock at a designated price or lower, at a price that is below the current 
	/// market price. Your limit price, in other words, is the maximum price you 
	/// are willing to pay to purchase your shares.
	/// 
	/// Buy Stop Order
	/// A Buy Stop Order is an order to buy a stock at a price above the current market 
	/// price. Once a stock's price trades at or above the price you have specified, 
	/// it becomes a Market Order to buy.	
	/// 
	/// Testing using 23 Range CL 1/1/13 to 10/9/13 3/26 MOM exit 4 bars since entry 
	/// 	1.47 PF, 24,045.00 profit, $2,550 Max DD, 433 Trades, 53.58% winning, autoexit, non-managed
	/// 
	/// Testing YM 2013 - 10/9 with non-managed bar count exit, optimized long and short seperately
	///   Long  - PF 3.25, 30 Range, 4/46 MOM, 5 bars since entry $3930.00 profit, $425.00 MDD, 67.4%
	///   Short - PF 1.69, 28 Range, 5/34 MOM, 3 bars since entry $2685.00 profit, $600.00 MDD, 60.6%
	/// 
	/// Testing NQ 2013 - 10/9 with non-managed bar count exit, optimized long and short seperately
	///   Long  - PF 1.84, 18 Range, 3/30 MOM, 12 bars since entry, $8,475.00 profit, $775.00 MDD, 61.11%
	///   Short - PF 1.17, 20 Range, 7/20 MOM, 7 bars since entry, $1,285.00 profit, $2,515.00 MDD, 56.14%
	/// 
	/// Testing ES 2013-6/9/2014 with 1 target at 2 had PF of 1.2 - $5797.50 Net, $3500 MDD, 56.72% profitable - w/ commission 
	/// 
    /// </summary>
    [Description("This strategy is based on concepts from the TJFreeAUDUSD strategy by Troy TJ Noonan, developer PremierTraderUniversity")]
    public class TrendJumper : Strategy
    {
        #region Variables
        // Wizard generated variables - Values optimized so far for 28 Range CL bars
        private int eMA1Len = 15; //50; // Default setting for Trailing Stop
        private int eMA2Len = 200; // Default setting for Trend Filter
        private int entryOffsetTics = 0; // Default setting for EntryOffsetTics
        private int mOM1Len = 4; // Default setting for MOM1Len
        private int mOM2Len = 24; // 9; // Default setting for MOM2Len
		private int sinceEntry = 4;
        private int stopOffsetTics = 12; // Default setting for StopOffsetTics
        private double tgt1X = 1.75; // Default setting for Tgt1X
        private double tgt2X = 2.0; // Default setting for Tgt2X
        private double tgt3X = 4.5; // Default setting for Tgt3X
        private int trailLen = 9; // Default setting for TrailLen
        // User defined variables (add any user defined variables below)
		TJ tj = null;
		IOrder entryLongOrder = null;
		IOrder entryShortOrder = null;
		int orderSize = 1;
		
		double limitPrice = 0;
		double stopPrice = 0;
			
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			tj = TJ("4/29/14", true, EntryOffsetTics, StopOffsetTics, Tgt1X, Tgt2X, Tgt3X, MOM1Len, MOM2Len, true);
			Add(tj);
			Add(PitColor(Color.Black, 80000, 25, 160000));
			
			Add(EMA(EMA1Len));
			Add(EMA(EMA2Len));
			Add(DonchianChannel(MOM1Len));
			Add(DonchianChannel(MOM2Len));
			//Add(DonchianChannel(TrailLen));
			
			//Add(HMARick(55,50));
			
			EMA(EMA1Len).Plots[0].Pen.Color = Color.DarkGray;
			EMA(EMA2Len).Plots[0].Pen.Color = Color.DarkBlue;

			DonchianChannel(MOM1Len).AutoScale = false;
			DonchianChannel(MOM1Len).Plots[0].Pen.DashStyle = DashStyle.Dash;
			DonchianChannel(MOM1Len).Plots[0].Pen.Color = Color.DarkCyan;	// mean
			DonchianChannel(MOM1Len).Plots[1].Pen.Color = Color.Transparent;		// top plot
			DonchianChannel(MOM1Len).Plots[2].Pen.Color = Color.Transparent;		// bottom plot
			DonchianChannel(MOM1Len).PaintPriceMarkers = false;
			
			DonchianChannel(MOM2Len).AutoScale = false;
			DonchianChannel(MOM2Len).Plots[0].Pen.DashStyle = DashStyle.Dash;
			DonchianChannel(MOM2Len).Plots[0].Pen.Color = Color.DarkRed;	// mean
			DonchianChannel(MOM2Len).Plots[1].Pen.Color = Color.Transparent;		// top plot
			DonchianChannel(MOM2Len).Plots[2].Pen.Color = Color.Transparent;		// bottom plot
			DonchianChannel(MOM2Len).PaintPriceMarkers = false;
			
			ExitOnClose = true;
            CalculateOnBarClose = true;
			if (SinceEntry == 0)
				Unmanaged = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			//DrawArrowUp(CurrentBar.ToString()+"momo", 0, High[0] + 10 * TickSize, Color.Aquamarine);
			//if (Position.MarketPosition != MarketPosition.Long)
			//double approxEntry = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			
			
			
			
			if (Position.MarketPosition != MarketPosition.Flat
				&& BarsSinceEntry() >= SinceEntry
				&& SinceEntry > 0) 
			{
				if (Position.MarketPosition == MarketPosition.Long)
				{
					this.ExitLong();
				}
				if (Position.MarketPosition == MarketPosition.Short)
				{
					this.ExitShort();
				}
			}
			else
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				if (tj.LongEntry.ContainsValue(0)) 
				{
					DrawArrowUp(CurrentBar.ToString()+"LE", 0, Low[0] - 10 * TickSize, Color.Green);

					if (SinceEntry > 0)
					{
						EnterLong();
					} 
					else
					{
						//this.EnterLongLimit(Close[0] - entryOffsetTics * TickSize);
						entryLongOrder = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, orderSize, 
							tj.LongEntry[0], tj.LongEntry[0], "dayTrade", "LongStopLimit");
						
						stopPrice = tj.LongStop[0];
						limitPrice = tj.LongTgt1[0];
					}
				} 
				if (tj.ShortEntry.ContainsValue(0)) 
				{
					DrawArrowDown(CurrentBar.ToString()+"SE", 0, High[0] + 10 * TickSize, Color.Red);
					
					if (SinceEntry > 0)
					{
						EnterShort();
					} 
					else
					{
						entryShortOrder = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, orderSize, 
							tj.ShortEntry[0], tj.ShortEntry[0], "dayTrade", "Sell Short");
						
						stopPrice = tj.ShortStop[0];
						limitPrice = tj.ShortTgt1[0];
					}
				} 
			}
        }
		
		
		#region OnExecution
		protected override void OnExecution(IExecution execution)
		{
			// we are just counting bars until exit, no need for stops
			if (SinceEntry > 0) 
			{
				return;
			}
			
			if (execution.Order == null)
			{
				Print(Time + " -->> OnExecution.Order is null");
				return;
			}
			
			
			
			if (execution.Order.OrderState == OrderState.Filled)
			{
				//enableTrade = false; // limit 1 trade / day
				if (execution.Order.OrderAction == OrderAction.Buy)
				{
					//if (UseTarget2 == 1)
					//	limitPrice = tj.LongTgt2[0];
					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, 0, "closeDayTrade", "Close Long Limit");
					//if (EnableStops == 1)
						SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 0, stopPrice, "closeDayTrade", "Close Long Stop");
				}
				
				if (execution.Order.OrderAction == OrderAction.Sell)
				{					
					//if (UseTarget2 == 1)
					//	limitPrice = tj.ShortTgt2[0];
					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, limitPrice, 0, "closeDayTrade", "Close Short Limit");
					//if (EnableStops == 1)
						SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 0, stopPrice, "closeDayTrade", "Close Short Stop");
				}
			} 
			else 
			{
				Print(Time + " execution.Order: " + execution.Order.ToString());
			}
				
		}
		#endregion		
		

        #region Properties
        [Description("Trailing Stop")]
        [GridCategory("Parameters")]
        public int EMA1Len
        {
            get { return eMA1Len; }
            set { eMA1Len = Math.Max(1, value); }
        }

        [Description("Trend Filter")]
        [GridCategory("Parameters")]
        public int EMA2Len
        {
            get { return eMA2Len; }
            set { eMA2Len = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MOM1Len
        {
            get { return mOM1Len; }
            set { mOM1Len = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MOM2Len
        {
            get { return mOM2Len; }
            set { mOM2Len = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailLen
        {
            get { return trailLen; }
            set { trailLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EntryOffsetTics
        {
            get { return entryOffsetTics; }
            set { entryOffsetTics = Math.Max(-10, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopOffsetTics
        {
            get { return stopOffsetTics; }
            set { stopOffsetTics = Math.Max(-10, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt1X
        {
            get { return tgt1X; }
            set { tgt1X = Math.Max(0.0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt2X
        {
            get { return tgt2X; }
            set { tgt2X = Math.Max(0.0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt3X
        {
            get { return tgt3X; }
            set { tgt3X = Math.Max(0.0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int SinceEntry
        {
            get { return sinceEntry; }
            set { sinceEntry = Math.Max(0, value); }
        }

        #endregion
    }
}
