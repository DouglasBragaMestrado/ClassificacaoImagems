
using Computer_Vision.Models;
using Computer_Vision.Service;
using Microsoft.AspNetCore.Mvc;

namespace Computer_Vision.Controllers
{
    public class AnaliseImagemController : Controller
    {
        private readonly IAnalyzeService _analyzeService;
        public AnaliseImagemController(IAnalyzeService analyzeService)
        {
            _analyzeService = analyzeService;
        }
        AnalyzeModel analyze = new AnalyzeModel();

        [Route("analise")]
        public IActionResult Index(List<IFormFile> files, bool checkAnalyze)
        {
            if (checkAnalyze)
            {
                analyze = _analyzeService.getAnalyze(files);

                ViewBag.Imagem = analyze.imagem;
                ViewBag.resultado = analyze.resposta;
            }
            return View();
        }
    }
}
