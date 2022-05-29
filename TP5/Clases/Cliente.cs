using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5.Clases
{
    class Cliente
    {
        public static string EN_ESPERA = "En espera";

        public static string SIENDO_ATENTIDO = "Siendo atendido";

        public static string LEYENDO = "Leyendo";

        private string estado { get; set; }

        public Cliente(string estado)
        {
            this.estado = estado;
        }
    }
}
