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

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Based on the PTU TrendJumper free calculator for AUDUSD
    /// </summary>
    [Description("Based on the PTU TrendJumper free calculator for AUDUSD")]
    public class TJ : Indicator
    {
        #region Variables
		private string i52_AlertSound = "Alert1.wav";
		private string i00_Version = "4/29/14";
		private bool i01_5DigitBroker = true;
		private int i01_EntryOffsetTics = 2;
		private int i02_StopOffsetTics = 1;
		private double i03_Tgt1X = 1.0;
		private double i04_Tgt2X = 1.5;
		private double i05_Tgt3X = 2.0;
		private int i12_SmallTics = 10;
		private int i14_LargeTics = 20;
		private int i21_EMA1Len = 15;
		private int i22_EMA2Len = 54;
		private int i23_JL1Len = 2;
		private int i24_JL2Len = 6;
		private int i31_EMAIslandTics = 999999999;
		private int i32_EMAProximityTics = 2;
		private int i33_JLIslandTics = 999999999;
		private int i34_JLProximityTics = 2;
		private bool i35_UseJLCross = true;
		private bool i36_UseEMAFilter = true;
		private Color i91_JLJumpColor = Color.FromKnownColor(KnownColor.Black);
		private Color i92_EMA1JumpColor = Color.FromKnownColor(KnownColor.DarkOrange);
		private Color i93_EMA2JumpColor = Color.FromKnownColor(KnownColor.Magenta);
		private Color i94_JLCrossColor = Color.FromKnownColor(KnownColor.DarkCyan);
		private bool j11_UseBreakevenStop = true;
		private int j12_BkevenTics = 1;
		private bool j21_UseTrailingStop = true;
		private int j22_TrailLength = 9;
		private double j23_TrailMMPct = 100.0;
		private int j24_TrailOffsetTics = 2;
		private double j32_LockInMMPct = 50.0;
		private int j33_LockInTics = 5;
		private int k01_KLMinor = 5;
		private int k02_KLMinorEZone = 1;
		private int k03_KLMinorEAdj = 1;
		private int k04_KLMinorSZone = 1;
		private int k05_KLMinorSAdj = 1;
		private int k08_KLMajor = 50;
		private int k09_KLMajorEZone = 1;
		private int k10_KLMajorEAdj = 2;
		private int k11_KLMajorSZone = 1;
		private int k12_KLMajorSAdj = 2;
		private int EntryOffsetTics = 2;
		private int StopOffsetTics = 2;
		private double Tgt1X = 1.0;
		private double Tgt2X = 1.5;
		private double Tgt3X = 2.0;
		private int SmallTics = 10;
		private int LargeTics = 20;
		private int EMA1Len = 50;
		private int EMA2Len = 200;
		private int MOM1Len = 4;
		private int MOM2Len = 9;
		private int EMAIslandTics = 2;
		private int EMAProximityTics = 2;
		private int MOMIslandTics = 2;
		private int MOMProximityTics = 2;
		private bool UseJLCross = true;
		private bool UseEMAFilter = true;
		private Color MOMJumpColor = Color.FromKnownColor(KnownColor.White);
		private Color EMA1JumpColor = Color.FromKnownColor(KnownColor.Yellow);
		private Color EMA2JumpColor = Color.FromKnownColor(KnownColor.Magenta);
		private Color MOMCrossColor = Color.FromKnownColor(KnownColor.Cyan);
		private bool UseFixedPos = true;
		private bool UseTrailPos = true;
		private int BkevenTics = 1;
		private int TrailLen = 9;
		private double TrailMMPct = 100.0;
		private int TrailOffsetTics = 2;
		private double LockInMMPct = 50.0;
		private int LockInTics = 5;
		private int KLMinor = 5;
		private int KLMinorEZone = 1;
		private int KLMinorEAdj = 1;
		private int KLMinorSZone = 1;
		private int KLMinorSAdj = 1;
		private int KLMinorTZone = 1;
		private int KLMajor = 50;
		private int KLMajorEZone = 2;
		private int KLMajorEAdj = 2;
		private int KLMajorSZone = 2;
		private int KLMajorSAdj = 2;
		private int KLMajorTZone = 2;
		private int FXKL = 10;
		private int FXZone = 9;
		private int LSetupBar = -1;
		private int SSetupBar = -1;
		private Color TJColor = Color.FromKnownColor(KnownColor.White);
		private bool i51_UseAlert;
		private DataSeries Trail;
		private double Value1;
		private double Value2;
		private double Value3;
		private bool Condition1;
		private bool Condition2;
		private bool Condition3;
		private bool Condition4;
		private bool i001_UseEntryBasedTgts;
		private int i031_MinTgt1Tics;
		private int i041_MinTgt2Tics;
		private int i051_MinTgt3Tics;
		private bool i11_UseSmallSetups;
		private bool i13_UseLargeSetups;
		private bool j01_UseFixedPos;
		private bool j02_UseTrailPos;
		private bool j03_UseClosestSetup;
		private int j111_MinBkevenMMTics;
		private int j231_MinTrailMMTics;
		private bool j31_UseLockInProfit;
		private int j321_MinLockInMMTics;
		private bool k00_UseKLAdjust;
		private int k06_KLMinorTZone;
		private int k07_KLMinorTAdj;
		private int k13_KLMajorTZone;
		private int k14_KLMajorTAdj;
		private bool k15_RoundFX2Pip;
		private bool AlternateTargeting;
		private int MinTgt1Tics;
		private double MinTgt1;
		private int MinTgt2Tics;
		private double MinTgt2;
		private int MinTgt3Tics;
		private double MinTgt3;
		private bool UseSmallSetups;
		private bool IsSmall;
		private bool UseLargeSetups;
		private int SDir;
		private int SDirLast;
		private int TDir;
		private int TDirLast;
		private double SLEntry;
		private double SLStop;
		private double SLTgt1;
		private double SLTgt2;
		private double SLTgt3;
		private double SSEntry;
		private double SSStop;
		private double SSTgt1;
		private double SSTgt2;
		private double SSTgt3;
		private double TLEntry;
		private double TLStop;
		private double TLStopLast;
		private double TLTgt1;
		private double TLTgt2;
		private double TLTgt3;
		private double TSEntry;
		private double TSStop;
		private double TSStopLast;
		private double TSTgt1;
		private double TSTgt2;
		private double TSTgt3;
		private bool UseClosest;
		private bool UseBreakeven;
		private double BkevenValue;
		private int MinBrkevenMMTics;
		private double MinBrkevenMM;
		private bool UseTrailing;
		private int MinTrailMMTics;
		private double MinTrailMM;
		private double TrailOffset;
		private bool UseLockInProfit;
		private int MinLockInMMTics;
		private double MinLockInMM;
		private double LockInAmt;
		private double BEMM;
		private double TMM;
		private double LIMM;
		private int SetupType;
		private int TSetupType;
		private bool NewSetup;
		private bool LongSetup;
		private bool ShortSetup;
		private bool UseKLAdjust;
		private int KLMinorTAdj;
		private int KLMajorTAdj;
		private bool RoundFX2Pip;
		private int FXAdj;
		private double OneTic;
		private double EMAIslDist;
		private double EMAProxDist;
		private double MOMIslDist;
		private double MOMProxDist;
		private DataSeries EMA1;
		private DataSeries EMA2;
		private DataSeries MOM1;
		private DataSeries MOM2;
		private double LIEMA1Value;
		private double LIEMA2Value;
		private double LIMOM1Value;
		private double LIMOM2Value;
		private bool LIslEMA1OK;
		private bool LIslEMA2OK;
		private bool LIslMOM1OK;
		private bool LIslMOM2OK;
		private bool LIslEMA1OKLast;
		private bool LIslEMA2OKLast;
		private bool LIslMOM1OKLast;
		private bool LIslMOM2OKLast;
		private bool LProxEMA1OK;
		private bool LProxEMA2OK;
		private bool LProxMOM1OK;
		private bool LProxMOM2OK;
		private bool LProxEMA1OKLast;
		private bool LProxEMA2OKLast;
		private bool LProxMOM1OKLast;
		private bool LProxMOM2OKLast;
		private bool LEMA1;
		private bool LEMA2;
		private bool LMOM1;
		private bool LMOM2;
		private double SIEMA1Value;
		private double SIEMA2Value;
		private double SIMOM1Value;
		private double SIMOM2Value;
		private bool SIslEMA1OK;
		private bool SIslEMA2OK;
		private bool SIslMOM1OK;
		private bool SIslMOM2OK;
		private bool SIslEMA1OKLast;
		private bool SIslEMA2OKLast;
		private bool SIslMOM1OKLast;
		private bool SIslMOM2OKLast;
		private bool SProxEMA1OK;
		private bool SProxEMA2OK;
		private bool SProxMOM1OK;
		private bool SProxMOM2OK;
		private bool SProxEMA1OKLast;
		private bool SProxEMA2OKLast;
		private bool SProxMOM1OKLast;
		private bool SProxMOM2OKLast;
		private bool SEMA1;
		private bool SEMA2;
		private bool SMOM1;
		private bool SMOM2;
		private bool IslUpTrend;
		private bool IslDownTrend;
		private bool ProxUpTrend;
		private bool ProxDownTrend;
		private bool LM1X;
		private bool SM1X;
		private bool LMOMCross;
		private bool SMOMCross;
		private int EMA1Dir;
		private int MOMCrossDir;
		private int MOMCrossDirLast;
		private double LTgt3;
		private double LTgt2;
		private double LTgt1;
		private double LEntry;
		private double LStop;
		private double SStop;
		private double SEntry;
		private double STgt1;
		private double STgt2;
		private double STgt3;
		private double SmallTrade;
		private double LargeTrade;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "LongTgt3"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "LongTgt2"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "LongTgt1"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 2, "LongEntry"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "LongStop"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "ShortStop"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 2, "ShortEntry"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "ShortTgt1"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "ShortTgt2"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), (PlotStyle) 3, "ShortTgt3"));
			Overlay = true;
			Plots[0].Min = 0.0;
			Plots[1].Min = 0.0;
			Plots[2].Min = 0.0;
			Plots[3].Min = 0.0;
			Plots[4].Min = 0.0;
			Plots[5].Min = 0.0;
			Plots[6].Min = 0.0;
			Plots[7].Min = 0.0;
			Plots[8].Min = 0.0;
			Plots[9].Min = 0.0;
			AlternateTargeting = i001_UseEntryBasedTgts;
			if (i01_5DigitBroker)
			{
				EntryOffsetTics = i01_EntryOffsetTics;
				StopOffsetTics = i02_StopOffsetTics;
				EMAProximityTics = i32_EMAProximityTics;
				MOMProximityTics = i34_JLProximityTics;
				EMAIslandTics = i31_EMAIslandTics;
				MOMIslandTics = i33_JLIslandTics;
			}
			else
			{
				EntryOffsetTics = i01_EntryOffsetTics / 10;
				StopOffsetTics = i02_StopOffsetTics / 10;
				EMAProximityTics = i32_EMAProximityTics / 10;
				MOMProximityTics = i34_JLProximityTics / 10;
				EMAIslandTics = i31_EMAIslandTics / 10;
				MOMIslandTics = i33_JLIslandTics / 10;
			}
			Tgt1X = i03_Tgt1X;
			Tgt2X = i04_Tgt2X;
			Tgt3X = i05_Tgt3X;
			MinTgt1Tics = i031_MinTgt1Tics;
			MinTgt2Tics = i041_MinTgt2Tics;
			MinTgt3Tics = i051_MinTgt3Tics;
			UseFixedPos = j01_UseFixedPos;
			UseTrailPos = j02_UseTrailPos;
			UseClosest = j03_UseClosestSetup;
			UseSmallSetups = i11_UseSmallSetups;
			SmallTics = i12_SmallTics;
			UseLargeSetups = i13_UseLargeSetups;
			LargeTics = i14_LargeTics;
			EMA1Len = i21_EMA1Len;
			EMA2Len = i22_EMA2Len;
			MOM1Len = i23_JL1Len;
			MOM2Len = i24_JL2Len;
			UseJLCross = i35_UseJLCross;
			UseEMAFilter = i36_UseEMAFilter;
			UseBreakeven = j11_UseBreakevenStop;
			BkevenTics = j12_BkevenTics;
			MinBrkevenMMTics = j111_MinBkevenMMTics;
			UseTrailing = j21_UseTrailingStop;
			TrailLen = j22_TrailLength;
			TrailMMPct = j23_TrailMMPct;
			MinTrailMMTics = j231_MinTrailMMTics;
			TrailOffsetTics = j24_TrailOffsetTics;
			UseLockInProfit = j31_UseLockInProfit;
			LockInMMPct = j32_LockInMMPct;
			MinLockInMMTics = j321_MinLockInMMTics;
			LockInTics = j33_LockInTics;
			MOMJumpColor = i91_JLJumpColor;
			EMA1JumpColor = i92_EMA1JumpColor;
			EMA2JumpColor = i93_EMA2JumpColor;
			MOMCrossColor = i94_JLCrossColor;
			UseKLAdjust = k00_UseKLAdjust;
			KLMinor = k01_KLMinor;
			KLMinorEZone = k02_KLMinorEZone;
			KLMinorEAdj = k03_KLMinorEAdj;
			KLMinorSZone = k04_KLMinorSZone;
			KLMinorSAdj = k05_KLMinorSAdj;
			KLMinorTZone = k06_KLMinorTZone;
			KLMinorTAdj = k07_KLMinorTAdj;
			KLMajor = k08_KLMajor;
			KLMajorEZone = k09_KLMajorEZone;
			KLMajorEAdj = k10_KLMajorEAdj;
			KLMajorSZone = k11_KLMajorSZone;
			KLMajorSAdj = k12_KLMajorSAdj;
			KLMajorTZone = k13_KLMajorTZone;
			KLMajorTAdj = k14_KLMajorTAdj;
			RoundFX2Pip = k15_RoundFX2Pip;
			EMA1 = new DataSeries((IndicatorBase) this);
			EMA2 = new DataSeries((IndicatorBase) this);
			MOM1 = new DataSeries((IndicatorBase) this);
			MOM2 = new DataSeries((IndicatorBase) this);
			Trail = new DataSeries((IndicatorBase) this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
      if (this.CurrentBar == 1)
      {
        this.OneTic = this.TickSize;
        this.EMAIslDist = (double) this.EMAIslandTics * this.OneTic;
        this.EMAProxDist = (double) this.EMAProximityTics * this.OneTic;
        this.MOMIslDist = (double) this.MOMIslandTics * this.OneTic;
        this.MOMProxDist = (double) this.MOMProximityTics * this.OneTic;
        this.SmallTrade = (double) this.SmallTics * this.OneTic;
        this.LargeTrade = (double) this.LargeTics * this.OneTic;
        this.MinTgt1 = (double) this.MinTgt1Tics * this.OneTic;
        this.MinTgt2 = (double) this.MinTgt2Tics * this.OneTic;
        this.MinTgt3 = (double) this.MinTgt3Tics * this.OneTic;
      }
      if (this.CurrentBar <= 1)
        return;
      if (this.FirstTickOfBar)
      {
        this.SDirLast = this.SDir;
        this.TDirLast = this.TDir;
        this.TLStopLast = this.TLStop;
        this.TSStopLast = this.TSStop;
        this.LIslEMA1OKLast = this.LIslEMA1OK;
        this.LIslEMA2OKLast = this.LIslEMA2OK;
        this.LIslMOM1OKLast = this.LIslMOM1OK;
        this.LIslMOM2OKLast = this.LIslMOM2OK;
        this.LProxEMA1OKLast = this.LProxEMA1OK;
        this.LProxEMA2OKLast = this.LProxEMA2OK;
        this.LProxMOM1OKLast = this.LProxMOM1OK;
        this.LProxMOM2OKLast = this.LProxMOM2OK;
        this.SIslEMA1OKLast = this.SIslEMA1OK;
        this.SIslEMA2OKLast = this.SIslEMA2OK;
        this.SIslMOM1OKLast = this.SIslMOM1OK;
        this.SIslMOM2OKLast = this.SIslMOM2OK;
        this.SProxEMA1OKLast = this.SProxEMA1OK;
        this.SProxEMA2OKLast = this.SProxEMA2OK;
        this.SProxMOM1OKLast = this.SProxMOM1OK;
        this.SProxMOM2OKLast = this.SProxMOM2OK;
        this.MOMCrossDirLast = this.MOMCrossDir;
        this.EMA1.Set(this.EMA1[1]);
        this.EMA2.Set(this.EMA2[1]);
        this.MOM1.Set(this.MOM1[1]);
        this.MOM2.Set(this.MOM2[1]);
        this.Trail.Set(this.Trail[1]);
      }
      this.EMA1.Set(this.EMA(this.Close, this.EMA1Len)[0]);
      this.EMA2.Set(this.EMA(this.Close, this.EMA2Len)[0]);
      this.MOM1.Set(this.Round2Fraction(this.DonchianChannel(this.MOM1Len)[0]));
      this.MOM2.Set(this.Round2Fraction(this.DonchianChannel(this.MOM2Len)[0]));
      this.Trail.Set(this.Round2Fraction(this.DonchianChannel(this.TrailLen)[0]));
      if (this.EMA1[0] - this.EMA1[1] > 0.0)
        this.EMA1Dir = 1;
      if (this.EMA1[0] - this.EMA1[1] < 0.0)
        this.EMA1Dir = -1;
      if (this.EMA1[0] - this.EMA1[1] == 0.0)
        this.EMA1Dir = 0;
      if (!this.IslUpTrend)
      {
        this.IslUpTrend = this.MOM1[0] > this.MOM2[0] || this.MOM1[0] == this.MOM2[0] && this.MOM1[1] > this.MOM2[1];
        if (this.IslUpTrend)
          this.IslDownTrend = false;
      }
      if (!this.IslDownTrend)
      {
        this.IslDownTrend = this.MOM1[0] < this.MOM2[0] || this.MOM1[0] == this.MOM2[0] && this.MOM1[1] < this.MOM2[1];
        if (this.IslDownTrend)
          this.IslUpTrend = false;
      }
      this.ProxUpTrend = true;
      this.ProxDownTrend = true;
      if (this.LMOM1 || this.Close[0] < this.MOM1[0])
      {
        this.LIslMOM1OK = false;
        this.LProxMOM1OK = false;
      }
      if (this.LMOM2 || this.Close[0] < this.MOM2[0])
      {
        this.LIslMOM2OK = false;
        this.LProxMOM2OK = false;
      }
      if (this.LEMA1 || this.Close[0] < this.EMA1[0])
      {
        this.LIslEMA1OK = false;
        this.LProxEMA1OK = false;
      }
      if (this.LEMA2 || this.Close[0] < this.EMA2[0])
      {
        this.LIslEMA2OK = false;
        this.LProxEMA2OK = false;
      }
      if (this.SMOM1 || this.Close[0] > this.MOM1[0])
      {
        this.SIslMOM1OK = false;
        this.SProxMOM1OK = false;
      }
      if (this.SMOM2 || this.Close[0] > this.MOM2[0])
      {
        this.SIslMOM2OK = false;
        this.SProxMOM2OK = false;
      }
      if (this.SEMA1 || this.Close[0] > this.EMA1[0])
      {
        this.SIslEMA1OK = false;
        this.SProxEMA1OK = false;
      }
      if (this.SEMA2 || this.Close[0] > this.EMA2[0])
      {
        this.SIslEMA2OK = false;
        this.SProxEMA2OK = false;
      }
      if (!this.LIslMOM1OK)
        this.LProxMOM1OK = false;
      if (!this.LIslMOM2OK)
        this.LProxMOM2OK = false;
      if (!this.LIslEMA1OK)
        this.LProxEMA1OK = false;
      if (!this.LIslEMA2OK)
        this.LProxEMA2OK = false;
      if (!this.SIslMOM1OK)
        this.SProxMOM1OK = false;
      if (!this.SIslMOM2OK)
        this.SProxMOM2OK = false;
      if (!this.SIslEMA1OK)
        this.SProxEMA1OK = false;
      if (!this.SIslEMA2OK)
        this.SProxEMA2OK = false;
      this.Value1 = this.Round2Fraction(this.Low[0] - this.MOM1[0]);
      if (this.Value1 >= this.MOMIslDist)
      {
        this.LIMOM1Value = !this.LIslMOM1OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.LIMOM1Value, this.Value1));
        this.LIslMOM1OK = true;
        this.SIslMOM1OK = false;
      }
      this.Value1 = this.Round2Fraction(this.Low[0] - this.MOM2[0]);
      if (this.Value1 >= this.MOMIslDist)
      {
        this.LIMOM2Value = !this.LIslMOM2OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.LIMOM2Value, this.Value1));
        this.LIslMOM2OK = true;
        this.SIslMOM2OK = false;
      }
      this.Value1 = this.Round2Fraction(this.Low[0] - this.EMA1[0]);
      if (this.Value1 >= this.EMAIslDist)
      {
        this.LIEMA1Value = !this.LIslEMA1OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.LIEMA1Value, this.Value1));
        this.LIslEMA1OK = true;
        this.SIslEMA1OK = false;
      }
      this.Value1 = this.Round2Fraction(this.Low[0] - this.EMA2[0]);
      if (this.Value1 >= this.EMAIslDist)
      {
        this.LIEMA2Value = !this.LIslEMA2OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.LIEMA2Value, this.Value1));
        this.LIslEMA2OK = true;
        this.SIslEMA2OK = false;
      }
      this.Value1 = this.Round2Fraction(this.MOM1[0]) - this.High[0];
      if (this.Value1 >= this.MOMIslDist)
      {
        this.SIMOM1Value = !this.SIslMOM1OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.SIMOM1Value, this.Value1));
        this.SIslMOM1OK = true;
        this.LIslMOM1OK = false;
      }
      this.Value1 = this.Round2Fraction(this.MOM2[0] - this.High[0]);
      if (this.Value1 >= this.MOMIslDist)
      {
        this.SIMOM2Value = !this.SIslMOM2OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.SIMOM2Value, this.Value1));
        this.SIslMOM2OK = true;
        this.LIslMOM2OK = false;
      }
      this.Value1 = this.Round2Fraction(this.EMA1[0]) - this.High[0];
      if (this.Value1 >= this.EMAIslDist)
      {
        this.SIEMA1Value = !this.SIslEMA1OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.SIEMA1Value, this.Value1));
        this.SIslEMA1OK = true;
        this.LIslEMA1OK = false;
      }
      this.Value1 = this.Round2Fraction(this.EMA2[0]) - this.High[0];
      if (this.Value1 >= this.EMAIslDist)
      {
        this.SIEMA2Value = !this.SIslEMA2OK ? this.Round2Fraction(this.Value1) : this.Round2Fraction(Math.Max(this.SIEMA2Value, this.Value1));
        this.SIslEMA2OK = true;
        this.LIslEMA2OK = false;
      }
      if (this.ProxUpTrend)
      {
        this.Value1 = this.Round2Fraction(this.Low[0] - this.MOM1[0]);
        this.LProxMOM1OK = this.Value1 <= this.MOMProxDist && this.Value1 < this.LIMOM1Value && this.LIslMOM1OKLast;
        this.Value1 = this.Round2Fraction(this.Low[0] - this.MOM2[0]);
        this.LProxMOM2OK = this.Value1 <= this.MOMProxDist && this.Value1 < this.LIMOM2Value && this.LIslMOM2OKLast;
        this.Value1 = this.Round2Fraction(this.Low[0] - this.EMA1[0]);
        this.LProxEMA1OK = this.Value1 <= this.EMAProxDist && this.Value1 < this.LIEMA1Value && this.LIslEMA1OKLast;
        this.Value1 = this.Round2Fraction(this.Low[0] - this.EMA2[0]);
        this.LProxEMA2OK = this.Value1 <= this.EMAProxDist && this.Value1 < this.LIEMA2Value && this.LIslEMA2OKLast;
      }
      if (this.ProxDownTrend)
      {
        this.Value1 = this.Round2Fraction(this.MOM1[0] - this.High[0]);
        this.SProxMOM1OK = this.Value1 <= this.MOMProxDist && this.Value1 < this.SIMOM1Value && this.SIslMOM1OKLast;
        this.Value1 = this.Round2Fraction(this.MOM2[0] - this.High[0]);
        this.SProxMOM2OK = this.Value1 <= this.MOMProxDist && this.Value1 < this.SIMOM2Value && this.SIslMOM2OKLast;
        this.Value1 = this.Round2Fraction(this.EMA1[0] - this.High[0]);
        this.SProxEMA1OK = this.Value1 <= this.EMAProxDist && this.Value1 < this.SIEMA1Value && this.SIslEMA1OKLast;
        this.Value1 = this.Round2Fraction(this.EMA2[0] - this.High[0]);
        this.SProxEMA2OK = this.Value1 <= this.EMAProxDist && this.Value1 < this.SIEMA2Value && this.SIslEMA2OKLast;
      }
      this.LMOM1 = false;
      this.LMOM2 = false;
      this.LEMA1 = false;
      this.LEMA2 = false;
      this.SMOM1 = false;
      this.SMOM2 = false;
      this.SEMA1 = false;
      this.SEMA2 = false;
      this.LMOMCross = false;
      this.SMOMCross = false;
      this.LSetupBar = -1;
      this.SSetupBar = -1;
      if (this.Close[0] >= this.Open[0])
      {
        if (this.LIslMOM2OK && this.LIslMOM2OKLast && (this.LProxMOM2OK || this.LProxMOM2OKLast))
        {
          if (this.Close[0] > this.MOM2[0] && this.IslUpTrend)
          {
            this.LMOM2 = true;
          }
          else
          {
            this.LMOM2 = false;
            this.LIslMOM2OK = false;
            this.LProxMOM2OK = false;
          }
        }
        if (this.LIslEMA1OK && this.LIslEMA1OKLast && (this.LProxEMA1OK || this.LProxEMA1OKLast))
        {
          if (this.Close[0] > this.EMA1[0] && this.IslUpTrend)
          {
            this.LEMA1 = true;
          }
          else
          {
            this.LEMA1 = false;
            this.LIslEMA1OK = false;
            this.LProxEMA1OK = false;
          }
        }
        if (this.LIslEMA2OK && this.LIslEMA2OKLast && (this.LProxEMA2OK || this.LProxEMA2OKLast))
        {
          if (this.Close[0] > this.EMA2[0] && this.IslUpTrend)
          {
            this.LEMA2 = true;
          }
          else
          {
            this.LEMA2 = false;
            this.LIslEMA2OK = false;
            this.LProxEMA2OK = false;
          }
        }
      }
      if (this.Close[0] <= this.Open[0])
      {
        if (this.SIslMOM2OK && this.SIslMOM2OKLast && (this.SProxMOM2OK || this.SProxMOM2OKLast))
        {
          if (this.Close[0] < this.MOM2[0] && this.IslDownTrend)
          {
            this.SMOM2 = true;
          }
          else
          {
            this.SMOM2 = false;
            this.SIslMOM2OK = false;
            this.SProxMOM2OK = false;
          }
        }
        if (this.SIslEMA1OK && this.SIslEMA1OKLast && (this.SProxEMA1OK || this.SProxEMA1OKLast))
        {
          if (this.Close[0] <= this.EMA1[0] && this.IslDownTrend)
          {
            this.SEMA1 = true;
          }
          else
          {
            this.SEMA1 = false;
            this.SIslEMA1OK = false;
            this.SProxEMA1OK = false;
          }
        }
        if (this.SIslEMA2OK && this.SIslEMA2OKLast && (this.SProxEMA2OK || this.SProxEMA2OKLast))
        {
          if (this.Close[0] < this.EMA2[0] && this.IslDownTrend)
          {
            this.SEMA2 = true;
          }
          else
          {
            this.SEMA2 = false;
            this.SIslEMA2OK = false;
            this.SProxEMA2OK = false;
          }
        }
      }
      if (this.LMOM1 || this.Close[0] < this.MOM1[0])
      {
        this.LIslMOM1OK = false;
        this.LProxMOM1OK = false;
      }
      if (this.LMOM2 || this.Close[0] < this.MOM2[0])
      {
        this.LIslMOM2OK = false;
        this.LProxMOM2OK = false;
      }
      if (this.LEMA1 || this.Close[0] < this.EMA1[0])
      {
        this.LIslEMA1OK = false;
        this.LProxEMA1OK = false;
      }
      if (this.LEMA2 || this.Close[0] < this.EMA2[0])
      {
        this.LIslEMA2OK = false;
        this.LProxEMA2OK = false;
      }
      if (this.SMOM1 || this.Close[0] > this.MOM1[0])
      {
        this.SIslMOM1OK = false;
        this.SProxMOM1OK = false;
      }
      if (this.SMOM2 || this.Close[0] > this.MOM2[0])
      {
        this.SIslMOM2OK = false;
        this.SProxMOM2OK = false;
      }
      if (this.SEMA1 || this.Close[0] > this.EMA1[0])
      {
        this.SIslEMA1OK = false;
        this.SProxEMA1OK = false;
      }
      if (this.SEMA2 || this.Close[0] > this.EMA2[0])
      {
        this.SIslEMA2OK = false;
        this.SProxEMA2OK = false;
      }
      if (this.UseJLCross)
      {
        if (this.MOM1[0] > this.MOM2[0])
          this.MOMCrossDir = 1;
        if (this.MOM1[0] < this.MOM2[0])
          this.MOMCrossDir = -1;
        if (this.MOMCrossDir == 1 && this.MOMCrossDirLast != 1)
        {
          this.LM1X = true;
          this.SM1X = false;
        }
        if (this.MOMCrossDir == -1 && this.MOMCrossDirLast != -1)
        {
          this.SM1X = true;
          this.LM1X = false;
        }
        if (this.LM1X && this.Close[0] >= this.Open[0] && this.Close[0] > this.MOM2[0] && (!this.UseEMAFilter || this.EMA1Dir != -1 || this.EMA1[0] - this.High[0] < (double) this.EntryOffsetTics * this.OneTic && this.Low[0] < this.EMA1[0]))
          this.LMOMCross = true;
        if (this.SM1X && this.Close[0] <= this.Open[0] && this.Close[0] < this.MOM2[0] && (!this.UseEMAFilter || this.EMA1Dir != 1 || this.Low[0] - this.EMA1[0] < (double) this.EntryOffsetTics * this.OneTic && this.High[0] > this.EMA1[0]))
          this.SMOMCross = true;
        if (this.LMOMCross)
          this.LM1X = false;
        if (this.SMOMCross)
          this.SM1X = false;
        if (this.LM1X && this.Close[0] >= this.Open[0] && (this.Close[0] > this.MOM2[0] && !this.LMOMCross))
          this.LM1X = false;
        if (this.SM1X && this.Close[0] <= this.Open[0] && (this.Close[0] < this.MOM2[0] && !this.SMOMCross))
          this.SM1X = false;
      }
      this.LongSetup = this.LMOM2 || this.LEMA1 || this.LEMA2 || this.LMOMCross;
      this.ShortSetup = this.SMOM2 || this.SEMA1 || this.SEMA2 || this.SMOMCross;
      if (this.LongSetup && this.LSetupBar == -1)
      {
        this.Value1 = this.Range()[0] * this.Tgt1X;
        this.Value2 = this.Range()[0] * this.Tgt2X;
        this.Value3 = this.Range()[0] * this.Tgt3X;
        this.LEntry = this.Round2Fraction(this.High[0] + (double) this.EntryOffsetTics * this.OneTic);
        this.LTgt3 = !this.AlternateTargeting ? this.Round2Fraction(this.High[0] + this.Value3) : this.Round2Fraction(this.LEntry + this.Value3);
        this.LTgt2 = !this.AlternateTargeting ? this.Round2Fraction(this.High[0] + this.Value2) : this.Round2Fraction(this.LEntry + this.Value2);
        this.LTgt1 = !this.AlternateTargeting ? this.Round2Fraction(this.High[0] + this.Value1) : this.Round2Fraction(this.LEntry + this.Value1);
        this.LStop = this.Round2Fraction(this.Low[0] - (double) this.StopOffsetTics * this.OneTic);
        int num = this.UseKLAdjust ? 1 : 0;
        if (this.UseSmallSetups)
        {
          this.IsSmall = false;
          if (this.Round2Fraction(this.LTgt2 - this.LEntry) <= this.SmallTrade)
          {
            this.Value1 = this.Tgt2X == this.Tgt1X ? this.Round2Fraction(this.LTgt3 + 2.0 * (this.LTgt3 - this.LTgt1)) : this.Round2Fraction(this.LTgt3 + (this.LTgt3 - this.LTgt2) * (this.Tgt3X - this.Tgt2X) / (this.Tgt2X - this.Tgt1X));
            this.IsSmall = true;
            this.LTgt1 = this.LTgt2;
            this.LTgt2 = this.LTgt3;
            this.LTgt3 = !this.AlternateTargeting ? this.Round2Fraction(this.Value1) : this.Round2Fraction(this.Value1);
          }
        }
        if (this.UseLargeSetups && !this.IsSmall && this.Round2Fraction(this.LTgt2 - this.LEntry) >= this.LargeTrade)
        {
          this.Value1 = this.Tgt3X == this.Tgt2X ? this.Round2Fraction(this.LTgt1 - 2.0 * (this.LTgt3 - this.LTgt1)) : this.Round2Fraction(this.LTgt1 - (this.LTgt2 - this.LTgt1) * (this.Tgt2X - this.Tgt1X) / (this.Tgt3X - this.Tgt2X));
          this.LTgt3 = this.LTgt2;
          this.LTgt2 = this.LTgt1;
          this.LTgt1 = !this.AlternateTargeting ? this.Round2Fraction(this.Value1) : this.Round2Fraction(this.Value1);
        }
        this.LTgt1 = Math.Max(this.LTgt1, this.Round2Fraction(this.LEntry + this.MinTgt1));
        this.LTgt2 = Math.Max(this.LTgt2, this.Round2Fraction(this.LEntry + this.MinTgt2));
        this.LTgt3 = Math.Max(this.LTgt3, this.Round2Fraction(this.LEntry + this.MinTgt3));
      }
      if (this.ShortSetup && this.SSetupBar == -1)
      {
        this.Value1 = this.Range()[0] * this.Tgt1X;
        this.Value2 = this.Range()[0] * this.Tgt2X;
        this.Value3 = this.Range()[0] * this.Tgt3X;
        this.SStop = this.Round2Fraction(this.High[0] + (double) this.StopOffsetTics * this.OneTic);
        this.SEntry = this.Round2Fraction(this.Low[0] - (double) this.EntryOffsetTics * this.OneTic);
        this.STgt1 = !this.AlternateTargeting ? this.Round2Fraction(this.Low[0] - this.Value1) : this.Round2Fraction(this.SEntry - this.Value1);
        this.STgt2 = !this.AlternateTargeting ? this.Round2Fraction(this.Low[0] - this.Value2) : this.Round2Fraction(this.SEntry - this.Value2);
        this.STgt3 = !this.AlternateTargeting ? this.Round2Fraction(this.Low[0] - this.Value3) : this.Round2Fraction(this.SEntry - this.Value3);
        int num = this.UseKLAdjust ? 1 : 0;
        if (this.UseSmallSetups)
        {
          this.IsSmall = false;
          if (this.Round2Fraction(this.SEntry - this.STgt2) <= this.SmallTrade)
          {
            this.Value1 = this.Tgt2X == this.Tgt1X ? this.Round2Fraction(this.STgt3 + 2.0 * (this.STgt3 - this.STgt1)) : this.Round2Fraction(this.STgt3 + (this.STgt3 - this.STgt2) * (this.Tgt3X - this.Tgt2X) / (this.Tgt2X - this.Tgt1X));
            this.IsSmall = true;
            this.STgt1 = this.STgt2;
            this.STgt2 = this.STgt3;
            this.STgt3 = !this.AlternateTargeting ? this.Round2Fraction(this.Value1) : this.Round2Fraction(this.Value1);
          }
        }
        if (this.UseLargeSetups && !this.IsSmall && this.Round2Fraction(this.SEntry - this.STgt2) >= this.LargeTrade)
        {
          this.Value1 = this.Tgt3X == this.Tgt2X ? this.Round2Fraction(this.STgt1 - 2.0 * (this.STgt3 - this.STgt1)) : this.Round2Fraction(this.STgt1 - (this.STgt2 - this.STgt1) * (this.Tgt2X - this.Tgt1X) / (this.Tgt3X - this.Tgt2X));
          this.STgt3 = this.STgt2;
          this.STgt2 = this.STgt1;
          this.STgt1 = !this.AlternateTargeting ? this.Round2Fraction(this.Value1) : this.Round2Fraction(this.Value1);
        }
        this.STgt1 = Math.Min(this.STgt1, this.Round2Fraction(this.SEntry - this.MinTgt1));
        this.STgt2 = Math.Min(this.STgt2, this.Round2Fraction(this.SEntry - this.MinTgt2));
        this.STgt3 = Math.Min(this.STgt3, this.Round2Fraction(this.SEntry - this.MinTgt3));
      }
      if (this.LongSetup)
      {
        int num1 = this.UseKLAdjust ? 1 : 0;
      }
      if (this.ShortSetup)
      {
        int num2 = this.UseKLAdjust ? 1 : 0;
      }
      if (this.LMOMCross || this.SMOMCross)
        this.TJColor = this.MOMCrossColor;
      if (this.LMOM2 || this.SMOM2)
        this.TJColor = this.MOMJumpColor;
      if (this.LEMA1 || this.SEMA1)
        this.TJColor = this.EMA1JumpColor;
      if (this.LEMA2 || this.SEMA2)
        this.TJColor = this.EMA2JumpColor;
      this.PlotColors[0][0] = this.TJColor;
      this.PlotColors[1][0] = this.TJColor;
      this.PlotColors[2][0] = this.TJColor;
      this.PlotColors[3][0] = this.TJColor;
      this.PlotColors[4][0] = this.TJColor;
      this.PlotColors[5][0] = this.TJColor;
      this.PlotColors[6][0] = this.TJColor;
      this.PlotColors[7][0] = this.TJColor;
      this.PlotColors[8][0] = this.TJColor;
      this.PlotColors[9][0] = this.TJColor;
      if (this.LongSetup)
      {
        this.LongTgt3.Set(this.LTgt3);
        this.LongTgt2.Set(this.LTgt2);
        this.LongTgt1.Set(this.LTgt1);
        this.LongEntry.Set(this.LEntry);
        this.LongStop.Set(this.LStop);
      }
      if (this.ShortSetup)
      {
        this.ShortStop.Set(this.SStop);
        this.ShortEntry.Set(this.SEntry);
        this.ShortTgt1.Set(this.STgt1);
        this.ShortTgt2.Set(this.STgt2);
        this.ShortTgt3.Set(this.STgt3);
      }
      if (this.i51_UseAlert && (this.LongSetup || this.ShortSetup))
        this.Alert("Alert", (Priority) 0, "New Setup", this.i52_AlertSound, 1, Color.Blue, Color.White);
      this.NewSetup = false;
    }

    private double Round2Fraction(double input)
    {
      return this.Instrument.MasterInstrument.Round2TickSize(input);
    }

    private double IFF(bool test, double trueValue, double falseValue)
    {
      if (test)
        return trueValue;
      else
        return falseValue;
    }

        #region Properties
		[XmlIgnore]
		[Browsable(false)]
		public DataSeries LongTgt3
		{
		get
		{
			return this.Values[0];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LongTgt2
		{
		get
		{
			return this.Values[1];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LongTgt1
		{
		get
		{
			return this.Values[2];
		}
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries LongEntry
		{
		get
		{
			return this.Values[3];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LongStop
		{
		get
		{
			return this.Values[4];
		}
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries ShortStop
		{
		get
		{
			return this.Values[5];
		}
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries ShortEntry
		{
		get
		{
			return this.Values[6];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries ShortTgt1
		{
		get
		{
			return this.Values[7];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries ShortTgt2
		{
		get
		{
			return this.Values[8];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries ShortTgt3
		{
		get
		{
			return this.Values[9];
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		public string I00_Version
		{
		get
		{
			return this.i00_Version;
		}
		set
		{
			this.i00_Version = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public bool I01_5DigitBroker
		{
		get
		{
			return this.i01_5DigitBroker;
		}
		set
		{
			this.i01_5DigitBroker = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool I51_UseAlert
		{
		get
		{
			return this.i51_UseAlert;
		}
		set
		{
			this.i51_UseAlert = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private string I52_AlertSound
		{
		get
		{
			return this.i52_AlertSound;
		}
		set
		{
			this.i52_AlertSound = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		public int I01_EntryOffsetTics
		{
		get
		{
			return this.i01_EntryOffsetTics;
		}
		set
		{
			this.i01_EntryOffsetTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private bool I001_UseEntryBasedTgts
		{
		get
		{
			return this.i001_UseEntryBasedTgts;
		}
		set
		{
			this.i001_UseEntryBasedTgts = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public int I02_StopOffsetTics
		{
		get
		{
			return this.i02_StopOffsetTics;
		}
		set
		{
			this.i02_StopOffsetTics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public double I03_Tgt1X
		{
		get
		{
			return this.i03_Tgt1X;
		}
		set
		{
			this.i03_Tgt1X = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I031_MinTgt1Tics
		{
		get
		{
			return this.i031_MinTgt1Tics;
		}
		set
		{
			this.i031_MinTgt1Tics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public double I04_Tgt2X
		{
		get
		{
			return this.i04_Tgt2X;
		}
		set
		{
			this.i04_Tgt2X = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int I041_MinTgt2Tics
		{
		get
		{
			return this.i041_MinTgt2Tics;
		}
		set
		{
			this.i041_MinTgt2Tics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public double I05_Tgt3X
		{
		get
		{
			return this.i05_Tgt3X;
		}
		set
		{
			this.i05_Tgt3X = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I051_MinTgt3Tics
		{
		get
		{
			return this.i051_MinTgt3Tics;
		}
		set
		{
			this.i051_MinTgt3Tics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool I11_UseSmallSetups
		{
		get
		{
			return this.i11_UseSmallSetups;
		}
		set
		{
			this.i11_UseSmallSetups = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I12_SmallTics
		{
		get
		{
			return this.i12_SmallTics;
		}
		set
		{
			this.i12_SmallTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private bool I13_UseLargeSetups
		{
		get
		{
			return this.i13_UseLargeSetups;
		}
		set
		{
			this.i13_UseLargeSetups = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I14_LargeTics
		{
		get
		{
			return this.i14_LargeTics;
		}
		set
		{
			this.i14_LargeTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I21_EMA1Len
		{
		get
		{
			return this.i21_EMA1Len;
		}
		set
		{
			this.i21_EMA1Len = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int I22_EMA2Len
		{
		get
		{
			return this.i22_EMA2Len;
		}
		set
		{
			this.i22_EMA2Len = value;
		}
		}

		[Description("MOM1Len")]
		[GridCategory("Parameters")]
		public int I23_JL1Len
		{
		get
		{
			return this.i23_JL1Len;
		}
		set
		{
			this.i23_JL1Len = value;
		}
		}

		[Description("MOM2Len")]
		[GridCategory("Parameters")]
		public int I24_JL2Len
		{
		get
		{
			return this.i24_JL2Len;
		}
		set
		{
			this.i24_JL2Len = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int I31_EMAIslandTics
		{
		get
		{
			return this.i31_EMAIslandTics;
		}
		set
		{
			this.i31_EMAIslandTics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int I32_EMAProximityTics
		{
		get
		{
			return this.i32_EMAProximityTics;
		}
		set
		{
			this.i32_EMAProximityTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I33_JLIslandTics
		{
		get
		{
			return this.i33_JLIslandTics;
		}
		set
		{
			this.i33_JLIslandTics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int I34_JLProximityTics
		{
		get
		{
			return this.i34_JLProximityTics;
		}
		set
		{
			this.i34_JLProximityTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private bool I35_UseJLCross
		{
		get
		{
			return this.i35_UseJLCross;
		}
		set
		{
			this.i35_UseJLCross = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool I36_UseEMAFilter
		{
		get
		{
			return this.i36_UseEMAFilter;
		}
		set
		{
			this.i36_UseEMAFilter = value;
		}
		}

		[GridCategory("Parameters")]
		[XmlIgnore]
		[Description("JumpLine Jump Color")]
		private Color I91_JLJumpColor
		{
		get
		{
			return this.i91_JLJumpColor;
		}
		set
		{
			this.i91_JLJumpColor = value;
		}
		}

		[Browsable(false)]
		private string I91_JLJumpColorSerialize
		{
		get
		{
			return SerializableColor.ToString(this.i91_JLJumpColor);
		}
		set
		{
			this.i91_JLJumpColor = SerializableColor.FromString(value);
		}
		}
		

		[GridCategory("Parameters")]
		[XmlIgnore]
		[Description("EMA1 Jump Color")]
		private Color I92_EMA1JumpColor
		{
		get
		{
			return this.i92_EMA1JumpColor;
		}
		set
		{
			this.i92_EMA1JumpColor = value;
		}
		}

		[Browsable(false)]
		private string I92_EMA1JumpColorSerialize
		{
		get
		{
			return SerializableColor.ToString(this.i92_EMA1JumpColor);
		}
		set
		{
			this.i92_EMA1JumpColor = SerializableColor.FromString(value);
		}
		}

		[XmlIgnore]
		[Description("EMA2 Jump Color")]
		[GridCategory("Parameters")]
		private Color I93_EMA2JumpColor
		{
		get
		{
			return this.i93_EMA2JumpColor;
		}
		set
		{
			this.i93_EMA2JumpColor = value;
		}
		}

		[Browsable(false)]
		private string I93_EMA2JumpColorSerialize
		{
		get
		{
			return SerializableColor.ToString(this.i93_EMA2JumpColor);
		}
		set
		{
			this.i93_EMA2JumpColor = SerializableColor.FromString(value);
		}
		}

		[GridCategory("Parameters")]
		[XmlIgnore]
		[Description("JumpLine Cross Color")]
		private Color I94_JLCrossColor
		{
		get
		{
			return this.i94_JLCrossColor;
		}
		set
		{
			this.i94_JLCrossColor = value;
		}
		}

		[Browsable(false)]
		private string I94_JLCrossColorSerialize
		{
		get
		{
			return SerializableColor.ToString(this.i94_JLCrossColor);
		}
		set
		{
			this.i94_JLCrossColor = SerializableColor.FromString(value);
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private bool J01_UseFixedPos
		{
		get
		{
			return this.j01_UseFixedPos;
		}
		set
		{
			this.j01_UseFixedPos = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool J02_UseTrailPos
		{
		get
		{
			return this.j02_UseTrailPos;
		}
		set
		{
			this.j02_UseTrailPos = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool J03_UseClosestSetup
		{
		get
		{
			return this.j03_UseClosestSetup;
		}
		set
		{
			this.j03_UseClosestSetup = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool J11_UseBreakevenStop
		{
		get
		{
			return this.j11_UseBreakevenStop;
		}
		set
		{
			this.j11_UseBreakevenStop = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int J111_MinBkevenMMTics
		{
		get
		{
			return this.j111_MinBkevenMMTics;
		}
		set
		{
			this.j111_MinBkevenMMTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int J12_BkevenTics
		{
		get
		{
			return this.j12_BkevenTics;
		}
		set
		{
			this.j12_BkevenTics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public bool J21_UseTrailingStop
		{
		get
		{
			return this.j21_UseTrailingStop;
		}
		set
		{
			this.j21_UseTrailingStop = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int J22_TrailLength
		{
		get
		{
			return this.j22_TrailLength;
		}
		set
		{
			this.j22_TrailLength = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private double J23_TrailMMPct
		{
		get
		{
			return this.j23_TrailMMPct;
		}
		set
		{
			this.j23_TrailMMPct = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int J231_MinTrailMMTics
		{
		get
		{
			return this.j231_MinTrailMMTics;
		}
		set
		{
			this.j231_MinTrailMMTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int J24_TrailOffsetTics
		{
		get
		{
			return this.j24_TrailOffsetTics;
		}
		set
		{
			this.j24_TrailOffsetTics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool J31_UseLockInProfit
		{
		get
		{
			return this.j31_UseLockInProfit;
		}
		set
		{
			this.j31_UseLockInProfit = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private double J32_LockInMMPct
		{
		get
		{
			return this.j32_LockInMMPct;
		}
		set
		{
			this.j32_LockInMMPct = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int J321_MinLockInMMTics
		{
		get
		{
			return this.j321_MinLockInMMTics;
		}
		set
		{
			this.j321_MinLockInMMTics = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int J33_LockInTics
		{
		get
		{
			return this.j33_LockInTics;
		}
		set
		{
			this.j33_LockInTics = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool K00_UseKLAdjust
		{
		get
		{
			return this.k00_UseKLAdjust;
		}
		set
		{
			this.k00_UseKLAdjust = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int K01_KLMinor
		{
		get
		{
			return this.k01_KLMinor;
		}
		set
		{
			this.k01_KLMinor = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K02_KLMinorEZone
		{
		get
		{
			return this.k02_KLMinorEZone;
		}
		set
		{
			this.k02_KLMinorEZone = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int K03_KLMinorEAdj
		{
		get
		{
			return this.k03_KLMinorEAdj;
		}
		set
		{
			this.k03_KLMinorEAdj = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int K04_KLMinorSZone
		{
		get
		{
			return this.k04_KLMinorSZone;
		}
		set
		{
			this.k04_KLMinorSZone = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K05_KLMinorSAdj
		{
		get
		{
			return this.k05_KLMinorSAdj;
		}
		set
		{
			this.k05_KLMinorSAdj = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K06_KLMinorTZone
		{
		get
		{
			return this.k06_KLMinorTZone;
		}
		set
		{
			this.k06_KLMinorTZone = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K07_KLMinorTAdj
		{
		get
		{
			return this.k07_KLMinorTAdj;
		}
		set
		{
			this.k07_KLMinorTAdj = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int K08_KLMajor
		{
		get
		{
			return this.k08_KLMajor;
		}
		set
		{
			this.k08_KLMajor = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K09_KLMajorEZone
		{
		get
		{
			return this.k09_KLMajorEZone;
		}
		set
		{
			this.k09_KLMajorEZone = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int K10_KLMajorEAdj
		{
		get
		{
			return this.k10_KLMajorEAdj;
		}
		set
		{
			this.k10_KLMajorEAdj = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int K11_KLMajorSZone
		{
		get
		{
			return this.k11_KLMajorSZone;
		}
		set
		{
			this.k11_KLMajorSZone = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K12_KLMajorSAdj
		{
		get
		{
			return this.k12_KLMajorSAdj;
		}
		set
		{
			this.k12_KLMajorSAdj = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K13_KLMajorTZone
		{
		get
		{
			return this.k13_KLMajorTZone;
		}
		set
		{
			this.k13_KLMajorTZone = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int K14_KLMajorTAdj
		{
			get { return this.k14_KLMajorTAdj; }
			set	{ this.k14_KLMajorTAdj = value; }
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool K15_RoundFX2Pip
		{
		get
		{
			return this.k15_RoundFX2Pip;
		}
		set
		{
			this.k15_RoundFX2Pip = value;
		}
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
        private TJ[] cacheTJ = null;

        private static TJ checkTJ = new TJ();

        /// <summary>
        /// Based on the PTU TrendJumper free calculator for AUDUSD
        /// </summary>
        /// <returns></returns>
        public TJ TJ(string i00_Version, bool i01_5DigitBroker, int i01_EntryOffsetTics, int i02_StopOffsetTics, double i03_Tgt1X, double i04_Tgt2X, double i05_Tgt3X, int i23_JL1Len, int i24_JL2Len, bool j21_UseTrailingStop)
        {
            return TJ(Input, i00_Version, i01_5DigitBroker, i01_EntryOffsetTics, i02_StopOffsetTics, i03_Tgt1X, i04_Tgt2X, i05_Tgt3X, i23_JL1Len, i24_JL2Len, j21_UseTrailingStop);
        }

        /// <summary>
        /// Based on the PTU TrendJumper free calculator for AUDUSD
        /// </summary>
        /// <returns></returns>
        public TJ TJ(Data.IDataSeries input, string i00_Version, bool i01_5DigitBroker, int i01_EntryOffsetTics, int i02_StopOffsetTics, double i03_Tgt1X, double i04_Tgt2X, double i05_Tgt3X, int i23_JL1Len, int i24_JL2Len, bool j21_UseTrailingStop)
        {
            if (cacheTJ != null)
                for (int idx = 0; idx < cacheTJ.Length; idx++)
                    if (cacheTJ[idx].I00_Version == i00_Version && cacheTJ[idx].I01_5DigitBroker == i01_5DigitBroker && cacheTJ[idx].I01_EntryOffsetTics == i01_EntryOffsetTics && cacheTJ[idx].I02_StopOffsetTics == i02_StopOffsetTics && Math.Abs(cacheTJ[idx].I03_Tgt1X - i03_Tgt1X) <= double.Epsilon && Math.Abs(cacheTJ[idx].I04_Tgt2X - i04_Tgt2X) <= double.Epsilon && Math.Abs(cacheTJ[idx].I05_Tgt3X - i05_Tgt3X) <= double.Epsilon && cacheTJ[idx].I23_JL1Len == i23_JL1Len && cacheTJ[idx].I24_JL2Len == i24_JL2Len && cacheTJ[idx].J21_UseTrailingStop == j21_UseTrailingStop && cacheTJ[idx].EqualsInput(input))
                        return cacheTJ[idx];

            lock (checkTJ)
            {
                checkTJ.I00_Version = i00_Version;
                i00_Version = checkTJ.I00_Version;
                checkTJ.I01_5DigitBroker = i01_5DigitBroker;
                i01_5DigitBroker = checkTJ.I01_5DigitBroker;
                checkTJ.I01_EntryOffsetTics = i01_EntryOffsetTics;
                i01_EntryOffsetTics = checkTJ.I01_EntryOffsetTics;
                checkTJ.I02_StopOffsetTics = i02_StopOffsetTics;
                i02_StopOffsetTics = checkTJ.I02_StopOffsetTics;
                checkTJ.I03_Tgt1X = i03_Tgt1X;
                i03_Tgt1X = checkTJ.I03_Tgt1X;
                checkTJ.I04_Tgt2X = i04_Tgt2X;
                i04_Tgt2X = checkTJ.I04_Tgt2X;
                checkTJ.I05_Tgt3X = i05_Tgt3X;
                i05_Tgt3X = checkTJ.I05_Tgt3X;
                checkTJ.I23_JL1Len = i23_JL1Len;
                i23_JL1Len = checkTJ.I23_JL1Len;
                checkTJ.I24_JL2Len = i24_JL2Len;
                i24_JL2Len = checkTJ.I24_JL2Len;
                checkTJ.J21_UseTrailingStop = j21_UseTrailingStop;
                j21_UseTrailingStop = checkTJ.J21_UseTrailingStop;

                if (cacheTJ != null)
                    for (int idx = 0; idx < cacheTJ.Length; idx++)
                        if (cacheTJ[idx].I00_Version == i00_Version && cacheTJ[idx].I01_5DigitBroker == i01_5DigitBroker && cacheTJ[idx].I01_EntryOffsetTics == i01_EntryOffsetTics && cacheTJ[idx].I02_StopOffsetTics == i02_StopOffsetTics && Math.Abs(cacheTJ[idx].I03_Tgt1X - i03_Tgt1X) <= double.Epsilon && Math.Abs(cacheTJ[idx].I04_Tgt2X - i04_Tgt2X) <= double.Epsilon && Math.Abs(cacheTJ[idx].I05_Tgt3X - i05_Tgt3X) <= double.Epsilon && cacheTJ[idx].I23_JL1Len == i23_JL1Len && cacheTJ[idx].I24_JL2Len == i24_JL2Len && cacheTJ[idx].J21_UseTrailingStop == j21_UseTrailingStop && cacheTJ[idx].EqualsInput(input))
                            return cacheTJ[idx];

                TJ indicator = new TJ();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.I00_Version = i00_Version;
                indicator.I01_5DigitBroker = i01_5DigitBroker;
                indicator.I01_EntryOffsetTics = i01_EntryOffsetTics;
                indicator.I02_StopOffsetTics = i02_StopOffsetTics;
                indicator.I03_Tgt1X = i03_Tgt1X;
                indicator.I04_Tgt2X = i04_Tgt2X;
                indicator.I05_Tgt3X = i05_Tgt3X;
                indicator.I23_JL1Len = i23_JL1Len;
                indicator.I24_JL2Len = i24_JL2Len;
                indicator.J21_UseTrailingStop = j21_UseTrailingStop;
                Indicators.Add(indicator);
                indicator.SetUp();

                TJ[] tmp = new TJ[cacheTJ == null ? 1 : cacheTJ.Length + 1];
                if (cacheTJ != null)
                    cacheTJ.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTJ = tmp;
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
        /// Based on the PTU TrendJumper free calculator for AUDUSD
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TJ TJ(string i00_Version, bool i01_5DigitBroker, int i01_EntryOffsetTics, int i02_StopOffsetTics, double i03_Tgt1X, double i04_Tgt2X, double i05_Tgt3X, int i23_JL1Len, int i24_JL2Len, bool j21_UseTrailingStop)
        {
            return _indicator.TJ(Input, i00_Version, i01_5DigitBroker, i01_EntryOffsetTics, i02_StopOffsetTics, i03_Tgt1X, i04_Tgt2X, i05_Tgt3X, i23_JL1Len, i24_JL2Len, j21_UseTrailingStop);
        }

        /// <summary>
        /// Based on the PTU TrendJumper free calculator for AUDUSD
        /// </summary>
        /// <returns></returns>
        public Indicator.TJ TJ(Data.IDataSeries input, string i00_Version, bool i01_5DigitBroker, int i01_EntryOffsetTics, int i02_StopOffsetTics, double i03_Tgt1X, double i04_Tgt2X, double i05_Tgt3X, int i23_JL1Len, int i24_JL2Len, bool j21_UseTrailingStop)
        {
            return _indicator.TJ(input, i00_Version, i01_5DigitBroker, i01_EntryOffsetTics, i02_StopOffsetTics, i03_Tgt1X, i04_Tgt2X, i05_Tgt3X, i23_JL1Len, i24_JL2Len, j21_UseTrailingStop);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Based on the PTU TrendJumper free calculator for AUDUSD
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TJ TJ(string i00_Version, bool i01_5DigitBroker, int i01_EntryOffsetTics, int i02_StopOffsetTics, double i03_Tgt1X, double i04_Tgt2X, double i05_Tgt3X, int i23_JL1Len, int i24_JL2Len, bool j21_UseTrailingStop)
        {
            return _indicator.TJ(Input, i00_Version, i01_5DigitBroker, i01_EntryOffsetTics, i02_StopOffsetTics, i03_Tgt1X, i04_Tgt2X, i05_Tgt3X, i23_JL1Len, i24_JL2Len, j21_UseTrailingStop);
        }

        /// <summary>
        /// Based on the PTU TrendJumper free calculator for AUDUSD
        /// </summary>
        /// <returns></returns>
        public Indicator.TJ TJ(Data.IDataSeries input, string i00_Version, bool i01_5DigitBroker, int i01_EntryOffsetTics, int i02_StopOffsetTics, double i03_Tgt1X, double i04_Tgt2X, double i05_Tgt3X, int i23_JL1Len, int i24_JL2Len, bool j21_UseTrailingStop)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TJ(input, i00_Version, i01_5DigitBroker, i01_EntryOffsetTics, i02_StopOffsetTics, i03_Tgt1X, i04_Tgt2X, i05_Tgt3X, i23_JL1Len, i24_JL2Len, j21_UseTrailingStop);
        }
    }
}
#endregion
