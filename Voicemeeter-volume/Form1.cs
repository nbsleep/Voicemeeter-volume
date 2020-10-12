using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Voicemeeter;

namespace Voicemeeter_volume
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource taskToken;
        private Task task;
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 50);
            taskToken = new CancellationTokenSource();
            VoiceMeeter.Remote.Initialize(RunVoicemeeterParam.VoicemeeterBanana).ConfigureAwait(false);
            var levels = new BusVolume(new int[] {
                0
            }, 100);
            levels.Subscribe(x => UpdateVolume(x));
        }

        private void UpdateVolume(float[] x)
        {
            this.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                this.Visible = true;
                int value = ((Convert.ToInt32(x[0]) + 80) * 100) / 92;
                this.progressBar1.Value = value;
                this.label1.Text = value.ToString();
            });

            if(task == null)
                task = Task.Delay(2000).ContinueWith(t => HideApp());
            else
            {
                if (task.IsCompleted)
                {
                    taskToken = new CancellationTokenSource();
                    task = Task.Delay(2000).ContinueWith(t => HideApp());
                }

                if(!taskToken.IsCancellationRequested)
                {
                    taskToken.Cancel();
                }
            }
            

        }

        private void HideApp()
        {
            Console.WriteLine("Hide: " + taskToken.IsCancellationRequested);
            // Were we already canceled?
            if (taskToken.IsCancellationRequested)
            {
                taskToken = new CancellationTokenSource();
                task = Task.Delay(2000).ContinueWith(t => HideApp());
                return;
            }
            this.Invoke((MethodInvoker)delegate {
                this.Visible = false;
            });

        }
    }
}
