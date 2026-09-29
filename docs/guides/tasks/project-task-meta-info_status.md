# 📋 Status: Project Task MetaInfo

This document provides a validation report comparing the metadata requirements outlined in `project-task-meta-info.md` against the current technical implementation.

## 🔍 Comparison Summary

| Property | Implementation Status | Findings & Discrepancies |
| :--- | :--- | :--- |
| **Identity (Id, ProjectId)** | ✅ Match | The core identity properties are correctly implemented in `ProjectTaskMetaInfo`. |
| **Lifecycle (Status)** | ✅ Match | `TaskStatusEnum` is present and aligns with the documented lifecycle stages. |
| **Priority** | ✅ Match | `TaskPriorityEnum` is implemented as described. |
| **ADHD Metadata: Difficulty** | ⚠️ Implementation Gap | The `Difficulty` property (intended to be `TaskDifficultyEnum`) is missing from the current model. |
| **ADHD Metadata: Quick Win** | ⚠️ Implementation Gap | The `IsQuickWin` boolean flag is missing from the current model. |
| **Time Estimation** | ✅ Match | `EstimatedMinutes` is correctly implemented. |

## 🛠️ Recommendations

*   **Implement Missing ADHD Fields**: Add `IsQuickWin` (bool) and `Difficulty` (using a new `TaskDifficultyEnum`) to the `ProjectTaskMetaInfo` model to enable the "Apothecary" energy-matching functionality described in the philosophy.
*   **Standardize Enum Naming**: Ensure that any newly added enums like `TaskDifficultyEnum` follow the established pattern in `src/Gadema.Core/Enums/`.

## 📋 Technical Audit

| Property | Status | Note |
| :--- | :--- | :--- |
| `Id` | ✅ Match | Unique identifier exists. |
| `ProjectId` | ✅ Match | Link to parent project is present. |
| `Status` | ✅ Match | Linked to `TaskStatusEnum`. |
| `Priority` | ✅ Match | Linked to `TaskPriorityEnum`. |
| `EstimatedMinutes` | ✅ Match | Property exists in the model. |
| `IsQuickWin` | ❌ Missing | Not found in `ProjectTaskMetaInfo.cs`. |
| `Difficulty` | ❌ Missing | Not found in `ProjectTaskMetaInfo.cs`. |
