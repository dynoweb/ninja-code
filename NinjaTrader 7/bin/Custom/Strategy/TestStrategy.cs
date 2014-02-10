#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Strategy which can be used to experiment ideas
    /// </summary>
    [Description("Strategy which can be used to experiment ideas")]
    public class TestStrategy : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
		ncatSpreadScalper ss;
		
		string alertOnHighLine = "Disabled";
		string alertOnLonw = "Disabled";
		string alertSoundDown = "Disabled";
		string alertSoundUp = "Disabled";
		int barPeriod = 15; // Bar timeframe
		Color barTextColor = Color.Black;
		int barTextSize = 7;
		Color boxDownColor = Color.Red;
		Color boxUpColor = Color.LawnGreen;
		int drawBoxTickDistance = 6;
		bool drawMarkers = true;
		int dwAboveAvgVol = 150;
		int dwHighClose = 70;
		int dwLowClose = 25;
		int dwNarrowSpread = 70;
		int dwUltraHighVol = 200;
		int dwWideSpread = 150;
		int extendBars = 5;
		bool highLine = true;
		Color highLineColor = Color.Green;
		DashStyle highLineDash = DashStyle.Dash;
		int highLineWidth = 2;
		Color hLtextColor = Color.Black;
		int hLtextSize = 7;
		bool lowLine = true;
		Color lowLineColor = Color.Red;
		DashStyle lowLineDash = DashStyle.Dash;
		int lowLineWidth = 2;
		bool shortLogo = true;
		string signalBarType = "Minutes";
		bool valuesLeft = true;
		bool valuesRight = true;
		int volumeEmaAve = 20;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ss = ncatSpreadScalper(alertOnHighLine, alertOnLonw, alertSoundDown,alertSoundUp,
						barPeriod,barTextColor,barTextSize,boxDownColor,boxUpColor,drawBoxTickDistance,
						drawMarkers,dwAboveAvgVol,dwHighClose,dwLowClose,dwNarrowSpread,dwUltraHighVol,
						dwWideSpread,extendBars,highLine,highLineColor,highLineDash,highLineWidth,
						hLtextColor,hLtextSize,lowLine,lowLineColor,lowLineDash,lowLineWidth,shortLogo,
						signalBarType,valuesLeft,valuesRight,volumeEmaAve);
			Add(ss);

			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			Print(Time + " activeHigh = " + ss.ActiveHigh[0] + " " + ss.ActiveLow[0]);
			if (ss.ActiveHigh[0] != 0 || ss.ActiveLow[0] != 0)
				Print(Time + " ==================================");
        }

		#region
        #endregion
    }
}
