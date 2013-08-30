using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;
using System;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Indicator;
using System.Drawing;


namespace Gom
{
	// attribute used to decorate hotkey callback methods
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class HotKey : Attribute
	{
		public readonly Keys KeyDef;

		public HotKey(Keys param)
		{
			this.KeyDef = param;
		}
	}


	public sealed class HotKeyClass : IDisposable
	{
		private const Keys ChangeIndyKey = Keys.Control | Keys.Space;

		private IndicatorBase indy;

		static private Dictionary<ChartControl, int> selectedHKindy = new Dictionary<ChartControl, int>();
		static private Dictionary<ChartControl, List<IndicatorBase>> HKindies = new Dictionary<ChartControl, List<IndicatorBase>>();

		//internal helper properties

		private ChartControl chartControl { get { return indy.ChartControl; } }
		
		private List<IndicatorBase> ListHKIndies { get { return HKindies[chartControl]; } }
	
		private int SelectedHKIndyIndex
		{
			get
			{
				CheckSelected();
				return selectedHKindy[chartControl];
			}
			set
			{
				selectedHKindy[chartControl] = value;
				CheckSelected();
				RedrawMessage();
			}
		}


		//internal helper methods
		
		private void CheckSelected()
		{
			if (selectedHKindy[chartControl] >= ListHKIndies.Count)
				selectedHKindy[chartControl] = 0;	
		}
		
		private void RedrawMessage()
		{
			foreach (IndicatorBase ind in ListHKIndies)
				ind.RemoveDrawObject("GomHKMsg");

			PropertyInfo pinfo = ListHKIndies[SelectedHKIndyIndex].GetType().GetProperty("GomHKMessage");

			string msg = "";

			if (pinfo != null)
				msg = pinfo.GetValue(ListHKIndies[SelectedHKIndyIndex], null).ToString();

			if (msg != ",")
			{
				if (msg==".")
					indy.DrawTextFixed("GomHKMsg", +SelectedHKIndyIndex + 1 + Environment.NewLine +" ", NinjaTrader.Data.TextPosition.BottomRight);
				else
					indy.DrawTextFixed("GomHKMsg", +SelectedHKIndyIndex + 1 + " - " + ListHKIndies[SelectedHKIndyIndex].Name + " " + msg + Environment.NewLine +" ", NinjaTrader.Data.TextPosition.BottomRight);
			}

		}

		
		private void ManageHotKeys(object sender, KeyEventArgs e)
		{
			int currentHKIndyIndex=ListHKIndies.IndexOf(indy);
			
			//manage selected indy change
			if (currentHKIndyIndex == 0)
				if (((ChangeIndyKey & Keys.Modifiers) == e.Modifiers) && ((ChangeIndyKey & Keys.KeyCode) == e.KeyCode))
					SelectedHKIndyIndex++;

			//manage hotkeys
			if (currentHKIndyIndex == SelectedHKIndyIndex)
			{
				var listMethods = indy.GetType()
										.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public)
										.Where(x => x.GetCustomAttributes(typeof(Gom.HotKey), false).Length > 0)
										.Select(m =>
													new
													{
														Method = m,
														HotKey = ((Gom.HotKey)(m.GetCustomAttributes(typeof(Gom.HotKey), false).First())).KeyDef
													}
												);

				foreach (var meth in listMethods)
					if (((meth.HotKey & Keys.Modifiers) == e.Modifiers) && ((meth.HotKey & Keys.KeyCode) == e.KeyCode))
					{
						meth.Method.Invoke(indy, new Object[] { });
						RedrawMessage();
					}
			}
		}
		
		 
		//contructor & destructor
		
		public HotKeyClass(IndicatorBase param)
		{
			indy = param;

			if (!HKindies.ContainsKey(indy.ChartControl))
				HKindies.Add(chartControl, new List<IndicatorBase>());

			HKindies[chartControl].Add(indy);

			if (!selectedHKindy.ContainsKey(chartControl))
				selectedHKindy.Add(chartControl, 0);

			chartControl.ChartPanel.KeyDown += ManageHotKeys;

			SelectedHKIndyIndex = 0;
		}
		
		public HotKeyClass(object o):this((IndicatorBase)o)		
		{
		}
	
		

		public void Dispose()
		{
			chartControl.ChartPanel.KeyDown -= ManageHotKeys;
			HKindies[chartControl].Remove(indy);
			CheckSelected();
		}
		
		
		//LEGACY
		public void ManageHotKeys(KeyEventArgs e)
		{
		}
		public void ShowNameOnScreen(Graphics graphics, Rectangle bounds, string msg)
		{}
		
		
	}
}