using NAudio.Wave;
using System.Collections.Generic;

namespace TalkToPush
{
    class AudioSystemHelper
    {

        public static IEnumerable<WaveInCapabilities> ListAudioInputDevices()
        {
            List<WaveInCapabilities> mics = new List<WaveInCapabilities>();

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                mics.Add(WaveIn.GetCapabilities(i));
            }

            return mics;
            
        }
    }
}
