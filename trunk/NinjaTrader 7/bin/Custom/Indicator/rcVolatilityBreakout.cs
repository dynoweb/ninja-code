// Type: NinjaTrader.Indicator.rcVolatilityBreakout
// Assembly: rcVolatilityBreakout, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 44D1D67D-B887-419E-93CB-54BC561A67EC
// Assembly location: C:\Users\rcromer\Documents\NinjaTrader 7\bin\Custom\rcVolatilityBreakout.dll

using A;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NinjaTrader.Indicator
{
  [Description(".")]
  public class rcVolatilityBreakout : Indicator
  {
    private int c1236bb80f125d2f64ede5d3554357b5e = 20;
    private double cf1446b47f2a286dd76c812e90dba61d8 = 2.0;
    private double c1430534891381b1f402f44f5c2bc45e6 = 1.5;
    private int cfe10c42f2805322d3451773938db24cf = 20;
    private Color c5e5ce75e928c7bcfe3104cb990dd6c94 = Color.Red;
    private Color c89334362fa1442d01d73b12d814ac7b2 = Color.Gray;
    private Color cd1221e8f166fdf0377235ae1eb1dd196 = Color.Lime;
    private Color c31651e18bc9d15393b0aa39095aec8dd = Color.DarkGreen;
    private Color c91bacb213c03d2aeabed3e1aa0dcb0e2 = Color.Red;
    private Color cb85460748cebb6f8d2ab7050809ea203 = Color.Maroon;
    private Color c0ad11e24cc4da365aece7bcf01a83671 = Color.DarkGreen;
    private Color c23fef937e4704665e16beb48327747e4 = Color.DarkRed;
    private int c1e21aef88f993ac837b826bb003a40e9 = 2;
    private int cfcd0213a6595b2e6e51ba676055690b4 = 5;
    private string c1d9289d3109d23c6bbe8ad91f54337a4 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1971);
    private List<IRectangle> c8e3bfc379648afc881c5b89cf9059f8a = new List<IRectangle>();
    private Dictionary<ILine, Color> c9ffb5283db4a57c6db0dc6feb7f764a0 = new Dictionary<ILine, Color>();
    private Dictionary<IText, Color> c36681af401b51ffcf73e4f48f976ba85 = new Dictionary<IText, Color>();
    private int[] c253aeae02261ab94a049d7a01dfe975e = new int[8];
    private int[] cba308ffc4c055fde426f73bdfd36e1e9 = new int[8];
    [XmlIgnore]
    private Plot c52fc7d455698dfea6cd86aaf0c23f658 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1974));
    private string c2d383bd69d6069688bcde2389f05aa78 = "";
    private int c88126af217b4132a66884925bcfffe67 = 30;
    private bool c0976f285618fa2f72c6a842d6e22d537 = true;
    private bool cdecf2853d16d3ae1374d69d0b63a35dc = true;
    [XmlIgnore]
    private Plot cd996e9dad932d32bc398d3e19f4bd922 = new Plot(new Pen(Color.Green, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1995));
    private bool cf802e897a8ec7a9a0dc5a33042c7fef0 = true;
    [XmlIgnore]
    private Plot c5f155ce944c1fc760117c90aa5fe632a = new Plot(new Pen(Color.Red, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2010));
    private bool c8c666df1647da5e0be2372b16be0da70 = true;
    [XmlIgnore]
    private Plot ccb7a3d16e77f50cb570e2b19b247ae9d = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2023));
    private int cb910efb6746af270cfa63ea6119b308a = 6;
    private int c22eda7b2c48a3c51b10ee578e7a98723 = 20;
    private Color c639e17861f64166b7377685b42faf5a7 = Color.Orange;
    private int cc9ca3141d96e48b218961a6a4a6c2e83 = 1;
    private bool c51a299b74e78d9d24e6a4a54ef2098f0 = true;
    private string cef78ed9a48e530a67243d70a0e6e03c6 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1211);
    private Color c93c4eee6215a4b05789c445423766398 = Color.Black;
    private string c01ec28e16876787b89a667457c9913a2 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2042);
    private bool c8bf6fdf4f7970894cd74c89a895cd299 = true;
    private bool c158d34f4cd6b3109a4d4134b5f26402f = true;
    private bool cbdda209ad9e84f19ec587ff0f51b194c = true;
    private int c132301e69bc69471bfc8126ab78337ee = 50;
    private int c68a4c407a5177fa72b3e372f4987e9f0 = 100;
    private int c39ad57272823a0eb86f3d3a88ce55689 = 200;
    [XmlIgnore]
    private Plot c635f754ce1a2c575c614ed5d8911b08e = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2045));
    [XmlIgnore]
    private Plot c8b22b97d5b299c02316d6dfa8981e1c6 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2082));
    [XmlIgnore]
    private Plot c86ffca2cd49015fb08b7eaceda83c211 = new Plot(new Pen(Color.Gray, 1f), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2119));
    private string ca9612a794be4fa61c2b14eaad35d59d8 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string c267485c17bfb8b21cee1f3704b0944ee = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string c99a78e50b34fcbd1b9e091467d576e8c = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string ccef9f98c4d79b41597db979d63c79cb4 = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private string c4b685b34ba086b4f8e622571ce57ea1a = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2156);
    private DataSeries dataSeries1;
    private DataSeries dataSeries2;
    private double ce532046b6b6b789cfb4e83dd4ded26e8;
    private double c4401083d3066e091774c61d40eed01ad;
    private double c3989dca8716e205f1ce35948cff57ac9;
    private double ce0ad2a035fced4d4b79ef3da6ec927ae;
    private double c5e0958df8f76b18b2ab9293382ae3d14;
    private double cb5ff3849b5f0c4d175ca3bba67c0b947;
    private double c37c8eb5e2c1b5ca77a4987ec1f116d56;
    private double cd6bc814fcea78d4e2c478b3e42c0a050;
    private double ca5ad4acedee8387a0d09b08e94ffd438;
    private double c95e8e518faa568cb6142f92d98772c66;
    private double cb638b9cc732de8bf65b6dbeeb52281d1;
    private double c98884df07a511384f7ab4afade318e44;
    private int c84334302133d3436d40fc5c5c9cd118a;
    private int c703c1b4d798d0f3074ba94d947ccca39;
    private ToolStripDropDownButton c77da1ce00f4d2cf1d739478d471b601e;
    private int ced563a5e9c3c71140c2e62f63afc998a;
    private Font c92b65488e202ae89a88b76ef9a4ce637;
    private Color cff629cbfa130d54186b496dc143b8484;
    private Color cfaf31b4c1fe0e36d277e224a6998ed89;
    private Image c497f3eec1deaf291fc952d3f44b5ad9c;
    private rcVolatilityBreakout.hitPanel cfd0f789956fddac7d881844489c6bb27;
    private int c70fc78f80bfde9358a1dc14ea234aa72;
    private int c92c86a6ca3a9f35bfb8f9042e2d54bbb;
    private int cdcf0ce7396813d4bd126a0b9fa8fea53;
    private double cfb105a9d24e167ceec6191571a1a8e43;
    private double c739d1f8b1109fe6f2de5bc1ec3c7d232;
    private ToolStripSeparator c457facdc3769e1f720cb334357b8307a;
    private ToolStripButton cbee666d482be9f7ba3d140408b89d9fb;
    private IRectangle cbdbec797f08dfdb6d31d4a0781088769;
    private ILine c2adc4d144eb4d1c02b1418f0ef7bdd15;
    private IText c63b68670261102fc9a5683b8ef26ddae;
    private int c6c9ad6a793905e213eacafa1be1fb516;
    private double c133da4068b26ce316e9f7ed309997e74;
    private double c950bfc49088216da2d12e2c56be386df;
    private int c13d059c56853219888b6d45fccc6b9e1;
    private bool ceafa24ab078c9719b466e842e5f1306f;
    private bool cc5dbd81508adf50cd50c8e46262b80d5;
    private bool ccfe28103c8fb75a8c45718f709797bbc;

	protected override void Initialize()      
	{
      Add(new Plot(Color.Transparent, (PlotStyle) 3, "Squeeze"));  // 0 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(839)
      Add(new Plot(Color.Transparent, (PlotStyle) 3, "SqueezeOn"));  // 1 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(854)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "PMomentumUp"));  // 2 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(873)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "PMomentumDown"));  // 3 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(896)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "NMomentumUp"));  // 4 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(923)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "NMomentumDown"));  // 5 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(946)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "CurrentValue"));  // 6 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(973)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "BarsSinceSqueeze"));  // 7 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(998)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeLength"));  // 8 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1031)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeHigh"));  // 9 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1058)
      Add(new Plot(Color.Transparent, (PlotStyle) 0, "SqueezeLow"));  // 10 - cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1081)
      dataSeries1 = new DataSeries((IndicatorBase) this);
      dataSeries2 = new DataSeries((IndicatorBase) this);
      CalculateOnBarClose = false;
      Overlay = true;
      PriceTypeSupported = false;
      PlotsConfigurable = false;
	}
	
    protected override void OnStartUp()
    {
//      if (this.get_ChartControl() == null)
//      {
//label_1:
//        switch (1)
//        {
//          case 0:
//            goto label_1;
//          default:
//            if (1 != 0)
//              break;
//            // ISSUE: method reference
//            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.OnStartUp);
//            break;
//        }
//      }
//      else
//      {
        this.logoSetup();
        //ToolStrip toolStrip = ((Control) this.ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
        //this.c457facdc3769e1f720cb334357b8307a = new ToolStripSeparator();
        //this.c77da1ce00f4d2cf1d739478d471b601e = new ToolStripDropDownButton(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629((int) byte.MaxValue));
        //this.c77da1ce00f4d2cf1d739478d471b601e.Name = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(278);
        //ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(291));
        //toolStripMenuItem1.Checked = this.c0976f285618fa2f72c6a842d6e22d537;
        //toolStripMenuItem1.Click += new EventHandler(this.ce3690bd0a1e9603192f7898e782b1f4f);
        //this.c77da1ce00f4d2cf1d739478d471b601e.DropDownItems.Add((ToolStripItem) toolStripMenuItem1);
        //ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(302));
        //toolStripMenuItem2.Checked = this.cdecf2853d16d3ae1374d69d0b63a35dc;
        //toolStripMenuItem2.Click += new EventHandler(this.c08cc5298d38e534db5d45a770a9e27d2);
        //this.c77da1ce00f4d2cf1d739478d471b601e.DropDownItems.Add((ToolStripItem) toolStripMenuItem2);
        //toolStrip.Items.Add((ToolStripItem) this.c457facdc3769e1f720cb334357b8307a);
        //toolStrip.Items.Add((ToolStripItem) this.c77da1ce00f4d2cf1d739478d471b601e);
        //this.c1d9289d3109d23c6bbe8ad91f54337a4 = ((object) this).GetType().Name;
        //this.c92b65488e202ae89a88b76ef9a4ce637 = new Font(((Control) this.get_ChartControl()).Font.FontFamily, (float) Convert.ToInt16(this.c01ec28e16876787b89a667457c9913a2));
//      }
    }

    protected override void OnBarUpdate()
    {
      if (this.get_CurrentBar() != 0)
      {
label_1:
        switch (6)
        {
          case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
              // ISSUE: method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.OnBarUpdate);
            }
            double num1 = this.ATR(this.c1236bb80f125d2f64ede5d3554357b5e).get_Item(0);
            double num2 = this.StdDev(this.get_Close(), this.c1236bb80f125d2f64ede5d3554357b5e).get_Item(0);
            double num3;
            if (this.c1430534891381b1f402f44f5c2bc45e6 * num1 == 0.0)
            {
label_5:
              switch (6)
              {
                case 0:
                  goto label_5;
                default:
                  num3 = 1.0;
                  break;
              }
            }
            else
              num3 = this.cf1446b47f2a286dd76c812e90dba61d8 * num2 / (this.c1430534891381b1f402f44f5c2bc45e6 * num1);
            if (num3 <= 1.0)
            {
label_9:
              switch (5)
              {
                case 0:
                  goto label_9;
                default:
                  if (this.Squeeze.get_Item(1) != 0.0)
                  {
label_11:
                    switch (1)
                    {
                      case 0:
                        goto label_11;
                      default:
                        this.c92c86a6ca3a9f35bfb8f9042e2d54bbb = this.get_CurrentBar();
                        this.cfb105a9d24e167ceec6191571a1a8e43 = this.get_High().get_Item(0);
                        this.c739d1f8b1109fe6f2de5bc1ec3c7d232 = this.get_Low().get_Item(0);
                        ++this.c70fc78f80bfde9358a1dc14ea234aa72;
                        break;
                    }
                  }
                  if (this.Squeeze.get_Item(1) == 0.0)
                  {
label_14:
                    switch (4)
                    {
                      case 0:
                        goto label_14;
                      default:
                        if (this.get_High().get_Item(0) > this.cfb105a9d24e167ceec6191571a1a8e43)
                        {
label_16:
                          switch (3)
                          {
                            case 0:
                              goto label_16;
                            default:
                              this.cfb105a9d24e167ceec6191571a1a8e43 = this.get_High().get_Item(0);
                              break;
                          }
                        }
                        if (this.get_Low().get_Item(0) < this.c739d1f8b1109fe6f2de5bc1ec3c7d232)
                        {
label_19:
                          switch (6)
                          {
                            case 0:
                              goto label_19;
                            default:
                              this.c739d1f8b1109fe6f2de5bc1ec3c7d232 = this.get_Low().get_Item(0);
                              break;
                          }
                        }
                        this.cdcf0ce7396813d4bd126a0b9fa8fea53 = this.get_CurrentBar();
                        if (this.c92c86a6ca3a9f35bfb8f9042e2d54bbb != this.cdcf0ce7396813d4bd126a0b9fa8fea53)
                        {
                          this.c133da4068b26ce316e9f7ed309997e74 = (this.cfb105a9d24e167ceec6191571a1a8e43 + this.c739d1f8b1109fe6f2de5bc1ec3c7d232) / 2.0;
                          this.c950bfc49088216da2d12e2c56be386df = this.cfb105a9d24e167ceec6191571a1a8e43 - this.c739d1f8b1109fe6f2de5bc1ec3c7d232;
                          this.cbdbec797f08dfdb6d31d4a0781088769 = this.DrawRectangle(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1102) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.c92c86a6ca3a9f35bfb8f9042e2d54bbb, this.cfb105a9d24e167ceec6191571a1a8e43, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.c52fc7d455698dfea6cd86aaf0c23f658.get_Pen().Color, this.c639e17861f64166b7377685b42faf5a7, this.cc9ca3141d96e48b218961a6a4a6c2e83);
                          ((IShape) this.cbdbec797f08dfdb6d31d4a0781088769).get_Pen().DashStyle = this.c52fc7d455698dfea6cd86aaf0c23f658.get_Pen().DashStyle;
                          ((IShape) this.cbdbec797f08dfdb6d31d4a0781088769).get_Pen().Width = this.c52fc7d455698dfea6cd86aaf0c23f658.get_Pen().Width;
                          if (this.cf802e897a8ec7a9a0dc5a33042c7fef0)
                          {
label_23:
                            switch (3)
                            {
                              case 0:
                                goto label_23;
                              default:
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1109) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.cfb105a9d24e167ceec6191571a1a8e43, this.cd996e9dad932d32bc398d3e19f4bd922.get_Pen().Color, this.cd996e9dad932d32bc398d3e19f4bd922.get_Pen().DashStyle, (int) this.cd996e9dad932d32bc398d3e19f4bd922.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                  this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
                                  this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                  break;
                                }
                                else
                                  break;
                            }
                          }
                          if (this.c8c666df1647da5e0be2372b16be0da70)
                          {
label_29:
                            switch (7)
                            {
                              case 0:
                                goto label_29;
                              default:
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1114) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, this.c5f155ce944c1fc760117c90aa5fe632a.get_Pen().Color, this.c5f155ce944c1fc760117c90aa5fe632a.get_Pen().DashStyle, (int) this.c5f155ce944c1fc760117c90aa5fe632a.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_31:
                                  switch (6)
                                  {
                                    case 0:
                                      goto label_31;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_34:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_34;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.cc5dbd81508adf50cd50c8e46262b80d5)
                          {
label_37:
                            switch (4)
                            {
                              case 0:
                                goto label_37;
                              default:
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1119) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c133da4068b26ce316e9f7ed309997e74, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.cb910efb6746af270cfa63ea6119b308a, this.c133da4068b26ce316e9f7ed309997e74, this.ccb7a3d16e77f50cb570e2b19b247ae9d.get_Pen().Color, this.ccb7a3d16e77f50cb570e2b19b247ae9d.get_Pen().DashStyle, (int) this.ccb7a3d16e77f50cb570e2b19b247ae9d.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_39:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_39;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_42:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_42;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.c8bf6fdf4f7970894cd74c89a895cd299)
                          {
label_45:
                            switch (7)
                            {
                              case 0:
                                goto label_45;
                              default:
                                double num4 = this.c950bfc49088216da2d12e2c56be386df * (double) this.c132301e69bc69471bfc8126ab78337ee / 100.0;
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1126) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.c635f754ce1a2c575c614ed5d8911b08e.get_Pen().Color, this.c635f754ce1a2c575c614ed5d8911b08e.get_Pen().DashStyle, (int) this.c635f754ce1a2c575c614ed5d8911b08e.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_47:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_47;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_50:
                                  switch (2)
                                  {
                                    case 0:
                                      goto label_50;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1135) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.c635f754ce1a2c575c614ed5d8911b08e.get_Pen().Color, this.c635f754ce1a2c575c614ed5d8911b08e.get_Pen().DashStyle, (int) this.c635f754ce1a2c575c614ed5d8911b08e.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_53:
                                  switch (3)
                                  {
                                    case 0:
                                      goto label_53;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_56:
                                  switch (6)
                                  {
                                    case 0:
                                      goto label_56;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.c158d34f4cd6b3109a4d4134b5f26402f)
                          {
label_59:
                            switch (2)
                            {
                              case 0:
                                goto label_59;
                              default:
                                double num4 = this.c950bfc49088216da2d12e2c56be386df * (double) this.c68a4c407a5177fa72b3e372f4987e9f0 / 100.0;
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1144) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.c8b22b97d5b299c02316d6dfa8981e1c6.get_Pen().Color, this.c8b22b97d5b299c02316d6dfa8981e1c6.get_Pen().DashStyle, (int) this.c8b22b97d5b299c02316d6dfa8981e1c6.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_61:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_61;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_64:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_64;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1153) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.c8b22b97d5b299c02316d6dfa8981e1c6.get_Pen().Color, this.c8b22b97d5b299c02316d6dfa8981e1c6.get_Pen().DashStyle, (int) this.c8b22b97d5b299c02316d6dfa8981e1c6.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_67:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_67;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_70:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_70;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.c158d34f4cd6b3109a4d4134b5f26402f)
                          {
label_73:
                            switch (1)
                            {
                              case 0:
                                goto label_73;
                              default:
                                double num4 = this.c950bfc49088216da2d12e2c56be386df * (double) this.c39ad57272823a0eb86f3d3a88ce55689 / 100.0;
                                Plot plot = this.c86ffca2cd49015fb08b7eaceda83c211;
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1162) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.cfb105a9d24e167ceec6191571a1a8e43 + num4, plot.get_Pen().Color, plot.get_Pen().DashStyle, (int) plot.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_75:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_75;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_78:
                                  switch (4)
                                  {
                                    case 0:
                                      goto label_78;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                this.c2adc4d144eb4d1c02b1418f0ef7bdd15 = this.DrawLine(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1171) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53 - this.c22eda7b2c48a3c51b10ee578e7a98723, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - num4, plot.get_Pen().Color, plot.get_Pen().DashStyle, (int) plot.get_Pen().Width);
                                if (!this.c9ffb5283db4a57c6db0dc6feb7f764a0.ContainsKey(this.c2adc4d144eb4d1c02b1418f0ef7bdd15))
                                {
label_81:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_81;
                                    default:
                                      this.c9ffb5283db4a57c6db0dc6feb7f764a0.Add(this.c2adc4d144eb4d1c02b1418f0ef7bdd15, this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color);
                                      break;
                                  }
                                }
                                if (!this.cdecf2853d16d3ae1374d69d0b63a35dc)
                                {
label_84:
                                  switch (7)
                                  {
                                    case 0:
                                      goto label_84;
                                    default:
                                      this.c2adc4d144eb4d1c02b1418f0ef7bdd15.get_Pen().Color = Color.Transparent;
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
                          if (this.c51a299b74e78d9d24e6a4a54ef2098f0)
                          {
label_87:
                            switch (7)
                            {
                              case 0:
                                goto label_87;
                              default:
                                int num4 = Convert.ToInt32((this.cfb105a9d24e167ceec6191571a1a8e43 - this.c739d1f8b1109fe6f2de5bc1ec3c7d232) / this.get_TickSize());
                                if (this.cef78ed9a48e530a67243d70a0e6e03c6 == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1180) || this.cef78ed9a48e530a67243d70a0e6e03c6 == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1191))
                                {
                                  this.c63b68670261102fc9a5683b8ef26ddae = this.DrawText(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1200) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, num4.ToString(), this.get_CurrentBar() - this.cdcf0ce7396813d4bd126a0b9fa8fea53, this.c133da4068b26ce316e9f7ed309997e74, 0, this.c93c4eee6215a4b05789c445423766398, this.c92b65488e202ae89a88b76ef9a4ce637, StringAlignment.Near, Color.Transparent, Color.Transparent, 1);
                                  if (!this.c36681af401b51ffcf73e4f48f976ba85.ContainsKey(this.c63b68670261102fc9a5683b8ef26ddae))
                                  {
label_90:
                                    switch (2)
                                    {
                                      case 0:
                                        goto label_90;
                                      default:
                                        this.c36681af401b51ffcf73e4f48f976ba85.Add(this.c63b68670261102fc9a5683b8ef26ddae, this.c63b68670261102fc9a5683b8ef26ddae.get_TextColor());
                                        break;
                                    }
                                  }
                                  if (!this.c0976f285618fa2f72c6a842d6e22d537)
                                  {
label_93:
                                    switch (7)
                                    {
                                      case 0:
                                        goto label_93;
                                      default:
                                        this.c63b68670261102fc9a5683b8ef26ddae.set_TextColor(Color.Transparent);
                                        break;
                                    }
                                  }
                                }
                                if (!(this.cef78ed9a48e530a67243d70a0e6e03c6 == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1211)))
                                {
label_96:
                                  switch (1)
                                  {
                                    case 0:
                                      goto label_96;
                                    default:
                                      if (this.cef78ed9a48e530a67243d70a0e6e03c6 == cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1191))
                                      {
label_98:
                                        switch (4)
                                        {
                                          case 0:
                                            goto label_98;
                                        }
                                      }
                                      else
                                        goto label_105;
                                  }
                                }
                                this.c63b68670261102fc9a5683b8ef26ddae = this.DrawText(this.c1d9289d3109d23c6bbe8ad91f54337a4 + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1220) + (string) (object) this.c70fc78f80bfde9358a1dc14ea234aa72, true, num4.ToString(), this.get_CurrentBar() - this.c92c86a6ca3a9f35bfb8f9042e2d54bbb, this.c133da4068b26ce316e9f7ed309997e74, 0, this.c93c4eee6215a4b05789c445423766398, this.c92b65488e202ae89a88b76ef9a4ce637, StringAlignment.Far, Color.Transparent, Color.Transparent, 1);
                                if (!this.c36681af401b51ffcf73e4f48f976ba85.ContainsKey(this.c63b68670261102fc9a5683b8ef26ddae))
                                {
label_100:
                                  switch (7)
                                  {
                                    case 0:
                                      goto label_100;
                                    default:
                                      this.c36681af401b51ffcf73e4f48f976ba85.Add(this.c63b68670261102fc9a5683b8ef26ddae, this.c63b68670261102fc9a5683b8ef26ddae.get_TextColor());
                                      break;
                                  }
                                }
                                if (!this.c0976f285618fa2f72c6a842d6e22d537)
                                {
label_103:
                                  switch (7)
                                  {
                                    case 0:
                                      goto label_103;
                                    default:
                                      this.c63b68670261102fc9a5683b8ef26ddae.set_TextColor(Color.Transparent);
                                      break;
                                  }
                                }
                                else
                                  break;
                            }
                          }
label_105:
                          if (!this.c0976f285618fa2f72c6a842d6e22d537)
                          {
label_106:
                            switch (6)
                            {
                              case 0:
                                goto label_106;
                              default:
                                this.cff629cbfa130d54186b496dc143b8484 = ((IShape) this.cbdbec797f08dfdb6d31d4a0781088769).get_AreaColor();
                                this.cfaf31b4c1fe0e36d277e224a6998ed89 = ((IShape) this.cbdbec797f08dfdb6d31d4a0781088769).get_Pen().Color;
                                ((IShape) this.cbdbec797f08dfdb6d31d4a0781088769).set_AreaColor(Color.Transparent);
                                ((IShape) this.cbdbec797f08dfdb6d31d4a0781088769).get_Pen().Color = Color.Transparent;
                                break;
                            }
                          }
                          if (!this.c8e3bfc379648afc881c5b89cf9059f8a.Contains(this.cbdbec797f08dfdb6d31d4a0781088769))
                          {
label_109:
                            switch (7)
                            {
                              case 0:
                                goto label_109;
                              default:
                                this.c8e3bfc379648afc881c5b89cf9059f8a.Add(this.cbdbec797f08dfdb6d31d4a0781088769);
                                break;
                            }
                          }
                          else
                            break;
                        }
                        else
                          break;
                    }
                  }
                  this.Squeeze.Set(0.0);
                  this.SqueezeOn.Reset();
                  break;
              }
            }
            else
            {
              this.Squeeze.Reset();
              this.SqueezeOn.Set(0.0);
            }
            this.dataSeries2.Set(this.get_Input().get_Item(0) - (this.DonchianChannel(this.get_Input(), this.cfe10c42f2805322d3451773938db24cf).Mean.Get(this.get_CurrentBar()) + this.EMA(this.get_Input(), this.cfe10c42f2805322d3451773938db24cf).get_Item(0)) / 2.0);
            this.dataSeries1.Set(this.LinReg((IDataSeries) this.dataSeries2, this.cfe10c42f2805322d3451773938db24cf).get_Item(0));
            this.PMomentumUp.Set(0.0);
            this.PMomentumDown.Set(0.0);
            this.NMomentumUp.Set(0.0);
            this.NMomentumDown.Set(0.0);
            if (this.dataSeries1.get_Item(0) > 0.0)
            {
label_114:
              switch (3)
              {
                case 0:
                  goto label_114;
                default:
                  if (this.dataSeries1.get_Item(0) > this.dataSeries1.get_Item(1))
                  {
label_116:
                    switch (5)
                    {
                      case 0:
                        goto label_116;
                      default:
                        this.PMomentumUp.Set(this.dataSeries1.get_Item(0));
                        break;
                    }
                  }
                  else
                  {
                    this.PMomentumDown.Set(this.dataSeries1.get_Item(0));
                    break;
                  }
              }
            }
            else if (this.dataSeries1.get_Item(0) > this.dataSeries1.get_Item(1))
            {
              this.NMomentumUp.Set(this.dataSeries1.get_Item(0));
              break;
            }
            else
            {
              this.NMomentumDown.Set(this.dataSeries1.get_Item(0));
              break;
            }
        }
      }
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundHigh, 0, this.cfb105a9d24e167ceec6191571a1a8e43, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1229));
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundLevel1, 1, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df * (double) this.c132301e69bc69471bfc8126ab78337ee / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1308));
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundLevel2, 2, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df * (double) this.c68a4c407a5177fa72b3e372f4987e9f0 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1401));
      this.cc9a41bd2d71b489e8f4134a31abfa2e6(this.SoundLevel3, 3, this.cfb105a9d24e167ceec6191571a1a8e43 + this.c950bfc49088216da2d12e2c56be386df * (double) this.c39ad57272823a0eb86f3d3a88ce55689 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1494));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLow, 4, this.c739d1f8b1109fe6f2de5bc1ec3c7d232, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1587));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLevel1, 5, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df * (double) this.c132301e69bc69471bfc8126ab78337ee / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1664));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLevel2, 6, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df * (double) this.c68a4c407a5177fa72b3e372f4987e9f0 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1755));
      this.c03c1af8a08fc173794566a08a468a918(this.SoundLevel3, 7, this.c739d1f8b1109fe6f2de5bc1ec3c7d232 - this.c950bfc49088216da2d12e2c56be386df * (double) this.c39ad57272823a0eb86f3d3a88ce55689 / 100.0, cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1846));
      this.CurrentValue.Set(this.dataSeries1.get_Item(0));
      if (this.c13d059c56853219888b6d45fccc6b9e1 != this.get_CurrentBar())
      {
label_123:
        switch (5)
        {
          case 0:
            goto label_123;
          default:
            this.c13d059c56853219888b6d45fccc6b9e1 = this.get_CurrentBar();
            if (this.SqueezeOn.get_Item(0) == 0.0)
            {
label_125:
              switch (4)
              {
                case 0:
                  goto label_125;
                default:
                  ++this.ced563a5e9c3c71140c2e62f63afc998a;
                  break;
              }
            }
            else
              this.ced563a5e9c3c71140c2e62f63afc998a = 0;
            if (this.Squeeze.get_Item(0) == 0.0)
            {
label_129:
              switch (2)
              {
                case 0:
                  goto label_129;
                default:
                  ++this.c6c9ad6a793905e213eacafa1be1fb516;
                  break;
              }
            }
            else
            {
              this.c6c9ad6a793905e213eacafa1be1fb516 = 0;
              break;
            }
        }
      }
      if (this.ced563a5e9c3c71140c2e62f63afc998a > this.c88126af217b4132a66884925bcfffe67)
      {
label_133:
        switch (5)
        {
          case 0:
            goto label_133;
          default:
            this.BarsSinceSqueeze.Set(0.0);
            break;
        }
      }
      else
        this.BarsSinceSqueeze.Set((double) this.ced563a5e9c3c71140c2e62f63afc998a);
      this.SqueezeLength.Set((double) this.c6c9ad6a793905e213eacafa1be1fb516);
      this.SqueezeHigh.Set(this.cfb105a9d24e167ceec6191571a1a8e43);
      this.SqueezeLow.Set(this.c739d1f8b1109fe6f2de5bc1ec3c7d232);
    }

    private void cc9a41bd2d71b489e8f4134a31abfa2e6(string cc411ee82ad01410449696e9d303287f0, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string c549929c7d4f914e2cdb9a099d7a7eb21)
    {
      if (!(cc411ee82ad01410449696e9d303287f0 != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937)))
        return;
