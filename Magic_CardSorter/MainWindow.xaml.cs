using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Magic_CardSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);




        const string IMAGESURL = "http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=";
        const string IMAGESURL2 = "&type=card";
        const string FOLDER = "./ManaCost";

        dynamic loadedCards;
        private dynamic actualSet;

        BitmapImage manaRed;
        BitmapImage manaBlue;
        BitmapImage manaGreen;
        BitmapImage manaBlack;
        BitmapImage manaWhite;


        public MainWindow()
        {
            InitializeComponent();
            loadJSON();
        }
        public async void loadJSON()
        {
            loadedCards = "";

            await loadFile();


            foreach (var item in loadedCards)
            {
                TextBlock myTextBlock = new TextBlock();
                myTextBlock.TextWrapping = TextWrapping.Wrap;
                myTextBlock.Margin = new Thickness(0, 0, 0, 20);
                myTextBlock.Text = item.Value.name;
                SetsContent.Items.Add(myTextBlock);
            }
        }
        async Task loadFile()
        {
            string tet = "";
            manaWhite = new BitmapImage();
            manaBlue = new BitmapImage();
            manaRed = new BitmapImage();
            manaGreen = new BitmapImage();
            manaBlack = new BitmapImage();

            await Task.Run(() =>
            {

                string JSON = "";
                try
                {
                    using (var streamReader = new StreamReader("./AllSets.json", Encoding.UTF8))
                    {
                        JSON = streamReader.ReadToEnd();
                    }

                }
                catch (Exception)
                {
                }

                try
                {
                    using (var streamReader = new FileStream(FOLDER+"/Mana_R.png", FileMode.Open, FileAccess.Read))
                    {
                        MemoryStream ms = new MemoryStream();
                        ms.SetLength(streamReader.Length);
                        streamReader.Read(ms.GetBuffer(), 0, (int)streamReader.Length);

                        ms.Flush();

                        Dispatcher.Invoke(() =>
                        {
                            manaRed.BeginInit();
                            manaRed.StreamSource = ms;
                            manaRed.EndInit();
                        });
                    }

                    using (var streamReader = new FileStream(FOLDER+"./Mana_U.png", FileMode.Open, FileAccess.Read))
                    {

                        MemoryStream ms = new MemoryStream();
                        ms.SetLength(streamReader.Length);
                        streamReader.Read(ms.GetBuffer(), 0, (int)streamReader.Length);

                        ms.Flush();
                        Dispatcher.Invoke(() =>
                        {
                            manaBlue.BeginInit();
                            manaBlue.StreamSource = ms;
                            manaBlue.EndInit();
                        });
                    }
                    using (var streamReader = new FileStream(FOLDER + "./Mana_G.png", FileMode.Open, FileAccess.Read))
                    {

                        MemoryStream ms = new MemoryStream();
                        ms.SetLength(streamReader.Length);
                        streamReader.Read(ms.GetBuffer(), 0, (int)streamReader.Length);

                        ms.Flush();
                        Dispatcher.Invoke(() =>
                        {
                            manaGreen.BeginInit();
                            manaGreen.StreamSource = ms;
                            manaGreen.EndInit();
                        });
                    }
                    using (var streamReader = new FileStream(FOLDER + "./Mana_B.png", FileMode.Open, FileAccess.Read))
                    {

                        MemoryStream ms = new MemoryStream();
                        ms.SetLength(streamReader.Length);
                        streamReader.Read(ms.GetBuffer(), 0, (int)streamReader.Length);

                        ms.Flush();
                        Dispatcher.Invoke(() =>
                        {
                            manaBlack.BeginInit();
                            manaBlack.StreamSource = ms;
                            manaBlack.EndInit();
                        });
                    }
                    using (var streamReader = new FileStream(FOLDER + "./Mana_W.png", FileMode.Open, FileAccess.Read))
                    {

                        MemoryStream ms = new MemoryStream();
                        ms.SetLength(streamReader.Length);
                        streamReader.Read(ms.GetBuffer(), 0, (int)streamReader.Length);

                        ms.Flush();
                        Dispatcher.Invoke(() =>
                        {
                            manaWhite.BeginInit();
                            manaWhite.StreamSource = ms;
                            manaWhite.EndInit();
                        });
                    }

                }
                catch (Exception ex)
                {
                    tet = ex.Message;
                }
                loadedCards = JsonConvert.DeserializeObject(JSON);

            });
            if (tet != "")
            {
                MessageBox.Show(tet); 
            }
        }

        private void SetsContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadCardsFromSet();
        }
        void loadCardsFromSet()
        {
            CardsContent.Items.Clear();
            int index = SetsContent.SelectedIndex;
            dynamic cardsFromSet = "";


            foreach (var item in loadedCards)
            {
                if (item.Value.name == ((TextBlock)SetsContent.Items.GetItemAt(index)).Text)
                {
                    foreach (var cards in item.Value.cards)
                    {

                        TextBlock myTextBlock = new TextBlock();
                        myTextBlock.TextWrapping = TextWrapping.Wrap;
                        myTextBlock.Margin = new Thickness(0, 0, 0, 20);
                        myTextBlock.Text = cards.name;
                        CardsContent.Items.Add(myTextBlock);
                        // Add child elements to the parent StackPanel

                        actualSet = item.Name;
                    }
                }
            }
        }

        private void CardsContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadCardData();
        }

        private void loadCardData()
        {

            if (CardsContent.SelectedIndex >= 0)
            {
                contentCards.Children.Clear();
                StringBuilder sb = new StringBuilder();
                StackPanel manaCost = new StackPanel();
                TextBlock content = new TextBlock();
                dynamic currentSet = loadedCards[actualSet]["cards"][(CardsContent.SelectedIndex)];
                BitmapImage temp = getImage((string)currentSet.name, (string)currentSet.multiverseid);
                
                if (temp != null)
                {
                    Image cardIm = new Image();
                    cardIm.Source = temp;
                    contentCards.Children.Add(cardIm);

                }
                contentCards.Children.Add(manaCost);
                contentCards.Children.Add(content);
                manaCost.Orientation = Orientation.Horizontal;
                int tempWidth = 0;
                if (currentSet.cmc!=null)
                {
                    tempWidth = 40;
                }
                if (currentSet.manaCost!=null)
                {
                    foreach (char t in ((string)currentSet.manaCost))
                    {
                        switch (t)
                        {
                            case '1':
                                
                            case '2':
                                
                            case '3':
                              
                            case '4':
                             
                            case '5':
                             
                            case '6':
                              
                            case '7':
                               
                            case '8':
                               
                            case '9':

                            case '0':
                                TextBlock textB = new TextBlock();
                                textB.Text = Convert.ToString(t);                                
                                textB.FontSize = tempWidth;
                                manaCost.Children.Add(textB);
                                break;
                        }
                        if (t == 'X')
                        {
                            TextBlock textB = new TextBlock();
                            textB.Text = Convert.ToString(t);
                            manaCost.Children.Add(textB);

                        }
                        if (t == 'G')
                        {
                            Image manaGreenIm = new Image();
                            manaGreenIm.Source = manaGreen;
                            manaGreenIm.Width = tempWidth;
                            manaCost.Children.Add(manaGreenIm);
                        }
                        if (t == 'U')
                        {
                            Image manaBlueIm = new Image();

                            manaBlueIm.Source = manaBlue;
                            manaBlueIm.Width = tempWidth;
                            manaCost.Children.Add(manaBlueIm);
                        }
                        if (t == 'B')
                        {
                            Image manaBlackIm = new Image();

                            manaBlackIm.Source = manaBlack;
                            manaBlackIm.Width = tempWidth;
                            manaCost.Children.Add(manaBlackIm);
                        }
                        if (t == 'W')
                        {
                            Image manaWhiteIm = new Image();


                            manaWhiteIm.Source = manaWhite;
                            manaWhiteIm.Width = tempWidth;
                            manaCost.Children.Add(manaWhiteIm);
                        }
                        if (t == 'R')
                        {
                            Image manaRedIm = new Image();

                            manaRedIm.Source = manaRed;
                            manaRedIm.Width = tempWidth;
                            manaCost.Children.Add(manaRedIm);
                        }
                    } 
                }

                if (currentSet.name!=null)
                {
                    sb.AppendLine("Name: " + currentSet.name);

                }                if (currentSet.artist!=null)
                {
                    sb.AppendLine("Artist: " + currentSet.artist);

                }                if (currentSet.text!=null)
                {
                    sb.AppendLine("Text: " + ((string)currentSet.text).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", ""));

                }                content.TextWrapping = TextWrapping.Wrap;
                content.Text = sb.ToString();

                

            }
        }
        bool existPicture(string id)
        {
            bool exists = false;
            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(IMAGESURL + id + IMAGESURL2);
            request.Timeout = 5000;
            request.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK) return true;
                else return false;
            }
            catch (WebException)
            {
                /* A WebException will be thrown if the status of the response is not `200 OK` */
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            }
            return exists;

        }
        BitmapImage getImage(string name, string id)
        {
            BitmapImage b = new BitmapImage();
            if (File.Exists("c:/temp/" + name + ".png"))
            {
                b.BeginInit();
                b.UriSource = new Uri("c:/temp/" + name + ".png");
                b.EndInit();
                return b;
            }
            if (IsConnectedToInternet())
            {

                if (existPicture(id))
                {
                    try
                    {

                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(IMAGESURL + id + IMAGESURL2, "c:/temp/" + name + ".png");
                        }
                        b.BeginInit();
                        b.UriSource = new Uri("c:/temp/" + name + ".png");
                        b.EndInit();

                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }

            return b;
        }
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

    }

}



