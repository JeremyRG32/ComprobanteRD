using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComprobanteRDAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddWhatsAppPhoneNumberIdToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WhatsAppPhoneNumberId",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhatsAppPhoneNumberId",
                table: "Companies");
        }
    }
}
