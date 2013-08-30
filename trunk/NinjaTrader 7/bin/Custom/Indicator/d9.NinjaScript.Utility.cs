#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Strategy;
using NinjaTrader;
#endregion

namespace d9.NinjaScript.Utility
{
    public enum FilterOrder
    {
        FirstOrder,
        SecondOrder,
        ThirdOrder
    }

    public enum maType
    {
        ADXVMA,
        SMMA,
        T3Tillson,
        T3FulksMatulich,
        SMA,
        EMA,
        HMA,
        JMA,
        JMAFast,
        JMASlow,
        TMA,
        DEMA,
        TEMA,
        REMA,
        WMA,
        SineWMA,
        VMA,
        VMAFast,
        VMASlow,
        VWMA,
        LSMA,
        LSMAe,
        ILRS,
        IE2,
        ZeroLagEMA,
        ZeroLagHATEMA,
        ZeroLagTEMA,
        NonLagMA,
        StepMA,
        ModifiedOptimumEllipticFilter,
        iTrend,
        AdvancedAMA,
        FRAMA,
        MAMA,
        Laguerre,
        Kalman,
        UnscentedKalman,
        UnscentedTurbo,
        UnscentedFast,
        UnscentedSlow
    };
	
	public static class Util
	{
		private static StringFormat stringFormat = new StringFormat();
        private static SolidBrush textBrush = new SolidBrush(Color.Black);
        private static System.Drawing.Font textFont = new Font("Arial", 8);
		
		static Util()
		{
			stringFormat.Alignment = StringAlignment.Far;
		}
		
		public static int getSlope(NinjaTrader.Indicator.Indicator ind, DataSeries series, int index)
		{
			if (!series.ContainsValue(3)) return 0;
			
			return (int)(((180 / Math.PI) * (Math.Atan((series.Get(index) - ((series.Get(index + 1) + series.Get(index + 2)) / 2)) / 1.5 / ind.TickSize))) * -1);
		}
		public static int getSlope(Strategy strat, DataSeries series, int index)
		{
			if (!series.ContainsValue(3)) return 0;
			
			return (int)(((180 / Math.PI) * (Math.Atan((series.Get(index) - ((series.Get(index + 1) + series.Get(index + 2)) / 2)) / 1.5 / strat.TickSize))) * -1);
		}
        public static void drawValueString(NinjaTrader.Indicator.Indicator ind, Graphics graphics, Rectangle bounds, string plotName, int plotNum, double val, int boxNum)
        {
            string s = plotName + ": ";
            StringBuilder str = new StringBuilder();
            str.Append((Math.Round(val, 3)).ToString());
            Color valColor = (val > 0) ? Color.YellowGreen : Color.OrangeRed;
            textBrush.Color = valColor;
            Brush b = textBrush;
			if (plotNum == -1) 
			{
				//do nothing
			}
            else if (ind.Plots[plotNum] == null)
            {				
                s = "Err: Plot Does not exsit!";                
            }
            else
			{
				b = (ind.Plots[plotNum].Pen.Color == Color.Transparent) ? textBrush : ind.Plots[plotNum].Pen.Brush;
			}
            graphics.DrawString(s, textFont, b, bounds.X + bounds.Width - 37, bounds.Y + bounds.Height - boxNum * 20, stringFormat);
            graphics.DrawString(str.ToString(), textFont, textBrush,  bounds.X + bounds.Width - 3, bounds.Y + bounds.Height - boxNum * 20, stringFormat);
        }

        public static void drawValueString(NinjaTrader.Indicator.Indicator ind, Graphics graphics, Rectangle bounds, string plotName, int plotNum, DataSeries series, int boxNum)
        {

            int index = -1;
            int bars = ind.CalculateOnBarClose ? (ind.ChartControl.BarsPainted-2) : (ind.ChartControl.BarsPainted-1);

            if (ind.Bars != null)
            {
                index = ((ind.ChartControl.LastBarPainted-ind.ChartControl.BarsPainted)-1)+bars;
                if (ind.ChartControl.ShowBarsRequired || ((index-ind.Displacement) >= ind.BarsRequired))
                {
                    double val = series.Get(index);
                    string s = plotName+": ";
                    StringBuilder str = new StringBuilder();
                    str.Append((Math.Round(val, 3)).ToString());
                    Color valColor = (val > 0) ? Color.YellowGreen : Color.OrangeRed;
                    textBrush.Color = valColor;
                    Brush b = textBrush;
                    if (plotNum == -1)
                    {
                        //do nothing
                    }
                    else if (ind.Plots[plotNum] == null)
                    {
                        s = "Err: Plot Does not exsit!";
                    }
                    else
                    {
                        b = (ind.Plots[plotNum].Pen.Color == Color.Transparent)
                                ? textBrush
                                : ind.Plots[plotNum].Pen.Brush;
                    }
                    graphics.DrawString(s, textFont, b, bounds.X+bounds.Width-37, bounds.Y+bounds.Height-boxNum*20,
                                        stringFormat);
                    graphics.DrawString(str.ToString(), textFont, textBrush, bounds.X+bounds.Width-3,
                                        bounds.Y+bounds.Height-boxNum*20, stringFormat);
                }
            }
        }

