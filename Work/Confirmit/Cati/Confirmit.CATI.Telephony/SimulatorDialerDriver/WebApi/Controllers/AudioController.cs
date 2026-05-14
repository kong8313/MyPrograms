using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using SimulatorDialerDriverClass = Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver;

namespace SimulatorDialerDriver.WebApi.Controllers
{
    [RoutePrefix("audio")]
    public class AudioController : ApiController
    {
        private SimulatorScenario Scenario { get; }
        private const int ReadStreamBufferSize = 1024 * 1024;

        public AudioController()
        {
            var scenarioFullFileName = Settings.Default.ScenarioXmlFileName;

            if (Path.GetDirectoryName(scenarioFullFileName) == string.Empty)
            {
                scenarioFullFileName = Path.Combine(SimulatorDialerDriverClass.GetServiceAppDataPath(), scenarioFullFileName);
            }

            SimulatorDialerDriverClass.Instance.Logger.Info("AudioController.AudioController",
                "scenarioFullFileName=[{0}]",
                scenarioFullFileName);

            Scenario = new SimulatorScenarioDeserializer().Deserialize(scenarioFullFileName);
        }

        /// <summary>
        /// Return Audio file by folder name and audio file name
        /// </summary>
        /// <param name="audioFolderName">Name of folder containing audio.</param>
        /// <param name="audioFileName">Name of audio file.</param>
        /// <returns>Audio file in wav format</returns>
        [HttpGet]
        [Route("{audioFolderName}/{audioFileName}")]
        public HttpResponseMessage GetAudio(string audioFolderName, string audioFileName)
        {
            var folderWithAudio = string.IsNullOrEmpty(Scenario.AudioFolderName) ? "audio" : Scenario.AudioFolderName;
            var audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderWithAudio, audioFolderName, audioFileName);
            var fileInfo = new FileInfo(audioPath);

            if (!fileInfo.Exists)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var totalLength = fileInfo.Length;

            var rangeHeader = Request.Headers.Range;
            var response = new HttpResponseMessage();

            response.Headers.AcceptRanges.Add("bytes");
            // The request will be treated as normal request if there is no Range header.
            if (rangeHeader == null || !rangeHeader.Ranges.Any())
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
                    =>
                {
                    using (outputStream) // Copy the file to output stream straightforward. 
                    using (Stream inputStream = fileInfo.OpenRead())
                    {
                        try
                        {
                            inputStream.CopyTo(outputStream, ReadStreamBufferSize);
                        }
                        catch (Exception ex)
                        {
                            SimulatorDialerDriverClass.Instance.Logger.Error(
                                "AudioController.GetAudio",
                                ex.ToString());
                        }
                    }
                }, new MediaTypeHeaderValue("audio/x-wav"));

                response.Content.Headers.ContentLength = totalLength;
                return response;
            }
            // 1. If the unit is not 'bytes'.
            // 2. If there are multiple ranges in header value.
            // 3. If start or end position is greater than file length.
            if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 ||
                !TryReadRangeItem(rangeHeader.Ranges.First(), totalLength, out var start, out var end))
            {
                response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Content = new StreamContent(Stream.Null);  // No content for this status.
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/x-wav");

                return response;
            }

            var contentRange = new ContentRangeHeaderValue(start, end, totalLength);

            // We are now ready to produce partial content.
            response.StatusCode = HttpStatusCode.PartialContent;
            response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
                =>
            {
                using (outputStream) // Copy the file to output stream in indicated range.
                using (Stream inputStream = fileInfo.OpenRead())
                    CreatePartialContent(inputStream, outputStream, start, end);

            }, new MediaTypeHeaderValue("audio/x-wav"));

            response.Content.Headers.ContentLength = end - start + 1;
            response.Content.Headers.ContentRange = contentRange;

            return response;
        }

        private void CreatePartialContent(Stream inputStream, Stream outputStream,
            long start, long end)
        {
            long remainingBytes = end - start + 1;
            long position;
            byte[] buffer = new byte[ReadStreamBufferSize];

            inputStream.Position = start;
            do
            {
                try
                {
                    var count = remainingBytes > ReadStreamBufferSize ? inputStream.Read(buffer, 0, ReadStreamBufferSize) : inputStream.Read(buffer, 0, (int)remainingBytes);
                    outputStream.Write(buffer, 0, count);
                }
                catch (Exception ex)
                {
                    SimulatorDialerDriverClass.Instance.Logger.Error(
                        "AudioController.CreatePartialContent",
                        ex.ToString());
                    break;
                }
                position = inputStream.Position;
                remainingBytes = end - position + 1;
            } while (position <= end);
        }

        private static bool TryReadRangeItem(RangeItemHeaderValue range, long contentLength,
            out long start, out long end)
        {
            if (range.From != null)
            {
                start = range.From.Value;
                if (range.To != null)
                    end = range.To.Value;
                else
                    end = contentLength - 1;
            }
            else
            {
                end = contentLength - 1;
                if (range.To != null)
                    start = contentLength - range.To.Value;
                else
                    start = 0;
            }
            return (start < contentLength && end < contentLength);
        }
    }
}