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
    /// RSI with a Stop Loss and Profit Target
    /// </summary>
    [Description("RSI with a Stop Loss and Profit Target")]
    public class Tutorial2 : Strategy
    {
        #region Variables
        // Wizard generated variables
		// These have been optimized for /CL, but in 8 months, there were only 5 trades, 4 reached max profit $500
		/*
        private int rSIPeriod = 26; // Default setting for RSIPeriod
        private int rSISmooth = 8; // Default setting for RSISmooth
        private int profitTarget = 14; // Default setting for ProfitTarget
        private int stopLoss = 6; // Default setting for StopLoss
		private int lowThreshold = 20;
*/
		// These have been optimized for /CL, but in 8 months, there were only 4 trades, all winners for $640
		
        private int rSIPeriod = 33; // Default setting for RSIPeriod
        private int rSISmooth = 10; // Default setting for RSISmooth
        private int profitTarget = 16; // Default setting for ProfitTarget
        private int stopLoss = 20; // Default setting for StopLoss
		private int lowThreshold = 22;
		
		
		// User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
			Add(RSI(RSIPeriod, RSISmooth));
			
			// Add stop loss and profit target orders
			SetStopLoss(CalculationMode.Ticks, StopLoss);
			SetProfitTarget(CalculationMode.Ticks, ProfitTarget);
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar <  RSIPeriod)
				return;
			
			if (CrossAbove(RSI(RSIPeriod, RSISmooth), lowThreshold, 1)) {
				EnterLong();
			}
        }

        #region Properties
        [Description("RSI Period")]
        [GridCategory("Parameters")]
        public int RSIPeriod
        {
            get { return rSIPeriod; }
            set { rSIPeriod = Math.Max(1, value); }
        }

        [Description("RSI Smooth")]
        [GridCategory("Parameters")]
        public int RSISmooth
        {
            get { return rSISmooth; }
            set { rSISmooth = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int LowThreshold
        {
            get { return lowThreshold; }
            set { lowThreshold = Math.Max(1, value); }
        }
        #endregion
    }
}
