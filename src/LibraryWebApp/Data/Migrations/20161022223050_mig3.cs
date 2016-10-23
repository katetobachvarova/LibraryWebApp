using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryWebApp.Data.Migrations
{
    public partial class mig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemMovements_AspNetUsers_LibrarianId1",
                table: "ItemMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemMovements_AspNetUsers_UserId",
                table: "ItemMovements");

            migrationBuilder.DropIndex(
                name: "IX_ItemMovements_LibrarianId1",
                table: "ItemMovements");

            migrationBuilder.DropIndex(
                name: "IX_ItemMovements_UserId",
                table: "ItemMovements");

            migrationBuilder.DropColumn(
                name: "LibrarianId1",
                table: "ItemMovements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ItemMovements");

            migrationBuilder.AlterColumn<string>(
                name: "LibrarianId",
                table: "ItemMovements",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ItemMovements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemMovements_ApplicationUserId",
                table: "ItemMovements",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMovements_LibrarianId",
                table: "ItemMovements",
                column: "LibrarianId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemMovements_AspNetUsers_ApplicationUserId",
                table: "ItemMovements",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemMovements_AspNetUsers_LibrarianId",
                table: "ItemMovements",
                column: "LibrarianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemMovements_AspNetUsers_ApplicationUserId",
                table: "ItemMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemMovements_AspNetUsers_LibrarianId",
                table: "ItemMovements");

            migrationBuilder.DropIndex(
                name: "IX_ItemMovements_ApplicationUserId",
                table: "ItemMovements");

            migrationBuilder.DropIndex(
                name: "IX_ItemMovements_LibrarianId",
                table: "ItemMovements");

            migrationBuilder.AddColumn<string>(
                name: "LibrarianId1",
                table: "ItemMovements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ItemMovements",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LibrarianId",
                table: "ItemMovements",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "ItemMovements",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_ItemMovements_LibrarianId1",
                table: "ItemMovements",
                column: "LibrarianId1");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMovements_UserId",
                table: "ItemMovements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemMovements_AspNetUsers_LibrarianId1",
                table: "ItemMovements",
                column: "LibrarianId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemMovements_AspNetUsers_UserId",
                table: "ItemMovements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
