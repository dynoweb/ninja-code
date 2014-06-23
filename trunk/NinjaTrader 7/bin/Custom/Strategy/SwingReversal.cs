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
		//private int sinceEntry = 0;
		double stopMult = 1.5;
		double tgt1Mult = 1.5;
		double tgt2Mult = 3.3;
		int trailLength = 3;

		// User defined variables (add any user defined variables below)
		PTUActiveSwingReversal asr = null;
		
		IOrder entryOrder = null;
		IOrder limitOrder = null;
		IOrder stopOrder = null;
		
		int orderSize = 1;
		
		double limitPrice = 0;
		double stopPrice = 0;
		
		int startHour = 0;
		int startMinute = 0;
		int stopHour = 24;
		int stopMinute = 0;
		
		//int dayOfWeek = 1;
			
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
			
			//Print(Time + " -->> MachineId: " + NinjaTrader.Cbi.License.MachineId);
			
			//ExitOnClose = true;
            CalculateOnBarClose = true;
			Unmanaged = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			//double approxEntry = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			Print(Time + " BarsSinceSession: " + Bars.BarsSinceSession + " - " + ((entryOrder==null) ? "" : entryOrder.OrderState.ToString()));

			// If it's Monday, do not trade.
			//if (Time[0].DayOfWeek == DayOfWeek.Wednesday)
        	//	return;

			
			// Trade Start Time filter
			if (ToTime(Time[0]) < ToTime(StartHour, StartMinute, 0)
				|| ToTime(Time[0]) >= ToTime(StopHour, StopMinute, 0)
				)
			{
				CandleOutlineColor = Color.Black;
				
				if (Close[0] < Open[0])
					BarColor = Color.Black;
				else
					BarColor = Color.White;

				//CancelWorkingOrders();				
				//return;
			}			
			
			// Sets the bar color to its default color as defined in the chart properties dialog
			BarColor = Color.Empty;
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				if (asr.Entry.ContainsValue(0))
				{
					// Long Trade Condition
					if (asr.Entry[0] > asr.Stop[0]) 
					{
						//DrawArrowUp(CurrentBar.ToString()+"LE", 0, Low[0] - 10 * TickSize, Color.Green);

						if (entryOrder == null)
						{							
							Print(Time + " Creating new order");
							entryOrder = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, orderSize, 
								asr.Entry[0], asr.Entry[0], "entry", "Buy Long");
						}
					} 
					
					// Short Trade Condition
					if (asr.Entry[0] < asr.Stop[0]) 
					{
						//DrawArrowDown(CurrentBar.ToString()+"SE", 0, High[0] + 10 * TickSize, Color.Red);
						
						if (entryOrder == null)
						{
							Print(Time + " Creating new order");
							entryOrder = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, orderSize, 
									asr.Entry[0], asr.Entry[0], "entry", "Sell Short");
						}
					} 
				}
			}
			
			if (asr.AStop.ContainsValue(0) && asr.ATgt1.ContainsValue(0))
			{
				if (Position.MarketPosition != MarketPosition.Flat)
				{
					if (stopOrder.StopPrice != asr.AStop[0])
						ChangeOrder(stopOrder, stopOrder.Quantity, asr.ATgt1[0], asr.AStop[0]);
				}
			}
			
			if (!asr.Target1.ContainsValue(0) && !asr.ATgt1.ContainsValue(0)) 
			{
				CancelWorkingOrders();
			}
        }
		
		private void CancelWorkingOrders()
		{
			if (entryOrder != null)
			{
				if (entryOrder.OrderState == OrderState.Working)  
				{
					DrawDot(CurrentBar+"mid", false, 0, Median[0], Color.Blue);
					Print(Time + " Cancelling working order, state: " + entryOrder.OrderState);
					CancelOrder(entryOrder);
				}
			}
		}
		
		#region OnExecution
		protected override void OnExecution(IExecution execution)
		{
			if (execution.Name == "Exit on close")
			{
				Print(Time + " " + execution.Name);
				//entryOrder = null;
				if (entryOrder != null)
					CancelOrder(entryOrder);
				if (limitOrder != null)
					CancelOrder(limitOrder);
				if (stopOrder != null)
					CancelOrder(stopOrder);
				
				return;
			}
			
			if (execution.Order == null)
			{
				Print(Time + " " + execution);
				Print(Time + " -->> OnExecution.Order is null");
				return;
			}						
			
			Print(Time + " --- execution: " + execution);
			Print(Time + " --- execution.Order: " + execution.Order);
			
			if (execution.Order.OrderState == OrderState.Filled)
			{
				stopPrice = asr.Stop.ContainsValue(0) ? asr.Stop[0] : asr.AStop[0];
				limitPrice = asr.Target1.ContainsValue(0) ? asr.Target1[0] : asr.ATgt1[0];
				
				//enableTrade = false; // limit 1 trade / day
				if (execution.Order.OrderAction == OrderAction.Buy)
				{
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					
					limitPrice = limitPrice - 1 * TickSize;
					
					limitOrder = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, limitPrice, 0, "exitTrigger1", "ATgt1");
					stopOrder = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 0, stopPrice, "exitTrigger1", "AStop");
				}
				
				if (execution.Order.OrderAction == OrderAction.Sell)
				{					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					
					limitPrice = limitPrice + 1 * TickSize;
					
					limitOrder = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, limitPrice, 0, "exitTrigger1", "ATgt1");
					stopOrder = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 0, stopPrice, "exitTrigger1", "AStop");
				}
				
				if (execution.Order.Name == limitOrder.Name || execution.Order.Name == stopOrder.Name)
				{
					entryOrder = null;
					limitOrder = null;
					stopOrder = null;
				}				
			} 
				
		}
		#endregion		
		
		protected override void OnOrderUpdate(IOrder order)
		{
			Print(order.ToString());
			
			if (entryOrder != null && entryOrder == order)
			{
				if (order.OrderState == OrderState.Cancelled)
				{
					DrawDot(CurrentBar+"cancelled", false, 0, Median[0], Color.Red);
					
					if (limitOrder != null && limitOrder.OrderState == OrderState.Working)
						CancelOrder(limitOrder);

					if (stopOrder != null && stopOrder.OrderState == OrderState.Working)
						CancelOrder(stopOrder);
					
					entryOrder = null;
				}
			}
			
			if (limitOrder != null && limitOrder == order)
			{
				if (order.OrderState == OrderState.Cancelled)
				{
					DrawDot(CurrentBar+"lo", false, 0, Median[0]+2*TickSize, Color.Pink);
					limitOrder = null;
				}
			}
			
			if (stopOrder != null && stopOrder == order)
			{
				if (order.OrderState == OrderState.Cancelled)
				{
					DrawDot(CurrentBar+"so", false, 0, Median[0]-2*TickSize, Color.Purple);
					stopOrder = null;
				}
			}
		}


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
        public int StartHour
        {
            get { return startHour; }
            set { startHour = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StartMinute
        {
            get { return startMinute; }
            set { startMinute = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopHour
        {
            get { return stopHour; }
            set { stopHour = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopMinute
        {
            get { return stopMinute; }
            set { stopMinute = value; }
        }

/*		
        [Description("")]
        [GridCategory("Parameters")]
        public int SinceEntry
        {
            get { return sinceEntry; }
            set { sinceEntry = Math.Max(0, value); }
        }
*/
        #endregion
    }
}
