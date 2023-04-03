using Azure.AI.Vision.Core.Input;
using Azure.AI.Vision.Core.Options;
using Azure.AI.Vision.ImageAnalysis;
using Computer_Vision.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Text;

namespace Computer_Vision.Service
{
    public class AnalyzeService : IAnalyzeService
    {
        private readonly IConfiguration _configuration;
        private readonly IServer _server;
        private StringBuilder Resposta = new StringBuilder(250);
        //string filePath = Path.GetTempPath();
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\");
        string fileName;

        public AnalyzeService(IConfiguration configuration, IServer server)
        {
            _configuration = configuration;
            _server = server;
        }

        public AnalyzeModel getAnalyze(List<IFormFile> files)
        {
            AnalyzeModel analyze = new AnalyzeModel();
            long size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    FileInfo fileInfo = new FileInfo(formFile.FileName);
                    fileName = formFile.FileName;
                    string fileNameWithPath = Path.Combine(filePath, fileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }

            var serviceOptions = new VisionServiceOptions(
            _configuration["VISION_ENDPOINT"].ToString(),
            _configuration["VISION_KEY"].ToString());

            string file = filePath + fileName;
            var imageSource = VisionSource.FromFile(file);

            var analysisOptions = new ImageAnalysisOptions()
            {
                Features = ImageAnalysisFeature.Caption | ImageAnalysisFeature.Text,

                Language = "en",

                GenderNeutralCaption = true
            };

            using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);

            var result = analyzer.Analyze();

            if (result.Reason == ImageAnalysisResultReason.Analyzed)
            {
                if (result.Caption != null)
                {
                    Resposta.Append(" Analisei e tenho ");
                    Resposta.Append($" {result.Caption.Confidence:0.0000} Pontos de confiança que sua imagem é ");
                    Resposta.Append($" {result.Caption.Content}");
                }

            }
            else if (result.Reason == ImageAnalysisResultReason.Error)
            {
                var errorDetails = ImageAnalysisErrorDetails.FromResult(result);
                Resposta.Append(" Analysis failed.");
                Resposta.Append($"   Error reason : {errorDetails.Reason}");
                Resposta.Append($"   Error code : {errorDetails.ErrorCode}");
                Resposta.Append($"   Error message: {errorDetails.Message}");
            }

            var local = _server.Features.Get<IServerAddressesFeature>().Addresses;
            foreach (var item in local)
            {
                analyze.imagem = item + "/img/" + fileName;
                break;
            }

            analyze.resposta = Resposta.ToString();
            return analyze;
        }
    }
}
