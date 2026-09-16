# Perbaikan Build 2026-09-12

1. DashboardPage.xaml.cs: memperbaiki `private public async Task LoadAsync()` menjadi `public async Task LoadAsync()` karena MainPage memanggil method tersebut.
2. MainLayoutPage.xaml.cs: memperbaiki nullable warning pada parameter IServiceProvider menjadi `IServiceProvider? serviceProvider = null`.

Catatan: build Windows perlu dilakukan di Visual Studio pada komputer pengembang.
