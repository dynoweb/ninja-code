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
    /// Based off of daily closes of stock using MAs
    /// </summary>
    [Description("Based off of daily closes of stock using MAs")]
    public class VantagePoint : Strategy
    {
        #region Variables
        // Wizard generated variables
			private int daysInMarket = 15;
			private int emaPeriod = 5; // Default setting for EmaPeriod
			private double profitTarget = 0.3;
			private int smaPeriod = 10; // Default setting for SmaPeriod
			private double trailingStop = 0.05;
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
			EMA ema = null;
			SMARick sma = null;
			SMARick sma200 = null;
			//ABCDWave abcdWave = null;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			// These only display for the primary Bars object on the chart BarsArray[0]
			ema = EMA(EmaPeriod);
			Add(ema);
			ema.Plots[0].Pen.Color = Color.Black;
			ema.Plots[0].Pen.Width = 2;
			
			sma = SMARick(SmaPeriod);
            Add(sma);
			sma200 = SMARick(100);
            Add(sma200);
			
			//abcdWave = ncatABCDWave(true, true, true, 5000, "bextABCD", "bexI_ABCD", "bexIT_ABCD", "8", 20, true, false);
			
			SetTrailStop(CalculationMode.Percent, trailingStop);
			SetProfitTarget(CalculationMode.Percent, profitTarget);
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			if (CrossAbove(ema, sma, 1) && Rising(sma200))
				EnterLong("Long");
			
			if (CrossBelow(ema, sma, 1) && Falling(sma200))
				EnterShort("Short");
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				if (BarsSinceEntry() > daysInMarket && daysInMarket > 0)
					ExitLong(Position.Quantity, "DIM", "Long");
				
				if (Falling(ema) && Close[0] < ema[0] && Falling(sma) && Close[0] < sma[0])
					ExitLong(Position.Quantity, "Reversal", "Long");
				
				if (CrossBelow(ema, sma, 1))
					ExitLong(Position.Quantity, "Cross Below", "Long");
			}
			
			if (Position.MarketPosition == MarketPosition.Short)
			{
				if (BarsSinceEntry() > daysInMarket && daysInMarket > 0)
					ExitShort(Position.Quantity, "DIM", "Short");
				
				if (Rising(ema) && Close[0] > ema[0] && Rising(sma) && Close[0] > sma[0])
					ExitShort(Position.Quantity, "Reversal", "Short");
				
				if (CrossAbove(ema, sma, 1))
					ExitShort(Position.Quantity, "Cross Above", "Short");
			}
        }

        #region Properties

        [Description("Days in trade max")]
        [GridCategory("Parameters")]
        public int DaysInMarket
        {
            get { return daysInMarket; }
            set { daysInMarket = Math.Max(0, value); }
        }

        [Description("EMA Period")]
        [GridCategory("Parameters")]
        public int EmaPeriod
        {
            get { return emaPeriod; }
            set { emaPeriod = Math.Max(1, value); }
        }
		
        [Description("SMA Period")]
        [GridCategory("Parameters")]
        public int SmaPeriod
        {
            get { return smaPeriod; }
            set { smaPeriod = Math.Max(1, value); }
        }

        [Description("Profit Target Percent")]
        [GridCategory("Parameters")]
        public double ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0.01, value); }
        }

        [Description("Trailing Stop Percent")]
        [GridCategory("Parameters")]
        public double TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(0.01, value); }
        }

		#endregion
    }
}
