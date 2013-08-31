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
    /// Indicator by Ducman Feb 5, 2013
	/// Very simple indicator that plots the first and second entries.
	/// Zero top needs to be above EMA. 
	/// Zero bottom needs to be below EMA.
    /// </summary>
    [Description("Plots Second Entries")]
    public class SecondEntries : Indicator
    {
        #region Variables
        private int 	timeStart 		= 144500; 	// Default setting for StartTime
        private int 	timeStop 		= 171500; 	// Default setting for StopTime
		private int 	periodEMA 		= 21; 		// Default setting for EMA period
		
		private bool	highZero		= false;	// Variable to set high 0
		private bool	highOne			= false;	// Variable to set high 1
		private bool	lowZero			= false;	// Variable to set Low 0
		private bool	lowOne			= false;	// Variable to set Low 1
		private bool	top				= false;	// Variable to set top
		private bool	bottom			= false;	// Variable to set bottom
		private bool	aboveEMA		= false;	// Variable to set above EMA
		private bool	belowEMA		= false;	// Variable to set below EMA
		
		private double	highZeroPrice	= 0;		// Variable to store high 0
		private double	highOnePrice	= 0;		// Variable to store high 1
		private double	lowZeroPrice	= 0;		// Variable to store low 0
		private double	lowOnePrice		= 0;		// Variable to store low 1
		private double	topPrice		= 0;		// Variable to store new top
		private double	bottomPrice		= 0;		// Variable to store new bottom
        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose = true;             
			Add(new Plot(Color.Orange, "EMA"));
			Overlay	= true;
			ClearOutputWindow();
        }

        protected override void OnBarUpdate()
        {	
			
		Value.Set(EMA(periodEMA)[0]);	
			
		#region Trading Time	
		// Halt strategy until # bars is > EMA period			
		if (CurrentBar < periodEMA) return;
		
		// Trading time / Change Background color
		if ((ToTime(Time[0]) <= timeStart || ToTime(Time[0]) >= timeStop)) BackColorAll = Color.Gray;		
		#endregion
		
		#region Tops, bottoms and EMA
		// Determine top and bottom			
		if (High[0] <= High[1] && High[1] >= High[2]) 
		{
			top = true; 
			topPrice = High[1];
		}
		else top = false;
	
		if (Low[0] >= Low[1] && Low[1] <= Low[2]) 
		{
			bottom = true;
			bottomPrice = Low[1];
		}
		else bottom = false;
		
		// Above or below EMA
		if (High[0] > EMA(periodEMA)[0] && High[1] > EMA(periodEMA)[1] && High[2] > EMA(periodEMA)[2]) 
		{
			aboveEMA = true; 
		}
		else aboveEMA = false;
		
		if (Low[0] < EMA(periodEMA)[0] && Low[1] < EMA(periodEMA)[1] && Low[2] < EMA(periodEMA)[2]) 
 		{
			belowEMA = true; 
		}
		else belowEMA = false;
		#endregion
		
		 							
		//#region Second entries
		// Reset highs during trading
		if (High[0] >= highZeroPrice) // higher high 0
		{
			highZeroPrice = High[0]; 	
			highOnePrice = High[0];
			highZero = false;
			highOne = false;
		}				
		else if (High[0] < highZeroPrice && High[0] >= highOnePrice) // higher high 1
		{
			highOnePrice = High[0]; 
		}
		// Reset lows during trading
		if (Low[0] <= lowZeroPrice) // lower low 0
		{
			lowZeroPrice = Low[0]; 
			lowOnePrice = Low[0];
			lowZero = false;
			lowOne = false;
		}
		else if (Low[0] > lowZeroPrice && Low[0] <= lowOnePrice) // lower low 1
		{
			lowOnePrice = Low[0]; 
		}
			
		// Explore new tops
		if (top && !highZero && !highOne && aboveEMA) // new top 0 above EMA		
		{
			highZero = true; 
			highZeroPrice = topPrice;
			DrawText("FirstEntry" + CurrentBar, "1", 1, High[1] + 20 * TickSize, Color.Blue);
		}
		else if (top && highZero && !highOne && High[1] < highZeroPrice) // new top 1
		{
			highOne = true; 
			highOnePrice = topPrice;
			DrawText("SecondEntry" + CurrentBar, "2", 1, High[1] + 20 * TickSize, Color.Blue);
			DrawText("EnterPrice" + CurrentBar, "---", 1, High[1] + 5 * TickSize, Color.Blue);
		}	
		else if (top && highZero && highOne) // failed two leg move
		{
			highZero = false;
			highOne = false;
			highZeroPrice = topPrice;
		}	
				
		// Explore new bottoms
		if (bottom && !lowZero && !lowOne && belowEMA) // new bottom 0 below EMA		
		{
			lowZero = true; 
			lowZeroPrice = bottomPrice;
			DrawText("FirstEntry" + CurrentBar, "1", 1, Low[1] - 20 * TickSize, Color.Red);					
		}
		else if (bottom && lowZero && !lowOne && Low[1] > lowZeroPrice) // new bottom 1
		{
			lowOne = true; 
			lowOnePrice = bottomPrice;
			DrawText("SecondEntry" + CurrentBar, "2", 1, Low[1] - 20 * TickSize, Color.Red);
			DrawText("EnterPrice" + CurrentBar, "---", 1, Low[1] - 1 * TickSize, Color.Red);
		}	
		else if (bottom && lowZero && lowOne) // failed two leg move
		{
			lowZero = false;
			lowOne = false;
			lowZeroPrice = bottomPrice;					
		}	
		//#endregion
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Start
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Stop
        {
            get { return Values[1]; }
        }

		[Description("EMA Period")]
        [GridCategory("Parameters")]
        public int PeriodEMA
        {
            get { return periodEMA; }
            set { periodEMA = Math.Max(1, value); }
        }
		
        [Description("Start Time")]
        [GridCategory("Parameters")]
        public int TimeStart
        {
            get { return timeStart; }
            set { timeStart = Math.Max(1, value); }
        }

        [Description("Stop Time")]
        [GridCategory("Parameters")]
        public int TimeStop
        {
            get { return timeStop; }
            set { timeStop = Math.Max(1, value); }
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
        private SecondEntries[] cacheSecondEntries = null;

        private static SecondEntries checkSecondEntries = new SecondEntries();

        /// <summary>
        /// Plots Second Entries
        /// </summary>
        /// <returns></returns>
        public SecondEntries SecondEntries(int periodEMA, int timeStart, int timeStop)
        {
            return SecondEntries(Input, periodEMA, timeStart, timeStop);
        }

        /// <summary>
        /// Plots Second Entries
        /// </summary>
        /// <returns></returns>
        public SecondEntries SecondEntries(Data.IDataSeries input, int periodEMA, int timeStart, int timeStop)
        {
            if (cacheSecondEntries != null)
                for (int idx = 0; idx < cacheSecondEntries.Length; idx++)
                    if (cacheSecondEntries[idx].PeriodEMA == periodEMA && cacheSecondEntries[idx].TimeStart == timeStart && cacheSecondEntries[idx].TimeStop == timeStop && cacheSecondEntries[idx].EqualsInput(input))
                        return cacheSecondEntries[idx];

            lock (checkSecondEntries)
            {
                checkSecondEntries.PeriodEMA = periodEMA;
                periodEMA = checkSecondEntries.PeriodEMA;
                checkSecondEntries.TimeStart = timeStart;
                timeStart = checkSecondEntries.TimeStart;
                checkSecondEntries.TimeStop = timeStop;
                timeStop = checkSecondEntries.TimeStop;

                if (cacheSecondEntries != null)
                    for (int idx = 0; idx < cacheSecondEntries.Length; idx++)
                        if (cacheSecondEntries[idx].PeriodEMA == periodEMA && cacheSecondEntries[idx].TimeStart == timeStart && cacheSecondEntries[idx].TimeStop == timeStop && cacheSecondEntries[idx].EqualsInput(input))
                            return cacheSecondEntries[idx];

                SecondEntries indicator = new SecondEntries();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodEMA = periodEMA;
                indicator.TimeStart = timeStart;
                indicator.TimeStop = timeStop;
                Indicators.Add(indicator);
                indicator.SetUp();

                SecondEntries[] tmp = new SecondEntries[cacheSecondEntries == null ? 1 : cacheSecondEntries.Length + 1];
                if (cacheSecondEntries != null)
                    cacheSecondEntries.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSecondEntries = tmp;
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
        /// Plots Second Entries
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SecondEntries SecondEntries(int periodEMA, int timeStart, int timeStop)
        {
            return _indicator.SecondEntries(Input, periodEMA, timeStart, timeStop);
        }

        /// <summary>
        /// Plots Second Entries
        /// </summary>
        /// <returns></returns>
        public Indicator.SecondEntries SecondEntries(Data.IDataSeries input, int periodEMA, int timeStart, int timeStop)
        {
            return _indicator.SecondEntries(input, periodEMA, timeStart, timeStop);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots Second Entries
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SecondEntries SecondEntries(int periodEMA, int timeStart, int timeStop)
        {
            return _indicator.SecondEntries(Input, periodEMA, timeStart, timeStop);
        }

        /// <summary>
        /// Plots Second Entries
        /// </summary>
        /// <returns></returns>
        public Indicator.SecondEntries SecondEntries(Data.IDataSeries input, int periodEMA, int timeStart, int timeStop)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SecondEntries(input, periodEMA, timeStart, timeStop);
        }
    }
}
#endregion
