using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataTransferAndIntegrationSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferLogRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransferLogId",
                table: "ErrorLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_TransferLogId",
                table: "ErrorLogs",
                column: "TransferLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_TransferLogs_TransferLogId",
                table: "ErrorLogs",
                column: "TransferLogId",
                principalTable: "TransferLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_TransferLogs_TransferLogId",
                table: "ErrorLogs");

            migrationBuilder.DropIndex(
                name: "IX_ErrorLogs_TransferLogId",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "TransferLogId",
                table: "ErrorLogs");
        }
    }
}
