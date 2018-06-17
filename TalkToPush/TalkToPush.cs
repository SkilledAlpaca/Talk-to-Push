using NAudio.Wave;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MovablePython;

namespace TalkToPush
{
    public partial class TalkToPush : Form
    {
        private KeyPresser presser;
        private LoudnessMonitor monitor;
        private TalkingJudge judge;
        private const int LINGER_PERIOD = 1000; // leave the voice channel open 1000 ms after the threshold is lost.
        private const int SNEEZE_PERIOD = 5000; // wait for five seconds of quiet before ending the mute.
        private Keys sneezeKey;
        private Hotkey sneezeHook;

        public TalkToPush()
        {
            InitializeComponent();

            presser = new KeyPresser();
            judge = new TalkingJudge(presser, LINGER_PERIOD, SNEEZE_PERIOD);
            monitor = new LoudnessMonitor();
            monitor.LoudnessChanged += monitor_LoudnessChanged;

            foreach (WaveInCapabilities microphone in AudioSystemHelper.ListAudioInputDevices().ToArray())
            {
                comboBox1.Items.Add(microphone.ProductName);
            }
            

            // Read in values from properties
            trackBar1.Value = Properties.Settings.Default.threshold;
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = Math.Min(comboBox1.Items.Count, Properties.Settings.Default.microphone);
            }
            SetTalkKey(Properties.Settings.Default.talkKey);
            SetSneezeKey(Properties.Settings.Default.sneezeKey);


        }

        private void setSneezeHook()
        {
            if (sneezeHook != null && sneezeHook.Registered)
            {
                sneezeHook.Unregister();
            }

            

            sneezeHook = new Hotkey(sneezeKey);
            sneezeHook.Pressed += sneezeHook_Pressed;

            try {
                sneezeHook.Register(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void sneezeHook_Pressed(object sender, HandledEventArgs e)
        {

            judge.sneezeIncoming();
            e.Handled = false;

        }

        private void SetTalkKey(Keys key)
        {
            talkKeyButton.Text = key.ToString();
            presser.setKey(key);
        }

        private void SetSneezeKey(Keys key)
        {
            sneezeKeyButton.Text = key.ToString();
            sneezeKey = key;
            setSneezeHook();
        }

        void monitor_LoudnessChanged(int loudness)
        {
            if (loudness > trackBar1.Value)
            {
                judge.gainSound();
            }
            else
            {
                judge.loseSound();
            }

            Invoke((MethodInvoker) delegate { UpdateUI(loudness); });
        }

        private void UpdateUI(int loudness)
        {

            progressBar1.Value = Clamp(loudness, progressBar1.Minimum, progressBar1.Maximum);

            muteStatus.BackColor = judge.isMuted() ? Color.Green : SystemColors.Control;
            pushStatus.BackColor = presser.isPressing() ? Color.Green : SystemColors.Control;
        }

        private int Clamp(int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max), min);
        }


        private void TalkToPush_FormClosing(object sender, FormClosingEventArgs e)
        {
            monitor.Dispose();
            if (sneezeHook.Registered)
            {
                sneezeHook.Unregister();
            }

            Properties.Settings.Default.talkKey = presser.getKey();
            Properties.Settings.Default.sneezeKey = sneezeKey;
            Properties.Settings.Default.threshold = trackBar1.Value;
            Properties.Settings.Default.microphone = comboBox1.SelectedIndex;

            Properties.Settings.Default.Save();
        }
        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            monitor.SetMicrophone(comboBox1.SelectedIndex);
        }


        private bool settingTalkKey = false;
        private bool settingSneezeKey = false;

        private void talkKeyButton_Click(object sender, EventArgs e)
        {
            settingTalkKey = true;
            settingSneezeKey = false;
            this.Cursor = Cursors.Help;
        }

        private void sneezeKeyButton_Click(object sender, EventArgs e)
        {
            settingSneezeKey = true;
            settingTalkKey = false;
            this.Cursor = Cursors.Help;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                keyData = Keys.Alt;
            }
            else if ((keyData & Keys.Control) == Keys.Control)
            {
                keyData = Keys.Control;
            }
            else
            {
                keyData &= Keys.KeyCode;
            }
            
            

            if (settingTalkKey)
            {
                SetTalkKey(keyData);
                settingTalkKey = false;
                this.Cursor = Cursors.Default;
                return true;
            }
            else if (settingSneezeKey)
            {
                SetSneezeKey(keyData);
                settingSneezeKey = false;
                this.Cursor = Cursors.Default;
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        
    }
}
