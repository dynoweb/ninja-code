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
    public class FiveBarSampleStrategy : Strategy
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//Add(FiveBarSample());
			Add(FiveBarPattern());
			
						
			// Store all series values instead of only the last 256 values
			MaximumBarsLookBack = MaximumBarsLookBack.Infinite;
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
//			if (FiveBarPattern().FiveBarLower.ContainsValue(0))
//				Print(Time + " 0 - CurrentBar: " + CurrentBar + " FiveBarLower: " + FiveBarPattern().FiveBarLower[0]);
			
			// For me to get Plot1[2], I have to leave the following Plot1[0] code uncommented out
			// Doesn't work when these next two lines are commented out
//			if (FiveBarPattern().Plot1.ContainsValue(0))
//				Print(Time + " 0 - CurrentBar: " + CurrentBar + " FiveBarLower: " + FiveBarPattern().Plot1[0]);
			
			
			if (FiveBarPattern().Plot1.ContainsValue(2))
				Print(Time + " 2 - CurrentBar: " + CurrentBar + " FiveBarLower: " + FiveBarPattern().Plot1[2]);
        }

        #region Properties
        #endregion
    }
}
