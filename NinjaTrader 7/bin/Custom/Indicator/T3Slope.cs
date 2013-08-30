#region Using declarations
using System;
using System.Collections;
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
    /// T3Slope Moving Average
    /// </summary>
    [Description("T3Slope Moving Average")]
    public class T3Slope : Indicator
    {
        #region Variables

        private double 			vFactor 		= 0.7; // Default setting for VFactor
		private int 			tCount 			= 3;
		private int 			period 			= 14;
		private bool			candles 		= true;
		private bool			paintBars		= false;
		private Color			upColor			= Color.DeepSkyBlue;
		private Color			downColor		= Color.OrangeRed;
		private int				plot0Width		= 2;
		private PlotStyle		plot0Style		= PlotStyle.Line;
		private DashStyle		dash0Style		= DashStyle.Solid;
		private ArrayList 		seriesCollection;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "T3Slope"));
            Overlay				= true;
 			PlotsConfigurable	= false;
       }

		/// <summary>
		/// </summary>
		protected override void OnStartUp()
		{
			candles = false;
			if (ChartControl != null && ChartControl.ChartStyleType == ChartStyleType.CandleStick)
				candles = true;
			Plots[0].Pen.Width 		= plot0Width;
			Plots[0].PlotStyle		= plot0Style;
			Plots[0].Pen.DashStyle 	= dash0Style;
		}
		
		/// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (TCount == 1)
			{
				CalculateGD(Inputs[0], Values[0]);
				return;
			}
			if (seriesCollection == null)
			{
				seriesCollection = new System.Collections.ArrayList();
				for (int i = 0; i < TCount - 1; i++) 
					seriesCollection.Add(new DataSeries(this));
			}
			CalculateGD(Inputs[0], (DataSeries) seriesCollection[0]);
			for (int i = 0; i <= seriesCollection.Count - 2; i++) 
				CalculateGD((DataSeries) seriesCollection[i], (DataSeries) seriesCollection[i + 1]);
			
			CalculateGD((DataSeries) seriesCollection[seriesCollection.Count - 1], Values[0]);
			
			if(CurrentBar > 0)
			{
				if (Rising(Values[0]))
					PlotColors[0][0] = upColor;
				else if(Falling(Values[0])) 
					PlotColors[0][0] = downColor;
				else
					PlotColors[0][0] = PlotColors[0][1];
			
				if(PaintBars)
				{
					if (Rising(Values[0]))
					{
						CandleOutlineColorSeries[0] = upColor;
						BarColor = upColor;
					}
					else if(Falling(Values[0])) 
					{
						CandleOutlineColorSeries[0] = downColor;
						BarColor  = downColor;
					}
					else
					{
						CandleOutlineColorSeries[0] = CandleOutlineColorSeries[1];
						BarColor = CandleOutlineColorSeries[1];
					}	
					if(Open[0] < Close[0] && candles) 
						BarColor  = Color.Transparent;
				}
			}
		}
		
		private void CalculateGD(IDataSeries input, DataSeries output)
		{
			output.Set((EMA(input, Period)[0] * (1 + VFactor)) - (EMA(EMA(input, Period), Period)[0] * VFactor));
		}
		
        #region Properties
		[Description("Numbers of bars used for calculations")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
		[Description("The smooth count")]
        [GridCategory("Parameters")]
        public int TCount
        {
            get { return tCount; }
            set { tCount = Math.Max(1, value); }
        }

        [Description("VFactor")]
        [GridCategory("Parameters")]
        public double VFactor
        {
            get { return vFactor; }
            set { vFactor = Math.Max(0, value); }
        }
 
		[Description("Color the bars in the direction of the trend?")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName ("Paint Bars")]
        public bool PaintBars
        {
            get { return paintBars; }
            set { paintBars = value; }
        }

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Select color for rising T3")]
		[Category("Plots")]
		[Gui.Design.DisplayName("T3 Rising")]
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
		[XmlIgnore()]
		[Description("Select color for falling T3")]
		[Category("Plots")]
		[Gui.Design.DisplayName("T3 Falling")]
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
		[Description("Width for T3 Line.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("DashStyle for T3 Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for T3 Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
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
        private T3Slope[] cacheT3Slope = null;

        private static T3Slope checkT3Slope = new T3Slope();

        /// <summary>
        /// T3Slope Moving Average
        /// </summary>
        /// <returns></returns>
        public T3Slope T3Slope(bool paintBars, int period, int tCount, double vFactor)
        {
            return T3Slope(Input, paintBars, period, tCount, vFactor);
        }

        /// <summary>
        /// T3Slope Moving Average
        /// </summary>
        /// <returns></returns>
        public T3Slope T3Slope(Data.IDataSeries input, bool paintBars, int period, int tCount, double vFactor)
        {
            if (cacheT3Slope != null)
                for (int idx = 0; idx < cacheT3Slope.Length; idx++)
                    if (cacheT3Slope[idx].PaintBars == paintBars && cacheT3Slope[idx].Period == period && cacheT3Slope[idx].TCount == tCount && Math.Abs(cacheT3Slope[idx].VFactor - vFactor) <= double.Epsilon && cacheT3Slope[idx].EqualsInput(input))
                        return cacheT3Slope[idx];

            lock (checkT3Slope)
            {
                checkT3Slope.PaintBars = paintBars;
                paintBars = checkT3Slope.PaintBars;
                checkT3Slope.Period = period;
                period = checkT3Slope.Period;
                checkT3Slope.TCount = tCount;
                tCount = checkT3Slope.TCount;
                checkT3Slope.VFactor = vFactor;
                vFactor = checkT3Slope.VFactor;

                if (cacheT3Slope != null)
                    for (int idx = 0; idx < cacheT3Slope.Length; idx++)
                        if (cacheT3Slope[idx].PaintBars == paintBars && cacheT3Slope[idx].Period == period && cacheT3Slope[idx].TCount == tCount && Math.Abs(cacheT3Slope[idx].VFactor - vFactor) <= double.Epsilon && cacheT3Slope[idx].EqualsInput(input))
                            return cacheT3Slope[idx];

                T3Slope indicator = new T3Slope();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PaintBars = paintBars;
                indicator.Period = period;
                indicator.TCount = tCount;
                indicator.VFactor = vFactor;
                Indicators.Add(indicator);
                indicator.SetUp();

                T3Slope[] tmp = new T3Slope[cacheT3Slope == null ? 1 : cacheT3Slope.Length + 1];
                if (cacheT3Slope != null)
                    cacheT3Slope.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheT3Slope = tmp;
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
        /// T3Slope Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.T3Slope T3Slope(bool paintBars, int period, int tCount, double vFactor)
        {
            return _indicator.T3Slope(Input, paintBars, period, tCount, vFactor);
        }

        /// <summary>
        /// T3Slope Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.T3Slope T3Slope(Data.IDataSeries input, bool paintBars, int period, int tCount, double vFactor)
        {
            return _indicator.T3Slope(input, paintBars, period, tCount, vFactor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// T3Slope Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.T3Slope T3Slope(bool paintBars, int period, int tCount, double vFactor)
        {
            return _indicator.T3Slope(Input, paintBars, period, tCount, vFactor);
        }

        /// <summary>
        /// T3Slope Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.T3Slope T3Slope(Data.IDataSeries input, bool paintBars, int period, int tCount, double vFactor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.T3Slope(input, paintBars, period, tCount, vFactor);
        }
    }
}
#endregion
