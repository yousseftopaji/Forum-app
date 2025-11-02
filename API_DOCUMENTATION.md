# Forum API Documentation

## Architecture Overview

### System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Client Layer                          │
└─────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                     WebApi (Controllers)                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │    Users     │  │    Posts     │  │   Comments   │      │
│  │  Controller  │  │  Controller  │  │  Controller  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                  Repository Contracts (Interfaces)           │
│    IUserRepository  │  IPostRepository  │  ICommentRepository│
└─────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Repository Implementations                       │
│  ┌──────────────────┐         ┌────────────────────┐        │
│  │  FileRepository  │    OR   │ InMemoryRepository │        │
│  │  (JSON Files)    │         │   (List<T>)        │        │
│  └──────────────────┘         └────────────────────┘        │
└─────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                      Data Storage                            │
│              users.json | posts.json | comments.json         │
└─────────────────────────────────────────────────────────────┘
```

### Domain Model

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│     User     │         │     Post     │         │   Comment    │
├──────────────┤         ├──────────────┤         ├──────────────┤
│ Id           │         │ Id           │         │ Id           │
│ Username     │1───────*│ Title        │1───────*│ Body         │
│ Password     │         │ Body         │         │ PostId       │
└──────────────┘         │ UserId (FK)  │         │ UserId (FK)  │
                         └──────────────┘         └──────────────┘
```

---

## RESTful API Endpoints

### Base URL

```
http://localhost:5000
```

---

## 👤 Users Resource (`/users`)

### **Get All Users**

```http
GET /users
```

**Query Parameters:**

- `UsernameContains` (optional) - Filter users by username substring

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "username": "john_doe",
    "password": "hashed_password"
  }
]
```

⚠️ **Note**: Returns full `User` entities including password. Consider using DTOs for security.

---

### **Create User**

```http
POST /users
```

**Request Body:**

```json
{
  "username": "john_doe",
  "password": "password123"
}
```

**Response:** `201 Created`

```json
{
  "username": "john_doe"
}
```

**Location Header:** `/users/{id}`

**Error Responses:**

- `409 Conflict` - Username already exists

---

### **Get User by ID**

```http
GET /users/{id}
```

**Response:** `200 OK`

```json
{
  "id": 1,
  "username": "john_doe",
  "password": "hashed_password"
}
```

⚠️ **Note**: Returns full `User` entity including password. Consider using DTOs for security.

**Error Responses:**

- `404 Not Found` - User not found

---

### **Update User**

```http
PATCH /users/{id}
```

**Request Body:**

```json
{
  "username": "new_username",
  "password": "new_password"
}
```

_Both fields are optional_

**Response:** `204 No Content`

**Error Responses:**

- `404 Not Found` - User not found
- `409 Conflict` - Username already taken

---

### **Delete User**

```http
DELETE /users/{id}
```

**Response:** `204 No Content`

---

### **Get User's Posts**

```http
GET /users/{userId}/posts
```

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "title": "My First Post",
    "body": "Post content here",
    "userId": 1
  }
]
```

⚠️ **Note**: Returns full `Post` entities with `id` field.

**Error Responses:**

- `404 Not Found` - User not found

---

### **Get Specific Post by User**

```http
GET /users/{userId}/posts/{postId}
```

**Response:** `200 OK`

```json
{
  "id": 1,
  "title": "My First Post",
  "body": "Post content here",
  "userId": 1
}
```

⚠️ **Note**: Returns full `Post` entity with `id` field.

**Error Responses:**

- `404 Not Found` - User or post not found

---

## 📝 Posts Resource (`/posts`)

### **Get All Posts**

```http
GET /posts
```

**Query Parameters:**

- `titleContains` (optional) - Filter by title substring
- `body` (optional) - Filter by body substring
- `authorUserId` (optional) - Filter by author user ID

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "title": "My First Post",
    "body": "Post content here",
    "userId": 1
  }
]
```

⚠️ **Note**: Returns full `Post` entities with `id` field.

---

### **Create Post**

```http
POST /posts
```

**Request Body:**

```json
{
  "title": "My First Post",
  "body": "Post content here",
  "authorUserId": "1"
}
```

**Response:** `201 Created`

```json
{
  "title": "My First Post",
  "body": "Post content here",
  "userId": 1
}
```

**Location Header:** `/posts/{id}`

**Error Responses:**

- `400 Bad Request` - User not found

---

### **Get Post by ID**

```http
GET /posts/{id}
```

**Response:** `200 OK`

```json
{
  "id": 1,
  "title": "My First Post",
  "body": "Post content here",
  "userId": 1
}
```

⚠️ **Note**: Returns full `Post` entity with `id` field.

**Error Responses:**

- `404 Not Found` - Post not found

---

### **Update Post**

```http
PATCH /posts/{id}
```

**Request Body:**

```json
{
  "title": "Updated Title",
  "body": "Updated content"
}
```

_Both fields are optional_

**Response:** `204 No Content`

**Error Responses:**

- `404 Not Found` - Post not found

---

### **Delete Post**

```http
DELETE /posts/{id}
```

**Response:** `204 No Content`

---

### **Get Post's Comments**

```http
GET /posts/{postId}/comments
```

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "body": "Great post!",
    "postId": 1,
    "userId": 2
  }
]
```

