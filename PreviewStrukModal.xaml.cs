using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace ApotekApp;

public class StrukItemModel
{
    public string NamaObat { get; set; } = "";
    public int Qty { get; set; }
    public string Harga { get; set; } = "0";
    public string Subtotal { get; set; } = "0";
}

public class StrukDataModel
{
    public string NoNota { get; set; } = "";
    public string Tanggal { get; set; } = "";
    public List<StrukItemModel> Items { get; set; } = new();
    public string Subtotal { get; set; } = "Rp 0";
    public string Diskon { get; set; } = "Rp 0";
    public string Ppn { get; set; } = "Rp 0";
    public string GrandTotal { get; set; } = "Rp 0";
    public string Bayar { get; set; } = "Rp 0";
    public string Kembali { get; set; } = "Rp 0";
}

public partial class PreviewStrukModal : ContentView
{
    private StrukDataModel? _currentData;

    public PreviewStrukModal()
    {
        InitializeComponent();
    }

    public void RenderStrukData(StrukDataModel data)
    {
        _currentData = data;

        // 1. AMBIL HEADER DARI SETTING (Preferences)
        string namaApotek = Preferences.Get("NamaApotek", "APOTEK SEHAT");
        string alamatApotek = Preferences.Get("AlamatApotek", "Jl. Kesehatan No. 1, Jakarta");
        string telpApotek = Preferences.Get("TeleponApotek", "0812-3456-7890");
        string logoPath = Preferences.Get("AppLogoPath", string.Empty);

        LblNamaApotek.Text = string.IsNullOrWhiteSpace(namaApotek) ? "APOTEK SEHAT" : namaApotek;
        LblAlamatApotek.Text = string.IsNullOrWhiteSpace(alamatApotek) ? "-" : alamatApotek;
        LblTelpApotek.Text = $"Telp: {telpApotek}";

        if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
        {
            ImgLogoStruk.Source = ImageSource.FromFile(logoPath);
            ImgLogoStruk.IsVisible = true;
        }
        else
        {
            ImgLogoStruk.IsVisible = false;
        }

        // 2. ISIKAN DATA NOTA & KEUANGAN
        LblNoNota.Text = data.NoNota;
        LblTanggal.Text = string.IsNullOrWhiteSpace(data.Tanggal) ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : data.Tanggal;
        LblSubtotal.Text = data.Subtotal;
        LblDiskon.Text = data.Diskon;
        LblPpn.Text = data.Ppn;
        LblGrandTotal.Text = data.GrandTotal;
        LblBayar.Text = data.Bayar;
        LblKembali.Text = data.Kembali;

        // 3. ISIKAN ITEM LIST KE UI
        ContainerStrukItems.Children.Clear();
        foreach (var item in data.Items)
        {
            var itemLayout = new VerticalStackLayout { Spacing = 2 };
            itemLayout.Children.Add(new Label
            {
                Text = item.NamaObat,
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#111827")
            });

            itemLayout.Children.Add(new Label
            {
                Text = $"{item.Qty} x Rp {item.Harga} = Rp {item.Subtotal}",
                FontSize = 10,
                TextColor = Color.FromArgb("#4B5563")
            });

            ContainerStrukItems.Children.Add(itemLayout);
        }

        this.IsVisible = true;
    }

    private void OnBatalClicked(object sender, EventArgs e)
    {
        this.IsVisible = false;
    }

    private async void OnCetakStrukClicked(object sender, EventArgs e)
    {
        if (_currentData == null) return;

        try
        {
            string namaApotek = Preferences.Get("NamaApotek", "APOTEK SEHAT");
            string alamatApotek = Preferences.Get("AlamatApotek", "Jl. Kesehatan No. 1, Jakarta");
            string telpApotek = Preferences.Get("TeleponApotek", "0812-3456-7890");
            string logoPath = Preferences.Get("AppLogoPath", string.Empty);

            string logoHtml = "";
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                byte[] imageBytes = await File.ReadAllBytesAsync(logoPath);
                string base64Image = Convert.ToBase64String(imageBytes);
                logoHtml = $"<img src='data:image/png;base64,{base64Image}' style='max-height:60px; margin-bottom:5px;' /><br/>";
            }

            var itemRowsHtml = new System.Text.StringBuilder();
            foreach (var item in _currentData.Items)
            {
                itemRowsHtml.AppendLine($@"
                    <div style='margin-bottom: 5px;'>
                        <div style='font-weight: bold;'>{item.NamaObat}</div>
                        <div>{item.Qty} x Rp {item.Harga} = Rp {item.Subtotal}</div>
                    </div>");
            }

            // DOKUMEN STRUK THERMAL METODE CETAK
            string htmlContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'/>
                <title>Struk Pembayaran</title>
                <style>
                    @page {{ size: 80mm auto; margin: 0; }}
                    body {{
                        font-family: 'Courier New', Courier, monospace;
                        width: 72mm;
                        margin: 0 auto;
                        padding: 10px;
                        font-size: 11px;
                        color: #000;
                    }}
                    .text-center {{ text-align: center; }}
                    .line {{ border-bottom: 1px dashed #000; margin: 6px 0; }}
                    .flex {{ display: flex; justify-content: space-between; }}
                    .bold {{ font-weight: bold; }}
                </style>
                <script>
                    window.onload = function() {{
                        window.print();
                    }};
                </script>
            </head>
            <body>
                <div class='text-center'>
                    {logoHtml}
                    <div style='font-size:14px; font-weight:bold;'>{namaApotek}</div>
                    <div>{alamatApotek}</div>
                    <div>Telp: {telpApotek}</div>
                </div>

                <div class='line'></div>

                <div>Nota: {_currentData.NoNota}</div>
                <div>Tgl : {_currentData.Tanggal}</div>

                <div class='line'></div>

                {itemRowsHtml}

                <div class='line'></div>

                <div class='flex'><span>Subtotal</span><span>{_currentData.Subtotal}</span></div>
                <div class='flex'><span>Diskon</span><span>{_currentData.Diskon}</span></div>
                <div class='flex'><span>PPN</span><span>{_currentData.Ppn}</span></div>
                <div class='flex bold' style='font-size:12px;'><span>Grand Total</span><span>{_currentData.GrandTotal}</span></div>
                <div class='flex'><span>Bayar</span><span>{_currentData.Bayar}</span></div>
                <div class='flex bold'><span>Kembali</span><span>{_currentData.Kembali}</span></div>

                <div class='line'></div>

                <div class='text-center' style='margin-top:10px;'>
                    -- Terima Kasih Semoga Lekas Sembuh --
                </div>
            </body>
            </html>";

            string filePath = Path.Combine(FileSystem.CacheDirectory, "struk_pembayaran.html");
            await File.WriteAllTextAsync(filePath, htmlContent);

            // BUKA STRUK KE SISTEM WINDOWS (TAMPIL DIALOG PRINT SECARA OTOMATIS)
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });

            this.IsVisible = false;
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error Cetak", ex.Message, "OK");
            }
        }
    }
}