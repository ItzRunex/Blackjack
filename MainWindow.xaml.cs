using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BlackjackGame
{
    public partial class MainWindow : Window
    {
        private Player player = new Player();
        private Deck deck = new Deck();
        private Card hidden; //A call for the hidden dealer card.
        private static int dealerScore;
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Bet(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name.Equals("b50"))
                player.BetAmt = 50;
            else if (button.Name.Equals("b100"))
                player.BetAmt = 100;
            else if (button.Name.Equals("b250"))
                player.BetAmt = 250;
            else if (button.Name.Equals("b500"))
                player.BetAmt = 500;
            player.Bet();
            UpdateStats();
            HideAnnouncer();
            DisableBets();
            await SpreadCards();
        }
        public async Task SpreadCards()
        {
            ShowScore(false);
            Card pc1 = deck.PickCard();
            Card pc2 = deck.PickCard();
            Card dc1 = deck.PickCard();
            Card dc2 = deck.PickCard();
            dealerScore = dc1.Value + dc2.Value;
            player.Score = pc1.Value;
            PCard1.Source = Card.CardToImage(pc1);
            UpdateScore();
            await Task.Delay(500);
            player.Score += pc2.Value;
            UpdateScore();
            PCard2.Source = Card.CardToImage(pc2);
            await Task.Delay(500);
            DCard1.Source = Card.DealerCardToImage(dc1); //This is the initially hidden dealer card.
            hidden = new Card(dc1.Name, dc1.Suit, dc1.Value); //We need to save it, because we'll need it in another method.
            await Task.Delay(500);
            DCard2.Source = Card.CardToImage(dc2);
            EnableActions();
            //Check if player blackjack or if dealer blackjack, show the dealer card and end the game. Else enable the action buttons.
            if (player.Score == 21 && dealerScore == 21)
            {
                announcer.Text = $"Push.{Environment.NewLine}+$0";
                await Reset();
            }
            else if (player.Score == 21)
            {
                announcer.Text = $"Blackjack! You win!{Environment.NewLine}+${player.Blackjack()}";
                await Reset();
            }
            else if (dealerScore == 21)
            {
                announcer.Text = $"Dealer blackjack! You lose.{Environment.NewLine}-${player.BetAmt}";
                await Reset();
            }
        }
        private async void Hit(object sender, RoutedEventArgs e)
        {
            Card nextCard = deck.PickCard();
            player.Score += nextCard.Value;
            switch (player.CardCount)
            {
                case 2:
                    PCard3.Source = Card.CardToImage(nextCard);
                    break;
                case 3:
                    PCard4.Source = Card.CardToImage(nextCard);
                    break;
                case 4:
                    PCard5.Source = Card.CardToImage(nextCard);
                    break;
                case 5:
                    PCard6.Source = Card.CardToImage(nextCard);
                    break;
                case 6:
                    PCard7.Source = Card.CardToImage(nextCard);
                    break;
                case 7:
                    PCard8.Source = Card.CardToImage(nextCard);
                    break;
                case 8:
                    PCard9.Source = Card.CardToImage(nextCard);
                    break;
            }
            UpdateScore();
            player.CardCount++;
            //If player blackjacks or player busts, show the dealer card and end the game. Else re-enable the action buttons.
            if (player.Score == 21 && player.Score > dealerScore)
            {
                announcer.Text = $"Blackjack! You win!{Environment.NewLine}+${player.Blackjack() - player.BetAmt}";
                await Reset();
            }
            else if (player.Score == 21 && player.Score == dealerScore)
            {
                announcer.Text = $"Push.{Environment.NewLine}+$0";
                await Reset();
            }
            else if (player.Score > 21)
            {
                announcer.Text = $"Bust! You lose.{Environment.NewLine}-${player.BetAmt}";
                await Reset();
            }
        }
        private async void Stand(object sender, RoutedEventArgs e)
        {
            await Stand();
        }
        private async Task Stand()
        {
            DisableActions();
            DCard1.Source = Card.CardToImage(hidden);
            ShowScore(true);
            int cardCount = 2;
            while (dealerScore < 17)
            {
                //Automated hit algorithm.
                Card nextCard = deck.PickCard();
                dealerScore += nextCard.Value;
                await Task.Delay(500);
                switch (cardCount)
                {
                    case 2:
                        DCard3.Source = Card.CardToImage(nextCard);
                        break;
                    case 3:
                        DCard4.Source = Card.CardToImage(nextCard);
                        break;
                    case 4:
                        DCard5.Source = Card.CardToImage(nextCard);
                        break;
                    case 5:
                        DCard6.Source = Card.CardToImage(nextCard);
                        break;
                    case 6:
                        DCard7.Source = Card.CardToImage(nextCard);
                        break;
                    case 7:
                        DCard8.Source = Card.CardToImage(nextCard);
                        break;
                    case 8:
                        DCard9.Source = Card.CardToImage(nextCard);
                        break;
                }
                cardCount++;
                UpdateScore();
            }
            //If score isn't blackjack for any sides, or if the player hasn't gone bust from hitting, one of the following will apply:
            if (dealerScore > 21)
                announcer.Text = $"Dealer bust! You win!{Environment.NewLine}+${player.Win() - player.BetAmt}";
            else if (dealerScore > player.Score)
                announcer.Text = $"Dealer wins.{Environment.NewLine}-${player.BetAmt}";
            else if (dealerScore < player.Score)
                announcer.Text = $"You win!{Environment.NewLine}+${player.Win() - player.BetAmt}";
            else
                announcer.Text = $"Push.{Environment.NewLine}+$0";
            await Reset();
        }
        private async void Double(object sender, RoutedEventArgs e)
        {
            DisableActions();
            player.Double();
            betDisplay.Text = "Bet: $" + player.BetAmt;
            Hit(this, null);
            if (player.Score != dealerScore && (player.Score == 21 || dealerScore == 21))
            {
                if (player.Score == 21)
                {
                    announcer.Text = $"Blackjack! You win!{Environment.NewLine}+${player.Blackjack() - player.BetAmt}";
                    await Reset();
                }
                else if (dealerScore == 21)
                {
                    announcer.Text = $"Dealer blackjack! You lose.{Environment.NewLine}-${player.BetAmt}";
                    await Reset();
                }
            }
            else
                await Stand();
        }
        private async Task Reset()
        {
            UpdateStats();
            ShowAnnouncer();
            DisableActions();
            DCard1.Source = Card.CardToImage(hidden);
            ShowScore(true);
            await Task.Delay(3000);
            HideScore();
            PCard1.Source = null;
            PCard2.Source = null;
            PCard3.Source = null;
            PCard4.Source = null;
            PCard5.Source = null;
            PCard6.Source = null;
            PCard7.Source = null;
            PCard8.Source = null;
            DCard1.Source = null;
            DCard2.Source = null;
            DCard3.Source = null;
            DCard4.Source = null;
            DCard5.Source = null;
            DCard6.Source = null;
            DCard7.Source = null;
            DCard8.Source = null;
            player.BetAmt = 0;
            player.Score = 0;
            player.CardCount = 2;
            deck = new Deck();
            dealerScore = 0;
            UpdateStats();
            if (player.IsBroke())
            {
                announcer.Text = $"You're in{Environment.NewLine}debt!";
                await Task.Delay(5000);
                Application.Current.Shutdown();
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            }
            else
            {
                announcer.Text = "Place your bet!";
                EnableBets();
            }
        }
        private void UpdateStats()
        {
            betDisplay.Text = "Bet: $" + player.BetAmt;
            balDisplay.Text = "Balance: $" + player.Balance;
        }
        private void UpdateScore()
        {
            PScore.Text = Convert.ToString(player.Score);
            DScore.Text = Convert.ToString(dealerScore);
        }
        private void ShowScore(bool dealer)
        {
            PDisplay.Visibility = Visibility.Visible;
            PScore.Visibility = Visibility.Visible;
            if (dealer)
            {
                DDisplay.Visibility = Visibility.Visible;
                DScore.Visibility = Visibility.Visible;
            }
        }
        private void HideScore()
        {
            PDisplay.Visibility = Visibility.Collapsed;
            PScore.Visibility = Visibility.Collapsed;
            DDisplay.Visibility = Visibility.Collapsed;
            DScore.Visibility = Visibility.Collapsed;
        }
        private void ShowAnnouncer()
        {
            announcer.Visibility = Visibility.Visible;
        }
        private void HideAnnouncer()
        {
            announcer.Visibility = Visibility.Hidden;
        }
        private void EnableBets()
        {
            b50.IsEnabled = true;
            b100.IsEnabled = true;
            b250.IsEnabled = true;
            b500.IsEnabled = true;
        }
        private void DisableBets()
        {
            b50.IsEnabled = false;
            b100.IsEnabled = false;
            b250.IsEnabled = false;
            b500.IsEnabled = false;
        }
        private void EnableActions()
        {
            hit.IsEnabled = true;
            stand.IsEnabled = true;
            dble.IsEnabled = true;
        }
        private void DisableActions()
        {
            hit.IsEnabled = false;
            stand.IsEnabled = false;
            dble.IsEnabled = false;
        }
    }
    public class Card
    {
        private string name;
        private string suit;
        private int value;
        public Card(string name, string suit, int value)
        {
            this.name = name;
            this.suit = suit;
            this.value = value;
        }
        public string Name
        {
            get { return name; }
        }
        public string Suit
        {
            get { return suit; }
        }
        public int Value
        {
            get { return value; }
        }
        //Replaces a placeholder string path with the card suit and name
        //and returns the image source of the card.
        public static BitmapImage CardToImage(Card card)
        {
            string src = @"Assets\Cards\REPLACEPLZ.png";
            src = src.Replace("REPLACEPLZ", $@"{card.Suit}\{card.Name}");
            BitmapImage source = new BitmapImage(new Uri(src, UriKind.Relative));
            return source;
        }
        //Works in the same way as ChooseCard(), but the card is hidden and instead an if 
        //determines whether the back of the card will be black or red.
        public static BitmapImage DealerCardToImage(Card card)
        {
            string src = @"Assets\Cards\REPLACEPLZ.png";
            if (card.Suit.Equals("Clubs") || card.Suit.Equals("Spades"))
                src = src.Replace("REPLACEPLZ", "Black");
            else
                src = src.Replace("REPLACEPLZ", "Red");
            BitmapImage source = new BitmapImage(new Uri(src, UriKind.Relative));
            return source;
        }
    }
    public class Deck
    {
        private List<Card> deck = new List<Card>();
        private static readonly string[] name = { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" }; //min 0 / max 12
        private static readonly string[] suit = { "Clubs", "Diamonds", "Hearts", "Spades" };
        public Deck() //Indices 0 - 12 are Clubs, 13 - 25 are Diamonds, 26 - 38 are Hearts and 39 - 51 are Spades.
        {
            for (int sI = 0; sI < 4; sI++) //Loop for setting the card suit index (sI).
                for (int nI = 0; nI <= 12; nI++) //Loop for setting the card name index (nI).
                    if (nI == 0)
                        deck.Add(new Card(name[nI], suit[sI], 11)); //We set a special case value of 11 to the Ace card.
                    else if (nI >= 10)
                        deck.Add(new Card(name[nI], suit[sI], 10)); //We also set a special case value of 10 to the J, Q, K cards.
                    else
                        deck.Add(new Card(name[nI], suit[sI], nI + 1));
        }
        public Card PickCard() //returns a random Card from index 0 - 51 and removes it from the deck object
        {
            Random index = new Random();
            Card card = deck[index.Next(0, deck.Count)];
            deck.Remove(card);
            return card; 
        }
    }
    public class Player
    {
        private int balance;
        private int bet;
        private int score;
        private int cardCount;
        public Player()
        {
            balance = 2500;
            bet = 0;
            score = 0;
            cardCount = 2;
        }
        public int Balance
        {
            get { return balance; }
        }
        public int BetAmt
        {
            get { return bet; }
            set { bet = value; }
        }
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        public int CardCount
        {
            get { return cardCount; }
            set { cardCount = value; }
        }
        public void Bet()
        {
            balance -= BetAmt;
        }
        public void Double()
        {
            balance -= BetAmt;
            BetAmt = BetAmt * 2;
        }
        //The next three methods adjust the balance after each type of win and return the amount won.
        public int Blackjack()
        {
            balance += BetAmt * 3;
            return BetAmt * 3;
        }
        public int Win()
        {
            balance += BetAmt * 2;
            return BetAmt * 2;
        }
        public bool IsBroke()
        {
            if (balance < 50)
                return true;
            else
                return false;
        }
    }
}