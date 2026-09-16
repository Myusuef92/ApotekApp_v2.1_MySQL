using MySqlConnector;

namespace ApotekApp;

public partial class LaporanPage : ContentPage
{
    public LaporanPage(ApotekApp.Data.AppDbContext context) { InitializeComponent(); Init(); }
    public LaporanPage() { InitializeComponent(); Init(); }
    void Init(){TglMulaiPicker.Date=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);TglSelesaiPicker.Date=DateTime.Today;JenisLaporanPicker.SelectedIndex=0;}
    protected override async void OnAppearing(){base.OnAppearing();await LoadAsync();}
    async void OnTampilkanClicked(object s,EventArgs e)=>await LoadAsync();
    async void OnFilterChanged(object s,EventArgs e)=>await LoadAsync();
    public async Task LoadAsync()
    {
        try
        {
            var awal = TglMulaiPicker.Date.Date;
            var akhir = TglSelesaiPicker.Date.Date.AddDays(1);
            string jenis = JenisLaporanPicker.SelectedItem?.ToString() ?? "Penjualan";
            using var c = new MySqlConnection(MauiProgram.ConnectionString);
            await c.OpenAsync();
            var rows = new List<LaporanItem>();

            if (jenis == "Penjualan")
            {
                const string sql = @"SELECT no_nota,tanggal,grand_total FROM penjualan WHERE tanggal>=@a AND tanggal<@b ORDER BY tanggal DESC";
                using var cmd = new MySqlCommand(sql, c);
                cmd.Parameters.AddWithValue("@a", awal); cmd.Parameters.AddWithValue("@b", akhir);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    rows.Add(new LaporanItem { Tanggal = Convert.ToDateTime(r["tanggal"]), NomorNota = r["no_nota"].ToString() ?? "", NamaUser = "Kasir", TotalRp = Convert.ToDecimal(r["grand_total"]) });
            }
            else if (jenis == "Pembelian")
            {
                const string sql = @"SELECT p.NoFaktur,p.TanggalPembelian,p.TotalHarga,COALESCE(s.NamaSupplier,'-') AS NamaSupplier FROM pembelians p LEFT JOIN suppliers s ON s.Id=p.SupplierId WHERE p.TanggalPembelian>=@a AND p.TanggalPembelian<@b ORDER BY p.TanggalPembelian DESC";
                using var cmd = new MySqlCommand(sql, c);
                cmd.Parameters.AddWithValue("@a", awal); cmd.Parameters.AddWithValue("@b", akhir);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    rows.Add(new LaporanItem { Tanggal = Convert.ToDateTime(r["TanggalPembelian"]), NomorNota = r["NoFaktur"].ToString() ?? "", NamaUser = r["NamaSupplier"].ToString() ?? "-", TotalRp = Convert.ToDecimal(r["TotalHarga"]) });
            }
            else
            {
                using var cmd = new MySqlCommand("SELECT KodeObat,NamaObat,Stok,HargaJual FROM obats ORDER BY NamaObat", c);
                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    rows.Add(new LaporanItem { Tanggal = DateTime.Today, NomorNota = r["KodeObat"].ToString() ?? "", NamaUser = r["NamaObat"].ToString() ?? "", TotalRp = Convert.ToDecimal(r["Stok"]) });
            }

            LaporanCollectionView.ItemsSource = rows;
            TotalTransaksiLabel.Text = $"{rows.Count} Data";
            TotalNominalLabel.Text = jenis == "Stok Obat" ? $"{rows.Sum(x => x.TotalRp):N0} Unit" : $"Rp {rows.Sum(x => x.TotalRp):N0}";
            RataRataLabel.Text = rows.Count == 0 ? (jenis == "Stok Obat" ? "0 Unit" : "Rp 0") : jenis == "Stok Obat" ? $"{rows.Average(x => x.TotalRp):N0} Unit" : $"Rp {rows.Average(x => x.TotalRp):N0}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Laporan", "Gagal memuat riwayat: " + ex.Message, "OK");
        }
    }

    async void OnCetakClicked(object s,EventArgs e)=>await DisplayAlert("Laporan","Laporan siap dicetak dari data yang tampil.","OK");
}
public class LaporanItem
{
    public DateTime Tanggal{get;set;} public string TanggalFormatted=>Tanggal.ToString("dd/MM/yyyy HH:mm");
    public string NomorNota{get;set;}=""; public string NamaUser{get;set;}=""; public decimal TotalRp{get;set;}
    public string TotalRpFormatted=>TotalRp.ToString("N0");
}
