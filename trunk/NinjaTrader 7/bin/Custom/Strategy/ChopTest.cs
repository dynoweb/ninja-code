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
    [Description("Enter the description of your strategy here")]
    public class ChopTest : Strategy
    {
        #region Variables
		
			int hullPeriod = 28;
			int stop = 10;
			int target1 = 33;
		
			int adxPeriod = 20;
			bool conservativeMode = true;
			static int RISING = 1;
			static int FALLING = -1;
			static int FLAT = 0;
		
			int		sharkyChopLookBack	= 50;		// Maximum number of lookback bars for Sharkychop indicator		
			double	sharkyChopTrigger	= 0.02;		// Default setting for SharkyChopTrigger          
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(HMARick(hullPeriod, 30));			
			Add(Sharkchop_v01_1(13, 1, 1, 16));	
			Add(_ADXVMA_Alerts_v01_5_1(adxPeriod, conservativeMode));
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			HMA hma = HMA(hullPeriod);
			Sharkchop_v01_1 sc = Sharkchop_v01_1(13, 1, 1, 16);
			_ADXVMA_Alerts_v01_5_1 adx = _ADXVMA_Alerts_v01_5_1(adxPeriod, conservativeMode);
			//EMA ema = EMA(20);
			
			ManageOrder();
			
			if (Position.MarketPosition != MarketPosition.Flat) return;
			

			if (Rising(hma) 
				&& adx.Signal[0] == RISING
				&& MAX(sc.Long, Math.Min(Bars.BarsSinceSession, sharkyChopLookBack))[0] >= sharkyChopTrigger
				&& sc.Long[0] > sc.Long[1]
				)  
			{
				GoLong();
			} else if (Falling(hma) 
				&& adx.Signal[0] == FALLING
				&& MAX(sc.Short, Math.Min(Bars.BarsSinceSession, sharkyChopLookBack))[0] >= sharkyChopTrigger
				&& sc.Short[0] > sc.Short[1]
				)
			{
				GoShort();
			}	
        }

		
        #region GoLong
		private void GoLong() 
		{
			SetStopLoss("Long", CalculationMode.Price, Close[0] - (Stop*TickSize), false);
			//SetStopLoss("target2", CalculationMode.Price, Close[0] - (Stop*TickSize), false);
			//SetStopLoss("target3", CalculationMode.Price, Close[0] - (Stop*TickSize), false);
			
			SetProfitTarget("Long", CalculationMode.Price, Close[0] + (Target1*TickSize));
			//SetProfitTarget("target2", CalculationMode.Price, Close[0] + ((Target1+Target2)*TickSize));
			//SetProfitTarget("target3", CalculationMode.Price, Close[0] + ((Target1+Target2+Target3)*TickSize));
			
			EnterLong("Long");
			//EnterLong("target2");
			//EnterLong("target3");
		}
		#endregion // GoLong

        #region GoShort
		private void GoShort() 
		{
			SetStopLoss("Short", CalculationMode.Price, Close[0] + (Stop*TickSize), false);
			//SetStopLoss("target2", CalculationMode.Price, Close[0] + (Stop*TickSize), false);
			//SetStopLoss("target3", CalculationMode.Price, Close[0] + (Stop*TickSize), false);
			
			SetProfitTarget("Short", CalculationMode.Price, Close[0] - (Target1*TickSize));
			//SetProfitTarget("target2", CalculationMode.Price, Close[0] - ((Target1+Target2)*TickSize));
			//SetProfitTarget("target3", CalculationMode.Price, Close[0] - ((Target1+Target2+Target3)*TickSize));
			
			EnterShort("Short");
			//EnterShort("target2");
			//EnterShort("target3");
		}
		#endregion // GoShort
		
		
        #region ManageOrder
		private void ManageOrder() 
		{
			/*
			if (Position.MarketPosition == MarketPosition.Long)
			{
				if (BE2 && High[0] > Position.AvgPrice + (Target1*TickSize))
					SetStopLoss("target2", CalculationMode.Price, Position.AvgPrice, false);
					
				if (BE3 && High[0] > Position.AvgPrice + (Target1*TickSize))
					SetStopLoss("target3", CalculationMode.Price, Position.AvgPrice, false);
			}
			
			if (Position.MarketPosition == MarketPosition.Short)
			{
				if (BE2 && High[0] < Position.AvgPrice - (Target1*TickSize))
					SetStopLoss("target2", CalculationMode.Price, Position.AvgPrice, false);
					
				if (BE3 && High[0] < Position.AvgPrice - (Target1*TickSize))
					SetStopLoss("target3", CalculationMode.Price, Position.AvgPrice, false);
			}
			*/
		}
		#endregion // ManageOrder
		
        #region Properties
		
        [Description("")]
        [GridCategory("Parameters")]
		public int Target1
        {
            get { return target1; }
            set { target1 = Math.Max(1, value); }
        }
		
        [GridCategory("Parameters")]
		public int Stop
        {
            get { return stop; }
            set { stop = Math.Max(1, value); }
        }

        #endregion
    }
}
