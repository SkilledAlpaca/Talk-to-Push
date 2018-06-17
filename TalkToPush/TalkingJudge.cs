using System;
using System.Timers;

namespace TalkToPush
{
    class TalkingJudge
    {
        private int lingerTime;
        private int sneezeTime;
        private KeyPresser presser;
        private Timer lingerTimer;
        private Timer sneezeTimer;
        private bool mutedForSneeze;

        public TalkingJudge(KeyPresser presser, int lingerTime, int sneezeTime) {
            this.lingerTime = lingerTime;
            this.presser = presser;
            this.sneezeTime = sneezeTime;

            setupTimers();

        }

        private void setupTimers() {

            lingerTimer = new Timer() { AutoReset = false, Interval = lingerTime };
            lingerTimer.Elapsed += lingerTimer_Elapsed;

            sneezeTimer = new Timer() { AutoReset = false, Interval = sneezeTime };
            sneezeTimer.Elapsed += sneezeTimer_Elapsed;
        }

        void sneezeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            mutedForSneeze = false;
            System.Media.SystemSounds.Beep.Play();
        }

        void lingerTimer_Elapsed(object sender, EventArgs e)
        {
            presser.release();
        }

        public void gainSound() {

            if (mutedForSneeze) {
                restartTimer(sneezeTimer);
            } else {
                if (lingerTimer.Enabled) {
                    lingerTimer.Stop();
                } else {
                    presser.beginHold();
                }
            }
        }

        public void loseSound() {

            if (!hearsTalking()) {
                return;
            }

            if (!lingerTimer.Enabled) {
                restartTimer(lingerTimer);
            }
        }

        public bool hearsTalking() {
            return presser.isPressing();
        }

        public void sneezeIncoming() {
            if (sneezeTimer.Enabled) {
                restartTimer(sneezeTimer);
            } else {
                presser.release();
                mutedForSneeze = true;
                sneezeTimer.Start();
                System.Media.SystemSounds.Hand.Play();
            }
        }

        public bool isMuted() {
            return mutedForSneeze;
        }

        private void restartTimer(Timer t)
        {
            t.Stop();
            t.Start();
        }
    }
}
