using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A_level_NEA_coursework___2D_game
{


    public partial class Form1 : Form
    {

        AudioManager audioManager = new AudioManager();


        // Variables needed for the game
        bool Go_left;
        bool Go_Right;
        bool jump;
        bool HasTreasure = false; // Default value to check whether the bird has the treasure
        bool enemyAlive = true;


        int Speedjump = 10;
       // int force = 9;
        int Score = 0;
        int SpeedBird = 7;
        int Background_Speed = 9;
        int CoinsCount = 0;
        int life = 3;
        Random rnd = new Random();
        int x;

        public Form1()
        {
            InitializeComponent();
            level_over.Hide();
            Game_Update();
            Value_label.Text = Properties.Settings.Default.High_Score;

            // Start playing the background music
            // audioManager.PlayBackgroundMusic();
        }

        // Event handler for the game timer
        private void MainGameTimer(object sender, EventArgs e)
        {
            Enemy_move();
            Game_Update();
            Bird_Move();



            TXTcoinsCollect.Text = "Coins: " + CoinsCount; // adds the value of the current CoinsCount integer onto the Coins label

            Bird.Top += Speedjump; // add jump speed to the top location of the bird so the bird will always be pulled downwards unless it hits a platform



            if (Go_left == true && backGround.Left < 0) // background scroll - if the bird is moving left

            {

                backGround.Left += Background_Speed; // the background will move right/forward

                MovingGame("right");

            }

            if (Go_Right == true && backGround.Left > -1081)

            {

                backGround.Left -= Background_Speed; // background will move left/backwards

                MovingGame("left");

            }

            // if jumping is true and force is less than 0

            // then change jumping to false

            if (jump && force < 0)

            {

                jump = false;

            }

            // if jumping is true

            // then change jump speed to -12

            // reduce force by 1

            if (jump)

            {

                Speedjump = -12;

                force -= 1;

            }

            else

            {

                // else change the jump speed to 12

                Speedjump = 12;

            }

            // Check collision with platforms and coins

            foreach (Control x in Controls)

            {

                if (x is PictureBox && (string)x.Tag == "platform" && Bird.Bounds.IntersectsWith(x.Bounds))

                {

                    force = 9;

                    Bird.Top = x.Top - Bird.Height;

                    Speedjump = 0;

                }

            }

            foreach (Control x in this.Controls) // interaction between coin and platform

            {

                if (x is PictureBox && (string)x.Tag == "platform")

                {

                    if (Bird.Bounds.IntersectsWith(x.Bounds) && jump == false)

                    {

                        force = 9;

                        Bird.Top = x.Top - Bird.Height;

                        Speedjump = 0;

                    }

                    x.BringToFront();

                }

                if (x is PictureBox && (string)x.Tag == "Coin")

                {

                    // if the player collides with the coin picture box

                    if (Bird.Bounds.IntersectsWith(x.Bounds))

                    {
                        // Play the treasure collected sound effect
                        audioManager.PlayCoinCollectedSound();

                        this.Controls.Remove(x); // then I will remove the coin image

                        CoinsCount++; // add 1 to the CoinsCount

                    }

                }

            }

            // if the player collides with the treasure picture box

            if (Bird.Bounds.IntersectsWith(Treasure.Bounds))

            {

                if (CoinsCount >= 10)// and has collected 15 coins or more

                {
                    // Play the treasure collected sound effect
                    audioManager.PlayTreasureSound();

                    // then we remove the key from the game

                    this.Controls.Remove(Treasure);

                    // change the has key boolean to true

                    HasTreasure = true;

                }

                else

                {

                    HasTreasure = false;

                }

            }

            // if the player collides with the door and has Treasure boolean is true

            if (Bird.Bounds.IntersectsWith(portalClose.Bounds) && HasTreasure)

            {

                portalClose.Image = Properties.Resources.portal_open; // switch the closed portal to open portal

                // we stop the timer

                GameTimer.Stop();

                MessageBox.Show("You Completed the level!!" + Environment.NewLine + "Click 'OK' to play again"); //show the message box

                RestartGAME();

            }



        }

        // Method to move the bird
        private void Bird_Move()
        {
            if (Go_left && Bird.Left > 60)
            {
                Bird.Left -= SpeedBird;
                Bird.Image = Properties.Resources.sprite_image__left_;
            }
            if (Go_Right && Bird.Left + (Bird.Width + 60) < this.ClientSize.Width)
            {
                Bird.Left += SpeedBird;
                Bird.Image = Properties.Resources.sprite_image__right_;
            }
        }

        // Event handler for key down event
        private void KeyDOWN(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                Go_left = true;
            }
            if (e.KeyCode == Keys.D)
            {
                Go_Right = true;
            }
            if (e.KeyCode == Keys.Space && !jump)
            {
                audioManager.PlayCharacterJump(); // playe jump sound effect
                jump = true;
            }
            if (e.KeyCode == Keys.Space && !jump && HasTreasure)
            {
                HasTreasure = false; // Use the treasure to defeat the enemy
            }
        }

        // Event handler for key up event
        private void KeyUP(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                Go_left = false;
            }
            if (e.KeyCode == Keys.D)
            {
                Go_Right = false;
            }
            if (jump)
            {
                jump = false;
            }
        }

        private void MovingGame(string Direction) // Method for moving game which is going to run inside the timer, so the direction the Bird is moving towards, I can move the elements with it so when the background is moving in the opposite direction it creates a continuous moving background

        {

            foreach (Control x in this.Controls)

            {

                if (x is PictureBox && ((string)x.Tag == "platform" || (string)x.Tag == "Coin" || (string)x.Tag == "Pipe" || (string)x.Tag == "Treasure" || (string)x.Tag == "Portal")) // if it's a picture box AND it has a Tag called either Platform or coin or pipe or Treasure or Portal

                {

                    if (Direction == "left")

                    {

                        x.Left -= Background_Speed; // all of the elements will move with the background speed

                    }

                    if (Direction == "right")

                    {

                        x.Left += Background_Speed;

                    }

                }

            }

        }



        // Event handler for form closed event
        private void CloseGame(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        // Method to handle game over
        private void GameOver(string message)
        {
            GameTimer.Stop();
            MessageBox.Show(message + " Game Over!\nClick 'OK' to play again");
            RestartGAME();
        }

        // Method to restart the game
        private void RestartGAME()
        {
            this.KeyDown -= new KeyEventHandler(KeyDOWN);
            this.KeyUp -= new KeyEventHandler(KeyUP);
            GameTimer.Stop();
            // Reset game variables
            Bird.Location = new Point(100, 100);
            CoinsCount = 0;
            HasTreasure = false;
            jump = false;
            Go_Right = false;
            Go_left = false;
            force = 9;
            Speedjump = 10;


            // Reset enemy position
            enemy.Location = new Point(827, 433);



            // Reload high score after reset
            Value_label.Text = Properties.Settings.Default.High_Score;
            Form1 newWindow = new Form1();
            newWindow.Show();
            this.Hide();


            // Start playing the background music again after restarting the game
            // audioManager.PlayBackgroundMusic();
        }

        // Method to update the game
        private void Game_Update()
        {
            // Check for collision between the bird and the enemy
            if (enemy.Bounds.IntersectsWith(Bird.Bounds))
            {
                if (HasTreasure)
                {
                    // Bird defeats the enemy if it has the treasure
                    enemyAlive = false;
                    Controls.Remove(enemy);
                }
                else
                {
                    // Bird loses life if it intersects with the enemy
                    life--;
                    Life_index();
                    Bird.Top = 50; // Move bird back to initial position
                    if (life == 0)
                    {
                        GameOver("You were caught by the enemy!");
                    }
                }



            }

            // Check if the Bird falls out of the screen
            if (Bird.Top + Bird.Height > this.ClientSize.Height)
            {
                life--;
                Life_index();
                Bird.Top = 50; // Move bird back to initial position
                if (life == 0)
                {
                    GameOver("You fell out of the screen!");
                }
            }

            // Stop the background music when the game is over
            //audioManager.StopBackgroundMusic();



        }


        // Method to move the enemy
        private void Enemy_move()
        {
            // Move the enemy towards the bird
            enemy.Top += 6;
            if (enemy.Top > 400)
            {
                Score++;
                TXTscore.Text = "Score : " + Score;
                x = rnd.Next(0, 300);
                enemy.Location = new Point(x, 0);
            }

        }

        // Method to update life UI
        private void Life_index()
        {
            if (life == 2)
            {
                life1.Image = Properties.Resources.life_white;
            }
            if (life == 1)
            {
                life2.Image = Properties.Resources.life_white;
            }
            if (life == 0)
            {
                life3.Image = Properties.Resources.life_white;
                level_over.Show();
                GameTimer.Stop();
                int a = Int32.Parse(Value_label.Text);
                if (Score > a)
                {
                    Value_label.Text = Score.ToString();
                    Properties.Settings.Default.High_Score = Value_label.Text;
                    Properties.Settings.Default.Save();
                }
            }
        }

        // Other event handlers...

        private void Bird_Click(object sender, EventArgs e)
        {
            // Handle bird click event
        }

        private void Treasure_Click(object sender, EventArgs e)
        {
            // Handle treasure click event
        }
    }

    public class AudioManager
    {
        private SoundPlayer treasureSound;
        //private SoundPlayer BackgroundMusic;
        private SoundPlayer CoinCollectedSound;
        //private SoundPlayer BirdJump;

        public AudioManager()
        {
            // Initialize my sound effects
            treasureSound = new SoundPlayer("treasure-collected-sfx.wav"); //  the path to my treasure sound file
          //  BackgroundMusic = new SoundPlayer("background-music-loop.mp3");
            CoinCollectedSound = new SoundPlayer("coin-collected-sfx.wav");
           // BirdJump = new SoundPlayer("character-jump-sfx.flac");
        }

        public void PlayTreasureSound()
        {
            // Play the jump sound effect
            treasureSound.Play();
        }

        public void PlayBackgroundMusic()
        {
            // Play the background music in a loop
           // BackgroundMusic.PlayLooping();
        }

        public void StopBackgroundMusic()
        {
            // Stop the background music
            //BackgroundMusic.Stop();
        }

        public void PlayCoinCollectedSound()
        {
            CoinCollectedSound.Play();
        }

        public void PlayCharacterJump()
        {
           // BirdJump.Play();
        }

    }




}









































































