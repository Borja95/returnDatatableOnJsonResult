using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TutorialSubirArchivos.Models
{

    public class Respuesta_Json
    {
        public int Codigo { get; set; }
        public string Mensaje_Respuesta { get; set; }
        public List<Dictionary<string, object>> Archivos { get; set; }
    }
}