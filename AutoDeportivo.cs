//auto deportivo
namespace RaceX
{
    public class AutoDeportivo : Auto
    {
        public AutoDeportivo(string nombre) : base(nombre)
        {
            TipoAuto = "Deportivo";
        }

        public override int Avanzar(int metrosBase, string clima)
        {
            int bonificacion = clima == "Soleado" ? 3 : 0;
            int avance = metrosBase + bonificacion;
            DistanciaRecorrida += avance;
            return avance;
        }
    }
}