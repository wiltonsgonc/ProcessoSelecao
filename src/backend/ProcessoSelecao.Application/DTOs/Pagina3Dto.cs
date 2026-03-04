using Microsoft.AspNetCore.Http;
using System;

namespace ProcessoSelecao.Application.DTOs
{
    public class Pagina3Dto
    {
        public IFormFile RgCpfCandidato { get; set; }
        public IFormFile AnexoI { get; set; }
        public IFormFile CurriculoLattesCandidato { get; set; }
        public IFormFile CurriculoLattesOrientador { get; set; }
        public IFormFile AnexoII { get; set; }
        public IFormFile ComprovanteMatricula { get; set; }
        public IFormFile HistoricoEscolar { get; set; }
    }
}