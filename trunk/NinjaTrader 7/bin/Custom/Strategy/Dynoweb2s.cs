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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("This is designed to trade YM using 5 min bars")]
    public class Dynoweb2s : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double target1 = 1.0; // Default setting for Target1
        private double target2 = 1.5; // Default setting for Target2
        private double target3 = 2.0; // Default setting for Target3
		
			int ema1 = 14;
			int ema2 = 28;
			int lengthFast = 2;
			int lengthSlow = 6;
			
        // User defined variables (add any user defined variables below)
		Dynoweb2 dw;	// my indicator for this strategy

		IOrder entryLongOrder = null;
		IOrder entryShortOrder = null;
		IExecution openExecution = null;
		
		int orderSize = 3;
		
		double targetPrice1 = 0;
		double targetPrice2 = 0;
		double targetPrice3 = 0;
		double stopPrice = 0;
		
		IOrder closeOrderStop1 = null;
		IOrder closeOrderStop2 = null;
		IOrder closeOrderStop3 = null;
		

		int tradeCount = 0;
		double dayNetBalance = 0;	// balance for entire trade day
		
		double credit = 0;			
		double debit = 0;
				
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			dw = Dynoweb2(Ema1, ema2, LengthFast, LengthSlow, Target1, Target2, Target3);
			
			dw.Plots[10].Pen.Color = Color.SteelBlue;
			dw.Plots[11].Pen.Color = Color.DarkBlue;
			dw.Plots[11].Pen.DashStyle = DashStyle.Solid;
			dw.Plots[11].PlotStyle = PlotStyle.Line;
			dw.Plots[12].Pen.Color = Color.Fuchsia;
			Add(dw);
			Add(PitColor(Color.Black, 83000, 25, 161500));
			
			Font labelFont = new Font("Arial", 12, FontStyle.Bold);
			String label = Name + " " + Instrument.FullName +				
				"\n" + Instrument.MasterInstrument.Description +
				"\nTickSize: " + Instrument.MasterInstrument.TickSize +
				" TickValue: $" + Instrument.MasterInstrument.PointValue * Instrument.MasterInstrument.TickSize; 
			
			//Add(BMTChartLabel(Color.DarkBlue, labelFont, Color.Yellow, 25, label, TextPosition.TopRight));
			
			Add(this.RicksChartLabel(Color.DarkBlue, Color.Yellow, label, labelFont, 25, TextPosition.TopLeft));
			
			ExitOnClose = true;
            CalculateOnBarClose = true;
			Unmanaged = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			DateTime exp = new DateTime(2015, 1, 1);
			if (ToDay(Time[0]) > ToDay(exp))
				return;
			
			if (BarsPeriod.Id != PeriodType.Minute) 
				return;

			// If it's Friday, do not trade.
		    //if (Time[0].DayOfWeek == DayOfWeek.Tuesday)
        	//	return;

		    if (Time[0].Hour < 8 || Time[0].Hour == 8 && Time[0].Minute < 20 || Time[0].Hour >= 14)
			{
				// reset these each day
				tradeCount = 0;
				dayNetBalance = 0;
        		return;
			}

			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				if (tradeCount < 4 && dayNetBalance <= 0)
				{
					if (dw.LongEntry.ContainsValue(0)) 
					{
						DrawArrowUp(CurrentBar.ToString()+"LE", 0, Low[0] - 5 * TickSize, Color.Green);

						if (entryLongOrder != null && entryLongOrder.OrderState == OrderState.Working)
						{
							CancelOrder(entryLongOrder);
						}
						
						entryLongOrder = SubmitOrder(0, OrderAction.Buy, OrderType.StopLimit, orderSize, 
							dw.LongEntry[0], dw.LongEntry[0], "openOrder", "Open Long");
						
						stopPrice   = dw.LongStop[0];
						targetPrice1 = dw.LongTgt2[0];
						targetPrice2 = dw.LongTgt3[0];
					} 
					if (dw.ShortEntry.ContainsValue(0)) 
					{
						DrawArrowDown(CurrentBar.ToString()+"SE", 0, High[0] + 5 * TickSize, Color.Red);
						
						if (entryShortOrder != null && entryShortOrder.OrderState == OrderState.Working)
						{
							CancelOrder(entryShortOrder);
						}
						
						entryShortOrder = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, orderSize, 
							dw.ShortEntry[0], dw.ShortEntry[0], "openOrder", "Open Short");
						
						stopPrice   = dw.ShortStop[0];
						targetPrice1 = dw.ShortTgt2[0];
						targetPrice2 = dw.ShortTgt3[0];
					} 
				}
			} 
			
			OrderManagement();
        }

		/// <summary>
		/// Use this to adjust trailing stops or to cancel open orders which are no longer valid
		/// </summary>
		private void OrderManagement()
		{
			//double limitPrice = 0;
			double stopPrice = 0;
		
			// cancel order if price action disqualifies order
			if (entryLongOrder != null && entryLongOrder.OrderState == OrderState.Working)
			{
				if (dw.FastLine[0] <= dw.SlowLine[0]
					|| Close[0] <= dw.TrailingStop[0])
				{
					CancelOrder(entryLongOrder);
				}
			}
			if (entryShortOrder != null && entryShortOrder.OrderState == OrderState.Working)
			{
				//Print(Time + " checking if order is still valid");
				if (dw.FastLine[0] >= dw.SlowLine[0]
					|| Close[0] >= dw.TrailingStop[0])
				{
					CancelOrder(entryShortOrder);
				}
			}
//			if (entryShortOrder != null && entryShortOrder.OrderState != OrderState.Working)
//			{
//				Print(Time + " "  + entryShortOrder.OrderState);
//			}
			
			// runner stop adjustment
			if (entryLongOrder != null && entryLongOrder.OrderState == OrderState.Filled)
			{
				//Print(Time + " checking if stop adjustment is required");
				if (openExecution.Quantity > 2					
					&& closeOrderStop3 != null
					&& closeOrderStop3.StopPrice > openExecution.Order.AvgFillPrice)
				{
					//Print(Time + " target1 adjustment has already been made, checking if additional stop adjustment is required");
					stopPrice = Instrument.MasterInstrument.Round2TickSize(dw.TrailingStop[HighestBar(dw.TrailingStop, BarsSinceEntry())]);
					if (stopPrice > closeOrderStop3.StopPrice)
					{
						//Print(Time + " setting stop to trail at " + stopPrice);
						ChangeOrder(closeOrderStop3, closeOrderStop3.Quantity, 0, stopPrice);
					}
				}
			}
			
			if (entryShortOrder != null && entryShortOrder.OrderState == OrderState.Filled)
			{
				//Print(Time + " checking if stop adjustment is required");
				if (openExecution.Quantity > 2					
					&& closeOrderStop3 != null
					&& closeOrderStop3.StopPrice < openExecution.Order.AvgFillPrice)
				{
					//Print(Time + " target1 adjustment has already been made, checking if additional stop adjustment is required");
					stopPrice = Instrument.MasterInstrument.Round2TickSize(dw.TrailingStop[LowestBar(dw.TrailingStop, BarsSinceEntry())]);
					if (stopPrice < closeOrderStop3.StopPrice)
					{
						//Print(Time + " setting stop to trail at " + stopPrice);
						ChangeOrder(closeOrderStop3, closeOrderStop3.Quantity, 0, stopPrice);
					}
				}
			}
//			
//			
//			
//			double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
//			//double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];
//
//			// if this is true, then the stop moves up
//			if (highSinceOpen > entryLongOrder.AvgFillPrice + 10)
//			{
//				
//				stopPrice = Math.Max(entryLongOrder.AvgFillPrice + 1, HighestBar(dw.TrailingStop, BarsSinceEntry()));
//				ChangeOrder(closeOrderStop2, closeOrderStop2.Quantity, closeOrderStop2.LimitPrice, stopPrice);
//			}
//			

			// Change long close order to close if stop trigger (closed below channel)
    		//if (null != closeOrderLongStop)
			//{
			//	if (closeOrderLong == null
			//		&& Low[0] < channelLow
			//		&& barNumberSinceFilled != barNumberSinceFilled
			//		)
			//}
		}
		
		
		#region OnExecution
		protected override void OnExecution(IExecution execution)
		{
			if (execution.Order == null)
			{
				Print(Time + " -->> OnExecution.Order is null");
				return;
			}
			
			// Order filled, set stops and targets
			if (execution.Order.OrderState == OrderState.Filled)
			{
//				Print(Time + " " + execution);
//				Print(Time + " " + execution.Order);
								
				// set for longs
				if (execution.Order.OrderAction == OrderAction.Buy)
				{
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					openExecution = execution;
					if (targetPrice1 - execution.Order.AvgFillPrice < 8) 
					{
						targetPrice1 = execution.Order.AvgFillPrice + 15 * TickSize;
						targetPrice2 = execution.Order.AvgFillPrice + 25 * TickSize;
					}

					debit = execution.Order.AvgFillPrice * execution.Order.Quantity;	// buy
					SubmitOrder(0, OrderAction.Sell, OrderType.Limit, 1, targetPrice1, 0, "closeTrade1", "Close1");
					closeOrderStop1 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, 1, 0, stopPrice, "closeTrade1", "Exit1");
					
					if (execution.Order.Quantity > 1)
					{
						SubmitOrder(0, OrderAction.Sell, OrderType.Limit, 1, targetPrice2, 0, "closeTrade2", "Close2");	
						closeOrderStop2 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, 1, 0, stopPrice, "closeTrade2", "Exit2");
					}
					
					if (execution.Order.Quantity > 2)
					{
						// no target, just stop
						//SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Quantity - 2, limitPrice3 + 10 * TickSize, 0, "closeDayTrade3", "Close Trade3");	
						closeOrderStop3 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Quantity - 2, 0, stopPrice, "closeTrade3", "Exit3");
					}
					tradeCount++;
				}
				
				// set for shorts
				if (execution.Order.OrderAction == OrderAction.SellShort)
				{					
					//DrawDot(CurrentBar + "limitPrice", false, 0, limitPrice, Color.Green);
					openExecution = execution;
					if (execution.Order.AvgFillPrice - targetPrice2 < 8) 
					{
						targetPrice1 = execution.Order.AvgFillPrice - 15 * TickSize;
						targetPrice2 = execution.Order.AvgFillPrice - 25 * TickSize;
					}
					
					credit = execution.Order.AvgFillPrice * execution.Order.Quantity;	// sellShort
					SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, 1, targetPrice1, 0, "closeTrade1", "Close1");
					closeOrderStop1 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, 1, 0, stopPrice, "closeTrade1", "Exit1");
					
					if (execution.Order.Quantity > 1)
					{
						SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, 1, targetPrice2, 0, "closeTrade2", "Close2");
						closeOrderStop2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, 1, 0, stopPrice, "closeTrade2", "Exit2");
					}
					
					if (execution.Order.Quantity > 2)
					{
						//SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Quantity - 2, targetPrice2 - 10 * TickSize, 0, "closeDayTrade2", "Close Short Limit 3");
						closeOrderStop3 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Quantity - 2, 0, stopPrice, "closeTrade3", "Exit3");
					}
					
					tradeCount++;
					//Print(Time + " " + entryShortOrder);
				}

				if (execution.Order.OrderAction == OrderAction.Sell)
				{
					// adjust stop
					if (execution.Name == "Close1")
					{
						if (openExecution.Quantity > 1
							&& closeOrderStop2 != null)
						{
							ChangeOrder(closeOrderStop2, closeOrderStop2.Quantity, 0, openExecution.Order.AvgFillPrice + 1 * TickSize);
						}
						if (openExecution.Quantity > 2
							&& closeOrderStop3 != null)
						{
							ChangeOrder(closeOrderStop3, closeOrderStop3.Quantity, 0, openExecution.Order.AvgFillPrice + 1 * TickSize);
						}
					}
					
					//Print(Time + " openExecution  " + openExecution);
					//Print(Time + " openOrder  " + openExecution.Order);
					//Print(Time + " closeExecution " + execution);
					//Print(Time + " closeOrder     " + execution.Order);
					
					credit += execution.Order.AvgFillPrice * execution.Quantity;			// sell
				}
				
				if (execution.Order.OrderAction == OrderAction.BuyToCover)
				{
					// adjust stop
					if (execution.Name == "Close1")
					{
						if (openExecution.Quantity > 1
							&& closeOrderStop2 != null)
						{
							ChangeOrder(closeOrderStop2, closeOrderStop2.Quantity, 0, openExecution.Order.AvgFillPrice - 1 * TickSize);
						}
						if (openExecution.Quantity > 2
							&& closeOrderStop3 != null)
						{
							ChangeOrder(closeOrderStop3, closeOrderStop3.Quantity, 0, openExecution.Order.AvgFillPrice - 1 * TickSize);
						}
					}
					
					//Print(Time + " openExecution  " + openExecution);
					//Print(Time + " openOrder  " + openExecution.Order);
					//Print(Time + " closeExecution " + execution);
					//Print(Time + " closeOrder     " + execution.Order);
					
					debit += execution.Order.AvgFillPrice * execution.Quantity;				// buyToCover
					
					// openExecution.Quantity * openExecution.Commission;					
					//Instrument.MasterInstrument.Round2TickSize(profitPoints));
				}
				
				//Print(Time + " tradeCount: " + tradeCount + " orderAction: " + execution.Order.OrderAction);
				//Print("");

			} 
			else 
			{
				Print(Time + " execution.Order: " + execution.Order.ToString());
			}
				
		}
		#endregion		

		protected override void OnPositionUpdate(IPosition position)
		{
			if (position.MarketPosition == MarketPosition.Flat)
			{
				double lastTrade = credit - debit;
				dayNetBalance += lastTrade;
				Print(Time + " Day trade count: "  + tradeCount + " dayNetBalance: " + dayNetBalance + " lastTrade: " + lastTrade + " credit: " + credit + " debit: " + debit); 
				credit  = 0;
				debit = 0;
			}
		}

		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Ema1
        {
            get { return ema1; }
            set { ema1 = Math.Max(0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int LengthFast
        {
            get { return lengthFast; }
            set { lengthFast = Math.Max(0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int LengthSlow
        {
            get { return lengthSlow; }
            set { lengthSlow = Math.Max(0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public double Target1
        {
            get { return target1; }
            set { target1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Target2
        {
            get { return target2; }
            set { target2 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Target3
        {
            get { return target3; }
            set { target3 = Math.Max(0, value); }
        }
        #endregion
    }
}
