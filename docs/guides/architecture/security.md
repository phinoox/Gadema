# 🛡️ Security & Identity Architecture

This guide defines the security implementation, authentication protocols, and data protection strategies for Anima Lab. It covers everything from JWT management to file upload sanitization.

## 1. Authentication & Authorization Protocols

Anima Lab uses a multi-layered approach to ensure that only authorized users can access sensitive content and perform critical actions.

### 1.1 Identity Management
* **JWT Bearer Tokens**: Uses HS256 (HMAC-SHA256) with a 1-hour expiry for stateless API authentication.
* **Refresh Token Rotation**: To mitigate the risk of stolen tokens, every use of a refresh token issues a new one and invalidates the old one.
* **Two-Factor Authentication (2FA)**: Supports TOTP (Time-based One-Time Password) via Google Authenticator or Authy to provide an extra layer of security.
* **Recovery Codes**: Provides eight single-use base64 recovery codes for users who lose access to their 2FA device.

### 1.2 Authorization Model
The system employs a **Capability-Based Authorization** model rather than simple role checks:
1.  **Identity Verification**: Confirms the user is authenticated via `IUserContext`.
2.  **Ownership Check**: If the user owns the resource, full access is granted (the "Owner" bypass).
3.  **Permission Evaluation**: For non-owners, the `IPermissionEngine` evaluates granular permissions (e.g., `CanEdit`, `CanPublish`) against the user's role and the resource's specific policy.

---

## 2. Data Protection & Security Best Practices

### 2.1 Password Security
* **Hashing Algorithm**: All passwords are hashed using **BCrypt** via PBKDF2.
* **Implementation**: We use a unique 32-byte random salt per password to prevent rainbow table attacks.
* **Complexity Requirements**: Minimum 8 characters, including uppercase, lowercase, digits, and special characters.

### 2.2 File Upload Sanitization
To prevent malicious uploads (e. Wrappers/OOM attacks), every upload follows this strict pipeline:
1.  **MIME Type Validation**: Validates against a strict whitelist (`image/png`, `image/jpeg`, `application/pdf`) **before** any processing.
2.  **Size Constraint**: Rejects files exceeding the 100MB limit immediately.
3.  **Filename Sanitization**: Strips directory traversal attempts and prefixes filenames with a unique UUID to prevent collisions and path injection.

### 2.3 Input Sanitization (XSS Prevention)
* All user-generated text (e.g., `Description`, `CommentText`) is encoded using `HtmlEncoder.Encode()` before being stored or rendered to prevent Cross-Site Scripting (XSS).

---

## 3. Infrastructure Hardening

### 3.1 API Security
* **Rate Limiting**: Enforced at the Caddy reverse proxy level to prevent brute force and DDoS attacks:
    * `General API`: 100 req/min per IP.
    * `Auth Endpoints`: 1000 req/hour per IP.
    * `Export Endpoints`: 5 req/min (due to high CPU cost).
* **CORS Policy**: Strictly configured to prevent unauthorized cross-origin requests in production environments.

### 3.2 Transport Security
* **HTTPS Enforcement**: All traffic must be served over TLS.
* **Security Headers**: The Caddy proxy injects critical headers:
    * `Strict-Transport-Security` (HSTS)
    * `X-Content-Type-Options: nosniff`
    * `X-Frame-Options: SAMEORIGIN`

---

## 4. Developer Checklist for Secure Implementation

- [ ] **JWT Key**: Is the signing key $\ge$ 64 random bytes?
- [ ] **Clock Skew**: Is `ClockSkew` set to `TimeSpan.Zero` in your JWT configuration?
- [ ] **Validation**: Are you checking `ModelState.IsValid` at the start of every controller action?
- [ ] **Streaming**: Are you using `FileStream` for uploads instead of loading bytes into memory?
- [ ] **Logging**: Ensure sensitive data (passwords, tokens) is NEVER written to logs.
