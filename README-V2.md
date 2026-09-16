# ApotekApp v2.0 – MYSQL / phpMyAdmin

Perbaikan utama:
- Upload logo menyimpan file lokal DAN binary `logo_data` ke database.
- Logo dibaca kembali dari database saat membuka Pengaturan.
- Pesan status database dibuat jujur; tidak lagi menampilkan pesan palsu.
- Pengaturan nama/alamat/telepon disimpan ke database.
- MainPage menunggu proses LoadSettingsAsync agar status tidak race.
- Ikon PNG tetap menggunakan file lokal di Resources/Images.

## Database
Jalankan Laragon/XAMPP dan pastikan database `db_apotek` tersedia. Jika database lama belum memiliki kolom logo:
`ALTER TABLE pengaturan ADD COLUMN logo_data MEDIUMBLOB NULL;`

## Login
admin / admin123
