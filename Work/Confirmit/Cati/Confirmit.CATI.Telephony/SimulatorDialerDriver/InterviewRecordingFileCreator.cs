using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using ConfirmitDialerInterface;
using NAudio.Wave;

namespace SimulatorDialerDriver
{
    public class InterviewRecordingFileCreator
    {
        private readonly ILogger _logger;

        public InterviewRecordingFileCreator(ILogger logger)
        {
            _logger = logger;
        }

        public void CreateAudioFile(string campaignId, string campaignName, long interviewId, TimeSpan interviewDuration, string audioPath, int callAttempt, string label = null)
        {
            try
            {
                CreateAudioFolderIfNeeded(audioPath);

                DateTime creationTime = DateTime.UtcNow - interviewDuration;
                var startingFilePath = CreateStartingAudio(campaignId, campaignName, interviewId, interviewDuration,
                    audioPath, label, callAttempt);

                var startingFileDuration = GetDuration(startingFilePath);

                if (interviewDuration > startingFileDuration)
                {
                    var musicFilePath = GetRandomMusicFilePath();
                    var musicFileDuration = GetDuration(musicFilePath);

                    var musicFilesCount = (int)Math.Ceiling((interviewDuration - startingFileDuration).TotalSeconds /
                                                             musicFileDuration.TotalSeconds);

                    for (int i = 0; i < musicFilesCount; i++)
                    {
                        Concatenate(startingFilePath + "2", new[] { startingFilePath, musicFilePath });
                        File.Delete(startingFilePath);
                        File.Move(startingFilePath + "2", startingFilePath);
                    }
                }

                TrimWavFile(startingFilePath, startingFilePath.Replace(".tmp", ""), TimeSpan.Zero,
                    GetDuration(startingFilePath) - interviewDuration);

                File.SetCreationTimeUtc(startingFilePath.Replace(".tmp", ""), creationTime);

                File.Delete(startingFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error("InterviewRecordingFileCreator.CreateAudioFile", ex.ToString());
                CreateAudioFileWithNoTextToSpeech(campaignId, interviewId, interviewDuration, audioPath, callAttempt, label);
            }
        }

        private void CreateAudioFileWithNoTextToSpeech(string campaignId, long interviewId, TimeSpan interviewDuration, string audioPath, int callAttempt, string label = null)
        {
            try
            {
                CreateAudioFolderIfNeeded(audioPath);
                DateTime creationTime = DateTime.UtcNow - interviewDuration;
                var startingFilePath = GetFilePath(campaignId, interviewId, audioPath, label, callAttempt) + ".tmp";
                var musicFilePath = GetRandomMusicFilePath();
                var musicFileDuration = GetDuration(musicFilePath);

                var musicFilesCount = (int)Math.Ceiling(interviewDuration.TotalSeconds /
                                                        musicFileDuration.TotalSeconds);

                for (int i = 0; i < musicFilesCount; i++)
                {
                    Concatenate(startingFilePath + "2", new[] { musicFilePath });
                    File.Move(startingFilePath + "2", startingFilePath);
                }

                TrimWavFile(startingFilePath, startingFilePath.Replace(".tmp", ""), TimeSpan.Zero,
                    GetDuration(startingFilePath) - interviewDuration);

                File.SetCreationTimeUtc(startingFilePath.Replace(".tmp", ""), creationTime);

                File.Delete(startingFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error("InterviewRecordingFileCreator.CreateAudioFileWithNoTextToSpeech", ex.ToString());
            }
        }

        private void CreateAudioFolderIfNeeded(string audioPath)
        {
            if (!Directory.Exists(audioPath))
            {
                Directory.CreateDirectory(audioPath);
            }
        }

        private string GetRandomMusicFilePath()
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "AudioTemplates");

            var files = Directory.EnumerateFiles(directory, "*.wav").ToArray();

            return files[new Random().Next(0, files.Length)];
        }

        private string CreateStartingAudio(string campaignId, string campaignName, long interviewId, TimeSpan duration,
            string audioPath,
            string label, int callAttempt)
        {
            // Initialize a new instance of the SpeechSynthesizer.  
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                var file = GetFilePath(campaignId, interviewId, audioPath, label, callAttempt) + ".tmp";

                synth.SelectVoiceByHints((callAttempt % 2 != 0) ? VoiceGender.Male : VoiceGender.Female);

                // Configure the audio output.   
                synth.SetOutputToWaveFile(file,
                    new SpeechAudioFormatInfo(EncodingFormat.ALaw, 8000, 8, 1, 8000, 1, null));

                // Speak the prompt.  
                if (string.IsNullOrWhiteSpace(label))
                    synth.Speak($"This is the recording for interview number {interviewId}, from survey {campaignName} p{campaignId}.");
                else
                    synth.Speak($"This is the sectional recording {label}, for interview number {interviewId}, from survey {campaignName} p{campaignId}.");

                synth.Speak($"Interview has been started at {DateTime.UtcNow - duration:F} UTC.");
                synth.Speak($"Total recording duration is {(int)duration.TotalMinutes} minutes and {duration.Seconds} seconds.");

                return file;
            }
        }

        private static string GetFilePath(string campaignId, long interviewId, string audioPath, string label, int callAttempt)
        {
            var surveyFolder = new DirectoryInfo(audioPath).GetDirectories()
                .FirstOrDefault(x => x.Name == campaignId);

            if (surveyFolder == null)
            {
                surveyFolder = new DirectoryInfo(Path.Combine(audioPath, campaignId));
                surveyFolder.Create();
            }

            var guid = Guid.NewGuid().ToString().Replace("-", "").ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(label))
            {
                return Path.Combine(surveyFolder.FullName, $"{interviewId}_{callAttempt}_{guid}.wav");
            }
            else
            {
                label = RemoveIllegalChars(label);
                return Path.Combine(surveyFolder.FullName, $"{interviewId}_{callAttempt}_{label}_{guid}.wav");
            }
        }

        private static string RemoveIllegalChars(string label)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                label = label.Replace(c.ToString(), "");
            }

            return label;
        }

        private static void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)cutFromStart.TotalMilliseconds * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

                    int endBytes = (int)cutFromEnd.TotalMilliseconds * bytesPerMillisecond;
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    int endPos = (int)reader.Length - endBytes;

                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }

        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private static void Concatenate(string outputFile, IEnumerable<string> sourceFiles)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    using (WaveFileReader reader = new WaveFileReader(sourceFile))
                    {
                        if (waveFileWriter == null)
                        {
                            // first time in create new Writer
                            waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);
                        }
                        else
                        {
                            if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                            {
                                throw new InvalidOperationException($"Can't concatenate WAV Files that don't share the same format: {reader.WaveFormat} and {waveFileWriter.WaveFormat}");
                            }
                        }

                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.Write(buffer, 0, read);
                        }
                    }
                }
            }
            finally
            {
                waveFileWriter?.Dispose();
            }

        }

        private TimeSpan GetDuration(string filePath)
        {
            using (var audioFileReader = new WaveFileReader(filePath))
            {
                return audioFileReader.TotalTime;
            }
        }
    }
}