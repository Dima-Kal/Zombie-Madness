using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ZombieMadnessMP
{
    /// <summary>
    /// This class manages and draws the map
    /// </summary>
    class Map
    {
        /// <summary>
        /// Map X*Y
        /// </summary>
        private const int mapSize = 50;
        /// <summary>
        /// Map tile X*Y
        /// </summary>
        private const int tileSize = 50;
        /// <summary>
        /// Map 2d array tiles
        /// </summary>
        private int[,] map;
        /// <summary>
        /// Tile textures list
        /// </summary>
        private List<Texture2D> tiles = new List<Texture2D>();


        public Map(Player player, ContentManager content, Vector2 ScreenSize)
        {
            tiles.Add(content.Load<Texture2D>("Map/Grass"));
            tiles.Add(content.Load<Texture2D>("Map/Wall"));
            tiles.Add(content.Load<Texture2D>("Map/Asphalt"));
            map = new int[mapSize, mapSize];
            ReadMapFile("Content/Map/Map1.txt");
        }

        private void ReadMapFile(string filename)
        {
            {
                char[] seps = { ',' };
                string[] arrayStrMap = System.IO.File.ReadAllLines(filename);
                for (int iRow = 0; iRow < mapSize; iRow++)
                {
                    string strMap = arrayStrMap[iRow];
                    string[] stringsMap = strMap.Split(seps);

                    for (int iCol = 0; iCol < mapSize; iCol++)
                    {
                        map[iRow, iCol] = int.Parse(stringsMap[iCol]);
                    }
                }
            }
        }

        public void Draw(SpriteBatch sBatch)
        {
            //a pair of for loops to iterate through both dimensions
            //of the array containing the map
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    sBatch.Draw(tiles[map[y, x]],
                        new Rectangle(
                            x * tileSize,
                            y * tileSize,
                            tileSize,
                            tileSize),
                            Color.White);
                }
            }

        }
    }
}
