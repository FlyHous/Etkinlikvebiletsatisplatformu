using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace WindowsFormsApp10
{
    public partial class Form3 : Form
    {
        private string connectionString = "Data Source=etkinlikler.db;Version=3;";

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Tarih ve saat formatını ayarlama
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "HH:mm"; // Saat formatı
            CreateDatabase();  // Veritabanını oluştur
        }

        private void CreateDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Eski tabloyu sil (isteğe bağlı, veritabanı sıfırlanır)
                string dropTableQuery = "DROP TABLE IF EXISTS Etkinlikler";
                SQLiteCommand dropCommand = new SQLiteCommand(dropTableQuery, connection);
                dropCommand.ExecuteNonQuery();

                // Yeni etkinlikler tablosunu oluştur
                string createEventTableQuery = @"
                CREATE TABLE IF NOT EXISTS Etkinlikler (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    EtkinlikAd TEXT, 
                    EtkinlikYeri TEXT,
                    EtkinlikTarihi DATE,
                    EtkinlikSaati TIME
                )";

                SQLiteCommand createEventTableCommand = new SQLiteCommand(createEventTableQuery, connection);
                createEventTableCommand.ExecuteNonQuery();

                // Kullanıcılar tablosunu oluştur
                string createUserTableQuery = @"
                CREATE TABLE IF NOT EXISTS Kullanıcılar (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    EtkinlikId INTEGER,
                    AdSoyad TEXT,
                    TCKimlikNo TEXT,
                    KatilimciSayisi INTEGER,
                    FOREIGN KEY (EtkinlikId) REFERENCES Etkinlikler(Id)
                )";

                SQLiteCommand createUserTableCommand = new SQLiteCommand(createUserTableQuery, connection);
                createUserTableCommand.ExecuteNonQuery();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string etkinlikAd = textBox1.Text;
            string etkinlikYeri = textBox2.Text;
            string etkinlikTarihi = dateTimePicker2.Value.ToString("yyyy-MM-dd"); // Tarih formatı
            string etkinlikSaati = dateTimePicker1.Value.ToString("HH:mm"); // Saat formatı

            // Veritabanına veri ekleme
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Etkinlik ekleme işlemi
                string insertQuery = "INSERT INTO Etkinlikler (EtkinlikAd, EtkinlikYeri, EtkinlikTarihi, EtkinlikSaati) " +
                                     "VALUES (@EtkinlikAd, @EtkinlikYeri, @EtkinlikTarihi, @EtkinlikSaati)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@EtkinlikAd", etkinlikAd);
                command.Parameters.AddWithValue("@EtkinlikYeri", etkinlikYeri);
                command.Parameters.AddWithValue("@EtkinlikTarihi", etkinlikTarihi);
                command.Parameters.AddWithValue("@EtkinlikSaati", etkinlikSaati);
                command.ExecuteNonQuery(); // Etkinliği ekle
            }

            // Yeni etkinlik ekledikten sonra, etkinlikleri yeniden yükle
            Form4 form4 = new Form4();
            form4.Show();
            form4.LoadEtkinlikler();  // Etkinlikleri yeniden yükle
            MessageBox.Show("Etkinlik başarıyla kaydedildi.");
        }
    }
    }

