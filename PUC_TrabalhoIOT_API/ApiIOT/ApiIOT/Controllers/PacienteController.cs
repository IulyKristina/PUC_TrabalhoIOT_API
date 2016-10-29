using ApiIOT.Models;
using ApiIOT.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiIOT.Controllers
{
    public class PacienteController : ApiController
    {
        private PacienteRepository pacienteRepository;

        public PacienteController()
        {
            if (pacienteRepository == null)
                pacienteRepository = new PacienteRepository();
        }

        // GET api/paciente
        [HttpGet]
        [Route("api/paciente/")]
        public async Task<Paciente[]> GetAll()
        {
            try
            {
                return await pacienteRepository.BuscarPacienteAsync();
            }
            catch(Exception)
            {
                throw;
            }
        }

        // GET api/paciente/5
        [HttpGet]
        [Route("api/paciente/{id}")]
        public async Task<Paciente> Get(string id)
        {
            return await pacienteRepository.BuscarPacienteAsync(id);
        }

        // POST api/paciente
        [HttpPost]
        [Route("api/paciente")]
        public async Task<string> Post([FromBody]Paciente paciente)
        {
            return await pacienteRepository.CadastrarAsync(paciente);
        }

        // PUT api/paciente/5
        [HttpPut]
        [Route("api/paciente/{id}")]
        public async Task<JObject> Put(string id, [FromBody]Paciente value)
        {
            string resposta = (false).ToString();

            try
            {
                await pacienteRepository.AlterarAsync(id, value);
                resposta = (true).ToString();
            }
            catch(Exception)
            {
                resposta = (false).ToString();
            }

            JObject retorno = new JObject();
            retorno.Add("resposta", resposta);

            return retorno;
        }
    }
}
