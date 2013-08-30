//#undef NT7
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace NinjaTrader.Strategy.PH
{

	public class OptionsDialog : Form
	{
		public OptionsDialog ()
		{
			InitializeComponent ();

#if !NT7
			toolTip1.SetToolTip (nudMaxGenerations,
				"The maximum number of generations.");
			toolTip1.SetToolTip (nudPopulationSize,
				"The number of individuals in each generation.");
#endif
		}

		private System.Windows.Forms.Button btnOk;
#if !NT7
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private Label labelMinPerf;
		private NumericUpDown nudMinPerf;
		private NumericUpDown nudMaxGenerations;
		private NumericUpDown nudPopulationSize;
#endif
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ToolTip toolTip1;
		private CheckBox chkWalkForward;
		private Panel panel1;
		private TableLayoutPanel tableLayoutPanel1;

#if !NT7
		public int MaxGenerations
		{
			get { return (int) nudMaxGenerations.Value; }
		}

		public int PopulationSize
		{
			get { return (int) nudPopulationSize.Value; }
		}

		public double MinimumPerformance
		{
			get { return (double) nudMinPerf.Value; }
		}
#endif
		public bool WalkForward { get { return chkWalkForward.Checked; } }


		#region Windows Form Designer generated code
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose ();
			}
			base.Dispose (disposing);
		}


		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.components = new System.ComponentModel.Container ();
			this.btnOk = new System.Windows.Forms.Button ();
			this.btnCancel = new System.Windows.Forms.Button ();
			this.toolTip1 = new System.Windows.Forms.ToolTip (this.components);
			this.chkWalkForward = new System.Windows.Forms.CheckBox ();
			this.panel1 = new System.Windows.Forms.Panel ();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel ();
#if !NT7
			this.label2 = new System.Windows.Forms.Label ();
			this.label1 = new System.Windows.Forms.Label ();
			this.nudMinPerf = new System.Windows.Forms.NumericUpDown ();
			this.labelMinPerf = new System.Windows.Forms.Label ();
			this.nudMaxGenerations = new System.Windows.Forms.NumericUpDown ();
			this.nudPopulationSize = new System.Windows.Forms.NumericUpDown ();
			((System.ComponentModel.ISupportInitialize) (this.nudMaxGenerations)).BeginInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudPopulationSize)).BeginInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudMinPerf)).BeginInit ();
#endif
			this.panel1.SuspendLayout ();
			this.SuspendLayout ();

			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point (340, 273);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size (75, 23);
			this.btnOk.TabIndex = 22;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
