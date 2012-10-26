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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DCollisionEngine
{
    /// <summary>
    /// Clase con funciones de ayuda.
    /// </summary>
    public static class Helper
    {
        private static Texture2D dummy;
        public static SpriteFont font;
        public static GraphicsDevice graphicDevice;
        public static SpriteBatch spriteBatch;

        /// <summary>
        /// Intercambia los valores entre dos variables.
        /// </summary>
        /// <typeparam name="T">Tipo de dato de las variables.</typeparam>
        /// <param name="a">Variable A.</param>
        /// <param name="b">Variable B.</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a; a = b; b = c;
        }

        /// <summary>
        /// Convierte una estructura Point en Vector2.
        /// </summary>
        /// <param name="point">Punto a convertir.</param>
        /// <returns>Estructura Vector2 con la informacion del punto.</returns>
        public static Vector2 PointToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        /// <summary>
        /// Convierte una estructura Vector2 en Point.
        /// </summary>
        /// <param name="vector">Vector a convertir.</param>
        /// <returns>Estructura Point con la informacion del vector.</returns>
        public static Point Vector2ToPoint(Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        private static void CreateDummyTexture()
        {
            dummy = new Texture2D(graphicDevice, 1, 1); // Creamos una textura base de 1x1;
            dummy.SetData<Color>(new Color[1] { Color.White }); // Asignamos el color blanco al pixel de la textura.
        }        

        /// <summary>
        /// Dibuja un rectangulo.
        /// </summary>
        /// <param name="area">Area del rectangulo en pantalla.</param>
        /// <param name="color">Color de relleno.</param>
        public static void DrawBox(Rectangle area, Color color)
        {
            if (dummy == null) CreateDummyTexture(); // Si no se inicializo la textura base se inicializa.
            spriteBatch.Draw(dummy, area, color); // Dibuja la textura con el area indicada y con el color indicado.
        }

        /// <summary>
        /// Dibuja una linea.
        /// </summary>
        /// <param name="a">Punto de inicio de la linea.</param>
        /// <param name="b">Punto final de la linea.</param>
        /// <param name="color">Color de la linea.</param>
        public static void DrawLine(Vector2 a, Vector2 b, Color color)
        {
            if (dummy == null) CreateDummyTexture(); // Si no se inicializo la textura base se inicializa.
            // Dibujamos la textura estirada y aplicando el angulo correcto:
            spriteBatch.Draw(dummy, new Rectangle((int)a.X, (int)a.Y, (int)Vector2.Distance(a,b), 1), null, color, MathHelper.ToRadians(MathTools.GetAngle(a, b)), Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Dibuja una cadena de texto.
        /// </summary>
        /// <param name="text">Cadena de texto a escribir.</param>
        /// <param name="location">Posicion del texto.</param>
        /// <param name="color">Color del texto.</param>
        public static void DrawText(string text, Vector2 location, Color color)
        {
            spriteBatch.DrawString(font, text, location, color);
        }
    }
}
