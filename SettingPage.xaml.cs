using Microsoft.Maui.Storage;
using System;
using System.IO;

namespace ApotekApp;

public partial class SettingPage : ContentPage
{
    public SettingPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadSettingsAsync();
    }

    public void LoadSettings() => _ = LoadSettingsAsync();

    public async Task LoadSettingsAsync()
    {
        try
        {
            await LoadSettingsFromDatabaseAsync();
        }
        catch (Exception ex)
        {
            // Database gagal dibaca: gunakan cache lokal, tetapi jangan menampilkan pesan palsu.
            LoadSettingsFromPreferences();
            LblLogoStatus.Text = "Pengaturan lokal digunakan sementara karena data database tidak dapat dimuat.";
            System.Diagnostics.Debug.WriteLine("[SETTING LOAD] " + ex);
        }
    }

    private void LoadSettingsFromPreferences()
    {
        EntNamaApotek.Text = Preferences.Get("NamaApotek", "APOTEK BANYUASIH");
        EntAlamatApotek.Text = Preferences.Get("AlamatApotek", "Jl. Merdeka No. 123, Banyuasin");
        EntTeleponApotek.Text = Preferences.Get("TeleponApotek", "0812-3456-7890");
        var path = Preferences.Get("AppLogoPath", "");
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            ImgLogoPreview.Source = ImageSource.FromFile(path);
            LblLogoStatus.Text = "Logo lokal sedang digunakan.";
        }
        else
        {
            ImgLogoPreview.Source = ImageSource.FromFile("logo_rpl.png");
            LblLogoStatus.Text = "Logo bawaan sedang digunakan.";
        }
    }

    private async Task LoadSettingsFromDatabaseAsync()
    {
        using var conn = new MySqlConnector.MySqlConnection(MauiProgram.ConnectionString);
        await conn.OpenAsync();
        const string sql = "SELECT nama_apotek, alamat, no_telepon, logo_data, logo_path FROM pengaturan WHERE id = 1 LIMIT 1";
        using var cmd = new MySqlConnector.MySqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            LoadSettingsFromPreferences();
            LblLogoStatus.Text = "✓ Database terhubung • Belum ada pengaturan. Logo bawaan digunakan.";
            return;
        }

        EntNamaApotek.Text = reader["nama_apotek"]?.ToString() ?? "APOTEK BANYUASIH";
        EntAlamatApotek.Text = reader["alamat"]?.ToString() ?? "";
        EntTeleponApotek.Text = reader["no_telepon"]?.ToString() ?? "";

        byte[]? logoBytes = null;
        if (!reader.IsDBNull(reader.GetOrdinal("logo_data")))
            logoBytes = (byte[])reader["logo_data"];

        string? dbLogoPath = reader["logo_path"]?.ToString();
        string? localPath = null;

        if (logoBytes is { Length: > 0 })
        {
            localPath = await SaveLogoCacheAsync(logoBytes, ".png");
            Preferences.Set("AppLogoPath", localPath);
            ImgLogoPreview.Source = ImageSource.FromFile(localPath);
            LblLogoStatus.Text = "✓ Database terhubung • Logo custom tersimpan di database.";
        }
        else if (!string.IsNullOrWhiteSpace(dbLogoPath) && File.Exists(dbLogoPath))
        {
            Preferences.Set("AppLogoPath", dbLogoPath);
            ImgLogoPreview.Source = ImageSource.FromFile(dbLogoPath);
            LblLogoStatus.Text = "✓ Database terhubung • Logo custom lokal sedang digunakan.";
        }
        else
        {
            var prefPath = Preferences.Get("AppLogoPath", "");
            if (!string.IsNullOrWhiteSpace(prefPath) && File.Exists(prefPath))
            {
                ImgLogoPreview.Source = ImageSource.FromFile(prefPath);
                LblLogoStatus.Text = "✓ Database terhubung • Logo lokal sedang digunakan.";
            }
            else
            {
                ImgLogoPreview.Source = ImageSource.FromFile("logo_rpl.png");
                LblLogoStatus.Text = "✓ Database terhubung • Logo bawaan sedang digunakan.";
            }
        }

        Preferences.Set("NamaApotek", EntNamaApotek.Text ?? "");
        Preferences.Set("AlamatApotek", EntAlamatApotek.Text ?? "");
        Preferences.Set("TeleponApotek", EntTeleponApotek.Text ?? "");
    }

    private async Task<string> SaveLogoCacheAsync(byte[] bytes, string extension)
    {
        string folder = Path.Combine(FileSystem.AppDataDirectory, "Apotek");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, "logo_apotek" + (string.IsNullOrWhiteSpace(extension) ? ".png" : extension.ToLowerInvariant()));
        await File.WriteAllBytesAsync(path, bytes);
        return path;
    }

    private async void OnPilihLogoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Pilih Logo Apotek",
                FileTypes = FilePickerFileType.Images
            });
            if (result == null) return;

            string extension = Path.GetExtension(result.FileName);
            if (string.IsNullOrWhiteSpace(extension)) extension = ".png";
            if (!new[] { ".png", ".jpg", ".jpeg", ".webp" }.Contains(extension.ToLowerInvariant()))
                extension = ".png";

            byte[] bytes;
            using (Stream source = await result.OpenReadAsync())
            using (var ms = new MemoryStream())
            {
                await source.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            string localPath = await SaveLogoCacheAsync(bytes, extension);
            Preferences.Set("AppLogoPath", localPath);

            // Simpan binary logo + path ke database dalam satu operasi.
            using var conn = new MySqlConnector.MySqlConnection(MauiProgram.ConnectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlConnector.MySqlCommand(MauiProgram.LogoUpsertSql, conn);
            AddBlobParameter(cmd, "@logo", bytes);
            cmd.Parameters.AddWithValue("@path", localPath);
            await cmd.ExecuteNonQueryAsync();

            ImgLogoPreview.Source = ImageSource.FromFile(localPath);
            LblLogoStatus.Text = "✓ Logo berhasil disimpan ke database dan digunakan aplikasi.";

            if (Application.Current?.MainPage is NavigationPage nav && nav.CurrentPage is MainPage main)
                main.RefreshApotekLogo();

            await DisplayAlert("Berhasil", "Logo Apotek berhasil di-upload dan disimpan ke database.", "OK");
        }
        catch (Exception ex)
        {
            LblLogoStatus.Text = "Gagal menyimpan logo ke database.";
            System.Diagnostics.Debug.WriteLine("[LOGO SAVE] " + ex);
            await DisplayAlert("Gagal Upload Logo", "Logo belum tersimpan ke database.\n\n" + ex.Message, "OK");
        }
    }

    private async void OnSimpanSettingClicked(object sender, EventArgs e)
    {
        try
        {
            string nama = EntNamaApotek.Text?.Trim() ?? "";
            string alamat = EntAlamatApotek.Text?.Trim() ?? "";
            string telepon = EntTeleponApotek.Text?.Trim() ?? "";
            Preferences.Set("NamaApotek", nama);
            Preferences.Set("AlamatApotek", alamat);
            Preferences.Set("TeleponApotek", telepon);

            using var conn = new MySqlConnector.MySqlConnection(MauiProgram.ConnectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlConnector.MySqlCommand(MauiProgram.SettingsUpsertSql, conn);
            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@alamat", alamat);
            cmd.Parameters.AddWithValue("@telp", telepon);
            await cmd.ExecuteNonQueryAsync();

            if (Application.Current?.MainPage is NavigationPage nav && nav.CurrentPage is MainPage main)
                main.RefreshApotekLogo();

            LblLogoStatus.Text = "✓ Database terhubung • Pengaturan berhasil disimpan.";
            await DisplayAlert("Berhasil", "Pengaturan apotek berhasil disimpan ke database.", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[SETTING SAVE] " + ex);
            await DisplayAlert("Gagal Simpan", "Pengaturan belum tersimpan ke database.\n\n" + ex.Message, "OK");
        }
    }

    private static void AddBlobParameter(MySqlConnector.MySqlCommand cmd, string name, byte[] bytes)
    {
        var p = cmd.Parameters.Add(name, MySqlConnector.MySqlDbType.MediumBlob);
        p.Value = bytes;
    }
}
