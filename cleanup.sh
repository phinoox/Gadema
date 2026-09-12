#!/bin/bash

# =============================================================================
# GaDeMa Cleanup Script - Final Restructuring
# =============================================================================

set -e

echo "=== Starting Final Cleanup ==="

# 1. Move remaining Models to Base/
echo "Moving remaining Models to Base/..."
mkdir -p src/Gadema.Core/Models/Base/Entities
mkdir -p src/Gadema.Core/Models/Base/Relations
mkdir -p src/Gadema.Core/Models/Base/Enums
mkdir -p src/Gadema.Core/Models/Base/DependencyResolver

# ProjectSeries (Base entity)
mv src/Gadema.Core/Models/Tasks/Projects/ProjectSeries.cs src/Gadema.Core/Models/Base/Entities/ 2>/dev/null || true

# ProjectTag, ProjectTagRelation (Base relations)
mv src/Gadema.Core/Models/Tasks/Projects/ProjectTag.cs src/Gadema.Core/Models/Base/Relations/ 2>/dev/null || true
mv src/Gadema.Core/Models/Tasks/Projects/ProjectTagRelation.cs src/Gadema.Core/Models/Base/Relations/ 2>/dev/null || true

# ProjectToken (Base relation)
mv src/Gadema.Core/Models/Tokens/ProjectToken.cs src/Gadema.Core/Models/Base/Relations/ 2>/dev/null || true

# ReviewStatus (Versioning)
mv src/Gadema.Core/Models/Tasks/Tasks/ReviewStatus.cs src/Gadema.Core/Models/Versioning/Entities/ 2>/dev/null || true

# MetaInfoTag, MetaInfoTagRelation, MediaAttachmentTagRelation (Base relations)
mv src/Gadema.Core/Models/Content/MetaInfoTag.cs src/Gadema.Core/Models/Base/Relations/ 2>/dev/null || true
mv src/Gadema.Core/Models/Content/MetaInfoTagRelation.cs src/Gadema.Core/Models/Base/Relations/ 2>/dev/null || true
mv src/Gadema.Core/Models/Content/MediaAttachmentTagRelation.cs src/Gadema.Core/Models/Base/Relations/ 2>/dev/null || true

# Comment (Base relation)
mv src/Gadema.Core/Models/Comment.cs src/Gadema.Core/Models/Base/Relations/ 2>/dev/null || true

