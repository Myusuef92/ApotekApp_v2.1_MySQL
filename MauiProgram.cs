using ApotekApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using MySqlConnector;

namespace ApotekApp;

public static class MauiProgram
{
    public const string ConnectionString = "Server=localhost;Port=3306;Database=db_apotek;User=root;Password=;";
    public const string LogoUpsertSql = @"INSERT INTO pengaturan (id, logo_path, logo_data) VALUES (1, @path, @logo) ON DUPLICATE KEY UPDATE logo_path=@path, logo_data=@logo";
    public const string SettingsUpsertSql = @"INSERT INTO pengaturan (id, nama_apotek, alamat, no_telepon) VALUES (1,@nama,@alamat,@telp) ON DUPLICATE KEY UPDATE nama_apotek=@nama, alamat=@alamat, no_telepon=@telp";

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows => windows.OnWindowCreated(window => window.ExtendsContentIntoTitleBar = true));
#endif
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)), ServiceLifetime.Transient);
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ObatPage>();
        builder.Services.AddTransient<SupplierPage>();
        builder.Services.AddTransient<UserPage>();
        builder.Services.AddTransient<TransaksiPage>();
        builder.Services.AddTransient<PembelianPage>();
        builder.Services.AddTransient<LaporanPage>();
        builder.Services.AddTransient<SettingPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        var app = builder.Build();
        EnsureDatabaseSchema();
        return app;
    }

    private static void EnsureDatabaseSchema()
    {
        try
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            const string sql = @"
CREATE TABLE IF NOT EXISTS pengaturan (
 id INT NOT NULL DEFAULT 1, nama_apotek VARCHAR(100) NOT NULL DEFAULT 'APOTEK SEHAT',
 nama_pemilik VARCHAR(100) NULL, alamat TEXT NULL, no_telepon VARCHAR(30) NULL,
 sia_sipa VARCHAR(100) NULL, catatan_struk VARCHAR(255) NULL, logo_path TEXT NULL,
 logo_data MEDIUMBLOB NULL, ukuran_kertas VARCHAR(10) DEFAULT '58mm', pajak_ppn DECIMAL(5,2) DEFAULT 0.00,
 PRIMARY KEY(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE IF NOT EXISTS detail_pembelians (
 IdDetailPembelian INT NOT NULL AUTO_INCREMENT, PembelianId INT NOT NULL, ObatId INT NOT NULL, Jumlah INT NOT NULL,
 HargaBeli DECIMAL(18,2) NOT NULL, PRIMARY KEY(IdDetailPembelian), KEY IX_DetailPembelian_PembelianId(PembelianId), KEY IX_DetailPembelian_ObatId(ObatId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE IF NOT EXISTS roles (id INT NOT NULL AUTO_INCREMENT, nama_role VARCHAR(50) NOT NULL, PRIMARY KEY(id), UNIQUE KEY uq_roles_nama(nama_role)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            try { using var alter = new MySqlCommand("ALTER TABLE pengaturan ADD COLUMN logo_data MEDIUMBLOB NULL", conn); alter.ExecuteNonQuery(); } catch { }
            using var seed = new MySqlCommand(@"INSERT IGNORE INTO roles(nama_role) VALUES ('Admin'),('Apoteker'),('Kasir');
INSERT IGNORE INTO users(username,password,nama_lengkap,role_id) SELECT 'admin','admin123','Administrator',id FROM roles WHERE nama_role='Admin' AND NOT EXISTS(SELECT 1 FROM users WHERE username='admin');", conn);
            seed.ExecuteNonQuery();
        }
        catch(Exception ex) { System.Diagnostics.Debug.WriteLine("[DB INIT] " + ex); }
    }
}
