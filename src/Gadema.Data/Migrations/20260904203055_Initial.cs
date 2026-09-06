using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gadema.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SnapshotVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    SnapshotType = table.Column<int>(type: "INTEGER", nullable: false),
                    SnapshotDataJson = table.Column<string>(type: "TEXT", maxLength: 50000, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaInfoTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ColorHex = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaInfoTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdentityType = table.Column<int>(type: "INTEGER", nullable: false),
                    IdentityTypeName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ColorHex = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    TemplateType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ImageUri = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    GoogleSubjectId = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecoveryCodeHash = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    TwoFactorSecret = table.Column<string>(type: "TEXT", nullable: true),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeSetDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttributeSetName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    BaseLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefaultSet = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeSetDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeSetDefinition_ProjectTemplates_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateClassTemplateDefinitions",
                columns: table => new
                {
                    ProjectTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClassTemplateName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BaseLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateClassTemplateDefinitions", x => new { x.ProjectTemplateId, x.ClassTemplateName });
                    table.ForeignKey(
                        name: "FK_TemplateClassTemplateDefinitions_ProjectTemplates_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateIdentityDefinitions",
                columns: table => new
                {
                    ProjectTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdentityName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdentityDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateIdentityDefinitions", x => new { x.ProjectTemplateId, x.IdentityName });
                    table.ForeignKey(
                        name: "FK_TemplateIdentityDefinitions_IdentityDefinition_IdentityDefinitionId",
                        column: x => x.IdentityDefinitionId,
                        principalTable: "IdentityDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateIdentityDefinitions_ProjectTemplates_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateNarrativeStructures",
                columns: table => new
                {
                    ProjectTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SequenceName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefaultStructure = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateNarrativeStructures", x => new { x.ProjectTemplateId, x.SequenceName });
                    table.ForeignKey(
                        name: "FK_TemplateNarrativeStructures_ProjectTemplates_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectSeriesId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EnableUserRegistration = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowManualInvites = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewMode = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false),
                    last_modified_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectSeries_ProjectSeriesId",
                        column: x => x.ProjectSeriesId,
                        principalTable: "ProjectSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProviderLinks",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    LinkedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "UTCNOW()"),
                    ExternalSubjectId = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProviderLinks", x => new { x.UserId, x.Provider });
                    table.ForeignKey(
                        name: "FK_UserProviderLinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateAttributeSetDefinitions",
                columns: table => new
                {
                    ProjectTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttributeSetDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateAttributeSetDefinitions", x => new { x.ProjectTemplateId, x.Name });
                    table.ForeignKey(
                        name: "FK_TemplateAttributeSetDefinitions_AttributeSetDefinition_AttributeSetDefinitionId",
                        column: x => x.AttributeSetDefinitionId,
                        principalTable: "AttributeSetDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateAttributeSetDefinitions_ProjectTemplates_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RelatedEntityType = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetaInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContentType = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ShortDesc = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewMode = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    References = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaInfos_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EndingDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ConditionsJson = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndingDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndingDefinitions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EngineExportConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EngineType = table.Column<int>(type: "INTEGER", nullable: false),
                    ExportFormat = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefaultConfig = table.Column<bool>(type: "INTEGER", nullable: false),
                    FieldMappingsJson = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineExportConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineExportConfigs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdentityDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdentityName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    IdentityTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityValues_IdentityDefinition_IdentityDefinitionId",
                        column: x => x.IdentityDefinitionId,
                        principalTable: "IdentityDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityValues_ProjectTemplates_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityValues_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectIdentityDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdentityType = table.Column<int>(type: "INTEGER", nullable: false),
                    IdentityName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectIdentityDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectIdentityDefinitions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTagRelations",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectTagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTagRelations", x => new { x.ProjectId, x.ProjectTagId });
                    table.ForeignKey(
                        name: "FK_ProjectTagRelations_ProjectTags_ProjectTagId",
                        column: x => x.ProjectTagId,
                        principalTable: "ProjectTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTagRelations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TokenName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PermissionsJson = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProjectId1 = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTokens_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTokens_Projects_ProjectId1",
                        column: x => x.ProjectId1,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectTeam",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeam", x => new { x.ProjectId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_ProjectTeam_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTeam_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InvitedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsPendingInvite = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbilityDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    AbilityType = table.Column<int>(type: "INTEGER", nullable: false),
                    CooldownSeconds = table.Column<decimal>(type: "TEXT", nullable: true),
                    ResourceCost = table.Column<decimal>(type: "TEXT", nullable: true),
                    MaxLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    ScalingFormulaJson = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    RequiresFlagCondition = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbilityDefinitions_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbilitySets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilitySets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbilitySets_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbilitySets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EnginePath = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    EngineAssetId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    EngineFileType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetLinks_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ValueType = table.Column<int>(type: "INTEGER", nullable: false),
                    FormulaExpression = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    LevelMappingJson = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    DefaultMin = table.Column<decimal>(type: "TEXT", nullable: true),
                    DefaultMax = table.Column<decimal>(type: "TEXT", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDefinitions_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeSets_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeSets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ClassTemplateId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterDetails_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommentedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommentText = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    Visibility = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaInfoTagRelations",
                columns: table => new
                {
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoTagId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaInfoTags", x => new { x.MetaInfoId, x.MetaInfoTagId });
                    table.ForeignKey(
                        name: "FK_MetaInfoTags_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MetaInfoTags_MetaInfoTags_MetaInfoTagId",
                        column: x => x.MetaInfoTagId,
                        principalTable: "MetaInfoTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentVersionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChangeDescription = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentVersionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentVersionLogs_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DialogueBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    VisualNodeImageUri = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    CharacterIconUri = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    IsRoot = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentNodeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogueBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogueBranches_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DialogueBranches_DialogueBranches_ParentNodeId",
                        column: x => x.ParentNodeId,
                        principalTable: "DialogueBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DialogueBranches_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ItemType = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryItems_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoreEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LoreType = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoreEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoreEntries_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoreEntries_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    storage_path = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaAttachments_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TaskTitle = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedMinutes = table.Column<decimal>(type: "TEXT", maxLength: 512, nullable: true),
                    AssignedToUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsQuickWin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    last_modified_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProjectTaskId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectTasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ReviewComments = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewStatuses_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewStatuses_MetaInfos_MetaInfoId1",
                        column: x => x.MetaInfoId1,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewStatuses_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusEffectDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    EffectType = table.Column<int>(type: "INTEGER", nullable: false),
                    DurationSeconds = table.Column<decimal>(type: "TEXT", nullable: true),
                    DamagePerTick = table.Column<decimal>(type: "TEXT", nullable: true),
                    RequiresCondition = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEffectDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusEffectDefinitions_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StorySequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentSequenceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SequenceName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    SequenceDescription = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false),
                    order_index = table.Column<int>(type: "INTEGER", nullable: false),
                    OutlineSummary = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorySequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorySequences_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StorySequences_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorySequences_StorySequences_ParentSequenceId",
                        column: x => x.ParentSequenceId,
                        principalTable: "StorySequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EngineFieldMappings",
                columns: table => new
                {
                    EngineExportConfigId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceColumn = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    TargetColumn = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContentType = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentFieldName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    EngineFieldName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineFieldMappings", x => new { x.EngineExportConfigId, x.SourceColumn, x.TargetColumn });
                    table.ForeignKey(
                        name: "FK_EngineFieldMappings_EngineExportConfigs_EngineExportConfigId",
                        column: x => x.EngineExportConfigId,
                        principalTable: "EngineExportConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EngineFieldMappings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdentityDefinitionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IdentityValueId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayText = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    IdentityType = table.Column<int>(type: "INTEGER", nullable: false),
                    IdentityTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterIdentities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterIdentities_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterIdentities_IdentityDefinition_IdentityDefinitionId",
                        column: x => x.IdentityDefinitionId,
                        principalTable: "IdentityDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterIdentities_IdentityValues_IdentityValueId",
                        column: x => x.IdentityValueId,
                        principalTable: "IdentityValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TokenUsageLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TokenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IPAddress = table.Column<string>(type: "TEXT", maxLength: 48, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Action = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProjectTokenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenUsageLogs_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TokenUsageLogs_ProjectTokens_ProjectTokenId",
                        column: x => x.ProjectTokenId,
                        principalTable: "ProjectTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TokenUsageLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharacterAttributes",
                columns: table => new
                {
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    CalculatedFromTemplate = table.Column<bool>(type: "INTEGER", nullable: false),
                    OverridesFormula = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAttributes", x => new { x.MetaInfoId, x.AttributeDefinitionId });
                    table.ForeignKey(
                        name: "FK_CharacterAttributes_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAttributes_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    AttributeSetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BaseLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLevel = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassTemplate_AttributeSets_AttributeSetId",
                        column: x => x.AttributeSetId,
                        principalTable: "AttributeSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassTemplate_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharacterBackgrounds",
                columns: table => new
                {
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterDetailsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterBackgrounds", x => x.MetaInfoId);
                    table.ForeignKey(
                        name: "FK_CharacterBackgrounds_CharacterDetails_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "CharacterDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterBackgrounds_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DialogueNodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    BranchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NodeText = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    SpeakerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ChoiceOptions = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    Conditions = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ParentNodeId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogueNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialogueNodes_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DialogueNodes_DialogueBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "DialogueBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DialogueNodes_DialogueNodes_ParentNodeId",
                        column: x => x.ParentNodeId,
                        principalTable: "DialogueNodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DialogueNodes_Users_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExternalReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentType = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExternalReferenceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MediaAttachmentId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalReferences_ExternalReferences_ExternalReferenceId",
                        column: x => x.ExternalReferenceId,
                        principalTable: "ExternalReferences",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExternalReferences_ExternalReferences_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ExternalReferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalReferences_MediaAttachments_MediaAttachmentId",
                        column: x => x.MediaAttachmentId,
                        principalTable: "MediaAttachments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MediaTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MediaAttachmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaTags_MetaInfoTags_TagId",
                        column: x => x.TagId,
                        principalTable: "MetaInfoTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaTags_MediaAttachments_MediaAttachmentId",
                        column: x => x.MediaAttachmentId,
                        principalTable: "MediaAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectTaskId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommentedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommentText = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TaskId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_ProjectTasks_ProjectTaskId",
                        column: x => x.ProjectTaskId,
                        principalTable: "ProjectTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryBeats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SequenceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeatTitle = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Published = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryBeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryBeats_StorySequences_SequenceId",
                        column: x => x.SequenceId,
                        principalTable: "StorySequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryOutlines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SequenceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    CharacterSnapshot = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    ThemeStatement = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    OutlineStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    StorySequenceId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryOutlines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryOutlines_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryOutlines_StorySequences_SequenceId",
                        column: x => x.SequenceId,
                        principalTable: "StorySequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryOutlines_StorySequences_StorySequenceId",
                        column: x => x.StorySequenceId,
                        principalTable: "StorySequences",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClassTemplateAttribute",
                columns: table => new
                {
                    ClassTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OverrideFormulaExpression = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    DefaultMinValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    DefaultMaxValue = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTemplateAttribute", x => new { x.ClassTemplateId, x.AttributeDefinitionId });
                    table.ForeignKey(
                        name: "FK_ClassTemplateAttribute_AttributeDefinitions_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassTemplateAttribute_ClassTemplate_ClassTemplateId",
                        column: x => x.ClassTemplateId,
                        principalTable: "ClassTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassTemplateAttribute_MetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbilityDefinitions_AbilityType",
                table: "AbilityDefinitions",
                column: "AbilityType");

            migrationBuilder.CreateIndex(
                name: "IX_AbilityDefinitions_MetaInfoId",
                table: "AbilityDefinitions",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AbilityDefinitions_Name",
                table: "AbilityDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AbilityDefinitions_slug",
                table: "AbilityDefinitions",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbilitySets_MetaInfoId",
                table: "AbilitySets",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AbilitySets_ProjectId",
                table: "AbilitySets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AbilitySets_slug",
                table: "AbilitySets",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbilitySets_Type",
                table: "AbilitySets",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_CreatedAt",
                table: "ActivityLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_EventType",
                table: "ActivityLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_ProjectId",
                table: "ActivityLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLinks_MetaInfoId",
                table: "AssetLinks",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_MetaInfoId",
                table: "AttributeDefinitions",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_Name",
                table: "AttributeDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_slug",
                table: "AttributeDefinitions",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_ValueType",
                table: "AttributeDefinitions",
                column: "ValueType");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSetDefinition_ProjectTemplateId",
                table: "AttributeSetDefinition",
                column: "ProjectTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_MetaInfoId",
                table: "AttributeSets",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_DisplayOrder",
                table: "AttributeSets",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_Name",
                table: "AttributeSets",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_ProjectId",
                table: "AttributeSets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAttributes_AttributeDefinitionId",
                table: "CharacterAttributes",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAttributes_MetaInfoId",
                table: "CharacterAttributes",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDetails_MetaInfoId",
                table: "CharacterDetails",
                column: "MetaInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterIdentities_MetaInfoId",
                table: "CharacterIdentities",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterIdentities_IdentityDefinitionId",
                table: "CharacterIdentities",
                column: "IdentityDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterIdentities_IdentityValueId",
                table: "CharacterIdentities",
                column: "IdentityValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTemplate_AttributeSetId",
                table: "ClassTemplate",
                column: "AttributeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTemplate_MetaInfoId",
                table: "ClassTemplate",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTemplate_Name",
                table: "ClassTemplate",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassTemplateAttribute_AttributeDefinitionId",
                table: "ClassTemplateAttribute",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTemplateAttribute_MetaInfoId",
                table: "ClassTemplateAttribute",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentedByUserId",
                table: "Comments",
                column: "CommentedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_MetaInfoId",
                table: "Comments",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedAt",
                table: "Comments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfos_ContentType",
                table: "MetaInfos",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfos_ProjectId",
                table: "MetaInfos",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfos_Published",
                table: "MetaInfos",
                column: "Published");

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfos_slug",
                table: "MetaInfos",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfos_Status",
                table: "MetaInfos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfoTags_MetaInfoTagId",
                table: "MetaInfoTagRelations",
                column: "MetaInfoTagId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_MetaInfoId",
                table: "ContentSnapshots",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_CreatedAt",
                table: "ContentSnapshots",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_CreatedByUserId",
                table: "ContentSnapshots",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_SnapshotType",
                table: "ContentSnapshots",
                column: "SnapshotType");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_SnapshotVersion",
                table: "ContentSnapshots",
                column: "SnapshotVersion");

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfoTags_Name",
                table: "MetaInfoTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MetaInfoTags_Slug",
                table: "MetaInfoTags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersionLogs_MetaInfoId",
                table: "ContentVersionLogs",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersionLogs_VersionNumber",
                table: "ContentVersionLogs",
                column: "VersionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueBranches_MetaInfoId",
                table: "DialogueBranches",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueBranches_IsRoot",
                table: "DialogueBranches",
                column: "IsRoot");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueBranches_ParentNodeId",
                table: "DialogueBranches",
                column: "ParentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueBranches_ProjectId",
                table: "DialogueBranches",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueBranches_slug",
                table: "DialogueBranches",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DialogueNodes_BranchId",
                table: "DialogueNodes",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueNodes_MetaInfoId",
                table: "DialogueNodes",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueNodes_ParentNodeId",
                table: "DialogueNodes",
                column: "ParentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DialogueNodes_SpeakerId",
                table: "DialogueNodes",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_EndingDefinitions_ProjectId",
                table: "EndingDefinitions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EndingDefinitions_slug",
                table: "EndingDefinitions",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EndingDefinitions_Title",
                table: "EndingDefinitions",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_EngineExportConfigs_ExportFormat",
                table: "EngineExportConfigs",
                column: "ExportFormat");

            migrationBuilder.CreateIndex(
                name: "IX_EngineExportConfigs_IsEnabled",
                table: "EngineExportConfigs",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_EngineExportConfigs_ProjectId",
                table: "EngineExportConfigs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineFieldMappings_ProjectId",
                table: "EngineFieldMappings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_ExternalReferenceId",
                table: "ExternalReferences",
                column: "ExternalReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_MediaAttachmentId",
                table: "ExternalReferences",
                column: "MediaAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_ParentId",
                table: "ExternalReferences",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_Url",
                table: "ExternalReferences",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityValues_IdentityDefinitionId",
                table: "IdentityValues",
                column: "IdentityDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityValues_IdentityTypeId",
                table: "IdentityValues",
                column: "IdentityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityValues_IsRequired",
                table: "IdentityValues",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityValues_ProjectId",
                table: "IdentityValues",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityValues_ProjectTemplateId",
                table: "IdentityValues",
                column: "ProjectTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_MetaInfoId",
                table: "InventoryItems",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemType",
                table: "InventoryItems",
                column: "ItemType");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ProjectId",
                table: "InventoryItems",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_Quantity",
                table: "InventoryItems",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_LoreEntries_MetaInfoId",
                table: "LoreEntries",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_LoreEntries_LoreType",
                table: "LoreEntries",
                column: "LoreType");

            migrationBuilder.CreateIndex(
                name: "IX_LoreEntries_ProjectId",
                table: "LoreEntries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LoreEntries_slug",
                table: "LoreEntries",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAttachments_MetaInfoId",
                table: "MediaAttachments",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaTags_Attachment",
                table: "MediaTags",
                column: "MediaAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaTags_Tag",
                table: "MediaTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectIdentityDefinitions_ProjectId_IdentityName",
                table: "ProjectIdentityDefinitions",
                columns: new[] { "ProjectId", "IdentityName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectSeriesId",
                table: "Projects",
                column: "ProjectSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_slug",
                table: "Projects",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Visibility",
                table: "Projects",
                column: "Visibility");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSeries_slug",
                table: "ProjectSeries",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTagRelations_ProjectTagId",
                table: "ProjectTagRelations",
                column: "ProjectTagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_Name",
                table: "ProjectTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_Slug",
                table: "ProjectTags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_MetaInfoId",
                table: "ProjectTasks",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_Difficulty",
                table: "ProjectTasks",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_IsQuickWin",
                table: "ProjectTasks",
                column: "IsQuickWin");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_ProjectId",
                table: "ProjectTasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_Status",
                table: "ProjectTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeam_RoleId",
                table: "ProjectTeam",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeam_TeamId",
                table: "ProjectTeam",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTemplates_TemplateType",
                table: "ProjectTemplates",
                column: "TemplateType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTokens_IsActive",
                table: "ProjectTokens",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTokens_ProjectId",
                table: "ProjectTokens",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTokens_ProjectId1",
                table: "ProjectTokens",
                column: "ProjectId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatuses_MetaInfoId",
                table: "ReviewStatuses",
                column: "MetaInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatuses_MetaInfoId1",
                table: "ReviewStatuses",
                column: "MetaInfoId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatuses_ReviewedAt",
                table: "ReviewStatuses",
                column: "ReviewedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatuses_ReviewedByUserId",
                table: "ReviewStatuses",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatuses_Status",
                table: "ReviewStatuses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StatusEffectDefinitions_MetaInfoId",
                table: "StatusEffectDefinitions",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusEffectDefinitions_EffectType",
                table: "StatusEffectDefinitions",
                column: "EffectType");

            migrationBuilder.CreateIndex(
                name: "IX_StatusEffectDefinitions_Name",
                table: "StatusEffectDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_StatusEffectDefinitions_slug",
                table: "StatusEffectDefinitions",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryBeats_SequenceId",
                table: "StoryBeats",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryBeats_slug",
                table: "StoryBeats",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryOutlines_MetaInfoId",
                table: "StoryOutlines",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryOutlines_SequenceId",
                table: "StoryOutlines",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryOutlines_StorySequenceId",
                table: "StoryOutlines",
                column: "StorySequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySequences_MetaInfoId",
                table: "StorySequences",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySequences_ParentSequenceId",
                table: "StorySequences",
                column: "ParentSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySequences_ProjectId",
                table: "StorySequences",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StorySequences_Published",
                table: "StorySequences",
                column: "Published");

            migrationBuilder.CreateIndex(
                name: "IX_StorySequences_slug",
                table: "StorySequences",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_CommentedByUserId",
                table: "TaskComments",
                column: "CommentedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_CreatedAt",
                table: "TaskComments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_ProjectTaskId",
                table: "TaskComments",
                column: "ProjectTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskId",
                table: "TaskComments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_RoleId",
                table: "TeamMembers",
                column: "RoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMembers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId",
                table: "TeamMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CreatedByUserId",
                table: "Teams",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Slug",
                table: "Teams",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateAttributeSetDefinitions_AttributeSetDefinitionId",
                table: "TemplateAttributeSetDefinitions",
                column: "AttributeSetDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateIdentityDefinitions_IdentityDefinitionId",
                table: "TemplateIdentityDefinitions",
                column: "IdentityDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenUsageLogs_MetaInfoId",
                table: "TokenUsageLogs",
                column: "MetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenUsageLogs_ProjectId",
                table: "TokenUsageLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenUsageLogs_ProjectTokenId",
                table: "TokenUsageLogs",
                column: "ProjectTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleSubjectId",
                table: "Users",
                column: "GoogleSubjectId",
                unique: true,
                filter: "\"GoogleSubjectId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbilityDefinitions");

            migrationBuilder.DropTable(
                name: "AbilitySets");

            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "AssetLinks");

            migrationBuilder.DropTable(
                name: "CharacterAttributes");

            migrationBuilder.DropTable(
                name: "CharacterBackgrounds");

            migrationBuilder.DropTable(
                name: "CharacterIdentities");

            migrationBuilder.DropTable(
                name: "ClassTemplateAttribute");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "MetaInfoTagRelations");

            migrationBuilder.DropTable(
                name: "ContentSnapshots");

            migrationBuilder.DropTable(
                name: "ContentVersionLogs");

            migrationBuilder.DropTable(
                name: "DialogueNodes");

            migrationBuilder.DropTable(
                name: "EndingDefinitions");

            migrationBuilder.DropTable(
                name: "EngineFieldMappings");

            migrationBuilder.DropTable(
                name: "ExternalReferences");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "LoreEntries");

            migrationBuilder.DropTable(
                name: "MediaTags");

            migrationBuilder.DropTable(
                name: "ProjectIdentityDefinitions");

            migrationBuilder.DropTable(
                name: "ProjectTagRelations");

            migrationBuilder.DropTable(
                name: "ProjectTeam");

            migrationBuilder.DropTable(
                name: "ReviewStatuses");

            migrationBuilder.DropTable(
                name: "StatusEffectDefinitions");

            migrationBuilder.DropTable(
                name: "StoryBeats");

            migrationBuilder.DropTable(
                name: "StoryOutlines");

            migrationBuilder.DropTable(
                name: "TaskComments");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "TemplateAttributeSetDefinitions");

            migrationBuilder.DropTable(
                name: "TemplateClassTemplateDefinitions");

            migrationBuilder.DropTable(
                name: "TemplateIdentityDefinitions");

            migrationBuilder.DropTable(
                name: "TemplateNarrativeStructures");

            migrationBuilder.DropTable(
                name: "TokenUsageLogs");

            migrationBuilder.DropTable(
                name: "UserProviderLinks");

            migrationBuilder.DropTable(
                name: "CharacterDetails");

            migrationBuilder.DropTable(
                name: "IdentityValues");

            migrationBuilder.DropTable(
                name: "AttributeDefinitions");

            migrationBuilder.DropTable(
                name: "ClassTemplate");

            migrationBuilder.DropTable(
                name: "DialogueBranches");

            migrationBuilder.DropTable(
                name: "EngineExportConfigs");

            migrationBuilder.DropTable(
                name: "MetaInfoTags");

            migrationBuilder.DropTable(
                name: "MediaAttachments");

            migrationBuilder.DropTable(
                name: "ProjectTags");

            migrationBuilder.DropTable(
                name: "StorySequences");

            migrationBuilder.DropTable(
                name: "ProjectTasks");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "AttributeSetDefinition");

            migrationBuilder.DropTable(
                name: "ProjectTokens");

            migrationBuilder.DropTable(
                name: "IdentityDefinition");

            migrationBuilder.DropTable(
                name: "AttributeSets");

            migrationBuilder.DropTable(
                name: "ProjectTemplates");

            migrationBuilder.DropTable(
                name: "MetaInfos");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectSeries");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
