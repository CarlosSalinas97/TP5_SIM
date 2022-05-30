﻿using System;
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

        private string nombre;
        private string estado;
        private string fin_uso_instalacion;
        private string hs_llegada;
        private string accion;
        private string posicion_cola;

        public Cliente(string nombre, double hs_llegada, string accion)
        {
            this.nombre = nombre;
            this.hs_llegada = hs_llegada.ToString();
            this.accion = accion;
        }

        // Nombre
        public string getNombre()
        {
            return nombre;
        }

        public void setNombre(string value)
        {
            nombre = value;
        }

        // Estado
        public string getEstado()
        {
            return estado;
        }

        public void setEstado(string value)
        {
            estado = value;
        }

        // Fin uso instalacion
        public string getFin_uso_instalacion()
        {
            return fin_uso_instalacion;
        }

        public void setFin_uso_instalacion(string value)
        {
            fin_uso_instalacion = value;
        }

        // Hs llegada
        public void setHs_llegada(string value)
        {
            hs_llegada = value;
        }

        public string getHs_llegada()
        {
            return hs_llegada;
        }

        // Accion
        public void setAccion(string value)
        {
            accion = value;
        }

        public string getAccion()
        {
            return accion;
        }

        // Posicion cola
        public string getPosicion_cola()
        {
            return posicion_cola;
        }

        public void setPosicion_cola(int value)
        {
            posicion_cola = value.ToString();
        }

        // Equals
        public override bool Equals(object obj)
        {
            var cliente = obj as Cliente;
            return cliente != null &&
                   nombre == cliente.nombre &&
                   estado == cliente.estado;
        }

        // Hashcode
        public override int GetHashCode()
        {
            var hashCode = -1442448609;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(nombre);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(estado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fin_uso_instalacion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(hs_llegada);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(accion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(posicion_cola);
            return hashCode;
        }
    }
}