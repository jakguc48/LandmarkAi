using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace LandmarkAi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg*.jpeg|All files(*.*)|*.*";//jeśli chcemy zaznaczyć jakie typy moga być wybrane w dialogu. Image files (*.png; *.jpg) - to jest tylko tekst natomiast filtr dizała po |
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }
        }

        private async void MakePredictionAsync(string fileName) //metode tworzymy jako asynchroniczną ponieważ czekamy na odpowiedź z serwera api
        {
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/cf674491-55b9-43ba-a3ab-bbd093447351/image?iterationId=20d6d309-0efb-484b-a528-7cf6176677e2"; //wartrość skopiowana ze strony
            string predictionKey = "2806aad94a054b46bdebed90e3672afd"; //wartość skopiowana ze strony 
            string contentType = "application/octet-stream";//wartość skopiowana ze strony 
            var file = File.ReadAllBytes(fileName); //przekazujemy plika jako tablice bajtów - typowe. 

            using (HttpClient client = new HttpClient()) //wykorzystujemy httpclient do nawiązania połączenia z api
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey); //dodajemy customowy header zgodnie z tym co mamy napisane na stronie api


                using (var content = new ByteArrayContent(file)) //tworzymy nowy plik na podstawie zdjęcia
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType); //przypisujemy zapisany już header - ten istnieje, nie musimy go dodawać jak poprzedniego
                    var response = await client.PostAsync(url, content); // otrzymujemy tutaj odpowiedź z serwera i czekamy na niego awaitem przekazując wymagane dane.
                }
            }

            
        }
    }
}
