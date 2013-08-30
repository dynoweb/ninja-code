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
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Donchian Channel. The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
	/// </summary>
	[Description("The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.")]
    public class DonchianRick : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 20; // Default setting for Period
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.LawnGreen), 2), PlotStyle.Line, "Mean"));
            Add(new Plot(Color.FromKnownColor(KnownColor.White), PlotStyle.Line, "Upper"));
            Add(new Plot(Color.FromKnownColor(KnownColor.White), PlotStyle.Line, "Lower"));
			Displacement = 2;

            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			Value.Set((MAX(High, Period)[0] + MIN(Low, Period)[0]) / 2);
			Upper.Set(MAX(High, Period)[0]);
			Lower.Set(MIN(Low, Period)[0]);
		}
		
		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[2]; }
		}
			
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Mean
		{
			get { return Values[0]; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[1]; }
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
        private DonchianRick[] cacheDonchianRick = null;

        private static DonchianRick checkDonchianRick = new DonchianRick();

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public DonchianRick DonchianRick(int period)
        {
            return DonchianRick(Input, period);
        }

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public DonchianRick DonchianRick(Data.IDataSeries input, int period)
        {
            if (cacheDonchianRick != null)
                for (int idx = 0; idx < cacheDonchianRick.Length; idx++)
                    if (cacheDonchianRick[idx].Period == period && cacheDonchianRick[idx].EqualsInput(input))
                        return cacheDonchianRick[idx];

            lock (checkDonchianRick)
            {
                checkDonchianRick.Period = period;
                period = checkDonchianRick.Period;

                if (cacheDonchianRick != null)
                    for (int idx = 0; idx < cacheDonchianRick.Length; idx++)
                        if (cacheDonchianRick[idx].Period == period && cacheDonchianRick[idx].EqualsInput(input))
                            return cacheDonchianRick[idx];

                DonchianRick indicator = new DonchianRick();
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

                DonchianRick[] tmp = new DonchianRick[cacheDonchianRick == null ? 1 : cacheDonchianRick.Length + 1];
                if (cacheDonchianRick != null)
                    cacheDonchianRick.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDonchianRick = tmp;
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
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DonchianRick DonchianRick(int period)
        {
            return _indicator.DonchianRick(Input, period);
        }

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public Indicator.DonchianRick DonchianRick(Data.IDataSeries input, int period)
        {
            return _indicator.DonchianRick(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DonchianRick DonchianRick(int period)
        {
            return _indicator.DonchianRick(Input, period);
        }

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public Indicator.DonchianRick DonchianRick(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DonchianRick(input, period);
        }
    }
}
#endregion