label_1:
      switch (1)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
            // ISSUE: method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.cc9a41bd2d71b489e8f4134a31abfa2e6);
          }
          if (this.c253aeae02261ab94a049d7a01dfe975e[cefea6044484c3257df671c52a39f09b2] == this.get_CurrentBar() || this.cba308ffc4c055fde426f73bdfd36e1e9[cefea6044484c3257df671c52a39f09b2] == this.cdcf0ce7396813d4bd126a0b9fa8fea53)
            break;
label_5:
          switch (1)
          {
            case 0:
              goto label_5;
            default:
              if (this.get_Close().get_Item(1) >= ca2bb1244f73ff9a012a77d9ca61f445d || this.get_Close().get_Item(0) < ca2bb1244f73ff9a012a77d9ca61f445d || this.get_CurrentBar() < this.cdcf0ce7396813d4bd126a0b9fa8fea53)
                return;
label_7:
              switch (5)
              {
                case 0:
                  goto label_7;
                default:
                  if (this.get_CurrentBar() >= this.cdcf0ce7396813d4bd126a0b9fa8fea53 + this.c22eda7b2c48a3c51b10ee578e7a98723)
                    return;
label_9:
                  switch (2)
                  {
                    case 0:
                      goto label_9;
                    default:
                      if (!this.ccfe28103c8fb75a8c45718f709797bbc)
                      {
label_11:
                        switch (7)
                        {
                          case 0:
                            goto label_11;
                          default:
                            this.cba308ffc4c055fde426f73bdfd36e1e9[cefea6044484c3257df671c52a39f09b2] = this.cdcf0ce7396813d4bd126a0b9fa8fea53;
                            break;
                        }
                      }
                      this.c253aeae02261ab94a049d7a01dfe975e[cefea6044484c3257df671c52a39f09b2] = this.get_CurrentBar();
                      this.c013129a8956cc8e16ff7ec447af66499(c549929c7d4f914e2cdb9a099d7a7eb21);
                      this.PlaySound(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1954) + cc411ee82ad01410449696e9d303287f0);
                      return;
                  }
              }
          }
      }
    }

	
    private void c08cc5298d38e534db5d45a770a9e27d2(object c113fd96257592fe24a96acf1688040b5, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      ToolStripMenuItem toolStripMenuItem = c113fd96257592fe24a96acf1688040b5 as ToolStripMenuItem;
      if (toolStripMenuItem.Checked)
      {
        using (Dictionary<ILine, Color>.Enumerator enumerator = this.c9ffb5283db4a57c6db0dc6feb7f764a0.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Key.get_Pen().Color = Color.Transparent;
label_5:
          switch (1)
          {
            case 0:
              goto label_5;
            default:
              if (1 == 0)
              {
                // ISSUE: method reference
                RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.c08cc5298d38e534db5d45a770a9e27d2);
                break;
              }
              else
                break;
          }
        }
        ((Control) this.get_ChartControl()).Refresh();
        this.cfd0f789956fddac7d881844489c6bb27.Refresh();
        toolStripMenuItem.Checked = false;
        this.cdecf2853d16d3ae1374d69d0b63a35dc = false;
      }
      else
      {
        if (toolStripMenuItem.Checked)
          return;
label_11:
        switch (6)
        {
          case 0:
            goto label_11;
          default:
            using (Dictionary<ILine, Color>.Enumerator enumerator = this.c9ffb5283db4a57c6db0dc6feb7f764a0.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<ILine, Color> current = enumerator.Current;
                current.Key.get_Pen().Color = current.Value;
              }
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.cfd0f789956fddac7d881844489c6bb27.Refresh();
            toolStripMenuItem.Checked = true;
            this.cdecf2853d16d3ae1374d69d0b63a35dc = true;
            break;
        }
      }
    }

    private void ce3690bd0a1e9603192f7898e782b1f4f(object c113fd96257592fe24a96acf1688040b5, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      ToolStripMenuItem toolStripMenuItem = c113fd96257592fe24a96acf1688040b5 as ToolStripMenuItem;
      if (toolStripMenuItem.Checked)
      {
label_1:
        switch (4)
        {
          case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
              // ISSUE: method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.ce3690bd0a1e9603192f7898e782b1f4f);
            }
            using (List<IRectangle>.Enumerator enumerator = this.c8e3bfc379648afc881c5b89cf9059f8a.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IRectangle current = enumerator.Current;
                this.cff629cbfa130d54186b496dc143b8484 = ((IShape) current).get_AreaColor();
                this.cfaf31b4c1fe0e36d277e224a6998ed89 = ((IShape) current).get_Pen().Color;
                ((IShape) current).set_AreaColor(Color.Transparent);
                ((IShape) current).get_Pen().Color = Color.Transparent;
              }
            }
            using (Dictionary<IText, Color>.Enumerator enumerator = this.c36681af401b51ffcf73e4f48f976ba85.GetEnumerator())
            {
              while (enumerator.MoveNext())
                enumerator.Current.Key.set_TextColor(Color.Transparent);
label_13:
              switch (1)
              {
                case 0:
                  goto label_13;
              }
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.cfd0f789956fddac7d881844489c6bb27.Refresh();
            toolStripMenuItem.Checked = false;
            this.c0976f285618fa2f72c6a842d6e22d537 = false;
            break;
        }
      }
      else
      {
        if (toolStripMenuItem.Checked)
          return;
label_17:
        switch (2)
        {
          case 0:
            goto label_17;
          default:
            using (List<IRectangle>.Enumerator enumerator = this.c8e3bfc379648afc881c5b89cf9059f8a.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IRectangle current = enumerator.Current;
                ((IShape) current).set_AreaColor(this.cff629cbfa130d54186b496dc143b8484);
                ((IShape) current).get_Pen().Color = this.cfaf31b4c1fe0e36d277e224a6998ed89;
              }
label_22:
              switch (1)
              {
                case 0:
                  goto label_22;
              }
            }
            using (Dictionary<IText, Color>.Enumerator enumerator = this.c36681af401b51ffcf73e4f48f976ba85.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<IText, Color> current = enumerator.Current;
                current.Key.set_TextColor(current.Value);
              }
label_28:
              switch (1)
              {
                case 0:
                  goto label_28;
              }
            }
            ((Control) this.get_ChartControl()).Refresh();
            this.cfd0f789956fddac7d881844489c6bb27.Refresh();
            toolStripMenuItem.Checked = true;
            this.c0976f285618fa2f72c6a842d6e22d537 = true;
            break;
        }
      }
    }

    protected virtual void OnTermination()
    {
      try
      {
        if (this.get_ChartControl() == null)
        {
label_1:
          switch (4)
          {
            case 0:
              goto label_1;
            default:
              if (1 != 0)
                break;
              // ISSUE: method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.OnTermination);
              break;
          }
        }
        else
        {
          this.cfd0f789956fddac7d881844489c6bb27.Click -= new EventHandler(this.cd7409d0fdab3b2ac0617894a0a49bbf0);
          this.get_ChartControl().get_ChartPanel().Controls.Remove((Control) this.cfd0f789956fddac7d881844489c6bb27);
          ToolStrip toolStrip = ((Control) this.get_ChartControl()).Controls[cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(240)] as ToolStrip;
          toolStrip.Items.Remove((ToolStripItem) this.c457facdc3769e1f720cb334357b8307a);
          toolStrip.Items.Remove((ToolStripItem) this.c77da1ce00f4d2cf1d739478d471b601e);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public void logoSetup()
    {
      string str1 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(313);
      string str2;
      if (this.ceafa24ab078c9719b466e842e5f1306f)
      {
label_1:
        switch (5)
        {
          case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
              // ISSUE: method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.logoSetup);
            }
            str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(332);
            break;
        }
      }
      else
        str2 = Core.get_UserDataDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(313);
      try
      {
        if (System.IO.File.Exists(str2))
        {
label_7:
          switch (5)
          {
            case 0:
              goto label_7;
            default:
              System.IO.File.GetCreationTime(str2);
              break;
          }
        }
        else
        {
          WebClient webClient = new WebClient();
          if (this.ceafa24ab078c9719b466e842e5f1306f)
          {
label_10:
            switch (5)
            {
              case 0:
                goto label_10;
              default:
                webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(359), str2);
                break;
            }
          }
          else
            webClient.DownloadFile(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(478), str2);
        }
        this.c497f3eec1deaf291fc952d3f44b5ad9c = Image.FromFile(str2);
        this.cfd0f789956fddac7d881844489c6bb27 = new rcVolatilityBreakout.hitPanel();
        this.cfd0f789956fddac7d881844489c6bb27.Height = this.c497f3eec1deaf291fc952d3f44b5ad9c.Height;
        this.cfd0f789956fddac7d881844489c6bb27.Width = this.c497f3eec1deaf291fc952d3f44b5ad9c.Width;
        this.cfd0f789956fddac7d881844489c6bb27.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.cfd0f789956fddac7d881844489c6bb27.Top = this.get_ChartControl().get_ChartPanel().Bounds.Bottom - this.c497f3eec1deaf291fc952d3f44b5ad9c.Height - 80;
        this.cfd0f789956fddac7d881844489c6bb27.Left = 1;
        this.cfd0f789956fddac7d881844489c6bb27.Click += new EventHandler(this.cd7409d0fdab3b2ac0617894a0a49bbf0);
        this.get_ChartControl().get_ChartPanel().Controls.Add((Control) this.cfd0f789956fddac7d881844489c6bb27);
      }
      catch (Exception ex)
      {
        this.Print(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(589));
      }
    }

    public virtual void Plot(Graphics g, Rectangle bounds, double min, double max)
    {
      g.DrawImage(this.c497f3eec1deaf291fc952d3f44b5ad9c, 2, bounds.Bottom - this.c497f3eec1deaf291fc952d3f44b5ad9c.Height - 18, this.c497f3eec1deaf291fc952d3f44b5ad9c.Width, this.c497f3eec1deaf291fc952d3f44b5ad9c.Height);
    }

    private void cd7409d0fdab3b2ac0617894a0a49bbf0(object c113fd96257592fe24a96acf1688040b5, EventArgs cb7779bfc6870535a4a28a971830ae75f)
    {
      Process.Start(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(622), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(647));
    }

    private void c013129a8956cc8e16ff7ec447af66499(string c63b68670261102fc9a5683b8ef26ddae)
    {
      if (this.c2d383bd69d6069688bcde2389f05aa78.Length <= 0)
        return;
label_1:
      switch (5)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
            // ISSUE: method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.c013129a8956cc8e16ff7ec447af66499);
          }
          this.SendMail(cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(700), this.c2d383bd69d6069688bcde2389f05aa78, this.get_Name() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(747), c63b68670261102fc9a5683b8ef26ddae + (object) cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(760) + (string) (object) DateTime.Now + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(795) + this.get_Instrument().get_MasterInstrument().get_Name() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(822) + (string) (object) this.get_Input().get_Item(0));
          break;
      }
    }


    private void c03c1af8a08fc173794566a08a468a918(string cc411ee82ad01410449696e9d303287f0, int cefea6044484c3257df671c52a39f09b2, double ca2bb1244f73ff9a012a77d9ca61f445d, string c549929c7d4f914e2cdb9a099d7a7eb21)
    {
      if (!(cc411ee82ad01410449696e9d303287f0 != cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937)))
        return;
