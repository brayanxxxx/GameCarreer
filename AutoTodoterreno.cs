namespace RaceX
{
    public class AutoTodoterreno : Auto
    {
        public AutoTodoterreno(string nombre) : base(nombre)
        {
            TipoAuto = "Todoterreno";
        }

        public override int Avanzar(int metrosBase, string clima)
        {
            int bonificacion = clima == "Lluvia" ? 2 : 0;
            int avance = metrosBase + bonificacion;
            DistanciaRecorrida += avance;
            return avance;
        }
    }
}