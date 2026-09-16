using MySqlConnector;
using System.Collections.ObjectModel;

namespace ApotekApp;

public class DashboardAlert
{
    public string NamaObat { get; set; } = "-";
    public int Stok { get; set; }
    public string TglExpired { get; set; } = "-";
}

public partial class DashboardPage : ContentPage
{
    private readonly ObservableCollection<DashboardAlert> _alerts = new();
    public DashboardPage()
    {
        InitializeComponent();
        CvAlert.ItemsSource = _alerts;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LblWelcome.Text = $"Selamat datang, {Preferences.Get("CurrentUserName", "Pengguna")}";
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        try
        {
            using var conn = new MySqlConnection(MauiProgram.ConnectionString);
            await conn.OpenAsync();

            int batasStok = Preferences.Get("BatasStokMenipis", 10);
            int batasExp = Preferences.Get("BatasHariExpired", 30);

            using (var cmd = new MySqlCommand("SELECT COUNT(*), COALESCE(SUM(Stok),0) FROM obats", conn))
            using (var r = await cmd.ExecuteReaderAsync())
            {
                if (await r.ReadAsync())
                {
                    LblTotalObat.Text = Convert.ToInt32(r.GetValue(0)).ToString("N0");
                    LblTotalStok.Text = Convert.ToInt32(r.GetValue(1)).ToString("N0");
                }
            }

            using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM obats WHERE Stok <= @batas", conn))
            {
                cmd.Parameters.AddWithValue("@batas", batasStok);
                LblStokMenipis.Text = Convert.ToInt32(await cmd.ExecuteScalarAsync()).ToString("N0");
            }

            using (var cmd = new MySqlCommand(@"SELECT COUNT(*) FROM obats
                WHERE TanggalKadaluarsa IS NOT NULL AND TanggalKadaluarsa <= @tgl", conn))
            {
                cmd.Parameters.AddWithValue("@tgl", DateTime.Today.AddDays(batasExp));
                LblAkanExpired.Text = Convert.ToInt32(await cmd.ExecuteScalarAsync()).ToString("N0");
            }

            _alerts.Clear();
            using var alertCmd = new MySqlCommand(@"SELECT NamaObat, Stok, TanggalKadaluarsa FROM obats
                WHERE Stok <= @batasStok OR (TanggalKadaluarsa IS NOT NULL AND TanggalKadaluarsa <= @tgl)
                ORDER BY Stok ASC, TanggalKadaluarsa ASC LIMIT 50", conn);
            alertCmd.Parameters.AddWithValue("@batasStok", batasStok);
            alertCmd.Parameters.AddWithValue("@tgl", DateTime.Today.AddDays(batasExp));
            using var ar = await alertCmd.ExecuteReaderAsync();
            while (await ar.ReadAsync())
                _alerts.Add(new DashboardAlert
                {
                    NamaObat = ar["NamaObat"]?.ToString() ?? "-",
                    Stok = Convert.ToInt32(ar["Stok"]),
                    TglExpired = ar["TanggalKadaluarsa"] == DBNull.Value ? "-" :
                        Convert.ToDateTime(ar["TanggalKadaluarsa"]).ToString("dd/MM/yyyy")
                });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Dashboard", "Gagal memuat data: " + ex.Message, "OK");
        }
    }
}
