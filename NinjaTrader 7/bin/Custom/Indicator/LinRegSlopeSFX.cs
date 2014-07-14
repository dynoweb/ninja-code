// LinRegSlopeSFX
// by Alex Matulich / Unicorn Research Corporation / 29 August 2008
// Linear regression super fast calculation with option for exponential weighting.
// This indicator can return slope, regression value, or average error.
//
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
    /// Linear regression slope - super fast calc with exponential weighting option
    /// </summary>
    [Description("Linear regression slope - super fast calc with exponential weighting option")]
    public class LinRegSlopeSFX : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 8; // Default setting for Period
            private bool exponential = false; // Default setting for Exponential
			private int returnval = 0; // Default setting for returnval
        // exponential moving regression slope variables
			private int nn = 0;
			private double n, w, sumX, sumX2, sumY1, sumXY1, correction;
		// super fast linear regression slope variables
			private double m1, m2, sumP, sumIP;
		// public results, updated each bar
			public double slope, yvalue, err;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumBlue), PlotStyle.Line, "SlopeSFX"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Zero"));
            CalculateOnBarClose	= true; // MUST BE TRUE, WILL NOT WORK ON REAL TIME DATA
            Overlay				= false;
            PriceTypeSupported	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double rtn;
			int ix;
			if (exponential) {
/*
 Exponential linear regression slope
 Copyright (c) 2004 by Alex Matulich / Unicorn Research Corporation.
 Ported from TradeStation to NinjaTrader August 2008 by Alex Matulich.
 Tradestation version is published at http://unicorn.us.com/trading/el.html

 This function calculates a linear regression slope using exponential
 moving averages as the summation terms in the slope formula.  A
 simple average multiplied by the number of elements equals the sum,
 so we can calcuate the regression slope using simple averages.
 However, if we replace the simple averages in the computation by
 exponential averages, the resulting slope magnitude turns out larger
 by a factor of 3N/(N+1), where N is the number of elements, therefore
 we must divide the result by this correction factor (this simple
 correction was actually a lot of work to figure out).

 The correction factor will give perfect results for regions of
 perfectly constant slopes, but for noisy data like markets, the
 response time of the exponential average will make the SWINGS in
 slope magnitude appear smaller than the actual regression slope,
 although the slope will be smoother in areas of fairly constant slope.
*/
				double denom, sumY = 0.0, sumXY = 0.0;
				if (period != nn || CurrentBar < 1) { // re-initialize if period changes
					nn = period;                // save new period
					n = (nn-1.0)*0.75 + 1.0;    // lag correction
					sumX = 0.5 * n*(n-1.0);     // sum of x values from 0 to n-1
					sumX2 = (n-1.0)*n*(2.0*n-1.0)/6.0;  //sum of x^2 from 0 to n-1
					sumY = Input[0] * n;        // initialize sum of Y values
					sumXY = sumX*Input[0];      // initialize sum of X*Y values
					w = 2.0 / (n + 1.0);        // exponential weighting factor
					correction = (n+1.0) / (3.0*n); // amplitude correction factor
					if (CurrentBar < 1) {
						sumY1 = sumY;			// would be sumY[1] in a series
						sumXY1 = sumXY;			// would be sumXY[1] in a series
					}
				} else { // calculate sum of Y values and sum of X*Y values
					sumY = w*Input[0]*n + (1.0-w)*sumY1;
					sumXY = Input[0]*(n-1.0) + sumXY1*(n-2.0)/n;
					sumY1 = sumY;               // save for next bar
					sumXY1 = sumXY;             // save for next bar
				}
				denom = n*sumX2 - sumX*sumX;    // denominator of slope formula
				if (denom < 0.00001) denom = 0.00001;
				slope = correction * n*(n*sumXY - sumX*sumY) / denom;
				yvalue = sumY/n + slope * 0.5*n; // regression value, public variable
			}
			else {
/*
 Linear Regression Slope Super Fast Calc
 Derived from an algorithm developed by Bob Fulks and separately by
 Mark A. Simms in 2002.
 Elimination of calculation loop by Alex Matulich, April 2004.
 Ported from TradeStation to NinjaTrader by Alex Matulich, August 2008.
 Tradestation version is published at http://unicorn.us.com/trading/el.html
 
 This is a super-efficient version of the Fulks/Simms Linear
 Regression Slope Fast Calc algorithm.  In this version, a loop gets
 executed only once during initialization, rather than at every bar.
 The result matches exactly the traditional linear regression slope.

 This function assumes that the Y-axis (where X=0) always coincides
 with the current bar.  Therefore, the Y-intercept is the same as
 the value of the regression line at the current bar, and is given
 by the formula:

 YIntercept = yvalue = Average(Price, Length) + slope * Length/2;
*/
				int ix1;
				// Re-initialize 0th bar, every 10000 bars, or when Period changes
				if (CurrentBar < period || nn != period || CurrentBar % 10000 == 0) {
					nn = period;
 					m1 = 6.0 / ((double)period * ((double)period + 1.0));
					m2 = 2.0 / ((double)period - 1.0);
					sumP = 0.0; sumIP = 0.0;
					// this loop is executed only during initialization
					for (ix = 0; ix < period; ++ix) {
						ix1 = (ix < CurrentBar) ? ix : CurrentBar;
						sumP += Input[ix1];
						sumIP += (ix1 * Input[ix1]);
					}
				} else {
					// Linear regression slope super fast calculation
					sumIP += (sumP - period * Input[period]);
					sumP += (Input[0] - Input[period]);
				}
				slope = m1 * (sumP - m2 * sumIP);
				yvalue = sumP/period + slope * period * 0.5;
			}
			switch (returnval) {
				case 3:
					rtn = 0.0;
					if (CurrentBar > 1) {
						for (ix = 0; ix < ((CurrentBar <= period) ? CurrentBar+1 : period); ++ix) {
							rtn = (Input[ix] - (-slope * ix + yvalue));
							err += rtn*rtn;
						}
						rtn = err = Math.Sqrt(err/ix);
					}
					break;
				case 2: rtn = yvalue; break;
				default: rtn = slope; break;
			}
			SlopeSFX.Set(rtn);         // result
        }
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SlopeSFX
        {
            get { return Values[0]; }
        }

        [Description("Lookback interval")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("true=exponential moving slope, false=normal weighting")]
        [Category("Parameters")]
        public bool Exponential
        {
            get { return exponential; }
            set { exponential = value; }
        }

		[Description("Return value 1=slope, 2=LinReg value, 3=average error")]
        [Category("Parameters")]
        public int Returnval
        {
            get { return returnval; }
            set { returnval = Math.Max(1, value); }
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
        private LinRegSlopeSFX[] cacheLinRegSlopeSFX = null;

        private static LinRegSlopeSFX checkLinRegSlopeSFX = new LinRegSlopeSFX();

        /// <summary>
        /// Linear regression slope - super fast calc with exponential weighting option
        /// </summary>
        /// <returns></returns>
        public LinRegSlopeSFX LinRegSlopeSFX(bool exponential, int period, int returnval)
        {
            return LinRegSlopeSFX(Input, exponential, period, returnval);
        }

        /// <summary>
        /// Linear regression slope - super fast calc with exponential weighting option
        /// </summary>
        /// <returns></returns>
        public LinRegSlopeSFX LinRegSlopeSFX(Data.IDataSeries input, bool exponential, int period, int returnval)
        {
            if (cacheLinRegSlopeSFX != null)
                for (int idx = 0; idx < cacheLinRegSlopeSFX.Length; idx++)
                    if (cacheLinRegSlopeSFX[idx].Exponential == exponential && cacheLinRegSlopeSFX[idx].Period == period && cacheLinRegSlopeSFX[idx].Returnval == returnval && cacheLinRegSlopeSFX[idx].EqualsInput(input))
                        return cacheLinRegSlopeSFX[idx];

            lock (checkLinRegSlopeSFX)
            {
                checkLinRegSlopeSFX.Exponential = exponential;
                exponential = checkLinRegSlopeSFX.Exponential;
                checkLinRegSlopeSFX.Period = period;
                period = checkLinRegSlopeSFX.Period;
                checkLinRegSlopeSFX.Returnval = returnval;
                returnval = checkLinRegSlopeSFX.Returnval;

                if (cacheLinRegSlopeSFX != null)
                    for (int idx = 0; idx < cacheLinRegSlopeSFX.Length; idx++)
                        if (cacheLinRegSlopeSFX[idx].Exponential == exponential && cacheLinRegSlopeSFX[idx].Period == period && cacheLinRegSlopeSFX[idx].Returnval == returnval && cacheLinRegSlopeSFX[idx].EqualsInput(input))
                            return cacheLinRegSlopeSFX[idx];

                LinRegSlopeSFX indicator = new LinRegSlopeSFX();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Exponential = exponential;
                indicator.Period = period;
                indicator.Returnval = returnval;
                Indicators.Add(indicator);
                indicator.SetUp();

                LinRegSlopeSFX[] tmp = new LinRegSlopeSFX[cacheLinRegSlopeSFX == null ? 1 : cacheLinRegSlopeSFX.Length + 1];
                if (cacheLinRegSlopeSFX != null)
                    cacheLinRegSlopeSFX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheLinRegSlopeSFX = tmp;
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
        /// Linear regression slope - super fast calc with exponential weighting option
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LinRegSlopeSFX LinRegSlopeSFX(bool exponential, int period, int returnval)
        {
            return _indicator.LinRegSlopeSFX(Input, exponential, period, returnval);
        }

        /// <summary>
        /// Linear regression slope - super fast calc with exponential weighting option
        /// </summary>
        /// <returns></returns>
        public Indicator.LinRegSlopeSFX LinRegSlopeSFX(Data.IDataSeries input, bool exponential, int period, int returnval)
        {
            return _indicator.LinRegSlopeSFX(input, exponential, period, returnval);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Linear regression slope - super fast calc with exponential weighting option
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LinRegSlopeSFX LinRegSlopeSFX(bool exponential, int period, int returnval)
        {
            return _indicator.LinRegSlopeSFX(Input, exponential, period, returnval);
        }

        /// <summary>
        /// Linear regression slope - super fast calc with exponential weighting option
        /// </summary>
        /// <returns></returns>
        public Indicator.LinRegSlopeSFX LinRegSlopeSFX(Data.IDataSeries input, bool exponential, int period, int returnval)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.LinRegSlopeSFX(input, exponential, period, returnval);
        }
    }
}
#endregion
