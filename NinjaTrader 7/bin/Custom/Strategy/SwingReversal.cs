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
    /// This strategy is based on concepts from the PTUActiveSwingReversal strategy by Troy TJ Noonan, developer PremierTraderUniversity
	/// http://www.premiertraderuniversity.com/precisionfx/getsystem.html?inf_contact_key=64a92413b3fa2a96dc5cea71bbd722058dc377af26d29bb16f3a9eef7834528d
	/// C:\Users\rcromer\Documents\Trading\PremierTraderUniversity
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
	/// Based on this version
	/// Test 1/1/14 -> 6/18/14, 0.8, -5, -50, -95, -50, 75, 0, 1.5, 1.5, 3.3, 3 - PF 1.17, profit: $10,185 ,MaxDD -$6985, Percent Profitable 69.27%
	/// - Change, added order canceling feature if indicator stopped showing entry, also pulled in target 1 by one tick to match indicator expectations on trade triggered
	/// Test 1/1/14 -> 6/18/14, 0.8, -5, -50, -95, -50, 75, 0, 1.5, 1.6, 3.3, 3 - PF 1.14, profit: $8,425 ,MaxDD -$7,480, Percent Profitable 68.57%
	/// 
    /// </summary>
    [Description("This strategy is based on concepts from the PTUActiveSwingReversal strategy by Troy TJ Noonan, developer PremierTraderUniversity")]
    public class SwingReversal : Strategy
    {
        #region Variables
        // Wizard generated variables - Values optimized so far for 8 Range SI bars
		//EntryMult, StopMult, Tgt1Mult, Tgt2Mult, TrailLength, false, PctRLen, OBLevel, OBReset, OSLevel, OSReset
		double entryMult = 0.8; 
		int oBLevel = -5;
		int oBReset = -50;
		int oSLevel = -95;
		int oSReset = -50;
		int pctRLen = 75;
		private int sinceEntry = 0;
		double stopMult = 1.5;
		double tgt1Mult = 1.5;
		double tgt2Mult = 3.3;
		int trailLength = 3;
//        private int eMA1Len = 15; //50; // Default setting for Trailing Stop
//        private int eMA2Len = 200; // Default setting for Trend Filter
//        private int entryOffsetTics = 0; // Default setting for EntryOffsetTics
//        private int mOM1Len = 4; // Default setting for MOM1Len
//        private int mOM2Len = 24; // 9; // Default setting for MOM2Len
//        private int stopOffsetTics = 12; // Default setting for StopOffsetTics
//        private double tgt1X = 1.75; // Default setting for Tgt1X
//        private double tgt2X = 2.0; // Default setting for Tgt2X
//        private double tgt3X = 4.5; // Default setting for Tgt3X
//        private int trailLen = 9; // Default setting for TrailLen
        // User defined variables (add any user defined variables below)
		PTUActiveSwingReversal asr = null;
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
			asr = PTUActiveSwingReversal("4/7/14", EntryMult, StopMult, Tgt1Mult, Tgt2Mult, TrailLength, 
						false, PctRLen, OBLevel, OBReset, OSLevel, OSReset);
			Add(asr);
			Add(PitColor(Color.Black, 80000, 25, 160000));
			
			Print(Time + " -->> MachineId: " + NinjaTrader.Cbi.License.MachineId);
			
			
//			Add(EMA(EMA1Len));
//			Add(EMA(EMA2Len));
//			Add(DonchianChannel(MOM1Len));
//			Add(DonchianChannel(MOM2Len));
//			//Add(DonchianChannel(TrailLen));
//			
//			//Add(HMARick(55,50));
//			
//			EMA(EMA1Len).Plots[0].Pen.Color = Color.DarkGray;
//			EMA(EMA2Len).Plots[0].Pen.Color = Color.DarkBlue;
//
//			DonchianChannel(MOM1Len).AutoScale = false;
//			DonchianChannel(MOM1Len).Plots[0].Pen.DashStyle = DashStyle.Dash;
//			DonchianChannel(MOM1Len).Plots[0].Pen.Color = Color.DarkCyan;	// mean
//			DonchianChannel(MOM1Len).Plots[1].Pen.Color = Color.Transparent;		// top plot
//			DonchianChannel(MOM1Len).Plots[2].Pen.Color = Color.Transparent;		// bottom plot
//			DonchianChannel(MOM1Len).PaintPriceMarkers = false;
//			
//			DonchianChannel(MOM2Len).AutoScale = false;
//			DonchianChannel(MOM2Len).Plots[0].Pen.DashStyle = DashStyle.Dash;
//			DonchianChannel(MOM2Len).Plots[0].Pen.Color = Color.DarkRed;	// mean
//			DonchianChannel(MOM2Len).Plots[1].Pen.Color = Color.Transparent;		// top plot
//			DonchianChannel(MOM2Len).Plots[2].Pen.Color = Color.Transparent;		// bottom plot
//			DonchianChannel(MOM2Len).PaintPriceMarkers = false;
			
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
				if (asr.Entry.ContainsValue(0))
				{
					// Trade Start Time filter
					int hour = 8;
					int minute = 0;
					if (ToTime(Time[0]) < ToTime(hour, minute, 0))
					{
						CancelWorkingOrders();
						return;
					}
					
					if (asr.Entry[0] > asr.Stop[0]) 
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
								asr.Entry[0], asr.Entry[0], "dayTrade", "LongStopLimit");
							
							stopPrice = asr.Stop[0];
							limitPrice = asr.Target1[0];
						}
					} 
					if (asr.Entry[0] < asr.Stop[0]) 
					{
						DrawArrowDown(CurrentBar.ToString()+"SE", 0, High[0] + 10 * TickSize, Color.Red);
						
						if (SinceEntry > 0)
						{
							EnterShort();
						} 
						else
						{
							entryShortOrder = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, orderSize, 
								asr.Entry[0], asr.Entry[0], "dayTrade", "Sell Short");
							
							stopPrice = asr.Stop[0];
							limitPrice = asr.Target1[0];
						}
					} 
				}
				else	// cancel opening order
				{
					CancelWorkingOrders();
				}
			}
        }
		
		private void CancelWorkingOrders()
		{
			if (entryLongOrder != null && entryLongOrder.OrderState == OrderState.Working)  
			{
				//Print(Time + " Canceling order, state: " + entryLongOrder.OrderState);
				CancelOrder(entryLongOrder);
			}
			
			if (entryShortOrder != null && entryShortOrder.OrderState == OrderState.Working)  
			{
				//Print(Time + " Canceling order, state: " + entryShortOrder.OrderState);
				CancelOrder(entryShortOrder);
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
				if (execution.Name == "Exit on close")
				{
					//Print(Time + " " + execution.Name);
					return;
				}
				Print(Time + " " + execution);
				Print(Time + " -->> OnExecution.Order is null");
				return;
			}
			
			
			
			if (execution.Order.OrderState == OrderState.Filled)
			{
				//enableTrade = false; // limit 1 trade / day
				if (execution.Order.OrderAction == OrderAction.Buy)
				{
					//if (UseTarget2 == 1)
					//	limitPrice = asr.LongTgt2[0];
					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice - 1 * TickSize, 0, "closeDayTrade", "Close Long Limit");
					//if (EnableStops == 1)
						SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 0, stopPrice, "closeDayTrade", "Close Long Stop");
				}
				
				if (execution.Order.OrderAction == OrderAction.Sell)
				{					
					//if (UseTarget2 == 1)
					//	limitPrice = asr.ShortTgt2[0];
					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, limitPrice + 1 * TickSize, 0, "closeDayTrade", "Close Short Limit");
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
        [Description("Entry multiplier")]
        [GridCategory("Parameters")]
        public double EntryMult
        {
            get { return entryMult; }
            set { entryMult = value; }
        }

        [Description("Stop multiplier")]
        [GridCategory("Parameters")]
        public double StopMult
        {
            get { return stopMult; }
            set { stopMult = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt1Mult
        {
            get { return tgt1Mult; }
            set { tgt1Mult = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Tgt2Mult
        {
            get { return tgt2Mult; }
            set { tgt2Mult = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailLength
        {
            get { return trailLength; }
            set { trailLength = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int PctRLen
        {
            get { return pctRLen; }
            set { pctRLen = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int OBLevel
        {
            get { return oBLevel; }
            set { oBLevel = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int OBReset
        {
            get { return oBReset; }
            set { oBReset = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int OSLevel
        {
            get { return oSLevel; }
            set { oSLevel = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int OSReset
        {
            get { return oSReset; }
            set { oSReset = value; }
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
