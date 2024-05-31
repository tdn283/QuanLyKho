using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Migrations
{
    /// <inheritdoc />
    public partial class updatePhieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thanhTien",
                table: "ChiTietPhieuXuat");

            migrationBuilder.DropColumn(
                name: "thanhTien",
                table: "ChiTietPhieuNhap");

            migrationBuilder.AlterColumn<string>(
                name: "moTa",
                table: "ThongTinThietBi",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "tongTien",
                table: "PhieuXuatHang",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tongTien",
                table: "PhieuNhapHang",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "donGia",
                table: "ChiTietPhieuXuat",
                type: "money",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tongTien",
                table: "PhieuXuatHang");

            migrationBuilder.DropColumn(
                name: "tongTien",
                table: "PhieuNhapHang");

            migrationBuilder.AlterColumn<string>(
                name: "moTa",
                table: "ThongTinThietBi",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "donGia",
                table: "ChiTietPhieuXuat",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AddColumn<double>(
                name: "thanhTien",
                table: "ChiTietPhieuXuat",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "thanhTien",
                table: "ChiTietPhieuNhap",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
