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
    /// ES 5m Experimenting with indicators to detect chop
    /// </summary>
    [Description("ES 5m Experimenting with indicators to detect chop")]
    public class ESChopDetector : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
		MACD macd;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(FisherTransform(5));
			Add(MACD(12, 26,9));
			
			SetStopLoss(CalculationMode.Ticks, 4);
			//SetTrailStop(CalculationMode.Ticks, 5);
			SetProfitTarget(CalculationMode.Ticks, 4);			
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			macd = MACD(12, 26,9);
			if (Math.Abs(macd[0]) < 0.3)
				BarColor = Color.Gray;
			else
			{
				if (Position.MarketPosition == MarketPosition.Flat)
				{
					if (macd[0] < 0 && FisherTransform(5)[0] < -0.3)
						EnterShort();
						//EnterLong();
					if (macd[0] > 0 && FisherTransform(5)[0] > 0.3)
						//EnterLongLimit(Close[0]-1*TickSize);
						//EnterShort();
						EnterLong();
				}
			}
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
        }
        #endregion
    }
}