# 2. Move Enums to Base/Enums/
echo "Moving Enums to Base/Enums/..."
mv src/Gadema.Core/Enums/*.cs src/Gadema.Core/Models/Base/Enums/ 2>/dev/null || true

# 3. Move DependencyResolver to Base/
echo "Moving DependencyResolver to Base/..."
mv src/Gadema.Core/DependencyResolver/*.cs src/Gadema.Core/Models/Base/DependencyResolver/ 2>/dev/null || true

# 4. Move Utils to Base/
echo "Moving Utils to Base/..."
mv src/Gadema.Core/Utils/*.cs src/Gadema.Core/Models/Base/ 2>/dev/null || true

# 5. Move Interfaces to Base/
echo "Moving Interfaces to Base/..."
mkdir -p src/Gadema.Core/Models/Base/Interfaces
mv src/Gadema.Core/Interfaces/*.cs src/Gadema.Core/Models/Base/Interfaces/ 2>/dev/null || true

# 6. Move remaining DTOs to Base/
echo "Moving remaining DTOs to Base/..."
mkdir -p src/Gadema.Core/Dtos/Base/Comments
mkdir -p src/Gadema.Core/Dtos/Base/Content
mkdir -p src/Gadema.Core/Dtos/Base/ExternalReferences
mkdir -p src/Gadema.Core/Dtos/Base/Identity
mkdir -p src/Gadema.Core/Dtos/Base/Reviews
mkdir -p src/Gadema.Core/Dtos/Base/Search
mkdir -p src/Gadema.Core/Dtos/Base/Tags
mkdir -p src/Gadema.Core/Dtos/Base/Response
mkdir -p src/Gadema.Core/Dtos/Base/ContentItems

mv src/Gadema.Core/Dtos/Comments/*.cs src/Gadema.Core/Dtos/Base/Comments/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/Content/ExternalReferenceCreateDto.cs src/Gadema.Core/Dtos/Base/Content/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/ExternalReferences/*.cs src/Gadema.Core/Dtos/Base/ExternalReferences/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/GlobalConfiguration.cs src/Gadema.Core/Dtos/Base/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/Identity/*.cs src/Gadema.Core/Dtos/Base/Identity/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/Reviews/*.cs src/Gadema.Core/Dtos/Base/Reviews/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/Search/*.cs src/Gadema.Core/Dtos/Base/Search/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/Tags/*.cs src/Gadema.Core/Dtos/Base/Tags/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/Response/*.cs src/Gadema.Core/Dtos/Base/Response/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/ApiResponseDto.cs src/Gadema.Core/Dtos/Base/ 2>/dev/null || true
mv src/Gadema.Core/Dtos/ContentItems/*.cs src/Gadema.Core/Dtos/Base/ContentItems/ 2>/dev/null || true

# 7. Move remaining Controllers to Base/
echo "Moving remaining Controllers to Base/..."
mkdir -p src/Gadema.Api/Controllers/Base/Content
mkdir -p src/Gadema.Api/Controllers/Base/Identity
mkdir -p src/Gadema.Api/Controllers/Base/Activities
mkdir -p src/Gadema.Api/Controllers/Base/ContentSegments

mv src/Gadema.Api/Controllers/Content/CommentsController.cs src/Gadema.Api/Controllers/Base/Content/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Content/ExternalReferencesController.cs src/Gadema.Api/Controllers/Base/Content/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Content/MetaInfoController.cs src/Gadema.Api/Controllers/Base/Content/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Content/ReviewStatusController.cs src/Gadema.Api/Controllers/Base/Content/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Content/SearchController.cs src/Gadema.Api/Controllers/Base/Content/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Content/TagsController.cs src/Gadema.Api/Controllers/Base/Content/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Content/DialogueBranchesController.cs src/Gadema.Api/Controllers/Story/DialogueTrees/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/ContentSegments/*.cs src/Gadema.Api/Controllers/Story/ContentSegments/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Identity/*.cs src/Gadema.Api/Controllers/Base/Identity/ 2>/dev/null || true
mv src/Gadema.Api/Controllers/Activities/*.cs src/Gadema.Api/Controllers/Base/Activities/ 2>/dev/null || true

# 8. Move remaining Configurations to Base/
echo "Moving remaining Configurations to Base/..."
mkdir -p src/Gadema.Data/Configurations/Base/Content
mkdir -p src/Gadema.Data/Configurations/Base/Identity
mkdir -p src/Gadema.Data/Configurations/Base/Inventory
mkdir -p src/Gadema.Data/Configurations/Base/Activities
mkdir -p src/Gadema.Data/Configurations/Base/Templates
mkdir -p src/Gadema.Data/Configurations/Base/Tokens
mkdir -p src/Gadema.Data/Configurations/Base/EngineIntegration

mv src/Gadema.Data/Configurations/Tasks/CommentEntityTypeConfiguration.cs src/Gadema.Data/Configurations/Base/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Tasks/ProjectTaskCommentsEntityTypeConfiguration.cs src/Gadema.Data/Configurations/Base/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Tasks/ProjectTaskEntityTypeConfiguration.cs src/Gadema.Data/Configurations/Base/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Tasks/ReviewStatusEntityTypeConfiguration.cs src/Gadema.Data/Configurations/Versioning/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/EngineIntegration/*.cs src/Gadema.Data/Configurations/Base/EngineIntegration/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Identity/*.cs src/Gadema.Data/Configurations/Base/Identity/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Inventory/*.cs src/Gadema.Data/Configurations/Base/Inventory/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Activities/*.cs src/Gadema.Data/Configurations/Base/Activities/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Templates/*.cs src/Gadema.Data/Configurations/Base/Templates/ 2>/dev/null || true
mv src/Gadema.Data/Configurations/Tokens/*.cs src/Gadema.Data/Configurations/Base/Tokens/ 2>/dev/null || true

# 9. Clean up empty directories
echo "Cleaning up empty directories..."
find src -type d -empty -delete 2>/dev/null || true

echo "=== Cleanup Complete ==="
echo "Next steps:"
echo "1. Update all namespace declarations"
echo "2. Update all using statements"
echo "3. Run 'dotnet build'"