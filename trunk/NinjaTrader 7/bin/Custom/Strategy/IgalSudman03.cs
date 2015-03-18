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
    /// CL 3 min bars
	/// 
	/// 1. ATR for stop loss rounded up even if its 7.1 we go to 8
	/// 2. profit target % of atr rounded down , even if its 7.9 we go to 7 but we are able to change the target to anyting 75% to 110%
	/// 3. BE is a % of profit rounded down can be 50 or 90 before we go to BE
	/// 4. another BE PLUS is when we get a certain % of profit 70-80-90 whatever we put to go to BE plus ??
	/// 5. trail stop when we get to % profit we trail by ( parameter input ) ex. if we get to 80-90% of profit target for every tick up we trail by 1 tick or for every 2 ticks we trail by 1
	/// 6. we may input the profit target to be 150% in order to let the trail work beacuse if we are right we may only lose 1 tick on the pullback fter hitting 80% of profit but have the opportunity to trail 3-4-5 ticks up which makes a difference on the profit PNL but does not really change what we give up ( at most 1-2 ticks )
	/// 7. paprameter for time in market , we will not trade during news or when market closes for ex in europe
	/// 
    /// </summary>
    [Description("CL 3 min bars")]
    public class IgalSudman03 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
		
		double channelHigh = 0;
		double channelLow = 0;
		double channelSize = 0;
		
		int startTime = 80000;	// 8 AM
		
		double upperTrigger = 0;
		double lowerTrigger = 0;
		double upperStopLoss = 0;
		double lowerStopLoss = 0;
		double atr = 0;
		
		int Qty1 = 1;

		IOrder buyOrder1 = null;
		IOrder sellOrder1 = null;
		IOrder closeLongOrderLimit1 = null;
		IOrder closeLongOrderStop1 = null;
		IOrder closeLongOrder1 = null;
		IOrder closeShortOrderLimit1 = null;
		IOrder closeShortOrderStop1 = null;
		IOrder closeShortOrder1 = null;
		
		double limitPrice = 0;
		double stopPrice = 0;
		
		string orderPrefix = "Igal03"; 
		
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Unmanaged = true;				// Use unmanaged order methods
			
			//Slippage = 2;
			BarsRequired = 22;
            CalculateOnBarClose = true;		// Onbar update happens only on the start of a new bar vrs each tick
			ExitOnClose = true;				// Closes open positions at the end of the session
			IncludeCommission = true;		// Commissions are used in the calculation of the profit/loss
			TraceOrders = false;			// Trace orders in the output window, used for debugging, normally false
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// reset variables at the start of each day
			if (Bars.BarsSinceSession == 1)
			{
				channelHigh = 0;
				channelLow = 0;
				channelSize = 0;
				upperTrigger = 0;
				lowerTrigger = 0;
				ResetTrades();
			}
			
			if (channelSize == 0 && ToTime(Time[0]) == startTime + 3000)
			{
				EstablishOpeningChannel();
			}

			// Open pit session trading only
			if (ToTime(Time[0]) < startTime || ToTime(Time[0]) > 150000)
			{
				CloseWorkingOrders();
				return;
			}
			
			if (channelSize != 0)
			{
				if (Time[0].Minute % 15 == 0)
				{
					CloseWorkingOrders();
					atr = CalcAtr();
					upperTrigger = MAX(High, 5)[0] + atr;
					lowerTrigger = MIN(Low, 5)[0] - atr;
					
					if (upperTrigger < channelLow || upperTrigger > channelHigh)
					{
						upperTrigger += atr*0.5;
					}
					if (lowerTrigger < channelLow || lowerTrigger > channelHigh)
					{
						lowerTrigger -= atr*0.5;
					}
					
					DrawLine(CurrentBar + "upperTrigger", 0, upperTrigger, -5, upperTrigger, Color.Blue);
					DrawLine(CurrentBar + "lowerTrigger", 0, lowerTrigger, -5, lowerTrigger, Color.Blue);
					
					upperStopLoss = upperTrigger + atr;
					lowerStopLoss = lowerTrigger - atr;
					
					DrawLine(CurrentBar + "upperStopLoss", 0, upperStopLoss, -5, upperStopLoss, Color.Red);
					DrawLine(CurrentBar + "lowerStopLoss", 0, lowerStopLoss, -5, lowerStopLoss, Color.Red);
				}
			}
			
