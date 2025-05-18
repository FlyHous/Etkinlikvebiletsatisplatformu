using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace WindowsFormsApp10
{
    public partial class Form4 : Form
    {
        private string connectionString = "Data Source=etkinlikler.db;Version=3;";

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadEtkinlikler(); // Etkinlikleri yükle
        }

        // Etkinlikleri veritabanından alıp ListBox1'e yükleyen fonksiyon
        public void LoadEtkinlikler()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT EtkinlikAd, EtkinlikYeri, EtkinlikTarihi, EtkinlikSaati FROM Etkinlikler";
                SQLiteCommand command = new SQLiteCommand(selectQuery, connection);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    listBox1.Items.Clear();  // ListBox1'i temizle
                    while (reader.Read())
                    {
                        string etkinlik = $"{reader["EtkinlikAd"]} - {reader["EtkinlikYeri"]} " +
                                          $"Tarih: {Convert.ToDateTime(reader["EtkinlikTarihi"]).ToString("yyyy-MM-dd")} " +
                                          $"Saat: {reader["EtkinlikSaati"]}";
                        listBox1.Items.Add(etkinlik);  // Etkinlikleri ListBox1'e ekle
                    }
                }
            }
        }


        // ListBox1'deki etkinlik seçildiğinde, kullanıcıları ListBox2'ye yükler
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedEvent = listBox1.SelectedItem.ToString();
                string etkinlikAd = selectedEvent.Split('-')[0].Trim(); // Etkinlik adını ayır

                LoadUsers(etkinlikAd); // Seçilen etkinlik için kullanıcıları yükle
            }
        }

        // Seçilen etkinlik için kullanıcıları veritabanından alıp ListBox2'ye yükleyen fonksiyon
        private void LoadUsers(string etkinlikAd)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Etkinlik id'sini al
                string getEventIdQuery = "SELECT Id FROM Etkinlikler WHERE EtkinlikAd = @EtkinlikAd";
                SQLiteCommand getEventIdCommand = new SQLiteCommand(getEventIdQuery, connection);
                getEventIdCommand.Parameters.AddWithValue("@EtkinlikAd", etkinlikAd);
                int etkinlikId = Convert.ToInt32(getEventIdCommand.ExecuteScalar());

                if (etkinlikId == 0)
                {
                    MessageBox.Show("Etkinlik bulunamadı.");
                    return;
                }

                // Kullanıcıları al
                string selectUsersQuery = "SELECT AdSoyad, TCKimlikNo, KatilimciSayisi FROM Kullanıcılar WHERE EtkinlikId = @EtkinlikId";
                SQLiteCommand selectUsersCommand = new SQLiteCommand(selectUsersQuery, connection);
                selectUsersCommand.Parameters.AddWithValue("@EtkinlikId", etkinlikId);

                using (SQLiteDataReader reader = selectUsersCommand.ExecuteReader())
                {
                    listBox2.Items.Clear(); // Mevcut öğeleri temizle
                    while (reader.Read())
                    {
                        string user = $"- AdSoyad: {reader["AdSoyad"]} - Katılımcı Sayısı: {reader["KatilimciSayisi"]}";
                        listBox2.Items.Add(user);
                    }
                }
            }
        }

        // Veritabanına yeni kullanıcı ekler
        private void button1_Click(object sender, EventArgs e)
        {
            // TextBox'lardaki verileri ListBox 2'ye ekle
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && listBox1.SelectedItem != null)
            {
                // ListBox2'ye ekleme
                string user = "İsim: " + textBox1.Text + ", Kişi sayısı: " + textBox2.Text;
                listBox2.Items.Add(user);

                // Veritabanına kaydetme
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Etkinlik adını al (listBox1'de seçilen etkinlik)
                    string selectedEvent = listBox1.SelectedItem.ToString();
                    string etkinlikAd = selectedEvent.Split('-')[0].Trim(); // Etkinlik adını ayır

                    // Etkinlik id'sini al
                    string getEventIdQuery = "SELECT Id FROM Etkinlikler WHERE EtkinlikAd = @EtkinlikAd";
                    SQLiteCommand getEventIdCommand = new SQLiteCommand(getEventIdQuery, connection);
                    getEventIdCommand.Parameters.AddWithValue("@EtkinlikAd", etkinlikAd);
                    int etkinlikId = Convert.ToInt32(getEventIdCommand.ExecuteScalar());

                    // Kullanıcıyı veritabanına ekle
                    string insertUserQuery = "INSERT INTO Kullanıcılar (AdSoyad, TCKimlikNo, KatilimciSayisi, EtkinlikId) " +
                                             "VALUES (@AdSoyad, @TCKimlikNo, @KatilimciSayisi, @EtkinlikId)";
                    SQLiteCommand insertUserCommand = new SQLiteCommand(insertUserQuery, connection);
                    insertUserCommand.Parameters.AddWithValue("@AdSoyad", textBox1.Text);
                    insertUserCommand.Parameters.AddWithValue("@TCKimlikNo", textBox2.Text);  // TC Kimlik No olarak textBox2'yi kullandık
                    insertUserCommand.Parameters.AddWithValue("@KatilimciSayisi", textBox2.Text); // Katılımcı sayısını textBox2'den alıyoruz
                    insertUserCommand.Parameters.AddWithValue("@EtkinlikId", etkinlikId); // Etkinlik id'sini alıyoruz

                    insertUserCommand.ExecuteNonQuery();
                }

                // Kullanıcı ekledikten sonra, ListBox2'yi yeniden yükleyelim
                LoadUsers(listBox1.SelectedItem.ToString().Split('-')[0].Trim());
            }
            else
            {
                MessageBox.Show("Lütfen önce etkinlik seçin ve verileri girin.");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
