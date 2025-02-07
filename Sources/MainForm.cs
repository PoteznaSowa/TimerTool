/*
 * Created by SharpDevelop.
 * User: Tebjan Halm
 * Date: 21.01.2014
 * Time: 16:55
 * 
 * Modified by PoteznaSowa.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
//using System.Globalization;

namespace TimerTool {
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form {
		public MainForm(string[] args) {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			notifyIcon1.Icon = (Icon)Properties.Resources.ResourceManager.GetObject("TimerIcon");

			// Process command line arguments.
			var i = 0;
			bool skip = false;
			foreach (var arg in args) {
				if (skip) {
					skip = false;
					continue;
				}
				string errormsg = "";
				switch (arg) {
					case "-h":
						goto default;
					case "-t":
						if (args.Length <= (i + 1)) {
							errormsg = "Missing <period> argument.\n\n";
							goto default;
						}
						i++;
						skip = true;
						if (int.TryParse(
								args[i],
								//NumberStyles.Integer | NumberStyles.AllowThousands,
								//CultureInfo.InvariantCulture,
								out int val
								) && Array.Exists(validValues, n => n == val)
							)
						{
							period = val;
						} else {
							errormsg = $"Invalid timer period value: {args[i]}\n\n";
							goto default;
						}
						break;
					case "-m":
						allowVisible = false;
						break;
					default:
						MessageBox.Show(
							errormsg +
							"-h: Show this message.\n" +
							"-t <period>: Set the timer resolution to <period>, " +
							"expressed in the number of 100 nanosecond units. " +
							"<period> must be one of these values: " +
							"62500, 78125, 80000, 100000, or 125000.\n" +
							"-m: Start the program minimised.",
							"Launch Arguments",
							MessageBoxButtons.OK,
							MessageBoxIcon.Asterisk
							);
						break;
				}

				i++;
			}
		}

		private bool allowVisible = true;
		private int period = 0;

		// Generally, raising timer resolution is bad, as higher timer
		// resolutions increase power consumption and may reduce performance.
		// However, some programs may work better with just a bit shorter
		// timer period of not less than about 4 ms.
		// The Windows XP HAL has a bug which prevents the hardware timer
		// from being set to 7.8125 ms (128 Hz) when a program requests
		// a resolution of 8 to 15 ms. In this case, the timer stays at
		// its normal resolution of 15.625 ms (64 Hz).
		// Thus, the resolution of less than 7.8125 has to be requested
		// to take effect.
		private int[] validValues = {
			62500,	// 160 Hz
			78125,	// 128 Hz
			80000,	// 125 Hz
			100000,	// 100 Hz
			125000	// 80 Hz
		};

		private void NotifyIcon1_Click(object sender, EventArgs e) {
			Show();
			WindowState = FormWindowState.Normal;
			ShowInTaskbar = true;
			notifyIcon1.Visible = false;
		}

		private void MainForm_Resize(object sender, EventArgs e) {
			switch (WindowState) {
				case FormWindowState.Minimized:
					Minimize();
					break;
				case FormWindowState.Normal:
					notifyIcon1.Visible = false;
					break;
			}
		}

		private void Minimize() {
			notifyIcon1.Visible = true;
			notifyIcon1.ShowBalloonTip(250);
			Hide();
		}

		int UpdateTimerInfo() {
			WinApiCalls.NtQueryTimerResolution(out int min, out int max, out int current);
			CurrentLabel.Text = $"Current: {current / 10000M} ms";
			MinLabel.Text = $"Max: {min / 10000M} ms";
			MaxLabel.Text = $"Min: {max / 10000M} ms";
			notifyIcon1.Text = $"TimerTool\nCurrent resolution: {current / 10000M} ms";
			//numericUpDown2.Minimum = max / 10000M;
			//numericUpDown2.Maximum = min / 10000M;
			return current;
		}

		int SetTimerResolution(int period) {
			WinApiCalls.NtSetTimerResolution(period, period > 0, out int current);
			return current;
		}

		void MainFormLoad(object sender, EventArgs e) {
#if false
			if (period > validValues[validValues.Length - 1]) {
				period = 0;
			}
			if (period > 0 && period < validValues[0]) {
				period = validValues[0];
			}
			for (int i = 1; i < validValues.Length; i++) {
				if (period == 0) {
					break;
				}
				if (period > validValues[i - 1] && period <= validValues[i]) {
					period = validValues[i];
					trackBar1.Value = i;
					break;
				}
			}
#endif

			if (period != 0) {
				SetTimerResolution(period);
				trackBar1.Value = Array.FindIndex(validValues, n => n == period);
			}

			UpdateTimerInfo();
			label2.Text = $"Desired resolution: {validValues[trackBar1.Value] / 10000M} ms";
			if (!allowVisible) {
				WindowState = FormWindowState.Minimized;
				ShowInTaskbar = false;
			}
		}

		void Timer1Tick(object sender, EventArgs e) {
			int current = UpdateTimerInfo();
			if (period > 0 && current < period) {
				SetTimerResolution(0);
				SetTimerResolution(period);
				UpdateTimerInfo();
			}
		}

		void SetTimerButtonClick(object sender, EventArgs e) {
			SetTimerResolution(0);
			period = validValues[trackBar1.Value];
			SetTimerResolution(period);
			UpdateTimerInfo();
		}

		void UnsetTimerButtonClick(object sender, EventArgs e) {
			SetTimerResolution(0);
			period = 0;
			UpdateTimerInfo();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			SetTimerResolution(0);
			period = 0;
		}

		private void trackBar1_ValueChanged(object sender, EventArgs e) {
			label2.Text = $"Desired resolution: {validValues[trackBar1.Value] / 10000M} ms";
		}
	}
}
