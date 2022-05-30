using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TP5.Clases;

namespace TP5
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        private Random random = new Random();
        private Empleado empleado1;
        private Empleado empleado2;
        // Variables
        private int reloj_max; // X
        private int cantidad_iteracciones; // i iteracciones desde la hora j
        private int hora_desde; // j
        private double llegada_personas;
        // Probabilidad de accion
        private int consulta_min;
        private int contulta_max;
        private int porc_retiran_biblo;
        private int tiempo_uso_instalacion;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void cargar_dt()
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

        private void cargar_dgv(int X)
        {
            string persona = "";
            int cont_personas_llegada = 0;
            int cont_personas_atendida = 0;
            string evento = "";
            double reloj = 0;
            double proxima_llegada = 4;
            double rnd_tipo_llegada = 0;
            string tipo_llegada = "";
            double rnd_tiempo_atencion = 0;
            double tiempo_atencion = 0;
            double fin_atencion1 = 0;
            double fin_atencion2 = 0;
            int cola = 0;
            int contador_atencion = 0;
            double tiempo_permanencia = 0;
            string estado = "";
            double fin_uso_instalacion = 0;
            int posicion_cola = 0;
            List<Cliente> clientes = new List<Cliente>();


            for (int i = 0; i < X; i++)
            {
                if (i != 0 && reloj % 4 == 0) {
                    proxima_llegada = calcular_proxima_llegada(proxima_llegada);
                }
                
                // Evento es llegada
                if (evento == "Llegada")
                {
                    cont_personas_llegada++;
                    persona = "P" + cont_personas_llegada;
                    rnd_tipo_llegada = redondear(random.NextDouble());
                    tipo_llegada = calcular_tipo_llegada(rnd_tipo_llegada);
                    clientes.Add(new Cliente(persona, reloj, tipo_llegada));
                    int index_cliente = clientes.Count - 1;
                    if (empleado1.getEstado().Equals(Empleado.LIBRE))
                    {
                        empleado1.setOcupado();
                        rnd_tiempo_atencion = redondear(random.NextDouble());
                        tiempo_atencion = calcular_tiempo_atencion(tipo_llegada, rnd_tiempo_atencion);
                        fin_atencion1 = calcular_fin_atencion(reloj, tiempo_atencion);
                        tiempo_permanencia += tiempo_atencion;
                        estado = "SA E1";
                        clientes[index_cliente].setEstado(estado);
                    }
                    else if (empleado2.getEstado().Equals(Empleado.LIBRE))
                    {
                        empleado2.setOcupado();
                        rnd_tiempo_atencion = redondear(random.NextDouble());
                        tiempo_atencion = calcular_tiempo_atencion(tipo_llegada, rnd_tiempo_atencion);
                        fin_atencion2 = calcular_fin_atencion(reloj, tiempo_atencion);
                        tiempo_permanencia += tiempo_atencion;
                        estado = "SA E2";
                        clientes[index_cliente].setEstado(estado);
                    }
                    else
                    {
                        cola++;
                        estado = "En espera";
                        posicion_cola = cola;
                        clientes[index_cliente].setPosicion_cola(posicion_cola);
                    }
                    agregar_columnas_persona(persona, estado, reloj, fin_uso_instalacion, tipo_llegada, posicion_cola);
                    // Fin evento llegada
                }
                else
                {
                    // Evento liberar empleado
                    if (empleado1.getEstado().Equals(Empleado.OCUPADO) && evento.Equals("Fin atencion E1"))
                    {
                        contador_atencion++;
                        fin_atencion1 = 0;
                        empleado1.setLibre();
                        cont_personas_atendida++;
                        persona = "P" + cont_personas_atendida;
                        eliminar_columnas_persona(persona);
                        if (cola > 0)
                        {
                            Cliente cliente_decola = clientes[0];
                            cola--;
                            empleado1.setOcupado();
                            rnd_tiempo_atencion = redondear(random.NextDouble());
                            tiempo_atencion = calcular_tiempo_atencion(cliente_decola.getAccion(), rnd_tiempo_atencion);
                            fin_atencion1 = calcular_fin_atencion(reloj, tiempo_atencion);
                            tiempo_permanencia += tiempo_atencion;
                            estado = "SA E1";
                        }
                    }
                    if (empleado2.getEstado().Equals(Empleado.OCUPADO) && evento.Equals("Fin atencion E2"))
                    {
                        contador_atencion++;
                        fin_atencion2 = 0;
                        empleado2.setLibre();
                        cont_personas_atendida++;
                        persona = "P" + cont_personas_atendida;
                        eliminar_columnas_persona(persona);
                        if (cola > 0)
                        {
                            Cliente cliente_decola = clientes[0];
                            cola--;
                            empleado2.setOcupado();
                            rnd_tiempo_atencion = redondear(random.NextDouble());
                            tiempo_atencion = calcular_tiempo_atencion(cliente_decola.getAccion(), rnd_tiempo_atencion);
                            fin_atencion2 = calcular_fin_atencion(reloj, tiempo_atencion);
                            tiempo_permanencia += tiempo_atencion;
                            estado = "SA E2";
                        }
                    }
                }

                dataTable.Rows.Add(i, evento, reloj, proxima_llegada, rnd_tipo_llegada, tipo_llegada, rnd_tiempo_atencion, tiempo_atencion, fin_atencion1, fin_atencion2, 0, empleado1.getEstado(), empleado2.getEstado(), cola, contador_atencion, tiempo_permanencia);

                reloj = salto_reloj(proxima_llegada, fin_atencion1, fin_atencion2);
                evento = calcular_evento(reloj, proxima_llegada, fin_atencion1, fin_atencion2);
                rnd_tipo_llegada = 0;
                tipo_llegada = "";
                rnd_tiempo_atencion = 0;
                tiempo_atencion = 0;
            }

            dgv_simulacion.DataSource = dataTable;
        }

        private void btn_simular_Click(object sender, EventArgs e)
        {
            empleado1 = new Empleado();
            empleado2 = new Empleado();
            int X = int.Parse(txt_X.Text);
            cargar_dt();
            cargar_dgv(X);
        }

        private void agregar_columnas_persona(string persona, string estado, double hora_llegada, double fin_uso_instalacion, string accion, int posicion_cola)
        {
            DataColumn column_estado = new DataColumn("Estado (" + persona + ")");
            column_estado.DefaultValue = estado;
            dataTable.Columns.Add(column_estado);

            DataColumn column_hs_llegada = new DataColumn("Hs llegada (" + persona + ")");
            column_hs_llegada.DefaultValue = hora_llegada;
            dataTable.Columns.Add(column_hs_llegada);

            DataColumn column_fin_uso_inst = new DataColumn("Fin uso instalacion (" + persona + ")");
            column_fin_uso_inst.DefaultValue = fin_uso_instalacion;
            dataTable.Columns.Add(column_fin_uso_inst);

            DataColumn column_accion = new DataColumn("Accion (" + persona + ")");
            column_accion.DefaultValue = accion;
            dataTable.Columns.Add(column_accion);

            DataColumn column_pos_cola = new DataColumn("Posicion en cola (" + persona + ")");
            column_pos_cola.DefaultValue = posicion_cola;
            dataTable.Columns.Add(column_pos_cola);
        }

        private void eliminar_columnas_persona(string persona)
        {
            int indice_columna = dataTable.Columns.IndexOf("Estado (" + persona + ")");
            if (dataTable.Columns.Count > 16) {
                for (int i = 0; i < 5; i++)
                {
                    dataTable.Columns.RemoveAt(indice_columna);
                }
            }
        }

        private double salto_reloj(double proxima_llegada, double fin_atencion1, double fin_atencion2)
        {
            // Calcula cual será el proximo salto que hará el reloj. Siempre debe saltar al evento más proximo.
            fin_atencion1 = fin_atencion1 == 0 ? 999999 : fin_atencion1;
            fin_atencion2 = fin_atencion2 == 0 ? 999999 : fin_atencion2;
            return Math.Min(proxima_llegada, Math.Min(fin_atencion1, fin_atencion2));
        }

        private string calcular_evento(double proximo_reloj, double proxima_llegada, double fin_atencion1, double fin_atencion2)
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
            return proximo_evento;
        }

        private double calcular_proxima_llegada(double proxima_llegada)
        {
            // Llegan cada 4 minutos. Se puede modificar
            return proxima_llegada + 4;
        }

        private string calcular_tipo_llegada(double random_tipo_llegada)
        {
            // Evento	    P()	    P(ac)
            // Pedido       0,45    0,45
            // Devolocion   0,45    0,9
            // Consulta     0,1     1

            if (random_tipo_llegada >= 0 && random_tipo_llegada <= 0.44)
            {
                return "Pedido";
            } else if (random_tipo_llegada >= 0.45 && random_tipo_llegada <= 0.89)
            {
                return "Devolucion";
            } else
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
            return Math.Log(1-random) / -lambda;
        }

        // lo llama calcular_tiempo_atencion()
        private double atencion_devolucion(double random)
        {
            // Devolucion -> U(1.5;2.5) | a y b fijos   | (b-a)*RND+a
            return (2.5 - 1.5)*random + 1.5;
        }

        // lo llama calcular_tiempo_atencion()
        private double atencion_consulta(double random)
        {
            // Consulta -> U(2;5) | a y b pueden variar | (b-a)*RND+a
            // TODO Falta poder modificar el tiempo
            double a = 2;
            double b = 5;
            return (b - a)*random + a;
        }
    

        private double calcular_fin_atencion(double reloj, double tiempo_atencion)
        {
            // Calcula cuanto tiempo dura la atencion: reloj + tiempo_atencion
            return reloj + tiempo_atencion;
        }

        private double redondear(double value)
        {
            return Math.Truncate(value * 100) / 100;
        }
    }
}
