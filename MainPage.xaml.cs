using ApotekApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ApotekApp;

public partial class MainPage : ContentPage
{
    private Button? _currentActiveButton;
    private readonly string _connString = MauiProgram.ConnectionString;

    public MainPage()
    {
        InitializeComponent();
        ApplyUserIdentity();
        RefreshApotekLogo();
        ApplyPermissions();
        _ = SetActiveMenuAsync(BtnDashboard, "DASHBOARD", new DashboardPage());
    }

    private void ApplyUserIdentity()
    {
        string name = Preferences.Get("CurrentUserName", "Pengguna");
        string role = Preferences.Get("CurrentUserRole", "Kasir");

        LblSidebarUser.Text = name;
        LblSidebarRole.Text = $"● {role}";
        LblHeaderUser.Text = $"{name} • {role}";
    }

    public void RefreshApotekLogo()
    {
        try
        {
            string path = Preferences.Get("AppLogoPath", string.Empty);
            ImgSidebarLogo.Source = !string.IsNullOrWhiteSpace(path) && File.Exists(path)
                ? ImageSource.FromFile(path)
                : ImageSource.FromFile("logo_rpl.png");
        }
        catch
        {
            ImgSidebarLogo.Source = ImageSource.FromFile("logo_rpl.png");
        }
    }

    private void ApplyPermissions()
    {
        string role = Preferences.Get("CurrentUserRole", "Kasir");
        bool admin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        // Admin: semua menu. Apoteker: master obat/supplier + transaksi/laporan.
        // Kasir: penjualan/laporan.
        BtnObat.IsVisible = admin || role.Equals("Apoteker", StringComparison.OrdinalIgnoreCase);
        BtnSupplier.IsVisible = admin || role.Equals("Apoteker", StringComparison.OrdinalIgnoreCase);
        BtnUser.IsVisible = admin;
        BtnPembelian.IsVisible = admin || role.Equals("Apoteker", StringComparison.OrdinalIgnoreCase);
        BtnPenjualan.IsVisible = true;
        BtnLaporan.IsVisible = true;
        BtnSetting.IsVisible = admin;
    }

    private async Task SetActiveMenuAsync(Button selectedButton, string title, ContentPage page)
    {
        try
        {
            if (_currentActiveButton != null)
            {
                _currentActiveButton.BackgroundColor = Colors.Transparent;
                _currentActiveButton.TextColor = Color.FromArgb("#94A3B8");
            }
            selectedButton.BackgroundColor = Color.FromArgb("#334155");
            selectedButton.TextColor = Colors.White;
            _currentActiveButton = selectedButton;
            LblPageTitle.Text = title;
            MainContent.Content = page.Content;

            switch (page)
            {
                case DashboardPage d: await d.LoadAsync(); break;
                case ObatPage o: await o.LoadAsync(); break;
                case SupplierPage s: await s.LoadAsync(); break;
                case UserPage u: await u.LoadAsync(); break;
                case PembelianPage p: await p.LoadAsync(); break;
                case LaporanPage l: await l.LoadAsync(); break;
                case TransaksiPage t: t.InitializeForHost(); break;
                case SettingPage st: await st.LoadSettingsAsync(); break;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Gagal Membuka Menu", ex.Message, "OK");
        }
    }

    private async void OnMenuDashboardClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnDashboard,"DASHBOARD",new DashboardPage());
    private async void OnMenuObatClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnObat,"DATA OBAT",new ObatPage());
    private async void OnMenuSupplierClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnSupplier,"DATA SUPPLIER",new SupplierPage());
    private async void OnMenuUserClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnUser,"DATA USER",new UserPage());
    private async void OnMenuPenjualanClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnPenjualan,"TRANSAKSI PENJUALAN",new TransaksiPage());
    private async void OnMenuPembelianClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnPembelian,"TRANSAKSI PEMBELIAN",new PembelianPage());
    private async void OnMenuLaporanClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnLaporan,"LAPORAN",new LaporanPage(CreateDbContext()));
    private async void OnMenuSettingClicked(object sender, EventArgs e) => await SetActiveMenuAsync(BtnSetting,"PENGATURAN SISTEM",new SettingPage());

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(_connString, ServerVersion.AutoDetect(_connString)).Options;
        return new AppDbContext(options);
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        if (!await DisplayAlert("Konfirmasi","Apakah Anda yakin ingin keluar?","Ya","Tidak")) return;
        Preferences.Clear();
        if(Application.Current!=null) Application.Current.MainPage=new NavigationPage(new LoginPage());
    }
}