label_1:
      switch (6)
      {
        case 0:
          goto label_1;
        default:
          if (1 == 0)
          {
            // ISSUE: method reference
            RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.c03c1af8a08fc173794566a08a468a918);
          }
          if (this.c253aeae02261ab94a049d7a01dfe975e[cefea6044484c3257df671c52a39f09b2] == this.get_CurrentBar())
            break;
label_5:
          switch (6)
          {
            case 0:
              goto label_5;
            default:
              if (this.cba308ffc4c055fde426f73bdfd36e1e9[cefea6044484c3257df671c52a39f09b2] == this.cdcf0ce7396813d4bd126a0b9fa8fea53)
                return;
label_7:
              switch (2)
              {
                case 0:
                  goto label_7;
                default:
                  if (this.get_Close().get_Item(1) <= ca2bb1244f73ff9a012a77d9ca61f445d || this.get_Close().get_Item(0) > ca2bb1244f73ff9a012a77d9ca61f445d)
                    return;
label_9:
                  switch (2)
                  {
                    case 0:
                      goto label_9;
                    default:
                      if (this.get_CurrentBar() < this.cdcf0ce7396813d4bd126a0b9fa8fea53 || this.get_CurrentBar() >= this.cdcf0ce7396813d4bd126a0b9fa8fea53 + this.c22eda7b2c48a3c51b10ee578e7a98723)
                        return;
label_11:
                      switch (7)
                      {
                        case 0:
                          goto label_11;
                        default:
                          if (!this.ccfe28103c8fb75a8c45718f709797bbc)
                          {
label_13:
                            switch (6)
                            {
                              case 0:
                                goto label_13;
                              default:
                                this.cba308ffc4c055fde426f73bdfd36e1e9[cefea6044484c3257df671c52a39f09b2] = this.cdcf0ce7396813d4bd126a0b9fa8fea53;
                                break;
                            }
                          }
                          this.c253aeae02261ab94a049d7a01dfe975e[cefea6044484c3257df671c52a39f09b2] = this.get_CurrentBar();
                          this.c013129a8956cc8e16ff7ec447af66499(c549929c7d4f914e2cdb9a099d7a7eb21);
                          this.PlaySound(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1954) + cc411ee82ad01410449696e9d303287f0);
                          return;
                      }
                  }
              }
          }
      }
    }

    private string c4152da8a89098d52e196178975c6ae4f(Color c4544fc12a8cffe1b31db6f02f2dd887e)
    {
      return c4544fc12a8cffe1b31db6f02f2dd887e.ToArgb().ToString();
    }

    private Color c8c61fd528f31c4e9976bfec210e23950(string c4544fc12a8cffe1b31db6f02f2dd887e)
    {
      return Color.FromArgb(Convert.ToInt32(c4544fc12a8cffe1b31db6f02f2dd887e));
    }

    public class hitPanel : Panel
    {
      protected override CreateParams CreateParams
      {
        get
        {
          CreateParams createParams = base.CreateParams;
          createParams.ExStyle |= 32;
          return createParams;
        }
      }

      protected override void OnPaintBackground(PaintEventArgs pevent)
      {
      }
    }

    public class XmlColor
    {
      private Color cf961a6779df9914187bdd86c52824c74 = Color.Black;

      [XmlAttribute]
      public string Web
      {
        get
        {
          return ColorTranslator.ToHtml(this.cf961a6779df9914187bdd86c52824c74);
        }
        set
        {
          try
          {
            if ((int) this.Alpha == (int) byte.MaxValue)
            {
label_1:
              switch (7)
              {
                case 0:
                  goto label_1;
                default:
                  if (1 == 0)
                  {
                    RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.XmlColor.set_Web);
                  }
                  this.cf961a6779df9914187bdd86c52824c74 = ColorTranslator.FromHtml(value);
                  break;
              }
            }
            else
              this.cf961a6779df9914187bdd86c52824c74 = Color.FromArgb((int) this.Alpha, ColorTranslator.FromHtml(value));
          }
          catch (Exception ex)
          {
            this.cf961a6779df9914187bdd86c52824c74 = Color.Black;
          }
        }
      }

      [XmlAttribute]
      public byte Alpha
      {
        get
        {
          return this.cf961a6779df9914187bdd86c52824c74.A;
        }
        set
        {
          if ((int) value == (int) this.cf961a6779df9914187bdd86c52824c74.A)
            return;
          this.cf961a6779df9914187bdd86c52824c74 = Color.FromArgb((int) value, this.cf961a6779df9914187bdd86c52824c74);
        }
      }

      public XmlColor()
      {
      }

      public XmlColor(Color c)
      {
        this.cf961a6779df9914187bdd86c52824c74 = c;
      }

      public static implicit operator Color(rcVolatilityBreakout.XmlColor x)
      {
        return x.ToColor();
      }

      public static implicit operator rcVolatilityBreakout.XmlColor(Color c)
      {
        return new rcVolatilityBreakout.XmlColor(c);
      }

      public Color ToColor()
      {
        return this.cf961a6779df9914187bdd86c52824c74;
      }

      public void FromColor(Color c)
      {
        this.cf961a6779df9914187bdd86c52824c74 = c;
      }

      public bool ShouldSerializeAlpha()
      {
        return (int) this.Alpha < (int) byte.MaxValue;
      }
    }

    public class WaveConverter : TypeConverter
    {
      private static string[] c881dc1ea8e7199b694d684a64842fb7d;

      static WaveConverter()
      {
      }

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        string[] array;
        if (rcVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d == null)
        {
label_1:
          switch (3)
          {
            case 0:
              goto label_1;
            default:
              if (1 == 0)
              {
                // ISSUE: method reference
                RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.WaveConverter.GetStandardValues);
              }
              string[] files = Directory.GetFiles(Core.get_InstallDir() + cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2177), cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2196));
              StringCollection stringCollection = new StringCollection();
              for (int index = 0; index < files.Length; ++index)
                stringCollection.Add(Path.GetFileName(files[index]));
