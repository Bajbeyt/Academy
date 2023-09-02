using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Text;

internal class Program
{
    public static SqlConnection connection;
    public static string connectionString = "Server=localhost;Database=DailyApp;User Id=SA;Password=reallyStrongPwd123;MultipleActiveResultSets=true;TrustServerCertificate=true;";
    private static SqlCommand command;
    private static string query;
    private static DateTime noteTime;

    public static void Main(string[] args)
    {
        using (connection = new SqlConnection(connectionString))
        {
            connection.Open();
            Menu();
        }
    }
    static void Menu()
    {
        Console.WriteLine("+++ UYGULAMAYA HOŞGELDİN +++\n-------");
        Console.WriteLine("1-Giriş\n-------");
        Console.WriteLine("2-Kayıt\n-------");
        Console.WriteLine("Yapmak İstediğin İşlemin Numarasını Gir ve Enterla :)");
        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                Console.Clear();
                Login();
                break;
            case 2:
                Console.Clear();
                SingUp();
                break;
        }
    }
    static void SingUp()
    {
        string password1, password2;
        Console.WriteLine("--GÜNLÜK UYGULAMASINDA KAYIT İŞLEMİNE HOŞGELDİN--\n");
        Console.WriteLine("Kullanıcı Adınız (Nickname) Nedir?: ");
        string nickName = Console.ReadLine();
        Console.WriteLine("Adınız : ");
        string name = Console.ReadLine();
        Console.WriteLine("Soyadınız :");
        string surname = Console.ReadLine();
        do
        {
            Console.Write("Parolanı Gir: ");
            password1 = ReadPassword();
            Console.Write("Parolanı Tekrar Gir: ");
            password2 = ReadPassword();
            if (password1 != password2)
            {
                Console.WriteLine("Parola Eşleşmedi! Lütfen Tekrar Dene.");
            }
        } while (password1 != password2);
        string hashedParola = HashPassword(password2);
        string j = Security_Question();
        int UserId = 0;
        query = "INSERT INTO Users (NickName,NameUser,SurnameUser,Password,Security_Question) VALUES (@NickName,@NameUser,@SurnameUser,@Password,@Security_Question)";
        using (command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@NickName", nickName);
            command.Parameters.AddWithValue("@NameUser", name);
            command.Parameters.AddWithValue("@SurnameUser", surname);
            command.Parameters.AddWithValue("@Password", hashedParola);
            command.Parameters.AddWithValue("@Security_Question", j);
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Şifreler eşleşti. Kayıt Başarılı");
        Console.Clear();
        Console.WriteLine("Menüye Yönlendiriliyorsun---\n");
        Menu();
    }

    static void Login()
    {
        Console.WriteLine("--Giriş--");
        Console.Write("Kullanıcı Adınız: ");
        string nickName = Console.ReadLine();
        Console.Write("Şifrenizi Giriniz: ");
        string password = ReadPassword();
        string hashHedPassword = HashPassword(password);
        int UserId = 0;
        query = "SELECT Password,UserId FROM Users WHERE NickName=@NickName";
        using (command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserId", UserId);
            command.Parameters.AddWithValue("@NickName", nickName);
            string storedHashedPassword = command.ExecuteScalar() as string;
            if (storedHashedPassword != null && storedHashedPassword.Equals(hashHedPassword, StringComparison.Ordinal))
            {
                Console.WriteLine("Giriş Başarılı");
                Console.Clear();
                HomePage(UserId);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("!!!!!!!GİRİŞ BAŞARISIZ LÜTFEN TEKRAR GİRİŞ YAPIN VEYA KAYIT OLUNUZ!!!!!!\n");
                Menu();
            }
        }
    }

    static string Security_Question()
    {
        string reply = "";
        string reply2 = "";
        string reply3 = "";
        Console.WriteLine("Güvenlik Sorunu Seç");
        Console.WriteLine("1-İlk Gittiğin Okulun Adı:");
        Console.WriteLine("2-En Sevdiğin Hayvan Nedir:");
        Console.WriteLine("3-Hobilerinden Birtanesi Nedir:");
        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                Console.Clear();
                Console.WriteLine("İlk Gittiğin Okulun Adı:");
                reply = Console.ReadLine();
                return reply;
                break;
            case 2:
                Console.Clear();
                Console.WriteLine("En Sevdiğin Hayvan Adı");
                reply2 = Console.ReadLine();
                return reply2;
                break;
            case 3:
                Console.Clear();
                Console.WriteLine("Hobilerinden Birtanesi Nedir");
                reply3 = Console.ReadLine();
                return reply3;
                break;
        }
        return reply;
        return reply2;
        return reply3;
    }

    static void HomePage(int UserId)
    {
        int NoteId = 0;
        Console.WriteLine("--HOŞGELDİN Kullanıcı Yapmak İstediğin İşlemi Seç--");
        Console.WriteLine("1-Yeni Kayıt Ekle: ");
        Console.WriteLine("2-Kayıtlı Günlük Listesi: ");
        Console.WriteLine("3-Tüm Kayıtları Sil:  ");
        Console.WriteLine("4-Çıkış Yap: ");
        Console.WriteLine("");

        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                if (DateControl())
                {
                    AddRecord(UserId);
                }
                else Console.WriteLine("Bugün Zaten Yazdın Tekrar Yazmak İstiyor Musun\n");
                NewDailyAdd(NoteId,UserId);
                break;
            case 2:
                ListMenu(NoteId,UserId);
                break;
            case 3:
                Console.Clear();
                DeleteRecords(UserId);
                break;
            case 4:
                Exit();
                break;
        }

    }
    static void AddRecord(int UserId)
    {
        noteTime = DateTime.Now;
        Console.WriteLine("Günlüğünüzü Yazmaya Başlayabilirsiniz :)");
        string notes = Console.ReadLine();
        Console.WriteLine(noteTime.ToString("dd-MM-yyyy"));
        query= "INSERT INTO Note (UserId, Notes, NoteTime) VALUES (@UserId, @Notes, @NoteTime)";

        using (command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserId", UserId);
            command.Parameters.AddWithValue("@Notes", notes);
            command.Parameters.AddWithValue("@NoteTime", noteTime);
            int count = command.ExecuteNonQuery();
            if (count > 0)
            {
                Console.WriteLine("Günlük Kaydınız Başarıyla Kaydedilmiştir.");
                Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Ana Menüye Dönemk İçin 1 e Basınız");
                Console.WriteLine("Çıkmak İstersende 2 ye Basabilirsin");
                int choose1 = Convert.ToInt32(Console.ReadLine());
                switch (choose1)
                {
                    case 1:
                        HomePage(UserId);
                        break;
                    case 2:
                        Exit();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Kaydınız Başarız Oldu Lütfen Tekrar Deneyiniz.");
                AddRecord(UserId);
            }
        }
    }
    static void UpdateDaily(int NoteId,int UserId)
    {
        noteTime = DateTime.Now;
        Console.WriteLine("Günlüğünüzü Yazmaya Başlayabilirsiniz :)");
        string notes = Console.ReadLine();
        Console.WriteLine(noteTime.ToString("dd-MM-yyyy"));

        query = "UPDATE Note SET UpdateTime=@UpdateTime,Notes=@Notes WHERE NoteId=@NoteId AND UserId";

        using (command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@NoteId", NoteId);
            command.Parameters.AddWithValue("@UserId", UserId);
            command.Parameters.AddWithValue("@Notes", notes);
            command.Parameters.AddWithValue("@UpdateTime", noteTime);
            int count = command.ExecuteNonQuery();
            if (count > 0)
            {
                Console.WriteLine("Günlük Kaydınız Başarıyla Kaydedilmiştir.");
                Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Ana Menüye Dönemk İçin 1 e Basınız");
                Console.WriteLine("Çıkmak İstersende 2 ye Basabilirsin");
                int choose1 = Convert.ToInt32(Console.ReadLine());
                switch (choose1)
                {
                    case 1:
                        HomePage(UserId);
                        break;
                    case 2:
                        Exit();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Kaydınız Başarız Oldu Lütfen Tekrar Deneyiniz.");
                AddRecord(UserId);
            }
        }


    }
    static void ListRecords(int NoteId, int UserId)
    {
        Console.WriteLine("--Günlük Kayıtlarınız--");
        query = "SELECT * FROM Note WHERE UserId=@UserId";
        using (command = new SqlCommand(query, connection))
            command.Parameters.AddWithValue("@UserId", UserId);
        command.Parameters.AddWithValue("@NoteId", NoteId);
        command.ExecuteNonQuery();
        using (SqlDataReader dataReader = command.ExecuteReader())
        {

            while (dataReader.Read())
            {
                UserId = (int)dataReader["UserId"];
                NoteId = (int)dataReader["NoteId"];
                string noteTime = dataReader["NoteTime"].ToString();
                string note = dataReader["Notes"].ToString();
                Console.WriteLine($"" + noteTime);
                Console.WriteLine($"{note}\n--------");
                Console.WriteLine("(1) Sonraki Kayıt | (2)Düzenle | (3) Sil | (4) Ana Sayfa");
                string j = Console.ReadLine();
                switch (j)
                {
                    case "1":
                        Console.Clear();
                        break;
                    case "2":
                        UpdateDaily(NoteId,UserId);
                        break;
                    case "3":
                        DeleteDaily(NoteId,UserId);
                        break;
                    case "4":
                        HomePage(UserId);
                        break;

                }
            }
            Console.WriteLine("Ana Sayfaya Yönlendiriliyorsun\n");
            HomePage(UserId);
        }
    }
    static void DeleteRecords(int UserId)
    {
        Console.WriteLine("--Kayıtları Silme İşlemleri--\n");
        Console.WriteLine("!!!Kayıtları Cidden Silmek İstiyor Musun?!!!");
        Console.WriteLine("Eğer Silmek İstiyorsan 1'e Bas");
        Console.WriteLine("Eğer Silmekten Vazgeçtiysen 2'ye Bas");
        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                Delete(UserId);
                break;
            case 2:
                HomePage(UserId);
                break;
        }
    }
    static void Delete(int UserId)
    {
        query = "DELETE FROM Note";
        using (command = new SqlCommand(query, connection))
        {
            int count = command.ExecuteNonQuery();
            Console.WriteLine($"{count} Tablodaki Veriler Silindi.");
        }
        Console.WriteLine("Hayırlı Olsun Kayıtlarını Sildin :)");
        Console.Clear();
        Console.WriteLine("Ana Menüye Dönemk İçin 1 e Basınız");
        Console.WriteLine("Çıkmak İstersende 2 ye Basabilirsin");
        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                HomePage(UserId);
                break;
            case 2:
                Exit();
                break;
        }
    }
    public static void Exit()
    {
        Console.WriteLine("Çıkış");
        Environment.Exit(0);
    }
    static bool DateControl()
    {
        DateTime date = DateTime.Now;
        string today = date.ToString("d");

        query = "SELECT NoteTime FROM Note";

        using (command = new SqlCommand(query, connection))
        using (SqlDataReader dataReader = command.ExecuteReader())
        {
            while (dataReader.Read())
            {
                string date1 = dataReader["NoteTime"].ToString();
                if (date1.Contains(today))
                    return false;
            }
        }
        return true;

    }
    static void DeleteDaily(int NoteId,int UserId)
    {
        Console.WriteLine("--Silme İşlemin Tamamlandı--");
        query = "DELETE FROM Note WHERE NoteId=@NoteId";
        using (command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserId", UserId);
            command.Parameters.AddWithValue("@NoteId", NoteId);
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Ana Menüye Dönemk İçin 1 e Basınız");
        Console.WriteLine("Çıkmak İstersende 2 ye Basabilirsin");
        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                HomePage(UserId);
                break;
            case 2:
                Exit();
                break;
        }
    }
    static void NewDailyAdd(int NoteId,int UserId)
    {
        Console.WriteLine("Cevabın Evet ise 1 \nCevabın Hayır ise 2");
        string choose = Console.ReadLine();
        switch (choose)
        {
            case "1":
                UpdateDaily(NoteId,UserId);
                break;
            case "2":
                Console.WriteLine("Ana Sayfaya Yönlendirildin");
                HomePage(UserId);
                break;
        }
    }
    static void ListMenu(int NoteId,int UserId)
    {
        Console.Clear();
        Console.WriteLine("**LİSTELEME MENÜSÜNE HOŞGELDİN**");
        Console.WriteLine("1-Günlükleri Listeme");
        Console.WriteLine("2-Günlüklerde Arama");
        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                Console.Clear();
                ListRecords(NoteId,UserId);
                break;
            case 2:
                SearchDaily(UserId);
                break;
        }
    }
    static void SearchDaily(int UserId)
    {
        Console.Clear();
        query = "SELECT * FROM Note WHERE NoteTime,UserId LIKE @search OR Notes LIKE @search";
        Console.WriteLine("ARAMAK İSTEDİĞNİZ GÜNLÜĞÜN TARİHİNİ GİRİNİZ\n(Gün olarak arama yapınız)\n------");
        string search = Console.ReadLine();
        string dailyTime = "";
        int a = 0;
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserId", UserId);
            command.Parameters.AddWithValue("@search", "%" + search + "%");
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    a++;
                    dailyTime = reader["NoteTime"].ToString();
                    string note = reader["Notes"].ToString();
                    Console.WriteLine($"" + dailyTime);
                    Console.WriteLine($"{note}\n--------");

                }
            }
        }
        if (a == 0)
        {
            Console.WriteLine("Aradığın Günlük Bilgileri Bulunamadı\n-----\nLütfen Tekrar Deneyiniz.");
            SearchDaily(UserId);
        }
        else
        {
            Console.WriteLine("Aradığın Günlük Bilgilerin\n----\n");
        }
        int NoteId = 0;
        Console.WriteLine("Ana Menüye Dönemk İçin 1 e Basınız");
        Console.WriteLine("Listleme Menüsüne Dönmek İçin 2 ye Basın");
        Console.WriteLine("Çıkmak İstersende 3 e Basabilirsin");
        int choose = Convert.ToInt32(Console.ReadLine());
        switch (choose)
        {
            case 1:
                HomePage(UserId);
                break;
            case 2:
                ListMenu(NoteId,UserId);
                break;
            case 3:
                Exit();
                break;
        }
    }
    static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

    }
    static string ReadPassword()
    {

        StringBuilder password = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }
        return password.ToString();
    }
}

