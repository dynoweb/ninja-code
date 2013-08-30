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
    /// This based on a tool that I watched the demo on called scalp trading on 1/4/12.   
	/// This is based on NQ 150 Tick chart
    /// </summary>
    [Description("This based on a tool that I watched the demo on called scalp trading on 1/4/12.   This is based on a 150 Tick chart")]
    public class STMStrategy : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int aTRPeriods = 13; // Default setting for ATRPeriods
        private double aTRRange = 6.000; // Default setting for ATRRange
        private int hullPeriod = 33; // Default setting for HullPeriod
        private int mAEnvPeriod = 160; // Default setting for MAEnvPeriod
        private double mAEnvPOffSet = 0.05; // Default setting for MAEnvPOffSet
        private int profitTarget = 5; // Default setting for ProfitTarget
        private double rPeriod = 0.002; // Default setting for RPeriod
        private int stop = 14; // Default setting for Stop
        private int trailingStop = 1; // Default setting for TrailingStop
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(HMA(HullPeriod));
            Add(MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod));
            Add(ATRTrailing(aTRRange, aTRPeriods, rPeriod));
            SetTrailStop("", CalculationMode.Ticks, TrailingStop, false);
            SetStopLoss("", CalculationMode.Ticks, Stop, false);
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Variable0 == 0) {
			}
			
			Boolean longStops = false;
			Double longStop = ATRTrailing(aTRRange, aTRPeriods, rPeriod).Upper[0];
			Double shortStop = ATRTrailing(aTRRange, aTRPeriods, rPeriod).Lower[0];
			if (Math.Abs(longStop - Close[0]) > Math.Abs(shortStop - Close[0])) {
				longStops = true;	
			}			
			
            if (longStops)
            {				
				//DrawDot("LongStops" + CurrentBar, true, 0, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle[0], Color.Blue);
			}
			
			// Condition set 1
			// show falling Hull
            //if (Falling(HMA(HullPeriod)) == true)
            if (Close[0] < ATRTrailing(aTRRange, aTRPeriods, rPeriod).Lower[0])
            {
				// Add dot if short still valid 
                //DrawDot("My dot" + CurrentBar, true, 0, Low[0] + -3 * TickSize, Color.Red);
            }
            if (Close[0] > ATRTrailing(aTRRange, aTRPeriods, rPeriod).Upper[0])
            {
				// Add dot if long still valid
                //DrawDot("My dot1" + CurrentBar, true, 0, High[0] + 3 * TickSize, Color.Blue);
            }
            if (ATRTrailing(aTRRange, aTRPeriods, rPeriod).Upper[0] > 0)
            {
				// Upper is long trailing stops
//                DrawDot("My dot2" + CurrentBar, true, 0, ATRTrailing(aTRRange, aTRPeriods, rPeriod).Upper[0] - 3 * TickSize, Color.Green);
				// Lower is short trailing stops
//                DrawDot("My dot3" + CurrentBar, true, 0, ATRTrailing(aTRRange, aTRPeriods, rPeriod).Lower[0] + 3 * TickSize, Color.Yellow);
            }

            // Condition set 2
			// reset potential long flag if MA is falling
            if (Falling(MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Upper) == true
                && Variable0 == 1)
                //Variable0 = 0;
			
            if (Rising(MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Lower) == true
                && Variable0 == -1)
                //Variable0 = 0;

			//Print("Variable0: " + Variable0);
			Variable0 = Variable0;
			
			if (Position.MarketPosition == MarketPosition.Flat) {
								
				// --------------- Set initial crossover event --------------------
				// in lower MAEnv and came from upper, initial crossover from upper
				if (High[0] < MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Lower[0]
					&& Variable0 >= 0) {
					Variable0 = -1;
					Print("Variable0 set to -1");
				}
				// in upper MAEnv and came from lower, initial crossover from lower
				if (Low[0] > MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Upper[0]
					&& Variable0 <= 0) {
					Variable0 = 1;
					Print("Variable0 set to 1");
				}
					
				// --------------- Set pullback depth --------------- 
				if (Variable0 == -1) {
					if (CrossAbove(Low, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Lower, 1)) 
						// pulled back within channel
						Variable0 = -2;
				}
				if (Variable0 == -2) {
					if (CrossAbove(Low, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle, 1)) 
						// pulled back to middle of the channel
						Variable0 = -3;
				}
				if (Variable0 == 1) {
					if (CrossBelow(High, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Upper, 1)) 
						// pulled back within channel
						Variable0 = 2;
				}
				if (Variable0 == 2) {
					if (CrossBelow(High, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle, 1)) 
						// pulled back to middle of the channel
						Variable0 = 3;
				}
			}
			
