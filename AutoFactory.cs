using System;

namespace RaceX
{
    public static class AutoFactory
    {
        public static Auto CrearAuto(string nombre, string tipo)
        {
            if (tipo == "Deportivo")
                return new AutoDeportivo(nombre);
            else if (tipo == "Todoterreno")
                return new AutoTodoterreno(nombre);
            else if (tipo == "Híbrido")
                return new AutoHibrido(nombre);
            else
                throw new ArgumentException("Tipo de auto no válido");
        }
    }
}