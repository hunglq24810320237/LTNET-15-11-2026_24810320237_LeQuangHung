IF DB_ID(N'LibraryDB') IS NULL CREATE DATABASE LibraryDB;
GO
USE LibraryDB;
GO
IF OBJECT_ID(N'dbo.DocGia',N'U') IS NULL
 CREATE TABLE dbo.DocGia(MaDocGia INT IDENTITY PRIMARY KEY,HoTen NVARCHAR(100) NOT NULL,Email VARCHAR(100) NOT NULL UNIQUE,NgayHetHanThe DATETIME NOT NULL);
IF OBJECT_ID(N'dbo.DauSach',N'U') IS NULL
 CREATE TABLE dbo.DauSach(MaSach INT IDENTITY PRIMARY KEY,TenSach NVARCHAR(200) NOT NULL,TheLoai NVARCHAR(50) NOT NULL,SoLuongCon INT NOT NULL CHECK(SoLuongCon>=0));
IF OBJECT_ID(N'dbo.PhieuMuon',N'U') IS NULL
 CREATE TABLE dbo.PhieuMuon(MaPhieu INT IDENTITY PRIMARY KEY,MaDocGia INT NOT NULL REFERENCES dbo.DocGia(MaDocGia),NgayMuon DATETIME NOT NULL DEFAULT GETDATE(),HanTra DATETIME NOT NULL,TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Đang mượn');
IF OBJECT_ID(N'dbo.ChiTietMuon',N'U') IS NULL
 CREATE TABLE dbo.ChiTietMuon(MaPhieu INT NOT NULL REFERENCES dbo.PhieuMuon(MaPhieu),MaSach INT NOT NULL REFERENCES dbo.DauSach(MaSach),SoLuong INT NOT NULL CHECK(SoLuong>0),CONSTRAINT PK_ChiTietMuon PRIMARY KEY(MaPhieu,MaSach));
IF NOT EXISTS(SELECT 1 FROM dbo.DocGia) INSERT dbo.DocGia(HoTen,Email,NgayHetHanThe) VALUES
(N'Nguyễn Minh Anh','minhanh@library.vn',DATEADD(year,1,GETDATE())),(N'Trần Quốc Bảo','quocbao@library.vn',DATEADD(month,6,GETDATE())),(N'Lê Thu Hà','thuha@library.vn',DATEADD(day,-10,GETDATE()));
IF NOT EXISTS(SELECT 1 FROM dbo.DauSach) INSERT dbo.DauSach(TenSach,TheLoai,SoLuongCon) VALUES
(N'Lập trình C# căn bản',N'Công nghệ thông tin',8),(N'Cơ sở dữ liệu SQL Server',N'Công nghệ thông tin',5),(N'Kinh tế học vi mô',N'Kinh tế',6),(N'Dế Mèn phiêu lưu ký',N'Văn học',9),(N'Truyện Kiều',N'Văn học',4);
IF NOT EXISTS(SELECT 1 FROM dbo.PhieuMuon) BEGIN
 DECLARE @reader INT=(SELECT MIN(MaDocGia) FROM dbo.DocGia),@book1 INT=(SELECT MIN(MaSach) FROM dbo.DauSach),@book2 INT=(SELECT MAX(MaSach) FROM dbo.DauSach);
 INSERT dbo.PhieuMuon(MaDocGia,NgayMuon,HanTra,TrangThai) VALUES(@reader,DATEADD(day,-20,GETDATE()),DATEADD(day,-6,GETDATE()),N'Đang mượn'),(@reader,DATEADD(day,-2,GETDATE()),DATEADD(day,12,GETDATE()),N'Đang mượn');
 INSERT dbo.ChiTietMuon(MaPhieu,MaSach,SoLuong) SELECT MIN(MaPhieu),@book1,1 FROM dbo.PhieuMuon;
 INSERT dbo.ChiTietMuon(MaPhieu,MaSach,SoLuong) SELECT MAX(MaPhieu),@book2,1 FROM dbo.PhieuMuon;
 UPDATE dbo.DauSach SET SoLuongCon=SoLuongCon-1 WHERE MaSach IN(@book1,@book2);
END;
GO
