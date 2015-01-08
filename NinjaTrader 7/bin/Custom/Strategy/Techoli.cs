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
using PriceActionSwing.Base;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
	/// Written by Rick Cromer (Dynoweb) based on John's (techoli) strategy.	
    /// </summary>
    [Description("FDAX 15 min bars - This is based on a strategy by BigMikeTrading.com member techoli")]
    public class Techoli : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int iparm1 = 3; // Default setting for Iparm1
        private int iparm2 = 1; // Default setting for Iparm2
        private double dparm1 = 0.009; // Default setting for Dparm1
        private double dparm2 = 0.009; // Default setting for Dparm2
        private int period1 = 30; // Default setting for Period1
        private int period2 = 27; // Default setting for Period2
        // User defined variables (add any user defined variables below)
		
		// PriceActionSwing inputs
		int dtbStrength = 1;
		double swingSize = 1.0; 
		SwingStyle swingType = SwingStyle.Gann;
		bool useCloseValues = false;
		NinjaTrader.Indicator.PriceActionSwing pas = null;
		
		double prevHigh = 0;
		double prevLow = 0;
		bool lowerHigh = false;
		bool higherLow = false;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			pas = PriceActionSwing(dtbStrength, swingSize, swingType, useCloseValues);
			Add(pas);			
			
            SetProfitTarget("", CalculationMode.Percent, dparm1);
            SetStopLoss("", CalculationMode.Percent, dparm1, false);
            //SetTrailStop("", CalculationMode.Percent, dparm2, false);

            CalculateOnBarClose = true;
			ExitOnClose = false;
			IncludeCommission = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()			
        {			
			updateState();
			
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				if (isBull())
				{
					EnterLongStop(High[0] + TickSize, "HL Long");
				}
				if (isBear())
				{
					EnterShortStop(Low[0] - TickSize, "LH Short");
				}
			} 
			else
			{
				if (Iparm1 == 0 || BarsSinceEntry() >= Iparm1)
				{
					if (isLong())
						ExitLong();
					if (isShort())
						ExitShort();
				}
			}
        }

		private void updateState()
		{
			// if we just switched from a High GannSwing to Low 
			if (pas.GannSwing[1] >= High[1]	&& pas.GannSwing[0] <= Low[0])
				prevHigh = pas.GannSwing[1];
			
			// if we just switched from a Low GannSwing to High
			if (pas.GannSwing[1] <= Low[1]	&& pas.GannSwing[0] >= High[0])
				prevLow = pas.GannSwing[1];
			
			// while in a high, is this high lower than the previous high
			lowerHigh = false;
			if (pas.GannSwing[0] >= High[0] && High[0] < prevHigh)
				lowerHigh = true;
			
			// while in a low, is this low higher than the previous low
			higherLow = false;
			if (pas.GannSwing[0] <= Low[0] && Low[0] > prevLow)
				higherLow = true;
		}

		#region helpers
		private bool isBull() 
		{
			bool bull = false;
			if (pas.GannSwing[0] == Low[0] && higherLow)
			{
				bull = true;
			}
			return bull;
		}		

		private bool isBear() 
		{
			bool bear = false;
			if (pas.GannSwing[0] == High[0] && lowerHigh)
			{
				bear = true;
			}
			return bear;
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
		#endregion
		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm1
        {
            get { return iparm1; }
            set { iparm1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm2
        {
            get { return iparm2; }
            set { iparm2 = Math.Max(0, value); }
        }

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

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm1
        {
            get { return dparm1; }
            set { dparm1 = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm2
        {
            get { return dparm2; }
            set { dparm2 = Math.Max(0.000, value); }
        }
        #endregion
    }
}
