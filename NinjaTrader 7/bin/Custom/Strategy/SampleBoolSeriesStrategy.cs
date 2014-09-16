// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

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
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Sample strategy demonstrating how to call an exposed BoolSeries object
    /// </summary>
    [Description("Sample strategy demonstrating how to call an exposed BoolSeries object")]
    public class SampleBoolSeriesStrategy : Strategy
    {
        #region Variables
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			// Add our indicators to the strategy for charting purposes
			Add(MACD(12, 26, 9));
			Add(SampleBoolSeries());
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			/* When our indicator gives us a bull signal we enter long. Notice that we are accessing the
			public BoolSeries we made in the indicator. */
			if (SampleBoolSeries().BullIndication[0])
				EnterLong();
			
			// When our indicator gives us a bear signal we enter short
			if (SampleBoolSeries().BearIndication[0])
				EnterShort();
			
			/* NOTE: This strategy is based on reversals thus there are no explicit exit orders. When you
			are long you will be closed and reversed into a short when the bear signal is received. The vice
			versa is true if you are short. */
			
			/* Print our exposed variable. Because we manually kept it up-to-date it will print values that
			match the bars object. */
			Print(SampleBoolSeries().ExposedVariable);
        }

        #region Properties
        #endregion
    }
}
