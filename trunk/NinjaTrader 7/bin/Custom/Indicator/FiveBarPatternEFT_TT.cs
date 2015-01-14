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
    /// Added markers to indicate 5 bar pattern resistance support and 10% target levels.
	/// This indicator is based on ETF's Trend Trading program
	/// Written by Rick Cromer for NinjaTrader
    /// </summary>
    [Description("Added markers to indicate 5 bar pattern resistance support")]
    public class FiveBarPatternEFT_TT : Indicator
    {
        #region Variables
			private int period = 10;
			private double priorHigh = 0;
			private double priorLow = Double.MaxValue;
			private int donchianPeriod = 22;
			private int smaPeriod = 100;
			private Color upColor = Color.Green;
			private Color downColor = Color.Red;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Dot, "Upper"));
            Add(new Plot(Color.Red, PlotStyle.Dot, "Lower"));
            Add(new Plot(Color.Green, PlotStyle.Dot, "UpperTarget"));
            Add(new Plot(Color.Green, PlotStyle.Dot, "LowerTarget"));
			//Add(new Plot(Color.SlateBlue, "UpperDonchian"));
			//Add(new Plot(Color.SlateBlue, "LowerDonchian"));
			Add(new Plot(Color.Green, "Trend"));
			
			Plots[6].Pen.Width = 2;
			Plots[6].Pen.Color = Color.Gray;
			
			Overlay				= true;			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Code for SMA 100 line
			if (CurrentBar == 0)
				Trend.Set(Input[0]);
			else
			{
				double last = Trend[1] * Math.Min(CurrentBar, smaPeriod);

				if (CurrentBar >= smaPeriod)
				{
					Trend.Set((last + Input[0] - Input[smaPeriod]) / Math.Min(CurrentBar, smaPeriod));
					
					if (CountIf(delegate {return Trend[0] > Trend[1];}, 15) == 15)
					//if (Rising(Trend))
					{
						PlotColors[6][0] = upColor;
					}
					else if (CountIf(delegate {return Trend[0] < Trend[1];}, 15) == 15)
					{
						PlotColors[6][0] = downColor;
					}
					
				}
				else
					Trend.Set((last + Input[0]) / (Math.Min(CurrentBar, smaPeriod) + 1));
				
			}

			// Code for other indicators
			if (CurrentBar < 5)
				return;
			
			if (High[0] < High[2] && High[1] < High[2] && High[3] < High[2] && High[4] < High[2]) 
			{
				double halfStdDevValue = (StdDev(period)[2])/2;
				//Plots[0].Pen = new Pen(Color.Green);
				Values[0][2] = High[2] + halfStdDevValue;
				if (High[2] > Close[HighestBar(Close, donchianPeriod)])
				{
					UpperTarget.Set(2, High[2] * 1.1);	//  10% above
				}
//				else
//				{
//					Print(Time + " CurrentBar: " + CurrentBar + " HighestBar(High, 22): " + HighestBar(High, 22));
//				}
			}
			
			if (Low[0] > Low[2] && Low[1] > Low[2] && Low[3] > Low[2] && Low[4] > Low[2]) 
			{
				double halfStdDevValue = (StdDev(period)[2])/2;
				//Values[1][2] = Low[2] - halfStdDevValue;
				Lower.Set(2, Low[2] - halfStdDevValue);
				if (Low[2] < Close[LowestBar(Close, donchianPeriod)])
				{
					LowerTarget.Set(2, Low[2] * 0.9);	// 10% below low
				}
			}
			
			//UpperDonchian.Set(MAX(Close, donchianPeriod)[0]);
			//LowerDonchian.Set(MIN(Close, donchianPeriod)[0]);

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
        public DataSeries Lower
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperTarget
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerTarget
        {
            get { return Values[3]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperDonchian
        {
            get { return Values[4]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerDonchian
        {
            get { return Values[5]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Trend
        {
            get { return Values[6]; }
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
        private FiveBarPatternEFT_TT[] cacheFiveBarPatternEFT_TT = null;

        private static FiveBarPatternEFT_TT checkFiveBarPatternEFT_TT = new FiveBarPatternEFT_TT();

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public FiveBarPatternEFT_TT FiveBarPatternEFT_TT()
        {
            return FiveBarPatternEFT_TT(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public FiveBarPatternEFT_TT FiveBarPatternEFT_TT(Data.IDataSeries input)
        {
            if (cacheFiveBarPatternEFT_TT != null)
                for (int idx = 0; idx < cacheFiveBarPatternEFT_TT.Length; idx++)
                    if (cacheFiveBarPatternEFT_TT[idx].EqualsInput(input))
                        return cacheFiveBarPatternEFT_TT[idx];

            lock (checkFiveBarPatternEFT_TT)
            {
                if (cacheFiveBarPatternEFT_TT != null)
                    for (int idx = 0; idx < cacheFiveBarPatternEFT_TT.Length; idx++)
                        if (cacheFiveBarPatternEFT_TT[idx].EqualsInput(input))
                            return cacheFiveBarPatternEFT_TT[idx];

                FiveBarPatternEFT_TT indicator = new FiveBarPatternEFT_TT();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                FiveBarPatternEFT_TT[] tmp = new FiveBarPatternEFT_TT[cacheFiveBarPatternEFT_TT == null ? 1 : cacheFiveBarPatternEFT_TT.Length + 1];
                if (cacheFiveBarPatternEFT_TT != null)
                    cacheFiveBarPatternEFT_TT.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheFiveBarPatternEFT_TT = tmp;
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
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FiveBarPatternEFT_TT FiveBarPatternEFT_TT()
        {
            return _indicator.FiveBarPatternEFT_TT(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarPatternEFT_TT FiveBarPatternEFT_TT(Data.IDataSeries input)
        {
            return _indicator.FiveBarPatternEFT_TT(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FiveBarPatternEFT_TT FiveBarPatternEFT_TT()
        {
            return _indicator.FiveBarPatternEFT_TT(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern resistance support
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarPatternEFT_TT FiveBarPatternEFT_TT(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.FiveBarPatternEFT_TT(input);
        }
    }
}
#endregion
