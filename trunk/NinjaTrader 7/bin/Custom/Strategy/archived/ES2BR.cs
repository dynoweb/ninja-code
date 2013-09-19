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
    /// This strategy opens a position after two bars move in the same direction 1 tick more.
	/// 
	/// Didn't really see any future in this strategy
    /// </summary>
    [Description("This strategy opens a position after two bars move in the same direction 1 tick more.")]
    public class ES2BR : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int profitTarget = 4; // Default setting for ProfitTarget
        private int trailingStop = 5; // Default setting for TrailingStop
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            SetProfitTarget("", CalculationMode.Ticks, profitTarget);
            SetTrailStop("", CalculationMode.Ticks, trailingStop, false);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Position.MarketPosition == MarketPosition.Flat 
            	&& Close[1] > Open[1]
                && Close[0] > Open[0])
            {
                EnterLongStop(DefaultQuantity, Close[0] + 1 * TickSize, "");
            }
        }

        #region Properties
        [Description("Profit Target")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(1, value); }
        }

        [Description("Trailing Stop")]
        [GridCategory("Parameters")]
        public int TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(3, value); }
        }
        #endregion
    }
}
