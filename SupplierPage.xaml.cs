using MySqlConnector;
using System.Collections.ObjectModel;

namespace ApotekApp;

public class SupplierItem { public int Id{get;set;} public string NamaSupplier{get;set;}=""; public string Telepon{get;set;}=""; public string Email{get;set;}=""; public string Alamat{get;set;}=""; }

public partial class SupplierPage : ContentPage
{
    readonly ObservableCollection<SupplierItem> _items=new(); int _editId;
    public SupplierPage(){InitializeComponent();CvSupplierList.ItemsSource=_items;}
    protected override async void OnAppearing(){base.OnAppearing();await LoadAsync();}
    public async Task LoadAsync(){try{_items.Clear();using var c=new MySqlConnection(MauiProgram.ConnectionString);await c.OpenAsync();using var cmd=new MySqlCommand("SELECT Id,NamaSupplier,Telepon,Alamat,Email FROM suppliers ORDER BY NamaSupplier",c);using var r=await cmd.ExecuteReaderAsync();while(await r.ReadAsync())_items.Add(new SupplierItem{Id=Convert.ToInt32(r["Id"]),NamaSupplier=r["NamaSupplier"]?.ToString()??"",Telepon=r["Telepon"]?.ToString()??"",Alamat=r["Alamat"]?.ToString()??"",Email=r["Email"]?.ToString()??""});}catch(Exception ex){await DisplayAlert("Supplier",ex.Message,"OK");}}
    void OnSearchTextChanged(object s,TextChangedEventArgs e){var k=e.NewTextValue?.Trim().ToLowerInvariant()??"";CvSupplierList.ItemsSource=string.IsNullOrEmpty(k)?_items:_items.Where(x=>(x.NamaSupplier+" "+x.Telepon+" "+x.Email+" "+x.Alamat).ToLowerInvariant().Contains(k)).ToList();}
    async void OnRefreshClicked(object s,EventArgs e)=>await LoadAsync();
    void OnTambahSupplierClicked(object s,EventArgs e){_editId=0;LblModalTitle.Text="Tambah Supplier";TxtNama.Text=TxtTelepon.Text=TxtEmail.Text=TxtAlamat.Text="";ModalLayout.IsVisible=true;}
    void OnEditClicked(object s,EventArgs e){if((s as Button)?.CommandParameter is not SupplierItem x)return;_editId=x.Id;LblModalTitle.Text="Edit Supplier";TxtNama.Text=x.NamaSupplier;TxtTelepon.Text=x.Telepon;TxtEmail.Text=x.Email;TxtAlamat.Text=x.Alamat;ModalLayout.IsVisible=true;}
    async void OnSimpanSupplierClicked(object s,EventArgs e){if(string.IsNullOrWhiteSpace(TxtNama.Text)){await DisplayAlert("Peringatan","Nama supplier wajib diisi.","OK");return;}try{using var c=new MySqlConnection(MauiProgram.ConnectionString);await c.OpenAsync();var sql=_editId==0?"INSERT INTO suppliers(NamaSupplier,Telepon,Alamat,Email) VALUES(@n,@t,@a,@e)":"UPDATE suppliers SET NamaSupplier=@n,Telepon=@t,Alamat=@a,Email=@e WHERE Id=@id";using var cmd=new MySqlCommand(sql,c);cmd.Parameters.AddWithValue("@n",TxtNama.Text.Trim());cmd.Parameters.AddWithValue("@t",TxtTelepon.Text?.Trim()??"");cmd.Parameters.AddWithValue("@a",TxtAlamat.Text?.Trim()??"");cmd.Parameters.AddWithValue("@e",TxtEmail.Text?.Trim()??"");if(_editId>0)cmd.Parameters.AddWithValue("@id",_editId);await cmd.ExecuteNonQueryAsync();ModalLayout.IsVisible=false;await LoadAsync();await DisplayAlert("Berhasil",_editId==0?"Supplier berhasil ditambahkan.":"Supplier berhasil diperbarui.","OK");}catch(Exception ex){await DisplayAlert("Gagal Simpan",ex.Message,"OK");}}
    async void OnHapusClicked(object s,EventArgs e){if((s as Button)?.CommandParameter is not SupplierItem x)return;if(!await DisplayAlert("Hapus",$"Hapus supplier {x.NamaSupplier}?","Ya","Batal"))return;try{using var c=new MySqlConnection(MauiProgram.ConnectionString);await c.OpenAsync();using var cmd=new MySqlCommand("DELETE FROM suppliers WHERE Id=@id",c);cmd.Parameters.AddWithValue("@id",x.Id);await cmd.ExecuteNonQueryAsync();await LoadAsync();await DisplayAlert("Berhasil",$"Supplier {x.NamaSupplier} berhasil dihapus.","OK");}catch(Exception ex){await DisplayAlert("Gagal Hapus",ex.Message,"OK");}}
    void OnBatalSupplierClicked(object s,EventArgs e)=>ModalLayout.IsVisible=false;
}
