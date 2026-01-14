# API Documentation - Image Conversion & Plan Management

## Base URL
```
https://localhost:5001/api
```

## CORS Configuration
The API is configured to accept requests from:
- `http://localhost:4200` (Angular development server)
- `https://localhost:4200` (Angular development server with HTTPS)

All HTTP methods and headers are allowed. Credentials are supported for cookie-based authentication.

## Overview
This API provides functionality for image conversion simulation (to WebP format) and subscription plan management. Designed for frontend applications.

---

## Image Upload & Conversion Endpoints

### 1. Upload and Convert Image
**POST** `/api/ImageUpload/upload`

Upload an image and simulate WebP conversion.

**Request Body (multipart/form-data):**
```
file: file (required) - Image file (JPG, PNG, GIF, BMP)
userId: integer (optional) - User ID for tracking conversions
```

**File Constraints:**
- Maximum size: 10 MB
- Allowed formats: JPG, JPEG, PNG, GIF, BMP

**Success Response (200 OK):**
```json
{
  "success": true,
  "conversionId": 1,
  "originalFile": {
    "id": 1,
    "fileName": "image.jpg",
    "size": 1024000,
    "format": "JPG"
  },
  "webpFile": {
    "id": 2,
    "fileName": "image.webp",
    "size": 716800,
    "format": "WEBP",
    "downloadUrl": "/api/ImageUpload/download/2"
  },
  "compressionRate": 30.0,
  "conversionDate": "2026-01-14T10:30:00Z"
}
```

**Error Responses:**

**400 Bad Request:**
```json
{
  "error": "No file uploaded"
}
```
```json
{
  "error": "File size exceeds 10 MB limit"
}
```
```json
{
  "error": "Only JPG, PNG, GIF, and BMP files are allowed"
}
```
```json
{
  "error": "Conversion limit reached. Please upgrade your plan."
}
```

**500 Internal Server Error:**
```json
{
  "error": "Error simulating conversion: [details]"
}
```

---

### 2. Download Image
**GET** `/api/ImageUpload/download/{imageId}`

Download a converted or original image by its ID.

**Path Parameters:**
- `imageId` (integer, required) - The ID of the image

**Success Response (200 OK):**
Returns the image file with appropriate content type:
- `image/webp` for WebP
- `image/jpeg` for JPG/JPEG
- `image/png` for PNG
- `image/gif` for GIF
- `image/bmp` for BMP

**Error Response (404 Not Found):**
```json
{
  "error": "Image not found"
}
```
```json
{
  "error": "File not found on server"
}
```

---

### 3. Get User Conversions
**GET** `/api/ImageUpload/conversions/{userId}`

Retrieve all conversions for a specific user.

**Path Parameters:**
- `userId` (integer, required) - The ID of the user

**Success Response (200 OK):**
```json
{
  "conversions": [
    {
      "id": 1,
      "conversionDate": "2026-01-14T10:30:00Z",
      "originalImage": {
        "id": 1,
        "fileName": "image.jpg",
        "size": 1024000,
        "format": "JPG"
      },
      "webpImage": {
        "id": 2,
        "fileName": "image.webp",
        "size": 716800,
        "format": "WEBP",
        "downloadUrl": "/api/ImageUpload/download/2"
      },
      "compressionRate": 30.0
    }
  ],
  "count": 1
}
```

**Error Response (500 Internal Server Error):**
```json
{
  "error": "Error retrieving conversions: [details]"
}
```

---

### 4. Get Specific Conversion
**GET** `/api/ImageUpload/conversion/{conversionId}`

Retrieve details of a specific conversion.

**Path Parameters:**
- `conversionId` (integer, required) - The ID of the conversion

**Success Response (200 OK):**
```json
{
  "id": 1,
  "conversionDate": "2026-01-14T10:30:00Z",
  "userId": 1,
  "originalImage": {
    "id": 1,
    "fileName": "image.jpg",
    "size": 1024000,
    "format": "JPG"
  },
  "webpImage": {
    "id": 2,
    "fileName": "image.webp",
    "size": 716800,
    "format": "WEBP",
    "downloadUrl": "/api/ImageUpload/download/2"
  }
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Conversion not found"
}
```

