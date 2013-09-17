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
    /// Indicator  to show the difference between bid and ask trades on each bar
    /// </summary>
    [Description("Indicator  to show the difference between bid and ask trades on each bar")]
    public class VolumeBidAsk : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		private int bidAskVolume = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Bar, "BidVolume"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Bar, "AskVolume"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DimGray), PlotStyle.Line, "OscLine"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "ZeroLine"));
            Overlay				= false;
			CalculateOnBarClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
		    // Only process entry signals on a bar by bar basis (not tick by tick)
		    if (FirstTickOfBar)
			{
				bidAskVolume = 0;
			}

			if (Close[0] >= GetCurrentAsk())
				bidAskVolume += 1;
			else //if (Close[0] <= GetCurrentBid())
				bidAskVolume -= 1;
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			if (bidAskVolume > 0)
				AskVolume.Set(bidAskVolume);
			else 
            	BidVolume.Set(bidAskVolume);
			
            OscLine.Set(bidAskVolume);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BidVolume
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AskVolume
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries OscLine
        {
            get { return Values[2]; }
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
        private VolumeBidAsk[] cacheVolumeBidAsk = null;

        private static VolumeBidAsk checkVolumeBidAsk = new VolumeBidAsk();

        /// <summary>
        /// Indicator  to show the difference between bid and ask trades on each bar
        /// </summary>
        /// <returns></returns>
        public VolumeBidAsk VolumeBidAsk()
        {
            return VolumeBidAsk(Input);
        }

        /// <summary>
        /// Indicator  to show the difference between bid and ask trades on each bar
        /// </summary>
        /// <returns></returns>
        public VolumeBidAsk VolumeBidAsk(Data.IDataSeries input)
        {
            if (cacheVolumeBidAsk != null)
                for (int idx = 0; idx < cacheVolumeBidAsk.Length; idx++)
                    if (cacheVolumeBidAsk[idx].EqualsInput(input))
                        return cacheVolumeBidAsk[idx];

            lock (checkVolumeBidAsk)
            {
                if (cacheVolumeBidAsk != null)
                    for (int idx = 0; idx < cacheVolumeBidAsk.Length; idx++)
                        if (cacheVolumeBidAsk[idx].EqualsInput(input))
                            return cacheVolumeBidAsk[idx];

                VolumeBidAsk indicator = new VolumeBidAsk();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeBidAsk[] tmp = new VolumeBidAsk[cacheVolumeBidAsk == null ? 1 : cacheVolumeBidAsk.Length + 1];
                if (cacheVolumeBidAsk != null)
                    cacheVolumeBidAsk.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeBidAsk = tmp;
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
        /// Indicator  to show the difference between bid and ask trades on each bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeBidAsk VolumeBidAsk()
        {
            return _indicator.VolumeBidAsk(Input);
        }

        /// <summary>
        /// Indicator  to show the difference between bid and ask trades on each bar
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeBidAsk VolumeBidAsk(Data.IDataSeries input)
        {
            return _indicator.VolumeBidAsk(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Indicator  to show the difference between bid and ask trades on each bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeBidAsk VolumeBidAsk()
        {
            return _indicator.VolumeBidAsk(Input);
        }

        /// <summary>
        /// Indicator  to show the difference between bid and ask trades on each bar
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeBidAsk VolumeBidAsk(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeBidAsk(input);
        }
    }
}
#endregion
