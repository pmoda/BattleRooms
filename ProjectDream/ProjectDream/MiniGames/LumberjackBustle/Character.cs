using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.LumberjackBustle
{
    public class Character : Player
    {
        public enum Action : int
        {
            Jump = 0, // Freezes current frame 
            Duck,
            Knockback,
            Slide,
            Slash,
            Run,
            Victorious,
            Die
        }

        //  Frame Settings
        private int currentFrame;
        private int numberOfFrames;
        private float timer;
        private float interval;

        //  Movement
        public bool onGround { get; private set; }
        private float groundLevel;

        //  Knockback Effect
        public bool isInvulnerable { get; private set; }
        private float alpha;                //  Gives a flash effect
        private float knockbackDuration;    //  1 second        
        private float knockbackTimer;

        private Vector2 velocity;
        private Action currentAction; // row for animation         
        public Vector2 size { get; private set; }
        public Vector2 collisionSize { get; private set; }
        public Vector2 scale{ get; private set; }
        public float rotation{ get; private set; }

        public Rectangle sourceRectangle { get; private set; }
        public bool dying;
        public bool  dead{ get; private set; }

        public Action CurrentAction
        {
            get { return currentAction; }
            set
            {
                currentAction = value;

                if (currentAction == Action.Knockback)
                {
                    isInvulnerable = true;
                }

                ActionSize();
                ActionFrames();
            }
        }

        public Character()
        {

        }

        public override void Initialize(Texture2D sprite, Vector2 position, Vector2 origin)
        {
            base.Initialize(sprite, position, origin);
            timer = 0.0f;
            interval = 100.0f;
            knockbackTimer = 0.0f;
            knockbackDuration = 500.0f;
            rotation = 0;
            scale = new Vector2(1, 1);
            alpha = 255.0f;
            onGround = true;
            dying = false;
            dead = false;
            groundLevel = position.Y;
            CurrentAction = Action.Run;
            this.ActionSize();
            origin = size / 2;
            //sourceRectangle = new Rectangle((int)(currentFrame * size.X), ((int)CurrentAction * (int)size.Y), (int)size.X, (int)size.Y);            
        }

        public override void Move(float elapsedTime)
        {
            switch (CurrentAction)
            {
                case Action.Knockback:
                    velocity.X = -1.5f;
                    KnockbackEffect(elapsedTime);
                    break;
                case Action.Jump:
                    if (onGround)
                    {
                        onGround = false;
                        velocity.Y = -16.0f;
                    }
                    break;
                case Action.Duck:
                case Action.Slash:
                case Action.Run:
                case Action.Slide:
                    velocity.X = 0.0f;
                    break;
                case Action.Victorious:
                case Action.Die:
                  //  scale -= new Vector2(0.00002f, 0.00002f);
                    break;
            }

            if (!onGround)
            {
                //jumpTimer += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 100);
                velocity.Y += 1;//(float)(Math.Pow(jumpTimer - 4, 2) - 16);
            }

            if (position.Y > groundLevel)
            {
                onGround = true;
                velocity = new Vector2(velocity.X, 0.0f);
                position = new Vector2(position.X, groundLevel);
            }
            if (dying)
            {
                collisionSize = new Vector2(0.0f, 0.0f);
                scale -= new Vector2(0.005f, 0.005f);
                rotation += 0.5f;
                if (scale.X <= 0 || scale.Y <= 0)
                {
                    dead = true;
                   // dying = false;
                }
            }
            position += velocity;
            ChangeFrames(elapsedTime);
        }

        private void ActionSize()
        {
            switch (CurrentAction)
            {
                case Action.Jump:
                case Action.Duck:
                case Action.Knockback:
                case Action.Run:
                    size = new Vector2(48.0f, 64.0f);
                    collisionSize = new Vector2(48.0f, 64.0f);
                    break;
                case Action.Slide:
                    size = new Vector2(64.0f, 64.0f);
                    collisionSize = new Vector2(64.0f, 40.0f);
                    break;
                case Action.Slash:
                    size = new Vector2(64.0f, 64.0f);
                    collisionSize = new Vector2(64.0f, 64.0f);
                    break;
                case Action.Victorious:
                    size = new Vector2(64.0f, 64.0f);
                    collisionSize = new Vector2(64.0f, 64.0f);
                    break;
                case Action.Die:
                    size = new Vector2(0.0f, 0.0f);
                    collisionSize = new Vector2(0.0f, 0.0f);
                    break;
            }
        }

        private void ActionFrames()
        {
            switch (CurrentAction)
            {
                case Action.Duck:
                case Action.Knockback:
                case Action.Jump:
                case Action.Slide:
                    numberOfFrames = 1;
                    break;
                case Action.Slash:
                    numberOfFrames = 2;
                    break;
                case Action.Run:
                    numberOfFrames = 2;
                    break;
                case Action.Victorious:
                case Action.Die:
                    break;
            }
        }


        private void ChangeFrames(float elapsedTime)
        {
            timer += elapsedTime;
            if (timer > interval || numberOfFrames == 1)
            {
                timer = 0.0f;
                currentFrame++;
                currentFrame = currentFrame % numberOfFrames;
            }
        }

        private void KnockbackEffect(float elapsedTime)
        {
            knockbackTimer += elapsedTime;
            if (knockbackTimer < knockbackDuration)
            {
                alpha += 1.0f;
                if (alpha > 255 || alpha < 0)
                {
                    alpha *= -1;
                }
            }
            else
            {
                knockbackTimer = 0.0f;
                isInvulnerable = false;
                currentAction = Action.Run;
                alpha = 255f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sourceRectangle = new Rectangle((int)(currentFrame * size.X), ((int)CurrentAction * (int)size.Y), (int)size.X, (int)size.Y);
            spriteBatch.Draw(sprite, position, sourceRectangle, new Color(255, 255, 255, alpha), rotation, origin, scale, SpriteEffects.None, 0.0f);
        }
    }
}
