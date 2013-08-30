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
    /// Empty placeholder indicator for creating plots in NinjaScript Strategies
    /// </summary>
    [Description("Empty placeholder indicator for creating plots in NinjaScript Strategies")]
    public class TCStrategyPlotMulti : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int id = 1; // Default setting for Id
			private bool overlayMode = true;
			private int numberOfPlots = 1;
		
        // User defined variables (add any user defined variables below)
		bool smartPriceFormatting = true;
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            for (int a = 1; a<= numberOfPlots; a++)
				Add(new Plot(Color.DarkOrange, PlotStyle.Line, "Strategy Plot "+a.ToString()));
			
            //CalculateOnBarClose	= true;
            Overlay				= OverlayMode;
            PriceTypeSupported	= false;
			
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// This is an empty indicator that just acts as a placeholder for creating a plot.
		}
		
		public override string FormatPriceMarker(double price)
		{
			if (id >= 0)
				return Instrument.MasterInstrument.FormatPrice(price);
			// example : Formats price values to 4 decimal places
			else 
				return price.ToString("N4");
		}


        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot1
        {
            get { return Values[Math.Min(1,Values.Length-1)]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot2
        {
            get { return Values[Math.Min(2,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot3
        {
            get { return Values[Math.Min(3,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot4
        {
            get { return Values[Math.Min(4,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot5
        {
            get { return Values[Math.Min(5,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot6
        {
            get { return Values[Math.Min(6,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot7
        {
            get { return Values[Math.Min(7,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot8
        {
            get { return Values[Math.Min(8,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot9
        {
            get { return Values[Math.Min(9,Values.Length-1)]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot10
        {
            get { return Values[Math.Min(10,Values.Length-1)]; }
        }

		// _______________________________________
      
		// _______________________________________
		[Description("")]		
        [Category("Parameters")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
		
		
        [Description("")]
        [Category("Parameters")]
        public int NumberOfPlots
        {
            get { return numberOfPlots; }
            set { numberOfPlots = Math.Max(1, Math.Min(11,value)); }
        }
		
		[Description("")]
        [Category("Parameters")]
        public bool OverlayMode
        {
            get { return overlayMode; }
            set { overlayMode = value; }
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
        private TCStrategyPlotMulti[] cacheTCStrategyPlotMulti = null;

        private static TCStrategyPlotMulti checkTCStrategyPlotMulti = new TCStrategyPlotMulti();

        /// <summary>
        /// Empty placeholder indicator for creating plots in NinjaScript Strategies
        /// </summary>
        /// <returns></returns>
        public TCStrategyPlotMulti TCStrategyPlotMulti(int id, int numberOfPlots, bool overlayMode)
        {
            return TCStrategyPlotMulti(Input, id, numberOfPlots, overlayMode);
        }

        /// <summary>
        /// Empty placeholder indicator for creating plots in NinjaScript Strategies
        /// </summary>
        /// <returns></returns>
        public TCStrategyPlotMulti TCStrategyPlotMulti(Data.IDataSeries input, int id, int numberOfPlots, bool overlayMode)
        {
            if (cacheTCStrategyPlotMulti != null)
                for (int idx = 0; idx < cacheTCStrategyPlotMulti.Length; idx++)
                    if (cacheTCStrategyPlotMulti[idx].Id == id && cacheTCStrategyPlotMulti[idx].NumberOfPlots == numberOfPlots && cacheTCStrategyPlotMulti[idx].OverlayMode == overlayMode && cacheTCStrategyPlotMulti[idx].EqualsInput(input))
                        return cacheTCStrategyPlotMulti[idx];

            lock (checkTCStrategyPlotMulti)
            {
                checkTCStrategyPlotMulti.Id = id;
                id = checkTCStrategyPlotMulti.Id;
                checkTCStrategyPlotMulti.NumberOfPlots = numberOfPlots;
                numberOfPlots = checkTCStrategyPlotMulti.NumberOfPlots;
                checkTCStrategyPlotMulti.OverlayMode = overlayMode;
                overlayMode = checkTCStrategyPlotMulti.OverlayMode;

                if (cacheTCStrategyPlotMulti != null)
                    for (int idx = 0; idx < cacheTCStrategyPlotMulti.Length; idx++)
                        if (cacheTCStrategyPlotMulti[idx].Id == id && cacheTCStrategyPlotMulti[idx].NumberOfPlots == numberOfPlots && cacheTCStrategyPlotMulti[idx].OverlayMode == overlayMode && cacheTCStrategyPlotMulti[idx].EqualsInput(input))
                            return cacheTCStrategyPlotMulti[idx];

                TCStrategyPlotMulti indicator = new TCStrategyPlotMulti();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Id = id;
                indicator.NumberOfPlots = numberOfPlots;
                indicator.OverlayMode = overlayMode;
                Indicators.Add(indicator);
                indicator.SetUp();

                TCStrategyPlotMulti[] tmp = new TCStrategyPlotMulti[cacheTCStrategyPlotMulti == null ? 1 : cacheTCStrategyPlotMulti.Length + 1];
                if (cacheTCStrategyPlotMulti != null)
                    cacheTCStrategyPlotMulti.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTCStrategyPlotMulti = tmp;
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
        /// Empty placeholder indicator for creating plots in NinjaScript Strategies
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TCStrategyPlotMulti TCStrategyPlotMulti(int id, int numberOfPlots, bool overlayMode)
        {
            return _indicator.TCStrategyPlotMulti(Input, id, numberOfPlots, overlayMode);
        }

        /// <summary>
        /// Empty placeholder indicator for creating plots in NinjaScript Strategies
        /// </summary>
        /// <returns></returns>
        public Indicator.TCStrategyPlotMulti TCStrategyPlotMulti(Data.IDataSeries input, int id, int numberOfPlots, bool overlayMode)
        {
            return _indicator.TCStrategyPlotMulti(input, id, numberOfPlots, overlayMode);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Empty placeholder indicator for creating plots in NinjaScript Strategies
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TCStrategyPlotMulti TCStrategyPlotMulti(int id, int numberOfPlots, bool overlayMode)
        {
            return _indicator.TCStrategyPlotMulti(Input, id, numberOfPlots, overlayMode);
        }

        /// <summary>
        /// Empty placeholder indicator for creating plots in NinjaScript Strategies
        /// </summary>
        /// <returns></returns>
        public Indicator.TCStrategyPlotMulti TCStrategyPlotMulti(Data.IDataSeries input, int id, int numberOfPlots, bool overlayMode)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TCStrategyPlotMulti(input, id, numberOfPlots, overlayMode);
        }
    }
}
#endregion
