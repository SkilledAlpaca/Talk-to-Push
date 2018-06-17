using NAudio.Wave;
using System;

namespace TalkToPush
{
    class LoudnessMonitor : IDisposable
    {
        public const int SCALING_FACTOR = 3;
        private WaveIn microphone;

        public delegate void LoudnessEventHandler(int loudness);

        public event LoudnessEventHandler LoudnessChanged;

        public LoudnessMonitor()
        {
            
        }

        public void SetMicrophone(int deviceNumber)
        {
            if (microphone != null)
            {
                microphone.DataAvailable -= microphone_DataAvailable;
                microphone.StopRecording();
            }

            microphone = new WaveIn();
            microphone.DeviceNumber = deviceNumber;
            microphone.WaveFormat = new WaveFormat(1000, 1);
            microphone.BufferMilliseconds = 40;
            microphone.DataAvailable += microphone_DataAvailable;
            microphone.StartRecording();
        }

        public void Dispose()
        {
            if (microphone != null)
            {
                microphone.Dispose();
            }
        }

        void microphone_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (LoudnessChanged != null)
            {
                int loudness = CalculateLoudness(e);
                LoudnessChanged(loudness);
            }
        }

        private int CalculateLoudness(WaveInEventArgs e)
        {
            short[] samples = getSamples(e);

            return (int)(SCALING_FACTOR * QuickVolume(samples));

        }

        private short[] getSamples(WaveInEventArgs e)
        {
            int numBytes = e.BytesRecorded;
            if (numBytes % 2 == 1)
            {
                numBytes--;
            }
            int numShorts = numBytes / 2;

            short[] samples = new short[numShorts];

            for (int i = 0; i < numBytes - 1; i += 2)
            {
                samples[i / 2] = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
            }

            return samples;
        }


        private int QuickVolume(short[] samples)
        {

            if (samples.Length == 0)
            {
                return 0;
            }

            long sum = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                sum += Math.Abs(samples[i]);
            }

            long average = sum / samples.Length;

            return (int)Math.Sqrt(average) * SCALING_FACTOR;
        }

    }
}
