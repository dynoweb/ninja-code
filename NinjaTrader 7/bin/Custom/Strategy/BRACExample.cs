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
    /// Strategy based on Chapter 2 of the book Building Reliable Trading Systems
	/// 
	/// Testing using daily data, 6E, CL, DX, ES, GC, NG, NQ, SI, TF, YM, ZB
	/// 1/1/2005 - 9/23/2013
	/// 
    /// </summary>
    [Description("Strategy based on Chapter 2 of the book Building Reliable Trading Systems")]
    public class BRACExample : Strategy
    {
        #region Variables
        // Wizard generated variables
		/* Optimized for ma and periods back for listed instruments
        private int maPeriod = 40; // Default setting for MaPeriod
        private int periodsBack = 110; // Default setting for PeriodsBack
		*/
        private int maPeriod = 28; // Default setting for MaPeriod
        private int periodsBack = 40; // Default setting for PeriodsBack
        private int stopLoss = 1000; // Default setting for StopLoss
        private double trailingStop = 0.07; // Default setting for TrailingStop
        private int useTrailing = 0; // Default setting for UseTrailing
		private double profitTarget = 0.09;
        // User defined variables (add any user defined variables below)
		string lastTrade = "";
		string stoppedOn = "";
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();
//			if (useTrailing == 0)
            	//SetStopLoss("", CalculationMode.Ticks, (int) StopLoss/TickSize, false);
//			SetStopLoss(CalculationMode.Percent, StopLoss);
//			else
//            	SetTrailStop("", CalculationMode.Ticks, TrailingStop/TickSize, false);
			if (trailingStop > 0)
				SetTrailStop(CalculationMode.Percent, trailingStop);
			
			if (profitTarget > 0)
				SetProfitTarget(CalculationMode.Percent, profitTarget);
			
			//Add(SMA(maPeriod));
			Add(SMA(periodsBack));
			Add(HMARick(maPeriod, 50));
			
			// Order Handling enforce only 1 position long or short at any time
			EntriesPerDirection = 1; 
			EntryHandling = EntryHandling.AllEntries;
			ExitOnClose = false;

			CalculateOnBarClose = true;
			BarsRequired = maPeriod + periodsBack;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			/*
			if (Rising(SMA(maPeriod))
				&& Close[0] > Close[periodsBack]	// look back filter ,= added to ignore this filter
				)
				EnterLong();
			if (Falling(SMA(maPeriod))
				&& Close[0] < Close[periodsBack]
				)
				EnterShort();
			*/
			if (Position.MarketPosition == MarketPosition.Long)
			{
				//if (Falling(HMA(maPeriod)))
					//ExitLong();
			}
			if (Position.MarketPosition == MarketPosition.Short)
			{
				//if (Rising(HMA(maPeriod)))
					//ExitShort();
			}
			
			if (Rising(HMA(maPeriod)) 
				&& Rising(SMA(periodsBack))
				&& stoppedOn != "long"
				)
			{
				EnterLong();
				lastTrade = "long";
				stoppedOn = "";
			}
			if (Falling(HMA(maPeriod))
				&& Falling(SMA(periodsBack))
				&& stoppedOn != "short"
				)
			{
				EnterShort();
				lastTrade = "short";
				stoppedOn = "";
			}
			
        }

		protected override void OnExecution(IExecution execution)
		{
			// Remember to check the underlying IOrder object for null before trying to access its properties
			if (execution.Order != null && execution.Name == "Trail stop") // && execution.Order.OrderState == OrderState.Filled)
			{
//				Print(Time + " OnExecution " + execution.ToString());
//				Print(Time + " OnExecution     " + execution.Order.ToString());
				//stoppedOn = lastTrade;
			} 
			else
			{
			}
		}

		protected override void OnOrderUpdate(IOrder order)
		{
			//if (entryOrder != null && entryOrder == order)
			//{
				//Print(Time + " OnOrderUpdate " + order.ToString());
//				if (order.OrderState == OrderState.Cancelled)
//				{
//					// Do something here
//					entryOrder = null;
//				}
			//}
		}
		
		
        #region Properties
        [Description("Moving Average Period")]
        [GridCategory("Parameters")]
        public int MaPeriod
        {
            get { return maPeriod; }
            set { maPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int PeriodsBack
        {
            get { return periodsBack; }
            set { periodsBack = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0, value); }
        }

        [Description("Stop Loss in dollars")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(0, value); }
        }

        [Description("Trailing Stop")]
        [GridCategory("Parameters")]
        public double TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(0, value); }
        }

        [Description("0 - Don't use trailing stop, 1 - Use Trailing Stop")]
        [GridCategory("Parameters")]
        public int UseTrailing
        {
            get { return useTrailing; }
            set { useTrailing = Math.Max(0, value); }
        }
        #endregion
    }
}
