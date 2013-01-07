using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectDream.MiniGames.FireballTactics
{
    class FT_PlayerManager
    {
        FT_Character[] characters;
        Player[] players;

        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;

        AudioEngine audioEngine;
        WaveBank waveBank; // has all our .wav files
        SoundBank soundBank; // collection of sounds

        Vector2 soloPosition;

        Texture2D fireball;
        Texture2D fireballMeter;
        Rectangle fireballRectangle;
        Texture2D firewall;

        Firewall leftFire;
        Firewall rightFire;

        Firewall[] deathFlames;

        public static float fireballMeterPosition;
        float fireballTimer;
        float elevatedHeight;

        List<Fireball> fireballsReleased;
        int numberOfShots;

        Vector2 fireballVelocity;
        float shootTimer;
        bool isShot;

        int id1 = 0;
        //const float MAX_SPEED = 4;
        //const float ACCELERATION = 0.1F;
        //const float DAMPENER = 75;

        public FT_PlayerManager(Player[] players)
        {
            this.players = players;
        }

        public void LoadContent(ContentManager Content, Vector2 center)
        {
            soloPosition = center + new Vector2(100, -270);
            elevatedHeight = 50.0f;
            fireball = Content.Load<Texture2D>("FireballTactics/Fireball");
            fireballMeter = Content.Load<Texture2D>("FireballTactics/FireballMeter");
            fireballsReleased = new List<Fireball>();
            firewall = Content.Load<Texture2D>("FireballTactics/Firewall");

            leftFire = new Firewall(firewall, new Vector2(96.0f, (center.Y * 2) - (72.0f + elevatedHeight)), 3.0f, 0, true);
            rightFire = new Firewall(firewall, new Vector2((center.X * 2) - 96.0f, (center.Y * 2) - (72.0f + elevatedHeight)), 3.0f, 2, true);

            numberOfShots = 5;
            isShot = false;
            shootTimer = 1500.0f;
            fireballRectangle = new Rectangle(0, 0, 64, 64);

            characters = new FT_Character[4];
            deathFlames = new Firewall[4];
            audioEngine = new AudioEngine(@"Content\Character\LumberjackBustle.xgs");

            waveBank = new WaveBank(audioEngine, @"Content\Character\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Character\Sound Bank.xsb");

            players[0].sprite = Content.Load<Texture2D>(@"Character\BlueKnightSpriteSheet");
            players[1].sprite = Content.Load<Texture2D>(@"Character\PinkKnightSpriteSheet");
            players[2].sprite = Content.Load<Texture2D>(@"Character\BlackKnightSpriteSheet");
            players[3].sprite = Content.Load<Texture2D>(@"Character\GreenKnightSpriteSheet");
            // whoever's the loser will be on top.            
            for (int i = 1; i < 4; i++)
            {
                if (players[i] != null)
                {
                    if (players[i].Score < players[id1].Score)
                    {
                        id1 = i;
                    }
                }
            }

            int bottomCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (i == id1)
                {
                    characters[i] = new FT_Character();
                    characters[i].Initialize(players[i].sprite,
                                          soloPosition,
                                          Vector2.Zero);
                    characters[i].team = 0;
                    deathFlames[i] = new Firewall(firewall, new Vector2(96.0f, (center.Y * 2) - (72.0f + elevatedHeight)), 1.5f, 0, false);
                }
                else
                {
                    characters[i] = new FT_Character();
                    characters[i].Initialize(players[i].sprite,
                                          new Vector2(center.X - 100 + bottomCount * 100, FT_Character.groundLevel),
                                          Vector2.Zero);
                    characters[i].team = 1;
                    deathFlames[i] = new Firewall(firewall, new Vector2(96.0f, (center.Y * 2) - (72.0f + elevatedHeight)), 1.5f, 0, false);
                    bottomCount++;
                }
            }
        }

        public void InitializePlayerPositions(Vector2 center)
        {
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;

            if (Firewall.firewallTimer > 150.0f)
            {
                Firewall.firewallTimer = 0.0f;
            }
            Firewall.firewallTimer += elapsedTime;

            PlayerActions(elapsedTime);

            int counter = 0;
            foreach (FT_Character player in characters)
            {
                if (player != null && !player.dead)
                {
                    if (player.team != 0 && !player.dying)
                    {
                        CheckCollision(player, counter);
                        CheckFireCollision(player);
                    }
                    if (player.dying)
                    {
                        deathFlames[counter].position = player.position + new Vector2(24, player.size.Y/2); 
                        deathFlames[counter].active = true;
                    }
                    player.Move(elapsedTime);
                }
                counter++;
            }
            foreach (Fireball fb in fireballsReleased)
            {
                fb.Move();
            }
        }

        public void UpdateNoInput(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            foreach (FT_Character player in characters)
            {
                if (player != null)
                    player.Move(elapsedTime);
            }

            if (Firewall.firewallTimer > 150.0f)
            {
                Firewall.firewallTimer = 0.0f;
            }
            Firewall.firewallTimer += elapsedTime;
            foreach (Fireball fb in fireballsReleased)
            {
                fb.Move();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            leftFire.Draw(spriteBatch);
            rightFire.Draw(spriteBatch);

            for (int i = 0; i < deathFlames.Length ; i++)
            {
                if (deathFlames[i].active && !characters[i].dead)
                {
                    deathFlames[i].Draw(spriteBatch);
                }
            }
            foreach (FT_Character player in characters)
            {
                if (player != null && !player.dead)
                    player.Draw(spriteBatch);
            }
            foreach (Fireball fb in fireballsReleased)
            {
                if(fb.active)
                    fb.Draw(spriteBatch);
            }
            for (int i = 0; i < numberOfShots; i++)
            {
                if (isShot)
                {
                    spriteBatch.Draw(fireballMeter, new Vector2(fireballMeterPosition - i * 48.0f, 32.0f), new Color(150, 150, 150));
                }
                else
                {
                    spriteBatch.Draw(fireballMeter, new Vector2(fireballMeterPosition - i * 48.0f, 32.0f), Color.White);
                }
            }
        }

        private void PlayerActions(float elapsedTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
                        
            if (!characters[0].dead && id1 != 0)
            {
                PlayerInput(characters[0], GamePad.GetState(PlayerIndex.One), Keys.G, Keys.T, Keys.F, Keys.H, elapsedTime);
            }
            else if (id1 == 0)
            {
                SoloPlayerInput(characters[0], GamePad.GetState(PlayerIndex.One), Keys.G, Keys.T, Keys.F, Keys.H, elapsedTime);
            }

            if (!characters[1].dead && id1 != 1)
            {
                PlayerInput(characters[1], GamePad.GetState(PlayerIndex.Two), Keys.K, Keys.I, Keys.J, Keys.L, elapsedTime);
            }
            else if (id1 == 1)
            {
                SoloPlayerInput(characters[1], GamePad.GetState(PlayerIndex.Two), Keys.K, Keys.I, Keys.J, Keys.L, elapsedTime);
            }

            if (!characters[2].dead && id1 != 2)
            {
                PlayerInput(characters[2], GamePad.GetState(PlayerIndex.Three), Keys.S, Keys.W, Keys.A, Keys.D, elapsedTime);
            }
            else if (id1 == 2)
            {
                SoloPlayerInput(characters[2], GamePad.GetState(PlayerIndex.Three), Keys.S, Keys.W, Keys.A, Keys.D, elapsedTime);
            }

            if (!characters[3].dead && id1 != 3)
            {
                PlayerInput(characters[3], GamePad.GetState(PlayerIndex.Four), Keys.Down, Keys.Up, Keys.Left, Keys.Right, elapsedTime);
            }
            else if (id1 == 3)
            {
                SoloPlayerInput(characters[3], GamePad.GetState(PlayerIndex.Four), Keys.Down, Keys.Up, Keys.Left, Keys.Right, elapsedTime);
            }
        }

        private void SoloPlayerInput(FT_Character player, GamePadState padState, Keys shoot, Keys left, Keys right, Keys specialAttack, float elapsedTime)
        {
            if (currentKeyboardState.IsKeyDown(shoot) || padState.IsButtonDown(Buttons.A))
            {
                player.CurrentAction = FT_Character.Action.Freeze;
                if (!isShot && numberOfShots > 0)
                {
                    isShot = true;
                    numberOfShots--;
                    if (player.isMovingLeft)
                    {
                        fireballVelocity = new Vector2(-5.0f, 2.5f);
                    }
                    else
                    {
                        fireballVelocity = new Vector2(5.0f, 2.5f);
                    }

                    fireballsReleased.Add(new Fireball(fireball, player.position + new Vector2(0.0f, 64.0f), fireballVelocity, 0.5f));
                }
            }
            else if (currentKeyboardState.IsKeyDown(specialAttack) || padState.IsButtonDown(Buttons.B))
            {
                player.CurrentAction = FT_Character.Action.Freeze;
                if (!isShot && numberOfShots >= 3)
                {
                    isShot = true;
                    numberOfShots -= 3;
                    fireballVelocity = new Vector2(5.0f, 0.0f);
                    fireballsReleased.Add(new Fireball(fireball, new Vector2(0.0f, Fireball.groundLevel), fireballVelocity, 1.0f));
                }
            }
            else if (currentKeyboardState.IsKeyDown(left) || padState.IsButtonDown(Buttons.DPadLeft))
            {
                player.CurrentAction = FT_Character.Action.RunLeft;
            }
            else if (currentKeyboardState.IsKeyDown(right) || padState.IsButtonDown(Buttons.DPadRight))
            {
                player.CurrentAction = FT_Character.Action.RunRight;
            }
            else
            {
                player.CurrentAction = FT_Character.Action.Freeze;
            }

            if (isShot)
            {
                shootTimer -= elapsedTime;
                if (shootTimer <= 0.0f)
                {
                    isShot = false;
                    shootTimer = 1500.0f;
                }
            }
            if (numberOfShots < 5)
            {
                fireballTimer += elapsedTime;
                if (fireballTimer > 3000.0f)
                {
                    fireballTimer = 0.0f;
                    numberOfShots++;
                }
            }
        }

        private void PlayerInput(FT_Character player, GamePadState padState, Keys duck, Keys jump, Keys left, Keys right, float elapsedTime)
        {
            if (player.dying)
            {
                player.CurrentAction = FT_Character.Action.Slide;
            }
            else
            {
                if (player.CurrentAction == FT_Character.Action.Knockback)
                {
                    //  do nothing until knockback effect is over.
                }
                else
                {
                    player.CurrentAction = FT_Character.Action.Freeze;

                    if (currentKeyboardState.IsKeyDown(left) || padState.IsButtonDown(Buttons.DPadLeft))
                    {
                        player.CurrentAction = FT_Character.Action.RunLeft;
                    }
                    if (currentKeyboardState.IsKeyDown(right) || padState.IsButtonDown(Buttons.DPadRight))
                    {
                        player.CurrentAction = FT_Character.Action.RunRight;
                    }
                    if (currentKeyboardState.IsKeyDown(duck) || padState.IsButtonDown(Buttons.X))
                    {
                        player.CurrentAction = FT_Character.Action.Duck;
                    }
                    if (currentKeyboardState.IsKeyDown(jump) || padState.IsButtonDown(Buttons.A))
                    {
                        player.CurrentAction = FT_Character.Action.Jump;
                        if (player.onGround)
                        {
                            soundBank.PlayCue("Jump");
                        }
                    }
                }
            }
        }

        public void CheckCollision(FT_Character checkingPlayer, int counter)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (counter != i)
                {
                    if (characters[i] != null && !characters[i].dead && !characters[i].dying)
                    {
                        if (checkingPlayer.collides(characters[i]))
                        {
                            if (checkingPlayer.position.X < characters[i].position.X)
                            {
                                checkingPlayer.velocity.X = -1.5f;
                                characters[i].velocity.X = 1.5f;
                            }
                            else
                            {
                                checkingPlayer.velocity.X = 1.5f;
                                characters[i].velocity.X = -1.5f;
                            }

                            soundBank.PlayCue("Collision");
                            characters[i].CurrentAction = FT_Character.Action.Knockback;
                            checkingPlayer.CurrentAction = FT_Character.Action.Knockback;
                        }
                    }
                }
            }
        }

        public void CheckFireCollision(FT_Character checkingPlayer)
        {
            if (leftFire.collides(checkingPlayer) || rightFire.collides(checkingPlayer))
            {
                checkingPlayer.dying = true;
                checkingPlayer.CurrentAction = FT_Character.Action.Slide;
            }
            else
            {
                foreach (Fireball fb in fireballsReleased)
                {
                    if (fb.bounces > 2)
                    {
                        fb.active = false;
                    }
                    else if (fb.collides(checkingPlayer))
                    {
                        fb.active = false;
                        checkingPlayer.dying = true;
                        checkingPlayer.CurrentAction = FT_Character.Action.Slide;
                    }
                }
            }
        }

        public int FindEliminated()
        {
            int deadCount = 0;
            int counter = 0;
            foreach (FT_Character character in characters)
            {
                if (character != null)
                {
                    if (character.dying)
                    {
                        deadCount++;
                    }
                    else if (character.dead)
                    {
                        deadCount++;
                        deathFlames[counter].active = false;
                        character.CurrentAction = FT_Character.Action.Die;
                    }
                }
            }
            counter++;
            return deadCount;
        }

        public int teamWin()
        {
            int deadCount = 0;

            foreach (FT_Character FT_Character in characters)
            {
                if (FT_Character != null)
                {
                    if (FT_Character.dead || FT_Character.dying)
                    {
                        if (FT_Character.team == 1)
                        {
                            deadCount++;
                        }
                    }
                }
            }
            if (deadCount == 3)
            {
                return 0;
            }
            return 1;
        }

        public void setWinner()
        {
            //TODO increment scores for winners players[i].Score += 1;
            int winningTeam = teamWin();
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    if (winningTeam == 1)
                    {
                        if (characters[i].team == 1)
                        {
                            characters[i].velocity = new Vector2(0, characters[i].velocity.Y);
                            characters[i].CurrentAction = FT_Character.Action.Jump;
                        }
                        else
                        {
                            characters[i].velocity = new Vector2(0, characters[i].velocity.Y);
                            characters[i].CurrentAction = FT_Character.Action.Duck;
                        }
                    }
                    else
                    {
                        if (characters[i].team == 0)
                        {
                            characters[i].velocity = new Vector2(0, characters[i].velocity.Y);
                            characters[i].CurrentAction = FT_Character.Action.Jump;
                        }
                        else
                        {
                            characters[i].velocity = new Vector2(0, 0);
                        }
                    }
                }
            }
            for (int j = 0; j < characters.Length; j++)
            {
                if (characters[j].team == winningTeam)
                {
                    players[j].Score += 1;
                }
            } 
        }
    }
}
