﻿using SmallDemoManager.HelperClass;
using NAudio.Wave;
using System.Text.RegularExpressions;
using static SmallDemoManager.HelperClass.SavedAudioFiles;

namespace SmallDemoManager.AudioExtract
{
    /// <summary>
    /// Helper class for reading and playing voice audio files extracted from demos.
    /// </summary>
    public static class AudioReadHelper
    {
        private static IWavePlayer? _waveOut;                  // Used for audio playback
        private static AudioFileReader? _audioFileReader;      // Used to read the audio file
        private static readonly object _lock = new(); // Synchronisation
        private static EventHandler<StoppedEventArgs>? _playbackStoppedHandler;

        /// <summary>
        /// Retrieves all audio entries for a given Steam ID from a specified folder.
        /// Parses .wav file names to extract round and time information.
        /// </summary>
        /// <param name="steamID">The Steam ID of the player.</param>
        /// <param name="audioFolderPath">The base path containing all player audio folders.</param>
        /// <returns>List of AudioEntry objects.</returns>
        public static List<AudioEntry> GetAudioEntries(string steamID, string audioFolderPath)
        {
            var result = new List<AudioEntry>();
            string userFolderPath = Path.Combine(audioFolderPath, steamID);

            if (!Directory.Exists(userFolderPath))
                return result;

            string[] files = Directory.GetFiles(userFolderPath, "*.wav");
            var regex = new Regex(@"round_(\d+)_t_(\d+)s", RegexOptions.IgnoreCase);

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var match = regex.Match(fileName);
                if (match.Success)
                {
                    // Extract round and timestamp from filename using regex
                    int round = int.Parse(match.Groups[1].Value);
                    int seconds = int.Parse(match.Groups[2].Value);
                    TimeSpan time = TimeSpan.FromSeconds(seconds);

                    double duration = GetWavDuration(file); // Get actual audio length

                    result.Add(new AudioEntry
                    {
                        Round = round,
                        Time = time,
                        DurationSeconds = duration,
                        FilePath = file
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Attempts to play the audio file associated with the given entry.
        /// </summary>
        /// <param name="entry">The AudioEntry to play.</param>
        public static void PlayAudio(Form owner, AudioEntry entry)
        {
            if (File.Exists(entry.FilePath))
            {
                Play(entry.FilePath);
            }
            else
            {
                MaterialUiHelper.ShowSnack(owner, $"Unable to read wave file!", true);
            }
        }

        /// <summary>
        /// Attempts to play the audio file associated with the given entry.
        /// </summary>
        /// <param name="entry">The AudioEntry to play.</param>
        public static void PlaySavedAudio(Form owner, AudioFileInfo entry)
        {
            if (File.Exists(entry.FullPath))
            {
                Play(entry.FullPath);
            }
            else
            {
                MaterialUiHelper.ShowSnack(owner, $"Unable to read wave file!", true);
            }
        }

        /// <summary>
        /// Plays a specified .wav file using NAudio.
        /// Automatically stops and disposes any existing playback instance first.
        /// </summary>
        /// <param name="filePath">The path to the .wav file.</param>
        public static void Play(string filePath)
        {
            lock (_lock)
            {
                Stop(); // Clean up previous instance.

                _waveOut = new WaveOutEvent();
                _audioFileReader = new AudioFileReader(filePath);

                _waveOut.Init(_audioFileReader);

                // EMark the event handler so that we can deregister it cleanly later on.
                _playbackStoppedHandler = (s, e) =>
                {
                    Stop();
                };
                _waveOut.PlaybackStopped += _playbackStoppedHandler;

                _waveOut.Play();
            }
        }

        /// <summary>
        /// Stops any currently playing audio and disposes associated resources.
        /// </summary>
        public static void Stop()
        {
            lock (_lock)
            {
                if (_waveOut != null && _playbackStoppedHandler != null)
                {
                    _waveOut.PlaybackStopped -= _playbackStoppedHandler;
                    _playbackStoppedHandler = null;
                }

                _waveOut?.Stop();
                _audioFileReader?.Dispose();
                _waveOut?.Dispose();

                _audioFileReader = null;
                _waveOut = null;
            }
        }

        /// <summary>
        /// Gets the duration of a .wav audio file in seconds.
        /// </summary>
        /// <param name="filePath">Path to the audio file.</param>
        /// <returns>Duration in seconds.</returns>
        private static double GetWavDuration(string filePath)
        {
            using (var reader = new AudioFileReader(filePath))
            {
                return reader.TotalTime.TotalSeconds;
            }
        }
    }
}