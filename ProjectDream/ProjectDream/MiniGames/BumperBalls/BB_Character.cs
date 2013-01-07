using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.BumperBalls
{
    public class BB_Character : Player
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

        const float BOUNCE = 0.5F;

        //  Frame Settings
        private int currentFrame;
        private int numberOfFrames;
        private float timer;
        private float interval;
        private float rotation;
        public float scale;

        //  Movement
        private bool onGround;        
        private float groundLevel;

        public Vector2 velocity;
        public Vector2 knockDirection;

        public bool isInvulnerable { get; private set; }        
        private float alpha;                //  Gives a flash effect
        private float knockbackDuration;    //  1 second        
        private float knockbackTimer;

        private Action currentAction; // row for animation         
        public Vector2 size { get; private set; }
        public Rectangle sourceRectangle { get; private set; }
        public bool dying;
        public bool dead { get; private set; }
        public int team;

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

        public BB_Character()
        {

        }

        public bool collides(BB_Character otherPlayer)
        {
             if(Vector2.Distance(position + size/2 ,otherPlayer.position + ((otherPlayer.size)/2)) <= size.Y ){
                 return true;
             }     
            return false;
        }

        public override void Initialize(Texture2D sprite, Vector2 position, Vector2 origin)
        {
            base.Initialize(sprite, position, origin);
            timer = 0.0f;
            interval = 100.0f;
            onGround = true; 
            knockbackTimer = 0.0f;
            knockbackDuration = 500.0f;
            rotation = 0;
            alpha = 255.0f;
            scale = 1f;
            groundLevel = position.Y;
            CurrentAction = Action.Run; 
            //sourceRectangle = new Rectangle((int)(currentFrame * size.X), ((int)CurrentAction * (int)size.Y), (int)size.X, (int)size.Y);            
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
                    break;
                case Action.Slide:
                    size = new Vector2(64.0f, 64.0f);
                    break;
                case Action.Slash:
                    size = new Vector2(64.0f, 64.0f);
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

        public override void Move(float gameTime)
        {
            switch (CurrentAction)
            {
                case Action.Knockback:
                    velocity = BOUNCE * knockDirection;
                    KnockbackEffect(gameTime);
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
                case Action.Run: break;
                case Action.Slide:
                    velocity.X = 0.0f;
                    break;
                case Action.Victorious:
                case Action.Die:
                    //  scale -= new Vector2(0.00002f, 0.00002f);
                    break;
            }
            if (dying)
            {
               // collisionSize = new Vector2(0.0f, 0.0f);
                scale -= 0.025f;
                rotation -= 0.5f;
                if (scale <= 0 )
                {
                    dead = true;
                }
            }
            this.position += this.velocity;
            //base.Move((float)gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            if (team == 0)
            {
            //    color = Color.Red;
               // color.A = 100;
            }
            if (team == 1)
            {
                //color = Color.Blue;
              //  color.A = 100;
            }
            sourceRectangle = new Rectangle((int)(currentFrame * size.X), ((int)CurrentAction * (int)size.Y), (int)size.X, (int)size.Y);
            spriteBatch.Draw(sprite, position, sourceRectangle, color, rotation++, size / 2, scale, SpriteEffects.None, 0.0f);
        }

    }
}
