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
using NinjaTrader.Gui.Design;
#endregion

/*
This Indicator has been ported from
Volume Price Analysis AFL - VPA Version 1.0
AFL by Karthikmarar. Detailed explanation available at www.vpanalysis.blogspot.com

03/30/09 - Fixed some error in the code that resulted from the AFL prot.
		   Changed the variables names to be meaningful so the code will be more readable.
           The original variables names are in remark in the "VSA Definitions" area so a reference to the AFL code can be made.
           Removed unnecessary DataSeries to save some bits of memory.
           Added property to determine whether to change the proce bars color according to the trend.

*/

#region Legend

/*
Here is a Legend to the symbols:

Red Square - UpThrust bar.
Blue Diamond - Reversal possible, yesterday was high volume wide spread up bar, but today we reached 10 days high with low close wide spread down bar.
Red Triangle Down - UpThrust confirmation.
Lime Square - Strength bar (either strength is showing in down trend or a supply test in up trend).
Yellow Triangle Up - An Upbar closing near High after a Test confirms strength.
Lime Diamond - Stopping volume. Normally indicates end of bearishness is nearing /OR/ No supply.
Lime Triangle Up - The previous bar saw strength coming back, This upbar confirms strength.
Blue Square - Psuedo UpThrust, A Sign of Weakness /OR/ A High Volume Up Bar closing down in a uptrend shows Distribution /OR/ No Demand.
Blue Triangle Down - A Down Bar closing down after a Pseudo Upthrust confirms weakness.
Yellow Triangle Down - High volume Downbar after an upmove on high volume indicates weakness.
Aqua Triangle Up - High volume upbar closing on the high indicates strength (in short term down trend).
Deep Pink Square - Test for supply.
Turquoise Diamond - Effort to Rise. Bullish sign.
Yellow Diamond - Effort to Fall. Bearish sign.
*/

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Volume Spread Analysis
    /// </summary>
    [Description("Volume Spread Analysis")]
    public class VPA : Indicator
    {
        #region Variables
        // Wizard generated variables
        private int volumeEmaAve = 30; // Default setting for VolumeEmaAve

        private double avgVolume = 0;
        private double highCloseFactor = 0.70;
        private double lowCloseFactor = 0.25;

        private double ultraHighVolfactor = 2;
        private double dwVeryHighVol = 2.2;
        private double dwHighVol = 1.8;
        private double aboveAvgVolfactor = 1.5;
        private double dwLowVol = 0.8;


        private double VolSD;
        private double x1;
        private double LongTermTrendSlope;
        private double MiddleTermTrendSlope;
        private double ShortTermTrendSlope;

        private double narrowSpreadFactor = 0.7;
        private double wideSpreadFactor = 1.5;


        private BoolSeries isUpThrustBar;
        private bool upThrustConditionOne;
        private bool upThrustConditionTwo;
        private bool upThrustConditionThree;
        private bool reversalLikelyBar;
        private BoolSeries isPseudoUpThrustBar;
        private bool pseudoUpThrustConfirmation;
        private bool weaknessBar;
        private BoolSeries isConfirmedUpThrustBar;
        private BoolSeries isNewConfirmedUpThrustBar;
        private bool strenghtInDownTrend0;
        private bool strenghtInDownTrend;
        private bool strenghtInDownTrend1;
        private bool strenghtInDownTrend2;
        private BoolSeries bycond1;
        private bool isStrengthConfirmationBar;
        private bool stopVolBar;
        private bool noDemandBar;
        private bool noSupplyBar;
        private BoolSeries supplyTestBar;
        private bool supplyTestInUpTrendBar;
        private bool successfulSupplyTestBar;
        private bool distributionBar;
        private BoolSeries effortUpMoveBar;
        private bool failedEffortUpMove;
        private bool effortDownMove;
        private BoolSeries isUpBar;
        private BoolSeries isDownBar;
        private BoolSeries isWideSpreadBar;
        private BoolSeries isNarrowSpreadBar;
        private bool isTwoDaysLowVol;
        private BoolSeries isTwoDaysHighVol;
        private bool isUpCloseBar;
        private bool isMidCloseBar;
        private bool isVeryHighCloseBar;
        private bool isDownCloseBar;


        private bool banners = true;
        private bool alertss = false;
		private bool colorBarToTrend = false;
		
        private string bannerstring;
        private string titlestring;

        private DataSeries spread;
        private DataSeries volumeSma;	//Volume moving average
        private double avgSpread;	//Wilders average of Range
        private DataSeries fiveDaysSma;	// SMA(C,5)

        private Font font = new Font("Arial", 9, FontStyle.Bold);
        private Color bandAreaColor = Color.Silver;
        private Color bannerColor = Color.White;
        private Color textColor = Color.White;
        private int bandAreaColorOpacity = 8;

        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Blue), 2), PlotStyle.Line, "VolumeEma"));
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.LimeGreen), 5), PlotStyle.Bar, "Strong"));
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Red), 5), PlotStyle.Bar, "Weak"));
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Yellow), 5), PlotStyle.Bar, "Strong-Weak"));

            CalculateOnBarClose = true;
            Overlay = false;
            PriceTypeSupported = false;

            spread = new DataSeries(this);
            volumeSma = new DataSeries(this);
            fiveDaysSma = new DataSeries(this);

            isUpThrustBar = new BoolSeries(this);
            isPseudoUpThrustBar = new BoolSeries(this);
            isConfirmedUpThrustBar = new BoolSeries(this);
            isNewConfirmedUpThrustBar = new BoolSeries(this);
            bycond1 = new BoolSeries(this);
            supplyTestBar = new BoolSeries(this);
            effortUpMoveBar = new BoolSeries(this);
            isUpBar = new BoolSeries(this);
            isDownBar = new BoolSeries(this);
            isWideSpreadBar = new BoolSeries(this);
            isNarrowSpreadBar = new BoolSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar < volumeEmaAve) return;

            spread.Set(High[0] - Low[0]);
            avgVolume = EMA(Volume, volumeEmaAve)[0];
            VolumeEma.Set(avgVolume);

            // Calculate Volume moving average and it's standard deviation
            volumeSma.Set(SMA(Volume, volumeEmaAve)[0]);
            VolSD = StdDev(volumeSma, volumeEmaAve)[0];

            // check if the vloume has been decreasing in the past two days.
            isTwoDaysLowVol = (Volume[0] < Volume[1] && Volume[0] < Volume[2]);

            // Calculate Range information
            avgSpread = (Wilder(spread, volumeEmaAve)[0]);
            isWideSpreadBar.Set(spread[0] > (wideSpreadFactor * avgSpread));
            isNarrowSpreadBar.Set(spread[0] < (narrowSpreadFactor * avgSpread));

            // Price information
            isUpBar.Set(Close[0] > Close[1]);
            isDownBar.Set(Close[0] < Close[1]);

            // Check if the close is in the Highs/Lows/Middle of the bar.
            x1 = (Close[0] - Low[0] == 0.00 ? avgSpread : (spread[0] / (Close[0] - Low[0])));
            isUpCloseBar = (x1 < 2);
            isDownCloseBar = (x1 > 2);
            isMidCloseBar = (x1 < 2.2 && x1 > 1.8);
            isVeryHighCloseBar = (x1 < 1.35);

            // Trend Definitions
            fiveDaysSma.Set(SMA(Close, 5)[0]);
            LongTermTrendSlope = LinRegSlopeSFX(fiveDaysSma, false, 40, 0)[0];
            MiddleTermTrendSlope = LinRegSlopeSFX(fiveDaysSma, false, 15, 0)[0];
            ShortTermTrendSlope = LinRegSlopeSFX(fiveDaysSma, false, 5, 0)[0];

            // VSA Definitions
			
			// utbar
            isUpThrustBar.Set(isWideSpreadBar[0] && isDownCloseBar && ShortTermTrendSlope > 0);
            // utcond1
			upThrustConditionOne = (isUpThrustBar[1] && isDownBar[0]);
            // utcond2
			upThrustConditionTwo = (isUpThrustBar[1] && isDownBar[0] && Volume[0] > Volume[1]);
            // utcond3
			upThrustConditionThree = (isUpThrustBar[0] && Volume[0] > 2 * volumeSma[0]);
            // scond1
			isConfirmedUpThrustBar.Set(upThrustConditionOne || upThrustConditionTwo || upThrustConditionThree);
            // scond
			isNewConfirmedUpThrustBar.Set(isConfirmedUpThrustBar[0] && !isConfirmedUpThrustBar[1]);

            // trbar
            reversalLikelyBar = (Volume[1] > volumeSma[0] && isUpBar[1] && isWideSpreadBar[1] && isDownBar[0] && isDownCloseBar && isWideSpreadBar[0] && LongTermTrendSlope > 0 && High[0] == MAX(High, 10)[0]);
            
            //hutbar
            isPseudoUpThrustBar.Set(isUpBar[1] && (Volume[1] > aboveAvgVolfactor * volumeSma[0]) && isDownBar[0] && isDownCloseBar && !isWideSpreadBar[0] && !isUpThrustBar[0]);
            //hutcond
            pseudoUpThrustConfirmation = (isPseudoUpThrustBar[1] && isDownBar[0] && isDownCloseBar && !isUpThrustBar[0]);
  
            // tcbar
            weaknessBar = (isUpBar[1] && High[0] == MAX(High, 5)[0] && isDownBar[0] && (isDownCloseBar || isMidCloseBar) && Volume[0] > volumeSma[0] && !isWideSpreadBar[0] && !isPseudoUpThrustBar[0]);

            // stdn, stdn0, stdn1, stdn2
            strenghtInDownTrend =  (Volume[0] > Volume[1] && isDownBar[1] && isUpBar[0] && (isUpCloseBar || isMidCloseBar) && ShortTermTrendSlope < 0 && MiddleTermTrendSlope < 0);
            strenghtInDownTrend0 = (Volume[0] > Volume[1] && isDownBar[1] && isUpBar[0] && (isUpCloseBar || isMidCloseBar) && ShortTermTrendSlope < 0 && MiddleTermTrendSlope < 0 && LongTermTrendSlope < 0);
            strenghtInDownTrend1 = (Volume[0] > (volumeSma[0] * aboveAvgVolfactor) && isDownBar[1] && isUpBar[0] && (isUpCloseBar || isMidCloseBar) && ShortTermTrendSlope < 0 && MiddleTermTrendSlope < 0 && LongTermTrendSlope < 0);
            strenghtInDownTrend2 = (Volume[1] < volumeSma[0] && isUpBar[0] && isVeryHighCloseBar && Volume[0] > volumeSma[0] && ShortTermTrendSlope < 0);
            
            bycond1.Set(strenghtInDownTrend || strenghtInDownTrend1);
            // bycond
			isStrengthConfirmationBar = (isUpBar[0] && bycond1[1]);

            // stvol
            stopVolBar = (Low[0] == MIN(Low, 5)[0] && (isUpCloseBar || isMidCloseBar) && Volume[0] > aboveAvgVolfactor * volumeSma[0] && LongTermTrendSlope < 0);
			
			// ndbar, nsbar
            noDemandBar = (isUpBar[0] && isNarrowSpreadBar[0] && isTwoDaysLowVol && isDownCloseBar); 
            noSupplyBar = (isDownBar[0] && isNarrowSpreadBar[0] && isTwoDaysLowVol && isDownCloseBar);

            // lvtbar, lvtbar1, lvtbar2
            supplyTestBar.Set(isTwoDaysLowVol && Low[0] < Low[1] && isUpCloseBar);
            supplyTestInUpTrendBar = (Volume[0] < volumeSma[0] && Low[0] < Low[1] && isUpCloseBar && LongTermTrendSlope > 0 && MiddleTermTrendSlope > 0 && isWideSpreadBar[0]);
            successfulSupplyTestBar = (supplyTestBar[1] && isUpBar[0] && isUpCloseBar);
			
			// dbar
            distributionBar = (Volume[0] > ultraHighVolfactor * volumeSma[0] && isDownCloseBar && isUpBar[0] && ShortTermTrendSlope > 0 && MiddleTermTrendSlope > 0 && !isConfirmedUpThrustBar[0] && !isUpThrustBar[0]);

            // eftup, eftupfl, eftdn
            effortUpMoveBar.Set(High[0] > High[1] && Low[0] > Low[1] && Close[0] > Close[1] && Close[0] >= ((High[0] - Low[0]) * highCloseFactor + Low[0]) && spread[0] > avgSpread && Volume[0] > Volume[1]);
            failedEffortUpMove = (effortUpMoveBar[1] && (isUpThrustBar[0] || upThrustConditionOne || upThrustConditionTwo || upThrustConditionThree));
            effortDownMove = (High[0] < High[1] && Low[0] < Low[1] && Close[0] < Close[1] && Close[0] <= ((High[0] - Low[0]) * lowCloseFactor + Low[0]) && spread[0] > avgSpread && Volume[0] > Volume[1]);

            #region Candle Definition
			if (ColorBarToTrend)
			{
				if(ShortTermTrendSlope>0 && MiddleTermTrendSlope>0 && LongTermTrendSlope>0)
					{
						BarColor = Color.Lime;
					}
				else if(ShortTermTrendSlope>0 && MiddleTermTrendSlope>0 && LongTermTrendSlope<0)
					{
						BarColor = Color.Green;
					}
				else if(ShortTermTrendSlope>0 && MiddleTermTrendSlope<0 && LongTermTrendSlope<0)
					{
						BarColor = Color.PaleGreen;
					}
				else if(ShortTermTrendSlope<0 && MiddleTermTrendSlope<0 && LongTermTrendSlope<0)
					{
						BarColor = Color.Red;
					}
				else if(ShortTermTrendSlope<0 && MiddleTermTrendSlope>0 && LongTermTrendSlope>0)
					{
						BarColor = Color.PaleGreen;
					}
					
				else if(ShortTermTrendSlope<0 && MiddleTermTrendSlope<0 && LongTermTrendSlope>0)
					{
						BarColor = Color.Orange;
					}	
					
				else 
					{
						BarColor = Color.Yellow;
					}
			}
            #endregion

            #region Volume Bar Definition

            if (upThrustConditionThree)
                Weak.Set(Volume[0]);
            else if (upThrustConditionTwo)
                Weak.Set(Volume[0]);
            else if (isUpThrustBar[0])
                Weak.Set(Volume[0]);
            else if (strenghtInDownTrend)
                Strong.Set(Volume[0]);
            else if (strenghtInDownTrend0)
                Strong.Set(Volume[0]);
            else if (strenghtInDownTrend1)
                Strong.Set(Volume[0]);
            else if (strenghtInDownTrend2)
                Strong.Set(Volume[0]);
            else
                StrongWeak.Set(Volume[0]);
            #endregion

            #region Title Banners

            if (isUpThrustBar[0])
            {
                titlestring = " An Upthrust Bar. A sign of weakness. ";

                textColor = Color.Yellow;
                bannerColor = Color.Red;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, Color.Black, Color.Yellow);
            }

            if (upThrustConditionOne)
            {
                titlestring = " A downbar after an Upthrust. Confirm weakness. ";

                textColor = Color.White;
                bannerColor = Color.Lime;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }
            if (upThrustConditionTwo && !upThrustConditionOne)
            {
                titlestring = " A High Volume downbar after an Upthrust. Confirm weakness.";

                textColor = Color.White;
                bannerColor = Color.Lime;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (upThrustConditionThree)
            {
                titlestring = "This upthrust at very High Voume, Confirms weakness";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, Color.Black, Color.Yellow);
            }

            if (strenghtInDownTrend1)
            {
                titlestring = "Strength seen returning after a down trend. High volume adds to strength. ";

                textColor = Color.Black;
                bannerColor = Color.Yellow;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (strenghtInDownTrend0 && !strenghtInDownTrend)
            {
                titlestring = "Strength seen returning after a down trend. ";

                textColor = Color.Red;
                bannerColor = Color.Lime;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (strenghtInDownTrend && !strenghtInDownTrend1)
            {
                titlestring = "Strength seen returning after a long down trend. ";

                textColor = Color.Red;
                bannerColor = Color.Lime;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (supplyTestBar[0])
            {
                titlestring = "Test for supply. ";

                textColor = Color.Blue;
                bannerColor = Color.Aqua;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (successfulSupplyTestBar)
            {
                titlestring = "An Upbar closing near High after a Test confirms strength. ";

                textColor = Color.Blue;
                bannerColor = Color.Aqua;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (isStrengthConfirmationBar)
            {
                titlestring = "An Upbar closing near High. Confirms return of Strength. ";

                textColor = Color.Blue;
                bannerColor = Color.Lime;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (distributionBar)
            {
                titlestring = "A High Volume Up Bar closing down in a uptrend shows Distribution. ";

                textColor = Color.Blue;
                bannerColor = Color.Yellow;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (isPseudoUpThrustBar[0])
            {
                titlestring = "Psuedo UpThrust.   A Sign of Weakness. ";

                textColor = Color.White;
                bannerColor = Color.Lime;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }
            if (pseudoUpThrustConfirmation)
            {
                titlestring = "A Down Bar closing down after a Pseudo Upthrust confirms weakness. ";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }
            if (supplyTestInUpTrendBar)
            {
                titlestring = "Test for supply in a uptrend. Sign of Strength. ";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (strenghtInDownTrend2)
            {
                titlestring = "High volume upbar closing on the high indicates strength. ";

                textColor = Color.Black;
                bannerColor = Color.Yellow;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (weaknessBar)
            {
                titlestring = "High volume Downbar after an upmove on high volume indicates weakness. ";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }
            if (noDemandBar)
            {
                titlestring = "No Demand. A sign of Weakness. ";

                textColor = Color.White;
                bannerColor = Color.Yellow;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (noSupplyBar)
            {
                titlestring = "No Supply. A sign of Strength. ";

                textColor = Color.White;
                bannerColor = Color.Lime;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (effortUpMoveBar[0])
            {
                titlestring = "Effort to Rise. Bullish sign ";

                textColor = Color.Blue;
                bannerColor = Color.Aquamarine;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }
            if (effortDownMove)
            {
                titlestring = "Effort to Fall. Bearish sign ";

                textColor = Color.Red;
                bannerColor = Color.Blue;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (failedEffortUpMove)
            {
                titlestring = "Effort to Move up has failed. Bearish sign ";

                textColor = Color.Red;
                bannerColor = Color.Blue;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }

            if (stopVolBar)
            {
                titlestring = "Stopping volume. Normally indicates end of bearishness is nearing. ";

                textColor = Color.Blue;
                bannerColor = Color.Turquoise;

                if (alertss)
                    Alert("myAlert" + CurrentBar, NinjaTrader.Cbi.Priority.High, titlestring, "Alert1.wav", 10, bannerColor, Color.Yellow);
            }


            if (banners)
                DrawTextFixed("Banner1", titlestring, TextPosition.TopLeft, Color.Black, font, Color.White, Color.White, 10);
            else
                RemoveDrawObject("Banner1");


            #endregion

            #region Message Banners

            if (isUpThrustBar[0])
            {
                bannerstring = "Up-thrusts are designed to catch stops and " + "\n" +
                "to mislead as many traders as possible.They are normally " + "\n" +
                "seen after there has been weakness in the background. The market" + "\n" +
                "makers know that the market is weak, so the price is marked up to " + "\n" +
                "catch stops, encourage traders to go long in a weak market,AND panic" + "\n" +
                "traders that are already Short into covering their very good position.";

                textColor = Color.Yellow;
                bannerColor = Color.Red;
            }
            if (upThrustConditionThree)
            {
                bannerstring = "This upthrust bar is at high volume." + "\n" +
                "This is a sure sign of weakness. One may even seriously" + "\n" +
                "consider ending the Longs AND be ready to reverse";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;
            }
            if (isUpThrustBar[0] || upThrustConditionThree)
            {
                bannerstring = "Also note that A wide spread down-bar" + "\n" +
                "that appears immediately after any up-thrust, tends to " + "\n" +
                "confirm the weakness (the market makers are locking in traders" + "\n" +
                "into poor positions). With the appearance of an upthrust you " + "\n" +
                "should certainly be paying attention to your trade AND your stops." + "\n" +
                "On many upthrusts you will find that the market will 'test' " + "\n" +
                "almost immediately.";

                textColor = Color.Yellow;
                bannerColor = Color.Red;
            }
            if (upThrustConditionOne)
            {
                bannerstring = "A wide spread down bar following a Upthrust Bar." + "\n" +
                "This confirms weakness. The Smart Money is locking in Traders " + "\n" +
                "into poor positions";

                textColor = Color.White;
                bannerColor = Color.Lime;
            }
            if (upThrustConditionTwo)
            {
                bannerstring = "Also here the volume is high(Above Average)." + "\n" +
                "This is a sure sign of weakness. The Smart Money is locking in" + "\n" +
                "Traders into poor positions";

                textColor = Color.White;
                bannerColor = Color.Lime;
            }
            if (strenghtInDownTrend)
            {
                bannerstring = "Strength Bar. The stock has been in a down Trend." + "\n" +
                "An upbar with higher Volume closing near the High is a sign of " + "\n" +
                "strength returning. The downtrend is likely to reverse soon.";

                textColor = Color.Red;
                bannerColor = Color.Lime;
            }
            if (strenghtInDownTrend1)
            {
                bannerstring = "Here the volume is very much above average." + "\n" +
                "This makes this indication more stronger.";

                textColor = Color.Black;
                bannerColor = Color.Yellow;
            }
            if (isStrengthConfirmationBar)
            {
                bannerstring = "The previous bar saw strength coming back. " + "\n" +
                "This upbar confirms strength.";

                textColor = Color.Blue;
                bannerColor = Color.Lime;
            }
            if (isPseudoUpThrustBar[0])
            {
                bannerstring = "A pseudo Upthrust. This normally appears after an" + "\n" +
                "Up Bar with above average volume. This looks like an upthrust bar " + "\n" +
                "closing down near the Low. But the Volume is normally Lower than average." + "\n" +
                "this is a sign of weakness.If the Volume is High then weakness increases." + "\n" +
                "Smart Money is trying to trap the retailers into bad position.";

                textColor = Color.White;
                bannerColor = Color.Lime;
            }
            if (pseudoUpThrustConfirmation)
            {
                bannerstring = "A downbar after a pseudo Upthrust Confirms weakness. " + "\n" +
                "If the volume is above average the weakness is increased.";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;
            }
            if (supplyTestInUpTrendBar)
            {
                bannerstring = "The previous bar was a successful Test of supply." + "\n" +
                "The current bar is a upbar with higher volume. This confirms strength";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;
            }
            if (distributionBar)
            {
                bannerstring = "A wide range, high volume bar in a up trend " + "\n" +
                "closing down is an indication the Distribution is in progress. " + "\n" +
                "The smart money is Selling the stock to the late Comers rushing to " + "\n" +
                "Buy the stock NOT to be Left Out Of a Bullish move.";

                textColor = Color.Blue;
                bannerColor = Color.Yellow;
            }
            if (successfulSupplyTestBar)
            {
                bannerstring = "The previous bar was a successful Test of" + "\n" +
                "supply. The current bar is a upbar with higher volume. This " + "\n" +
                "confirms strength";

                textColor = Color.Blue;
                bannerColor = Color.Aqua;
            }
            if (weaknessBar)
            {
                bannerstring = "The stock has been moving up on high volume." + "\n" +
                "The current bar is a Downbar with high volume. Indicates weakness" + "\n" +
                "and probably end of the up move";

                textColor = Color.Yellow;
                bannerColor = Color.Blue;
            }
            if (effortUpMoveBar[0])
            {
                bannerstring = "Effort to Rise bar. This normally " + "\n" +
                "found in the beginning of a Markup Phase and is bullish" + "\n" +
                "sign.These may be found at the top of an Upmove as the " + "\n" +
                "Smart money makes a last effort to move the price to the maximum";

                textColor = Color.Blue;
                bannerColor = Color.Aquamarine;
            }
            if (effortDownMove)
            {
                bannerstring = "Effort to Fall bar. This normally" + "\n" +
                "found in the beginning of a Markdown phase.";

                textColor = Color.Red;
                bannerColor = Color.Blue;
            }
            if (noSupplyBar)
            {
                bannerstring = "No Supply. A no supply bar indicates" + "\n" +
                "supply has been removed and the Smart money can markup the" + "\n" +
                "price. It is better to wait for confirmation";

                textColor = Color.White;
                bannerColor = Color.Lime;
            }

            if (stopVolBar)
            {
                bannerstring = "Stopping Volume. This will be " + "\n" +
                "an downbar during a bearish period closing towards" + "\n" +
                "the Top accompanied by High volume. A stopping Volume " + "\n" +
                "normally indicates that smart money is absorbing the " + "\n" +
                "supply which is a Indication that they are Bullishon " + "\n" +
                "the MArket.Hence we Can expect a reversal in the down trend.";

                textColor = Color.Blue;
                bannerColor = Color.Turquoise;
            }

            if (noDemandBar)
            {
                bannerstring = "No Demand Brief Description: " + "\n" +
                "Any up bar which closes in the middle OR Low," + "\n" +
                "especially if the Volume has fallen off, is a " + "\n" +
                "potential sign of weakness. Things to Look Out for: " + "\n" +
                "if the market is still strong, you will normally see " + "\n" +
                "signs of strength in the next few bars, which will most " + "\n" +
                "probably show itself as a: Down bar with a narrow spread," + "\n" +
                " closing in the middle OR High. * Down bar on Low Volume.";

                textColor = Color.White;
                bannerColor = Color.Yellow;
            }

            if (banners)
                DrawTextFixed("Banner2", bannerstring, TextPosition.BottomLeft, Color.Black, font, Color.White, Color.White, 10);
            else
                RemoveDrawObject("Banner2");

            #endregion

            #region Trend Text Definition

            string textMsg1 = "Vol:";
            if (Volume[0] > (volumeSma[0] + 2.0 * VolSD))
                textMsg1 = textMsg1 + " Very High";
            else if (Volume[0] > (volumeSma[0] + 1.0 * VolSD))
                textMsg1 = textMsg1 + " High";
            else if (Volume[0] > volumeSma[0])
                textMsg1 = textMsg1 + " Above Average";
            else if (Volume[0] < volumeSma[0] && Volume[0] > (volumeSma[0] - 1.0 * VolSD))
                textMsg1 = textMsg1 + " Less Than Average";
            else if (Volume[0] < (volumeSma[0] - 1.0 * VolSD))
                textMsg1 = textMsg1 + " Low";
            else textMsg1 = textMsg1 + " ";


            textMsg1 = textMsg1 + "\nSpread:";
            if (Range()[0] > (avgSpread * 2.0))
                textMsg1 = textMsg1 + " Wide";
            else if (Range()[0] > avgSpread)
                textMsg1 = textMsg1 + " Above Average";
            else
                textMsg1 = textMsg1 + " Narrow";


            textMsg1 = textMsg1 + "\nClose:";
            if (isVeryHighCloseBar)
                textMsg1 = textMsg1 + " Very High";
            else if (isUpCloseBar)
                textMsg1 = textMsg1 + " High";
            else if (isMidCloseBar)
                textMsg1 = textMsg1 + " Mid";
            else if (isDownCloseBar)
                textMsg1 = textMsg1 + " Down";
            else
                textMsg1 = textMsg1 + "Very Low";

            string textMsg2 = "Trend:";
            if (ShortTermTrendSlope > 0)
                textMsg2 = textMsg2 + "  Shrt Trm-Up";
            else
                textMsg2 = textMsg2 + "  Shrt Trm-Down";

            if (MiddleTermTrendSlope > 0)
                textMsg2 = textMsg2 + "\nMid Trm-Up";
            else
                textMsg2 = textMsg2 + "\nMid Trm-Down";
            if (LongTermTrendSlope > 0)
                textMsg2 = textMsg2 + "  Lng Trm-Up";
            else
                textMsg2 = textMsg2 + "  Lng Trm-Down";
            DrawTextFixed("Msg2", textMsg1 + "\n" + textMsg2, TextPosition.BottomRight, Color.Black, font, Color.White, Color.White, 10);
            #endregion

            #region Shapes

			// IF YOU CHANGE THE SHAPES/COLORS PLEASE UPDATE THE LEGEND SECTION AT THE TOP.
			
            if (isUpThrustBar[0] && !isNewConfirmedUpThrustBar[0])
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Red);
            }
            if (reversalLikelyBar)
            {
                //Change Small Circle from original code to Diamond
                DrawDiamond("MyDiamond" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Blue);
            }
            if (isNewConfirmedUpThrustBar[0])
            {
                DrawTriangleDown("MyTriangleDown" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Red);
            }
            if (strenghtInDownTrend)
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.Lime);
            }
            if (strenghtInDownTrend1)
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.Lime);
            }
            if (supplyTestInUpTrendBar)
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.Lime);
            }
            if (successfulSupplyTestBar)
            {
                DrawTriangleUp("MyTriangleUp" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.Yellow);
            }
            if (stopVolBar)
            {
                //Change Small Circle from original code to Diamond
                DrawDiamond("MyDiamond" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.Lime);
            }
            if (isStrengthConfirmationBar)
            {
                DrawTriangleUp("MyTriangleUp" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.Lime);
            }
            if (isPseudoUpThrustBar[0])
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Blue);
            }
            if (pseudoUpThrustConfirmation)
            {
                DrawTriangleDown("MyTriangleDown" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Blue);
            }
            if (weaknessBar)
            {
                DrawTriangleDown("MyTriangleDown" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Yellow);
            }
            if (strenghtInDownTrend2)
            {
                DrawTriangleUp("MyTriangleUp" + CurrentBar, true,0, Low[0] - 2 * TickSize, Color.Aqua);
            }
            if (distributionBar)
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Blue);
            }
            if (supplyTestBar[0])
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.DeepPink);
            }
            if (noDemandBar)
            {
                DrawSquare("MySquare" + CurrentBar, true, 0, High[0] + 2 * TickSize, Color.Blue);
            }
            if (noSupplyBar)
            {
                DrawDiamond("MyDiamond" + CurrentBar, true, 0, Low[0] - 2 * TickSize, Color.Lime);
            }

            if (effortUpMoveBar[0])
            {
                DrawDiamond("MyDiamond" + CurrentBar, true, 0, Median[0], Color.Turquoise);
            }

            if (effortDownMove)
            {
                DrawDiamond("MyDiamond" + CurrentBar, true, 0, Median[0], Color.Yellow);
            }


            #endregion
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VolumeEma
        {
            get { return Values[0]; }
        }


        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Strong
        {
            get { return Values[1]; }
        }


        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Weak
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries StrongWeak
        {
            get { return Values[3]; }
        }


        [Description("Periods Volume EMA Average")]
        [Category("Parameters")]
        [Gui.Design.DisplayNameAttribute("Periods Volume Average")]
        public int VolumeEmaAve
        {
            get { return volumeEmaAve; }
            set { volumeEmaAve = Math.Max(1, value); }
        }

        [Description("Wide Spread Factor")]
        [Category("Spread Parameters")]
        [Gui.Design.DisplayNameAttribute("Wide Spread Factor")]
        public double DwWideSpread
        {
            get { return wideSpreadFactor; }
            set { wideSpreadFactor = Math.Max(1, value); }
        }

        [Description("Narrow Spread Factor")]
        [Category("Spread Parameters")]
        [Gui.Design.DisplayNameAttribute("Narrow Spread Factor")]
        public double DwNarrowSpread
        {
            get { return narrowSpreadFactor; }
            set { narrowSpreadFactor = Math.Max(1, value); }
        }
        //		[Description("Spread Middle Factor")]
        //        [Category("Spread Parameters")]
        //		[Gui.Design.DisplayNameAttribute("Spread Middle Factor")]
        //        public double DwSpreadMiddle 
        //        {
        //            get { return dwSpreadMiddle; }
        //            set { dwSpreadMiddle = Math.Max(1, value); }
        //        }

        [Description("High Close Factor")]
        [Category("Price Parameters")]
        [Gui.Design.DisplayNameAttribute("High Close Factor")]
        public double DwHighClose
        {
            get { return highCloseFactor; }
            set { highCloseFactor = Math.Max(1, value); }
        }

        [Description("Low Close Factor")]
        [Category("Price Parameters")]
        [Gui.Design.DisplayNameAttribute("Low Close Factor")]
        public double DwLowClose
        {
            get { return lowCloseFactor; }
            set { lowCloseFactor = Math.Max(1, value); }
        }

        [Description("Ultra HighVol Factor")]
        [Category("Volume Parameters")]
        [Gui.Design.DisplayNameAttribute("Ultra HighVol Factor")]
        public double DwUltraHighVol
        {
            get { return ultraHighVolfactor; }
            set { ultraHighVolfactor = Math.Max(1, value); }
        }
        //		
        //		[Description("Very HighVol Factor")]
        //        [Category("Volume Parameters")]
        //		[Gui.Design.DisplayNameAttribute("Very HighVol Factor")]
        //        public double DwVeryHighVol 
        //        {
        //            get { return dwVeryHighVol; }
        //            set { dwVeryHighVol = Math.Max(1, value); }
        //        }

        //		[Description("HighVol Factor")]
        //        [Category("Volume Parameters")]
        //		[Gui.Design.DisplayNameAttribute("High Vol Factor")]
        //        public double DwHighVol 
        //        {
        //            get { return dwHighVol; }
        //            set { dwHighVol = Math.Max(1, value); }
        //        }
        [Description("Above AvgVol Factor")]
        [Category("Volume Parameters")]
        [Gui.Design.DisplayNameAttribute("Above AvgVol Factor")]
        public double DwAboveAvgVol
        {
            get { return aboveAvgVolfactor; }
            set { aboveAvgVolfactor = Math.Max(1, value); }
        }
        //
        //		[Description("Low Vol Factor")]
        //        [Category("Volume Parameters")]
        //		[Gui.Design.DisplayNameAttribute("Low Volume Factor")]
        //        public double  DwLowVol  
        //        {
        //            get { return  dwLowVol ; }
        //            set {dwLowVol = Math.Max(1, value); }
        //        }

        [Description("Show Banners")]
        [Category("Visual Aids")]
        [Gui.Design.DisplayNameAttribute("Show Banners ?")]
        public bool Banners
        {
            get { return banners; }
            set { banners = value; }
        }

        [Description("Show Alerts")]
        [Category("Visual Aids")]
        [Gui.Design.DisplayNameAttribute("Show Alerts ?")]
        public bool Alertss
        {
            get { return alertss; }
            set { alertss = value; }
        }

        [XmlIgnore()]
        [Description("Band Color")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Band Color")]
        public Color BandAreaColor
        {
            get { return bandAreaColor; }
            set { bandAreaColor = value; }
        }
		
		[XmlIgnore()]
        [Description("Change Bars color according to trend")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Change Bar color")]
        public bool ColorBarToTrend
        {
            get { return colorBarToTrend; }
            set { colorBarToTrend = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BandAreaColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bandAreaColor); }
            set { bandAreaColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }

        [XmlIgnore()]
        [Description("Band Color Opacity")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Band Color Opacity")]
        public int BandAreaColorOpacity
        {
            get { return bandAreaColorOpacity; }
            set { bandAreaColorOpacity = Math.Max(value, 1); }
        }


        [XmlIgnore()]
        [Description("Font")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Banner Font")]
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        [Browsable(false)]
        public SerializableFont FontSerializable
        {
            get { return SerializableFont.FromFont(font); }
            set { font = SerializableFont.ToFont(value); }
        }

        #endregion

        #region PlotRegion
        public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {

            base.Plot(graphics, bounds, min, max);

            if (Bars == null || ChartControl == null)
                return;

            SolidBrush brush = new SolidBrush(Color.FromArgb(bandAreaColorOpacity * 20, bandAreaColor));
            int barWidth = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
            SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
            GraphicsPath path = new GraphicsPath();

            for (int seriesCount = 0; seriesCount < 2; seriesCount++)
            {
                int lastX = -1;
                int lastY = -1;
                DataSeries series = (DataSeries)Values[seriesCount];
                double val = 0;
                Gui.Chart.Plot plot = Plots[seriesCount];

                for (int count = 0; count < ChartControl.BarsPainted; count++)
                {
                    int idx = ChartControl.LastBarPainted - ChartControl.BarsPainted + 1 + count;
                    if (idx < 0 || idx >= Input.Count || (!ChartControl.ShowBarsRequired && idx < BarsRequired))
                        continue;

                    if (seriesCount == 0) val = series.Get(idx); else val = 0;
                    //					if (val == 0)						continue;

                    int x = (int)(ChartControl.CanvasRight - ChartControl.BarMarginRight - barWidth / 2
                        - (ChartControl.BarsPainted - 1) * ChartControl.BarSpace + count * ChartControl.BarSpace) + 1;
                    int y = (int)((bounds.Y + bounds.Height) - ((val - min) / Gui.Chart.ChartControl.MaxMinusMin(max, min)) * bounds.Height);

                    if (lastX >= 0)
                    {
                        path.AddLine(lastX - plot.Pen.Width / 2, lastY, x - plot.Pen.Width / 2, y);

                    }

                    lastX = x;
                    lastY = y;
                }
                path.Reverse();

            }
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPath(brush, path);
            graphics.SmoothingMode = oldSmoothingMode;
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
        private VPA[] cacheVPA = null;

        private static VPA checkVPA = new VPA();

        /// <summary>
        /// Volume Spread Analysis
        /// </summary>
        /// <returns></returns>
        public VPA VPA(int volumeEmaAve)
        {
            return VPA(Input, volumeEmaAve);
        }

        /// <summary>
        /// Volume Spread Analysis
        /// </summary>
        /// <returns></returns>
        public VPA VPA(Data.IDataSeries input, int volumeEmaAve)
        {
            if (cacheVPA != null)
                for (int idx = 0; idx < cacheVPA.Length; idx++)
                    if (cacheVPA[idx].VolumeEmaAve == volumeEmaAve && cacheVPA[idx].EqualsInput(input))
                        return cacheVPA[idx];

            lock (checkVPA)
            {
                checkVPA.VolumeEmaAve = volumeEmaAve;
                volumeEmaAve = checkVPA.VolumeEmaAve;

                if (cacheVPA != null)
                    for (int idx = 0; idx < cacheVPA.Length; idx++)
                        if (cacheVPA[idx].VolumeEmaAve == volumeEmaAve && cacheVPA[idx].EqualsInput(input))
                            return cacheVPA[idx];

                VPA indicator = new VPA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.VolumeEmaAve = volumeEmaAve;
                Indicators.Add(indicator);
                indicator.SetUp();

                VPA[] tmp = new VPA[cacheVPA == null ? 1 : cacheVPA.Length + 1];
                if (cacheVPA != null)
                    cacheVPA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVPA = tmp;
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
        /// Volume Spread Analysis
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VPA VPA(int volumeEmaAve)
        {
            return _indicator.VPA(Input, volumeEmaAve);
        }

        /// <summary>
        /// Volume Spread Analysis
        /// </summary>
        /// <returns></returns>
        public Indicator.VPA VPA(Data.IDataSeries input, int volumeEmaAve)
        {
            return _indicator.VPA(input, volumeEmaAve);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Volume Spread Analysis
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VPA VPA(int volumeEmaAve)
        {
            return _indicator.VPA(Input, volumeEmaAve);
        }

        /// <summary>
        /// Volume Spread Analysis
        /// </summary>
        /// <returns></returns>
        public Indicator.VPA VPA(Data.IDataSeries input, int volumeEmaAve)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VPA(input, volumeEmaAve);
        }
    }
}
#endregion
