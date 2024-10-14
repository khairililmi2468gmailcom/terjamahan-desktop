using Avalonia.Controls;
using Avalonia.Input;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace terjemahan_desktop
{
    public partial class MainWindow : Window
    {
        private bool isBahasaIndonesiaActive = true; // Menyimpan status bahasa aktif
        private readonly HttpClient httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;

            inputTextBox.TextChanged += InputTextBox_TextChanged; // Event untuk perubahan teks
        }

        private async void InputTextBox_TextChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Ambil teks dari input
            string inputText = inputTextBox.Text;

            if (!string.IsNullOrWhiteSpace(inputText))
            {
                // Panggil fungsi untuk menerjemahkan teks
                string translatedText = await TranslateTextAsync(inputText);
                outputTextBox.Text = translatedText; // Tampilkan hasil terjemahan
            }
            else
            {
                outputTextBox.Text = string.Empty; // Kosongkan output jika input kosong
            }
        }

        private async Task<string> TranslateTextAsync(string text)
        {
            string url = isBahasaIndonesiaActive ? "http://localhost:5001/translate-indo-gayo" : "http://localhost:5001/translate-gayo-indo";

            var requestData = new { text = text }; // Membuat objek permintaan
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Pastikan respon sukses

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var translationResponse = JsonConvert.DeserializeObject<TranslationResponse>(jsonResponse);

                return translationResponse.Translation; // Mengembalikan terjemahan
            }
            catch (HttpRequestException ex)
            {
                // Tangani kesalahan jika ada (misalnya, koneksi gagal)
                return "Error: " + ex.Message;
            }
        }

        // Event handler untuk pertukaran bahasa
        private void SwitchLanguages(object sender, PointerPressedEventArgs e)
        {
            if (isBahasaIndonesiaActive)
            {
                textBahasaIndonesia.Text = "Bahasa Gayo";
                textBahasaGayo.Text = "Bahasa Indonesia";
            }
            else
            {
                textBahasaIndonesia.Text = "Bahasa Indonesia";
                textBahasaGayo.Text = "Bahasa Gayo";
            }

            isBahasaIndonesiaActive = !isBahasaIndonesiaActive;
        }

        // Kelas untuk deserialisasi respon
        private class TranslationResponse
        {
            public string? Translation { get; set; }
        }
    }
}
