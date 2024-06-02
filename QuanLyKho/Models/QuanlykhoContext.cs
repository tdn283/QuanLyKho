using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyKho.Models;

public partial class QuanlykhoContext : DbContext
{
    public QuanlykhoContext()
    {
    }

    public QuanlykhoContext(DbContextOptions<QuanlykhoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }

    public virtual DbSet<ChiTietPhieuXuat> ChiTietPhieuXuats { get; set; }

    public virtual DbSet<DanhMucThietBi> DanhMucThietBis { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhieuNhapHang> PhieuNhapHangs { get; set; }

    public virtual DbSet<PhieuXuatHang> PhieuXuatHangs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThongTinThietBi> ThongTinThietBis { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuNhap, e.MaThietBi });
            entity
                .ToTable("ChiTietPhieuNhap");


            entity.Property(e => e.DonGia)
                .HasColumnType("money")
                .HasColumnName("donGia");
            entity.Property(e => e.MaPhieuNhap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maPhieuNhap");
            entity.Property(e => e.MaThietBi)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maThietBi");

            entity.HasOne(d => d.MaPhieuNhapNavigation)
                .WithMany(p => p.ChiTietPhieuNhaps)
                .HasForeignKey(d => d.MaPhieuNhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPhieuNhap_PhieuNhapHang");

            entity.HasOne(d => d.MaThietBiNavigation)
                .WithMany(p => p.ChiTietPhieuNhaps)
                .HasForeignKey(d => d.MaThietBi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPhieuNhap_ThongTinThietBi");
        });

        modelBuilder.Entity<ChiTietPhieuXuat>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuXuat, e.MaThietBi });
            entity
                .ToTable("ChiTietPhieuXuat");

            entity.Property(e => e.DonGia)
                .HasColumnType("money")
                .HasColumnName("donGia");
            entity.Property(e => e.MaPhieuXuat)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maPhieuXuat");
            entity.Property(e => e.MaThietBi)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maThietBi");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");

            entity.HasOne(d => d.MaPhieuXuatNavigation)
                .WithMany(p => p.ChiTietPhieuXuats)
                .HasForeignKey(d => d.MaPhieuXuat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPhieuXuat_PhieuXuatHang");

            entity.HasOne(d => d.MaThietBiNavigation)
                .WithMany(p => p.ChiTietPhieuXuats)
                .HasForeignKey(d => d.MaThietBi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPhieuXuat_ThongTinThietBi");
        });

        modelBuilder.Entity<DanhMucThietBi>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMucT__6B0F914CC5325A74");

            entity.ToTable("DanhMucThietBi");

            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maDanhMuc");
            entity.Property(e => e.MoTa)
                .HasColumnType("text")
                .HasColumnName("moTa");
            entity.Property(e => e.TenDanhMuc)
                .HasMaxLength(20)
                .HasColumnName("tenDanhMuc");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__NhaCungC__D0B4D6DE7CFA4E94");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.MaNhaCungCap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maNhaCungCap");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(200)
                .HasColumnName("diaChi");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("soDienThoai");
            entity.Property(e => e.TenNhaCungCap)
                .HasMaxLength(100)
                .HasColumnName("tenNhaCungCap");
        });

        modelBuilder.Entity<PhieuNhapHang>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhap).HasName("PK__PhieuNha__E27639348A6EC541");

            entity.ToTable("PhieuNhapHang");

            entity.Property(e => e.MaPhieuNhap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maPhieuNhap");
            entity.Property(e => e.GhiChu)
                .HasColumnType("text")
                .HasColumnName("ghiChu");
            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maNguoiDung");
            entity.Property(e => e.NgayNhap)
                .HasColumnType("date")
                .HasColumnName("ngayNhap");
            entity.Property(e => e.TongTien)
                .HasColumnType("money")
                .HasColumnName("tongTien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .IsUnicode(true)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhieuNhapHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PhieuNhap__maNgu__34C8D9D1");
        });

        modelBuilder.Entity<PhieuXuatHang>(entity =>
        {
            entity.HasKey(e => e.MaPhieuXuat).HasName("PK__PhieuXua__2A661240FDB13C39");

            entity.ToTable("PhieuXuatHang");

            entity.Property(e => e.MaPhieuXuat)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maPhieuXuat");
            entity.Property(e => e.GhiChu)
                .HasColumnType("text")
                .HasColumnName("ghiChu");
            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maNguoiDung");
            entity.Property(e => e.NgayXuat)
                .HasColumnType("date")
                .HasColumnName("ngayXuat");
            entity.Property(e => e.NhanVienXuat)
                .HasMaxLength(100)
                .HasColumnName("nhanVienXuat");
            entity.Property(e => e.TongTien)
                .HasColumnType("money")
                .HasColumnName("tongTien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .IsUnicode(true)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhieuXuatHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PhieuXuat__maNgu__3B75D760");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__TaiKhoan__446439EAF452C162");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.Email, "UQ_Taikhoan_Email").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__59267D4A0852A008").IsUnique();

            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maNguoiDung");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.HoVaTen)
                .HasMaxLength(30)
                .HasColumnName("hoVaTen");
            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maVaiTro");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("matKhau");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("soDienThoai");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("tenDangNhap");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK__TaiKhoan__trangT__286302EC");
        });

        modelBuilder.Entity<ThongTinThietBi>(entity =>
        {
            entity.HasKey(e => e.MaThietBi).HasName("PK__ThongTin__DC419B30B30F8AB0");

            entity.ToTable("ThongTinThietBi");

            entity.Property(e => e.MaThietBi)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maThietBi");
            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maDanhMuc");
            entity.Property(e => e.MaNhaCungCap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maNhaCungCap");
            entity.Property(e => e.MoTa)
                .HasColumnType("text")
                .HasColumnName("moTa");
            entity.Property(e => e.Model)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("model");
            entity.Property(e => e.NhaSanXuat)
                .HasMaxLength(100)
                .HasColumnName("nhaSanXuat");
            entity.Property(e => e.SoLuongCon).HasColumnName("soLuongCon");
            entity.Property(e => e.TenThietBi)
                .HasMaxLength(30)
                .HasColumnName("tenThietBi");
            entity.Property(e => e.ThoiGianBaoHanh)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("thoiGianBaoHanh");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasColumnName("trangThai");
            entity.Property(e => e.GiaBan)
                .HasColumnType("money")
                .HasColumnName("giaBan");

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.ThongTinThietBis)
                .HasForeignKey(d => d.MaDanhMuc)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ThongTinT__maDan__30F848ED");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.ThongTinThietBis)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ThongTinT__maNha__31EC6D26");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__BFC88AB70E796BBA");

            entity.ToTable("VaiTro");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("maVaiTro");
            entity.Property(e => e.MoTa)
                .HasColumnType("text")
                .HasColumnName("moTa");
            entity.Property(e => e.TenVaiTro)
                .HasMaxLength(20)
                .HasColumnName("tenVaiTro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