---

### 5. Delete Conversion
**DELETE** `/api/ImageUpload/conversion/{conversionId}`

Delete a conversion and its associated images (database records and physical files).

**Path Parameters:**
- `conversionId` (integer, required) - The ID of the conversion to delete

**Success Response (200 OK):**
```json
{
  "message": "Conversion deleted successfully"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Conversion not found"
}
```

**Error Response (500 Internal Server Error):**
```json
{
  "error": "Error deleting conversion: [details]"
}
```

---

### 6. Get Today's Conversions with Limit
**GET** `/api/ImageUpload/today/{userId}`

Retrieve all conversions created today for a specific user, along with their conversion limit information.

**Path Parameters:**
- `userId` (integer, required) - The ID of the user

**Success Response (200 OK):**
```json
{
  "today": [
    {
      "id": 1,
      "conversionDate": "2026-01-14T10:30:00Z",
      "image": {
        "id": 1,
        "fileName": "image.jpg",
        "size": 1024000,
        "format": "JPG",
        "downloadUrl": "/api/ImageUpload/download/1"
      }
    }
  ],
  "todayCount": 1,
  "totalCount": 5,
  "limit": 100,
  "remainingConversions": 95,
  "limitReached": false
}
```

**Response Fields:**
- `today` - Array of today's conversions
- `todayCount` - Number of conversions made today
- `totalCount` - Total number of conversions ever made by this user
- `limit` - User's plan conversion limit
- `remainingConversions` - How many conversions remaining before limit
- `limitReached` - Boolean indicating if limit has been reached

**Error Response (400 Bad Request):**
```json
{
  "error": "User not found"
}
```

**Error Response (500 Internal Server Error):**
```json
{
  "error": "Error retrieving today's conversions: [details]"
}
```

---

## Plan Management Endpoints

### 1. Get All Plans
**GET** `/api/Plans`

Retrieve all available subscription plans.

**Success Response (200 OK):**
```json
{
  "plans": [
    {
      "id": 1,
      "name": "Free",
      "limit": 10,
      "price": 0
    },
    {
      "id": 2,
      "name": "Pro",
      "limit": 100,
      "price": 999
    },
    {
      "id": 3,
      "name": "Enterprise",
      "limit": 1000,
      "price": 4999
    }
  ],
  "count": 3
}
```

**Error Response (500 Internal Server Error):**
```json
{
  "error": "Error retrieving plans: [details]"
}
```

---

### 2. Get Specific Plan
**GET** `/api/Plans/{planId}`

Retrieve details of a specific plan.

**Path Parameters:**
- `planId` (integer, required) - The ID of the plan

**Success Response (200 OK):**
```json
{
  "id": 1,
  "name": "Free",
  "limit": 10,
  "price": 0
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Plan not found"
}
```

---

### 3. Get User's Current Plan
**GET** `/api/Plans/user/{userId}`

Retrieve the current subscription plan for a user.

**Path Parameters:**
- `userId` (integer, required) - The ID of the user

**Success Response (200 OK):**
```json
{
  "id": 1,
  "name": "Free",
  "limit": 10,
  "price": 0
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "User or plan not found"
}
```

---

### 4. Change User's Plan
**POST** `/api/Plans/user/{userId}/change`

Change a user's subscription plan.

**Path Parameters:**
- `userId` (integer, required) - The ID of the user

**Request Body (application/json):**
```json
{
  "newPlanId": 2
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Plan changed successfully",
  "user": {
    "id": 1,
    "email": "user@example.com"
  },
  "newPlan": {
    "id": 2,
    "name": "Pro",
    "limit": 100,
    "price": 999
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Invalid plan ID"
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "User or plan not found"
}
```

---

