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
public enum BollingerBandwidth_MATypes {EMA, SMA}
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	
    /// <summary>
    /// The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, 
	/// expressing the distance between upper and lower bands as percentage of the middle band value 
	/// (SMA/EMA around which the bands are plotted). The main plot is accompanied with 
	/// two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth 
	/// value reached on the specified period, and, similarly, Squeeze plot shows the 
	/// lowest bandwidth value.
    /// </summary>
    [Description("The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, expressing the distance between upper and lower bands as percentage of the middle band value (SMA/EMA around which the bands are plotted). The main plot is accompanied with two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth value reached on the specified period, and, similarly, Squeeze plot shows the lowest bandwidth value.")]
    public class BollingerBandwidth : Indicator
    {
        #region Variables
            private int period = 20; // Default setting for Length
			private BollingerBandwidth_MATypes averageType = BollingerBandwidth_MATypes.SMA; 
            private int bulgeLength = 100; // Default setting for BulgeLength
            private int squeezeLength = 100; // Default setting for SqueezeLength
			private double numStdDev = 2.0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "Bandwidth"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Bulge"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "Squeeze"));
			
			BarsRequired = 1;
			
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// used to set values for bars less than the period
			int tmpPeriod = Math.Min(CurrentBar, Period);
			
			double average = Close[0];
			
			switch (averageType)
			{
				case BollingerBandwidth_MATypes.EMA:
					average = EMA(tmpPeriod)[0];
					break;
				case BollingerBandwidth_MATypes.SMA:
					average = SMA(tmpPeriod)[0];
					break;
			}
			
			double stdDevValue = StdDev(tmpPeriod)[0];
			double upperValue = average + numStdDev * stdDevValue;
			double lowerValue = average - numStdDev * stdDevValue;

			Bandwidth.Set((upperValue - lowerValue) / average * 100);
			Bulge.Set(MAX(Bandwidth, bulgeLength)[0]);
			Squeeze.Set(MIN(Bandwidth, squeezeLength)[0]);
			}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bandwidth
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bulge
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Squeeze
        {
            get { return Values[2]; }
        }

        [Description("band period")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("Uses either EMA or SMA")]
        [GridCategory("Parameters")]
        public BollingerBandwidth_MATypes AverageType
        {
            get { return averageType; }
            set { averageType = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int BulgeLength
        {
            get { return bulgeLength; }
            set { bulgeLength = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SqueezeLength
        {
            get { return squeezeLength; }
            set { squeezeLength = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public double NumStdDev
        {
            get { return numStdDev; }
            set { numStdDev = Math.Max(0.1, value); }
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
        private BollingerBandwidth[] cacheBollingerBandwidth = null;

        private static BollingerBandwidth checkBollingerBandwidth = new BollingerBandwidth();

        /// <summary>
        /// The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, expressing the distance between upper and lower bands as percentage of the middle band value (SMA/EMA around which the bands are plotted). The main plot is accompanied with two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth value reached on the specified period, and, similarly, Squeeze plot shows the lowest bandwidth value.
        /// </summary>
        /// <returns></returns>
        public BollingerBandwidth BollingerBandwidth(BollingerBandwidth_MATypes averageType, int bulgeLength, double numStdDev, int period, int squeezeLength)
        {
            return BollingerBandwidth(Input, averageType, bulgeLength, numStdDev, period, squeezeLength);
        }

        /// <summary>
        /// The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, expressing the distance between upper and lower bands as percentage of the middle band value (SMA/EMA around which the bands are plotted). The main plot is accompanied with two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth value reached on the specified period, and, similarly, Squeeze plot shows the lowest bandwidth value.
        /// </summary>
        /// <returns></returns>
        public BollingerBandwidth BollingerBandwidth(Data.IDataSeries input, BollingerBandwidth_MATypes averageType, int bulgeLength, double numStdDev, int period, int squeezeLength)
        {
            if (cacheBollingerBandwidth != null)
                for (int idx = 0; idx < cacheBollingerBandwidth.Length; idx++)
                    if (cacheBollingerBandwidth[idx].AverageType == averageType && cacheBollingerBandwidth[idx].BulgeLength == bulgeLength && Math.Abs(cacheBollingerBandwidth[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheBollingerBandwidth[idx].Period == period && cacheBollingerBandwidth[idx].SqueezeLength == squeezeLength && cacheBollingerBandwidth[idx].EqualsInput(input))
                        return cacheBollingerBandwidth[idx];

            lock (checkBollingerBandwidth)
            {
                checkBollingerBandwidth.AverageType = averageType;
                averageType = checkBollingerBandwidth.AverageType;
                checkBollingerBandwidth.BulgeLength = bulgeLength;
                bulgeLength = checkBollingerBandwidth.BulgeLength;
                checkBollingerBandwidth.NumStdDev = numStdDev;
                numStdDev = checkBollingerBandwidth.NumStdDev;
                checkBollingerBandwidth.Period = period;
                period = checkBollingerBandwidth.Period;
                checkBollingerBandwidth.SqueezeLength = squeezeLength;
                squeezeLength = checkBollingerBandwidth.SqueezeLength;

                if (cacheBollingerBandwidth != null)
                    for (int idx = 0; idx < cacheBollingerBandwidth.Length; idx++)
                        if (cacheBollingerBandwidth[idx].AverageType == averageType && cacheBollingerBandwidth[idx].BulgeLength == bulgeLength && Math.Abs(cacheBollingerBandwidth[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheBollingerBandwidth[idx].Period == period && cacheBollingerBandwidth[idx].SqueezeLength == squeezeLength && cacheBollingerBandwidth[idx].EqualsInput(input))
                            return cacheBollingerBandwidth[idx];

                BollingerBandwidth indicator = new BollingerBandwidth();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AverageType = averageType;
                indicator.BulgeLength = bulgeLength;
                indicator.NumStdDev = numStdDev;
                indicator.Period = period;
                indicator.SqueezeLength = squeezeLength;
                Indicators.Add(indicator);
                indicator.SetUp();

                BollingerBandwidth[] tmp = new BollingerBandwidth[cacheBollingerBandwidth == null ? 1 : cacheBollingerBandwidth.Length + 1];
                if (cacheBollingerBandwidth != null)
                    cacheBollingerBandwidth.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBollingerBandwidth = tmp;
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
        /// The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, expressing the distance between upper and lower bands as percentage of the middle band value (SMA/EMA around which the bands are plotted). The main plot is accompanied with two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth value reached on the specified period, and, similarly, Squeeze plot shows the lowest bandwidth value.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BollingerBandwidth BollingerBandwidth(BollingerBandwidth_MATypes averageType, int bulgeLength, double numStdDev, int period, int squeezeLength)
        {
            return _indicator.BollingerBandwidth(Input, averageType, bulgeLength, numStdDev, period, squeezeLength);
        }

        /// <summary>
        /// The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, expressing the distance between upper and lower bands as percentage of the middle band value (SMA/EMA around which the bands are plotted). The main plot is accompanied with two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth value reached on the specified period, and, similarly, Squeeze plot shows the lowest bandwidth value.
        /// </summary>
        /// <returns></returns>
        public Indicator.BollingerBandwidth BollingerBandwidth(Data.IDataSeries input, BollingerBandwidth_MATypes averageType, int bulgeLength, double numStdDev, int period, int squeezeLength)
        {
            return _indicator.BollingerBandwidth(input, averageType, bulgeLength, numStdDev, period, squeezeLength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, expressing the distance between upper and lower bands as percentage of the middle band value (SMA/EMA around which the bands are plotted). The main plot is accompanied with two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth value reached on the specified period, and, similarly, Squeeze plot shows the lowest bandwidth value.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BollingerBandwidth BollingerBandwidth(BollingerBandwidth_MATypes averageType, int bulgeLength, double numStdDev, int period, int squeezeLength)
        {
            return _indicator.BollingerBandwidth(Input, averageType, bulgeLength, numStdDev, period, squeezeLength);
        }

        /// <summary>
        /// The Bollinger Bandwidth study is a technical indicator based upon Bollinger Bands® study, expressing the distance between upper and lower bands as percentage of the middle band value (SMA/EMA around which the bands are plotted). The main plot is accompanied with two additional ones: Bulge and Squeeze. Bulge plot displays the highest bandwidth value reached on the specified period, and, similarly, Squeeze plot shows the lowest bandwidth value.
        /// </summary>
        /// <returns></returns>
        public Indicator.BollingerBandwidth BollingerBandwidth(Data.IDataSeries input, BollingerBandwidth_MATypes averageType, int bulgeLength, double numStdDev, int period, int squeezeLength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BollingerBandwidth(input, averageType, bulgeLength, numStdDev, period, squeezeLength);
        }
    }
}
#endregion
