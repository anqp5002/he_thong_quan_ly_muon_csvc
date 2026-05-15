# Hệ Thống Quản Lý Mượn Cơ Sở Vật Chất - PTIT HCM

Dự án phát triển ứng dụng desktop hỗ trợ quản lý, mượn và trả cơ sở vật chất cho Học viện Công nghệ Bưu chính Viễn thông cơ sở TP.HCM.

## Tổng quan
Hệ thống được xây dựng dựa trên kiến trúc 3 lớp, sử dụng mô hình MVVM (Model-View-ViewModel) để đảm bảo tính dễ bảo trì và mở rộng. 

## Công nghệ sử dụng
- Ngôn ngữ: C# 12
- Framework: .NET 8 LTS
- Giao diện: WPF kết hợp với MaterialDesignInXamlToolkit
- Cơ sở dữ liệu: SQL Server 2022 (chạy trên Docker)
- ORM: Entity Framework Core 8
- Kiến trúc: Dependency Injection, MVVM

## Phân quyền người dùng
Hệ thống hỗ trợ 4 vai trò chính:
1. Sinh viên: Tra cứu và đăng ký mượn cơ sở vật chất trong giờ học.
2. Đoàn thể (Bí thư Đoàn, Chủ nhiệm CLB): Tạo đơn mượn cơ sở vật chất ngoài giờ.
3. Quản lý Cơ sở vật chất: Duyệt đơn, bàn giao, nhận trả và xử lý sự cố.
4. Admin: Quản lý tài khoản, danh mục tài sản, và cấu hình hệ thống.

## Các tính năng chính
- Xác thực và phân quyền người dùng.
- Quản lý danh mục tài sản, phòng học.
- Tạo và xét duyệt đơn đăng ký mượn cơ sở vật chất.
- Ghi nhận bàn giao và nhận trả tài sản.
- Quản lý sự cố hư hỏng, thất thoát tài sản.
- Thống kê, báo cáo hoạt động.

## Cách khởi chạy hệ thống
1. Clone repository về máy.
2. Sử dụng Docker Compose để khởi tạo SQL Server.
3. Chạy ứng dụng từ project App để Entity Framework tự động tạo database và seed dữ liệu mẫu.
4. Đăng nhập với các tài khoản mẫu đã được cung cấp để trải nghiệm các chức năng.
