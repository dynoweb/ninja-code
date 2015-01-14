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

#region Global Enums
public enum BollingerPercentB_MATypes {EMA, SMA}
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.
    /// </summary>
    [Description("The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.")]
    public class BollingerPercentB : Indicator
    {
        #region Variables
        // Wizard generated variables
            private BollingerPercentB_MATypes averageType = BollingerPercentB_MATypes.SMA; // Default setting for AverageType
            private int period = 20; // Default setting for Period
            private double numStdDev = 2.000; // Default setting for NumStdDev
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkCyan), PlotStyle.Bar, "PercentB"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 0, "ZeroLine"));
            Add(new Line(Color.FromKnownColor(KnownColor.HotTrack), 50, "HalfLine"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 100, "UnitLine"));
			PriceTypeSupported = true;
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			int tmpPeriod = Math.Min(CurrentBar, Period);
			
			double average = Close[0];
			
			switch (averageType)
			{
				case BollingerPercentB_MATypes.EMA:
					average = EMA(tmpPeriod)[0];
					break;
				case BollingerPercentB_MATypes.SMA:
					average = SMA(tmpPeriod)[0];
					break;
			}
			
			double stdDevValue = StdDev(tmpPeriod)[0];
			double upperValue = average + numStdDev * stdDevValue;
			double lowerValue = average - numStdDev * stdDevValue;

			//PercentB.Set((Input[0] - lowerValue)/(upperValue - lowerValue) * 100);
			PercentB.Set((Close[0] - lowerValue)/(upperValue - lowerValue) * 100);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PercentB
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public BollingerPercentB_MATypes AverageType
        {
            get { return averageType; }
            set { averageType = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double NumStdDev
        {
            get { return numStdDev; }
            set { numStdDev = Math.Max(0.100, value); }
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
        private BollingerPercentB[] cacheBollingerPercentB = null;

        private static BollingerPercentB checkBollingerPercentB = new BollingerPercentB();

        /// <summary>
        /// The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.
        /// </summary>
        /// <returns></returns>
        public BollingerPercentB BollingerPercentB(BollingerPercentB_MATypes averageType, double numStdDev, int period)
        {
            return BollingerPercentB(Input, averageType, numStdDev, period);
        }

        /// <summary>
        /// The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.
        /// </summary>
        /// <returns></returns>
        public BollingerPercentB BollingerPercentB(Data.IDataSeries input, BollingerPercentB_MATypes averageType, double numStdDev, int period)
        {
            if (cacheBollingerPercentB != null)
                for (int idx = 0; idx < cacheBollingerPercentB.Length; idx++)
                    if (cacheBollingerPercentB[idx].AverageType == averageType && Math.Abs(cacheBollingerPercentB[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheBollingerPercentB[idx].Period == period && cacheBollingerPercentB[idx].EqualsInput(input))
                        return cacheBollingerPercentB[idx];

            lock (checkBollingerPercentB)
            {
                checkBollingerPercentB.AverageType = averageType;
                averageType = checkBollingerPercentB.AverageType;
                checkBollingerPercentB.NumStdDev = numStdDev;
                numStdDev = checkBollingerPercentB.NumStdDev;
                checkBollingerPercentB.Period = period;
                period = checkBollingerPercentB.Period;

                if (cacheBollingerPercentB != null)
                    for (int idx = 0; idx < cacheBollingerPercentB.Length; idx++)
                        if (cacheBollingerPercentB[idx].AverageType == averageType && Math.Abs(cacheBollingerPercentB[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheBollingerPercentB[idx].Period == period && cacheBollingerPercentB[idx].EqualsInput(input))
                            return cacheBollingerPercentB[idx];

                BollingerPercentB indicator = new BollingerPercentB();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AverageType = averageType;
                indicator.NumStdDev = numStdDev;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                BollingerPercentB[] tmp = new BollingerPercentB[cacheBollingerPercentB == null ? 1 : cacheBollingerPercentB.Length + 1];
                if (cacheBollingerPercentB != null)
                    cacheBollingerPercentB.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBollingerPercentB = tmp;
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
        /// The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BollingerPercentB BollingerPercentB(BollingerPercentB_MATypes averageType, double numStdDev, int period)
        {
            return _indicator.BollingerPercentB(Input, averageType, numStdDev, period);
        }

        /// <summary>
        /// The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.
        /// </summary>
        /// <returns></returns>
        public Indicator.BollingerPercentB BollingerPercentB(Data.IDataSeries input, BollingerPercentB_MATypes averageType, double numStdDev, int period)
        {
            return _indicator.BollingerPercentB(input, averageType, numStdDev, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BollingerPercentB BollingerPercentB(BollingerPercentB_MATypes averageType, double numStdDev, int period)
        {
            return _indicator.BollingerPercentB(Input, averageType, numStdDev, period);
        }

        /// <summary>
        /// The Bollinger Bands® Percentage B is a technical indicator based upon the Bollinger Bands® study; it plots a histogram showing position of price relative to the bands. It is calculated as percentage ratio of two differences: first one is the difference between the price and the lower band value, second one is the difference between values of upper and lower bands.
        /// </summary>
        /// <returns></returns>
        public Indicator.BollingerPercentB BollingerPercentB(Data.IDataSeries input, BollingerPercentB_MATypes averageType, double numStdDev, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BollingerPercentB(input, averageType, numStdDev, period);
        }
    }
}
#endregion