#if !NT7
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (39, 43);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (85, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Generation &Size:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (39, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (93, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Generation Count:";
			// 
			// nudMaxGenerations
			// 
			this.nudMaxGenerations.Location = new System.Drawing.Point (134, 15);
			this.nudMaxGenerations.Maximum = new decimal (new int [] {
            1000,
            0,
            0,
            0});
			this.nudMaxGenerations.Name = "nudMaxGenerations";
			this.nudMaxGenerations.Size = new System.Drawing.Size (50, 20);
			this.nudMaxGenerations.TabIndex = 1;
			this.nudMaxGenerations.Value = new decimal (new int [] {
            200,
            0,
            0,
            0});
			// 
			// nudPopulationSize
			// 
			this.nudPopulationSize.Location = new System.Drawing.Point (134, 41);
			this.nudPopulationSize.Maximum = new decimal (new int [] {
            10000,
            0,
            0,
            0});
			this.nudPopulationSize.Name = "nudPopulationSize";
			this.nudPopulationSize.Size = new System.Drawing.Size (50, 20);
			this.nudPopulationSize.TabIndex = 3;
			this.nudPopulationSize.Value = new decimal (new int [] {
            300,
            0,
            0,
            0});
			// 
			// nudMinPerf
			// 
			this.nudMinPerf.DecimalPlaces = 2;
			this.nudMinPerf.Location = new System.Drawing.Point (134, 219);
			this.nudMinPerf.Maximum = decimal.MaxValue;
			this.nudMinPerf.Minimum = decimal.MinValue;
			this.nudMinPerf.Name = "nudMinPerf";
			this.nudMinPerf.Size = new System.Drawing.Size (73, 20);
			this.nudMinPerf.TabIndex = 26;
			this.nudMinPerf.Value = new decimal (new int [] {
            9999999,
            0,
            0,
            0});
			// 
			// labelMinPerf
			// 
			this.labelMinPerf.AutoSize = true;
			this.labelMinPerf.Location = new System.Drawing.Point (39, 221);
			this.labelMinPerf.Name = "labelMinPerf";
			this.labelMinPerf.Size = new System.Drawing.Size (93, 13);
			this.labelMinPerf.TabIndex = 25;
			this.labelMinPerf.Text = "Min Performance:";
#endif
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point (421, 273);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size (75, 23);
			this.btnCancel.TabIndex = 23;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// toolTip1
			// 
			this.toolTip1.IsBalloon = true;
			// 
			// chkWalkForward
			// 
			this.chkWalkForward.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkWalkForward.Location = new System.Drawing.Point (39, 247);
			this.chkWalkForward.Name = "chkWalkForward";
			this.chkWalkForward.Size = new System.Drawing.Size (109, 24);
			this.chkWalkForward.TabIndex = 21;
			this.chkWalkForward.Text = "&Walk Forward";
			this.chkWalkForward.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add (this.tableLayoutPanel1);
			this.panel1.Location = new System.Drawing.Point (213, 13);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size (283, 248);
			this.panel1.TabIndex = 24;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add (new System.Windows.Forms.ColumnStyle (System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add (new System.Windows.Forms.ColumnStyle (System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel1.Location = new System.Drawing.Point (0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add (new System.Windows.Forms.RowStyle (System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add (new System.Windows.Forms.RowStyle (System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size (281, 0);
			this.tableLayoutPanel1.TabIndex = 0;

			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF (96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size (508, 308);
			this.Controls.Add (this.panel1);
			this.Controls.Add (this.chkWalkForward);
			this.Controls.Add (this.btnCancel);
			this.Controls.Add (this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsDialog";
			this.Text = "PH Genetic Options";
#if !NT7
			this.Controls.Add (this.labelMinPerf);
			this.Controls.Add (this.label2);
			this.Controls.Add (this.label1);
			this.Controls.Add (this.nudMinPerf);
			this.Controls.Add (this.nudPopulationSize);
			this.Controls.Add (this.nudMaxGenerations);
			((System.ComponentModel.ISupportInitialize) (this.nudMaxGenerations)).EndInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudPopulationSize)).EndInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudMinPerf)).EndInit ();
#endif
			this.panel1.ResumeLayout (false);
			this.panel1.PerformLayout ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}
		#endregion

		ParameterDefinitions _rgParameterDefinitions;

		internal void InitializeParameters (ParameterDefinitions rgParameterDefinitions)
		{
			if (_rgParameterDefinitions != null)
				rgParameterDefinitions.InitializeValuesFrom (_rgParameterDefinitions);

			tableLayoutPanel1.Controls.Clear ();
			tableLayoutPanel1.RowCount = 1;
			_rgParameterDefinitions = rgParameterDefinitions;

			bool fShowColumns = false;

			int iRow = 0;

			foreach (ParameterDefinition pd in rgParameterDefinitions)
			{
				SetParameterDefinition ed = pd as SetParameterDefinition;
				if (ed != null)
				{
					fShowColumns = true;

					CheckBox cbTitle = new CheckBox ();
					cbTitle.Text = ed.Parameter.Name;
					cbTitle.AutoSize = true;
					cbTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

					int iRowStart = iRow;

					List<CheckBox> rgCheckboxes = new List<CheckBox> ();

					bool fUpdating = false;

					Action<bool> initTitleCheckbox = delegate (bool f)
					{
						bool fAllChecked = true;
						bool fAllUnchecked = true;

						foreach (CheckBox cb in rgCheckboxes)
						{
							if (cb.Checked)
								fAllUnchecked = false;
							else
								fAllChecked = false;
						}

						if (!fAllChecked && !fAllUnchecked)
							cbTitle.CheckState = CheckState.Indeterminate;
						else
							cbTitle.CheckState = fAllChecked ? CheckState.Checked : CheckState.Unchecked;
					};

					foreach (SetParameterDefinition.ValueInfo pvdEnum in ed.GetPossibleValues ())
					{
						SetParameterDefinition.ValueInfo pvd = pvdEnum;	// extract for closure

						CheckBox cbValue = new CheckBox ();
						cbValue.Text = pvd.Name;
						cbValue.AutoSize = true;
						cbValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						cbValue.Margin = new Padding (0);
						cbValue.Checked = pvd.IsActive;
						cbValue.CheckedChanged += delegate (object sender, EventArgs args)
						{
							pvd.Setter (cbValue.Checked);

							if (fUpdating)
								return;

							try
							{
								fUpdating = true;
								initTitleCheckbox (true);
							}
							finally
							{
								fUpdating = false;
							}
						};

						rgCheckboxes.Add (cbValue);

						while (iRow >= tableLayoutPanel1.RowCount)
							tableLayoutPanel1.RowCount++;

						tableLayoutPanel1.RowStyles.Add (new RowStyle (SizeType.AutoSize));
						tableLayoutPanel1.Controls.Add (cbValue, 1, iRow);

						iRow++;
					}

					tableLayoutPanel1.Controls.Add (cbTitle, 0, iRowStart);
					tableLayoutPanel1.SetRowSpan (cbTitle, tableLayoutPanel1.RowCount - iRowStart - 1);

					initTitleCheckbox (true);

					cbTitle.CheckedChanged += delegate (object sender, EventArgs args)
					{
						if (fUpdating)
							return;

						try
						{
							fUpdating = true;
							foreach (CheckBox cb in rgCheckboxes)
								cb.Checked = cbTitle.Checked;
						}
						finally
						{
							fUpdating = false;
						}
					};
				}
			}

			if (fShowColumns != panel1.Enabled)
			{
				panel1.Visible = panel1.Enabled = fShowColumns;

				if (fShowColumns)
					this.Width += panel1.Width;
				else
					this.Width -= panel1.Width;
			}
		}
	}



	public class ParameterDefinitions : IEnumerable<ParameterDefinition>
	{
		ParameterCollection _rgParams;
		List<ParameterDefinition> _rgParameterDefinitions = new List<ParameterDefinition> ();

		public ParameterDefinitions (StrategyBase strategy, bool fInitializeValues)
		{
			_rgParams = strategy.Parameters;

			Type typeStrategy = strategy.GetType ();

			for (int iParam = 0; iParam < _rgParams.Count; iParam++)
			{
				Parameter param = _rgParams [iParam];
				Type type = param.ParameterType;
				if (type.IsEnum)
				{
					EnumParameterDefinition epdEnum = new EnumParameterDefinition (typeStrategy, param, iParam);
					_rgParameterDefinitions.Add (epdEnum);

					if (fInitializeValues)
					{
						object value = epdEnum.ReadValue (strategy);
						if (value != null)
							epdEnum.AddValue (value);
					}
				}
				else if (type == typeof (bool))
				{
					BooleanParameterDefinition epdBool = new BooleanParameterDefinition (typeStrategy, param, iParam);
					_rgParameterDefinitions.Add (epdBool);

					if (fInitializeValues)
						epdBool.AddValue (epdBool.ReadValue (strategy));
				}
				else if (type.IsPrimitive)
				{
					_rgParameterDefinitions.Add (new IntegralParameterDefinition (_rgParams, iParam));
				}
			}
		}

		public ParameterDefinition this [int i] { get { return _rgParameterDefinitions [i]; } }

		public string ToString (ParameterSet dna)
		{
			StringBuilder sb = new StringBuilder ();
			for (int iParam = 0; iParam < _rgParameterDefinitions.Count; iParam++)
			{
				if (iParam > 0)
					sb.Append (",\t");

				ParameterDefinition pd = _rgParameterDefinitions [iParam];
				sb.Append (pd.ToString ());
				sb.Append ('=');
				sb.Append (pd.ToString (dna.GetParam (iParam)));
			}
			return sb.ToString ();
		}

		public void WriteToStrategy (ParameterSet child, StrategyBase strategy, ParameterCollection rgParams)
		{
			// initialize the strategy parameters
			for (int iParam = 0; iParam < _rgParameterDefinitions.Count; iParam++)
			{
				ParameterDefinition pd = _rgParameterDefinitions [iParam];
				pd.WriteValue (strategy, rgParams, child.GetParam (iParam));
			}
		}

		public ParameterDefinition Find (ParameterDefinition pd)
		{
			foreach (ParameterDefinition pdFind in _rgParameterDefinitions)
			{
				if (pdFind.Parameter.Name == pd.Parameter.Name &&
					pdFind.Parameter.ParameterType.Name == pd.Parameter.ParameterType.Name)
				{
					return pdFind;
				}
			}
			return null;
		}

		public void InitializeValuesFrom (ParameterDefinitions other)
		{
			foreach (ParameterDefinition pdNew in this)
			{
				ParameterDefinition pdOld = other.Find (pdNew);
				if (pdOld == null)
					continue;

				SetParameterDefinition spdOld = pdOld as SetParameterDefinition;
				SetParameterDefinition spdNew = pdNew as SetParameterDefinition;
				if (spdOld == null || spdNew == null)
					continue;

				spdNew.InitializeFrom (spdOld);
			}
		}

		public int Count { get { return _rgParameterDefinitions.Count; } }

		public decimal CountCombinations
		{
			get
			{
				decimal cCombinations = 1;
				foreach (ParameterDefinition pd in _rgParameterDefinitions)
					cCombinations *= pd.CountValues;
				return cCombinations;
			}
		}

		public IEnumerator<ParameterDefinition> GetEnumerator ()
		{
			return _rgParameterDefinitions.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _rgParameterDefinitions.GetEnumerator ();
		}
	}

	public abstract class ParameterDefinition : IEquatable<ParameterDefinition>
	{
		readonly Parameter _param;
		public Parameter Parameter { get { return _param; } }

		readonly int _iParam;
		protected int Index { get { return _iParam; } }


		public ParameterDefinition (Parameter param, int iParam)
		{
			_param = param;
			_iParam = iParam;
		}

		public abstract void WriteValue (StrategyBase strategy, ParameterCollection rgParams, int iValue);
		public abstract int CountValues { get; }

		#region IEquatable<ParameterDefinition> Members

		public bool Equals (ParameterDefinition other)
		{
			if (other == null)
				return false;

			if (this.Parameter.Name != other.Parameter.Name)
				return false;

			if (this.Parameter.ParameterType.Name != other.Parameter.ParameterType.Name)
				return false;

			return true;
		}

		#endregion

		public override string ToString ()
		{
			return _param.Name;
		}

		public abstract string ToString (int iValue);
	}

	class IntegralParameterDefinition : ParameterDefinition
	{
		readonly int _cValues;

		readonly ParameterCollection _params;
		public ParameterCollection Parameters { get { return _params; } }

		public IntegralParameterDefinition (ParameterCollection rgParams, int iParam)
			: base (rgParams [iParam], iParam)
		{
			_params = rgParams;

			double max = Math.Max (this.Parameter.Max, this.Parameter.Min);
			_cValues = 1 + (int) Math.Floor ((max - this.Parameter.Min + double.Epsilon) / this.Parameter.Increment);
		}

		public override int CountValues { get { return _cValues; } }

		public override void WriteValue (StrategyBase strategy, ParameterCollection rgParams, int iValue)
		{
			if (iValue < 0 || iValue >= this.CountValues)
				throw new ArgumentOutOfRangeException ("iValue");

			rgParams [this.Index].Value = this.Parameter.Min + this.Parameter.Increment * iValue;
		}

		public override string ToString (int iValue)
		{
			double value = this.Parameter.Min + this.Parameter.Increment * iValue;

			if (this.Parameter.ParameterType == typeof (double) ||
				this.Parameter.ParameterType == typeof (float))
			{
				return value.ToString ("N2");
			}
			return value.ToString ();
		}
	}

	abstract class SetParameterDefinition : ParameterDefinition
	{
		public class ValueInfo
		{
			public readonly string Name;
			public readonly bool IsActive;
			public readonly Action<bool> Setter;

			public ValueInfo (string strName, bool fActive, Action<bool> setter)
			{
				this.Name = strName;
				this.IsActive = fActive;
				this.Setter = setter;
			}
		}

		public SetParameterDefinition (Parameter param, int iParam)
			: base (param, iParam)
		{
		}

		public abstract IEnumerable<ValueInfo> GetPossibleValues ();

		public abstract void InitializeFrom (SetParameterDefinition other);
	}

	abstract class SetParameterDefinition<T> : SetParameterDefinition
	{
		protected List<T> _rgValues = new List<T> ();

		PropertyInfo _pi;

		public SetParameterDefinition (Type typeStrategy, Parameter param, int iParam)
			: base (param, iParam)
		{
			_pi = typeStrategy.GetProperty (this.Parameter.Name);
		}

		public override int CountValues { get { return _rgValues.Count; } }

		public override void WriteValue (StrategyBase strategy, ParameterCollection rgParams, int iValue)
		{
			if (iValue < 0 || iValue >= this.CountValues)
				throw new ArgumentOutOfRangeException ("iValue");

			rgParams [this.Index].Value = GetValueToWrite (iValue);
			_pi.SetValue (strategy, _rgValues [iValue], null);
		}

		protected virtual object GetValueToWrite (int iValue)
		{
			return _rgValues [iValue];
		}

		public T ReadValue (StrategyBase strategy)
		{
			return (T) _pi.GetValue (strategy, null);
		}

		public T GetValue (int iValue)
		{
			if (iValue < 0 || iValue >= this.CountValues)
				throw new ArgumentOutOfRangeException ("iValue");

			return _rgValues [iValue];
		}

		internal void AddValue (T value)
		{
			if (!_rgValues.Contains (value))
				_rgValues.Add (value);
		}

		internal void RemoveValue (T value)
		{
			_rgValues.Remove (value);
		}

		public bool Contains (T value)
		{
			return _rgValues.Contains (value);
		}

		public override void InitializeFrom (SetParameterDefinition otherTmp)
		{
			SetParameterDefinition<T> other = otherTmp as SetParameterDefinition<T>;
			if (other == null)
				return;

			_rgValues = new List<T> (other._rgValues);
		}

		public override string ToString (int iValue)
		{
			return GetValue (iValue).ToString ();
		}
	}


	class BooleanParameterDefinition : SetParameterDefinition<bool>
	{
		public BooleanParameterDefinition (Type typeStrategy, Parameter param, int iParam)
			: base (typeStrategy, param, iParam)
		{
		}

		static readonly bool [] __rgValues = new bool [] { true, false };

		public override IEnumerable<ValueInfo> GetPossibleValues ()
		{
			foreach (bool valueEnum in __rgValues)
			{
				bool value = valueEnum;	// extract for closure
				yield return new ValueInfo (value.ToString (), this.Contains (value), delegate (bool fChecked) { if (fChecked) AddValue (value); else RemoveValue (value); });
			}
		}
	}

	class EnumParameterDefinition : SetParameterDefinition<object>
	{
		public EnumParameterDefinition (Type typeStrategy, Parameter param, int iParam)
			: base (typeStrategy, param, iParam)
		{
		}

		public override IEnumerable<ValueInfo> GetPossibleValues ()
		{
			Type type = this.Parameter.ParameterType;
			foreach (object valueEnum in Enum.GetValues (type))
			{
				object value = valueEnum;	// extract for closure
				yield return new ValueInfo (Enum.GetName (type, value), this.Contains (value), delegate (bool fChecked) { if (fChecked) AddValue (value); else RemoveValue (value); });
			}
		}

		public override void InitializeFrom (SetParameterDefinition otherTmp)
		{
			EnumParameterDefinition other = otherTmp as EnumParameterDefinition;
			if (other == null)
				return;

			for (int iValue = 0; iValue < other.CountValues; iValue++)
			{
				object value = other.GetValue (iValue);
				string strName = Enum.GetName (otherTmp.Parameter.ParameterType, value);
				if (strName != null)
				{
					object valueNew = Enum.Parse (this.Parameter.ParameterType, strName);
					if (valueNew != null)
						this.AddValue (valueNew);
				}
			}
		}

		protected override object GetValueToWrite (int iValue)
		{
#if NT7
			return (int) _rgValues [iValue];
#else
			return Convert.ToDouble (_rgValues [iValue]);
#endif
		}
	}

	public class ParameterSet : IEquatable<ParameterSet>, IComparable<ParameterSet>, IComparable
	{
		public double Performance;
		public double Weight = 1;

		int _hashcode = 0;
		int [] _rgParams;

		public ParameterSet (int [] rgParams)
		{
			_rgParams = rgParams;

			foreach (int i in rgParams)
				_hashcode = (_hashcode << 1) ^ i ^ ((_hashcode & 0x80020) >> 4);
		}

		public int CompareTo (ParameterSet i)
		{
			return i.Performance.CompareTo (this.Performance);
		}

		public int CompareTo (object o)
		{
			ParameterSet i = (ParameterSet) o;
			return i.Performance.CompareTo (this.Performance);
		}

		public override int GetHashCode () { return _hashcode; }

		public bool Equals (ParameterSet other)
		{
			if (other == null)
				return false;

			if (_rgParams.Length != other._rgParams.Length)
				return false;

			for (int i = 0; i < _rgParams.Length; i++)
				if (_rgParams [i] != other._rgParams [i])
					return false;

			return true;
		}

		public int GetParam (int iParam) { return _rgParams [iParam]; }

		public override string ToString ()
		{
			StringBuilder sb = null;

			foreach (int i in _rgParams)
			{
				if (sb == null)
					sb = new StringBuilder ();
				else
					sb.Append (',');
				sb.Append (i);
			}
			return sb.ToString ();
		}

		public void OnIterationComplete (object state, double performanceValue)
		{
			this.Performance = performanceValue;
		}
	}

	public class AbortOptimizationException : Exception
	{
	}

	public abstract class PHOptimizationMethod : OptimizationMethod
	{
		protected Random _rand = new Random ();

		protected ParameterDefinitions _rgParameterDefinitions;

		protected void Print (string str)
		{
			System.Diagnostics.Debug.WriteLine (str);
			Strategy.Print (str);
		}

		protected IEnumerable<T> RunIterations<T> (IEnumerable<T> rgChildren)
			where T : ParameterSet
		{
			foreach (T child in rgChildren)
				RunIteration (child);

#if NT7
			WaitForIterationsCompleted (false);
#endif

			return rgChildren;
		}

		protected void RunIteration (ParameterSet child)
		{
			if (Gui.Globals.Progress.Aborted)
				throw new AbortOptimizationException ();

			_rgParameterDefinitions.WriteToStrategy (child, this.Strategy, this.Parameters);

#if NT7
			RunIteration (child.OnIterationComplete, null);
#else
			this.Strategy.RunIteration ();

			// record the strategy's performance
			child.Performance = NinjaTrader.Strategy.Strategy.LastPerformanceValue;
#endif

		}

		public void Dump (ParameterSet dna)
		{
			for (int iParam = 0; iParam < _rgParameterDefinitions.Count; iParam++)
			{
				ParameterDefinition pd = _rgParameterDefinitions [iParam];
				Print (pd.ToString () + "=\t" + pd.ToString (dna.GetParam (iParam)));
			}
		}

		protected int _cMaxGenerations = 200;
		protected int _nPopulationSize = 300;
		protected double _nMinimumPerformce = 10000000;

#if NT7
		[Description ("The maximum number of generations.")]
		[Category ("Optimize")]
		[DisplayName ("Generation Count")]
		public int MaxGenerations
		{
			get { return _cMaxGenerations; }
			set { _cMaxGenerations = Math.Max (1, value); }
		}

		[Description ("The number of individuals in each generation.")]
		[Category ("Optimize")]
		[DisplayName ("Generation Size")]
		public int PopulationSize
		{
			get { return _nPopulationSize; }
			set { _nPopulationSize = Math.Max (1, value); }
		}

		[Description ("The minimum required performance. Optimization will stop once this performance is reached.")]
		[Category ("Optimize")]
		[DisplayName ("Minimum required Performance")]
		public double MinimumPerformance
		{
			get { return _nMinimumPerformce; }
			set { _nMinimumPerformce = value; }
		}

		public override int Iterations
		{
			get { return _cMaxGenerations * _nPopulationSize; }
		}

		public override bool MultiThreadSupport
		{
			get { return (Strategy != null && Strategy.MultiThreadSupport && NinjaTrader.Cbi.Globals.ProcessorsEnabled > 1); }
		}
#else
		protected ParameterCollection Parameters { get { return this.Strategy.Parameters; } }
#endif
	}
}
