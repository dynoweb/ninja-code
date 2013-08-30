// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
	/// </summary>
	[Description("The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.")]
	public class anaMovingMedian : Indicator
	{
		#region Variables
		private int	period			= 14;
		private int medianIndex		= 7;
		private int priorIndex		= 6;
		private bool even			= true;
		private ArrayList mArray 	= new ArrayList();
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.DeepSkyBlue, "anaMovingMedian"));
			Overlay = true;
		}

		protected override void OnStartUp()
		{
			for (int i = 0; i<Period; i++)
				mArray.Add((double)0.0);
			if (Period % 2 == 0)
			{
				even = true;
				medianIndex = Period/2;
				priorIndex = medianIndex - 1;
			}
			else
			{
				even = false;
				medianIndex = (Period - 1)/2;
			}
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick).
		/// </summary>
		protected override void OnBarUpdate()
		{
			if(CurrentBar < Period)
			{
				int sPeriod = CurrentBar + 1;
				for (int i = 0; i<sPeriod; i++)
				{
					mArray[i] = Input[i];
				}
				mArray.Sort();
				if (sPeriod % 2 == 0)
					Value.Set( 0.5 * ((double) mArray[Period - 1 - sPeriod/2] + (double) mArray[Period - sPeriod/2]));
				else
					Value.Set( (double) mArray[Period - (1 + sPeriod)/2]);
			}
			else
			{
				for (int i = 0; i<Period; i++)
				{
					mArray[i] = Input[i];
				}
				mArray.Sort();
				if (even)
					Value.Set( 0.5 * ((double) mArray[medianIndex] + (double) mArray[priorIndex]));
				else
					Value.Set( (double) mArray[medianIndex]);
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
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
        private anaMovingMedian[] cacheanaMovingMedian = null;

        private static anaMovingMedian checkanaMovingMedian = new anaMovingMedian();

        /// <summary>
        /// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public anaMovingMedian anaMovingMedian(int period)
        {
            return anaMovingMedian(Input, period);
        }

        /// <summary>
        /// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public anaMovingMedian anaMovingMedian(Data.IDataSeries input, int period)
        {
            if (cacheanaMovingMedian != null)
                for (int idx = 0; idx < cacheanaMovingMedian.Length; idx++)
                    if (cacheanaMovingMedian[idx].Period == period && cacheanaMovingMedian[idx].EqualsInput(input))
                        return cacheanaMovingMedian[idx];

            lock (checkanaMovingMedian)
            {
                checkanaMovingMedian.Period = period;
                period = checkanaMovingMedian.Period;

                if (cacheanaMovingMedian != null)
                    for (int idx = 0; idx < cacheanaMovingMedian.Length; idx++)
                        if (cacheanaMovingMedian[idx].Period == period && cacheanaMovingMedian[idx].EqualsInput(input))
                            return cacheanaMovingMedian[idx];

                anaMovingMedian indicator = new anaMovingMedian();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaMovingMedian[] tmp = new anaMovingMedian[cacheanaMovingMedian == null ? 1 : cacheanaMovingMedian.Length + 1];
                if (cacheanaMovingMedian != null)
                    cacheanaMovingMedian.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaMovingMedian = tmp;
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
        /// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMovingMedian anaMovingMedian(int period)
        {
            return _indicator.anaMovingMedian(Input, period);
        }

        /// <summary>
        /// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMovingMedian anaMovingMedian(Data.IDataSeries input, int period)
        {
            return _indicator.anaMovingMedian(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaMovingMedian anaMovingMedian(int period)
        {
            return _indicator.anaMovingMedian(Input, period);
        }

        /// <summary>
        /// The anaMovingMedian (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaMovingMedian anaMovingMedian(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaMovingMedian(input, period);
        }
    }
}
#endregion