/*			
            // Condition set 3
			// set potential long flag since the price has crossed below the moving average
            if (Falling(HMA(HullPeriod)) == true
                && CrossBelow(Low, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle, 1))
            {
                Variable0 = 1;
            }

			// set potential short flag since the price has crossed below the moving average
            if (Rising(HMA(HullPeriod)) == true
                && CrossAbove(High, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle, 1))
            {
                Variable0 = -1;
            }
*/
			if (Variable0 < 0) {
				DrawDot("PotentialLong" + CurrentBar, true, 0, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle[0], Color.Green);
			} else if (Variable0 > 0) {
				DrawDot("PotentialLong" + CurrentBar, true, 0, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle[0], Color.Red);
			} else {
				DrawDot("PotentialLong" + CurrentBar, true, 0, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle[0], Color.Yellow);
			}
			
            // Condition set 4
			// enter a long position if the long flag is set, hull is positive
            if (Position.MarketPosition == MarketPosition.Flat
                && Rising(HMA(HullPeriod)) == true
                && Variable0 > 1
				&& CrossAbove(Low, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Upper, 1)
				//&& Low[0] > ATRTrailing(aTRRange, aTRPeriods, rPeriod).Upper[0]
				&& ToTime(Time[0]) > ToTime(8, 0, 0)
				//&& ToTime(Time[0]) > ToTime(1, 0, 0)
				//&& longStops
                //&& Rising(MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle) == true
				)
            {
                EnterLong(DefaultQuantity, "");
                Variable0 = 1;
            }

			// enter a short position if the short flag is set, hull is falling
            if (Position.MarketPosition == MarketPosition.Flat
                && Falling(HMA(HullPeriod)) == true
                && Variable0 < -1
				&& CrossBelow(High, MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Lower, 1)
				//&& Low[0] > ATRTrailing(aTRRange, aTRPeriods, rPeriod).Lower[0]
				&& ToTime(Time[0]) > ToTime(8, 0, 0)
				//&& ToTime(Time[0]) > ToTime(1, 0, 0)
                //&& Falling(MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle) == true
				)
            {
                EnterShort(DefaultQuantity, "");
                Variable0 = -1;
            }

            // Condition set 5
			// Exit condition
            if (Position.MarketPosition == MarketPosition.Long 
				&& BarsSinceEntry() > 4
                && Falling(HMA(HullPeriod)) == true)
            {
				DrawDot("Long Exit" + CurrentBar, true, 0, Low[0], Color.Red);
				// Condition set 5a
            	if (Low[0] < ATRTrailing(aTRRange, aTRPeriods, rPeriod).Upper[0])
            	{
    	            ExitLong("ATR Exit5a", "");
	                Variable0 = 0;
	            }
				if (Close[0] < MAEnvelopes(MAEnvPOffSet, 3, MAEnvPeriod).Middle[0])
				{
    	            ExitLong("ATR Exit5b", "");
	                Variable0 = 0;
				}
            }
			
            if (Position.MarketPosition == MarketPosition.Short 
				&& BarsSinceEntry() > 4
                && Rising(HMA(HullPeriod)) == true)
            {
				// Condition set 5a
            	if (High[0] > ATRTrailing(aTRRange, aTRPeriods, rPeriod).Lower[0])
            	{
    	            ExitShort("ATR Exit5c", "");
	                Variable0 = 0;
	            }
            }
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public double ATRRange
        {
            get { return aTRRange; }
            set { aTRRange = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ATRPeriods
        {
            get { return aTRPeriods; }
            set { aTRPeriods = Math.Max(1, value); }
        }

        [Description("ATR Rachet Periods")]
        [GridCategory("Parameters")]
        public double RPeriod
        {
            get { return rPeriod; }
            set { rPeriod = Math.Max(0.000, value); }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MAEnvPeriod
        {
            get { return mAEnvPeriod; }
            set { mAEnvPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double MAEnvPOffSet
        {
            get { return mAEnvPOffSet; }
            set { mAEnvPOffSet = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(1, value); }
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
        public int Stop
        {
            get { return stop; }
            set { stop = Math.Max(1, value); }
        }
        #endregion
    }
}