//			if (buyOrder1 != null)
//			{
//				if (TraceOrders == true)
//				{
//					//Print(Time + " isFlat() " + isFlat());
//					if (buyOrder1.OrderState != OrderState.Working)
//						Print(Time + " OrderState: " + buyOrder1.OrderState);
//				}
//				if (buyOrder1.OrderState == OrderState.Cancelled) 
//					buyOrder1 = null;
//			}			
			
			if (isFlat() && lowerTrigger != 0 && buyOrder1 == null)
			{
				limitPrice = lowerTrigger;
				stopPrice = limitPrice;
				buyOrder1 = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, Qty1, limitPrice, stopPrice, 
					orderPrefix + "oco1", "B1");

				double target = lowerTrigger + atr*0.75;
				DrawLine(CurrentBar+"longTarget", 0, target, -5, target, Color.Green);				
			}
			
			if (isFlat() && upperTrigger != 0 && sellOrder1 == null)
			{
				limitPrice = upperTrigger;
				stopPrice = limitPrice;
				sellOrder1 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, Qty1, limitPrice, stopPrice, 
					orderPrefix + "oco1", "S1");
				
				double target = upperTrigger - atr*0.75;
				DrawLine(CurrentBar+"shortTarget", 0, target, -5, target, Color.Green);
			}
			
        }
		
		protected override void OnExecution(IExecution execution)
		{
			double limitPrice = 0;
			double stopPrice = 0;
			
			if (execution.Order == null)
			{
//				ResetTrades();
				Print(Time + " executed/reset on close");
				return;
			}
			
			if (TraceOrders)
			{
				//Print(Time + " execution: " + execution.ToString());
				//Print(Time + " execution.Order: " + execution.Order.ToString());
			}
			
			// ============================================
			// New long order placed, now set stops/limits
			// ============================================
			if (buyOrder1 != null && buyOrder1 == execution.Order)
			{
				//Print(Time + " buyOrder1: " + buyOrder1);
				
				if (closeLongOrderLimit1 == null) 
				{
					limitPrice = 0;
					stopPrice = buyOrder1.AvgFillPrice - atr;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeLongOrderStop1 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CSB1");
					//Print(Time + " stopPrice: " + stopPrice);

					stopPrice = 0;
					limitPrice = buyOrder1.AvgFillPrice + atr*0.75;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeLongOrderLimit1 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseB1", "CLB1");
					//Print(Time + " limitPrice: " + limitPrice);
				} 
			} 


			// ============================================
			// New short order placed, now set stops/limits
			// ============================================
			if (sellOrder1 != null && sellOrder1 == execution.Order)
			{
				//Print(Time + " sellOrder1: " + sellOrder1);
				
				if (closeShortOrderLimit1 == null) 
				{
					limitPrice = 0;
					stopPrice = sellOrder1.AvgFillPrice + atr;
					DrawDot(CurrentBar + "stopPrice", false, 0, stopPrice, Color.Red);
					closeLongOrderStop1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS1", "CSS1");

					stopPrice = 0;
					limitPrice = sellOrder1.AvgFillPrice - atr*0.75;
					DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					closeShortOrderLimit1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity, 
						limitPrice, stopPrice, orderPrefix + "ocoCloseS1", "CLS1");
				} 
			} 
			
			// ===================================================
			//   Trade hit Long limit
			// ===================================================
			if (closeLongOrderLimit1 != null && closeLongOrderLimit1 == execution.Order)
			{
				ResetTrades();
			}
			
			// ===================================================
			//   Trade hit Long stop 
			// ===================================================
			if (closeLongOrderStop1 != null && closeLongOrderStop1 == execution.Order)
			{
				ResetTrades();
			}
			
			// ===================================================
			//   Trade hit Long limit
			// ===================================================
			if (closeShortOrderLimit1 != null && closeShortOrderLimit1 == execution.Order)
			{
				ResetTrades();
			}
			
			// ===================================================
			//   Trade hit Long stop 
			// ===================================================
			if (closeShortOrderStop1 != null && closeShortOrderStop1 == execution.Order)
			{
				ResetTrades();
			}
			
		}
		
		private void CloseWorkingOrders()
		{
			if (buyOrder1 != null && buyOrder1.OrderState == OrderState.Working)
			{
				CancelOrder(buyOrder1);
				buyOrder1 = null;
			}
			if (sellOrder1 != null && sellOrder1.OrderState == OrderState.Working)
			{
				CancelOrder(sellOrder1);
				sellOrder1 = null;
			}					
		}	
		
		private void ResetTrades()
		{
			closeLongOrderStop1 = null;
			closeLongOrderLimit1 = null;
			closeShortOrderLimit1 = null;
			closeShortOrderStop1 = null;
			buyOrder1 = null;
			sellOrder1 = null;
			lowerTrigger = 0;
			upperTrigger = 0;
		}
			
		private void EstablishOpeningChannel()
		{
			channelHigh = MAX(High, 10)[0];
			channelLow = MIN(Low, 10)[0];
			channelSize = channelHigh - channelLow;
			DrawRectangle(CurrentBar + "channel", false, 10, channelHigh, -30, channelLow, Color.SeaGreen, Color.SeaGreen, 1);
		}
		
		private double CalcAtr()
		{
			double _atr = 0;
			for (int i = 0; i < 10; i++) 
			{
				_atr += ATR(10)[i];
			}
			for (int i = 0; i < 5; i++) 
			{
				_atr += ATR(5)[i];
			}
			return Instrument.MasterInstrument.Round2TickSize(_atr/15);
		}
		
		private bool isFlat()
		{
			if (Position.MarketPosition == MarketPosition.Flat)
				return true;
			else
				return false;
		}
		
		private bool isLong()
		{
			if (Position.MarketPosition == MarketPosition.Long)
				return true;
			else
				return false;
		}
		
		private bool isShort()
		{
			if (Position.MarketPosition == MarketPosition.Short)
				return true;
			else
				return false;
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
