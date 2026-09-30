# 🌌 Anima Lab Documentation Index

This document provides a functional overview of the documentation. Each entry describes the core information contained within the guide and its current implementation status.

## 🧭 The Status Pattern
Every guide is accompanied by a `[filename]_status.md` file used for technical validation.
* ✅ **Match**: Concept fully implemented.
* ⚠️ **Partial / In Progress**: Implementation is incomplete or evolving.
* ℹ️ **Conceptual / UI-Driven**: Design goals/UX patterns.

---

## 🧬 Access & Identity
The foundation of the security model, managing user identities and project-scoped access.
* [Overview](access/overview.md): The fundamental identity anchor and scoped access model. *[Status: ✅ Match]*
* [Project Member](access/project-member.md): Roles and membership within a specific project. *[Status: ✅ Match]*
* [Project Token](access/project-token.md): Secure, hashed keys for automated system access. *[Status: ✅ Match]*
* [User](access/user.md): The central identity anchor for all system interactions. *[Status: ✅ Match]*

## 🏗️ Architecture & Infrastructure
The technical blueprints for core patterns, permissions, and performance.
* [Enums](architecture/enums.md): Core type definitions and integer-based state management. *[Status: ⚠️ Partial]*
* [Identity System](architecture/identity-system.md): The mechanism for identity anchoring and synchronization. *[Status: ✅ Match]*
* [Permissions](architecture/permissions.md): The authorization engine (Brain, Muscle, Law) and access gates. *[Status: ⚠️ Partial]*
* [Schema](architecture/schema.md): Entity configuration patterns and polymorphic ownership logic. *[Status: ✅ Match]*
* [Security](architecture/security.md): Data protection protocols including JWT and sanitization. *[Status: ⚠️ Verification Required]*
* [Performance](infrastructure/performance.md): Query optimization, pagination, and infrastructure requirements. *[Status: ✅ Match]*

## ✍️ Writing & Narrative Engine
The creative engine for non-linear, graph-based storytelling.
* [Content Items](writing/content-items.md): The polymorphic content model and narrative hierarchy. *[Status: ✅ Match]*
* [Dialogue](writing/dialogue.md): Branching dialogue trees and node-based decision logic. *[Status: ⚠️ In Progress]*
* [Overview](writing/overview.md): Writing philosophy, focusing on Graph vs Tree structures. *[Status: ✅ Match]*
* [Scene Canvas](writing/scene-canvas.md): The scene as a unit of content with structural markers (Beats/Segments). *[Status: ⚠️ In Progress]*
* [Structure](writing/structure.md): Macro-level hierarchy from Story to Scene. *[Status: ✅ Match]*

## 📋 Task & Productivity Layer
A task management system optimized for ADHD-friendly workflows.
* [Project Task](tasks/project-task.md): Lifecycle and features of individual project tasks. *[Status: ⚠️ In Progress]*
* [Task Comment](tasks/project-task-comment.md): The model for task-linked communication. *[Status: ✅ Match]*
* [Task MetaInfo](tasks/project-task-meta-info.md): Metadata (Priority, Status) and ADHD energy-matching fields. *[Status: ⚠️ In Progress]*
* [User Stories](tasks/user-stories.md): Functional requirements and acceptance criteria for system features. *[Status: ✅ Mixed]*

## 🌌 The Universe
The high-level architectural scale-invariance principles.
* [Fractal Universe Concept](universe/fractal_universe_concept.md): The Anchor & Component pattern and scale-invariant design. *[Status: ✅ Match]*
* [System Architecture](universe/SYSTEM_ARCHITECTURE.md): Core patterns, orchestration, and the Root vs Content distinction. *[Status: ⚠️ In Progress]*
* [Identity Sync & Search](universe/IDENTITY_SYNC_SEARCH.md): Protocols for identity synchronization and search discovery. *[Status: ⚠️ In Progress]*

## 🎨 User Experience
UX design principles and sensory state metaphors.
* [ADHD Workflow](user-experience/adhd-workflow.md): Interaction patterns like Quick Wins and the two-tier save system. *[Status: ⚠️ Partial]*
* [Philosophy](user-experience/philosophy.md): Conceptual framework for energy management (Stamina/Mana). *[Status: ⚠️ Implementation Gap]*

---
*Last Updated: 2025-05-14*