using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.FireballTactics
{
    class FT_Character : Player
    {
        public enum Action : int
        {
            Jump = 0, // Freezes current frame 
            Duck,
            Knockback,
            Slide,
            Slash,            
            RunRight,
            RunLeft,
            Freeze,
            Victorious,
            Die
        }        

        //  Frame Settings
        private int currentFrame;
        private int numberOfFrames;
        private float timer;
        private float interval;
        private float rotation;

        SpriteEffects effect;
        //  Movement
        public bool onGround { get; private set; }
        public static float groundLevel;


        public bool isInvulnerable { get; private set; }

        public Vector2 velocity;
        
        private float alpha;                //  Gives a flash effect
        private float knockbackDuration;    //  1 second        
        private float knockbackTimer;
        private Color color = Color.White;
        private Action currentAction; // row for animation         
        public Vector2 size { get; private set; }
        public Vector2 collisionSize { get; private set; }
        public Vector2 scale { get; private set; }
        public Rectangle sourceRectangle { get; private set; }

        public bool isMovingLeft { get; private set; }
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
                    //isInvulnerable = true;
                }

                ActionSize();
                ActionFrames();
            }
        }

        public FT_Character()
        {

        }

        public bool collides(FT_Character otherPlayer)
        {
            if (currentAction != Action.Knockback)
            {
                Rectangle playerRectangle = new Rectangle((int)position.X, (int)(position.Y + (64 - size.Y)), (int)collisionSize.X, (int)collisionSize.Y);
                Rectangle otherPlayerRectangle = new Rectangle((int)otherPlayer.position.X, (int)(otherPlayer.position.Y + (64 - otherPlayer.size.Y)), (int)otherPlayer.collisionSize.X, (int)otherPlayer.collisionSize.Y);
                if (playerRectangle.Intersects(otherPlayerRectangle))
                    return true;
            }
            return false;
        }

        public override void Initialize(Texture2D sprite, Vector2 position, Vector2 origin)
        {
            base.Initialize(sprite, position, origin);
            timer = 0.0f;
            interval = 100.0f;
            
            //  Knockback effect
            knockbackTimer = 0.0f;
            knockbackDuration = 300.0f;
            alpha = 255.0f;

            rotation = 0;
            scale = new Vector2(1, 1);
            
            onGround = true;
            dying = false;
            dead = false;            
            effect = SpriteEffects.None;
            CurrentAction = Action.Freeze;
            this.ActionSize();
            origin = size / 2;            
        }

        public override void Move(float elapsedTime)
        {
            switch (CurrentAction)
            {
                case Action.Knockback:                    
                    KnockbackEffect(elapsedTime);
                    break;
                case Action.Jump:
                    if (onGround)
                    {
                        onGround = false;
                        velocity.Y = -20.0f;
                    }
                    break;
                case Action.Duck:
                    if (onGround)
                    {
                        velocity = new Vector2(0.0f, velocity.Y);
                    }
                    break;
                case Action.Slide:
                    if (onGround)
                    {
                        velocity = new Vector2(0.0f, velocity.Y);
                    }
                    break;                
                case Action.RunLeft:
                    isMovingLeft = true;
                    velocity = new Vector2(-5.0f, velocity.Y);
                    effect = SpriteEffects.FlipHorizontally;
                    break;
                case Action.RunRight:
                    isMovingLeft = false;
                    velocity = new Vector2(5.0f, velocity.Y);
                    effect = SpriteEffects.None;
                    break;
                case Action.Freeze:                    
                    velocity = new Vector2(0.0f, velocity.Y);                        
                    break;
                case Action.Victorious:
                    //currentAction = Action.Jump;
                    break;
                case Action.Die:
                    dead = true;
                    isInvulnerable = true;
                    break;
            }

            if (!onGround)
            {                
                velocity.Y += 1;
            }

            if (position.Y > groundLevel)
            {
                onGround = true;
                velocity = new Vector2(velocity.X, 0.0f);
                position = new Vector2(position.X, FT_Character.groundLevel);
            }
            if (dying)
            {
                collisionSize = new Vector2(0.0f, 0.0f);
                color = Color.Gray;
              //  scale -= new Vector2(0.001f, 0.001f);
                alpha -= 2f;
                if (alpha <= 0.0f)
                {
                    dead = true;
                    //dying = false;
                }
            }
            position += velocity;
            if (team == 0)
            {
                if (position.X >= 1200)
                {
                    position = new Vector2(1200, position.Y);
                }
                if (position.X <= 20)
                {
                    position = new Vector2(20, position.Y);
                }
            }
            ChangeFrames(elapsedTime);
        }

        private void ActionSize()
        {
            switch (CurrentAction)
            {
                case Action.Knockback:
                case Action.Jump:
                    size = new Vector2(48.0f, 64.0f);
                    collisionSize = new Vector2(48.0f, 64.0f);
                    break;
                case Action.Duck:
                    size = new Vector2(48.0f, 64.0f);
                    collisionSize = new Vector2(48.0f, 40.0f);
                    break;
                case Action.Freeze:
                case Action.RunRight:
                case Action.RunLeft:
                    size = new Vector2(48.0f, 64.0f);
                    collisionSize = new Vector2(48.0f, 64.0f);
                    break;
                case Action.Slide:
                    size = new Vector2(64.0f, 64.0f);
                    collisionSize = new Vector2(0.0f, 0.0f);
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
                    numberOfFrames = 1;
                    break;
                case Action.RunLeft:
                case Action.RunRight:
                    numberOfFrames = 2;
                    break;
                case Action.Slide:
                    numberOfFrames = 1;
                    break;
                case Action.Victorious:
                case Action.Die:
                    numberOfFrames = 1;
                    break;
            }
        }


        private void ChangeFrames(float elapsedTime)
        {
            if (currentAction == Action.Freeze)
            {
                currentFrame = 1;
            }
            else
            {
                timer += elapsedTime;
                if (timer > interval || numberOfFrames == 1)
                {
                    timer = 0.0f;
                    currentFrame++;
                    currentFrame = currentFrame % numberOfFrames;
                }
            }
        }

        private void KnockbackEffect(float elapsedTime)
        {
            knockbackTimer += elapsedTime;
            if (knockbackTimer < knockbackDuration)
            {
                alpha += 0.5f;
                if (alpha > 255 || alpha < 0)
                {
                    alpha *= -1;
                }
            }
            else
            {
                knockbackTimer = 0.0f;                
                currentAction = Action.Freeze;
                alpha = 255f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (currentAction == Action.Freeze || currentAction == Action.RunLeft)
            {
                sourceRectangle = new Rectangle((int)(currentFrame * size.X), ((int)Action.RunRight * (int)size.Y), (int)size.X, (int)size.Y);
            }                
            else
            {
                sourceRectangle = new Rectangle((int)(currentFrame * size.X), ((int)CurrentAction * (int)size.Y), (int)size.X, (int)size.Y);
            }
            spriteBatch.Draw(sprite, position, sourceRectangle, Color.FromNonPremultiplied(color.R, color.G, color.B, (int)alpha), rotation, origin, scale, effect, 0.0f);
        }
    }
}
