// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Sample demonstrating how to use the BoolSeries object
    /// </summary>
    [Description("Sample demonstrating how to use the BoolSeries object")]
    public class SampleBoolSeries : Indicator
    {
        #region Variables
		/* We declare two BoolSeries objects here. We will expose these objects with the use of 
		public properties later. When we want to expose an object we should always use the associated
		IDataSeries class type with it. This will ensure that the value accessed is kept up-to-date. */
		private BoolSeries bearIndication;
		private BoolSeries bullIndication;
		
		/* If you happen to have an object that does not have an IDataSeries class that can be used, you
		will need to manually ensure its values are kept up-to-date. This process will be done in the
		"Property" region of the code. */
		private double exposedVariable;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			/* "this" syncs the BoolSeries to the historical bar object of the indicator. It will generate
			one bool value for every price bar. */
			bearIndication			= new BoolSeries(this);
			bullIndication			= new BoolSeries(this);
			
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// MACD Crossover: Fast Line cross above Slow Line
			if (CrossAbove(MACD(12, 26, 9), MACD(12, 26, 9).Avg, 1))
			{
				// Paint the current price bar lime to draw our attention to it
				BarColor = Color.Lime;
				
				/* This crossover condition is considered bullish so we set the "bullIndication" BoolSeries object to true.
				We also set the "bearIndication" object to false so it does not take on a null value. */
				bullIndication.Set(true);
				bearIndication.Set(false);
			}
			
			// MACD Crossover: Fast Line cross below Slow Line
			else if (CrossBelow(MACD(12, 26, 9), MACD(12, 26, 9).Avg, 1))
			{
				// Paint the current price bar magenta to draw our attention to it
				BarColor = Color.Magenta;
				
				/* This crossover condition is considered bearish so we set the "bearIndication" BoolSeries object to true.
				We also set the "bullIndication" object to false so it does not take on a null value. */
				bullIndication.Set(false);
				bearIndication.Set(true);
			}
			
			// MACD Crossover: No cross
			else
			{
				/* Since no crosses occured we are not receiving any bullish or bearish signals so we
				set our BoolSeries objects both to false. */
				bullIndication.Set(false);
				bearIndication.Set(false);
			}
			
			// We set our variable to the close value.
			exposedVariable = Close[0];
        }

		// Important code segment in the Properties section. Please expand to view.
        #region Properties
		
		// Creating public properties that access our internal BoolSeries allows external access to this indicator's BoolSeries
		
		[Browsable(false)]
        [XmlIgnore()]
        public BoolSeries BearIndication
        {
            get { return bearIndication; }	// Allows our public BearIndication BoolSeries to access and expose our interal bearIndication BoolSeries
        }
		
		[Browsable(false)]
        [XmlIgnore()]
        public BoolSeries BullIndication
        {
            get { return bullIndication; }	// Allows our public BullIndication BoolSeries to access and expose our interal bullIndication BoolSeries
        }
		
		[Browsable(false)]
        [XmlIgnore()]
        public double ExposedVariable
        {
			// We need to call the Update() method to ensure our exposed variable is in up-to-date.
            get { Update(); return exposedVariable; }
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
        private SampleBoolSeries[] cacheSampleBoolSeries = null;

        private static SampleBoolSeries checkSampleBoolSeries = new SampleBoolSeries();

        /// <summary>
        /// Sample demonstrating how to use the BoolSeries object
        /// </summary>
        /// <returns></returns>
        public SampleBoolSeries SampleBoolSeries()
        {
            return SampleBoolSeries(Input);
        }

        /// <summary>
        /// Sample demonstrating how to use the BoolSeries object
        /// </summary>
        /// <returns></returns>
        public SampleBoolSeries SampleBoolSeries(Data.IDataSeries input)
        {
            if (cacheSampleBoolSeries != null)
                for (int idx = 0; idx < cacheSampleBoolSeries.Length; idx++)
                    if (cacheSampleBoolSeries[idx].EqualsInput(input))
                        return cacheSampleBoolSeries[idx];

            lock (checkSampleBoolSeries)
            {
                if (cacheSampleBoolSeries != null)
                    for (int idx = 0; idx < cacheSampleBoolSeries.Length; idx++)
                        if (cacheSampleBoolSeries[idx].EqualsInput(input))
                            return cacheSampleBoolSeries[idx];

                SampleBoolSeries indicator = new SampleBoolSeries();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                SampleBoolSeries[] tmp = new SampleBoolSeries[cacheSampleBoolSeries == null ? 1 : cacheSampleBoolSeries.Length + 1];
                if (cacheSampleBoolSeries != null)
                    cacheSampleBoolSeries.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSampleBoolSeries = tmp;
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
        /// Sample demonstrating how to use the BoolSeries object
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SampleBoolSeries SampleBoolSeries()
        {
            return _indicator.SampleBoolSeries(Input);
        }

        /// <summary>
        /// Sample demonstrating how to use the BoolSeries object
        /// </summary>
        /// <returns></returns>
        public Indicator.SampleBoolSeries SampleBoolSeries(Data.IDataSeries input)
        {
            return _indicator.SampleBoolSeries(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Sample demonstrating how to use the BoolSeries object
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SampleBoolSeries SampleBoolSeries()
        {
            return _indicator.SampleBoolSeries(Input);
        }

        /// <summary>
        /// Sample demonstrating how to use the BoolSeries object
        /// </summary>
        /// <returns></returns>
        public Indicator.SampleBoolSeries SampleBoolSeries(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SampleBoolSeries(input);
        }
    }
}
#endregion
