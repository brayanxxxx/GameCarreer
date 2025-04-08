namespace RaceX
{
    public class AutoHibrido : Auto
    {
        public AutoHibrido(string nombre) : base(nombre)
        {
            TipoAuto = "Híbrido";
        }

        public override int Avanzar(int metrosBase, string clima)
        {
            int penalizacion = clima == "Ventoso" ? -1 : 0;
            int avance = metrosBase + penalizacion;
            DistanciaRecorrida += avance;
            return avance;
        }
    }
}