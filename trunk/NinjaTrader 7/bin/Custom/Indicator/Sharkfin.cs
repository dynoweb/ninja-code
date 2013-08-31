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
    /// v1_1	7/15/09 - zeller4 - set up dots to plot after signal has a second occurrence
	/// 		7/28/09 - zeller4 - colors per Sharky settings
	/// 		7/29/09 - zeller4 - drawdots default off - selectable - "Zeller_Sharky_Fin"
    /// </summary>
    [Description("_Sharkfin - Barbara Star - PhD")]
    public class _Sharkfin : Indicator
    {
        #region Variables
            private int length = 6;//28;//5; // Default setting for Length
		
			private DataSeries LRReversal;
			
			private DataSeries lRR;
			private DataSeries signal;
			
			private bool	drawDots = false;
			private bool	drawLines = false;
			
		
			#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			
//          Add(new Plot(new Pen(Color.DarkOrange, 2), PlotStyle.Line, "LRR"));
			Add(new Plot(new Pen(Color.Blue,4), PlotStyle.Line, "Rising"));
            Add(new Plot(new Pen(Color.Red,4), PlotStyle.Line, "Falling"));
            Add(new Plot(new Pen(Color.Yellow,2), PlotStyle.Line, "Neutral"));
			
            CalculateOnBarClose	= false;
            DrawOnPricePanel 	= false;
			AutoScale			= false;
            Overlay				= false;
            PriceTypeSupported	= true;
			DisplayInDataBox	= false;
			
			
//			Pen BlackPen = new Pen(Color.Black, 2);
//			Add(new Line(BlackPen, 0, "Zero"));
			
			LRReversal = new DataSeries(this);
			lRR = new DataSeries(this);
			signal = new DataSeries(this);
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar<length)
				return;
			
			
			LRReversal.Set(LinReg(Close,Length)[0]);
			lRR.Set(LinReg(Close,Length)[0]);
			if (LRReversal[0] >= LRReversal[1])
			{
				lRR[0] = 1;
				
			}
			else
			{
				lRR[0] = -1;
			}
			
			
			bool rising = false;
			bool falling = false;
				
			if (lRR[0] == 1)//> lRR[1])
				{ 
					Rising.Set(lRR[0]); //Rising.Set(1, lRR[1]);
					rising = true;
					
//					if (ColorBars)
//					{ CandleOutlineColor = BarColorOutline; BarColor = BarColorUp; }
				}
//				else 
				if (lRR[0] == -1)//< lRR[1])
				{ 
					Falling.Set(lRR[0]); //Falling.Set(1, lRR[1]);
					falling = true;
					
//					if (ColorBars)
//					{ CandleOutlineColor = BarColorOutline; BarColor = BarColorDown; }
				}

				//else
				if //((lRR[0] > lRR[1]) || (lRR[0] < lRR[1]))
					((lRR[1] == 1 && lRR[0] == -1)|| (lRR[1] == -1 && lRR[0] == 1))
				{ 
					Neutral.Set(lRR[0]); 
					Neutral.Set(1, lRR[1]); 
					rising = false; falling = false;
					
//					if (ColorBars)
//					{ CandleOutlineColor = BarColorOutline; BarColor = BarColorNeutral; }
				}
			
			
			