⚠️ **Note**: Returns full `Comment` entities with `id` field.

**Error Responses:**

- `404 Not Found` - Post not found

---

### **Add Comment to Post**

```http
POST /posts/{postId}/comments
```

**Request Body:**

```json
{
  "body": "Great post!",
  "userId": 2
}
```

**Response:** `201 Created`

```json
{
  "id": 1,
  "body": "Great post!",
  "postId": 1,
  "userId": 2
}
```

**Location Header:** `/posts/{postId}/comments/{commentId}`

**Error Responses:**

- `404 Not Found` - Post not found

---

### **Delete Comment from Post**

```http
DELETE /posts/{postId}/comments/{commentId}
```

**Response:** `204 No Content`

**Error Responses:**

- `404 Not Found` - Post or comment not found
- `400 Bad Request` - Comment doesn't belong to this post

---

## 💬 Comments Resource (`/comments`)

### **Get All Comments**

```http
GET /comments
```

**Query Parameters:**

- `PostId` (optional) - Filter by post ID
- `UserId` (optional) - Filter by user ID

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "body": "Great post!",
    "postId": 1,
    "userId": 2
  }
]
```

⚠️ **Note**: Returns full `Comment` entities with `id` field.

---

### **Create Comment**

```http
POST /comments
```

**Request Body:**

```json
{
  "body": "Great post!",
  "postId": 1,
  "userId": 2
}
```

**Response:** `201 Created`

```json
{
  "id": 1,
  "body": "Great post!",
  "postId": 1,
  "userId": 2
}
```

**Location Header:** `/comments/{id}`

**Error Responses:**

- `400 Bad Request` - Post not found

---

### **Get Comment by ID**

```http
GET /comments/{id}
```

**Response:** `200 OK`

```json
{
  "id": 1,
  "body": "Great post!",
  "postId": 1,
  "userId": 2
}
```

⚠️ **Note**: Returns full `Comment` entity with `id` field.

**Error Responses:**

- `404 Not Found` - Comment not found

---

### **Update Comment**

```http
PATCH /comments/{id}
```

**Request Body:**

```json
{
  "body": "Updated comment text"
}
```

**Response:** `204 No Content`

**Error Responses:**

- `404 Not Found` - Comment not found

---

### **Update Comment on Specific Post**

```http
PATCH /posts/{postId}/comments/{commentId}
```

**Request Body:**

```json
{
  "body": "Updated comment text"
}
```

**Response:** `204 No Content`

**Error Responses:**

- `404 Not Found` - Post or comment not found
- `400 Bad Request` - Comment doesn't belong to this post

---

### **Delete Comment**

```http
DELETE /comments/{id}
```

**Response:** `204 No Content`

---

## HTTP Status Codes

| Code              | Meaning            | Usage                                          |
| ----------------- | ------------------ | ---------------------------------------------- |
| `200 OK`          | Success            | GET requests that return data                  |
| `201 Created`     | Resource created   | POST requests                                  |
| `204 No Content`  | Success, no body   | PATCH, DELETE requests                         |
| `400 Bad Request` | Invalid request    | Validation errors, referenced entity not found |
| `404 Not Found`   | Resource not found | Entity doesn't exist                           |
| `409 Conflict`    | Conflict           | Duplicate username                             |

---

## Design Principles

### ✅ RESTful Best Practices

- **Resource-based URLs**: `/users`, `/posts`, `/comments`
- **HTTP verbs**: GET, POST, PATCH, DELETE
- **Nested routes for relationships**: `/users/{id}/posts`, `/posts/{id}/comments`
- **Proper status codes**: 200, 201, 204, 400, 404, 409

### ✅ Separation of Concerns

- **Controllers**: Handle HTTP requests/responses
- **Repositories**: Abstract data access logic
- **DTOs**: Separate API models from domain entities
- **Entities**: Core domain models

### ✅ Clean Architecture

- **Dependency Inversion**: Controllers depend on repository interfaces
- **Repository Pattern**: Swap between FileRepository and InMemoryRepository
- **Async/Await**: Proper async patterns throughout
- **Validation**: Username uniqueness, entity existence checks

---

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Data Storage**: JSON files (FileRepository) or In-Memory (InMemoryRepository)
- **ORM**: Entity Framework Core (for async LINQ extensions)
- **API Documentation**: Swagger/OpenAPI

---

## Running the Application

```bash
cd "Server/WebApi"
dotnet run
```

Application will start on: `http://localhost:5000`

Swagger UI available at: `http://localhost:5000/swagger`
