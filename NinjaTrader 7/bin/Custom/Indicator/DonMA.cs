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
    /// Sample MA indicator that allows you to select different MA types.
	/// Khoi(Cory) Nguyen 12/12/2009
    /// </summary>
    [Description("Donchian MA indicator with color.")]
    public class DonMA: Indicator
    {
        #region Variables
		// Create a variable that stores the user's selection for a moving average 

		private int				period	= 14;
		private DataSeries MAHolder;
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			// Adds a plot for the MA values to be stored in
			Add(new Plot(Color.Blue, "MARising"));
			Add(new Plot(Color.Red, "MAFalling"));
			
			MAHolder = new DataSeries(this);
			
            CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if ( CurrentBar < 2 )
				return;
			
			MAHolder.Set((MAX(High, Period)[0] + MIN(Low, Period)[0]) / 2);
			if ( MAHolder[1] < MAHolder[0])
			{
				MARising[1] = MAHolder[1];
				MARising.Set(MAHolder[0]);
			}
			else
			{
				MAFalling[1] = MAHolder[1];
				MAFalling.Set(MAHolder[0]);
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MARising
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MAFalling
        {
            get { return Values[1]; }
        }
		
		
		[Description("Numbers of bars to find Min and Max for calculations")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
        private DonMA[] cacheDonMA = null;

        private static DonMA checkDonMA = new DonMA();

        /// <summary>
        /// Donchian MA indicator with color.
        /// </summary>
        /// <returns></returns>
        public DonMA DonMA(int period)
        {
            return DonMA(Input, period);
        }

        /// <summary>
        /// Donchian MA indicator with color.
        /// </summary>
        /// <returns></returns>
        public DonMA DonMA(Data.IDataSeries input, int period)
        {
            if (cacheDonMA != null)
                for (int idx = 0; idx < cacheDonMA.Length; idx++)
                    if (cacheDonMA[idx].Period == period && cacheDonMA[idx].EqualsInput(input))
                        return cacheDonMA[idx];

            lock (checkDonMA)
            {
                checkDonMA.Period = period;
                period = checkDonMA.Period;

                if (cacheDonMA != null)
                    for (int idx = 0; idx < cacheDonMA.Length; idx++)
                        if (cacheDonMA[idx].Period == period && cacheDonMA[idx].EqualsInput(input))
                            return cacheDonMA[idx];

                DonMA indicator = new DonMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                DonMA[] tmp = new DonMA[cacheDonMA == null ? 1 : cacheDonMA.Length + 1];
                if (cacheDonMA != null)
                    cacheDonMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDonMA = tmp;
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
        /// Donchian MA indicator with color.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DonMA DonMA(int period)
        {
            return _indicator.DonMA(Input, period);
        }

        /// <summary>
        /// Donchian MA indicator with color.
        /// </summary>
        /// <returns></returns>
        public Indicator.DonMA DonMA(Data.IDataSeries input, int period)
        {
            return _indicator.DonMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Donchian MA indicator with color.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DonMA DonMA(int period)
        {
            return _indicator.DonMA(Input, period);
        }

        /// <summary>
        /// Donchian MA indicator with color.
        /// </summary>
        /// <returns></returns>
        public Indicator.DonMA DonMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DonMA(input, period);
        }
    }
}
#endregion
