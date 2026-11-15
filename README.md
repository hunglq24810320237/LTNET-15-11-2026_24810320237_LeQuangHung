# BÁO CÁO BÀI TẬP / ĐỒ ÁN

## THÔNG TIN SINH VIÊN
- **Họ và tên:** Lê Quang Hưng
- **Mã số sinh viên:** 24810320237
- **Lớp:** D19QTANM1
- **Tên môn học:** Lập trình .NET (C# / Windows Forms)
- **Tên bài tập:** Bài 6.6, 6.7, 6.8

## CẤU TRÚC DỰ ÁN

Solution `LTNET-15-11-2026_24810320237_LeQuangHung.sln` gồm 3 project Windows Forms (.NET 10). Mỗi project tổ chức theo các lớp DTO, DAL, BUS và GUI, kết nối SQL Server bằng `Microsoft.Data.SqlClient`.

| Project | Cơ sở dữ liệu | Nội dung |
|---|---|---|
| `LibraryManager` | `LibraryDB` | Quản lý đầu sách, độc giả, phiếu mượn/trả, lọc trạng thái, quá hạn và thống kê theo thể loại |
| `ClinicManager` | `ClinicDB` | Tiếp nhận bệnh nhân, hồ sơ khám, quản lý thuốc, kê đơn, xuất kho và thanh toán |
| `BilliardsManager` | `BilliardsDB` | Sơ đồ bàn, mở bàn, gọi dịch vụ bằng `SqlDataAdapter`/`DataTable`, tính giờ và thanh toán |

---

## CÀI ĐẶT VÀ CHẠY

1. Mở SQL Server Management Studio và thực thi lần lượt `Database/LibraryDB.sql`, `Database/ClinicDB.sql`, `Database/BilliardsDB.sql` trên instance `localhost\SQLEXPRESS` (Windows Authentication). Các script tạo bảng, quan hệ và dữ liệu mẫu tiếng Việt.
2. Mở `src/LTNET-15-11-2026_24810320237_LeQuangHung.sln` bằng Visual Studio 2026.
3. Chọn project cần chạy làm **Startup Project**, sau đó nhấn **F5**. Connection string hiện cấu hình cho `localhost\SQLEXPRESS`; nếu dùng instance khác, sửa hằng `Cs` trong repository tương ứng.

## KẾT QUẢ THỰC HÀNH

## Bài 6.6: Hệ thống quản lý mượn trả sách thư viện

### 1. Màn hình Chính / Tổng quan
![Màn hình chính](./images/Bai6.6/01_main_screen.jpg)

*Thông tin thư viện và tổng quan số liệu lấy từ LibraryDB.*

### 2. Màn hình Quản lý Danh mục / Thêm Sửa Xóa (CRUD)
![Màn hình CRUD](./images/Bai6.6/02_crud_screen.jpg)

*Quản lý đầu sách và số lượng sách còn trong kho.*

### 3. Màn hình Thực thi Nghiệp vụ chính
![Màn hình nghiệp vụ](./images/Bai6.6/03_transaction_screen.jpg)

*Tạo phiếu mượn có chi tiết sách và lưu vào cơ sở dữ liệu.*

### 4. Màn hình Báo cáo - Thống kê
![Màn hình thống kê](./images/Bai6.6/04_report_screen.jpg)

*Danh sách sách/phiếu quá hạn được truy vấn và hiển thị trên form.*

### 5. Minh họa xử lý lỗi / bảo vệ giao dịch
![Minh họa xử lý lỗi](./images/Bai6.6/05_error_handling.jpg)

*Từ chối lập phiếu khi thẻ độc giả hết hạn.*

---

## Bài 6.7: Hệ thống quản lý phòng khám và kê đơn thuốc

### 1. Màn hình Chính / Tổng quan
![Màn hình chính](./images/Bai6.7/01_main_screen.jpg)

*Tiếp nhận bệnh nhân và lập hồ sơ khám.*

### 2. Màn hình Quản lý Danh mục / Thêm Sửa Xóa (CRUD)
![Màn hình CRUD](./images/Bai6.7/02_crud_screen.jpg)

*Danh mục thuốc, đơn giá và số lượng tồn kho.*

### 3. Màn hình Thực thi Nghiệp vụ chính
![Màn hình nghiệp vụ](./images/Bai6.7/03_transaction_screen.jpg)

*Lưu đơn, xuất kho và hiển thị hóa đơn chờ thanh toán.*

### 4. Màn hình Báo cáo - Thống kê
![Màn hình thống kê](./images/Bai6.7/04_report_screen.jpg)

*Danh sách hồ sơ khám lấy từ ClinicDB.*

### 5. Minh họa chống lỗi Giao dịch (Rollback)
![Minh họa xử lý lỗi](./images/Bai6.7/05_error_handling.jpg)

*Kê vượt tồn kho bị từ chối; thao tác lưu đơn dùng `SqlTransaction` để rollback khi dữ liệu không hợp lệ.*

---

## Bài 6.8: Hệ thống quản lý bàn bida và dịch vụ giải trí

### 1. Màn hình Chính / Tổng quan
![Màn hình chính](./images/Bai6.8/01_main_screen.jpg)

*Sơ đồ bàn trực quan đổi màu theo trạng thái trống/đang chơi.*

### 2. Màn hình Quản lý Danh mục / Thêm Sửa Xóa (CRUD)
![Màn hình CRUD](./images/Bai6.8/02_crud_screen.jpg)

*Danh mục đồ uống, món ăn và dịch vụ thuê; hiển thị đơn giá và tồn kho.*

### 3. Màn hình Thực thi Nghiệp vụ chính
![Màn hình nghiệp vụ](./images/Bai6.8/03_transaction_screen.jpg)

*Mở bàn, lưu chi tiết gọi món và thanh toán hóa đơn.*

### 4. Màn hình Báo cáo - Thống kê
![Màn hình thống kê](./images/Bai6.8/04_report_screen.jpg)

*Lịch sử hóa đơn, tiền giờ chơi, tiền dịch vụ và tổng tiền.*

### 5. Minh họa chống SQL Injection hoặc Báo lỗi Giao dịch (Rollback)
![Minh họa xử lý lỗi](./images/Bai6.8/05_error_handling.jpg)

*Thanh toán bị hủy khi tồn kho không đủ; dữ liệu được bảo vệ trong giao dịch SQL.*
