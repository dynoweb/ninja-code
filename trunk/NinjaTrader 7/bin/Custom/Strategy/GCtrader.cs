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
using PriceActionSwingOscillator.Utility;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
	/// 
	/// Written in NinjaScript by Rick Cromer (dynoweb)  Jan 26 2015
	/// 	
	/// This code worked well with FDAX, ZL, GC & NG
	/// It also worked using a 1 hour period with a 40 period lookback (period1 & period2) and stop and targets set.
	/// 
    /// </summary>
    [Description("GC 30 min bars - based on Kevin Davey's pdf 'A Simple Gold System'")]
    public class GCtrader : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int period1 = 80; // Default setting for Period1
        private int period2 = 80; // Default setting for Period2
        // User defined variables (add any user defined variables below)
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
		/// 
        /// </summary>
        protected override void Initialize()
        {
			Period2 = Period1;
			
            //SetProfitTarget("", CalculationMode.Ticks, 200);
            //SetStopLoss("", CalculationMode.Ticks, 300, false);
            //SetTrailStop("", CalculationMode.Percent, dparm2, false);

			Slippage = 2;
            CalculateOnBarClose = true;
			ExitOnClose = false;
			IncludeCommission = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {
			if (isBull())
			{
				EnterLong();
			}
			if (isBear())
			{
				EnterShort();
			}
			
			if (isLong())
			{
				if (LowestBar(Low, Period2) == 0)
				{
					ExitLong();
				}
			}
				
			if (isShort())
			{
				if (HighestBar(High, Period2) == 0)
				{
					ExitShort();
				}
			}
				
				
        }
		
		private bool isBull()
		{
			if (HighestBar(High, Period1) == 0)
			{
				return true;
			}
			return false;
		}
		
		private bool isBear()
		{
			if (LowestBar(Low, Period1) == 0)
			{
				return true;
			}
			return false;
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
        public int Period1
        {
            get { return period1; }
            set { period1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period2
        {
            get { return period2; }
            set { period2 = Math.Max(1, value); }
        }

        #endregion
    }
}
