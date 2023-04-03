using Computer_Vision.Models;

namespace Computer_Vision.Service
{
    public interface IAnalyzeService
    {
        public AnalyzeModel getAnalyze(List<IFormFile> files);
    }
}