	    public static void DrawSlopeString(NinjaTrader.Indicator.Indicator ind, Graphics graphics, Rectangle bounds, string plotName, int plotNum, int slopeVal, int boxNum)
        {
            string s = plotName + ": ";
            StringBuilder str = new StringBuilder();
            str.Append((slopeVal).ToString());
            str.Append(Convert.ToChar(0176));
            Color slopeColor = (slopeVal > 0) ? Color.YellowGreen : Color.OrangeRed;
            textBrush.Color = slopeColor;
            Brush b = textBrush;
			if (plotNum == -1) 
			{
                //do nothing
			}
            else if (ind.Plots[plotNum] == null)
            {				
                s = "Err: Plot Does not exsit!";                
            }
            else
			{
				b = (ind.Plots[plotNum].Pen.Color == Color.Transparent) ? textBrush : ind.Plots[plotNum].Pen.Brush;
			}
            graphics.DrawString(s, textFont, b, bounds.X + bounds.Width - 28, bounds.Y + bounds.Height - boxNum * 20, stringFormat);
            graphics.DrawString(str.ToString(), textFont, textBrush,  bounds.X + bounds.Width - 3, bounds.Y + bounds.Height - boxNum * 20, stringFormat);
        }
    }

     public class HSLColor
     {
         // Private data members below are on scale 0-1
         // They are scaled for use externally based on scale
         private double _hue = 1.0;
         private double _saturation = 1.0;
         private double _luminosity = 1.0;
  
         private const double scale = 1.0;
  
         public double Hue
         {
             get { return _hue * scale; }
             set { _hue = CheckRange(value / scale); }
         }
         public double Saturation
         {
             get { return _saturation * scale; }
             set { _saturation = CheckRange(value / scale); }
         }
         public double Luminosity
         {
             get { return _luminosity * scale; }
             set { _luminosity = CheckRange(value / scale); }
         }
  
         private double CheckRange(double value)
         {
             if (value < 0.0)
                 value = 0.0;
             else if (value > 1.0)
                 value = 1.0;
             return value;
         }
  
         public override string ToString()
         {
             return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}",   Hue, Saturation, Luminosity);
         }
  
         public string ToRGBString()
         {
             Color color = (Color)this;
             return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
         }
  
         #region Casts to/from System.Drawing.Color
         public static implicit operator Color(HSLColor hslColor)
         {
             double r = 0, g = 0, b = 0;
             if (hslColor._luminosity != 0)
             {
                 if (hslColor._saturation == 0)
                     r = g = b = hslColor._luminosity;
                 else
                 {
                     double temp2 = GetTemp2(hslColor);
                     double temp1 = 2.0 * hslColor._luminosity - temp2;
  
                     r = GetColorComponent(temp1, temp2, hslColor._hue + 1.0 / 3.0);
                     g = GetColorComponent(temp1, temp2, hslColor._hue);
                     b = GetColorComponent(temp1, temp2, hslColor._hue - 1.0 / 3.0);
                 }
             }
             return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
         }
  
         private static double GetColorComponent(double temp1, double temp2, double temp3)
         {
             temp3 = MoveIntoRange(temp3);
             if (temp3 < 1.0 / 6.0)
                 return temp1 + (temp2 - temp1) * 6.0 * temp3;
             else if (temp3 < 0.5)
                 return temp2;
             else if (temp3 < 2.0 / 3.0)
                 return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
             else
                 return temp1;
         }
         private static double MoveIntoRange(double temp3)
         {
             if (temp3 < 0.0)
                 temp3 += 1.0;
             else if (temp3 > 1.0)
                 temp3 -= 1.0;
             return temp3;
         }
         private static double GetTemp2(HSLColor hslColor)
         {
             double temp2;
             if (hslColor._luminosity < 0.5)  //<=??
                 temp2 = hslColor._luminosity * (1.0 + hslColor._saturation);
             else
                 temp2 = hslColor._luminosity + hslColor._saturation - (hslColor._luminosity * hslColor._saturation);
             return temp2;
         }
  
         public static implicit operator HSLColor(Color color)
         {
             HSLColor hslColor = new HSLColor();
             hslColor._hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
             hslColor._luminosity = color.GetBrightness();
             hslColor._saturation = color.GetSaturation();
             return hslColor;
         }
         #endregion
  
         public void SetRGB(int red, int green, int blue)
         {
             HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
             this._hue = hslColor._hue;
             this._saturation = hslColor._saturation;
             this._luminosity = hslColor._luminosity;
         }
  
         public HSLColor() { }
         public HSLColor(Color color)
         {
             SetRGB(color.R, color.G, color.B);
         }
         public HSLColor(int red, int green, int blue)
         {
             SetRGB(red, green, blue);
         }
         public HSLColor(double hue, double saturation, double luminosity)
         {
             this.Hue = hue;
             this.Saturation = saturation;
             this.Luminosity = luminosity;
         }
  
  
     }

	}