///////////////////////////////////////////////////////////////////////////////////////////////
			/// draw dot at second instance of +1 or -1
			///////////////////////////////////////////////////////////////////////////////////
		if(drawDots)
		{
			if(LRR[2] == -1 &&
				LRR[1] == 1 &&
				LRR[0] == 1
				)
			{
					DrawDot("Dot" + CurrentBar,false,0,0,Color.Blue);
				signal.Set(1);
			}
			else if(LRR[2] == 1 &&
				LRR[1] == -1 &&
				LRR[0] == -1
				)
			{
					DrawDot("Dot" + CurrentBar,false,0,0,Color.Red);
				signal.Set(-1);
			}
			else
			{
				RemoveDrawObject("Dot");
			}
		}
		
			if (signal[1] != 1 &&
				signal[0] == 1)
			{
				if(DrawLines)
				{
					DrawVerticalLine("tag1l"+CurrentBar,0,Color.Blue,DashStyle.DashDot,3);
					//DrawDot("tag1d"+CurrentBar,false,0,Low[0] - 8*TickSize,Color.Blue);
				}
			}
			if (signal[1] != -1 &&
				signal[0] == -1)
			{
				if(DrawLines)
				{
					DrawVerticalLine("tag2l"+CurrentBar,0,Color.Red,DashStyle.DashDot,3);
					//DrawDot("tag2d"+CurrentBar,false,0,High[0] + 8*TickSize,Color.Red);
				}
			}
		
			
		}
		
		
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LRR
        {
            get { return lRR; }
        }
		[Browsable(false)]	//
        [XmlIgnore()]		// 
        public DataSeries Rising
        {
            get { return Values[0]; }
        }
		[Browsable(false)]	// .
		[XmlIgnore()]		// 
        public DataSeries Falling
        {
            get { return Values[1]; }
        }
		[Browsable(false)]	// ..
        [XmlIgnore()]		// 
        public DataSeries Neutral
        {
            get { return Values[2]; }
        }
        
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Signal
		{
		get { return signal; }
		}
        
 
        [Description("Linear Regression Length")]
        [Category("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
        }
		[Description("Draw Dots?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("01. Draw Dots?")]
        public bool DrawDots
        {
            get { return drawDots; }
            set { drawDots = value; }
        }
		[Description("Draw Lines on Price Panel?")]
        [Category("Visual")]
        [Gui.Design.DisplayName("02. Draw Vertical Lines with Dots?")]
        public bool DrawLines
        {
            get { return drawLines; }
            set { drawLines = value; }
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
        private _Sharkfin[] cache_Sharkfin = null;

        private static _Sharkfin check_Sharkfin = new _Sharkfin();

        /// <summary>
        /// _Sharkfin - Barbara Star - PhD
        /// </summary>
        /// <returns></returns>
        public _Sharkfin _Sharkfin(int length)
        {
            return _Sharkfin(Input, length);
        }

        /// <summary>
        /// _Sharkfin - Barbara Star - PhD
        /// </summary>
        /// <returns></returns>
        public _Sharkfin _Sharkfin(Data.IDataSeries input, int length)
        {
            if (cache_Sharkfin != null)
                for (int idx = 0; idx < cache_Sharkfin.Length; idx++)
                    if (cache_Sharkfin[idx].Length == length && cache_Sharkfin[idx].EqualsInput(input))
                        return cache_Sharkfin[idx];

            lock (check_Sharkfin)
            {
                check_Sharkfin.Length = length;
                length = check_Sharkfin.Length;

                if (cache_Sharkfin != null)
                    for (int idx = 0; idx < cache_Sharkfin.Length; idx++)
                        if (cache_Sharkfin[idx].Length == length && cache_Sharkfin[idx].EqualsInput(input))
                            return cache_Sharkfin[idx];

                _Sharkfin indicator = new _Sharkfin();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                _Sharkfin[] tmp = new _Sharkfin[cache_Sharkfin == null ? 1 : cache_Sharkfin.Length + 1];
                if (cache_Sharkfin != null)
                    cache_Sharkfin.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cache_Sharkfin = tmp;
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
        /// _Sharkfin - Barbara Star - PhD
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator._Sharkfin _Sharkfin(int length)
        {
            return _indicator._Sharkfin(Input, length);
        }

        /// <summary>
        /// _Sharkfin - Barbara Star - PhD
        /// </summary>
        /// <returns></returns>
        public Indicator._Sharkfin _Sharkfin(Data.IDataSeries input, int length)
        {
            return _indicator._Sharkfin(input, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// _Sharkfin - Barbara Star - PhD
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator._Sharkfin _Sharkfin(int length)
        {
            return _indicator._Sharkfin(Input, length);
        }

        /// <summary>
        /// _Sharkfin - Barbara Star - PhD
        /// </summary>
        /// <returns></returns>
        public Indicator._Sharkfin _Sharkfin(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator._Sharkfin(input, length);
        }
    }
}
#endregion
