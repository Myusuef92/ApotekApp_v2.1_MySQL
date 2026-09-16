-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Sep 11, 2026 at 06:54 PM
-- Server version: 8.0.30
-- PHP Version: 8.2.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `db_apotek`
--

-- --------------------------------------------------------

--
-- Table structure for table `detail_penjualan`
--

CREATE TABLE `detail_penjualan` (
  `id` int NOT NULL,
  `no_nota` varchar(50) NOT NULL,
  `id_obat` int NOT NULL,
  `harga` decimal(12,2) NOT NULL,
  `qty` int NOT NULL,
  `subtotal` decimal(12,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `detail_penjualan`
--

INSERT INTO `detail_penjualan` (`id`, `no_nota`, `id_obat`, `harga`, `qty`, `subtotal`) VALUES
(1, 'PJ-20260911005044', 13, 4000.00, 115, 460000.00),
(2, 'PJ-20260911005449', 13, 4000.00, 1, 4000.00),
(3, 'PJ-20260911010209', 13, 4000.00, 1, 4000.00),
(4, 'PJ-20260911012814', 10, 8500.00, 1, 8500.00),
(5, 'PJ-20260911130657', 10, 8500.00, 1, 8500.00),
(6, 'PJ-20260911132040', 5, 9500.00, 1, 9500.00),
(7, 'PJ-20260911132542', 10, 8500.00, 4, 34000.00),
(8, 'PJ-20260911232352', 13, 4000.00, 5, 20000.00);

-- --------------------------------------------------------

--
-- Table structure for table `obats`
--

CREATE TABLE `obats` (
  `Id` int NOT NULL,
  `KodeObat` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NamaObat` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Barcode` varchar(100) DEFAULT NULL,
  `Satuan` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Kategori` varchar(100) DEFAULT 'Umum',
  `LokasiRak` varchar(100) DEFAULT '-',
  `HargaBeli` decimal(18,2) NOT NULL,
  `HargaJual` decimal(18,2) NOT NULL,
  `Stok` int NOT NULL,
  `StokMin` int NOT NULL,
  `TanggalKadaluarsa` date DEFAULT NULL,
  `Gambar` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `obats`
--

INSERT INTO `obats` (`Id`, `KodeObat`, `NamaObat`, `Barcode`, `Satuan`, `Kategori`, `LokasiRak`, `HargaBeli`, `HargaJual`, `Stok`, `StokMin`, `TanggalKadaluarsa`, `Gambar`) VALUES
(1, 'OBT01', 'Paracetamol 50mg', '8,99275E+12', 'Strip', 'Analgesik', 'Rak A1', 3500.00, 5000.00, 50, 10, '2027-12-31', NULL),
(2, 'OBT02', 'Amoxicillin 500mg', '8,99275E+12', 'Strip', 'Antibiotik', 'Rak A2', 6000.00, 8500.00, 30, 5, '2027-10-15', NULL),
(3, 'OBT03', 'Bodrex Extra', '8,991E+12', 'Strip', 'Sakit Kepala', 'Rak B1', 2500.00, 4000.00, 5, 10, '2028-01-20', NULL),
(4, 'OBT04', 'Panadol Extra', '8,99274E+12', 'Strip', 'Analgesik', 'Rak B1', 10000.00, 13500.00, 25, 5, '2027-11-30', NULL),
(5, 'OBT05', 'Promag Tablet', '8,99277E+12', 'Strip', 'Obat Maag', 'Rak B2', 7000.00, 9500.00, 59, 10, '2028-03-10', NULL),
(6, 'OBT06', 'Mylanta Liquid 50ml', '8,99801E+12', 'Botol', 'Obat Maag', 'Rak B2', 15000.00, 18500.00, 15, 5, '2027-08-25', NULL),
(7, 'OBT07', 'Komix Herbal Sachet', '8,99123E+12', 'Sachet', 'Batuk & Flu', 'Rak C1', 1500.00, 2500.00, 100, 20, '2027-09-18', NULL),
(8, 'OBT08', 'Woods Peppermint 60ml', '8,993E+12', 'Botol', 'Batuk & Flu', 'Rak C1', 18000.00, 22000.00, 20, 5, '2028-02-14', NULL),
(9, 'OBT09', 'Diapet Kapsul', '8,994E+12', 'Strip', 'Obat Diare', 'Rak C2', 3000.00, 4500.00, 45, 10, '2027-07-22', NULL),
(10, 'OBT10', 'Entrostop Tablet', '8,995E+12', 'Strip', 'Obat Diare', 'Rak C2', 6500.00, 8500.00, 29, 10, '2028-04-05', NULL),
(11, 'OBT11', 'Sangobion Kapsul', '8,99276E+12', 'Strip', 'Vitamin & Suplemen', 'Rak D1', 12000.00, 15000.00, 30, 5, '2028-06-30', NULL),
(12, 'OBT12', 'Neurobion Forte', '8,99276E+12', 'Strip', 'Vitamin & Suplemen', 'Rak D1', 35000.00, 42000.00, 20, 5, '2028-05-12', NULL),
(13, 'OBT13', 'Tolak Angin Sachet', '8,996E+12', 'Sachet', '4000', 'Rak D2', 3000.00, 4000.00, 0, 30, '2027-11-11', 'C:\\Users\\User\\AppData\\Local\\Packages\\com.companyname.apotekapp_9zz4h110yvjzm\\LocalState\\5e340540-224b-4c4c-a7f0-692eedb0ad73.png');

-- --------------------------------------------------------

--
-- Table structure for table `pembelians`
--

CREATE TABLE `pembelians` (
  `IdPembelian` int NOT NULL,
  `NoFaktur` varchar(255) NOT NULL,
  `TanggalPembelian` datetime NOT NULL,
  `TotalHarga` decimal(18,2) NOT NULL,
  `Catatan` longtext,
  `SupplierId` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `pembelians`
--

INSERT INTO `pembelians` (`IdPembelian`, `NoFaktur`, `TanggalPembelian`, `TotalHarga`, `Catatan`, `SupplierId`) VALUES
(1, 'INV-SUP-01', '2026-09-11 00:00:00', 3000.00, NULL, 1);

-- --------------------------------------------------------

--
-- Table structure for table `pengaturan`
--

CREATE TABLE `pengaturan` (
  `id` int NOT NULL DEFAULT '1',
  `nama_apotek` varchar(100) NOT NULL DEFAULT 'APOTEK SEHAT',
  `nama_pemilik` varchar(100) DEFAULT 'Apt. Nama Pemilik, S.Farm',
  `alamat` text,
  `no_telepon` varchar(30) DEFAULT '0812-3456-7890',
  `sia_sipa` varchar(100) DEFAULT 'SIA: 440/001/SIA/2026',
  `catatan_struk` varchar(255) DEFAULT '-- Terima Kasih Semoga Lekas Sembuh --',
  `logo_path` text,
  `logo_data` mediumblob,
  `ukuran_kertas` varchar(10) DEFAULT '58mm',
  `pajak_ppn` decimal(5,2) DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `pengaturan`
--

INSERT INTO `pengaturan` (`id`, `nama_apotek`, `nama_pemilik`, `alamat`, `no_telepon`, `sia_sipa`, `catatan_struk`, `logo_path`, `logo_data`, `ukuran_kertas`, `pajak_ppn`) VALUES
(1, 'APOTEK SEHAT', 'Apt. Nama Pemilik, S.Farm', 'Jl. Kesehatan No. 1, Jakarta', '0812-3456-7890', 'SIA: 440/001/SIA/2026', '-- Terima Kasih Semoga Lekas Sembuh --', NULL, NULL, '58mm', 11.00);

-- --------------------------------------------------------

--
-- Table structure for table `penjualan`
--

CREATE TABLE `penjualan` (
  `id` int NOT NULL,
  `no_nota` varchar(50) NOT NULL,
  `tanggal` datetime NOT NULL,
  `total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `diskon` decimal(12,2) NOT NULL DEFAULT '0.00',
  `grand_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `bayar` decimal(12,2) NOT NULL DEFAULT '0.00',
  `kembali` decimal(12,2) NOT NULL DEFAULT '0.00'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `penjualan`
--

INSERT INTO `penjualan` (`id`, `no_nota`, `tanggal`, `total`, `diskon`, `grand_total`, `bayar`, `kembali`) VALUES
(1, 'PJ-20260911005044', '2026-09-11 00:50:44', 460000.00, 0.00, 460000.00, 500000.00, 40000.00),
(2, 'PJ-20260911005449', '2026-09-11 00:54:49', 4000.00, 0.00, 4000.00, 5000.00, 1000.00),
(3, 'PJ-20260911010209', '2026-09-11 01:02:09', 4000.00, 0.00, 4000.00, 10000.00, 6000.00),
(4, 'PJ-20260911012814', '2026-09-11 01:28:14', 8500.00, 0.00, 9435.00, 20000.00, 10565.00),
(5, 'PJ-20260911130657', '2026-09-11 13:06:57', 8500.00, 0.00, 9435.00, 20000.00, 10565.00),
(6, 'PJ-20260911132040', '2026-09-11 13:20:40', 9500.00, 0.00, 10545.00, 20000.00, 9455.00),
(7, 'PJ-20260911132542', '2026-09-11 13:25:42', 34000.00, 0.00, 37740.00, 50000.00, 12260.00),
(8, 'PJ-20260911232352', '2026-09-11 23:23:52', 20000.00, 0.00, 22200.00, 30000.00, 7800.00);

-- --------------------------------------------------------

--
-- Table structure for table `penjualans`
--

CREATE TABLE `penjualans` (
  `Id` int NOT NULL,
  `NoNota` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Tanggal` datetime(6) NOT NULL,
  `TotalHarga` decimal(18,2) NOT NULL,
  `Bayar` decimal(18,2) NOT NULL,
  `Kembali` decimal(18,2) NOT NULL,
  `UserId` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `roles`
--

CREATE TABLE `roles` (
  `id` int NOT NULL,
  `nama_role` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `roles`
--

INSERT INTO `roles` (`id`, `nama_role`) VALUES
(1, 'Admin'),
(2, 'Apoteker'),
(3, 'Kasir');

-- --------------------------------------------------------

--
-- Table structure for table `suppliers`
--

CREATE TABLE `suppliers` (
  `Id` int NOT NULL,
  `NamaSupplier` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Telepon` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Alamat` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `suppliers`
--

INSERT INTO `suppliers` (`Id`, `NamaSupplier`, `Telepon`, `Alamat`, `Email`) VALUES
(1, 'Hari', '4444444444', 'mauk', 'email@yahoo.com'),
(2, 'Yusup', '08888778787', 'Rajeg', 'suplier342@gmail.com'),
(3, 'PT Kimia Farma', '08977767669', 'Rajeg', 'farma@gmail.com');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `nama_lengkap` varchar(100) NOT NULL,
  `role_id` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `username`, `password`, `nama_lengkap`, `role_id`) VALUES
(1, 'admin', 'admin123', 'Yusuef', 1),
(2, 'apoteker', 'apo123', 'Hari', 3),
(3, 'kasir', 'kasir123', 'Kasir Shift 1', 3);

--
ALTER TABLE `detail_pembelians` ADD PRIMARY KEY (`IdDetailPembelian`), ADD KEY `IX_DetailPembelian_PembelianId` (`PembelianId`), ADD KEY `IX_DetailPembelian_ObatId` (`ObatId`);
ALTER TABLE `detail_pembelians` MODIFY `IdDetailPembelian` int NOT NULL AUTO_INCREMENT;

-- Indexes for dumped tables
--

--
-- Indexes for table `detail_penjualan`
--
ALTER TABLE `detail_penjualan`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `obats`
--
ALTER TABLE `obats`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `pembelians`
--
ALTER TABLE `pembelians`
  ADD PRIMARY KEY (`IdPembelian`),
  ADD KEY `SupplierId` (`SupplierId`);

--
-- Indexes for table `pengaturan`
--
ALTER TABLE `pengaturan`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `penjualan`
--
ALTER TABLE `penjualan`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `no_nota` (`no_nota`);

--
-- Indexes for table `penjualans`
--
ALTER TABLE `penjualans`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Penjualans_UserId` (`UserId`);

--
-- Indexes for table `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nama_role` (`nama_role`);

--
-- Indexes for table `suppliers`
--
ALTER TABLE `suppliers`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`),
  ADD KEY `fk_users_roles` (`role_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `detail_penjualan`
--
ALTER TABLE `detail_penjualan`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `obats`
--
ALTER TABLE `obats`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT for table `pembelians`
--
ALTER TABLE `pembelians`
  MODIFY `IdPembelian` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `penjualan`
--
ALTER TABLE `penjualan`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `penjualans`
--
ALTER TABLE `penjualans`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `roles`
--
ALTER TABLE `roles`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `suppliers`
--
ALTER TABLE `suppliers`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `pembelians`
--
ALTER TABLE `pembelians`
  ADD CONSTRAINT `pembelians_ibfk_1` FOREIGN KEY (`SupplierId`) REFERENCES `suppliers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `penjualans`
--
ALTER TABLE `penjualans`
  ADD CONSTRAINT `FK_Penjualans_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `fk_users_roles` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
