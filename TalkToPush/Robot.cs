using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TalkToPush
{
    /**
     * This class delegates its work to RobotWrapper.jar.
     * I do this because java's robot class is the only thing that has proven to send keystrokes
     * to the Battlefield 4 Beta reliably.
     */
    class Robot : IDisposable
    {

        private Process roboProcess;

        public Robot()
        {
            initRoboProcess();
        }

        public void initRoboProcess()
        {
            roboProcess = new Process();
            roboProcess.StartInfo.FileName = "java";
            roboProcess.StartInfo.Arguments = "-jar " + "\"" + Path.GetDirectoryName(Application.ExecutablePath) + "\\lib\\RobotWrapper.jar" + "\"";
            roboProcess.StartInfo.RedirectStandardInput = true;
            roboProcess.StartInfo.UseShellExecute = false;
            roboProcess.StartInfo.CreateNoWindow = true;
            roboProcess.Start();
        }

        internal void keyPress(Keys key)
        {
            if (roboProcess.HasExited)
            {
                initRoboProcess();
            }

            roboProcess.StandardInput.WriteLine("1 " + KeyToCode(key));
            roboProcess.StandardInput.Flush(); // Send the press asap so we don't miss the first syllable.
        }

        internal void keyRelease(Keys key)
        {
            if (roboProcess.HasExited)
            {
                initRoboProcess();
            }

            roboProcess.StandardInput.WriteLine("0 " + KeyToCode(key));
            roboProcess.StandardInput.Flush(); // Send the release asap (might be a mute).
        }

        private byte KeyToCode(Keys key)
        {
            // Alt, Shift, and Control need to be changed from modifiers into keys
            switch (key)
            {
                case Keys.Alt:
                    return (byte)18;
                case Keys.Control:
                    return (byte)17;
                case Keys.Shift:
                    return (byte)16;
                default:
                    return (byte)key;
            }
        }


        void IDisposable.Dispose()
        {
            if (!roboProcess.HasExited)
            {
                roboProcess.StandardInput.WriteLine("-1");
            }
        }
    }
}
