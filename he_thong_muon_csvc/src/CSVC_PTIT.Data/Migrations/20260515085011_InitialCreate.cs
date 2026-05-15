using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSVC_PTIT.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "asset_categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    department_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    department_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.department_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    room_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    building = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    floor_no = table.Column<int>(type: "int", nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: true),
                    room_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.room_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    borrower_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    student_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    employee_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    class_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    identity_no = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    last_login_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "department_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assets",
                columns: table => new
                {
                    asset_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    asset_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    asset_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    management_mode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    total_quantity = table.Column<int>(type: "int", nullable: false),
                    available_quantity = table.Column<int>(type: "int", nullable: false),
                    serial_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    condition_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    availability_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    current_room_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assets", x => x.asset_id);
                    table.ForeignKey(
                        name: "FK_assets_asset_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "asset_categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_assets_rooms_current_room_id",
                        column: x => x.current_room_id,
                        principalTable: "rooms",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "borrow_requests",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    request_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    contact_phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    borrow_start_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    borrow_end_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expected_return_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actual_return_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    priority_level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    request_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    reject_reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    requester_id = table.Column<int>(type: "int", nullable: false),
                    approved_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_borrow_requests", x => x.request_id);
                    table.ForeignKey(
                        name: "FK_borrow_requests_users_approved_by",
                        column: x => x.approved_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_borrow_requests_users_requester_id",
                        column: x => x.requester_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "borrow_request_assets",
                columns: table => new
                {
                    request_asset_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity_requested = table.Column<int>(type: "int", nullable: false),
                    quantity_approved = table.Column<int>(type: "int", nullable: false),
                    quantity_checked_out = table.Column<int>(type: "int", nullable: false),
                    quantity_returned = table.Column<int>(type: "int", nullable: false),
                    item_note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    request_id = table.Column<int>(type: "int", nullable: false),
                    asset_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_borrow_request_assets", x => x.request_asset_id);
                    table.ForeignKey(
                        name: "FK_borrow_request_assets_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_borrow_request_assets_borrow_requests_request_id",
                        column: x => x.request_id,
                        principalTable: "borrow_requests",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "borrow_request_rooms",
                columns: table => new
                {
                    request_room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usage_note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    request_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_borrow_request_rooms", x => x.request_room_id);
                    table.ForeignKey(
                        name: "FK_borrow_request_rooms_borrow_requests_request_id",
                        column: x => x.request_id,
                        principalTable: "borrow_requests",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_borrow_request_rooms_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "checkouts",
                columns: table => new
                {
                    checkout_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    checkout_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    checkout_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    checkout_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    request_id = table.Column<int>(type: "int", nullable: false),
                    checked_out_by = table.Column<int>(type: "int", nullable: false),
                    checked_out_to = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkouts", x => x.checkout_id);
                    table.ForeignKey(
                        name: "FK_checkouts_borrow_requests_request_id",
                        column: x => x.request_id,
                        principalTable: "borrow_requests",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkouts_users_checked_out_by",
                        column: x => x.checked_out_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkouts_users_checked_out_to",
                        column: x => x.checked_out_to,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "checkout_items",
                columns: table => new
                {
                    checkout_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    condition_before = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    is_returned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    checkout_id = table.Column<int>(type: "int", nullable: false),
                    request_asset_id = table.Column<int>(type: "int", nullable: false),
                    asset_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkout_items", x => x.checkout_item_id);
                    table.ForeignKey(
                        name: "FK_checkout_items_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkout_items_borrow_request_assets_request_asset_id",
                        column: x => x.request_asset_id,
                        principalTable: "borrow_request_assets",
                        principalColumn: "request_asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkout_items_checkouts_checkout_id",
                        column: x => x.checkout_id,
                        principalTable: "checkouts",
                        principalColumn: "checkout_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "returns",
                columns: table => new
                {
                    return_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    returned_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    return_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    checkout_id = table.Column<int>(type: "int", nullable: false),
                    received_by = table.Column<int>(type: "int", nullable: false),
                    returned_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_returns", x => x.return_id);
                    table.ForeignKey(
                        name: "FK_returns_checkouts_checkout_id",
                        column: x => x.checkout_id,
                        principalTable: "checkouts",
                        principalColumn: "checkout_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_returns_users_received_by",
                        column: x => x.received_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_returns_users_returned_by",
                        column: x => x.returned_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "return_items",
                columns: table => new
                {
                    return_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    condition_after = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    quantity_returned = table.Column<int>(type: "int", nullable: false),
                    is_damaged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_lost = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    damage_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    return_id = table.Column<int>(type: "int", nullable: false),
                    checkout_item_id = table.Column<int>(type: "int", nullable: false),
                    asset_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_return_items", x => x.return_item_id);
                    table.ForeignKey(
                        name: "FK_return_items_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_return_items_checkout_items_checkout_item_id",
                        column: x => x.checkout_item_id,
                        principalTable: "checkout_items",
                        principalColumn: "checkout_item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_return_items_returns_return_id",
                        column: x => x.return_id,
                        principalTable: "returns",
                        principalColumn: "return_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "damage_reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    incident_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    estimated_compensation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    actual_compensation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    reported_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    resolution_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    request_id = table.Column<int>(type: "int", nullable: false),
                    return_item_id = table.Column<int>(type: "int", nullable: true),
                    asset_id = table.Column<int>(type: "int", nullable: false),
                    reported_by = table.Column<int>(type: "int", nullable: false),
                    responsible_user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_damage_reports", x => x.report_id);
                    table.ForeignKey(
                        name: "FK_damage_reports_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_damage_reports_borrow_requests_request_id",
                        column: x => x.request_id,
                        principalTable: "borrow_requests",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_damage_reports_return_items_return_item_id",
                        column: x => x.return_item_id,
                        principalTable: "return_items",
                        principalColumn: "return_item_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_damage_reports_users_reported_by",
                        column: x => x.reported_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_damage_reports_users_responsible_user_id",
                        column: x => x.responsible_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_asset_categories_category_code",
                table: "asset_categories",
                column: "category_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_assets_asset_code",
                table: "assets",
                column: "asset_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_assets_category_id",
                table: "assets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_assets_current_room_id",
                table: "assets",
                column: "current_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_request_assets_asset_id",
                table: "borrow_request_assets",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_request_assets_request_id_asset_id",
                table: "borrow_request_assets",
                columns: new[] { "request_id", "asset_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_borrow_request_rooms_request_id_room_id",
                table: "borrow_request_rooms",
                columns: new[] { "request_id", "room_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_borrow_request_rooms_room_id",
                table: "borrow_request_rooms",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_requests_approved_by",
                table: "borrow_requests",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_requests_request_code",
                table: "borrow_requests",
                column: "request_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_borrow_requests_requester_id",
                table: "borrow_requests",
                column: "requester_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_requests_status",
                table: "borrow_requests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_checkout_items_asset_id",
                table: "checkout_items",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkout_items_checkout_id",
                table: "checkout_items",
                column: "checkout_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkout_items_request_asset_id",
                table: "checkout_items",
                column: "request_asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkouts_checked_out_by",
                table: "checkouts",
                column: "checked_out_by");

            migrationBuilder.CreateIndex(
                name: "IX_checkouts_checked_out_to",
                table: "checkouts",
                column: "checked_out_to");

            migrationBuilder.CreateIndex(
                name: "IX_checkouts_checkout_code",
                table: "checkouts",
                column: "checkout_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_checkouts_request_id",
                table: "checkouts",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_asset_id",
                table: "damage_reports",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_reported_by",
                table: "damage_reports",
                column: "reported_by");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_request_id",
                table: "damage_reports",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_responsible_user_id",
                table: "damage_reports",
                column: "responsible_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_return_item_id",
                table: "damage_reports",
                column: "return_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_damage_reports_status",
                table: "damage_reports",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_departments_department_code",
                table: "departments",
                column: "department_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_return_items_asset_id",
                table: "return_items",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_items_checkout_item_id",
                table: "return_items",
                column: "checkout_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_items_return_id",
                table: "return_items",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_returns_checkout_id",
                table: "returns",
                column: "checkout_id");

            migrationBuilder.CreateIndex(
                name: "IX_returns_received_by",
                table: "returns",
                column: "received_by");

            migrationBuilder.CreateIndex(
                name: "IX_returns_returned_by",
                table: "returns",
                column: "returned_by");

            migrationBuilder.CreateIndex(
                name: "IX_roles_role_code",
                table: "roles",
                column: "role_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_room_code",
                table: "rooms",
                column: "room_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_department_id",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_employee_code",
                table: "users",
                column: "employee_code",
                filter: "[employee_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_student_code",
                table: "users",
                column: "student_code",
                filter: "[student_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "borrow_request_rooms");

            migrationBuilder.DropTable(
                name: "damage_reports");

            migrationBuilder.DropTable(
                name: "return_items");

            migrationBuilder.DropTable(
                name: "checkout_items");

            migrationBuilder.DropTable(
                name: "returns");

            migrationBuilder.DropTable(
                name: "borrow_request_assets");

            migrationBuilder.DropTable(
                name: "checkouts");

            migrationBuilder.DropTable(
                name: "assets");

            migrationBuilder.DropTable(
                name: "borrow_requests");

            migrationBuilder.DropTable(
                name: "asset_categories");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