label_7:
              switch (5)
              {
                case 0:
                  goto label_7;
                default:
                  array = new string[stringCollection.Count + 1];
                  stringCollection.CopyTo(array, 1);
                  array[0] = cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937);
                  rcVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d = array;
                  break;
              }
          }
        }
        else
          array = rcVolatilityBreakout.WaveConverter.c881dc1ea8e7199b694d684a64842fb7d;
        return new TypeConverter.StandardValuesCollection((ICollection) array);
      }
    }

    private class cb7fd7594b63cf924b3e645a1b96661fe : UITypeEditor
    {
      public override bool GetPaintValueSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override void PaintValue(PaintValueEventArgs e)
      {
        if (e.Value == null)
          return;
label_1:
        switch (4)
        {
          case 0:
            goto label_1;
          default:
            if (1 == 0)
            {
              // ISSUE: method reference
              RuntimeMethodHandle runtimeMethodHandle = __methodref (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe.PaintValue);
            }
            SolidBrush solidBrush = new SolidBrush(((Plot) e.Value).get_Pen().Color);
            e.Graphics.FillRectangle((Brush) solidBrush, e.Bounds);
            solidBrush.Dispose();
            break;
        }
      }
    }

    public class positionModeConverter : TypeConverter
    {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        return new TypeConverter.StandardValuesCollection((ICollection) new string[4]
        {
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1211),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1180),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1191),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(1937)
        });
      }
    }

    public class sizeConverter : TypeConverter
    {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return true;
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        return new TypeConverter.StandardValuesCollection((ICollection) new string[6]
        {
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2207),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2210),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2042),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2213),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2216),
          cee5e96d25be00bb50a036ae3849574cc.c43d8687509d9536665c509709459d629(2219)
        });
      }
    }
	
	#region Properties
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Category("Visual")]
    [Description("")]
    public Plot BoxOutline
    {
      get { return this.c52fc7d455698dfea6cd86aaf0c23f658; }
      set { this.c52fc7d455698dfea6cd86aaf0c23f658 = value; }
    }

    [GridCategory("\tNinjacators")]
    [Description("")]
    public bool ShortLogo
    {
      get { return this.ceafa24ab078c9719b466e842e5f1306f; }
      set { this.ceafa24ab078c9719b466e842e5f1306f = value; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries Squeeze
    {
      get { return this.get_Values()[0]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries SqueezeOn
    {
      get { return this.get_Values()[1]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries PMomentumUp
    {
      get { return this.get_Values()[2]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries PMomentumDown
    {
      get { return this.get_Values()[3]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries NMomentumUp
    {
      get { return this.get_Values()[4]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries NMomentumDown
    {
      get { return this.get_Values()[5]; }
    }

    [Browsable(false)]
    [XmlIgnore]
    public DataSeries CurrentValue
    {
      get { return this.get_Values()[6]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries BarsSinceSqueeze
    {
      get { return this.get_Values()[7]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeLength
    {
      get { return this.get_Values()[8]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeHigh
    {
      get { return this.get_Values()[9]; }
    }

    [XmlIgnore]
    [Browsable(false)]
    public DataSeries SqueezeLow
    {
      get { return this.get_Values()[10]; }
    }

    [Description("Volatility Strength")]
    [GridCategory("Consolidation box")]
    public int Length
    {
      get { return this.c1236bb80f125d2f64ede5d3554357b5e; }
      set { this.c1236bb80f125d2f64ede5d3554357b5e = Math.Max(1, value); }
    }

    [Description("")]
    [Category("Alert")]
    public string Email
    {
      get { return this.c2d383bd69d6069688bcde2389f05aa78; }
      set { this.c2d383bd69d6069688bcde2389f05aa78 = value; }
    }

    [Description("")]
    [Category("Global")]
    public int BarsLookback
    {
      get { return this.c88126af217b4132a66884925bcfffe67; }
      set { this.c88126af217b4132a66884925bcfffe67 = value; }
    }

    [Category("Global")]
    [Description("ShowBoxes")]
    public bool ExtendedVisible
    {
      get { return this.c0976f285618fa2f72c6a842d6e22d537; }
      set { this.c0976f285618fa2f72c6a842d6e22d537 = value; }
    }

    [Description("ShowLines")]
    [Category("Global")]
    public bool ShowLines
    {
      get { return this.cdecf2853d16d3ae1374d69d0b63a35dc; }
      set { this.cdecf2853d16d3ae1374d69d0b63a35dc = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedHigh
    {
      get { return this.cd996e9dad932d32bc398d3e19f4bd922; }
      set { this.cd996e9dad932d32bc398d3e19f4bd922 = value; }
    }

    [Category("Settings")]
    public bool ExtendedHighVisible
    {
      get { return this.cf802e897a8ec7a9a0dc5a33042c7fef0; }
      set { this.cf802e897a8ec7a9a0dc5a33042c7fef0 = value; }
    }

    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Category("Visual")]
    [Description("")]
    public Plot ExtendedLow
    {
      get { return this.c5f155ce944c1fc760117c90aa5fe632a; }
      set { this.c5f155ce944c1fc760117c90aa5fe632a = value; }
    }

    [Category("Settings")]
    public bool ExtendedLowVisible
    {
      get { return this.c8c666df1647da5e0be2372b16be0da70; }
      set { this.c8c666df1647da5e0be2372b16be0da70 = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedMiddle
    {
      get { return this.ccb7a3d16e77f50cb570e2b19b247ae9d; }
      set { this.ccb7a3d16e77f50cb570e2b19b247ae9d = value; }
    }

    [Category("Settings")]
    public bool ExtendedMiddleVisible
    {
      get { return this.cc5dbd81508adf50cd50c8e46262b80d5; }
      set { this.cc5dbd81508adf50cd50c8e46262b80d5 = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedMiddleLineLength
    {
      get { return this.cb910efb6746af270cfa63ea6119b308a; }
      set { this.cb910efb6746af270cfa63ea6119b308a = value; }
    }

    [Description("")]
    [Category("Settings")]
    public int ExtendedLineLength
    {
      get { return this.c22eda7b2c48a3c51b10ee578e7a98723; }
      set { this.c22eda7b2c48a3c51b10ee578e7a98723 = value; }
    }

    [Category("Visual")]
    [Description("")]
    [XmlElement(Type = typeof (rcVolatilityBreakout.XmlColor))]
    public Color BoxAreaColor
    {
      get { return this.c639e17861f64166b7377685b42faf5a7; }
      set { this.c639e17861f64166b7377685b42faf5a7 = value; }
    }

    [Description("")]
    [Category("Visual")]
    public int BoxAreaOpacity
    {
      get { return this.cc9ca3141d96e48b218961a6a4a6c2e83; }
      set { this.cc9ca3141d96e48b218961a6a4a6c2e83 = value; }
    }

    [Category("Visual")]
    [Description("TickHeightVisible")]
    public bool TickHeightVisible
    {
      get { return this.c51a299b74e78d9d24e6a4a54ef2098f0; }
      set { this.c51a299b74e78d9d24e6a4a54ef2098f0 = value; }
    }

    [Category("Visual")]
    [TypeConverter(typeof (rcVolatilityBreakout.positionModeConverter))]
    [Description("TickHeightPosition")]
    public string TickHeightPosition
    {
      get { return this.cef78ed9a48e530a67243d70a0e6e03c6; }
      set { this.cef78ed9a48e530a67243d70a0e6e03c6 = value; }
    }

    [Category("Visual")]
    [XmlElement(Type = typeof (rcVolatilityBreakout.XmlColor))]
    [Description("")]
    public Color TickHeightColor
    {
      get { return this.c93c4eee6215a4b05789c445423766398; }
      set { this.c93c4eee6215a4b05789c445423766398 = value; }
    }

    [Description("FontSize")]
    [Category("Settings")]
    [TypeConverter(typeof (rcVolatilityBreakout.sizeConverter))]
    public string FontSize
    {
      get { return this.c01ec28e16876787b89a667457c9913a2; }
      set { this.c01ec28e16876787b89a667457c9913a2 = value; }
    }

    [Category("Settings")]
    public bool ExtendedLevel1Visible
    {
      get { return this.c8bf6fdf4f7970894cd74c89a895cd299; }
      set { this.c8bf6fdf4f7970894cd74c89a895cd299 = value; }
    }

    [Category("Settings")]
    public bool ExtendedLevel2Visible
    {
      get { return this.c158d34f4cd6b3109a4d4134b5f26402f; }
      set { this.c158d34f4cd6b3109a4d4134b5f26402f = value; }
    }

    [Category("Settings")]
    public bool ExtendedLevel3Visible
    {
      get { return this.cbdda209ad9e84f19ec587ff0f51b194c; }
      set { this.cbdda209ad9e84f19ec587ff0f51b194c = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedLevel1
    {
      get { return this.c132301e69bc69471bfc8126ab78337ee; }
      set { this.c132301e69bc69471bfc8126ab78337ee = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedLevel2
    {
      get { return this.c68a4c407a5177fa72b3e372f4987e9f0; }
      set { this.c68a4c407a5177fa72b3e372f4987e9f0 = value; }
    }

    [Category("Settings")]
    [Description("")]
    public int ExtendedLevel3
    {
      get { return this.c39ad57272823a0eb86f3d3a88ce55689; }
      set { this.c39ad57272823a0eb86f3d3a88ce55689 = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedLineLevel1
    {
      get { return this.c635f754ce1a2c575c614ed5d8911b08e; }
      set { this.c635f754ce1a2c575c614ed5d8911b08e = value; }
    }

    [Category("Visual")]
    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    public Plot ExtendedLineLevel2
    {
      get { return this.c8b22b97d5b299c02316d6dfa8981e1c6; }
      set { this.c8b22b97d5b299c02316d6dfa8981e1c6 = value; }
    }

    [Editor(typeof (rcVolatilityBreakout.cb7fd7594b63cf924b3e645a1b96661fe), typeof (UITypeEditor))]
    [Description("")]
    [Category("Visual")]
    public Plot ExtendedLineLevel3
    {
      get { return this.c86ffca2cd49015fb08b7eaceda83c211; }
      set { this.c86ffca2cd49015fb08b7eaceda83c211 = value; }
    }

    [Category("Alert")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Description("")]
    public string SoundHigh
    {
      get { return this.ca9612a794be4fa61c2b14eaad35d59d8; }
      set { this.ca9612a794be4fa61c2b14eaad35d59d8 = value; }
    }

    [Description("")]
    [Category("Alert")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    public string SoundLow
    {
      get { return this.c267485c17bfb8b21cee1f3704b0944ee; }
      set { this.c267485c17bfb8b21cee1f3704b0944ee = value; }
    }

    [Description("")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Category("Alert")]
    public string SoundLevel1
    {
      get { return this.c99a78e50b34fcbd1b9e091467d576e8c; }
      set { this.c99a78e50b34fcbd1b9e091467d576e8c = value; }
    }

    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Category("Alert")]
    [Description("")]
    public string SoundLevel2
    {
      get { return this.ccef9f98c4d79b41597db979d63c79cb4; }
      set { this.ccef9f98c4d79b41597db979d63c79cb4 = value; }
    }

    [Description("")]
    [TypeConverter(typeof (rcVolatilityBreakout.WaveConverter))]
    [Category("Alert")]
    public string SoundLevel3
    {
      get { return this.c4b685b34ba086b4f8e622571ce57ea1a; }
      set { this.c4b685b34ba086b4f8e622571ce57ea1a = value; }
    }

    [Description("")]
    [Category("Alert")]
    public bool MultiAlert
    {
      get { return this.ccfe28103c8fb75a8c45718f709797bbc; }
      set { this.ccfe28103c8fb75a8c45718f709797bbc = value; }
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
        private rcVolatilityBreakout[] cachercVolatilityBreakout = null;

        private static rcVolatilityBreakout checkrcVolatilityBreakout = new rcVolatilityBreakout();

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public rcVolatilityBreakout rcVolatilityBreakout(int length, bool shortLogo)
        {
            return rcVolatilityBreakout(Input, length, shortLogo);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length, bool shortLogo)
        {
            if (cachercVolatilityBreakout != null)
                for (int idx = 0; idx < cachercVolatilityBreakout.Length; idx++)
                    if (cachercVolatilityBreakout[idx].Length == length && cachercVolatilityBreakout[idx].ShortLogo == shortLogo && cachercVolatilityBreakout[idx].EqualsInput(input))
                        return cachercVolatilityBreakout[idx];

            lock (checkrcVolatilityBreakout)
            {
                checkrcVolatilityBreakout.Length = length;
                length = checkrcVolatilityBreakout.Length;
                checkrcVolatilityBreakout.ShortLogo = shortLogo;
                shortLogo = checkrcVolatilityBreakout.ShortLogo;

                if (cachercVolatilityBreakout != null)
                    for (int idx = 0; idx < cachercVolatilityBreakout.Length; idx++)
                        if (cachercVolatilityBreakout[idx].Length == length && cachercVolatilityBreakout[idx].ShortLogo == shortLogo && cachercVolatilityBreakout[idx].EqualsInput(input))
                            return cachercVolatilityBreakout[idx];

                rcVolatilityBreakout indicator = new rcVolatilityBreakout();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                indicator.ShortLogo = shortLogo;
                Indicators.Add(indicator);
                indicator.SetUp();

                rcVolatilityBreakout[] tmp = new rcVolatilityBreakout[cachercVolatilityBreakout == null ? 1 : cachercVolatilityBreakout.Length + 1];
                if (cachercVolatilityBreakout != null)
                    cachercVolatilityBreakout.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachercVolatilityBreakout = tmp;
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
        /// .
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(int length, bool shortLogo)
        {
            return _indicator.rcVolatilityBreakout(Input, length, shortLogo);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length, bool shortLogo)
        {
            return _indicator.rcVolatilityBreakout(input, length, shortLogo);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(int length, bool shortLogo)
        {
            return _indicator.rcVolatilityBreakout(Input, length, shortLogo);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public Indicator.rcVolatilityBreakout rcVolatilityBreakout(Data.IDataSeries input, int length, bool shortLogo)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.rcVolatilityBreakout(input, length, shortLogo);
        }
    }
}
#endregion
