# ApotekApp - Versi Stabil

Platform: .NET MAUI Windows / .NET 8
Database: MySQL/MariaDB `db_apotek`

## Yang diperbaiki
- Satu alur navigasi: MainPage menjadi host menu.
- Login menyimpan user, nama, dan role.
- Koneksi MySQL dipusatkan di MauiProgram.ConnectionString.
- Dashboard memuat statistik saat menu dibuka.
- Master Obat: tampil, cari, tambah, edit, hapus, import/export CSV.
- Master Supplier: tampil, cari, tambah, edit, hapus.
- Master User: tampil, cari, tambah, edit, hapus dan role.
- Pembelian: pilih supplier, tambah item, simpan transaksi, detail, stok otomatis bertambah.
- Penjualan memakai tabel `penjualan` + `detail_penjualan` dan stok otomatis berkurang.
- Laporan membaca tabel transaksi yang benar (`penjualan` dan `pembelians`).
- Setting tetap menggunakan Preferences dan digunakan oleh transaksi.
- Startup membuat tabel `detail_pembelians` bila belum ada.

## Database
1. Jalankan MySQL/MariaDB dari XAMPP atau Laragon.
2. Import `db_apotek.sql` melalui phpMyAdmin.
3. Pastikan database bernama `db_apotek`.
4. Default login:
   - admin / admin123
   - apoteker / apo123
   - kasir / kasir123

Jika password/username berbeda di database, gunakan data yang ada.

## Visual Studio
Buka `ApotekApp.csproj`, pilih Windows Machine, lalu Run.

Catatan:
- Versi ini ditujukan untuk Windows karena project menggunakan target `net8.0-windows10.0.19041.0`.
- Printer struk tetap bergantung pada driver printer Windows.
