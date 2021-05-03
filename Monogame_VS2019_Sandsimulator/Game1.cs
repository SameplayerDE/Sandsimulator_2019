using System;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;

namespace Monogame_VS2019_Sandsimulator
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Grid _grid;
        private int size = 10;
        private int _type = 1;

        private Texture2D _pixel;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _grid = new Grid(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = Content.Load<Texture2D>("pixel");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                _type = 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                _type = 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                _type = 3;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                for (int i = -size; i < size; i++)
                {
                    for (int j = -size; j < size; j++)
                    {
                        Vector2 spawnLoc = new Vector2(Mouse.GetState().X + i, Mouse.GetState().Y + j);
                        Vector2 MouseLoc = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                        if (Vector2.Distance(spawnLoc, MouseLoc) <= size)
                        {
                            _grid.Add(Mouse.GetState().X + i, Mouse.GetState().Y + j, new Particle(_type));
                        }
                    }
                }
                
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                for (int i = -size; i < size; i++)
                {
                    for (int j = -size; j < size; j++)
                    {
                        Vector2 spawnLoc = new Vector2(Mouse.GetState().X + i, Mouse.GetState().Y + j);
                        Vector2 MouseLoc = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                        if (Vector2.Distance(spawnLoc, MouseLoc) <= size)
                        {
                            _grid.Remove(Mouse.GetState().X + i, Mouse.GetState().Y + j);
                        }
                    }
                }

            }

            _grid.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for (int y = 0; y < _grid.Height; y++)
            {

                for (int x = 0; x < _grid.Width; x++)
                {
                    if (_grid.Particles[x, y] != null)
                    {
                        Color color = Color.Black;
                        if (_grid.Particles[x, y].Type == 1)
                        {
                            color = Color.Wheat;
                        }
                        if (_grid.Particles[x, y].Type == 2)
                        {
                            color = Color.LightBlue;
                        }
                        _spriteBatch.Draw(_pixel, new Vector2(x, y), color);
                    }
                    

                }
            }

           
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    internal class Grid
    {
        public Particle[,] Particles;
        public int Width = 0;
        public int Height = 0;

        public int StepsPerSecond = 10;
        public TimeSpan LastTimeSpan;

        public Grid(int w, int h)
        {
            Width = w;
            Height = h;
            Particles = new Particle[w, h];
        }

        public void Add(int x, int y, Particle particle)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return;
            }

            if (Particles[x, y] == null)
            {
                Particles[x, y] = particle;
            }
        }

        public void Remove(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return;
            }

            Particles[x, y] = null;
        }

        public int IsSpace(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return -1;
            }
            else
            {
                if (Particles[x, y] != null)
                {
                    return 0;
                }
            }

            return 1;
        }

        public void Update(GameTime gameTime)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Particles[x, y] != null)
                    {
                        Particles[x, y].Updated = false;
                    }
                }
            }

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Particles[x, y] != null)
                    {
                        if (Particles[x, y].Updated == true)
                        {
                            continue;
                        }
                        Particles[x, y].Updated = true;
                        if (Particles[x, y].Type == 1)
                        {

                            if (IsSpace(x, y + 1) == 1)
                            {
                                Particles[x, y + 1] = Particles[x, y];
                                Particles[x, y] = null;
                            }

                            else if (IsSpace(x - 1, y + 1) == 1)
                            {
                                Particles[x - 1, y + 1] = Particles[x, y];
                                Particles[x, y] = null;
                            }

                            else if (IsSpace(x + 1, y + 1) == 1)
                            {
                                Particles[x + 1, y + 1] = Particles[x, y];
                                Particles[x, y] = null;
                            }
                        }
                        else if (Particles[x, y].Type == 2)
                        {
                            if (IsSpace(x, y + 1) == 1)
                            {
                                Particles[x, y + 1] = Particles[x, y];
                                Particles[x, y] = null;
                            }

                            else if (IsSpace(x - 1, y + 1) == 1)
                            {
                                Particles[x - 1, y + 1] = Particles[x, y];
                                Particles[x, y] = null;
                            }

                            else if (IsSpace(x + 1, y + 1) == 1)
                            {
                                Particles[x + 1, y + 1] = Particles[x, y];
                                Particles[x, y] = null;
                            }

                            else if (IsSpace(x - 1, y) == 1)
                            {
                                Particles[x - 1, y] = Particles[x, y];
                                Particles[x, y] = null;
                            }

                            else if (IsSpace(x + 1, y) == 1)
                            {
                                Particles[x + 1, y] = Particles[x, y];
                                Particles[x, y] = null;
                            }

                        }
                        else if (Particles[x, y].Type == 3)
                        {

                        }

                    }
                }
            }

            if (gameTime.TotalGameTime - LastTimeSpan >= new TimeSpan(0, 0, 0, 0, 5))
            {
                

                for (int i = 0; i < StepsPerSecond; i++) {
                    
                    
                }

                LastTimeSpan = gameTime.TotalGameTime;
            }
        }

    }

    internal class Particle
    {
        public int Type = 0;
        public bool Updated = false;

        public Particle(int type)
        {
            Type = type;
        }
    }
}
