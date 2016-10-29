using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace ApiIOT.Models
{
    public class Paciente
    {
        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Foto { get; set; }
        public int Idade { get; set; }
        public float Altura {get; set; }
        public float Peso { get; set; }
        public string TelefoneFixo { get; set; }
        public string TelefoneCelular { get; set; }
        public string Endereco { get; set; }
        public string TipoSanguineo { get; set; }
        public string PodeDoar { get; set; }
        public string PodeReceber { get; set; }
        public Alergia[] Alergias { get; set; }
        public Vacina[] Vacinas { get; set; }
        public Doenca[] Doencas { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}