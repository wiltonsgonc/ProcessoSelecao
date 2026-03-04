using Microsoft.AspNetCore.Mvc;
using ProcessoSelecao.Application.DTOs;
using System;

namespace ProcessoSelecao.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormularioController : ControllerBase
    {
        [HttpPost("pagina1")]
        public IActionResult PostPagina1([FromBody] Pagina1Dto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Processar e armazenar os dados
            return Ok(new { message = "Dados da Página 1 recebidos com sucesso!" });
        }

        [HttpPost("pagina2")]
        public IActionResult PostPagina2([FromBody] Pagina2Dto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Processar e armazenar os dados
            return Ok(new { message = "Dados da Página 2 recebidos com sucesso!" });
        }

        [HttpPost("pagina3")]
        public IActionResult PostPagina3([FromForm] Pagina3Dto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Processar e armazenar os dados
            return Ok(new { message = "Dados da Página 3 recebidos com sucesso!" });
        }

        [HttpPost("pagina4")]
        public IActionResult PostPagina4([FromBody] Pagina4Dto dados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Processar e armazenar os dados
            return Ok(new { message = "Dados da Página 4 recebidos com sucesso!" });
        }

        [HttpPost("completa")]
        public IActionResult PostInscricaoCompleta([FromBody] InscricaoCompletaDto dados)
        {
            if (dados == null)
            {
                return BadRequest(new { message = "Dados da inscrição não fornecidos" });
            }

            try
            {
                // Aqui você implementaria a lógica para salvar a inscrição completa
                // Por exemplo: salvar no banco de dados, enviar e-mails, etc.

                return Ok(new { 
                    message = "Inscrição realizada com sucesso!",
                    inscricaoId = Guid.NewGuid()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao processar inscrição", erro = ex.Message });
            }
        }
    }
}