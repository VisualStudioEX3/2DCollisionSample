// Copyright (C) 2012, José Miguel Sánchez Fernández 
//                                                          
// This file is part of 2DCollisionEngine project, a XNA program sample.
//
// 2DCollisionEngine project is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, version 2 of the License.
//
// 2DCollisionEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with 2DCollisionEngine project. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace _2DCollisionEngine
{
    /// <summary>
    /// Trazador de rayos.
    /// </summary>
    /// <remarks>Permite trazar una trayectoria desde un punto origen para obtener el punto de corte mas cercano con los objetos de la escena.</remarks>
    public class RayTracer
    {
        #region Estructuras internas
        /// <summary>
        /// Estructuras para gestionar las intersecciones, distancias y cuerpos en la busqueda de intersecciones del trazador de rayos.
        /// </summary>
        private struct Line
        {
            public Vector2 a;
            public Vector2 b;
            public Body hitBody;

            public Line(Vector2 a, Vector2 b, Body hitBody)
            {
                this.a = a; this.b = b; this.hitBody = hitBody;
            }
        }

        private struct IntersectionPoint
        {
            public Vector2 p;
            public float distance;
            public Body hitBody;

            public IntersectionPoint(Vector2 p, float distance, Body hitBody)
            {
                this.p = p; this.distance = distance; this.hitBody = hitBody;
            }
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Origen del rayo.
        /// </summary>
        public Vector2 Source { get; set; }
        
        /// <summary>
        /// Cuerpo origen desde el que se traza el rayo.
        /// </summary>
        /// <remarks>Se utiliza para descartar el cuerpo en el trazado del rayo. 
        /// Si no hubiera cuerpo de origen esta propiedad se establece a Null.</remarks>
        public Body SourceBody { get; set; }
        
        /// <summary>
        /// Punto de impacto.
        /// </summary>
        /// <remarks>Devuelve las coordenadas de impacto del rayo.
        /// Si el rayo no impacta con ningun cuerpo devuelve el punto más lejano del rayo.</remarks>
        public Vector2 Hit { get; internal set; }
        
        /// <summary>
        /// Direccion en la que se traza el rayo.
        /// </summary>
        public float Direction { get; set; }
        
        /// <summary>
        /// Alcance maximo que tendra el rayo.
        /// </summary>
        public int Radius { get; set; }
        
        /// <summary>
        /// Referencia al simulador al que esta asociado el trazador de rayos.
        /// </summary>
        public World World { get; internal set; }

        /// <summary>
        /// Color con el que se dibujara el rayo.
        /// </summary>
        public Color Color { get; set; }
        #endregion

        #region Constructores
        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="World">Referencia a la instancia del simulador al que se asociara el rayo.</param>
        /// <param name="Source">Punto de origen del rayo.</param>
        public RayTracer(World World)
        {
            this.World = World;
            this.Source = Vector2.Zero;
            this.Color = Color.Yellow;
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="World">Referencia a la instancia del simulador al que se asociara el rayo.</param>
        /// <param name="Source">Punto de origen del rayo.</param>
        public RayTracer(World World, Vector2 Source)
            : this(World)
        {
            this.Source = Source;
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="World">Referencia a la instancia del simulador al que se asociara el rayo.</param>
        /// <param name="Source">Punto de origen del rayo.</param>
        /// <param name="Radius">Radio maximo de alcance del rayo.</param>
        public RayTracer(World World, Vector2 Source, int Radius)
            : this(World, Source)
        {
            this.Radius = Radius;
        }
        #endregion

        #region Metodos publicos
        /// <summary>
        /// Traza el rayo con los parametros establecidos.
        /// </summary>
        /// <returns>Devuelve el cuerpo con el que impacta.</returns>
        /// <remarks>La propiedad Hit devuelve el punto de impacto exacto del rayo.</remarks>
        public Body Trace()
        {
            // Recorremos los cuerpos que esten dentro del area definida por la diagonal del rayo:
            List<Line> lines = new List<Line>();
            foreach (Body b in this.GetBodiesInArea(this.ComputeRayArea()))
            {
                // Añadimos las lineas del cuerpo a la lista a evaluar:
                lines.Add(new Line(new Vector2(b.Rectangle.Left, b.Rectangle.Top), new Vector2(b.Rectangle.Right, b.Rectangle.Top), b));
                lines.Add(new Line(new Vector2(b.Rectangle.Left, b.Rectangle.Top), new Vector2(b.Rectangle.Left, b.Rectangle.Bottom), b));
                lines.Add(new Line(new Vector2(b.Rectangle.Right, b.Rectangle.Top), new Vector2(b.Rectangle.Right, b.Rectangle.Bottom), b));
                lines.Add(new Line(new Vector2(b.Rectangle.Left, b.Rectangle.Bottom), new Vector2(b.Rectangle.Right, b.Rectangle.Bottom), b));
            }

            // Obtenemos todos los puntos de corte que haya en el trazado del rayo:
            List<IntersectionPoint> points = new List<IntersectionPoint>(); Vector2? ret;
            foreach (Line line in lines)
            {
                ret = MathTools.IntersectLines(this.Source, this.Hit, line.a, line.b);
                if (ret != null)
                    points.Add(new IntersectionPoint((Vector2)ret, Vector2.Distance(this.Source, (Vector2)ret), line.hitBody));
            }

            // Si existen puntos de interseccion obtenemos el mas cercano:
            if (points.Count > 0)
            {
                IntersectionPoint near = points[0];
                foreach (IntersectionPoint p in points)
                    if (p.distance < near.distance) near = p;
                this.Hit = near.p;      // Marcamos el impacto con el punto de interseccion mas cercano.
                return near.hitBody;    // Devolvemos el cuerpo de impacto mas cercano.
            }
            else // Si no hay puntos de corte devolvemos null y mantenemos el punto mas lejano del rayo como impacto:
                return null;
        }

        /// <summary>
        /// Dibuja el trazado del rayo.
        /// </summary>
        /// <remarks>Funcion de depuracion que representa el rayo en pantalla.</remarks>
        public void Draw()
        {
            Helper.DrawLine(Source, Hit, Color);
        }
        #endregion

        #region Metodos privados para calcular las intersecciones con los cuerpos
        /// <summary>
        /// Computa el area donde se proyecta el rayo.
        /// </summary>
        /// <returns></returns>
        private Rectangle ComputeRayArea()
        {
            Vector2 a = this.Source;
            Vector2 b = MathTools.Move(this.Source, this.Radius, Math.Abs(this.Direction)); this.Hit = b;

            if (a.X > b.X) { Helper.Swap<float>(ref a.X, ref b.X); }
            if (a.Y > b.Y) { Helper.Swap<float>(ref a.Y, ref b.Y); }

            /* En caso de que la direccion sea perpendicular, para evitar que el area de interseccion sea plana (sin volumen)
               segun la direccion incrementamos o decrementamos en 1 la altura o anchura del area: */
            switch ((int)this.Direction)
            {
                case 0:
                case 180: a.Y--; b.Y++; break;
                case 90:
                case 270: a.X--; b.X++; break;
            }

            return new Rectangle((int)a.X, (int)a.Y, (int)(b.X - a.X), (int)(b.Y - a.Y));
        }

        /// <summary>
        /// Obtiene la lista de cuerpos activos que estan dentro del area del rayo.
        /// </summary>
        /// <param name="RayArea">Area definida por la diagonal del rayo.</param>
        /// <returns>Lista de cuerpos encontrados.</returns>
        private List<Body> GetBodiesInArea(Rectangle RayArea)
        {
            List<Body> ret = new List<Body>();
            foreach (Body b in this.World.bodies)
                if (b != this.SourceBody && b.Enabled && b.Solid && Rectangle.Intersect(RayArea, b.Rectangle) != Rectangle.Empty)
                    ret.Add(b);
            return ret;
        }
        #endregion
    }
}