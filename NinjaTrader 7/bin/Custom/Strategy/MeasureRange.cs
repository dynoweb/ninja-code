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
    /// 
    /// </summary>
    [Description("Sends bar range to output window")]
    public class MeasureRange : Strategy
    {
        #region Variables
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();
			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double points = Instrument.MasterInstrument.Round2TickSize(High[0] - Low[0]);
			double dollarRange = Instrument.MasterInstrument.PointValue * points;
			Print(Time + ", " + Open[0] + ", " + Low[0] + ", " + High[0] + ", " + Close[0] 
				+ ", " + points + ", * " + Instrument.MasterInstrument.PointValue
			 	+ ", $" + dollarRange);
        }

		
        #region Properties
        #endregion
    }
}
