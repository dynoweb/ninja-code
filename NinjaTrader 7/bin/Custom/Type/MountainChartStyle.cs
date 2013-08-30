// 
// Copyright (C) 2012, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
#endregion

// This namespace holds all chart styles. Do not change it.
namespace NinjaTrader.Gui.Chart
{
    public class MountainChartStyle : ChartStyle
	{
		private static bool	registered	= Chart.ChartStyle.Register(new MountainChartStyle());

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			MountainChartStyle ret	= new MountainChartStyle();
			ret.BarWidth			= BarWidth;
			ret.Pen					= Gui.Globals.Clone(Pen);
			ret.DownColor			= DownColor;
			ret.UpColor				= UpColor;
			return ret;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string DisplayName
		{ 
			get { return "Mountain"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth)
		{
			// middle line + 2 * open/close lines
			return (int) (Pen.Width + 2 * (2 + barWidth));
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);
			
			properties.Remove(properties.Find("Pen2", true));
			properties.Remove(properties.Find("BarWidthUI", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\r\rUpper Gradient");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "DownColor", 		"\r\r\rLower Gradient");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Pen", 				"\r\r\rLine");
			
			

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent
		{
			get { return UpColor == Color.Transparent && DownColor == Color.Transparent && Pen.Color == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		public MountainChartStyle() : base(ChartStyleType.Custom2)
		{
			this.DownColor	= Color.White;
			this.UpColor = Color.IndianRed;
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
		{
			if (fromIdx >= toIdx) // DrawLines needs at least 2 elements to draw line
				return; 

			System.Collections.ArrayList	points	= new System.Collections.ArrayList();
			
			System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(bounds, UpColor, DownColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
	
				
			if (fromIdx > 0)			fromIdx--;
			if (toIdx < bars.Count - 1) toIdx++;

			for (int idx = fromIdx; idx <= toIdx; idx++)
			{
				int		x		= chartControl.GetXByBarIdx(bars, idx);
				Point	point	= new Point(x, chartControl.GetYByValue(bars, bars.GetClose(idx)));

				points.Add(point);
			}

			if (points.Count == 0)
				return;
						
			
			System.Drawing.Drawing2D.SmoothingMode	oldSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			graphics.DrawLines(Pen, (Point[])points.ToArray(typeof(Point)));
			
			
			points.Add(new Point(chartControl.GetXByBarIdx(bars, toIdx), bounds.Bottom));
			points.Add(new Point(chartControl.GetXByBarIdx(bars, fromIdx), bounds.Bottom));
			graphics.FillPolygon(brush, (Point[])points.ToArray(typeof(Point)));
			
			
			graphics.SmoothingMode = oldSmoothingMode;
		}
	}
}
