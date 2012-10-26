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
    /// Mundo fisico.
    /// </summary>
    /// <remarks>Define un espacio donde contener y representar objetos fisicos con los que representar e interactuar en la escena.</remarks>
    public class World
    {
        #region Miembros y propiedades
        private Body[] emptyListBody;
        private List<Body> collides;
        private List<Body> collisions;

        /// <summary>
        /// Lista de cuerpos que contiene la escena.
        /// </summary>
        public List<Body> bodies;  
        #endregion   

        #region Constructor
        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        public World()
        {
            collides = new List<Body>();
            bodies = new List<Body>();
            collisions = new List<Body>();
        } 
        #endregion

        #region Metodos y funciones
        /// <summary>
        /// Determina si un rectangulo intersecta con el area de algun cuerpo de la escena.
        /// </summary>
        /// <param name="rectangle">Rectangulo a evaluar.</param>
        /// <returns>Devuelve verdadero si hay interseccion con algun cuerpo.</returns>
        public bool IntersectRect(Rectangle rectangle)
        {
            return IntersectRect(rectangle, out emptyListBody);
        }

        /// <summary>
        /// Determina si un rectangulo intersecta con el area de algun cuerpo de la escena.
        /// </summary>
        /// <param name="rectangle">Rectangulo a evaluar.</param>
        /// <param name="collideWith">Devuelve la lista con los cuerpos con los que intersecta.</param>
        /// <returns>Devuelve verdadero si hay interseccion con algun cuerpo.</returns>
        public bool IntersectRect(Rectangle rectangle, out Body[] collideWith)
        {
            collides.Clear();
            foreach (Body body in bodies)
            {
                if (rectangle.Intersects(body.Rectangle))
                    collides.Add(body);
            }

            collideWith = collides.ToArray();
            return collides.Count > 0;
        }

        /// <summary>
        /// Actualiza el estado de la escena.
        /// </summary>
        public void Update()
        {
            // Actualizamos los estados de todos los cuerpos de la escena que no sean fijos y esten activos:
            foreach (Body body in bodies)
            {
                // Si se definio, ejecutamos el metodo de pre actualizacion del cuerpo:
                if (body.PreUpdate != null) body.PreUpdate(body);
                body.Update();
            }

            // Recorremos la lista de cuerpos de la escena y calculamos sus colisiones y su respuesta en caso de haber colision:
            foreach (Body currentBody in bodies)
            {
                // Buscamos colisiones con el resto de cuerpos solidos de la escena:
                collisions.Clear();
                foreach (Body body in bodies)
                {
                    // Descartamos al cuerpo actual en la busqueda:
                    if (currentBody != body && currentBody.Enabled)
                        if (currentBody.Rectangle.Intersects(body.Rectangle))
                            collisions.Add(body);
                }

                // Evaluamos la respuesta a la colision:
                if (collisions.Count > 0)
                {
                    // Si hay colisiones con el cuerpo actual lanzamos el evento y enviamos la lista de cuerpos que colisionan:
                    if (currentBody.OnCollision != null)
                        currentBody.OnCollision(collisions.ToArray());

                    // Descartamos primero cualquier cuerpo que no sea solido para no calcular respuesta con el:
                    List<Body> collisionsToResponse = new List<Body>();
                    foreach (Body body in collisions)
                        if (body.Solid && body.Fixed) collisionsToResponse.Add(body);

                    if (collisionsToResponse.Count > 0)
                    {
                        // Invertimos el angulo de direccion que tenia el cuerpo:
                        float dir = currentBody.Direction + (currentBody.Direction > 180 ? -180 : 180);

                        // Si hay colisiones obtenemos la respuesta a la colision para recolocar el cuerpo:
                        int count; do
                        {
                            // Retrocediendo un pixel en la direccion opuesta que recorria el cuerpo:
                            currentBody.Location = MathTools.Move(currentBody.Location, 1, dir);

                            // Comprobamos las colisiones restantes:
                            count = collisionsToResponse.Count;
                            foreach (Body b in collisionsToResponse)
                                if (!currentBody.Rectangle.Intersects(b.Rectangle)) count--;
                        }
                        while (count > 0);

                        // Igualamos la ultima posicion con la actual de la correccion para anular la direccion tomada en el proximo Update() del cuerpo:
                        currentBody.lastPoint = currentBody.Location; 
                    }
                }
            }

            // Si se definio, ejecutamos el metodo de post actualizacion del cuerpo:
            foreach (Body body in bodies)
                if (body.PostUpdate != null) body.PostUpdate(body);
        }

        /// <summary>
        /// Dibuja todos los cuerpos fisicos:
        /// </summary>
        public void Draw()
        {
            foreach (Body body in bodies)
                body.Draw();
        } 
        #endregion
    }
}
