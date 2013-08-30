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
    /// Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.
    /// </summary>
    [Description("Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.")]
    public class ZigZagFib : Indicator
    {
        #region Variables

			private DeviationType	deviationType		= DeviationType.Points;
			private double			deviationValue		= 0.5;
			private bool			useHighLow			= true;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Blue, PlotStyle.Line, "Upper"));
			Add(new Plot(Color.Yellow, PlotStyle.Line, "UpperFib"));
			Add(new Plot(Color.Lime, PlotStyle.Line, "LowerFib"));
			Add(new Plot(Color.Red, PlotStyle.Line, "Lower"));
			
			DisplayInDataBox	= true;
			PaintPriceMarkers	= true;
            Overlay				= true;
			
        }
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			ZigZag zz = ZigZag(deviationType, deviationValue, useHighLow);
	
			Upper.Set(zz.ZigZagHigh[0]);
			Lower.Set(zz.ZigZagLow[0]); 
			UpperFib.Set(Upper[0] - (Upper[0]-Lower[0])*0.382);
			LowerFib.Set(Lower[0] + (Upper[0]-Lower[0])*0.382);
        }

        #region Properties

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Upper
        {
            get { return Values[0]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperFib
        {
            get { return Values[1]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerFib
        {
            get { return Values[2]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Lower
        {
            get { return Values[3]; }
        }
		
		[Description("Deviation in percent or points regarding on the deviation type")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Deviation value")]
        public double DeviationValue
        {
            get { return deviationValue; }
            set { deviationValue = Math.Max(0.0, value); }
        }

        [Description("Type of the deviation value")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Deviation type")]
        public DeviationType DeviationType
        {
            get { return deviationType; }
            set { deviationType = value; }
        }

        [Description("If true, high and low instead of selected price type is used to plot indicator.")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Use high and low")]
		[RefreshProperties(RefreshProperties.All)]
        public bool UseHighLow
        {
            get { return useHighLow; }
            set { useHighLow = value; }
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
        private ZigZagFib[] cacheZigZagFib = null;

        private static ZigZagFib checkZigZagFib = new ZigZagFib();

        /// <summary>
        /// Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.
        /// </summary>
        /// <returns></returns>
        public ZigZagFib ZigZagFib(DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return ZigZagFib(Input, deviationType, deviationValue, useHighLow);
        }

        /// <summary>
        /// Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.
        /// </summary>
        /// <returns></returns>
        public ZigZagFib ZigZagFib(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            if (cacheZigZagFib != null)
                for (int idx = 0; idx < cacheZigZagFib.Length; idx++)
                    if (cacheZigZagFib[idx].DeviationType == deviationType && Math.Abs(cacheZigZagFib[idx].DeviationValue - deviationValue) <= double.Epsilon && cacheZigZagFib[idx].UseHighLow == useHighLow && cacheZigZagFib[idx].EqualsInput(input))
                        return cacheZigZagFib[idx];

            lock (checkZigZagFib)
            {
                checkZigZagFib.DeviationType = deviationType;
                deviationType = checkZigZagFib.DeviationType;
                checkZigZagFib.DeviationValue = deviationValue;
                deviationValue = checkZigZagFib.DeviationValue;
                checkZigZagFib.UseHighLow = useHighLow;
                useHighLow = checkZigZagFib.UseHighLow;

                if (cacheZigZagFib != null)
                    for (int idx = 0; idx < cacheZigZagFib.Length; idx++)
                        if (cacheZigZagFib[idx].DeviationType == deviationType && Math.Abs(cacheZigZagFib[idx].DeviationValue - deviationValue) <= double.Epsilon && cacheZigZagFib[idx].UseHighLow == useHighLow && cacheZigZagFib[idx].EqualsInput(input))
                            return cacheZigZagFib[idx];

                ZigZagFib indicator = new ZigZagFib();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DeviationType = deviationType;
                indicator.DeviationValue = deviationValue;
                indicator.UseHighLow = useHighLow;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZigZagFib[] tmp = new ZigZagFib[cacheZigZagFib == null ? 1 : cacheZigZagFib.Length + 1];
                if (cacheZigZagFib != null)
                    cacheZigZagFib.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZigZagFib = tmp;
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
        /// Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZigZagFib ZigZagFib(DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return _indicator.ZigZagFib(Input, deviationType, deviationValue, useHighLow);
        }

        /// <summary>
        /// Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZigZagFib ZigZagFib(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return _indicator.ZigZagFib(input, deviationType, deviationValue, useHighLow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZigZagFib ZigZagFib(DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return _indicator.ZigZagFib(Input, deviationType, deviationValue, useHighLow);
        }

        /// <summary>
        /// Extends the ZigZag indicator by showing the high, low lines and adds in a 38.2% plot above the low and below the top.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZigZagFib ZigZagFib(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZigZagFib(input, deviationType, deviationValue, useHighLow);
        }
    }
}
#endregion
