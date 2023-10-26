using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using System.Timers;


namespace WpfGameSandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        DispatcherTimer Timer = new DispatcherTimer();

        bool goLeft, goRight, goUp, goDown, isVisible;

        int ShipTop = 290;
        int ShipLeft = 350;

        int ParcelTop = 0;
        int ParcelLeft = 200;

        int SwordTop = 0;
        int SwordLeft = 200;

        int playerHealth = 1;

        int output1 = 0; // Output För Antal Boxar
        int output2 = 3; // Output För Life
        int OutputSwordSpeed = 0; // Output För Att Göra Svärdet Snabbare Vid Mer Liv
        public MainWindow()
        {
            InitializeComponent();

            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Start();
            LifeBar.Width = 0;
            StartingScreen(); //Du kan starta när du själv är redo!
        }

        
        private void WpfGameSandbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true; goRight = false;
                goUp = false; goDown = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = true; goLeft = false;
                goUp = false; goDown = false;
            }
            if (e.Key == Key.Down)
            {
                goLeft = false; goRight = false; goUp = false;
                goDown = true;
            }
            if (e.Key == Key.Up)
            {
                goLeft = false; goRight = false; goDown = false;
                goUp = true;
            }
            if (e.Key == Key.Space)
            {
                goLeft = false; goRight = false; goDown = false; goUp = false;
                isVisible = !isVisible;
                Ship.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Collision(); //Kollidering funktionen
            Ending(); //Ending, ser till att spelet kan stanna!
            lifebar(); // Styr Lifebar!
            ship(); // Styr ship, dess movement!
            Parcalmove(); // Funktionen som styr Parcal
            SwordMove(); // Funktionen som styr svärdet!
        }

        //Funktior vid kollidering!
        private void Collision()
        {

            Rect ShipRect = new Rect(Canvas.GetLeft(Ship), Canvas.GetTop(Ship), Ship.Width, Ship.Height);

            Rect ParcelRect = new Rect(Canvas.GetLeft(Parcel), Canvas.GetTop(Parcel), Parcel.Width, Parcel.Height);

            Rect SwordRect = new Rect(Canvas.GetLeft(Sword), Canvas.GetTop(Sword), Sword.Width, Sword.Height);

            if (ShipRect.IntersectsWith(ParcelRect))
            {
                playerHealth += 20; // Öka life med 10 om ship kolliderar med  parcels
                if (playerHealth > 200)
                {
                    playerHealth = 200; //Stanna på 200 ej mer
                }
                ParcelTop = -60;
                ParcelLeft = random.Next(0, 730);
                output1 += 1; // Lägg till 1 in "Antal boxar" när du kolliderar
            }
            if (ShipRect.IntersectsWith(SwordRect))
            {
                SwordTop = -60;
                SwordLeft = random.Next(0, 730);
                output2 -= 1; //Vid kollidering ta bort 1 "life"
                playerHealth = 0;
            }
            LifeBar.Width = playerHealth; // uppdaterar lifebar baserad på spelarens liv!
            Antaltxt.Text = output1.ToString();
        }

        private void lifebar()
        {
            if (LifeBar.Width < 200) 
            {
                LifeBar.Width += 1;
            }
            if (LifeBar.Width == 200)
            {
                playerHealth = 0;
                output2 += 1; // Om lifebar blir 200, sätt lifebar till 0 och ge 1 "life"
            }
            if (LifeBar.Width == 0)
            {
                playerHealth = 0;
                output2 -= 1;
            }

            TxLife.Text = output2.ToString();
        }

        private void restartbtn(object sender, RoutedEventArgs e)
        {
            output1 = 0; output2 = 3;
            Timer.Start();
        }

        private void ship()
        {
            //Rörelse händelser för Ship!
            //Gå vänster & Höger
            if (goLeft && ShipLeft > 0)
            {

                Canvas.SetLeft(Ship, ShipLeft -= 3);


            }
            else if (goRight && ShipLeft < 732)
            {
                Canvas.SetLeft(Ship, ShipLeft += 3);
            }
            //Gå uppåt
            if (goUp && ShipTop > 0)
            {
                Canvas.SetTop(Ship, ShipTop -= 3);
            }
            //Gå neråt
            if (goDown)
            {
                Canvas.SetTop(Ship, ShipTop += 3);
            }

        }

        private void Parcalmove()
        {
            //Parcel Top funktion, random x position går neråt
            ParcelTop += 3;
            if (ParcelTop > 400)
            {
                ParcelLeft = random.Next(0, 730); // random "x" kordinater 
                ParcelTop = -50;
            }
            Canvas.SetTop(Parcel, ParcelTop);
            Canvas.SetLeft(Parcel, ParcelLeft);

        }

        private void StartingBtn(object sender, RoutedEventArgs e)
        {
            StartBtn.Visibility = Visibility.Collapsed; //Knappen göms när timern startas
            Starting(); //Starta Spelet när knappen trycks
        }

        private void SwordMove()
        {
            //Sword Top funktion, random x position går neråt
            SwordTop += 5 + OutputSwordSpeed;
            if (SwordTop > 400)
            {
                SwordLeft = random.Next(0, 730);
                SwordTop = -50;
            }
            Canvas.SetTop(Sword, SwordTop);
            Canvas.SetLeft(Sword, SwordLeft);
            SwordSpeed();
        }

        //Ska stanna spelet om livet går ner på 0!


        private void Ending()
        {
            if (output2 == 0)
            {
                MessageBox.Show("Game Over"); //Gameover message box
                Timer.Stop();
                RestartBtn.Visibility = Visibility.Visible; //RestartBtn är synlig när timern har stannat
            }
            else
            {
                RestartBtn.Visibility = Visibility.Collapsed; //gömd när timern är på!
            }
        }

        private void StartingScreen()
        {
            //Om timern startas på så stängs av
            if (Timer.IsEnabled)
            {
                Timer.Stop();
            }
            // Knapparna, boxarna och sprites gömda till timern startas!
            RestartBtn.Visibility = Visibility.Collapsed;
                lifebarlabel.Visibility = Visibility.Collapsed;
                TxLife.Visibility = Visibility.Collapsed;
                Lifetxt.Visibility = Visibility.Collapsed;
                Antaltxt.Visibility = Visibility.Collapsed;
                Timertxt.Visibility = Visibility.Collapsed;
                StartBtn.Visibility = Visibility.Visible;
                Sword.Visibility = Visibility.Collapsed;
                Parcel.Visibility = Visibility.Collapsed;
        }
        
        private void Starting()
        {
            //När timern startas ska allt bli visas igen och spelet kan starta!
            RestartBtn.Visibility = Visibility.Visible;
            lifebarlabel.Visibility = Visibility.Visible;
            TxLife.Visibility = Visibility.Visible;
            Lifetxt.Visibility = Visibility.Visible;
            Antaltxt.Visibility = Visibility.Visible;
            Timertxt.Visibility = Visibility.Visible;
            StartBtn.Visibility = Visibility.Collapsed;
            Sword.Visibility = Visibility.Visible;
            Parcel.Visibility = Visibility.Visible;
            Timer.Start();
        }

        private void SwordSpeed()
        {
            if (output2 < 3)
            {
                OutputSwordSpeed = 1;
            }
            else if (output2 >= 3)
            {
                OutputSwordSpeed = (int)(output2 / 3); // Öka med tiden
            }
        }
        
    }
}
