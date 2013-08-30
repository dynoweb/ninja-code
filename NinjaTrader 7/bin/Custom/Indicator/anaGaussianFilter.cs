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
    /// </summary>
    /// This is an implementation of the 2-pole, 3-pole and 4-pole Gaussian Filters, as published by John F. Ehlers
	/// in his publication "Gaussian and Other Low Lag Filters".
	[Description("Gaussian Filter")]
    [Gui.Design.DisplayName("anaGaussianFilter")]
    public class anaGaussianFilter : Indicator
    {
        #region Variables
        // Wizard generated variables
        private int period = 20;
        private int poles = 3;
        private double alpha = 0;
        private double beta = 0;
        private double alpha2 = 0;
        private double alpha3 = 0;
        private double alpha4 = 0;
		private double coeff1 = 0;
 		private double coeff2 = 0;
		private double coeff3 = 0;
		private double coeff4 = 0;
        private double recurrentPart = 0;
        #endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.DeepSkyBlue, PlotStyle.Line, "Gaussian"));
            CalculateOnBarClose = false;
            Overlay = true;
            PriceTypeSupported = true;
        }

        protected override void OnStartUp()
        {
			double Pi = Math.PI;
			double Sq2 = Math.Sqrt(2.0);
			beta = (1 - Math.Cos(2*Pi/period))/(Math.Pow (Sq2, 2.0/Poles) - 1);
			alpha = - beta + Math.Sqrt(beta*(beta + 2));
			alpha2 = alpha*alpha;
			alpha3 = alpha2*alpha;
			alpha4 = alpha3*alpha;
			coeff1 = 1.0 - alpha;
			coeff2 = coeff1*coeff1;
			coeff3 = coeff2*coeff1;
			coeff4 = coeff3*coeff1;
        }
		
		/// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar < Poles)
            {
                Gaussian.Set(Input[0]);
				return;
            }
          	if (FirstTickOfBar)
			{
				if ( Poles == 1)
					recurrentPart = coeff1*Value[1];
				else if (Poles == 2)
					recurrentPart = 2*coeff1*Value[1] - coeff2*Value[2];
				else if (Poles == 3)
					recurrentPart = 3*coeff1*Value[1] - 3*coeff2*Value[2] + coeff3*Value[3];
				else if (Poles == 4)	
					recurrentPart = 4*coeff1*Value[1] - 6*coeff2*Value[2] + 4*coeff3*Value[3] - coeff4*Value[4];
			}
			if ( Poles == 1)
				Gaussian.Set(alpha*Input[0] + recurrentPart);
			else if (Poles == 2)
				Gaussian.Set(alpha2*Input[0] + recurrentPart);
			else if (Poles == 3)
				Gaussian.Set(alpha3*Input[0] + recurrentPart);
			else if (Poles == 4)	
				Gaussian.Set(alpha4*Input[0] + recurrentPart);
        }

        #region Properties
        [Browsable(false)]	
        [XmlIgnore()]		
        public DataSeries Gaussian
        {
            get { return Values[0]; }
        }

        [Description("")]
        [Category("Parameters")]
        public int Poles
        {
            get { return poles; }
            set { poles = Math.Min(Math.Max(1,value),4); }
        }

        [Description("")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
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
        private anaGaussianFilter[] cacheanaGaussianFilter = null;

        private static anaGaussianFilter checkanaGaussianFilter = new anaGaussianFilter();

        /// <summary>
        /// Gaussian Filter
        /// </summary>
        /// <returns></returns>
        public anaGaussianFilter anaGaussianFilter(int period, int poles)
        {
            return anaGaussianFilter(Input, period, poles);
        }

        /// <summary>
        /// Gaussian Filter
        /// </summary>
        /// <returns></returns>
        public anaGaussianFilter anaGaussianFilter(Data.IDataSeries input, int period, int poles)
        {
            if (cacheanaGaussianFilter != null)
                for (int idx = 0; idx < cacheanaGaussianFilter.Length; idx++)
                    if (cacheanaGaussianFilter[idx].Period == period && cacheanaGaussianFilter[idx].Poles == poles && cacheanaGaussianFilter[idx].EqualsInput(input))
                        return cacheanaGaussianFilter[idx];

            lock (checkanaGaussianFilter)
            {
                checkanaGaussianFilter.Period = period;
                period = checkanaGaussianFilter.Period;
                checkanaGaussianFilter.Poles = poles;
                poles = checkanaGaussianFilter.Poles;

                if (cacheanaGaussianFilter != null)
                    for (int idx = 0; idx < cacheanaGaussianFilter.Length; idx++)
                        if (cacheanaGaussianFilter[idx].Period == period && cacheanaGaussianFilter[idx].Poles == poles && cacheanaGaussianFilter[idx].EqualsInput(input))
                            return cacheanaGaussianFilter[idx];

                anaGaussianFilter indicator = new anaGaussianFilter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Poles = poles;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaGaussianFilter[] tmp = new anaGaussianFilter[cacheanaGaussianFilter == null ? 1 : cacheanaGaussianFilter.Length + 1];
                if (cacheanaGaussianFilter != null)
                    cacheanaGaussianFilter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaGaussianFilter = tmp;
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
        /// Gaussian Filter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaGaussianFilter anaGaussianFilter(int period, int poles)
        {
            return _indicator.anaGaussianFilter(Input, period, poles);
        }

        /// <summary>
        /// Gaussian Filter
        /// </summary>
        /// <returns></returns>
        public Indicator.anaGaussianFilter anaGaussianFilter(Data.IDataSeries input, int period, int poles)
        {
            return _indicator.anaGaussianFilter(input, period, poles);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Gaussian Filter
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaGaussianFilter anaGaussianFilter(int period, int poles)
        {
            return _indicator.anaGaussianFilter(Input, period, poles);
        }

        /// <summary>
        /// Gaussian Filter
        /// </summary>
        /// <returns></returns>
        public Indicator.anaGaussianFilter anaGaussianFilter(Data.IDataSeries input, int period, int poles)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaGaussianFilter(input, period, poles);
        }
    }
}
#endregion
