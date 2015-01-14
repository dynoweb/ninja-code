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
    /// Calculates Width of Bollinger Bands based on period and std. deviation
    /// </summary>
    [Description("Calculates Width of Bollinger Bands based on period and std. deviation")]
    public class BollingerBandWidth2 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 20; // Default setting for Period
            private double deviation = 2.000; // Default setting for Deviation
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "Plot0"));
            CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            //Plot0.Set(Close[0]);
			
			if (CurrentBar > Period) 
			{
				Plot0.Set((Bollinger(deviation,Period).Upper[0] - Bollinger(deviation,Period).Lower[0])/SMA(20)[0]);
				
			}
			else
			{		
				Plot0.Set(0);
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("Used to calculates Bollinger Bands for these # of days")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("Used to calculates Bollinger Bands for this standard deviation")]
        [Category("Parameters")]
        public double Deviation
        {
            get { return deviation; }
            set { deviation = Math.Max(1, value); }
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
        private BollingerBandWidth2[] cacheBollingerBandWidth2 = null;

        private static BollingerBandWidth2 checkBollingerBandWidth2 = new BollingerBandWidth2();

        /// <summary>
        /// Calculates Width of Bollinger Bands based on period and std. deviation
        /// </summary>
        /// <returns></returns>
        public BollingerBandWidth2 BollingerBandWidth2(double deviation, int period)
        {
            return BollingerBandWidth2(Input, deviation, period);
        }

        /// <summary>
        /// Calculates Width of Bollinger Bands based on period and std. deviation
        /// </summary>
        /// <returns></returns>
        public BollingerBandWidth2 BollingerBandWidth2(Data.IDataSeries input, double deviation, int period)
        {
            if (cacheBollingerBandWidth2 != null)
                for (int idx = 0; idx < cacheBollingerBandWidth2.Length; idx++)
                    if (Math.Abs(cacheBollingerBandWidth2[idx].Deviation - deviation) <= double.Epsilon && cacheBollingerBandWidth2[idx].Period == period && cacheBollingerBandWidth2[idx].EqualsInput(input))
                        return cacheBollingerBandWidth2[idx];

            lock (checkBollingerBandWidth2)
            {
                checkBollingerBandWidth2.Deviation = deviation;
                deviation = checkBollingerBandWidth2.Deviation;
                checkBollingerBandWidth2.Period = period;
                period = checkBollingerBandWidth2.Period;

                if (cacheBollingerBandWidth2 != null)
                    for (int idx = 0; idx < cacheBollingerBandWidth2.Length; idx++)
                        if (Math.Abs(cacheBollingerBandWidth2[idx].Deviation - deviation) <= double.Epsilon && cacheBollingerBandWidth2[idx].Period == period && cacheBollingerBandWidth2[idx].EqualsInput(input))
                            return cacheBollingerBandWidth2[idx];

                BollingerBandWidth2 indicator = new BollingerBandWidth2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Deviation = deviation;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                BollingerBandWidth2[] tmp = new BollingerBandWidth2[cacheBollingerBandWidth2 == null ? 1 : cacheBollingerBandWidth2.Length + 1];
                if (cacheBollingerBandWidth2 != null)
                    cacheBollingerBandWidth2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBollingerBandWidth2 = tmp;
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
        /// Calculates Width of Bollinger Bands based on period and std. deviation
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BollingerBandWidth2 BollingerBandWidth2(double deviation, int period)
        {
            return _indicator.BollingerBandWidth2(Input, deviation, period);
        }

        /// <summary>
        /// Calculates Width of Bollinger Bands based on period and std. deviation
        /// </summary>
        /// <returns></returns>
        public Indicator.BollingerBandWidth2 BollingerBandWidth2(Data.IDataSeries input, double deviation, int period)
        {
            return _indicator.BollingerBandWidth2(input, deviation, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Calculates Width of Bollinger Bands based on period and std. deviation
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BollingerBandWidth2 BollingerBandWidth2(double deviation, int period)
        {
            return _indicator.BollingerBandWidth2(Input, deviation, period);
        }

        /// <summary>
        /// Calculates Width of Bollinger Bands based on period and std. deviation
        /// </summary>
        /// <returns></returns>
        public Indicator.BollingerBandWidth2 BollingerBandWidth2(Data.IDataSeries input, double deviation, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BollingerBandWidth2(input, deviation, period);
        }
    }
}
#endregion
