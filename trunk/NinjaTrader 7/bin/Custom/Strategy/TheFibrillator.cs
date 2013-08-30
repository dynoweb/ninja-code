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
    /// Designed for CL 4 Range Bars. This shows the fibinocci swing trading within a range.
    /// </summary>
    [Description("Designed for CL 4 Range Bars. This shows the fibinocci swing trading within a range.")]
    public class TheFibrillator : Strategy
    {
        #region Variables
			
			// ZigZag Settings
			DeviationType deviationType = DeviationType.Points;
			double deviationValue = 0.5;
			bool useHighLow = true;
			
			int hmaPeriod = 26;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(ZigZag(deviationType, deviationValue, useHighLow));			
			Add(ZigZagFib(deviationType, deviationValue, useHighLow));			
			Add(HMA(hmaPeriod));
			ZigZag(deviationType, deviationValue, useHighLow).Plots[0].Pen.Color = Color.White;

			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
        }
		
        #region Properties

        [Description("ZigZag deviation setting")]
        [GridCategory("Parameters")]
        public double DeviationValue
        {
            get { return deviationValue; }
            set { deviationValue = Math.Max(0.0001, value); }
        }
		
        [Description("Hull Moving Average Period")]
        [GridCategory("Parameters")]
        public int HmaPeriod
        {
            get { return hmaPeriod; }
            set { hmaPeriod = Math.Max(5, value); }
        }

		#endregion
    }
}
