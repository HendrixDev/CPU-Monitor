using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;

namespace CPU_Monitor
{
    class Program
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            synth.Speak("Hello");

            #region Performance counters
            // pull the current cpu load in percentage
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor time", "_Total");
            perfCpuCount.NextValue();

            //pull the current available memory in Megabytes
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();


            //pull the system uptime, in seconds
            PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            #endregion

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
            string systemUptimeMessage = string.Format("The current system uptime is {0} days {1} hours {2} minutes {3} seconds",
                (int)uptimeSpan.TotalDays,
                (int)uptimeSpan.Hours,
                (int)uptimeSpan.Minutes,
                (int)uptimeSpan.Seconds);

            int speechSpeed = 1;

            //tell the user what the current system uptime is
            Speak(systemUptimeMessage, VoiceGender.Male, speechSpeed++);

            while (true)
            {
                //get current performance 
                int currentCpuPercentage = (int)perfCpuCount.NextValue();
                int currentAvailableMemory = (int)perfMemCount.NextValue();
                //every 1 sec, print the CPU load to the screen
                Console.WriteLine("CPU Load: {0}%", currentCpuPercentage);
                Console.WriteLine("Available Memory: {0}MB", currentAvailableMemory);

                //speech notifications if cpu load is above 80%
                if (currentCpuPercentage > 80)
                {
                    if (currentCpuPercentage == 100)
                    {
                        if (speechSpeed < 5)
                        {
                            speechSpeed++;
                        }
                        string cpuLoadVocalMessage = String.Format("WARNING: You're CPU load is maxed out");
                        Speak(cpuLoadVocalMessage, VoiceGender.Female, speechSpeed);
                    }
                    else
                    {
                        //tell user what the current values are
                        string cpuLoadVocalMessage = String.Format("The current CPU Load is {0}", currentCpuPercentage);
                        Speak(cpuLoadVocalMessage, VoiceGender.Female, 5);
                    }
                }

                //notifications if memory is below one gigabyte
                if (currentAvailableMemory < 1024)
                {
                    //speak to user to tell them what the current values are
                    synth.SelectVoiceByHints(VoiceGender.Male);
                    string memAvailableVocalMessage = String.Format("You currently have {0} megabytes of memory available ", currentAvailableMemory);
                    Speak(memAvailableVocalMessage, VoiceGender.Male, 10);
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// speaks with a selected voice
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        public static void Speak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
        }

        /// <summary>
        /// speaks with a selected voice, at a selected speed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        /// <param name="rate"></param>
        public static void Speak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            Speak(message, voiceGender);
        }
    }
}