### 5. Create Plan (Admin)
**POST** `/api/Plans`

Create a new subscription plan.

**Request Body (application/json):**
```json
{
  "name": "Premium",
  "limit": 500,
  "price": 2999
}
```

**Validation Rules:**
- `name`: Required, non-empty string
- `limit`: Required, must be > 0
- `price`: Required, must be >= 0

**Success Response (201 Created):**
```json
{
  "id": 4,
  "name": "Premium",
  "limit": 500,
  "price": 2999
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Plan name is required"
}
```
```json
{
  "error": "Limit must be greater than 0"
}
```
```json
{
  "error": "Price cannot be negative"
}
```

---

### 6. Update Plan (Admin)
**PUT** `/api/Plans/{planId}`

Update an existing subscription plan.

**Path Parameters:**
- `planId` (integer, required) - The ID of the plan to update

**Request Body (application/json):**
```json
{
  "name": "Pro Plus",
  "limit": 150,
  "price": 1299
}
```

**Validation Rules:**
- `name`: Required, non-empty string
- `limit`: Required, must be > 0
- `price`: Required, must be >= 0

**Success Response (200 OK):**
```json
{
  "id": 2,
  "name": "Pro Plus",
  "limit": 150,
  "price": 1299
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Plan not found"
}
```

---

### 7. Delete Plan (Admin)
**DELETE** `/api/Plans/{planId}`

Delete a subscription plan. Cannot delete if users are using this plan.

**Path Parameters:**
- `planId` (integer, required) - The ID of the plan to delete

**Success Response (200 OK):**
```json
{
  "message": "Plan deleted successfully"
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Cannot delete plan. Either plan not found or users are using this plan."
}
```

---

## Data Models

### Plan
```typescript
{
  id: number;
  name: string;
  limit: number; // Max conversions allowed
  price: number; // Price in cents (e.g., 999 = $9.99)
}
```

### Image
```typescript
{
  id: number;
  md5: string; // MD5 hash of the image
  path: string; // File path on server
  size: number; // File size in bytes
  format: string; // "JPG", "PNG", "GIF", "BMP", "WEBP"
}
```

### Conversion
```typescript
{
  id: number;
  userId: number;
  imageIdFrom: number; // Original image ID
  imageIdTo: number; // Converted WebP image ID
  datetime: string; // ISO 8601 format
}
```

---

## Error Handling

All endpoints follow consistent error response format:

```json
{
  "error": "Error message description"
}
```

### HTTP Status Codes

- **200 OK** - Request successful
- **201 Created** - Resource created successfully
- **400 Bad Request** - Invalid input or business logic error
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server-side error

---

## Notes

### Conversion Simulation
The API simulates WebP conversion rather than performing actual conversion:
- Original image is saved physically
- WebP size is estimated as 70% of original size (30% compression)
- MD5 hash is calculated for original image
- Simulated MD5 is generated for WebP image
- Both records are saved to database

### File Upload Limits
- Maximum file size: 10 MB
- Allowed formats: JPG, JPEG, PNG, GIF, BMP
- Files are stored in `/uploads` directory

### Conversion Limits
Users are limited based on their subscription plan:
- Free: 10 conversions
- Pro: 100 conversions
- Enterprise: 1000 conversions
- Custom plans can have any limit

---

## Example Usage

### JavaScript/Fetch Example

```javascript
// Upload and convert an image
const formData = new FormData();
formData.append('file', fileInput.files[0]);
formData.append('userId', 1);

const response = await fetch('https://localhost:5001/api/ImageUpload/upload', {
  method: 'POST',
  body: formData
});

const result = await response.json();
console.log(result);

// Get user conversions
const conversions = await fetch('https://localhost:5001/api/ImageUpload/conversions/1');
const data = await conversions.json();
console.log(data.conversions);

// Get all plans
const plans = await fetch('https://localhost:5001/api/Plans');
const plansData = await plans.json();
console.log(plansData.plans);

// Get today's conversions with limit
const today = await fetch('https://localhost:5001/api/ImageUpload/today/1');
const todayData = await today.json();
console.log(`Today: ${todayData.todayCount}, Remaining: ${todayData.remainingConversions}`);

// Change user plan
const planChange = await fetch('https://localhost:5001/api/Plans/user/1/change', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({ newPlanId: 2 })
});

const planResult = await planChange.json();
console.log(planResult);
```

