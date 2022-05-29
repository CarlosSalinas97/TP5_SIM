using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5.Clases
{
    class Empleado
    {
        public static string LIBRE = "Libre";

        public static string OCUPADO = "Ocupado";

        private string estado;

        public Empleado()
        {
            this.estado = LIBRE;
        }

        public void setOcupado()
        {
            this.estado = OCUPADO;
        }

        public void setLibre()
        {
            this.estado = LIBRE;
        }

        public string getEstado()
        {
            return this.estado;
        }
    }
}
