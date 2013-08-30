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
    /// ADXVMA
	/// June 20 2009 updated to v2.0 by Big Mike (www.bigmiketrading.com)
    /// </summary>
    [Description("ADXVMA v2.0")]
    [Gui.Design.DisplayName("ADXVMA (v2.0)")]
    public class ADXVMA : Indicator
    {
        #region Variables
        // Wizard generated variables
		private int aDXPeriod = 6;
        // User defined variables (add any user defined variables below)
		private DataSeries PDI;
		private DataSeries PDM;
		private DataSeries MDM;
		private DataSeries MDI;
		private DataSeries Out;
		private double WeightDM;
		private double WeightDI;
		private double WeightDX;
		private double ChandeEMA;
		private double HHV = double.MinValue;
		private double LLV = double.MaxValue;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Lime), PlotStyle.Line, "ADXVMAPlot"));
            Overlay				= true;
            PriceTypeSupported	= false;
			PDI = new DataSeries( this );
			PDM = new DataSeries( this );
			MDM = new DataSeries( this );
			MDI = new DataSeries( this );
			Out = new DataSeries( this );
			WeightDX = ADXPeriod;
			WeightDM = ADXPeriod;
			WeightDI = ADXPeriod;
			ChandeEMA = ADXPeriod;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if( CurrentBar < 2 )
			{
				ADXVMAPlot.Set( 0 );
				PDM.Set( 0 );
				MDM.Set( 0 );
				PDI.Set( 0 );
				MDI.Set( 0 );
				Out.Set( 0 );
				return;
			}
			try
			{
				int i = 0;
				PDM.Set( 0 );
				MDM.Set( 0 );
				if(Close[i]>Close[i+1])
					PDM.Set( Close[i]-Close[i+1] );//This array is not displayed.
				else
					MDM.Set( Close[i+1]-Close[i] );//This array is not displayed.
				
				PDM.Set(((WeightDM-1)*PDM[i+1] + PDM[i])/WeightDM);//ema.
				MDM.Set(((WeightDM-1)*MDM[i+1] + MDM[i])/WeightDM);//ema.
				
				double TR=PDM[i]+MDM[i];
				
				if (TR>0)
				{
					PDI.Set(PDM[i]/TR);
					MDI.Set(MDM[i]/TR);
				}//Avoid division by zero. Minimum step size is one unnormalized price pip.
				else
				{
					PDI.Set(0);
					MDI.Set(0);
				}
				
				PDI.Set(((WeightDI-1)*PDI[i+1] + PDI[i])/WeightDI);//ema.
				MDI.Set(((WeightDI-1)*MDI[i+1] + MDI[i])/WeightDI);//ema.

				double DI_Diff=PDI[i]-MDI[i];  
				if (DI_Diff<0)
					DI_Diff= -DI_Diff;//Only positive momentum signals are used.
				double DI_Sum=PDI[i]+MDI[i];
				double DI_Factor=0;//Zero case, DI_Diff will also be zero when DI_Sum is zero.
				if (DI_Sum>0)
					Out.Set(DI_Diff/DI_Sum);//Factional, near zero when PDM==MDM (horizonal), near 1 for laddering.
				else
					Out.Set(0);
	
				  Out.Set(((WeightDX-1)*Out[i+1] + Out[i])/WeightDX);
				
				if (Out[i]>Out[i+1])
				{
					HHV=Out[i];
					LLV=Out[i+1];
				}
				else
				{
					HHV=Out[i+1];
					LLV=Out[i];
				}
	
				for(int j=1;j<Math.Min(ADXPeriod,CurrentBar);j++)
				{
					if(Out[i+j+1]>HHV)HHV=Out[i+j+1];
					if(Out[i+j+1]<LLV)LLV=Out[i+j+1];
				}
				
				
				double diff = HHV - LLV;//Veriable reference scale, adapts to recent activity level, unnormalized.
				double VI=0;//Zero case. This fixes the output at its historical level. 
				if (diff>0)
					VI=(Out[i]-LLV)/diff;//Normalized, 0-1 scale.
				
				//   if (VI_0.VIsq_1.VIsqroot_2==1)VI*=VI;
				//   if (VI_0.VIsq_1.VIsqroot_2==2)VI=MathSqrt(VI);
				//   if (VI>VImax)VI=VImax;//Used by Bemac with VImax=0.4, still used in vma1 and affects 5min trend definition.
										//All the ema weight settings, including Chande, affect 5 min trend definition.
				//   if (VI<=zeroVIbelow)VI=0;                    
										
				ADXVMAPlot.Set(((ChandeEMA-VI)*ADXVMAPlot[i+1]+VI*Close[i])/ChandeEMA);//Chande VMA formula with ema built in.
			}
			catch( Exception ex )
			{
				Print( ex.ToString() );
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ADXVMAPlot
        {
            get { return Values[0]; }
        }

        [Description("ADX Period")]
        [Category("Parameters")]
        public int ADXPeriod
        {
            get { return aDXPeriod; }
            set { aDXPeriod = Math.Max(1, value); }
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
        private ADXVMA[] cacheADXVMA = null;

        private static ADXVMA checkADXVMA = new ADXVMA();

        /// <summary>
        /// ADXVMA v2.0
        /// </summary>
        /// <returns></returns>
        public ADXVMA ADXVMA(int aDXPeriod)
        {
            return ADXVMA(Input, aDXPeriod);
        }

        /// <summary>
        /// ADXVMA v2.0
        /// </summary>
        /// <returns></returns>
        public ADXVMA ADXVMA(Data.IDataSeries input, int aDXPeriod)
        {
            if (cacheADXVMA != null)
                for (int idx = 0; idx < cacheADXVMA.Length; idx++)
                    if (cacheADXVMA[idx].ADXPeriod == aDXPeriod && cacheADXVMA[idx].EqualsInput(input))
                        return cacheADXVMA[idx];

            lock (checkADXVMA)
            {
                checkADXVMA.ADXPeriod = aDXPeriod;
                aDXPeriod = checkADXVMA.ADXPeriod;

                if (cacheADXVMA != null)
                    for (int idx = 0; idx < cacheADXVMA.Length; idx++)
                        if (cacheADXVMA[idx].ADXPeriod == aDXPeriod && cacheADXVMA[idx].EqualsInput(input))
                            return cacheADXVMA[idx];

                ADXVMA indicator = new ADXVMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ADXPeriod = aDXPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                ADXVMA[] tmp = new ADXVMA[cacheADXVMA == null ? 1 : cacheADXVMA.Length + 1];
                if (cacheADXVMA != null)
                    cacheADXVMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheADXVMA = tmp;
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
        /// ADXVMA v2.0
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADXVMA ADXVMA(int aDXPeriod)
        {
            return _indicator.ADXVMA(Input, aDXPeriod);
        }

        /// <summary>
        /// ADXVMA v2.0
        /// </summary>
        /// <returns></returns>
        public Indicator.ADXVMA ADXVMA(Data.IDataSeries input, int aDXPeriod)
        {
            return _indicator.ADXVMA(input, aDXPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ADXVMA v2.0
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADXVMA ADXVMA(int aDXPeriod)
        {
            return _indicator.ADXVMA(Input, aDXPeriod);
        }

        /// <summary>
        /// ADXVMA v2.0
        /// </summary>
        /// <returns></returns>
        public Indicator.ADXVMA ADXVMA(Data.IDataSeries input, int aDXPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ADXVMA(input, aDXPeriod);
        }
    }
}
#endregion
