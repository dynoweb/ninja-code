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
    /// Creates a HMA with a rising color and a falling color
    /// </summary>
    [Description("Creates a HMA with a rising color and a falling color")]
    public class HallMaColored : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 50; // Default setting for Period
			private Color upColor			= Color.DarkGreen;
			private Color downColor			= Color.Red;
			private int plot0Width 			= 2;
			private PlotStyle plot0Style	= PlotStyle.Line;
			private DashStyle dash0Style	= DashStyle.Solid;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Gray, PlotStyle.Line, "HmaRising"));
			//PanelUI = 1;
			PlotsConfigurable = false;	
            Overlay				= true;
        }
		
		protected override void OnStartUp()
		{
			Plots[0].Pen.Width = plot0Width;
			Plots[0].PlotStyle = plot0Style;
			Plots[0].Pen.DashStyle = dash0Style;
			Plots[0].Pen.Color = Color.Gray;
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < Period)
				return;
			
			HmaRising.Set(HMA(Period)[0]); 
			
			if (Rising(HMA(Period)))
			{
				PlotColors[0][0] = UpColor;
				//PlotColors[1][0] = Color.DarkGreen;
				//Plots[0].Pen.Width = 2;
				//HmaRising.Set(HMA(Period)[0]); 
				//Values[0].Set(HMA(Period)[0]); 
				//Values[1].Set(HMA(Period)[0] + 100 * TickSize); 
				//Values[1].Reset();
				//PlotColors[0][0] = UpColor;
			} else
			//if (Falling(HMA(Period)))
			{
				PlotColors[0][0] = downColor;
				//PlotColors[1][0] = Color.Red;
				//HmaFalling.Set(HMA(Period)[0]); 
				//Values[0].Set(HMA(Period)[0]); 
				//Values[1].Set(HMA(Period)[0] + 100 * TickSize); 
				//Values[0].Reset();
				//PlotColors[0][0] = DownColor;
			}
			
        }

        #region Properties
		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HmaRising
        {
            get { return Values[0]; }
        }

        //[ Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        //[ XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        //public DataSeries HmaFalling
        //{
        //    get { return Values[1]; }
        //}

		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Uptrend")]
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
		[Description("Select color for downtrend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Downtrend")]
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
		
		[Description("Width for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		[Description("PlotStyle for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Line")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		[Description("DashStyle for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Line")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 

        [Description("Moving Average Period")]
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
        private HallMaColored[] cacheHallMaColored = null;

        private static HallMaColored checkHallMaColored = new HallMaColored();

        /// <summary>
        /// Creates a HMA with a rising color and a falling color
        /// </summary>
        /// <returns></returns>
        public HallMaColored HallMaColored(int period)
        {
            return HallMaColored(Input, period);
        }

        /// <summary>
        /// Creates a HMA with a rising color and a falling color
        /// </summary>
        /// <returns></returns>
        public HallMaColored HallMaColored(Data.IDataSeries input, int period)
        {
            if (cacheHallMaColored != null)
                for (int idx = 0; idx < cacheHallMaColored.Length; idx++)
                    if (cacheHallMaColored[idx].Period == period && cacheHallMaColored[idx].EqualsInput(input))
                        return cacheHallMaColored[idx];

            lock (checkHallMaColored)
            {
                checkHallMaColored.Period = period;
                period = checkHallMaColored.Period;

                if (cacheHallMaColored != null)
                    for (int idx = 0; idx < cacheHallMaColored.Length; idx++)
                        if (cacheHallMaColored[idx].Period == period && cacheHallMaColored[idx].EqualsInput(input))
                            return cacheHallMaColored[idx];

                HallMaColored indicator = new HallMaColored();
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

                HallMaColored[] tmp = new HallMaColored[cacheHallMaColored == null ? 1 : cacheHallMaColored.Length + 1];
                if (cacheHallMaColored != null)
                    cacheHallMaColored.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHallMaColored = tmp;
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
        /// Creates a HMA with a rising color and a falling color
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HallMaColored HallMaColored(int period)
        {
            return _indicator.HallMaColored(Input, period);
        }

        /// <summary>
        /// Creates a HMA with a rising color and a falling color
        /// </summary>
        /// <returns></returns>
        public Indicator.HallMaColored HallMaColored(Data.IDataSeries input, int period)
        {
            return _indicator.HallMaColored(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Creates a HMA with a rising color and a falling color
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HallMaColored HallMaColored(int period)
        {
            return _indicator.HallMaColored(Input, period);
        }

        /// <summary>
        /// Creates a HMA with a rising color and a falling color
        /// </summary>
        /// <returns></returns>
        public Indicator.HallMaColored HallMaColored(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HallMaColored(input, period);
        }
    }
}
#endregion