### cURL Examples

```bash
# Upload image
curl -X POST https://localhost:5001/api/ImageUpload/upload \
  -F "file=@image.jpg" \
  -F "userId=1"

# Get user conversions
curl https://localhost:5001/api/ImageUpload/conversions/1

# Download image
curl https://localhost:5001/api/ImageUpload/download/2 -o converted.webp

# Get today's conversions with limit
curl https://localhost:5001/api/ImageUpload/today/1

# Get all plans
curl https://localhost:5001/api/Plans

# Get specific plan
curl https://localhost:5001/api/Plans/1

# Get user's current plan
curl https://localhost:5001/api/Plans/user/1

# Change user plan
curl -X POST https://localhost:5001/api/Plans/user/1/change \
  -H "Content-Type: application/json" \
  -d '{"newPlanId": 2}'

# Create plan (Admin)
curl -X POST https://localhost:5001/api/Plans \
  -H "Content-Type: application/json" \
  -d '{"name":"Premium","limit":500,"price":2999}'

# Update plan (Admin)
curl -X PUT https://localhost:5001/api/Plans/2 \
  -H "Content-Type: application/json" \
  -d '{"name":"Pro Plus","limit":150,"price":1299}'

# Delete plan (Admin)
curl -X DELETE https://localhost:5001/api/Plans/4
```

---

## CORS Configuration Details

### Allowed Origins
- `http://localhost:4200` (Angular development)
- `https://localhost:4200` (Angular development with HTTPS)

### Allowed Methods
- GET
- POST
- PUT
- DELETE
- OPTIONS (preflight requests)

### Allowed Headers
- All headers are permitted

### Credentials
- Cookies and authentication credentials are allowed

### Troubleshooting CORS Issues

If you encounter CORS errors:

1. **Ensure the frontend is running on port 4200**
   ```bash
   ng serve --port 4200
   ```

2. **Check the API is using the correct port** (44379 or 5001)

3. **Restart the API server** after CORS configuration changes

4. **Clear browser cache** if issues persist

5. **Check browser console** for specific CORS error messages

### Adding Additional Origins

To allow requests from other origins, modify `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200", 
                "https://localhost:4200",
                "https://yourdomain.com"  // Add production URL
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

---

## Quick Reference

### Image Upload Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/ImageUpload/upload` | Upload and convert image |
| GET | `/api/ImageUpload/download/{imageId}` | Download image |
| GET | `/api/ImageUpload/conversions/{userId}` | Get user conversions |
| GET | `/api/ImageUpload/conversion/{conversionId}` | Get specific conversion |
| DELETE | `/api/ImageUpload/conversion/{conversionId}` | Delete conversion |
| GET | `/api/ImageUpload/today/{userId}` | Get today's conversions with limit |

### Plan Management Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Plans` | Get all plans |
| GET | `/api/Plans/{planId}` | Get specific plan |
| GET | `/api/Plans/user/{userId}` | Get user's current plan |
| POST | `/api/Plans/user/{userId}/change` | Change user's plan |
| POST | `/api/Plans` | Create plan (Admin) |
| PUT | `/api/Plans/{planId}` | Update plan (Admin) |
| DELETE | `/api/Plans/{planId}` | Delete plan (Admin) |

---

## Architecture

```
Frontend App
    ↓
API Controllers (API Layer)
    ↓
Service Layer (SL) - Business Logic
    ↓
Data Access Layer (DAL) - Repositories
    ↓
CodeFirst - Models & DbContext
    ↓
Database (SQL Server)
```

---

**Last Updated:** January 14, 2026

