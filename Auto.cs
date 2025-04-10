using System;

//auto clase abstracta 
namespace RaceX
{
    public abstract class Auto
    {
        public string Nombre { get; private set; }
        public string TipoAuto { get; protected set; }
        public int DistanciaRecorrida { get; protected set; }

        protected Auto(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                throw new ArgumentException("El nombre del auto no puede estar vacío");

            Nombre = nombre;
            DistanciaRecorrida = 0;
        }

        public void AplicarObstaculo(int metros)
        {
            DistanciaRecorrida = Math.Max(0, DistanciaRecorrida + metros);
        }

        public abstract int Avanzar(int metrosBase, string clima);
    }
}