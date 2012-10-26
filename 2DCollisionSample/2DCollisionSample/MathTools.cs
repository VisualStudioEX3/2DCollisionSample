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
    /// Libreria matematica de ayuda.
    /// </summary>
    public static class MathTools
    {
        /// <summary>
        /// Calcula el angulo entre dos puntos.
        /// </summary>
        /// <param name="a">Punto A.</param>
        /// <param name="b">Punto B.</param>
        /// <returns>Devuelve el angulo en grados en los dos puntos.</returns>
        public static float GetAngle(Vector2 a, Vector2 b)
        {
            float angle = MathHelper.ToDegrees((float)Math.Atan2(b.Y - a.Y, b.X - a.X));
            return (angle < 0 ? angle + 360 : angle);
        }

        /// <summary>
        /// Desplaza un vector en un angulo.
        /// </summary>
        /// <param name="point">Vector a desplazar.</param>
        /// <param name="distance">Distancia en pixeles.</param>
        /// <param name="direction">Angulo que define la direccion.</param>
        /// <returns>Devuelve la nueva posicion del vector tras aplicar el desplazamiento.</returns>
        public static Vector2 Move(Vector2 point, int distance, float direction)
        {
            float rad = Microsoft.Xna.Framework.MathHelper.ToRadians(direction);

            // Forzamos rendodeo a mayor para asegurarnos de que el vector devuelto sea correcto con la 
            // posicion en pixeles y evitar fallos de precision por los decimales:
            float x = (float)Math.Round(point.X + distance * Math.Cos(rad), 0);
            float y = (float)Math.Round(point.Y + distance * Math.Sin(rad), 0);

            // Generamos el vector en base a los valores redondeados:
            return new Vector2(x, y);
        }

        /// <summary>
        /// Calcula la interseccion entre dos lineas.
        /// </summary>
        /// <param name="a">Inicio de la primera linea.</param>
        /// <param name="b">Final de la primera linea.</param>
        /// <param name="c">Inicio de la segunda linea.</param>
        /// <param name="d">Final de la segunda linea.</param>
        /// <returns>La coordenada de la interseccion o null si no hay interseccion.</returns>
        /// <remarks>Basado en el codigo de Marius Watz para Java: 
        /// http://workshop.evolutionzone.com/2007/09/10/code-2d-line-intersection/
        /// </remarks>
        public static Vector2? IntersectLines(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float xD1, yD1, xD2, yD2, xD3, yD3;
            double dot, deg, len1, len2;
            double segmentLen1, segmentLen2;
            float ua, ub, div;

            // calculate differences  
            xD1 = b.X - a.X;
            xD2 = d.X - c.X;
            yD1 = b.Y - a.Y;
            yD2 = d.Y - c.Y;
            xD3 = a.X - c.X;
            yD3 = a.Y - c.Y;

            // calculate the lengths of the two lines  
            len1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1);
            len2 = Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate angle between the two lines.  
            dot = (xD1 * xD2 + yD1 * yD2); // dot product  
            deg = dot / (len1 * len2);

            // if abs(angle)==1 then the lines are parallell,  
            // so no intersection is possible  
            if (Math.Abs(deg) == 1) return null;

            // find intersection Pt between two lines  
            Vector2 pt = new Vector2(0, 0);
            div = yD2 * xD1 - xD2 * yD1;
            ua = (xD2 * yD3 - yD2 * xD3) / div;
            ub = (xD1 * yD3 - yD1 * xD3) / div;
            pt.X = a.X + ua * xD1;
            pt.Y = a.Y + ua * yD1;

            // calculate the combined length of the two segments  
            // between Pt-A and Pt-B  
            xD1 = pt.X - a.X;
            xD2 = pt.X - b.X;
            yD1 = pt.Y - a.Y;
            yD2 = pt.Y - b.Y;
            segmentLen1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate the combined length of the two segments  
            // between Pt-C and Pt-D  
            xD1 = pt.X - c.X;
            xD2 = pt.X - d.X;
            yD1 = pt.Y - c.Y;
            yD2 = pt.Y - d.Y;
            segmentLen2 = Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // if the lengths of both sets of segments are the same as  
            // the lenghts of the two lines the point is actually  
            // on the line segment.  

            // if the point isn't on the line, return null  
            if (Math.Abs(len1 - segmentLen1) > 0.01 || Math.Abs(len2 - segmentLen2) > 0.01)
                return null;

            // return the valid intersection  
            return pt;
        }
    }
}
