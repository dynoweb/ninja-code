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
    /// November 28, 2009 by Big Mike (www.bigmiketrading.com)
    /// </summary>
    [Description("BMT Chart Label v1.01 - VIP Only - www.bigmiketrading.com")]
    public class BMTChartLabel : Indicator
    {
        private Font	drawfont	= new Font("Arial", 12, FontStyle.Bold);
		private TextPosition tpos	= TextPosition.TopLeft;
		private Color	bgcolor		= Color.Blue;
		private Color	fgcolor		= Color.White;
		private int		opacity		= 5;
		private string	text		= "Chart Label Text Here";
		
        protected override void Initialize()
        {
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			this.Name			= "";
        }

      
        protected override void OnBarUpdate()
        {
            DrawTextFixed("BMT Label", Text, TPos, FGColor, DrawFont, Color.Empty, BGColor, Opacity);
        }

        #region Properties
        [XmlIgnore()]
		[Description("Font info")]
		[Gui.Design.DisplayNameAttribute("01. Font")]
        [Category("Parameters")]
        public Font DrawFont
        {
            get { return drawfont; }
            set { drawfont = value; }
        }
		[Browsable(false)]
        public string DrawFontSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableFont.ToString(drawfont); }
            set { drawfont = NinjaTrader.Gui.Design.SerializableFont.FromString(value); }
			
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
		[Gui.Design.DisplayNameAttribute("06. Text")]
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
        private BMTChartLabel[] cacheBMTChartLabel = null;

        private static BMTChartLabel checkBMTChartLabel = new BMTChartLabel();

        /// <summary>
        /// BMT Chart Label v1.01 - VIP Only - www.bigmiketrading.com
        /// </summary>
        /// <returns></returns>
        public BMTChartLabel BMTChartLabel(Color bGColor, Font drawFont, Color fGColor, int opacity, string text, TextPosition tPos)
        {
            return BMTChartLabel(Input, bGColor, drawFont, fGColor, opacity, text, tPos);
        }

        /// <summary>
        /// BMT Chart Label v1.01 - VIP Only - www.bigmiketrading.com
        /// </summary>
        /// <returns></returns>
        public BMTChartLabel BMTChartLabel(Data.IDataSeries input, Color bGColor, Font drawFont, Color fGColor, int opacity, string text, TextPosition tPos)
        {
            if (cacheBMTChartLabel != null)
                for (int idx = 0; idx < cacheBMTChartLabel.Length; idx++)
                    if (cacheBMTChartLabel[idx].BGColor == bGColor && cacheBMTChartLabel[idx].DrawFont == drawFont && cacheBMTChartLabel[idx].FGColor == fGColor && cacheBMTChartLabel[idx].Opacity == opacity && cacheBMTChartLabel[idx].Text == text && cacheBMTChartLabel[idx].TPos == tPos && cacheBMTChartLabel[idx].EqualsInput(input))
                        return cacheBMTChartLabel[idx];

            lock (checkBMTChartLabel)
            {
                checkBMTChartLabel.BGColor = bGColor;
                bGColor = checkBMTChartLabel.BGColor;
                checkBMTChartLabel.DrawFont = drawFont;
                drawFont = checkBMTChartLabel.DrawFont;
                checkBMTChartLabel.FGColor = fGColor;
                fGColor = checkBMTChartLabel.FGColor;
                checkBMTChartLabel.Opacity = opacity;
                opacity = checkBMTChartLabel.Opacity;
                checkBMTChartLabel.Text = text;
                text = checkBMTChartLabel.Text;
                checkBMTChartLabel.TPos = tPos;
                tPos = checkBMTChartLabel.TPos;

                if (cacheBMTChartLabel != null)
                    for (int idx = 0; idx < cacheBMTChartLabel.Length; idx++)
                        if (cacheBMTChartLabel[idx].BGColor == bGColor && cacheBMTChartLabel[idx].DrawFont == drawFont && cacheBMTChartLabel[idx].FGColor == fGColor && cacheBMTChartLabel[idx].Opacity == opacity && cacheBMTChartLabel[idx].Text == text && cacheBMTChartLabel[idx].TPos == tPos && cacheBMTChartLabel[idx].EqualsInput(input))
                            return cacheBMTChartLabel[idx];

                BMTChartLabel indicator = new BMTChartLabel();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BGColor = bGColor;
                indicator.DrawFont = drawFont;
                indicator.FGColor = fGColor;
                indicator.Opacity = opacity;
                indicator.Text = text;
                indicator.TPos = tPos;
                Indicators.Add(indicator);
                indicator.SetUp();

                BMTChartLabel[] tmp = new BMTChartLabel[cacheBMTChartLabel == null ? 1 : cacheBMTChartLabel.Length + 1];
                if (cacheBMTChartLabel != null)
                    cacheBMTChartLabel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBMTChartLabel = tmp;
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
        /// BMT Chart Label v1.01 - VIP Only - www.bigmiketrading.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BMTChartLabel BMTChartLabel(Color bGColor, Font drawFont, Color fGColor, int opacity, string text, TextPosition tPos)
        {
            return _indicator.BMTChartLabel(Input, bGColor, drawFont, fGColor, opacity, text, tPos);
        }

        /// <summary>
        /// BMT Chart Label v1.01 - VIP Only - www.bigmiketrading.com
        /// </summary>
        /// <returns></returns>
        public Indicator.BMTChartLabel BMTChartLabel(Data.IDataSeries input, Color bGColor, Font drawFont, Color fGColor, int opacity, string text, TextPosition tPos)
        {
            return _indicator.BMTChartLabel(input, bGColor, drawFont, fGColor, opacity, text, tPos);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// BMT Chart Label v1.01 - VIP Only - www.bigmiketrading.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BMTChartLabel BMTChartLabel(Color bGColor, Font drawFont, Color fGColor, int opacity, string text, TextPosition tPos)
        {
            return _indicator.BMTChartLabel(Input, bGColor, drawFont, fGColor, opacity, text, tPos);
        }

        /// <summary>
        /// BMT Chart Label v1.01 - VIP Only - www.bigmiketrading.com
        /// </summary>
        /// <returns></returns>
        public Indicator.BMTChartLabel BMTChartLabel(Data.IDataSeries input, Color bGColor, Font drawFont, Color fGColor, int opacity, string text, TextPosition tPos)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BMTChartLabel(input, bGColor, drawFont, fGColor, opacity, text, tPos);
        }
    }
}
#endregion
