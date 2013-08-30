#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region Global Enums

public enum anaBollingerMATypeX {DEMA, EMA, HMA, HoltEMA, LinReg, SMA, SMMA, TEMA, TMA, VWMA, WMA, ZLEMA}

#endregion

//This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
	/// </summary>
	[Description("Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.")]
    public class anaBollingerUniversalX : Indicator
	{
		#region Variables
		private	double numStdDev					= 2.0;
		private double middle						= 0.0;
		private double upper						= 0.0;
		private double lower						= 0.0;
		private double neutralSlope					= 0.0;
		private int period							= 20;
		private int stdDevPeriod					= 20;
		private int threshold						= 30;
		private int index							= 1;
		private int bullish							= 0;
		private int priorBull						= 0;
		private bool showMidband					= true;
		private bool showChannels					= true;
		private bool smoothed						= true;
		private bool useCentralSlope				= false;
		private Color upColor 						= Color.LimeGreen;
		private Color downColor 					= Color.Red;
		private Color neutralColor					= Color.Yellow;
		private int plotBWidth 						= 1;
		private PlotStyle plotBStyle 				= PlotStyle.Line;
		private DashStyle dashBStyle 				= DashStyle.Solid;
		private int plotAWidth 						= 1;
		private PlotStyle plotAStyle 				= PlotStyle.Line;
		private DashStyle dashAStyle 				= DashStyle.Solid;
		private int opacity							= 0;
		private anaBollingerMATypeX selectedMATypeX	= anaBollingerMATypeX.SMA; 
		private IDataSeries average;
		private IDataSeries smoothedAverage;
		private IDataSeries volatility;
		private IDataSeries smoothedVolatility;
		private IDataSeries averageTrueRange;
		private Font textFont						= new Font("Arial", 12);
		private string errorData1					= "HoltEMA not found!";
		private string errorData2					= "SMMA not found!";
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		
		protected override void Initialize()
		{
			Add(new Plot(Color.Gray, "Upper band"));
			Add(new Plot(Color.Gray, "Middle band"));
			Add(new Plot(Color.Gray, "Lower band"));
			Overlay				= true;
			PlotsConfigurable   = false;
		}

		protected override void OnStartUp()
		{
			switch (selectedMATypeX)
			{
				case anaBollingerMATypeX.DEMA: 
					average = DEMA(Input, Period);
					break;
				case anaBollingerMATypeX.EMA: 
					average = EMA(Input, Period);
					break;
				case anaBollingerMATypeX.HMA: 
					average = HMA(Input, Period);
					break;
				case anaBollingerMATypeX.HoltEMA: 
					try
					{
						average = anaHoltEMA(Input, Period, 2*Period);
					}
					catch
					{			
						DrawTextFixed("errortag1", errorData1, TextPosition.Center, ChartControl.AxisColor, textFont, Color.Transparent,Color.Transparent,0);
					}
					break;
				case anaBollingerMATypeX.LinReg: 
					average = LinReg(Input, Period);
					break;
				case anaBollingerMATypeX.SMA: 
					average = SMA(Input, Period);
					break;
				case anaBollingerMATypeX.SMMA: 
					try
					{
						average = anaSMMA(Input, Period);
					}
					catch
					{			
						DrawTextFixed("errortag2", errorData2, TextPosition.Center, ChartControl.AxisColor, textFont, Color.Transparent,Color.Transparent,0);
					}
					break;
				case anaBollingerMATypeX.TEMA: 
					average = TEMA(Input, Period);
					break;
				case anaBollingerMATypeX.TMA: 
					average = TMA(Input, Period);
					break;
				case anaBollingerMATypeX.VWMA: 
					average = VWMA(Input, Period);
					break;
				case anaBollingerMATypeX.WMA: 
					average = WMA(Input, Period);
					break;
				case anaBollingerMATypeX.ZLEMA: 
					average = ZLEMA(Input, Period);
					break;
			}
			smoothedAverage = SMA(average,3);
			volatility = StdDev(Input, StdDevPeriod);
			smoothedVolatility = SMA(volatility,3);
			averageTrueRange = ATR(20);
			Plots[0].Pen.Width = plotBWidth;
			Plots[0].PlotStyle = plotBStyle;
			Plots[0].Pen.DashStyle = dashBStyle;
			Plots[1].Pen.Width= plotAWidth;
			Plots[1].PlotStyle = plotAStyle;
			Plots[1].Pen.DashStyle = dashAStyle;
			Plots[2].Pen.Width = plotBWidth;
			Plots[2].PlotStyle = plotBStyle;
			Plots[2].Pen.DashStyle = dashBStyle;
			index = 1;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double averageValue = 0;
			if (Smoothed)
				averageValue    = smoothedAverage[0];
			else
				averageValue    = average[0];
			double offset = 0;
			if (Smoothed)
				offset = NumStdDev * smoothedVolatility[0];
			else
				offset = NumStdDev * volatility[0];
			
			if(showMidband)
            	Middle.Set(averageValue);
			else
				Middle.Reset();
			if(showChannels)
			{
				Upper.Set(averageValue + offset);
				Lower.Set(averageValue - offset);
			}
			else
			{
				Upper.Reset();
				Lower.Reset();
			}
			
			if (CurrentBar < Math.Max(Period, StdDevPeriod))
				return;
			if(FirstTickOfBar)
			{
            	neutralSlope =  threshold * averageTrueRange[1] / 1000;
				priorBull = bullish;
				middle = Middle[1] + Middle[2];
				upper = Upper[1] + Upper[2];
				lower = Lower[1] + Lower[2];
			}
			
			if(2*averageValue > middle + 3*neutralSlope)
			{
				PlotColors[1][0] = upColor;
				if(useCentralSlope)
				{
					PlotColors[0][0] = UpColor;
					PlotColors[2][0] = UpColor;
				}
				if(priorBull <=0 )
					index = CurrentBar;
				if (Opacity != 0)
					DrawRegion("bollinger" + index, CurrentBar - index + 1, 0, Upper, Lower, Color.Transparent, UpColor, Opacity);
				bullish = 1;
			}
			else if(2*averageValue < middle - 3*neutralSlope)
			{
				PlotColors[1][0] = DownColor;
				if(useCentralSlope)
				{
					PlotColors[0][0] = DownColor;
					PlotColors[2][0] = DownColor;
				}
				if(priorBull >= 0)
					index = CurrentBar;
				if (Opacity != 0)
					DrawRegion("bollinger" + index, CurrentBar - index + 1, 0, Upper, Lower, Color.Transparent, DownColor, Opacity);
				bullish = -1;
			}	
			else
			{
				PlotColors[1][0] = NeutralColor;
				if(useCentralSlope)
				{
					PlotColors[0][0] = NeutralColor;
					PlotColors[2][0] = NeutralColor;
				}
				if(priorBull != 0)
					index = CurrentBar;
				if (Opacity != 0)
					DrawRegion("bollinger" + index, CurrentBar - index + 1, 0, Upper, Lower, Color.Transparent, NeutralColor, Opacity);
				bullish = 0;
			}	
				
			if(!useCentralSlope)
			{	
				if (2*averageValue + 2*offset >= upper + 3*neutralSlope)
					PlotColors[0][0] = UpColor;
				else if (2*averageValue + 2*offset <= upper - 3*neutralSlope)
					PlotColors[0][0] = DownColor;
				else
					PlotColors[0][0] = NeutralColor;
				
				if (2*averageValue - 2*offset >= lower + 3*neutralSlope)
					PlotColors[2][0] = UpColor;
				else if (2*averageValue - 2*offset <= lower - 3*neutralSlope)
					PlotColors[2][0] = DownColor;
				else
					PlotColors[2][0] = NeutralColor;
			}
		}

		#region Properties
		/// <summary>
		/// Get the upper value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// Get the middle value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Middle
		{
			get { return Values[1]; }
		}
		
		/// <summary>
		/// Gets the lower value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[2]; }
		}

		/// <summary>
		/// </summary>
		[Description("Moving Average Type")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Type for MA")]
		public anaBollingerMATypeX SelectedMATypeX
		{
			get { return selectedMATypeX; }
			set { selectedMATypeX = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Number of standard deviations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("# of std. dev.")]
		public double NumStdDev
		{
			get { return numStdDev; }
			set { numStdDev = Math.Max(0, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for MA")]
		public int Period 
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of Bars Used for Calculation of Volatility")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for StdDevBands")]
		public int StdDevPeriod
		{
			get { return stdDevPeriod; }
			set { stdDevPeriod = Math.Max(2, value); }
		}

		/// </summary>
		[Description("Slope as percentage of average true range")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Neutral Threshold")]
		public int Threshold 
		{
			get { return threshold; }
			set { threshold = Math.Max(0, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Option to plot or not to plot the midband")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Show Midband")]
		public bool ShowMidband
		{
			get { return showMidband; }
			set { showMidband = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Option to plot or not to plot the channels")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Show Channels")]
		public bool ShowChannels
		{
			get { return showChannels; }
			set { showChannels = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Smoothing for Center Line and Channels")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Smoothed Channels")]
		public bool Smoothed
		{
			get { return smoothed; }
			set { smoothed = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Use the slope of the midband to color the channels")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Use Midband Slope")]
		public bool UseCentralSlope
		{
			get { return useCentralSlope; }
			set { useCentralSlope = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Select color for Rising Plots")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Plots Rising")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
			set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Select color for Falling Plots")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Plots Falling")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColor); }
			set { downColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Select color for Neutral Plots")]
		[Category("Plots")]
		[Gui.Design.DisplayName("Plots Neutral")]
		public Color NeutralColor
		{
			get { return neutralColor; }
			set { neutralColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string NeutralColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(neutralColor); }
			set { neutralColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Width for Bands.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width Bands")]
		public int PlotBWidth
		{
			get { return plotBWidth; }
			set { plotBWidth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for Bands.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Bands")]
		public PlotStyle PlotBStyle
		{
			get { return plotBStyle; }
			set { plotBStyle = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for Bands.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Bands")]
		public DashStyle DashBStyle
		{
			get { return dashBStyle; }
			set { dashBStyle = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Width for Average.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width Average")]
		public int PlotAWidth
		{
			get { return plotAWidth; }
			set { plotAWidth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for Average.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Average")]
		public PlotStyle PlotAStyle
		{
			get { return plotAStyle; }
			set { plotAStyle = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for Average.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Average")]
		public DashStyle DashAStyle
		{
			get { return dashAStyle; }
			set { dashAStyle = value; }
		} 

		/// <summary>
		/// </summary>
		[Description("Region Opacity")]
		[Category("Plots")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = Math.Max(0, value); }
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
        private anaBollingerUniversalX[] cacheanaBollingerUniversalX = null;

        private static anaBollingerUniversalX checkanaBollingerUniversalX = new anaBollingerUniversalX();

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public anaBollingerUniversalX anaBollingerUniversalX(double numStdDev, int period, anaBollingerMATypeX selectedMATypeX, bool showChannels, bool showMidband, bool smoothed, int stdDevPeriod, int threshold, bool useCentralSlope)
        {
            return anaBollingerUniversalX(Input, numStdDev, period, selectedMATypeX, showChannels, showMidband, smoothed, stdDevPeriod, threshold, useCentralSlope);
        }

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public anaBollingerUniversalX anaBollingerUniversalX(Data.IDataSeries input, double numStdDev, int period, anaBollingerMATypeX selectedMATypeX, bool showChannels, bool showMidband, bool smoothed, int stdDevPeriod, int threshold, bool useCentralSlope)
        {
            if (cacheanaBollingerUniversalX != null)
                for (int idx = 0; idx < cacheanaBollingerUniversalX.Length; idx++)
                    if (Math.Abs(cacheanaBollingerUniversalX[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheanaBollingerUniversalX[idx].Period == period && cacheanaBollingerUniversalX[idx].SelectedMATypeX == selectedMATypeX && cacheanaBollingerUniversalX[idx].ShowChannels == showChannels && cacheanaBollingerUniversalX[idx].ShowMidband == showMidband && cacheanaBollingerUniversalX[idx].Smoothed == smoothed && cacheanaBollingerUniversalX[idx].StdDevPeriod == stdDevPeriod && cacheanaBollingerUniversalX[idx].Threshold == threshold && cacheanaBollingerUniversalX[idx].UseCentralSlope == useCentralSlope && cacheanaBollingerUniversalX[idx].EqualsInput(input))
                        return cacheanaBollingerUniversalX[idx];

            lock (checkanaBollingerUniversalX)
            {
                checkanaBollingerUniversalX.NumStdDev = numStdDev;
                numStdDev = checkanaBollingerUniversalX.NumStdDev;
                checkanaBollingerUniversalX.Period = period;
                period = checkanaBollingerUniversalX.Period;
                checkanaBollingerUniversalX.SelectedMATypeX = selectedMATypeX;
                selectedMATypeX = checkanaBollingerUniversalX.SelectedMATypeX;
                checkanaBollingerUniversalX.ShowChannels = showChannels;
                showChannels = checkanaBollingerUniversalX.ShowChannels;
                checkanaBollingerUniversalX.ShowMidband = showMidband;
                showMidband = checkanaBollingerUniversalX.ShowMidband;
                checkanaBollingerUniversalX.Smoothed = smoothed;
                smoothed = checkanaBollingerUniversalX.Smoothed;
                checkanaBollingerUniversalX.StdDevPeriod = stdDevPeriod;
                stdDevPeriod = checkanaBollingerUniversalX.StdDevPeriod;
                checkanaBollingerUniversalX.Threshold = threshold;
                threshold = checkanaBollingerUniversalX.Threshold;
                checkanaBollingerUniversalX.UseCentralSlope = useCentralSlope;
                useCentralSlope = checkanaBollingerUniversalX.UseCentralSlope;

                if (cacheanaBollingerUniversalX != null)
                    for (int idx = 0; idx < cacheanaBollingerUniversalX.Length; idx++)
                        if (Math.Abs(cacheanaBollingerUniversalX[idx].NumStdDev - numStdDev) <= double.Epsilon && cacheanaBollingerUniversalX[idx].Period == period && cacheanaBollingerUniversalX[idx].SelectedMATypeX == selectedMATypeX && cacheanaBollingerUniversalX[idx].ShowChannels == showChannels && cacheanaBollingerUniversalX[idx].ShowMidband == showMidband && cacheanaBollingerUniversalX[idx].Smoothed == smoothed && cacheanaBollingerUniversalX[idx].StdDevPeriod == stdDevPeriod && cacheanaBollingerUniversalX[idx].Threshold == threshold && cacheanaBollingerUniversalX[idx].UseCentralSlope == useCentralSlope && cacheanaBollingerUniversalX[idx].EqualsInput(input))
                            return cacheanaBollingerUniversalX[idx];

                anaBollingerUniversalX indicator = new anaBollingerUniversalX();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.NumStdDev = numStdDev;
                indicator.Period = period;
                indicator.SelectedMATypeX = selectedMATypeX;
                indicator.ShowChannels = showChannels;
                indicator.ShowMidband = showMidband;
                indicator.Smoothed = smoothed;
                indicator.StdDevPeriod = stdDevPeriod;
                indicator.Threshold = threshold;
                indicator.UseCentralSlope = useCentralSlope;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaBollingerUniversalX[] tmp = new anaBollingerUniversalX[cacheanaBollingerUniversalX == null ? 1 : cacheanaBollingerUniversalX.Length + 1];
                if (cacheanaBollingerUniversalX != null)
                    cacheanaBollingerUniversalX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaBollingerUniversalX = tmp;
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
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaBollingerUniversalX anaBollingerUniversalX(double numStdDev, int period, anaBollingerMATypeX selectedMATypeX, bool showChannels, bool showMidband, bool smoothed, int stdDevPeriod, int threshold, bool useCentralSlope)
        {
            return _indicator.anaBollingerUniversalX(Input, numStdDev, period, selectedMATypeX, showChannels, showMidband, smoothed, stdDevPeriod, threshold, useCentralSlope);
        }

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaBollingerUniversalX anaBollingerUniversalX(Data.IDataSeries input, double numStdDev, int period, anaBollingerMATypeX selectedMATypeX, bool showChannels, bool showMidband, bool smoothed, int stdDevPeriod, int threshold, bool useCentralSlope)
        {
            return _indicator.anaBollingerUniversalX(input, numStdDev, period, selectedMATypeX, showChannels, showMidband, smoothed, stdDevPeriod, threshold, useCentralSlope);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaBollingerUniversalX anaBollingerUniversalX(double numStdDev, int period, anaBollingerMATypeX selectedMATypeX, bool showChannels, bool showMidband, bool smoothed, int stdDevPeriod, int threshold, bool useCentralSlope)
        {
            return _indicator.anaBollingerUniversalX(Input, numStdDev, period, selectedMATypeX, showChannels, showMidband, smoothed, stdDevPeriod, threshold, useCentralSlope);
        }

        /// <summary>
        /// Bollinger Bands are plotted at standard deviation levels above and below a moving average. Since standard deviation is a measure of volatility, the bands are self-adjusting: widening during volatile markets and contracting during calmer periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaBollingerUniversalX anaBollingerUniversalX(Data.IDataSeries input, double numStdDev, int period, anaBollingerMATypeX selectedMATypeX, bool showChannels, bool showMidband, bool smoothed, int stdDevPeriod, int threshold, bool useCentralSlope)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaBollingerUniversalX(input, numStdDev, period, selectedMATypeX, showChannels, showMidband, smoothed, stdDevPeriod, threshold, useCentralSlope);
        }
    }
}
#endregion
