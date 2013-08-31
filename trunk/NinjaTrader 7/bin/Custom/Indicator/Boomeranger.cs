#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Indicator;

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart
	/// Written by Rick Cromer 2013
    /// </summary>
    [Description("Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart")]
    public class Boomeranger : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int lookBackPeriod = 6; // Default setting for LookBackPeriod
			private int threshold = 60;
		
        // User defined variables (add any user defined variables below)
			private int signalLinePeriod = 10; // Default setting for SignalLinePeriod
			int priceTriggerPeriod = 5;		// entry price trigger
			int slowSignalPeriod = 14;	// slow signal
			//int HmaPeriod1 = 10;
			int HmaPeriod2 = 50;
		
			// used for long term trend, when signal crosses look to open
			// long band is when signal is above red slowSignalPeriod - slow signal
			int trendBand = 120;	
		
			int trend = 0;	// -1 - short, 0 - neutral, 1 - long		
		
			IDataSeries signal;
			HMARick dynamicTrend;
			IDataSeries priceTrigger;
			IDataSeries slowSignal;
		
        #endregion

		protected override void OnStartUp()
		{
			signal = HMA(signalLinePeriod);
			dynamicTrend = HMARick(trendBand, Threshold);
			priceTrigger = SMARick(priceTriggerPeriod);
			slowSignal = SMA(slowSignalPeriod);
			
			dynamicTrend.DownColor = Color.Purple;
			dynamicTrend.UpColor = Color.DarkCyan;
			dynamicTrend.PaintPriceMarkers = false;

//			slowSignal.Plots[0].Pen.Color = Color.Red;
//			slowSignal.PaintPriceMarkers = false;
//			slowSignal.CalculateOnBarClose = false;		
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			/*
			Add(PitColor(Color.Black, 83000, 25, 161500));
			Add(TickCounter(true, false));
			
			Add(SMA(slowSignalPeriod));
			SMA(slowSignalPeriod).Plots[0].Pen.Color = Color.Red;
			SMA(slowSignalPeriod).PaintPriceMarkers = false;
			
			// TMA(40) may be an interesting alternative
			// DWMA(70) is also interesting
			Add(HMARick(trendBand, threshold));

			Add(SMARick(priceTriggerPeriod));			
			SMARick(priceTriggerPeriod).DownColor = Color.Firebrick;
			SMARick(priceTriggerPeriod).UpColor = Color.RoyalBlue;
			SMARick(priceTriggerPeriod).Dash0Style = DashStyle.Dash;
			SMARick(priceTriggerPeriod).PaintPriceMarkers = false;
			
			Add(HMA(signalLinePeriod));
			HMA(signalLinePeriod).Plots[0].Pen.Color = Color.DimGray;
			HMA(signalLinePeriod).Plots[0].Pen.DashStyle = DashStyle.Solid;
			HMA(signalLinePeriod).Plots[0].Pen.Width = 2;
			HMA(signalLinePeriod).PaintPriceMarkers = false;
			HMA(signalLinePeriod).CalculateOnBarClose = true;
			

			HallMaColored(HmaPeriod2).DownColor = Color.HotPink;
			HallMaColored(HmaPeriod2).UpColor = Color.MediumAquamarine;
			HallMaColored(HmaPeriod2).Dash0Style = DashStyle.DashDot;
			HallMaColored(HmaPeriod2).Plot0Width = 2;
			HallMaColored(HmaPeriod2).PaintPriceMarkers = false;
			*/
			Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			
			if (CrossAbove(signal, trend, lookBackPeriod))
			{
				//DrawArrowUp(CurrentBar + "Up", 0, slowSignal[0] - 2 * TickSize, Color.Green);
				DrawDot(CurrentBar + "dot", false, 0, priceTrigger[0], Color.Blue);
			}
			
			if (CrossAbove(trend, signal, lookBackPeriod))
			{
				DrawDot(CurrentBar + "dot", false, 0, priceTrigger[0], Color.Yellow);
			}
			
			if (CrossAbove(signal, slowSignal, 1))
			{
				DrawArrowUp(CurrentBar + "Up", 0, slowSignal[0] - 2 * TickSize, Color.Green);
				//BackColorAll = Color.Cyan;
				//DrawSquare(CurrentBar + "dot3", false, 0, slowSignal[0], Color.Black);
			}
			if (CrossAbove(slowSignal, signal, 1))
			{
				DrawArrowDown(CurrentBar + "Down", 0, slowSignal[0] + 2 * TickSize, Color.Red);
				//BackColorAll = Color.Fuchsia;
				//DrawSquare(CurrentBar + "dot4", false, 0, slowSignal[0], Color.Black);
			}
			
        }

        #region Properties

		/// </summary>
		[Description("Slope as percentage of average true range")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Neutral Threshold")]
		public int Threshold 
		{
			get { return threshold; }
			set { threshold = Math.Max(0, value); }
		}

        [Description("Bars to look back to signal crossing trend")]
        [GridCategory("Parameters")]
        public int LookBackPeriod
        {
            get { return lookBackPeriod; }
            set { lookBackPeriod = Math.Max(1, value); }
        }
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private Boomeranger[] cacheBoomeranger = null;

        private static Boomeranger checkBoomeranger = new Boomeranger();

        /// <summary>
        /// Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart
        /// </summary>
        /// <returns></returns>
        public Boomeranger Boomeranger(int lookBackPeriod, int threshold)
        {
            return Boomeranger(Input, lookBackPeriod, threshold);
        }

        /// <summary>
        /// Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart
        /// </summary>
        /// <returns></returns>
        public Boomeranger Boomeranger(Data.IDataSeries input, int lookBackPeriod, int threshold)
        {
            if (cacheBoomeranger != null)
                for (int idx = 0; idx < cacheBoomeranger.Length; idx++)
                    if (cacheBoomeranger[idx].LookBackPeriod == lookBackPeriod && cacheBoomeranger[idx].Threshold == threshold && cacheBoomeranger[idx].EqualsInput(input))
                        return cacheBoomeranger[idx];

            lock (checkBoomeranger)
            {
                checkBoomeranger.LookBackPeriod = lookBackPeriod;
                lookBackPeriod = checkBoomeranger.LookBackPeriod;
                checkBoomeranger.Threshold = threshold;
                threshold = checkBoomeranger.Threshold;

                if (cacheBoomeranger != null)
                    for (int idx = 0; idx < cacheBoomeranger.Length; idx++)
                        if (cacheBoomeranger[idx].LookBackPeriod == lookBackPeriod && cacheBoomeranger[idx].Threshold == threshold && cacheBoomeranger[idx].EqualsInput(input))
                            return cacheBoomeranger[idx];

                Boomeranger indicator = new Boomeranger();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LookBackPeriod = lookBackPeriod;
                indicator.Threshold = threshold;
                Indicators.Add(indicator);
                indicator.SetUp();

                Boomeranger[] tmp = new Boomeranger[cacheBoomeranger == null ? 1 : cacheBoomeranger.Length + 1];
                if (cacheBoomeranger != null)
                    cacheBoomeranger.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBoomeranger = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Boomeranger Boomeranger(int lookBackPeriod, int threshold)
        {
            return _indicator.Boomeranger(Input, lookBackPeriod, threshold);
        }

        /// <summary>
        /// Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart
        /// </summary>
        /// <returns></returns>
        public Indicator.Boomeranger Boomeranger(Data.IDataSeries input, int lookBackPeriod, int threshold)
        {
            return _indicator.Boomeranger(input, lookBackPeriod, threshold);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Boomeranger Boomeranger(int lookBackPeriod, int threshold)
        {
            return _indicator.Boomeranger(Input, lookBackPeriod, threshold);
        }

        /// <summary>
        /// Uses concepts from BoomerangeTrading.com to indicate how to trade NQ using 450 T chart
        /// </summary>
        /// <returns></returns>
        public Indicator.Boomeranger Boomeranger(Data.IDataSeries input, int lookBackPeriod, int threshold)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Boomeranger(input, lookBackPeriod, threshold);
        }
    }
}
#endregion
