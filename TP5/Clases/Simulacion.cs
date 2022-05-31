using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5.Clases
{
    class Simulacion
    {
        private DataTable dataTable;
        private Random random = new Random();
        private Empleado empleado1 = new Empleado("E1");
        private Empleado empleado2 = new Empleado("E2");
        private Form1 formulario;
        private int maximo_simulacion = 100000;
        private int cont_personas_llegada = 0;
        private string evento = "";
        private double reloj = 0;
        private double proxima_llegada = 0;
        private double rnd_tipo_llegada = 0;
        private string tipo_llegada = "";
        private double rnd_tiempo_atencion = 0;
        private double tiempo_atencion = 0;
        private int cola = 0;
        private int contador_atencion = 0;
        private double tiempo_permanencia = 0;
        private string estado = "";
        private double rnd_permanencia = 0;
        private List<Cliente> clientes_llegaron_biblioteca = new List<Cliente>();
        private List<Cliente> clientes_permanencen_biblioteca = new List<Cliente>();
        private List<Cliente> clientes_en_cola = new List<Cliente>();
        private int puntero_lista_llegaron = 0;
        private double prox_fin_uso_instalacion = 0;

        public Simulacion(Form1 formulario)
        {
            this.formulario = formulario;
        }

        // Genera columnas del DataTable
        private void generar_dt()
        {
            dataTable = new DataTable();

            dataTable.Columns.Add("i");
            dataTable.Columns.Add("Evento");
            dataTable.Columns.Add("Reloj");
            dataTable.Columns.Add("Proxima llegada");
            dataTable.Columns.Add("RND Tipo llegada");
            dataTable.Columns.Add("Tipo");
            dataTable.Columns.Add("RND Tiempo atencion");
            dataTable.Columns.Add("Tiempo atencion");
            dataTable.Columns.Add("Fin atencion(1)");
            dataTable.Columns.Add("Fin atencion(2)");
            dataTable.Columns.Add("RND Permanencia");
            dataTable.Columns.Add("Estado (E1)");
            dataTable.Columns.Add("Estado (E2)");
            dataTable.Columns.Add("Cola");
            dataTable.Columns.Add("Contador atencion");
            dataTable.Columns.Add("Tiempo permanencia");
        }

        public DataTable generar_simulacion()
        {
            generar_dt();
            proxima_llegada = formulario.llegada_personas;

            for (int i = 0; i < maximo_simulacion; i++)
            {
                if (i != 0 && reloj % formulario.llegada_personas == 0)
                {
                    proxima_llegada = calcular_proxima_llegada(proxima_llegada);
                }

                if (evento == "Llegada")
                {
                    simular_llegada(null);
                }
                else if (evento == "Fin uso instalación")
                {
                    simular_fin_uso_instalacion();

                } else
                {
                    simular_fin_atencion();
                }

                dataTable.Rows.Add(i, evento, reloj, proxima_llegada, rnd_tipo_llegada, tipo_llegada, rnd_tiempo_atencion, tiempo_atencion, empleado1.getFinAtencion(), empleado2.getFinAtencion(), rnd_permanencia, empleado1.getEstado(), empleado2.getEstado(), cola, contador_atencion, tiempo_permanencia);

                prox_fin_uso_instalacion = proximo_fin_uso_instalacion();
                reloj = salto_reloj(proxima_llegada, empleado1.getFinAtencion(), empleado2.getFinAtencion(), prox_fin_uso_instalacion);
                evento = calcular_evento(reloj, proxima_llegada, empleado1.getFinAtencion(), empleado2.getFinAtencion(), prox_fin_uso_instalacion);
                rnd_tipo_llegada = 0;
                tipo_llegada = "";
                rnd_tiempo_atencion = 0;
                tiempo_atencion = 0;
                rnd_permanencia = 0;

                if (reloj > formulario.reloj_max)
                {
                    i = maximo_simulacion;
                }

                
            }

            agregar_columnas_persona(clientes_permanencen_biblioteca);
            return dataTable;
        }

        private void simular_llegada(Cliente cliente)
        {
            if (cliente == null)
            {
                // Incrementa la cantidad de personas que llegaron
                cont_personas_llegada++;

                // Establece a que vino el cliente
                rnd_tipo_llegada = redondear(random.NextDouble());
                tipo_llegada = calcular_tipo_llegada(rnd_tipo_llegada);

                // Agrega una instancia de cliente dentro de la lista de clientes en la biblioteca.
                clientes_llegaron_biblioteca.Add(new Cliente(puntero_lista_llegaron, cont_personas_llegada, reloj, tipo_llegada));
            } else
            {
                puntero_lista_llegaron = cliente.getIndiceCola();
            }

            if (empleado1.getEstado().Equals(Empleado.LIBRE))
            {
                atencion_empleado(empleado1, puntero_lista_llegaron);
            }

            else if (empleado2.getEstado().Equals(Empleado.LIBRE))
            {
                atencion_empleado(empleado2, puntero_lista_llegaron);
            }

            else
            {
                cola++;
                estado = "En espera";
                clientes_llegaron_biblioteca[puntero_lista_llegaron].setEstado(estado);
                clientes_en_cola.Add(clientes_llegaron_biblioteca[puntero_lista_llegaron]);
            }

            // Se agrega el cliente a la lista de clientes que permanencen en la biblioteca
            clientes_permanencen_biblioteca.Add(clientes_llegaron_biblioteca[puntero_lista_llegaron]);

            // Por cada cliente que llega, el puntero se incrementa para apuntar al siguiente en la lista.
            puntero_lista_llegaron++;
        }

        private void simular_fin_atencion()
        {
            if (empleado1.getEstado().Equals(Empleado.OCUPADO) && evento.Equals("Fin atencion E1"))
            {
                liberar_empleado(empleado1);
                if (cola > 0)
                {
                    atender_cliente_cola(empleado1);
                }
            }
            if (empleado2.getEstado().Equals(Empleado.OCUPADO) && evento.Equals("Fin atencion E2"))
            {
                liberar_empleado(empleado2);
                if (cola > 0)
                {
                    atender_cliente_cola(empleado2);
                }
            }
        }

        private void atencion_empleado(Empleado empleado, int index_cliente)
        {
            // Se setea ocupado
            empleado.setOcupado();

            // Asignar cliente a empleado
            empleado.setClienteAtendiendo(index_cliente);

            // Establecer estado a cliente
            estado = "SA " + empleado.getNombre();
            clientes_llegaron_biblioteca[index_cliente].setEstado(estado);

            // Calcular tiempo de atencion
            rnd_tiempo_atencion = redondear(random.NextDouble());
            tipo_llegada = clientes_llegaron_biblioteca[index_cliente].getAccion();
            tiempo_atencion = calcular_tiempo_atencion(tipo_llegada, rnd_tiempo_atencion);

            // Setear fin de atencion a empleado
            empleado.setFinAtencion(calcular_fin_atencion(reloj, tiempo_atencion));

            // Acumular tiempo de permanencia
            tiempo_permanencia += tiempo_atencion;
        }

        private void liberar_empleado(Empleado empleado)
        {
            // Obtiene el cliente atendido
            int index_cliente_atendido = empleado.getClienteAtendiendo();
            Cliente cliente_atendido = clientes_llegaron_biblioteca[index_cliente_atendido];

            // Establece si el cliente usará la instalacion (en el caso de pedir libro)
            calcular_cliente_usa_instalacion(cliente_atendido, index_cliente_atendido);

            // Incrementar acumulador de atencion
            contador_atencion++;

            // Se libera el empleado y se limpia el atributo fin atencion
            empleado.setLibre();
            empleado.setFinAtencion(0);
        }

        private void simular_fin_uso_instalacion()
        {
            foreach(Cliente cliente in clientes_permanencen_biblioteca)
            {
                if (cliente.getFin_uso_instalacion() != null && double.Parse(cliente.getFin_uso_instalacion()) == reloj)
                {
                    
                    clientes_llegaron_biblioteca.Remove(cliente);
                    int puntero_lista = clientes_llegaron_biblioteca.Count - 1;
                    clientes_permanencen_biblioteca.Remove(cliente);
                    cliente.setIndiceCola(puntero_lista);
                    cliente.setFin_uso_instalacion(0.ToString());
                    cliente.setAccion("Devolucion");
                    clientes_llegaron_biblioteca.Add(cliente);
                    simular_llegada(cliente);
                    break;
                }
            }

        }

        private void atender_cliente_cola(Empleado empleado)
        {
            // Obtiene el cliente primero en la cola
            Cliente cliente_decola = clientes_en_cola[0];

            // Obtiene el index del cliente 
            int index_cliente = clientes_llegaron_biblioteca.IndexOf(cliente_decola);

            // Lo tiene que atender el empleado
            atencion_empleado(empleado, index_cliente);

            // Se reduce el tamaño de la cola, y se elimina el cliente que esta siendo atendido
            cola--;
            clientes_en_cola.Remove(cliente_decola);
        }

        private void calcular_cliente_usa_instalacion(Cliente cliente, int index_cliente)
        {
            if (cliente.getAccion().Equals("Pedido") && !cliente.getPidioLibro()) 
            {
                // Significa que el cliente hizo un pedido y se realiza el random para determinar si se queda.
                rnd_permanencia = redondear(random.NextDouble());
                if (calcular_probabilidad_permanencia(rnd_permanencia))
                {
                    clientes_llegaron_biblioteca[index_cliente].setEstado(Cliente.LEYENDO);
                    clientes_llegaron_biblioteca[index_cliente].setFin_uso_instalacion((reloj + formulario.tiempo_uso_instalacion).ToString());
                    cliente.setPidioLibro(true);
                    tiempo_permanencia += formulario.tiempo_uso_instalacion;
                }
                else
                {
                    // El cliente no se queda a leer. Se elimina de la lista de clientes que permanencen en biblioteca
                    clientes_permanencen_biblioteca.Remove(cliente);
                }
            }
            else
            {
                // El cliente fue a devolver o consultar. Se elimina de la lista de clientes que permanencen en biblioteca
                clientes_permanencen_biblioteca.Remove(cliente);
            }
        }
 
        private void agregar_columnas_persona(List<Cliente> clientes)
        {
            foreach (Cliente cliente in clientes)
            {
                string persona = cliente.getNombre();
                DataColumn column_estado = new DataColumn("Estado (" + persona + ")");
                column_estado.DefaultValue = cliente.getEstado();
                dataTable.Columns.Add(column_estado);

                DataColumn column_hs_llegada = new DataColumn("Hs llegada (" + persona + ")");
                column_hs_llegada.DefaultValue = cliente.getHs_llegada();
                dataTable.Columns.Add(column_hs_llegada);

                DataColumn column_fin_uso_inst = new DataColumn("Fin uso instalacion (" + persona + ")");
                column_fin_uso_inst.DefaultValue = cliente.getFin_uso_instalacion();
                dataTable.Columns.Add(column_fin_uso_inst);

                DataColumn column_accion = new DataColumn("Accion (" + persona + ")");
                column_accion.DefaultValue = cliente.getAccion();
                dataTable.Columns.Add(column_accion);
            }
        }

        private double proximo_fin_uso_instalacion()
        {
            double min = 99999;
            foreach(Cliente cliente in clientes_permanencen_biblioteca) {
                if (cliente.getFin_uso_instalacion() != null && double.Parse(cliente.getFin_uso_instalacion()) < min)
                {
                    min = double.Parse(cliente.getFin_uso_instalacion());
                }
            }
            return min;
        }

        private double salto_reloj(double proxima_llegada, double fin_atencion1, double fin_atencion2, double fin_uso_instalacion)
        {
            // Calcula cual será el proximo salto que hará el reloj. Siempre debe saltar al evento más proximo.
            fin_atencion1 = fin_atencion1 == 0 ? 999999 : fin_atencion1;
            fin_atencion2 = fin_atencion2 == 0 ? 999999 : fin_atencion2;
            fin_uso_instalacion = fin_uso_instalacion == 0 ? 999999 : fin_uso_instalacion;
            return Math.Min(proxima_llegada, Math.Min(fin_uso_instalacion, Math.Min(fin_atencion1, fin_atencion2)));
        }

        private string calcular_evento(double proximo_reloj, double proxima_llegada, double fin_atencion1, double fin_atencion2, double fin_uso_instalacion)
        {
            string proximo_evento = "";
            if (proximo_reloj == proxima_llegada)
            {
                proximo_evento = "Llegada";
            }
            if (proximo_reloj == fin_atencion1)
            {
                proximo_evento = "Fin atencion E1";
            }
            if (proximo_reloj == fin_atencion2)
            {
                proximo_evento = "Fin atencion E2";
            }
            if (proximo_reloj == fin_uso_instalacion)
            {
                proximo_evento = "Fin uso instalación";
            }
            return proximo_evento;
        }

        private double calcular_proxima_llegada(double proxima_llegada)
        {
            return proxima_llegada + formulario.llegada_personas;
        }

        private string calcular_tipo_llegada(double random_tipo_llegada)
        {
            // Evento	    P()	    P(ac)
            // Pedido       0,45    0,45
            // Devolocion   0,45    0,9
            // Consulta     0,1     1
            random_tipo_llegada = random_tipo_llegada == 0 ? 0.01 : random_tipo_llegada;
            if (random_tipo_llegada >= 0 && random_tipo_llegada <= formulario.prob_pedido)
            {
                return "Pedido";
            }
            else if (random_tipo_llegada >= formulario.prob_pedido + 0.01 && random_tipo_llegada <= formulario.prob_pedido + formulario.prob_devolucion)
            {
                return "Devolucion";
            }
            else
            {
                return "Consulta";
            }
        }

        private double calcular_tiempo_atencion(string tipo_atencion, double random)
        {
            // Depende del tipo de atencion
            // Pedido -> EXP-(6)  | Siempre es 6        | LN(1-RND)/(-LAMBDA)
            // Consulta -> U(2;5) | a y b pueden variar | (b-a)*RND+a
            // Devolucion -> U(1.5;2.5) | a y b fijos   | (b-a)*RND+a
            double tiempo_atencion_calculado = 0;
            switch (tipo_atencion)
            {
                case "Pedido":
                    tiempo_atencion_calculado = atencion_pedido(random);
                    break;
                case "Devolucion":
                    tiempo_atencion_calculado = atencion_devolucion(random);
                    break;
                case "Consulta":
                    tiempo_atencion_calculado = atencion_consulta(random);
                    break;
            }

            return redondear(tiempo_atencion_calculado);
        }

        // lo llama calcular_tiempo_atencion()
        private double atencion_pedido(double random)
        {
            // Pedido -> EXP-(6)  | Siempre es 6 | LN(1-RND)/(-LAMBDA)
            double lambda = 0.1666666667;
            return Math.Log(1 - random) / -lambda;
        }

        // lo llama calcular_tiempo_atencion()
        private double atencion_devolucion(double random)
        {
            // Devolucion -> U(1.5;2.5) | a y b fijos   | (b-a)*RND+a
            return (2.5 - 1.5) * random + 1.5;
        }

        // lo llama calcular_tiempo_atencion()
        private double atencion_consulta(double random)
        {
            return (formulario.consulta_max - formulario.consulta_min) * random + formulario.consulta_min;
        }


        private double calcular_fin_atencion(double reloj, double tiempo_atencion)
        {
            // Calcula cuanto tiempo dura la atencion: reloj + tiempo_atencion
            return reloj + tiempo_atencion;
        }

        private bool calcular_probabilidad_permanencia(double random)
        {
            if (random >= 0 && random <= formulario.porc_retiran_biblo)
            {
                return false;
            };
            return true;
        }

        private double redondear(double value)
        {
            return Math.Truncate(value * 100) / 100;
        }

    }
}
