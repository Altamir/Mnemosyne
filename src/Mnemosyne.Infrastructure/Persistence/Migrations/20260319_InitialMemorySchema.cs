using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mnemosyne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMemorySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mnemosyne");

            migrationBuilder.CreateTable(
                name: "memories",
                schema: "mnemosyne",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memories", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "memories",
                schema: "mnemosyne");
        }
    }
}
