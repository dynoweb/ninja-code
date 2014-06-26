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
    [Description("Premier Trader University Active Swing Reversal indicator")]
    public class PTUActiveSwingReversal : Indicator
    {
        #region Variables
    private string i00_Version = "4/7/14";
    private double i01_EntryMult = 1.0;
    private double i02_StopMult = 1.0;
    private double i03_Tgt1Mult = 2.0;
    private double i04_Tgt2Mult = 3.3;
    private bool i11_ExitAtTarget = true;
    private int i12_ExitTgt = 2;
    private double i18_PullbackPct = 25.0;
    private bool i21_UseBkevenStop = true;
    private bool i26_UseTrailStop = true;
    private double i27_TrailMMPct = 150.0;
    private double i28_TrailMult = 0.25;
    private double i32_MMPct = 65.0;
    private double i33_MMLockInMult = 1.0;
    private int i41_TrailLength = 3;
    private int i42_IncrLength = 14;
    private double i43_IncrX = 1.0;
    private int i44_SentimentLength = 7;
    private int j01_PctRLen = 55;
    private double j02_OBLevel = -10.0;
    private double j03_OBReset = -50.0;
    private double j04_OSLevel = -90.0;
    private double j05_OSReset = -50.0;
    private int PctRLen = 55;
    private double OBLevel = -10.0;
    private double OBReset = -50.0;
    private double OSLevel = -90.0;
    private double OSReset = -50.0;
    private double EntryMult = 1.0;
    private double SetupStopMult = 0.5;
    private double SetupTgtMult = 1.0;
    private double SetupTgt2Mult = 2.0;
    private bool ExitAtFixed = true;
    private int ExitTarget = 1;
    private int UseRevTrig = 1;
    private double ThrustPct = 50.0;
    private double TrailerMult = 0.5;
    private double MMPct = 50.0;
    private double MMLockInMult = 1.0;
    private int TrailerLength = 9;
    private int IncrLen = 14;
    private double IncrX = 1.0;
    private int MktSentmntLength = 34;
    private bool UseFullCross = true;
    private int Target1or2 = 1;
    private double TrigStopMult = 0.5;
    private double TrigTgtMult = 1.0;
    private double TrigTgt2Mult = 2.0;
    private bool UseMktSentiment = true;
    private int ProximityTics = 1;
    private double StopMult = 1.0;
    private double TgtMult = 1.0;
    private double IslandMult = 1.0;
    private int BOMBDATE = 22130630;
    private bool i16_UseRevTrig;
    private bool i17_UsePullback;
    private bool i31_UseMMStop;
    private bool i51_UseAlert;
    private double PctR;
    private bool PctROB;
    private bool PctROS;
    private bool CloseAbove;
    private bool CloseBelow;
    private bool CloseAboveLast;
    private bool CloseBelowLast;
    private int UseRevStop;
    private bool PlotRevAlways;
    private int UseThrustReentry;
    private int UseBkevenStop;
    private bool UseTrailing;
    private double TrailerMMPct;
    private int UseMMStop;
    private double Proximity;
    private int MOMTargeting;
    private int UseTrigBarStop;
    private bool UseIslandExit;
    private double IslandMMPct;
    private int NextPB;
    private bool PullbackOK;
    private int NSDir;
    private double NSEntry;
    private double NSTgt;
    private double NSTgt2;
    private double NSStop;
    private int SDir;
    private double SEntry;
    private double STgt;
    private double STgt2;
    private double SStop;
    private double SStmt;
    private int TDir;
    private double TEntry;
    private double TTgt;
    private double TTgt2;
    private double TStop;
    private double TTgtA;
    private double TTgt2A;
    private double TTgtB;
    private double TTgt2B;
    private double TStop2;
    private double TMM;
    private double TIMM;
    private double BMM;
    private double MMM;
    private double BaseMult;
    private DataSeries Trail;
    private int NeedPB;
    private bool Pullback;
    private double SHH;
    private double SLL;
    private double RevEFX;
    private double RevENFX;
    private double ReEFX;
    private double ReENFX;
    private double Val1;
    private DataSeries Val2;
    private double Val3;
    private DataSeries Val3a;
    private DataSeries MktStmt;
    private double Value1;
    private double Value2;
    private bool Condition1;
    private bool Condition2;
    private bool Condition3;
    private bool Condition4;
    private bool Condition5;
    private bool Condition6;
    private bool condition31;
    private int SDirLast;
    private int TDirLast;
    private double TStopLast;
    private double RevEFXLast;
    private double RevENFXLast;
    private bool securityValid;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			this.Add(new Plot(Color.FromKnownColor(KnownColor.DarkCyan), (PlotStyle) 2, "Entry"));
			this.Add(new Plot(Color.FromKnownColor(KnownColor.DarkRed), (PlotStyle) 3, "Stop"));
			this.Add(new Plot(Color.FromKnownColor(KnownColor.Green), (PlotStyle) 3, "Target1"));
			this.Add(new Plot(Color.FromKnownColor(KnownColor.Green), (PlotStyle) 3, "Target2"));
			this.Add(new Plot(Color.FromKnownColor(KnownColor.DarkRed), (PlotStyle) 3, "AStop"));
			this.Add(new Plot(Color.FromKnownColor(KnownColor.Green), (PlotStyle) 3, "ATgt1"));
			this.Add(new Plot(Color.FromKnownColor(KnownColor.Green), (PlotStyle) 3, "ATgt2"));
			this.Overlay = true;
			this.Plots[0].Min = 0.0;
			this.Plots[1].Min = 0.0;
			this.Plots[2].Min = 0.0;
			this.Plots[3].Min = 0.0;
			this.Plots[4].Min = 0.0;
			this.Plots[5].Min = 0.0;
			this.Plots[6].Min = 0.0;
			this.EntryMult = this.i01_EntryMult;
			this.SetupStopMult = this.i02_StopMult;
			this.SetupTgtMult = this.i03_Tgt1Mult;
			this.SetupTgt2Mult = this.i04_Tgt2Mult;
			this.ExitAtFixed = this.i11_ExitAtTarget;
			this.ExitTarget = this.i12_ExitTgt;
			this.UseRevTrig = !this.i16_UseRevTrig ? 0 : 1;
			this.UseThrustReentry = !this.i17_UsePullback ? 0 : 1;
			this.ThrustPct = this.i18_PullbackPct;
			this.UseBkevenStop = !this.i21_UseBkevenStop ? 0 : 1;
			this.UseTrailing = this.i26_UseTrailStop;
			this.TrailerMMPct = this.i27_TrailMMPct;
			this.TrailerMult = this.i28_TrailMult;
			this.UseMMStop = !this.i31_UseMMStop ? 0 : 1;
			this.MMPct = this.i32_MMPct;
			this.MMLockInMult = this.i33_MMLockInMult;
			this.TrailerLength = this.i41_TrailLength;
			this.IncrLen = this.i42_IncrLength;
			this.IncrX = this.i43_IncrX;
			this.MktSentmntLength = this.i44_SentimentLength;
			this.PctRLen = this.j01_PctRLen;
			this.OBLevel = this.j02_OBLevel;
			this.OBReset = this.j03_OBReset;
			this.OSLevel = this.j04_OSLevel;
			this.OSReset = this.j05_OSReset;
			this.Val2 = new DataSeries((IndicatorBase) this);
			this.Val3a = new DataSeries((IndicatorBase) this);
			this.Trail = new DataSeries((IndicatorBase) this);
			this.MktStmt = new DataSeries((IndicatorBase) this);
			this.BOMBDATE = this.Instrument.MasterInstrument.Name == "EEM" || this.Instrument.MasterInstrument.Name == "AAPL" || this.Instrument.MasterInstrument.Name == "DIA" || (this.Instrument.MasterInstrument.Name == "GOOGL" || this.Instrument.MasterInstrument.Name == "NFLX") ? 22131231 : 20000101;
			this.securityValid = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			//if (!this.securityValid || this.Time[0].Year * 10000 + this.Time[0].Month * 100 + this.Time[0].Day > this.BOMBDATE || this.CurrentBar <= 1)
			//	return;
			if (this.FirstTickOfBar)
			{
				this.SDirLast = this.SDir;
				this.TDirLast = this.TDir;
				this.TStopLast = this.TStop;
				this.RevEFXLast = this.RevEFX;
				this.RevENFXLast = this.RevENFX;
				this.CloseAboveLast = this.CloseAbove;
				this.CloseBelowLast = this.CloseBelow;
			}
			this.Val1 = (this.High[0] - this.Low[0]) / 2.0;
			this.Val2[0] = this.High[0] - this.Val1;
			this.Val3a[0] = this.EMA((IDataSeries) this.Val2, this.MktSentmntLength)[0];
			this.Val3 = this.EMA((IDataSeries) this.Val3a, this.MktSentmntLength)[0];
			if (this.Val3 < this.Close[0])
				this.condition31 = false;
			else if (this.Val3 > this.Close[0])
				this.condition31 = true;
			if (!this.condition31)
				this.MktStmt[0] = 1.0;
			if (this.condition31)
				this.MktStmt[0] = (-1.0);
			if (this.CurrentBar == 2)
			{
				// No idea what 2 and 11 are
				//if (this.BarsPeriod.Id == 2 || this.BarsPeriod.Id == 11)
				if (this.BarsPeriod.Id == (PeriodType) 2 || this.BarsPeriod.Id == (PeriodType) 11)
				this.BaseMult = (double) this.BarsPeriod.Value * this.TickSize;
				this.Proximity = (double) this.ProximityTics * this.TickSize;
			}
			if (this.BarsPeriod.Id != (PeriodType) 2 && this.BarsPeriod.Id != (PeriodType) 11)
				this.BaseMult = this.IncrX * this.ATR(this.IncrLen)[0];
			this.Trail[0] = this.DonchianChannel(this.TrailerLength)[0];
			if (this.NeedPB != 0)
			{
				if (this.NeedPB == 1 && this.Low[0] - this.Trail[0] < this.Proximity)
				this.Pullback = true;
				if (this.NeedPB == -1 && this.Trail[0] - this.High[0] < this.Proximity)
				this.Pullback = true;
			}
			if (this.RevENFX != 0.0)
			{
				if (this.NextPB == 1 && (this.UseRevTrig == 1 && this.Low[0] <= this.RevENFX || this.UseRevTrig != 1))
				{
				this.PullbackOK = true;
				this.ReEFX = 0.0;
				this.ReENFX = 0.0;
				}
				if (this.NextPB == -1 && (this.UseRevTrig == 1 && this.High[0] >= this.RevENFX || this.UseRevTrig != 1))
				{
				this.PullbackOK = true;
				this.ReEFX = 0.0;
				this.ReENFX = 0.0;
				}
			}
			if (this.RevEFX != 0.0)
			{
				if (this.NextPB == 1 && (this.UseRevTrig == 1 && this.Low[0] <= this.RevEFX || this.UseRevTrig != 1))
				{
				this.PullbackOK = true;
				this.ReEFX = 0.0;
				this.ReENFX = 0.0;
				}
				if (this.NextPB == -1 && (this.UseRevTrig == 1 && this.High[0] >= this.RevEFX || this.UseRevTrig != 1))
				{
				this.PullbackOK = true;
				this.ReEFX = 0.0;
				this.ReENFX = 0.0;
				}
			}
			if (this.MktStmt[0] == 1.0 && this.ReEFX != 0.0 && this.High[0] >= this.ReEFX)
				this.ReEFX = 0.0;
			if (this.MktStmt[0] == 1.0 && this.ReENFX != 0.0 && this.High[0] >= this.ReENFX)
				this.ReENFX = 0.0;
			if (this.MktStmt[0] == -1.0 && this.ReEFX != 0.0 && this.Low[0] <= this.ReEFX)
				this.ReEFX = 0.0;
			if (this.MktStmt[0] == -1.0 && this.ReENFX != 0.0 && this.Low[0] <= this.ReENFX)
				this.ReENFX = 0.0;
			if (this.TDir == 1 && this.SDir == -1 && this.MktStmt[0] == 1.0)
				this.SDir = 0;
			if (this.TDir == -1 && this.SDir == 1 && this.MktStmt[0] == -1.0)
				this.SDir = 0;
			if (this.SDir != 0 && (this.SDir == 1 && this.High[0] >= this.SEntry || this.SDir == -1 && this.Low[0] <= this.SEntry))
			{
				this.TDir = this.SDir;
				this.TEntry = this.SEntry;
				this.TTgtA = this.STgt;
				this.TTgt2A = this.STgt2;
				this.TStop = this.SStop;
				if (this.SDir == 1)
				{
				this.NextPB = 1;
				this.TTgtB = this.SEntry + this.Instrument.MasterInstrument.Round2TickSize(this.TrigTgtMult * (this.SEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
				this.TTgt2B = this.SEntry + this.Instrument.MasterInstrument.Round2TickSize(this.TrigTgt2Mult * (this.SEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
				this.TStop2 = this.Instrument.MasterInstrument.Round2TickSize(this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.TrigStopMult * this.BaseMult);
				if (this.MOMTargeting == 0)
					this.TTgtB = this.SEntry + this.Instrument.MasterInstrument.Round2TickSize(this.TrigTgtMult * (this.SEntry - this.Low[0]));
				}
				if (this.SDir == -1)
				{
				this.NextPB = -1;
				this.TTgtB = this.SEntry - this.Instrument.MasterInstrument.Round2TickSize(this.TrigTgtMult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.SEntry));
				this.TTgt2B = this.SEntry - this.Instrument.MasterInstrument.Round2TickSize(this.TrigTgt2Mult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.SEntry));
				this.TStop2 = this.Instrument.MasterInstrument.Round2TickSize(this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) + this.TrigStopMult * this.BaseMult);
				if (this.MOMTargeting == 0)
					this.TTgtB = this.SEntry - this.Instrument.MasterInstrument.Round2TickSize(this.TrigTgtMult * (this.High[0] - this.SEntry));
				}
				if (this.Target1or2 == 1)
				{
				this.TTgt = this.STgt;
				this.TTgt2 = this.STgt2;
				this.TStop = this.SStop;
				}
				else
				{
				this.TTgt = this.TTgtB;
				this.TTgt2 = this.TTgt2B;
				this.TStop = this.TStop2;
				}
				if (this.UseTrigBarStop == 1)
				{
				if (this.TDir == 1)
					this.TStop = this.Low[0] - this.Instrument.MasterInstrument.Round2TickSize(this.TrigStopMult * this.BaseMult);
				if (this.TDir == -1)
					this.TStop = this.High[0] + this.Instrument.MasterInstrument.Round2TickSize(this.TrigStopMult * this.BaseMult);
				}
				if (this.TDir == 1)
				this.TMM = this.Instrument.MasterInstrument.Round2TickSize(this.TEntry + (this.TTgt - this.TEntry) * this.TrailerMMPct / 100.0);
				if (this.TDir == -1)
				this.TMM = this.Instrument.MasterInstrument.Round2TickSize(this.TEntry - (this.TEntry - this.TTgt) * this.TrailerMMPct / 100.0);
				if (this.TDir == 1)
				this.TIMM = this.Instrument.MasterInstrument.Round2TickSize(this.TEntry + (this.TTgt - this.TEntry) * this.IslandMMPct / 100.0);
				if (this.TDir == -1)
				this.TIMM = this.Instrument.MasterInstrument.Round2TickSize(this.TEntry - (this.TEntry - this.TTgt) * this.IslandMMPct / 100.0);
				if (this.TDir == 1)
				this.MMM = this.Instrument.MasterInstrument.Round2TickSize(this.TTgt + (this.TTgt2 - this.TTgt) * this.MMPct / 100.0);
				if (this.TDir == -1)
				this.MMM = this.Instrument.MasterInstrument.Round2TickSize(this.TTgt - (this.TTgt - this.TTgt2) * this.MMPct / 100.0);
				if ((double) this.SDir != this.SStmt)
				{
				this.NeedPB = this.SDir;
				this.Pullback = false;
				}
				else
				this.NeedPB = 0;
				this.SDir = 0;
			}
			if (this.NextPB == 1 && this.Condition2 && this.SDir == 1)
				this.SDir = 0;
			if (this.NextPB == -1 && this.Condition1 && this.SDir == -1)
				this.SDir = 0;
			if (this.TDir == 1)
			{
				if (this.ExitAtFixed && this.ExitTarget == 1 && this.High[0] >= this.TTgt)
				this.TDir = 0;
				if (this.ExitAtFixed && this.ExitTarget == 2 && this.High[0] >= this.TTgt2)
				this.TDir = 0;
				if (this.Low[0] <= this.TStop)
				this.TDir = 0;
				if (this.UseTrailing)
				{
				if (this.TMM != -1.0 && this.High[0] >= this.TMM)
					this.TMM = -1.0;
				if (this.TMM == -1.0 && this.TStop < this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.TrailerMult * this.BaseMult)
					this.TStop = this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.TrailerMult * this.BaseMult;
				}
				if (this.UseIslandExit)
				{
				if (this.TIMM != -1.0 && this.High[0] >= this.TIMM)
					this.TIMM = -1.0;
				if (this.TIMM == -1.0 && this.High[0] < this.Trail[0] && this.TStop < this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.IslandMult * this.BaseMult))
					this.TStop = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.IslandMult * this.BaseMult);
				}
				if (this.UseBkevenStop == 1 && this.High[0] >= this.TTgt && this.TStop < this.TEntry)
				this.TStop = this.TEntry;
				if (this.UseMMStop == 1)
				{
				if (this.MMM != -1.0 && this.High[0] >= this.MMM)
					this.MMM = -1.0;
				if (this.MMM == -1.0 && this.TStop < this.Instrument.MasterInstrument.Round2TickSize(this.TEntry + this.MMLockInMult * this.BaseMult))
					this.TStop = this.Instrument.MasterInstrument.Round2TickSize(this.TEntry + this.MMLockInMult * this.BaseMult);
				}
			}
			if (this.TDir == -1)
			{
				if (this.ExitAtFixed && this.ExitTarget == 1 && this.Low[0] <= this.TTgt)
				this.TDir = 0;
				if (this.ExitAtFixed && this.ExitTarget == 2 && this.Low[0] <= this.TTgt2)
				this.TDir = 0;
				if (this.High[0] >= this.TStop)
				this.TDir = 0;
				if (this.UseTrailing)
				{
				if (this.TMM != -1.0 && this.Low[0] <= this.TMM)
					this.TMM = -1.0;
				if (this.TMM == -1.0 && this.TStop > this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) + this.TrailerMult * this.BaseMult)
					this.TStop = this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) + this.TrailerMult * this.BaseMult;
				}
				if (this.UseIslandExit)
				{
				if (this.TIMM != -1.0 && this.Low[0] <= this.TIMM)
					this.TIMM = -1.0;
				if (this.TIMM == -1.0 && this.Low[0] > this.Trail[0] && this.TStop > this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.IslandMult * this.BaseMult))
					this.TStop = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.IslandMult * this.BaseMult);
				}
				if (this.UseBkevenStop == 1 && this.Low[0] <= this.TTgt && this.TStop > this.TEntry)
				this.TStop = this.TEntry;
				if (this.UseMMStop == 1)
				{
				if (this.MMM != -1.0 && this.Low[0] <= this.MMM)
					this.MMM = -1.0;
				if (this.MMM == -1.0 && this.TStop > this.Instrument.MasterInstrument.Round2TickSize(this.TEntry - this.MMLockInMult * this.BaseMult))
					this.TStop = this.Instrument.MasterInstrument.Round2TickSize(this.TEntry - this.MMLockInMult * this.BaseMult);
				}
			}
			this.Value1 = (this.Trail[0] - this.Low[0]) * 100.0 / Math.Max(this.Range()[0], this.TickSize);
			this.Value2 = (this.High[0] - this.Trail[0]) * 100.0 / Math.Max(this.Range()[0], this.TickSize);
			this.Condition1 = this.CrossAbove(this.Close[0], (IDataSeries) this.Trail, 1);
			this.Condition2 = this.CrossBelow(this.Close[0], (IDataSeries) this.Trail, 1);
			this.Condition3 = this.Close[0] == this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) && this.Close[1] < this.Instrument.MasterInstrument.Round2TickSize(this.Trail[1]);
			this.Condition4 = this.Close[0] == this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) && this.Close[1] > this.Instrument.MasterInstrument.Round2TickSize(this.Trail[1]);
			this.Condition5 = this.UseThrustReentry == 1 && this.Close[0] >= this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) && this.Close[1] > this.Instrument.MasterInstrument.Round2TickSize(this.Trail[1]) && this.Value1 >= this.ThrustPct;
			this.Condition6 = this.UseThrustReentry == 1 && this.Close[0] <= this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) && this.Close[1] < this.Instrument.MasterInstrument.Round2TickSize(this.Trail[1]) && this.Value2 >= this.ThrustPct;
			if (this.CrossAbove((IDataSeries) this.MktStmt, 0.0, 1))
			{
				if (this.TDir == 0)
				this.NextPB = 0;
				this.PullbackOK = false;
				this.NSDir = 1;
				this.NSEntry = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.EntryMult * this.BaseMult);
				this.NSTgt = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				this.NSTgt2 = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
				this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.SetupStopMult * this.BaseMult);
				this.ReEFX = 0.0;
				this.ReENFX = 0.0;
				this.RevEFX = 0.0;
				this.RevENFX = 0.0;
			}
			if (this.CrossBelow((IDataSeries) this.MktStmt, 0.0, 1))
			{
				if (this.TDir == 0)
				this.NextPB = 0;
				this.PullbackOK = false;
				this.NSDir = -1;
				this.NSEntry = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.EntryMult * this.BaseMult);
				this.NSTgt = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				this.NSTgt2 = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
				this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.SetupStopMult * this.BaseMult);
				this.ReEFX = 0.0;
				this.ReENFX = 0.0;
				this.RevEFX = 0.0;
				this.RevENFX = 0.0;
			}
			if (this.NextPB == 1 && this.Condition2)
			{
				this.PullbackOK = false;
				this.RevEFX = this.Instrument.MasterInstrument.Round2TickSize(this.Low[1] - this.EntryMult * this.BaseMult);
			}
			if (this.NextPB == -1 && this.Condition1)
			{
				this.PullbackOK = false;
				this.RevEFX = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.EntryMult * this.BaseMult);
			}
			if (this.NextPB == 1 && this.Condition4)
			{
				this.PullbackOK = false;
				this.RevENFX = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.EntryMult * this.BaseMult);
			}
			if (this.NextPB == -1 && this.Condition3)
			{
				this.PullbackOK = false;
				this.RevENFX = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.EntryMult * this.BaseMult);
			}
			if (this.NextPB == 1 && this.Close[0] > this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]))
				this.RevEFX = 0.0;
			if (this.NextPB == -1 && this.Close[0] < this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]))
				this.RevEFX = 0.0;
			if (this.NextPB == 1 && this.Close[0] > this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]))
				this.RevENFX = 0.0;
			if (this.NextPB == -1 && this.Close[0] < this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]))
				this.RevENFX = 0.0;
			if (this.UseRevTrig == 1 || this.PlotRevAlways)
			{
				if (this.RevEFX != 0.0)
				this.REX[0] = (this.RevEFX);
				if (this.RevENFX != 0.0)
				this.RENX[0] = (this.RevENFX);
			}
			if (this.PullbackOK)
			{
				if (this.NextPB == 1 && this.Condition1)
				this.ReEFX = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.EntryMult * this.BaseMult);
				if (this.NextPB == -1 && this.Condition2)
				this.ReEFX = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.EntryMult * this.BaseMult);
				if (this.NextPB == 1 && this.Condition3)
				this.ReENFX = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.EntryMult * this.BaseMult);
				if (this.NextPB == -1 && this.Condition4)
				this.ReENFX = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.EntryMult * this.BaseMult);
				if (this.NextPB == 1 && (this.Condition1 || this.Condition3) && this.MktStmt[0] == 1.0)
				{
				if (this.ReEFX != 0.0)
				{
					this.NSEntry = this.ReEFX;
					this.NSDir = 1;
					this.NSTgt = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * (this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
					this.NSTgt2 = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * (this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
					this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.SetupStopMult * this.BaseMult);
				}
				if (this.ReENFX != 0.0)
				{
					this.NSEntry = this.ReENFX;
					this.NSDir = 1;
					this.NSTgt = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * (this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
					this.NSTgt2 = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * (this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
					this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.SetupStopMult * this.BaseMult);
				}
				if (this.ReEFX != 0.0 && this.ReENFX != 0.0)
				{
					this.NSEntry = Math.Min(this.ReEFX, this.ReENFX);
					this.NSDir = 1;
					this.NSTgt = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * (this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
					this.NSTgt2 = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * (this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0])));
					this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.SetupStopMult * this.BaseMult);
				}
				if (this.UseRevStop == 1 && (this.RevEFXLast != 0.0 || this.RevENFXLast != 0.0))
				{
					if (this.RevEFXLast != 0.0)
					this.NSStop = Math.Max(this.NSStop, this.RevEFXLast);
					if (this.RevENFXLast != 0.0)
					this.NSStop = Math.Max(this.NSStop, this.RevENFXLast);
					if (this.RevEFXLast != 0.0 && this.RevENFX != 0.0)
					this.NSStop = Math.Max(this.NSStop, Math.Max(this.RevEFXLast, this.RevENFXLast));
				}
				}
				if (this.NextPB == -1 && (this.Condition2 || this.Condition4) && this.MktStmt[0] == -1.0)
				{
				if (this.ReEFX != 0.0)
				{
					this.NSEntry = this.ReEFX;
					this.NSDir = -1;
					this.NSTgt = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.NSEntry));
					this.NSTgt2 = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.NSEntry));
					this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.SetupStopMult * this.BaseMult);
				}
				if (this.ReENFX != 0.0)
				{
					this.NSEntry = this.ReENFX;
					this.NSDir = -1;
					this.NSTgt = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.NSEntry));
					this.NSTgt2 = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.NSEntry));
					this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.SetupStopMult * this.BaseMult);
				}
				if (this.ReEFX != 0.0 && this.ReENFX != 0.0)
				{
					this.NSEntry = Math.Min(this.ReEFX, this.ReENFX);
					this.NSDir = -1;
					this.NSTgt = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.NSEntry));
					this.NSTgt2 = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * (this.Instrument.MasterInstrument.Round2TickSize(this.Trail[0]) - this.NSEntry));
					this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.SetupStopMult * this.BaseMult);
				}
				if (this.UseRevStop == 1 && (this.RevEFXLast != 0.0 || this.RevENFXLast != 0.0))
				{
					if (this.RevEFXLast != 0.0)
					this.NSStop = Math.Min(this.NSStop, this.RevEFXLast);
					if (this.RevENFXLast != 0.0)
					this.NSStop = Math.Min(this.NSStop, this.RevENFXLast);
					if (this.RevEFXLast != 0.0 && this.RevENFXLast != 0.0)
					this.NSStop = Math.Min(this.NSStop, Math.Min(this.RevEFXLast, this.RevENFXLast));
				}
				}
			}
			if (this.NextPB == 1 && this.Condition5 && this.MktStmt[0] == 1.0)
			{
				this.NSDir = 1;
				this.NSEntry = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.EntryMult * this.BaseMult);
				this.NSTgt = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				this.NSTgt2 = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
				this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.SetupStopMult * this.BaseMult);
			}
			if (this.NextPB == -1 && this.Condition6 && this.MktStmt[0] == -1.0)
			{
				this.NSDir = -1;
				this.NSEntry = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.EntryMult * this.BaseMult);
				this.NSTgt = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				this.NSTgt2 = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
				this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.SetupStopMult * this.BaseMult);
			}
			if (this.NextPB == 1 && this.Condition2 && this.SDir == 1)
				this.SDir = 0;
			if (this.NextPB == -1 && this.Condition1 && this.SDir == -1)
				this.SDir = 0;
			if (this.NSDir != 0)
				this.NSDir = 0;
			this.PctR = this.WilliamsR(this.PctRLen)[0];
			if (this.PctR >= this.OBLevel)
				this.PctROB = true;
			if (this.PctR < this.OBReset)
				this.PctROB = false;
			if (this.PctR <= this.OSLevel)
				this.PctROS = true;
			if (this.PctR > this.OSReset)
				this.PctROS = false;
			if (this.Close[0] > this.Trail[0])
			{
				this.CloseAbove = true;
				this.CloseBelow = false;
			}
			if (this.Close[0] < this.Trail[0])
			{
				this.CloseBelow = true;
				this.CloseAbove = false;
			}
			if (this.PctROS && this.CloseAbove && this.CloseBelowLast)
			{
				this.NSDir = 1;
				this.NSEntry = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.EntryMult * this.BaseMult);
				this.NSTgt = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				this.NSTgt2 = this.NSEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
				this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.SetupStopMult * this.BaseMult);
			}
			if (this.PctROB && this.CloseBelow && this.CloseAboveLast)
			{
				this.NSDir = -1;
				this.NSEntry = this.Instrument.MasterInstrument.Round2TickSize(this.Low[0] - this.EntryMult * this.BaseMult);
				this.NSTgt = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				this.NSTgt2 = this.NSEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
				this.NSStop = this.Instrument.MasterInstrument.Round2TickSize(this.High[0] + this.SetupStopMult * this.BaseMult);
			}
			if (this.CloseBelow && this.SDir == 1)
				this.SDir = 0;
			if (this.CloseAbove && this.SDir == -1)
				this.SDir = 0;
			if (this.TDir != 0)
			{
				if (this.SDir == this.TDir)
				this.SDir = 0;
				if (this.NSDir == this.TDir)
				this.NSDir = 0;
			}
			if (this.TDir != this.NSDir && this.SDir == 0 && this.NSDir != 0)
			{
				this.SDir = this.NSDir;
				this.SEntry = this.NSEntry;
				this.STgt = this.NSTgt;
				this.STgt2 = this.NSTgt2;
				this.SStop = this.NSStop;
				this.SStmt = this.MktStmt[0];
				this.SHH = this.High[0];
				this.SLL = this.Low[0];
				this.NSDir = 0;
			}
			if (this.SDir != 0 && this.NSDir != 0)
			{
				if (this.SDir != this.NSDir)
				{
				this.SDir = this.NSDir;
				this.SEntry = this.NSEntry;
				this.STgt = this.NSTgt;
				this.STgt2 = this.NSTgt2;
				this.SStop = this.NSStop;
				this.SStmt = this.MktStmt[0];
				this.SHH = this.High[0];
				this.SLL = this.Low[0];
				this.NSDir = 0;
				}
				if (this.SDir == this.NSDir)
				{
				if (this.SDir == 1 && this.NSEntry < this.SEntry || this.SDir == -1 && this.NSEntry > this.SEntry)
				{
					this.SEntry = this.NSEntry;
					this.STgt = this.NSTgt;
					this.STgt2 = this.NSTgt2;
					this.SStop = this.NSStop;
					this.SStmt = this.MktStmt[0];
				}
				else
					this.NSEntry = this.SEntry;
				this.NSDir = 0;
				}
			}
			if (this.MOMTargeting != 1 && this.PullbackOK)
			{
				if (this.SDir == this.SDirLast)
				{
				this.SHH = Math.Max(this.SHH, this.High[0]);
				this.SLL = Math.Min(this.SLL, this.Low[0]);
				}
				if (this.SDir == 1)
				this.STgt = this.SEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				if (this.SDir == 1)
				this.STgt2 = this.SEntry + this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
				if (this.SDir == -1)
				this.STgt = this.SEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgtMult * this.BaseMult);
				if (this.SDir == -1)
				this.STgt2 = this.SEntry - this.Instrument.MasterInstrument.Round2TickSize(this.SetupTgt2Mult * this.BaseMult);
			}
			if (this.UseMktSentiment && this.SDir != 0 && (double) this.SDir == this.MktStmt[0])
				this.SStmt = this.MktStmt[0];
			if (this.SDir != 0)
			{
				this.Entry[0] = (this.SEntry);
				this.Stop[0] = (this.SStop);
				this.Target1[0] = (this.STgt);
				this.Target2[0] = (this.STgt2);
			}
			if (this.TDir != 0)
			{
				this.AStop[0] = (this.TStop);
				this.ATgt1[0] = (this.TTgtA);
				this.ATgt2[0] = (this.TTgt2A);
			}
			if (this.i51_UseAlert && this.SDir != 0 && this.SDir != this.SDirLast)
				this.Alert("Alert", (Priority) 0, "New Setup", "Alert1.wav", 1, Color.Blue, Color.White);
			if (!this.i51_UseAlert || this.TDir == 0 || (this.TDir != this.TDirLast || this.TStop == this.TStopLast))
				return;
			this.Alert("Alert", (Priority) 0, "Move Stop", "Alert1.wav", 1, Color.Red, Color.White);
		}

        #region Properties
		
		[XmlIgnore]
		[Browsable(false)]
		public DataSeries Entry
		{
		get
		{
			return this.Values[0];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Stop
		{
		get
		{
			return this.Values[1];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Target1
		{
		get
		{
			return this.Values[2];
		}
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries Target2
		{
		get
		{
			return this.Values[3];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		public DataSeries AStop
		{
		get
		{
			return this.Values[4];
		}
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries ATgt1
		{
		get
		{
			return this.Values[5];
		}
		}

		[XmlIgnore]
		[Browsable(false)]
		public DataSeries ATgt2
		{
		get
		{
			return this.Values[6];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		private DataSeries REX
		{
		get
		{
			return this.Values[7];
		}
		}

		[Browsable(false)]
		[XmlIgnore]
		private DataSeries RENX
		{
		get
		{
			return this.Values[8];
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
		public double I01_EntryMul
		{
		get
		{
			return this.i01_EntryMult;
		}
		set
		{
			this.i01_EntryMult = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		public double I02_StopMult
		{
		get
		{
			return this.i02_StopMult;
		}
		set
		{
			this.i02_StopMult = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public double I03_Tgt1Mult
		{
		get
		{
			return this.i03_Tgt1Mult;
		}
		set
		{
			this.i03_Tgt1Mult = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		public double I04_Tgt2Mult
		{
		get
		{
			return this.i04_Tgt2Mult;
		}
		set
		{
			this.i04_Tgt2Mult = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool I11_ExitAtTarget
		{
		get
		{
			return this.i11_ExitAtTarget;
		}
		set
		{
			this.i11_ExitAtTarget = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I12_ExitTgt
		{
		get
		{
			return this.i12_ExitTgt;
		}
		set
		{
			this.i12_ExitTgt = Math.Max(1, value);
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private bool I16_UseRevTrig
		{
		get
		{
			return this.i16_UseRevTrig;
		}
		set
		{
			this.i16_UseRevTrig = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool I17_UsePullback
		{
		get
		{
			return this.i17_UsePullback;
		}
		set
		{
			this.i17_UsePullback = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private double I18_PullbackPct
		{
		get
		{
			return this.i18_PullbackPct;
		}
		set
		{
			this.i18_PullbackPct = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool I21_UseBkevenStop
		{
		get
		{
			return this.i21_UseBkevenStop;
		}
		set
		{
			this.i21_UseBkevenStop = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private bool I26_UseTrailStop
		{
		get
		{
			return this.i26_UseTrailStop;
		}
		set
		{
			this.i26_UseTrailStop = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private double I27_TrailMMPct
		{
		get
		{
			return this.i27_TrailMMPct;
		}
		set
		{
			this.i27_TrailMMPct = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private double I28_TrailMult
		{
		get
		{
			return this.i28_TrailMult;
		}
		set
		{
			this.i28_TrailMult = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private bool I31_UseMMStop
		{
		get
		{
			return this.i31_UseMMStop;
		}
		set
		{
			this.i31_UseMMStop = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private double I32_MMPct
		{
		get
		{
			return this.i32_MMPct;
		}
		set
		{
			this.i32_MMPct = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private double I33_MMLockInMult
		{
		get
		{
			return this.i33_MMLockInMult;
		}
		set
		{
			this.i33_MMLockInMult = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public int I41_TrailLength
		{
		get
		{
			return this.i41_TrailLength;
		}
		set
		{
			this.i41_TrailLength = Math.Max(1, value);
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		private int I42_IncrLength
		{
		get
		{
			return this.i42_IncrLength;
		}
		set
		{
			this.i42_IncrLength = Math.Max(1, value);
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private double I43_IncrX
		{
		get
		{
			return this.i43_IncrX;
		}
		set
		{
			this.i43_IncrX = Math.Max(0.0, value);
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		private int I44_SentimentLength
		{
		get
		{
			return this.i44_SentimentLength;
		}
		set
		{
			this.i44_SentimentLength = Math.Max(1, value);
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public bool I51_UseAlert
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

		[Description("")]
		[GridCategory("Parameters")]
		public int J01_PctRLen
		{
		get
		{
			return this.j01_PctRLen;
		}
		set
		{
			this.j01_PctRLen = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public double J02_OBLevel
		{
		get
		{
			return this.j02_OBLevel;
		}
		set
		{
			this.j02_OBLevel = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		public double J03_OBReset
		{
		get
		{
			return this.j03_OBReset;
		}
		set
		{
			this.j03_OBReset = value;
		}
		}

		[GridCategory("Parameters")]
		[Description("")]
		public double J04_OSLevel
		{
		get
		{
			return this.j04_OSLevel;
		}
		set
		{
			this.j04_OSLevel = value;
		}
		}

		[Description("")]
		[GridCategory("Parameters")]
		public double J05_OSReset
		{
		get
		{
			return this.j05_OSReset;
		}
		set
		{
			this.j05_OSReset = value;
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
        private PTUActiveSwingReversal[] cachePTUActiveSwingReversal = null;

        private static PTUActiveSwingReversal checkPTUActiveSwingReversal = new PTUActiveSwingReversal();

        /// <summary>
        /// Premier Trader University Active Swing Reversal indicator
        /// </summary>
        /// <returns></returns>
        public PTUActiveSwingReversal PTUActiveSwingReversal(string i00_Version, double i01_EntryMul, double i02_StopMult, double i03_Tgt1Mult, double i04_Tgt2Mult, int i41_TrailLength, bool i51_UseAlert, int j01_PctRLen, double j02_OBLevel, double j03_OBReset, double j04_OSLevel, double j05_OSReset)
        {
            return PTUActiveSwingReversal(Input, i00_Version, i01_EntryMul, i02_StopMult, i03_Tgt1Mult, i04_Tgt2Mult, i41_TrailLength, i51_UseAlert, j01_PctRLen, j02_OBLevel, j03_OBReset, j04_OSLevel, j05_OSReset);
        }

        /// <summary>
        /// Premier Trader University Active Swing Reversal indicator
        /// </summary>
        /// <returns></returns>
        public PTUActiveSwingReversal PTUActiveSwingReversal(Data.IDataSeries input, string i00_Version, double i01_EntryMul, double i02_StopMult, double i03_Tgt1Mult, double i04_Tgt2Mult, int i41_TrailLength, bool i51_UseAlert, int j01_PctRLen, double j02_OBLevel, double j03_OBReset, double j04_OSLevel, double j05_OSReset)
        {
            if (cachePTUActiveSwingReversal != null)
                for (int idx = 0; idx < cachePTUActiveSwingReversal.Length; idx++)
                    if (cachePTUActiveSwingReversal[idx].I00_Version == i00_Version && Math.Abs(cachePTUActiveSwingReversal[idx].I01_EntryMul - i01_EntryMul) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].I02_StopMult - i02_StopMult) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].I03_Tgt1Mult - i03_Tgt1Mult) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].I04_Tgt2Mult - i04_Tgt2Mult) <= double.Epsilon && cachePTUActiveSwingReversal[idx].I41_TrailLength == i41_TrailLength && cachePTUActiveSwingReversal[idx].I51_UseAlert == i51_UseAlert && cachePTUActiveSwingReversal[idx].J01_PctRLen == j01_PctRLen && Math.Abs(cachePTUActiveSwingReversal[idx].J02_OBLevel - j02_OBLevel) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].J03_OBReset - j03_OBReset) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].J04_OSLevel - j04_OSLevel) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].J05_OSReset - j05_OSReset) <= double.Epsilon && cachePTUActiveSwingReversal[idx].EqualsInput(input))
                        return cachePTUActiveSwingReversal[idx];

            lock (checkPTUActiveSwingReversal)
            {
                checkPTUActiveSwingReversal.I00_Version = i00_Version;
                i00_Version = checkPTUActiveSwingReversal.I00_Version;
                checkPTUActiveSwingReversal.I01_EntryMul = i01_EntryMul;
                i01_EntryMul = checkPTUActiveSwingReversal.I01_EntryMul;
                checkPTUActiveSwingReversal.I02_StopMult = i02_StopMult;
                i02_StopMult = checkPTUActiveSwingReversal.I02_StopMult;
                checkPTUActiveSwingReversal.I03_Tgt1Mult = i03_Tgt1Mult;
                i03_Tgt1Mult = checkPTUActiveSwingReversal.I03_Tgt1Mult;
                checkPTUActiveSwingReversal.I04_Tgt2Mult = i04_Tgt2Mult;
                i04_Tgt2Mult = checkPTUActiveSwingReversal.I04_Tgt2Mult;
                checkPTUActiveSwingReversal.I41_TrailLength = i41_TrailLength;
                i41_TrailLength = checkPTUActiveSwingReversal.I41_TrailLength;
                checkPTUActiveSwingReversal.I51_UseAlert = i51_UseAlert;
                i51_UseAlert = checkPTUActiveSwingReversal.I51_UseAlert;
                checkPTUActiveSwingReversal.J01_PctRLen = j01_PctRLen;
                j01_PctRLen = checkPTUActiveSwingReversal.J01_PctRLen;
                checkPTUActiveSwingReversal.J02_OBLevel = j02_OBLevel;
                j02_OBLevel = checkPTUActiveSwingReversal.J02_OBLevel;
                checkPTUActiveSwingReversal.J03_OBReset = j03_OBReset;
                j03_OBReset = checkPTUActiveSwingReversal.J03_OBReset;
                checkPTUActiveSwingReversal.J04_OSLevel = j04_OSLevel;
                j04_OSLevel = checkPTUActiveSwingReversal.J04_OSLevel;
                checkPTUActiveSwingReversal.J05_OSReset = j05_OSReset;
                j05_OSReset = checkPTUActiveSwingReversal.J05_OSReset;

                if (cachePTUActiveSwingReversal != null)
                    for (int idx = 0; idx < cachePTUActiveSwingReversal.Length; idx++)
                        if (cachePTUActiveSwingReversal[idx].I00_Version == i00_Version && Math.Abs(cachePTUActiveSwingReversal[idx].I01_EntryMul - i01_EntryMul) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].I02_StopMult - i02_StopMult) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].I03_Tgt1Mult - i03_Tgt1Mult) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].I04_Tgt2Mult - i04_Tgt2Mult) <= double.Epsilon && cachePTUActiveSwingReversal[idx].I41_TrailLength == i41_TrailLength && cachePTUActiveSwingReversal[idx].I51_UseAlert == i51_UseAlert && cachePTUActiveSwingReversal[idx].J01_PctRLen == j01_PctRLen && Math.Abs(cachePTUActiveSwingReversal[idx].J02_OBLevel - j02_OBLevel) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].J03_OBReset - j03_OBReset) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].J04_OSLevel - j04_OSLevel) <= double.Epsilon && Math.Abs(cachePTUActiveSwingReversal[idx].J05_OSReset - j05_OSReset) <= double.Epsilon && cachePTUActiveSwingReversal[idx].EqualsInput(input))
                            return cachePTUActiveSwingReversal[idx];

                PTUActiveSwingReversal indicator = new PTUActiveSwingReversal();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.I00_Version = i00_Version;
                indicator.I01_EntryMul = i01_EntryMul;
                indicator.I02_StopMult = i02_StopMult;
                indicator.I03_Tgt1Mult = i03_Tgt1Mult;
                indicator.I04_Tgt2Mult = i04_Tgt2Mult;
                indicator.I41_TrailLength = i41_TrailLength;
                indicator.I51_UseAlert = i51_UseAlert;
                indicator.J01_PctRLen = j01_PctRLen;
                indicator.J02_OBLevel = j02_OBLevel;
                indicator.J03_OBReset = j03_OBReset;
                indicator.J04_OSLevel = j04_OSLevel;
                indicator.J05_OSReset = j05_OSReset;
                Indicators.Add(indicator);
                indicator.SetUp();

                PTUActiveSwingReversal[] tmp = new PTUActiveSwingReversal[cachePTUActiveSwingReversal == null ? 1 : cachePTUActiveSwingReversal.Length + 1];
                if (cachePTUActiveSwingReversal != null)
                    cachePTUActiveSwingReversal.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePTUActiveSwingReversal = tmp;
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
        /// Premier Trader University Active Swing Reversal indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PTUActiveSwingReversal PTUActiveSwingReversal(string i00_Version, double i01_EntryMul, double i02_StopMult, double i03_Tgt1Mult, double i04_Tgt2Mult, int i41_TrailLength, bool i51_UseAlert, int j01_PctRLen, double j02_OBLevel, double j03_OBReset, double j04_OSLevel, double j05_OSReset)
        {
            return _indicator.PTUActiveSwingReversal(Input, i00_Version, i01_EntryMul, i02_StopMult, i03_Tgt1Mult, i04_Tgt2Mult, i41_TrailLength, i51_UseAlert, j01_PctRLen, j02_OBLevel, j03_OBReset, j04_OSLevel, j05_OSReset);
        }

        /// <summary>
        /// Premier Trader University Active Swing Reversal indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.PTUActiveSwingReversal PTUActiveSwingReversal(Data.IDataSeries input, string i00_Version, double i01_EntryMul, double i02_StopMult, double i03_Tgt1Mult, double i04_Tgt2Mult, int i41_TrailLength, bool i51_UseAlert, int j01_PctRLen, double j02_OBLevel, double j03_OBReset, double j04_OSLevel, double j05_OSReset)
        {
            return _indicator.PTUActiveSwingReversal(input, i00_Version, i01_EntryMul, i02_StopMult, i03_Tgt1Mult, i04_Tgt2Mult, i41_TrailLength, i51_UseAlert, j01_PctRLen, j02_OBLevel, j03_OBReset, j04_OSLevel, j05_OSReset);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Premier Trader University Active Swing Reversal indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PTUActiveSwingReversal PTUActiveSwingReversal(string i00_Version, double i01_EntryMul, double i02_StopMult, double i03_Tgt1Mult, double i04_Tgt2Mult, int i41_TrailLength, bool i51_UseAlert, int j01_PctRLen, double j02_OBLevel, double j03_OBReset, double j04_OSLevel, double j05_OSReset)
        {
            return _indicator.PTUActiveSwingReversal(Input, i00_Version, i01_EntryMul, i02_StopMult, i03_Tgt1Mult, i04_Tgt2Mult, i41_TrailLength, i51_UseAlert, j01_PctRLen, j02_OBLevel, j03_OBReset, j04_OSLevel, j05_OSReset);
        }

        /// <summary>
        /// Premier Trader University Active Swing Reversal indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.PTUActiveSwingReversal PTUActiveSwingReversal(Data.IDataSeries input, string i00_Version, double i01_EntryMul, double i02_StopMult, double i03_Tgt1Mult, double i04_Tgt2Mult, int i41_TrailLength, bool i51_UseAlert, int j01_PctRLen, double j02_OBLevel, double j03_OBReset, double j04_OSLevel, double j05_OSReset)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PTUActiveSwingReversal(input, i00_Version, i01_EntryMul, i02_StopMult, i03_Tgt1Mult, i04_Tgt2Mult, i41_TrailLength, i51_UseAlert, j01_PctRLen, j02_OBLevel, j03_OBReset, j04_OSLevel, j05_OSReset);
        }
    }
}
#endregion
