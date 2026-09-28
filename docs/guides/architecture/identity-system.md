<|channel>thought
<channel|># 🧬 The Law of Identity: Soul & Body (The Anchor Pattern)

To maintain stability in a complex, modular world, every entity must follow the **Law of Identity**. This is achieved by decoupling an entity's permanent essence from its evolving data.
---

## ⚖️ The Core Concept: Soul vs. Body

In Anima, we do not treat entities as monolithic blocks. Instead, we split them into two distinct parts to ensure scale-invariant stability.

* **The Anchor (the Soul)**: This is the permanent identity of an entity (e.g., a Character's Name or a Setting's Title). It acts as the stable foundation that remains constant even as the world evolves.
* **The Body (the Modules)**: These are the specialized, evolving parts of your creation (e.g., a `Story Profile` for narrative, or a `Game Profile` for mechanics).

By separating the **Soul** from the **Body**, we ensure that any change to the "Body" can be correctly reflected in the "Soul," maintaining structural integrity across the entire system.
---

## 🔄 The Protocol: Harmonization (The Sync Mechanism)

When a part of the **Body** is updated, the **Anchor (the Soul)** must be aligned to match. We do not perform manual updates; we use the **Harmonization Protocol**.

### Implementation Detail
To maintain this equilibrium, developers must use the `SyncIdentityAsync<T>` method provided by the `ICoreServices` gateway.

**The Workflow:**
1. **Update the Body**: The domain service performs its primary business logic (e.g., updating a character's stats).
2. **Invoke Harmonization**: The service calls `SyncIdentityAsync<T>(id, updateData)`.
3. **Execute Strategy**: The system routes this call to a specialized `IIdentitySyncStrategy<T>`, which calculates the delta and updates the `MetaInfo` anchor accordingly.

> [!IMPORTANT]
> **The Law of Identity**: Never attempt to manually update `MetaInfo` properties within a domain service. Always use the **Harmonization** protocol to ensure identity stability.
---

## 🛠️ Technical Implementation Reference

| Component | Interface / Class | Responsibility |
| :--- | :--- | :--- |
| **Identity Anchor** | `MetaInfo` | The central, stable identity record. |
| **The Gateway** | `ICoreServices` | The entry point for all identity operations. |
| **The Strategy** | `IIdentitySyncStrategy<T>` | The logic that maps Body changes to Soul updates. |

### Example Usage (C#)

