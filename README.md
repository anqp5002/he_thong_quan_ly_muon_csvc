# H? Th?ng Qu?n Lý Mu?n Co S? V?t Ch?t - PTIT HCM

D? án phát tri?n ?ng d?ng desktop h? tr? qu?n lý, mu?n và tr? co s? v?t ch?t cho H?c vi?n Công ngh? Buu chính Vi?n thông co s? TP.HCM.

## T?ng quan
H? th?ng du?c xây d?ng d?a trên ki?n trúc 3 l?p, s? d?ng mô hình MVVM (Model-View-ViewModel) d? d?m b?o tính d? b?o trì và m? r?ng. 

## Công ngh? s? d?ng
- Ngôn ng?: C# 12
- Framework: .NET 8 LTS
- Giao di?n: WPF k?t h?p v?i MaterialDesignInXamlToolkit
- Co s? d? li?u: SQL Server 2022 (ch?y trên Docker)
- ORM: Entity Framework Core 8
- Ki?n trúc: Dependency Injection, MVVM

## Phân quy?n ngu?i dùng
H? th?ng h? tr? 4 vai trò chính:
1. Sinh viên: Tra c?u và dang ký mu?n co s? v?t ch?t trong gi? h?c.
2. Ðoàn th? (Bí thu Ðoàn, Ch? nhi?m CLB): T?o don mu?n co s? v?t ch?t ngoài gi?.
3. Qu?n lý Co s? v?t ch?t: Duy?t don, bàn giao, nh?n tr? và x? lý s? c?.
4. Admin: Qu?n lý tài kho?n, danh m?c tài s?n, và c?u hình h? th?ng.

## Các tính nang chính
- Xác th?c và phân quy?n ngu?i dùng.
- Qu?n lý danh m?c tài s?n, phòng h?c.
- T?o và xét duy?t don dang ký mu?n co s? v?t ch?t.
- Ghi nh?n bàn giao và nh?n tr? tài s?n.
- Qu?n lý s? c? hu h?ng, th?t thoát tài s?n.
- Th?ng kê, báo cáo ho?t d?ng.

## Cách kh?i ch?y h? th?ng
1. Clone repository v? máy.
2. S? d?ng Docker Compose d? kh?i t?o SQL Server.
3. Ch?y ?ng d?ng t? project App d? Entity Framework t? d?ng t?o database và seed d? li?u m?u.
4. Ðang nh?p v?i các tài kho?n m?u dã du?c cung c?p d? tr?i nghi?m các ch?c nang.

## Tài kho?n Demo
H? th?ng dã du?c kh?i t?o (seed) s?n m?t s? tài kho?n theo các phân quy?n d? thu?n ti?n cho vi?c test:

- **Admin**: admin@ptithcm.edu.vn / M?t kh?u: admin123
- **Qu?n lý CSVC**: anpham5002@gmail.com, habt@ptithcm.edu.vn / M?t kh?u: 123456
- **Sinh viên**: n23dccn001@student.ptithcm.edu.vn, n23dccn002@student.ptithcm.edu.vn / M?t kh?u: 123456
- **Gi?ng viên**: chaupm@ptithcm.edu.vn, dunglh@ptithcm.edu.vn / M?t kh?u: 123456
- **Ðoàn th? (CLB, Ð?i nhóm)**: n22dccn010@student.ptithcm.edu.vn (LCÐ), n23dccn020@student.ptithcm.edu.vn (CLB) / M?t kh?u: 123456

