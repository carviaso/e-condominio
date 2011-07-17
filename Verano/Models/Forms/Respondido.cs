using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Verano.Models.Forms
{
    public class Respondido
    {
        public int IdMorador { get; set; }
        public int IdPergunta { get; set; }
        public int IdResposta { get; set; }
        public int IdOpcao { get; set; }
        public string Comentario { get; set; }
    }
}