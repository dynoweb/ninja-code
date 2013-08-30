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
    /// Plots a horizontal line at current price, useful for market profile type indicators.
    /// </summary>
    [Description("Plots a horizontal line at current price, useful for market profile type indicators.")]
    public class DH_HorizPriceLine : Indicator
    {
        #region Variables
        double CurrentPrice;
		private Color MyColor = Color.Goldenrod;
		private DashStyle MyLine = DashStyle.Dash;
		private int lineThickness = 1;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
		CurrentPrice = Close[0];
			DrawHorizontalLine("CurrentPrice", false, CurrentPrice, MyColor, MyLine, lineThickness);
        }

        #region Properties
       
		[Browsable(false)]
		public string MyColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(MyColor); }
			set { MyColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
		[Description("Color of the Horizontal Price Line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Line Color")]
		public Color myColor
		{
			get { return MyColor; }
			set { MyColor = value; }
		}
		
		[Description("Style of the Horizontal Price Line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Line Style")]
		public DashStyle myLine
		{
			get { return MyLine; }
			set { MyLine = value; }
		}
		
		[Description("Line thickness.")]
		[Gui.Design.DisplayName ("Line thickness")]
		[Category("Line Parameters")]
		public int LineThickness
		{
			get { return lineThickness; }
			set { lineThickness = Math.Max(0, value); }
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
        private DH_HorizPriceLine[] cacheDH_HorizPriceLine = null;

        private static DH_HorizPriceLine checkDH_HorizPriceLine = new DH_HorizPriceLine();

        /// <summary>
        /// Plots a horizontal line at current price, useful for market profile type indicators.
        /// </summary>
        /// <returns></returns>
        public DH_HorizPriceLine DH_HorizPriceLine()
        {
            return DH_HorizPriceLine(Input);
        }

        /// <summary>
        /// Plots a horizontal line at current price, useful for market profile type indicators.
        /// </summary>
        /// <returns></returns>
        public DH_HorizPriceLine DH_HorizPriceLine(Data.IDataSeries input)
        {
            if (cacheDH_HorizPriceLine != null)
                for (int idx = 0; idx < cacheDH_HorizPriceLine.Length; idx++)
                    if (cacheDH_HorizPriceLine[idx].EqualsInput(input))
                        return cacheDH_HorizPriceLine[idx];

            lock (checkDH_HorizPriceLine)
            {
                if (cacheDH_HorizPriceLine != null)
                    for (int idx = 0; idx < cacheDH_HorizPriceLine.Length; idx++)
                        if (cacheDH_HorizPriceLine[idx].EqualsInput(input))
                            return cacheDH_HorizPriceLine[idx];

                DH_HorizPriceLine indicator = new DH_HorizPriceLine();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                DH_HorizPriceLine[] tmp = new DH_HorizPriceLine[cacheDH_HorizPriceLine == null ? 1 : cacheDH_HorizPriceLine.Length + 1];
                if (cacheDH_HorizPriceLine != null)
                    cacheDH_HorizPriceLine.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDH_HorizPriceLine = tmp;
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
        /// Plots a horizontal line at current price, useful for market profile type indicators.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DH_HorizPriceLine DH_HorizPriceLine()
        {
            return _indicator.DH_HorizPriceLine(Input);
        }

        /// <summary>
        /// Plots a horizontal line at current price, useful for market profile type indicators.
        /// </summary>
        /// <returns></returns>
        public Indicator.DH_HorizPriceLine DH_HorizPriceLine(Data.IDataSeries input)
        {
            return _indicator.DH_HorizPriceLine(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots a horizontal line at current price, useful for market profile type indicators.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DH_HorizPriceLine DH_HorizPriceLine()
        {
            return _indicator.DH_HorizPriceLine(Input);
        }

        /// <summary>
        /// Plots a horizontal line at current price, useful for market profile type indicators.
        /// </summary>
        /// <returns></returns>
        public Indicator.DH_HorizPriceLine DH_HorizPriceLine(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DH_HorizPriceLine(input);
        }
    }
}
#endregion
