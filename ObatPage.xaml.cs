using MySqlConnector;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace ApotekApp;

public class ObatItem
{
    public int No { get; set; }
    public int Id { get; set; }
    public string KodeObat { get; set; } = "";
    public string NamaObat { get; set; } = "";
    public string Barcode { get; set; } = "";
    public string Satuan { get; set; } = "";
    public string Kategori { get; set; } = "";
    public int Stok { get; set; }
    public string HargaFormatted { get; set; } = "Rp 0";
    public decimal HargaBeli { get; set; }
    public decimal HargaJual { get; set; }
    public int StokMin { get; set; }
    public DateTime? Expired { get; set; }
}

public partial class ObatPage : ContentPage
{
    readonly ObservableCollection<ObatItem> _items = new();
    int _editId;

    public ObatPage() { InitializeComponent(); CvObatList.ItemsSource = _items; }

    protected override async void OnAppearing() { base.OnAppearing(); await LoadAsync(); }

    public async Task LoadAsync()
    {
        try
        {
            _items.Clear();
            using var c = new MySqlConnection(MauiProgram.ConnectionString);
            await c.OpenAsync();
            using var cmd = new MySqlCommand(@"SELECT Id,KodeObat,NamaObat,Barcode,Satuan,Kategori,Stok,StokMin,HargaBeli,HargaJual,TanggalKadaluarsa
                                               FROM obats ORDER BY NamaObat", c);
            using var r = await cmd.ExecuteReaderAsync();
            int no=1;
            while(await r.ReadAsync())
            {
                var hj=Convert.ToDecimal(r["HargaJual"]);
                _items.Add(new ObatItem {
                    No=no++, Id=Convert.ToInt32(r["Id"]), KodeObat=r["KodeObat"]?.ToString()??"",
                    NamaObat=r["NamaObat"]?.ToString()??"", Barcode=r["Barcode"]?.ToString()??"",
                    Satuan=r["Satuan"]?.ToString()??"", Kategori=r["Kategori"]?.ToString()??"",
                    Stok=Convert.ToInt32(r["Stok"]), StokMin=Convert.ToInt32(r["StokMin"]),
                    HargaBeli=Convert.ToDecimal(r["HargaBeli"]), HargaJual=hj,
                    HargaFormatted="Rp "+hj.ToString("N0"), Expired=r["TanggalKadaluarsa"]==DBNull.Value?null:Convert.ToDateTime(r["TanggalKadaluarsa"])
                });
            }
        } catch(Exception ex) { await DisplayAlert("Error Obat", ex.Message, "OK"); }
    }

    void OnSearchTextChanged(object s, TextChangedEventArgs e)
    {
        var k=e.NewTextValue?.Trim().ToLowerInvariant()??"";
        CvObatList.ItemsSource=string.IsNullOrEmpty(k)?_items:_items.Where(x=>
            x.KodeObat.ToLowerInvariant().Contains(k)||x.NamaObat.ToLowerInvariant().Contains(k)||
            x.Barcode.ToLowerInvariant().Contains(k)||x.Kategori.ToLowerInvariant().Contains(k)).ToList();
    }
    async void OnRefreshClicked(object s, EventArgs e)=>await LoadAsync();

    void OnTambahObatClicked(object s, EventArgs e)
    {
        _editId=0; LblModalTitle.Text="Tambah Obat"; ClearForm(); ModalLayout.IsVisible=true;
    }
    void OnEditClicked(object s, EventArgs e)
    {
        if ((s as Button)?.CommandParameter is not ObatItem x) return;
        _editId=x.Id; LblModalTitle.Text="Edit Obat"; TxtKode.Text=x.KodeObat; TxtBarcode.Text=x.Barcode;
        TxtNama.Text=x.NamaObat; TxtKategori.Text=x.Kategori; TxtLokasi.Text="";
        TxtSatuan.Text=x.Satuan; TxtStok.Text=x.Stok.ToString(); TxtStokMin.Text=x.StokMin.ToString();
        TxtHargaBeli.Text=x.HargaBeli.ToString("0.##",CultureInfo.InvariantCulture); TxtHargaJual.Text=x.HargaJual.ToString("0.##",CultureInfo.InvariantCulture);
        DtpExpired.Date=x.Expired??DateTime.Today; ModalLayout.IsVisible=true;
    }
    async void OnHapusClicked(object s, EventArgs e)
    {
        if ((s as Button)?.CommandParameter is not ObatItem x) return;
        if(!await DisplayAlert("Hapus",$"Hapus obat {x.NamaObat}?","Ya","Batal")) return;
        try { using var c=new MySqlConnection(MauiProgram.ConnectionString); await c.OpenAsync();
            using var cmd=new MySqlCommand("DELETE FROM obats WHERE Id=@id",c); cmd.Parameters.AddWithValue("@id",x.Id); await cmd.ExecuteNonQueryAsync(); await LoadAsync();
        } catch(Exception ex){await DisplayAlert("Gagal Hapus",ex.Message,"OK");}
    }
    async void OnSimpanClicked(object s, EventArgs e)
    {
        if(string.IsNullOrWhiteSpace(TxtKode.Text)||string.IsNullOrWhiteSpace(TxtNama.Text)){await DisplayAlert("Peringatan","Kode dan nama wajib diisi.","OK");return;}
        int.TryParse(TxtStok.Text,out var stok); int.TryParse(TxtStokMin.Text,out var min);
        decimal.TryParse(TxtHargaBeli.Text?.Replace(",","."),NumberStyles.Any,CultureInfo.InvariantCulture,out var hb);
        decimal.TryParse(TxtHargaJual.Text?.Replace(",","."),NumberStyles.Any,CultureInfo.InvariantCulture,out var hj);
        try{
            using var c=new MySqlConnection(MauiProgram.ConnectionString); await c.OpenAsync();
            MySqlCommand cmd;
            if(_editId==0) cmd=new MySqlCommand(@"INSERT INTO obats(KodeObat,NamaObat,Barcode,Satuan,Kategori,LokasiRak,HargaBeli,HargaJual,Stok,StokMin,TanggalKadaluarsa)
                VALUES(@kode,@nama,@barcode,@satuan,@kat,@rak,@hb,@hj,@stok,@min,@exp)",c);
            else cmd=new MySqlCommand(@"UPDATE obats SET KodeObat=@kode,NamaObat=@nama,Barcode=@barcode,Satuan=@satuan,Kategori=@kat,LokasiRak=@rak,HargaBeli=@hb,HargaJual=@hj,Stok=@stok,StokMin=@min,TanggalKadaluarsa=@exp WHERE Id=@id",c);
            cmd.Parameters.AddWithValue("@kode",TxtKode.Text.Trim()); cmd.Parameters.AddWithValue("@nama",TxtNama.Text.Trim());
            cmd.Parameters.AddWithValue("@barcode",string.IsNullOrWhiteSpace(TxtBarcode.Text)?TxtKode.Text.Trim():TxtBarcode.Text.Trim());
            cmd.Parameters.AddWithValue("@satuan",string.IsNullOrWhiteSpace(TxtSatuan.Text)?"Pcs":TxtSatuan.Text.Trim());
            cmd.Parameters.AddWithValue("@kat",string.IsNullOrWhiteSpace(TxtKategori.Text)?"Umum":TxtKategori.Text.Trim());
            cmd.Parameters.AddWithValue("@rak",string.IsNullOrWhiteSpace(TxtLokasi.Text)?"-":TxtLokasi.Text.Trim());
            cmd.Parameters.AddWithValue("@hb",hb);cmd.Parameters.AddWithValue("@hj",hj);cmd.Parameters.AddWithValue("@stok",stok);cmd.Parameters.AddWithValue("@min",min);cmd.Parameters.AddWithValue("@exp",DtpExpired.Date);
            if(_editId!=0)cmd.Parameters.AddWithValue("@id",_editId);
            await cmd.ExecuteNonQueryAsync(); ModalLayout.IsVisible=false; await LoadAsync();
        }catch(Exception ex){await DisplayAlert("Gagal Simpan",ex.Message,"OK");}
    }
    void OnBatalClicked(object s,EventArgs e)=>ModalLayout.IsVisible=false;
    void ClearForm(){foreach(var e in new[]{TxtKode,TxtBarcode,TxtNama,TxtKategori,TxtLokasi,TxtSatuan,TxtStok,TxtStokMin,TxtHargaBeli,TxtHargaJual})e.Text="";DtpExpired.Date=DateTime.Today;}

    async void OnExportClicked(object s,EventArgs e)
    {
        try{
            using var c=new MySqlConnection(MauiProgram.ConnectionString);await c.OpenAsync();
            using var cmd=new MySqlCommand("SELECT KodeObat,NamaObat,Barcode,Satuan,Kategori,LokasiRak,HargaBeli,HargaJual,Stok,StokMin,TanggalKadaluarsa FROM obats ORDER BY NamaObat",c);
            using var r=await cmd.ExecuteReaderAsync();var sb=new StringBuilder("KodeObat,NamaObat,Barcode,Satuan,Kategori,LokasiRak,HargaBeli,HargaJual,Stok,StokMin,TanggalKadaluarsa\n");
            while(await r.ReadAsync()) sb.AppendLine(string.Join(",",Enumerable.Range(0,r.FieldCount).Select(i=>$"\"{r.GetValue(i)?.ToString()?.Replace("\"","\"\"")}\"")));
            await SaveCsvAsync($"Data_Obat_{DateTime.Now:yyyyMMdd_HHmmss}.csv",sb.ToString());
        }catch(Exception ex){await DisplayAlert("Export",ex.Message,"OK");}
    }
    async void OnImportClicked(object s,EventArgs e)
    {
        try{
            var f=await FilePicker.Default.PickAsync(new PickOptions{PickerTitle="Pilih CSV"});
            if(f==null)return;
            using var stream=await f.OpenReadAsync();using var sr=new StreamReader(stream);await sr.ReadLineAsync();
            using var c=new MySqlConnection(MauiProgram.ConnectionString);await c.OpenAsync();int ok=0;
            while(await sr.ReadLineAsync() is string line){
                if(string.IsNullOrWhiteSpace(line))continue;var a=line.Split(',');if(a.Length<10)continue;
                string Clean(string v)=>v.Trim().Trim('"'); 
                using var cmd=new MySqlCommand(@"INSERT INTO obats(KodeObat,NamaObat,Barcode,Satuan,Kategori,LokasiRak,HargaBeli,HargaJual,Stok,StokMin,TanggalKadaluarsa)
                    VALUES(@k,@n,@b,@s,@cat,@rak,@hb,@hj,@st,@mn,@ex)
                    ON DUPLICATE KEY UPDATE NamaObat=VALUES(NamaObat),HargaBeli=VALUES(HargaBeli),HargaJual=VALUES(HargaJual),Stok=VALUES(Stok),StokMin=VALUES(StokMin),TanggalKadaluarsa=VALUES(TanggalKadaluarsa)",c);
                cmd.Parameters.AddWithValue("@k",Clean(a[0]));cmd.Parameters.AddWithValue("@n",Clean(a[1]));cmd.Parameters.AddWithValue("@b",Clean(a[2]));cmd.Parameters.AddWithValue("@s",Clean(a[3]));cmd.Parameters.AddWithValue("@cat",Clean(a[4]));cmd.Parameters.AddWithValue("@rak",Clean(a[5]));
                decimal.TryParse(Clean(a[6]),NumberStyles.Any,CultureInfo.InvariantCulture,out var hb);decimal.TryParse(Clean(a[7]),NumberStyles.Any,CultureInfo.InvariantCulture,out var hj);
                int.TryParse(Clean(a[8]),out var st);int.TryParse(Clean(a[9]),out var mn);DateTime.TryParse(a.Length>10?Clean(a[10]):"",out var ex);
                cmd.Parameters.AddWithValue("@hb",hb);cmd.Parameters.AddWithValue("@hj",hj);cmd.Parameters.AddWithValue("@st",st);cmd.Parameters.AddWithValue("@mn",mn);cmd.Parameters.AddWithValue("@ex",ex==default?null:ex);await cmd.ExecuteNonQueryAsync();ok++;
            } await LoadAsync();await DisplayAlert("Import", $"Berhasil memproses {ok} baris.","OK");
        }catch(Exception ex){await DisplayAlert("Import",ex.Message,"OK");}
    }
    async Task SaveCsvAsync(string name,string content){
#if WINDOWS
        var picker=new Windows.Storage.Pickers.FileSavePicker();picker.SuggestedStartLocation=Windows.Storage.Pickers.PickerLocationId.Downloads;picker.FileTypeChoices.Add("CSV",new List<string>{".csv"});picker.SuggestedFileName=name;
        var w=Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;if(w!=null){var hwnd=WinRT.Interop.WindowNative.GetWindowHandle(w);WinRT.Interop.InitializeWithWindow.Initialize(picker,hwnd);}
        var f=await picker.PickSaveFileAsync();if(f!=null)await Windows.Storage.FileIO.WriteTextAsync(f,content);
#else
        var p=Path.Combine(FileSystem.CacheDirectory,name);await File.WriteAllTextAsync(p,content);
#endif
    }
}
