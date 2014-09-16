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
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class RicksChartLabel : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
		
			Font labelFont = new Font("Arial", 12, FontStyle.Bold);
			Color	bgcolor		= Color.Blue;
			Color	fgcolor		= Color.Yellow;
			int		opacity		= 25;
			TextPosition tpos   = TextPosition.BottomLeft;
		
			string text = "label text";
						
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
//			Label = Name + " " + Instrument.FullName +				
//				"\n" + Instrument.MasterInstrument.Description +
//				"\nTickSize: " + Instrument.MasterInstrument.TickSize +
//				" TickValue: $" + Instrument.MasterInstrument.PointValue * Instrument.MasterInstrument.TickSize; 
			
			Name = "";	
            CalculateOnBarClose	= true;
			Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
		{
			DrawTextFixed("RicksChartLabelTag1", Text, TPos, FGColor, LabelFont, Color.Empty, BGColor, Opacity);
		}

			#region Properties
			
			[XmlIgnore()]
			[Description("Font info")]
			[Gui.Design.DisplayNameAttribute("01. Font")]
			[Category("Parameters")]
			public Font LabelFont
			{
				get { return labelFont; }
				set { labelFont = value; }
			}
			
			[Browsable(false)]
			public string DrawFontSerialize
			{
				get { return NinjaTrader.Gui.Design.SerializableFont.ToString(labelFont); }
				set { labelFont = NinjaTrader.Gui.Design.SerializableFont.FromString(value); }
				
			}
			
			[Description("Position")]
			[Gui.Design.DisplayNameAttribute("02. Position")]
			[Category("Parameters")]
			public TextPosition TPos
			{
				get { return tpos; }
				set { tpos = value; }
			}
			
			[XmlIgnore()]
			[Description("Background Color")]
			[Category("Parameters")]
			[Gui.Design.DisplayNameAttribute("03. BG color")]
			public Color BGColor
			{
				get { return bgcolor; }
				set { bgcolor = value; }
			}
			

			[Browsable(false)]
			public string BGColorSerialize
			{
				get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bgcolor); }
				set { bgcolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}
			
			[XmlIgnore()]
			[Description("Foreground Color")]
			[Category("Parameters")]
			[Gui.Design.DisplayNameAttribute("04. FG color")]
			public Color FGColor
			{
				get { return fgcolor; }
				set { fgcolor = value; }
			}

			[Browsable(false)]
			public string FGColorSerialize
			{
				get { return NinjaTrader.Gui.Design.SerializableColor.ToString(fgcolor); }
				set { fgcolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
			}
			
			[Description("Opacity, 0-10")]
			[Gui.Design.DisplayNameAttribute("05. Opacity")]
			[Category("Parameters")]
			public int Opacity
			{
				get { return opacity; }
				set { opacity = Math.Max(0, Math.Min(10, value)); }
			}
			
			[Description("The chart label text")]
			[Gui.Design.DisplayNameAttribute("06. Label Text")]
			[Category("Parameters")]
			public string Text
			{
				get { return text; }
				set { text = value; }
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
        private RicksChartLabel[] cacheRicksChartLabel = null;

        private static RicksChartLabel checkRicksChartLabel = new RicksChartLabel();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public RicksChartLabel RicksChartLabel(Color bGColor, Color fGColor, Font labelFont, int opacity, string text, TextPosition tPos)
        {
            return RicksChartLabel(Input, bGColor, fGColor, labelFont, opacity, text, tPos);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public RicksChartLabel RicksChartLabel(Data.IDataSeries input, Color bGColor, Color fGColor, Font labelFont, int opacity, string text, TextPosition tPos)
        {
            if (cacheRicksChartLabel != null)
                for (int idx = 0; idx < cacheRicksChartLabel.Length; idx++)
                    if (cacheRicksChartLabel[idx].BGColor == bGColor && cacheRicksChartLabel[idx].FGColor == fGColor && cacheRicksChartLabel[idx].LabelFont == labelFont && cacheRicksChartLabel[idx].Opacity == opacity && cacheRicksChartLabel[idx].Text == text && cacheRicksChartLabel[idx].TPos == tPos && cacheRicksChartLabel[idx].EqualsInput(input))
                        return cacheRicksChartLabel[idx];

            lock (checkRicksChartLabel)
            {
                checkRicksChartLabel.BGColor = bGColor;
                bGColor = checkRicksChartLabel.BGColor;
                checkRicksChartLabel.FGColor = fGColor;
                fGColor = checkRicksChartLabel.FGColor;
                checkRicksChartLabel.LabelFont = labelFont;
                labelFont = checkRicksChartLabel.LabelFont;
                checkRicksChartLabel.Opacity = opacity;
                opacity = checkRicksChartLabel.Opacity;
                checkRicksChartLabel.Text = text;
                text = checkRicksChartLabel.Text;
                checkRicksChartLabel.TPos = tPos;
                tPos = checkRicksChartLabel.TPos;

                if (cacheRicksChartLabel != null)
                    for (int idx = 0; idx < cacheRicksChartLabel.Length; idx++)
                        if (cacheRicksChartLabel[idx].BGColor == bGColor && cacheRicksChartLabel[idx].FGColor == fGColor && cacheRicksChartLabel[idx].LabelFont == labelFont && cacheRicksChartLabel[idx].Opacity == opacity && cacheRicksChartLabel[idx].Text == text && cacheRicksChartLabel[idx].TPos == tPos && cacheRicksChartLabel[idx].EqualsInput(input))
                            return cacheRicksChartLabel[idx];

                RicksChartLabel indicator = new RicksChartLabel();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BGColor = bGColor;
                indicator.FGColor = fGColor;
                indicator.LabelFont = labelFont;
                indicator.Opacity = opacity;
                indicator.Text = text;
                indicator.TPos = tPos;
                Indicators.Add(indicator);
                indicator.SetUp();

                RicksChartLabel[] tmp = new RicksChartLabel[cacheRicksChartLabel == null ? 1 : cacheRicksChartLabel.Length + 1];
                if (cacheRicksChartLabel != null)
                    cacheRicksChartLabel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRicksChartLabel = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RicksChartLabel RicksChartLabel(Color bGColor, Color fGColor, Font labelFont, int opacity, string text, TextPosition tPos)
        {
            return _indicator.RicksChartLabel(Input, bGColor, fGColor, labelFont, opacity, text, tPos);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.RicksChartLabel RicksChartLabel(Data.IDataSeries input, Color bGColor, Color fGColor, Font labelFont, int opacity, string text, TextPosition tPos)
        {
            return _indicator.RicksChartLabel(input, bGColor, fGColor, labelFont, opacity, text, tPos);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RicksChartLabel RicksChartLabel(Color bGColor, Color fGColor, Font labelFont, int opacity, string text, TextPosition tPos)
        {
            return _indicator.RicksChartLabel(Input, bGColor, fGColor, labelFont, opacity, text, tPos);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.RicksChartLabel RicksChartLabel(Data.IDataSeries input, Color bGColor, Color fGColor, Font labelFont, int opacity, string text, TextPosition tPos)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RicksChartLabel(input, bGColor, fGColor, labelFont, opacity, text, tPos);
        }
    }
}
#endregion
